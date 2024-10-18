import { CommonModule } from '@angular/common';
import { NgModule } from "@angular/core";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { NgbModule } from "@ng-bootstrap/ng-bootstrap";

import { DatePickerComponent } from "./date-picker.component";

@NgModule({
    imports: [
        FormsModule,
        NgbModule,
        ReactiveFormsModule,
        CommonModule,
    ],
    declarations: [DatePickerComponent],
    exports: [DatePickerComponent],
})

export class DatePickerModule {}