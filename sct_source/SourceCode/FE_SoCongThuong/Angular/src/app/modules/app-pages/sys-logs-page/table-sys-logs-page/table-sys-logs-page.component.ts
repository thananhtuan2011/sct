import { PageInfoService, PageLink } from '../../../../_metronic/layout/core/page-info.service';
import { PaginatorState } from '../../../../_metronic/shared/crud-table/models/paginator.model';
import { ISearchView } from '../../../../_metronic/shared/crud-table/models/search.model';
import { IFilterView } from '../../../../_metronic/shared/crud-table/models/filter.model';
import { ISortView, SortState } from '../../../../_metronic/shared/crud-table/models/sort.model';
import { GroupingState, IGroupingView } from '../../../../_metronic/shared/crud-table/models/grouping.model';
import { ICreateAction, IDeleteAction, IDeleteSelectedAction, IEditAction, IFetchSelectedAction, IUpdateStatusForSelectedAction } from '../../../../_metronic/shared/crud-table/models/table.model';
import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { catchError, finalize, Observable, of, Subscription, tap } from 'rxjs';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
// import { EditMarketManagementModalComponent } from './components/edit-importexportinfo-modal/edit-importexportinfo-modal.component';
import Swal from 'sweetalert2';
import { Options } from 'select2';
import { SysLogsPageService } from '../_services/sys-logs-page.service';



@Component({
  selector: 'app-table-sys-logs-page',
  templateUrl: './table-sys-logs-page.component.html'
})
export class TableSysLogsPageComponent implements OnInit,
  OnDestroy,
  // ICreateAction,
  // IEditAction,
  // IDeleteAction,
  // IDeleteSelectedAction,
  IFetchSelectedAction,
  IUpdateStatusForSelectedAction,
  ISortView,
  IFilterView,
  IGroupingView,
  // ISearchView,
  IFilterView {
  paginator: PaginatorState;
  sorting: SortState;
  grouping: GroupingState;
  isLoading: boolean;
  filterGroup: FormGroup;
  searchGroup: FormGroup;
  public options: Options;

  //Thị trường
  public countryData: Array<any>;
  public countryId: string = '00000000-0000-0000-0000-000000000000';

  //MinDate
  public minDate: string;

  //MaxDate
  public maxDate: string;

  public username: string;

  //Phương thức
  public methodData: Array<any> = [
    {
      id: 'NONE',
      text: '-- Chọn --',
    },
    {
      id: 'LOGIN',
      text: 'Đăng nhập',
    },
    {
      id: 'LOGOUT',
      text: 'Đăng xuất',
    },
    // {
    //   id: 'VIEW',
    //   text: 'Xem',
    // },
    {
      id: 'CREATE',
      text: 'Tạo mới',
    },
    {
      id: 'UPDATE',
      text: 'Chỉnh sửa',
    },
    {
      id: 'DELETE',
      text: 'Xóa',
    }
  ];
  public methodId: string = 'NONE';

  private subscriptions: Subscription[] = []; // Read more: => https://brianflove.com/2016/12/11/anguar-2-unsubscribe-observables/

  constructor(
    private fb: FormBuilder,
    public sysLogsPageService: SysLogsPageService,
    private modalService: NgbModal
  ) { }

  ngOnInit(): void {
    this.filterForm();

    this.sysLogsPageService.fetch();
    this.grouping = this.sysLogsPageService.grouping;
    this.paginator = this.sysLogsPageService.paginator;
    this.sorting = this.sysLogsPageService.sorting;
    const sb = this.sysLogsPageService.isLoading$.subscribe((res: any) => this.isLoading = res);
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
    return jQuery('<span class="form-control form-control-lg form-control-solid">' + state.text + '</span>');
  }

  ngOnDestroy() {
    this.subscriptions.forEach((sb) => sb.unsubscribe());
    this.sysLogsPageService.setDefaults();
  }
  // filtration
  filterForm() {
    this.filterGroup = this.fb.group({
      countryId: [this.countryId],
      minDate: [this.minDate],
      maxDate: [this.maxDate],
      methodId: [this.methodId],
      username: [this.username],
    });
  }

  change_value(event: any, ControlName: string){
    this.filterGroup.controls[ControlName].setValue(event);
    this.filterGroup.updateValueAndValidity();
  }

  filter() {
    const filter: { [key: string]: string } = {};

    const methodId = this.filterGroup.controls['methodId'].value;
    const methodName = this.methodData.find(x => x.id == methodId).id;

    const minDate = this.filterGroup.controls['minDate'].value;

    const maxDate = this.filterGroup.controls['maxDate'].value;

    const username = this.filterGroup.controls['username'].value;

    if (methodId != 'NONE') {
      filter['Method'] = methodName;
    }

    if (minDate != null && minDate.length > 0) {
      filter['MinTime'] = minDate;
    }

    if (maxDate != null && maxDate.length > 0) {
      filter['MaxTime'] = maxDate;
    }

    if (username != null && username != '') {
      filter['Keyword'] = username;
    }

    this.sysLogsPageService.patchState({ filter });
  }

  reset_filter() {
    this.filterGroup.controls.methodId.reset(this.methodId);
    this.filterGroup.controls.minDate.reset(this.minDate);
    this.filterGroup.controls.maxDate.reset(this.maxDate);
    this.filterGroup.controls.username.reset(this.username);
    this.filter()
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
    this.sysLogsPageService.patchState({ sorting });
  }

  // pagination
  paginate(paginator: PaginatorState) {
    this.sysLogsPageService.patchState({ paginator });
  }


  updateStatusForSelected() {
  }

  fetchSelected() {
  }
  
  delete(id: any){
    Swal.fire({
      title: 'Bạn có muốn xóa ?',
      text: 'Hành động này không thể hoàn tác!',
      icon: 'warning',
      showCancelButton: true,
      confirmButtonColor: '#3085d6',
      cancelButtonColor: '#d33',
      confirmButtonText: 'Xác nhận',
      cancelButtonText: 'Thoát'
    }).then((result) => {
      if (result.isConfirmed) {
        this.isLoading = true;
        const sb = this.sysLogsPageService.deleteLog(id).pipe(
          tap((res) => {
            Swal.fire({
              icon: res.status == 1 ? 'success' : 'error',
              title: res.status == 1 ? 'Thành công' : 'Thất bại',
              confirmButtonText: 'Xác nhận',
              text: 'Xóa ' + (res.status == 1 ? 'thành công' : 'thất bại'),
            });
            this.sysLogsPageService.fetch();
          }),
          catchError((errorMessage) => {
            return of(undefined);
          }),
          finalize(() => {
            this.isLoading = false;
          })
        ).subscribe();
      }
    })
  }
}


