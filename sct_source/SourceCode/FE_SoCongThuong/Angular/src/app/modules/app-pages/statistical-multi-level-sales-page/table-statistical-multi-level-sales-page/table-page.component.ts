import { PageInfoService, PageLink } from '../../../../_metronic/layout/core/page-info.service';
import { PaginatorState } from '../../../../_metronic/shared/crud-table/models/paginator.model';
import { ISearchView } from '../../../../_metronic/shared/crud-table/models/search.model';
import { IFilterView } from '../../../../_metronic/shared/crud-table/models/filter.model';
import { ISortView, SortState } from '../../../../_metronic/shared/crud-table/models/sort.model';
import { GroupingState, IGroupingView } from '../../../../_metronic/shared/crud-table/models/grouping.model';
import { ChangeDetectorRef, Component, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { catchError, of, Subscription, throwIfEmpty } from 'rxjs';
import { StatisticalMultiLevelSalesPageService } from '../_services/statistical-multi-level-sales-page.service';
import { CommonService } from 'src/app/_metronic/shared/services/common.service';
import { environment } from 'src/environments/environment';
import { HttpClient } from '@angular/common/http';
import Swal from 'sweetalert2';
import * as moment from 'moment';
import { Options } from 'select2';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { EditMultiLevelSalesManagementModalComponent } from '../../multi-level-sales-management-page/table-multi-level-sales-management-page/components/edit-multi-level-sales-management-modal/edit-modal.component';
import { MultiLevelSalesManagementPageService } from '../../multi-level-sales-management-page/_services/multi-level-sales-management-page.service';

@Component({
  selector: 'app-table-page',
  templateUrl: './table-page.component.html',
  styleUrls: ['./table-page.component.scss']
})

export class TableStatisticalMultiLevelSalesPageComponent implements OnInit,
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
  private subscriptions: Subscription[] = []; // Read more: => https://brianflove.com/2016/12/11/anguar-2-unsubscribe-observables/

  /** API Data */
  data: any = [];
  total: any = {};
  business: any = [];
  district: any = [];
  YearData: any = [];
  listBusinessMultiLevel: any = [];

  constructor(
    private fb: FormBuilder,
    public statisticalMultiLevelSalesPageService: StatisticalMultiLevelSalesPageService,
    private commonService: CommonService,
    private cd: ChangeDetectorRef,
    private modalService: NgbModal,
    private multiLevelSalesManagementPageService: MultiLevelSalesManagementPageService,

  ) { }

  ngOnInit(): void {
    this.loadListBusinessMultiLevel();
    this.searchForm();
    this.filterForm();
    this.loadBusiness();
    this.loadDistrict();
    this.getYearsList();
    this.statisticalMultiLevelSalesPageService.fetch();
    this.grouping = this.statisticalMultiLevelSalesPageService.grouping;
    this.paginator = this.statisticalMultiLevelSalesPageService.paginator;
    this.sorting = this.statisticalMultiLevelSalesPageService.sorting;
    const sb = this.statisticalMultiLevelSalesPageService.isLoading$.subscribe((res: any) => this.isLoading = res);
    this.subscriptions.push(sb);
    this.options = {
      theme: 'bootstrap5',
      templateSelection: this.templateSelection,
    };
  }

  ngOnDestroy() {
    this.subscriptions.forEach((sb) => sb.unsubscribe());
    this.statisticalMultiLevelSalesPageService.setDefaults();
  }

  public templateSelection = (state: any): JQuery | string => {
    if (!state.id) {
      return state.text;
    }
    return jQuery('<span class="form-select form-select-solid form-select-lg">' + state.text + '</span>');
  }

  loadBusiness() {
    this.commonService.getBusiness().subscribe((res: any) => {
      const defaultOption = {
        id: "00000000-0000-0000-0000-000000000000",
        text: '-- Chọn --',
      };
      const result = [...res.items.map((item: any) => ({ id: item.businessId, text: item.businessNameVi }))];
      result.sort((i1, i2) => i1.text.localeCompare(i2.text));
      result.unshift(defaultOption);
      this.business = result;
    })
  }

  calcuTotal() {
    this.statisticalMultiLevelSalesPageService.items$.subscribe(items => {
      let obj = {
        totalRevenue: items.reduce((acc, item) => acc + item.revenue, 0),
        totalScale: items.reduce((acc, item) => acc + item.scale, 0)
      }
      this.total = obj;
    });
    return this.total
  }

  loadDistrict() {
    this.commonService.getDistrict().subscribe((res: any) => {
      const defaultOption = {
        id: "00000000-0000-0000-0000-000000000000",
        text: '-- Chọn --',
      };
      const result = [...res.items.map((item: any) => ({ id: item.districtId, text: item.districtName }))];
      result.sort((i1, i2) => i1.text.localeCompare(i2.text));
      result.unshift(defaultOption);
      this.district = result;
    })
  }

  getYearsList() {
    const currentYear = new Date().getFullYear();
    const yearsList: any = [{ id: 0, text: "-- Chọn --" }];

    for (let i = -10; i <= 10; i++) {
      const year = currentYear + i;
      yearsList.push({ id: year, text: year });
    }

    this.YearData = yearsList;
  }

  filterForm() {
    this.filterGroup = this.fb.group({
      BusinessId: ['00000000-0000-0000-0000-000000000000'],
      DistrictId: ['00000000-0000-0000-0000-000000000000'],
      Year: [""],
      // MinDate: [null],
      // MaxDate: [null]
    });
    this.filterGroup.controls.BusinessId.valueChanges.subscribe(() =>
      this.filter()
    )
    this.filterGroup.controls.DistrictId.valueChanges.subscribe(() =>
      this.filter()
    )
    this.filterGroup.controls.Year.valueChanges.subscribe(() =>
      this.filter()
    )
    // this.filterGroup.controls.MinDate.valueChanges.subscribe(() =>
    //   this.filter()
    // )
    // this.filterGroup.controls.MaxDate.valueChanges.subscribe(() =>
    //   this.filter()
    // )
  }

  filter() {
    const filter: { [key: string]: string } = {};
    // const MinDate = this.filterGroup.controls['MinDate'].value;
    // const MaxDate = this.filterGroup.controls['MaxDate'].value;
    const BusinessId = this.filterGroup.controls['BusinessId'].value;
    const DistrictId = this.filterGroup.controls['DistrictId'].value;
    const Year = this.filterGroup.controls['Year'].value;
    // if (MinDate != null) {
    //   filter['MinDate'] = MinDate;
    // }
    // if (MaxDate != null) {
    //   filter['MaxDate'] = MaxDate;
    // }
    if (BusinessId != '00000000-0000-0000-0000-000000000000') {
      filter['BusinessId'] = BusinessId;
    }
    if (DistrictId != '00000000-0000-0000-0000-000000000000') {
      filter['DistrictId'] = DistrictId;
    }
    if (Year != "") {
      filter['YearReport'] = Year;
    }
    this.statisticalMultiLevelSalesPageService.patchState({ filter });
  }

  resetFilter() {
    this.filterGroup.controls['BusinessId'].reset('00000000-0000-0000-0000-000000000000');
    this.filterGroup.controls['DistrictId'].reset('00000000-0000-0000-0000-000000000000');
    this.filterGroup.controls['Year'].reset("");
  }

  searchForm() {
    this.searchGroup = this.fb.group({
      searchTerm: [''],
    });
    const searchEvent = this.searchGroup.controls.searchTerm.valueChanges
      .subscribe((val) => this.search(val));
    this.subscriptions.push(searchEvent);
  }

  search(searchTerm: string) {
    if (searchTerm.length == 0)
      this.statisticalMultiLevelSalesPageService.patchState({ searchTerm });
  }

  onEnter() {
    this.searchData(this.searchGroup.controls.searchTerm.value)
  }

  searchData(searchTerm: string) {
    this.statisticalMultiLevelSalesPageService.patchState({ searchTerm });
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
    this.statisticalMultiLevelSalesPageService.patchState({ sorting });
  }

  // pagination
  paginate(paginator: PaginatorState) {
    this.statisticalMultiLevelSalesPageService.patchState({ paginator });
  }
  
  loadListBusinessMultiLevel(){
    const sb = this.multiLevelSalesManagementPageService.getBusinessMultiLevel().subscribe((res: any) => {
      const data = [
        {
          id: '00000000-0000-0000-0000-000000000000',
          text: '-- Chọn -- ',
          startDate: null,
          numCert: '',
          goods: '',
          contact: '',
          phoneNumber: '',
          contactAddress: '',
          address: '',
          statusName: '',
          districtName: '',
          note: '',
          localConfirm: '',
          certDate: '',
          certExp: ''
        },
        ...res.items.map((item: any) => ({
          id: item.businessMultiLevelId,
          text: item.businessName,
          startDate: item.startDate,
          numCert: item.numCert,
          goods: item.goods,
          contact: item.contact,
          phoneNumber: item.phoneNumber,
          contactAddress: item.addressContact,
          address: item.address,
          statusName: item.statusName,
          districtName: item.districtName,
          note: item.note,
          localConfirm: item.localConfirm,
          certDate: item.certDate,
          certExp: item.certExp,
          businessCode: item.businessCode
        }))
      ]
      this.listBusinessMultiLevel = data;
    })
    this.subscriptions.push(sb);
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

  getDistrict() {
    const value = this.filterGroup.controls.DistrictId.value;
    if (value != '00000000-0000-0000-0000-000000000000') {
      const text = this.district.find((x: any) => x.id == value).text
      return " - Huyện " + text
    } else {
      return ""
    }
  }

  getYear() {
    const value = this.filterGroup.controls.Year.value;
    if (value != 0) {
      return " - Năm " + value
    } else {
      return ""
    }
  }
  
  view(id: any, type: any) {
    const modalRef = this.modalService.open(EditMultiLevelSalesManagementModalComponent, { size: 'xl' });
    modalRef.componentInstance.id = id;
    modalRef.componentInstance.type = type;
    modalRef.componentInstance.viewInfo = 'viewInfo';
    modalRef.componentInstance.listBusinessMultiLevel = this.listBusinessMultiLevel;
    modalRef.result.then(() =>
      this.statisticalMultiLevelSalesPageService.fetch(),
      () => { }
    );
  }
}
