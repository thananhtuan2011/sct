import { PageInfoService, PageLink } from '../../../../_metronic/layout/core/page-info.service';
import { PaginatorState } from '../../../../_metronic/shared/crud-table/models/paginator.model';
import { ISearchView } from '../../../../_metronic/shared/crud-table/models/search.model';
import { IFilterView } from '../../../../_metronic/shared/crud-table/models/filter.model';
import { ISortView, SortState } from '../../../../_metronic/shared/crud-table/models/sort.model';
import { GroupingState, IGroupingView } from '../../../../_metronic/shared/crud-table/models/grouping.model';
import { ICreateAction, IDeleteAction, IEditAction, IFetchSelectedAction, IUpdateStatusForSelectedAction } from '../../../../_metronic/shared/crud-table/models/table.model';
import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { catchError, finalize, Observable, of, Subscription, tap } from 'rxjs';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import Swal from 'sweetalert2';
import * as moment from 'moment';
import { EditCateCriteriaModalComponent } from './components/edit-cate-criteria-modal/edit-cate-criteria-modal.component';
import { CateCriteriaNumberSevenService } from '../_services/cate-criteria.service';
import { Options } from 'select2';
import { CommonService } from 'src/app/_metronic/shared/services/common.service';
import { environment } from 'src/environments/environment';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-table-cate-criteria-page',
  styleUrls: ['./table-cate-criteria-page.component.scss'],
  templateUrl: './table-cate-criteria-page.component.html'
})

export class TableCriteriaModalComponent implements OnInit,
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
  IFilterView {
  paginator: PaginatorState;
  sorting: SortState;
  grouping: GroupingState;
  isLoading: boolean;
  filterGroup: FormGroup;
  searchGroup: FormGroup;
  options: Options;
  inputDataPersonData: Array<any> = [];
  inputDataPersonId: any = '00000000-0000-0000-0000-000000000000'
  private subscriptions: Subscription[] = []; // Read more: => https://brianflove.com/2016/12/11/anguar-2-unsubscribe-observables/
  //MinDate
  public minDate: string;

  //MaxDate
  public maxDate: string;
  constructor(
    private fb: FormBuilder,
    public cateCriteriaNumberSevenService: CateCriteriaNumberSevenService,
    private modalService: NgbModal,
    public commonService: CommonService,
    private http: HttpClient
  ) { }

  ngOnInit(): void {
    this.loadUser();
    this.filterForm();
    this.searchForm();
    this.cateCriteriaNumberSevenService.fetch();
    this.grouping = this.cateCriteriaNumberSevenService.grouping;
    this.paginator = this.cateCriteriaNumberSevenService.paginator;
    this.sorting = this.cateCriteriaNumberSevenService.sorting;
    const sb = this.cateCriteriaNumberSevenService.isLoading$.subscribe((res: any) => this.isLoading = res);
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

  loadUser() {
    this.commonService.getUser().subscribe((res: any) => {
      var userData = [
        {
          id: '00000000-0000-0000-0000-000000000000',
          text: 'Người tạo',
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

  ngOnDestroy() {
    this.subscriptions.forEach((sb) => sb.unsubscribe());
    this.cateCriteriaNumberSevenService.setDefaults();
  }

  change_value(event: any, ControlName: string) {
    this.filterGroup.controls[ControlName].setValue(event);
    this.filterGroup.updateValueAndValidity();
  }

  // filtration
  filterForm() {
    this.filterGroup = this.fb.group({
      minDate: [this.minDate],
      maxDate: [this.maxDate],
      InputDataPersonId: [this.inputDataPersonId]
    });
    this.subscriptions.push(
      this.filterGroup.controls.minDate.valueChanges.subscribe(() =>
        this.filter()
      )
    );
    this.subscriptions.push(
      this.filterGroup.controls.maxDate.valueChanges.subscribe(() => this.filter())
    );
    this.subscriptions.push(
      this.filterGroup.controls.InputDataPersonId.valueChanges.subscribe(() =>
        this.filter()
      )
    );
  }
  
  convert_date(string_date: string) {
    var result = moment.utc(string_date, "DD/MM/YYYY");
    return result
  }

  filter() {
    const filter: { [key: string]: string } = {};
    const minDate = this.filterGroup.controls['minDate'].value;
    const maxDate = this.filterGroup.controls['maxDate'].value;
    const inputDataPersonId = this.filterGroup.controls['InputDataPersonId'].value;

    if (minDate != null && minDate.length > 0) {
      filter['MinTime'] = (minDate);
    }
    if (maxDate != null && maxDate.length > 0) {
      filter['MaxTime'] = maxDate;
    }
    if (inputDataPersonId != '00000000-0000-0000-0000-000000000000') {
      filter['InputDataPersonId'] = inputDataPersonId;
    }
    this.cateCriteriaNumberSevenService.patchState({ filter });
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
    if(searchTerm.length == 0)
      this.cateCriteriaNumberSevenService.patchState({ searchTerm });
  }

  onEnter() {
    this.searchData(this.searchGroup.controls.searchTerm.value)
  }
  
  searchData(searchTerm: string) {
    this.cateCriteriaNumberSevenService.patchState({ searchTerm });
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
    this.cateCriteriaNumberSevenService.patchState({ sorting });
  }

  // pagination
  paginate(paginator: PaginatorState) {
    this.cateCriteriaNumberSevenService.patchState({ paginator });
  }

  // form actions
  create() {
    this.edit(undefined);
  }

  edit(id: any) {
    const modalRef = this.modalService.open(EditCateCriteriaModalComponent, { size: 'xl' });
    modalRef.componentInstance.id = id;
    modalRef.result.then(() =>
      this.cateCriteriaNumberSevenService.fetch(),
      () => { }
    );
  }

  view(id: any) {
    const modalRef = this.modalService.open(EditCateCriteriaModalComponent, { size: 'xl' });
    modalRef.componentInstance.id = id;
    modalRef.componentInstance.view = 'view';
    modalRef.result.then(() =>
      this.cateCriteriaNumberSevenService.fetch(),
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
        const sb = this.cateCriteriaNumberSevenService.deletebyId(id).pipe(
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

  exportFile() {
    const now = new Date();
    const timeString = now.toLocaleString('en-US', {
      day: '2-digit',
      month: '2-digit',
      year: 'numeric',
      hour: '2-digit',
      minute: '2-digit',
      second: '2-digit'
    }).replace(/\D/g, '');
    const fileName = "Tieuchiso7_" + timeString + ".xlsx"

    Swal.fire({
            icon: 'info',
      title: 'Đang xuất File...',
      // text: 'Vui lòng đợi một lúc trước khi file của bạn sẵn sàng!',
      didOpen: () => {
        Swal.showLoading()
      },
    })

    this.http.get(`${environment.apiUrl}/catecriterianumberseven/export`, {
      responseType: 'blob',
    }).pipe(
      catchError((errorMessage: any) => {
        console.error(errorMessage)
        Swal.fire({
          icon: 'error',
          title: 'Xuất File thất bại',
          confirmButtonText: 'Xác nhận',
        });
        return of();
      }),
    ).subscribe(
      (res) => {
        const file = new Blob([res], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' });
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
      },
    );
  }
}


