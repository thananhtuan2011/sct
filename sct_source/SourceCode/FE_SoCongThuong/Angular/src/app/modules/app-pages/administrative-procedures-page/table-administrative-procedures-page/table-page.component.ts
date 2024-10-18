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
import { AdministrativeProceduresPageService } from '../_services/administrative-procedures-page.service';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { EditAdministrativeProceduresModalComponent } from './components/edit-administrative-procedures-modal/edit-modal.component';
import Swal from 'sweetalert2';
import { Options } from 'select2';
import { environment } from 'src/environments/environment';
import { HttpClient } from '@angular/common/http';


@Component({
  selector: 'app-table-page',
  templateUrl: './table-page.component.html',
  styleUrls: ['./table-page.component.scss']
})

export class TableAdministrativeProceduresPageComponent implements OnInit,
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
  options: Options;
  filterValue: any = {}

  StatusData: any = [
    {
      id: 0,
      text: "-- Chọn --"
    },
    {
      id: 1,
      text: "Chưa xử lý"
    },
    {
      id: 2,
      text: "Đang xử lý"
    },
    {
      id: 3,
      text: "Đã xử lý"
    },
  ]
  Status: any = 0;

  ReceptionFormData: any = [
    {
      id: 0,
      text: "-- Chọn --"
    },
    {
      id: 1,
      text: "Trực tiếp"
    },
    {
      id: 2,
      text: "Trực tuyến"
    },
  ]
  ReceptionForm: any = 0;
  AdministrativeProceduresFieldData: any;
  AdministrativeProceduresFieldId: string = '00000000-0000-0000-0000-000000000000';


  private subscriptions: Subscription[] = []; // Read more: => https://brianflove.com/2016/12/11/anguar-2-unsubscribe-observables/

  constructor(
    private fb: FormBuilder,
    public administrativeProceduresPageService: AdministrativeProceduresPageService,
    private modalService: NgbModal,
    private http: HttpClient
  ) { }

  ngOnInit(): void {
    this.loadFieldData();
    this.filterForm();
    this.searchForm();
    this.administrativeProceduresPageService.fetch();
    this.grouping = this.administrativeProceduresPageService.grouping;
    this.paginator = this.administrativeProceduresPageService.paginator;
    this.sorting = this.administrativeProceduresPageService.sorting;
    const sb = this.administrativeProceduresPageService.isLoading$.subscribe((res: any) => this.isLoading = res);
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

  loadFieldData() {
    this.administrativeProceduresPageService.loadField().subscribe((res: any) => {
      const field_data = [
        {
          id: '00000000-0000-0000-0000-000000000000',
          text: "-- Chọn --",
          piority: 0
        },
      ]

      for (var i of res.items) {
        let obj = {
          id: i.categoryId,
          text: i.categoryName,
          piority: i.piority,
        };
        field_data.push(obj);
      }

      this.AdministrativeProceduresFieldData = field_data.sort((i1, i2) => {
        if (i1.piority > i2.piority) {
          return 1;
        }
        if (i1.piority < i2.piority) {
          return -1;
        }
        return 0;
      });
    })
  }

  ngOnDestroy() {
    this.subscriptions.forEach((sb) => sb.unsubscribe());
    this.administrativeProceduresPageService.setDefaults();
  }

  // filtration
  filterForm() {
    this.filterGroup = this.fb.group({
      AdministrativeProceduresFieldId: [this.AdministrativeProceduresFieldId],
      Status: [this.Status],
      ReceptionForm: [this.ReceptionForm],
    });
    this.subscriptions.push(
      this.filterGroup.controls.AdministrativeProceduresFieldId.valueChanges.subscribe(() =>
        this.filter()
      )
    );
    this.subscriptions.push(
      this.filterGroup.controls.Status.valueChanges.subscribe(() =>
        this.filter()
      )
    );
    this.subscriptions.push(
      this.filterGroup.controls.ReceptionForm.valueChanges.subscribe(() =>
        this.filter()
      )
    );
  }

  filter() {
    const filter: { [key: string]: string } = {};
    const AdministrativeProceduresField = this.filterGroup.controls['AdministrativeProceduresFieldId'].value;
    const Status = this.filterGroup.controls['Status'].value;
    const ReceptionForm = this.filterGroup.controls['ReceptionForm'].value;
    if (AdministrativeProceduresField != '00000000-0000-0000-0000-000000000000') {
      filter['AdministrativeProceduresField'] = AdministrativeProceduresField;
    }
    if (Status != 0) {
      filter['Status'] = Status;
    }
    if (ReceptionForm != 0) {
      filter['ReceptionForm'] = ReceptionForm;
    }
    this.filterValue = filter
    this.administrativeProceduresPageService.patchState({ filter });
  }
  
  resetFilter() {
    this.filterGroup.controls.AdministrativeProceduresFieldId.reset("00000000-0000-0000-0000-000000000000");
    this.filterGroup.controls.Status.reset(0);
    this.filterGroup.controls.ReceptionForm.reset(0);
    
    this.administrativeProceduresPageService.fetch();
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
      this.administrativeProceduresPageService.patchState({ searchTerm });
  }

  onEnter() {
    this.searchData(this.searchGroup.controls.searchTerm.value)
  }

  searchData(searchTerm: string) {
    this.administrativeProceduresPageService.patchState({ searchTerm });
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
    this.administrativeProceduresPageService.patchState({ sorting });
  }

  // pagination
  paginate(paginator: PaginatorState) {
    this.administrativeProceduresPageService.patchState({ paginator });
  }

  // form actions
  create() {
    this.edit(0);
  }

  edit(id: any) {
    const modalRef = this.modalService.open(EditAdministrativeProceduresModalComponent, { size: 'lg' });
    modalRef.componentInstance.id = id;
    modalRef.result.then(() =>
      this.administrativeProceduresPageService.fetch(),
      () => { }
    );
  }

  view(id: any) {
    const modalRef = this.modalService.open(EditAdministrativeProceduresModalComponent, { size: 'lg' });
    modalRef.componentInstance.id = id;
    modalRef.componentInstance.type = 'view';
    modalRef.result.then(() =>
      this.administrativeProceduresPageService.fetch(),
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
        const sb = this.administrativeProceduresPageService.delete(id).pipe(
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
        const sb = this.administrativeProceduresPageService.deletes(this.grouping.getSelectedRows()).pipe(
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

    this.http.post(`${environment.apiUrl}/AdministrativeProcedures/Export`, query,
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


