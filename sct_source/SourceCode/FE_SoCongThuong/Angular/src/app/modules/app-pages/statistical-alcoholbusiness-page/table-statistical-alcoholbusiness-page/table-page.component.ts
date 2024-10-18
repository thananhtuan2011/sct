import { PageInfoService, PageLink } from '../../../../_metronic/layout/core/page-info.service';
import { PaginatorState } from '../../../../_metronic/shared/crud-table/models/paginator.model';
import { ISearchView } from '../../../../_metronic/shared/crud-table/models/search.model';
import { IFilterView } from '../../../../_metronic/shared/crud-table/models/filter.model';
import { ISortView, SortState } from '../../../../_metronic/shared/crud-table/models/sort.model';
import { GroupingState, IGroupingView } from '../../../../_metronic/shared/crud-table/models/grouping.model';
import { ChangeDetectorRef, Component, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { catchError, of, Subscription } from 'rxjs';
import { StatisticalAlcoholBusinessPageService } from '../_services/statistical-alcoholbusiness-page.service';
import { environment } from 'src/environments/environment';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';

import Swal from 'sweetalert2';
import { Options } from 'select2';
import { CommonService } from 'src/app/_metronic/shared/services/common.service';
import * as moment from 'moment';


@Component({
  selector: 'app-table-page',
  templateUrl: './table-page.component.html',
  styleUrls: ['./table-page.component.scss'],
})

export class TableStatisticalAlcoholBusinessPageComponent implements OnInit,
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
  filterGroup2: FormGroup;
  searchGroup: FormGroup;
  data: any = [];
  dataNotBuildUpgrade :any = [];
  dataDistrictNotBuildUpgrade : any = [];
  districtName : string = '';
 // districtId : string = '';
  
  dataCraftAlcoholBusiness: any = [];
  dataIndustrialAlcoholBusiness: any = [];
  districtData: any = [];
  businessId: string = "00000000-0000-0000-0000-000000000000"
  districtId: string = "00000000-0000-0000-0000-000000000000"
  yearId: any = moment().year();
  businessId2: string = "00000000-0000-0000-0000-000000000000"
  districtId2: string = "00000000-0000-0000-0000-000000000000"
  yearId2: any = moment().year();
  businessData: any = [];
  yearData: any = [];
  options: Options;
  districtFirst = ''
  districtName2 = ''
  dataSearch: any
  dataSearchIndustry : any
  private subscriptions: Subscription[] = []; // Read more: => https://brianflove.com/2016/12/11/anguar-2-unsubscribe-observables/

  public parameterValue: string;
  filterBody: any = { "filter": {} };;
  filterBody2: any = { "filter": {} };;
  

  constructor(
    private fb: FormBuilder,
    private statisticalAlcoholBusinessPageService: StatisticalAlcoholBusinessPageService,
    private http: HttpClient,
    private router: Router,
    private changeDetectorRef: ChangeDetectorRef,
    private commonService: CommonService
  ) { }

  ngOnInit(): void {
    this.loadDistrict();
    this.loadYear();
    this.loadBusiness();
    this.filterForm();
    this.searchForm();
    this.options = {
      theme: 'bootstrap5',
      templateSelection: this.templateSelection,
    };
  }
  loadYear(){
    const data : any = [];
    for(let i = 0; i < 30; i++){
      let obj = {
        id: moment().year()- 15 + i,
        text: (moment().year()- 15 + i).toString()
      }
      data.push(obj);
    }
    this.yearData = data;
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
  
  public templateSelection = (state: any): JQuery | string => {
    if (!state.id) {
      return state.text;
    }
    return jQuery('<span class="form-select form-select-solid form-select-lg">' + state.text + '</span>');
  }

  ngOnDestroy() {
    this.subscriptions.forEach((sb) => sb.unsubscribe());
    this.statisticalAlcoholBusinessPageService.setDefaults();
  }

  loadDataCraftAlcoholBusiness(filter: any){
    this.statisticalAlcoholBusinessPageService.loadDataCraftAlcoholBusiness(filter).subscribe((res: any) => {
      this.dataCraftAlcoholBusiness = res;
      if(this.districtId == "00000000-0000-0000-0000-000000000000"){
        this.districtName = res.dataCountBusiness[0].districtName;
        this.districtFirst = res.dataCountBusiness[0].districtName;
      }
      this.changeDetectorRef.detectChanges();
    })
  }
  
  loadDataIndustrialAlcoholBusiness(filter: any){
    this.statisticalAlcoholBusinessPageService.loadDataIndustrialAlcoholBusiness(filter).subscribe((res: any) => {
      this.dataIndustrialAlcoholBusiness = res;
      if(this.districtId2 == "00000000-0000-0000-0000-000000000000"){
        this.districtName2 = res.dataCountBusiness[0].districtName;
        this.districtFirst = res.dataCountBusiness[0].districtName;
      }
      this.changeDetectorRef.detectChanges();
    })
  }
  
  // filtration
  filterForm() {
    this.filterGroup = this.fb.group({
      district: [this.districtId],
      business: [this.businessId],
      year: [this.yearId]
    });
    this.filterGroup.controls.district.valueChanges.subscribe(() => {
      this.filter(this.districtId)
    })
    this.filterGroup.controls.business.valueChanges.subscribe(() => {
      this.filter(this.districtId)
    })
    this.filterGroup.controls.year.valueChanges.subscribe(()=>{
      this.filter(this.districtId);
    })
    
    this.filterGroup2 =this.fb.group({
      district2: [this.districtId2],
      business2: [this.businessId2],
      year2: [this.yearId2]
    });
    
    this.filterGroup2.controls.district2.valueChanges.subscribe(() => {
      this.filter(this.districtId2)
    })
    this.filterGroup2.controls.business2.valueChanges.subscribe(() => {
      this.filter(this.districtId2)
    })
    this.filterGroup2.controls.year2.valueChanges.subscribe(()=>{
      this.filter(this.districtId2);
    })
    
    this.loadByNextPage();
  }
    loadByNextPage(){
      this.loadDataCraftAlcoholBusiness({  "filter": {"Year": "2022" } });
      this.loadDataIndustrialAlcoholBusiness({  "filter": {"Year": "2022" } });
    }
  
  filter(districtId? : any) {
    const filter: { [key: string]: string } = {};
    const district = districtId;
    const business = this.filterGroup.controls['business'].value;
    const year = this.filterGroup.controls['year'].value;
    
    const district2 = this.filterGroup2.controls['district2'].value;
    const business2 = this.filterGroup2.controls['business2'].value;
    const year2 = this.filterGroup2.controls['year2'].value;
    const filter2: { [key: string]: string } = {};
    
    if (district && district != "00000000-0000-0000-0000-000000000000") {
      filter['DistrictId'] = district;
      let _districtName = this.districtData.filter((x:any) => x.id == district)
      this.districtName = _districtName[0].text;
    };
    if(district == '00000000-0000-0000-0000-000000000000')
    {
      this.districtName = this.districtFirst
      this.districtName2 = this.districtFirst
    }
    if (business && business != "00000000-0000-0000-0000-000000000000") {
      filter['BusinessId'] = business;
    };
    if(year){
       filter['Year'] = year.toString();
       this.yearId = year.toString();
    }
    
    if (district && district != "00000000-0000-0000-0000-000000000000") {
      filter2['DistrictId'] = district;
      let _districtName = this.districtData.filter((x:any) => x.id == district)
      this.districtName2 = _districtName[0].text;
    };
    if (business2 && business2 != "00000000-0000-0000-0000-000000000000") {
      filter2['BusinessId'] = business2;
    };
    if(year2){
       filter2['Year'] = year2.toString();
       this.yearId2 = year2.toString();
    }
    this.filterBody = { "filter": filter };
    this.filterBody2 = { "filter": filter2 };
  }

 
  // search
  searchForm() {
    this.searchGroup = this.fb.group({
      searchTerm: [''],
      searchTermIndustry: ['']
    });
    this.dataSearch = this.searchGroup.controls.searchTerm.value;
    this.dataSearchIndustry = this.searchGroup.controls.searchTermIndustry.value;
    
    const searchEvent = this.searchGroup.controls.searchTerm.valueChanges
      .subscribe((val) => val == '' ? this.onEnter(): '');
    const search = this.searchGroup.controls.searchTermIndustry.valueChanges
      .subscribe((val) => val == '' ? this.onEnterIndustry(): '');
    this.subscriptions.push(searchEvent, search);
  }

  search(searchTerm: string) {
    // this.statisticalAlcoholBusinessPageService.patchState({ searchTerm });
  }

  onEnter() {
    this.dataSearch = this.searchGroup.controls.searchTerm.value;
  }
  
  onEnterIndustry(){
    this.dataSearchIndustry = this.searchGroup.controls.searchTermIndustry.value;
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
    this.statisticalAlcoholBusinessPageService.patchState({ sorting });
  }

  // pagination
  paginate(paginator: PaginatorState) {
    this.statisticalAlcoholBusinessPageService.patchState({ paginator });
  }

  getTotal(column_name: any) {
    return this.data.reduce((sum: any, item: any) => sum + item[column_name], 0);
  }


  getHeight(): any {
		let tmp_height = 0;
		tmp_height = window.innerHeight - 300;
		return tmp_height + 'px';
	}
  
  applyFilter() {
    this.loadDataCraftAlcoholBusiness(this.filterBody);
    this.loadDataIndustrialAlcoholBusiness(this.filterBody2);
  }

  clearFilter() {
      this.filterGroup.controls.district.setValue(this.districtId);
      this.filterGroup.controls.business.setValue("00000000-0000-0000-0000-000000000000");
      this.filterGroup.controls.year.setValue(moment().year() - 1);
      
      this.filterGroup2.controls.district2.setValue("00000000-0000-0000-0000-000000000000");
      this.filterGroup2.controls.business2.setValue("00000000-0000-0000-0000-000000000000");
      this.filterGroup2.controls.year2.setValue(moment().year() - 1);
      
      this.filterGroup.updateValueAndValidity();
      this.loadDataCraftAlcoholBusiness({ "filter": {"Year": "2022", "DistrictId": this.districtId  } })
      this.loadDataIndustrialAlcoholBusiness({ "filter": {"Year": "2022", "DistrictId": this.districtId2  } })


  }
  
  changeValueDistrict(districtId: any){
    this.districtId = districtId
    this.filter(districtId);
    this.loadDataCraftAlcoholBusiness(this.filterBody);
    
  }
  changeValueDistrict2(districtId: any){
    this.districtId2 = districtId
    this.filter(districtId);
    this.loadDataIndustrialAlcoholBusiness(this.filterBody2);
  }
  
}


