import { AbstractControl, ValidatorFn } from '@angular/forms';

export function SelectValidator(): ValidatorFn {
    return (control: AbstractControl): { [key: string]: any } | null => {
        const isValid = control.value != "00000000-0000-0000-0000-000000000000";
        return isValid ? null : { defaultvalue: true };
    };
}