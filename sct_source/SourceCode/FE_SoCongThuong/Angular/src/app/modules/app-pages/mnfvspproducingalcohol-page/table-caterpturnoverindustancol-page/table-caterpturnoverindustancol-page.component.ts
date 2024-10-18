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
import { CateRPTurnOverIndustAncolPageService } from '../_services/caterpturnoverindustancol-page.service';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { EditCateRPTurnOverIndustAncolModalComponent } from './components/edit-caterpturnoverindustancol-modal/edit-caterpturnoverindustancol-modal.component';
import Swal from 'sweetalert2';
import { Options } from 'select2';
import { environment } from 'src/environments/environment';
import { HttpClient } from '@angular/common/http';


@Component({
  selector: 'app-table-caterpturnoverindustancol-page',
  templateUrl: './table-caterpturnoverindustancol-page.component.html'
})
export class TableCateRPTurnOverIndustAncolPageComponent implements OnInit,
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

  TradeFormData: Array<any> = [
    {
      id: 0,
      text: '-- Hình thức --'
    },
    {
      id: 1,
      text: 'Buôn rượu'
    },
    {
      id: 2,
      text: 'Bán lẻ'
    }
  ];

  tradeFormId: any = 0;

  private subscriptions: Subscription[] = []; // Read more: => https://brianflove.com/2016/12/11/anguar-2-unsubscribe-observables/
  filterValue: { [key: string]: string; } = {};
  yearRange: any;

  constructor(
    private fb: FormBuilder,
    public caterpturnoverindustancolPageService: CateRPTurnOverIndustAncolPageService,
    private modalService: NgbModal,
    private http: HttpClient,
    ) {}

  ngOnInit(): void {
    this.filterForm();
    this.searchForm();
    this.getYearsList();
    this.caterpturnoverindustancolPageService.fetch();
    this.grouping = this.caterpturnoverindustancolPageService.grouping;
    this.paginator = this.caterpturnoverindustancolPageService.paginator;
    this.sorting = this.caterpturnoverindustancolPageService.sorting;
    const sb = this.caterpturnoverindustancolPageService.isLoading$.subscribe((res:any) => this.isLoading = res);
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
    this.caterpturnoverindustancolPageService.setDefaults();
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

  f_currency(value: any, args?: any): any {
    let nbr = Number((value + '').replace(/,|-/g, ''));
    return (nbr + '').replace(/(\d)(?=(\d{3})+(?!\d))/g, '$1,');
  }

  prenventInputNonNumber(event: any) {
    if (event.which < 48 || event.which > 57) {
      event.preventDefault();
    }
  }

  filterForm() {
    this.filterGroup = this.fb.group({
      YearReport: [""],
    });
    this.subscriptions.push(
      this.filterGroup.controls.YearReport.valueChanges.subscribe(() => this.filter())
    );
  }

  filter() {
    const filter: {[key:string]: string} = {};
    const YearReport = this.filterGroup.controls['YearReport'].value;
    if (YearReport != "") {
      filter['YearReport'] = YearReport;
    }
    this.filterValue = filter;
    this.caterpturnoverindustancolPageService.patchState({ filter });
  }

  resetFilter() {
    this.filterGroup.controls.YearReport.reset("");
    this.caterpturnoverindustancolPageService.fetch();
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
      this.caterpturnoverindustancolPageService.patchState({ searchTerm });
  }

  onEnter() {
    this.searchData(this.searchGroup.controls.searchTerm.value)
  }
  
  searchData(searchTerm: string) {
    this.caterpturnoverindustancolPageService.patchState({ searchTerm });
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
    this.caterpturnoverindustancolPageService.patchState({ sorting });
  }

  // pagination
  paginate(paginator: PaginatorState) {
    this.caterpturnoverindustancolPageService.patchState({ paginator });
  }

  // form actions
  create() {
    this.edit(undefined);
  }

  edit(id: any) {
    const modalRef = this.modalService.open(EditCateRPTurnOverIndustAncolModalComponent, { size: 'lg' });
    modalRef.componentInstance.id = id;
    modalRef.result.then(() =>
      this.caterpturnoverindustancolPageService.fetch(),
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
        const sb = this.caterpturnoverindustancolPageService.deleteCateRPTurnOverIndustAncol(id).pipe(
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
        const sb = this.caterpturnoverindustancolPageService.deleteCateRPTurnOverIndustAncols(this.grouping.getSelectedRows()).pipe(
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
    const fileName = "Baocaokinhdoanhruoucongnghiep_" + timeString + ".xlsx"

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
      title: 'Đang xuất File...',
      didOpen: () => {
        Swal.showLoading()
      },
    })


    this.http.post(`${environment.apiUrl}/CateRPTurnOverIndustAncol/Export`, query,
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


