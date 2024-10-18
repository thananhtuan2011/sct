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
import { RPIndustrialPageService } from '../_services/RP-industrial-page.service';
import { EditRPIndustrialModalComponent } from './components/edit-RP-industrial-modal/edit-RP-industrial-modal.component';
import * as moment from 'moment';
import { Options } from 'select2';
import { SelectOptionData } from 'src/app/_metronic/shared/components/select-custom/select-custom.interface';
import { environment } from 'src/environments/environment';
import { HttpClient } from '@angular/common/http';
import { CommonService } from 'src/app/_metronic/shared/services/common.service';


@Component({
  selector: 'app-table-rp-industrial-page',
  templateUrl: './table-RP-industrial-page.component.html'
})
export class TableRPIndustrialComponent implements OnInit,
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
  groupType: any = [];
  districtData: any = [];
  private subscriptions: Subscription[] = []; // Read more: => https://brianflove.com/2016/12/11/anguar-2-unsubscribe-observables/
  //MinDate
  public minDate: string;
  yearData: any = [];
  filterValue : any = {}
  public year: any = moment().year();
  options: Options;
  public reportingPeriod: "0";
  //MaxDate
  public maxDate: string;
  public datKyBaoCao: Array<SelectOptionData>;
  currentURL='';
  constructor(
    private fb: FormBuilder,
    public rPIndustrialPageService: RPIndustrialPageService,
    private modalService: NgbModal,
    private http: HttpClient,
    private commonService: CommonService
    
  ) { }

  ngOnInit(): void {
    this.loadYear();
    this.loadDistrict();
    this.loadGroupType();
    this.loadKyBaoCao();
    this.filterForm();
    this.searchForm();
    this.rPIndustrialPageService.fetch();
    this.grouping = this.rPIndustrialPageService.grouping;
    this.paginator = this.rPIndustrialPageService.paginator;
    this.sorting = this.rPIndustrialPageService.sorting;
    const sb = this.rPIndustrialPageService.isLoading$.subscribe((res: any) => this.isLoading = res);
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
    this.rPIndustrialPageService.setDefaults();
  }

  // filtration
  filterForm() {
    this.filterGroup = this.fb.group({
      reportingPeriod: [this.reportingPeriod],
      year: [this.year],
      District: '00000000-0000-0000-0000-000000000000'
    });
    this.subscriptions.push(
      this.filterGroup.controls.reportingPeriod.valueChanges.subscribe(() =>
        this.filter()
      )
    );
    this.subscriptions.push(
      this.filterGroup.controls.year.valueChanges.subscribe(() =>
        this.filter()
      )
    );
    this.subscriptions.push(
      this.filterGroup.controls.District.valueChanges.subscribe(() =>
        this.filter()
      )
    );
  }

  change_value(event: any, ControlName: string) {
    this.filterGroup.controls[ControlName].setValue(event);
    this.filterGroup.updateValueAndValidity();
  }

  f_currency(value: any, args?: any): any {
    let nbr = Number((value + '').replace(/,|-/g, ''));
    return (nbr + '').replace(/(\d)(?=(\d{3})+(?!\d))/g, '$1,');
  }

  filter() {
    const filter: { [key: string]: string } = {};
    const reportingPeriod = this.filterGroup.controls['reportingPeriod'].value;
    const year = this.filterGroup.controls['year'].value;
    const district = this.filterGroup.controls['District'].value;
    if (reportingPeriod != null && reportingPeriod != "0") {
      filter['ReportingPeriod'] = (reportingPeriod);
    }

    if (year != null ) {
      filter['Year'] = year.toString();
    }else{
      filter['Year'] = this.year.toString();
    }
    
    if(district != '00000000-0000-0000-0000-000000000000'){
      filter['District'] = district;
    }
    this.filterValue = filter;
    this.rPIndustrialPageService.patchState({ filter });
  }
  
  resetFilter() {
    this.filterGroup.controls.reportingPeriod.reset("0");
    this.filterGroup.controls.year.reset(this.year.toString());
    this.filterGroup.controls.District.reset('00000000-0000-0000-0000-000000000000');
    this.rPIndustrialPageService.fetch();
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
      this.rPIndustrialPageService.patchState({ searchTerm });
  }

  onEnter() {
    this.searchData(this.searchGroup.controls.searchTerm.value)
  }

  searchData(searchTerm: string) {
    this.rPIndustrialPageService.patchState({ searchTerm });
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
    this.rPIndustrialPageService.patchState({ sorting });
  }

  // pagination
  paginate(paginator: PaginatorState) {
    this.rPIndustrialPageService.patchState({ paginator });
  }

  // form actions
  create() {
    this.edit(0);
  }

  edit(id: any) {
    const modalRef = this.modalService.open(EditRPIndustrialModalComponent, { size: '500px' });
    modalRef.componentInstance.id = id;
    modalRef.componentInstance.groupType = this.groupType;
    modalRef.componentInstance.districtData = this.districtData;
    modalRef.result.then(() =>
      this.rPIndustrialPageService.fetch(),
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
        const sb = this.rPIndustrialPageService.deleteRP(id).pipe(
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
    const fileName = "BaoCaoTongHopTinhHinhCumCongNghiep_" + timeString + ".xlsx"
    const query = {
      filter: this.filterValue,
      grouping: {},
      paginator: {},
      sorting: { column: "id", direction: "desc" },
      searchTerm: this.searchGroup.controls.searchTerm.value
    }
    if(!query.filter.ReportingPeriod){
      Swal.fire({
        icon: 'warning',
        title: 'Vui lòng chọn kỳ báo cáo!',
        confirmButtonText: 'Xác nhận',
      });
    }else{
      Swal.fire({
        icon: 'info',
      title: 'Đang xuất File...',
      // text: 'Vui lòng đợi một lúc trước khi file của bạn sẵn sàng!',
      didOpen: () => {
        Swal.showLoading()
      },
    })
      this.http.post(`${environment.apiUrl}/ReportIndustrialClusters/Export`, query,
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
  
  loadKyBaoCao() {
    const data = [{
      id: "0",
      text: '-- Chọn --'
    }];
    let obj1 = {
      id: "1",
      text: "6 tháng đầu năm",
    };
    let obj2 = {
      id: "2",
      text: "Cả năm",
    }
    data.push(obj1);
    data.push(obj2);
    this.datKyBaoCao = data;
    return this.datKyBaoCao;
  }
  
  loadGroupType(){
    this.commonService.GetConfig("GROUP_TARGET").subscribe((res: any) => {
      const data = [{
        id: "00000000-0000-0000-0000-000000000000",
        text: '-- Chọn --'
      },
      ...res.items.listConfig.map((item: any) => ({
        id: item.categoryId,
        text: item.categoryName
      }))]
      this.groupType = data;
    })
  }
  
  loadDistrict(){
    this.commonService.getDistrict().subscribe((res: any) => {
      const data = [{
        id: "00000000-0000-0000-0000-000000000000",
        text: '-- Chọn --'
      },
      ...res.items.map((item: any) => ({
        id: item.districtId,
        text: item.districtName
      }))]
      this.districtData = data;
    })
  }
  
}


