import { ChangeDetectorRef, Component, Input, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal, NgbDateAdapter, NgbDateParserFormatter } from '@ng-bootstrap/ng-bootstrap';
import { of, Subscription } from 'rxjs';
import { catchError, finalize, first, tap } from 'rxjs/operators';
import Swal from 'sweetalert2';

import { TypeOfProfessionModel } from '../../../_models/typeofprofession.model';
import { TypeOfProfessionPageService } from '../../../_services/typeofprofession-page.service';

const EMPTY_CUSTOM: TypeOfProfessionModel = {
  id: '',
  typeOfProfessionId: '',
  typeOfProfessionCode: '',
  typeOfProfessionName: '',
};

@Component({
  selector: 'app-edit-typeofprofession-modal',
  templateUrl: './edit-typeofprofession-modal.component.html',
  styleUrls: ['./edit-typeofprofession-modal.component.scss'],

})
export class EditTypeOfProfessionModalComponent implements OnInit, OnDestroy {
  @Input() id: any;
  isLoading$:any;
  typeofprofessionData: TypeOfProfessionModel;
  formGroup: FormGroup;

  private subscriptions: Subscription[] = [];
  public default_value = "00000000-0000-0000-0000-000000000000"
  
  constructor(
    private typeofprofessionService: TypeOfProfessionPageService,
    private fb: FormBuilder, public modal: NgbActiveModal,
    ) { }

  ngOnInit(): void {
    this.isLoading$ = this.typeofprofessionService.isLoading$;
    this.loadProfession();
  }

  loadProfession() {
    if (!this.id) {
      EMPTY_CUSTOM.typeOfProfessionCode='';
      EMPTY_CUSTOM.typeOfProfessionName='';
      this.typeofprofessionData = EMPTY_CUSTOM;
      this.loadForm();
    } else {
      const sb = this.typeofprofessionService.getItemById(this.id).pipe(
        first(),
        catchError((errorMessage) => {
          this.modal.dismiss(errorMessage);
          return of(EMPTY_CUSTOM);
        })
      ).subscribe((res: any) => {
        this.typeofprofessionData = res.items[0];
        this.loadForm();
      });
      this.subscriptions.push(sb);
    }
  }

  loadForm() {
    this.formGroup = this.fb.group({
      TypeOfProfessionCode: [this.typeofprofessionData.typeOfProfessionCode, Validators.compose([Validators.required, Validators.pattern("[a-zA-Z0-9]{1,15}")])],
      TypeOfProfessionName: [this.typeofprofessionData.typeOfProfessionName, Validators.required],
    });
  }

  save() {
    this.prepareTypeOfProfession();
    if (this.typeofprofessionData.typeOfProfessionId) {
      this.edit();
    } else {
      this.typeofprofessionData.typeOfProfessionId = "00000000-0000-0000-0000-000000000000"
      this.create();
    }
  }

  edit() {
    const sbUpdate = this.typeofprofessionService.update(this.typeofprofessionData).pipe(
      tap(() => {
        this.modal.close();
      }),
      catchError((errorMessage) => {
        this.modal.dismiss(errorMessage);
        return of(this.typeofprofessionData);
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
    const sbCreate = this.typeofprofessionService.create(this.typeofprofessionData).pipe(
      tap(() => {
        this.modal.close();
      }),
      catchError((errorMessage) => {
        this.modal.dismiss(errorMessage);
        return of(this.typeofprofessionData);
      }),
    ).subscribe((res: any) => {
      Swal.fire({
        icon: res.status == 1 ? 'success' : 'error',
        title: res.status == 1 ? 'Thêm mới thành công' : 'Thêm mới thất bại',
        confirmButtonText: 'Xác nhận',
        text: res.status == 0 ? res.error.msg :  'Thêm mới ' + (res.status == 1 ? 'thành công' : 'thất bại'),
      });
      this.typeofprofessionData = EMPTY_CUSTOM
    });
    this.subscriptions.push(sbCreate);
    EMPTY_CUSTOM.typeOfProfessionId='';
    EMPTY_CUSTOM.typeOfProfessionCode='';
    EMPTY_CUSTOM.typeOfProfessionName='';
    this.typeofprofessionData = EMPTY_CUSTOM;
  }

  private prepareTypeOfProfession() {
    const formData = this.formGroup.value;
    this.typeofprofessionData.typeOfProfessionCode = formData.TypeOfProfessionCode;
    this.typeofprofessionData.typeOfProfessionName = formData.TypeOfProfessionName;
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
