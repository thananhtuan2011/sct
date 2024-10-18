import { Component, Input, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { Subscription } from 'rxjs';
import { CommonService } from 'src/app/_metronic/shared/services/common.service';
import { TradePromotionActivityReportDetailModel } from '../../../_models/trade-promotion-activity-report-detail.model';
import { Options } from 'select2';

const EMPTY_CUSTOM: TradePromotionActivityReportDetailModel = {
  id: '',
  tradePromotionActivityReportDetailId: '',
  tradePromotionActivityReportId: '',
  businessId: '',
  businessName: '',
  address: '',
};

@Component({
  selector: 'app-edit-modal',
  templateUrl: './edit-modal.component.html',
  styleUrls: ['./edit-modal.component.scss'],
})

export class AddEnterpriseOutsideProvinceModalComponent implements OnInit, OnDestroy {
  @Input() id: any;
  isLoading$: any;
  editData: TradePromotionActivityReportDetailModel;
  formGroup: FormGroup;
  businessData: any;
  typeOfProfessionData: any;
  options: Options;
  show: boolean = false;

  private subscriptions: Subscription[] = [];
  

  constructor(
    private fb: FormBuilder, public modal: NgbActiveModal,
    public commonService: CommonService,
  ) { }

  ngOnInit(): void {
    this.clear();
    this.loadForm();

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

  loadForm() {
    this.formGroup = this.fb.group({
      BusinessName: [this.editData.businessName, Validators.required],
      Address: [this.editData.address, Validators.required],
    });
    this.show = true;
  }

  clear() {
    EMPTY_CUSTOM.tradePromotionActivityReportDetailId = '00000000-0000-0000-0000-000000000000',
    EMPTY_CUSTOM.tradePromotionActivityReportId = this.id,
    EMPTY_CUSTOM.businessId = '00000000-0000-0000-0000-000000000000',
    EMPTY_CUSTOM.businessName = '',
    EMPTY_CUSTOM.address = '',
    this.editData = EMPTY_CUSTOM
  }

  save() {
    this.modal.dismiss(this.prepareData());
  }

  private prepareData() {
    const formData = this.formGroup.value;
    this.editData.businessName = formData.BusinessName;
    this.editData.address = formData.Address;

    let result = {
      BusinessId: this.editData.businessId,
      BusinessName: this.editData.businessName,
      Address: this.editData.address,
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
      this.formGroup.updateValueAndValidity();
    }
    else {
      this.save()
    }
  }
}