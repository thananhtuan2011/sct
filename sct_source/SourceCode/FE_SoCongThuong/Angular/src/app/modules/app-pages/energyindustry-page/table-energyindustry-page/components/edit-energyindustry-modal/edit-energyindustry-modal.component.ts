import { ChangeDetectorRef, Component, Input, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal, NgbDateAdapter, NgbDateParserFormatter } from '@ng-bootstrap/ng-bootstrap';
import { of, Subscription } from 'rxjs';
import { catchError, finalize, first, tap } from 'rxjs/operators';
import Swal from 'sweetalert2';

import { EnergyIndustryModel } from '../../../_models/energyindustry.model';
import { EnergyIndustryPageService } from '../../../_services/energyindustry-page.service';

const EMPTY_CUSTOM: EnergyIndustryModel = {
  id: '',
  energyIndustryId: '',
  energyIndustryCode: '',
  energyIndustryName: '',
};

@Component({
  selector: 'app-edit-energyindustry-modal',
  templateUrl: './edit-energyindustry-modal.component.html',
  styleUrls: ['./edit-energyindustry-modal.component.scss'],

})
export class EditEnergyIndustryModalComponent implements OnInit, OnDestroy {
  @Input() id: any;
  isLoading$:any;
  energyindustryData: EnergyIndustryModel;
  formGroup: FormGroup;

  private subscriptions: Subscription[] = [];
  public default_value = "00000000-0000-0000-0000-000000000000"
  
  constructor(
    private energyindustryService: EnergyIndustryPageService,
    private fb: FormBuilder, public modal: NgbActiveModal,
    ) { }

  ngOnInit(): void {
    this.isLoading$ = this.energyindustryService.isLoading$;
    this.loadEnergyIndustry();
  }

  loadEnergyIndustry() {
    if (!this.id) {
      EMPTY_CUSTOM.energyIndustryCode='';
      EMPTY_CUSTOM.energyIndustryName='';
      this.energyindustryData = EMPTY_CUSTOM;
      this.loadForm();
    } else {
      const sb = this.energyindustryService.getItemById(this.id).pipe(
        first(),
        catchError((errorMessage) => {
          this.modal.dismiss(errorMessage);
          return of(EMPTY_CUSTOM);
        })
      ).subscribe((res: any) => {
        this.energyindustryData = res.items[0];
        this.loadForm();
      });
      this.subscriptions.push(sb);
    }
  }

  loadForm() {
    this.formGroup = this.fb.group({
      EnergyIndustryCode: [this.energyindustryData.energyIndustryCode, Validators.compose([Validators.required, Validators.pattern("[a-zA-Z0-9]{1,15}")])],
      EnergyIndustryName: [this.energyindustryData.energyIndustryName, Validators.required],
    });
  }

  save() {
    this.prepareTypeOfEnergy();
    if (this.energyindustryData.energyIndustryId) {
      this.edit();
    } else {
      this.energyindustryData.energyIndustryId = "00000000-0000-0000-0000-000000000000"
      this.create();
    }
  }

  edit() {
    const sbUpdate = this.energyindustryService.update(this.energyindustryData).pipe(
      tap(() => {
        this.modal.close();
      }),
      catchError((errorMessage) => {
        this.modal.dismiss(errorMessage);
        return of(this.energyindustryData);
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
    const sbCreate = this.energyindustryService.create(this.energyindustryData).pipe(
      tap(() => {
        this.modal.close();
      }),
      catchError((errorMessage) => {
        this.modal.dismiss(errorMessage);
        return of(this.energyindustryData);
      }),
    ).subscribe((res: any) => {
      Swal.fire({
        icon: res.status == 1 ? 'success' : 'error',
        title: res.status == 1 ? 'Thêm mới thành công' : 'Thêm mới thất bại',
        confirmButtonText: 'Xác nhận',
        text: res.status == 0 ? res.error.msg : 'Thêm mới ' + (res.status == 1 ? 'thành công' : 'thất bại'),
      });
      this.energyindustryData = EMPTY_CUSTOM
    });
    this.subscriptions.push(sbCreate);
    EMPTY_CUSTOM.energyIndustryId='';
    EMPTY_CUSTOM.energyIndustryCode='';
    EMPTY_CUSTOM.energyIndustryName='';
    this.energyindustryData = EMPTY_CUSTOM;
  }

  private prepareTypeOfEnergy() {
    const formData = this.formGroup.value;
    this.energyindustryData.energyIndustryCode = formData.EnergyIndustryCode;
    this.energyindustryData.energyIndustryName = formData.EnergyIndustryName;
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
    }
    else {
      this.save()
    }
  }
}
