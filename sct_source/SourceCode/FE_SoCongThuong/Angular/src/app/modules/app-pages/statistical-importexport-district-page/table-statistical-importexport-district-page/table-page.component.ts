import { PageInfoService, PageLink } from '../../../../_metronic/layout/core/page-info.service';
import { PaginatorState } from '../../../../_metronic/shared/crud-table/models/paginator.model';
import { ISearchView } from '../../../../_metronic/shared/crud-table/models/search.model';
import { IFilterView } from '../../../../_metronic/shared/crud-table/models/filter.model';
import { ISortView, SortState } from '../../../../_metronic/shared/crud-table/models/sort.model';
import { GroupingState, IGroupingView } from '../../../../_metronic/shared/crud-table/models/grouping.model';
import { ChangeDetectorRef, Component, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { catchError, of, Subscription, throwIfEmpty } from 'rxjs';
import { StatisticalImportExportDistrictPageService } from '../_services/statistical-importexport-district-page.service';
import { CommonService } from 'src/app/_metronic/shared/services/common.service';
import { environment } from 'src/environments/environment';
import { HttpClient } from '@angular/common/http';
import { ActivatedRoute, Router } from '@angular/router';
import Swal from 'sweetalert2';
import * as moment from 'moment';
import { Options } from 'select2';
import { MatTabChangeEvent } from '@angular/material/tabs';


@Component({
  selector: 'app-table-page',
  templateUrl: './table-page.component.html',
  styleUrls: ['./table-page.component.scss'],
})

export class TableStatisticalImportExportDistrictPageComponent implements OnInit,
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
  options: Options;

  //API data
  data: any = [];
  totalData: any = [];
  districtData: any = [];

  //Filter
  filterBody: any = { "filter": {} };
  districtId: string = '';
  minDate: any = null;
  maxDate: any = null;
  // yearData: any = [];
  // yearNow: number = moment().year();

  //Current Tab
  currentTab: number = 0; //0 - Import, 1 - Export

  districtName = "Ba Tri"

  isNext: boolean = false;


  private subscriptions: Subscription[] = []; // Read more: => https://brianflove.com/2016/12/11/anguar-2-unsubscribe-observables/
  public parameterValue: string;

  constructor(
    private fb: FormBuilder,
    private statisticalImportExportDistrictPageService: StatisticalImportExportDistrictPageService,
    private commonService: CommonService,
    private http: HttpClient,
    private cd: ChangeDetectorRef,
    private route: ActivatedRoute,
  ) { 
    this.route.queryParams.subscribe(res => {
      if (res['DistrictId']) {
        this.isNext = true;
        this.districtId = res['DistrictId'];
        this.minDate = res['MinDate'] ?? null;
        this.maxDate = res['MaxDate'] ?? null;
      }
    })
  }

  ngOnInit(): void {
    this.searchForm();
    this.filterForm();
    this.loadDistrict();
    // this.loadYear();
    // this.statisticalImportExportDistrictPageService.fetch();
    // this.grouping = this.statisticalImportExportDistrictPageService.grouping;
    // this.paginator = this.statisticalImportExportDistrictPageService.paginator;
    // this.sorting = this.statisticalImportExportDistrictPageService.sorting;
    // const sb = this.statisticalImportExportDistrictPageService.isLoading$.subscribe((res: any) => this.isLoading = res);
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
    this.statisticalImportExportDistrictPageService.setDefaults();
  }

  //Khi thay đổi giữa 2 tab Xuất - Nhập khẩu:
  tabChanged = (tabChangeEvent: MatTabChangeEvent): void => {
    this.currentTab = tabChangeEvent.index
    this.filter();
    // this.loadData(this.filterBody) //{ "filter": { "Type": this.currentTab.toString(), "DistrictId": this.districtId} }
    // this.filterGroup.controls.MinDate.setValue(null, { emitEvent: false });
    // this.filterGroup.controls.MaxDate.setValue(null, { emitEvent: false });
    // this.filterGroup.controls.Year.setValue(moment().year(), { emitEvent: false });
    // this.filterGroup.controls.District.setValue(this.districtId, { emitEvent: false });
    // this.filterGroup.updateValueAndValidity();
  }

  loadDistrict() {
    this.commonService.getDistrict().subscribe((res: any) => {
      const districts = [];
      
      for (var district of res.items) {
        let obj_district = {
          id: district.districtId,
          text: district.districtName
        }
        districts.push(obj_district)
      }
      this.districtData = districts
      // this.loadData({"filter": {"Type": this.currentTab.toString(), "DistrictId": this.districtId}});
      if (this.isNext) {
        this.filterGroup.patchValue({
          District: this.districtId,
          MinDate: this.minDate,
          MaxDate: this.maxDate
        })
      } else {
        this.filterGroup.controls.District.setValue(res.items[0].districtId);
      }
      // if (this.filterGroup.controls.District.value == "" ) {
      //   this.filterGroup.controls.District.setValue(this.districtId);
      // }
    })
  }

  // loadYear() {
  //   for (let i = moment().year() - 20; i <= moment().year() + 20; i++) {
  //     let item: any = {};
  //     item.id = i;
  //     item.text = i.toString();
  //     this.yearData.push(item);
  //   }
  // }

  loadData(Filter: any) {
    this.statisticalImportExportDistrictPageService.loadData(Filter).subscribe((res: any) => {
      this.data = res.data.data;
      this.totalData = res.data.total;
      this.cd.detectChanges();
    })
  }

  filterForm() {
    this.filterGroup = this.fb.group({
      MinDate: [null],
      MaxDate: [null],
      District: [this.districtId],
      // Year: [this.yearNow],
    });
    this.filterGroup.controls.MinDate.valueChanges.subscribe(() =>
      this.filter()
    )
    this.filterGroup.controls.MaxDate.valueChanges.subscribe(() =>
      this.filter()
    )
    this.filterGroup.controls.District.valueChanges.subscribe((x) => {
      this.districtName = this.districtData.find((f : any) => f.id == x).text;
      this.filter()
    })
    // this.filterGroup.controls.Year.valueChanges.subscribe(() =>
    //   this.filter()
    // )
  }

  filter() {
    const filter: { [key: string]: string } = {};
    const MinDate = this.filterGroup.controls['MinDate'].value;
    const MaxDate = this.filterGroup.controls['MaxDate'].value;
    const District = this.filterGroup.controls['District'].value;
    // const Year = this.filterGroup.controls['Year'].value;
    if (MinDate != null) {
      filter['MinDate'] = MinDate;
    }
    if (MaxDate != null) {
      filter['MaxDate'] = MaxDate;
    }
    if (District) {
      filter['DistrictId'] = District;
    }
    // if (Year) {
    //   filter['Year'] = Year.toString();
    // }
    filter['Type'] = this.currentTab.toString();
    this.filterBody = { "filter": filter };
    this.loadData(this.filterBody)
    // this.statisticalImportExportDistrictPageService.patchState({ filter });
  }

  searchForm() {
    this.searchGroup = this.fb.group({
      searchTerm: [''],
    });
    // const searchEvent = this.searchGroup.controls.searchTerm.valueChanges
    //   .subscribe((val) => this.search(val));
    // this.subscriptions.push(searchEvent);
  }

  search(searchTerm: string) {
    // this.statisticalImportExportDistrictPageService.patchState({ searchTerm });
  }

  onEnter() {
    // this.searchTypeOfBusiness(this.searchGroup.controls.searchTerm.value)
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
    this.statisticalImportExportDistrictPageService.patchState({ sorting });
  }

  // pagination
  paginate(paginator: PaginatorState) {
    this.statisticalImportExportDistrictPageService.patchState({ paginator });
  }

  exportFile() {
    // const now = new Date();
    // const timeString = now.toLocaleString('en-US', {
    //   year: 'numeric',
    //   month: '2-digit',
    //   day: '2-digit',
    //   hour: '2-digit',
    //   minute: '2-digit',
    //   second: '2-digit'
    // }).replace(/\D/g, '');
    // const fileName = "Thongkexaydungnangcapchosieuthitrungtamthuongmaitinh_" + timeString + ".xlsx"

    // Swal.fire({
    //   title: 'Đang xuất File...',
    //   // text: 'Vui lòng đợi một lúc trước khi file của bạn sẵn sàng!',
    //   didOpen: () => {
    //     Swal.showLoading()
    //   },
    // })

    // this.http.get(`${environment.apiUrl}/Statistical/ExportStatisticalHasBeenImportExportd`, {
    //   responseType: 'blob',
    // }).pipe(
    //   catchError((errorMessage: any) => {
    //     console.error(errorMessage)
    //     Swal.fire({
    //       icon: 'error',
    //       title: 'Xuất File thất bại',
    //       confirmButtonText: 'Xác nhận',
    //     });
    //     return of();
    //   }),
    // ).subscribe(
    //   (res) => {
    //     const file = new Blob([res], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' });
    //     const fileURL = URL.createObjectURL(file);
    //     const a = document.createElement('a');
    //     a.href = fileURL;
    //     a.download = fileName;
    //     document.body.append(a);
    //     a.click();
    //     a.remove();
    //     URL.revokeObjectURL(fileURL);
    //     console.log(`File ${fileName} đã tải xong.`);
    //     Swal.fire({
    //       icon: 'success',
    //       title: 'Xuất File thành công',
    //       confirmButtonText: 'Xác nhận',
    //     });
    //   },
    // );
  }

  getHeight(): any {
    let tmp_height = 0;
    tmp_height = window.innerHeight - 300;
    return tmp_height + 'px';
  }
}


