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
import { ImportExportInfoPageService } from '../_services/importexportinfo-page.service';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
// import { EditMarketManagementModalComponent } from './components/edit-importexportinfo-modal/edit-importexportinfo-modal.component';
import Swal from 'sweetalert2';
import { Options } from 'select2';



@Component({
  selector: 'app-table-importexportinfo-page',
  templateUrl: './table-importexportinfo-page.component.html'
})
export class TableImportExportInfoPageComponent implements OnInit,
  OnDestroy,
  // ICreateAction,
  // IEditAction,
  // IDeleteAction,
  // IDeleteSelectedAction,
  IFetchSelectedAction,
  IUpdateStatusForSelectedAction,
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

  //Thị trường
  public countryData: Array<any>;
  public countryId: string = '00000000-0000-0000-0000-000000000000';

  //MinDate
  public minDate: string;

  //MaxDate
  public maxDate: string;

  //Phương thức
  public methodData: Array<any> = [
    {
      id: '00000000-0000-0000-0000-000000000000',
      text: '-- Chọn --',
    },
    {
      id: '1',
      text: 'Xuất Khẩu',
    },
    {
      id: '2',
      text: 'Nhập Khẩu',
    }
  ];
  public methodId: string = '00000000-0000-0000-0000-000000000000';

  //Criteria
  public criteriaData: Array<any> = [
    {
      id: '00000000-0000-0000-0000-000000000000',
      text: '-- Chọn --',
    },
    {
      id: '1',
      text: 'Doanh nghiệp'
    },
    {
      id: '2',
      text: 'Mặt hàng'
    },
    {
      id: '3',
      text: 'Số lượng'
    },
    {
      id: '4',
      text: 'Giá trị'
    }
  ]
  public criteriaId: string = '00000000-0000-0000-0000-000000000000';

  public goodsData: Array<any>;
  public goodsId: any = '00000000-0000-0000-0000-000000000000';

  public businessData: Array<any>;
  public businessId: string = '00000000-0000-0000-0000-000000000000';

  public amountData: Array<any>;
  public amountId: string = '00000000-0000-0000-0000-000000000000';

  public itemsData: Array<any>;
  public itemsId: string = '00000000-0000-0000-0000-000000000000';

  public priceData: Array<any>;
  public priceId: string = '00000000-0000-0000-0000-000000000000';

  private subscriptions: Subscription[] = []; // Read more: => https://brianflove.com/2016/12/11/anguar-2-unsubscribe-observables/

  constructor(
    private fb: FormBuilder,
    public importexportinfoPageService: ImportExportInfoPageService,
    private modalService: NgbModal
  ) { }

  ngOnInit(): void {
    this.filterForm();
    this.loadcountry();
    this.loadbusiness();
    this.loadcriteria();
    this.loaditems();

    this.importexportinfoPageService.fetch();
    this.grouping = this.importexportinfoPageService.grouping;
    this.paginator = this.importexportinfoPageService.paginator;
    this.sorting = this.importexportinfoPageService.sorting;
    const sb = this.importexportinfoPageService.isLoading$.subscribe((res: any) => this.isLoading = res);
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
    this.importexportinfoPageService.setDefaults();
  }

  //load data
  loadcriteria() {
    this.importexportinfoPageService.loadCriteria().subscribe((res: any) => {
      var amounts = [
        {
          id: '00000000-0000-0000-0000-000000000000',
          text: '-- Chọn --',
        },
      ];
      var prices = [
        {
          id: '00000000-0000-0000-0000-000000000000',
          text: '-- Chọn --',
        },
      ];

      for (var item of res.items) {
        let obj_amount = {
          id: item.amount,
          text: item.amount,
        }
        amounts.push(obj_amount)
      }

      for (var item of res.items) {
        let obj_price = {
          id: item.price,
          text: item.price,
        }
        prices.push(obj_price)
      }
      
      this.amountData = [...new Map(amounts.map(item => [item['id'], item])).values()];
      this.amountData = this.amountData.sort((i1, i2) => {
        if (i1.text > i2.text) {
          return 1;
        }
        if (i1.text < i2.text) {
          return -1;
        }
        return 0;
      });
      
      this.priceData = [...new Map(prices.map(item => [item['id'], item])).values()];
      this.priceData = this.priceData.sort((i1, i2) => {
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

  loaditems() {
    this.importexportinfoPageService.loadItems().subscribe((res: any) => {
      var items = [
        {
          id: '00000000-0000-0000-0000-000000000000',
          text: '-- Chọn --',
        },
      ];
      for (var item of res.items) {
        let obj_item = {
          id: item.itemName,
          text: item.itemName,
        }
        items.push(obj_item)
      }
      this.itemsData = items.sort((i1, i2) => {
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

  loadcountry() {
    this.importexportinfoPageService.loadCountry().subscribe((res: any) => {
      var countries = [
        {
          id: '00000000-0000-0000-0000-000000000000',
          text: '-- Chọn --',
        },
      ];
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

  loadbusiness() {
    this.importexportinfoPageService.loadBusiness().subscribe((res: any) => {
      var businesses = [
        {
          id: '00000000-0000-0000-0000-000000000000',
          text: '-- Chọn --',
        },
      ];
      for (var item of res.items) {
        let obj_business = {
          id: item.businessId,
          text: item.businessName,
        }
        businesses.push(obj_business)
      }
      this.businessData = businesses.sort((i1, i2) => {
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

  // filtration
  filterForm() {
    this.filterGroup = this.fb.group({
      countryId: [this.countryId],
      criteriaId: [this.criteriaId],
      businessId: [this.businessId],
      amountId: [this.amountId],
      priceId: [this.priceId],
      itemId: [this.itemsId],
      minDate: [this.minDate],
      maxDate: [this.maxDate],
      methodId: [this.methodId],
    });
  }

  change_value(event: any, ControlName: string){
    this.filterGroup.controls[ControlName].setValue(event);
    this.filterGroup.updateValueAndValidity();
  }

  change_criteria(event: any, ControlName: string){
    if (event != '00000000-0000-0000-0000-000000000000') {
      this.filterGroup.controls[ControlName].setValue(event);
      this.filterGroup.updateValueAndValidity();
    }
    else {
      this.filterGroup.controls["businessId"].setValue('00000000-0000-0000-0000-000000000000');
      this.filterGroup.controls["itemId"].setValue('00000000-0000-0000-0000-000000000000');
      this.filterGroup.controls["amountId"].setValue('00000000-0000-0000-0000-000000000000');
      this.filterGroup.controls["priceId"].setValue('00000000-0000-0000-0000-000000000000');
      this.filterGroup.updateValueAndValidity();
    }
  }

  filter() {
    const filter: { [key: string]: string } = {};

    const countryId = this.filterGroup.controls['countryId'].value;
    const countryName = this.countryData.find(x => x.id == countryId).text;

    const businessId = this.filterGroup.controls['businessId'].value;
    const businessName = this.businessData.find(x => x.id == businessId).text;

    const methodId = this.filterGroup.controls['methodId'].value;
    const methodName = this.methodData.find(x => x.id == methodId).text;
    
    const amount = this.filterGroup.controls['amountId'].value;
  
    const price = this.filterGroup.controls['priceId'].value;

    const item = this.filterGroup.controls['itemId'].value;

    const minDate = this.filterGroup.controls['minDate'].value;

    const maxDate = this.filterGroup.controls['maxDate'].value;

    if (countryId != '00000000-0000-0000-0000-000000000000') {
      filter['CountryName'] = countryName;
    }

    if (businessId != '00000000-0000-0000-0000-000000000000') {
      filter['BusinessName'] = businessName;
    }

    if (methodId != '00000000-0000-0000-0000-000000000000') {
      filter['Method'] = methodName;
    }

    if (amount != '00000000-0000-0000-0000-000000000000') {
      filter['Amount'] = amount;
    }

    if (price != '00000000-0000-0000-0000-000000000000') {
      filter['Price'] = price;
    }

    if (item != '00000000-0000-0000-0000-000000000000') {
      filter['GoodsName'] = item;
    }

    if (minDate != null && minDate.length > 0) {
      filter['MinTime'] = minDate;
    }

    if (maxDate != null && maxDate.length > 0) {
      filter['MaxTime'] = maxDate;
    }

    this.importexportinfoPageService.patchState({ filter });
  }

  // search
  // searchForm() {
  //   this.searchGroup = this.fb.group({
  //     searchTerm: [''],
  //   });
  //   const searchEvent = this.searchGroup.controls.searchTerm.valueChanges
  //     .subscribe((val) => this.search(val));
  //   this.subscriptions.push(searchEvent);
  // }

  // search(searchTerm: string) {
  //   this.importexportinfoPageService.patchState({ searchTerm });
  // }

  // onEnter() {
  //   this.searchBuildAndUpgrade(this.searchGroup.controls.searchTerm.value)
  // }
  // searchBuildAndUpgrade(searchTerm: string) {
  //   this.importexportinfoPageService.patchState({ searchTerm });
  // }

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
    this.importexportinfoPageService.patchState({ sorting });
  }

  // pagination
  paginate(paginator: PaginatorState) {
    this.importexportinfoPageService.patchState({ paginator });
  }
  f_currency(value: any, args?: any): any {
    let nbr = Number((value + '').replace(/,|-/g, ""));
    const result = (nbr + '').replace(/(\d)(?=(\d{3})+(?!\d))/g, "$1,");
    return result
  }

  // form actions
  // create() {
  //   this.edit(0);
  // }

  // edit(id: any) {
  //   const modalRef = this.modalService.open(EditMarketManagementModalComponent, { size: 'lg' });
  //   modalRef.componentInstance.id = id;
  //   modalRef.result.then(() =>
  //     this.marketmanagementPageService.fetch(),
  //     () => { }
  //   );
  // }

  // delete(id: any) {
  //   Swal.fire({
  //     title: 'Bạn có muốn xóa ?',
  //     text: 'Hành động này không thể hoàn tác!',
  //     icon: 'warning',
  //     showCancelButton: true,
  //     confirmButtonColor: '#3085d6',
  //     cancelButtonColor: '#d33',
  //     confirmButtonText: 'Xác nhận',
  //     cancelButtonText: 'Thoát'
  //   }).then((result) => {
  //     if (result.isConfirmed) {
  //       this.isLoading = true;
  //       const sb = this.marketmanagementPageService.deleteMarketManagement(id).pipe(
  //         tap((res) => {
  //           Swal.fire({
  //             icon: res.status == 1 ? 'success' : 'error',
  //             title: res.status == 1 ? 'Thành công' : 'Thất bại',
  //             confirmButtonText: 'Xác nhận',
  //             text: 'Xóa ' + (res.status == 1 ? 'thành công' : 'thất bại'),
  //           });
  //           this.filter()
  //         }),
  //         catchError((errorMessage) => {
  //           return of(undefined);
  //         }),
  //         finalize(() => {
  //           this.isLoading = false;
  //         })
  //       ).subscribe();
  //     }
  //   })
  // }

  // deleteSelected() {
  //   Swal.fire({
  //     title: 'Bạn có muốn xóa ?',
  //     text: 'Hành động này không thể hoàn tác!',
  //     icon: 'warning',
  //     showCancelButton: true,
  //     confirmButtonColor: '#3085d6',
  //     cancelButtonColor: '#d33',
  //     confirmButtonText: 'Xác nhận',
  //     cancelButtonText: 'Thoát'
  //   }).then((result) => {
  //     if (result.isConfirmed) {
  //       this.isLoading = true;
  //       const sb = this.marketmanagementPageService.deleteMarketManagements(this.grouping.getSelectedRows()).pipe(
  //         tap((res) => {
  //           Swal.fire({
  //             icon: res.status == 1 ? 'success' : 'error',
  //             title: res.status == 1 ? 'Thành công' : 'Thất bại',
  //             confirmButtonText: 'Xác nhận',
  //             text: 'Xóa ' + (res.status == 1 ? 'thành công' : 'thất bại'),
  //           });
  //           this.filter()
  //         }),
  //         catchError((errorMessage) => {
  //           return of(undefined);
  //         }),
  //         finalize(() => {
  //           this.isLoading = false;
  //         })
  //       ).subscribe();
  //     }
  //   })
  // }

  updateStatusForSelected() {
  }

  fetchSelected() {
  }
}


