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
import { ExportGoodsPageService } from '../_services/exportgoods-page.service';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { EditExportGoodsModalComponent } from './components/edit-exportgoods-modal/edit-exportgoods-modal.component';
import Swal from 'sweetalert2';
import { environment } from 'src/environments/environment';
import { HttpClient } from '@angular/common/http';
import { Options } from 'select2';

@Component({
  selector: 'app-table-exportgoods-page',
  templateUrl: './table-exportgoods-page.component.html'
})

export class TableExportGoodsPageComponent implements OnInit,
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
  yearRange: any = [
    {
      id: 0,
      text: "-- Chọn --"
    }
  ]
  monthRange: any = [
    {
      id: 0,
      text: "-- Chọn --"
    },
    {
      id: 1,
      text: "Tháng 1"
    },
    {
      id: 2,
      text: "Tháng 2"
    },
    {
      id: 3,
      text: "Tháng 3"
    },
    {
      id: 4,
      text: "Tháng 4"
    },
    {
      id: 5,
      text: "Tháng 5"
    },
    {
      id: 6,
      text: "Tháng 6"
    },
    {
      id: 7,
      text: "Tháng 7"
    },
    {
      id: 8,
      text: "Tháng 8"
    },
    {
      id: 9,
      text: "Tháng 9"
    },
    {
      id: 10,
      text: "Tháng 10"
    },
    {
      id: 11,
      text: "Tháng 11"
    },
    {
      id: 12,
      text: "Tháng 12"
    },
  ];
  options: Options;

  private subscriptions: Subscription[] = []; // Read more: => https://brianflove.com/2016/12/11/anguar-2-unsubscribe-observables/
  filterValue: { [key: string]: string; } = {};

  constructor(
    private fb: FormBuilder,
    public exportgoodsPageService: ExportGoodsPageService,
    private modalService: NgbModal,
    private http: HttpClient,
  ) { }

  ngOnInit(): void {
    this.filterForm();
    this.searchForm();
    this.exportgoodsPageService.fetch();
    this.grouping = this.exportgoodsPageService.grouping;
    this.paginator = this.exportgoodsPageService.paginator;
    this.sorting = this.exportgoodsPageService.sorting;
    const sb = this.exportgoodsPageService.isLoading$.subscribe((res: any) => this.isLoading = res);
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
    this.exportgoodsPageService.setDefaults();
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
    this.getYearsList();
    this.filterGroup = this.fb.group({
      Month: ["0"],
      Year: ["0"],
    });
    this.subscriptions.push(
      this.filterGroup.controls.Month.valueChanges.subscribe(() => this.filter())
    );
    this.subscriptions.push(
      this.filterGroup.controls.Year.valueChanges.subscribe(() => this.filter())
    );
  }

  filter() {
    const filter: {[key:string]: string} = {};
    const Month = this.filterGroup.controls['Month'].value;
    if (Month != "0") {
      filter['Month'] = Month;
    }
    const Year = this.filterGroup.controls['Year'].value;
    if (Year != "0") {
      filter['Year'] = Year;
    }
    this.filterValue = filter;
    this.exportgoodsPageService.patchState({ filter });
  }

  resetFilter() {
    this.filterGroup.controls.Month.reset("0");
    this.filterGroup.controls.Year.reset("0");
    this.exportgoodsPageService.fetch();
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
      this.exportgoodsPageService.patchState({ searchTerm });
  }

  onEnter() {
    this.searchData(this.searchGroup.controls.searchTerm.value)
  }

  searchData(searchTerm: string) {
    this.exportgoodsPageService.patchState({ searchTerm });
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
    this.exportgoodsPageService.patchState({ sorting });
  }

  // pagination
  paginate(paginator: PaginatorState) {
    this.exportgoodsPageService.patchState({ paginator });
  }

  // form actions
  create() {
    this.edit(0);
  }

  edit(id: any) {
    const modalRef = this.modalService.open(EditExportGoodsModalComponent, { size: '100px' });
    modalRef.componentInstance.id = id;
    modalRef.result.then(() =>
      this.exportgoodsPageService.fetch(),
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
        const sb = this.exportgoodsPageService.deleteExportGood(id).pipe(
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
        const sb = this.exportgoodsPageService.deleteExportGoods(this.grouping.getSelectedRows()).pipe(
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
    const fileName = "QuanLyXuatKhau_" + timeString + ".xlsx"
    const query = {
      filter: this.filterValue,
      grouping: {},
      paginator: {},
      sorting: { column: "id", direction: "desc" },
      searchTerm: this.searchGroup.controls.searchTerm.value
    }
    if(!query.filter.Month || !query.filter.Year){
      let str = "";
      if(!query.filter.Month && !query.filter.Year)
      {
        str = "Vui lòng chọn tháng, năm báo cáo!"
      }else if(!query.filter.Month)
      {
        str = "Vui lòng chọn tháng báo cáo!"
      }else{
        str = "Vui lòng chọn năm báo cáo!"
      }
      Swal.fire({
        icon: 'warning',
        title: str,
        confirmButtonText: 'Xác nhận',
      });
    }else {
    Swal.fire({
      icon: 'info',
      title: 'Đang xuất File...',
      // text: 'Vui lòng đợi một lúc trước khi file của bạn sẵn sàng!',
      didOpen: () => {
        Swal.showLoading()
      },
    })


    this.http.post(`${environment.apiUrl}/ExportGoods/Export`, query, {
      responseType: 'blob',
    })
      .pipe(
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
  
  f_currency(value: any, args?: any): any {
    let nbr = Number((value + '').replace(/,|-/g, ""));
    const result = (nbr + '').replace(/(\d)(?=(\d{3})+(?!\d))/g, "$1,");
    return result
  }

}


