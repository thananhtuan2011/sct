import { CommonService } from 'src/app/_metronic/shared/services/common.service';
import { PageInfoService, PageLink } from '../../../../_metronic/layout/core/page-info.service';
import { PaginatorState } from '../../../../_metronic/shared/crud-table/models/paginator.model';
import { ISearchView } from '../../../../_metronic/shared/crud-table/models/search.model';
import { IFilterView } from '../../../../_metronic/shared/crud-table/models/filter.model';
import { ISortView, SortState } from '../../../../_metronic/shared/crud-table/models/sort.model';
import { GroupingState, IGroupingView } from '../../../../_metronic/shared/crud-table/models/grouping.model';
import { IFetchSelectedAction, IUpdateStatusForSelectedAction } from '../../../../_metronic/shared/crud-table/models/table.model';
import { ChangeDetectorRef, Component, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { catchError, finalize, Observable, of, Subscription, tap, filter } from 'rxjs';
import { ReportAdministrativeProceduresPageService } from '../_services/report-administrative-procedures-page.service';
import { Options } from 'select2';
import { AuthService } from 'src/app/modules/auth/services/auth.service';
import { environment } from 'src/environments/environment';
import { HttpClient } from '@angular/common/http';
import Swal from 'sweetalert2';
import * as moment from 'moment';
import { animate, state, style, transition, trigger } from '@angular/animations';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { EditAdministrativeProceduresModalComponent } from '../../administrative-procedures-page/table-administrative-procedures-page/components/edit-administrative-procedures-modal/edit-modal.component';
import { EditReportAdministrativeProceduresModalComponent } from './components/edit-report-administrative-procedures-page/edit-modal.component';

@Component({
  selector: 'app-table-page',
  templateUrl: './table-page.component.html',
  styleUrls: ['./table-page.component.scss'],
})

export class TableReportAdministrativeProceduresPageComponent implements OnInit,
  OnDestroy,
  // IFetchSelectedAction,
  // IUpdateStatusForSelectedAction,
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
  YearData: any = []
  PeriodsData: any;

  Periods: string = "00000000-0000-0000-0000-000000000000";
  Year: string = moment().year().toString();
  filterBody: any = {}
  data: any;
  total: {
    'totalReceive': number,
    'onlineInPeriod': number,
    'offlineInPeriod': number,
    'fromPreviousPeriod': number,
    'totalProcessed': number,
    'onTimeProcessed': number,
    'outOfDateProcessed': number,
    'beforeDeadlineProcessed': number,
    'totalProcessing': number,
    'onTimeProcessing': number,
    'outOfDateProcessing': number,
  };
  options: Options;

  userLv: number = 0;
  userArea: string = "00000000-0000-0000-0000-000000000000"

  isNext: boolean = false;

  private subscriptions: Subscription[] = [];

  constructor(
    private fb: FormBuilder,
    public reportAdministrativeProceduresPageService: ReportAdministrativeProceduresPageService,
    public commonService: CommonService,
    private modalService: NgbModal,
    private http: HttpClient,
  ) { }

  ngOnInit(): void {
    this.filterForm();
    this.searchForm();
    this.loadPeriod();
    this.reportAdministrativeProceduresPageService.fetch();
    this.grouping = this.reportAdministrativeProceduresPageService.grouping;
    this.paginator = this.reportAdministrativeProceduresPageService.paginator;
    this.sorting = this.reportAdministrativeProceduresPageService.sorting;
    const sb = this.reportAdministrativeProceduresPageService.isLoading$.subscribe((res: any) => this.isLoading = res);
    this.subscriptions.push(sb);
    const items = this.reportAdministrativeProceduresPageService.items$.subscribe((res: any) => this.countTotal(res))
    this.subscriptions.push(items);

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

  loadPeriod() {
    this.commonService.GetConfig('REPORT_ADMINISTRATIVE_PROCEDURES_PERIOD').subscribe((res: any) => {
        const data = [
            ...res.items.listConfig.map((item: any) => ({
                id: item.categoryId,
                text: item.categoryName,
            }))
        ]
        this.PeriodsData = data;
        const currentMonth = moment().month();
        switch(true) {
          case (currentMonth > 0 && currentMonth < 4):
            this.Periods = this.PeriodsData[0].id;
            break;
          case (currentMonth > 3 && currentMonth < 7):
            this.Periods = this.PeriodsData[1].id;
            break;
          case (currentMonth > 7 && currentMonth < 10):
            this.Periods = this.PeriodsData[2].id;
            break;
          case (currentMonth > 10 && currentMonth < 13):
            this.Periods = this.PeriodsData[3].id;
            break;
        }
        this.filterGroup.controls.Periods.reset(this.Periods);
    })
  }

  ngOnDestroy() {
    this.subscriptions.forEach((sb) => sb.unsubscribe());
    this.reportAdministrativeProceduresPageService.setDefaults();
  }

  countTotal(item: []) {
    this.total = {
      totalReceive: 0,
      onlineInPeriod: 0,
      offlineInPeriod: 0,
      fromPreviousPeriod: 0,
      totalProcessed: 0,
      onTimeProcessed: 0,
      outOfDateProcessed: 0,
      beforeDeadlineProcessed: 0,
      totalProcessing: 0,
      onTimeProcessing: 0,
      outOfDateProcessing: 0,
    };
    if (item.length > 0) {
      this.total.totalReceive = item.reduce((x: any, y: any) => x + y.totalReceive, 0);
      this.total.onlineInPeriod = item.reduce((x: any, y: any) => x + y.onlineInPeriod, 0);
      this.total.offlineInPeriod = item.reduce((x: any, y: any) => x + y.offlineInPeriod, 0);
      this.total.fromPreviousPeriod = item.reduce((x: any, y: any) => x + y.fromPreviousPeriod, 0);
      this.total.totalProcessed = item.reduce((x: any, y: any) => x + y.totalProcessed, 0);
      this.total.onTimeProcessed = item.reduce((x: any, y: any) => x + y.onTimeProcessed, 0);
      this.total.outOfDateProcessed = item.reduce((x: any, y: any) => x + y.outOfDateProcessed, 0);
      this.total.beforeDeadlineProcessed = item.reduce((x: any, y: any) => x + y.beforeDeadlineProcessed, 0);
      this.total.totalProcessing = item.reduce((x: any, y: any) => x + y.totalProcessing, 0);
      this.total.onTimeProcessing = item.reduce((x: any, y: any) => x + y.onTimeProcessing, 0);
      this.total.outOfDateProcessing = item.reduce((x: any, y: any) => x + y.outOfDateProcessing, 0);
    }
  }

  // filtration
  filterForm() {
    this.filterGroup = this.fb.group({
      Periods: [this.Periods],
      Year: [this.Year],
    });
    this.filterGroup.controls.Periods.valueChanges.subscribe(() =>
      this.filter()
    )
    this.filterGroup.controls.Year.valueChanges.subscribe(() =>
      this.filter()
    )
  }

  filter() {
    const filter: { [key: string]: string } = {};
    const Period = this.filterGroup.controls['Periods'].value;
    const Year = this.filterGroup.controls['Year'].value;
    if (Period) {
      filter['Period'] = Period;
    }
    if (Year) {
      filter['Year'] = Year;
    }
    this.filterBody = { "filter": filter };
    this.reportAdministrativeProceduresPageService.patchState({ filter })
  }

  clearFilter() {
    this.filterGroup.controls.Year.setValue(this.Year);
    this.filterGroup.controls.Periods.setValue(this.Periods);
    this.filterGroup.updateValueAndValidity();
  }

  // search
  searchForm() {
    this.searchGroup = this.fb.group({
      searchTerm: [''],
    });
    this.searchGroup.controls.searchTerm.valueChanges.subscribe((res: string) => this.search(res))
  }

  search(searchTerm: string) {
    if (searchTerm.length == 0)
      this.reportAdministrativeProceduresPageService.patchState({ searchTerm });
  }

  onEnter() {
    this.searchData(this.searchGroup.controls.searchTerm.value)
  }

  searchData(searchTerm: string) {
    this.reportAdministrativeProceduresPageService.patchState({ searchTerm });
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
    this.reportAdministrativeProceduresPageService.patchState({ sorting });
  }

  // pagination
  paginate(paginator: PaginatorState) {
    this.reportAdministrativeProceduresPageService.patchState({ paginator });
  }

  getHeight(): any {
    let tmp_height = 0;
    tmp_height = window.innerHeight - 270;
    return tmp_height + 'px';
  }

  create() {
    this.edit(undefined)
  }

  edit(id: any) {
    const modalRef = this.modalService.open(EditReportAdministrativeProceduresModalComponent, { size: 'lg' });
    modalRef.componentInstance.id = id;
    modalRef.result.then(() =>
      this.reportAdministrativeProceduresPageService.fetch(),
      () => { }
    );
  }

  view(id: any) {
    const modalRef = this.modalService.open(EditReportAdministrativeProceduresModalComponent, { size: 'lg' });
    modalRef.componentInstance.id = id;
    modalRef.componentInstance.type = "view";
    modalRef.result.then(() =>
      this.reportAdministrativeProceduresPageService.fetch(),
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
        const sb = this.reportAdministrativeProceduresPageService.delete(id).pipe(
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

  exportFile() {
    const moment = require("moment");
    const timeString = moment().format("DDMMYYYYHHmmss");
    const fileName = "Baocaotinhhinhgiaiquyetthutuchanhchinh_" + timeString + ".xlsx"
    
    if (!this.filterBody.filter.Year) {
      Swal.fire({
        icon: 'warning',
        title: 'Hãy chọn năm báo cáo!',
        confirmButtonText: 'Xác nhận',
      });
      return
    }

    Swal.fire({
      icon: 'info',
      title: 'Đang xuất File...',
      // text: 'Vui lòng đợi một lúc trước khi file của bạn sẵn sàng!',
      didOpen: () => {
        Swal.showLoading()
      },
    })

    this.http.post(`${environment.apiUrl}/ReportAdministrativeProcedures/ExportExcel`, this.filterBody,
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


