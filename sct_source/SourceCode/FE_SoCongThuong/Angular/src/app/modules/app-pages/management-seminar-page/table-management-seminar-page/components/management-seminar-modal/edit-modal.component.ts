import { result } from 'lodash';
import { ChangeDetectorRef, Component, Input, OnDestroy, OnInit } from '@angular/core';
import { FormArray, FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal, NgbDateAdapter, NgbDateParserFormatter, NgbModal } from '@ng-bootstrap/ng-bootstrap';
import * as moment from 'moment';
import { of, Subscription, filter } from 'rxjs';
import { catchError, finalize, first, tap, startWith } from 'rxjs/operators';
import { CommonService } from 'src/app/_metronic/shared/services/common.service';
import Swal from 'sweetalert2';

import { ManagementSeminarModel } from '../../../_models/management-seminar.model';
// import { ManagementSeminarDetailModel } from '../../../_models/management-seminar-detail.model';
import { ManagementSeminarService } from '../../../_services/management-seminar-page.service';
import { Options } from 'select2';
import { CustomAdapter, CustomDateParserFormatter } from 'src/app/_metronic/shared/pipe/CustomNgbDate/CustomNgbDate';
import { data } from 'jquery';

const EMPTY_CUSTOM: ManagementSeminarModel = {
  id: '',
  profileCode: '',
  managemenetSeminarId : '00000000-0000-0000-0000-000000000000',
  businessId : '00000000-0000-0000-0000-000000000000',
  title : "",
  districtId: '00000000-0000-0000-0000-000000000000',
  address: "",
  contact: "",
  phoneNumber: "",
  numberParticipant: 0,
  note: "",
  listTime:  [],
};

@Component({
  selector: 'app-edit-modal',
  templateUrl: './edit-modal.component.html',
  styleUrls: ['./edit-modal.component.scss'],
})

export class EditManagementSeminarModalComponent implements OnInit, OnDestroy {
  @Input() id: any;
  @Input() type: any;
  @Input() dataDistrict: any;
  isLoading$: any;
  managementSeminarData: ManagementSeminarModel;
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
  
  businessData: any = [];
  dataCommune:  any = [];
  dataCommuneFilter: any = [];
  districtId: string = "00000000-0000-0000-0000-000000000000";
  communeId: string = "00000000-0000-0000-0000-000000000000";

  private subscriptions: Subscription[] = [];
  public default_value = "00000000-0000-0000-0000-000000000000"

  constructor(
    private managementSeminarService: ManagementSeminarService,
    private fb: FormBuilder, public modal: NgbActiveModal,
    public commonService: CommonService,
    private modalService: NgbModal,
    private changeDetectorRefs: ChangeDetectorRef,
  ) { }

  ngOnInit(): void {
    this.isLoading$ = this.managementSeminarService.isLoading$;
    (async () => {
      this.loadBusiness();
      await this.delay(150);
      this.loadManagementSeminar();
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
    
  loadManagementSeminar() {
    if (!this.id) {
      this.clear();
      this.loadForm();
    } else {
      const sb = this.managementSeminarService.getItemById(this.id).pipe(
        first(),
        catchError((errorMessage) => {
          this.modal.dismiss(errorMessage);
          return of(EMPTY_CUSTOM);
        })
      ).subscribe((res: any) => {
        
        this.districtId = res.data.districtId
        this.detailData = res.data.details;
        this.managementSeminarData = res.data;
        
        //this.loadCommuneByDistrict(this.managementSeminarData.districtId);
        this.loadForm();
        if(this.managementSeminarData.listTime.length > 0){
          const listData = this.managementSeminarData.listTime;
          for(let i = 0 ; i < listData.length ; i++){
            this.timeLine().push(this.fb.group({
              StartTime: [listData[i].startTime, Validators.required], 
              EndTime: [listData[i].endTime, Validators.required]
            }));
          }
          this.delTime(0)
        }
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
    this.formGroup = this.fb.group({
      ProfileCode: [this.managementSeminarData.profileCode, Validators.required],
      BusinessId: [this.managementSeminarData.businessId],
      Title: [this.managementSeminarData.title, Validators.required],
      DistrictId: this.managementSeminarData.districtId,
      Address: [this.managementSeminarData.address],
      Contact: this.managementSeminarData.contact,
      PhoneNumber: this.managementSeminarData.phoneNumber,
      NumberParticipant: this.managementSeminarData.numberParticipant == 0 ? "" : this.managementSeminarData.numberParticipant ,
      Note: this.managementSeminarData.note,
      listTime: this.fb.array([ this.fb.group({
        StartTime: [null, Validators.required], 
        EndTime: [null, Validators.required],
      })])
    });
  }

  clear() {
    EMPTY_CUSTOM.profileCode = '';
    EMPTY_CUSTOM.managemenetSeminarId  = '00000000-0000-0000-0000-000000000000';
    EMPTY_CUSTOM.businessId  = '00000000-0000-0000-0000-000000000000';
    EMPTY_CUSTOM.title  = "";
    EMPTY_CUSTOM.districtId = '00000000-0000-0000-0000-000000000000';
    EMPTY_CUSTOM.address = "";
    EMPTY_CUSTOM.contact = "";
    EMPTY_CUSTOM.phoneNumber = "";
    EMPTY_CUSTOM.numberParticipant = 0;
    EMPTY_CUSTOM.note = "";
    EMPTY_CUSTOM.listTime =  [];
    this.managementSeminarData = EMPTY_CUSTOM;
  }

  private prepareManagementSeminar() {
    const formData = this.formGroup.value;
    this.managementSeminarData.profileCode = formData.ProfileCode;
    this.managementSeminarData.businessId = formData.BusinessId;
    this.managementSeminarData.title = formData.Title;
    this.managementSeminarData.districtId = formData.DistrictId;
    this.managementSeminarData.address = formData.Address;
    this.managementSeminarData.contact = formData.Contact;
    this.managementSeminarData.phoneNumber = formData.PhoneNumber;
    this.managementSeminarData.numberParticipant = formData.NumberParticipant == "" ? 0 : formData.NumberParticipant;
    this.managementSeminarData.note = formData.Note;
    this.managementSeminarData.listTime =  formData.listTime;
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
    this.prepareManagementSeminar();
    if (this.id) {
      this.edit();
    } else {
      this.create();
    }
  }

  edit() {
    const sbUpdate = this.managementSeminarService.update(this.managementSeminarData).pipe(
      tap(() => {
        this.modal.close();
      }),
      catchError((errorMessage) => {
        this.modal.dismiss(errorMessage);
        return of(this.managementSeminarData);
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
    const sbCreate = this.managementSeminarService.create(this.managementSeminarData).pipe(
      tap(() => {
        this.modal.close();
      }),
      catchError((errorMessage) => {
        this.modal.dismiss(errorMessage);
        return of(this.managementSeminarData);
      }),
    ).subscribe((res: any) => {
      Swal.fire({
        icon: res.status == 1 ? 'success' : 'error',
        title: res.status == 1 ? 'Thêm mới thành công' : 'Thêm mới thất bại',
        confirmButtonText: 'Xác nhận',
        text: 'Thêm mới ' + (res.status == 1 ? 'thành công' : 'thất bại'),
      });
      this.managementSeminarData = EMPTY_CUSTOM
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

  add_detail(data: any){
    if (this.detailData.findIndex((x: any) => x.businessNameVi === data.BusinessNameVi) === -1) {
      let obj_add = {
        managementSeminarId : this.id == '' ? '00000000-0000-0000-0000-000000000000' : this.id,
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
  
  loadBusiness(){
    const sb = this.commonService.getBusiness().subscribe((res: any) => {
      const data = [
        {
          id: '00000000-0000-0000-0000-000000000000',
          text: '-- Chọn --'
        },
        ...res.items.map((item: any) => ({
          id: item.businessId,
          text: item.businessNameVi
        }))
      ];
      this.businessData = data;
    })
    this.subscriptions.push(sb);
  }
  
  timeLine(){
    return this.formGroup.get('listTime') as FormArray;
  }
  
  delTime(index: number){
    if(this.timeLine().length <= 1){
      return
    }
    this.timeLine().removeAt(index);
  }
  
  addTime() {
    this.timeLine().push(this.fb.group({
      StartTime: [null, Validators.required], 
      EndTime: [null, Validators.required],
    }));
  }
  
  arrayControlHasError(validation: any, controlName: any, index: any): boolean {
    const control = this.timeLine().controls[index].get(controlName);
    if (control) {
      return control.hasError(validation) && (control.dirty || control.touched);
    }
    else {
      return false
    }
  }
}
