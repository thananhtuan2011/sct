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
import { ManageArchiveRecordsPageService } from '../_services/manage-archive-records-page.service';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { EditManageArchiveRecordsModalComponent } from './components/edit-manage-archive-records-modal/edit-modal.component';
import Swal from 'sweetalert2';
import { Options } from 'select2';
import * as moment from 'moment';
import { environment } from 'src/environments/environment';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-table-page',
  templateUrl: './table-page.component.html'
})
export class TableManageArchiveRecordsPageComponent implements OnInit,
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
  options: Options
  yearData: any = []
  profileGroup: any = [
    {
      id: 0,
      text: "-- Chọn --"
    },
    {
      id: 1,
      text: "An toàn thực phẩm"
    },
    {
      id: 2,
      text: "Bảo vệ môi trường"
    },
    {
      id: 3,
      text: "An toàn hóa chất"
    },
    {
      id: 4,
      text: "Công tác phòng chống cháy nổ"
    },
    {
      id: 5,
      text: "Lĩnh vực kinh doanh khí"
    },
  ];
  private subscriptions: Subscription[] = []; // Read more: => https://brianflove.com/2016/12/11/anguar-2-unsubscribe-observables/
  filterValue: { [key: string]: string; } = {};

  constructor(
    private fb: FormBuilder,
    public pageService: ManageArchiveRecordsPageService,
    private modalService: NgbModal,
    private http: HttpClient,
  ) { }

  ngOnInit(): void {
    this.loadYear();
    this.searchForm();
    this.filterForm();
    this.pageService.fetch();
    this.grouping = this.pageService.grouping;
    this.paginator = this.pageService.paginator;
    this.sorting = this.pageService.sorting;
    const sb = this.pageService.isLoading$.subscribe((res: any) => this.isLoading = res);
    this.subscriptions.push(sb);
    this.options = {
      theme:'bootstrap5',
      templateSelection: this.templateSelection,
    };
  }
  
  public templateSelection = (state: any): JQuery | string => {
    if (!state.id) {
      return state.text;
    }
    return jQuery('<span class="form-select form-select-solid form-select-lg">'+ state.text + '</span>');
  }

  ngOnDestroy() {
    this.subscriptions.forEach((sb) => sb.unsubscribe());
    this.pageService.setDefaults();
  }

  loadYear(){
    const data = [
      {
        id: 0,
        text: "-- Chọn --"
      }
    ];
    for(let i = 0; i < 30; i++){
      let obj = {
        id: moment().year()- 15 + i,
        text: (moment().year()- 15 + i).toString()
      }
      data.push(obj);
    }
    this.yearData = data;
  }

  filterForm() {
    this.filterGroup = this.fb.group({
      RecordsFinancePlan: [0],
      Year: [moment().year()],
    });
    this.subscriptions.push(
      this.filterGroup.controls.RecordsFinancePlan.valueChanges.subscribe(() => this.filter())
    );
    this.subscriptions.push(
      this.filterGroup.controls.Year.valueChanges.subscribe(() => this.filter())
    );
  }

  filter() {
    const filter: { [key: string]: string } = {};
    const RecordsFinancePlan = this.filterGroup.controls['RecordsFinancePlan'].value;
    const Year = this.filterGroup.controls['Year'].value;
    if (RecordsFinancePlan != 0 ) {
      filter['RecordsFinancePlan'] = RecordsFinancePlan;
    }
    if (Year) {
      filter['Year'] = Year.toString();
    }

    this.filterValue = filter;
    this.pageService.patchState({ filter });
  }

  clearFilter(){
    this.filterGroup.controls.RecordsFinancePlan.setValue(0);
    this.filterGroup.controls.Year.setValue(moment().year());
    this.filter();
  }

  searchForm() {
    this.searchGroup = this.fb.group({
      searchTerm: [''],
    });
    const searchEvent = this.searchGroup.controls.searchTerm.valueChanges.subscribe((val) => this.search(val));
    this.subscriptions.push(searchEvent);
  }

  search(searchTerm: string) {
    if (searchTerm.length == 0)
      this.pageService.patchState({ searchTerm });
  }

  onEnter() {
    this.searchData(this.searchGroup.controls.searchTerm.value)
  }

  searchData(searchTerm: string) {
    this.pageService.patchState({ searchTerm });
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
    this.pageService.patchState({ sorting });
  }

  paginate(paginator: PaginatorState) {
    this.pageService.patchState({ paginator });
  }

  create() {
    this.edit(undefined);
  }

  edit(item: any) {
    const modalRef = this.modalService.open(EditManageArchiveRecordsModalComponent, { size: '100px' });
    if(item){
      modalRef.componentInstance.id = item.manageArchiveRecordsId;
    }
    modalRef.result.then(() =>
      this.pageService.fetch(),
      () => { }
    );
  }

  view(item: any) {
    const modalRef = this.modalService.open(EditManageArchiveRecordsModalComponent, { size: '100px' });
    if(item){
      modalRef.componentInstance.id = item.manageArchiveRecordsId;
      modalRef.componentInstance.view = item.manageArchiveRecordsId;
    }
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
              text: res.status == 1 ? 'Xóa thành công': res.status == 0 ? res.error.msg : 'Xóa thất bại',
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
    const fileName = "QuanLyHoSoLuuTru_" + timeString + ".xlsx"

    Swal.fire({
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

    this.http.post(`${environment.apiUrl}/ManageArchiveRecords/Export`, query,
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


