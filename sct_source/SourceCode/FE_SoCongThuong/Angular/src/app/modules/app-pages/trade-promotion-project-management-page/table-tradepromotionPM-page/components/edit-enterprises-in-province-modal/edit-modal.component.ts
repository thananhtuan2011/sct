import { TypeOfProfessionModel } from '../../../../typeofprofession-page/_models/typeofprofession.model';
import { ChangeDetectorRef, Component, Input, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal, NgbDateAdapter, NgbDateParserFormatter } from '@ng-bootstrap/ng-bootstrap';
import { of, Subscription } from 'rxjs';
import { catchError, finalize, first, tap } from 'rxjs/operators';
import { CommonService } from 'src/app/_metronic/shared/services/common.service';
import Swal from 'sweetalert2';

import { TradePromotionProjectManagementDetailModel } from '../../../_models/tradepromotionPMdetail.model';
import { Options } from 'select2';

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
export class AddEnterpriseInProvinceModalComponent implements OnInit, OnDestroy {
  @Input() id: any;
  isLoading$:any;
  tradePromotionProjectManagementDetailData: TradePromotionProjectManagementDetailModel;
  formGroup: FormGroup;
  businessData: any;
  typeOfProfessionData: any;
  options: Options;

  private subscriptions: Subscription[] = [];
  
  constructor(
    private fb: FormBuilder, public modal: NgbActiveModal,
    public commonService: CommonService,
    ) { }

  ngOnInit(): void {
    (async () => {
      this.loadBusiness();
      this.loadTypeOfProfession();
      await this.delay(150);
      this.clear();
      this.loadForm();
    })();


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

  delay(ms: number) {
    return new Promise(resolve => setTimeout(resolve, ms));
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
          representative: item.nguoiDaiDien,
          typeOfProfession: item.loaiNganhNghe,
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
    })
  }

  loadTypeOfProfession() {
    this.commonService.getTypeOfProfession().subscribe((res_profession: any) => {
      const data_typeofprofession = [{
        id: "00000000-0000-0000-0000-000000000000",
        text: 'Không có'
      }]
      for (var typeofprofession of res_profession.items) {
        let obj_typeofprofession = {
          id: typeofprofession.typeOfProfessionId,
          text: typeofprofession.typeOfProfessionName,
        }
        data_typeofprofession.push(obj_typeofprofession)
      }
      this.typeOfProfessionData = data_typeofprofession
    })
    return this.typeOfProfessionData
  }

  loadForm() {
    this.formGroup = this.fb.group({
      BusinessId: [this.tradePromotionProjectManagementDetailData.businessId],
      BusinessCode: [this.tradePromotionProjectManagementDetailData.businessCode],
      NganhNghe: [this.tradePromotionProjectManagementDetailData.nganhNghe],
      DiaChi: [this.tradePromotionProjectManagementDetailData.diaChi],
      NguoiDaiDien: [this.tradePromotionProjectManagementDetailData.nguoiDaiDien],
    });
    this.subscriptions.push(
      this.formGroup.controls.BusinessId.valueChanges.subscribe((x) => {
        const find_data = this.businessData.find((x: any) => x.id == this.formGroup.controls.BusinessId.value)
        if (find_data.id !== "00000000-0000-0000-0000-000000000000") {
          this.formGroup.patchValue({
            "BusinessCode": find_data.code,
            "DiaChi": find_data.address,
            "NguoiDaiDien": find_data.representative,
            "NganhNghe": this.typeOfProfessionData.find((x: any) => x.id == find_data.typeOfProfession).text,
          }, { emitEvent: false })
        }
        else {
          this.formGroup.patchValue({
            "BusinessCode": '',
            "DiaChi": '',
            "NguoiDaiDien": '',
            "NganhNghe": '',
          }, { emitEvent: false })
        }
      })
    );
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
    this.tradePromotionProjectManagementDetailData.businessNameVi = this.businessData.find((x: any) => x.id == formData.BusinessId).text;
    this.tradePromotionProjectManagementDetailData.diaChi = formData.DiaChi;
    this.tradePromotionProjectManagementDetailData.nguoiDaiDien = formData.NguoiDaiDien;
    this.tradePromotionProjectManagementDetailData.nganhNghe = formData.NganhNghe;

    let result = {
      BusinessId: this.tradePromotionProjectManagementDetailData.businessId,
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
