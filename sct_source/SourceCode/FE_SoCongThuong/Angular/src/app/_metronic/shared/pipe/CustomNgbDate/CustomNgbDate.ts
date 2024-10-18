import { Injectable } from '@angular/core';
import { NgbDateAdapter, NgbDateParserFormatter, NgbDateStruct } from '@ng-bootstrap/ng-bootstrap';

@Injectable()
export class CustomAdapter extends NgbDateAdapter<string> {
	readonly DELIMITER = '/';

	fromModel(value: string | null): NgbDateStruct | null {
		if (value) {
			const [day, month, year] = value.split(this.DELIMITER).map(Number);
			return { day, month, year };
		}
		return null;
	}

	toModel(date: NgbDateStruct | null): string | null {
		if (!date) {
			return null;
		}

		const day = date.day.toString().padStart(2, '0');
		const month = date.month.toString().padStart(2, '0');
		const year = date.year;
		return `${day}${this.DELIMITER}${month}${this.DELIMITER}${year}`;
	}
}

@Injectable()
export class CustomDateParserFormatter extends NgbDateParserFormatter {
	readonly DELIMITER = '/';

	parse(value: string): NgbDateStruct | null {
		if (value) {
			const [day, month, year] = value.split(this.DELIMITER).map(Number);
			return { day, month, year };
		}
		return null;
	}

	format(date: NgbDateStruct | null): string {
		if (!date) {
			return '';
		}

		const day = date.day.toString().padStart(2, '0');
		const month = date.month.toString().padStart(2, '0');
		const year = date.year;
		return `${day}${this.DELIMITER}${month}${this.DELIMITER}${year}`;
	}
}
