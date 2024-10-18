import { PaginatorState } from '../../../../_metronic/shared/crud-table/models/paginator.model';
import { ISearchView } from '../../../../_metronic/shared/crud-table/models/search.model';
import { IFilterView } from '../../../../_metronic/shared/crud-table/models/filter.model';
import { ISortView, SortState } from '../../../../_metronic/shared/crud-table/models/sort.model';
import { GroupingState, IGroupingView } from '../../../../_metronic/shared/crud-table/models/grouping.model';
import { ICreateAction, IDeleteAction, IDeleteSelectedAction, IEditAction, IFetchSelectedAction, IUpdateStatusForSelectedAction } from '../../../../_metronic/shared/crud-table/models/table.model';
import { AfterViewInit, Component, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { catchError, finalize, of, Subscription, tap } from 'rxjs';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import Swal from 'sweetalert2';

import { Target7PageService } from '../_services/target7-page.service';
import { EditTarget7ModalComponent } from './components/edit-target7-modal/edit-modal.component';
import { Options } from 'select2';
import { HttpClient } from '@angular/common/http';
import { environment } from 'src/environments/environment';
import { CommonService } from 'src/app/_metronic/shared/services/common.service';
import * as moment from 'moment';

@Component({
  selector: 'app-table-page',
  templateUrl: './table-page.component.html'
})
export class TableTarget7PageComponent implements OnInit,
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
  AfterViewInit,
  IFilterView {
  paginator: PaginatorState;
  sorting: SortState;
  grouping: GroupingState;
  isLoading: boolean;
  filterGroup: FormGroup;
  searchGroup: FormGroup;
  private subscriptions: Subscription[] = []; // Read more: => https://brianflove.com/2016/12/11/anguar-2-unsubscribe-observables/
  stageData: any[];
  districtData: any[];
  options: Options;
  currentStage: any;
  previousStage: any;
  showPrevious: boolean = true;
  showFilter: boolean = false;
  title: string = '';
  filterValue: { [key: string]: string; };
  communeData: any[];
  communeDataFilter: any[];
  dataProvice: any = [];
  districtId: string = '00000000-0000-0000-0000-000000000000'
  
  queryData = {
    filter: {},
    grouping: {},
    paginator: {},
    sorting: { column: "id", direction: "desc" },
    searchTerm: "",
  }
  selectedIndex: number = 1;
  
  constructor(
    private fb: FormBuilder,
    public pageService: Target7PageService,
    public commonService: CommonService,
    private modalService: NgbModal,
    private http: HttpClient,
  ) { }
  
  ngAfterViewInit(): void {
    this.selectedIndex = 0;
  }

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
    const sb = this.commonService.GetConfig('TARGET7_STAGE').subscribe((res: any) => {
      const data = [
        //{ id: "00000000-0000-0000-0000-000000000000", text: "-- Chọn --" },
        ...res.items.listConfig.map((item: any) => ({
          id: item.categoryId,
          text: item.categoryName,
        }))
      ]
      this.stageData = data;
      this.filterGroup.controls.StageId.setValue(this.stageData[0].id);
      this.title = this.stageData[0].text;
      this.queryData = {...this.queryData, filter: {
        Stage: this.stageData[0].id,
        Year: moment().year().toString()
      }}
      this.loadData();
    })
    this.subscriptions.push(sb);
  }

  filterForm() {
    this.filterGroup = this.fb.group({
      DistrictId: ["00000000-0000-0000-0000-000000000000"],
      CommuneId: ["00000000-0000-0000-0000-000000000000"],
      StageId: ["00000000-0000-0000-0000-000000000000"],
      Year: [moment().year()],    
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
    this.subscriptions.push(
      this.filterGroup.controls.StageId.valueChanges.subscribe((x) => {
        this.filter();
        this.title = this.stageData.find(i => i.id == x).text;
      })
    );
    this.subscriptions.push(
      this.filterGroup.controls.Year.valueChanges.subscribe(() => this.filter())
    );
   // this.loadData();
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
    const Year = this.filterGroup.controls['Year'].value;
    if (Year) {
      filter['Year'] = Year.toString();
    }
    this.filterValue = filter;

    this.queryData = {...this.queryData, filter: this.filterValue};
    this.loadData();
    this.pageService.patchState({ filter });
  }

  resetFilter() {
    this.filterGroup.controls.DistrictId.reset("00000000-0000-0000-0000-000000000000");
    this.filterGroup.controls.StageId.reset(this.stageData[0].id);
    this.filterGroup.controls.Year.reset(moment().year());
    this.filterGroup.controls.CommuneId.reset("00000000-0000-0000-0000-000000000000");
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
    const modalRef = this.modalService.open(EditTarget7ModalComponent, { size: '100px' });
    modalRef.componentInstance.id = id;
    modalRef.result.then(() =>
      this.pageService.fetch(),
      () => { }
    );
  }

  view(id: any) {
    const modalRef = this.modalService.open(EditTarget7ModalComponent, { size: '100px' });
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
  
  getDataDetail(id: string){
    this.districtId = id;
    this.filterGroup.controls.DistrictId.patchValue(id);
    this.communeDataFilter = this.communeData.filter((y: any) => y.districtId == id || y.id == '00000000-0000-0000-0000-000000000000');
    this.selectedIndex = 1;
    this.filter();
  }
  
  loadData(){
    this.subscriptions.push(this.pageService.loadDataProvice(this.queryData).subscribe((res: any) => {
      this.dataProvice = res.items;
    }))
  }

  exportFile() {
    const moment = require("moment");
    const timeString = moment().format("DDMMYYYYHHmmss");
    const fileName = "Baocaothuchientieuchidiennongthon_" + timeString + ".xlsx"

    if (this.filterGroup.controls.StageId.value == '00000000-0000-0000-0000-000000000000' && !this.filterGroup.controls.Year.value) {
      Swal.fire({
        icon: 'warning',
        title: 'Hãy chọn giai đoạn và năm báo cáo.',
        confirmButtonText: 'Xác nhận',
      })
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

    const query = {
      filter: this.filterValue,
      grouping: {},
      paginator: {},
      sorting: { column: "id", direction: "desc" },
      searchTerm: "",
    }

    this.http.post(`${environment.apiUrl}/Target7/ExportExcel`, query,
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
  
  tabChange(event: any){
    this.selectedIndex = event.index;
    this.filterGroup.controls.Year.patchValue(this.filterGroup.controls.Year.value)
    this.filterGroup.controls.StageId.patchValue(this.filterGroup.controls.StageId.value)    
  }
}


