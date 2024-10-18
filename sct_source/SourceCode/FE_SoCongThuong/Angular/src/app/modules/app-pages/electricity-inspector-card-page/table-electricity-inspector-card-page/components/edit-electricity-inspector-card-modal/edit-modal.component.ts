import { ChangeDetectorRef, Component, Input, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal, NgbDate, NgbDateAdapter, NgbDateParserFormatter, NgbDateStruct } from '@ng-bootstrap/ng-bootstrap';
import * as moment from 'moment';
import { of, Subscription } from 'rxjs';
import { catchError, finalize, first, tap } from 'rxjs/operators';
import { Options } from 'select2';
import Swal from 'sweetalert2';

import { ElectricityInspectorCardModel } from '../../../_models/electricity-inspector-card.model';
import { ElectricityInspectorCardPageService } from '../../../_services/electricity-inspector-card-page.service';
import { CommonService } from 'src/app/_metronic/shared/services/common.service';

const EMPTY_CUSTOM: ElectricityInspectorCardModel = {
  id: '',
  electricityInspectorCardId: '',
  inspectorName: '',
  birthday: '',
  licenseDate: '',
  degree: '',
  unit: '',
  seniority: '',
  /// Màu thẻ: 0 - Cam, 1 - Vàng, 2 - Hồng, 3 - "-- Chọn --"
  cardColor: "00000000-0000-0000-0000-000000000000",
};

@Component({
  selector: 'app-edit-modal',
  templateUrl: './edit-modal.component.html',
  styleUrls: ['./edit-modal.component.scss'],

})
export class EditElectricityInspectorCardModalComponent implements OnInit, OnDestroy {
  @Input() id: any;
  isLoading$: any;
  electricityInspectorCardData: ElectricityInspectorCardModel;
  formGroup: FormGroup;
  options: Options;

  cardColorData: any = [];

  MinDate: NgbDateStruct = { year: 1975, month: 1, day: 1 };
  MaxDate: NgbDateStruct = { year: new Date().getFullYear(), month: new Date().getMonth() + 1, day: new Date().getDate() };

  private subscriptions: Subscription[] = [];

  constructor(
    private electricityInspectorCardService: ElectricityInspectorCardPageService,
    private fb: FormBuilder,
    public modal: NgbActiveModal,
    private commonService: CommonService
  ) { }

  ngOnInit(): void {
    this.isLoading$ = this.electricityInspectorCardService.isLoading$;
    this.loadCardColor()
    this.options = {
      theme: 'bootstrap5',
      templateSelection: this.templateSelection,
    };
  }

  public loadCardColor() {
    const sub = this.commonService.GetConfig("CARD_COLOR").subscribe((res: any) => {
      const data = [
        { id: "00000000-0000-0000-0000-000000000000", text: "-- Chọn --" },
        ...res.items.listConfig.map((item: any) => ({
          id: item.categoryId,
          text: item.categoryName,
          typeCode: item.categoryTypeCode,
          code: item.categoryCode,
          priority: item.priority,
        }))
      ]
      this.cardColorData = data;
      this.loadElectricityInspectorCard();
    })
    this.subscriptions.push(sub);
  }

  public templateSelection = (state: any): JQuery | string => {
    if (!state.id) {
      return state.text;
    }
    return jQuery('<span class="form-select form-select-solid form-select-lg">' + state.text + '</span>');
  }

  loadElectricityInspectorCard() {
    if (!this.id) {
      this.clear();
      this.loadForm();
    } else {
      const sb = this.electricityInspectorCardService.getItemById(this.id).pipe(
        first(),
        catchError((errorMessage) => {
          this.modal.dismiss(errorMessage);
          return of(EMPTY_CUSTOM);
        })
      ).subscribe((res: any) => {
        this.electricityInspectorCardData = res.items[0];
        this.loadForm();
      });
      this.subscriptions.push(sb);
    }
  }

  loadForm() {
    this.formGroup = this.fb.group({
      InspectorName: [this.electricityInspectorCardData.inspectorName, Validators.required],
      Birthday: [this.electricityInspectorCardData.birthday, Validators.required],
      LicenseDate: [this.electricityInspectorCardData.licenseDate, Validators.required],
      Degree: [this.electricityInspectorCardData.degree, Validators.required],
      Unit: [this.electricityInspectorCardData.unit, Validators.required],
      Seniority: [this.electricityInspectorCardData.seniority, Validators.required],
      CardColor: [this.electricityInspectorCardData.cardColor],
    });
  }

  private prepareElectricityInspectorCard() {
    const formData = this.formGroup.value;
    this.electricityInspectorCardData.inspectorName = formData.InspectorName;
    this.electricityInspectorCardData.birthday = formData.Birthday;
    this.electricityInspectorCardData.licenseDate = formData.LicenseDate;
    this.electricityInspectorCardData.degree = formData.Degree;
    this.electricityInspectorCardData.unit = formData.Unit;
    this.electricityInspectorCardData.seniority = formData.Seniority;
    this.electricityInspectorCardData.cardColor = formData.CardColor;
  }

  clear() {
    EMPTY_CUSTOM.electricityInspectorCardId = "00000000-0000-0000-0000-000000000000",
      EMPTY_CUSTOM.inspectorName = '',
      EMPTY_CUSTOM.birthday = '',
      EMPTY_CUSTOM.licenseDate = '',
      EMPTY_CUSTOM.degree = '',
      EMPTY_CUSTOM.unit = '',
      EMPTY_CUSTOM.seniority = '',
      EMPTY_CUSTOM.cardColor = "00000000-0000-0000-0000-000000000000",
      this.electricityInspectorCardData = EMPTY_CUSTOM;
  }

  save() {
    this.prepareElectricityInspectorCard();
    if (this.id) {
      this.edit();
    } else {
      this.create();
    }
  }

  edit() {
    const sbUpdate = this.electricityInspectorCardService.update(this.electricityInspectorCardData).pipe(
      tap(() => {
        this.modal.close();
      }),
      catchError((errorMessage) => {
        this.modal.dismiss(errorMessage);
        return of(this.electricityInspectorCardData);
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
    const sbCreate = this.electricityInspectorCardService.create(this.electricityInspectorCardData).pipe(
      tap(() => {
        this.modal.close();
      }),
      catchError((errorMessage) => {
        this.modal.dismiss(errorMessage);
        return of(this.electricityInspectorCardData);
      }),
    ).subscribe((res: any) => {
      Swal.fire({
        icon: res.status == 1 ? 'success' : 'error',
        title: res.status == 1 ? 'Thêm mới thành công' : 'Thêm mới thất bại',
        confirmButtonText: 'Xác nhận',
        text: res.status == 0 ? res.error.msg : 'Thêm mới ' + (res.status == 1 ? 'thành công' : 'thất bại'),
      });
      this.electricityInspectorCardData = EMPTY_CUSTOM
    });
    this.subscriptions.push(sbCreate);
  }

  prenventInputNonNumber(event: any) {
    if (event.which < 48 || event.which > 57) {
      event.preventDefault();
    }
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
    if (value == '00000000-0000-0000-0000-000000000000') {
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
}
