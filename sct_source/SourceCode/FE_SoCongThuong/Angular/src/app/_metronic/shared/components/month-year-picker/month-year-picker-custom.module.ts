import { CommonModule } from '@angular/common';
import { NgModule } from "@angular/core";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { NgbModule } from "@ng-bootstrap/ng-bootstrap";

import { MonthYearPickerCustomComponent } from "./month-year-picker-custom.component";

@NgModule({
    imports: [
        FormsModule,
        NgbModule,
        ReactiveFormsModule,
        CommonModule,
    ],
    declarations: [MonthYearPickerCustomComponent],
    exports: [MonthYearPickerCustomComponent],
})

export class MonthYearPickerCustomModule {}