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
import { ChemicalBusinessManagementPageService } from '../_services/chemical-business-management-page.service';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { EditChemicalBusinessManagementModalComponent } from './components/edit-chemical-business-management-modal/edit-modal.component';
import Swal from 'sweetalert2';
import { Options } from 'select2';
import { CommonService } from 'src/app/_metronic/shared/services/common.service';


@Component({
  selector: 'app-table-page',
  templateUrl: './table-page.component.html',
  styleUrls: ['./table-page.component.scss']
})
export class TableChemicalBusinessManagementPageComponent implements OnInit,
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
  public options: Options;
  private subscriptions: Subscription[] = []; // Read more: => https://brianflove.com/2016/12/11/anguar-2-unsubscribe-observables/
  yearRange: any;
  districtData: { id: string; text: string; }[];
  communeData: { id: string; text: string; districtId: string; }[];
  communeDataByDistrictId: { id: string; text: string; districtId: string; }[];

  constructor(
    private fb: FormBuilder,
    public chemicalBusinessManagementPageService: ChemicalBusinessManagementPageService,
    private modalService: NgbModal,
    private commonService: CommonService,
  ) { }

  ngOnInit(): void {
    this.getYearsList();
    this.filterForm();
    this.searchForm();
    this.loadDistrict();
    this.loadCommune();
    this.chemicalBusinessManagementPageService.fetch();
    this.grouping = this.chemicalBusinessManagementPageService.grouping;
    this.paginator = this.chemicalBusinessManagementPageService.paginator;
    this.sorting = this.chemicalBusinessManagementPageService.sorting;
    const sb = this.chemicalBusinessManagementPageService.isLoading$.subscribe((res: any) => this.isLoading = res);
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
    this.chemicalBusinessManagementPageService.setDefaults();
  }

  loadDistrict(){
    this.commonService.getDistrict().subscribe((res: any) => {
      const data = [
        {
          id: "00000000-0000-0000-0000-000000000000",
          text: '-- Chọn --'
        }
      ]
      for (var item of res.items) {
        let obj = {
          id: item.districtId,
          text: item.districtName,
        }
        data.push(obj)
      }
      this.districtData = data;
    })
  }
  
  loadCommune(){
    this.commonService.getCommune().subscribe((res: any) => {
      const data = [
        {
          id: "00000000-0000-0000-0000-000000000000",
          text: '-- Chọn --',
          districtId: '00000000-0000-0000-0000-000000000000'
        }
      ]
      for (let item of res.items) {
        let obj = {
          id: item.communeId,
          text: item.communeName,
          districtId: item.districtId
        }
        data.push(obj)
      }
      this.communeData = data;
      this.communeDataByDistrictId = data;
    })
  }

  getYearsList() {
    const currentYear = new Date().getFullYear();
    const yearsList: any = [{ id: 0, text: "-- Chọn --" }];
    for (let i = -10; i <= 10; i++) {
      const year = currentYear + i;
      yearsList.push({ id: year, text: "Kiểm tra năm " + year.toString() });
    }
    this.yearRange = yearsList;
  }

  filterForm() {
    this.filterGroup = this.fb.group({
      Status: [0],
      DistrictId: ["00000000-0000-0000-0000-000000000000"],
      CommuneId: ["00000000-0000-0000-0000-000000000000"],
    });
    this.subscriptions.push(
      this.filterGroup.controls.Status.valueChanges.subscribe(() => this.filter())
    );
    const districtChange = this.filterGroup.controls.DistrictId.valueChanges.subscribe(x => {
      if (x != '00000000-0000-0000-0000-000000000000') {
        this.communeDataByDistrictId = this.communeData.filter((y: any) => y.districtId == x || y.id == '00000000-0000-0000-0000-000000000000');
      } else {
        this.communeDataByDistrictId = this.communeData;
      }
      this.filter()
    });
    this.subscriptions.push(districtChange)
    this.subscriptions.push(
      this.filterGroup.controls.CommuneId.valueChanges.subscribe(() => this.filter())
    );
  }

  filter() {
    const filter: {[key:string]: string} = {};
    const Status = this.filterGroup.controls['Status'].value;
    if (Status > 0) {
      filter['Status'] = Status;
    }
    const DistrictId = this.filterGroup.controls['DistrictId'].value;
    if (DistrictId != "00000000-0000-0000-0000-000000000000") {
      filter['DistrictId'] = DistrictId;
    }
    const CommuneId = this.filterGroup.controls['CommuneId'].value;
    if (CommuneId != "00000000-0000-0000-0000-000000000000") {
      filter['CommuneId'] = CommuneId;
    }
    this.chemicalBusinessManagementPageService.patchState({ filter });
  }

  resetFilter() {
    this.filterGroup.controls.DistrictId.reset("00000000-0000-0000-0000-000000000000");
    this.filterGroup.controls.CommuneId.reset("00000000-0000-0000-0000-000000000000");
    this.filterGroup.controls.Status.reset(0);
    this.chemicalBusinessManagementPageService.fetch();
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
    if(searchTerm.length == 0)
      this.chemicalBusinessManagementPageService.patchState({ searchTerm });
  }

  onEnter() {
    this.searchData(this.searchGroup.controls.searchTerm.value)
  }

  searchData(searchTerm: string) {
    this.chemicalBusinessManagementPageService.patchState({ searchTerm });
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
    this.chemicalBusinessManagementPageService.patchState({ sorting });
  }

  // pagination
  paginate(paginator: PaginatorState) {
    this.chemicalBusinessManagementPageService.patchState({ paginator });
  }

  // form actions
  create() {
    this.edit(0);
  }

  edit(id: any) {
    const modalRef = this.modalService.open(EditChemicalBusinessManagementModalComponent, { size: '100px' });
    modalRef.componentInstance.id = id;
    modalRef.result.then(() =>
      this.chemicalBusinessManagementPageService.fetch(),
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
        const sb = this.chemicalBusinessManagementPageService.delete(id).pipe(
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
        const sb = this.chemicalBusinessManagementPageService.deletes(this.grouping.getSelectedRows()).pipe(
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
}


