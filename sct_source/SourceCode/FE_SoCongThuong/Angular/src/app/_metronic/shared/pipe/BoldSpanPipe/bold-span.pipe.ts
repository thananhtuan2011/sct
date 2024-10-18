import { Pipe, PipeTransform, Sanitizer, SecurityContext } from '@angular/core';

@Pipe({
    name: 'boldSpan'
})
export class BoldSpanPipe implements PipeTransform {

    constructor(
        private sanitizer: Sanitizer
    ) { }

    transform(value: string, regex: any): any {
        return this.sanitize(this.replace(value, regex));
    }

    replace(str: any, regex: any) {
        return str.replace(new RegExp(`(${regex})`, 'gi'), '<b>$1</b>');
    }

    sanitize(str: any) {
        return this.sanitizer.sanitize(SecurityContext.HTML, str);
    }
}