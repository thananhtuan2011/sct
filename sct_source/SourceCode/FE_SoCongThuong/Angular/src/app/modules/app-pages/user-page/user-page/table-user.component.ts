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
import Swal from 'sweetalert2';
import { UserService } from '../_services/user.service';
import { EditUserModalComponent } from './components/edit-user-modal/edit-user-modal.component';
import { ChangePasswordModalComponent } from './components/change-password-modal/change-password-modal.component';
import { TreeUserComponent } from './components/tree-user-modal/tree-user-modal.component';

@Component({
  selector: 'app-table-user',
  templateUrl: './table-user.component.html'
})
export class TableUserComponent implements OnInit,
  OnDestroy,
  ICreateAction,
  IEditAction,
  IDeleteAction,
  IDeleteSelectedAction,
  IFetchSelectedAction,
  IUpdateStatusForSelectedAction,
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
  private subscriptions: Subscription[] = []; // Read more: => https://brianflove.com/2016/12/11/anguar-2-unsubscribe-observables/
  constructor(
    private fb: FormBuilder,
    public userService: UserService,
    private modalService: NgbModal
  ) { }

  ngOnInit(): void {
    this.filterForm();
    this.searchForm();
    this.userService.fetch();
    this.grouping = this.userService.grouping;
    this.paginator = this.userService.paginator;
    this.sorting = this.userService.sorting;
    const sb = this.userService.isLoading$.subscribe((res: any) => this.isLoading = res);
    this.subscriptions.push(sb);
  }

  ngOnDestroy() {
    this.subscriptions.forEach((sb) => sb.unsubscribe());
    this.userService.setDefaults();
  }

  // filtration
  filterForm() {
    this.filterGroup = this.fb.group({
      status: [''],
      type: [''],
      searchTerm: [''],
    });
    this.subscriptions.push(
      this.filterGroup.controls.status.valueChanges.subscribe(() =>
        this.filter()
      )
    );
    this.subscriptions.push(
      this.filterGroup.controls.type.valueChanges.subscribe(() => this.filter())
    );
  }

  filter() {
    const filter: { [key: string]: string } = {};
    const status = this.filterGroup.controls['status'].value;
    if (status) {
      filter['status'] = status;
    }
    this.userService.patchState({ filter });
  }

  // search
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
      this.userService.patchState({ searchTerm });
  }

  onEnter() {
    this.searchData(this.searchGroup.controls.searchTerm.value)
  }

  searchData(searchTerm: string) {
    this.userService.patchState({ searchTerm });
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
    this.userService.patchState({ sorting });
  }

  // pagination
  paginate(paginator: PaginatorState) {
    this.userService.patchState({ paginator });
  }

  // form actions
  create() {
    this.edit(0);
  }

  edit(id: any) {
    const modalRef = this.modalService.open(EditUserModalComponent, { size: 'xl' });
    modalRef.componentInstance.id = id;
    modalRef.result.then(() =>
      this.userService.fetch(),
      () => { }
    );
  }

  openTree(id: string) {
    const modalRef = this.modalService.open(TreeUserComponent, { size: '100px' });
    modalRef.componentInstance.id = id;
    modalRef.result.then(() =>
      this.userService.fetch(),
      () => { }
    );
  }

  delete(item: any) {
    Swal.fire({
      title: 'Bạn có muốn xóa?',
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
        item.isDel = true
        const sb = this.userService.update(item).pipe(
          tap((res) => {
            Swal.fire({
              icon: res.status == 1 ? 'success' : 'error',
              title: res.status == 1 ? 'Thành công' : 'Thất bại',
              confirmButtonText: 'Xác nhận',
              text: '' + (res.status == 1 ? 'Xóa thành công' : res.error.msg),
            });
            this.filter()
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

  deleteSelected() {
    Swal.fire({
      title: 'Bạn có muốn xóa?',
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
        const sb = this.userService.deleteItems(this.grouping.getSelectedRows()).pipe(
          tap((res) => {
            Swal.fire({
              icon: res.status == 1 ? 'success' : 'error',
              title: res.status == 1 ? 'Thành công' : 'Thất bại',
              confirmButtonText: 'Xác nhận',
              text: '' + (res.status == 1 ? 'Xóa sửa thành công' : res.error.msg),
            });
            this.filter()
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
  
  lockUser(item: any) {
    Swal.fire({
      title: item.status === 1 ? 'Bạn có muốn mở khóa người dùng này?' : 'Bạn có muốn khóa người dùng này?',
      text: item.status === 1 ? 'Mở khóa người dùng.' : 'Khóa người dùng.',
      icon: 'warning',
      showCancelButton: true,
      confirmButtonColor: '#3085d6',
      cancelButtonColor: '#d33',
      confirmButtonText: 'Xác nhận',
      cancelButtonText: 'Thoát'
    }).then((result) => {
      if (result.isConfirmed) {
        this.isLoading = true;
        if (item.status == 1) {
          item.status = 0;
          item.countLoginFail = 0;
          item.timeLock = null
        } else {
          item.status = 1
          item.countLoginFail = 8;

          const today = new Date();
          const futureDate = new Date(today.getFullYear() + 100, today.getMonth(), today.getDate());
          item.timeLock = futureDate;
        }
        const sb = this.userService.update(item).pipe(
          tap((res) => {
            Swal.fire({
              icon: res.status == 1 ? 'success' : 'error',
              title: res.status == 1 ? 'Thành công.' : 'Thất bại.',
              confirmButtonText: 'Xác nhận',
              text: ( item.status == 0 ? "Mở khoá " : "Khoá " ) + ( res.status == 1 ? 'thành công.' : 'thất bại.' ),
            });
            this.filter()
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

  doiMatKhau(item: any) {
    const modalRef = this.modalService.open(ChangePasswordModalComponent, {size: "lg"});
    modalRef.componentInstance.username = item.username;
    modalRef.result.then(() =>
      this.userService.fetch(),
      () => { }
    );
  }

  updateStatusForSelected() {
  }

  fetchSelected() {
  }
}


