import { CommonModule } from '@angular/common';
import { NgModule } from "@angular/core";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { NgbModule } from "@ng-bootstrap/ng-bootstrap";

import { DatePickerCustomComponent } from "./date-picker-custom.component";

@NgModule({
    imports: [
        FormsModule,
        NgbModule,
        ReactiveFormsModule,
        CommonModule,
    ],
    declarations: [DatePickerCustomComponent],
    exports: [DatePickerCustomComponent],
})

export class DatePickerCustomModule {}