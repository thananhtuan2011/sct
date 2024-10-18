import { ChangeDetectorRef, Component, Input, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal, NgbDateAdapter, NgbDateParserFormatter } from '@ng-bootstrap/ng-bootstrap';
import { of, Subscription } from 'rxjs';
import { catchError, finalize, first, tap } from 'rxjs/operators';
import Swal from 'sweetalert2';
import { Options } from 'select2';
import * as moment from 'moment';

import { CateRPSoldAncolModel } from '../../../_models/caterpsoldancol.model';
import { CateRPSoldAncolPageService } from '../../../_services/caterpsoldancol-page.service';
import { CommonService } from 'src/app/_metronic/shared/services/common.service';

const EMPTY_CUSTOM: CateRPSoldAncolModel = {
  id: '',
  cateReportSoldAncolId: '00000000-0000-0000-0000-000000000000',
  businessId: '00000000-0000-0000-0000-000000000000',
  quantityBoughtOfYear: null, //Số lượng mua trong năm
  totalPriceBoughtOfYear: null, //Tổng giá trị mua trong năm
  quantitySoldOfYear: null, //Số lượng bán trong năm
  totalPriceSoldOfYear: null, //Tổng giá trị bán trong năm
  yearId: 0
};

@Component({
  selector: 'app-edit-caterpsoldancol-modal',
  templateUrl: './edit-caterpsoldancol-modal.component.html',
  styleUrls: ['./edit-caterpsoldancol-modal.component.scss'],
})
export class EditCateRPSoldAncolModalComponent implements OnInit, OnDestroy {
  @Input() id: any;
  isLoading$: any;
  caterpsoldancolData: CateRPSoldAncolModel;
  formGroup: FormGroup;
  options: Options;
  businessData: any;
  yearData: any = [];
  show: boolean = false;

  private subscriptions: Subscription[] = [];

  constructor(
    private caterpsoldancolService: CateRPSoldAncolPageService,
    private fb: FormBuilder, public modal: NgbActiveModal,
    public commonService: CommonService,
  ) { }

  ngOnInit(): void {
    this.isLoading$ = this.caterpsoldancolService.isLoading$;
    this.loadBusiness();
    this.loadYear();

    this.options = {
      theme: 'bootstrap5',
      templateSelection: this.templateSelection,
    };
  }

  loadYear() {
    const data = [
      {
        id: 0,
        text: "-- Chọn --"
      }
    ];
    for (let i = 0; i < 30; i++) {
      let obj = {
        id: moment().year() - 15 + i,
        text: (moment().year() - 15 + i).toString()
      }
      data.push(obj);
    }
    this.yearData = data;
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
        }
        businesses.push(obj_business)
      }
      this.businessData = businesses
      this.loadCateRPSoldAncol();
    })
  }

  public templateSelection = (state: any): JQuery | string => {
    if (!state.id) {
      return state.text;
    }
    return jQuery('<span class="form-select form-select-solid form-select-lg">' + state.text + '</span>');
  }

  delay(ms: number) {
    return new Promise(resolve => setTimeout(resolve, ms));
  }

  loadCateRPSoldAncol() {
    if (!this.id) {
      this.clear();
      this.loadForm();
    } else {
      const sb = this.caterpsoldancolService.getItemById(this.id).pipe(
        first(),
        catchError((errorMessage) => {
          this.modal.dismiss(errorMessage);
          return of(EMPTY_CUSTOM);
        })
      ).subscribe((res: any) => {
        this.caterpsoldancolData = res.data;
        this.caterpsoldancolData.quantityBoughtOfYear = this.f_currency(res.data.quantityBoughtOfYear);
        this.caterpsoldancolData.totalPriceBoughtOfYear = this.f_currency(res.data.totalPriceBoughtOfYear);
        this.caterpsoldancolData.quantitySoldOfYear = this.f_currency(res.data.quantitySoldOfYear);
        this.caterpsoldancolData.totalPriceSoldOfYear = this.f_currency(res.data.totalPriceSoldOfYear);
        this.loadForm();
      });
      this.subscriptions.push(sb);
    }
  }

  loadForm() {
    this.formGroup = this.fb.group({
      BusinessId: [this.caterpsoldancolData.businessId, Validators.required],
      YearId: [Number(this.caterpsoldancolData.yearId), Validators.required],
      QuantityBoughtOfYear: [this.caterpsoldancolData.quantityBoughtOfYear, Validators.required],
      TotalPriceBoughtOfYear: [this.caterpsoldancolData.totalPriceBoughtOfYear, Validators.required],
      QuantitySoldOfYear: [this.caterpsoldancolData.quantitySoldOfYear, Validators.required],
      TotalPriceSoldOfYear: [this.caterpsoldancolData.totalPriceSoldOfYear, Validators.required],
    });

    this.subscriptions.push(
      this.formGroup.controls.QuantityBoughtOfYear.valueChanges.subscribe(x => {
        this.formGroup.patchValue({
          'QuantityBoughtOfYear': this.f_currency(x)
        }, { emitEvent: false })
      })
    )

    this.subscriptions.push(
      this.formGroup.controls.TotalPriceBoughtOfYear.valueChanges.subscribe(x => {
        this.formGroup.patchValue({
          'TotalPriceBoughtOfYear': this.f_currency(x)
        }, { emitEvent: false })
      })
    )

    this.subscriptions.push(
      this.formGroup.controls.QuantitySoldOfYear.valueChanges.subscribe(x => {
        this.formGroup.patchValue({
          'QuantitySoldOfYear': this.f_currency(x)
        }, { emitEvent: false })
      })
    )

    this.subscriptions.push(
      this.formGroup.controls.TotalPriceSoldOfYear.valueChanges.subscribe(x => {
        this.formGroup.patchValue({
          'TotalPriceSoldOfYear': this.f_currency(x)
        }, { emitEvent: false })
      })
    )

    this.subscriptions.push(
      this.formGroup.controls.YearId.valueChanges.subscribe(x => {
        this.caterpsoldancolData.yearId = this.yearData[x.id]
      })
    )
    
    this.show = true;
  }

  convert_date(string_date: string) {
    var result = moment.utc(string_date, "DD/MM/YYYY");
    return result
  }

  convert_date_string(string_date: string) {
    var date = string_date.split("T")[0];
    var list = date.split("-"); //["year", "month", "day"]
    var result = list[2] + "/" + list[1] + "/" + list[0]
    return result
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

  private prepareCateRPSoldAncol() {
    const formData = this.formGroup.value;
    this.caterpsoldancolData.businessId = formData.BusinessId;
    this.caterpsoldancolData.quantityBoughtOfYear = Number(formData.QuantityBoughtOfYear.replaceAll(',', ''));
    this.caterpsoldancolData.totalPriceBoughtOfYear = Number(formData.TotalPriceBoughtOfYear.replaceAll(',', ''));
    this.caterpsoldancolData.quantitySoldOfYear = Number(formData.QuantitySoldOfYear.replaceAll(',', ''));
    this.caterpsoldancolData.totalPriceSoldOfYear = Number(formData.TotalPriceSoldOfYear.replaceAll(',', ''));
    this.caterpsoldancolData.yearId = formData.YearId;
  }

  clear() {
    EMPTY_CUSTOM.cateReportSoldAncolId = '00000000-0000-0000-0000-000000000000',
    EMPTY_CUSTOM.businessId = '00000000-0000-0000-0000-000000000000',
    EMPTY_CUSTOM.quantityBoughtOfYear = null, //Số lượng mua trong năm
    EMPTY_CUSTOM.totalPriceBoughtOfYear = null, //Tổng giá trị mua trong năm
    EMPTY_CUSTOM.quantitySoldOfYear = null, //Số lượng bán trong năm
    EMPTY_CUSTOM.totalPriceSoldOfYear = null, //Tổng giá trị bán trong năm,
    EMPTY_CUSTOM.yearId = 0,
    this.caterpsoldancolData = EMPTY_CUSTOM;
  }

  save() {
    this.prepareCateRPSoldAncol();
    if (this.id) {
      this.edit();
    } else {
      this.create();
    }
  }

  edit() {
    const sbUpdate = this.caterpsoldancolService.update(this.caterpsoldancolData).pipe(
      tap(() => {
        this.modal.close();
      }),
      catchError((errorMessage) => {
        this.modal.dismiss(errorMessage);
        return of(this.caterpsoldancolData);
      }),
    ).subscribe((res: any) => {
      Swal.fire({
        icon: res.status == 1 ? 'success' : 'error',
        title: res.status == 1 ? 'Chỉnh sửa thành công' : 'Chỉnh sửa thất bại',
        confirmButtonText: 'Xác nhận',
        text: 'Chỉnh sửa ' + (res.status == 1 ? 'thành công' : 'thất bại'),
      });
    });
    this.subscriptions.push(sbUpdate);
  }

  create() {
    const sbCreate = this.caterpsoldancolService.create(this.caterpsoldancolData).pipe(
      tap(() => {
        this.modal.close();
      }),
      catchError((errorMessage) => {
        this.modal.dismiss(errorMessage);
        return of(this.caterpsoldancolData);
      }),
    ).subscribe((res: any) => {
      Swal.fire({
        icon: res.status == 1 ? 'success' : 'error',
        title: res.status == 1 ? 'Thêm mới thành công' : 'Thêm mới thất bại',
        confirmButtonText: 'Xác nhận',
        text: 'Thêm mới ' + (res.status == 1 ? 'thành công' : 'thất bại'),
      });
      this.caterpsoldancolData = EMPTY_CUSTOM
    });
    this.subscriptions.push(sbCreate);
  }

  ngOnDestroy(): void {
    this.subscriptions.forEach(sb => sb.unsubscribe());
  }

  // helpers for View
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
}
