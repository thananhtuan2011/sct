import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';

import { ProductData } from '../../../_models/regulation-conformity-AM.model';

@Component({
    selector: 'app-add-modal',
    templateUrl: './add-modal.component.html',
    styleUrls: ['./add-modal.component.scss'],

})
export class AddProductModalComponent implements OnInit {
    productData: ProductData = {} as ProductData;
    formGroup: FormGroup;

    constructor(
        private fb: FormBuilder,
        public modal: NgbActiveModal,
    ) { }

    ngOnInit(): void {
        this.loadForm();
    }

    loadForm() {
        this.formGroup = this.fb.group({
            ProductName: [this.productData.productName, Validators.required],
            Note: [this.productData.note]
        });
    }

    save() {
        this.modal.dismiss(this.prepareData());
    }

    private prepareData(): ProductData {
        const formData = this.formGroup.value;
        let obj: ProductData = {
            productName: formData.ProductName,
            note: formData.Note,
        }
        return obj
    }

    // helpers for View
    isControlValid(controlName: string): boolean {
        const control = this.formGroup.controls[controlName];
        return control.valid && (control.dirty || control.touched);
    }

    isControlInvalid(controlName: string): boolean {
        const control = this.formGroup.controls[controlName];
        return control.invalid && (control.dirty || control.touched);
    }

    controlHasError(validation: any, controlName: any): boolean {
        const control = this.formGroup.controls[controlName];
        return control.hasError(validation) && (control.dirty || control.touched);
    }

    isControlTouched(controlName: any): boolean {
        const control = this.formGroup.controls[controlName];
        return control.dirty || control.touched;
    }

    check_formGroup() {
        if (this.formGroup.invalid) {
            this.formGroup.markAllAsTouched();
        }
        else {
            this.save();
        }
    }
}
