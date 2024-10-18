import { ChangeDetectorRef, Component, Input, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal, NgbDateAdapter, NgbDateParserFormatter } from '@ng-bootstrap/ng-bootstrap';
import { of, Subscription } from 'rxjs';
import { catchError, finalize, first, tap } from 'rxjs/operators';
import Swal from 'sweetalert2';

import { BusinessLineModel } from '../../../_models/business-line.model';
import { BusinessLinePageService } from '../../../_services/business-line-page.service';

const EMPTY_CUSTOM: BusinessLineModel = {
  id: '',
  businessLineId: '',
  businessLineCode: '',
  businessLineName: '',
};

@Component({
  selector: 'app-edit-businessline-modal',
  templateUrl: './edit-modal.component.html',
  styleUrls: ['./edit-modal.component.scss'],

})
export class EditBusinessLineModalComponent implements OnInit, OnDestroy {
  @Input() id: any;
  isLoading$:any;
  businessLineData: BusinessLineModel;
  formGroup: FormGroup;

  private subscriptions: Subscription[] = [];
  public default_value = "00000000-0000-0000-0000-000000000000"
  
  constructor(
    private businessLineService: BusinessLinePageService,
    private fb: FormBuilder, public modal: NgbActiveModal,
    ) { }

  ngOnInit(): void {
    this.isLoading$ = this.businessLineService.isLoading$;
    this.loadBusinessLine();
  }

  loadBusinessLine() {
    if (!this.id) {
      EMPTY_CUSTOM.businessLineCode='';
      EMPTY_CUSTOM.businessLineName='';
      this.businessLineData = EMPTY_CUSTOM;
      this.loadForm();
    } else {
      const sb = this.businessLineService.getItemById(this.id).pipe(
        first(),
        catchError((errorMessage) => {
          this.modal.dismiss(errorMessage);
          return of(EMPTY_CUSTOM);
        })
      ).subscribe((res: any) => {
        this.businessLineData = res.items[0];
        this.loadForm();
      });
      this.subscriptions.push(sb);
    }
  }

  loadForm() {
    this.formGroup = this.fb.group({
      BusinessLineCode: [this.businessLineData.businessLineCode, Validators.compose([Validators.required, Validators.pattern("[a-zA-Z0-9]{1,15}")])],
      BusinessLineName: [this.businessLineData.businessLineName, Validators.required],
    });
  }

  save() {
    this.prepareBusinessLine();
    if (this.businessLineData.businessLineId) {
      this.edit();
    } else {
      this.businessLineData.businessLineId = "00000000-0000-0000-0000-000000000000"
      this.create();
    }
  }

  edit() {
    const sbUpdate = this.businessLineService.update(this.businessLineData).pipe(
      tap(() => {
        this.modal.close();
      }),
      catchError((errorMessage) => {
        this.modal.dismiss(errorMessage);
        return of(this.businessLineData);
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
    const sbCreate = this.businessLineService.create(this.businessLineData).pipe(
      tap(() => {
        this.modal.close();
      }),
      catchError((errorMessage) => {
        this.modal.dismiss(errorMessage);
        return of(this.businessLineData);
      }),
    ).subscribe((res: any) => {
      Swal.fire({
        icon: res.status == 1 ? 'success' : 'error',
        title: res.status == 1 ? 'Thêm mới thành công' : 'Thêm mới thất bại',
        confirmButtonText: 'Xác nhận',
        text: res.status == 0 ? res.error.msg : 'Thêm mới ' + (res.status == 1 ? 'thành công' : 'thất bại'),
      });
      this.businessLineData = EMPTY_CUSTOM
    });
    this.subscriptions.push(sbCreate);
    EMPTY_CUSTOM.businessLineId='';
    EMPTY_CUSTOM.businessLineCode='';
    EMPTY_CUSTOM.businessLineName='';
    this.businessLineData = EMPTY_CUSTOM;
  }

  private prepareBusinessLine() {
    const formData = this.formGroup.value;
    this.businessLineData.businessLineCode = formData.BusinessLineCode;
    this.businessLineData.businessLineName = formData.BusinessLineName;
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
