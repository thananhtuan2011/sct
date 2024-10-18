import { CommonService } from 'src/app/_metronic/shared/services/common.service';
import { ChangeDetectorRef, Component, Input, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal, NgbDateAdapter, NgbDateParserFormatter } from '@ng-bootstrap/ng-bootstrap';
import { of, Subscription } from 'rxjs';
import { catchError, finalize, first, tap } from 'rxjs/operators';
import { Options } from 'select2';
import Swal from 'sweetalert2';

import { BusinessMultiLevelModel } from '../../../_models/business-multi-level.model';
import { BusinessMultiLevelPageService } from '../../../_services/business-multi-level-page.service';
import * as moment from 'moment';
import { MatDatepicker } from '@angular/material/datepicker';

const EMPTY_CUSTOM: BusinessMultiLevelModel = {
  id: '',
  businessMultiLevelId: '00000000-0000-0000-0000-000000000000',
  businessId: '00000000-0000-0000-0000-000000000000',
  districtId: '00000000-0000-0000-0000-000000000000',
  address: '',
  startDate: null,
  status: '00000000-0000-0000-0000-000000000000',
  numCert: '',
  certDate: null,
  certExp: null,
  contact: '',
  phoneNumber: '',
  addressContact: '',
  goods: '',
  localConfirm: '',
  note: ''
};

@Component({
  selector: 'app-edit-modal',
  templateUrl: './edit-modal.component.html',
  styleUrls: ['./edit-modal.component.scss'],

})
export class EditBusinessMultiLevelModalComponent implements OnInit, OnDestroy {
  @Input() id: any;
  @Input() type: any;
  @Input() districtData: any;
  @Input() statusData: any;
  isLoading$: any;
  multiLevelSalesManagementData: BusinessMultiLevelModel;
  formGroup: FormGroup;
  options: Options;

  BusinessData: any = [];
  YearData: any = [];

  private subscriptions: Subscription[] = [];

  constructor(
    private businessMultiLevelPageService: BusinessMultiLevelPageService,
    private commonService: CommonService,
    private fb: FormBuilder, public modal: NgbActiveModal,
  ) { }

  ngOnInit(): void {
    this.isLoading$ = this.businessMultiLevelPageService.isLoading$;
    (async () => {
      this.getYearsList();
      this.loadBusiness();
      await this.delay(150);
      this.loadBusinessMultiLevel();
    })();

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
    return new Promise(resolve => setTimeout(resolve, ms));
  }

  prenventInputNonNumber(event: any) {
    if (event.which < 48 || event.which > 57) {
      event.preventDefault();
    }
  }

  //Date
  convert_date(string_date: string) {
    let result = moment.utc(string_date, "DD/MM/YYYY");
    return result
  }

  convert_date_string(string_date: string|null) {
    if(string_date == null){
      return null;
    }
    let date = string_date.split("T")[0];
    let list = date.split("-"); //["year", "month", "day"]
    let result = list[2] + "/" + list[1] + "/" + list[0]
    return result
  }

  //Ô số
  f_currency(value: any, args?: any): any {
    if (value) {
      let nbr = Number((value + '').replace(/,|-/g, ""));
      const result = (nbr + '').replace(/(\d)(?=(\d{3})+(?!\d))/g, "$1,");
      return result
    }
    return null
  }

  getYearsList() {
    const currentYear = new Date().getFullYear();
    const yearsList: any = [];
  
    for (let i = -10; i <= 10; i++) {
      const year = currentYear + i;
      yearsList.push({id: year, text: year});
    }
  
    this.YearData = yearsList;
  }

  loadBusiness() {
    this.commonService.getBusiness().subscribe((res: any) => {
      const defaultOption = {
        id: '00000000-0000-0000-0000-000000000000',
        text: "-- Chọn --",
        businessCode: ''
      }

      const business_data = [...res.items.map((item: any) => ({ id: item.businessId, text: item.businessNameVi, businessCode: item.businessCode }))];
      business_data.sort((i1, i2) => i1.text.localeCompare(i2.text));
      business_data.unshift(defaultOption);
      this.BusinessData = business_data;
    })
  }

  loadBusinessMultiLevel() {
    if (!this.id) {
      this.clear();
      this.loadForm();
    } else {
      const sb = this.businessMultiLevelPageService.getItemById(this.id).pipe(
        first(),
        catchError((errorMessage) => {
          this.modal.dismiss(errorMessage);
          return of(EMPTY_CUSTOM);
        })
      ).subscribe((res: any) => {
        this.multiLevelSalesManagementData = res.items[0];
        this.loadForm();
        if (this.type == "view") {
          this.formGroup.disable();
          this.formGroup.updateValueAndValidity();
        }
      });
      this.subscriptions.push(sb);
    }
  }

  loadForm() {
    this.formGroup = this.fb.group({
      BusinessId: [this.multiLevelSalesManagementData.businessId],
      Contact: this.multiLevelSalesManagementData.contact,
      BusinessCode: '',
      PhoneNumber: this.multiLevelSalesManagementData.phoneNumber,
      DistrictId: [this.multiLevelSalesManagementData.districtId],
      Address: this.multiLevelSalesManagementData.address,
      AddressContact: this.multiLevelSalesManagementData.addressContact,
      StartDate: [this.convert_date_string(this.multiLevelSalesManagementData.startDate), Validators.required],
      Status: this.multiLevelSalesManagementData.status,
      Goods: this.multiLevelSalesManagementData.goods,
      NumCert: [this.multiLevelSalesManagementData.numCert, Validators.required],
      LocalConfirm: this.multiLevelSalesManagementData.localConfirm,
      CertDate: [this.convert_date_string(this.multiLevelSalesManagementData.certDate), Validators.required],
      CertExp: [this.convert_date_string(this.multiLevelSalesManagementData.certExp), Validators.required],
      Note: this.multiLevelSalesManagementData.note
    });
    this.loadBusinessCode();
    this.subscriptions.push(
      this.formGroup.controls.BusinessId.valueChanges.subscribe((x: any) => {
       this.loadBusinessCode();
      })
    );
    
  }
  
  loadBusinessCode(){
    const find_data = this.BusinessData.find((x: any) => x.id == this.formGroup.controls.BusinessId.value)
    if (find_data && find_data.id !== "00000000-0000-0000-0000-000000000000") {
      this.formGroup.patchValue({
        "BusinessCode": find_data.businessCode,
      }, { emitEvent: false })
    }
    else {
      this.formGroup.patchValue({
        "BusinessCode": '',
      }, { emitEvent: false })
    }
  }

  clear() {
    EMPTY_CUSTOM.id =  '';
    EMPTY_CUSTOM.businessMultiLevelId =  '00000000-0000-0000-0000-000000000000';
    EMPTY_CUSTOM.businessId =  '00000000-0000-0000-0000-000000000000';
    EMPTY_CUSTOM.districtId =  '00000000-0000-0000-0000-000000000000';
    EMPTY_CUSTOM.address =  '';
    EMPTY_CUSTOM.startDate =  null;
    EMPTY_CUSTOM.status =  '00000000-0000-0000-0000-000000000000';
    EMPTY_CUSTOM.numCert =  '';
    EMPTY_CUSTOM.certDate =  null;
    EMPTY_CUSTOM.certExp =  null;
    EMPTY_CUSTOM.contact =  '';
    EMPTY_CUSTOM.phoneNumber =  '';
    EMPTY_CUSTOM.addressContact =  '';
    EMPTY_CUSTOM.goods =  '';
    EMPTY_CUSTOM.localConfirm =  '';
    EMPTY_CUSTOM.note = '';
    this.multiLevelSalesManagementData = EMPTY_CUSTOM;
  }

  private prepareData() {
    const formData = this.formGroup.value;
    this.multiLevelSalesManagementData.businessId = formData.BusinessId;
    this.multiLevelSalesManagementData.districtId =  formData.DistrictId;
    this.multiLevelSalesManagementData.address =  formData.Address;
    this.multiLevelSalesManagementData.startDate = this.convert_date(formData.StartDate);
    this.multiLevelSalesManagementData.status =  formData.Status;
    this.multiLevelSalesManagementData.numCert =  formData.NumCert;
    this.multiLevelSalesManagementData.certDate =  this.convert_date(formData.CertDate);
    this.multiLevelSalesManagementData.certExp =  this.convert_date(formData.CertExp);
    this.multiLevelSalesManagementData.contact =  formData.Contact;
    this.multiLevelSalesManagementData.phoneNumber =  formData.PhoneNumber;
    this.multiLevelSalesManagementData.addressContact =  formData.AddressContact;
    this.multiLevelSalesManagementData.goods =  formData.Goods;
    this.multiLevelSalesManagementData.localConfirm =  formData.LocalConfirm;
    this.multiLevelSalesManagementData.note = formData.Note;
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
    const sbUpdate = this.businessMultiLevelPageService.update(this.multiLevelSalesManagementData).pipe(
      tap(() => {
        this.modal.close();
      }),
      catchError((errorMessage) => {
        this.modal.dismiss(errorMessage);
        return of(this.multiLevelSalesManagementData);
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
    const sbCreate = this.businessMultiLevelPageService.create(this.multiLevelSalesManagementData).pipe(
      tap(() => {
        this.modal.close();
      }),
      catchError((errorMessage) => {
        this.modal.dismiss(errorMessage);
        return of(this.multiLevelSalesManagementData);
      }),
    ).subscribe((res: any) => {
      Swal.fire({
        icon: res.status == 1 ? 'success' : 'error',
        title: res.status == 1 ? 'Thêm mới thành công' : 'Thêm mới thất bại',
        confirmButtonText: 'Xác nhận',
        text: res.status == 0 ? res.error.msg : 'Thêm mới ' + (res.status == 1 ? 'thành công' : 'thất bại'),
      });
      this.multiLevelSalesManagementData = EMPTY_CUSTOM
    });
    this.subscriptions.push(sbCreate);
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
    if (value == '00000000-0000-0000-0000-000000000000' || value == 0) {
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
    }
    else {
      this.save();
    }
  }

  openYearPicker(datepicker: MatDatepicker<moment.Moment>) {
    datepicker.viewChanged;
    datepicker.open();
  }

  selectedYear(normalizedMonthAndYear: moment.Moment, datepicker: MatDatepicker<moment.Moment>) {
    datepicker.close();
  }
}
