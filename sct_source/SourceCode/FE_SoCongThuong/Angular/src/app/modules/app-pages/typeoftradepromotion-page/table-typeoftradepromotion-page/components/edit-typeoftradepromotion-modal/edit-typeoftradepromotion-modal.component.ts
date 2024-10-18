import { ChangeDetectorRef, Component, Input, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal, NgbDateAdapter, NgbDateParserFormatter } from '@ng-bootstrap/ng-bootstrap';
import { of, Subscription } from 'rxjs';
import { catchError, finalize, first, tap } from 'rxjs/operators';
import Swal from 'sweetalert2';

import { TypeOfTradePromotionModel } from '../../../_models/typeoftradepromotion.model';
import { TypeOfTradePromotionPageService } from '../../../_services/typeoftradepromotion-page.service';

const EMPTY_CUSTOM: TypeOfTradePromotionModel = {
  id: '',
  typeOfTradePromotionId: '',
  typeOfTradePromotionCode: '',
  typeOfTradePromotionName: '',
};

@Component({
  selector: 'app-edit-typeoftradepromotion-modal',
  templateUrl: './edit-typeoftradepromotion-modal.component.html',
  styleUrls: ['./edit-typeoftradepromotion-modal.component.scss'],
})

export class EditTypeOfTradePromotionModalComponent implements OnInit, OnDestroy {
  @Input() id: any;
  isLoading$:any;
  typeoftradepromotionData: TypeOfTradePromotionModel;
  formGroup: FormGroup;
  private subscriptions: Subscription[] = [];
  public default_value = "00000000-0000-0000-0000-000000000000"
  
  constructor(
    private typeoftradepromotionService: TypeOfTradePromotionPageService,
    private fb: FormBuilder, public modal: NgbActiveModal,
    ) { }

  ngOnInit(): void {
    this.isLoading$ = this.typeoftradepromotionService.isLoading$;
    this.loadTypeOfTradePromotion();
  }

  loadTypeOfTradePromotion() {
    if (!this.id) {
      EMPTY_CUSTOM.typeOfTradePromotionCode='';
      EMPTY_CUSTOM.typeOfTradePromotionName='';
      this.typeoftradepromotionData = EMPTY_CUSTOM;
      this.loadForm();
    } else {
      const sb = this.typeoftradepromotionService.getItemById(this.id).pipe(
        first(),
        catchError((errorMessage) => {
          this.modal.dismiss(errorMessage);
          return of(EMPTY_CUSTOM);
        })
      ).subscribe((res: any) => {
        this.typeoftradepromotionData = res.items[0];
        this.loadForm();
      });
      this.subscriptions.push(sb);
    }
  }

  loadForm() {
    this.formGroup = this.fb.group({
      TypeOfTradePromotionCode: [this.typeoftradepromotionData.typeOfTradePromotionCode, Validators.compose([Validators.required, Validators.pattern("[a-zA-Z0-9]{1,15}")])],
      TypeOfTradePromotionName: [this.typeoftradepromotionData.typeOfTradePromotionName, Validators.required],
    });
  }

  save() {
    this.prepareTypeOfEnergy();
    if (this.typeoftradepromotionData.typeOfTradePromotionId) {
      this.edit();
    } else {
      this.typeoftradepromotionData.typeOfTradePromotionId = "00000000-0000-0000-0000-000000000000"
      this.create();
    }
  }

  edit() {
    const sbUpdate = this.typeoftradepromotionService.update(this.typeoftradepromotionData).pipe(
      tap(() => {
        this.modal.close();
      }),
      catchError((errorMessage) => {
        this.modal.dismiss(errorMessage);
        return of(this.typeoftradepromotionData);
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
    const sbCreate = this.typeoftradepromotionService.create(this.typeoftradepromotionData).pipe(
      tap(() => {
        this.modal.close();
      }),
      catchError((errorMessage) => {
        this.modal.dismiss(errorMessage);
        return of(this.typeoftradepromotionData);
      }),
    ).subscribe((res: any) => {
      Swal.fire({
        icon: res.status == 1 ? 'success' : 'error',
        title: res.status == 1 ? 'Thêm mới thành công' : 'Thêm mới thất bại',
        confirmButtonText: 'Xác nhận',
        text: res.status == 0 ? res.error.msg : 'Thêm mới ' + (res.status == 1 ? 'thành công' : 'thất bại'),
      });
      this.typeoftradepromotionData = EMPTY_CUSTOM
    });
    this.subscriptions.push(sbCreate);
    EMPTY_CUSTOM.typeOfTradePromotionId='';
    EMPTY_CUSTOM.typeOfTradePromotionCode='';
    EMPTY_CUSTOM.typeOfTradePromotionName='';
    this.typeoftradepromotionData = EMPTY_CUSTOM;
  }

  private prepareTypeOfEnergy() {
    const formData = this.formGroup.value;
    this.typeoftradepromotionData.typeOfTradePromotionCode = formData.TypeOfTradePromotionCode;
    this.typeoftradepromotionData.typeOfTradePromotionName = formData.TypeOfTradePromotionName;
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
