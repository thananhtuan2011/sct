import { Component, forwardRef, OnInit, Input, Output, EventEmitter, AfterViewInit, OnChanges, ChangeDetectorRef } from '@angular/core';
import { NgbDateAdapter, NgbDateParserFormatter, NgbDatepickerI18n, NgbDateStruct } from '@ng-bootstrap/ng-bootstrap';
import { CustomAdapter, CustomDateParserFormatter } from '../../pipe/CustomNgbDate/CustomNgbDate';
import { NG_VALUE_ACCESSOR, ControlValueAccessor, NgControl, FormGroup, FormControl, Validators } from "@angular/forms";
import { CustomNgbDatepickerI18n } from '../../pipe/CustomNgbDate/ngb-date-picker-i18n.vi';
import { DateValidator } from '../../pipe/CustomNgbDate/ValidatorDate';

@Component({
  selector: "app-date-picker-custom",
  templateUrl: "./date-picker-custom.component.html",
  styleUrls: ["./date-picker-custom.component.scss"],
  providers: [
    { provide: NgbDateAdapter, useClass: CustomAdapter },
    { provide: NgbDateParserFormatter, useClass: CustomDateParserFormatter },
    { provide: NgbDatepickerI18n, useClass: CustomNgbDatepickerI18n },
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => DatePickerCustomComponent),
      multi: true,
    },
  ],
})

export class DatePickerCustomComponent implements ControlValueAccessor, OnInit {
  @Input() MinDate: NgbDateStruct = { year: new Date().getFullYear() - 50, month: 1, day: 1 }; //Mặc định 50 năm trước năm hiện tại cấu trúc là {day: number, month: number, year: number}
  @Input() MaxDate: NgbDateStruct = { year: new Date().getFullYear() + 50, month: 12, day: 31 }; //Mặc định 50 năm sau năm hiện tại cấu trúc là {day: number, month: number, year: number}
  @Input() Placeholder: string = "DD/MM/YYYY"; //Placeholder
  @Input() Disable: boolean = false;
  @Input() AutoComplete: string = "off"; //Tự hoàn thành
  @Input() AllowInput: boolean = false; //Cho phép nhập
  @Input() AllowPaste: boolean = false; //Cho phép Paste
  @Input() Height: string;
  @Input() Width: string;
  @Output() ngbDateChange = new EventEmitter<string>();

  constructor() { }

  form = new FormGroup({
    date: new FormControl("", Validators.compose([Validators.pattern(/^([0-2][0-9]|(3)[0-1])(\/)(((0)[0-9])|((1)[0-2]))(\/)\d{4}$/)]))
  });

  onChange: any = () => { };
  onTouch: any = () => { };

  ngOnInit() {
    this.form.controls.date.valueChanges.subscribe(x => {
      this.writeValue(x);
      this.onChange(x);
      this.onTouch(x);
    })
    this.setDisabledState(this.Disable);
  }

  writeValue(value: any) {
    if (value != this.form.controls.date.value) {
      this.form.controls.date.setValue(value, { emitEvent: false });
    }
  }

  registerOnChange(fn: any) {
    this.onChange = fn;
  }

  registerOnTouched(fn: any): void {
    this.onTouch = fn;
  }

  setDisabledState(isDisabled: boolean) {
    // this.Disable = isDisabled;
    if (this.Disable || isDisabled) {
      this.form.disable();
    }
  }

  // onKeyDown(event: KeyboardEvent) {
  //   if (!this.AllowInput) {
  //     // Ngăn người dùng nhập bất kỳ phím nào vào ô input trừ Backspace
  //     if (event.key !== 'Backspace') {
  //       event.preventDefault();
  //     } 
  //     // Nếu người dùng ấn Backspace thì xoá ngày set value null
  //     else {
  //       this.form.controls.date.setValue(null);
  //       this.onChange("");
  //       this.form.updateValueAndValidity();
  //     }
  //   }
  // }

  // onPaste(event: ClipboardEvent) {
  //   if (!this.AllowPaste) {
  //     // Ngăn người dùng dán bất kỳ nội dung nào vào ô input
  //     event.preventDefault();
  //   }
  // }
}