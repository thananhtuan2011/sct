import { CommonService } from 'src/app/_metronic/shared/services/common.service';
import { PageInfoService, PageLink } from '../../../../_metronic/layout/core/page-info.service';
import { PaginatorState } from '../../../../_metronic/shared/crud-table/models/paginator.model';
import { ISearchView } from '../../../../_metronic/shared/crud-table/models/search.model';
import { IFilterView } from '../../../../_metronic/shared/crud-table/models/filter.model';
import { ISortView, SortState } from '../../../../_metronic/shared/crud-table/models/sort.model';
import { GroupingState, IGroupingView } from '../../../../_metronic/shared/crud-table/models/grouping.model';
import { IFetchSelectedAction, IUpdateStatusForSelectedAction } from '../../../../_metronic/shared/crud-table/models/table.model';
import { ChangeDetectorRef, Component, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { catchError, finalize, Observable, of, Subscription, tap, filter } from 'rxjs';
import { StatiscialMarketPageService } from '../_services/statistical-market-page.services';
import { Options } from 'select2';
import { AuthService } from 'src/app/modules/auth/services/auth.service';

import { environment } from 'src/environments/environment';
import { HttpClient } from '@angular/common/http';
import Swal from 'sweetalert2';
import { ActivatedRoute } from '@angular/router';


@Component({
  selector: 'app-table-page',
  templateUrl: './table-page.component.html',
  styleUrls: ['./table-page.component.scss'],
})

export class TableStatisticalMarketPageComponent implements OnInit,
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
  communeData: any = [];
  commercialData: any = [];
  businessLineData: any = [];
  communeDataByDistrict: any = [];
  commercialDataByDistrict: any = [];
  districtId: string = "00000000-0000-0000-0000-000000000000";
  communeId: string = "00000000-0000-0000-0000-000000000000";
//  typeId: string = "00000000-0000-0000-0000-000000000000";
  businessLineId : string = "00000000-0000-0000-0000-000000000000";
  marketId : string = "00000000-0000-0000-0000-000000000000";
  data: any;
  total: any;
  options: Options;
  filterBody: any = { "filter": {} };
  userLv: number = 0;
  userArea: string = "00000000-0000-0000-0000-000000000000"

  isNext: boolean = false;
  dataSearch: any 

  private subscriptions: Subscription[] = []; // Read more: => https://brianflove.com/2016/12/11/anguar-2-unsubscribe-observables/

  public parameterValue: string;

  constructor(
    private fb: FormBuilder,
    public statiscialMarketPageService: StatiscialMarketPageService,
    public commonService: CommonService,
    private changeDetectorRef: ChangeDetectorRef,
    private authService: AuthService,
    private http: HttpClient,
    private route: ActivatedRoute,
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
    this.loadCommune();
    this.loadBusinessLine();
    this.loadCommercial();
    this.filterForm();
    // this.statiscialMarketPageService.fetch();
    // this.grouping = this.statiscialMarketPageService.grouping;
    // this.paginator = this.statiscialMarketPageService.paginator;
    // this.sorting = this.statiscialMarketPageService.sorting;
    // const sb = this.statiscialMarketPageService.isLoading$.subscribe((res: any) => this.isLoading = res);
    // this.subscriptions.push(sb);
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
      this.communeDataByDistrict = this.communeData.filter((x: any) => x.district == this.districtId || x.district == "00000000-0000-0000-0000-000000000000")
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
      this.communeDataByDistrict = this.communeData.filter((x: any) => x.district == this.userArea || x.district == "00000000-0000-0000-0000-000000000000")
    } else {
      this.loadData({ "filter": {} });
    }
  }

  loadData(filter: any) {
    this.statiscialMarketPageService.loadData(filter).subscribe((res: any) => {
      this.data = res;
     // this.total = res.total;
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

  loadCommune() {
    this.commonService.getCommune().subscribe((res: any) => {
      const data = [
        {
          id: "00000000-0000-0000-0000-000000000000",
          text: '-- Chọn --',
          district: "00000000-0000-0000-0000-000000000000",
        }
      ]
      for (var item of res.items) {
        let obj = {
          id: item.communeId,
          text: item.communeName,
          district: item.districtId
        }
        data.push(obj)
      }
      this.communeData = data;
      this.communeDataByDistrict = data;
    })
  }
  
  loadCommercial(){
    this.commonService.getCommercial().subscribe((res: any) => {
      const data = [
        {
          id: "00000000-0000-0000-0000-000000000000",
          text: '-- Chọn --',
          district: "00000000-0000-0000-0000-000000000000",
          commune: "00000000-0000-0000-0000-000000000000"
        }
      ]
      for (var item of res.items) {
        let obj = {
          id: item.commercialId,
          text: item.name,
          district: item.districtId,
          commune: item.communeId
        }
        data.push(obj)
      }
      this.commercialData = data;
      this.commercialDataByDistrict = data;
    })
  }

  loadBusinessLine() {
    this.commonService.getBusinessLine().subscribe((res: any) => {
      const data = [
        {
          id: "00000000-0000-0000-0000-000000000000",
          text: '-- Chọn --',
        }
      ]
      for (var item of res.items) {
        let obj = {
          id: item.businessLineId,
          text: item.businessLineName,
        }
        data.push(obj)
      }
      this.businessLineData = data;
    })
  }
  // filtration
  filterForm() {
    this.filterGroup = this.fb.group({
      district: [this.districtId],
      commune: [this.communeId],
      //type: [this.typeId],
      businessLine: [this.businessLineId],
      market: [this.marketId],
    });
    this.filterGroup.controls.district.valueChanges.subscribe((x: any) => {
      if(x != "00000000-0000-0000-0000-000000000000") {
        this.communeDataByDistrict = this.communeData.filter((i: any) => i.district == x || i.district == "00000000-0000-0000-0000-000000000000")
        this.commercialDataByDistrict = this.commercialData.filter((item: any) => item.district == x || item.district == "00000000-0000-0000-0000-000000000000") 
      } else {
        this.communeDataByDistrict = this.communeData
        this.commercialDataByDistrict = this.commercialData
      }
      this.filter()
    })
    this.filterGroup.controls.commune.valueChanges.subscribe((x: any) => {
      if(x != "00000000-0000-0000-0000-000000000000") {
        this.commercialDataByDistrict = this.commercialData.filter((item: any) => item.commune == x || item.commune == "00000000-0000-0000-0000-000000000000") 
      } else {
        this.commercialDataByDistrict = this.commercialData
      }
      this.filter()
    })
    this.filterGroup.controls.market.valueChanges.subscribe(() => 
      this.filter()
    )
    this.filterGroup.controls.businessLine.valueChanges.subscribe(() => 
    this.filter()
  )
    this.loadByNextPage();
  }

  filter() {
    const filter: { [key: string]: string } = {};
    const district = this.filterGroup.controls['district'].value;
    const commune = this.filterGroup.controls['commune'].value;
    const market = this.filterGroup.controls['market'].value;
    const businessLine = this.filterGroup.controls['businessLine'].value;
    if (district && district != "00000000-0000-0000-0000-000000000000") {
      filter['DistrictId'] = district;
    };
    if (commune && commune != "00000000-0000-0000-0000-000000000000") {
      filter['CommuneId'] = commune;
    };
    if (market && market != "00000000-0000-0000-0000-000000000000") {
      filter['MarketId'] = market;
    };
    if(businessLine && businessLine != "00000000-0000-0000-0000-000000000000"){
      filter['BusinessLineId'] = businessLine;
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
      this.filterGroup.controls.commune.setValue("00000000-0000-0000-0000-000000000000");
      this.filterGroup.controls.market.setValue("00000000-0000-0000-0000-000000000000");
      this.filterGroup.controls.businessLine.setValue("00000000-0000-0000-0000-000000000000");
      this.communeDataByDistrict = this.communeData;
      this.filterGroup.updateValueAndValidity();
      this.loadData({ "filter": {} })
    } else {
      this.filterGroup.controls.commune.setValue("00000000-0000-0000-0000-000000000000");
      this.filterGroup.controls.type.setValue("00000000-0000-0000-0000-000000000000");
      this.filterGroup.updateValueAndValidity();
      this.loadData({ "filter": {"DistrictId": this.userArea} })
    }

  }

  // search
  searchForm() {
    this.searchGroup = this.fb.group({
      searchTerm: [''],
    });
    const searchEvent = this.searchGroup.controls.searchTerm.valueChanges
      .subscribe((val) => val == '' ? this.onEnter() : '');
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
  
  f_currency(value: any, args?: any): any {
    let nbr = Number((value + '').replace(/,|-/g, ""));
    const result = (nbr + '').replace(/(\d)(?=(\d{3})+(?!\d))/g, "$1,");
    return result
  }

  exportFile() {
    const moment = require("moment");
    const timeString = moment().format("DDMMYYYYHHmmss");
    const fileName = "Thongkethongtinchosieuthitrungtamthuongmaihuyen_" + timeString + ".xlsx"

    Swal.fire({
            icon: 'info',
      title: 'Đang xuất File...',
      // text: 'Vui lòng đợi một lúc trước khi file của bạn sẵn sàng!',
      didOpen: () => {
        Swal.showLoading()
      },
    })

    this.http.post(`${environment.apiUrl}/Statistical/ExportStatisticalMarket`, this.filterBody,
    {
      responseType: 'blob',
    }).pipe(
      catchError((errorMessage: any) => {
        console.error(errorMessage)
        Swal.fire({
          icon: 'error',
          title: 'Xuất File thất bại',
          confirmButtonText: 'Xác nhận',
        });
        return of();
      }),
    ).subscribe(
      (res) => {
        const file = new Blob([res], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' });
        const fileURL = URL.createObjectURL(file);
        const a = document.createElement('a');
        a.href = fileURL;
        a.download = fileName;
        document.body.append(a);
        a.click();
        a.remove();
        URL.revokeObjectURL(fileURL);
        Swal.fire({
          icon: 'success',
          title: 'Xuất File thành công',
          confirmButtonText: 'Xác nhận',
        });
      },
    );
  }
}


