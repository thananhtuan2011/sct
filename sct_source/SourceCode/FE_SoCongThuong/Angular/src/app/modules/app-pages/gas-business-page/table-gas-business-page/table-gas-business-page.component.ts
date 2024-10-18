import {
  PageInfoService,
  PageLink,
} from '../../../../_metronic/layout/core/page-info.service';
import { PaginatorState } from '../../../../_metronic/shared/crud-table/models/paginator.model';
import { ISearchView } from '../../../../_metronic/shared/crud-table/models/search.model';
import { IFilterView } from '../../../../_metronic/shared/crud-table/models/filter.model';
import {
  ISortView,
  SortState,
} from '../../../../_metronic/shared/crud-table/models/sort.model';
import {
  GroupingState,
  IGroupingView,
} from '../../../../_metronic/shared/crud-table/models/grouping.model';
import {
  ICreateAction,
  IDeleteAction,
  IDeleteSelectedAction,
  IEditAction,
  IFetchSelectedAction,
  IUpdateStatusForSelectedAction,
} from '../../../../_metronic/shared/crud-table/models/table.model';
import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { catchError, finalize, Observable, of, Subscription, tap } from 'rxjs';
import { GasBusinessPageService } from '../_services/gas-business-page.service';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { EditGasBusinessModalComponent } from './components/edit-gas-business-modal/edit-modal.component';
import Swal from 'sweetalert2';
import { Options } from 'select2';
import * as moment from 'moment';
import { environment } from 'src/environments/environment';
import { HttpClient } from '@angular/common/http';
import { CommonService } from 'src/app/_metronic/shared/services/common.service';

@Component({
  selector: 'app-table-gas-business-page',
  templateUrl: './table-gas-business-page.component.html',
})
export class TableGasBusinessPageComponent
  implements
    OnInit,
    OnDestroy,
    ICreateAction,
    IEditAction,
    IDeleteAction,
    IFetchSelectedAction,
    IUpdateStatusForSelectedAction,
    ISortView,
    IFilterView,
    IGroupingView,
    ISearchView,
    IFilterView
{
  paginator: PaginatorState;
  sorting: SortState;
  grouping: GroupingState;
  isLoading: boolean;
  filterGroup: FormGroup;
  searchGroup: FormGroup;
  filerExport: any = {};
  typeBusiness: any = [
    {
      id: 2,
      text: 'Tất cả',
    },
    {
      id: 0,
      text: 'Thương nhân kinh doanh',
    },
    {
      id: 1,
      text: 'Cửa hàng bán lẻ',
    },
  ];
  Status: any = [
    {
      id: 0,
      text: 'Tất cả',
    },
    {
      id: 1,
      text: 'Đủ điều kiện cấp GCN',
    },
    {
      id: 2,
      text: 'Chưa đủ điều kiện cấp GCN',
    },
  ];
  options: Options;
  complianceStatusData: any;
  districtData: any;
  private subscriptions: Subscription[] = []; // Read more: => https://brianflove.com/2016/12/11/anguar-2-unsubscribe-observables/
  constructor(
    private fb: FormBuilder,
    public gasBusiness: GasBusinessPageService,
    private modalService: NgbModal,
    private http: HttpClient,
    private commonService: CommonService
  ) {}

  ngOnInit(): void {
    this.loadComplianceStatus();
    this.loadDistrict();
    this.filterForm();
    this.searchForm();
    this.gasBusiness.fetch();
    this.grouping = this.gasBusiness.grouping;
    this.paginator = this.gasBusiness.paginator;
    this.sorting = this.gasBusiness.sorting;
    const sb = this.gasBusiness.isLoading$.subscribe(
      (res: any) => (this.isLoading = res)
    );
    this.subscriptions.push(sb);
    this.options = {
      theme: 'bootstrap5',
      templateSelection: this.templateSelection,
    };
  }

  public templateSelection = (state: any): JQuery | string => {
    if (!state.id) {
      return state.text;
    }
    return jQuery(
      '<span class="form-select form-select-solid form-select-lg">' +
        state.text +
        '</span>'
    );
  };

  ngOnDestroy() {
    this.subscriptions.forEach((sb) => sb.unsubscribe());
    this.gasBusiness.setDefaults();
  }

  // filtration
  filterForm() {
    this.filterGroup = this.fb.group({
      TypeBusiness: 2,
      Status: 0,
      ComplianceStatus: '00000000-0000-0000-0000-000000000000',
      District: '00000000-0000-0000-0000-000000000000',
      DateStart: null,
      DateEnd: null,
      //searchTerm: [''],
    });
    this.subscriptions.push(
      this.filterGroup.controls.TypeBusiness.valueChanges.subscribe(() =>
        this.filter()
      )
    );
    this.subscriptions.push(
      this.filterGroup.controls.Status.valueChanges.subscribe(() =>
        this.filter()
      )
    );
    this.subscriptions.push(
      this.filterGroup.controls.ComplianceStatus.valueChanges.subscribe(() =>
        this.filter()
      )
    );
    this.subscriptions.push(
      this.filterGroup.controls.District.valueChanges.subscribe(() =>
        this.filter()
      )
    );
    this.subscriptions.push(
      this.filterGroup.controls.DateStart.valueChanges.subscribe(() =>
        this.filter()
      )
    );
    this.subscriptions.push(
      this.filterGroup.controls.DateEnd.valueChanges.subscribe(() =>
        this.filter()
      )
    );
  }

  filter() {
    const filter: { [key: string]: string } = {};
    const TypeBusiness = this.filterGroup.controls['TypeBusiness'].value;
    const Status = this.filterGroup.controls['Status'].value;
    if (TypeBusiness && TypeBusiness != 2) {
      filter['TypeBusiness'] = TypeBusiness.toString();
    }
    if (Status && Status != 0) {
      filter['Status'] = Status.toString();
    }
    const complianceStatus =
      this.filterGroup.controls['ComplianceStatus'].value;
    const district = this.filterGroup.controls['District'].value;
    const dateStart = this.filterGroup.controls['DateStart'].value;
    const dateEnd = this.filterGroup.controls['DateEnd'].value;

    if (
      complianceStatus &&
      complianceStatus != '00000000-0000-0000-0000-000000000000'
    ) {
      filter['ComplianceStatus'] = complianceStatus;
    }

    if (district && district != '00000000-0000-0000-0000-000000000000') {
      filter['District'] = district;
    }

    if (dateStart != null) {
      filter['DateStart'] = dateStart;
    }
    if (dateEnd != null) {
      filter['DateEnd'] = dateEnd;
    }
    this.filerExport = filter;
    this.gasBusiness.patchState({ filter });
  }

  // search
  searchForm() {
    this.searchGroup = this.fb.group({
      searchTerm: [''],
    });
    const searchEvent =
      this.searchGroup.controls.searchTerm.valueChanges.subscribe((val) =>
        this.search(val)
      );
    this.subscriptions.push(searchEvent);
  }

  search(searchTerm: string) {
    if (searchTerm.length == 0) this.gasBusiness.patchState({ searchTerm });
  }

  onEnter() {
    this.searchData(this.searchGroup.controls.searchTerm.value);
  }

  searchData(searchTerm: string) {
    this.gasBusiness.patchState({ searchTerm });
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
    this.gasBusiness.patchState({ sorting });
  }

  // pagination
  paginate(paginator: PaginatorState) {
    this.gasBusiness.patchState({ paginator });
  }

  // form actions
  create() {
    this.edit(undefined);
  }

  edit(item: any) {
    const modalRef = this.modalService.open(EditGasBusinessModalComponent, {
      size: 'lg',
    });
    if (item) {
      modalRef.componentInstance.id = item.gasBusinessId;
      modalRef.componentInstance.itemData = item;
      modalRef.componentInstance.type = '';
    }
    modalRef.componentInstance.districtData = this.districtData;
    modalRef.componentInstance.complianceStatusData = this.complianceStatusData;
    modalRef.result.then(
      () => this.gasBusiness.fetch(),
      () => {}
    );
  }

  view(item: any) {
    const modalRef = this.modalService.open(EditGasBusinessModalComponent, {
      size: 'lg',
    });
    if (item) {
      modalRef.componentInstance.districtData = this.districtData;
      modalRef.componentInstance.id = item.gasBusinessId;
      modalRef.componentInstance.complianceStatusData =
        this.complianceStatusData;
      modalRef.componentInstance.itemData = item;
      modalRef.componentInstance.type = 'view';
    }
    modalRef.result.then(
      () => this.gasBusiness.fetch(),
      () => {}
    );
  }

  delete(id: any) {
    Swal.fire({
      title: 'Bạn có muốn xóa ?',
      text: 'Hành động này không thể hoàn tác!',
      icon: 'warning',
      showCancelButton: true,
      confirmButtonColor: '#3085d6',
      cancelButtonColor: '#d33',
      confirmButtonText: 'Xác nhận',
      cancelButtonText: 'Thoát',
    }).then((result) => {
      if (result.isConfirmed) {
        this.isLoading = true;
        const sb = this.gasBusiness
          .delete(id)
          .pipe(
            tap((res) => {
              Swal.fire({
                icon: res.status == 1 ? 'success' : 'error',
                title: res.status == 1 ? 'Thành công' : 'Thất bại',
                confirmButtonText: 'Xác nhận',
                text:
                  res.status == 1
                    ? 'Xóa thành công'
                    : res.status == 0
                    ? res.error.msg
                    : 'Xóa thất bại',
              });
              this.filter();
            }),
            catchError((errorMessage) => {
              return of(undefined);
            }),
            finalize(() => {
              this.isLoading = false;
            })
          )
          .subscribe();
      }
    });
  }

  updateStatusForSelected() {}

  fetchSelected() {}

  exportFile() {
    const moment = require('moment');
    const timeString = moment().format('DDMMYYYYHHmmss');
    const fileName = 'QuanLyLinhVucKinhDoanhKhi_' + timeString + '.xlsx';

    Swal.fire({
      icon: 'info',
      title: 'Đang xuất File...',
      // text: 'Vui lòng đợi một lúc trước khi file của bạn sẵn sàng!',
      didOpen: () => {
        Swal.showLoading();
      },
    });
    const query = {
      filter: this.filerExport,
      grouping: {},
      paginator: {},
      sorting: { column: 'id', direction: 'desc' },
      searchTerm: this.searchGroup.controls.searchTerm.value,
    };
    this.http
      .post(`${environment.apiUrl}/GasBusiness/Export`, query, {
        responseType: 'blob',
      })
      .pipe(
        catchError((errorMessage: any) => {
          console.error(errorMessage);
          Swal.fire({
            icon: 'error',
            title: 'Xuất File thất bại',
            confirmButtonText: 'Xác nhận',
          });
          return of();
        })
      )
      .subscribe((res) => {
        const file = new Blob([res], {
          type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet',
        });
        const fileURL = URL.createObjectURL(file);
        const a = document.createElement('a');
        a.href = fileURL;
        a.download = fileName;
        document.body.append(a);
        a.click();
        a.remove();
        URL.revokeObjectURL(fileURL);
        console.log(`File ${fileName} đã tải xong.`);
        Swal.fire({
          icon: 'success',
          title: 'Xuất File thành công',
          confirmButtonText: 'Xác nhận',
        });
      });
  }
  clearFilter() {
    this.filterGroup.controls.TypeBusiness.setValue(2);
    this.filterGroup.controls.Status.setValue(0);
    this.filterGroup.controls.ComplianceStatus.reset(
      '00000000-0000-0000-0000-000000000000'
    );
    this.filterGroup.controls.District.reset(
      '00000000-0000-0000-0000-000000000000'
    );
    this.filterGroup.controls.DateStart.reset(null);
    this.filterGroup.controls.DateEnd.reset(null);
    this.filter();
  }

  loadComplianceStatus() {
    const sb = this.commonService
      .GetConfig('COMPLIANCE_STATUS')
      .subscribe((res: any) => {
        const data = [
          { id: '00000000-0000-0000-0000-000000000000', text: '-- Chọn --' },
          ...res.items.listConfig.map((item: any) => ({
            id: item.categoryId,
            text: item.categoryName,
          })),
        ];
        this.complianceStatusData = data;
      });
    this.subscriptions.push(sb);
  }

  loadDistrict() {
    const sb = this.commonService.getDistrict().subscribe((res: any) => {
      const data = [
        { id: '00000000-0000-0000-0000-000000000000', text: '-- Chọn --' },
        ...res.items.map((item: any) => ({
          id: item.districtId,
          text: item.districtName,
        })),
      ];
      this.districtData = data;
    });
    this.subscriptions.push(sb);
  }

  convert_date(string_date: string) {
    var result = moment.utc(string_date, 'DD/MM/YYYY');
    return result;
  }

  convert_date_string(string_date: string | null) {
    if (string_date == null) {
      return null;
    }
    let date = string_date.split('T')[0];
    let list = date.split('-'); //["year", "month", "day"]
    let result = list[2] + '/' + list[1] + '/' + list[0];
    return result;
  }
}
