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
import { ParticipateTradePromotionService } from '../_services/participate-trade-promotion-page.service';
import { InfoParticipateTradePromotionComponent } from './components/info-participate-trade-promotion-model/info-modal.component';
import { environment } from 'src/environments/environment';
import { HttpClient } from '@angular/common/http';
import { EditIndustrialPromotionProjectModalComponent } from '../../industrial-promotion-project-page/table-industrial-promotion-project-page/components/edit-industrial-promotion-project-modal/edit-modal.component';
import { Options } from 'select2';


@Component({
  selector: 'app-table-participate-trade-promotion-page.component',
  templateUrl: './table-participate-trade-promotion-page.component.html'
})
export class TableParticipateTradePromotionsComponent implements OnInit,
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
  options: Options
  sorting: SortState;
  grouping: GroupingState;
  isLoading: boolean;
  filterGroup: FormGroup;
  searchGroup: FormGroup;
  private subscriptions: Subscription[] = []; // Read more: => https://brianflove.com/2016/12/11/anguar-2-unsubscribe-observables/
  //MinDate
  public minDate: string;

  //MaxDate
  public maxDate: string;
  
  typeBusiness: any = [
    {
      id: 0,
      text: '-- Tất cả --'
    },
    {
      id: 1,
      text: 'Trong tỉnh'
    },
    {
      id: 2,
      text: 'Ngoài tỉnh'
    }
  ]
  
  filterValue: any = {}
  constructor(
    private fb: FormBuilder,
    public participateTradePromotionService: ParticipateTradePromotionService,
    private modalService: NgbModal,
    private http: HttpClient,
  ) { }
  create(): void {
    this.edit(0);
  }

  edit(id: any) {
    const modalRef = this.modalService.open(EditIndustrialPromotionProjectModalComponent, { size: 'xl' });
    modalRef.componentInstance.id = id;
    modalRef.result.then(() =>
      this.participateTradePromotionService.fetch(),
      () => { }
    );
  }
  
  ngOnInit(): void {
    this.filterForm();
    this.searchForm();
    this.participateTradePromotionService.fetch();
    this.grouping = this.participateTradePromotionService.grouping;
    this.paginator = this.participateTradePromotionService.paginator;
    this.sorting = this.participateTradePromotionService.sorting;
    const sb = this.participateTradePromotionService.isLoading$.subscribe((res: any) => this.isLoading = res);
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
    this.participateTradePromotionService.setDefaults();
  }

  // filtration
  filterForm() {
    this.filterGroup = this.fb.group({
      TypeBusiness: 0
    });
    this.subscriptions.push(
      this.filterGroup.controls.TypeBusiness.valueChanges.subscribe(() =>
        this.filter()
      )
    );
  }

  change_value(event: any, ControlName: string) {

    this.filterGroup.controls[ControlName].setValue(event);
    this.filterGroup.updateValueAndValidity();
  }
  filter() {

    const filter: { [key: string]: string } = {};
    const TypeBusiness = this.filterGroup.controls['TypeBusiness'].value;
    if (TypeBusiness != null && TypeBusiness > 0) {
      filter['TypeBusiness'] = TypeBusiness.toString();
    }
    this.filterValue = filter;
    this.participateTradePromotionService.patchState({ filter });
  }
  
  clearFilter(){
    this.filterGroup.controls.TypeBusiness.setValue(0);
    this.filter();
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
      this.participateTradePromotionService.patchState({ searchTerm });
  }

  onEnter() {
    this.searchData(this.searchGroup.controls.searchTerm.value)
  }
  
  searchData(searchTerm: string) {
    this.participateTradePromotionService.patchState({ searchTerm });
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
    this.participateTradePromotionService.patchState({ sorting });
  }

  // pagination
  paginate(paginator: PaginatorState) {
    this.participateTradePromotionService.patchState({ paginator });
  }

  // form actions
  // edit(id: any) {
  //   const modalRef = this.modalService.open(InfoParticipateTradePromotionComponent, { size: '150px' });
  //   modalRef.componentInstance.id = id;
  //   modalRef.result.then(() =>
  //     this.participateTradePromotionService.fetch(),
  //     () => { }
  //   );
  // }

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
        const sb = this.participateTradePromotionService.deleteRP(id).pipe(
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
  view(id: any) {
    const modalRef = this.modalService.open(InfoParticipateTradePromotionComponent, { size: 'xl' });
    modalRef.componentInstance.id = id;
    modalRef.result.then(() =>
      this.participateTradePromotionService.fetch(),
      () => { }
    );
  }

  updateStatusForSelected() {
  }

  fetchSelected() {
  }

  exportFile() {
    const now = new Date();
    const timeString = now.toLocaleString('en-US', {
      day: '2-digit',
      month: '2-digit',
      year: 'numeric',
      hour: '2-digit',
      minute: '2-digit',
      second: '2-digit'
    }).replace(/\D/g, '');
    const fileName = "DanhsachdoanhnghiepthamgiachuongtrinhXTTM_" + timeString + ".xlsx"

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
    this.http.post(`${environment.apiUrl}/ParticipateTradePromotion/export`, query, {
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
        console.log(`File ${fileName} đã tải xong.`);
        Swal.fire({
          icon: 'success',
          title: 'Xuất File thành công',
          confirmButtonText: 'Xác nhận',
        });
      },
    );
  }
}


