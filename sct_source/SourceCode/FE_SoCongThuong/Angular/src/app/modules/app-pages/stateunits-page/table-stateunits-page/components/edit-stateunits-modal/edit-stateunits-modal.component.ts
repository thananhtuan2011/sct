import { Component, Input, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { of, Subscription } from 'rxjs';
import { catchError, finalize, first, tap } from 'rxjs/operators';
import Swal from 'sweetalert2';

import { StateUnitsModel } from '../../../_models/stateunits.model';
import { StateUnitsPageService } from '../../../_services/stateunits-page.service';
import { Options } from 'select2';

const EMPTY_CUSTOM: StateUnitsModel = {
  id: '',
  stateUnitsId: '00000000-0000-0000-0000-000000000000',
  stateUnitsCode: '',
  stateUnitsName: '',
  parentId: '00000000-0000-0000-0000-000000000000',
};

@Component({
  selector: 'app-edit-stateunits-modal',
  templateUrl: './edit-stateunits-modal.component.html',
  styleUrls: ['./edit-stateunits-modal.component.scss'],

})
export class EditStateUnitsModalComponent implements OnInit, OnDestroy {
  @Input() id: any;
  isLoading$:any;
  stateunitsData: StateUnitsModel;
  formGroup: FormGroup;

  private subscriptions: Subscription[] = [];
  ParentData: {id: string, text: string}[] = [];
  options: Options;
  
  constructor(
    private stateunitsService: StateUnitsPageService,
    private fb: FormBuilder, public modal: NgbActiveModal,
    ) { }

  ngOnInit(): void {
    this.isLoading$ = this.stateunitsService.isLoading$;
    this.loadParent();
    this.options = {
      theme: 'bootstrap5',
      templateSelection: this.templateSelection,
    };
  }

  public templateSelection = (state: any): JQuery | string => {
    if (!state.id) {
      return state.text;
    }
    return jQuery('<span class="form-select form-select-solid form-select-lg">' + state.text + '</span>');
  }

  loadParent() {
    this.stateunitsService.loadParent().subscribe((res: any) => {
      const data: {id: string, text: string}[] = [
        {id: "00000000-0000-0000-0000-000000000000", text: "-- Chọn/Không có --"}, 
        ...res.items.map((item: any) => ({ id: item.stateUnitsId, text: item.stateUnitsName }))
      ];
      if (this.id) {
        this.ParentData = data.filter(x => x.id !== this.id);
      } else {
        this.ParentData = data;
      }
      this.loadData();
    })
  }

  loadData() {
    if (!this.id) {
      EMPTY_CUSTOM.stateUnitsId='00000000-0000-0000-0000-000000000000';
      EMPTY_CUSTOM.stateUnitsCode='';
      EMPTY_CUSTOM.stateUnitsName='';
      EMPTY_CUSTOM.parentId='00000000-0000-0000-0000-000000000000';
      this.stateunitsData = EMPTY_CUSTOM;
      this.loadForm();
    } else {
      const sb = this.stateunitsService.getItemById(this.id).pipe(
        first(),
        catchError((errorMessage) => {
          this.modal.dismiss(errorMessage);
          return of(EMPTY_CUSTOM);
        })
      ).subscribe((res: any) => {
        this.stateunitsData = res.items[0];
        this.loadForm();
      });
      this.subscriptions.push(sb);
    }
  }

  loadForm() {
    this.formGroup = this.fb.group({
      StateUnitsCode: [this.stateunitsData.stateUnitsCode, Validators.compose([Validators.required, Validators.pattern("[a-zA-Z0-9]{1,15}")])],
      StateUnitsName: [this.stateunitsData.stateUnitsName, Validators.required],
      ParentId: [this.stateunitsData.parentId]
    });
  }

  save() {
    this.prepareStateUnits();
    if (this.id) {
      this.edit();
    } else {
      this.create();
    }
  }

  edit() {
    const sbUpdate = this.stateunitsService.update(this.stateunitsData).pipe(
      tap(() => {
        this.modal.close();
      }),
      catchError((errorMessage) => {
        this.modal.dismiss(errorMessage);
        return of(this.stateunitsData);
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
    const sbCreate = this.stateunitsService.create(this.stateunitsData).pipe(
      tap(() => {
        this.modal.close();
      }),
      catchError((errorMessage) => {
        this.modal.dismiss(errorMessage);
        return of(this.stateunitsData);
      }),
    ).subscribe((res: any) => {
      Swal.fire({
        icon: res.status == 1 ? 'success' : 'error',
        title: res.status == 1 ? 'Thêm mới thành công' : 'Thêm mới thất bại',
        confirmButtonText: 'Xác nhận',
        text: res.status == 0 ? res.error.msg : 'Thêm mới ' + (res.status == 1 ? 'thành công' : 'thất bại'),
      });
      this.stateunitsData = EMPTY_CUSTOM
    });
    this.subscriptions.push(sbCreate);
    EMPTY_CUSTOM.stateUnitsId='';
    EMPTY_CUSTOM.stateUnitsCode='';
    EMPTY_CUSTOM.stateUnitsName='';
    EMPTY_CUSTOM.parentId='00000000-0000-0000-0000-000000000000';
    this.stateunitsData = EMPTY_CUSTOM;
  }

  private prepareStateUnits() {
    const formData = this.formGroup.value;
    this.stateunitsData.stateUnitsCode = formData.StateUnitsCode;
    this.stateunitsData.stateUnitsName = formData.StateUnitsName;
    this.stateunitsData.parentId = formData.ParentId;
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
