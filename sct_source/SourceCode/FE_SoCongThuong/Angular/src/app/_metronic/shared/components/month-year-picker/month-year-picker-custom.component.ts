import { Component, forwardRef, OnInit, Input, AfterViewInit, ChangeDetectorRef } from '@angular/core';
import { NG_VALUE_ACCESSOR, ControlValueAccessor } from "@angular/forms";

import * as $ from 'jquery';
import 'bootstrap-datepicker';

@Component({
  selector: "app-month-year-picker-custom",
  templateUrl: "./month-year-picker-custom.component.html",
  styleUrls: ["./month-year-picker-custom.component.scss"],
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => MonthYearPickerCustomComponent),
      multi: true,
    },
  ],
})

export class MonthYearPickerCustomComponent implements ControlValueAccessor, AfterViewInit, OnInit {
  @Input() ComponentId: string;
  @Input() ViewMode: "months" | "years" = "months";
  @Input() Placeholder: string = "";
  onChange: any = () => { };
  onTouch: any = () => { };
  onDisable: boolean = false;
  dom: any;
  first: boolean = false;

  currentValue: any;

  constructor(
    private cdr: ChangeDetectorRef
  ) { }

  ngOnInit(): void {
    /** Việt hóa */
    $(() => {
      ($ as any).fn.datepicker.dates.vi = {
        days: ["Chủ nhật", "Thứ hai", "Thứ ba", "Thứ tư", "Thứ năm", "Thứ sáu", "Thứ bảy"],
        daysShort: ["CN", "Thứ 2", "Thứ 3", "Thứ 4", "Thứ 5", "Thứ 6", "Thứ 7"],
        daysMin: ["CN", "T2", "T3", "T4", "T5", "T6", "T7"],
        months: ["Tháng 1", "Tháng 2", "Tháng 3", "Tháng 4", "Tháng 5", "Tháng 6", "Tháng 7", "Tháng 8", "Tháng 9", "Tháng 10", "Tháng 11", "Tháng 12"],
        monthsShort: ["Th1", "Th2", "Th3", "Th4", "Th5", "Th6", "Th7", "Th8", "Th9", "Th10", "Th11", "Th12"],
        today: "Hôm nay",
        clear: "Xóa",
      }
    })
  }

  writeValue(value: any) {
    if (value != "" && value != null) {
      /** Tháng/Năm */
      if (this.ViewMode == "months") {
        const date = value.split("-");
        if (date.length == 2) {
          this.currentValue = date;
        }
        this.setValue();
      }
      /** Năm */
      else {
        this.currentValue = value;
        this.setValue();
      }
    } else if (value == "" && this.first) {
      this.currentValue = "";
      this.setValue();
    }
    this.first = true;
  }

  registerOnChange(fn: any) {
    this.onChange = fn;
  }

  registerOnTouched(fn: any): void {
    this.onTouch = fn;
  }

  setDisabledState(Disabled: boolean) {
    this.onDisable = Disabled;
    this.cdr.detectChanges();
  }

  setValue() {
    if (this.dom != undefined && this.dom.length > 0) {
      if (this.currentValue != undefined) {
        $(() => {
          if (this.currentValue == "") {
            this.dom.datepicker("update", "");
          } else if (this.ViewMode == "months") {
            this.dom.datepicker("update", `${this.currentValue[1]}/${this.currentValue[0]}`);
          } else if (this.ViewMode == "years") {
            this.dom.datepicker("update", `${this.currentValue}`);
          }
        })
      }
    }
  }

  open() {
    if (this.dom != undefined && this.dom.length > 0) {
      $(() => {
        this.dom.datepicker('show')
      })
    }
  }

  ngAfterViewInit() {
    $(() => {
      try {
        this.dom = ($(`#${this.ComponentId}`) as any);
        if (this.dom.length > 0) {
          let dp = this.dom.datepicker({
            format: this.ViewMode == "months" ? "mm/yyyy" : "yyyy",
            viewMode: this.ViewMode,
            minViewMode: this.ViewMode,
            autoclose: true,
            language: 'vi'
          });
          dp.on('change', (e: any) => {
            if (e.target.value != "" && e.target.value != undefined) {
              if (this.ViewMode == "months") {
                const date = e.target.value.split("/");
                if (date.length == 2) {
                  this.onChange(`${date[1]}-${date[0]}`);
                  this.onTouch();
                }
              } else if (this.ViewMode == "years") {
                this.onChange(e.target.value);
                this.onTouch();
              }
            } else {
              this.onChange("");
              this.onTouch();
            }
          })
          this.setValue();
        }
      }
      catch (error) {
        console.error(error);
      }
    })
  }
}