import { ChangeDetectorRef, Component, Input, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal, NgbDateAdapter, NgbDateParserFormatter } from '@ng-bootstrap/ng-bootstrap';
import { of, Subscription } from 'rxjs';
import { catchError, finalize, first, tap } from 'rxjs/operators';
import Swal from 'sweetalert2';

import { ParticipateSupportFairDetailModel } from '../../../_models/participate-support-fair-detail.model';

const EMPTY_CUSTOM: ParticipateSupportFairDetailModel = {
  id: '',
  participateSupportFairDetailId: '',
  participateSupportFairId: '',
  businessId: '',
  businessCode: '',
  businessNameVi: '',
  nganhNghe: '',
  diaChi: '',
  nguoiDaiDien: '',
  huyen: '',
  xa: ''
};

@Component({
  selector: 'app-edit-modal',
  templateUrl: './edit-modal.component.html',
  styleUrls: ['./edit-modal.component.scss'],

})
export class AddEnterpriseOutsideProvinceModalComponent implements OnInit, OnDestroy {
  @Input() id: any;
  isLoading$:any;
  participateSupportFairDetailData: ParticipateSupportFairDetailModel;
  formGroup: FormGroup;
  

  private subscriptions: Subscription[] = [];
  
  constructor(
    private fb: FormBuilder, public modal: NgbActiveModal,
    ) { }

  ngOnInit(): void {
    this.clear();
    this.loadForm();
  }

  loadForm() {
    this.formGroup = this.fb.group({
      BusinessNameVi: [this.participateSupportFairDetailData.businessNameVi, Validators.required],
      BusinessCode: [this.participateSupportFairDetailData.businessCode, Validators.required],
      NganhNghe: [this.participateSupportFairDetailData.nganhNghe, Validators.required],
      DiaChi: [this.participateSupportFairDetailData.diaChi],
      NguoiDaiDien: [this.participateSupportFairDetailData.nguoiDaiDien],
      Huyen: [this.participateSupportFairDetailData.huyen],
      Xa: [this.participateSupportFairDetailData.xa]
    });
  }

  clear(){
    EMPTY_CUSTOM.participateSupportFairDetailId = '00000000-0000-0000-0000-000000000000',
    EMPTY_CUSTOM.participateSupportFairId = this.id,
    EMPTY_CUSTOM.businessId = '00000000-0000-0000-0000-000000000000',
    EMPTY_CUSTOM.businessCode = '',
    EMPTY_CUSTOM.businessNameVi = '',
    EMPTY_CUSTOM.nganhNghe = '',
    EMPTY_CUSTOM.diaChi = '',
    EMPTY_CUSTOM.nguoiDaiDien = '',
    EMPTY_CUSTOM.huyen = '',
    EMPTY_CUSTOM.xa = '',
    this.participateSupportFairDetailData = EMPTY_CUSTOM
  }

  save() {
    this.modal.dismiss(this.prepareData());
  }

  private prepareData() {
    const formData = this.formGroup.value;
    this.participateSupportFairDetailData.businessId = formData.BusinessId;
    this.participateSupportFairDetailData.businessCode = formData.BusinessCode;
    this.participateSupportFairDetailData.businessNameVi = formData.BusinessNameVi;
    this.participateSupportFairDetailData.diaChi = formData.DiaChi;
    this.participateSupportFairDetailData.nguoiDaiDien = formData.NguoiDaiDien;
    this.participateSupportFairDetailData.nganhNghe = formData.NganhNghe;
    this.participateSupportFairDetailData.huyen = formData.Huyen;
    this.participateSupportFairDetailData.xa = formData.Xa;

    let result = {
      BusinessId: '00000000-0000-0000-0000-000000000000',
      BusinessCode: this.participateSupportFairDetailData.businessCode,
      BusinessNameVi: this.participateSupportFairDetailData.businessNameVi,
      DiaChi: this.participateSupportFairDetailData.diaChi,
      NguoiDaiDien: this.participateSupportFairDetailData.nguoiDaiDien,
      NganhNghe: this.participateSupportFairDetailData.nganhNghe,
      Huyen: this.participateSupportFairDetailData.huyen,
      Xa: this.participateSupportFairDetailData.xa
    }
    return result
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
}
