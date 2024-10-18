import { Component, Input, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal, NgbDateAdapter, NgbDateParserFormatter } from '@ng-bootstrap/ng-bootstrap';
import { of, Subscription } from 'rxjs';
import { catchError, finalize, first, tap } from 'rxjs/operators';
import Swal from 'sweetalert2';
import { UnitsModel } from '../../../_models/units.model';
import { UnitsPageService } from '../../../_services/units-page.service';

import { SelectOptionData } from 'src/app/_metronic/shared/components/select-custom/select-custom.interface';
import { Options } from 'select2';

const EMPTY_CUSTOM: UnitsModel = {
  id: '',
  unitId: '00000000-0000-0000-0000-000000000000',
  unitName: '',
  unitNameEn: null,
  unitCode: null,
  exchange: null,
  note: null,
};

@Component({
  selector: 'app-edit-units-modal',
  templateUrl: './edit-units-modal.component.html',
  styleUrls: ['./edit-units-modal.component.scss'],

})
export class EditCustomModalComponent implements OnInit, OnDestroy {
  @Input() id: any;
  isLoading$:any;
  unitData: UnitsModel;
  formGroup: FormGroup;
  private subscriptions: Subscription[] = [];

  constructor(
    private unitsService: UnitsPageService,
    private fb: FormBuilder, public modal: NgbActiveModal
    ) { }

  ngOnInit(): void {
    this.isLoading$ = this.unitsService.isLoading$;
    this.loadCustom();

  }

  loadCustom() {
    if (!this.id) {
      this.clear();
      this.loadForm();
    } else {
      const sb = this.unitsService.getItemById(this.id).pipe(
        first(),
        catchError((errorMessage) => {
          this.modal.dismiss(errorMessage);
          return of(EMPTY_CUSTOM);
        })
      ).subscribe((res: any) => {
        this.unitData = res.items[0];
        this.loadForm();
      });
      this.subscriptions.push(sb);
    }
  }

  loadForm() {
    this.formGroup = this.fb.group({
      UnitName: [this.unitData.unitName, Validators.required],
      UnitNameEn: [this.unitData.unitNameEn],
      UnitCode: [this.unitData.unitCode],
      Exchange: [this.unitData.exchange],
      Note: [this.unitData.note],
    });
  }

  save() {
    this.prepareCustomer();
    if (this.unitData.unitId != '00000000-0000-0000-0000-000000000000') {
      this.edit();
    } else {
      this.create();
    }
  }

  edit() {
    const sbUpdate = this.unitsService.update(this.unitData).pipe(
      tap(() => {
        this.modal.close();
      }),
      catchError((errorMessage) => {
        this.modal.dismiss(errorMessage);
        return of(this.unitData);
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
    this.clear()
  }

  create() {
    const sbCreate = this.unitsService.create(this.unitData).pipe(
      tap(() => {
        this.modal.close();
      }),
      catchError((errorMessage) => {
        this.modal.dismiss(errorMessage);
        return of(this.unitData);
      }),
    ).subscribe((res: any) => {
      Swal.fire({
        icon: res.status == 1 ? 'success' : 'error',
        title: res.status == 1 ? 'Thêm mới thành công' : 'Thêm mới thất bại',
        confirmButtonText: 'Xác nhận',
        text: res.status == 0 ? res.error.msg : 'Thêm mới ' + (res.status == 1 ? 'thành công' : 'thất bại'),
      });
      this.unitData = EMPTY_CUSTOM
    });
    this.subscriptions.push(sbCreate);
    this.clear()
  }

  clear(){
    EMPTY_CUSTOM.unitId = '00000000-0000-0000-0000-000000000000',
    EMPTY_CUSTOM.unitName = '',
    EMPTY_CUSTOM.unitNameEn = null,
    EMPTY_CUSTOM.unitCode = null,
    EMPTY_CUSTOM.exchange = null,
    EMPTY_CUSTOM.note = null,
    this.unitData = EMPTY_CUSTOM
  }

  private prepareCustomer() {
    const formData = this.formGroup.value;
    this.unitData.unitName = formData.UnitName;
    this.unitData.unitNameEn = formData.UnitNameEn;
    this.unitData.unitCode = formData.UnitCode;
    this.unitData.exchange = formData.Exchange;
    this.unitData.note = formData.Note;
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
