import { ChangeDetectorRef, Component, Input, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal, NgbDateAdapter, NgbDateParserFormatter } from '@ng-bootstrap/ng-bootstrap';
import { of, Subscription } from 'rxjs';
import { catchError, finalize, first, tap } from 'rxjs/operators';
import Swal from 'sweetalert2';

import { ApprovedPowerProjectsModel } from '../../../_models/approved-power-projects.model';
import { ApprovedPowerProjectsPageService } from '../../../_services/approved-power-projects-page.service';
import { CommonService } from 'src/app/_metronic/shared/services/common.service';
import { Options } from 'select2';
import * as moment from 'moment';

const EMPTY_CUSTOM: ApprovedPowerProjectsModel = {
  id: '',
  energyIndustryId: "00000000-0000-0000-0000-000000000000",
  approvedPowerProjectId: "00000000-0000-0000-0000-000000000000",
  projectName: '',
  investorName: '',
  districtId: "00000000-0000-0000-0000-000000000000",
  address: '',
  policyDecision: '',
  wattage: null,
  turbines: null,
  substation: null,
  powerOutput: null,
  year: moment().year(),
  status: "00000000-0000-0000-0000-000000000000",
  area: null,
  note: '',
};

@Component({
  selector: 'app-edit-modal',
  templateUrl: './edit-modal.component.html',
  styleUrls: ['./edit-modal.component.scss'],
})
export class EditApprovedPowerProjectsModalComponent implements OnInit, OnDestroy {
  @Input() id: any;
  @Input() type: any;
  isLoading$:any;
  approvedPowerProjectsData: ApprovedPowerProjectsModel;
  formGroup: FormGroup;

  private subscriptions: Subscription[] = [];
  EnergyIndustryData: any[];
  status: any[];
  options: Options;
  apiLoaded: number = 0;
  districtData: any[];
  
  constructor(
    private approvedPowerProjectsService: ApprovedPowerProjectsPageService,
    private fb: FormBuilder, 
    public modal: NgbActiveModal,
    private commonService: CommonService
    ) { }

  ngOnInit(): void {
    this.isLoading$ = this.approvedPowerProjectsService.isLoading$;
    this.loadEnergyIndustry();
    this.loadStatus();
    this.loadDistrict();
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

  loadDistrict() {
    this.commonService.getDistrict().subscribe((res: any) => {
      const data = [
        { id: "00000000-0000-0000-0000-000000000000", text: "-- Chọn --" },
        ...res.items.map((item: any) => ({
          id: item.districtId,
          text: item.districtName,
        }))
      ]
      this.districtData = data.sort((a, b) => a.text.localeCompare(b.text));
      this.loadApprovedPowerProjects();
    })
  }

  loadEnergyIndustry() {
    const sb = this.commonService.getTypeOfEnergy().subscribe((res: any) => {
      const data = [
        { id: "00000000-0000-0000-0000-000000000000", text: "-- Chọn --" },
        ...res.items.map((item: any) => ({
          id: item.typeOfEnergyId,
          text: item.typeOfEnergyName,
        }))
      ]
      this.EnergyIndustryData = data;
      this.loadApprovedPowerProjects();
    })
    this.subscriptions.push(sb);
  }

  loadStatus() {
    const sb = this.commonService.GetConfig('APPROVED_POWER_PROJECTS_STATUS').subscribe((res: any) => {
      const data = [
        { id: "00000000-0000-0000-0000-000000000000", text: "-- Chọn --" },
        ...res.items.listConfig.map((item: any) => ({
          id: item.categoryId,
          text: item.categoryName,
        }))
      ]
      this.status = data;
      this.loadApprovedPowerProjects();
    })
    this.subscriptions.push(sb);
  }

  loadApprovedPowerProjects() {
    this.apiLoaded++;
    if (this.apiLoaded < 3) {
      return
    }
    if (!this.id) {
      this.clear();
      this.loadForm();
    } else {
      const sb = this.approvedPowerProjectsService.getItemById(this.id).pipe(
        first(),
        catchError((errorMessage) => {
          this.modal.dismiss(errorMessage);
          return of(EMPTY_CUSTOM);
        })
      ).subscribe((res: any) => {
        this.approvedPowerProjectsData = res.items[0];
        // this.approvedPowerProjectsData.wattage = this.f_currency(res.items[0].wattage);
        // this.approvedPowerProjectsData.area = this.f_currency(res.items[0].area);
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
      EnergyIndustryId: [this.approvedPowerProjectsData.energyIndustryId],
      ProjectName: [this.approvedPowerProjectsData.projectName, Validators.required],
      InvestorName: [this.approvedPowerProjectsData.investorName, Validators.required],
      DistrictId: [this.approvedPowerProjectsData.districtId],
      Address: [this.approvedPowerProjectsData.address, Validators.required],
      PolicyDecision: [this.approvedPowerProjectsData.policyDecision, Validators.required],
      Wattage: [this.approvedPowerProjectsData.wattage, Validators.required],
      Turbines: [this.approvedPowerProjectsData.turbines, Validators.required],
      Substation: [this.approvedPowerProjectsData.substation, Validators.required],
      PowerOutput: [this.approvedPowerProjectsData.powerOutput],
      Area: [this.approvedPowerProjectsData.area, Validators.required],
      Year: [this.approvedPowerProjectsData.year, Validators.required],
      Status: [this.approvedPowerProjectsData.status],
      Note: [this.approvedPowerProjectsData.note]
    });
    // this.formGroup.controls.Wattage.valueChanges.subscribe((x) => {
    //   this.formGroup.patchValue({
    //     'Wattage' : this.f_currency(x)
    //   }, {emitEvent : false})
    // })
    // this.formGroup.controls.Area.valueChanges.subscribe((x) => {
    //   this.formGroup.patchValue({
    //     'Area' : this.f_currency(x)
    //   }, {emitEvent : false})
    // })
  }

  clear() {
    EMPTY_CUSTOM.approvedPowerProjectId = "00000000-0000-0000-0000-000000000000",
    EMPTY_CUSTOM.energyIndustryId = "00000000-0000-0000-0000-000000000000",
    EMPTY_CUSTOM.projectName = '',
    EMPTY_CUSTOM.investorName = '',
    EMPTY_CUSTOM.districtId = "00000000-0000-0000-0000-000000000000",
    EMPTY_CUSTOM.address = '',
    EMPTY_CUSTOM.policyDecision = '',
    EMPTY_CUSTOM.wattage = null,
    EMPTY_CUSTOM.area = null,
    EMPTY_CUSTOM.turbines = null,
    EMPTY_CUSTOM.substation = null,
    EMPTY_CUSTOM.powerOutput = null,
    EMPTY_CUSTOM.year = moment().year(),
    EMPTY_CUSTOM.status = "00000000-0000-0000-0000-000000000000",
    EMPTY_CUSTOM.note = '',
    this.approvedPowerProjectsData = EMPTY_CUSTOM;
  }

  private prepareApprovedPowerProjects() {
    const formData = this.formGroup.value;
    this.approvedPowerProjectsData.energyIndustryId = formData.EnergyIndustryId;
    this.approvedPowerProjectsData.projectName = formData.ProjectName;
    this.approvedPowerProjectsData.investorName = formData.InvestorName;
    this.approvedPowerProjectsData.districtId = formData.DistrictId;
    this.approvedPowerProjectsData.address = formData.Address;
    this.approvedPowerProjectsData.policyDecision = formData.PolicyDecision;
    this.approvedPowerProjectsData.wattage = Number(formData.Wattage);
    this.approvedPowerProjectsData.turbines = Number(formData.Turbines);
    this.approvedPowerProjectsData.substation = Number(formData.Substation);
    this.approvedPowerProjectsData.powerOutput = formData.PowerOutput != null ? Number(formData.PowerOutput) : null;
    this.approvedPowerProjectsData.area = Number(formData.Area);
    this.approvedPowerProjectsData.year = Number(formData.Year);
    this.approvedPowerProjectsData.status = formData.Status;
    this.approvedPowerProjectsData.note = formData.Note;
  }

  save() {
    this.prepareApprovedPowerProjects();
    if (this.id) {
      this.edit();
    } else {
      this.create();
    }
  }

  edit() {
    const sbUpdate = this.approvedPowerProjectsService.update(this.approvedPowerProjectsData).pipe(
      tap(() => {
        this.modal.close();
      }),
      catchError((errorMessage) => {
        this.modal.dismiss(errorMessage);
        return of(this.approvedPowerProjectsData);
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
    const sbCreate = this.approvedPowerProjectsService.create(this.approvedPowerProjectsData).pipe(
      tap(() => {
        this.modal.close();
      }),
      catchError((errorMessage) => {
        this.modal.dismiss(errorMessage);
        return of(this.approvedPowerProjectsData);
      }),
    ).subscribe((res: any) => {
      Swal.fire({
        icon: res.status == 1 ? 'success' : 'error',
        title: res.status == 1 ? 'Thêm mới thành công' : 'Thêm mới thất bại',
        confirmButtonText: 'Xác nhận',
        text: res.status == 0 ? res.error.msg : 'Thêm mới ' + (res.status == 1 ? 'thành công' : 'thất bại'),
      });
      this.approvedPowerProjectsData = EMPTY_CUSTOM
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

  isDefaultValue(controlName: any) {
    const control = this.formGroup.controls[controlName];
    const value = control.value;
    if (value == '00000000-0000-0000-0000-000000000000') {
      control.setErrors({ defaultvalue: true });
    }
    else {
      control.setErrors({ defaultvalue: null });
      control.updateValueAndValidity();
    }
    return control.invalid && (control.dirty || control.touched);
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
}
