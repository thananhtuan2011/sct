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
import { TradeFairOrganizationCertificationPageService } from '../_services/trade-fair-organization-page.service';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { EditTradeFairOrganizationCertificationModalComponent } from './components/edit-trade-fair-organization-modal/edit-modal.component';
import Swal from 'sweetalert2';


@Component({
  selector: 'app-table-page',
  templateUrl: './table-page.component.html'
})
export class TableTradeFairOrganizationCertificationPageComponent implements OnInit,
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
  minDate: string;
  maxDate: string;
  private subscriptions: Subscription[] = []; // Read more: => https://brianflove.com/2016/12/11/anguar-2-unsubscribe-observables/

  constructor(
    private fb: FormBuilder,
    public tradeFairOrganizationCertificationPageService: TradeFairOrganizationCertificationPageService,
    private modalService: NgbModal
  ) { }

  ngOnInit(): void {
    this.filterForm();
    this.searchForm();
    this.tradeFairOrganizationCertificationPageService.fetch();
    this.grouping = this.tradeFairOrganizationCertificationPageService.grouping;
    this.paginator = this.tradeFairOrganizationCertificationPageService.paginator;
    this.sorting = this.tradeFairOrganizationCertificationPageService.sorting;
    const sb = this.tradeFairOrganizationCertificationPageService.isLoading$.subscribe((res: any) => this.isLoading = res);
    this.subscriptions.push(sb);
  }

  ngOnDestroy() {
    this.subscriptions.forEach((sb) => sb.unsubscribe());
    this.tradeFairOrganizationCertificationPageService.setDefaults();
  }

  f_currency(value: any, args?: any): any {
    let nbr = Number((value + '').replace(/,|-/g, ""));
    const result = (nbr + '').replace(/(\d)(?=(\d{3})+(?!\d))/g, "$1,");
    return result
  }

  // filtration
  filterForm() {
    this.filterGroup = this.fb.group({
      MinDate: [this.minDate],
      MaxDate: [this.maxDate],
    });
    this.subscriptions.push(
      this.filterGroup.controls.MinDate.valueChanges.subscribe(() =>
        this.filter()
      )
    );
    this.subscriptions.push(
      this.filterGroup.controls.MaxDate.valueChanges.subscribe(() => this.filter())
    );
  }

  filter() {
    const filter: { [key: string]: string } = {};
    const minDate = this.filterGroup.controls['MinDate'].value;
    const maxDate = this.filterGroup.controls['MaxDate'].value;
    if (minDate != null && minDate.length > 0) {
      filter['MinTime'] = (minDate);
    }

    if (maxDate != null && maxDate.length > 0) {
      filter['MaxTime'] = maxDate;
    }
    this.tradeFairOrganizationCertificationPageService.patchState({ filter });
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
      this.tradeFairOrganizationCertificationPageService.patchState({ searchTerm });
  }

  onEnter() {
    this.searchData(this.searchGroup.controls.searchTerm.value)
  }

  searchData(searchTerm: string) {
    this.tradeFairOrganizationCertificationPageService.patchState({ searchTerm });
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
    this.tradeFairOrganizationCertificationPageService.patchState({ sorting });
  }

  // pagination
  paginate(paginator: PaginatorState) {
    this.tradeFairOrganizationCertificationPageService.patchState({ paginator });
  }

  // form actions
  create() {
    this.edit(0);
  }

  edit(id: any) {
    const modalRef = this.modalService.open(EditTradeFairOrganizationCertificationModalComponent, { size: 'lg' });
    modalRef.componentInstance.id = id;
    modalRef.result.then(() =>
      this.tradeFairOrganizationCertificationPageService.fetch(),
      () => { }
    );
  }

  view(id: any, type: any) {
    const modalRef = this.modalService.open(EditTradeFairOrganizationCertificationModalComponent, { size: 'lg' });
    modalRef.componentInstance.id = id;
    modalRef.componentInstance.type = type;
    modalRef.result.then(() =>
      this.tradeFairOrganizationCertificationPageService.fetch(),
      () => { }
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
      cancelButtonText: 'Thoát'
    }).then((result) => {
      if (result.isConfirmed) {
        this.isLoading = true;
        const sb = this.tradeFairOrganizationCertificationPageService.delete(id).pipe(
          tap((res) => {
            Swal.fire({
              icon: res.status == 1 ? 'success' : 'error',
              title: res.status == 1 ? 'Thành công' : 'Thất bại',
              confirmButtonText: 'Xác nhận',
              text: 'Xóa ' + (res.status == 1 ? 'thành công' : 'thất bại'),
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
        const sb = this.tradeFairOrganizationCertificationPageService.deletes(this.grouping.getSelectedRows()).pipe(
          tap((res) => {
            Swal.fire({
              icon: res.status == 1 ? 'success' : 'error',
              title: res.status == 1 ? 'Thành công' : 'Thất bại',
              confirmButtonText: 'Xác nhận',
              text: 'Xóa ' + (res.status == 1 ? 'thành công' : 'thất bại'),
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

  updateStatusForSelected() {
  }

  fetchSelected() {
  }
}


