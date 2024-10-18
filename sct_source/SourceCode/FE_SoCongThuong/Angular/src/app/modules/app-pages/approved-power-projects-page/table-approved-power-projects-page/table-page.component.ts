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
import { ApprovedPowerProjectsPageService } from '../_services/approved-power-projects-page.service';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { EditApprovedPowerProjectsModalComponent } from './components/edit-approved-power-projects-modal/edit-modal.component';
import Swal from 'sweetalert2';
import { Options } from 'select2';
import { environment } from 'src/environments/environment';
import { HttpClient } from '@angular/common/http';
import { CommonService } from 'src/app/_metronic/shared/services/common.service';


@Component({
  selector: 'app-table-page',
  templateUrl: './table-page.component.html'
})
export class TableApprovedPowerProjectsPageComponent implements OnInit,
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
  options: Options
  filterValue: { [key: string]: string; } = {};
  energyIndustryData: any[];
  statusData: any[];
  private subscriptions: Subscription[] = []; // Read more: => https://brianflove.com/2016/12/11/anguar-2-unsubscribe-observables/

  constructor(
    private fb: FormBuilder,
    public approvedPowerProjectsPageService: ApprovedPowerProjectsPageService,
    private modalService: NgbModal,
    private http: HttpClient,
    private commonService: CommonService,
  ) { }

  ngOnInit(): void {
    this.loadEnergyIndustry();
    this.loadStatus();
    this.filterForm();
    this.searchForm();
    this.approvedPowerProjectsPageService.fetch();
    this.grouping = this.approvedPowerProjectsPageService.grouping;
    this.paginator = this.approvedPowerProjectsPageService.paginator;
    this.sorting = this.approvedPowerProjectsPageService.sorting;
    const sb = this.approvedPowerProjectsPageService.isLoading$.subscribe((res: any) => this.isLoading = res);
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
    this.approvedPowerProjectsPageService.setDefaults();
  }

  loadEnergyIndustry() {
    const sb = this.commonService.getTypeOfEnergy().subscribe((res: any) => {
      const data = [
        { id: "00000000-0000-0000-0000-000000000000", text: "-- Chọn --" },
        ...res.items.map((item: any) => ({
          id: item.typeOfEnergyId,
          text: item.typeOfEnergyName,
        }))
      ]
      this.energyIndustryData = data;
    })
    this.subscriptions.push(sb);
  }

  loadStatus() {
    const sb = this.commonService.GetConfig('APPROVED_POWER_PROJECTS_STATUS').subscribe((res: any) => {
      const data = [
        { id: "00000000-0000-0000-0000-000000000000", text: "-- Chọn --" },
        ...res.items.listConfig.map((item: any) => ({
          id: item.categoryId,
          text: item.categoryName,
        }))
      ]
      this.statusData = data;
    })
    this.subscriptions.push(sb);
  }

  // filtration
  filterForm() {
    this.filterGroup = this.fb.group({
      EnergyIndustryId: ['00000000-0000-0000-0000-000000000000'],
      StatusId: ['00000000-0000-0000-0000-000000000000'],
      Year: [''],
      PowerOutput: [''],
    });
    this.subscriptions.push(
      this.filterGroup.controls.EnergyIndustryId.valueChanges.subscribe(() =>
        this.filter()
      )
    );
    this.subscriptions.push(
      this.filterGroup.controls.StatusId.valueChanges.subscribe(() =>
        this.filter()
      )
    );
    this.subscriptions.push(
      this.filterGroup.controls.Year.valueChanges.subscribe(() =>
        this.filter()
      )
    );
    this.subscriptions.push(
      this.filterGroup.controls.PowerOutput.valueChanges.subscribe(() =>
        this.filter()
      )
    );
  }

  filter() {
    const filter: { [key: string]: string } = {};
    const EnergyIndustryId = this.filterGroup.controls['EnergyIndustryId'].value;
    if (EnergyIndustryId && EnergyIndustryId != "00000000-0000-0000-0000-000000000000") {
      filter['EnergyIndustryId'] = EnergyIndustryId;
    }
    const StatusId = this.filterGroup.controls['StatusId'].value;
    if (StatusId && StatusId != '00000000-0000-0000-0000-000000000000') {
      filter['StatusId'] = StatusId;
    }
    const Year = this.filterGroup.controls['Year'].value;
    if (Year) {
      filter['Year'] = Year;
    }
    const PowerOutput = this.filterGroup.controls['PowerOutput'].value;
    if (PowerOutput) {
      filter['PowerOutput'] = PowerOutput + "";
    }
    this.filterValue = filter;
    this.approvedPowerProjectsPageService.patchState({ filter });
  }

  resetFilter() {
    this.filterGroup.controls.EnergyIndustryId.reset('00000000-0000-0000-0000-000000000000');
    this.filterGroup.controls.StatusId.reset('00000000-0000-0000-0000-000000000000');
    this.filterGroup.controls.Year.reset('');
    this.filterGroup.controls.PowerOutput.reset('');
    this.approvedPowerProjectsPageService.fetch();
  }

  f_currency(value: any, args?: any): any {
    let nbr = Number((value + '').replace(/,|-/g, ''));
    return (nbr + '').replace(/(\d)(?=(\d{3})+(?!\d))/g, '$1,');
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
      this.approvedPowerProjectsPageService.patchState({ searchTerm });
  }

  onEnter() {
    this.searchData(this.searchGroup.controls.searchTerm.value)
  }

  searchData(searchTerm: string) {
    this.approvedPowerProjectsPageService.patchState({ searchTerm });
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
    this.approvedPowerProjectsPageService.patchState({ sorting });
  }

  // pagination
  paginate(paginator: PaginatorState) {
    this.approvedPowerProjectsPageService.patchState({ paginator });
  }

  // form actions
  create() {
    this.edit(undefined);
  }

  edit(id: any) {
    const modalRef = this.modalService.open(EditApprovedPowerProjectsModalComponent, { size: 'lg' });
    modalRef.componentInstance.id = id;
    modalRef.result.then(() =>
      this.approvedPowerProjectsPageService.fetch(),
      () => { }
    );
  }

  view(id: any) {
    const modalRef = this.modalService.open(EditApprovedPowerProjectsModalComponent, { size: 'lg' });
    modalRef.componentInstance.id = id;
    modalRef.componentInstance.type = 'view';
    modalRef.result.then(() =>
      this.approvedPowerProjectsPageService.fetch(),
      () => { }
    );
  }

  prenventInputNonNumber(event: any) {
    if (event.which < 48 || event.which > 57) {
      event.preventDefault();
    }
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
        const sb = this.approvedPowerProjectsPageService.delete(id).pipe(
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
        const sb = this.approvedPowerProjectsPageService.deletes(this.grouping.getSelectedRows()).pipe(
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
    const fileName = "Danhsachduandienduocpheduyet_" + timeString + ".xlsx"
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
    this.http.post(`${environment.apiUrl}/ApprovedPowerProject/ExportExcel`, query,
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


