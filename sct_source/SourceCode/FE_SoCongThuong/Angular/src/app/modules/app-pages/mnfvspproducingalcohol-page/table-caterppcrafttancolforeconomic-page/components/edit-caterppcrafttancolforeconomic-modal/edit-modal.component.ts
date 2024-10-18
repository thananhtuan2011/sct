import { ChangeDetectorRef, Component, Input, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal, NgbDateAdapter, NgbDateParserFormatter } from '@ng-bootstrap/ng-bootstrap';
import { of, Subscription } from 'rxjs';
import { catchError, finalize, first, tap } from 'rxjs/operators';
import Swal from 'sweetalert2';
import { Options } from 'select2';
import * as moment from 'moment';

import { CateRPPCrafttAncolForEconomicModel } from '../../../_models/caterppcrafttancolforeconomic.model';
import { CateRPPCrafttAncolForEconomicPageService } from '../../../_services/caterppcrafttancolforeconomic-page.service';
import { CommonService } from 'src/app/_metronic/shared/services/common.service';

const EMPTY_CUSTOM: CateRPPCrafttAncolForEconomicModel = {
  id: '',
  cateReportProduceCrafttAncolForEconomicId: '00000000-0000-0000-0000-000000000000',
  typeofWine: '', //Loại rượu
  quantity: null, //Sản lượng
  quantityConsume: null, //Sản lượng tiêu thụ
  businessId: '00000000-0000-0000-0000-000000000000',
  yearReport: 0,
};

@Component({
  selector: 'app-edit-modal',
  templateUrl: './edit-modal.component.html',
  styleUrls: ['./edit-modal.component.scss'],
})
export class EditCateRPPCrafttAncolForEconomicModalComponent implements OnInit, OnDestroy {
  @Input() id: any;
  isLoading$: any;
  caterppcrafttancolforeconomicData: CateRPPCrafttAncolForEconomicModel;
  formGroup: FormGroup;
  options: Options;
  businessData: any;
  show: boolean = false;

  private subscriptions: Subscription[] = [];
  yearRange: any;

  constructor(
    private caterppcrafttancolforeconomicService: CateRPPCrafttAncolForEconomicPageService,
    private fb: FormBuilder, public modal: NgbActiveModal,
    public commonService: CommonService,
  ) { }

  ngOnInit(): void {
    this.isLoading$ = this.caterppcrafttancolforeconomicService.isLoading$;
    this.loadBusiness();
    this.getYearsList();
    this.options = {
      theme: 'bootstrap5',
      templateSelection: this.templateSelection,
    };
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

  public templateSelection = (state: any): JQuery | string => {
    if (!state.id) {
      return state.text;
    }
    return jQuery('<span class="form-select form-select-solid form-select-lg">' + state.text + '</span>');
  }

  loadCateRPSoldAncol() {
    if (!this.id) {
      this.clear();
      this.loadForm();
    } else {
      const sb = this.caterppcrafttancolforeconomicService.getItemById(this.id).pipe(
        first(),
        catchError((errorMessage) => {
          this.modal.dismiss(errorMessage);
          return of(EMPTY_CUSTOM);
        })
      ).subscribe((res: any) => {
        this.caterppcrafttancolforeconomicData = res.data;
        this.caterppcrafttancolforeconomicData.quantity = this.f_currency(res.data.quantity)
        this.caterppcrafttancolforeconomicData.quantityConsume = this.f_currency(res.data.quantityConsume)
        this.loadForm();
      });
      this.subscriptions.push(sb);
    }
  }

  loadForm() {
    this.formGroup = this.fb.group({
      BusinessId: [this.caterppcrafttancolforeconomicData.businessId, Validators.required],
      TypeofWine: [this.caterppcrafttancolforeconomicData.typeofWine, Validators.required],
      Quantity: [this.caterppcrafttancolforeconomicData.quantity, Validators.required],
      QuantityConsume: [this.caterppcrafttancolforeconomicData.quantityConsume, Validators.required],
      YearReport: [this.caterppcrafttancolforeconomicData.yearReport]
    });

    this.subscriptions.push(
      this.formGroup.controls.Quantity.valueChanges.subscribe(x => {
        this.formGroup.patchValue({
          'Quantity': this.f_currency(x)
        }, { emitEvent: false })
      })
    );

    this.subscriptions.push(
      this.formGroup.controls.QuantityConsume.valueChanges.subscribe(x => {
        this.formGroup.patchValue({
          'QuantityConsume': this.f_currency(x)
        }, { emitEvent: false })
      })
    );

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

  private prepareCateRPSoldAncol() {
    const formData = this.formGroup.value;
    this.caterppcrafttancolforeconomicData.businessId = formData.BusinessId;
    this.caterppcrafttancolforeconomicData.typeofWine = formData.TypeofWine;
    this.caterppcrafttancolforeconomicData.quantity = Number(formData.Quantity.replaceAll(',', ''));
    this.caterppcrafttancolforeconomicData.quantityConsume = Number(formData.QuantityConsume.replaceAll(',', ''));
    this.caterppcrafttancolforeconomicData.yearReport = formData.YearReport;
  }

  clear() {
    EMPTY_CUSTOM.cateReportProduceCrafttAncolForEconomicId = '00000000-0000-0000-0000-000000000000',
    EMPTY_CUSTOM.businessId = '00000000-0000-0000-0000-000000000000',
    EMPTY_CUSTOM.typeofWine = '', //Loại rượu
    EMPTY_CUSTOM.quantity = null, //Sản lượng
    EMPTY_CUSTOM.quantityConsume = null, //Sản lượng tiêu thụ
    EMPTY_CUSTOM.yearReport = 0,
    this.caterppcrafttancolforeconomicData = EMPTY_CUSTOM;
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
    const sbUpdate = this.caterppcrafttancolforeconomicService.update(this.caterppcrafttancolforeconomicData).pipe(
      tap(() => {
        this.modal.close();
      }),
      catchError((errorMessage) => {
        this.modal.dismiss(errorMessage);
        return of(this.caterppcrafttancolforeconomicData);
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
    const sbCreate = this.caterppcrafttancolforeconomicService.create(this.caterppcrafttancolforeconomicData).pipe(
      tap(() => {
        this.modal.close();
      }),
      catchError((errorMessage) => {
        this.modal.dismiss(errorMessage);
        return of(this.caterppcrafttancolforeconomicData);
      }),
    ).subscribe((res: any) => {
      Swal.fire({
        icon: res.status == 1 ? 'success' : 'error',
        title: res.status == 1 ? 'Thêm mới thành công' : 'Thêm mới thất bại',
        confirmButtonText: 'Xác nhận',
        text: 'Thêm mới ' + (res.status == 1 ? 'thành công' : 'thất bại'),
      });
      this.caterppcrafttancolforeconomicData = EMPTY_CUSTOM
    });
    this.subscriptions.push(sbCreate);
  }

  prenventInputNonNumber(event: any) {
    if (event.which < 48 || event.which > 57) {
      event.preventDefault();
    }
  }

  f_currency(value: any, args?: any): any {
    let nbr = Number((value + '').replace(/,|-/g, ''));
    return (nbr + '').replace(/(\d)(?=(\d{3})+(?!\d))/g, '$1,');
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
      this.formGroup.updateValueAndValidity();
    }
    else {
      this.save()
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
      this.loadCateRPSoldAncol();
    })
  }
}
