import { Component, Input, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal, NgbDateAdapter, NgbDateParserFormatter } from '@ng-bootstrap/ng-bootstrap';
import { of, Subscription } from 'rxjs';
import { catchError, finalize, first, tap } from 'rxjs/operators';
import Swal from 'sweetalert2';
import { GasModel } from '../../../_models/gas-page.model';
import { GasPageService } from '../../../_services/gas-page.service';

const EMPTY_CUSTOM: GasModel = {
  id: '',
  gasId: '00000000-0000-0000-0000-000000000000',
  code: '',
  name: '',
};

@Component({
  selector: 'app-edit-gas-modal',
  templateUrl: './edit-modal.component.html',
  styleUrls: ['./edit-modal.component.scss'],

})
export class EditGasModalComponent implements OnInit, OnDestroy {
  @Input() id: any;
  @Input() itemData: any;
  isLoading$:any;
  data: GasModel;
  formGroup: FormGroup;
  private subscriptions: Subscription[] = [];
  constructor(
    private gasService: GasPageService,
    private fb: FormBuilder, public modal: NgbActiveModal
    ) { }

  ngOnInit(): void {
    this.isLoading$ = this.gasService.isLoading$;
    this.loadForm();
  }

  loadForm() {
    if(!this.id){
      this.clearModel();
    } else{
      this.data = this.itemData;
    }
    this.formGroup = this.fb.group({
      Code: [this.data.code, Validators.compose([Validators.required, Validators.pattern("[a-zA-Z0-9]{1,15}")])],
      Name: [this.data.name, Validators.required],
    });
  }

  save() {
    this.prepareCustomer();
    if (this.id) {
      this.edit();
    } else {
      this.create();
    }
  }

  edit() {
    const sbUpdate = this.gasService.update(this.data).pipe(
      tap(() => {
        this.modal.close();
      }),
      catchError((errorMessage) => {
        this.modal.dismiss(errorMessage);
        return of(this.data);
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
    const sbCreate = this.gasService.create(this.data).pipe(
      tap(() => {
        this.modal.close();
      }),
      catchError((errorMessage) => {
        this.modal.dismiss(errorMessage);
        return of(this.data);
      }),
    ).subscribe((res: any) => {
      Swal.fire({
        icon: res.status == 1 ? 'success' : 'error',
        title: res.status == 1 ? 'Thêm mới thành công' : 'Thêm mới thất bại',
        confirmButtonText: 'Xác nhận',
        text: (res.status == 1 ? 'Thêm mới thành công' : res.status == 0 ? res.error.msg : 'Thêm mới thất bại' ),
      });
      this.data = EMPTY_CUSTOM
    });
    this.subscriptions.push(sbCreate);
  }

  private prepareCustomer() {
    const formData = this.formGroup.value;
    this.data.code = formData.Code;
    this.data.name = formData.Name;
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
  
  clearModel(){
    EMPTY_CUSTOM.code = ''
    EMPTY_CUSTOM.name = ''
    EMPTY_CUSTOM.gasId = '00000000-0000-0000-0000-000000000000'
    this.data = EMPTY_CUSTOM;
  }
}