import { ChangeDetectorRef, Component, Input, OnDestroy, OnInit } from '@angular/core';
import { AbstractControl, FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal, NgbDateAdapter, NgbDateParserFormatter } from '@ng-bootstrap/ng-bootstrap';
import { of, Subscription } from 'rxjs';
import { catchError, finalize, first, tap } from 'rxjs/operators';
import Swal from 'sweetalert2';
import { IndustryPromotionReportModel } from '../../../_models/industry-promotion-report.model';
import { IndustryPromotionReportPageService } from '../../../_services/industry-promotion-report-page.service';

import { SelectOptionData } from 'src/app/_metronic/shared/components/select-custom/select-custom.interface';
import { Options } from 'select2';
import { data } from 'jquery';

const EMPTY_CUSTOM: IndustryPromotionReportModel = {
  id: '',
  rpIndustrialPromotionFundingId: '',
  yearReport: null,
  nationalReport: null,
  localReport: null,
  targets: '',
  unit: '',
};

@Component({
  selector: 'app-edit-modal',
  templateUrl: './edit-modal.component.html',
  styleUrls: ['./edit-modal.component.scss'],

})
export class EditIndustryPromotionReportModalComponent implements OnInit, OnDestroy {
  @Input() id: any;
  isLoading$: any;
  industryPromotionReportData: IndustryPromotionReportModel;
  formGroup: FormGroup;


  private subscriptions: Subscription[] = [];

  public selectData: any = [];
  public options: Options;
  show: boolean = false;


  constructor(
    private industryPromotionReportService: IndustryPromotionReportPageService,
    private fb: FormBuilder, public modal: NgbActiveModal,
  ) { }


  ngOnInit(): void {
    this.isLoading$ = this.industryPromotionReportService.isLoading$;
    this.loadTargets()
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

  delay(ms: number) {
    return new Promise(resolve => setTimeout(resolve, ms));
  }

  loadTargets() {
    this.industryPromotionReportService.loadTargets().subscribe(res => {
      const data = [
        {
          id: "00000000-0000-0000-0000-000000000000",
          text: '-- Chọn --',
          piority: 0
        }
      ]
      for (var targets of res.items) {
        let obj = {
          id: targets.categoryId,
          text: targets.categoryName,
          piority: targets.piority
        }
        data.push(obj)
      }
      this.selectData = data.sort((i1, i2) => {
        if (i1.piority > i2.piority) {
          return 1;
        }
        if (i1.piority < i2.piority) {
          return -1;
        }
        return 0;
      });
      this.loadIndustryPromotionReport();
    }
    );
  }

  loadIndustryPromotionReport() {
    if (!this.id) {
      this.clear();
      this.loadForm();
    } else {
      const sb = this.industryPromotionReportService.getItemById(this.id).pipe(
        first(),
        catchError((errorMessage) => {
          this.modal.dismiss(errorMessage);
          return of(EMPTY_CUSTOM);
        })
      ).subscribe((res: any) => {
        this.industryPromotionReportData = res.data;
        this.loadForm();
        this.formGroup.controls.NationalReport.patchValue(this.f_currency(res.data.nationalReport), {emitEvent: false})
        this.formGroup.controls.LocalReport.patchValue(this.f_currency(res.data.localReport), {emitEvent: false})
      });
      this.subscriptions.push(sb);
    }
  }

  loadForm() {
    this.formGroup = this.fb.group({
      YearReport: [this.industryPromotionReportData.yearReport, Validators.required],
      NationalReport: [this.industryPromotionReportData.nationalReport, Validators.required],
      LocalReport: [this.industryPromotionReportData.localReport, Validators.required],
      Targets: [this.industryPromotionReportData.targets],
      Unit: [this.industryPromotionReportData.unit],
    });
    this.subscriptions.push(this.formGroup.controls.NationalReport.valueChanges.subscribe((x: any) => {
      this.formGroup.controls.NationalReport.patchValue(this.f_currency(x), {emitEvent: false})
    }));
    this.subscriptions.push(this.formGroup.controls.LocalReport.valueChanges.subscribe((x: any) => {
      this.formGroup.controls.LocalReport.patchValue(this.f_currency(x), {emitEvent: false})
    }));
    this.show = true;
  }

  save() {
    this.prepareCommune();
    if (this.id) {
      this.edit();
    } else {
      this.create();
    }
  }

  prenventInputNonNumber(event: any) {
    if (event.which < 48 || event.which > 57) {
      event.preventDefault();
    }
  }

  f_currency(value: any, args?: any): any {
    let nbr = Number((value + '').replace(/,|-/g, ''));
    return (nbr + '').replace(/(\d)(?=(\d{3})+(?!\d))/g, '$1,');
  }

  edit() {
    const sbUpdate = this.industryPromotionReportService.update(this.industryPromotionReportData).pipe(
      tap(() => {
        this.modal.close();
      }),
      catchError((errorMessage) => {
        this.modal.dismiss(errorMessage);
        return of(this.industryPromotionReportData);
      }),
    ).subscribe((res: any) => {
      Swal.fire({
        icon: res.status == 1 ? 'success' : 'error',
        title: res.status == 1 ? 'Chỉnh sửa thành công' : 'Chỉnh sửa thất bại',
        confirmButtonText: 'Xác nhận',
        text: res.status == 1 ? 'Chỉnh sửa thành công' : res.status == 0 ? res.error.msg : 'Chỉnh sửa thất bại',
      });
    });
    this.subscriptions.push(sbUpdate);
  }

  create() {
    const sbCreate = this.industryPromotionReportService.create(this.industryPromotionReportData).pipe(
      tap(() => {
        this.modal.close();
      }),
      catchError((errorMessage) => {
        this.modal.dismiss(errorMessage);
        return of(this.industryPromotionReportData);
      }),
    ).subscribe((res: any) => {
      Swal.fire({
        icon: res.status == 1 ? 'success' : 'error',
        title: res.status == 1 ? 'Thêm mới thành công' : 'Thêm mới thất bại',
        confirmButtonText: 'Xác nhận',
        text: res.status == 1 ? 'Thêm mới thành công' : res.status == 0 ? res.error.msg : 'Thêm mới thất bại',
      });
      this.industryPromotionReportData = EMPTY_CUSTOM
    });
    this.subscriptions.push(sbCreate);
  }

  private prepareCommune() {
    const formData = this.formGroup.value;
    this.industryPromotionReportData.yearReport = Number(formData.YearReport);
    this.industryPromotionReportData.nationalReport = Number(formData.NationalReport.replaceAll(',' , ''));
    this.industryPromotionReportData.localReport = Number(formData.LocalReport.replaceAll(',' , ''));
    this.industryPromotionReportData.targets = formData.Targets;
    this.industryPromotionReportData.unit = formData.Unit;
  }

  ngOnDestroy(): void {
    this.subscriptions.forEach(sb => sb.unsubscribe());
  }

  clear() {
    EMPTY_CUSTOM.rpIndustrialPromotionFundingId = '00000000-0000-0000-0000-000000000000',
    EMPTY_CUSTOM.yearReport = null,
    EMPTY_CUSTOM.nationalReport = null,
    EMPTY_CUSTOM.localReport = null,
    EMPTY_CUSTOM.unit = '',
    EMPTY_CUSTOM.targets = '00000000-0000-0000-0000-000000000000',
    this.industryPromotionReportData = EMPTY_CUSTOM;
  }

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

  isDefaultValue(controlName: any): boolean {
    const control = this.formGroup.controls[controlName];
    const value = control.value;
    const isdefaultvalue = (value == "00000000-0000-0000-0000-000000000000")
    if (isdefaultvalue) {
      control.setErrors({
        default_value: true
      })
    }
    return control.invalid && (control.dirty || control.touched);
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
