import { NgbTimeStruct, NgbDateStruct } from "@ng-bootstrap/ng-bootstrap";
import { DatePipe } from "@angular/common";

export interface NgbDateTimeStruct extends NgbDateStruct, NgbTimeStruct { }

export class DateTimeModel implements NgbDateTimeStruct {
  year: number;
  month: number;
  day: number;
  hour: number;
  minute: number;
  second: number;

  timeZoneOffset: number;

  public constructor(init?: Partial<DateTimeModel>) {
    Object.assign(this, init);
  }

  public static fromLocalString(dateString: string, type: 'api' | 'client'): any {
    if (dateString.length == 0) {
      return null;
    }

    if (type == 'client') {
      const dateFormat: string = "dd/mm/yyyy HH:MM";
      const regexPattern: string = dateFormat
        .replace(/dd/g, "\\d{2}")
        .replace(/mm/g, "\\d{2}")
        .replace(/yyyy/g, "\\d{4}")
        .replace(/HH/g, "\\d{2}")
        .replace(/MM/g, "\\d{2}");

      const isMatch: boolean = new RegExp(`^${regexPattern}$`).test(dateString);

      if (isMatch) {
        //Dữ liệu client thay đổi
        const [day, month, year, hours, minutes] = dateString.match(/\d+/g)!.map(Number);
        const date: Date = new Date(year, month - 1, day, hours, minutes);

        return new DateTimeModel({
          year: date.getFullYear(),
          month: date.getMonth() + 1,
          day: date.getDate(),
          hour: date.getHours(),
          minute: date.getMinutes(),
          second: 0,
          timeZoneOffset: date.getTimezoneOffset()
        });
      }

      else {
        return null;
      }
    }
    if (type == 'api') {
      // Dữ liệu API trả về
      const date = new Date(dateString);
      const isValidDate = !isNaN(date.valueOf());
      if (dateString.length > 0 || isValidDate) {
        return new DateTimeModel({
          year: date.getFullYear(),
          month: date.getMonth() + 1,
          day: date.getDate(),
          hour: date.getHours(),
          minute: date.getMinutes(),
          second: 0,
          timeZoneOffset: date.getTimezoneOffset()
        });
      }
    }
  }

  private isInteger(value: any): value is number {
    return (
      typeof value === "number" &&
      isFinite(value) &&
      Math.floor(value) === value
    );
  }

  public toString(): any {
    if (this.isInteger(this.year) && this.isInteger(this.month) && this.isInteger(this.day)) {
      const year = this.year.toString().padStart(2, "0");
      const month = this.month.toString().padStart(2, "0");
      const day = this.day.toString().padStart(2, "0");

      if (!this.hour) {
        this.hour = 0;
      }
      if (!this.minute) {
        this.minute = 0;
      }
      if (!this.second) {
        this.second = 0;
      }
      if (!this.timeZoneOffset) {
        this.timeZoneOffset = new Date().getTimezoneOffset();
      }

      const hour = this.hour.toString().padStart(2, "0");
      const minute = this.minute.toString().padStart(2, "0");
      const second = this.second.toString().padStart(2, "0");

      const tzo = -this.timeZoneOffset;
      const dif = tzo >= 0 ? "+" : "-",
        pad = function (num: any) {
          const norm = Math.floor(Math.abs(num));
          return (norm < 10 ? "0" : "") + norm;
        };

      const isoString = `${pad(year)}-${pad(month)}-${pad(day)}T${pad(hour)}:${pad(minute)}:${pad(second)}${dif}${pad(tzo / 60)}:${pad(tzo % 60)}`;
      return isoString;
    }

    return null;
  }
}
