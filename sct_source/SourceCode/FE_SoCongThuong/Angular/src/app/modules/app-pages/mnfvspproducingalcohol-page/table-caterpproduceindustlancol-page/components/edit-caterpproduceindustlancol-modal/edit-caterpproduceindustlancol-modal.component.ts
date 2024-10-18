import { ChangeDetectorRef, Component, Input, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal, NgbDateAdapter, NgbDateParserFormatter } from '@ng-bootstrap/ng-bootstrap';
import { of, Subscription } from 'rxjs';
import { catchError, finalize, first, tap } from 'rxjs/operators';
import Swal from 'sweetalert2';
import { Options } from 'select2';
import * as moment from 'moment';

import { CateRPProduceIndustlAncolModel } from '../../../_models/caterpproduceindustlancol.model';
import { CateRPProduceIndustlAncolPageService } from '../../../_services/caterpproduceindustlancol-page.service';
import { CommonService } from 'src/app/_metronic/shared/services/common.service';

const EMPTY_CUSTOM: CateRPProduceIndustlAncolModel = {
  id: '',
  cateReportProduceIndustlAncolId: '00000000-0000-0000-0000-000000000000',
  businessId: '00000000-0000-0000-0000-000000000000',
  typeofWine: '', //Loại rượu
  designCapacity: '', //Công suất thiết kế
  quantityProduction: null, //Sản lượng sản xuất
  quantityConsume: null, //Sản lượng tiêu thụ
  investment: null, //vốn đầu tư
  yearReport: 0,
};

@Component({
  selector: 'app-edit-caterpproduceindustlancol-modal',
  templateUrl: './edit-caterpproduceindustlancol-modal.component.html',
  styleUrls: ['./edit-caterpproduceindustlancol-modal.component.scss'],

})
export class EditCateRPProduceIndustlAncolModalComponent implements OnInit, OnDestroy {
  @Input() id: any;
  isLoading$: any;
  caterpproduceindustlancolData: CateRPProduceIndustlAncolModel;
  formGroup: FormGroup;
  options: Options;
  businessData: any;
  show: boolean = false;
  // ProductionFormData: Array<any> = [
  //   {
  //     id: 0,
  //     text: '-- Chọn --'
  //   },
  //   {
  //     id: 1,
  //     text: 'Thủ công'
  //   },
  //   {
  //     id: 2,
  //     text: 'Công nghiệp'
  //   }
  // ]

  private subscriptions: Subscription[] = [];
  yearRange: any;

  constructor(
    private caterpproduceindustlancolService: CateRPProduceIndustlAncolPageService,
    private fb: FormBuilder, public modal: NgbActiveModal,
    public commonService: CommonService,
  ) { }

  ngOnInit(): void {
    this.isLoading$ = this.caterpproduceindustlancolService.isLoading$;
    this.loadBusiness();
    this.getYearsList();
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
          phoneNumber: item.soDienThoai,
          giayPhepSanXuat: item.giayPhepSanXuat,
          ngayCapPhep: item.ngayCapPhep,
          representative: item.nguoiDaiDien,
          districtId: item.districtId,
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

  getYearsList() {
    const currentYear = new Date().getFullYear();
    const yearsList: any = [{ id: 0, text: "-- Chọn --" }];
    for (let i = -10; i <= 10; i++) {
      const year = currentYear + i;
      yearsList.push({ id: year, text: year });
    }
    this.yearRange = yearsList;
  }

  delay(ms: number) {
    return new Promise(resolve => setTimeout(resolve, ms));
  }

  loadCateRPSoldAncol() {
    if (!this.id) {
      this.clear();
      this.loadForm();
    } else {
      const sb = this.caterpproduceindustlancolService.getItemById(this.id).pipe(
        first(),
        catchError((errorMessage) => {
          this.modal.dismiss(errorMessage);
          return of(EMPTY_CUSTOM);
        })
      ).subscribe((res: any) => {
        this.caterpproduceindustlancolData = res.data;
        this.caterpproduceindustlancolData.quantityProduction = this.f_currency(res.data.quantityProduction)
        this.caterpproduceindustlancolData.quantityConsume = this.f_currency(res.data.quantityConsume)
        this.caterpproduceindustlancolData.investment = this.f_currency(res.data.investment)
        this.loadForm();
      });
      this.subscriptions.push(sb);
    }
  }

  loadForm() {
    this.formGroup = this.fb.group({
      BusinessId: [this.caterpproduceindustlancolData.businessId, Validators.required],
      TypeofWine: [this.caterpproduceindustlancolData.typeofWine, Validators.required],
      DesignCapacity: [this.caterpproduceindustlancolData.designCapacity, Validators.required],
      QuantityProduction: [this.caterpproduceindustlancolData.quantityProduction, Validators.required],
      QuantityConsume: [this.caterpproduceindustlancolData.quantityConsume, Validators.required],
      Investment: [this.caterpproduceindustlancolData.investment, Validators.required],
      YearReport : [this.caterpproduceindustlancolData.yearReport],
    });

    this.subscriptions.push(
      this.formGroup.controls.QuantityProduction.valueChanges.subscribe(x => {
        this.formGroup.patchValue({
          'QuantityProduction': this.f_currency(x)
        }, { emitEvent: false })
      })
    )

    this.subscriptions.push(
      this.formGroup.controls.QuantityConsume.valueChanges.subscribe(x => {
        this.formGroup.patchValue({
          'QuantityConsume': this.f_currency(x)
        }, { emitEvent: false })
      })
    )

    this.subscriptions.push(
      this.formGroup.controls.Investment.valueChanges.subscribe(x => {
        this.formGroup.patchValue({
          'Investment': this.f_currency(x)
        }, { emitEvent: false })
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
    this.caterpproduceindustlancolData.businessId = formData.BusinessId;
    this.caterpproduceindustlancolData.typeofWine = formData.TypeofWine;
    this.caterpproduceindustlancolData.designCapacity = formData.DesignCapacity;
    this.caterpproduceindustlancolData.quantityProduction = Number(formData.QuantityProduction.replaceAll(',', ''));
    this.caterpproduceindustlancolData.quantityConsume = Number(formData.QuantityConsume.replaceAll(',', ''));
    this.caterpproduceindustlancolData.investment = Number(formData.Investment.replaceAll(',', ''));
    this.caterpproduceindustlancolData .yearReport = formData.YearReport
  }

  clear() {
    EMPTY_CUSTOM.cateReportProduceIndustlAncolId = '00000000-0000-0000-0000-000000000000',
    EMPTY_CUSTOM.businessId = '00000000-0000-0000-0000-000000000000',
    EMPTY_CUSTOM.typeofWine = '', //Loại rượu
    EMPTY_CUSTOM.designCapacity = '', //Công suất thiết kế
    EMPTY_CUSTOM.quantityProduction = null, //Sản lượng sản xuất
    EMPTY_CUSTOM.quantityConsume = null, //Sản lượng tiêu thụ
    EMPTY_CUSTOM.investment = null, //vốn đầu tư
    EMPTY_CUSTOM.yearReport = 0,
    this.caterpproduceindustlancolData = EMPTY_CUSTOM;
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
    const sbUpdate = this.caterpproduceindustlancolService.update(this.caterpproduceindustlancolData).pipe(
      tap(() => {
        this.modal.close();
      }),
      catchError((errorMessage) => {
        this.modal.dismiss(errorMessage);
        return of(this.caterpproduceindustlancolData);
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
    const sbCreate = this.caterpproduceindustlancolService.create(this.caterpproduceindustlancolData).pipe(
      tap(() => {
        this.modal.close();
      }),
      catchError((errorMessage) => {
        this.modal.dismiss(errorMessage);
        return of(this.caterpproduceindustlancolData);
      }),
    ).subscribe((res: any) => {
      Swal.fire({
        icon: res.status == 1 ? 'success' : 'error',
        title: res.status == 1 ? 'Thêm mới thành công' : 'Thêm mới thất bại',
        confirmButtonText: 'Xác nhận',
        text: 'Thêm mới ' + (res.status == 1 ? 'thành công' : 'thất bại'),
      });
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
