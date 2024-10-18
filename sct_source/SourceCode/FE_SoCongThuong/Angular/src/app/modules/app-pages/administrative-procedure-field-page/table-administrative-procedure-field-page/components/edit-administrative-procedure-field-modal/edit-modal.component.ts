import { ChangeDetectorRef, Component, Input, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal, NgbDateAdapter, NgbDateParserFormatter } from '@ng-bootstrap/ng-bootstrap';
import { of, Subscription } from 'rxjs';
import { catchError, finalize, first, tap } from 'rxjs/operators';
import Swal from 'sweetalert2';

import { AdministrativeProcedureFieldModel } from '../../../_models/administrative-procedure-field-page.model';
import { AdministrativeProcedureFieldService } from '../../../_services/administrative-procedure-field-page.service';

const EMPTY_CUSTOM: AdministrativeProcedureFieldModel = {
  id: '',
  categoryId: '',
  categoryCode: '',
  categoryName: '',
};

@Component({
  selector: 'app-edit-modal',
  templateUrl: './edit-modal.component.html',
  styleUrls: ['./edit-modal.component.scss'],

})
export class EditAdministrativeProcedureFieldModalComponent implements OnInit, OnDestroy {
  @Input() id: any;
  isLoading$:any;
  categoryData: AdministrativeProcedureFieldModel;
  formGroup: FormGroup;

  private subscriptions: Subscription[] = [];
  public default_value = "00000000-0000-0000-0000-000000000000"
  
  constructor(
    private categoryService: AdministrativeProcedureFieldService,
    private fb: FormBuilder, public modal: NgbActiveModal,
    ) { }

  ngOnInit(): void {
    this.isLoading$ = this.categoryService.isLoading$;
    this.loadCategory();
  }

  loadCategory() {
    if (!this.id) {
      EMPTY_CUSTOM.categoryCode='';
      EMPTY_CUSTOM.categoryName='';
      this.categoryData = EMPTY_CUSTOM;
      this.loadForm();
    } else {
      const sb = this.categoryService.getItemById(this.id).pipe(
        first(),
        catchError((errorMessage) => {
          this.modal.dismiss(errorMessage);
          return of(EMPTY_CUSTOM);
        })
      ).subscribe((res: any) => {
        this.categoryData = res.items[0];
        this.loadForm();
      });
      this.subscriptions.push(sb);
    }
  }

  loadForm() {
    this.formGroup = this.fb.group({
      CategoryCode: [this.categoryData.categoryCode, Validators.compose([Validators.required, Validators.pattern("[a-zA-Z0-9]{1,15}")])],
      CategoryName: [this.categoryData.categoryName,Validators.required] // Validators.compose([Validators.required, Validators.minLength(2), Validators.maxLength(100)])],
    });
  }

  save() {
    this.prepareCategory();
    if (this.categoryData.categoryId) {
      this.edit();
    } else {
      this.categoryData.categoryId = "00000000-0000-0000-0000-000000000000"
      this.create();
    }
  }

  edit() {
    const sbUpdate = this.categoryService.update(this.categoryData).pipe(
      tap(() => {
        this.modal.close();
      }),
      catchError((errorMessage) => {
        this.modal.dismiss(errorMessage);
        return of(this.categoryData);
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
    const sbCreate = this.categoryService.create(this.categoryData).pipe(
      tap(() => {
        this.modal.close();
      }),
      catchError((errorMessage) => {
        this.modal.dismiss(errorMessage);
        return of(this.categoryData);
      }),
    ).subscribe((res: any) => {
      Swal.fire({
        icon: res.status == 1 ? 'success' : 'error',
        title: res.status == 1 ? 'Thêm mới thành công' : 'Thêm mới thất bại',
        confirmButtonText: 'Xác nhận',
        text: res.status == 0 ? res.error.msg : 'Thêm mới ' + (res.status == 1 ? 'thành công' : 'thất bại'),
      });
      this.categoryData = EMPTY_CUSTOM
    });
    this.subscriptions.push(sbCreate);
    EMPTY_CUSTOM.categoryId='';
    EMPTY_CUSTOM.categoryCode='';
    EMPTY_CUSTOM.categoryName='';
    this.categoryData = EMPTY_CUSTOM;
  }

  private prepareCategory() {
    const formData = this.formGroup.value;
    this.categoryData.categoryCode = formData.CategoryCode;
    this.categoryData.categoryName = formData.CategoryName;
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
