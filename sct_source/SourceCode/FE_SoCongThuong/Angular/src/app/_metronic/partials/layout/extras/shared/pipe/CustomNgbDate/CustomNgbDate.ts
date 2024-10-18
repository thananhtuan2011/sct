import { Injectable } from '@angular/core';
import { NgbDateAdapter, NgbDateParserFormatter, NgbDateStruct } from '@ng-bootstrap/ng-bootstrap';

@Injectable()
export class CustomAdapter extends NgbDateAdapter<string> {
	readonly DELIMITER = '/';

	fromModel(value: string | null): NgbDateStruct | null {
		if (value) {
			const date = value.split(this.DELIMITER);
			return {
				day: parseInt(date[0], 10),
				month: parseInt(date[1], 10),
				year: parseInt(date[2], 10),
			};
		}
		return null;
	}

	toModel(date: NgbDateStruct | null): string | null{
		if (!date) {
			return null;
		}
		else if (date?.day != null && date?.day.toString().length < 2 && date?.month != null && date?.month.toString().length < 2) {
			return "0" + date.day + this.DELIMITER + "0" + date.month + this.DELIMITER + date.year;
		}
		else if (date?.day != null && date?.day.toString().length < 2) {
			return "0" + date.day + this.DELIMITER + date.month + this.DELIMITER + date.year;
		}
		else if (date?.month != null && date?.month.toString().length < 2) {
			return date.day + this.DELIMITER + "0" + date.month + this.DELIMITER + date.year;
		}
		else {
			return date.day + this.DELIMITER + date.month + this.DELIMITER + date.year;
		}		
	}
}

@Injectable()
export class CustomDateParserFormatter extends NgbDateParserFormatter {
	readonly DELIMITER = '/';

	parse(value: string): NgbDateStruct | null {
		if (value) {
			const date = value.split(this.DELIMITER);
			return {
				day: parseInt(date[0], 10),
				month: parseInt(date[1], 10),
				year: parseInt(date[2], 10),
			};
		}
		return null;
	}

	format(date: NgbDateStruct | null): string {
		if (!date) {
			return '';
		}
		else if (date?.day != null && date?.day.toString().length < 2 && date?.month != null && date?.month.toString().length < 2) {
			return "0" + date.day + this.DELIMITER + "0" + date.month + this.DELIMITER + date.year;
		}
		else if (date?.day != null && date?.day.toString().length < 2) {
			return "0" + date.day + this.DELIMITER + date.month + this.DELIMITER + date.year;
		}
		else if (date?.month != null && date?.month.toString().length < 2) {
			return date.day + this.DELIMITER + "0" + date.month + this.DELIMITER + date.year;
		}
		else {
			return date.day + this.DELIMITER + date.month + this.DELIMITER + date.year;
		}	
	}
}