import { ChangeDetectorRef, Component, Input, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal, NgbDateAdapter, NgbDateParserFormatter } from '@ng-bootstrap/ng-bootstrap';
import { of, Subscription } from 'rxjs';
import { catchError, finalize, first, tap } from 'rxjs/operators';
import { SelectOptionData } from 'src/app/_metronic/shared/components/select-custom/select-custom.interface';
import Swal from 'sweetalert2';
import { Options } from 'select2';
import { CateRetailDetailModel, CateRetailModel } from '../../../_models/cate-retail.model';
import { CateRetailPageService } from '../../../_services/cate-retail.service';

const EMPTY_CUSTOM: CateRetailDetailModel = {
  id: '',
  criteria: '00000000-0000-0000-0000-000000000000',
  CateRetailId: '',
  criteriaName: '',
  performLastmonth: 0,
  estimateReportingMonth: 0,
  cumulativeToReportingMonth: 0,
  performReporting: 0,
  type: 0,
  isDel: false,
};

@Component({
  selector: 'app-edit-cate-retail-detail1-modal.component',
  templateUrl: './edit-cate-retail-detail1-modal.component.html',
  styleUrls: ['./edit-cate-retail-detail1-modal.component.scss'],
})

export class EditCateRetailDetail1ModalComponent implements OnInit, OnDestroy {
  isLoading$: any;
  CateRetailDetailModel: CateRetailDetailModel;
  formGroup: FormGroup;
  options: Options;
  dataSource: any[] = [];
  lstStore: any[] = [];
  show: boolean = false;

  private subscriptions: Subscription[] = [];
  public dataCategory: Array<SelectOptionData>;

  constructor(
    private pageService: CateRetailPageService,
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
      criteria: [this.CateRetailDetailModel.criteria, Validators.compose([Validators.required])],
      performLastmonth: [this.CateRetailDetailModel.performLastmonth],
      estimateReportingMonth: [this.CateRetailDetailModel.estimateReportingMonth],
      cumulativeToReportingMonth: [this.CateRetailDetailModel.cumulativeToReportingMonth],
    });

    this.show = true;
  }

  clear_model() {
    EMPTY_CUSTOM.CateRetailId = '',
    EMPTY_CUSTOM.criteria = '00000000-0000-0000-0000-000000000000',
    EMPTY_CUSTOM.performLastmonth = 0,
    EMPTY_CUSTOM.cumulativeToReportingMonth = 0,
    EMPTY_CUSTOM.estimateReportingMonth = 0,
    EMPTY_CUSTOM.performReporting = 0,
    this.CateRetailDetailModel = EMPTY_CUSTOM
  }

  save() {
    const formData = this.formGroup.value;
    this.CateRetailDetailModel.criteria = formData.criteria;
    this.CateRetailDetailModel.performLastmonth = formData.performLastmonth;
    this.CateRetailDetailModel.estimateReportingMonth = formData.estimateReportingMonth;
    this.CateRetailDetailModel.cumulativeToReportingMonth = formData.cumulativeToReportingMonth;
    this.CateRetailDetailModel.performReporting = formData.performReporting;
    this.CateRetailDetailModel.type = 1;

    this.dataCategory.forEach(x => {
      if (x.id == this.CateRetailDetailModel.criteria) {
        this.CateRetailDetailModel.criteriaName = x.text;
      }
    });

    this.modal.close(this.CateRetailDetailModel);
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
