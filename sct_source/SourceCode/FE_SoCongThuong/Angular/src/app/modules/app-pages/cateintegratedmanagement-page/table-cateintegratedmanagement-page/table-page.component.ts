import { CommonService } from './../../../../_metronic/shared/services/common.service';
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
import { CateIntegratedManagementPageService } from '../_services/cateintegratedmanagement-page.service';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { InfoPageComponent } from './components/info-cateintegratedmanagement-modal/info-page.component';
import Swal from 'sweetalert2';
import { Options } from 'select2';
import { Router } from '@angular/router';



@Component({
  selector: 'app-table-page',
  templateUrl: './table-page.component.html'
})
export class TableCateIntegratedManagementPageComponent implements OnInit,
  OnDestroy,
  // ICreateAction,
  // IEditAction,
  // IDeleteAction,
  // IDeleteSelectedAction,
  // IFetchSelectedAction,
  // IUpdateStatusForSelectedAction,
  ISortView,
  IFilterView,
  IGroupingView,
  // ISearchView,
  IFilterView {
  paginator: PaginatorState;
  sorting: SortState;
  grouping: GroupingState;
  isLoading: boolean;
  filterGroup: FormGroup;
  searchGroup: FormGroup;
  public options: Options;

  //Loại dự án
  public projectTypeData: Array<any> = [
    {
      id: 0,
      text: '-- Chọn --'
    },
    {
      id: 1,
      text: 'Lũy kế DDI ngoài Khu công nghiệp'
    },
    {
      id: 2,
      text: 'Lũy kế FDI ngoài Khu công nghiệp'
    },
    {
      id: 3,
      text: 'Thu hồi lũy kế'
    },
    {
      id: 4,
      text: 'Mua bán góp vốn'
    },
  ];
  public projectTypeId: number = 0;
  public projectTypeName: string;

  //Khu vực
  public areaData: Array<any> = [
    {
      id: 0,
      text: '-- Chọn --'
    },
    {
      id: 1,
      text: 'Trong nước'
    },
    {
      id: 2,
      text: 'Ngoài nước'
    },
  ];
  public areaId: number = 0;
  public areaName: string;

  //Tên nhà đầu tư
  public investorData: Array<any>;
  public investorsId: string;
  public investorsName: string = '';

  //Quốc tịch / Đối tác
  public countryData: Array<any>;
  public countryId: string = '00000000-0000-0000-0000-000000000000';
  public countryName: string;

  //Số chứng nhận đầu tư
  public investmentCertificateCode: string = '';

  //Thời gian cấp chứng nhận đầu tư
  public investmentCertificateDate: any;

  //Người đại diện
  public projectLegalRepresent: string = '';

  //Thời gian thu hồi luỹ kế
  public projectDecisionToWithdrawDate: any;


  private subscriptions: Subscription[] = []; // Read more: => https://brianflove.com/2016/12/11/anguar-2-unsubscribe-observables/

  constructor(
    private fb: FormBuilder,
    public cateintegratedmanagementPageService: CateIntegratedManagementPageService,
    public commonService: CommonService,
    private modalService: NgbModal,
    private router: Router,
  ) { }

  ngOnInit(): void {
    this.filterForm();
    this.loadcountry()
    this.cateintegratedmanagementPageService.fetch();
    this.grouping = this.cateintegratedmanagementPageService.grouping;
    this.paginator = this.cateintegratedmanagementPageService.paginator;
    this.sorting = this.cateintegratedmanagementPageService.sorting;
    const sb = this.cateintegratedmanagementPageService.isLoading$.subscribe((res: any) => this.isLoading = res);
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
    this.cateintegratedmanagementPageService.setDefaults();
  }

  loadcountry() {
    this.commonService.getListCountry().subscribe((res: any) => {
      const countries = [
        {
          id: '00000000-0000-0000-0000-000000000000',
          text: '-- Chọn --',
        },
      ]
      for (var item of res.items) {
        let obj_country = {
          id: item.countryId,
          text: item.countryName,
        }
        countries.push(obj_country)
      }
      this.countryData = countries.sort((i1, i2) => {
        if (i1.text > i2.text) {
          return 1;
        }
        if (i1.text < i2.text) {
          return -1;
        }
        return 0;
      });
    })
  }

  change_value(event: any, ControlName: string) {
    this.filterGroup.controls[ControlName].setValue(event);
    this.filterGroup.updateValueAndValidity();
  }

  // filtration
  filterForm() {
    this.filterGroup = this.fb.group({
      ProjectTypeId: [this.projectTypeId],
      AreaId: [this.areaId],
      InvestorsName: [this.investorsName],
      // CountryId: [this.countryId],
      CountryName: [this.countryName],
      InvestmentCertificateCode: [this.investmentCertificateCode],
      InvestmentCertificateDate: [this.investmentCertificateDate],
      ProjectLegalRepresent: [this.projectLegalRepresent],
      ProjectDecisionToWithdrawDate: [this.projectDecisionToWithdrawDate],
    });
  }

  filter() {
    const filter: { [key: string]: string } = {};
    
    //Loại dự án
    const ProjectTypeId = this.filterGroup.controls['ProjectTypeId'].value;
    this.projectTypeName = this.projectTypeData.find(x => x.id == ProjectTypeId).text;
    
    //Khu vực
    const AreaId = this.filterGroup.controls['AreaId'].value;
    // this.areaName = this.areaData.find(x => x.id == AreaId).text;

    //Tên nhà đầu tư
    const InvestorsName = this.filterGroup.controls['InvestorsName'].value;

    //Quốc tịch / Đối tác
    // const CountryId = this.filterGroup.controls['CountryId'].value;
    // const CountryName = this.countryData.find(x => x.id == CountryId).text;
    const CountryName = this.filterGroup.controls['CountryName'].value;


    //Số chứng nhận đầu tư
    const InvestmentCertificateCode = this.filterGroup.controls['InvestmentCertificateCode'].value;

    //Thời gian cấp chứng nhận đầu tư
    const InvestmentCertificateDate = this.filterGroup.controls['InvestmentCertificateDate'].value;

    //Người đại diện
    const ProjectLegalRepresent = this.filterGroup.controls['ProjectLegalRepresent'].value;

    //Thời gian thu hồi luỹ kế
    const ProjectDecisionToWithdrawDate = this.filterGroup.controls['ProjectDecisionToWithdrawDate'].value;

    //Check
    if (ProjectTypeId != 0) {
      filter['ProjectType'] = this.projectTypeName;
    }

    if (AreaId != 0) {
      filter['Area'] = AreaId;
    }

    if (InvestorsName != '') {
      filter['InvestorsName'] = InvestorsName;
    }

    if (CountryName != '') {
      filter['CountryName'] = CountryName;
    }

    if (InvestmentCertificateCode != '') {
      filter['InvestmentCertificateCode'] = InvestmentCertificateCode;
    }

    if (InvestmentCertificateDate != null && InvestmentCertificateDate != '') {
      filter['InvestmentCertificateDate'] = InvestmentCertificateDate;
    }

    if (ProjectLegalRepresent != '') {
      filter['ProjectLegalRepresent'] = ProjectLegalRepresent;
    }

    if (ProjectDecisionToWithdrawDate != null && ProjectDecisionToWithdrawDate != '') {
      filter['ProjectDecisionToWithdrawDate'] = ProjectDecisionToWithdrawDate;
    }

    this.cateintegratedmanagementPageService.patchState({ filter });
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
    this.cateintegratedmanagementPageService.patchState({ sorting });
  }

  // pagination
  paginate(paginator: PaginatorState) {
    this.cateintegratedmanagementPageService.patchState({ paginator });
  }

  view(id: any) {
    const modalRef = this.modalService.open(InfoPageComponent, { size: 'xl' });
    modalRef.componentInstance.id = id;
    modalRef.result.then(() =>
      this.cateintegratedmanagementPageService.fetch(),
      () => { }
    );
  }
}


