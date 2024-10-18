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
import { CommitManagerPageService } from '../_services/commit-manager-page.service';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { EditCommitManagerModalComponent } from './components/edit-commit-manager-modal/edit-modal.component';
import Swal from 'sweetalert2';
import { Options } from 'select2';
import * as moment from 'moment';
import { environment } from 'src/environments/environment';
import { HttpClient } from '@angular/common/http';
import { CommonService } from 'src/app/_metronic/shared/services/common.service';


@Component({
  selector: 'app-table-commit-manager-page',
  templateUrl: './table-commit-manager-page.component.html'
})
export class TableCommitManagerPageComponent implements OnInit,
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
  districtData: any;
  filerExport: any = {};
  private subscriptions: Subscription[] = []; // Read more: => https://brianflove.com/2016/12/11/anguar-2-unsubscribe-observables/
  constructor(
    private fb: FormBuilder,
    public commitManager: CommitManagerPageService,
    private modalService: NgbModal,
    private http: HttpClient,
    private commonService: CommonService
  ) { }

  ngOnInit(): void {
    this.loadDistrict();
    this.filterForm();
    this.searchForm();
    this.commitManager.fetch();
    this.grouping = this.commitManager.grouping;
    this.paginator = this.commitManager.paginator;
    this.sorting = this.commitManager.sorting;
    const sb = this.commitManager.isLoading$.subscribe((res: any) => this.isLoading = res);
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
    this.commitManager.setDefaults();
  }

  // filtration
  filterForm() {
    this.filterGroup = this.fb.group({
      Huyen: '00000000-0000-0000-0000-000000000000',
      TuNgayCamKet: null,
      DenNgayCamKet: null,
      TuNgayNhanHoSo: null,
      DenNgayNhanHoSo: null
      //searchTerm: [''],
    });
    this.subscriptions.push(
      this.filterGroup.controls.Huyen.valueChanges.subscribe(() =>
        this.filter()
      )
    );
    this.subscriptions.push(
      this.filterGroup.controls.TuNgayCamKet.valueChanges.subscribe(() =>
        this.filter()
      )
    );
    this.subscriptions.push(
      this.filterGroup.controls.DenNgayCamKet.valueChanges.subscribe(() =>
        this.filter()
      )
    );
    this.subscriptions.push(
      this.filterGroup.controls.TuNgayNhanHoSo.valueChanges.subscribe(() =>
        this.filter()
      )
    );
    this.subscriptions.push(
      this.filterGroup.controls.DenNgayNhanHoSo.valueChanges.subscribe(() =>
        this.filter()
      )
    );
  }

  filter() {
    const filter: { [key: string]: string } = {};
    const huyen = this.filterGroup.controls['Huyen'].value;
    const tuNgayNhanHS = this.filterGroup.controls['TuNgayNhanHoSo'].value;
    const denNgayNhanHS = this.filterGroup.controls['DenNgayNhanHoSo'].value;
    const tuNgayCamKet = this.filterGroup.controls['TuNgayCamKet'].value;
    const denNgayCamKet = this.filterGroup.controls['DenNgayCamKet'].value;
    if(huyen && huyen != '00000000-0000-0000-0000-000000000000'){
      filter['Huyen'] = huyen;
    }
    if(tuNgayCamKet != null){
      filter['TuNgayCamKet'] = tuNgayCamKet;
    }
    if(denNgayCamKet != null){
      filter['DenNgayCamKet'] = tuNgayCamKet;
    }
    if(tuNgayNhanHS != null){
      filter['TuNgayNhanHoSo'] = tuNgayNhanHS;
    }
    if(denNgayNhanHS != null){
      filter['DenNgayNhanHoSo'] = denNgayNhanHS;
    }
    
    this.filerExport = filter;
    this.commitManager.patchState({ filter });
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
      this.commitManager.patchState({ searchTerm });
  }

  onEnter() {
    this.searchData(this.searchGroup.controls.searchTerm.value)
  }

  searchData(searchTerm: string) {
    this.commitManager.patchState({ searchTerm });
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
    this.commitManager.patchState({ sorting });
  }

  // pagination
  paginate(paginator: PaginatorState) {
    this.commitManager.patchState({ paginator });
  }

  // form actions
  create() {
    this.edit(undefined);
  }

  edit(item: any) {
    const modalRef = this.modalService.open(EditCommitManagerModalComponent, { size: 'lg' });
    if(item){
      modalRef.componentInstance.id = item.commitManagerId;
      modalRef.componentInstance.itemData = item;
    }
    modalRef.componentInstance.districtData = this.districtData;
    modalRef.result.then(() =>
      this.commitManager.fetch(),
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
        const sb = this.commitManager.delete(id).pipe(
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
  
  exportFile() {
    const moment = require("moment");
    const timeString = moment().format("DDMMYYYYHHmmss");
    const fileName = "QuanLyCamKetSanXuatKinhDoanhVuaSanXuatVuaKinhDoanh_" + timeString + ".xlsx"

    Swal.fire({
            icon: 'info',
      title: 'Đang xuất File...',
      // text: 'Vui lòng đợi một lúc trước khi file của bạn sẵn sàng!',
      didOpen: () => {
        Swal.showLoading()
      },
    })
    const query = {
      filter: {},
      grouping: {},
      paginator: {},
      sorting: {column: "id", direction: "desc"},
      searchTerm: this.searchGroup.controls.searchTerm.value
    } 
    this.http.post(`${environment.apiUrl}/CommitManager/Export`,query, {
      responseType: 'blob',
    })
    .pipe(
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
        console.log(`File ${fileName} đã tải xong.`);
        Swal.fire({
          icon: 'success',
          title: 'Xuất File thành công',
          confirmButtonText: 'Xác nhận',
        });
      },
    );
  }
  
  view(item: any){
    const modalRef = this.modalService.open(EditCommitManagerModalComponent, { size: 'lg' });
    if(item){
      modalRef.componentInstance.id = item.commitManagerId;
      modalRef.componentInstance.itemData = item;
      modalRef.componentInstance.type = 'view';
      modalRef.componentInstance.districtData = this.districtData;
    }
    modalRef.result.then(() =>
      this.commitManager.fetch(),
      () => { }
    );
  }
  clearFilter(){
    this.filterGroup.controls.Huyen.reset('00000000-0000-0000-0000-000000000000');
    this.filterGroup.controls.TuNgayCamKet.reset(null);
    this.filterGroup.controls.DenNgayCamKet.reset(null);
    this.filterGroup.controls.TuNgayNhanHoSo.reset(null);
    this.filterGroup.controls.DenNgayNhanHoSo.reset(null);
    this.filter();
  }
  
  loadDistrict(){
    const sb = this.commonService.getDistrict().subscribe((res: any) => {
      const data = [
        {
          id: '00000000-0000-0000-0000-000000000000',
          text: '-- Chọn --'
        },
        ...res.items.map((item: any) => ({
          id: item.districtId,
          text: item.districtName
        }))
      ];
      this.districtData = data;
    })
    this.subscriptions.push(sb)
  }
}


