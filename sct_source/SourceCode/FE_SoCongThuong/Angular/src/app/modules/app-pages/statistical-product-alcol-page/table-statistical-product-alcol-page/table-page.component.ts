import { PageInfoService, PageLink } from '../../../../_metronic/layout/core/page-info.service';
import { PaginatorState } from '../../../../_metronic/shared/crud-table/models/paginator.model';
import { ISearchView } from '../../../../_metronic/shared/crud-table/models/search.model';
import { IFilterView } from '../../../../_metronic/shared/crud-table/models/filter.model';
import { ISortView, SortState } from '../../../../_metronic/shared/crud-table/models/sort.model';
import { GroupingState, IGroupingView } from '../../../../_metronic/shared/crud-table/models/grouping.model';
import { ChangeDetectorRef, Component, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { catchError, of, Subscription } from 'rxjs';
import { StatisticalProductAlcolPageService } from '../_services/statistical-product-alcol-page.service';
import { environment } from 'src/environments/environment';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';

import Swal from 'sweetalert2';
import { Options } from 'select2';
import { CommonService } from 'src/app/_metronic/shared/services/common.service';


@Component({
  selector: 'app-table-staticial-product-alcol-page',
  templateUrl: './table-page.component.html',
  styleUrls: ['./table-page.component.scss'],
})

export class TableStatisticalProductAlcolPageComponent implements OnInit,
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
  data: any = [];
  dataProductAlcol :any = [];
  dataProductAlcolFirst: any = [];
  dataDistrictProductAlcol : any = [];

  dataIndusAlcol :any = [];
  dataDistrictIndusAlcol : any = [];
  dataDistrictIndusAlcolFirst: any = []
  districtName : string = '';
  districtNameInstruct : string = '';
  districtId : string = '00000000-0000-0000-0000-000000000000';
  businessData: any = []
  businessId: string = "00000000-0000-0000-0000-000000000000"
  options: Options;

  private subscriptions: Subscription[] = []; // Read more: => https://brianflove.com/2016/12/11/anguar-2-unsubscribe-observables/

  public parameterValue: string;
  dataSearch: any
  dataSearchIndustry: any

  constructor(
    private fb: FormBuilder,
    private statisticalProductAlcolPageService: StatisticalProductAlcolPageService,
    private http: HttpClient,
    private router: Router,
    private changeDetectorRef: ChangeDetectorRef,
    private commonService: CommonService
  ) { }

  ngOnInit(): void {
     this.filterForm();
    this.searchForm();
    // this.loadData();
    this.loadBusiness();
    this.loadDataProductAlcol();
    this.loadDataIndusAlcol();
    this.options = {
      theme: 'bootstrap5',
      templateSelection: this.templateSelection,
    };
    this.changeDetectorRef.detectChanges();
    // this.statisticalBuildUpgradePageService.fetch();
    // this.grouping = this.statisticalBuildUpgradePageService.grouping;
    // this.paginator = this.statisticalBuildUpgradePageService.paginator;
    // this.sorting = this.statisticalBuildUpgradePageService.sorting;
    // const sb = this.statisticalBuildUpgradePageService.isLoading$.subscribe((res: any) => this.isLoading = res);
    // this.subscriptions.push(sb);
  }
  
  public templateSelection = (state: any): JQuery | string => {
    if (!state.id) {
      return state.text;
    }
    return jQuery('<span class="form-select form-select-solid form-select-lg">' + state.text + '</span>');
  }

  ngOnDestroy() {
    this.subscriptions.forEach((sb) => sb.unsubscribe());
    this.statisticalProductAlcolPageService.setDefaults();
  }
  
  filterForm() {
    this.filterGroup = this.fb.group({
      business: [this.businessId],
    });
    this.filterGroup.controls.business.valueChanges.subscribe(() => {
      this.filter()
    })
  }
  
  filter() {
    this.businessId = this.filterGroup.controls['business'].value;
  }

  // loadData() {
  //   this.statisticalProductAlcolPageService.loadData().subscribe((res: any) => {
  //     this.data = res;
  //   })
  // }
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
  loadDataProductAlcol(){
    this.statisticalProductAlcolPageService.loadDataProductAlcol().subscribe((res: any) => {
      this.dataProductAlcol = res;
      this.districtId = res.details[0].districtId;
      this.districtName = res.details[0].districtName;
      this.loadDataDistrictProductAlcol(this.districtId);
      this.changeDetectorRef.detectChanges();
    })
  }
  loadDataIndusAlcol(){
    this.statisticalProductAlcolPageService.loadDataIndusAlcol().subscribe((res: any) => {
      this.dataIndusAlcol = res;
      this.districtId = res.details[0].districtId;
      this.districtNameInstruct = res.details[0].districtName;
      this.loadDataDistrictindusAlcol(this.districtId)
      this.changeDetectorRef.detectChanges();
    })
  }
  loadDataDistrictProductAlcol(id: any){
    this.statisticalProductAlcolPageService.loadDataDistrictProductAlcol(id).subscribe((res: any) => {
      this.dataDistrictProductAlcol = res;
      this.dataProductAlcolFirst = res;
      this.changeDetectorRef.detectChanges();
    })
  }

  loadDataDistrictindusAlcol(id: any){
    this.statisticalProductAlcolPageService.loadDataDistrictIndusAlcol(id).subscribe((res: any) => {
      this.dataDistrictIndusAlcol = res;
      this.dataDistrictIndusAlcolFirst = res;
      this.changeDetectorRef.detectChanges();
    })
  }
  // filtration

  // search
  searchForm() {
    this.searchGroup = this.fb.group({
      searchTerm: [''],
      searchTermIndustry: ['']
    });
    const searchEvent = this.searchGroup.controls.searchTerm.valueChanges
      .subscribe((val) => val == '' ? this.onEnter() : '');
    const search = this.searchGroup.controls.searchTermIndustry.valueChanges
      .subscribe((val) => val == '' ? this.onEnterIndustry() : '');
    this.subscriptions.push(searchEvent, search);
  }

  search(searchTerm: string) {
    // this.statisticalBuildUpgradePageService.patchState({ searchTerm });
  }

  onEnter() {
    this.dataSearch = this.searchGroup.controls.searchTerm.value

  }
  
  onEnterIndustry(){
    this.dataSearchIndustry = this.searchGroup.controls.searchTermIndustry.value
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
    this.statisticalProductAlcolPageService.patchState({ sorting });
  }

  // pagination
  paginate(paginator: PaginatorState) {
    this.statisticalProductAlcolPageService.patchState({ paginator });
  }

  getTotal(column_name: any) {
    return this.data.reduce((sum: any, item: any) => sum + item[column_name], 0);
  }


  getHeight(): any {
		let tmp_height = 0;
		tmp_height = window.innerHeight - 300;
		return tmp_height + 'px';
	}

  getDataProduct(id: string, name: string){
    this.loadDataDistrictProductAlcol(id);
    this.districtName = name;
    this.districtId = id;
  }
  getDataIndus(id: string, name: string){
    this.loadDataDistrictindusAlcol(id);
    this.districtNameInstruct = name;
    this.districtId = id;
  }
  
  applyFilter(){
    if(this.businessId != '00000000-0000-0000-0000-000000000000')
    {
      this.dataDistrictProductAlcol = this.dataProductAlcolFirst.filter((x: any) => x.businessId == this.businessId)
      this.dataDistrictIndusAlcol = this.dataDistrictIndusAlcolFirst.filter((x: any) => x.businessId == this.businessId)
      this.changeDetectorRef.detectChanges()
    }else{
      this.dataDistrictIndusAlcol = this.dataDistrictIndusAlcolFirst
      this.dataDistrictProductAlcol = this.dataProductAlcolFirst
      this.changeDetectorRef.detectChanges()
    }
  }
  
  clearFilter(){
    this.filterGroup.controls.business.setValue("00000000-0000-0000-0000-000000000000");
    this.filterGroup.updateValueAndValidity();
    this.dataDistrictIndusAlcol = this.dataDistrictIndusAlcolFirst
    this.dataDistrictProductAlcol = this.dataProductAlcolFirst
    this.changeDetectorRef.detectChanges()
  }
  
  f_currency(value: any, args?: any): any {
    let nbr = Number((value + '').replace(/,|-/g, ""));
    const result = (nbr + '').replace(/(\d)(?=(\d{3})+(?!\d))/g, "$1,");
    return result
  }
}


