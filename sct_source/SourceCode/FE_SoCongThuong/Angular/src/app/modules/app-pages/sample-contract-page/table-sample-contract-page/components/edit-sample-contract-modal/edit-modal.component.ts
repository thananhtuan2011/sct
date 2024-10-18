import { ChangeDetectorRef, Component, Input, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal, NgbDateAdapter, NgbDateParserFormatter } from '@ng-bootstrap/ng-bootstrap';
import * as moment from 'moment';
import { of, Subscription } from 'rxjs';
import { catchError, finalize, first, tap } from 'rxjs/operators';
import { Options } from 'select2';
import Swal from 'sweetalert2';

import { SampleContractModel } from '../../../_models/sample-contract.model';
import { SampleContractPageService } from '../../../_services/sample-contract-page.service';

const EMPTY_CUSTOM: SampleContractModel = {
  id: '',
  sampleContractId: '00000000-0000-0000-0000-000000000000',
  sampleContractField: '00000000-0000-0000-0000-000000000000',
  registrationTime: null,
  profileNumber: '',
  registrantName: '',
  phoneNumber: '',
  businessName: '',
  taxCode: '',
  businessPhoneNumber: null,
  address: null,
};

@Component({
  selector: 'app-edit-modal',
  templateUrl: './edit-modal.component.html',
  styleUrls: ['./edit-modal.component.scss'],

})
export class EditSampleContractModalComponent implements OnInit, OnDestroy {
  @Input() id: any;
  @Input() type: any = '';
  isLoading$: any;
  sampleContractData: SampleContractModel;
  formGroup: FormGroup;
  options: Options;

  private subscriptions: Subscription[] = [];
  FieldData: any = [];

  constructor(
    private sampleContractService: SampleContractPageService,
    private fb: FormBuilder, public modal: NgbActiveModal,
  ) { }

  ngOnInit(): void {
    this.isLoading$ = this.sampleContractService.isLoading$;
    (async () => {
      this.loadFieldData();
      await this.delay(150)
      this.loadSampleContract();
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

  prenventInputNonNumber(event: any) {
    if (event.which < 48 || event.which > 57) {
      event.preventDefault();
    }
  }

  //Date
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

  loadFieldData() {
    this.sampleContractService.loadField().subscribe((res: any) => {
      const field_data = [
        {
          id: '00000000-0000-0000-0000-000000000000',
          text: "-- Chọn --",
          piority: 0
        },
      ]

      for (var i of res.items) {
        let obj = {
          id: i.categoryId,
          text: i.categoryName,
          piority: i.piority,
        };
        field_data.push(obj);
      }
      
      this.FieldData = field_data.sort((i1, i2) => {
        if (i1.piority > i2.piority) {
          return 1;
        }
        if (i1.piority < i2.piority) {
          return -1;
        }
        return 0;
      });
    })
  }

  loadSampleContract() {
    if (!this.id) {
      this.clear();
      this.loadForm();
    } else {
      const sb = this.sampleContractService.getItemById(this.id).pipe(
        first(),
        catchError((errorMessage) => {
          this.modal.dismiss(errorMessage);
          return of(EMPTY_CUSTOM);
        })
      ).subscribe((res: any) => {
        this.sampleContractData = res.items[0];
        this.loadForm();
        this.formGroup.controls['RegistrationTime'].setValue(this.convert_date_string(this.formGroup.controls['RegistrationTime'].value));

        if (this.type == 'view') {
          this.formGroup.disable()
        }
        
        this.formGroup.updateValueAndValidity();
      });
      this.subscriptions.push(sb);
    }
  }

  loadForm() {
    this.formGroup = this.fb.group({
      SampleContractField: [this.sampleContractData.sampleContractField],
      RegistrationTime: [this.sampleContractData.registrationTime, Validators.required],
      ProfileNumber: [this.sampleContractData.profileNumber, Validators.required],
      RegistrantName: [this.sampleContractData.registrantName, Validators.required],
      PhoneNumber: [this.sampleContractData.phoneNumber, Validators.required],
      BusinessName: [this.sampleContractData.businessName, Validators.required],
      TaxCode: [this.sampleContractData.taxCode, Validators.required],
      BusinessPhoneNumber: [this.sampleContractData.businessPhoneNumber],
      Address: [this.sampleContractData.address],
    });
  }

  clear() {
    EMPTY_CUSTOM.sampleContractId = '00000000-0000-0000-0000-000000000000',
    EMPTY_CUSTOM.sampleContractField = '00000000-0000-0000-0000-000000000000',
    EMPTY_CUSTOM.registrationTime = null,
    EMPTY_CUSTOM.profileNumber = '',
    EMPTY_CUSTOM.registrantName = '',
    EMPTY_CUSTOM.phoneNumber = '',
    EMPTY_CUSTOM.businessName = '',
    EMPTY_CUSTOM.taxCode = '',
    EMPTY_CUSTOM.businessPhoneNumber = null,
    EMPTY_CUSTOM.address = null,
    this.sampleContractData = EMPTY_CUSTOM;
  }

  private prepareData() {
    const formData = this.formGroup.value;
    this.sampleContractData.sampleContractField = formData.SampleContractField;
    this.sampleContractData.registrationTime = this.convert_date(formData.RegistrationTime);
    this.sampleContractData.profileNumber = formData.ProfileNumber;
    this.sampleContractData.registrantName = formData.RegistrantName;
    this.sampleContractData.phoneNumber = formData.PhoneNumber;
    this.sampleContractData.businessName = formData.BusinessName;
    this.sampleContractData.taxCode = formData.TaxCode;
    this.sampleContractData.businessPhoneNumber = formData.BusinessPhoneNumber ? formData.BusinessPhoneNumber : null;
    this.sampleContractData.address = formData.Address ? formData.Address : null;
  }

  save() {
    this.prepareData();
    if (this.id) {
      this.edit();
    } else {
      this.create();
    }
  }

  edit() {
    const sbUpdate = this.sampleContractService.update(this.sampleContractData).pipe(
      tap(() => {
        this.modal.close();
      }),
      catchError((errorMessage) => {
        this.modal.dismiss(errorMessage);
        return of(this.sampleContractData);
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
    const sbCreate = this.sampleContractService.create(this.sampleContractData).pipe(
      tap(() => {
        this.modal.close();
      }),
      catchError((errorMessage) => {
        this.modal.dismiss(errorMessage);
        return of(this.sampleContractData);
      }),
    ).subscribe((res: any) => {
      Swal.fire({
        icon: res.status == 1 ? 'success' : 'error',
        title: res.status == 1 ? 'Thêm mới thành công' : 'Thêm mới thất bại',
        confirmButtonText: 'Xác nhận',
        text: res.status == 0 ? res.error.msg : 'Thêm mới ' + (res.status == 1 ? 'thành công' : 'thất bại'),
      });
      this.sampleContractData = EMPTY_CUSTOM
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
