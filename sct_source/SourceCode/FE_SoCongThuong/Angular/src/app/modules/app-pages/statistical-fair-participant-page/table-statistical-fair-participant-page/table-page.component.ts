import { CommonService } from 'src/app/_metronic/shared/services/common.service';
import { PaginatorState } from '../../../../_metronic/shared/crud-table/models/paginator.model';
import { ISearchView } from '../../../../_metronic/shared/crud-table/models/search.model';
import { IFilterView } from '../../../../_metronic/shared/crud-table/models/filter.model';
import { ISortView, SortState } from '../../../../_metronic/shared/crud-table/models/sort.model';
import { GroupingState, IGroupingView } from '../../../../_metronic/shared/crud-table/models/grouping.model';
import { ChangeDetectorRef, Component, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { catchError, of, Subscription } from 'rxjs';
import { StatisticalFairParticipantPageService } from '../_services/statistical-fair-participant-page.service';
import { Options } from 'select2';

import { environment } from 'src/environments/environment';
import { HttpClient } from '@angular/common/http';
import Swal from 'sweetalert2';
import { ActivatedRoute } from '@angular/router';
import { AuthService } from 'src/app/modules/auth';
import { SearchPipe } from 'src/app/_metronic/shared/pipe/filter-pipe/filter.pipe';


@Component({
  selector: 'app-table-page',
  templateUrl: './table-page.component.html',
  styleUrls: ['./table-page.component.scss'],
})

export class TableStatisticalFairParticipantPageComponent implements OnInit,
  OnDestroy,
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
  timeStart: any = null;
  timeEnd: any = null;
  districtId: string = "00000000-0000-0000-0000-000000000000";
  data: any;
  total: any;
  options: Options;
  filterBody: any = { "filter": {} };
  userLv: number = 0;
  userArea: string = "00000000-0000-0000-0000-000000000000"
  tongQuyMo: number = 0;
  tongDoanhNghiep: number = 0;

  isNext: boolean = false;
  
  dataSearch: any
  
  dataTotal: any = {
    SLChuongTrinh: 0,
    SLDoanhNghiep: 0,
    quyMo: 0,
  }

  private subscriptions: Subscription[] = []; // Read more: => https://brianflove.com/2016/12/11/anguar-2-unsubscribe-observables/

  public parameterValue: string;

  constructor(
    private fb: FormBuilder,
    public statisticalFairParticipantPageService: StatisticalFairParticipantPageService,
    public commonService: CommonService,
    private changeDetectorRef: ChangeDetectorRef,
    private http: HttpClient,
    private route: ActivatedRoute,
    private _AuthService: AuthService,
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
    this.searchForm();
    this.loadDistrict();
    this.filterForm();
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
    this.statisticalFairParticipantPageService.setDefaults();
  }

  loadByNextPage() {
    if (this.isNext == true) {
      this.loadData({ "filter": { "DistrictId": this.districtId } });
      this.filterGroup.controls.district.setValue(this.districtId);
    } else {
      this.loadByUser();
    }
  }

  loadByUser() {
    this.loadData({ "filter": {} });
  }

  loadData(filter: any) {
    let TongQuyMo: number = 0;
    let TongDoanhNghiep: number = 0;
    this.statisticalFairParticipantPageService.loadData(filter).subscribe((res: any) => {
      this.data = res.items;
      this.data.forEach((x: any) => {
        TongQuyMo += Number(x.quyMo);
        TongDoanhNghiep += x.soLuongDoanhNghiep;
      })
      this.total = res.total;
      this.tongQuyMo = TongQuyMo;
      this.tongDoanhNghiep = TongDoanhNghiep;
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


  // filtration
  filterForm() {
    this.filterGroup = this.fb.group({
      district: [this.districtId],
      TimeStart: [this.timeStart],
      TimeEnd: [this.timeEnd]
    });
    this.filterGroup.controls.district.valueChanges.subscribe((x: any) => {
      this.filter()
    })
    this.filterGroup.controls.TimeStart.valueChanges.subscribe(() =>
      this.filter()
    )
    this.filterGroup.controls.TimeEnd.valueChanges.subscribe(() =>
      this.filter()
    )
    this.loadByNextPage();
  }

  filter() {
    const filter: { [key: string]: string } = {};
    const district = this.filterGroup.controls['district'].value;
    const timeStart = this.filterGroup.controls['TimeStart'].value;
    const timeEnd = this.filterGroup.controls['TimeEnd'].value;
    if (district && district != "00000000-0000-0000-0000-000000000000") {
      filter['DistrictId'] = district;
    };
    if (timeStart != null && timeStart.length > 0) {
      filter['TimeStart'] = (timeStart);
    }

    if (timeEnd != null && timeEnd.length > 0) {
      filter['TimeEnd'] = timeEnd;
    }
    this.filterBody = { "filter": filter };
    // this.loadData(this.filterBody);
  }

  applyFilter() {
    this.loadData(this.filterBody);
  }

  clearFilter() {
    this.filterGroup.controls.district.setValue("00000000-0000-0000-0000-000000000000");
    this.filterGroup.controls.TimeStart.setValue(null);
    this.filterGroup.controls.TimeEnd.setValue(null);
    this.filterGroup.updateValueAndValidity();
    this.loadData({ "filter": {} })
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
      this.statisticalFairParticipantPageService.patchState({ searchTerm });
  }

  onEnter() {
    this.dataTotal = {
      SLChuongTrinh: 0,
      SLDoanhNghiep: 0,
      quyMo: 0,
    }
    this.dataSearch = this.searchGroup.controls.searchTerm.value
    const _data = this.searchPipe.transform(this.data, this.dataSearch, ['districtId','participateSupportFairId'])
    this.dataTotal.SLChuongTrinh = _data.length
    _data.forEach( (item: any) => {
      this.dataTotal.quyMo += Number(item.quyMo)
      this.dataTotal.SLDoanhNghiep += item.soLuongDoanhNghiep
    })
  }

  searchData(searchTerm: string) {
    this.statisticalFairParticipantPageService.patchState({ searchTerm });
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
    this.statisticalFairParticipantPageService.patchState({ sorting });
  }

  // pagination
  paginate(paginator: PaginatorState) {
    this.statisticalFairParticipantPageService.patchState({ paginator });
  }

  getHeight(): any {
    let tmp_height = 0;
    tmp_height = window.innerHeight - 300;
    return tmp_height + 'px';
  }

  exportFile() {
    const moment = require("moment");
    const timeString = moment().format("DDMMYYYYHHmmss");
    const fileName = "Thongkedanhsachthamgiahoicho" + timeString + ".xlsx"

    Swal.fire({
            icon: 'info',
      title: 'Đang xuất File...',
      // text: 'Vui lòng đợi một lúc trước khi file của bạn sẵn sàng!',
      didOpen: () => {
        Swal.showLoading()
      },
    })

    this.http.post(`${environment.apiUrl}/Statistical/ExportStatisticalFairParticipant`, this.filterBody,
      {
        headers: this._AuthService.getHTTPHeaders(),
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


