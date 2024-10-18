import { PageInfoService, PageLink } from '../../../../_metronic/layout/core/page-info.service';
import { PaginatorState } from '../../../../_metronic/shared/crud-table/models/paginator.model';
import { ISearchView } from '../../../../_metronic/shared/crud-table/models/search.model';
import { IFilterView } from '../../../../_metronic/shared/crud-table/models/filter.model';
import { ISortView, SortState } from '../../../../_metronic/shared/crud-table/models/sort.model';
import { GroupingState, IGroupingView } from '../../../../_metronic/shared/crud-table/models/grouping.model';
import { ChangeDetectorRef, Component, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { catchError, of, Subscription } from 'rxjs';
import { StatisticalBuildUpgradePageService } from '../_services/statistical-buildupgrade-page.service';
import { environment } from 'src/environments/environment';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';

import Swal from 'sweetalert2';
import { SearchPipe } from 'src/app/_metronic/shared/pipe/filter-pipe/filter.pipe';


@Component({
  selector: 'app-table-page',
  templateUrl: './table-page.component.html',
  styleUrls: ['./table-page.component.scss'],
})

export class TableStatisticalBuildUpgradePageComponent implements OnInit,
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
  dataNotBuildUpgrade :any = [];
  dataDistrictNotBuildUpgrade : any = [];
  districtName : string = '';
  districtId : string = '';
  private subscriptions: Subscription[] = []; // Read more: => https://brianflove.com/2016/12/11/anguar-2-unsubscribe-observables/

  public parameterValue: string;
  dataSearch: any ;
  dataSearchNotBuild: any;
  
  dataTotal: any ={
    tongCho: 0,
    tongVonDautu: 0,
    vonCQSDDat: 0,
    vonDaThucHien: 0,
    vonKhac: 0,
    vonNganSach: 0,
    vonVay: 0
  }

  constructor(
    private fb: FormBuilder,
    private statisticalBuildUpgradePageService: StatisticalBuildUpgradePageService,
    private http: HttpClient,
    private router: Router,
    private cd: ChangeDetectorRef,
    private searchPipe: SearchPipe
  ) { }

  ngOnInit(): void {
    // this.filterForm();
    this.searchForm();
    this.loadData();
    this.loadDataNotBuildUpgrade();
    // this.statisticalBuildUpgradePageService.fetch();
    // this.grouping = this.statisticalBuildUpgradePageService.grouping;
    // this.paginator = this.statisticalBuildUpgradePageService.paginator;
    // this.sorting = this.statisticalBuildUpgradePageService.sorting;
    // const sb = this.statisticalBuildUpgradePageService.isLoading$.subscribe((res: any) => this.isLoading = res);
    // this.subscriptions.push(sb);
  }

  ngOnDestroy() {
    this.subscriptions.forEach((sb) => sb.unsubscribe());
    this.statisticalBuildUpgradePageService.setDefaults();
  }

  loadData() {
    this.statisticalBuildUpgradePageService.loadData().subscribe((res: any) => {
      this.data = res;
      this.onEnter()
      this.cd.detectChanges();
    })
  }

  loadDataNotBuildUpgrade(){
    this.statisticalBuildUpgradePageService.loadDataNotBuildUpgrade().subscribe((res: any) => {
      this.dataNotBuildUpgrade = res;
      this.districtId = res.details[0].districtId
      this.districtName = res.details[0].districtName
      this.getData(this.districtId, this.districtName)
      this.cd.detectChanges();
    })
  }
  
  loadDataDistrictNotBuildUpgrade(id: any){
    this.statisticalBuildUpgradePageService.loadDataDistrictNotBuildUpgrade(id).subscribe((res: any) => {
      this.dataDistrictNotBuildUpgrade = res;
      this.cd.detectChanges();
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
    // this.statisticalBuildUpgradePageService.patchState({ filter });
  }

  // search
  searchForm() {
    this.searchGroup = this.fb.group({
      searchTerm: [''],
      searchTermNotBuild: ['']
    });
    const searchEvent = this.searchGroup.controls.searchTerm.valueChanges
      .subscribe((val) => val == '' ? this.onEnter() : '');
    const search = this.searchGroup.controls.searchTermNotBuild.valueChanges
      .subscribe((val) => val == '' ? this.onEnterNotBuild() : '')
    this.subscriptions.push(searchEvent, search);
  }

  search(searchTerm: string) {
    // this.statisticalBuildUpgradePageService.patchState({ searchTerm });
  }

  onEnter() {
    this.dataTotal = {
      tongCho: 0,
      tongVonDautu: 0,
      vonCQSDDat: 0,
      vonDaThucHien: 0,
      vonKhac: 0,
      vonNganSach: 0,
      vonVay: 0
    }
    this.dataSearch = this.searchGroup.controls.searchTerm.value;
    const _data = this.searchPipe.transform(this.data.details, this.dataSearch, ['maHuyen'])
    _data.forEach((item: any) => {
      this.dataTotal.tongCho += item.tongCho;
      this.dataTotal.tongVonDautu += item.tongVonDautu
      this.dataTotal.vonCQSDDat += item.vonCQSDDat
      this.dataTotal.vonDaThucHien += item.vonDaThucHien
      this.dataTotal.vonKhac += item.vonKhac
      this.dataTotal.vonNganSach += item.vonNganSach
      this.dataTotal.vonVay += item.vonVay
    })
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
    this.statisticalBuildUpgradePageService.patchState({ sorting });
  }

  // pagination
  paginate(paginator: PaginatorState) {
    this.statisticalBuildUpgradePageService.patchState({ paginator });
  }

  getTotal(column_name: any) {
    return this.data.reduce((sum: any, item: any) => sum + item[column_name], 0);
  }

  exportFile() {
    const moment = require("moment");
    const timeString = moment().format("DDMMYYYYHHmmss");
    const fileName = "Thongkexaydungnangcapchosieuthitrungtamthuongmaitinh_" + timeString + ".xlsx"

    Swal.fire({
      icon: 'info',
      title: 'Đang xuất File...',
      // text: 'Vui lòng đợi một lúc trước khi file của bạn sẵn sàng!',
      didOpen: () => {
        Swal.showLoading()
      },
    })

    this.http.get(`${environment.apiUrl}/Statistical/ExportStatisticalHasBeenBuildUpgraded`, {
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
  
  getData(id: string, name: string){
    this.loadDataDistrictNotBuildUpgrade(id);
    this.districtName = name;
    this.districtId = id;
  }
  
  exportFileNotUpgrade(){
    const moment = require("moment");
    const timeString = moment().format("DDMMYYYYHHmmss");
    const fileName = "Thongkechosieuthitrungtamthuongmaichuaxaydungnangcap" + timeString + ".xlsx"

    Swal.fire({
            icon: 'info',
      title: 'Đang xuất File...',
      // text: 'Vui lòng đợi một lúc trước khi file của bạn sẵn sàng!',
      didOpen: () => {
        Swal.showLoading()
      },
    })

    this.http.get(`${environment.apiUrl}/Statistical/ExportStatisticalHasNotBuildUpgraded/${this.districtId}`, {
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
  
  f_currency(value: any, args?: any): any {
    let nbr = Number((value + '').replace(/,|-/g, ""));
    const result = (nbr + '').replace(/(\d)(?=(\d{3})+(?!\d))/g, "$1,");
    return result
  }
  
  onEnterNotBuild(){
    this.dataSearchNotBuild = this.searchGroup.controls.searchTermNotBuild.value;
  }
}


