import { CommonModule } from '@angular/common';
import { NgModule } from "@angular/core";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { NgbModule } from "@ng-bootstrap/ng-bootstrap";

import { BrowserModule } from "@angular/platform-browser";
// import {
//     FaIconLibrary,
//     FontAwesomeModule
// } from "@fortawesome/angular-fontawesome";

// import { faCalendar, faClock } from "@fortawesome/free-regular-svg-icons";

import { DateTimePickerComponent } from "./date-time-picker.component";

@NgModule({
    imports: [
        FormsModule,
        NgbModule,
        ReactiveFormsModule,
        CommonModule,
        // FontAwesomeModule,
        // BrowserModule,
    ],
    declarations: [DateTimePickerComponent],
    exports: [DateTimePickerComponent],
})

export class DateTimePickerModule {
    // constructor(library: FaIconLibrary) {
    //     library.addIcons(faCalendar, faClock);
    // }
}