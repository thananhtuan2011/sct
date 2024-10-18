import { Directive, ElementRef, HostListener } from '@angular/core';

@Directive({
    selector: 'input'
})
export class NoSpaceAtFirstDirective {

    constructor(private el: ElementRef) { }

    @HostListener('keydown', ['$event'])
    onInputChange(event: any) {
        const inputElement = event.target as HTMLInputElement;
        const selectionEnd = inputElement.selectionEnd
        const selectionStart = inputElement.selectionStart == null ? 0 : inputElement.selectionStart
        const valueLength = event.target.value.length

        if (valueLength === 0 && event.keyCode === 32) {
            event.preventDefault();
        } 
        else if (valueLength === selectionEnd && valueLength > selectionStart && selectionStart == 0 && event.keyCode === 32) {
            event.preventDefault();
        }
    }

    @HostListener('paste', ['$event'])
    onPasteChange(event: any) {
        if (event.target.value.length === 0 && event.clipboardData.getData('text').trim() === '') {
            event.preventDefault();
        }
    }
}