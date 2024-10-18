import { AbstractControl, FormControl, ValidatorFn } from '@angular/forms';

export function DateValidator(): ValidatorFn {
    return (control: AbstractControl): { [key: string]: any } | null => {
        const value = control.value;
        const [day, month, year] = value.split('/');
        const date = new Date(`${month}/${day}/${year}`);
        if (isNaN(date.getTime())) {
            return { date: true };
        }
        return null;
    };
}
