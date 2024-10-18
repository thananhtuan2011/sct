import { ChangeDetectorRef, Component, Input, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal, NgbDateAdapter, NgbDateParserFormatter } from '@ng-bootstrap/ng-bootstrap';
import { of, Subscription } from 'rxjs';
import { catchError, finalize, first, tap } from 'rxjs/operators';
import Swal from 'sweetalert2';

import { ProposedPowerProjectsModel } from '../../../_models/proposed-power-projects.model';
import { ProposedPowerProjectsPageService } from '../../../_services/proposed-power-projects-page.service';
import { Options } from 'select2';
import { CommonService } from 'src/app/_metronic/shared/services/common.service';

const EMPTY_CUSTOM: ProposedPowerProjectsModel = {
  id: '',
  proposedPowerProjectId: "00000000-0000-0000-0000-000000000000",
  energyIndustryId: "00000000-0000-0000-0000-000000000000",
  projectName: '',
  statusId: '00000000-0000-0000-0000-000000000000',
  investorName: '',
  address: '',
  policyDecision: '',
  wattage: null,
  note: '',
  // proposedDate: '',
};

@Component({
  selector: 'app-edit-modal',
  templateUrl: './edit-modal.component.html',
  styleUrls: ['./edit-modal.component.scss'],
})
export class EditProposedPowerProjectsModalComponent implements OnInit, OnDestroy {
  @Input() id: any;
  @Input() type: any;
  isLoading$:any;
  proposedPowerProjectsData: ProposedPowerProjectsModel;
  formGroup: FormGroup;

  private subscriptions: Subscription[] = [];
  options: Options;
  EnergyIndustryData: any[];
  StatusData: any[] = [];
  apiLoaded: number = 0;
  
  constructor(
    private proposedPowerProjectsService: ProposedPowerProjectsPageService,
    private fb: FormBuilder, 
    public modal: NgbActiveModal,
    private commonService: CommonService
    ) { }

  ngOnInit(): void {
    this.isLoading$ = this.proposedPowerProjectsService.isLoading$;
    this.loadEnergyIndustry();
    this.loadStatus();
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
      this.loadProposedPowerProjects();
    })
    this.subscriptions.push(sb);
  }

  loadStatus() {
    const sb = this.commonService.GetConfig('PROPOSED_POWER_PROJECT_STATUS').subscribe((res: any) => {
      const data = [
        { id: "00000000-0000-0000-0000-000000000000", text: "-- Chọn --" },
        ...res.items.listConfig.map((item: any) => ({
          id: item.categoryId,
          text: item.categoryName,
        }))
      ]
      this.StatusData = data;
      this.loadProposedPowerProjects();
    })
    this.subscriptions.push(sb);
  }

  loadProposedPowerProjects() {
    this.apiLoaded++
    if (this.apiLoaded < 2) {
      return
    }
    if (!this.id) {
      this.clear();
      this.loadForm();
    } else {
      const sb = this.proposedPowerProjectsService.getItemById(this.id).pipe(
        first(),
        catchError((errorMessage) => {
          this.modal.dismiss(errorMessage);
          return of(EMPTY_CUSTOM);
        })
      ).subscribe((res: any) => {
        this.proposedPowerProjectsData = res.items[0];
        this.proposedPowerProjectsData.wattage = this.f_currency(res.items[0].wattage);
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
      EnergyIndustryId: [this.proposedPowerProjectsData.energyIndustryId],
      ProjectName: [this.proposedPowerProjectsData.projectName, Validators.required],
      StatusId: [this.proposedPowerProjectsData.statusId],
      InvestorName: [this.proposedPowerProjectsData.investorName, Validators.required],
      Address: [this.proposedPowerProjectsData.address, Validators.required],
      PolicyDecision: [this.proposedPowerProjectsData.policyDecision, Validators.required],
      Wattage: [this.proposedPowerProjectsData.wattage, Validators.required],
      Note: [this.proposedPowerProjectsData.note],
      // ProposedDate: [this.proposedPowerProjectsData.proposedDate, Validators.required]
    });
    this.formGroup.controls.Wattage.valueChanges.subscribe((x) => {
      this.formGroup.patchValue({
        'Wattage' : this.f_currency(x)
      }, {emitEvent : false})
    })
  }

  clear() {
    EMPTY_CUSTOM.proposedPowerProjectId = "00000000-0000-0000-0000-000000000000",
    EMPTY_CUSTOM.energyIndustryId = "00000000-0000-0000-0000-000000000000",
    EMPTY_CUSTOM.projectName = '',
    EMPTY_CUSTOM.statusId = '00000000-0000-0000-0000-000000000000',
    EMPTY_CUSTOM.investorName = '',
    EMPTY_CUSTOM.address = '',
    EMPTY_CUSTOM.policyDecision = '',
    EMPTY_CUSTOM.wattage = null,
    EMPTY_CUSTOM.note = '',
    // EMPTY_CUSTOM.proposedDate = '',
    this.proposedPowerProjectsData = EMPTY_CUSTOM;
  }

  private prepareProposedPowerProjects() {
    const formData = this.formGroup.value;
    this.proposedPowerProjectsData.energyIndustryId = formData.EnergyIndustryId;
    this.proposedPowerProjectsData.projectName = formData.ProjectName;
    this.proposedPowerProjectsData.statusId = formData.StatusId;
    this.proposedPowerProjectsData.investorName = formData.InvestorName;
    this.proposedPowerProjectsData.address = formData.Address;
    this.proposedPowerProjectsData.policyDecision = formData.PolicyDecision;
    this.proposedPowerProjectsData.wattage = Number(formData.Wattage.replaceAll(',',''));
    this.proposedPowerProjectsData.note = formData.Note;
    // this.proposedPowerProjectsData.proposedDate = formData.ProposedDate;
  }

  save() {
    this.prepareProposedPowerProjects();
    if (this.id) {
      this.edit();
    } else {
      this.create();
    }
  }

  edit() {
    const sbUpdate = this.proposedPowerProjectsService.update(this.proposedPowerProjectsData).pipe(
      tap(() => {
        this.modal.close();
      }),
      catchError((errorMessage) => {
        this.modal.dismiss(errorMessage);
        return of(this.proposedPowerProjectsData);
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
    const sbCreate = this.proposedPowerProjectsService.create(this.proposedPowerProjectsData).pipe(
      tap(() => {
        this.modal.close();
      }),
      catchError((errorMessage) => {
        this.modal.dismiss(errorMessage);
        return of(this.proposedPowerProjectsData);
      }),
    ).subscribe((res: any) => {
      Swal.fire({
        icon: res.status == 1 ? 'success' : 'error',
        title: res.status == 1 ? 'Thêm mới thành công' : 'Thêm mới thất bại',
        confirmButtonText: 'Xác nhận',
        text: res.status == 0 ? res.error.msg : 'Thêm mới ' + (res.status == 1 ? 'thành công' : 'thất bại'),
      });
      this.proposedPowerProjectsData = EMPTY_CUSTOM
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
