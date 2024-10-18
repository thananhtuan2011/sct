import { PaginatorState } from '../../../../_metronic/shared/crud-table/models/paginator.model';
import { ISearchView } from '../../../../_metronic/shared/crud-table/models/search.model';
import { IFilterView } from '../../../../_metronic/shared/crud-table/models/filter.model';
import { ISortView, SortState } from '../../../../_metronic/shared/crud-table/models/sort.model';
import { GroupingState, IGroupingView } from '../../../../_metronic/shared/crud-table/models/grouping.model';
import { ICreateAction, IDeleteAction, IDeleteSelectedAction, IEditAction, IFetchSelectedAction, IUpdateStatusForSelectedAction } from '../../../../_metronic/shared/crud-table/models/table.model';
import { ChangeDetectorRef, Component, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { catchError, finalize, of, Subscription, tap } from 'rxjs';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import Swal from 'sweetalert2';

import { NewRuralCriteriaPageService } from '../_services/new-rural-criteria-page.service';
import { Options } from 'select2';
import { HttpClient } from '@angular/common/http';
import { environment } from 'src/environments/environment';
import { EditNewRuralCriteriaModalComponent } from './components/edit-new-rural-criteria-modal/edit-modal.component'
import { CommonService } from 'src/app/_metronic/shared/services/common.service';
import * as moment from 'moment';

@Component({
  selector: 'app-table-page',
  templateUrl: './table-page.component.html'
})
export class TableNewRuralCriteriaPageComponent implements OnInit,
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
  options: Options;
  private subscriptions: Subscription[] = []; // Read more: => https://brianflove.com/2016/12/11/anguar-2-unsubscribe-observables/
  filterGroup: FormGroup;
  searchGroup: FormGroup;
  districtData: any[];
  communeData: any[];
  communeDataFilter: any[];
  viewStatus: any[] = [
    {
      id: "0",
      text: "-- Hiển thị (Tất cả) --"
    },
    {
      id: "1",
      text: "Nông thôn mới"
    },
    {
      id: "2",
      text: "Nông thôn mới nâng cao"
    },
  ]
  titleData: any = [
    {
      id: "0",
      text: "-- Chọn --"
    },
    {
      id: "1",
      text: "Nông thôn mới"
    },
    {
      id: "2",
      text: "Nông thôn mới nâng cao"
    },
  ];
  apiLoaded: number = 0;

  constructor(
    private fb: FormBuilder,
    public pageService: NewRuralCriteriaPageService,
    public commonService: CommonService,
    private modalService: NgbModal,
    private cd: ChangeDetectorRef,
  ) { }

  ngOnInit(): void {
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
        { id: "00000000-0000-0000-0000-000000000000", text: "-- Huyện --" },
        ...res.items.map((item: any) => ({
          id: item.districtId,
          text: item.districtName,
        }))
      ]
      this.districtData = data;
    })
  }

  loadCommune() {
    this.commonService.getCommune().subscribe((res: any) => {
      const data = [
        { id: "00000000-0000-0000-0000-000000000000", text: "-- Xã --" },
        ...res.items.map((item: any) => ({
          id: item.communeId,
          text: item.communeName,
          districtId: item.districtId,
        }))
      ]
      this.communeData = data;
      this.communeDataFilter = data;
    })
  }

  filterForm() {
    this.filterGroup = this.fb.group({
      DistrictId: ["00000000-0000-0000-0000-000000000000"],
      CommuneId: ["00000000-0000-0000-0000-000000000000"],
      Title: ["0"],
      // Year: [moment().year()],
      // View: ["0"],
    });
    this.subscriptions.push(
      this.filterGroup.controls.DistrictId.valueChanges.subscribe((x: any) => {
        if (x != "00000000-0000-0000-0000-000000000000") {
          this.communeDataFilter = this.communeData.filter(i => i.id == "00000000-0000-0000-0000-000000000000" || i.districtId == x);
          this.filterGroup.controls.CommuneId.reset("00000000-0000-0000-0000-000000000000", { emitEvent: false });
          this.cd.detectChanges();
        } else {
          this.communeDataFilter = this.communeData;
          this.filterGroup.controls.CommuneId.reset("00000000-0000-0000-0000-000000000000", { emitEvent: false });
          this.cd.detectChanges();
        }
        this.filter()
      })
    );
    this.subscriptions.push(
      this.filterGroup.controls.CommuneId.valueChanges.subscribe(() => this.filter())
    );
    this.subscriptions.push(
      this.filterGroup.controls.Title.valueChanges.subscribe(() => this.filter())
    );
    // this.subscriptions.push(
    //   this.filterGroup.controls.View.valueChanges.subscribe(() => this.cd.detectChanges())
    // );
  }

  filter() {
    const filter: { [key: string]: string } = {};
    const DistrictId = this.filterGroup.controls['DistrictId'].value;
    if (DistrictId != "00000000-0000-0000-0000-000000000000") {
      filter['District'] = DistrictId;
    }
    const CommuneId = this.filterGroup.controls['CommuneId'].value;
    if (CommuneId != "00000000-0000-0000-0000-000000000000") {
      filter['Commune'] = CommuneId;
    }
    const Title = this.filterGroup.controls['Title'].value;
    if (Title != "0") {
      filter['Title'] = Title;
    }
    this.pageService.patchState({ filter });
  }

  resetFilter() {
    this.filterGroup.controls.DistrictId.reset("00000000-0000-0000-0000-000000000000");
    this.filterGroup.controls.CommuneId.reset("00000000-0000-0000-0000-000000000000");
    this.filterGroup.controls.Title.reset("0");
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
    const modalRef = this.modalService.open(EditNewRuralCriteriaModalComponent, { size: '100px' });
    modalRef.componentInstance.id = id;
    modalRef.result.then(() =>
      this.pageService.fetch(),
      () => { }
    );
  }

  view(id: any) {
    const modalRef = this.modalService.open(EditNewRuralCriteriaModalComponent, { size: '100px' });
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

  deleteSelected() { }

  updateStatusForSelected() { }

  fetchSelected() { }
}


