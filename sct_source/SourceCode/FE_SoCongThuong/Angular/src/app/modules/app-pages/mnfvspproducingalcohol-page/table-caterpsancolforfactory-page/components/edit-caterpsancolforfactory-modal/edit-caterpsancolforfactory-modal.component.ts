import { ChangeDetectorRef, Component, Input, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal, NgbDateAdapter, NgbDateParserFormatter } from '@ng-bootstrap/ng-bootstrap';
import { of, Subscription } from 'rxjs';
import { catchError, finalize, first, tap } from 'rxjs/operators';
import Swal from 'sweetalert2';
import { Options } from 'select2';

import { CateRPSAncolForFactoryModel } from '../../../_models/caterpsancolforfactory.model';
import { CateRPSAncolForFactoryPageService } from '../../../_services/caterpsancolforfactory-page.service';
import { CommonService } from 'src/app/_metronic/shared/services/common.service';

const EMPTY_CUSTOM: CateRPSAncolForFactoryModel = {
  id: '',
  cateReportSoldAncolForFactoryLicenseId: '00000000-0000-0000-0000-000000000000',
  businessId: '00000000-0000-0000-0000-000000000000',
  typeofWine: '', //Loại rượu
  quantity: null, //Sản lượng
  wineFactory: '', //Nhà máy mua rượu để chế biến lại
  yearReport: 0,
};

@Component({
  selector: 'app-edit-caterpsancolforfactory-modal',
  templateUrl: './edit-caterpsancolforfactory-modal.component.html',
  styleUrls: ['./edit-caterpsancolforfactory-modal.component.scss'],
})
export class EditCateRPSAncolForFactoryModalComponent implements OnInit, OnDestroy {
  @Input() id: any;
  isLoading$:any;
  caterpsancolforfactoryData: CateRPSAncolForFactoryModel;
  formGroup: FormGroup;
  options: Options;
  businessData : any;

  private subscriptions: Subscription[] = [];
  yearRange: any = [];
  show: boolean = false;
  
  constructor(
    private caterpsancolforfactoryService: CateRPSAncolForFactoryPageService,
    private fb: FormBuilder, public modal: NgbActiveModal,
    public commonService: CommonService,
    ) { }

  ngOnInit(): void {
    this.isLoading$ = this.caterpsancolforfactoryService.isLoading$;
    this.getYearsList();
    this.loadBusiness();

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

  getYearsList() {
    const currentYear = new Date().getFullYear();
    const yearsList: any = [{ id: 0, text: "-- Chọn --" }];
    for (let i = -10; i <= 10; i++) {
      const year = currentYear + i;
      yearsList.push({ id: year, text: year });
    }
    this.yearRange = yearsList;
  }

  f_currency(value: any, args?: any): any {
    let nbr = Number((value + '').replace(/,|-/g, ''));
    return (nbr + '').replace(/(\d)(?=(\d{3})+(?!\d))/g, '$1,');
  }

  prenventInputNonNumber(event: any) {
    if (event.which < 48 || event.which > 57) {
      event.preventDefault();
    }
  }
  loadCateRPSAncolForFactory() {
    if (!this.id) {
      this.clear();
      this.loadForm();
    } else {
      const sb = this.caterpsancolforfactoryService.getItemById(this.id).pipe(
        first(),
        catchError((errorMessage) => {
          this.modal.dismiss(errorMessage);
          return of(EMPTY_CUSTOM);
        })
      ).subscribe((res: any) => {
        this.caterpsancolforfactoryData = res.data;
        this.caterpsancolforfactoryData.quantity = this.f_currency(res.data.quantity)
        this.loadForm();
      });
      this.subscriptions.push(sb);
    }
  }

  loadForm() {
    this.formGroup = this.fb.group({
      BusinessId: [this.caterpsancolforfactoryData.businessId],
      TypeofWine: [this.caterpsancolforfactoryData.typeofWine, Validators.required],
      Quantity: [this.caterpsancolforfactoryData.quantity, Validators.required],
      WineFactory: [this.caterpsancolforfactoryData.wineFactory, Validators.required],
      YearReport: [this.caterpsancolforfactoryData.yearReport],
    });
    this.subscriptions.push(
      this.formGroup.controls.Quantity.valueChanges.subscribe(x => {
        this.formGroup.patchValue({
          'Quantity' : this.f_currency(x)
        }, { emitEvent: false })
      })
    );

    this.show = true;
  }

  private prepareCateRPSAncolForFactory() {
    const formData = this.formGroup.value;
    this.caterpsancolforfactoryData.businessId = formData.BusinessId;
    this.caterpsancolforfactoryData.typeofWine = formData.TypeofWine;
    this.caterpsancolforfactoryData.quantity = Number(formData.Quantity.replaceAll(',', ''));
    this.caterpsancolforfactoryData.wineFactory = formData.WineFactory;
    this.caterpsancolforfactoryData.yearReport = formData.YearReport;
  }

  clear() {
    EMPTY_CUSTOM.cateReportSoldAncolForFactoryLicenseId = '00000000-0000-0000-0000-000000000000',
    EMPTY_CUSTOM.typeofWine = '', //Loại rượu
    EMPTY_CUSTOM.quantity = null, //Sản lượng
    EMPTY_CUSTOM.wineFactory = '', //Nhà máy mua rượu để chế biến lại
    EMPTY_CUSTOM.businessId = '00000000-0000-0000-0000-000000000000',
    EMPTY_CUSTOM.yearReport = 0,
    this.caterpsancolforfactoryData = EMPTY_CUSTOM;
  }

  save() {
    this.prepareCateRPSAncolForFactory();
    if (this.id) {
      this.edit();
    } else {
      this.create();
    }
  }

  edit() {
    const sbUpdate = this.caterpsancolforfactoryService.update(this.caterpsancolforfactoryData).pipe(
      tap(() => {
        this.modal.close();
      }),
      catchError((errorMessage) => {
        this.modal.dismiss(errorMessage);
        return of(this.caterpsancolforfactoryData);
      }),
    ).subscribe((res: any) => {
      Swal.fire({
        icon: res.status == 1 ? 'success' : 'error',
        title: res.status == 1 ? 'Chỉnh sửa thành công' : 'Chỉnh sửa thất bại',
        confirmButtonText: 'Xác nhận',
        text: res.status == 0 ? res.error.msg : 'Chỉnh sửa ' + (res.status == 1 ? 'thành công' : 'thất bại'),
      });
    });
    this.subscriptions.push(sbUpdate);
  }

  create() {
    const sbCreate = this.caterpsancolforfactoryService.create(this.caterpsancolforfactoryData).pipe(
      tap(() => {
        this.modal.close();
      }),
      catchError((errorMessage) => {
        this.modal.dismiss(errorMessage);
        return of(this.caterpsancolforfactoryData);
      }),
    ).subscribe((res: any) => {
      Swal.fire({
        icon: res.status == 1 ? 'success' : 'error',
        title: res.status == 1 ? 'Thêm mới thành công' : 'Thêm mới thất bại',
        confirmButtonText: 'Xác nhận',
        text: res.status == 0 ? res.error.msg : 'Thêm mới ' + (res.status == 1 ? 'thành công' : 'thất bại'),
      });
      this.caterpsancolforfactoryData = EMPTY_CUSTOM
    });
    this.subscriptions.push(sbCreate);
  }

  ngOnDestroy(): void {
    this.subscriptions.forEach(sb => sb.unsubscribe());
  }

  isControlValid(controlName: string): boolean {
    const control = this.formGroup.controls[controlName];
    return control.valid && (control.dirty || control.touched);
  }

  isControlInvalid(controlName: string): boolean {
    const control = this.formGroup.controls[controlName];
    return control.invalid && (control.dirty || control.touched);
  }

  controlHasError(validation: any, controlName: any): boolean {
    const control = this.formGroup.controls[controlName];
    return control.hasError(validation) && (control.dirty || control.touched);
  }

  isControlTouched(controlName: any): boolean {
    const control = this.formGroup.controls[controlName];
    return control.dirty || control.touched;
  }

  isDefaultValue(controlName: any) {
    const control = this.formGroup.controls[controlName];
    const value = control.value;
    if (value == '00000000-0000-0000-0000-000000000000' || value == 0) {
      control.setErrors({ defaultvalue: true });
    }
    else {
      control.setErrors({ defaultvalue: null });
      control.updateValueAndValidity();
    }
    return control.invalid && (control.dirty || control.touched);
  }

  check_formGroup() {
    if (this.formGroup.invalid) {
      this.formGroup.markAllAsTouched();
    }
    else {
      this.save();
    }
  }
  
  loadBusiness() {
    this.commonService.getBusiness().subscribe((res: any) => {
      const businesses = [
        {
          id: '00000000-0000-0000-0000-000000000000',
          text: '-- Chọn --',
        },
      ]
      for (var item of res.items) {
        let obj_business = {
          id: item.businessId,
          text: item.businessNameVi,
          code: item.businessCode,
          address: item.diaChi,
          phoneNumber: item.soDienThoai
        }
        businesses.push(obj_business)
      }
      this.businessData = businesses
      // .sort((i1, i2) => {
      //   if (i1.text > i2.text) {
      //     return 1;
      //   }
      //   if (i1.text < i2.text) {
      //     return -1;
      //   }
      //   return 0;
      // });
      this.loadCateRPSAncolForFactory();
    })
  }
}
