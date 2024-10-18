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
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import Swal from 'sweetalert2';

import { ReportIndexIndustryPageService } from '../_services/report-index-industry-page.service';
import { EditReportIndexIndustryModalComponent } from './components/edit-page-modal/edit-page-modal.component';
import * as moment from 'moment';
import { HttpClient } from '@angular/common/http';
import { environment } from 'src/environments/environment';
import { AuthService } from 'src/app/modules/auth';

@Component({
  selector: 'app-table-report-index-industry-page',
  templateUrl: './table-page.component.html',
  styleUrls: ['./table-page.component.scss']
})
export class TableReportIndexIndustryPageComponent implements OnInit,
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
  filerExport: any  = {}
  private subscriptions: Subscription[] = []; // Read more: => https://brianflove.com/2016/12/11/anguar-2-unsubscribe-observables/
  targetData: any = [
    {
      id: 0,
      text: 'Toàn ngành công nghiệp'
    },
    {
      id: 1,
      text: 'Khai khoáng'
    },
    {
      id: 2,
      text: 'Công nghiệp chế biến, chế tạo'
    },
    {
      id: 3,
      text: 'Sản xuất và phân phối điện, khí đốt, nước nóng, hơi nước và điều hòa không khí'
    },
    {
      id: 4,
      text: 'Cung cấp nước, hoạt động quản lý và xử lý rác thải, nước thải'
    }
  ]
  month: number = moment().month();
  year: number = moment().year();
  constructor(
    private fb: FormBuilder,
    public reportIndexIndustryPageService: ReportIndexIndustryPageService,
    private modalService: NgbModal,
    private http: HttpClient,
    private auth:AuthService
  ) { }

  ngOnInit(): void {
    this.filterForm();
    this.searchForm();
    this.reportIndexIndustryPageService.fetch();
    this.grouping = this.reportIndexIndustryPageService.grouping;
    this.paginator = this.reportIndexIndustryPageService.paginator;
    this.sorting = this.reportIndexIndustryPageService.sorting;
    const sb = this.reportIndexIndustryPageService.isLoading$.subscribe((res: any) => this.isLoading = res);
    this.subscriptions.push(sb);
  }

  ngOnDestroy() {
    this.subscriptions.forEach((sb) => sb.unsubscribe());
    this.reportIndexIndustryPageService.setDefaults();
  }

  filterForm() {
    const month = moment().month() < 10 ? `0${moment().month()}` : `${moment().month()}`
    this.filterGroup = this.fb.group({
      Month: [`${moment().year()}-${month}`],
    });
    this.subscriptions.push(
      this.filterGroup.controls.Month.valueChanges.subscribe(() => this.filter())
    );
  }

  filter() {
    const filter: { [key: string]: string } = {};
    const Month = this.filterGroup.controls['Month'].value;
    if (Month) {
      filter['Month'] = Month;
      this.month = Number(Month.split('-')[1])
      this.year = Number(Month.split('-')[0])
    } else{
      const month = moment().month() < 10 ? `0${moment().month()}` : `${moment().month()}`
      const value = `${moment().year()}-${month}`
      filter['Month'] = value;
      this.month = Number(Month.split('-')[1])
      this.year = Number(Month.split('-')[0])
    }
    this.filerExport = filter
    this.reportIndexIndustryPageService.patchState({ filter });
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
      this.reportIndexIndustryPageService.patchState({ searchTerm });
  }

  onEnter() {
    this.searchData(this.searchGroup.controls.searchTerm.value)
  }

  searchData(searchTerm: string) {
    this.reportIndexIndustryPageService.patchState({ searchTerm });
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
    this.reportIndexIndustryPageService.patchState({ sorting });
  }

  paginate(paginator: PaginatorState) {
    this.reportIndexIndustryPageService.patchState({ paginator });
  }

  create() {
    this.edit(undefined);
  }

  edit(item: any) {
    const modalRef = this.modalService.open(EditReportIndexIndustryModalComponent, { size: 'lg' });
    if (item) {
      modalRef.componentInstance.id = item.reportIndexIndustryId;
      modalRef.componentInstance.type = item.type;
      modalRef.componentInstance.itemData = item
    }
    modalRef.result.then(() =>
      this.reportIndexIndustryPageService.fetch(),
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
        const sb = this.reportIndexIndustryPageService.delete(id).pipe(
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

  updateStatusForSelected() {
  }

  fetchSelected() {
  }
  
  getTargetData(value: any){
    const data = this.targetData.filter((item: any) => item.id == value)
    if(data.length > 0){
      return data[0].text
    }
    return null;
  }
 
  exportFile() {
    const moment = require("moment");
    const timeString = moment().format("DDMMYYYYHHmmss");
    const fileName = "Quanlybaocaochisosanxuatcongnghiep_" + timeString + ".xlsx"

    Swal.fire({
            icon: 'info',
      title: 'Đang xuất File...',
      // text: 'Vui lòng đợi một lúc trước khi file của bạn sẵn sàng!',
      didOpen: () => {
        Swal.showLoading()
      },
    })
    const query = {
      filter: this.filerExport,
      grouping: {},
      paginator: {},
      sorting: {column: "id", direction: "desc"},
      searchTerm: this.searchGroup.controls.searchTerm.value
    } 
    const httpHeaders = this.auth.getHTTPHeaders();
    this.http.post(`${environment.apiUrl}/ReportIndexIndustry/Export`,query, {
      responseType: 'blob',
      headers: httpHeaders
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


