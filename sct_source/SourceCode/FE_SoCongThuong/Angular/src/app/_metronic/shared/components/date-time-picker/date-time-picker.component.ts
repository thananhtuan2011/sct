import { Component, OnInit, Input, forwardRef, ViewChild, AfterViewInit, Injector } from "@angular/core";
import { NgbTimeStruct, NgbDateStruct, NgbPopoverConfig, NgbPopover, NgbDatepicker } from "@ng-bootstrap/ng-bootstrap";
import { NG_VALUE_ACCESSOR, ControlValueAccessor, NgControl } from "@angular/forms";
import { DatePipe } from "@angular/common";
import { DateTimeModel } from "./date-time.model";
import { noop } from "rxjs";

@Component({
  selector: "app-date-time-picker",
  templateUrl: "./date-time-picker.component.html",
  styleUrls: ["./date-time-picker.component.scss"],
  providers: [
    DatePipe,
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => DateTimePickerComponent),
      multi: true
    }
  ]
})
export class DateTimePickerComponent implements ControlValueAccessor, OnInit, AfterViewInit {
  @Input() dateString: string;
  @Input() hourStep = 1;
  @Input() minuteStep = 10;
  // @Input() secondStep = 30;
  // @Input() seconds = true;
  @Input() disabled = false;
  // @Input() AllowInput = false;
  // @Input() AllowPaste = false;

  //Mặc định 50 năm trước năm hiện tại cấu trúc là {day: number, month: number, year: number}
  @Input() MinDate: NgbDateStruct = { year: new Date().getFullYear() - 50, month: 1, day: 1 };

  //Mặc định 50 năm sau năm hiện tại cấu trúc là {day: number, month: number, year: number}
  @Input() MaxDate: NgbDateStruct = { year: new Date().getFullYear() + 50, month: 12, day: 31 };

  public inputDatetimeFormat = "dd/MM/yyyy HH:mm";
  public showTimePickerToggle = false;
  public datetime: DateTimeModel = new DateTimeModel()

  // public firstTimeAssign = true;
  // @ViewChild(NgbDatepicker, { static: true })
  // private dp: NgbDatepicker;

  @ViewChild(NgbPopover, { static: true })
  public popover: NgbPopover;

  public onTouched: () => void = noop;
  public onChange: (_: any) => void = noop;

  public ngControl: NgControl;
  showError: boolean = false;

  constructor(public config: NgbPopoverConfig, public inj: Injector) {
    config.autoClose = "outside";
    config.placement = "auto";
  }

  ngOnInit(): void {
    this.ngControl = this.inj.get(NgControl);
  }

  ngAfterViewInit(): void {
    this.popover.hidden.subscribe($event => {
      this.showTimePickerToggle = false;
    });
  }

  writeValue(newModel: string) {
    if (newModel) {
      this.datetime = Object.assign(
        this.datetime,
        DateTimeModel.fromLocalString(newModel, 'api')
      );
      this.setDateStringModel();
    } else {
      this.datetime = new DateTimeModel();
      this.dateString = ""
    }
  }

  registerOnChange(fn: any): void {
    this.onChange = fn;
  }

  registerOnTouched(fn: any): void {
    this.onTouched = fn;
  }

  setDisabledState?(isDisabled: boolean): void {
    this.disabled = isDisabled;
  }

  toggleDateTimeState(event: any) {
    this.showTimePickerToggle = !this.showTimePickerToggle;
    event.stopPropagation();
  }

  onInputChange(event: any) {
    const value = event.target.value.trim();
    const dt = DateTimeModel.fromLocalString(value, 'client');
    if (dt) {
      this.datetime = dt;
      this.showError = false;
      this.setDateStringModel();
    }
    else {
      this.showError = true;
      this.onChange("");
    }
  }

  onDateChange($event: string | NgbDateStruct, dp: NgbDatepicker) {
    const date = new DateTimeModel($event);
    if (!date) {
      this.dateString = this.dateString;
      return;
    }
    if (!this.datetime) {
      this.datetime = date;
    }
    this.datetime.year = date.year;
    this.datetime.month = date.month;
    this.datetime.day = date.day;
    this.datetime.hour = 0;
    this.datetime.minute = 0;
    this.datetime.second = 0;
    const adjustedDate = new Date(this.datetime.toString() ?? '');
    if (this.datetime.timeZoneOffset !== adjustedDate.getTimezoneOffset()) {
      this.datetime.timeZoneOffset = adjustedDate.getTimezoneOffset();
    }

    this.setDateStringModel();
  }

  onTimeChange(event: NgbTimeStruct) {
    this.datetime.hour = event.hour;
    this.datetime.minute = event.minute;
    this.datetime.second = event.second;

    this.setDateStringModel();
  }

  setDateStringModel() {
    this.dateString = this.datetime.toString();

    if (this.dateString) {
      this.onChange(this.dateString);
    }
  }

  inputBlur(event: any) {
    this.onTouched();
  }
}
