import { Injectable } from '@angular/core';
import { NgbDatepickerI18n, NgbDateStruct } from '@ng-bootstrap/ng-bootstrap';

@Injectable()
export class CustomNgbDatepickerI18n extends NgbDatepickerI18n {
    //Việt hoá NgbDatepicker:

    getWeekdayShortName(weekday: number): string {
        // Thay đổi giá trị của mảng weekdaysShort
        const weekdaysShort = ['T2', 'T3', 'T4', 'T5', 'T6', 'T7', 'CN'];
        return weekdaysShort[weekday - 1];
    }

    getMonthFullName(month: number): string {
        // Thay đổi giá trị của mảng monthsFull
        const monthsFull = [
            'Tháng 1', 'Tháng 2', 'Tháng 3', 'Tháng 4',
            'Tháng 5', 'Tháng 6', 'Tháng 7', 'Tháng 8',
            'Tháng 9', 'Tháng 10', 'Tháng 11', 'Tháng 12'
        ];
        return monthsFull[month - 1];
    }

    getMonthShortName(month: number): string {
        // Thay đổi giá trị của mảng monthsShort
        const monthsShort = [
            'Th1', 'Th2', 'Th3', 'Th4',
            'Th5', 'Th6', 'Th7', 'Th8',
            'Th9', 'Th10', 'Th11', 'Th12'
        ];
        return monthsShort[month - 1];
    }

    getDayAriaLabel(date: NgbDateStruct): string {
        // Thay đổi định dạng của chuỗi ngày theo định dạng "dd/MM/yyyy"
        return `${date.day}/${date.month}/${date.year}`;
    }

    getWeekdayLabel(weekday: number): string {
        // Thay đổi giá trị của mảng weekdays
        const weekdays = ['T2', 'T3', 'T4', 'T5', 'T6', 'T7', 'CN'];
        return weekdays[weekday - 1];
    }
}
