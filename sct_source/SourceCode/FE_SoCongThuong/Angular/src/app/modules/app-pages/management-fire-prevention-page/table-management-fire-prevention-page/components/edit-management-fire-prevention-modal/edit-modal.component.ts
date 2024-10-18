import { ChangeDetectorRef, Component, Input, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal, NgbDateAdapter, NgbDateParserFormatter } from '@ng-bootstrap/ng-bootstrap';
import { of, Subscription } from 'rxjs';
import { catchError, finalize, first, tap } from 'rxjs/operators';
import { Options } from 'select2';
import Swal from 'sweetalert2';

import { ManagementFirePreventionModel } from '../../../_models/management-fire-prevention.model';
import { ManagementFirePreventionPageService } from '../../../_services/management-fire-prevention-page.service';

const EMPTY_CUSTOM: ManagementFirePreventionModel = {
  id: '',
  managementFirePreventionId: '00000000-0000-0000-0000-000000000000',
  businessName: '',
  address: '',
  reality: 3,
};

@Component({
  selector: 'app-edit-modal',
  templateUrl: './edit-modal.component.html',
  styleUrls: ['./edit-modal.component.scss'],

})
export class EditManagementFirePreventionModalComponent implements OnInit, OnDestroy {
  @Input() id: any;
  isLoading$:any;
  managementFirePreventionData: ManagementFirePreventionModel;
  formGroup: FormGroup;
  options: Options;

  realityData: any = [
    {
      id: 3,
      text: "-- Chọn --",
    },
    {
      id: 2,
      text: "Tốt"
    },
    {
      id: 1,
      text: "Trung bình"
    },
    {
      id: 0,
      text: "Không đạt"
    },
  ]

  private subscriptions: Subscription[] = [];
  
  constructor(
    private managementFirePreventionService: ManagementFirePreventionPageService,
    private fb: FormBuilder, public modal: NgbActiveModal,
    ) { }

  ngOnInit(): void {
    this.isLoading$ = this.managementFirePreventionService.isLoading$;
    this.loadManagementFirePrevention();
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

  loadManagementFirePrevention() {
    if (!this.id) {
      this.clear();
      this.loadForm();
    } else {
      const sb = this.managementFirePreventionService.getItemById(this.id).pipe(
        first(),
        catchError((errorMessage) => {
          this.modal.dismiss(errorMessage);
          return of(EMPTY_CUSTOM);
        })
      ).subscribe((res: any) => {
        this.managementFirePreventionData = res.items[0];
        this.loadForm();
      });
      this.subscriptions.push(sb);
    }
  }

  loadForm() {
    this.formGroup = this.fb.group({
      BusinessName: [this.managementFirePreventionData.businessName, Validators.required],
      Address: [this.managementFirePreventionData.address, Validators.required],
      Reality: [this.managementFirePreventionData.reality]
    });
  }

  clear() {
    EMPTY_CUSTOM.managementFirePreventionId = "00000000-0000-0000-0000-000000000000",
    EMPTY_CUSTOM.businessName = '',
    EMPTY_CUSTOM.address = '',
    EMPTY_CUSTOM.reality = 3,
    this.managementFirePreventionData = EMPTY_CUSTOM;
  }

  save() {
    this.prepareManagementFirePrevention();
    if (this.id) {
      this.edit();
    } else {
      this.create();
    }
  }

  edit() {
    const sbUpdate = this.managementFirePreventionService.update(this.managementFirePreventionData).pipe(
      tap(() => {
        this.modal.close();
      }),
      catchError((errorMessage) => {
        this.modal.dismiss(errorMessage);
        return of(this.managementFirePreventionData);
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
    const sbCreate = this.managementFirePreventionService.create(this.managementFirePreventionData).pipe(
      tap(() => {
        this.modal.close();
      }),
      catchError((errorMessage) => {
        this.modal.dismiss(errorMessage);
        return of(this.managementFirePreventionData);
      }),
    ).subscribe((res: any) => {
      Swal.fire({
        icon: res.status == 1 ? 'success' : 'error',
        title: res.status == 1 ? 'Thêm mới thành công' : 'Thêm mới thất bại',
        confirmButtonText: 'Xác nhận',
        text: res.status == 0 ? res.error.msg : 'Thêm mới ' + (res.status == 1 ? 'thành công' : 'thất bại'),
      });
      this.managementFirePreventionData = EMPTY_CUSTOM
    });
    this.subscriptions.push(sbCreate);
  }

  private prepareManagementFirePrevention() {
    const formData = this.formGroup.value;
    this.managementFirePreventionData.businessName = formData.BusinessName;
    this.managementFirePreventionData.address = formData.Address;
    this.managementFirePreventionData.reality = formData.Reality;
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
    if (value == '00000000-0000-0000-0000-000000000000' || value == 3) {
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
