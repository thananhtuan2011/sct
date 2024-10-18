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
import { BusinessMultiLevelPageService } from '../_services/business-multi-level-page.service';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { EditBusinessMultiLevelModalComponent } from './components/edit-business-multi-level-modal/edit-modal.component';
import Swal from 'sweetalert2';
import { Options } from 'select2';
import { environment } from 'src/environments/environment';
import { HttpClient } from '@angular/common/http';
import * as moment from 'moment';
import { CommonService } from 'src/app/_metronic/shared/services/common.service';


@Component({
  selector: 'app-table-page',
  templateUrl: './table-page.component.html',
  styleUrls: ['./table-page.component.scss']
})
export class TableBusinessMultiLevelPageComponent implements OnInit,
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
  districtData: any = [];
  statusData: any = [];

  filterValue: any = {};
  private subscriptions: Subscription[] = []; // Read more: => https://brianflove.com/2016/12/11/anguar-2-unsubscribe-observables/

  constructor(
    private fb: FormBuilder,
    public businessMultiLevelPageService: BusinessMultiLevelPageService,
    private modalService: NgbModal,
    private http: HttpClient,
    private commonService: CommonService
  ) { }

  ngOnInit(): void {
    this.loadDistrict();
    this.loadStatus();
    this.filterForm();
    this.searchForm();
    this.getYearsList();
    this.businessMultiLevelPageService.fetch();
    this.grouping = this.businessMultiLevelPageService.grouping;
    this.paginator = this.businessMultiLevelPageService.paginator;
    this.sorting = this.businessMultiLevelPageService.sorting;
    const sb = this.businessMultiLevelPageService.isLoading$.subscribe((res: any) => this.isLoading = res);
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
    this.businessMultiLevelPageService.setDefaults();
  }

  // filtration
  filterForm() {
    this.filterGroup = this.fb.group({
      MinDate: [this.MinDate],
      MaxDate: [this.MaxDate],
      DistrictId: ["00000000-0000-0000-0000-000000000000"],
      Status: ["00000000-0000-0000-0000-000000000000"],
      
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
      this.filterGroup.controls.DistrictId.valueChanges.subscribe(() =>
        this.filter()
      )
    );
    this.subscriptions.push(
      this.filterGroup.controls.Status.valueChanges.subscribe(() =>
        this.filter()
      )
    );
  }

  filter() {
    const filter: { [key: string]: string } = {};
    const MinDate = this.filterGroup.controls['MinDate'].value;
    const MaxDate = this.filterGroup.controls['MaxDate'].value;
    const DistrictId = this.filterGroup.controls['DistrictId'].value;
    const Status = this.filterGroup.controls['Status'].value;
    if (MinDate) {
      filter['MinDate'] = MinDate;
    }
    if (MaxDate) {
      filter['MaxDate'] = MaxDate;
    }
    if (DistrictId && DistrictId != "00000000-0000-0000-0000-000000000000") {
      filter['DistrictId'] = DistrictId;
    }
    if (Status && Status != "00000000-0000-0000-0000-000000000000") {
      filter['Status'] = Status;
    }
    this.filterValue = filter;
    this.businessMultiLevelPageService.patchState({ filter });
  }

  resetFilter() {
    this.filterGroup.controls.MinDate.reset(this.MinDate);
    this.filterGroup.controls.MaxDate.reset(this.MaxDate);
    this.filterGroup.controls.Status.reset("00000000-0000-0000-0000-000000000000");
    this.filterGroup.controls.DistrictId.reset("00000000-0000-0000-0000-000000000000");
    this.businessMultiLevelPageService.fetch();
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
      this.businessMultiLevelPageService.patchState({ searchTerm });
  }

  onEnter() {
    this.searchData(this.searchGroup.controls.searchTerm.value)
  }
  
  searchData(searchTerm: string) {
    this.businessMultiLevelPageService.patchState({ searchTerm });
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
    this.businessMultiLevelPageService.patchState({ sorting });
  }

  // pagination
  paginate(paginator: PaginatorState) {
    this.businessMultiLevelPageService.patchState({ paginator });
  }

  // form actions
  create() {
    this.edit(0);
  }

  edit(id: any) {
    const modalRef = this.modalService.open(EditBusinessMultiLevelModalComponent, { size: 'xl' });
    modalRef.componentInstance.id = id;
    modalRef.componentInstance.statusData = this.statusData;
    modalRef.componentInstance.districtData = this.districtData;
    modalRef.result.then(() =>
      this.businessMultiLevelPageService.fetch(),
      () => { }
    );
  }

  view(id: any, type: any) {
    const modalRef = this.modalService.open(EditBusinessMultiLevelModalComponent, { size: 'xl' });
    modalRef.componentInstance.id = id;
    modalRef.componentInstance.type = type;
    modalRef.componentInstance.statusData = this.statusData;
    modalRef.componentInstance.districtData = this.districtData;
    modalRef.result.then(() =>
      this.businessMultiLevelPageService.fetch(),
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
        const sb = this.businessMultiLevelPageService.delete(id).pipe(
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
        const sb = this.businessMultiLevelPageService.deletes(this.grouping.getSelectedRows()).pipe(
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
    const fileName = "Danhsachcosobanhangdacap_" + timeString + ".xlsx"

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
    
    this.http.post(`${environment.apiUrl}/BusinessMultiLevel/export`, query, {
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
  
  loadDistrict(){
    const sb = this.commonService.getDistrict().subscribe((res: any) => {
      const data =[
        {
          id: '00000000-0000-0000-0000-000000000000',
          text: '-- Chọn --'
        },
        ...res.items.map((item: any) =>({
          id: item.districtId,
          text: item.districtName
        }))
      ]
      this.districtData = data;
    })
    
    this.subscriptions.push(sb);
  }
  
  loadStatus(){
    const sb = this.commonService.GetConfig('STATUS_BUSINESS_MULTI_LEVEL').subscribe((res: any) => {
      const data = [
        {
          id: '00000000-0000-0000-0000-000000000000',
          text: '-- Chọn --'
        },
        ... res.items.listConfig.map((item: any) => ({
          id: item.categoryId,
          text: item.categoryName
        }))
      ]
      
      this.statusData = data;
    })
    
    this.subscriptions.push(sb);
  }
  
  convert_date_string(string_date: string|null) {
    if(string_date == null){
      return null;
    }
    let date = string_date.split("T")[0];
    let list = date.split("-"); //["year", "month", "day"]
    let result = list[2] + "/" + list[1] + "/" + list[0]
    return result
  }
}


