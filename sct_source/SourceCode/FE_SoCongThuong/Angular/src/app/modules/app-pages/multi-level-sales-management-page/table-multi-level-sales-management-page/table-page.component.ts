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
import { MultiLevelSalesManagementPageService } from '../_services/multi-level-sales-management-page.service';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { EditMultiLevelSalesManagementModalComponent } from './components/edit-multi-level-sales-management-modal/edit-modal.component';
import Swal from 'sweetalert2';
import { Options } from 'select2';
import { environment } from 'src/environments/environment';
import { HttpClient } from '@angular/common/http';
import * as moment from 'moment';


@Component({
  selector: 'app-table-page',
  templateUrl: './table-page.component.html',
  styleUrls: ['./table-page.component.scss']
})
export class TableMultiLevelSalesManagementPageComponent implements OnInit,
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

  MinDate: any = null;
  MaxDate: any = null;
  YearData: any = [];
  listBusinessMultiLevel: any = [];

  filterValue: any = {};
  private subscriptions: Subscription[] = []; // Read more: => https://brianflove.com/2016/12/11/anguar-2-unsubscribe-observables/

  constructor(
    private fb: FormBuilder,
    public multiLevelSalesManagementPageService: MultiLevelSalesManagementPageService,
    private modalService: NgbModal,
    private http: HttpClient,
  ) { }

  ngOnInit(): void {
    this.loadListBusinessMultiLevel();
    this.filterForm();
    this.searchForm();
    this.getYearsList();
    this.multiLevelSalesManagementPageService.fetch();
    this.grouping = this.multiLevelSalesManagementPageService.grouping;
    this.paginator = this.multiLevelSalesManagementPageService.paginator;
    this.sorting = this.multiLevelSalesManagementPageService.sorting;
    const sb = this.multiLevelSalesManagementPageService.isLoading$.subscribe((res: any) => this.isLoading = res);
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

  getYearsList() {
    const currentYear = new Date().getFullYear();
    const yearsList: any = [{id: 0, text:"-- Chọn --"}];
  
    for (let i = -10; i <= 10; i++) {
      const year = currentYear + i;
      yearsList.push({id: year, text: year});
    }
  
    this.YearData = yearsList;
  }

  ngOnDestroy() {
    this.subscriptions.forEach((sb) => sb.unsubscribe());
    this.multiLevelSalesManagementPageService.setDefaults();
  }

  // filtration
  filterForm() {
    this.filterGroup = this.fb.group({
      MinDate: [this.MinDate],
      MaxDate: [this.MaxDate],
      Year: [""],
    });
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
      this.filterGroup.controls.Year.valueChanges.subscribe(() =>
        this.filter()
      )
    );
  }

  filter() {
    const filter: { [key: string]: string } = {};
    const MinDate = this.filterGroup.controls['MinDate'].value;
    const MaxDate = this.filterGroup.controls['MaxDate'].value;
    const Year = this.filterGroup.controls['Year'].value;

    if (MinDate) {
      filter['MinDate'] = MinDate;
    }
    if (MaxDate) {
      filter['MaxDate'] = MaxDate;
    }
    if (Year != "") {
      filter['YearReport'] = Year;
    }
    this.filterValue = filter;
    this.multiLevelSalesManagementPageService.patchState({ filter });
  }

  resetFilter() {
    this.filterGroup.controls.MinDate.reset(this.MinDate);
    this.filterGroup.controls.MaxDate.reset(this.MaxDate);
    this.filterGroup.controls.Year.reset("");
    this.multiLevelSalesManagementPageService.fetch();
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
      this.multiLevelSalesManagementPageService.patchState({ searchTerm });
  }

  onEnter() {
    this.searchData(this.searchGroup.controls.searchTerm.value)
  }
  
  searchData(searchTerm: string) {
    this.multiLevelSalesManagementPageService.patchState({ searchTerm });
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
    this.multiLevelSalesManagementPageService.patchState({ sorting });
  }

  // pagination
  paginate(paginator: PaginatorState) {
    this.multiLevelSalesManagementPageService.patchState({ paginator });
  }

  // form actions
  create() {
    this.edit(0);
  }

  edit(id: any) {
    const modalRef = this.modalService.open(EditMultiLevelSalesManagementModalComponent, { size: 'xl' });
    modalRef.componentInstance.id = id;
    modalRef.componentInstance.listBusinessMultiLevel = this.listBusinessMultiLevel;
    modalRef.result.then(() =>
      this.multiLevelSalesManagementPageService.fetch(),
      () => { }
    );
  }

  view(id: any, type: any) {
    const modalRef = this.modalService.open(EditMultiLevelSalesManagementModalComponent, { size: 'xl' });
    modalRef.componentInstance.id = id;
    modalRef.componentInstance.type = type;
    modalRef.componentInstance.listBusinessMultiLevel = this.listBusinessMultiLevel;
    modalRef.result.then(() =>
      this.multiLevelSalesManagementPageService.fetch(),
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
        const sb = this.multiLevelSalesManagementPageService.delete(id).pipe(
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
        const sb = this.multiLevelSalesManagementPageService.deletes(this.grouping.getSelectedRows()).pipe(
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
    const fileName = "Danhsachcosohoatdongbanhangdacap_" + timeString + ".xlsx"

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
    
    this.http.post(`${environment.apiUrl}/MultiLevelSalesManagement/export`, query, {
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
  
  loadListBusinessMultiLevel(){
    const sb = this.multiLevelSalesManagementPageService.getBusinessMultiLevel().subscribe((res: any) => {
      const data = [
        {
          id: '00000000-0000-0000-0000-000000000000',
          text: '-- Chọn -- ',
          startDate: null,
          numCert: '',
          goods: '',
          contact: '',
          phoneNumber: '',
          contactAddress: '',
          address: ''
        },
        ...res.items.map((item: any) => ({
          id: item.businessMultiLevelId,
          text: item.businessName,
          startDate: item.startDate,
          numCert: item.numCert,
          goods: item.goods,
          contact: item.contact,
          phoneNumber: item.phoneNumber,
          contactAddress: item.addressContact,
          address: item.address
        }))
      ]
      this.listBusinessMultiLevel = data;
    })
    this.subscriptions.push(sb);
  }
}


