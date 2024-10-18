import { CommonService } from 'src/app/_metronic/shared/services/common.service';
import { PageInfoService, PageLink } from '../../../../_metronic/layout/core/page-info.service';
import { PaginatorState } from '../../../../_metronic/shared/crud-table/models/paginator.model';
import { ISearchView } from '../../../../_metronic/shared/crud-table/models/search.model';
import { IFilterView } from '../../../../_metronic/shared/crud-table/models/filter.model';
import { ISortView, SortState } from '../../../../_metronic/shared/crud-table/models/sort.model';
import { GroupingState, IGroupingView } from '../../../../_metronic/shared/crud-table/models/grouping.model';
import { IFetchSelectedAction, IUpdateStatusForSelectedAction } from '../../../../_metronic/shared/crud-table/models/table.model';
import { ChangeDetectorRef, Component, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { catchError, finalize, Observable, of, Subscription, tap, filter } from 'rxjs';
import { DistrictTradeStatisticsPageService } from '../_services/district-trade-statistics-page.service';
import { Options } from 'select2';
import { AuthService } from 'src/app/modules/auth/services/auth.service';

import { environment } from 'src/environments/environment';
import { HttpClient } from '@angular/common/http';
import Swal from 'sweetalert2';
import { ActivatedRoute } from '@angular/router';
import { SearchPipe } from 'src/app/_metronic/shared/pipe/filter-pipe/filter.pipe';
import { DistrictTradeStatisticsModel } from '../_models/district-trade-statistics.model';


@Component({
  selector: 'app-table-page',
  templateUrl: './table-page.component.html',
  styleUrls: ['./table-page.component.scss'],
})

export class TableDistrictTradeStatisticsPageComponent implements OnInit,
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
  districtData: any = [];
  communeData: any = [];
  communeDataByDistrict: any = [];
  typeOfCategoryData: any = [
    {
      id: "00000000-0000-0000-0000-000000000000",
      text: "-- Chọn --",
    },
    {
      id: "M11",
      text: "Chợ trong quy hoạch",
    },
    {
      id: "M121",
      text: "Chợ ngoài quy hoạch",
    },
    {
      id: "M122",
      text: "Chợ đêm",
    },
    {
      id: "M123",
      text: "Chợ nổi",
    },
    {
      id: "M2",
      text: "Siêu thị",
    },
    {
      id: "M3",
      text: "Trung tâm thương mại",
    },
    {
      id: "M4",
      text: "Cửa hàng tiện lợi",
    },
    {
      id: "M5",
      text: "Cửa hàng chuyên doanh",
    },
    {
      id: "M6",
      text: "Cửa hàng tạp hoá, thực phẩm truyền thống",
    },
    {
      id: "M7",
      text: "Trung tâm Logistics",
    },
  ];
  districtId: string = "00000000-0000-0000-0000-000000000000";
  communeId: string = "00000000-0000-0000-0000-000000000000";
  typeId: string = "00000000-0000-0000-0000-000000000000";
  data: any;
  total: any;
  options: Options;
  filterBody: any = { "filter": {} };
  userLv: number = 0;
  userArea: string = "00000000-0000-0000-0000-000000000000"

  isNext: boolean = false;
  dataTotal: any = {
    khuVuc: 0,
    loaiHinh: 0,
    soSap: 0,
    soNganhHangKinhDoanh: 0
  }

  private subscriptions: Subscription[] = []; // Read more: => https://brianflove.com/2016/12/11/anguar-2-unsubscribe-observables/

  public parameterValue: string;
  
  dataSearch: any;

  constructor(
    private fb: FormBuilder,
    public districtTradeStatisticsPageService: DistrictTradeStatisticsPageService,
    public commonService: CommonService,
    private changeDetectorRef: ChangeDetectorRef,
    private authService: AuthService,
    private http: HttpClient,
    private route: ActivatedRoute,
    private searchPipe: SearchPipe
  ) {
    this.route.queryParams.subscribe(res => {
      if (res['id']) {
        this.isNext = true;
        this.districtId = res['id'];
      }
    })
  }

  ngOnInit(): void {
    // this.filterForm();
    this.searchForm();
    this.loadDistrict();
    this.loadCommune();
    this.filterForm();
    // this.districtTradeStatisticsPageService.fetch();
    // this.grouping = this.districtTradeStatisticsPageService.grouping;
    // this.paginator = this.districtTradeStatisticsPageService.paginator;
    // this.sorting = this.districtTradeStatisticsPageService.sorting;
    // const sb = this.districtTradeStatisticsPageService.isLoading$.subscribe((res: any) => this.isLoading = res);
    // this.subscriptions.push(sb);
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
    this.districtTradeStatisticsPageService.setDefaults();
  }

  loadByNextPage() {
    if (this.isNext == true) {
      this.loadData({ "filter": { "DistrictId": this.districtId } });
      this.filterGroup.controls.district.setValue(this.districtId);
      this.communeDataByDistrict = this.communeData.filter((x: any) => x.district == this.districtId || x.district == "00000000-0000-0000-0000-000000000000")
    } else {
      this.loadByUser();
    }
  }

  loadByUser() {
    const data = this.authService.getDataUser();
    this.userLv = data.levelUser;
    this.userArea = data.areaID

    if (this.userLv == 1) {
      this.loadData({ "filter": { "DistrictId": this.userArea } })
      this.filterGroup.controls.district.setValue(this.userArea);
      this.filterGroup.controls.district.disable();
      this.communeDataByDistrict = this.communeData.filter((x: any) => x.district == this.userArea || x.district == "00000000-0000-0000-0000-000000000000")
    } else {
      this.loadData({ "filter": {} });
    }
  }

  loadData(filter: any) {
      this.districtTradeStatisticsPageService.loadData(filter).subscribe((res: any) => {
      this.data = res.details;
      this.total = res.total;
      this.onEnter();
      this.changeDetectorRef.detectChanges();
    })
  }

  loadDistrict() {
    this.commonService.getDistrict().subscribe((res: any) => {
      const data = [
        {
          id: "00000000-0000-0000-0000-000000000000",
          text: '-- Chọn --'
        }
      ]
      for (var item of res.items) {
        let obj = {
          id: item.districtId,
          text: item.districtName,
        }
        data.push(obj)
      }
      this.districtData = data;
    })
  }

  loadCommune() {
    this.commonService.getCommune().subscribe((res: any) => {
      const data = [
        {
          id: "00000000-0000-0000-0000-000000000000",
          text: '-- Chọn --',
          district: "00000000-0000-0000-0000-000000000000",
        }
      ]
      for (var item of res.items) {
        let obj = {
          id: item.communeId,
          text: item.communeName,
          district: item.districtId
        }
        data.push(obj)
      }
      this.communeData = data;
      this.communeDataByDistrict = data;
    })
  }

  // filtration
  filterForm() {
    this.filterGroup = this.fb.group({
      district: [this.districtId],
      commune: [this.communeId],
      type: [this.typeId],
    });
    this.filterGroup.controls.district.valueChanges.subscribe((x: any) => {
      if (x != "00000000-0000-0000-0000-000000000000") {
        this.communeDataByDistrict = this.communeData.filter((i: any) => i.district == x || i.district == "00000000-0000-0000-0000-000000000000")
      } else {
        this.communeDataByDistrict = this.communeData
      }
      this.filter()
    })
    this.filterGroup.controls.commune.valueChanges.subscribe(() =>
      this.filter()
    )
    this.filterGroup.controls.type.valueChanges.subscribe(() =>
      this.filter()
    )
    this.loadByNextPage();
  }

  filter() {
    const filter: { [key: string]: string } = {};
    const district = this.filterGroup.controls['district'].value;
    const commune = this.filterGroup.controls['commune'].value;
    const type = this.filterGroup.controls['type'].value;
    if (district && district != "00000000-0000-0000-0000-000000000000") {
      filter['DistrictId'] = district;
    };
    if (commune && commune != "00000000-0000-0000-0000-000000000000") {
      filter['CommuneId'] = commune;
    };
    if (type && type != "00000000-0000-0000-0000-000000000000") {
      filter['TypeOfCategoryId'] = type;
    };
    this.filterBody = { "filter": filter };
    // this.loadData(this.filterBody);
  }

  applyFilter() {
    this.loadData(this.filterBody);
  }

  clearFilter() {
    if (this.userLv == 0) {
      this.filterGroup.controls.district.setValue("00000000-0000-0000-0000-000000000000");
      this.filterGroup.controls.commune.setValue("00000000-0000-0000-0000-000000000000");
      this.filterGroup.controls.type.setValue("00000000-0000-0000-0000-000000000000");
      this.communeDataByDistrict = this.communeData;
      this.filterGroup.updateValueAndValidity();
      this.loadData({ "filter": {} })
    } else {
      this.filterGroup.controls.commune.setValue("00000000-0000-0000-0000-000000000000");
      this.filterGroup.controls.type.setValue("00000000-0000-0000-0000-000000000000");
      this.filterGroup.updateValueAndValidity();
      this.loadData({ "filter": { "DistrictId": this.userArea } })
    }

  }

  // search
  searchForm() {
    this.searchGroup = this.fb.group({
      searchTerm: [''],
    });
    this.dataSearch = this.searchGroup.controls.searchTerm.value;
    const searchEvent = this.searchGroup.controls.searchTerm.valueChanges
      .subscribe((val) => val == '' ? this.onEnter(): '');
    this.subscriptions.push(searchEvent);
  }

  search(searchTerm: string) {
    if (searchTerm.length == 0)
      this.districtTradeStatisticsPageService.patchState({ searchTerm });
  }

  onEnter() {
    this.dataTotal = {
        khuVuc: 0,
        loaiHinh: 0,
        soSap: 0,
        soNganhHangKinhDoanh: 0
    }
    this.dataSearch = this.searchGroup.controls.searchTerm.value;
    const _data = this.searchPipe.transform(this.data, this.dataSearch, ['tenHuyen','maCho','maHuyen','maXa'])
    _data.forEach( (item: any) => {
      this.dataTotal.soSap += item.soSap
      this.dataTotal.soNganhHangKinhDoanh += item.soNganhHangKinhDoanh
    })
    const dataGroupKhuVuc = _data.reduce((group: { [x: string]: any[]; }, item: { maXa: any;}) => {
      const { maXa } = item;
      group[maXa] = group[maXa] ?? [];
      group[maXa].push(item);
      return group;
    }, {})
    const dataGroupLoaiHinh = _data.reduce((group: { [x: string]: any[]; }, item: { loaiHinh: any;}) => {
      const { loaiHinh } = item;
      group[loaiHinh] = group[loaiHinh] ?? [];
      group[loaiHinh].push(item);
      return group;
    }, {})
    this.dataTotal.khuVuc =  Object.keys(dataGroupKhuVuc).length
    this.dataTotal.loaiHinh = Object.keys(dataGroupLoaiHinh).length
  }

  searchData(searchTerm: string) {
    this.districtTradeStatisticsPageService.patchState({ searchTerm });
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
    this.districtTradeStatisticsPageService.patchState({ sorting });
  }

  // pagination
  paginate(paginator: PaginatorState) {
    this.districtTradeStatisticsPageService.patchState({ paginator });
  }

  getHeight(): any {
    let tmp_height = 0;
    tmp_height = window.innerHeight - 300;
    return tmp_height + 'px';
  }

  exportFile() {
    const moment = require("moment");
    const timeString = moment().format("DDMMYYYYHHmmss");
    const fileName = "Thongkethongtinchosieuthitrungtamthuongmaihuyen_" + timeString + ".xlsx"

    Swal.fire({
            icon: 'info',
      title: 'Đang xuất File...',
      // text: 'Vui lòng đợi một lúc trước khi file của bạn sẵn sàng!',
      didOpen: () => {
        Swal.showLoading()
      },
    })

    this.http.post(`${environment.apiUrl}/Statistical/ExportByDistrict`, this.filterBody,
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


