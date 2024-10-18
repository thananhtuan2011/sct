import { Component, Input, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { of, Subscription } from 'rxjs';
import { catchError, first, tap } from 'rxjs/operators';
import Swal from 'sweetalert2';
import { RegulationConformityAMModel } from '../../../_models/regulation-conformity-AM.model';
import { RegulationConformityAMPageService } from '../../../_services/regulation-conformity-AM.service';
import { Options } from 'select2';
import { CommonService } from 'src/app/_metronic/shared/services/common.service';

const EMPTY_CUSTOM: RegulationConformityAMModel = {
  id: '',
  regulationConformityAMId: '00000000-0000-0000-0000-000000000000',
  dayReception: '',
  establishmentId: '00000000-0000-0000-0000-000000000000',
  districtId: '00000000-0000-0000-0000-000000000000',
  address: '',
  phone: '',
  num: '',
  productName: '',
  content: '',
  dateOfPublication: '',
  note: ''
};

@Component({
  selector: 'app-edit-reg-conform-am-modal',
  templateUrl: './edit-modal.component.html',
  styleUrls: ['./edit-modal.component.scss'],

})
export class EditRegulationConformityAMModalComponent implements OnInit, OnDestroy {
  @Input() id: any;
  @Input() type: any;
  @Input() logs: any = [];
  isLoading$: any;
  editData: RegulationConformityAMModel;
  formGroup: FormGroup;
  options: Options;
  districtData: any[];
  apiLoaded: number = 0;

  private subscriptions: Subscription[] = [];
  public default_value = "00000000-0000-0000-0000-000000000000"
  businessData: any[];
  show: boolean = false;

  constructor(
    private regulationConformityAMService: RegulationConformityAMPageService,
    private commonService: CommonService,
    private fb: FormBuilder,
    public modal: NgbActiveModal,
  ) { }

  ngOnInit(): void {
    this.isLoading$ = this.regulationConformityAMService.isLoading$;
    this.loadBusiness();
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

  loadBusiness() {
    this.commonService.getBusiness().subscribe((res: any) => {
      const data = [
        { id: "00000000-0000-0000-0000-000000000000", text: "-- Chọn --" },
        ...res.items.map((item: any) => ({
          id: item.businessId,
          text: item.businessNameVi,
          address: item.diaChiTruSo,
          manager: item.giamDoc,
          phoneNumber: item.soDienThoai,
          districtId: item.districtId
        }))
      ]
      this.businessData = data;
      this.loadRegulationConformityAM();
    })
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
      this.loadRegulationConformityAM();
    })
  }

  clearmodel() {
    EMPTY_CUSTOM.regulationConformityAMId = '00000000-0000-0000-0000-000000000000';
    EMPTY_CUSTOM.dayReception = '';
    EMPTY_CUSTOM.establishmentId = '00000000-0000-0000-0000-000000000000';
    EMPTY_CUSTOM.districtId = '00000000-0000-0000-0000-000000000000';
    EMPTY_CUSTOM.address = '';
    EMPTY_CUSTOM.phone = '';
    EMPTY_CUSTOM.num = '';
    EMPTY_CUSTOM.productName = '';
    EMPTY_CUSTOM.content = '';
    EMPTY_CUSTOM.dateOfPublication = '';
    EMPTY_CUSTOM.note = '';
    this.editData = EMPTY_CUSTOM;
  }

  loadRegulationConformityAM() {
    this.apiLoaded++
    if (this.apiLoaded < 2) {
      return
    }
    if (!this.id) {
      this.clearmodel();
      this.loadForm();
    } else {
      const sb = this.regulationConformityAMService.getItemById(this.id).pipe(
        first(),
        catchError((errorMessage) => {
          this.modal.dismiss(errorMessage);
          return of(EMPTY_CUSTOM);
        })
      ).subscribe((res: any) => {
        this.editData = res.data;
        this.loadForm();
        if (this.type) {
          this.formGroup.disable();
          this.formGroup.updateValueAndValidity();
        }
      });
      this.subscriptions.push(sb);
    }
  }

  loadForm() {
    this.formGroup = this.fb.group({
      DayReception: [this.editData.dayReception, Validators.required],
      EstablishmentId: [this.editData.establishmentId],
      DistrictId: [this.editData.districtId],
      Address: [this.editData.address, Validators.required],
      Phone: [this.editData.phone, Validators.compose([Validators.required, Validators.minLength(10), Validators.maxLength(11), Validators.pattern("^0[0-9]{9,10}$")])],
      Num: [this.editData.num, Validators.required],
      ProductName: [this.editData.productName, Validators.required],
      Content: [this.editData.content, Validators.required],
      DateOfPublication: [this.editData.dateOfPublication, Validators.required],
      Note: [this.editData.note]
    });
    this.show = true;
    const businessChange = this.formGroup.controls.EstablishmentId.valueChanges.subscribe((x: any) => {
      if (x != '00000000-0000-0000-0000-000000000000') {
        const data = this.businessData.find(y => y.id == x);
        if (data != null) {
          this.formGroup.controls.Address.setValue(data.address ?? "");
          this.formGroup.controls.Address.markAsTouched();
          this.formGroup.controls.Phone.setValue(data.phoneNumber ?? "");
          this.formGroup.controls.Phone.markAsTouched();
          this.formGroup.controls.DistrictId.setValue(data.districtId ?? "");
          this.formGroup.controls.DistrictId.markAsTouched();
        }
      } else {
        this.formGroup.controls.Address.setValue("");
        this.formGroup.controls.Address.markAsTouched();
        this.formGroup.controls.Phone.setValue("");
        this.formGroup.controls.Phone.markAsTouched();
        this.formGroup.controls.DistrictId.setValue("");
        this.formGroup.controls.DistrictId.markAsTouched();
      }
    });
    this.subscriptions.push(businessChange);
  }

  private prepareRegulationConformityAM() {
    const formValue = this.formGroup.value;
    this.editData.dayReception = formValue.DayReception;
    this.editData.establishmentId = formValue.EstablishmentId;
    this.editData.districtId = formValue.DistrictId;
    this.editData.address = formValue.Address;
    this.editData.phone = formValue.Phone;
    this.editData.num = formValue.Num;
    this.editData.productName = formValue.ProductName;
    this.editData.content = formValue.Content;
    this.editData.dateOfPublication = formValue.DateOfPublication;
    this.editData.note = formValue.Note;
  }

  save() {
    this.prepareRegulationConformityAM();
    if (this.id) {
      this.edit();
    } else {
      this.create();
    }
  }

  edit() {
    const sbUpdate = this.regulationConformityAMService.update(this.editData).pipe(
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
    const sbCreate = this.regulationConformityAMService.create(this.editData).pipe(
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
    if (this.formGroup.invalid) {
      this.formGroup.markAllAsTouched();
    }
    else {
      this.save();
    }
  }

  prenventInputNonNumber(event: any) {
    if (event.which < 48 || event.which > 57) {
      event.preventDefault();
    }
  }
}
