import { PaginatorState } from '../../../../_metronic/shared/crud-table/models/paginator.model';
import { ISearchView } from '../../../../_metronic/shared/crud-table/models/search.model';
import { IFilterView } from '../../../../_metronic/shared/crud-table/models/filter.model';
import { ISortView, SortState } from '../../../../_metronic/shared/crud-table/models/sort.model';
import { GroupingState, IGroupingView } from '../../../../_metronic/shared/crud-table/models/grouping.model';
import { ICreateAction, IDeleteAction, IDeleteSelectedAction, IEditAction, IFetchSelectedAction, IUpdateStatusForSelectedAction } from '../../../../_metronic/shared/crud-table/models/table.model';
import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { catchError, finalize, of, Subscription, tap } from 'rxjs';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import Swal from 'sweetalert2';

import { Target1708PageService } from '../_services/target1708-page.service';
import { EditTarget1708ModalComponent } from './components/edit-target1708-modal/edit-modal.component';
import { Options } from 'select2';
import { HttpClient } from '@angular/common/http';
import { environment } from 'src/environments/environment';
import { CommonService } from 'src/app/_metronic/shared/services/common.service';
import * as moment from 'moment';

@Component({
  selector: 'app-table-page',
  templateUrl: './table-page.component.html'
})
export class TableTarget1708PageComponent implements OnInit,
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
  private subscriptions: Subscription[] = []; // Read more: => https://brianflove.com/2016/12/11/anguar-2-unsubscribe-observables/
  stageData: any[] = [];
  districtData: any[];
  options: Options;
  currentStage: any = {
    id: '',
    text: '',
    yearStart: '',
    yearEnd: '',
  };
  previousStage: any = {
    id: '',
    text: '',
    yearStart: '',
    yearEnd: '',
  };
  showPrevious: boolean = true;
  showFilter: boolean = false;
  filterValue: { [key: string]: string; } = {};
  communeData: any[];
  communeDataFilter: any[];

  constructor(
    private fb: FormBuilder,
    public pageService: Target1708PageService,
    public commonService: CommonService,
    private modalService: NgbModal,
    private http: HttpClient,
  ) { }

  ngOnInit(): void {
    this.loadStage();
    this.loadDistrict();
    this.loadCommune();
    this.filterForm();
    this.searchForm();
    this.pageService.fetch();
    this.grouping = this.pageService.grouping;
    this.paginator = this.pageService.paginator;
    this.sorting = this.pageService.sorting;
    const sb = this.pageService.isLoading$.subscribe((res: any) => this.isLoading = res);
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
    this.pageService.setDefaults();
  }

  loadDistrict() {
    this.commonService.getDistrict().subscribe((res: any) => {
      const data = [
        { id: "00000000-0000-0000-0000-000000000000", text: "-- Chọn --" },
        ...res.items.map((item: any) => ({
          id: item.districtId,
          text: item.districtName,
        }))
      ]
      this.districtData = data.sort((a, b) => a.text.localeCompare(b.text));;
    })
  }

  loadCommune() {
    this.commonService.getCommune().subscribe((res: any) => {
      const data = [
        { id: "00000000-0000-0000-0000-000000000000", text: "-- Chọn --" },
        ...res.items.map((item: any) => ({
          id: item.communeId,
          text: item.communeName,
          districtId: item.districtId
        }))
      ]
      this.communeData = data;
      this.communeDataFilter = data;
    })
  }

  loadStage() {
    this.pageService.loadStage().subscribe((res: any) => {
      const data = [
        ...res.items.map((item: any) => ({
          id: item.stageId,
          text: item.stageName,
          yearStart: item.startYear,
          yearEnd: item.endYear,
        }))
      ]
      this.stageData = data;

      /** Giai đoạn hiện tại */
      this.currentStage = this.stageData.find(x => moment().year() >= x.yearStart && moment().year() <= x.yearEnd);
      if (this.currentStage != null) {
        this.filterGroup.controls.StageId.setValue(this.currentStage.id);
      } else {
        this.filterGroup.controls.StageId.setValue(this.stageData[0].id);
      }

      /** Giai đoạn trước */
      this.previousStage = this.stageData.find(x => x.yearStart == this.currentStage.yearStart - 5 && x.yearEnd == this.currentStage.yearEnd - 5);
      if (this.previousStage == null) {
        const currentIndex = this.stageData.findIndex(x => moment().year() >= x.yearStart && moment().year() <= x.yearEnd);
        this.previousStage = this.stageData[currentIndex - 1];
      }

      /** Hiển thị ô Filter */
      this.showFilter = true;
    })
  }

  filterForm() {
    this.filterGroup = this.fb.group({
      DistrictId: ["00000000-0000-0000-0000-000000000000"],
      CommuneId: ["00000000-0000-0000-0000-000000000000"],
      StageId: [""],
    });
    const districtChange = this.filterGroup.controls.DistrictId.valueChanges.subscribe(x => {
      if (x != '00000000-0000-0000-0000-000000000000') {
        this.communeDataFilter = this.communeData.filter((y: any) => y.districtId == x || y.id == '00000000-0000-0000-0000-000000000000');
      } else {
        this.communeDataFilter = this.communeData;
      }
      this.filter()
    });
    this.subscriptions.push(districtChange)
    this.subscriptions.push(
      this.filterGroup.controls.CommuneId.valueChanges.subscribe(() => this.filter())
    );
    // this.subscriptions.push(
    //   this.filterGroup.controls.DistrictId.valueChanges.subscribe(() => this.filter())
    // );
    this.subscriptions.push(
      this.filterGroup.controls.StageId.valueChanges.subscribe((x) => {
        this.filter();
        /** Tìm current */
        const Current = this.stageData.find((i: any) => i.id == x);
        const currentIndex = this.stageData.findIndex((i: any) => i.id == x);
        this.currentStage = Current;
        
        /** Tìm previous */
        if (!!this.stageData[currentIndex - 1]) {
          this.previousStage = this.stageData[currentIndex - 1];
          this.showPrevious = true;
        } else {
          this.showPrevious = false;
        }
      })
    );
  }

  filter() {
    const filter: { [key: string]: string } = {};
    const StageId = this.filterGroup.controls['StageId'].value;
    if (StageId != "00000000-0000-0000-0000-000000000000") {
      filter['Stage'] = StageId;
    }
    const DistrictId = this.filterGroup.controls['DistrictId'].value;
    if (DistrictId != "00000000-0000-0000-0000-000000000000") {
      filter['District'] = DistrictId;
    }
    const CommuneId = this.filterGroup.controls['CommuneId'].value;
    if (CommuneId != "00000000-0000-0000-0000-000000000000") {
      filter['Commune'] = CommuneId;
    }
    this.filterValue = filter;
    this.pageService.patchState({ filter });
  }

  resetFilter() {
    this.filterGroup.controls.DistrictId.reset("00000000-0000-0000-0000-000000000000");
    this.filterGroup.controls.CommuneId.reset("00000000-0000-0000-0000-000000000000");
    this.currentStage = this.stageData.find(x => moment().year() >= x.yearStart && moment().year() <= x.yearEnd);
    if (this.currentStage != null) {
      this.filterGroup.controls.StageId.setValue(this.currentStage.id);
    } else {
      this.filterGroup.controls.StageId.setValue(this.stageData[0].id);
    }
    this.pageService.fetch();
  }

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
      this.pageService.patchState({ searchTerm });
  }

  onEnter() {
    this.searchData(this.searchGroup.controls.searchTerm.value)
  }

  searchData(searchTerm: string) {
    this.pageService.patchState({ searchTerm });
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
    this.pageService.patchState({ sorting });
  }

  paginate(paginator: PaginatorState) {
    this.pageService.patchState({ paginator });
  }

  create() {
    this.edit(undefined);
  }

  edit(id: any) {
    const modalRef = this.modalService.open(EditTarget1708ModalComponent, { size: '100px' });
    modalRef.componentInstance.id = id;
    modalRef.result.then(() =>
      this.pageService.fetch(),
      () => { }
    );
  }

  view(id: any) {
    const modalRef = this.modalService.open(EditTarget1708ModalComponent, { size: '100px' });
    modalRef.componentInstance.id = id;
    modalRef.componentInstance.type = "view";
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

  deleteSelected() {
  }

  updateStatusForSelected() {
  }

  fetchSelected() {
  }

  exportFile() {
    const moment = require("moment");
    const timeString = moment().format("DDMMYYYYHHmmss");
    const fileName = "Baocaoantoanthucphamnongthonmoi_" + timeString + ".xlsx"

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
      searchTerm: "",
    }

    this.http.post(`${environment.apiUrl}/Target1708/ExportExcel`, query,
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


