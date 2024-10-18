import { ChangeDetectorRef, Component, Input, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal, NgbDateAdapter, NgbDateParserFormatter } from '@ng-bootstrap/ng-bootstrap';
import { of, Subscription } from 'rxjs';
import { catchError, finalize, first, tap } from 'rxjs/operators';
import Swal from 'sweetalert2';

import { TradePromotionProjectManagementDetailModel } from '../../../_models/tradepromotionPMdetail.model';

const EMPTY_CUSTOM: TradePromotionProjectManagementDetailModel = {
  id: '',
  tradePromotionProjectManagementDetailId: '',
  tradePromotionProjectManagementId: '',
  businessId: '',
  businessCode: '',
  businessNameVi: '',
  nganhNghe: '',
  diaChi: '',
  nguoiDaiDien: '',
};

@Component({
  selector: 'app-edit-modal',
  templateUrl: './edit-modal.component.html',
  styleUrls: ['./edit-modal.component.scss'],

})
export class AddEnterpriseOutsideProvinceModalComponent implements OnInit, OnDestroy {
  @Input() id: any;
  isLoading$:any;
  tradePromotionProjectManagementDetailData: TradePromotionProjectManagementDetailModel;
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
      BusinessNameVi: [this.tradePromotionProjectManagementDetailData.businessNameVi, Validators.required],
      BusinessCode: [this.tradePromotionProjectManagementDetailData.businessCode],
      NganhNghe: [this.tradePromotionProjectManagementDetailData.nganhNghe, Validators.required],
      DiaChi: [this.tradePromotionProjectManagementDetailData.diaChi],
      NguoiDaiDien: [this.tradePromotionProjectManagementDetailData.nguoiDaiDien],
    });
  }

  clear(){
    EMPTY_CUSTOM.tradePromotionProjectManagementDetailId = '00000000-0000-0000-0000-000000000000',
    EMPTY_CUSTOM.tradePromotionProjectManagementId = this.id,
    EMPTY_CUSTOM.businessId = '00000000-0000-0000-0000-000000000000',
    EMPTY_CUSTOM.businessCode = '',
    EMPTY_CUSTOM.businessNameVi = '',
    EMPTY_CUSTOM.nganhNghe = '',
    EMPTY_CUSTOM.diaChi = '',
    EMPTY_CUSTOM.nguoiDaiDien = '',
    this.tradePromotionProjectManagementDetailData = EMPTY_CUSTOM
  }

  save() {
    this.modal.dismiss(this.prepareData());
  }

  private prepareData() {
    const formData = this.formGroup.value;
    this.tradePromotionProjectManagementDetailData.businessId = formData.BusinessId;
    this.tradePromotionProjectManagementDetailData.businessCode = formData.BusinessCode;
    this.tradePromotionProjectManagementDetailData.businessNameVi = formData.BusinessNameVi;
    this.tradePromotionProjectManagementDetailData.diaChi = formData.DiaChi;
    this.tradePromotionProjectManagementDetailData.nguoiDaiDien = formData.NguoiDaiDien;
    this.tradePromotionProjectManagementDetailData.nganhNghe = formData.NganhNghe;

    let result = {
      BusinessId: '00000000-0000-0000-0000-000000000000',
      BusinessCode: this.tradePromotionProjectManagementDetailData.businessCode,
      BusinessNameVi: this.tradePromotionProjectManagementDetailData.businessNameVi,
      DiaChi: this.tradePromotionProjectManagementDetailData.diaChi,
      NguoiDaiDien: this.tradePromotionProjectManagementDetailData.nguoiDaiDien,
      NganhNghe: this.tradePromotionProjectManagementDetailData.nganhNghe
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
  
  check_formGroup() {
    if (this.formGroup.invalid) {
      this.formGroup.markAllAsTouched();
    }
    else {
      this.save();
    }
  }
}
