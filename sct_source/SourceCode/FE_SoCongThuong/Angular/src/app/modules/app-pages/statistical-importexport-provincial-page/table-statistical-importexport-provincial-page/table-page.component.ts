import { PageInfoService, PageLink } from '../../../../_metronic/layout/core/page-info.service';
import { PaginatorState } from '../../../../_metronic/shared/crud-table/models/paginator.model';
import { ISearchView } from '../../../../_metronic/shared/crud-table/models/search.model';
import { IFilterView } from '../../../../_metronic/shared/crud-table/models/filter.model';
import { ISortView, SortState } from '../../../../_metronic/shared/crud-table/models/sort.model';
import { GroupingState, IGroupingView } from '../../../../_metronic/shared/crud-table/models/grouping.model';
import { ChangeDetectorRef, Component, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { catchError, of, Subscription, throwIfEmpty } from 'rxjs';
import { StatisticalImportExportProvincialPageService } from '../_services/statistical-importexport-provincial-page.service';
import { CommonService } from 'src/app/_metronic/shared/services/common.service';
import { environment } from 'src/environments/environment';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import Swal from 'sweetalert2';
import * as moment from 'moment';
import { Options } from 'select2';


@Component({
  selector: 'app-table-page',
  templateUrl: './table-page.component.html',
  styleUrls: ['./table-page.component.scss'],
})

export class TableStatisticalImportExportProvincialPageComponent implements OnInit,
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

  private subscriptions: Subscription[] = []; // Read more: => https://brianflove.com/2016/12/11/anguar-2-unsubscribe-observables/
  public parameterValue: string;

  constructor(
    private fb: FormBuilder,
    private statisticalImportExportProvincialPageService: StatisticalImportExportProvincialPageService,
    private http: HttpClient,
    private cd: ChangeDetectorRef,
    private router: Router,
  ) { }

  ngOnInit(): void {
    this.searchForm();
    this.filterForm();
    this.loadData(this.filterBody);
    // this.loadYear();
    // this.statisticalImportExportProvincialPageService.fetch();
    // this.grouping = this.statisticalImportExportProvincialPageService.grouping;
    // this.paginator = this.statisticalImportExportProvincialPageService.paginator;
    // this.sorting = this.statisticalImportExportProvincialPageService.sorting;
    // const sb = this.statisticalImportExportProvincialPageService.isLoading$.subscribe((res: any) => this.isLoading = res);
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
    this.statisticalImportExportProvincialPageService.setDefaults();
  }

  loadData(Filter: any) {
    this.statisticalImportExportProvincialPageService.loadData(Filter).subscribe((res: any) => {
      this.data = res.data.data;
      this.totalData = res.data.total;
      this.cd.detectChanges();
    })
  }

  filterForm() {
    this.filterGroup = this.fb.group({
      MinDate: [null],
      MaxDate: [null],
    });
    this.filterGroup.controls.MinDate.valueChanges.subscribe(() =>
      this.filter()
    )
    this.filterGroup.controls.MaxDate.valueChanges.subscribe(() =>
      this.filter()
    )
  }

  filter() {
    const filter: { [key: string]: string } = {};
    const MinDate = this.filterGroup.controls['MinDate'].value;
    const MaxDate = this.filterGroup.controls['MaxDate'].value;
    if (MinDate != null) {
      filter['MinDate'] = MinDate;
    }
    if (MaxDate != null) {
      filter['MaxDate'] = MaxDate;
    }
    this.filterBody = { "filter": filter };
    this.loadData(this.filterBody)
    // this.statisticalImportExportProvincialPageService.patchState({ filter });
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
    // this.statisticalImportExportProvincialPageService.patchState({ searchTerm });
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
    this.statisticalImportExportProvincialPageService.patchState({ sorting });
  }

  // pagination
  paginate(paginator: PaginatorState) {
    this.statisticalImportExportProvincialPageService.patchState({ paginator });
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

  goToDistrict(DistrictId: string) {
    const QueryParams: { [key: string]: string } = {};
    const MinDate = this.filterGroup.controls['MinDate'].value;
    const MaxDate = this.filterGroup.controls['MaxDate'].value;

    if (MinDate != null) {
      QueryParams['MinDate'] = MinDate;
    }
    if (MaxDate != null) {
      QueryParams['MaxDate'] = MaxDate;
    }
    QueryParams['DistrictId'] = DistrictId;

    this.router.navigate(['/StatisticalImportExportDistrict'], { queryParams: QueryParams});
  }
  
  f_currency(value: any, args?: any): any {
    let nbr = Number((value + '').replace(/,|-/g, ""));
    const result = (nbr + '').replace(/(\d)(?=(\d{3})+(?!\d))/g, "$1,");
    return result
  }
}


