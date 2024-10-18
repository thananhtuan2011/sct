import { PaginatorState } from '../../../../../../_metronic/shared/crud-table/models/paginator.model';
import { ISearchView } from '../../../../../../_metronic/shared/crud-table/models/search.model';
import { IFilterView } from '../../../../../../_metronic/shared/crud-table/models/filter.model';
import { ISortView, SortState } from '../../../../../../_metronic/shared/crud-table/models/sort.model';
import { GroupingState, IGroupingView } from '../../../../../../_metronic/shared/crud-table/models/grouping.model';
import { ICreateAction, IDeleteAction, IDeleteSelectedAction, IEditAction, IFetchSelectedAction, IUpdateStatusForSelectedAction } from '../../../../../../_metronic/shared/crud-table/models/table.model';
import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { catchError, finalize, of, Subscription, tap } from 'rxjs';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import Swal from 'sweetalert2';

import { MonthReportPageService } from '../../../_services/month-report-page.service';
import { EditMonthReportModalComponent } from '../.././components/edit-month-report-modal/edit-modal.component';
import { Options } from 'select2';
import { HttpClient } from '@angular/common/http';
import { environment } from 'src/environments/environment';
import { CommonService } from 'src/app/_metronic/shared/services/common.service';

@Component({
  selector: 'app-table-month-report-page',
  templateUrl: './table-page.component.html',
  styleUrls: ['./table-page.component.scss'],
})
export class TableMonthReportPageComponent implements OnInit,
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
  monthData: {id: any, text: string}[] = [];
  options: Options;
  filterValue: { [key: string]: string; } = {};

  constructor(
    private fb: FormBuilder,
    public pageService: MonthReportPageService,
    private commonService: CommonService,
    private modalService: NgbModal,
    private http: HttpClient,
    ) {}

  ngOnInit(): void {
    this.loadMonthRange();
    this.filterForm();
    this.searchForm();
    this.pageService.fetch();
    this.grouping = this.pageService.grouping;
    this.paginator = this.pageService.paginator;
    this.sorting = this.pageService.sorting;
    const sb = this.pageService.isLoading$.subscribe((res:any) => this.isLoading = res);
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

  loadMonthRange() {
    const data = [
      {
        id: "0",
        text: "-- Tháng --"
      },
    ]
    for (let i = 1; i < 13; i++) {
      let obj = {
        id: `${i}`,
        text: `Tháng ${i}`
      }
      data.push(obj)
    }
    this.monthData = data;
  }

  ngOnDestroy() {
    this.subscriptions.forEach((sb) => sb.unsubscribe());
    this.pageService.setDefaults();
  }

  filterForm() {
    this.filterGroup = this.fb.group({
      Year: [""],
      Month: ["0"],
    });
    this.subscriptions.push(
      this.filterGroup.controls.Year.valueChanges.subscribe(() => this.filter())
    );
    this.subscriptions.push(
      this.filterGroup.controls.Month.valueChanges.subscribe(() => this.filter())
    );
  }

  filter() {
    const filter: {[key:string]: string} = {};
    const Year = this.filterGroup.controls['Year'].value;
    if (Year != "") {
      filter['Year'] = Year;
    }
    const Month = this.filterGroup.controls['Month'].value;
    if (Month != "0") {
      filter['Month'] = Month;
    }
    this.filterValue = filter;
    this.pageService.patchState({ filter });
  }

  resetFilter() {
    this.filterGroup.controls.Month.reset("");
    this.pageService.fetch();
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
    if(searchTerm.length == 0)
      this.pageService.patchState({ searchTerm });
  }

  onEnter() {
    this.searchData(this.searchGroup.controls.searchTerm.value)
  }

  searchData(searchTerm: string) {
    this.pageService.patchState({ searchTerm });
  }

  sort(column: string) {
    const sorting = this.sorting;
    const isActiveColumn = sorting.column === column;
    if (!isActiveColumn) {
      sorting.column = column;
      sorting.direction = 'asc';
    } else {
      sorting.direction = sorting.direction === 'asc' ? 'desc' : 'asc';
    }
    this.pageService.patchState({ sorting });
  }

  paginate(paginator: PaginatorState) {
    this.pageService.patchState({ paginator });
  }

  create() {
    this.edit(undefined);
  }

  edit(id: any) {
    const modalRef = this.modalService.open(EditMonthReportModalComponent, { size: 'lg' });
    modalRef.componentInstance.id = id;
    modalRef.result.then(() =>
      this.pageService.fetch(),
      () => { }
    );
  }

  view(id: any) {
    const modalRef = this.modalService.open(EditMonthReportModalComponent, { size: 'lg' });
    modalRef.componentInstance.id = id;
    modalRef.componentInstance.type = "view";
    modalRef.result.then(() =>
      this.pageService.fetch(),
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
        const sb = this.pageService.delete(id).pipe(
          tap((res) => {
              Swal.fire({
                  icon: res.status == 1 ? 'success' : 'error',
                  title: res.status == 1 ? 'Thành công' : 'Thất bại',
                  confirmButtonText: 'Xác nhận',
                  text: res.status == 1 ? 'Xóa thành công' : res.status == 0 ? res.error.msg : 'Xóa thất bại',
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
  }

  updateStatusForSelected() {
  }

  fetchSelected() {
  }

  exportFile() {
    const moment = require("moment");
    const timeString = moment().format("DDMMYYYYHHmmss");
    const fileName = "Thongtinquanlyhoatdongdienluc_" + timeString + ".xlsx"
    Swal.fire({
      icon: 'info',
      title: 'Đang xuất File...',
      // text: 'Vui lòng đợi một lúc trước khi file của bạn sẵn sàng!',
      didOpen: () => {
        Swal.showLoading()
      },
    })
    const query = {
      filter: this.filterValue,
      grouping: {},
      paginator: {},
      sorting: { column: "id", direction: "desc" },
      searchTerm: this.searchGroup.controls.searchTerm.value
    }
    this.http.post(`${environment.apiUrl}/ManagementElectricityActivities/ExportExcel`, query,
    {
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
        Swal.fire({
          icon: 'success',
          title: 'Xuất File thành công',
          confirmButtonText: 'Xác nhận',
        });
      },
    );
  }
}


