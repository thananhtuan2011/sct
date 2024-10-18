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
import { ManageConfirmPromotionPageService } from '../_services/manage-confirm-promotion-page.service';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { EditManageConfirmPromotionModalComponent } from './components/edit-manage-confirm-promotion-page-modal/edit-manage-confirm-promotion-page.component'
import Swal from 'sweetalert2';
import { Options } from 'select2';




@Component({
  selector: 'app-table-manageconfirmpromotion-page',
  templateUrl: './table-manage-confirm-promotion-page.component.html',
  styleUrls: ['./table-manage-confirm-promotion-page.component.scss']
})

export class TableManageConfirmPromotionPageComponent implements OnInit,
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
  options: Options;
  minDate: any = null;
  maxDate: any = null;
  implementResultData: Array<any> = [
    {
      id: 0,
      text: 'Không thực hiện'
    },
    {
      id: 1,
      text: 'Có thực hiện'
    },
    {
      id: 2,
      text: '--Chọn--'
    }
  ];
  inputDataPersonData: Array<any> = [];
  inputDataPersonId: any = '00000000-0000-0000-0000-000000000000'
  implementResultId: any = 2;
  private subscriptions: Subscription[] = []; // Read more: => https://brianflove.com/2016/12/11/anguar-2-unsubscribe-observables/

  constructor(
    private fb: FormBuilder,
    public manageConfirmPromotionPageService: ManageConfirmPromotionPageService,
    private modalService: NgbModal
  ) { }
  deleteSelected(): void {
    throw new Error('Method not implemented.');
  }

  ngOnInit(): void {
    this.loadUser();
    this.filterForm();
    this.searchForm();
    this.manageConfirmPromotionPageService.fetch();
    this.grouping = this.manageConfirmPromotionPageService.grouping;
    this.paginator = this.manageConfirmPromotionPageService.paginator;
    this.sorting = this.manageConfirmPromotionPageService.sorting;
    const sb = this.manageConfirmPromotionPageService.isLoading$.subscribe((res: any) => this.isLoading = res);
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
    return jQuery('<span class="form-select form-select-solid form-select-lg">' + state.text + '</span>');
  }

  ngOnDestroy() {
    this.subscriptions.forEach((sb) => sb.unsubscribe());
    this.manageConfirmPromotionPageService.setDefaults();
  }

  change_value(event: any, ControlName: string) {
    this.filterGroup.controls[ControlName].setValue(event);
    this.filterGroup.updateValueAndValidity();
  }

  loadUser() {
    this.manageConfirmPromotionPageService.loadUser().subscribe((res: any) => {
      var userData = [
        {
          id: '00000000-0000-0000-0000-000000000000',
          text: '-- Chọn --',
        },
      ];
      for (var item of res.items) {
        let user = {
          id: item.userId,
          text: item.fullName,
        }
        userData.push(user)
      }
      this.inputDataPersonData = userData
    })
  }

  // filtration
  filterForm() {
    this.filterGroup = this.fb.group({
      status: [''],
      type: [''],
      searchTerm: [''],
      MinDate: [this.minDate],
      MaxDate: [this.maxDate],
      ImplementResultId: [this.implementResultId],
      InputDataPersonId: [this.inputDataPersonId]
    });
    this.subscriptions.push(
      this.filterGroup.controls.status.valueChanges.subscribe(() =>
        this.filter()
      )
    );
    this.subscriptions.push(
      this.filterGroup.controls.type.valueChanges.subscribe(() => this.filter())
    );
    this.subscriptions.push(
      this.filterGroup.controls.ImplementResultId.valueChanges.subscribe(() => this.filter())
    );
    this.subscriptions.push(
      this.filterGroup.controls.MinDate.valueChanges.subscribe(() =>
        this.filter()
      )
    );
    this.subscriptions.push(
      this.filterGroup.controls.MaxDate.valueChanges.subscribe(() =>
        this.filter()
      )
    );

    this.subscriptions.push(
      this.filterGroup.controls.InputDataPersonId.valueChanges.subscribe(() =>
        this.filter()
      )
    );

  }

  filter() {
    const filter: { [key: string]: string } = {};
    const status = this.filterGroup.controls['status'].value;
    const implementResultId = this.filterGroup.controls['ImplementResultId'].value;
    const inputDataPersonId = this.filterGroup.controls['InputDataPersonId'].value;
    const minDate = this.filterGroup.controls['MinDate'].value;
    const maxDate = this.filterGroup.controls['MaxDate'].value;
    if (status) {
      filter['status'] = status;
    }

    if (implementResultId != 2) {
      filter['ImplementResult'] = implementResultId;
    }
    if (inputDataPersonId != '00000000-0000-0000-0000-000000000000') {
      filter['InputDataPersonId'] = inputDataPersonId;
    }
    if (minDate != null && minDate.length > 0) {
      filter['MinTime'] = (minDate);
    }

    if (maxDate != null && maxDate.length > 0) {
      filter['MaxTime'] = maxDate;
    }

    this.manageConfirmPromotionPageService.patchState({ filter });
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
      this.manageConfirmPromotionPageService.patchState({ searchTerm });
  }

  onEnter() {
    this.searchData(this.searchGroup.controls.searchTerm.value)
  }
  
  searchData(searchTerm: string) {
    this.manageConfirmPromotionPageService.patchState({ searchTerm });
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
    this.manageConfirmPromotionPageService.patchState({ sorting });
  }

  // pagination
  paginate(paginator: PaginatorState) {
    this.manageConfirmPromotionPageService.patchState({ paginator });
  }

  // form actions
  create() {
    this.edit(0);
  }

  edit(id: any) {
    const modalRef = this.modalService.open(EditManageConfirmPromotionModalComponent, { size: 'xl' });
    modalRef.componentInstance.id = id;
    modalRef.result.then(() =>
      this.manageConfirmPromotionPageService.fetch(),
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
        const sb = this.manageConfirmPromotionPageService.deleteManageConfirmPromotion(id).pipe(
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

  view(id: any, type: any) {
    const modalRef = this.modalService.open(EditManageConfirmPromotionModalComponent, { size: 'xl' });
    modalRef.componentInstance.id = id;
    modalRef.componentInstance.type = type;
    modalRef.result.then(() =>
      this.manageConfirmPromotionPageService.fetch(),
      () => { }
    );
  }

  updateStatusForSelected() {
  }

  fetchSelected() {
  }
}


