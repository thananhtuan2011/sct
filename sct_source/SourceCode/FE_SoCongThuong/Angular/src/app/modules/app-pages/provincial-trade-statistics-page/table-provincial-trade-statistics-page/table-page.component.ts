import { PageInfoService, PageLink } from '../../../../_metronic/layout/core/page-info.service';
import { PaginatorState } from '../../../../_metronic/shared/crud-table/models/paginator.model';
import { ISearchView } from '../../../../_metronic/shared/crud-table/models/search.model';
import { IFilterView } from '../../../../_metronic/shared/crud-table/models/filter.model';
import { ISortView, SortState } from '../../../../_metronic/shared/crud-table/models/sort.model';
import { GroupingState, IGroupingView } from '../../../../_metronic/shared/crud-table/models/grouping.model';
import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { catchError, EMPTY, of, Subscription } from 'rxjs';
import { ProvincialTradeStatisticsPageService } from '../_services/provincial-trade-statistics-page.service';
import { environment } from 'src/environments/environment';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';

import Swal from 'sweetalert2';
import { SearchPipe } from 'src/app/_metronic/shared/pipe/filter-pipe/filter.pipe';

const EMPTY_TOTAL: any = {
  tongCong: 0,
  SLTrungTamThuongMai: 0,
  SLSieuThi: 0,
  SLChoTrongQuyHoach: 0,
  SLChoNgoaiQuyHoach: 0,
  SLChoDem: 0,
  SLChoNoi: 0,
  SLCuaHangTienLoi: 0,
  SLCuaHangTapHoa: 0,
  SLCuaHangChuyenDoanh: 0,
  SLTrungTamLogictis: 0
}

@Component({
  selector: 'app-table-page',
  templateUrl: './table-page.component.html',
  styleUrls: ['./table-page.component.scss'],
})


export class TableProvincialTradeStatisticsPageComponent implements OnInit,
  OnDestroy,
  // IFetchSelectedAction,
  // IUpdateStatusForSelectedAction,
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
  data: any = [];
  dataSearch : any;
  total: any = EMPTY_TOTAL
  private subscriptions: Subscription[] = []; // Read more: => https://brianflove.com/2016/12/11/anguar-2-unsubscribe-observables/

  public parameterValue: string;

  constructor(
    private fb: FormBuilder,
    private provincialTradeStatisticsPageService: ProvincialTradeStatisticsPageService,
    private http: HttpClient,
    private router: Router,
    private searchPipe: SearchPipe
  ) { }

  ngOnInit(): void {
    // this.filterForm();
    this.searchForm();
    this.loadData();
    // this.provincialTradeStatisticsPageService.fetch();
    // this.grouping = this.provincialTradeStatisticsPageService.grouping;
    // this.paginator = this.provincialTradeStatisticsPageService.paginator;
    // this.sorting = this.provincialTradeStatisticsPageService.sorting;
    // const sb = this.provincialTradeStatisticsPageService.isLoading$.subscribe((res: any) => this.isLoading = res);
    // this.subscriptions.push(sb);
  }

  ngOnDestroy() {
    this.subscriptions.forEach((sb) => sb.unsubscribe());
    this.provincialTradeStatisticsPageService.setDefaults();
  }

  loadData() {
    this.provincialTradeStatisticsPageService.loadData().subscribe((res: any) => {
      this.data = res;
      this.onEnter()
    })
  }

  // filtration
  filterForm() {
    // this.filterGroup = this.fb.group({
    //   status: [''],
    //   type: [''],
    //   searchTerm: [''],
    // });
    // this.subscriptions.push(
    //   this.filterGroup.controls.status.valueChanges.subscribe(() =>
    //     this.filter()
    //   )
    // );
    // this.subscriptions.push(
    //   this.filterGroup.controls.type.valueChanges.subscribe(() => this.filter())
    // );
  }

  filter() {
    // const filter: { [key: string]: string } = {};
    // const status = this.filterGroup.controls['status'].value;
    // if (status) {
    //   filter['status'] = status;
    // }
    // this.provincialTradeStatisticsPageService.patchState({ filter });
  }

  // search
  searchForm() {
    this.searchGroup = this.fb.group({
      searchTerm: [''],
    });
    this.dataSearch = this.searchGroup.controls.searchTerm.value;
    const searchEvent = this.searchGroup.controls.searchTerm.valueChanges
      .subscribe((val) => val == '' ? this.onEnter() : '');
    this.subscriptions.push(searchEvent);
  }

  search(searchTerm: string) {
    // this.provincialTradeStatisticsPageService.patchState({ searchTerm });
  }

  onEnter() {
    this.resetTotal()
    this.dataSearch = this.searchGroup.controls.searchTerm.value;
    const _data = this.searchPipe.transform(this.data, this.dataSearch, ['districtId'])
    _data.forEach((item: any) => {
      this.total.tongCong += item.total,
      this.total.SLTrungTamThuongMai += item.slTrungTamThuongMai
      this.total.SLSieuThi += item.slSieuThi
      this.total.SLChoTrongQuyHoach += item.slChoTrongQuyHoach
      this.total.SLChoNgoaiQuyHoach += item.slChoNgoaiQuyHoach
      this.total.SLChoDem += item.slChoDem
      this.total.SLChoNoi += item.slChoNoi
      this.total.SLCuaHangTienLoi += item.slCuaHangTienLoi
      this.total.SLCuaHangTapHoa += item.slCuaHangTapHoa
      this.total.SLCuaHangChuyenDoanh += item.slCuaHangChuyenDoanh
      this.total.SLTrungTamLogictis += item.slTrungTamLogictis
    })
  }
  
  resetTotal(){
    this.total.tongCong = 0
    this.total.SLTrungTamThuongMai = 0
    this.total.SLSieuThi = 0,
    this.total.SLChoTrongQuyHoach = 0
    this.total.SLChoNgoaiQuyHoach = 0
    this.total.SLChoDem = 0
    this.total.SLChoNoi = 0
    this.total.SLCuaHangTienLoi = 0
    this.total.SLCuaHangTapHoa = 0
    this.total.SLCuaHangChuyenDoanh = 0
    this.total.SLTrungTamLogictis = 0
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
    this.provincialTradeStatisticsPageService.patchState({ sorting });
  }

  // pagination
  paginate(paginator: PaginatorState) {
    this.provincialTradeStatisticsPageService.patchState({ paginator });
  }

  getTotal(column_name: any) {
    return this.data.reduce((sum: any, item: any) => sum + item[column_name], 0);
  }

  exportFile() {
    const moment = require("moment");
    const timeString = moment().format("DDMMYYYYHHmmss");
    const fileName = "Thongkethongtinchosieuthitrungtamthuongmaitinh_" + timeString + ".xlsx"

    Swal.fire({
            icon: 'info',
      title: 'Đang xuất File...',
      // text: 'Vui lòng đợi một lúc trước khi file của bạn sẵn sàng!',
      didOpen: () => {
        Swal.showLoading()
      },
    })

    this.http.get(`${environment.apiUrl}/Statistical/ExportByProvince`, {
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

  getHeight(): any {
		let tmp_height = 0;
		tmp_height = window.innerHeight - 300;
		return tmp_height + 'px';
	}

  goToDistrictPage(id: string){
    this.router.navigate(['/DistrictTradeStatistics'], { queryParams: {id : id}});
  }
}


