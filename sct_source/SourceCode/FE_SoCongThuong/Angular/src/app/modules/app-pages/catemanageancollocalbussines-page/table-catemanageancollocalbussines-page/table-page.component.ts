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
import { CateManageAncolLocalBussinesPageService } from '../_services/catemanageancollocalbussines-page.service';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { EditCateManageAncolLocalBussinesModalComponent } from './components/edit-catemanageancollocalbussines-modal/edit-modal.component';
import Swal from 'sweetalert2';
import { environment } from 'src/environments/environment';
import { HttpClient } from '@angular/common/http';
import { Options } from 'select2';


@Component({
  selector: 'app-table-page',
  templateUrl: './table-page.component.html'
})

export class TableCateManageAncolLocalBussinesPageComponent implements OnInit,
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
  yearRange: any;
  filterValue: { [key: string]: string; } = {};
  options: Options;

  constructor(
    private fb: FormBuilder,
    public catemanageancollocalbussinesPageService: CateManageAncolLocalBussinesPageService,
    private modalService: NgbModal,
    private http: HttpClient,
    ) {}

  ngOnInit(): void {
    this.filterForm();
    this.searchForm();
    this.getYearsList();
    this.catemanageancollocalbussinesPageService.fetch();
    this.grouping = this.catemanageancollocalbussinesPageService.grouping;
    this.paginator = this.catemanageancollocalbussinesPageService.paginator;
    this.sorting = this.catemanageancollocalbussinesPageService.sorting;
    const sb = this.catemanageancollocalbussinesPageService.isLoading$.subscribe((res:any) => this.isLoading = res);
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

  f_currency(value: any, args?: any): any {
    let nbr = Number((value + '').replace(/,|-/g, ""));
    const result = (nbr + '').replace(/(\d)(?=(\d{3})+(?!\d))/g, "$1,");
    return result
  }

  ngOnDestroy() {
    this.subscriptions.forEach((sb) => sb.unsubscribe());
    this.catemanageancollocalbussinesPageService.setDefaults();
  }

  getYearsList() {
    const currentYear = new Date().getFullYear();
    const yearsList: any = [{ id: 0, text: "-- Chọn --" }];
    for (let i = -10; i <= 10; i++) {
      const year = currentYear + i;
      yearsList.push({ id: year, text: year });
    }
    this.yearRange = yearsList;
  }

  filterForm() {
    this.filterGroup = this.fb.group({
      YearReport: [0],
    });
    this.subscriptions.push(
      this.filterGroup.controls.YearReport.valueChanges.subscribe(() => this.filter())
    );
  }

  filter() {
    const filter: {[key:string]: string} = {};
    const YearReport = this.filterGroup.controls['YearReport'].value;
    if (YearReport > 0) {
      filter['YearReport'] = YearReport;
    }
    this.filterValue = filter;
    this.catemanageancollocalbussinesPageService.patchState({ filter });
  }

  resetFilter() {
    this.filterGroup.controls.YearReport.reset(0);
    this.catemanageancollocalbussinesPageService.fetch();
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
      this.catemanageancollocalbussinesPageService.patchState({ searchTerm });
  }

  onEnter() {
    this.searchData(this.searchGroup.controls.searchTerm.value)
  }

  searchData(searchTerm: string) {
    this.catemanageancollocalbussinesPageService.patchState({ searchTerm });
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
    this.catemanageancollocalbussinesPageService.patchState({ sorting });
  }

  // pagination
  paginate(paginator: PaginatorState) {
    this.catemanageancollocalbussinesPageService.patchState({ paginator });
  }

  // form actions
  create() {
    this.edit(0);
  }

  edit(id: any) {
    const modalRef = this.modalService.open(EditCateManageAncolLocalBussinesModalComponent, { size: 'lg' });
    modalRef.componentInstance.id = id;
    modalRef.result.then(() =>
      this.catemanageancollocalbussinesPageService.fetch(),
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
        const sb = this.catemanageancollocalbussinesPageService.deleteCateManageAncolLocalBussines(id).pipe(
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
        const sb = this.catemanageancollocalbussinesPageService.deletes(this.grouping.getSelectedRows()).pipe(
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
    const moment = require("moment");
    const timeString = moment().format("DDMMYYYYHHmmss");
    const fileName = "Danhsachdoanhnghiepsanxuatcongnghiep_" + timeString + ".xlsx"
    
    const query = {
      filter: this.filterValue,
      grouping: {},
      paginator: {},
      sorting: { column: "id", direction: "desc" },
      searchTerm: this.searchGroup.controls.searchTerm.value
    }
    if(!query.filter.YearReport){
      Swal.fire({
        icon: 'warning',
        title: "Vui lòng chọn năm báo cáo!",
        confirmButtonText: 'Xác nhận',
      });
    }else {
    Swal.fire({
      icon: 'info',
      title: 'Đang xuất File...',
      didOpen: () => {
        Swal.showLoading()
      },
    })


    this.http.post(`${environment.apiUrl}/CateManageAncolLocalBussines/Export`, query,
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
}


