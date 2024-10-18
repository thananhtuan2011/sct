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
import { RecordsManagerPageService } from '../_services/records-manager-page.service';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { EditRecordsManagerModalComponent } from './components/edit-records-manager-modal/edit-modal.component';
import Swal from 'sweetalert2';
import { Options } from 'select2';
import * as moment from 'moment';
import { HttpClient } from '@angular/common/http';
import { environment } from 'src/environments/environment';


@Component({
  selector: 'app-table-records-manager-page',
  templateUrl: './table-records-manager-page.component.html'
})
export class TableRecordsManagerPageComponent implements OnInit,
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
  dataRecordsGroup: any = []
  options: Options
  yearData: any = []
  MinDate: any = null;
  MaxDate: any = null;
  filterValue: { [key: string]: string; } = {};
  private subscriptions: Subscription[] = []; // Read more: => https://brianflove.com/2016/12/11/anguar-2-unsubscribe-observables/
  constructor(
    private fb: FormBuilder,
    public recordsManager: RecordsManagerPageService,
    private modalService: NgbModal,
    private http: HttpClient
  ) { }

  ngOnInit(): void {
    this.loadYear();
    this.filterForm();
    this.searchForm();
    this.loadRecordsFinance();
    this.recordsManager.fetch();
    this.grouping = this.recordsManager.grouping;
    this.paginator = this.recordsManager.paginator;
    this.sorting = this.recordsManager.sorting;
    const sb = this.recordsManager.isLoading$.subscribe((res: any) => this.isLoading = res);
    this.subscriptions.push(sb);
    this.options = {
      theme:'bootstrap5',
      templateSelection: this.templateSelection,
    };
  }
  
  public templateSelection = (state: any): JQuery | string => {
    if (!state.id) {
      return state.text;
    }
    return jQuery('<span class="form-select form-select-solid form-select-lg">'+ state.text + '</span>');
  }

  ngOnDestroy() {
    this.subscriptions.forEach((sb) => sb.unsubscribe());
    this.recordsManager.setDefaults();
  }

  // filtration
  filterForm() {
    
    this.filterGroup = this.fb.group({
      RecordsFinancePlan: ['00000000-0000-0000-0000-000000000000'],
     // Year: 0,
      MinDate: [this.MinDate],
      MaxDate: [this.MaxDate],
      //searchTerm: [''],
    });
    this.subscriptions.push(
      this.filterGroup.controls.RecordsFinancePlan.valueChanges.subscribe(() =>
        this.filter()
      )
    );
    this.subscriptions.push(
      this.filterGroup.controls.MinDate.valueChanges.subscribe(() => this.filter())
    );
    this.subscriptions.push(
      this.filterGroup.controls.MaxDate.valueChanges.subscribe(() => this.filter())
    );
    // this.subscriptions.push(
    //   this.filterGroup.controls.Year.valueChanges.subscribe(() => this.filter())
    // );
  }

  filter() {
    
    const filter: { [key: string]: string } = {};
    const RecordsFinancePlan = this.filterGroup.controls['RecordsFinancePlan'].value;
   // const Year = this.filterGroup.controls['Year'].value;
   const MinDate = this.filterGroup.controls['MinDate'].value;
   const MaxDate = this.filterGroup.controls['MaxDate'].value;
   
    if (RecordsFinancePlan && RecordsFinancePlan != '00000000-0000-0000-0000-000000000000' ) {
      filter['RecordsFinancePlan'] = RecordsFinancePlan;
    }
    
    if (MinDate) {
      filter['MinDate'] = MinDate;
    }
    if (MaxDate) {
      filter['MaxDate'] = MaxDate;
    }
    // if (Year) {
    //   filter['Year'] = Year.toString();
    // }
    this.filterValue = filter;
    this.recordsManager.patchState({ filter });
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
      this.recordsManager.patchState({ searchTerm });
  }

  onEnter() {
    this.searchData(this.searchGroup.controls.searchTerm.value)
  }

  searchData(searchTerm: string) {
    this.recordsManager.patchState({ searchTerm });
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
    this.recordsManager.patchState({ sorting });
  }

  // pagination
  paginate(paginator: PaginatorState) {
    this.recordsManager.patchState({ paginator });
  }

  // form actions
  create() {
    this.edit(undefined);
  }

  edit(item: any) {
    const modalRef = this.modalService.open(EditRecordsManagerModalComponent, { size: '100px' });
    if(item){
      modalRef.componentInstance.id = item.recordsManagerId;
      modalRef.componentInstance.itemData = item;
    }
    modalRef.result.then(() =>
      this.recordsManager.fetch(),
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
        const sb = this.recordsManager.delete(id).pipe(
          tap((res) => {
            Swal.fire({
              icon: res.status == 1 ? 'success' : 'error',
              title: res.status == 1 ? 'Thành công' : 'Thất bại',
              confirmButtonText: 'Xác nhận',
              text: res.status == 1 ? 'Xóa thành công': res.status == 0 ? res.error.msg : 'Xóa thất bại',
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
  
  loadRecordsFinance(){
    this.recordsManager.getAllRecordsFianacePlan().subscribe(res => {
      const data = [
        {
          id: "00000000-0000-0000-0000-000000000000",
          text: '-- Chọn --'
        }
      ]
      
      for(var item of res.items) { 
        let obj = {
          id: item.recordsFinancePlanId,
          text: item.name
        }
        data.push(obj)
      }
      this.dataRecordsGroup = data
    })
  }
  clearFilter(){
    this.filterGroup.controls.RecordsFinancePlan.setValue("00000000-0000-0000-0000-000000000000");
    this.filterGroup.controls.MinDate.setValue(null);
    this.filterGroup.controls.MaxDate.setValue(null);
    this.filter();
  }
  
  loadYear(){
    const data = [
      {
        id: 0,
        text: "-- Chọn --"
      }
    ];
    for(let i = 0; i < 30; i++){
      let obj = {
        id: moment().year()- 15 + i,
        text: (moment().year()- 15 + i).toString()
      }
      data.push(obj);
    }
    this.yearData = data;
  }
  
  convert_date_string(string_date: string) {
    var date = string_date.split("T")[0];
    var list = date.split("-"); //["year", "month", "day"]
    var result = list[2] + "/" + list[1] + "/" + list[0]
    return result
  }
  
  exportFile() {
    const moment = require("moment");
    const timeString = moment().format("DDMMYYYYHHmmss");
    const fileName = "QuanLyHoSoLuuTruPhongKHTC_" + timeString + ".xlsx"

    const query = {
      filter: this.filterValue,
      grouping: {},
      paginator: {},
      sorting: { column: "id", direction: "desc" },
      searchTerm: this.searchGroup.controls.searchTerm.value
    }
    if(!query.filter.MinDate){
      Swal.fire({
        icon: 'warning',
        title: "Vui lòng chọn thời gian tiếp nhận!",
        confirmButtonText: 'Xác nhận',
      });
    }else {
    Swal.fire({
            icon: 'info',
      title: 'Đang xuất File...',
      // text: 'Vui lòng đợi một lúc trước khi file của bạn sẵn sàng!',
      didOpen: () => {
        Swal.showLoading()
      },
    })


    this.http.post(`${environment.apiUrl}/RecordsManager/Export`, query,
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
  view(item: any){
    const modalRef = this.modalService.open(EditRecordsManagerModalComponent, { size: '100px' });
    if(item){
      modalRef.componentInstance.id = item.recordsManagerId;
      modalRef.componentInstance.itemData = item;
      modalRef.componentInstance.type = 'view';
    }
    modalRef.result.then(() =>
      this.recordsManager.fetch(),
      () => { }
    );
  }
}


