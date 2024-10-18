import { ChangeDetectorRef, Component, Input, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal, NgbDateAdapter, NgbDateParserFormatter } from '@ng-bootstrap/ng-bootstrap';
import { of, Subscription } from 'rxjs';
import { catchError, finalize, first, tap } from 'rxjs/operators';
import Swal from 'sweetalert2';

import { TypeOfBusinessModel } from '../../../_models/typeofbusiness.model';
import { TypeOfBusinessPageService } from '../../../_services/typeofbusiness-page.service';

const EMPTY_CUSTOM: TypeOfBusinessModel = {
  id: '',
  typeOfBusinessId: '',
  typeOfBusinessCode: '',
  typeOfBusinessName: '',
};

@Component({
  selector: 'app-edit-typeofbusiness-modal',
  templateUrl: './edit-typeofbusiness-modal.component.html',
  styleUrls: ['./edit-typeofbusiness-modal.component.scss'],

})
export class EditTypeOfBusinessModalComponent implements OnInit, OnDestroy {
  @Input() id: any;
  isLoading$:any;
  typeofbusinessData: TypeOfBusinessModel;
  formGroup: FormGroup;

  private subscriptions: Subscription[] = [];
  public default_value = "00000000-0000-0000-0000-000000000000"
  
  constructor(
    private typeofbusinessService: TypeOfBusinessPageService,
    private fb: FormBuilder, public modal: NgbActiveModal,
    ) { }

  ngOnInit(): void {
    this.isLoading$ = this.typeofbusinessService.isLoading$;
    this.loadCountry();
  }

  loadCountry() {
    if (!this.id) {
      EMPTY_CUSTOM.typeOfBusinessCode='';
      EMPTY_CUSTOM.typeOfBusinessName='';
      this.typeofbusinessData = EMPTY_CUSTOM;
      this.loadForm();
    } else {
      const sb = this.typeofbusinessService.getItemById(this.id).pipe(
        first(),
        catchError((errorMessage) => {
          this.modal.dismiss(errorMessage);
          return of(EMPTY_CUSTOM);
        })
      ).subscribe((res: any) => {
        this.typeofbusinessData = res.items[0];
        this.loadForm();
      });
      this.subscriptions.push(sb);
    }
  }

  loadForm() {
    this.formGroup = this.fb.group({
      TypeOfBusinessCode: [this.typeofbusinessData.typeOfBusinessCode, Validators.compose([Validators.required, Validators.pattern("[a-zA-Z0-9]{1,15}")])],
      TypeOfBusinessName: [this.typeofbusinessData.typeOfBusinessName, Validators.required],
    });
  }

  save() {
    this.prepareTypeOfBusiness();
    if (this.typeofbusinessData.typeOfBusinessId) {
      this.edit();
    } else {
      this.typeofbusinessData.typeOfBusinessId = "00000000-0000-0000-0000-000000000000"
      this.create();
    }
  }

  edit() {
    const sbUpdate = this.typeofbusinessService.update(this.typeofbusinessData).pipe(
      tap(() => {
        this.modal.close();
      }),
      catchError((errorMessage) => {
        this.modal.dismiss(errorMessage);
        return of(this.typeofbusinessData);
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
    const sbCreate = this.typeofbusinessService.create(this.typeofbusinessData).pipe(
      tap(() => {
        this.modal.close();
      }),
      catchError((errorMessage) => {
        this.modal.dismiss(errorMessage);
        return of(this.typeofbusinessData);
      }),
    ).subscribe((res: any) => {
      Swal.fire({
        icon: res.status == 1 ? 'success' : 'error',
        title: res.status == 1 ? 'Thêm mới thành công' : 'Thêm mới thất bại',
        confirmButtonText: 'Xác nhận',
        text: res.status == 0 ? res.error.msg : 'Thêm mới ' + (res.status == 1 ? 'thành công' : 'thất bại'),
      });
      this.typeofbusinessData = EMPTY_CUSTOM
    });
    this.subscriptions.push(sbCreate);
    EMPTY_CUSTOM.typeOfBusinessId='';
    EMPTY_CUSTOM.typeOfBusinessCode='';
    EMPTY_CUSTOM.typeOfBusinessName='';
    this.typeofbusinessData = EMPTY_CUSTOM;
  }

  private prepareTypeOfBusiness() {
    const formData = this.formGroup.value;
    this.typeofbusinessData.typeOfBusinessCode = formData.TypeOfBusinessCode;
    this.typeofbusinessData.typeOfBusinessName = formData.TypeOfBusinessName;
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
