import { Component, forwardRef, OnInit } from '@angular/core';
import { NgbDateAdapter, NgbDateParserFormatter } from '@ng-bootstrap/ng-bootstrap';
import { CustomAdapter, CustomDateParserFormatter } from '../../pipe/CustomNgbDate/CustomNgbDate';
import { NG_VALUE_ACCESSOR, ControlValueAccessor, NgControl } from "@angular/forms";

@Component({
  selector: "app-date-picker",
  templateUrl: "./date-picker.component.html",
  styleUrls: ["./date-picker.component.scss"],
  providers: [
    { provide: NgbDateAdapter, useClass: CustomAdapter },
		{ provide: NgbDateParserFormatter, useClass: CustomDateParserFormatter },
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => DatePickerComponent),
      multi: true,
    },
  ],
})
export class DatePickerComponent implements ControlValueAccessor, OnInit {
  public DateString: any = null;
  isDisabled: boolean;
  
  public onChange: (_: any) => void;
  public onTouched: () => void;

  ngOnInit() { }

  writeValue(value: any) {
    this.DateString = value;
  }

  registerOnChange(fn: any) {
    this.onChange = fn;
  }

  registerOnTouched(fn: any): void {
    this.onTouched = fn;
  }

  setDisabledState(isDisabled: boolean) {
    this.isDisabled = isDisabled;
  }

  onInputChange(event: any) {
    this.writeValue(event);
    this.onChange(event);
  }
}