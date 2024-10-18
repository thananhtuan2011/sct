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
import { CateInvestmentProjectPageService } from '../_services/cate-investment-project-page.service';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { EditCateInvestmentProjectModalComponent } from './components/edit-cate-investment-project-modal/edit-modal.component';
import Swal from 'sweetalert2';
import { environment } from 'src/environments/environment';
import { HttpClient } from '@angular/common/http';
import { CommonService } from 'src/app/_metronic/shared/services/common.service';
import { Options } from 'select2';

@Component({
  selector: 'app-table-page',
  templateUrl: './table-page.component.html'
})
export class TableCateInvestmentProjectPageComponent implements OnInit,
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
  districtData: any = [];
  options: Options;
  filterExport: any = {};
  
  private subscriptions: Subscription[] = []; // Read more: => https://brianflove.com/2016/12/11/anguar-2-unsubscribe-observables/

  constructor(
    private fb: FormBuilder,
    public cateInvestmentProjectPageService: CateInvestmentProjectPageService,
    private modalService: NgbModal,
    private http: HttpClient,
    private commonService: CommonService
    
  ) { }

  ngOnInit(): void {
    this.loadDistrict();
    this.filterForm();
    this.searchForm();
    this.cateInvestmentProjectPageService.fetch();
    this.grouping = this.cateInvestmentProjectPageService.grouping;
    this.paginator = this.cateInvestmentProjectPageService.paginator;
    this.sorting = this.cateInvestmentProjectPageService.sorting;
    const sb = this.cateInvestmentProjectPageService.isLoading$.subscribe((res: any) => this.isLoading = res);
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
    this.cateInvestmentProjectPageService.setDefaults();
  }

  // filtration
  filterForm() {
    this.filterGroup = this.fb.group({
      District: '00000000-0000-0000-0000-000000000000'
    });
    this.subscriptions.push(
      this.filterGroup.controls.District.valueChanges.subscribe(() =>
        this.filter()
      )
    );
  }

  filter() {
    const filter: { [key: string]: string } = {};
    const district = this.filterGroup.controls['District'].value;
    if (district != '00000000-0000-0000-0000-000000000000') {
      filter['District'] = district;
    }
    this.filterExport = filter;
    this.cateInvestmentProjectPageService.patchState({ filter });
  }

  // search
  searchForm() {
    this.searchGroup = this.fb.group({
      searchTerm: [''],
    });
    const searchEvent = this.searchGroup.controls.searchTerm.valueChanges.subscribe((val) => this.search(val));
    this.subscriptions.push(searchEvent);
  }

  search(searchTerm: string) {
    if (searchTerm.length == 0)
      this.cateInvestmentProjectPageService.patchState({ searchTerm });
  }

  onEnter() {
    this.searchData(this.searchGroup.controls.searchTerm.value)
  }

  searchData(searchTerm: string) {
    this.cateInvestmentProjectPageService.patchState({ searchTerm });
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
    this.cateInvestmentProjectPageService.patchState({ sorting });
  }

  // pagination
  paginate(paginator: PaginatorState) {
    this.cateInvestmentProjectPageService.patchState({ paginator });
  }

  // form actions
  create() {
    this.edit(0);
  }

  edit(id: any) {
    const modalRef = this.modalService.open(EditCateInvestmentProjectModalComponent, { size: 'lg' });
    modalRef.componentInstance.id = id;
    modalRef.componentInstance.districtData = this.districtData;
    modalRef.result.then(() =>
      this.cateInvestmentProjectPageService.fetch(),
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
        const sb = this.cateInvestmentProjectPageService.deleteCateInvestmentProject(id).pipe(
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
        const sb = this.cateInvestmentProjectPageService.deletes(this.grouping.getSelectedRows()).pipe(
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
    const fileName = "QuanLyDuAnDauTu_" + timeString + ".xlsx"

    Swal.fire({
            icon: 'info',
      title: 'Đang xuất File...',
      // text: 'Vui lòng đợi một lúc trước khi file của bạn sẵn sàng!',
      didOpen: () => {
        Swal.showLoading()
      },
    })

    const query = {
      filter: this.filterExport,
      grouping: {},
      paginator: {},
      sorting: { column: "id", direction: "desc" },
      searchTerm: this.searchGroup.controls.searchTerm.value
    }

    this.http.post(`${environment.apiUrl}/CateInvestmentProject/Export`, query,
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
  
  loadDistrict(){
    this.commonService.getDistrict().subscribe((res: any) => {
      const data = [{
        id: "00000000-0000-0000-0000-000000000000",
        text: '-- Chọn --'
      },
      ...res.items.map((item: any) => ({
        id: item.districtId,
        text: item.districtName
      }))]
      this.districtData = data;
    })
  }
  
  resetFilter(){
    this.filterGroup.controls.District.reset('00000000-0000-0000-0000-000000000000');
    this.cateInvestmentProjectPageService.fetch();
  }
}


