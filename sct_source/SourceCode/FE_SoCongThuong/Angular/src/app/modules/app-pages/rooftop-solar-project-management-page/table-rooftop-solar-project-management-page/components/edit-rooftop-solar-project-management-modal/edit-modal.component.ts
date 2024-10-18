import { ChangeDetectorRef, Component, Input, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal, NgbDateAdapter, NgbDateParserFormatter } from '@ng-bootstrap/ng-bootstrap';
import { of, Subscription } from 'rxjs';
import { catchError, finalize, first, tap } from 'rxjs/operators';
import Swal from 'sweetalert2';

import { RooftopSolarProjectManagementModel } from '../../../_models/rooftop-solar-project-management.model';
import { RooftopSolarProjectManagementPageService } from '../../../_services/rooftop-solar-project-management-page.service';
import { CommonService } from 'src/app/_metronic/shared/services/common.service';
import { Options } from 'select2';
import * as moment from 'moment';

const EMPTY_CUSTOM: RooftopSolarProjectManagementModel = {
  id: '',
  rooftopSolarProjectManagementId: "00000000-0000-0000-0000-000000000000",
  projectName: '',
  investorName: '',
  address: '',
  area: null,
  surveyPolicy: '',
  wattage: null,
  progress: '',
  district: "00000000-0000-0000-0000-000000000000",
  operationDay: null
};

@Component({
  selector: 'app-edit-modal',
  templateUrl: './edit-modal.component.html',
  styleUrls: ['./edit-modal.component.scss'],

})
export class EditRooftopSolarProjectManagementModalComponent implements OnInit, OnDestroy {
  @Input() id: any;
  @Input() type: any;
  @Input() districtData: any;
  
  options: Options;
  isLoading$:any;
  rooftopSolarProjectManagementData: RooftopSolarProjectManagementModel;
  formGroup: FormGroup;
  private subscriptions: Subscription[] = [];
  
  constructor(
    private rooftopSolarProjectManagementService: RooftopSolarProjectManagementPageService,
    private fb: FormBuilder, public modal: NgbActiveModal,
    ) { }

  ngOnInit(): void {
    this.isLoading$ = this.rooftopSolarProjectManagementService.isLoading$;
    this.loadRooftopSolarProjectManagement();
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

  loadRooftopSolarProjectManagement() {
    if (!this.id) {
      this.clear();
      this.loadForm();
    } else {
      const sb = this.rooftopSolarProjectManagementService.getItemById(this.id).pipe(
        first(),
        catchError((errorMessage) => {
          this.modal.dismiss(errorMessage);
          return of(EMPTY_CUSTOM);
        })
      ).subscribe((res: any) => {
        this.rooftopSolarProjectManagementData = res.items[0];
        // this.rooftopSolarProjectManagementData.area = this.f_currency(res.items[0].area);
        // this.rooftopSolarProjectManagementData.wattage = this.f_currency(res.items[0].wattage);
        this.loadForm();
        if (this.type) {
          this.formGroup.disable();
        }
        this.formGroup.updateValueAndValidity();
      });
      this.subscriptions.push(sb);
    }
  }

  loadForm() {
    this.formGroup = this.fb.group({
      ProjectName: [this.rooftopSolarProjectManagementData.projectName, Validators.required],
      InvestorName: [this.rooftopSolarProjectManagementData.investorName, Validators.required],
      Address: [this.rooftopSolarProjectManagementData.address, Validators.required],
      Area: [this.rooftopSolarProjectManagementData.area, Validators.required],
      SurveyPolicy: [this.rooftopSolarProjectManagementData.surveyPolicy, Validators.required],
      Wattage: [this.rooftopSolarProjectManagementData.wattage, Validators.required],
      Progress: [this.rooftopSolarProjectManagementData.progress, Validators.required],
      District: [this.rooftopSolarProjectManagementData.district],
      OperationDay: [this.rooftopSolarProjectManagementData.operationDay !== null ? this.convert_date_string(this.rooftopSolarProjectManagementData.operationDay) : null]
    });
  }

  clear() {
    EMPTY_CUSTOM.rooftopSolarProjectManagementId = "00000000-0000-0000-0000-000000000000";
    EMPTY_CUSTOM.projectName = '';
    EMPTY_CUSTOM.investorName = '';
    EMPTY_CUSTOM.address = '';
    EMPTY_CUSTOM.area = null;
    EMPTY_CUSTOM.surveyPolicy = '';
    EMPTY_CUSTOM.wattage = null;
    EMPTY_CUSTOM.progress = '';
    EMPTY_CUSTOM.district = "00000000-0000-0000-0000-000000000000",
    EMPTY_CUSTOM.operationDay = null
    this.rooftopSolarProjectManagementData = EMPTY_CUSTOM;
  }

  private prepareRooftopSolarProjectManagement() {
    const formData = this.formGroup.value;
    this.rooftopSolarProjectManagementData.projectName = formData.ProjectName;
    this.rooftopSolarProjectManagementData.investorName = formData.InvestorName;
    this.rooftopSolarProjectManagementData.address = formData.Address;
    this.rooftopSolarProjectManagementData.area = Number(formData.Area);
    this.rooftopSolarProjectManagementData.surveyPolicy = formData.SurveyPolicy;
    this.rooftopSolarProjectManagementData.wattage = Number(formData.Wattage);
    this.rooftopSolarProjectManagementData.progress = formData.Progress;
    this.rooftopSolarProjectManagementData.district = formData.District;  
    this.rooftopSolarProjectManagementData.operationDay = this.convert_date(formData.OperationDay);
    
  }

  save() {
    this.prepareRooftopSolarProjectManagement();
    if (this.id) {
      this.edit();
    } else {
      this.create();
    }
  }

  edit() {
    const sbUpdate = this.rooftopSolarProjectManagementService.update(this.rooftopSolarProjectManagementData).pipe(
      tap(() => {
        this.modal.close();
      }),
      catchError((errorMessage) => {
        this.modal.dismiss(errorMessage);
        return of(this.rooftopSolarProjectManagementData);
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
    const sbCreate = this.rooftopSolarProjectManagementService.create(this.rooftopSolarProjectManagementData).pipe(
      tap(() => {
        this.modal.close();
      }),
      catchError((errorMessage) => {
        this.modal.dismiss(errorMessage);
        return of(this.rooftopSolarProjectManagementData);
      }),
    ).subscribe((res: any) => {
      Swal.fire({
        icon: res.status == 1 ? 'success' : 'error',
        title: res.status == 1 ? 'Thêm mới thành công' : 'Thêm mới thất bại',
        confirmButtonText: 'Xác nhận',
        text: res.status == 0 ? res.error.msg : 'Thêm mới ' + (res.status == 1 ? 'thành công' : 'thất bại'),
      });
      this.rooftopSolarProjectManagementData = EMPTY_CUSTOM
    });
    this.subscriptions.push(sbCreate);
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
      this.save();
    }
  }
  
  isDefaultValue(controlName: any) {
    const control = this.formGroup.controls[controlName];
    const value = control.value;
    if (value == '00000000-0000-0000-0000-000000000000' || value == 0) {
      control.setErrors({ defaultvalue: true });
    }
    else {
      control.setErrors(null);
    }
    return control.invalid && (control.touched || !control.pristine);
  }
  
  convert_date_string(string_date: string) {
    var date = string_date.split("T")[0];
    var list = date.split("-"); //["year", "month", "day"]
    var result = list[2] + "/" + list[1] + "/" + list[0]
    return result
  }
  
  convert_date(string_date: string) {
    var result = moment.utc(string_date, "DD/MM/YYYY");
    return result
  }
}
