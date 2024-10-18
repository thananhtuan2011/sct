import { Component, Input, OnDestroy, OnInit, ViewEncapsulation } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { of, Subscription } from 'rxjs';
import { catchError, finalize, first, tap } from 'rxjs/operators';
import Swal from 'sweetalert2';

import { ElectricOperatingUnitsModel } from '../../../_models/electric-operating-units.model';
import { ElectricOperatingUnitsPageService } from '../../../_services/electric-operating-units-page.service';
import { Options } from 'select2';
import { CommonService } from 'src/app/_metronic/shared/services/common.service';

@Component({
  selector: 'app-edit-modal',
  templateUrl: './edit-modal.component.html',
  styleUrls: ['./edit-modal.component.scss'],
  encapsulation: ViewEncapsulation.Emulated,
})

export class EditElectricOperatingUnitsModalComponent implements OnInit, OnDestroy {
  @Input() id: any;
  @Input() type: any;
  private subscriptions: Subscription[] = [];
  isLoading$: any;
  options: Options;
  editData: ElectricOperatingUnitsModel;
  formGroup: FormGroup;
  show: boolean = false;

  businessData: any[];
  supplierData: any[];
  statusData: any[] = [
    {
      id: 0,
      text: "Còn hoạt động"
    }, {
      id: 1,
      text: "Ngừng hoạt động"
    }
  ];
  apiLoaded: number = 0;

  constructor(
    private service: ElectricOperatingUnitsPageService,
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
    this.loadBusiness();
    this.loadSupplier();
  }

  public templateSelection = (state: any): JQuery | string => {
    if (!state.id) {
      return state.text;
    }
    return jQuery('<span class="form-select form-select-solid form-select-lg">' + state.text + '</span>');
  }

  loadBusiness() {
    this.commonService.getBusiness().subscribe((res: any) => {
      const data = [
        { id: "00000000-0000-0000-0000-000000000000", text: "-- Chọn --" },
        ...res.items.map((item: any) => ({
          id: item.businessId,
          text: item.businessNameVi,
          address: item.diaChiTruSo,
          manager: item.giamDoc,
          phoneNumber: item.soDienThoai
        }))
      ]
      this.businessData = data;
      this.loadData();
    })
  }

  loadSupplier() {
    this.commonService.GetConfig('SUPPLIER_ELECTRIC_OPERATING_UNIT').subscribe((res: any) => {
      const data = [
        {
          id: "00000000-0000-0000-0000-000000000000", 
          text: "-- Chọn --"
        },
        ...res.items.listConfig.map((item: any) => ({
          id: item.categoryId,
          text: item.categoryName,
          typeCode: item.categoryTypeCode,
          code: item.categoryCode,
          priority: item.priority,
        }))
      ]
      this.supplierData = data;
      this.loadData();
    })
  }

  loadData() {
    this.apiLoaded++
    if (this.apiLoaded < 2) {
      return
    }
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
        this.editData = res.items[0] as ElectricOperatingUnitsModel;
        this.loadForm();
        if (this.type) {
          this.formGroup.disable();
        }
      });
      this.subscriptions.push(sb);
    }
  }

  clear() {
    const EmptyModel = {
      electricOperatingUnitsId: '00000000-0000-0000-0000-000000000000',
      customerName: '00000000-0000-0000-0000-000000000000',
      address: '',
      phoneNumber: '',
      presidentName: '',
      numOfGP: '',
      signDay: '',
      supplier: '00000000-0000-0000-0000-000000000000',
      isPowerGeneration: false,
      isPowerDistribution: false,
      isElectricityRetail: false,
      isConsulting: false,
      isSurveillance: false,
      status: 0,
    } as ElectricOperatingUnitsModel;
    this.editData = EmptyModel;
    return EmptyModel;
  }

  loadForm() {
    this.formGroup = this.fb.group({
      CustomerName: [this.editData.customerName],
      Address: [this.editData.address, Validators.required],
      PhoneNumber: [this.editData.phoneNumber, Validators.compose([Validators.minLength(10), Validators.maxLength(11)])],
      PresidentName: [this.editData.presidentName],
      NumOfGP: [this.editData.numOfGP, Validators.required],
      SignDay: [this.editData.signDay, Validators.required],
      Supplier: [this.editData.supplier],
      IsPowerGeneration: [this.editData.isPowerGeneration],
      IsPowerDistribution: [this.editData.isPowerDistribution],
      IsElectricityRetail: [this.editData.isElectricityRetail],
      IsConsulting: [this.editData.isConsulting],
      IsSurveillance: [this.editData.isSurveillance],
      Status: [this.editData.status]
    })

    this.show = true;

    const businessChange = this.formGroup.controls.CustomerName.valueChanges.subscribe((x: any) => {
      if (x != '00000000-0000-0000-0000-000000000000') {
        const data = this.businessData.find(y => y.id == x);
        if (data != null) {
          this.formGroup.controls.Address.setValue(data.address ?? "");
          this.formGroup.controls.Address.markAsTouched();
          this.formGroup.controls.PhoneNumber.setValue(data.phoneNumber ?? "");
          this.formGroup.controls.PhoneNumber.markAsTouched();
          this.formGroup.controls.PresidentName.setValue(data.manager ?? "");
          this.formGroup.controls.PresidentName.markAsTouched();
        }
      } else {
        this.formGroup.controls.Address.setValue("");
        this.formGroup.controls.Address.markAsTouched();
        this.formGroup.controls.PhoneNumber.setValue("");
        this.formGroup.controls.PhoneNumber.markAsTouched();
        this.formGroup.controls.PresidentName.setValue("");
        this.formGroup.controls.PresidentName.markAsTouched();
      }
    });
    this.subscriptions.push(businessChange);
  }

  private prepare() {
    const formData = this.formGroup.value;
    this.editData.customerName = formData.CustomerName;
    this.editData.address = formData.Address;
    this.editData.phoneNumber = formData.PhoneNumber;
    this.editData.presidentName = formData.PresidentName;
    this.editData.numOfGP = formData.NumOfGP;
    this.editData.signDay = formData.SignDay;
    this.editData.supplier = formData.Supplier;
    this.editData.isPowerGeneration = formData.IsPowerGeneration;
    this.editData.isPowerDistribution = formData.IsPowerDistribution;
    this.editData.isElectricityRetail = formData.IsElectricityRetail;
    this.editData.isConsulting = formData.IsConsulting;
    this.editData.isSurveillance = formData.IsSurveillance;
    this.editData.status = formData.Status;
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

  isCheckBox() {
    const IsPowerGeneration = this.formGroup.controls.IsPowerGeneration;
    const IsPowerDistribution = this.formGroup.controls.IsPowerDistribution;
    const IsElectricityRetail = this.formGroup.controls.IsElectricityRetail;
    const IsConsulting = this.formGroup.controls.IsConsulting;
    const IsSurveillance = this.formGroup.controls.IsSurveillance;
    if (!IsConsulting.value && !IsSurveillance.value && !IsElectricityRetail.value && !IsPowerGeneration.value && !IsPowerDistribution.value) {
      this.formGroup.setErrors({ noCheckBox: true })
      return this.formGroup.invalid && (IsConsulting.touched || IsSurveillance.touched || IsElectricityRetail.touched || IsPowerGeneration.touched || IsPowerDistribution.touched);
    } else {
      this.formGroup.setErrors(null)
      return false;
    }
  }

  prenventInputNonNumber(event: any) {
    if (event.which < 48 || event.which > 57) {
      event.preventDefault();
    }
  }

  // isCheckBox(): boolean {
  //   const { IsConsulting, IsSurveillance, IsElectricityRetail } = this.formGroup.controls;
  //   const atLeastOneChecked = [IsConsulting, IsSurveillance, IsElectricityRetail].some((c) => c.value);
  //   this.formGroup.setErrors(atLeastOneChecked ? null : { noCheckBox: true });
  //   return this.formGroup.invalid && (IsConsulting.touched || IsSurveillance.touched || IsElectricityRetail.touched);
  // }

  check_formGroup() {
    this.formGroup.markAllAsTouched();
    if (!this.formGroup.invalid) {
      this.save()
    }
  }
}
