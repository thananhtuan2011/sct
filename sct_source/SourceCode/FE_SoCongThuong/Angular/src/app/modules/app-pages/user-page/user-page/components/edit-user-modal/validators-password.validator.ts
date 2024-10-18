import { AbstractControl, ValidatorFn } from '@angular/forms';

export function PasswordValidator(): ValidatorFn {
    return (control: AbstractControl): { [key: string]: any } | null => {
        const hasUppercase = /[A-Z]/.test(control.value);
        const hasLowercase = /[a-z]/.test(control.value);
        const hasNumber = /\d/.test(control.value);
        const hasSpecialCharacter = /[!@#$%^&*()_+\-=\[\]{};':"\\|,.<>\/?]/.test(control.value);
        const isValid = hasUppercase && hasLowercase && hasNumber && hasSpecialCharacter;

        return isValid ? null : { invalidPassword: true };
    };
}
