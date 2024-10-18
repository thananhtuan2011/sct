import { AbstractControl } from '@angular/forms';

export class ConfirmPasswordValidator {
  /**
   * Check matching password with confirm password
   * @param control AbstractControl
   */
  static MatchPassword(control: AbstractControl): void {
    const password = control.get('MatKhau')?.value;
    const confirmPassword = control.get('NhapLaiMatKhau')?.value;

    if (password !== confirmPassword) {
      control.get('NhapLaiMatKhau')?.setErrors({ ConfirmPassword: true });
    }
    else{
      control.get('NhapLaiMatKhau')?.setErrors(null);
    }
  }
}
