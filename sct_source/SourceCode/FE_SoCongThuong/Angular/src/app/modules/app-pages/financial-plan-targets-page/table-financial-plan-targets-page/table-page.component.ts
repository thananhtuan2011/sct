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

import { FinancialPlanTargetsPageService } from '../_services/financial-plan-targets-page.service';
import { EditFinancialPlanTargetsModalComponent } from './components/edit-financial-plan-targets-modal/edit-modal.component';
import { Options } from 'select2';
import { environment } from 'src/environments/environment';
import { HttpClient } from '@angular/common/http';
import * as moment from 'moment';

@Component({
  selector: 'app-table-page',
  templateUrl: './table-page.component.html',
  styleUrls: ['./table-page.component.scss']
})
export class TableFinancialPlanTargetsPageComponent implements OnInit,
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
  options: Options;
  dateGroup: FormGroup;
  filterGroup: FormGroup;
  searchGroup: FormGroup;
  show: boolean = false;
  firstLoad: boolean = true;
  private subscriptions: Subscription[] = []; // Read more: => https://brianflove.com/2016/12/11/anguar-2-unsubscribe-observables/
  Targets: { id: string, text: string }[] = [
    {
      id: "0",
      text: "-- Chọn --"
    },
    {
      id: "1",
      text: "Giá trị"
    },
    {
      id: "2",
      text: "Sản phẩm"
    },
    {
      id: "3",
      text: "Xuất khẩu"
    },
    {
      id: "4",
      text: "Nhập khẩu"
    },
  ]
  Date: string;
  filterValue: { [key: string]: string; } = {};

  constructor(
    private fb: FormBuilder,
    public financialPlanTargetsPageService: FinancialPlanTargetsPageService,
    private modalService: NgbModal,
    private http: HttpClient,
  ) { }

  ngOnInit(): void {
    this.filterForm();
    this.searchForm();
    this.dateForm();
    this.financialPlanTargetsPageService.fetch();
    this.grouping = this.financialPlanTargetsPageService.grouping;
    this.paginator = this.financialPlanTargetsPageService.paginator;
    this.sorting = this.financialPlanTargetsPageService.sorting;
    const sb = this.financialPlanTargetsPageService.isLoading$.subscribe((res: any) => this.isLoading = res);
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
    this.financialPlanTargetsPageService.setDefaults();
  }

  GetYearMonth(type: string) {
    if (!this.dateGroup.controls.Date.value) {
      return ""
    }
    const result = this.dateGroup.controls.Date.value.split("-"); // ["YYYY", "MM"]
    if (type == "Year"){
      return Number(result[0]);
    }
    if (type == "LastYear"){
      return Number(result[0]) - 1;
    }
    if (type == "Month"){
      return Number(result[1]);
    }
    if (type == "LastMonth"){
      if (result[1] == "1") {
        return 12
      }
      return Number(result[1]) - 1;
    }
  }

  filterForm() {
    this.filterGroup = this.fb.group({
      Target: ['0'],
      MinDate: [null],
      MaxDate: [null],
    });
    this.subscriptions.push(
      this.filterGroup.controls.Target.valueChanges.subscribe(() => this.filter())
    );
    this.subscriptions.push(
      this.filterGroup.controls.MinDate.valueChanges.subscribe(() => this.filter())
    );
    this.subscriptions.push(
      this.filterGroup.controls.MaxDate.valueChanges.subscribe(() => this.filter())
    );
  }

  dateForm() {
    this.Date = moment().month() < 10 ? `0${moment().month()}` : `${moment().month()}`
    this.dateGroup = this.fb.group({
      Date: [this.Date]
    });

    this.subscriptions.push(
      this.dateGroup.controls.Date.valueChanges.subscribe(() => this.filter())
    );

    const item = this.financialPlanTargetsPageService.items$.subscribe((res: any) => {
      if (res.length > 0 && this.firstLoad) {
        this.dateGroup.controls.Date.reset(res[0].date);
        this.firstLoad = false;
      } 
    })
    this.subscriptions.push(item);
    
    this.show = true;
  }

  filter() {
    const filter: { [key: string]: string } = {};
    const Target = this.filterGroup.controls['Target'].value;
    const MinDate = this.filterGroup.controls['MinDate'].value;
    const MaxDate = this.filterGroup.controls['MaxDate'].value;
    const Date = this.dateGroup.controls['Date'].value;
    if (Target && Target != "0") {
      filter['Target'] = Target;
    }
    if (MinDate) {
      filter['MinDate'] = MinDate;
    }
    if (MaxDate) {
      filter['MaxDate'] = MaxDate;
    }
    if (Date) {
      filter['Date'] = Date;
    }
    this.filterValue = filter;
    this.financialPlanTargetsPageService.patchState({ filter });
  }
  
  resetFilter() {
    this.filterGroup.controls.Target.reset("0");
    this.filterGroup.controls.MinDate.reset(null);
    this.filterGroup.controls.MaxDate.reset(null);
    this.financialPlanTargetsPageService.fetch();
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
      this.financialPlanTargetsPageService.patchState({ searchTerm });
  }

  onBackspace(event: any) {
    if (event.target.value == "") {
      this.searchData("")
    }
  }

  onEnter() {
    this.searchData(this.searchGroup.controls.searchTerm.value)
  }

  searchData(searchTerm: string) {
    this.financialPlanTargetsPageService.patchState({ searchTerm });
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
    this.financialPlanTargetsPageService.patchState({ sorting });
  }

  paginate(paginator: PaginatorState) {
    this.financialPlanTargetsPageService.patchState({ paginator });
  }

  create() {
    this.edit(undefined);
  }

  edit(item: any) {
    const modalRef = this.modalService.open(EditFinancialPlanTargetsModalComponent, { size: 'lg' });
    if (item) {
      modalRef.componentInstance.id = item.financialPlanTargetsId;
      modalRef.componentInstance.type = item.type;
    }
    modalRef.result.then(() =>
      this.financialPlanTargetsPageService.fetch(),
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
        const sb = this.financialPlanTargetsPageService.delete(id).pipe(
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

  exportFile() {
    const moment = require("moment");
    const timeString = moment().format("DDMMYYYYHHmmss");
    const fileName = "Chitieusanxuatkinhdoanhxuatkhauchuyeu_" + timeString + ".xlsx"

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
    
    this.http.post(`${environment.apiUrl}/FinancialPlanTarget/ExportExcel`, query, {
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


