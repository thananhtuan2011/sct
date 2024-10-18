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
import { RuralDevelopmentPlanPageService } from '../_services/rural-development-plan-page.service';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { EditRuralDevelopmentPlanModalComponent } from './components/edit-rural-development-plan-modal/edit-modal.component';
import Swal from 'sweetalert2';
import { Options } from 'select2';
import { HttpClient } from '@angular/common/http';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-table-page',
  templateUrl: './table-page.component.html',
  styleUrls: ['./table-page.component.scss']
})

export class TableRuralDevelopmentPlanPageComponent implements OnInit,
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
  stageData: any[] = [];
  stageId: string;
  yearRange: any[] = [];
  options: Options;
  private subscriptions: Subscription[] = []; // Read more: => https://brianflove.com/2016/12/11/anguar-2-unsubscribe-observables/
  filterObj: { [key: string]: string; };

  constructor(
    private fb: FormBuilder,
    public ruralDevelopmentPlanPageService: RuralDevelopmentPlanPageService,
    private modalService: NgbModal,
    private http: HttpClient,
  ) { }

  ngOnInit(): void {
    this.filterForm();
    this.searchForm();
    this.grouping = this.ruralDevelopmentPlanPageService.grouping;
    this.paginator = this.ruralDevelopmentPlanPageService.paginator;
    this.sorting = this.ruralDevelopmentPlanPageService.sorting;
    const sb = this.ruralDevelopmentPlanPageService.isLoading$.subscribe((res: any) => this.isLoading = res);
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
    this.ruralDevelopmentPlanPageService.setDefaults();
  }

  loadStage() {
    this.ruralDevelopmentPlanPageService.loadStage().subscribe((res: any) => {
      var stages = []
      for (var item of res.items) {
        let stage = {
          id: item.stageId,
          text: item.stageName,
          start_year: item.startYear,
          end_year: item.endYear,
        }
        stages.push(stage)
      }
      this.stageData = stages
      this.stageId = this.stageData[1].id
      // this.ruralDevelopmentPlanPageService.fetch();
      this.filterGroup.controls.StageId.setValue(this.stageId);
      this.create_range(this.stageId);
    })
  }

  create_range(id: any) {
    const item = this.stageData.find((x: any) => x.id == id)
    this.yearRange = [];
    if (item.start_year != item.end_year) {
      for (var i = item.start_year; i <= item.end_year; i++) {
        this.yearRange.push(i)
      }
    }
  }

  get_value(item: any, year: any) {
    if (item.length > 0) {
      const result = item.find((x: any) => x.year == year)
      if (result) {
        return result.budget == null ? "" : this.f_currency(result.budget)
      }
      else {
        return ""
      }
    } else {
      return ""
    }
  }

  get_type(typeId: any) {
    if (typeId == 0) {
      return "Xây dựng"
    } else if (typeId == 1) {
      return "Nâng cấp"
    } else {
      return ""
    }
  }

  f_currency(value: any, args?: any): any {
    let nbr = Number((value + '').replace(/,|-/g, ''));
    return (nbr + '').replace(/(\d)(?=(\d{3})+(?!\d))/g, '$1,');
  }

  // filtration
  filterForm() {
    this.filterGroup = this.fb.group({
      StageId: [this.stageId]
    });
    this.loadStage();
    this.subscriptions.push(
      this.filterGroup.controls.StageId.valueChanges.subscribe((x: any) => {
        this.filter(),
          this.create_range(x)
      })
    );
  }

  filter() {
    const filter: { [key: string]: string } = {};
    const Stage = this.filterGroup.controls['StageId'].value;
    if (Stage) {
      filter['Stage'] = Stage;
    }
    this.filterObj = filter;
    this.ruralDevelopmentPlanPageService.patchState({ filter });
  }

  resetFilter() {
    this.filterGroup.controls.StageId.reset(this.stageId);
    this.ruralDevelopmentPlanPageService.fetch();
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
      this.ruralDevelopmentPlanPageService.patchState({ searchTerm });
  }

  onEnter() {
    this.searchData(this.searchGroup.controls.searchTerm.value)
  }

  searchData(searchTerm: string) {
    this.ruralDevelopmentPlanPageService.patchState({ searchTerm });
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
    this.ruralDevelopmentPlanPageService.patchState({ sorting });
  }

  // pagination
  paginate(paginator: PaginatorState) {
    this.ruralDevelopmentPlanPageService.patchState({ paginator });
  }

  // form actions
  create() {
    this.edit(undefined);
  }

  edit(id: any) {
    const modalRef = this.modalService.open(EditRuralDevelopmentPlanModalComponent, { size: 'lg' });
    modalRef.componentInstance.id = id;
    modalRef.result.then(() =>
      this.ruralDevelopmentPlanPageService.fetch(),
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
        const sb = this.ruralDevelopmentPlanPageService.delete(id).pipe(
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
        const sb = this.ruralDevelopmentPlanPageService.deletes(this.grouping.getSelectedRows()).pipe(
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
    const fileName = "KeHoachPhatTrienNongThon_" + timeString + ".xlsx"

    Swal.fire({
            icon: 'info',
      title: 'Đang xuất File...',
      // text: 'Vui lòng đợi một lúc trước khi file của bạn sẵn sàng!',
      didOpen: () => {
        Swal.showLoading()
      },
    })

    const query = {
      filter: this.filterObj,
      grouping: {},
      paginator: {},
      sorting: { column: "id", direction: "desc" },
      searchTerm: this.searchGroup.controls.searchTerm.value
    }

    this.http.post(`${environment.apiUrl}/RuralDevelopmentPlan/Export`, query, {
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


