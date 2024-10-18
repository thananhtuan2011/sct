import { ChangeDetectorRef, Component, Input, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal, NgbDateAdapter, NgbDateParserFormatter } from '@ng-bootstrap/ng-bootstrap';
import { of, Subscription } from 'rxjs';
import { catchError, finalize, first, tap } from 'rxjs/operators';
import Swal from 'sweetalert2';
import { Options } from 'select2';
import * as moment from 'moment';

import { CateRPTurnOverIndustAncolModel } from '../../../_models/caterpturnoverindustancol.model';
import { CateRPTurnOverIndustAncolPageService } from '../../../_services/caterpturnoverindustancol-page.service';
import { CommonService } from 'src/app/_metronic/shared/services/common.service';

const EMPTY_CUSTOM: CateRPTurnOverIndustAncolModel = {
  id: '',
  cateReportTurnOverIndustAncolId: '00000000-0000-0000-0000-000000000000',
  businessId: '00000000-0000-0000-0000-000000000000',
  quantityBoughtOfYear: null, //Số lượng mua trong năm
  totalPriceBoughtOfYear: null, //Tổng giá trị mua trong năm
  quantitySoldOfYear: null, //Số lượng bán trong năm
  totalPriceSoldOfYear: null, //Tổng giá trị bán trong năm
  yearId: 0
};

@Component({
  selector: 'app-edit-caterpturnoverindustancol-modal',
  templateUrl: './edit-caterpturnoverindustancol-modal.component.html',
  styleUrls: ['./edit-caterpturnoverindustancol-modal.component.scss'],

})
export class EditCateRPTurnOverIndustAncolModalComponent implements OnInit, OnDestroy {
  @Input() id: any;
  isLoading$: any;
  caterpturnoverindustancolData: CateRPTurnOverIndustAncolModel;
  formGroup: FormGroup;
  options: Options;
  businessData: any;
  yearData: any = [];
  // TradeFormData: Array<any> = [
  //   {
  //     id: 0,
  //     text: '-- Chọn --'
  //   },
  //   {
  //     id: 1,
  //     text: 'Buôn rượu'
  //   },
  //   {
  //     id: 2,
  //     text: 'Bán lẻ'
  //   }
  // ]
  show: boolean = false;

  private subscriptions: Subscription[] = [];

  constructor(
    private caterpturnoverindustancolService: CateRPTurnOverIndustAncolPageService,
    private fb: FormBuilder, public modal: NgbActiveModal,
    public commonService: CommonService,
  ) { }

  ngOnInit(): void {
    this.isLoading$ = this.caterpturnoverindustancolService.isLoading$;
    this.loadBusiness();
    this.loadYear();
    this.options = {
      theme: 'bootstrap5',
      templateSelection: this.templateSelection,
    };
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
          phoneNumber: item.soDienThoai,
          giayPhepSanXuat: item.giayPhepSanXuat,
          ngayCapPhep: item.ngayCapPhep
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
      const sb = this.caterpturnoverindustancolService.getItemById(this.id).pipe(
        first(),
        catchError((errorMessage) => {
          this.modal.dismiss(errorMessage);
          return of(EMPTY_CUSTOM);
        })
      ).subscribe((res: any) => {
        this.caterpturnoverindustancolData = res.data;
        this.caterpturnoverindustancolData.quantityBoughtOfYear = this.f_currency(res.data.quantityBoughtOfYear);
        this.caterpturnoverindustancolData.totalPriceBoughtOfYear = this.f_currency(res.data.totalPriceBoughtOfYear);
        this.caterpturnoverindustancolData.quantitySoldOfYear = this.f_currency(res.data.quantitySoldOfYear);
        this.caterpturnoverindustancolData.totalPriceSoldOfYear = this.f_currency(res.data.totalPriceSoldOfYear);
        this.loadForm();
      });
      this.subscriptions.push(sb);
    }
  }

  loadForm() {
    this.formGroup = this.fb.group({
      BusinessId: [this.caterpturnoverindustancolData.businessId, Validators.required],
      QuantityBoughtOfYear: [this.caterpturnoverindustancolData.quantityBoughtOfYear, Validators.required],
      TotalPriceBoughtOfYear: [this.caterpturnoverindustancolData.totalPriceBoughtOfYear, Validators.required],
      QuantitySoldOfYear: [this.caterpturnoverindustancolData.quantitySoldOfYear, Validators.required],
      TotalPriceSoldOfYear: [this.caterpturnoverindustancolData.totalPriceSoldOfYear, Validators.required],
      YearId: [Number(this.caterpturnoverindustancolData.yearId), Validators.required]
    });
    this.subscriptions.push(
      this.formGroup.controls.QuantityBoughtOfYear.valueChanges.subscribe(x => {
        this.formGroup.patchValue({
          'QuantityBoughtOfYear': this.f_currency(x)
        }, { emitEvent: false })
      })
    );
    this.subscriptions.push(
      this.formGroup.controls.TotalPriceBoughtOfYear.valueChanges.subscribe(x => {
        this.formGroup.patchValue({
          'TotalPriceBoughtOfYear': this.f_currency(x)
        }, { emitEvent: false })
      })
    );
    this.subscriptions.push(
      this.formGroup.controls.QuantitySoldOfYear.valueChanges.subscribe(x => {
        this.formGroup.patchValue({
          'QuantitySoldOfYear': this.f_currency(x)
        }, { emitEvent: false })
      })
    );
    this.subscriptions.push(
      this.formGroup.controls.TotalPriceSoldOfYear.valueChanges.subscribe(x => {
        this.formGroup.patchValue({
          'TotalPriceSoldOfYear': this.f_currency(x)
        }, { emitEvent: false })
      })
    );
    // this.subscriptions.push(
    //   this.formGroup.controls.YearId.valueChanges.subscribe(x => {
    //     this.caterpturnoverindustancolData.yearId = this.yearData[x.id]
    //   })
    // );
    this.show = true;
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

  private prepareCateRPSoldAncol() {
    const formData = this.formGroup.value;
    this.caterpturnoverindustancolData.businessId = formData.BusinessId;
    this.caterpturnoverindustancolData.quantityBoughtOfYear = Number(formData.QuantityBoughtOfYear.replaceAll(',', ''));
    this.caterpturnoverindustancolData.totalPriceBoughtOfYear = Number(formData.TotalPriceBoughtOfYear.replaceAll(',', ''));
    this.caterpturnoverindustancolData.quantitySoldOfYear = Number(formData.QuantitySoldOfYear.replaceAll(',', ''));
    this.caterpturnoverindustancolData.totalPriceSoldOfYear = Number(formData.TotalPriceSoldOfYear.replaceAll(',', ''));
    this.caterpturnoverindustancolData.yearId = formData.YearId;

  }

  clear() {
    EMPTY_CUSTOM.cateReportTurnOverIndustAncolId = '00000000-0000-0000-0000-000000000000',
    EMPTY_CUSTOM.quantityBoughtOfYear = null, //Số lượng mua trong năm
    EMPTY_CUSTOM.totalPriceBoughtOfYear = null, //Tổng giá trị mua trong năm
    EMPTY_CUSTOM.quantitySoldOfYear = null, //Số lượng bán trong năm
    EMPTY_CUSTOM.totalPriceSoldOfYear = null, //Tổng giá trị bán trong năm
    EMPTY_CUSTOM.businessId = '00000000-0000-0000-0000-000000000000',
    EMPTY_CUSTOM.yearId = 0,
    this.caterpturnoverindustancolData = EMPTY_CUSTOM;
  }

  save() {
    this.prepareCateRPSoldAncol();
    if (this.caterpturnoverindustancolData.cateReportTurnOverIndustAncolId != '00000000-0000-0000-0000-000000000000') {
      this.edit();
    } else {
      this.create();
    }
  }

  edit() {
    const sbUpdate = this.caterpturnoverindustancolService.update(this.caterpturnoverindustancolData).pipe(
      tap(() => {
        this.modal.close();
      }),
      catchError((errorMessage) => {
        this.modal.dismiss(errorMessage);
        return of(this.caterpturnoverindustancolData);
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
    const sbCreate = this.caterpturnoverindustancolService.create(this.caterpturnoverindustancolData).pipe(
      tap(() => {
        this.modal.close();
      }),
      catchError((errorMessage) => {
        this.modal.dismiss(errorMessage);
        return of(this.caterpturnoverindustancolData);
      }),
    ).subscribe((res: any) => {
      Swal.fire({
        icon: res.status == 1 ? 'success' : 'error',
        title: res.status == 1 ? 'Thêm mới thành công' : 'Thêm mới thất bại',
        confirmButtonText: 'Xác nhận',
        text: 'Thêm mới ' + (res.status == 1 ? 'thành công' : 'thất bại'),
      });
      this.caterpturnoverindustancolData = EMPTY_CUSTOM
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
}
