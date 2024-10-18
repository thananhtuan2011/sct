import { ChangeDetectorRef, Component, Input, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal, NgbDateAdapter, NgbDateParserFormatter } from '@ng-bootstrap/ng-bootstrap';
import { of, Subscription } from 'rxjs';
import { catchError, finalize, first, tap } from 'rxjs/operators';
import Swal from 'sweetalert2';

import { TypeOfEnergyModel } from '../../../_models/typeofenergy.model';
import { TypeOfEnergyPageService } from '../../../_services/typeofenergy-page.service';

const EMPTY_CUSTOM: TypeOfEnergyModel = {
  id: '',
  typeOfEnergyId: '',
  typeOfEnergyCode: '',
  typeOfEnergyName: '',
};

@Component({
  selector: 'app-edit-typeofenergy-modal',
  templateUrl: './edit-typeofenergy-modal.component.html',
  styleUrls: ['./edit-typeofenergy-modal.component.scss'],

})
export class EditTypeOfEnergyModalComponent implements OnInit, OnDestroy {
  @Input() id: any;
  isLoading$:any;
  typeofenergyData: TypeOfEnergyModel;
  formGroup: FormGroup;

  private subscriptions: Subscription[] = [];
  public default_value = "00000000-0000-0000-0000-000000000000"
  
  constructor(
    private typeofenergyService: TypeOfEnergyPageService,
    private fb: FormBuilder, public modal: NgbActiveModal,
    ) { }

  ngOnInit(): void {
    this.isLoading$ = this.typeofenergyService.isLoading$;
    this.loadTypeOfEnergy();
  }

  loadTypeOfEnergy() {
    if (!this.id) {
      EMPTY_CUSTOM.typeOfEnergyCode='';
      EMPTY_CUSTOM.typeOfEnergyName='';
      this.typeofenergyData = EMPTY_CUSTOM;
      this.loadForm();
    } else {
      const sb = this.typeofenergyService.getItemById(this.id).pipe(
        first(),
        catchError((errorMessage) => {
          this.modal.dismiss(errorMessage);
          return of(EMPTY_CUSTOM);
        })
      ).subscribe((res: any) => {
        this.typeofenergyData = res.items[0];
        this.loadForm();
      });
      this.subscriptions.push(sb);
    }
  }

  loadForm() {
    this.formGroup = this.fb.group({
      TypeOfEnergyCode: [this.typeofenergyData.typeOfEnergyCode, Validators.compose([Validators.required, Validators.pattern("[a-zA-Z0-9]{1,15}")])],
      TypeOfEnergyName: [this.typeofenergyData.typeOfEnergyName, Validators.required],
    });
  }

  save() {
    this.prepareTypeOfEnergy();
    if (this.typeofenergyData.typeOfEnergyId) {
      this.edit();
    } else {
      this.typeofenergyData.typeOfEnergyId = "00000000-0000-0000-0000-000000000000"
      this.create();
    }
  }

  edit() {
    const sbUpdate = this.typeofenergyService.update(this.typeofenergyData).pipe(
      tap(() => {
        this.modal.close();
      }),
      catchError((errorMessage) => {
        this.modal.dismiss(errorMessage);
        return of(this.typeofenergyData);
      }),
    ).subscribe((res: any) => {
      Swal.fire({
        icon: res.status == 1 ? 'success' : 'error',
        title: res.status == 1 ? 'Chỉnh sửa thành công' : 'Chỉnh sửa thất bại',
        confirmButtonText: 'Xác nhận',
        text: res.status == 0 ? res.error.msg : 'Chỉnh sửa ' + (res.status == 1 ? 'thành công' : 'thất bại'),
      });
    });
    this.subscriptions.push(sbUpdate);
  }

  create() {
    const sbCreate = this.typeofenergyService.create(this.typeofenergyData).pipe(
      tap(() => {
        this.modal.close();
      }),
      catchError((errorMessage) => {
        this.modal.dismiss(errorMessage);
        return of(this.typeofenergyData);
      }),
    ).subscribe((res: any) => {
      Swal.fire({
        icon: res.status == 1 ? 'success' : 'error',
        title: res.status == 1 ? 'Thêm mới thành công' : 'Thêm mới thất bại',
        confirmButtonText: 'Xác nhận',
        text: res.status == 0 ? res.error.msg : 'Thêm mới ' + (res.status == 1 ? 'thành công' : 'thất bại'),
      });
      this.typeofenergyData = EMPTY_CUSTOM
    });
    this.subscriptions.push(sbCreate);
    EMPTY_CUSTOM.typeOfEnergyId='';
    EMPTY_CUSTOM.typeOfEnergyCode='';
    EMPTY_CUSTOM.typeOfEnergyName='';
    this.typeofenergyData = EMPTY_CUSTOM;
  }

  private prepareTypeOfEnergy() {
    const formData = this.formGroup.value;
    this.typeofenergyData.typeOfEnergyCode = formData.TypeOfEnergyCode;
    this.typeofenergyData.typeOfEnergyName = formData.TypeOfEnergyName;
  }

  ngOnDestroy(): void {
    this.subscriptions.forEach(sb => sb.unsubscribe());
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
