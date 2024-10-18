import { Component, Input, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal, NgbDateAdapter, NgbDateParserFormatter } from '@ng-bootstrap/ng-bootstrap';
import { of, Subscription } from 'rxjs';
import { catchError, finalize, first, tap } from 'rxjs/operators';
import Swal from 'sweetalert2';
import { DistrictModel } from '../../../_models/district.model';
import { DistrictPageService } from '../../../_services/district-page.service';

const EMPTY_CUSTOM: DistrictModel = {
  id: '',
  districtCode: '',
  districtName: '',
  communeNumber: null,
};

@Component({
  selector: 'app-edit-customer-modal',
  templateUrl: './edit-district-modal.component.html',
  styleUrls: ['./edit-district-modal.component.scss'],

})
export class EditDistrictModalComponent implements OnInit, OnDestroy {
  @Input() id: any;
  isLoading$:any;
  districtData: DistrictModel;
  formGroup: FormGroup;
  private subscriptions: Subscription[] = [];
  constructor(
    private districtService: DistrictPageService,
    private fb: FormBuilder, public modal: NgbActiveModal
    ) { }

  ngOnInit(): void {
    this.isLoading$ = this.districtService.isLoading$;
    this.loadCustom();
  }

  loadCustom() {
    if (!this.id) {
      EMPTY_CUSTOM.districtName='';
      EMPTY_CUSTOM.districtCode='';
      EMPTY_CUSTOM.communeNumber=null;
      this.districtData = EMPTY_CUSTOM;
      this.loadForm();
    } else {
      const sb = this.districtService.getItemById(this.id).pipe(
        first(),
        catchError((errorMessage) => {
          this.modal.dismiss(errorMessage);
          return of(EMPTY_CUSTOM);
        })
      ).subscribe((res: any) => {
        this.districtData = res.items[0];
        this.loadForm();
      });
      this.subscriptions.push(sb);
    }
  }

  loadForm() {
    this.formGroup = this.fb.group({
      DistrictCode: [this.districtData.districtCode, Validators.compose([Validators.required, Validators.pattern("[a-zA-Z0-9]{1,15}")])],
      DistrictName: [this.districtData.districtName, Validators.required],
      // CommuneNumber: [this.districtData.communeNumber],
    });
  }

  save() {
    this.prepareCustomer();
    if (this.districtData.id != 0) {
      this.edit();
    } else {
      this.create();
    }
  }

  edit() {
    this.districtData.id = this.districtData.id + "";
    const sbUpdate = this.districtService.update(this.districtData).pipe(
      tap(() => {
        this.modal.close();
      }),
      catchError((errorMessage) => {
        this.modal.dismiss(errorMessage);
        return of(this.districtData);
      }),
    ).subscribe((res: any) => {
      Swal.fire({
        icon: res.status == 1 ? 'success' : 'error',
        title: res.status == 1 ? 'Chỉnh sửa thành công' : 'Chỉnh sửa thất bại',
        confirmButtonText: 'Xác nhận',
        text: (res.status == 1 ? 'Chỉnh sửa thành công' : res.status == 0 ? res.error.msg : "Chỉnh sửa thất bại" ),
      });
    });
    this.subscriptions.push(sbUpdate);
  }

  create() {
    const sbCreate = this.districtService.create(this.districtData).pipe(
      tap(() => {
        this.modal.close();
      }),
      catchError((errorMessage) => {
        this.modal.dismiss(errorMessage);
        return of(this.districtData);
      }),
    ).subscribe((res: any) => {
      Swal.fire({
        icon: res.status == 1 ? 'success' : 'error',
        title: res.status == 1 ? 'Thêm mới thành công' : 'Thêm mới thất bại',
        confirmButtonText: 'Xác nhận',
        text: (res.status == 1 ? 'Thêm mới thành công' : res.status == 0 ? res.error.msg : 'Thêm mới thất bại' ),
      });
    });
    this.subscriptions.push(sbCreate);
  }

  private prepareCustomer() {
    const formData = this.formGroup.value;
    this.districtData.districtCode = formData.DistrictCode;
    this.districtData.districtName = formData.DistrictName;
    this.districtData.communeNumber = null;
    //formData.CommuneNumber ? null : formData.CommuneNumber;
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

  isDefaultValue(controlName: any): boolean {
    const control = this.formGroup.controls[controlName];
    if (control.value && control.value < 0) {
      control.setErrors({'default': true})
    } else {
      control.setErrors(null)
    }
    return control.hasError('default') && (control.dirty || control.touched);
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
