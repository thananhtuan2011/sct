import { ChangeDetectorRef, Component, Input, OnChanges, OnDestroy, OnInit, SimpleChanges } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal, NgbDateAdapter, NgbDateParserFormatter } from '@ng-bootstrap/ng-bootstrap';
import { of, Subscription } from 'rxjs';
import { catchError, finalize, first, tap } from 'rxjs/operators';
import Swal from 'sweetalert2';

import { FinancialPlanTargetsModel } from '../../../_models/financial-plan-targets.model';
import { FinancialPlanTargetsPageService } from '../../../_services/financial-plan-targets-page.service';
import { Options } from 'select2';
import * as moment from 'moment';

const EMPTY_CUSTOM: FinancialPlanTargetsModel = {
  id: '',
  financialPlanTargetsId: '00000000-0000-0000-0000-000000000000',
  //Tên
  name: '',
  /** Phân loại - Type
   * 1: Giá trị sản xuất
   * 2: Sản phẩm chủ yếu
   * Xuất khẩu
   * 3: Tổng kim ngạch xuất khẩu
   * 4: Phân theo khối doanh nghiệp
   * 5: Phân theo nhóm hàng
   * 6: Mặt hàng xuất khẩu chủ yếu
   * 7: Thị trường xuất khẩu
   * Nhập khẩu:
   * 8: Tổng kim ngạch nhập khẩu
   * 9: Mặt hàng nhập khẩu chủ yếu
   * 10: Tổng MBLHH-DVXH
   */
  type: 0,
  //Đơn vị
  unit: '',
  //Năm tháng báo cáo "YYYY-MM"
  date: moment().format('YYYY-MM'),
  // (1) Kế hoạch năm
  plan: 0,
  // (2) Thực hiện cùng tháng năm trước
  // valueSameMonthLastYear: 0,
  // (3) Thực hiện tháng trước
  // valueLastMonth: 0,
  // (4) Ước tính tháng thực hiện
  estimatedMonth: 0,
  // (5) Cộng dồn đến tháng
  cumulativeToMonth: 0,
  // (6) Cộng dồn đến tháng năm trước
  // cumulativeToMonthLastYear: 0,
  // (7)=(4)/(3) So sánh tháng trước: = estimatedMonth / valueLastMonth
  // compareLastMonth: 0,
  // (8)=(4)/(2) So sánh tháng cùng kỳ = estimatedMonth / valueSameMonthLastYear
  // comparedSameMonth: 0,
  // (9)=(4)/(1) Luỹ kế so kế hoạch năm = estimatedMonth / plan
  // accumulatedComparedYearPlan: 0,
  // (10)=(5)/(6) Luỹ kế so cùng kỳ = cumulativeToMonth / cumulativeToMonthLastYear
  // accumulatedComparedPeriod: 0,
};

@Component({
  selector: 'app-edit-production-business-modal',
  templateUrl: './edit-modal.component.html',
  styleUrls: ['./edit-modal.component.scss'],
})
export class EditProductionBusinessModalComponent implements OnInit, OnDestroy, OnChanges {
  @Input() id: any;
  isLoading$: any;
  financialPlanTargetsData: FinancialPlanTargetsModel;
  formGroup: FormGroup;
  options: Options;
  private subscriptions: Subscription[] = [];
  productionValue: { id: any, text: string }[] = [
    {
      id: "Khu vực kinh tế trong nước",
      text: "Khu vực kinh tế trong nước"
    },
    {
      id: "Khu vực có vốn ĐTNN",
      text: "Khu vực có vốn ĐTNN"
    }
  ]

  show: boolean = false;

  constructor(
    private financialPlanTargetsService: FinancialPlanTargetsPageService,
    private fb: FormBuilder,
    public modal: NgbActiveModal,
  ) { }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes.id.currentValue) {
      const sb = this.financialPlanTargetsService.getItemById(this.id).pipe(
        first(),
        catchError((errorMessage) => {
          this.modal.dismiss(errorMessage);
          return of(EMPTY_CUSTOM);
        })
      ).subscribe((res: any) => {
        this.financialPlanTargetsData = res.items[0];
        this.loadForm();
        this.financialPlanTargetsData.type == 1 ? this.formGroup.controls.Classify.reset(1) : this.formGroup.controls.Classify.reset(2);
        this.formGroup.controls.Name.reset(this.financialPlanTargetsData.name);
        this.show = true;
      });
      this.subscriptions.push(sb);
    } else {
      this.financialPlanTargetsData = EMPTY_CUSTOM;
      this.loadForm();
      this.resetForm();
      this.show = true;
    }
  }

  ngOnInit(): void {
    this.isLoading$ = this.financialPlanTargetsService.isLoading$;
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

  GetValue(control1: string, control2: string) {
    if (this.formGroup.value[control1] !== 0 && this.formGroup.value[control2] !== 0) {
      const result = this.formGroup.value[control1] / this.formGroup.value[control2];
      return Math.floor(result * 100) / 100;
    }
    return 0
  }

  GetYearMonth(type: string) {
    const result = this.formGroup.controls.Date.value.split("-") // ["YYYY", "MM"]
    if (type == "Year"){
      return result[0]
    }
    if (type == "Month"){
      return result[1]
    }
  }

  loadForm() {
    this.formGroup = this.fb.group({
      Classify: [1],
      Name: ['Khu vực kinh tế trong nước', Validators.required],
      Unit: [this.financialPlanTargetsData.unit, Validators.required],
      Date: [this.financialPlanTargetsData.date],
      Plan: [this.financialPlanTargetsData.plan],
      // ValueSameMonthLastYear: [this.financialPlanTargetsData.valueSameMonthLastYear],
      // ValueLastMonth: [this.financialPlanTargetsData.valueLastMonth],
      EstimatedMonth: [this.financialPlanTargetsData.estimatedMonth],
      CumulativeToMonth: [this.financialPlanTargetsData.cumulativeToMonth],
      // CumulativeToMonthLastYear: [this.financialPlanTargetsData.cumulativeToMonthLastYear],
    });
    this.formGroup.controls.Classify.valueChanges.subscribe((x: number) => {
      if (x == 1) {
        this.formGroup.controls.Name.reset('Khu vực kinh tế trong nước')
      } else {
        this.formGroup.controls.Name.reset('')
      }
    })
  }

  resetForm() {
    this.formGroup.controls.Classify.reset(1);
    this.formGroup.controls.Name.reset('Khu vực kinh tế trong nước', { onlySelf: true, emitEvent: false });
    this.formGroup.controls.Unit.reset("");
    this.formGroup.controls.Date.reset(moment().format('YYYY-MM'));
    this.formGroup.controls.Plan.reset(0);
    // this.formGroup.controls.ValueSameMonthLastYear.reset(0);
    // this.formGroup.controls.ValueLastMonth.reset(0);
    this.formGroup.controls.EstimatedMonth.reset(0);
    this.formGroup.controls.CumulativeToMonth.reset(0);
    // this.formGroup.controls.CumulativeToMonthLastYear.reset(0);
  }

  save() {
    this.prepareData();
    if (this.id) {
      this.edit();
    } else {
      this.create();
    }
  }

  private prepareData() {
    const formData = this.formGroup.value;
    this.financialPlanTargetsData.name = formData.Name;
    this.financialPlanTargetsData.type = formData.Classify == 1 ? 1 : 2;
    this.financialPlanTargetsData.unit = formData.Unit;
    this.financialPlanTargetsData.date = formData.Date;
    this.financialPlanTargetsData.plan = formData.Plan;
    // this.financialPlanTargetsData.valueSameMonthLastYear = formData.ValueSameMonthLastYear;
    // this.financialPlanTargetsData.valueLastMonth = formData.ValueLastMonth;
    this.financialPlanTargetsData.estimatedMonth = formData.EstimatedMonth;
    this.financialPlanTargetsData.cumulativeToMonth = formData.CumulativeToMonth;
    // this.financialPlanTargetsData.cumulativeToMonthLastYear = formData.CumulativeToMonthLastYear;
  }

  edit() {
    const sbUpdate = this.financialPlanTargetsService.update(this.financialPlanTargetsData).pipe(
      tap(() => {
        this.modal.close();
      }),
      catchError((errorMessage) => {
        this.modal.dismiss(errorMessage);
        return of(this.financialPlanTargetsData);
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
    const sbCreate = this.financialPlanTargetsService.create(this.financialPlanTargetsData).pipe(
      tap(() => {
        this.modal.close();
      }),
      catchError((errorMessage) => {
        this.modal.dismiss(errorMessage);
        return of(this.financialPlanTargetsData);
      }),
    ).subscribe((res: any) => {
      Swal.fire({
        icon: res.status == 1 ? 'success' : 'error',
        title: res.status == 1 ? 'Thêm mới thành công' : 'Thêm mới thất bại',
        confirmButtonText: 'Xác nhận',
        text: res.status == 0 ? res.error.msg : 'Thêm mới ' + (res.status == 1 ? 'thành công' : 'thất bại'),
      });
    });
    this.subscriptions.push(sbCreate);
  }

  ngOnDestroy(): void {
    this.subscriptions.forEach(sb => sb.unsubscribe());
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
