import { Component, Input, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { CommitManagerPageService } from '../../../_services/commit-manager-page.service';
import { Options } from 'select2';

@Component({
  selector: 'app-edit-items-modal',
  templateUrl: './edit-modal.component.html',
  styleUrls: ['./edit-modal.component.scss'],

})
export class EditItemsComponent implements OnInit {
  @Input() typeData: any;
  isLoading$:any;
  formGroup: FormGroup;
  options: Options
  constructor(
    public commitManager: CommitManagerPageService,
    private fb: FormBuilder, public modal: NgbActiveModal,
    ) { }

  ngOnInit(): void {
    this.isLoading$ = this.commitManager.isLoading$;
    this.loadForm();
    this.options = {
      theme:'bootstrap5',
      templateSelection: this.templateSelection,
    };
  }
  
  public templateSelection = (state: any): JQuery | string => {
    if (!state.id) {
      return state.text;
    }
    return jQuery('<span class="form-select form-select-solid form-select-lg">'+ state.text + '</span>');
  }
  
  loadForm() {
    this.formGroup = this.fb.group({
      LoaiHinh: '00000000-0000-0000-0000-000000000000',
      TenMatHang: ['', Validators.required]
    });
  }
  
  controlHasError(validation: any, controlName: any): boolean {
    const control = this.formGroup.controls[controlName];
    return control.hasError(validation) && (control.dirty || control.touched);
  }

  save() {
    this.modal.dismiss(this.prepareData());
  }

  private prepareData() {
    const formData = this.formGroup.value;
    return {
      loaiHinh: formData.LoaiHinh,
      tenLoaiHinh: this.typeData.find((item: any) => item.id == formData.LoaiHinh).text,
      tenMatHang: formData.TenMatHang
    }
  }

  isDefaultValue(controlName: any): boolean {
    const control = this.formGroup.controls[controlName];
    if (control.value == '' || control.value == '00000000-0000-0000-0000-000000000000') {
      control.setErrors({'default': true})
    } else {
      control.setErrors(null)
    }
    return control.hasError('default') && (control.dirty || control.touched);
  }

  check_formGroup() {
    if (this.formGroup.invalid ) {
      this.formGroup.markAllAsTouched();
      this.formGroup.updateValueAndValidity();
    }
    else {
      this.save()
    }
  }
    
}
