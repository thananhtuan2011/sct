import { CommonService } from 'src/app/_metronic/shared/services/common.service';
import { PageInfoService, PageLink } from '../../../../_metronic/layout/core/page-info.service';
import { PaginatorState } from '../../../../_metronic/shared/crud-table/models/paginator.model';
import { ISearchView } from '../../../../_metronic/shared/crud-table/models/search.model';
import { IFilterView } from '../../../../_metronic/shared/crud-table/models/filter.model';
import { ISortView, SortState } from '../../../../_metronic/shared/crud-table/models/sort.model';
import { GroupingState, IGroupingView } from '../../../../_metronic/shared/crud-table/models/grouping.model';
import { ChangeDetectorRef, Component, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { catchError, finalize, Observable, of, Subscription, tap, filter } from 'rxjs';
import { StatiscialPetroleumPageService } from '../_services/statistical-petroleum-page.services';
import { Options } from 'select2';
import { AuthService } from 'src/app/modules/auth/services/auth.service';

import { environment } from 'src/environments/environment';
import { HttpClient } from '@angular/common/http';
import Swal from 'sweetalert2';
import { ActivatedRoute } from '@angular/router';
import { InfoPetroleumBusinessDetailModalComponent } from '../../petroleum-business-page/petroleum-business-page/components/info-detail-modal/info-detail-modal.component';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';


@Component({
  selector: 'app-table-page',
  templateUrl: './table-page.component.html',
  styleUrls: ['./table-page.component.scss'],
})

export class TableStatisticalPetroleumPageComponent implements OnInit,
  OnDestroy,
  // IFetchSelectedAction,
  // IUpdateStatusForSelectedAction,
  ISortView,
  IFilterView,
  IGroupingView,
  ISearchView,
  IFilterView {
  paginator: PaginatorState;
  sorting: SortState;
  grouping: GroupingState;
  isLoading: boolean;
  filterGroup: FormGroup;
  searchGroup: FormGroup;
  districtData: any = [];
  districtId: string = "00000000-0000-0000-0000-000000000000";
  businessId: string = "00000000-0000-0000-0000-000000000000";
  nguoiDaiDien: string = "";
  data: any;
  total: any;
  options: Options;
  filterBody: any = { "filter": {} };
  userLv: number = 0;
  userArea: string = "00000000-0000-0000-0000-000000000000"

  isNext: boolean = false;
  
  businessData : any = [];
  dataSearch: any ;

  private subscriptions: Subscription[] = []; // Read more: => https://brianflove.com/2016/12/11/anguar-2-unsubscribe-observables/

  public parameterValue: string;

  constructor(
    private fb: FormBuilder,
    public statiscialMarketPageService: StatiscialPetroleumPageService,
    public commonService: CommonService,
    private changeDetectorRef: ChangeDetectorRef,
    private authService: AuthService,
    private http: HttpClient,
    private route: ActivatedRoute,
    private modalService: NgbModal
  ) {
    this.route.queryParams.subscribe(res => {
      if(res['id']) {
        this.isNext = true;
        this.districtId = res['id'];
      }
    }) 
  }

  ngOnInit(): void {
    // this.filterForm();
    this.searchForm();
    this.loadDistrict();
    this.filterForm();
    this.loadBusiness();
    this.options = {
      theme: 'bootstrap5',
      templateSelection: this.templateSelection,
    };
  }

  public templateSelection = (state: any): JQuery | string => {
    if (!state.id) {
      return state.text;
    }
    return jQuery('<span class="form-select form-select-solid form-select-lg">' + state.text + '</span>');
  }

  ngOnDestroy() {
    this.subscriptions.forEach((sb) => sb.unsubscribe());
    this.statiscialMarketPageService.setDefaults();
  }

  loadByNextPage() {
    if (this.isNext == true) {
      this.loadData({ "filter": {"DistrictId": this.districtId} });
      this.filterGroup.controls.district.setValue(this.districtId);
    } else {
      this.loadByUser();
    }
  }

  loadByUser() {
    const data = this.authService.getDataUser();
    this.userLv = data.levelUser;
    this.userArea = data.areaID

    if(this.userLv == 1){
      this.loadData({ "filter": {"DistrictId": this.userArea} })
      this.filterGroup.controls.district.setValue(this.userArea);
      this.filterGroup.controls.district.disable();
    } else {
      this.loadData({ "filter": {} });
    }
  }

  loadData(filter: any) {
    this.statiscialMarketPageService.loadData(filter).subscribe((res: any) => {
      const data = res.map((item: any) =>({
        ...item, 
        isExpand: false,
        isExpandXangDau: false,
        isExpandThuocLa: false,
        isExpandRuou: false
      }));
      this.data = data;
      this.changeDetectorRef.detectChanges();
    })
  }

  loadDistrict() {
    this.commonService.getDistrict().subscribe((res: any) => {
      const data = [
        {
          id: "00000000-0000-0000-0000-000000000000",
          text: '-- Chọn --'
        }
      ]
      for (var item of res.items) {
        let obj = {
          id: item.districtId,
          text: item.districtName,
        }
        data.push(obj)
      }
      this.districtData = data;
    })
  }
  
  loadBusiness(){
    this.commonService.getBusiness().subscribe((res: any) => {
      const data = [
        {
          id: "00000000-0000-0000-0000-000000000000",
          text: '-- Chọn --'
        }
      ]
      for (var item of res.items) {
        let obj = {
          id: item.businessId,
          text: item.businessNameVi,
        }
        data.push(obj)
      }
      this.businessData = data;
    })
  }

  // filtration
  filterForm() {
    this.filterGroup = this.fb.group({
      district: [this.districtId],
      business: [this.businessId],
      nguoiDaiDien: [this.nguoiDaiDien]
    });
    this.filterGroup.controls.district.valueChanges.subscribe((x: any) => {
      this.filter()
    })
    this.filterGroup.controls.business.valueChanges.subscribe((x: any) => {
      this.filter()
    })
    this.filterGroup.controls.nguoiDaiDien.valueChanges.subscribe(()=>{
      this.filter();
    })
    this.loadByNextPage();
  }

  filter() {
    const filter: { [key: string]: string } = {};
    const district = this.filterGroup.controls['district'].value;
    const business = this.filterGroup.controls['business'].value;
    const nguoiDaiDien = this.filterGroup.controls['nguoiDaiDien'].value;
    if (district && district != "00000000-0000-0000-0000-000000000000") {
      filter['DistrictId'] = district;
    };
    if (business && business != "00000000-0000-0000-0000-000000000000") {
      filter['BusinessId'] = business;
    };
    if(nguoiDaiDien && nguoiDaiDien != ""){
      filter['NguoiDaiDien'] = nguoiDaiDien;
    }
    this.filterBody = { "filter": filter };
    // this.loadData(this.filterBody);
  }

  applyFilter() {
    this.loadData(this.filterBody);
  }

  clearFilter() {
    if (this.userLv == 0) {
      this.filterGroup.controls.district.setValue("00000000-0000-0000-0000-000000000000");
      this.filterGroup.controls.business.setValue("00000000-0000-0000-0000-000000000000");
      this.filterGroup.controls.nguoiDaiDien.setValue("");
      
      this.filterGroup.updateValueAndValidity();
      this.loadData({ "filter": {} })
    } else {
      this.filterGroup.controls.district.setValue("00000000-0000-0000-0000-000000000000");
      this.filterGroup.controls.business.setValue("00000000-0000-0000-0000-000000000000");
      this.filterGroup.controls.nguoiDaiDien.setValue("");
      this.filterGroup.updateValueAndValidity();
      this.loadData({ "filter": {"DistrictId": this.userArea} })
    }

  }

  // search
  searchForm() {
    this.searchGroup = this.fb.group({
      searchTerm: [''],
    });
    this.dataSearch = this.searchGroup.controls.searchTerm.value
    const searchEvent = this.searchGroup.controls.searchTerm.valueChanges
      .subscribe((val) => {if(val == ''){this.onEnter()}});
    this.subscriptions.push(searchEvent);
  }

  search(searchTerm: string) {
    if (searchTerm.length == 0)
      this.statiscialMarketPageService.patchState({ searchTerm });
  }

  onEnter() {
    this.dataSearch = this.searchGroup.controls.searchTerm.value
  }

  searchData(searchTerm: string) {
    this.statiscialMarketPageService.patchState({ searchTerm });
  }

  // sorting
  sort(column: string) {
    const sorting = this.sorting;
    const isActiveColumn = sorting.column === column;
    if (!isActiveColumn) {
      sorting.column = column;
      sorting.direction = 'asc';
    } else {
      sorting.direction = sorting.direction === 'asc' ? 'desc' : 'asc';
    }
    this.statiscialMarketPageService.patchState({ sorting });
  }

  // pagination
  paginate(paginator: PaginatorState) {
    this.statiscialMarketPageService.patchState({ paginator });
  }

  getHeight(): any {
    let tmp_height = 0;
    tmp_height = window.innerHeight - 300;
    return tmp_height + 'px';
  }
  
  changeValue(event: any){
    if(event == ''){
      //this.searchData(this.searchGroup.controls.searchTerm.value)
      this.onEnter()
    }
  }
  
  view(id: any){
    const sb = this.statiscialMarketPageService.loadPetroStoreDetail(id).subscribe((res: any) => {
      const modalRef = this.modalService.open(InfoPetroleumBusinessDetailModalComponent, { size: 'xl' });
      modalRef.componentInstance.props = res.items[0];
      modalRef.result.then(() =>
        {}
      );
    })
    this.subscriptions.push(sb)
  }
}


