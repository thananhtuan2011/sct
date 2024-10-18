import { Component, Input, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal, NgbDateAdapter, NgbDateParserFormatter } from '@ng-bootstrap/ng-bootstrap';
import { of, Subscription } from 'rxjs';
import { catchError, finalize, first, tap } from 'rxjs/operators';
import { Options } from 'select2';
import { SelectOptionData } from 'src/app/_metronic/shared/components/select-custom/select-custom.interface';
import Swal from 'sweetalert2';
import { UserModel } from '../../../_models/user.model';
import { UserService } from '../../../_services/user.service';
import { ConfirmPasswordValidator } from '../edit-user-modal/confirm-password.validator';
import { PasswordValidator } from '../edit-user-modal/validators-password.validator';

@Component({
  selector: 'app-change-password-modal',
  templateUrl: './change-password-modal.component.html',
  styleUrls: ['./change-password-modal.component.scss']
})

export class ChangePasswordModalComponent implements OnInit {
  @Input() username: any;
  isLoading$: any;
  userData: UserModel;
  formGroup: FormGroup;
  default_value = "00000000-0000-0000-0000-000000000000"
  public options: Options;
  private subscriptions: Subscription[] = [];
  constructor(
    private UserService: UserService,
    private fb: FormBuilder,
    public modal: NgbActiveModal
  ) { }

  ngOnInit(): void {
    this.isLoading$ = this.UserService.isLoading$;
    this.loadForm()
  }

  loadForm() {
    this.formGroup = this.fb.group({
      MatKhau: ["", [Validators.minLength(8), Validators.required, PasswordValidator()]],
      NhapLaiMatKhau: ["", 
        [Validators.minLength(8), Validators.required]
      ],
    },
    {
        validator: ConfirmPasswordValidator.MatchPassword
    });
  }

  save() {
    const formData = this.formGroup.value;
    let data = {
      userName: this.username,
      passWord: formData.MatKhau,
      recaptchaToken: ""
    }
    const sbUpdate = this.UserService.changePassword(data).pipe(
      tap(() => {
        this.modal.close();
      }),
      catchError((errorMessage) => {
        this.modal.dismiss(errorMessage);
        return of(data);
      }),
    ).subscribe((res: any) => {
      Swal.fire({
        icon: res.status == 1 ? 'success' : 'error',
        title: res.status == 1 ? 'Đổi mật khẩu thành công' : 'Đổi mật khẩu thất bại',
        confirmButtonText: 'Xác nhận',
        text: '' + (res.status == 1 ? 'Đổi mật khẩu thành công' : res.error.msg),
      });
    });
    this.subscriptions.push(sbUpdate);
  }

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

  matchedPassword(): boolean {
    const password = this.formGroup.controls['MatKhau'].value;
    const confirmPassword = this.formGroup.controls['NhapLaiMatKhau'].value;

    if (confirmPassword.dirty || confirmPassword.touched) {
      return password == confirmPassword;
    }
    return false;
  }
}
