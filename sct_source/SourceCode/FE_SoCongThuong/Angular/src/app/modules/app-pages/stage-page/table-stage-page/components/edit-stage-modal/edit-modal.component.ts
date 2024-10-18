import { ChangeDetectorRef, Component, Input, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal, NgbDateAdapter, NgbDateParserFormatter } from '@ng-bootstrap/ng-bootstrap';
import { of, Subscription } from 'rxjs';
import { catchError, finalize, first, tap } from 'rxjs/operators';
import Swal from 'sweetalert2';

import { StageModel } from '../../../_models/stage.model';
import { StagePageService } from '../../../_services/stage-page.service';

const EMPTY_CUSTOM: StageModel = {
  id: '',
  stageId: '00000000-0000-0000-0000-000000000000',
  stageName: '',
  startYear: null,
  endYear: null,
};

@Component({
  selector: 'app-edit-modal',
  templateUrl: './edit-modal.component.html',
  styleUrls: ['./edit-modal.component.scss'],

})
export class EditStageModalComponent implements OnInit, OnDestroy {
  @Input() id: any;
  isLoading$:any;
  stageData: StageModel;
  formGroup: FormGroup;

  private subscriptions: Subscription[] = [];
  
  constructor(
    private stageService: StagePageService,
    private fb: FormBuilder, public modal: NgbActiveModal,
    ) { }

  ngOnInit(): void {
    this.isLoading$ = this.stageService.isLoading$;
    this.loadStage();
  }

  loadStage() {
    if (!this.id) {
      this.clear();
      this.loadForm();
    } else {
      const sb = this.stageService.getItemById(this.id).pipe(
        first(),
        catchError((errorMessage) => {
          this.modal.dismiss(errorMessage);
          return of(EMPTY_CUSTOM);
        })
      ).subscribe((res: any) => {
        this.stageData = res.items[0];
        this.loadForm();
      });
      this.subscriptions.push(sb);
    }
  }

  loadForm() {
    this.formGroup = this.fb.group({
      StageName: [this.stageData.stageName],
      StartYear: [this.stageData.startYear, Validators.required],
      EndYear: [this.stageData.endYear, Validators.required]
    });
    this.formGroup.controls.StartYear.valueChanges.subscribe(() => {
      this.formGroup.patchValue({
        'StageName' : (this.formGroup.controls.StartYear.value ?? '') + ' - ' + (this.formGroup.controls.EndYear.value ?? '')
      }, {emitEvent : false})
    })
    this.formGroup.controls.EndYear.valueChanges.subscribe(() => {
      this.formGroup.patchValue({
        'StageName' : (this.formGroup.controls.StartYear.value ?? '') + ' - ' + (this.formGroup.controls.EndYear.value ?? '')
      }, {emitEvent : false})
    })
  }

  clear() {
    EMPTY_CUSTOM.stageId = "00000000-0000-0000-0000-000000000000",
    EMPTY_CUSTOM.stageName = '',
    EMPTY_CUSTOM.startYear = null,
    EMPTY_CUSTOM.endYear = null,
    this.stageData = EMPTY_CUSTOM;
  }

  save() {
    this.prepareStage();
    if (this.id) {
      this.edit();
    } else {
      this.create();
    }
  }

  edit() {
    const sbUpdate = this.stageService.update(this.stageData).pipe(
      tap(() => {
        this.modal.close();
      }),
      catchError((errorMessage) => {
        this.modal.dismiss(errorMessage);
        return of(this.stageData);
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
    const sbCreate = this.stageService.create(this.stageData).pipe(
      tap(() => {
        this.modal.close();
      }),
      catchError((errorMessage) => {
        this.modal.dismiss(errorMessage);
        return of(this.stageData);
      }),
    ).subscribe((res: any) => {
      Swal.fire({
        icon: res.status == 1 ? 'success' : 'error',
        title: res.status == 1 ? 'Thêm mới thành công' : 'Thêm mới thất bại',
        confirmButtonText: 'Xác nhận',
        text: res.status == 0 ? res.error.msg : 'Thêm mới ' + (res.status == 1 ? 'thành công' : 'thất bại'),
      });
      this.stageData = EMPTY_CUSTOM
    });
    this.subscriptions.push(sbCreate);
  }

  private prepareStage() {
    const formData = this.formGroup.value;
    this.stageData.stageName = formData.StageName;
    this.stageData.startYear = formData.StartYear;
    this.stageData.endYear = formData.EndYear;
  }

  prenventInputNonNumber(event: any) {
    if (event.which < 48 || event.which > 57) {
      event.preventDefault();
    }
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
      this.formGroup.updateValueAndValidity();
    } else if (this.formGroup.controls.StartYear.value >= this.formGroup.controls.EndYear.value) {
      this.formGroup.controls.StartYear.setErrors({'start': true});
      this.formGroup.updateValueAndValidity();
    }
    else {
      this.save()
    }
  }
}
