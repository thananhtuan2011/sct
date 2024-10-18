import { PageInfoService, PageLink } from '../../../../_metronic/layout/core/page-info.service';
import { PaginatorState } from '../../../../_metronic/shared/crud-table/models/paginator.model';
import { ISearchView } from '../../../../_metronic/shared/crud-table/models/search.model';
import { IFilterView } from '../../../../_metronic/shared/crud-table/models/filter.model';
import { ISortView, SortState } from '../../../../_metronic/shared/crud-table/models/sort.model';
import { GroupingState, IGroupingView } from '../../../../_metronic/shared/crud-table/models/grouping.model';
import { ICreateAction, IDeleteAction, IEditAction, IFetchSelectedAction, IUpdateStatusForSelectedAction } from '../../../../_metronic/shared/crud-table/models/table.model';
import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { catchError, finalize, Observable, of, Subscription, tap } from 'rxjs';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import Swal from 'sweetalert2';
import { CateProjectPageService } from '../_services/cate-project-page.service';
import { EditCateProjectModalComponent } from './components/edit-cate-project-modal/edit-cate-project-modal.component';
import { Options } from 'select2';
import { environment } from 'src/environments/environment';
import { HttpClient } from '@angular/common/http';



@Component({
  selector: 'app-table-cate-project-page',
  templateUrl: './table-cate-project-page.component.html'
})
export class TableCateProjectModalComponent implements OnInit,
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
  options: Options
  filterValue: any = {};
  private subscriptions: Subscription[] = []; // Read more: => https://brianflove.com/2016/12/11/anguar-2-unsubscribe-observables/
  //MinDate
  public minDate: string;
  
  dataProjectType: any = [
    {
      id: 0,
      text: '-- Chọn --'
    },
    {
      id: 1,
      text: 'Lũy kế DDI ngoài Khu công nghiệp'
    },
    {
      id: 2,
      text: 'Lũy kế FDI ngoài Khu công nghiệp'
    },
    {
      id: 3,
      text: 'Thu hồi lũy kế'
    },
    {
      id: 4,
      text: 'Mua bán góp vốn'
    }
  ]

  //MaxDate
  public maxDate: string;
  constructor(
    private fb: FormBuilder,
    public cateProjectPageService: CateProjectPageService,
    private modalService: NgbModal,
    private http: HttpClient
  ) { }

  ngOnInit(): void {
    this.filterForm();
    this.searchForm();
    this.cateProjectPageService.fetch();
    this.grouping = this.cateProjectPageService.grouping;
    this.paginator = this.cateProjectPageService.paginator;
    this.sorting = this.cateProjectPageService.sorting;
    const sb = this.cateProjectPageService.isLoading$.subscribe((res: any) => this.isLoading = res);
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
    this.cateProjectPageService.setDefaults();
  }

  // filtration
  filterForm() {
    this.filterGroup = this.fb.group({
      projectType: 0
    });
    this.subscriptions.push(
      this.filterGroup.controls.projectType.valueChanges.subscribe(() =>
        this.filter()
      )
    );
  }
  f_currency(value: any, args?: any): any {
    let nbr = Number((value + '').replace(/,|-/g, ''));
    return (nbr + '').replace(/(\d)(?=(\d{3})+(?!\d))/g, '$1,');
  }
  change_value(event: any, ControlName: string) {

    this.filterGroup.controls[ControlName].setValue(event);
    this.filterGroup.updateValueAndValidity();
  }
  filter() {
    const filter: { [key: string]: string } = {};
    const projectType = this.filterGroup.controls['projectType'].value;
    if (projectType != null && projectType > 0) {
      filter['ProjectType'] = projectType.toString();
    }
    
    this.filterValue = filter;
    this.cateProjectPageService.patchState({ filter });
  }
  
  resetFilter() {
    this.filterGroup.controls.projectType.reset(0);
    this.cateProjectPageService.fetch();
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
      this.cateProjectPageService.patchState({ searchTerm });
  }

  onEnter() {
    this.searchData(this.searchGroup.controls.searchTerm.value)
  }

  searchData(searchTerm: string) {
    this.cateProjectPageService.patchState({ searchTerm });
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
    this.cateProjectPageService.patchState({ sorting });
  }

  // pagination
  paginate(paginator: PaginatorState) {
    this.cateProjectPageService.patchState({ paginator });
  }

  // form actions
  create() {
    this.edit(0);
  }

  edit(id: any) {
    const modalRef = this.modalService.open(EditCateProjectModalComponent, { size: 'xl ' });
    modalRef.componentInstance.id = id;
    modalRef.result.then(() =>
      this.cateProjectPageService.fetch(),
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
        const sb = this.cateProjectPageService.delete(id).pipe(
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
    const now = new Date();
    const timeString = now.toLocaleString('en-US', {
      year: 'numeric',
      month: '2-digit', 
      day: '2-digit',
      hour: '2-digit',
      minute: '2-digit',
      second: '2-digit'
    }).replace(/\D/g, '');
    const fileName = "QuanLyDuAnFDIDDI_" + timeString + ".xlsx"
    const query = {
      filter: this.filterValue,
      grouping: {},
      paginator: {},
      sorting: { column: "id", direction: "desc" },
      searchTerm: this.searchGroup.controls.searchTerm.value
    }
      Swal.fire({
        icon: 'info',
      title: 'Đang xuất File...',
      // text: 'Vui lòng đợi một lúc trước khi file của bạn sẵn sàng!',
      didOpen: () => {
        Swal.showLoading()
      },
    })
      this.http.post(`${environment.apiUrl}/CateProject/Export`, query,
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


