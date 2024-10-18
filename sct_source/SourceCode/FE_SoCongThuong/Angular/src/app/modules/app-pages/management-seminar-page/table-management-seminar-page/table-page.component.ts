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
import { ManagementSeminarService } from '../_services/management-seminar-page.service';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { EditManagementSeminarModalComponent } from './components/management-seminar-modal/edit-modal.component';
import Swal from 'sweetalert2';
import { environment } from 'src/environments/environment';
import { HttpClient } from '@angular/common/http';
import { Options } from 'select2';
import { CommonService } from 'src/app/_metronic/shared/services/common.service';


@Component({
  selector: 'app-table-page',
  templateUrl: './table-page.component.html'
})
export class TableManagementSeminarPageComponent implements OnInit,
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
  dataDistrict: any = [];
  filterValue: any = {}
  typeBusiness: any = [
    {
      id: 0,
      text: '-- Tất cả --'
    },
    {
      id: 1,
      text: 'Trong tỉnh'
    },
    {
      id: 2,
      text: 'Ngoài tỉnh'
    }
  ]
  private subscriptions: Subscription[] = []; // Read more: => https://brianflove.com/2016/12/11/anguar-2-unsubscribe-observables/

  constructor(
    private fb: FormBuilder,
    public managementSeminarPageService: ManagementSeminarService,
    private modalService: NgbModal,
    private http: HttpClient,
    private commonService: CommonService
  ) { }

  ngOnInit(): void {
    this.loadDistrict();
    this.filterForm();
    this.searchForm();
    this.managementSeminarPageService.fetch();
    this.grouping = this.managementSeminarPageService.grouping;
    this.paginator = this.managementSeminarPageService.paginator;
    this.sorting = this.managementSeminarPageService.sorting;
    const sb = this.managementSeminarPageService.isLoading$.subscribe((res: any) => this.isLoading = res);
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
    this.managementSeminarPageService.setDefaults();
  }

  // filtration
  filterForm() {
    this.filterGroup = this.fb.group({
      MinDate: null,
      MaxDate: null,
      DistrictId: '00000000-0000-0000-0000-000000000000'
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

  }

  filter() {
    const filter: { [key: string]: string } = {};
    const minDate = this.filterGroup.controls['MinDate'].value;
    const maxDate = this.filterGroup.controls['MaxDate'].value;
    const district = this.filterGroup.controls['DistrictId'].value;
    if (minDate && minDate !== null) {
      filter['MinDate'] = minDate;
    }
    
    if(maxDate && maxDate !== null){
      filter['MaxDate'] = maxDate;
    }
    
    if(district && district != '00000000-0000-0000-0000-000000000000'){
      filter['District'] = district;
    }
    this.filterValue = filter;
    this.managementSeminarPageService.patchState({ filter });
  }
  
  clearFilter(){
    this.filterGroup.controls.MinDate.setValue(null);
    this.filterGroup.controls.MaxDate.setValue(null);
    this.filterGroup.controls.DistrictId.setValue('00000000-0000-0000-0000-000000000000');
    
    this.filter();
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
      this.managementSeminarPageService.patchState({ searchTerm });
  }

  onEnter() {
    this.searchData(this.searchGroup.controls.searchTerm.value)
  }

  searchData(searchTerm: string) {
    this.managementSeminarPageService.patchState({ searchTerm });
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
    this.managementSeminarPageService.patchState({ sorting });
  }

  // pagination
  paginate(paginator: PaginatorState) {
    this.managementSeminarPageService.patchState({ paginator });
  }

  // form actions
  create() {
    this.edit(0);
  }

  edit(id: any) {
    const modalRef = this.modalService.open(EditManagementSeminarModalComponent, { size: 'lg' });
    modalRef.componentInstance.id = id;
    modalRef.componentInstance.dataDistrict = this.dataDistrict;
    modalRef.result.then(() =>
      this.managementSeminarPageService.fetch(),
      () => { }
    );
  }

  view(id: any) {
    const modalRef = this.modalService.open(EditManagementSeminarModalComponent, { size: 'lg' });
    modalRef.componentInstance.id = id;
    modalRef.componentInstance.type = "view";
    modalRef.componentInstance.dataDistrict = this.dataDistrict;
    modalRef.result.then(() =>
      this.managementSeminarPageService.fetch(),
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
        const sb = this.managementSeminarPageService.delete(id).pipe(
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
        const sb = this.managementSeminarPageService.deletes(this.grouping.getSelectedRows()).pipe(
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
    const fileName = "TheoDoiThongbaoToChuHoiNghiHoiThaoDaoTaoBHDC_" + timeString + ".xlsx"

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
    
    this.http.post(`${environment.apiUrl}/ManagementSeminar/export`, query, {
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
    this.commonService.getDistrict().subscribe((res: any) => {
      const distrcits = [
        {
          id: '00000000-0000-0000-0000-000000000000',
          text: '-- Chọn --',
        },
      ]
      for (var item of res.items) {
        let objDistrict = {
          id: item.districtId,
          text: item.districtName,
        }
        distrcits.push(objDistrict)
      }
      this.dataDistrict = distrcits.sort((i1, i2) => {
        if (i1.text > i2.text) {
          return 1;
        }
        if (i1.text < i2.text) {
          return -1;
        }
        return 0;
      });
    })
  }
}


