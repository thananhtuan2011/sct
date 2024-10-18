import { result } from 'lodash';
import { ChangeDetectorRef, Component, Input, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal, NgbDateAdapter, NgbDateParserFormatter, NgbModal } from '@ng-bootstrap/ng-bootstrap';
import * as moment from 'moment';
import { of, Subscription, filter } from 'rxjs';
import { catchError, finalize, first, tap, startWith } from 'rxjs/operators';
import { CommonService } from 'src/app/_metronic/shared/services/common.service';
import Swal from 'sweetalert2';

import { ParticipateSupportFairModel } from '../../../_models/participate-support-fair.model';
// import { ParticipateSupportFairDetailModel } from '../../../_models/participate-support-fair-detail.model';
import { ParticipateSupportFairService } from '../../../_services/participate-support-fair-page.service';
import { AddEnterpriseInProvinceModalComponent } from '../edit-enterprises-in-province-modal/edit-modal.component';
import { AddEnterpriseOutsideProvinceModalComponent } from '../edit-enterprises-outside-province-modal/edit-modal.component';
import { Options } from 'select2';
import { CustomAdapter, CustomDateParserFormatter } from 'src/app/_metronic/shared/pipe/CustomNgbDate/CustomNgbDate';
import { data } from 'jquery';

const EMPTY_CUSTOM: ParticipateSupportFairModel = {
  id: '',
  participateSupportFairId: '00000000-0000-0000-0000-000000000000',
  participateSupportFairName: '',
  address: '',
  country: '00000000-0000-0000-0000-000000000000',
  scale: '',
  startTime: null,
  endTime: null,
  planJoin: 0,
  details: [],
  districtId: '00000000-0000-0000-0000-000000000000',
  communeId: '00000000-0000-0000-0000-000000000000',
  implementCost: 0,
};

@Component({
  selector: 'app-edit-modal',
  templateUrl: './edit-modal.component.html',
  styleUrls: ['./edit-modal.component.scss'],
})

export class EditParticipateSupportFairModalComponent implements OnInit, OnDestroy {
  @Input() id: any;
  @Input() type: any;
  isLoading$: any;
  participateSupportFairData: ParticipateSupportFairModel;
  formGroup: FormGroup;
  searchGroup: FormGroup;
  countryData: any;
  options: Options;
  detailData: any = [];
  test: any;
  planJoinData: any = [
    {
      id: 0,
      text: "-- Chọn --"
    },
    {
      id: 1,
      text: "Sở tham gia"
    },
    {
      id: 2,
      text: "Hỗ trợ doanh nghiệp tham gia"
    },
  ]
  
  dataDistrict: any = [];
  dataCommune:  any = [];
  dataCommuneFilter: any = [];
  districtId: string = "00000000-0000-0000-0000-000000000000";
  communeId: string = "00000000-0000-0000-0000-000000000000";

  private subscriptions: Subscription[] = [];
  public default_value = "00000000-0000-0000-0000-000000000000"

  constructor(
    private participateSupportFairService: ParticipateSupportFairService,
    private fb: FormBuilder, public modal: NgbActiveModal,
    public commonService: CommonService,
    private modalService: NgbModal,
    private changeDetectorRefs: ChangeDetectorRef,
  ) { }

  ngOnInit(): void {
    this.isLoading$ = this.participateSupportFairService.isLoading$;
    (async () => {
      this.loadcountry();
      this.loadDistrcit();
      this.loadCommune();
      await this.delay(150);
      this.loadParticipateSupportFair();
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

  loadcountry() {
    this.commonService.getListCountry().subscribe((res: any) => {
      const countries = [
        {
          id: '00000000-0000-0000-0000-000000000000',
          text: '-- Chọn / Không có --',
        },
      ]
      for (var item of res.items) {
        let obj_country = {
          id: item.countryId,
          text: item.countryName,
        }
        countries.push(obj_country)
      }
      this.countryData = countries.sort((i1, i2) => {
        if (i1.text > i2.text) {
          return 1;
        }
        if (i1.text < i2.text) {
          return -1;
        }
        return 0;
      });
    })
  }
  
  loadDistrcit(){
    this.commonService.getDistrict().subscribe((res: any) => {
      const distrcits = [
        {
          id: '00000000-0000-0000-0000-000000000000',
          text: '-- Chọn --',
        },
      ]
      for (var item of res.items) {
        let objDistrict = {
          id: item.districtId,
          text: item.districtName,
        }
        distrcits.push(objDistrict)
      }
      this.dataDistrict = distrcits.sort((i1, i2) => {
        if (i1.text > i2.text) {
          return 1;
        }
        if (i1.text < i2.text) {
          return -1;
        }
        return 0;
      });
    })
  }
  
  loadCommune(){
    this.commonService.getCommune().subscribe((res: any) => {
      const communes = [
        {
          id: '00000000-0000-0000-0000-000000000000',
          text: '-- Chọn --',
          districtId: '00000000-0000-0000-0000-000000000000'
        },
      ]
      for (let item of res.items) {
        let objCommune = {
          id: item.communeId,
          text: item.communeName,
          districtId: item.districtId
        }
        communes.push(objCommune)
      }
      this.dataCommuneFilter = communes
      this.dataCommune = communes.sort((i1, i2) => {
        if (i1.text > i2.text) {
          return 1;
        }
        if (i1.text < i2.text) {
          return -1;
        }
        return 0;
      });
    })
    
  }
  
  loadCommuneByDistrict(event: any){
    if (event != '00000000-0000-0000-0000-000000000000') {
      let result = this.dataCommune.filter((x: any) => x.id == '00000000-0000-0000-0000-000000000000' || x.districtId == event);
      this.dataCommuneFilter = result;
    }
    else {
      this.dataCommuneFilter = this.dataCommuneFilter;
    }
    this.formGroup.controls['CommuneId'].setValue('00000000-0000-0000-0000-000000000000')
  }

  loadParticipateSupportFair() {
    if (!this.id) {
      this.clear();
      this.loadForm();
    } else {
      const sb = this.participateSupportFairService.getItemById(this.id).pipe(
        first(),
        catchError((errorMessage) => {
          this.modal.dismiss(errorMessage);
          return of(EMPTY_CUSTOM);
        })
      ).subscribe((res: any) => {
        this.districtId = res.data.districtId
        this.participateSupportFairData = res.data;
        this.detailData = res.data.details;
        //this.loadCommuneByDistrict(this.participateSupportFairData.districtId);
        this.loadForm();
        if (this.type) {
          this.formGroup.disable();
          this.formGroup.controls['Search'].enable();
          this.formGroup.updateValueAndValidity();
        }
      });
      this.subscriptions.push(sb);
    }
  }

  loadForm() {
    let result = this.dataCommune.filter((x: any) => x.id == '00000000-0000-0000-0000-000000000000' || x.districtId == this.districtId);
    this.dataCommuneFilter = result;
    
    this.formGroup = this.fb.group({
      ParticipateSupportFairName: [this.participateSupportFairData.participateSupportFairName, Validators.required],
      Address: [this.participateSupportFairData.address],
      Country: [this.participateSupportFairData.country],
      Scale: [this.participateSupportFairData.scale, Validators.required],
      StartTime: [this.participateSupportFairData.startTime, Validators.required],
      EndTime: [this.participateSupportFairData.endTime],
      PlanJoin: [this.participateSupportFairData.planJoin],
      DistrictId: [this.participateSupportFairData.districtId, Validators.required],
      CommuneId: [this.participateSupportFairData.communeId, Validators.required],
      Search: [''],
      ImplementCost: [this.participateSupportFairData.implementCost, Validators.required]
    });
  }

  clear() {
    EMPTY_CUSTOM.participateSupportFairId = '00000000-0000-0000-0000-000000000000';
    EMPTY_CUSTOM.participateSupportFairName = '';
    EMPTY_CUSTOM.address = '';
    EMPTY_CUSTOM.country = '00000000-0000-0000-0000-000000000000';
    EMPTY_CUSTOM.scale = '';
    EMPTY_CUSTOM.startTime = null;
    EMPTY_CUSTOM.endTime = null;
    EMPTY_CUSTOM.planJoin = 0;
    EMPTY_CUSTOM.details = []
    EMPTY_CUSTOM.districtId = '00000000-0000-0000-0000-000000000000',
    EMPTY_CUSTOM.communeId = '00000000-0000-0000-0000-000000000000',
    EMPTY_CUSTOM.implementCost = 0,
    this.participateSupportFairData = EMPTY_CUSTOM;
  }

  private prepareParticipateSupportFair() {
    const formData = this.formGroup.value;
    this.participateSupportFairData.participateSupportFairName = formData.ParticipateSupportFairName;
    this.participateSupportFairData.address = formData.Address;
    this.participateSupportFairData.country = formData.Country;
    this.participateSupportFairData.scale = formData.Scale.toString();
    this.participateSupportFairData.startTime = this.convert_datetime(formData.StartTime);
    this.participateSupportFairData.endTime = this.convert_datetime(formData.EndTime);
    this.participateSupportFairData.planJoin = formData.PlanJoin;
    this.participateSupportFairData.details = this.detailData;
    this.participateSupportFairData.districtId = formData.DistrictId,
    this.participateSupportFairData.communeId = formData.CommuneId,
    this.participateSupportFairData.implementCost = formData.ImplementCost
  }

  //Date
  convert_date(string_date: string) {
    if (string_date) {
      var result = moment.utc(string_date, "DD/MM/YYYY");
      return result
    }
    return null
  }

  convert_date_string(string_date: string | null) {
    if (string_date) {
      var date = string_date.split("T")[0];
      var list = date.split("-"); //["year", "month", "day"]
      var result = list[2] + "/" + list[1] + "/" + list[0]
      return result
    }
    return null
  }

  convert_datetime(date: string){
    if (date) {
      if (date.includes("+07:00")) {
        const result = moment.utc(date)
        return result
      }
      else {
        const result = moment.utc(date + "+07:00")
        return result
      }
    }
    else {
      return null
    }
  }

  save() {
    this.prepareParticipateSupportFair();
    if (this.id) {
      this.edit();
    } else {
      this.create();
    }
  }

  edit() {
    const sbUpdate = this.participateSupportFairService.update(this.participateSupportFairData).pipe(
      tap(() => {
        this.modal.close();
      }),
      catchError((errorMessage) => {
        this.modal.dismiss(errorMessage);
        return of(this.participateSupportFairData);
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
    const sbCreate = this.participateSupportFairService.create(this.participateSupportFairData).pipe(
      tap(() => {
        this.modal.close();
      }),
      catchError((errorMessage) => {
        this.modal.dismiss(errorMessage);
        return of(this.participateSupportFairData);
      }),
    ).subscribe((res: any) => {
      Swal.fire({
        icon: res.status == 1 ? 'success' : 'error',
        title: res.status == 1 ? 'Thêm mới thành công' : 'Thêm mới thất bại',
        confirmButtonText: 'Xác nhận',
        text: 'Thêm mới ' + (res.status == 1 ? 'thành công' : 'thất bại'),
      });
      this.participateSupportFairData = EMPTY_CUSTOM
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
    if (value == '00000000-0000-0000-0000-000000000000' || value == 0 || value == null) {
      control.setErrors({ defaultvalue: true });
    }
    else {
      control.setErrors({ defaultvalue: null });
      control.updateValueAndValidity();
    }
    return control.invalid && (control.dirty || control.touched);
  }

  prenventInputNonNumber(event: any) {
    if (event.which < 48 || event.which > 57) {
      event.preventDefault();
    }
  }

  //Enterprise
  add_enterprise_in_province(id: any) {
    const modalRef = this.modalService.open(AddEnterpriseInProvinceModalComponent, { size: '100px' });
    modalRef.result.then(({...res}) =>
      res,
      (res) => {
        if (res) {
          this.add_detail(res)
        }
      }
    );
  }

  add_enterprise_outside_province(id: any) {
    const modalRef = this.modalService.open(AddEnterpriseOutsideProvinceModalComponent, { size: '100px' });
    modalRef.result.then(({...res}) =>
      res,
      (res) => {
        if (res) {
          this.add_detail(res)
        }
      }
    );
  }

  add_detail(data: any){
    if (this.detailData.findIndex((x: any) => x.businessNameVi === data.BusinessNameVi) === -1) {
      let obj_add = {
        participateSupportFairId : this.id == '' ? '00000000-0000-0000-0000-000000000000' : this.id,
        businessId : data.BusinessId,
        businessCode : data.BusinessCode,
        businessNameVi : data.BusinessNameVi,
        nganhNghe : data.NganhNghe,
        diaChi : data.DiaChi,
        nguoiDaiDien : data.NguoiDaiDien,
        huyen: data.Huyen,
        xa: data.Xa
      }
      this.detailData.push(obj_add);
    }
  }

  delete_detail(data: any){
    this.detailData = this.detailData.filter((x: any) => x.businessNameVi !== data.businessNameVi)
  }

  check_formGroup() {
    if (this.formGroup.invalid) {
      this.formGroup.markAllAsTouched();
    }
    else {
      this.save();
    }
  }
}
