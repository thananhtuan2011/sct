import { ChangeDetectorRef, Component, Input, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal, NgbDateAdapter, NgbDateParserFormatter } from '@ng-bootstrap/ng-bootstrap';
import { of, Subscription } from 'rxjs';
import { catchError, finalize, first, tap } from 'rxjs/operators';
import Swal from 'sweetalert2';

import { TradePromotionProjectModel } from '../../../_models/tradepromotionproject.model';
import { TradePromotionProjectPageService } from '../../../_services/tradepromotionproject-page.service';

const EMPTY_CUSTOM: TradePromotionProjectModel = {
  id: '',
  tradePromotionProjectId: '',
  tradePromotionProjectCode: '',
  tradePromotionProjectName: '',
};

@Component({
  selector: 'app-edit-tradepromotionproject-modal',
  templateUrl: './edit-tradepromotionproject-modal.component.html',
  styleUrls: ['./edit-tradepromotionproject-modal.component.scss'],

})
export class EditTradePromotionProjectModalComponent implements OnInit, OnDestroy {
  @Input() id: any;
  isLoading$:any;
  tradepromotionprojectData: TradePromotionProjectModel;
  formGroup: FormGroup;

  private subscriptions: Subscription[] = [];
  public default_value = "00000000-0000-0000-0000-000000000000"
  
  constructor(
    private tradepromotionprojectService: TradePromotionProjectPageService,
    private fb: FormBuilder, public modal: NgbActiveModal,
    ) { }

  ngOnInit(): void {
    this.isLoading$ = this.tradepromotionprojectService.isLoading$;
    this.loadTradePromotionProject();
  }

  loadTradePromotionProject() {
    if (!this.id) {
      EMPTY_CUSTOM.tradePromotionProjectCode='';
      EMPTY_CUSTOM.tradePromotionProjectName='';
      this.tradepromotionprojectData = EMPTY_CUSTOM;
      this.loadForm();
    } else {
      const sb = this.tradepromotionprojectService.getItemById(this.id).pipe(
        first(),
        catchError((errorMessage) => {
          this.modal.dismiss(errorMessage);
          return of(EMPTY_CUSTOM);
        })
      ).subscribe((res: any) => {
        this.tradepromotionprojectData = res.items[0];
        this.loadForm();
      });
      this.subscriptions.push(sb);
    }
  }

  loadForm() {
    this.formGroup = this.fb.group({
      TradePromotionProjectCode: [this.tradepromotionprojectData.tradePromotionProjectCode, Validators.compose([Validators.required, Validators.pattern("[a-zA-Z0-9]{1,15}")])],
      TradePromotionProjectName: [this.tradepromotionprojectData.tradePromotionProjectName, Validators.required],
    });
  }

  save() {
    this.prepareTypeOfEnergy();
    if (this.tradepromotionprojectData.tradePromotionProjectId) {
      this.edit();
    } else {
      this.tradepromotionprojectData.tradePromotionProjectId = "00000000-0000-0000-0000-000000000000"
      this.create();
    }
  }

  edit() {
    const sbUpdate = this.tradepromotionprojectService.update(this.tradepromotionprojectData).pipe(
      tap(() => {
        this.modal.close();
      }),
      catchError((errorMessage) => {
        this.modal.dismiss(errorMessage);
        return of(this.tradepromotionprojectData);
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
    const sbCreate = this.tradepromotionprojectService.create(this.tradepromotionprojectData).pipe(
      tap(() => {
        this.modal.close();
      }),
      catchError((errorMessage) => {
        this.modal.dismiss(errorMessage);
        return of(this.tradepromotionprojectData);
      }),
    ).subscribe((res: any) => {
      Swal.fire({
        icon: res.status == 1 ? 'success' : 'error',
        title: res.status == 1 ? 'Thêm mới thành công' : 'Thêm mới thất bại',
        confirmButtonText: 'Xác nhận',
        text: res.status == 0 ? res.error.msg : 'Thêm mới ' + (res.status == 1 ? 'thành công' : 'thất bại'),
      });
      this.tradepromotionprojectData = EMPTY_CUSTOM
    });
    this.subscriptions.push(sbCreate);
    EMPTY_CUSTOM.tradePromotionProjectId='';
    EMPTY_CUSTOM.tradePromotionProjectCode='';
    EMPTY_CUSTOM.tradePromotionProjectName='';
    this.tradepromotionprojectData = EMPTY_CUSTOM;
  }

  private prepareTypeOfEnergy() {
    const formData = this.formGroup.value;
    this.tradepromotionprojectData.tradePromotionProjectCode = formData.TradePromotionProjectCode;
    this.tradepromotionprojectData.tradePromotionProjectName = formData.TradePromotionProjectName;
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
