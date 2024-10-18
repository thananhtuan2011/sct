import { ChangeDetectorRef, Component, Input, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal, NgbDateAdapter, NgbDateParserFormatter } from '@ng-bootstrap/ng-bootstrap';
import { of, Subscription } from 'rxjs';
import { catchError, finalize, first, tap } from 'rxjs/operators';
import { SelectOptionData } from 'src/app/_metronic/shared/components/select-custom/select-custom.interface';
import Swal from 'sweetalert2';
import { Options } from 'select2';
import { ConsumerServiceRevenueDetailModel } from '../../../_models/consumer-service-revenue.model';
import { ConsumerServiceRevenuePageService } from '../../../_services/consumer-service-revenue.service';

const EMPTY_CUSTOM: ConsumerServiceRevenueDetailModel = {
  id: '',
  criteria: '00000000-0000-0000-0000-000000000000',
  ConsumerServiceRevenueId: '',
  criteriaName: '',
  performLastmonth: 0,
  estimateReportingMonth: 0,
  cumulativeToReportingMonth: 0,
  performReporting: 0,
  type: 0,
  isDel: false,
};

@Component({
  selector: 'app-edit-criteria-modal.component',
  templateUrl: './edit-modal.component.html',
  styleUrls: ['./edit-modal.component.scss'],
})

export class EditCriteriaModalComponent implements OnInit, OnDestroy {
  isLoading$: any;
  editData: ConsumerServiceRevenueDetailModel;
  formGroup: FormGroup;
  options: Options;
  dataSource: any[] = [];
  lstStore: any[] = [];
  show: boolean = false;

  private subscriptions: Subscription[] = [];
  public dataCategory: Array<SelectOptionData>;

  constructor(
    private pageService: ConsumerServiceRevenuePageService,
    private fb: FormBuilder, 
    public modal: NgbActiveModal,
  ) { }

  ngOnInit(): void {
    this.isLoading$ = this.pageService.isLoading$;
    this.loadUser();
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

  loadDetail() {
    this.clear_model();
    this.loadForm();
  }

  loadForm() {
    this.formGroup = this.fb.group({
      criteria: [this.editData.criteria, Validators.compose([Validators.required])],
      performLastmonth: [this.editData.performLastmonth],
      estimateReportingMonth: [this.editData.estimateReportingMonth],
      cumulativeToReportingMonth: [this.editData.cumulativeToReportingMonth],
    });

    this.show = true;
  }

  clear_model() {
    EMPTY_CUSTOM.ConsumerServiceRevenueId = '',
    EMPTY_CUSTOM.criteria = '00000000-0000-0000-0000-000000000000',
    EMPTY_CUSTOM.performLastmonth = 0,
    EMPTY_CUSTOM.cumulativeToReportingMonth = 0,
    EMPTY_CUSTOM.estimateReportingMonth = 0,
    EMPTY_CUSTOM.performReporting = 0,
    this.editData = EMPTY_CUSTOM
  }

  save() {
    const formData = this.formGroup.value;
    this.editData.criteria = formData.criteria;
    this.editData.performLastmonth = formData.performLastmonth;
    this.editData.estimateReportingMonth = formData.estimateReportingMonth;
    this.editData.cumulativeToReportingMonth = formData.cumulativeToReportingMonth;
    this.editData.performReporting = formData.performReporting;
    this.editData.type = 1;

    this.dataCategory.forEach(x => {
      if (x.id == this.editData.criteria) {
        this.editData.criteriaName = x.text;
      }
    });

    this.modal.close(this.editData);
  }

  loadUser() {
    this.pageService.loadcategory().subscribe(res => {
      const data = [{
        id: "00000000-0000-0000-0000-000000000000",
        text: '-- Chọn --'
      }]
      for (var user of res.items) {
        let obj_business = {
          id: user.categoryId,
          text: user.categoryName,
        }
        data.push(obj_business);
      }
      this.dataCategory = data;
      this.loadDetail();
    })
  }

  //Chuyển output của ngbDate thành dạng Date
  convertNgbDateToDate(dateToConvert: any) {
    return new Date(Date.UTC(dateToConvert.year, dateToConvert.month - 1, dateToConvert.day))
  }

  //Chuyển Date thành định dạng của ngbDate
  converDateToNbgDate(dateToConvert: any) {
    const data = new Date(dateToConvert)
    let NgbDate = {
      year: data.getFullYear(),
      month: data.getMonth() + 1,
      day: data.getDate(),
    }
    return NgbDate
  }

  prenventInputNonNumber(event: any) {
    if (event.which < 48 || event.which > 57) {
      event.preventDefault();
    }
  }

  isDefaultValue(controlName: any)//: boolean 
  {
    const control = this.formGroup.controls[controlName];
    const isdefaultvalue = (control.value == "00000000-0000-0000-0000-000000000000")
    if (isdefaultvalue) {
      control.setErrors({ default: true })
    }
    return control.invalid && (control.dirty || control.touched)
  }

  ngOnDestroy(): void {
    this.subscriptions.forEach(sb => sb.unsubscribe());
  }

  // helpers for View
  isControlValid(controlName: any): boolean {
    const control = this.formGroup.controls[controlName];
    return control.valid && (control.dirty || control.touched);
  }

  isControlInvalid(controlName: any): boolean {
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
