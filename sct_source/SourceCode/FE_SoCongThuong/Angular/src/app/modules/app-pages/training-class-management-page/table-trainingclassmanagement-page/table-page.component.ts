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
import { TrainingClassManagementPageService } from '../_services/trainingclassmanagement.service';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { EditTrainingClassManagementModalComponent } from './components/edit-trainingclassmanagement-modal/edit-modal.component';
import Swal from 'sweetalert2';
import { Options } from 'select2';
import { HttpClient } from '@angular/common/http';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-table-trainingclassmanagement-page',
  templateUrl: './table-page.component.html',
  styleUrls: ['./table-page.component.scss']
})

export class TableTrainingClassManagementPageComponent implements OnInit,
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
  options: Options;
  minDate: any = null;
  maxDate: any = null;
  YearData: Array <any> = [
    {
      id: 0,
      text: '-- Chọn --'
    }
  ];
  
  yearId: any = 0;
  filterValue: any = {}
  private subscriptions: Subscription[] = []; // Read more: => https://brianflove.com/2016/12/11/anguar-2-unsubscribe-observables/
  
  constructor(
    private fb: FormBuilder,
    public trainingClassManagementPageService: TrainingClassManagementPageService,
    private modalService: NgbModal,
    private http: HttpClient
    ) {}

  ngOnInit(): void {
    this.filterForm();
    this.searchForm();
    this.loadYear();
    this.trainingClassManagementPageService.fetch();
    this.grouping = this.trainingClassManagementPageService.grouping;
    this.paginator = this.trainingClassManagementPageService.paginator;
    this.sorting = this.trainingClassManagementPageService.sorting;
    const sb = this.trainingClassManagementPageService.isLoading$.subscribe((res:any) => this.isLoading = res);
    this.subscriptions.push(sb);
    this.options = {
      theme: 'bootstrap5',
      templateSelection: this.templateSelection,
    };
  }
  
  loadYear(){
    for(let i = 1; i<= 20 ; i++ ){
      let item: any = {};
      item.id = i;
      item.text = (new Date().getFullYear()- 10 + i).toString(); 
      this.YearData.push(item);
    }
  }
  
  public templateSelection = (state: any): JQuery | string => {
    if (!state.id) {
      return state.text;
    }
    return jQuery('<span class="form-select form-select-solid form-select-lg">' + state.text + '</span>');
  }

  ngOnDestroy() {
    this.subscriptions.forEach((sb) => sb.unsubscribe());
    this.trainingClassManagementPageService.setDefaults();
  }
  
  change_value(event: any, ControlName: string){
    this.filterGroup.controls[ControlName].setValue(event);
    this.filterGroup.updateValueAndValidity();
  }

  filterForm() {
    this.filterGroup = this.fb.group({
      MinDate: [""],
      MaxDate: [""],
    });
    this.subscriptions.push(
      this.filterGroup.controls.MinDate.valueChanges.subscribe(() => this.filter())
    );
    this.subscriptions.push(
      this.filterGroup.controls.MaxDate.valueChanges.subscribe(() => this.filter())
    );
  }

  filter() {
    const filter: {[key:string]: string} = {};
    const MinDate = this.filterGroup.controls['MinDate'].value;
    if (MinDate.length > 0) {
      filter['MinDate'] = MinDate;
    }
    const MaxDate = this.filterGroup.controls['MaxDate'].value;
    if (MinDate.length > 0) {
      filter['MaxDate'] = MaxDate;
    }
    this.filterValue = filter;
    this.trainingClassManagementPageService.patchState({ filter });
  }

  resetFilter() {
    this.filterGroup.controls.MinDate.reset("");
    this.filterGroup.controls.MaxDate.reset("");
    this.trainingClassManagementPageService.fetch();
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
      this.trainingClassManagementPageService.patchState({ searchTerm });
  }

  onEnter() {
    this.searchData(this.searchGroup.controls.searchTerm.value)
  }

  searchData(searchTerm: string) {
    this.trainingClassManagementPageService.patchState({ searchTerm });
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
    this.trainingClassManagementPageService.patchState({ sorting });
  }

  // pagination
  paginate(paginator: PaginatorState) {
    this.trainingClassManagementPageService.patchState({ paginator });
  }

  // form actions
  create() {
    this.edit(0);
  }

  edit(id: any) {
    // console.log('id: ',id)
    const modalRef = this.modalService.open(EditTrainingClassManagementModalComponent, { size: '100px' });
    modalRef.componentInstance.id = id;
    modalRef.result.then(() =>
      this.trainingClassManagementPageService.fetch(),
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
        const sb = this.trainingClassManagementPageService.deleteTrainingClassManagement(id).pipe(
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

  view(id: any, type: any) {
    const modalRef = this.modalService.open(EditTrainingClassManagementModalComponent, { size: '100px' });
    modalRef.componentInstance.id = id;
    modalRef.componentInstance.type = type;
    modalRef.result.then(() =>
      this.trainingClassManagementPageService.fetch(),
      () => { }
    );
  }

  updateStatusForSelected() {
  }

  fetchSelected() {
  }
  
  exportFile() {
    const moment = require("moment");
    const timeString = moment().format("DDMMYYYYHHmmss");
    const fileName = "ATKTMT_LopTapHuan" + timeString + ".xlsx"

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

    this.http.post(`${environment.apiUrl}/TrainingClassManagement/Export`, query,
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


