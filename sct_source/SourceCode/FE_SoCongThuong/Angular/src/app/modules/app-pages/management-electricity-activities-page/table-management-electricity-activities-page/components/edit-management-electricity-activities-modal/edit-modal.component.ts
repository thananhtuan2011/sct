import { Component, Input, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { of, Subscription } from 'rxjs';
import { catchError, finalize, first, tap } from 'rxjs/operators';
import Swal from 'sweetalert2';

import { ManagementElectricityActivitiesModel } from '../../../_models/management-electricity-activities.model';
import { ManagementElectricityActivitiesPageService } from '../../../_services/management-electricity-activities-page.service';
import { Options } from 'select2';
import { CommonService } from 'src/app/_metronic/shared/services/common.service';

@Component({
  selector: 'app-edit-modal',
  templateUrl: './edit-modal.component.html',
  styleUrls: ['./edit-modal.component.scss'],
})

export class EditManagementElectricityActivitiesModalComponent implements OnInit, OnDestroy {
  @Input() id: any;
  @Input() type: any;
  private subscriptions: Subscription[] = [];
  isLoading$: any;
  options: Options;
  editData: ManagementElectricityActivitiesModel;
  formGroup: FormGroup;
  show: boolean = false;

  districtData: {id: string, text: string}[];

  constructor(
    private service: ManagementElectricityActivitiesPageService,
    private fb: FormBuilder,
    public modal: NgbActiveModal,
    private commonService: CommonService,
  ) { }

  ngOnInit(): void {
    this.isLoading$ = this.service.isLoading$;
    this.options = {
      theme: 'bootstrap5',
      templateSelection: this.templateSelection,
    };
    this.loadDistrict();
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
      this.districtData = data;
      this.loadData();
    })
  }

  loadData() {
    if (!this.id) {
      this.clear();
      this.loadForm();
    } else {
      const sb = this.service.getItemById(this.id).pipe(
        first(),
        catchError((errorMessage) => {
          this.modal.dismiss(errorMessage);
          return of(this.clear());
        })
      ).subscribe((res: any) => {
        this.editData = res.items[0] as ManagementElectricityActivitiesModel;
        this.loadForm();

        if(this.type) {
          this.formGroup.disable();
        }
      });
      this.subscriptions.push(sb);
    }
  }

  clear() {
    const EmptyModel = {
      managementElectricityActivitiesId: '00000000-0000-0000-0000-000000000000',
      projectName: '',
      districtId: '00000000-0000-0000-0000-000000000000',
      wattage: 0,
      maxWattage: 0,
      type: null,
      dateOfAcceptance: '',
      connectorAgreement: '',
      powerPurchaseAgreement: '',
      anotherContent:'',
    } as ManagementElectricityActivitiesModel;
    this.editData = EmptyModel;
    return EmptyModel;
  }

  loadForm() {
    this.formGroup = this.fb.group({
      ProjectName: [this.editData.projectName, Validators.required],
      DistrictId: [this.editData.districtId],
      Wattage: [this.editData.wattage, Validators.required],
      MaxWattage: [this.editData.maxWattage, Validators.required],
      Type: [this.editData.type, Validators.required],
      DateOfAcceptance: [this.editData.dateOfAcceptance, Validators.required],
      ConnectorAgreement: [this.editData.connectorAgreement, Validators.required],
      PowerPurchaseAgreement: [this.editData.powerPurchaseAgreement, Validators.required],
      AnotherContent: [this.editData.anotherContent]
    })

    this.show = true;
  }

  private prepare() {
    const formData = this.formGroup.value;
    this.editData.projectName = formData.ProjectName;
    this.editData.districtId = formData.DistrictId;
    this.editData.wattage = formData.Wattage;
    this.editData.maxWattage = formData.MaxWattage;
    this.editData.type = formData.Type;
    this.editData.dateOfAcceptance = formData.DateOfAcceptance;
    this.editData.connectorAgreement = formData.ConnectorAgreement;
    this.editData.powerPurchaseAgreement = formData.PowerPurchaseAgreement;
    this.editData.anotherContent = formData.AnotherContent;
  }

  save() {
    this.prepare();
    if (this.id) {
      this.edit();
    } else {
      this.create();
    }
  }

  edit() {
    const sbUpdate = this.service.update(this.editData).pipe(
      tap(() => {
        this.modal.close();
      }),
      catchError((errorMessage) => {
        this.modal.dismiss(errorMessage);
        return of(this.editData);
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
    const sbCreate = this.service.create(this.editData).pipe(
      tap(() => {
        this.modal.close();
      }),
      catchError((errorMessage) => {
        this.modal.dismiss(errorMessage);
        return of(this.editData);
      }),
    ).subscribe((res: any) => {
      Swal.fire({
        icon: res.status == 1 ? 'success' : 'error',
        title: res.status == 1 ? 'Thêm mới thành công' : 'Thêm mới thất bại',
        confirmButtonText: 'Xác nhận',
        text: res.status == 0 ? res.error.msg : 'Thêm mới ' + (res.status == 1 ? 'thành công' : 'thất bại'),
      });
      this.clear();
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

  isDefaultValue(controlName: any) {
    const control = this.formGroup.controls[controlName];
    const value = control.value;
    if (value == '00000000-0000-0000-0000-000000000000') {
      control.setErrors({ defaultvalue: true });
      return control.invalid && (control.touched || control.dirty);
    }
    else {
      control.setErrors(null);
      return false;
    }
  }

  check_formGroup() {
    this.formGroup.markAllAsTouched();
    if (!this.formGroup.invalid) {
      this.save()
    }
  }
}
