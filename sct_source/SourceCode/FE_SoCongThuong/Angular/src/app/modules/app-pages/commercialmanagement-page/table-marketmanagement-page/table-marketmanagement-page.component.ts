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
import { MarketManagementPageService } from '../_services/marketmanagement-page.service';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { EditMarketManagementModalComponent } from './components/edit-marketmanagement-modal/edit-marketmanagement-modal.component';
import Swal from 'sweetalert2';
import { Options } from 'select2';
import { environment } from 'src/environments/environment';
import { HttpClient } from '@angular/common/http';



@Component({
  selector: 'app-table-marketmanagement-page',
  templateUrl: './table-marketmanagement-page.component.html',
  styleUrls: ['./table-marketmanagement-page.component.scss'],
})

export class TableMarketManagementPageComponent implements OnInit,
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
  public options: Options;
  public districtData: Array<any>;
  public districtId: any = '00000000-0000-0000-0000-000000000000';
  public communeData: Array<any>;
  public communeDataFilter: Array<any>;
  public communeId: any = '00000000-0000-0000-0000-000000000000';
  public marketData: Array<any>;
  public marketDataFilter: Array<any>;
  public marketId: any = '00000000-0000-0000-0000-000000000000';
  filterValue: { [key: string]: string; } = {};
  private subscriptions: Subscription[] = []; // Read more: => https://brianflove.com/2016/12/11/anguar-2-unsubscribe-observables/

  constructor(
    private fb: FormBuilder,
    public marketmanagementPageService: MarketManagementPageService,
    private modalService: NgbModal,
    private http: HttpClient
  ) { }

  ngOnInit(): void {
    this.loaddistrict();
    this.loadcommune();
    this.loadmarket();
    this.filterForm();
    this.searchForm();
    this.marketmanagementPageService.fetch();
    this.grouping = this.marketmanagementPageService.grouping;
    this.paginator = this.marketmanagementPageService.paginator;
    this.sorting = this.marketmanagementPageService.sorting;
    const sb = this.marketmanagementPageService.isLoading$.subscribe((res: any) => this.isLoading = res);
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
    this.marketmanagementPageService.setDefaults();
  }

  f_currency(value: any, args?: any): any {
    let nbr = Number((value + '').replace(/,|-/g, ""));
    const result = (nbr + '').replace(/(\d)(?=(\d{3})+(?!\d))/g, "$1,");
    return result
  }

  //load data
  loaddistrict() {
    this.marketmanagementPageService.loadDistrict().subscribe((res: any) => {
      var districts = [
        {
          id: '00000000-0000-0000-0000-000000000000',
          text: '-- Chọn Huyện --',
        },
      ];
      for (var item of res.items) {
        let obj_district = {
          id: item.districtId,
          text: item.districtName,
        }
        districts.push(obj_district)
      }
      this.districtData = districts.sort((a, b) => {
        if (a.text < b.text) {
          return -1;
        }
        if (a.text > b.text) {
          return 1;
        }
        return 0;
      });
    })
  }

  changedistrict(event: any) {
    if (event != '00000000-0000-0000-0000-000000000000') {
      // this.filterGroup.controls['DistrictId'].setValue(event)
      var result_commune = this.communeData.filter(x => x.districtId == event || x.id == '00000000-0000-0000-0000-000000000000')
      this.communeDataFilter = result_commune
    }
    else {
      this.communeDataFilter = this.communeData
      this.marketDataFilter = this.marketData
    }
    this.filterGroup.controls['CommuneId'].setValue('00000000-0000-0000-0000-000000000000')
    this.filterGroup.controls['MarketId'].setValue('00000000-0000-0000-0000-000000000000')
    this.filterGroup.updateValueAndValidity()
  }

  loadcommune() {
    this.marketmanagementPageService.loadCommune().subscribe((res: any) => {
      var communes = [
        {
          id: '00000000-0000-0000-0000-000000000000',
          text: '-- Chọn Xã --',
        },
      ];
      for (var item of res.items) {
        let obj_commune = {
          id: item.communeId,
          text: item.communeName,
          districtId: item.districtId,
        }
        communes.push(obj_commune)
      }
      this.communeData = communes.sort((a, b) => {
        if (a < b) {
          return -1;
        }
        if (a > b) {
          return 1;
        }
        return 0;
      });
      this.communeDataFilter = communes.sort((a, b) => {
        if (a < b) {
          return -1;
        }
        if (a > b) {
          return 1;
        }
        return 0;
      });
    })
  }

  changecommune(event: any) {
    if (event != '00000000-0000-0000-0000-000000000000') {
      this.filterGroup.controls["DistrictId"].setValue(this.communeData.find(x => x.id == event).districtId)
      var result_market = this.marketData.filter(x => x.communeId == event || x.id == '00000000-0000-0000-0000-000000000000')
      this.marketDataFilter = result_market
    }
    else {
      this.changedistrict(this.filterGroup.controls['DistrictId'].value)
    }
    this.filterGroup.controls['MarketId'].setValue('00000000-0000-0000-0000-000000000000')
    this.filterGroup.updateValueAndValidity()
  }

  loadmarket() {
    this.marketmanagementPageService.loadMarket().subscribe((res: any) => {
      var markets = [
        {
          id: '00000000-0000-0000-0000-000000000000',
          text: '-- Chọn Chợ --',
        },
      ];
      for (var item of res.items) {
        let obj_market = {
          id: item.marketId,
          text: item.marketName,
          districtId: item.districtId,
          communeId: item.communeId,
        }
        markets.push(obj_market)
      }
      this.marketData = markets
      this.marketDataFilter = markets
    })
  }

  // filtration
  filterForm() {
    this.filterGroup = this.fb.group({
      DistrictId: [this.districtId],
      CommuneId: [this.communeId],
      MarketId: [this.marketId],
    });
    this.subscriptions.push(
      this.filterGroup.controls.DistrictId.valueChanges.subscribe(() => this.filter())
    );
    this.subscriptions.push(
      this.filterGroup.controls.CommuneId.valueChanges.subscribe(() => this.filter())
    );
    this.subscriptions.push(
      this.filterGroup.controls.MarketId.valueChanges.subscribe(() => this.filter())
    );
  }

  filter() {
    const filter: { [key: string]: string } = {};
    const districtid = this.filterGroup.controls['DistrictId'].value;
    const communeid = this.filterGroup.controls['CommuneId'].value;
    const marketid = this.filterGroup.controls['MarketId'].value;
    if (districtid != '00000000-0000-0000-0000-000000000000') {
      filter['DistrictId'] = districtid;
    }
    if (communeid != '00000000-0000-0000-0000-000000000000') {
      filter['CommuneId'] = communeid;
    }
    if (marketid != '00000000-0000-0000-0000-000000000000') {
      filter['MarketId'] = marketid;
    }
    this.filterValue = filter;
    this.marketmanagementPageService.patchState({ filter });
  }

  clearFilter(){
    this.filterGroup.controls.DistrictId.setValue("00000000-0000-0000-0000-000000000000");
    this.filterGroup.controls.CommuneId.setValue("00000000-0000-0000-0000-000000000000");
    this.filterGroup.controls.MarketId.setValue("00000000-0000-0000-0000-000000000000");
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
    if(searchTerm.length == 0)
      this.marketmanagementPageService.patchState({ searchTerm });
  }

  onEnter() {
    this.searchData(this.searchGroup.controls.searchTerm.value)
  }

  searchData(searchTerm: string) {
    this.marketmanagementPageService.patchState({ searchTerm });
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
    this.marketmanagementPageService.patchState({ sorting });
  }

  // pagination
  paginate(paginator: PaginatorState) {
    this.marketmanagementPageService.patchState({ paginator });
  }

  // form actions
  create() {
    this.edit(0);
  }

  edit(id: any) {
    const modalRef = this.modalService.open(EditMarketManagementModalComponent, { size: 'lg' });
    modalRef.componentInstance.id = id;
    modalRef.result.then(() =>
      this.marketmanagementPageService.fetch(),
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
        const sb = this.marketmanagementPageService.deleteMarketManagement(id).pipe(
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
        const sb = this.marketmanagementPageService.deleteMarketManagements(this.grouping.getSelectedRows()).pipe(
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

  view(id: any){
    const modalRef = this.modalService.open(EditMarketManagementModalComponent, { size: 'lg' });
    modalRef.componentInstance.id = id;
    modalRef.componentInstance.type = 'view'
    modalRef.result.then(() =>
      this.marketmanagementPageService.fetch(),
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
    const fileName = "Danhsachthongtincho_" + timeString + ".xlsx"

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

    this.http.post(`${environment.apiUrl}/MarketManagement/ExportExcel`, query,
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


