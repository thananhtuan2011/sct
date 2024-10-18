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
import { AdminFormalitiesPageService } from '../_services/adminformalities-page.service';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { EditAdminFormalitiesModalComponent } from './components/edit-adminformalities-modal/edit-adminformalities-modal.component';
import Swal from 'sweetalert2';
import { Options } from 'select2';
import { environment } from 'src/environments/environment';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-table-adminformalities-page',
  templateUrl: './table-adminformalities-page.component.html'
})
export class TableAdminFormalitiesPageComponent implements OnInit,
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
  FieldData: { id: string; text: string; priority: number; }[];
  DVCLevelData: any = [
    {
      id: 0,
      text: '-- Chọn --',
    },
    {
      id: 1,
      text: 'Toàn trình'
    },
    {
      id: 2,
      text: 'Còn lại',
    },
  ];
  options: Options;
  filterValue: any = {};
  
  constructor(
    private fb: FormBuilder,
    public adminformalitiesPageService: AdminFormalitiesPageService,
    private modalService: NgbModal,
    private http: HttpClient,
    ) {}

  ngOnInit(): void {
    this.filterForm();
    this.searchForm();
    this.loadFields();
    this.adminformalitiesPageService.fetch();
    this.grouping = this.adminformalitiesPageService.grouping;
    this.paginator = this.adminformalitiesPageService.paginator;
    this.sorting = this.adminformalitiesPageService.sorting;
    const sb = this.adminformalitiesPageService.isLoading$.subscribe((res:any) => this.isLoading = res);
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

  loadFields() {
    this.adminformalitiesPageService.loadField().subscribe((res: any) => {
      var fields = [
        {
          id: '00000000-0000-0000-0000-000000000000',
          text: '-- Chọn --',
          priority: 0,
        },
      ];
      for (var item of res.items) {
        let field = {
          id: item.fieldId,
          text: item.fieldName,
          priority: item.priority,
        }
        fields.push(field)
      }
      this.FieldData = fields.sort((i1, i2) => {
        if (i1.priority > i2.priority) {
          return 1;
        }
        if (i1.priority < i2.priority) {
          return -1;
        }
        return 0;
      })
    })
  }

  ngOnDestroy() {
    this.subscriptions.forEach((sb) => sb.unsubscribe());
    this.adminformalitiesPageService.setDefaults();
  }

  // filtration
  filterForm() {
    this.filterGroup = this.fb.group({
      FieldId: ['00000000-0000-0000-0000-000000000000'],
      DVCLevel: [0]
    });
    this.subscriptions.push(
      this.filterGroup.controls.FieldId.valueChanges.subscribe(() =>
        this.filter()
      )
    );
    this.subscriptions.push(
      this.filterGroup.controls.DVCLevel.valueChanges.subscribe(() =>
        this.filter()
      )
    );
  }

  filter() {
    const filter: {[key:string]: string} = {};
    const FieldId = this.filterGroup.controls['FieldId'].value;
    if (FieldId != '00000000-0000-0000-0000-000000000000') {
      filter['FieldId'] = FieldId;
    }
    const DVCLevel = this.filterGroup.controls['DVCLevel'].value;
    if (DVCLevel != 0) {
      filter['DVCLevel'] = DVCLevel;
    }
    this.filterValue = filter;
    this.adminformalitiesPageService.patchState({ filter });
  }

  resetFilter() {
    this.filterGroup.controls.FieldId.reset("00000000-0000-0000-0000-000000000000");
    this.filterGroup.controls.DVCLevel.reset(0);
    this.adminformalitiesPageService.fetch();
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
      this.adminformalitiesPageService.patchState({ searchTerm });
  }

  onEnter() {
    this.searchData(this.searchGroup.controls.searchTerm.value)
  }

  searchData(searchTerm: string) {
    this.adminformalitiesPageService.patchState({ searchTerm });
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
    this.adminformalitiesPageService.patchState({ sorting });
  }

  // pagination
  paginate(paginator: PaginatorState) {
    this.adminformalitiesPageService.patchState({ paginator });
  }

  // form actions
  create() {
    this.edit(0);
  }

  edit(id: any) {
    const modalRef = this.modalService.open(EditAdminFormalitiesModalComponent, { size: '100px' });
    modalRef.componentInstance.id = id;
    modalRef.result.then(() =>
      this.adminformalitiesPageService.fetch(),
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
        const sb = this.adminformalitiesPageService.deleteAdminFormality(id).pipe(
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
        const sb = this.adminformalitiesPageService.deleteAdminFormalities(this.grouping.getSelectedRows()).pipe(
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
    const fileName = "DanhSachThuTucHanhChinh_" + timeString + ".xlsx"

    Swal.fire({
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

    this.http.post(`${environment.apiUrl}/adminformalities/Export`, query,
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


