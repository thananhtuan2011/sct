import { NgModule } from '@angular/core';
import { PasswordStrengthComponent } from './password-strength.component';
import { CommonModule } from '@angular/common';

@NgModule({
    imports: [CommonModule],
    declarations: [PasswordStrengthComponent],
    exports: [PasswordStrengthComponent]
})

export class PasswordStrengthModule { }