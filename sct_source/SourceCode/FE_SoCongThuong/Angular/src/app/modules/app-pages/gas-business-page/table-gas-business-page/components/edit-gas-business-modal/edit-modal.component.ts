import { Component, Input, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import {
  NgbActiveModal,
  NgbDateAdapter,
  NgbDateParserFormatter,
} from '@ng-bootstrap/ng-bootstrap';
import { of, Subscription } from 'rxjs';
import { catchError, finalize, first, tap } from 'rxjs/operators';
import Swal from 'sweetalert2';
import { GasBusinessModel } from '../../../_models/gas-business-page.model';
import { GasBusinessPageService } from '../../../_services/gas-business-page.service';
import { Options } from 'select2';
import * as moment from 'moment';
import { CommonService } from 'src/app/_metronic/shared/services/common.service';

const EMPTY_CUSTOM: GasBusinessModel = {
  id: '',
  gasBusinessId: '00000000-0000-0000-0000-000000000000',
  typeBusiness: 0,
  businessId: '00000000-0000-0000-0000-000000000000',
  businessName: '',
  foreignTransactionName: '',
  gasBusiness: '00000000-0000-0000-0000-000000000000',
  address: '',
  phoneNumber: '',
  fax: '',
  businessCertificate: '',
  licensors: '',
  licenseDate: null,
  taxId: '',
  numDoc: '',
  dateEnd: null,
  dateStart: null,
  complianceStatus: '00000000-0000-0000-0000-000000000000',
};

@Component({
  selector: 'app-edit-commit-finance-plan-modal',
  templateUrl: './edit-modal.component.html',
  styleUrls: ['./edit-modal.component.scss'],
})
export class EditGasBusinessModalComponent implements OnInit, OnDestroy {
  @Input() id: any;
  @Input() itemData: any;
  @Input() type: any;
  @Input() complianceStatusData: any;
  @Input() districtData: any;

  isLoading$: any;
  data: GasBusinessModel;
  formGroup: FormGroup;
  dataCommitGroup: any = [];
  private subscriptions: Subscription[] = [];
  dataCreator: any = [];
  options: Options;
  businessData: any = [];
  gasBusinessData: any = [];
  typeBusiness: number = 0;
  show: boolean = false;
  apiLoaded: number = 0;

  constructor(
    private gasBusiness: GasBusinessPageService,
    private fb: FormBuilder,
    public modal: NgbActiveModal,
    private commonService: CommonService
  ) {}

  ngOnInit(): void {
    this.isLoading$ = this.gasBusiness.isLoading$;
    this.loadBusiness();
    this.loadGasBusiness();
    this.options = {
      theme: 'bootstrap5',
      templateSelection: this.templateSelection,
    };
  }

  public templateSelection = (state: any): JQuery | string => {
    if (!state.id) {
      return state.text;
    }
    return jQuery(
      '<span class="form-select form-select-solid form-select-lg">' +
        state.text +
        '</span>'
    );
  };

  loadForm() {
    if (this.apiLoaded < 1) {
      this.apiLoaded++;
      return;
    }
    if (!this.id) {
      this.clearModel();
    } else {
      this.data = this.itemData;
      this.typeBusiness = this.data.typeBusiness;
    }
    this.formGroup = this.fb.group({
      BusinessName: [this.data.businessId],
      TypeBusiness: [this.data.typeBusiness, Validators.required],
      ForeignTransactionName: [this.data.foreignTransactionName],
      GasBusiness: this.data.gasBusiness,
      Address: [this.data.address, Validators.required],
      BusinessCertificate: [this.data.businessCertificate],
      LicenseDate: [
        this.convert_date_string(this.data.licenseDate),
        Validators.required,
      ],
      Licensors: [this.data.licensors, Validators.required],
      PhoneNumber: this.data.phoneNumber,
      Fax: this.data.fax,
      TaxId: this.data.taxId,
      NumDoc: this.data.numDoc,
      DateStart: this.convert_date_string(this.data.dateStart),
      DateEnd: this.convert_date_string(this.data.dateEnd),
      ComplianceStatus: this.data.complianceStatus,
      District: '',
    });
    this.loadAdressBusiness();
    this.subscriptions.push(
      this.formGroup.controls.BusinessName.valueChanges.subscribe((x) => {
        this.loadAdressBusiness();
      })
    );
    this.show = true;
  }

  loadAdressBusiness() {
    const business = this.businessData.find(
      (x: any) => x.id === this.formGroup.controls.BusinessName.value
    );
    if (
      business &&
      business.districtId !== '00000000-0000-0000-0000-000000000000'
    ) {
      const districtName = this.districtData.find(
        (x: any) => x.id === business.districtId
      );
      this.formGroup.patchValue(
        {
          District: districtName.text,
        },
        { emitEvent: false }
      );
    } else {
      this.formGroup.patchValue(
        {
          District: '',
        },
        { emitEvent: false }
      );
    }
  }

  save() {
    this.prepareData();
    if (this.id) {
      this.edit();
    } else {
      this.create();
    }
  }

  edit() {
    const sbUpdate = this.gasBusiness
      .update(this.data)
      .pipe(
        tap(() => {
          this.modal.close();
        }),
        catchError((errorMessage) => {
          this.modal.dismiss(errorMessage);
          return of(this.data);
        })
      )
      .subscribe((res: any) => {
        Swal.fire({
          icon: res.status == 1 ? 'success' : 'error',
          title:
            res.status == 1 ? 'Chỉnh sửa thành công' : 'Chỉnh sửa thất bại',
          confirmButtonText: 'Xác nhận',
          text:
            res.status == 1
              ? 'Chỉnh sửa thành công'
              : res.status == 0
              ? res.error.msg
              : 'Chỉnh sửa thất bại',
        });
      });
    this.subscriptions.push(sbUpdate);
  }

  create() {
    const sbCreate = this.gasBusiness
      .create(this.data)
      .pipe(
        tap(() => {
          this.modal.close();
        }),
        catchError((errorMessage) => {
          this.modal.dismiss(errorMessage);
          return of(this.data);
        })
      )
      .subscribe((res: any) => {
        Swal.fire({
          icon: res.status == 1 ? 'success' : 'error',
          title: res.status == 1 ? 'Thêm mới thành công' : 'Thêm mới thất bại',
          confirmButtonText: 'Xác nhận',
          text:
            res.status == 1
              ? 'Thêm mới thành công'
              : res.status == 0
              ? res.error.msg
              : 'Thêm mới thất bại',
        });
        this.data = EMPTY_CUSTOM;
      });
    this.subscriptions.push(sbCreate);
  }

  private prepareData() {
    const formData = this.formGroup.value;
    this.data.typeBusiness = formData.TypeBusiness;
    this.data.businessId = formData.BusinessName;
    this.data.foreignTransactionName = formData.ForeignTransactionName;
    this.data.gasBusiness = formData.GasBusiness;
    this.data.address = formData.Address;
    this.data.phoneNumber = formData.PhoneNumber;
    this.data.fax = formData.Fax;
    this.data.businessCertificate = formData.BusinessCertificate;
    this.data.licensors = formData.Licensors;
    this.data.licenseDate = this.convert_date(formData.LicenseDate);
    this.data.taxId = formData.TaxId;
    this.data.numDoc = formData.NumDoc;
    this.data.dateEnd =
      formData.DateEnd !== null ? this.convert_date(formData.DateEnd) : null;
    this.data.dateStart =
      formData.DateStart !== null
        ? this.convert_date(formData.DateStart)
        : null;
    this.data.complianceStatus = formData.ComplianceStatus;
  }

  ngOnDestroy(): void {
    this.subscriptions.forEach((sb) => sb.unsubscribe());
  }

  // helpers for View
  isControlValid(controlName: string): boolean {
    const control = this.formGroup.controls[controlName];
    if (control.value > 0) {
      return control.valid && (control.dirty || control.touched);
    }
    return control.valid && (control.dirty || control.touched);
  }

  isControlInvalid(controlName: string): boolean {
    const control = this.formGroup.controls[controlName];
    if (control.value <= 0) {
      return control.invalid && (control.dirty || control.touched);
    }
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

  isDefaultValue(controlName: any): boolean {
    const control = this.formGroup.controls[controlName];
    if (
      control.value == '' ||
      control.value == null ||
      control.value == '00000000-0000-0000-0000-000000000000'
    ) {
      control.setErrors({ default: true });
    } else {
      control.setErrors(null);
    }
    return control.hasError('default') && (control.dirty || control.touched);
  }

  check_formGroup() {
    if (this.formGroup.invalid) {
      this.formGroup.markAllAsTouched();
      this.formGroup.updateValueAndValidity();
    } else {
      this.save();
    }
  }

  clearModel() {
    EMPTY_CUSTOM.id = '';
    EMPTY_CUSTOM.gasBusinessId = '00000000-0000-0000-0000-000000000000';
    EMPTY_CUSTOM.typeBusiness = 0;
    EMPTY_CUSTOM.businessName = '';
    EMPTY_CUSTOM.businessId = '00000000-0000-0000-0000-000000000000';
    EMPTY_CUSTOM.foreignTransactionName = '';
    EMPTY_CUSTOM.gasBusiness = '00000000-0000-0000-0000-000000000000';
    EMPTY_CUSTOM.address = '';
    EMPTY_CUSTOM.phoneNumber = '';
    EMPTY_CUSTOM.fax = '';
    EMPTY_CUSTOM.businessCertificate = '';
    EMPTY_CUSTOM.licensors = '';
    EMPTY_CUSTOM.licenseDate = null;
    EMPTY_CUSTOM.taxId = '';
    EMPTY_CUSTOM.numDoc = '';
    EMPTY_CUSTOM.dateEnd = null;
    EMPTY_CUSTOM.dateStart = null;
    EMPTY_CUSTOM.complianceStatus = '00000000-0000-0000-0000-000000000000';
    this.data = EMPTY_CUSTOM;
  }

  convert_date(string_date: string) {
    var result = moment.utc(string_date, 'DD/MM/YYYY');
    return result;
  }

  convert_date_string(string_date: string | null) {
    if (string_date == null) {
      return null;
    }
    let date = string_date.split('T')[0];
    let list = date.split('-'); //["year", "month", "day"]
    let result = list[2] + '/' + list[1] + '/' + list[0];
    return result;
  }

  loadBusiness() {
    this.commonService.getBusiness().subscribe((res: any) => {
      const businesses = [
        {
          id: '00000000-0000-0000-0000-000000000000',
          text: '-- Chọn --',
          districtId: '00000000-0000-0000-0000-000000000000',
        },
      ];
      for (var item of res.items) {
        let obj_business = {
          id: item.businessId,
          text: item.businessNameVi,
          address: item.diaChiTruSo,
          phoneNumber: item.soDienThoai,
          nguoiDaiDien: item.nguoiDaiDien,
          giayPhepSanXuat: item.giayPhepSanXuat,
          ngayCapPhep: item.ngayCapPhep,
          businessCode: item.businessCode,
          maSoThue: item.maSoThue,
          nameEn: item.businessNameEn,
          districtId: item.districtId,
        };
        businesses.push(obj_business);
      }
      this.businessData = businesses;
      this.loadForm();
    });
  }

  loadGasBusiness() {
    this.gasBusiness.getAllGasBusiness().subscribe((res: any) => {
      const gasBusiness = [
        {
          id: '00000000-0000-0000-0000-000000000000',
          text: '-- Chọn --',
        },
      ];
      for (var item of res.items) {
        let obj_business = {
          id: item.gasId,
          text: item.name,
        };
        gasBusiness.push(obj_business);
      }
      this.gasBusinessData = gasBusiness;
      this.loadForm();
    });
  }

  changeValue(value: any) {
    const _dataValue = this.businessData.filter((x: any) => x.id == value);
    this.formGroup.controls.Address.setValue(_dataValue[0].address);
    this.formGroup.controls.BusinessCertificate.setValue(
      _dataValue[0].giayPhepSanXuat
    );
    this.formGroup.controls.LicenseDate.setValue(
      this.convert_date_string(_dataValue[0].ngayCapPhep)
    );
    this.formGroup.controls.PhoneNumber.setValue(_dataValue[0].phoneNumber);
    this.formGroup.controls.TaxId.setValue(_dataValue[0].maSoThue);
    this.formGroup.controls.ForeignTransactionName.setValue(
      _dataValue[0].nameEn
    );
  }

  changeTyeBusiness() {
    this.typeBusiness = this.formGroup.controls.TypeBusiness.value;
  }

  prenventInputNonNumber(event: any) {
    if (event.which < 48 || event.which > 57) {
      event.preventDefault();
    }
  }
}
