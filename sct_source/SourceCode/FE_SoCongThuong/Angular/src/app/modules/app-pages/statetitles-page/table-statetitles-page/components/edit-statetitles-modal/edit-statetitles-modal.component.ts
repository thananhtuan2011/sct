import { ChangeDetectorRef, Component, Input, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal, NgbDateAdapter, NgbDateParserFormatter } from '@ng-bootstrap/ng-bootstrap';
import { of, Subscription } from 'rxjs';
import { catchError, finalize, first, tap } from 'rxjs/operators';
import Swal from 'sweetalert2';

import { StateTitlesModel } from '../../../_models/statetitles.model';
import { StateTitlesPageService } from '../../../_services/statetitles-page.service';

const EMPTY_CUSTOM: StateTitlesModel = {
  id: '',
  stateTitlesId: '',
  stateTitlesCode: '',
  stateTitlesName: '',
  piority: 1,
};

@Component({
  selector: 'app-edit-statetitles-modal',
  templateUrl: './edit-statetitles-modal.component.html',
  styleUrls: ['./edit-statetitles-modal.component.scss'],

})
export class EditStateTitlesModalComponent implements OnInit, OnDestroy {
  @Input() id: any;
  isLoading$:any;
  statetitlesData: StateTitlesModel;
  formGroup: FormGroup;

  private subscriptions: Subscription[] = [];
  public default_value = "00000000-0000-0000-0000-000000000000"
  
  constructor(
    private statetitlesService: StateTitlesPageService,
    private fb: FormBuilder, public modal: NgbActiveModal,
    ) { }

  ngOnInit(): void {
    this.isLoading$ = this.statetitlesService.isLoading$;
    this.loadMarket();
  }

  loadMarket() {
    if (!this.id) {
      EMPTY_CUSTOM.stateTitlesCode='';
      EMPTY_CUSTOM.stateTitlesName='';
      EMPTY_CUSTOM.piority=0;
      this.statetitlesData = EMPTY_CUSTOM;
      this.loadForm();
    } else {
      const sb = this.statetitlesService.getItemById(this.id).pipe(
        first(),
        catchError((errorMessage) => {
          this.modal.dismiss(errorMessage);
          return of(EMPTY_CUSTOM);
        })
      ).subscribe((res: any) => {
        this.statetitlesData = res.items[0];
        this.loadForm();
      });
      this.subscriptions.push(sb);
    }
  }

  loadForm() {
    this.formGroup = this.fb.group({
      StateTitlesCode: [this.statetitlesData.stateTitlesCode, Validators.compose([Validators.required, Validators.pattern("[a-zA-Z0-9]{1,15}")])],
      StateTitlesName: [this.statetitlesData.stateTitlesName, Validators.required],
      Piority: [this.statetitlesData.piority, Validators.required]
    });
  }

  save() {
    this.prepareStateTitles();
    if (this.statetitlesData.stateTitlesId) {
      this.edit();
    } else {
      this.statetitlesData.stateTitlesId = this.default_value
      this.create();
    }
  }

  edit() {
    const sbUpdate = this.statetitlesService.update(this.statetitlesData).pipe(
      tap(() => {
        this.modal.close();
      }),
      catchError((errorMessage) => {
        this.modal.dismiss(errorMessage);
        return of(this.statetitlesData);
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
    const sbCreate = this.statetitlesService.create(this.statetitlesData).pipe(
      tap(() => {
        this.modal.close();
      }),
      catchError((errorMessage) => {
        this.modal.dismiss(errorMessage);
        return of(this.statetitlesData);
      }),
    ).subscribe((res: any) => {
      Swal.fire({
        icon: res.status == 1 ? 'success' : 'error',
        title: res.status == 1 ? 'Thêm mới thành công' : 'Thêm mới thất bại',
        confirmButtonText: 'Xác nhận',
        text: res.status == 0 ? res.error.msg : 'Thêm mới ' + (res.status == 1 ? 'thành công' : 'thất bại'),
      });
      this.statetitlesData = EMPTY_CUSTOM
    });
    this.subscriptions.push(sbCreate);
    EMPTY_CUSTOM.stateTitlesId='';
    EMPTY_CUSTOM.stateTitlesCode='';
    EMPTY_CUSTOM.stateTitlesName='';
    EMPTY_CUSTOM.piority=0;
    this.statetitlesData = EMPTY_CUSTOM;
  }

  private prepareStateTitles() {
    const formData = this.formGroup.value;
    this.statetitlesData.stateTitlesCode = formData.StateTitlesCode;
    this.statetitlesData.stateTitlesName = formData.StateTitlesName;
    this.statetitlesData.piority = formData.Piority;
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
