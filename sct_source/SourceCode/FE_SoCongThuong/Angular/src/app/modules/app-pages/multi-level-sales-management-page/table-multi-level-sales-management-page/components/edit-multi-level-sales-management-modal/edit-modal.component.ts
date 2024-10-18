import { CommonService } from 'src/app/_metronic/shared/services/common.service';
import { ChangeDetectorRef, Component, Input, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal, NgbDateAdapter, NgbDateParserFormatter } from '@ng-bootstrap/ng-bootstrap';
import { of, Subscription } from 'rxjs';
import { catchError, finalize, first, tap } from 'rxjs/operators';
import { Options } from 'select2';
import Swal from 'sweetalert2';

import { MultiLevelSalesManagementModel } from '../../../_models/multi-level-sales-management.model';
import { MultiLevelSalesManagementPageService } from '../../../_services/multi-level-sales-management-page.service';
import * as moment from 'moment';
import { MatDatepicker } from '@angular/material/datepicker';

const EMPTY_CUSTOM: MultiLevelSalesManagementModel = {
  id: '',
  multiLevelSalesManagementId: '00000000-0000-0000-0000-000000000000',

  //Cơ sở hoạt động
  businessId: '00000000-0000-0000-0000-000000000000', //Doanh nghiệp
  startDate: null, //Ngày bắt đầu hoạt động
  yearReport: moment().year(),
  multiLevelSellingPlace: '', //Địa điểm hoạt động bán hàng đa cấp
  contactPersonName: '', //Người liên hệ
  contactPersonPhoneNumber: '', //Số điện thoại
  contactPersonAddress: '', //Địa chỉ người liên hệ

  //Kết quả hoạt động
  participants: null, //Số người tham gia bán hàng đa cấp
  newParticipants: null, //Số người tham gia bán hàng đa cấp phát sinh thêm
  terminations: null, //Số người tham gia bán hàng đa cấp kết thúc hợp đồng
  basicTrainings: null, //Số lượng đào tạo căn bản
  turnover: null, //Doanh thu bán hàng đa cấp trên địa bàn (Triệu đồng)
  commission: null, //Tổng hoa hồng, tiền thưởng, lợi ích kinh tế đã nhận (Triệu đồng)
  promotionalValue: null, //Giá trị khuyến mãi quy đổi thành tiền (Triệu đồng)
  taxDeduction: null, //Khấu trừ thuế thu nhập cá nhân
  buyBackGoods: null, //Mua lại hàng hoá từ người tham gia bán hàng đa cấp (Triệu đồng)
};

@Component({
  selector: 'app-edit-modal',
  templateUrl: './edit-modal.component.html',
  styleUrls: ['./edit-modal.component.scss'],

})
export class EditMultiLevelSalesManagementModalComponent implements OnInit, OnDestroy {
  @Input() id: any;
  @Input() type: any;
  @Input() listBusinessMultiLevel: any;
  @Input() viewInfo: any ;
  isLoading$: any;
  multiLevelSalesManagementData: MultiLevelSalesManagementModel;
  formGroup: FormGroup;
  options: Options;

  BusinessData: any = [];
  YearData: any = [];
  districtData: any = [];
  statusData: any = [];

  private subscriptions: Subscription[] = [];

  constructor(
    private multiLevelSalesManagementPageService: MultiLevelSalesManagementPageService,
    private commonService: CommonService,
    private fb: FormBuilder, public modal: NgbActiveModal,
  ) { }

  ngOnInit(): void {
    this.isLoading$ = this.multiLevelSalesManagementPageService.isLoading$;
    (async () => {
      this.getYearsList();
      this.loadBusiness();
      this.loadDistrict();
      this.loadStatus();
      await this.delay(150);
      this.loadMultiLevelSalesManagement();
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
    var result = moment.utc(string_date, "DD/MM/YYYY");
    return result
  }

  convert_date_string(string_date: string | null) {
    if(string_date == null){
      return null;
    }
    var date = string_date.split("T")[0];
    var list = date.split("-"); //["year", "month", "day"]
    var result = list[2] + "/" + list[1] + "/" + list[0]
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
    this.subscriptions.push(this.commonService.getBusiness().subscribe((res: any) => {
      const defaultOption = {
        id: '00000000-0000-0000-0000-000000000000',
        text: "-- Chọn --",
      }

      const business_data = [...res.items.map((item: any) => ({ id: item.businessId, text: item.businessNameVi }))];
      business_data.sort((i1, i2) => i1.text.localeCompare(i2.text));
      business_data.unshift(defaultOption);
      this.BusinessData = business_data;
    }))
  }
  
  loadDistrict() {
    this.subscriptions.push(this.commonService.getDistrict().subscribe((res: any) => {
      const defaultOption = {
        id: '00000000-0000-0000-0000-000000000000',
        text: "-- Chọn --",
      }

      const data = [...res.items.map((item: any) => ({ id: item.districtId, text: item.districtName }))];
      data.sort((i1, i2) => i1.text.localeCompare(i2.text));
      data.unshift(defaultOption);
      this.districtData = data;
    }))
  }
  
  loadStatus(){
    const sb = this.commonService.GetConfig('STATUS_BUSINESS_MULTI_LEVEL').subscribe((res: any) => {
      const data = [
        {
          id: '00000000-0000-0000-0000-000000000000',
          text: '-- Chọn --'
        },
        ... res.items.listConfig.map((item: any) => ({
          id: item.categoryId,
          text: item.categoryName
        }))
      ]
      
      this.statusData = data;
    })
    
    this.subscriptions.push(sb);
  }

  loadMultiLevelSalesManagement() {
    if (!this.id) {
      this.clear();
      this.loadForm();
    } else {
      const sb = this.multiLevelSalesManagementPageService.getItemById(this.id).pipe(
        first(),
        catchError((errorMessage) => {
          this.modal.dismiss(errorMessage);
          return of(EMPTY_CUSTOM);
        })
      ).subscribe((res: any) => {
        this.multiLevelSalesManagementData = res.items[0];
        this.loadForm();
        this.formGroup.controls.StartDate.setValue(this.convert_date_string(this.formGroup.controls.StartDate.value));
        this.formGroup.controls.Participants.setValue(this.f_currency(this.formGroup.controls.Participants.value));
        this.formGroup.controls.NewParticipants.setValue(this.f_currency(this.formGroup.controls.NewParticipants.value));
        this.formGroup.controls.Terminations.setValue(this.f_currency(this.formGroup.controls.Terminations.value));
        this.formGroup.controls.BasicTrainings.setValue(this.f_currency(this.formGroup.controls.BasicTrainings.value));
        this.formGroup.controls.Turnover.setValue(this.f_currency(this.formGroup.controls.Turnover.value));
        this.formGroup.controls.Commission.setValue(this.f_currency(this.formGroup.controls.Commission.value));
        this.formGroup.controls.PromotionalValue.setValue(this.f_currency(this.formGroup.controls.PromotionalValue.value));
        this.formGroup.controls.TaxDeduction.setValue(this.f_currency(this.formGroup.controls.TaxDeduction.value));
        this.formGroup.controls.BuyBackGoods.setValue(this.f_currency(this.formGroup.controls.BuyBackGoods.value));
        this.formGroup.updateValueAndValidity();
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
      StartDate: [this.multiLevelSalesManagementData.startDate, Validators.required],
      YearReport: [this.multiLevelSalesManagementData.yearReport, Validators.required],
      MultiLevelSellingPlace: [this.multiLevelSalesManagementData.multiLevelSellingPlace],
      ContactPersonName: [this.multiLevelSalesManagementData.contactPersonName],
      ContactPersonPhoneNumber: [this.multiLevelSalesManagementData.contactPersonPhoneNumber],
      ContactPersonAddress: [this.multiLevelSalesManagementData.contactPersonAddress],
      Participants: [this.multiLevelSalesManagementData.participants, Validators.required],
      NewParticipants: [this.multiLevelSalesManagementData.newParticipants, Validators.required],
      Terminations: [this.multiLevelSalesManagementData.terminations, Validators.required],
      BasicTrainings: [this.multiLevelSalesManagementData.basicTrainings, Validators.required],
      Turnover: [this.multiLevelSalesManagementData.turnover, Validators.required],
      Commission: [this.multiLevelSalesManagementData.commission, Validators.required],
      PromotionalValue: [this.multiLevelSalesManagementData.promotionalValue],
      TaxDeduction: [this.multiLevelSalesManagementData.taxDeduction],
      BuyBackGoods: [this.multiLevelSalesManagementData.buyBackGoods],
      NumCert: '',
      Goods: '',
      BusinessCode: '',
      DistrictName: '',
      StatusName: '',
      LocalConfirm: '',
      CertDate: null,
      CertExp: null,
      Note: ''
    });
    this.loadInfoBusiness();
    this.subscriptions.push(
      this.formGroup.controls.BusinessId.valueChanges.subscribe((x) => {
        this.loadInfoBusiness();
      })
    );
    this.subscriptions.push(
      this.formGroup.controls.Participants.valueChanges.subscribe((x) => {
        this.formGroup.patchValue({
          "Participants": this.f_currency(x)
        }, { emitEvent: false })
      })
    );
    this.subscriptions.push(
      this.formGroup.controls.NewParticipants.valueChanges.subscribe((x) => {
        this.formGroup.patchValue({
          "NewParticipants": this.f_currency(x)
        }, { emitEvent: false })
      })
    );
    this.subscriptions.push(
      this.formGroup.controls.Terminations.valueChanges.subscribe((x) => {
        this.formGroup.patchValue({
          "Terminations": this.f_currency(x)
        }, { emitEvent: false })
      })
    );
    this.subscriptions.push(
      this.formGroup.controls.BasicTrainings.valueChanges.subscribe((x) => {
        this.formGroup.patchValue({
          "BasicTrainings": this.f_currency(x)
        }, { emitEvent: false })
      })
    );
    this.subscriptions.push(
      this.formGroup.controls.Turnover.valueChanges.subscribe((x) => {
        this.formGroup.patchValue({
          "Turnover": this.f_currency(x)
        }, { emitEvent: false })
      })
    );
    this.subscriptions.push(
      this.formGroup.controls.Commission.valueChanges.subscribe((x) => {
        this.formGroup.patchValue({
          "Commission": this.f_currency(x)
        }, { emitEvent: false })
      })
    );
    this.subscriptions.push(
      this.formGroup.controls.PromotionalValue.valueChanges.subscribe((x) => {
        this.formGroup.patchValue({
          "PromotionalValue": this.f_currency(x)
        }, { emitEvent: false })
      })
    );
    this.subscriptions.push(
      this.formGroup.controls.TaxDeduction.valueChanges.subscribe((x) => {
        this.formGroup.patchValue({
          "TaxDeduction": this.f_currency(x)
        }, { emitEvent: false })
      })
    );
    this.subscriptions.push(
      this.formGroup.controls.BuyBackGoods.valueChanges.subscribe((x) => {
        this.formGroup.patchValue({
          "BuyBackGoods": this.f_currency(x)
        }, { emitEvent: false })
      })
    );
  }
  
  loadInfoBusiness(){
    const find_data = this.listBusinessMultiLevel.find((x: any) => x.id == this.formGroup.controls.BusinessId.value)
    if (find_data && find_data.id !== "00000000-0000-0000-0000-000000000000") {
      this.formGroup.patchValue({
        "StartDate": this.convert_date_string(find_data.startDate),
        "NumCert" : find_data.numCert,
        "Goods": find_data.goods,
        "ContactPersonName": find_data.contact,
        "ContactPersonPhoneNumber":find_data.phoneNumber,
        "ContactPersonAddress": find_data.contactAddress,
        "MultiLevelSellingPlace": find_data.address,
        "BusinessCode": find_data.businessCode,
        "DistrictName": find_data.districtName,
        "StatusName": find_data.statusName,
        "LocalConfirm": find_data.localConfirm,
        "CertDate": this.convert_date_string(find_data.certDate),
        "CertExp": this.convert_date_string(find_data.certExp),
        "Note": find_data.note
        
      }, { emitEvent: false })
    }
    else {
      this.formGroup.patchValue({
        "StartDate": null,
        "NumCert" : '',
        "Goods": '',
        "ContactPersonName": '',
        "ContactPersonPhoneNumber":'',
        "ContactPersonAddress": '',
        "MultiLevelSellingPlace": '',
        "BusinessCode": "",
        "DistrictName": "",
        "StatusName": "",
        "LocalConfirm": "",
        "CertDate": null,
        "CertExp": null,
        "Note": ""
      }, { emitEvent: false })
    }
  }

  clear() {
    EMPTY_CUSTOM.multiLevelSalesManagementId = '00000000-0000-0000-0000-000000000000',
      EMPTY_CUSTOM.businessId = '00000000-0000-0000-0000-000000000000',
      EMPTY_CUSTOM.startDate = null,
      EMPTY_CUSTOM.yearReport = moment().year(),
      EMPTY_CUSTOM.multiLevelSellingPlace = '',
      EMPTY_CUSTOM.contactPersonName = '',
      EMPTY_CUSTOM.contactPersonPhoneNumber = '',
      EMPTY_CUSTOM.contactPersonAddress = '',
      EMPTY_CUSTOM.participants = null,
      EMPTY_CUSTOM.newParticipants = null,
      EMPTY_CUSTOM.terminations = null,
      EMPTY_CUSTOM.basicTrainings = null,
      EMPTY_CUSTOM.turnover = null,
      EMPTY_CUSTOM.commission = null,
      EMPTY_CUSTOM.promotionalValue = null,
      EMPTY_CUSTOM.taxDeduction = null,
      EMPTY_CUSTOM.buyBackGoods = null,
      this.multiLevelSalesManagementData = EMPTY_CUSTOM;
  }

  private prepareData() {
    const formData = this.formGroup.value;
    this.multiLevelSalesManagementData.businessId = formData.BusinessId,
      this.multiLevelSalesManagementData.startDate = this.convert_date(formData.StartDate),
      this.multiLevelSalesManagementData.yearReport = formData.YearReport,
      this.multiLevelSalesManagementData.multiLevelSellingPlace = formData.MultiLevelSellingPlace,
      this.multiLevelSalesManagementData.contactPersonName = formData.ContactPersonName,
      this.multiLevelSalesManagementData.contactPersonPhoneNumber = formData.ContactPersonPhoneNumber,
      this.multiLevelSalesManagementData.contactPersonAddress = formData.ContactPersonAddress,
      this.multiLevelSalesManagementData.participants = Number(formData.Participants.replaceAll(',', '')),
      this.multiLevelSalesManagementData.newParticipants = Number(formData.NewParticipants.replaceAll(',', '')),
      this.multiLevelSalesManagementData.terminations = Number(formData.Terminations.replaceAll(',', '')),
      this.multiLevelSalesManagementData.basicTrainings = Number(formData.BasicTrainings.replaceAll(',', '')),
      this.multiLevelSalesManagementData.turnover = Number(formData.Turnover.replaceAll(',', '')),
      this.multiLevelSalesManagementData.commission = Number(formData.Commission.replaceAll(',', '')),
      this.multiLevelSalesManagementData.promotionalValue = formData.PromotionalValue ? Number(formData.PromotionalValue.replaceAll(',', '')) : null,
      this.multiLevelSalesManagementData.taxDeduction = formData.TaxDeduction ? Number(formData.TaxDeduction.replaceAll(',', '')) : null,
      this.multiLevelSalesManagementData.buyBackGoods = formData.BuyBackGoods ? Number(formData.BuyBackGoods.replaceAll(',', '')) : null
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
    const sbUpdate = this.multiLevelSalesManagementPageService.update(this.multiLevelSalesManagementData).pipe(
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
    const sbCreate = this.multiLevelSalesManagementPageService.create(this.multiLevelSalesManagementData).pipe(
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
    console.log(normalizedMonthAndYear, datepicker)
    // const ctrlValue = this.date.value!;
    // ctrlValue.month(normalizedMonthAndYear.month());
    // ctrlValue.year(normalizedMonthAndYear.year());
    // this.date.setValue(ctrlValue);
  }
}
