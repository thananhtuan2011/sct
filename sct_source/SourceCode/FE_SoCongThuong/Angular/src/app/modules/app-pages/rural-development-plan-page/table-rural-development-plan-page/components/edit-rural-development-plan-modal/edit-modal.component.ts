import { ChangeDetectorRef, Component, Input, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal, NgbDateAdapter, NgbDateParserFormatter } from '@ng-bootstrap/ng-bootstrap';
import { of, Subscription } from 'rxjs';
import { catchError, finalize, first, tap } from 'rxjs/operators';
import Swal from 'sweetalert2';

import { RuralDevelopmentPlanModel } from '../../../_models/rural-development-plan.model';
import { RuralDevelopmentPlanPageService } from '../../../_services/rural-development-plan-page.service';

import { Options } from 'select2';

const EMPTY_CUSTOM: RuralDevelopmentPlanModel = {
  id: '',
  /// Kế hoạch phát triển chợ nông thôn
  ruralDevelopmentPlanId: '00000000-0000-0000-0000-000000000000',
  /// Tên TTTM / Siêu thị
  superMarketShoppingMallName: '',
  /// Địa chỉ
  address: '',
  /// Tổng vốn đầu tư
  totalInvestment: null,
  /// Ngân sách
  budget: null,
  /// Ngoài ngân sách
  outOfBudget: null,
  /// Loại hình: 0 - Xây dựng, 1 - Nâng cấp
  type: null,
  /// Giai đoạn
  stageId: '',
  /// Dữ liệu giai đoạn
  stages: [],
};

@Component({
  selector: 'app-edit-modal',
  templateUrl: './edit-modal.component.html',
  styleUrls: ['./edit-modal.component.scss'],
})

export class EditRuralDevelopmentPlanModalComponent implements OnInit, OnDestroy {
  @Input() id: any;
  isLoading$: any;
  ruralDevelopmentPlanData: RuralDevelopmentPlanModel;
  formGroup: FormGroup;
  options: Options;
  stageData: any[] = [
    // {
    //   id: '00000000-0000-0000-0000-000000000000',
    //   text: '-- Chọn giai đoạn --',
    //   start_year: 0,
    //   end_year: 0,
    // }
  ];
  currentYearRange: any[] = [];
  pastYearRange: any[] = [];
  isBuild: boolean = false;
  isUpgrade: boolean = false;
  show: boolean = false;

  private subscriptions: Subscription[] = [];

  constructor(
    private ruralDevelopmentPlanService: RuralDevelopmentPlanPageService,
    private fb: FormBuilder, public modal: NgbActiveModal,
  ) { }

  ngOnInit(): void {
    this.isLoading$ = this.ruralDevelopmentPlanService.isLoading$;
    this.loadStage();
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
    return new Promise( resolve => setTimeout(resolve, ms) );
  }

  loadStage() {
    this.ruralDevelopmentPlanService.loadStage().subscribe((res: any) => {
      var stages = [
        // {
        //   id: '00000000-0000-0000-0000-000000000000',
        //   text: '-- Chọn giai đoạn --',
        //   start_year: 0,
        //   end_year: 0,
        // }
      ]
      for (var item of res.items) {
        let stage = {
          id: item.stageId,
          text: item.stageName,
          start_year: item.startYear,
          end_year: item.endYear,
        }
        stages.push(stage)
      }
      this.stageData = stages
      this.loadRuralDevelopmentPlan();
    })
  }

  create_range(id: any) {
    const item = this.stageData.find((x: any) => x.id == id)
    this.currentYearRange = [];
    if (item.start_year != item.end_year) {    
      for (var i = item.start_year; i <= item.end_year; i++) {
      this.currentYearRange.push(i)
      }
    }
  }

  create_form_by_range() {
    if (this.currentYearRange.length > 0) {
      for (var i of this.currentYearRange) {
        this.formGroup.addControl(i, this.fb.control(null));
        this.formGroup.updateValueAndValidity();
      }
      this.pastYearRange = this.currentYearRange
    }
  }

  remove_form_by_change() {
    for (var i of this.pastYearRange) {
      this.formGroup.removeControl(i);
      this.formGroup.updateValueAndValidity();
    }
    this.pastYearRange = [];
  }

  dynamic_form_value_change(event: any, control: any){
    this.formGroup.patchValue({
      [String(control)] : this.f_currency(event.target.value)
    }, { emitEvent: false })
  }

  load_data_for_dynamic_form(data: any) {
    if (data.length > 0) {
      this.currentYearRange.forEach(year => {
        const item = data.find((x: any) => x.year == year)
        if (item.budget) {
          this.formGroup.controls[year].setValue(this.f_currency(item.budget))
        } else {
          this.formGroup.controls[year].setValue(null)
        }
        this.formGroup.updateValueAndValidity();
      })
    }
  }

  load_type(typeId: any) {
    if (typeId == 0) {
      this.formGroup.controls.IsBuild.setValue(true)
    } else if (typeId == 1) {
      this.formGroup.controls.IsUpgrade.setValue(true)
    }
  }

  loadRuralDevelopmentPlan() {
    if (!this.id) {
      this.clear();
      this.ruralDevelopmentPlanData.stageId = this.stageData[0].id
      this.loadForm();
      this.create_range(this.ruralDevelopmentPlanData.stageId);
      this.create_form_by_range();
      this.show = true;
    } else {
      const sb = this.ruralDevelopmentPlanService.getItemById(this.id).pipe(
        first(),
        catchError((errorMessage) => {
          this.modal.dismiss(errorMessage);
          return of(EMPTY_CUSTOM);
        })
      ).subscribe((res: any) => {
        this.ruralDevelopmentPlanData = res.data;
        this.ruralDevelopmentPlanData.totalInvestment = this.f_currency(res.data.totalInvestment)
        this.ruralDevelopmentPlanData.budget = this.f_currency(res.data.budget)
        this.ruralDevelopmentPlanData.outOfBudget = this.f_currency(res.data.outOfBudget)
        this.loadForm();
        this.create_range(this.ruralDevelopmentPlanData.stageId);
        this.create_form_by_range();
        this.load_data_for_dynamic_form(res.data.stages);
        this.load_type(res.data.type);
        this.show = true;
      });
      this.subscriptions.push(sb);
    }
  }

  loadForm() {
    this.formGroup = this.fb.group({
    /// Tên TTTM / Siêu thị
    SuperMarketShoppingMallName: [this.ruralDevelopmentPlanData.superMarketShoppingMallName, Validators.required],
    /// Địa chỉ
    Address: [this.ruralDevelopmentPlanData.address, Validators.required],
    /// Tổng vốn đầu tư
    TotalInvestment: [this.ruralDevelopmentPlanData.totalInvestment],
    /// Ngân sách
    Budget: [this.ruralDevelopmentPlanData.budget],
    /// Ngoài ngân sách
    OutOfBudget: [this.ruralDevelopmentPlanData.outOfBudget],
    /// Loại hình: isBuild - 0 - Xây dựng, isUpgrade - 1 - Nâng cấp
    /// Xây dựng: true / false
    IsBuild: [this.isBuild],
    /// Nâng cấp: true / false
    IsUpgrade: [this.isUpgrade],
    /// Giai đoạn
    StageId : [this.ruralDevelopmentPlanData.stageId],
    });
    this.formGroup.controls.StageId.valueChanges.subscribe((id) => {
      this.remove_form_by_change();
      this.create_range(id);
      this.create_form_by_range();
    })
    this.formGroup.controls.TotalInvestment.valueChanges.subscribe((x) => {
      this.formGroup.patchValue({
        "TotalInvestment": this.f_currency(x)
      }, { emitEvent: false })
    })
    this.formGroup.controls.Budget.valueChanges.subscribe((x) => {
      this.formGroup.patchValue({
        "Budget": this.f_currency(x)
      }, { emitEvent: false })
    })
    this.formGroup.controls.OutOfBudget.valueChanges.subscribe((x) => {
      this.formGroup.patchValue({
        "OutOfBudget": this.f_currency(x)
      }, { emitEvent: false })
    })
  }

  clear() {
    /// Kế hoạch phát triển chợ nông thôn
    EMPTY_CUSTOM.ruralDevelopmentPlanId = '00000000-0000-0000-0000-000000000000',
    /// Tên TTTM / Siêu thị
    EMPTY_CUSTOM.superMarketShoppingMallName = '',
    /// Địa chỉ
    EMPTY_CUSTOM.address = '',
    /// Tổng vốn đầu tư
    EMPTY_CUSTOM.totalInvestment = null,
    /// Ngân sách
    EMPTY_CUSTOM.budget = null,
    /// Ngoài ngân sách
    EMPTY_CUSTOM.outOfBudget = null,
    /// Loại hình: 0 - Xây dựng, 1 - Nâng cấp
    EMPTY_CUSTOM.type = null,
    /// Giai đoạn
    EMPTY_CUSTOM.stageId = '',
    /// Dữ liệu giai đoạn
    EMPTY_CUSTOM.stages = [],

    this.ruralDevelopmentPlanData = EMPTY_CUSTOM;
  }

  private prepareRuralDevelopmentPlanData() {
    const formData = this.formGroup.value;
    this.ruralDevelopmentPlanData.superMarketShoppingMallName = formData.SuperMarketShoppingMallName,
    this.ruralDevelopmentPlanData.address = formData.Address,
    this.ruralDevelopmentPlanData.totalInvestment = formData.TotalInvestment ? Number(formData.TotalInvestment.replaceAll(',' , '')) : null,
    this.ruralDevelopmentPlanData.budget = formData.Budget ? Number(formData.Budget.replaceAll(',' , '')) : null,
    this.ruralDevelopmentPlanData.outOfBudget = formData.OutOfBudget ? Number(formData.OutOfBudget.replaceAll(',' , '')) : null,
    this.ruralDevelopmentPlanData.type = this.return_type();
    this.ruralDevelopmentPlanData.stageId = formData.StageId;
    this.ruralDevelopmentPlanData.stages = this.prepareStageDate();
  }

  prepareStageDate() {
    const result: any[] = [];
    const formData = this.formGroup;
    this.currentYearRange.forEach(element => {
      let obj = {
        planStageId: '00000000-0000-0000-0000-000000000000',
        ruralDevelopmentPlanId: this.ruralDevelopmentPlanData.ruralDevelopmentPlanId,
        stageId: this.ruralDevelopmentPlanData.stageId,
        year: element,
        budget: formData.controls[element].value && formData.controls[element].value != "0" ? Number(formData.controls[element].value.replaceAll(',' , '')) : null,
      }
      result.push(obj)
    });
    return result
  }

  save() {
    this.prepareRuralDevelopmentPlanData();
    // console.log(this.ruralDevelopmentPlanData)
    if (this.id) {
      this.edit();
    } else {
      this.create();
    }
  }

  edit() {
    const sbUpdate = this.ruralDevelopmentPlanService.update(this.ruralDevelopmentPlanData).pipe(
      tap(() => {
        this.modal.close();
      }),
      catchError((errorMessage) => {
        this.modal.dismiss(errorMessage);
        return of(this.ruralDevelopmentPlanData);
      }),
    ).subscribe((res: any) => {
      Swal.fire({
        icon: res.status == 1 ? 'success' : 'error',
        title: res.status == 1 ? 'Chỉnh sửa thành công' : 'Chỉnh sửa thất bại',
        confirmButtonText: 'Xác nhận',
        text: 'Chỉnh sửa ' + (res.status == 1 ? 'thành công' : 'thất bại'),
      });
    });
    this.subscriptions.push(sbUpdate);
  }

  create() {
    const sbCreate = this.ruralDevelopmentPlanService.create(this.ruralDevelopmentPlanData).pipe(
      tap(() => {
        this.modal.close();
      }), 
      catchError((errorMessage) => {
        this.modal.dismiss(errorMessage);
        return of(this.ruralDevelopmentPlanData);
      }),
    ).subscribe((res: any) => {
      Swal.fire({
        icon: res.status == 1 ? 'success' : 'error',
        title: res.status == 1 ? 'Thêm mới thành công' : 'Thêm mới thất bại',
        confirmButtonText: 'Xác nhận',
        text: 'Thêm mới ' + (res.status == 1 ? 'thành công' : 'thất bại'),
      });
    });
    this.subscriptions.push(sbCreate);
  }

  return_type() {
    const isBuild_Value = this.formGroup.controls['IsBuild'].value;
    const isUpgrade_Value = this.formGroup.controls['IsUpgrade'].value;
    if (isBuild_Value == true && isUpgrade_Value == false) {
      return 0;
    }
    else if (isBuild_Value == false && isUpgrade_Value == true) {
      return 1;
    }
    else {
      return null
    }
  }

  buildorupgrade(controlName: any) {
    const value = this.formGroup.controls[controlName].value
    if (controlName == 'IsBuild' && value == true) {
      this.formGroup.controls['IsUpgrade'].setValue(!value)
    }
    else if (controlName == 'IsUpgrade' && value == true) {
      this.formGroup.controls['IsBuild'].setValue(!value)
    }
  }

  f_currency(value: any, args?: any): any {
    let nbr = Number((value + '').replace(/,|-/g, ''));
    return (nbr + '').replace(/(\d)(?=(\d{3})+(?!\d))/g, '$1,');
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

  isCheckBoxChecked(): boolean {
    const control_build = this.formGroup.controls['IsBuild']
    const control_upgrade = this.formGroup.controls['IsUpgrade']
    const check_build = control_build.value;
    const check_upgrade = control_upgrade.value;
    if (check_build == check_upgrade) {
      control_build.setErrors({ none_checked: true })
    }
    return control_build.invalid && (control_build.dirty || control_build.touched || control_upgrade.dirty || control_upgrade.touched);
  }

  isnullcapitalunit(controlName: any, controlNameUnit: any) {
    const control = this.formGroup.controls[controlName];
    const controlUnit = this.formGroup.controls[controlNameUnit];
    const value = control.value;
    const unit_value = controlUnit.value;
    if (value && (unit_value == 'null')) {
      controlUnit.setErrors({noneUnit : true});
    }
    else {
      controlUnit.setErrors({noneUnit : null});
      controlUnit.updateValueAndValidity();
    }
    return controlUnit.invalid;
  }

  isnullcapital(controlName: any, controlNameUnit: any) {
    const control = this.formGroup.controls[controlName];
    const controlUnit = this.formGroup.controls[controlNameUnit];
    const value = control.value;
    const unit_value = controlUnit.value;
    if (!value && (unit_value != 'null')) {
      control.setErrors({noneValue : true});
    }
    else {
      control.setErrors({noneValue : null});
      control.updateValueAndValidity();
    };
    return control.invalid;
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
