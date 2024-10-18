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
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import Swal from 'sweetalert2';

import { MarketTargetSevenPageService } from '../_services/market-target-seven-page.service';
import { EditMarketTargetSevenModalComponent } from './components/edit-market-target-seven-modal/edit-modal.component';
import { Options } from 'select2';
import { environment } from 'src/environments/environment';
import { HttpClient } from '@angular/common/http';
import * as moment from 'moment';
import { CommonService } from 'src/app/_metronic/shared/services/common.service';

@Component({
  selector: 'app-table-page',
  templateUrl: './table-page.component.html',
  styleUrls: ['./table-page.component.scss']
})
export class TableMarketTargetSevenPageComponent implements OnInit,
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
  options: Options;
  dateGroup: FormGroup;
  filterGroup: FormGroup;
  searchGroup: FormGroup;
  show: boolean = false;
  firstLoad: boolean = true;
  districtData: any = [];
  communeData: any = [];
  communeByDistrictData = [
    {
      id: '00000000-0000-0000-0000-000000000000',
      text: '-- Chọn --',
      districtId: '00000000-0000-0000-0000-000000000000'
    },
  ]
  
  private subscriptions: Subscription[] = []; // Read more: => https://brianflove.com/2016/12/11/anguar-2-unsubscribe-observables/
  
  Date: string;
  filterValue: { [key: string]: string; } = {};

  constructor(
    private fb: FormBuilder,
    public pageService: MarketTargetSevenPageService,
    private modalService: NgbModal,
    private http: HttpClient,
    private commonService: CommonService
  ) { }

  ngOnInit(): void {
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

  loadCommunData(){
    const find_data = this.communeData.filter((item: any) =>  item.districtId == this.filterGroup.controls['District'].value || item.districtId == '00000000-0000-0000-0000-000000000000')
    this.communeByDistrictData = find_data;
  }
  filterForm() {
    this.filterGroup = this.fb.group({
      District: ['00000000-0000-0000-0000-000000000000'],
      Commune: ['00000000-0000-0000-0000-000000000000'],
      Year: '',
    });
    this.show = true;
    this.subscriptions.push(
      this.filterGroup.controls.District.valueChanges.subscribe(() =>
      {
        this.filter();
        this.loadCommunData()

      })
        
    );
    this.subscriptions.push(
      this.filterGroup.controls.Commune.valueChanges.subscribe(() => this.filter())
    );
    this.subscriptions.push(
      this.filterGroup.controls.Year.valueChanges.subscribe(() => this.filter())
    );
  }

  filter() {
    const filter: { [key: string]: string } = {};
    const district = this.filterGroup.controls['District'].value;
    const commune = this.filterGroup.controls['Commune'].value;
    const year = this.filterGroup.controls['Year'].value;
    if (district && district != "00000000-0000-0000-0000-000000000000") {
      filter['District'] = district;
    }
    if (commune && commune != "00000000-0000-0000-0000-000000000000") {
      filter['Commune'] = commune;
    }
    if (year && year != "" && year !== null) {
      filter['Year'] = year.toString();
    }
    this.filterValue = filter;
    this.pageService.patchState({ filter });
  }
  
  resetFilter() {
    this.filterGroup.controls.District.reset("00000000-0000-0000-0000-000000000000");
    this.filterGroup.controls.Commune.reset("00000000-0000-0000-0000-000000000000");
    this.filterGroup.controls.Year.reset('');
    this.pageService.fetch();
  }

  searchForm() {
    this.searchGroup = this.fb.group({
      searchTerm: [''],
    });
    const searchEvent = this.searchGroup.controls.searchTerm.valueChanges.subscribe((val) => this.search(val));
    this.subscriptions.push(searchEvent);
  }

  search(searchTerm: string) {
    if (searchTerm.length == 0)
      this.pageService.patchState({ searchTerm });
  }

  onBackspace(event: any) {
    if (event.target.value == "") {
      this.searchData("")
    }
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

  edit(item: any) {
    const modalRef = this.modalService.open(EditMarketTargetSevenModalComponent, { size: 'lg' });
    if (item) {
      modalRef.componentInstance.id = item;
      // modalRef.componentInstance.type = item.type;
    }
    modalRef.componentInstance.districtData = this.districtData;
    modalRef.componentInstance.communeData = this.communeData;
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

  updateStatusForSelected() {
  }

  fetchSelected() {
  }
  
  f_currency(value: any, args?: any): any {
    let nbr = Number((value + '').replace(/,|-/g, ""));
    const result = (nbr + '').replace(/(\d)(?=(\d{3})+(?!\d))/g, "$1,");
    return result
  }

  exportFile() {
    const moment = require("moment");
    const timeString = moment().format("DDMMYYYYHHmmss");
    const fileName = "DanhSachCacXaDatTieuChiSo7_" + timeString + ".xlsx"

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
    
    this.http.post(`${environment.apiUrl}/MarketTargetSeven/Export`, query, {
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
  
  loadDistrict(){
    this.subscriptions.push(this.commonService.getDistrict().subscribe((res: any) => {
      const data = [
        {
          id: '00000000-0000-0000-0000-000000000000',
          text: '-- Chọn --'
        },
        ...res.items.map((item: any) => ({
          id: item.districtId,
          text: item.districtName
        }))
      ]
      this.districtData = data;
    }))
  }
  
  loadCommune(){
    this.subscriptions.push(this.commonService.getCommune().subscribe((res: any) => {
      const data = [
        {
          id: '00000000-0000-0000-0000-000000000000',
          text: '-- Chọn --',
          districtId: '00000000-0000-0000-0000-000000000000'
        },
        ...res.items.map((item: any) => ({
          id: item.communeId,
          text: item.communeName,
          districtId: item.districtId
        }))
      ]
      this.communeData = data;
    }))
  }
  
}


