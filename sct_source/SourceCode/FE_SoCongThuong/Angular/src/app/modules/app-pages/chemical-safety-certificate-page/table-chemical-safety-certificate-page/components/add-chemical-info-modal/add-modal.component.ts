import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-add-modal',
  templateUrl: './add-modal.component.html',
  styleUrls: ['./add-modal.component.scss'],
})

export class AddChemicalInfoModalComponent implements OnInit {
  formGroup: FormGroup;
  show: boolean = false;

  constructor(
    private fb: FormBuilder,
    public modal: NgbActiveModal,
  ) { }

  ngOnInit(): void {
    this.loadForm();
  }

  loadForm() {
    this.formGroup = this.fb.group({
      tradeName: ["", Validators.required],
      nameOfChemical: ["", Validators.required],
      casCode: [""],
      chemicalFormula: [""],
      content: [""],
      mass: [""],
    })
    this.show = true;
  }

  private prepare() {
    const formData = this.formGroup.value;
    return formData
  }

  save() {
    this.modal.dismiss(this.prepare());
  }

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
    this.formGroup.markAllAsTouched();
    if (!this.formGroup.invalid) {
      this.save()
    }
  }
}
