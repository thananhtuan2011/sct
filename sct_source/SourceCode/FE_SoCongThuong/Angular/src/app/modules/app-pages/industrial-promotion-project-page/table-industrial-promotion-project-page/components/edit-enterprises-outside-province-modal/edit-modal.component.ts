import { ChangeDetectorRef, Component, Input, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal, NgbDateAdapter, NgbDateParserFormatter } from '@ng-bootstrap/ng-bootstrap';
import { of, Subscription } from 'rxjs';
import { catchError, finalize, first, tap } from 'rxjs/operators';
import Swal from 'sweetalert2';

import { IndustrialPromotionProjectDetailModel } from '../../../_models/industrial-promotion-project-detail.model';

const EMPTY_CUSTOM: IndustrialPromotionProjectDetailModel = {
  id: '',
  industrialPromotionProjectDetailId: '',
  industrialPromotionProjectId: '',
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
  industrialPromotionProjectDetailData: IndustrialPromotionProjectDetailModel;
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
      BusinessNameVi: [this.industrialPromotionProjectDetailData.businessNameVi, Validators.required],
      BusinessCode: [this.industrialPromotionProjectDetailData.businessCode, Validators.required],
      NganhNghe: [this.industrialPromotionProjectDetailData.nganhNghe, Validators.required],
      DiaChi: [this.industrialPromotionProjectDetailData.diaChi],
      NguoiDaiDien: [this.industrialPromotionProjectDetailData.nguoiDaiDien],
    });
  }

  clear(){
    EMPTY_CUSTOM.industrialPromotionProjectDetailId = '00000000-0000-0000-0000-000000000000',
    EMPTY_CUSTOM.industrialPromotionProjectId = this.id,
    EMPTY_CUSTOM.businessId = '00000000-0000-0000-0000-000000000000',
    EMPTY_CUSTOM.businessCode = '',
    EMPTY_CUSTOM.businessNameVi = '',
    EMPTY_CUSTOM.nganhNghe = '',
    EMPTY_CUSTOM.diaChi = '',
    EMPTY_CUSTOM.nguoiDaiDien = '',
    this.industrialPromotionProjectDetailData = EMPTY_CUSTOM
  }

  save() {
    this.modal.dismiss(this.prepareData());
  }

  private prepareData() {
    const formData = this.formGroup.value;
    this.industrialPromotionProjectDetailData.businessId = formData.BusinessId;
    this.industrialPromotionProjectDetailData.businessCode = formData.BusinessCode;
    this.industrialPromotionProjectDetailData.businessNameVi = formData.BusinessNameVi;
    this.industrialPromotionProjectDetailData.diaChi = formData.DiaChi;
    this.industrialPromotionProjectDetailData.nguoiDaiDien = formData.NguoiDaiDien;
    this.industrialPromotionProjectDetailData.nganhNghe = formData.NganhNghe;

    let result = {
      BusinessId: '00000000-0000-0000-0000-000000000000',
      BusinessCode: this.industrialPromotionProjectDetailData.businessCode,
      BusinessNameVi: this.industrialPromotionProjectDetailData.businessNameVi,
      DiaChi: this.industrialPromotionProjectDetailData.diaChi,
      NguoiDaiDien: this.industrialPromotionProjectDetailData.nguoiDaiDien,
      NganhNghe: this.industrialPromotionProjectDetailData.nganhNghe
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
