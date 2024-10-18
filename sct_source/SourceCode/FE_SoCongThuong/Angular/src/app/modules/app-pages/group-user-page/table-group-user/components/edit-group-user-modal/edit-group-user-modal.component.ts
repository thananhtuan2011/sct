import { Component, Input, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal, NgbDateAdapter, NgbDateParserFormatter } from '@ng-bootstrap/ng-bootstrap';
import { of, Subscription } from 'rxjs';
import { catchError, finalize, first, tap } from 'rxjs/operators';
import { Options } from 'select2';
import { SelectOptionData } from 'src/app/_metronic/shared/components/select-custom/select-custom.interface';
import Swal from 'sweetalert2';
import { GroupUserModel } from '../../../_models/group-user.model';
import { GroupUserService } from '../../../_services/group-user.service';

const EMPTY_CUSTOM: GroupUserModel = {
  id: '',
  groupName: '',
  groupId: '00000000-0000-0000-0000-000000000000',
  priority: 0
};

@Component({
  selector: 'app-edit-group-user-modal',
  templateUrl: './edit-group-user-modal.component.html',
  styleUrls: ['./edit-group-user-modal.component.scss']
})
export class EditGroupUserModalComponent implements OnInit, OnDestroy {
  @Input() id: number;
  isLoading$:any;
  userData: GroupUserModel;
  formGroup: FormGroup;
  default_value = "00000000-0000-0000-0000-000000000000"
  private subscriptions: Subscription[] = [];
  constructor(
    private UserService: GroupUserService,
    private fb: FormBuilder, public modal: NgbActiveModal
    ) { }

  ngOnInit(): void {
    this.isLoading$ = this.UserService.isLoading$;
    this.loadCustom();
  }

  clear(){
    EMPTY_CUSTOM.groupId = '00000000-0000-0000-0000-000000000000'
    EMPTY_CUSTOM.groupName = ""
    EMPTY_CUSTOM.priority = 0
    EMPTY_CUSTOM.id = ""
  }
  // function for selection template
  public templateSelection = (state: any): JQuery | string => {
    if (!state.id) {
      return state.text;
    }
    return jQuery('<span class="form-select form-select-solid form-select-lg">'+ state.text + '</span>');
  }

  loadCustom() {
    if (!this.id) {
      this.userData = EMPTY_CUSTOM;
      this.loadForm();
    } else {
      const sb = this.UserService.getItemById(this.id).pipe(
        first(),
        catchError((errorMessage) => {
          this.modal.dismiss(errorMessage);
          return of(EMPTY_CUSTOM);
        })
      ).subscribe((res: any) => {
        this.userData = res.items[0];
        this.loadForm();
      });
      this.subscriptions.push(sb);
    }
  }

  loadForm() {
    this.formGroup = this.fb.group({
      TenNhom: [this.userData.groupName, Validators.compose([Validators.required, Validators.minLength(3), Validators.maxLength(50)])],
      DoUuTien: [this.userData.priority, Validators.compose([Validators.required, Validators.pattern(/^-?(0|[0-9]\d*)?$/)])],
    });
  }

  save() {
    this.prepareCustomer();
    if (this.userData.groupId && this.userData.groupId !== this.default_value) {
      this.edit();
    } else {
      this.create();
    }
  }

  edit() {
    const sbUpdate = this.UserService.update(this.userData).pipe(
      tap(() => {
        this.modal.close();
      }),
      catchError((errorMessage) => {
        this.modal.dismiss(errorMessage);
        return of(this.userData);
      }),
    ).subscribe((res: any) => {
      Swal.fire({
        icon: res.status == 1 ? 'success' : 'error',
        title: res.status == 1 ? 'Chỉnh sửa thành công' : 'Chỉnh sửa thất bại',
        confirmButtonText: 'Xác nhận',
        text: 'Chỉnh sửa ' + (res.status == 1 ? 'thành công' : 'thất bại'),
      });
    });
    this.subscriptions.push(sbUpdate);
  }

  create() {
    const sbCreate = this.UserService.create(this.userData).pipe(
      tap(() => {
        this.modal.close();
      }),
      catchError((errorMessage) => {
        
        Swal.fire({
          icon: 'error',
          title: 'Thêm mới thất bại',
          confirmButtonText: 'Xác nhận',
          text: errorMessage.error.msg,
        });
        this.modal.dismiss(errorMessage);
        return of(this.userData);
      }),
    ).subscribe((res: any) => {
      Swal.fire({
        icon: res.status == 1 ? 'success' : 'error',
        title: res.status == 1 ? 'Thêm mới thành công' : 'Thêm mới thất bại',
        confirmButtonText: 'Xác nhận',
        text: 'Thêm mới ' + (res.status == 1 ? 'thành công' : 'thất bại'),
      });
      this.clear()
      this.userData = EMPTY_CUSTOM
    });
    this.subscriptions.push(sbCreate);
  }

  private prepareCustomer() {
    const formData = this.formGroup.value;
    this.userData.groupName = formData.TenNhom;
    this.userData.priority = Number(formData.DoUuTien);
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
