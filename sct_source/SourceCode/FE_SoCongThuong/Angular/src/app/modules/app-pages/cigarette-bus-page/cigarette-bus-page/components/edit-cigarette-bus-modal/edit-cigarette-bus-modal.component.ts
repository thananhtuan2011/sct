import { ChangeDetectorRef, Component, Input, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal, NgbDateAdapter, NgbDateParserFormatter } from '@ng-bootstrap/ng-bootstrap';
import { of, Subscription } from 'rxjs';
import { catchError, finalize, first, tap } from 'rxjs/operators';
import { SelectOptionData } from 'src/app/_metronic/shared/components/select-custom/select-custom.interface';
import Swal from 'sweetalert2';
import { CigaretteBusinessModel, CigaretteBusinessDetailModel } from '../../../_models/cigarette-bus.model';
import { CigaretteBusinessPageService } from '../../../_services/cigarette-bus-page.service';
import { Options } from 'select2';
import { CommonService } from 'src/app/_metronic/shared/services/common.service';
import * as moment from 'moment';

const EMPTY_CUSTOM: CigaretteBusinessModel = {
  id: '',
  CigaretteBusinessId: '',
  cigaretteBusinessName: '00000000-0000-0000-0000-000000000000',
  cigaretteBusinessDetail: []
};

const EMPTY_DETAIL: CigaretteBusinessDetailModel = {
  TenDoanhNghiep: '',
  NguoiDaiDien: '',
  SoDienThoai: '',
  Huyen: '00000000-0000-0000-0000-000000000000',
  Xa: '00000000-0000-0000-0000-000000000000',
  DiaChi: '',
  GiayPhepKinhDoanh: '',
  NgayHetHan: null,
  DonViCungCap: '',
  DiaChiDonViCungCap: '',
  PhoneDonViCungCap: '',
  NgayCap: null,
  GhiChu: ''
}

@Component({
  selector: 'app-edit-cigarette-bus-modal.component',
  templateUrl: './edit-cigarette-bus-modal.component.html',
  styleUrls: ['./edit-cigarette-bus-modal.component.scss'],
})

export class EditCigaretteBusinessModalComponent implements OnInit, OnDestroy {
  @Input() id: any;
  isLoading$: any;
  cigaretteBusinessData: CigaretteBusinessModel;
  formGroup: FormGroup;
  public options: Options;
  dataSource: any[] = [];
  lstStore: any[] = [];
  displayedColumns: string[] = ['stt', 'name', 'action'];
  cigaretteBusinessDetail: CigaretteBusinessDetailModel;
  districtData: any = [];
  communeData: any = [];
  infoBusiness: any = {};
  communeDataByDistrictId: any = [
    {
      id: '00000000-0000-0000-0000-000000000000',
      text: '-- Chọn --',
    }
  ];
  private subscriptions: Subscription[] = [];
  public default_value = "00000000-0000-0000-0000-000000000000"
  public typeofbusinessData: any;
  public apiLoaded: number = 0;
  public show: boolean = false;

  constructor(
    private cigaretteBusinessService: CigaretteBusinessPageService,
    private fb: FormBuilder, public modal: NgbActiveModal,
    private changeDetectorRefs: ChangeDetectorRef,
    private commonService: CommonService
  ) { }

  ngOnInit(): void {
    this.isLoading$ = this.cigaretteBusinessService.isLoading$;
    this.loadDistrict();
    this.loadCommune();
    this.loadBusiness();
    this.options = {
      theme: 'bootstrap5',
      templateSelection: this.templateSelection,
    };
    this.changeDetectorRefs.detectChanges();

  }

  public templateSelection = (state: any): JQuery | string => {
    if (!state.id) {
      return state.text;
    }
    return jQuery('<span class="form-select form-select-solid form-select-lg">' + state.text + '</span>');
  }

  loadDistrict() {
    this.commonService.getDistrict().subscribe((res: any) => {
      var districts = [
        {
          id: '00000000-0000-0000-0000-000000000000',
          text: '-- Chọn --',
        },
      ];
      for (var item of res.items) {
        let obj_district = {
          id: item.districtId,
          text: item.districtName,
        }
        districts.push(obj_district)
      }
      this.districtData = districts.sort((a, b) => {
        if (a.text < b.text) {
          return -1;
        }
        if (a.text > b.text) {
          return 1;
        }
        return 0;
      });
      //this.changeDetectorRefs.detectChanges();
      this.loadDetail();
    })
  }

  loadCommune() {
    this.commonService.getCommune().subscribe((res: any) => {
      var communes = [
        {
          id: '00000000-0000-0000-0000-000000000000',
          text: '-- Chọn --',
        },
      ];
      for (var item of res.items) {
        let obj_commune = {
          id: item.communeId,
          text: item.communeName,
          districtId: item.districtId,
        };
        communes.push(obj_commune)
      }
      this.communeData = communes.sort((a, b) => {
        if (a < b) {
          return -1;
        }
        if (a > b) {
          return 1;
        }
        return 0;
      });;
      this.communeDataByDistrictId = this.communeData;
      //this.changeDetectorRefs.detectChanges();
      this.loadDetail();
    })
  }

  delay(ms: number) {
    return new Promise(resolve => setTimeout(resolve, ms));
  }

  loadDetail() {
    if (this.apiLoaded < 2) {
      this.apiLoaded++
      return
    }
    if (!this.id) {
      this.clear_model();
      this.loadForm();
    } else {
      const sb = this.cigaretteBusinessService.getItemById(this.id).pipe(
        first(),
        catchError((errorMessage) => {
          this.modal.dismiss(errorMessage);
          return of(EMPTY_CUSTOM);
        })
      ).subscribe((res: any) => {
        this.cigaretteBusinessData = res.items[0];
        this.dataSource = res.items[0].cigaretteBusinessDetail;
        this.infoBusiness = res.items[0];
        this.clear_modelDetail();;
        this.lstStore = res.items[0].cigaretteBusinessDetail;
        this.loadForm();
      });
      this.subscriptions.push(sb);
    }
  }

  loadForm() {
    this.formGroup = this.fb.group({
      cigaretteBusinessName: [this.cigaretteBusinessData.cigaretteBusinessName, Validators.compose([Validators.required])],
      TenDoanhNghiep: [this.cigaretteBusinessDetail.TenDoanhNghiep, Validators.compose([Validators.required])],
      NguoiDaiDien: [this.cigaretteBusinessDetail.NguoiDaiDien, Validators.compose([Validators.required])],
      SoDienThoai: [this.cigaretteBusinessDetail.SoDienThoai, Validators.compose([Validators.required])],
      Huyen: [this.cigaretteBusinessDetail.Huyen, Validators.compose([Validators.required])],
      Xa: [this.cigaretteBusinessDetail.Xa, Validators.compose([Validators.required])],
      DiaChi: [this.cigaretteBusinessDetail.DiaChi],
      GiayPhepKinhDoanh: [this.cigaretteBusinessDetail.GiayPhepKinhDoanh],
      NgayHetHan: [this.cigaretteBusinessDetail.NgayHetHan],
      DonViCungCap: [this.cigaretteBusinessDetail.DonViCungCap],
      GhiChu: [this.cigaretteBusinessDetail.GhiChu],
      NgayCap: [this.cigaretteBusinessDetail.NgayCap],
      PhoneDonViCungCap: [this.cigaretteBusinessDetail.PhoneDonViCungCap],
      DiaChiDonViCungCap: [this.cigaretteBusinessDetail.DiaChiDonViCungCap],
      NgayCapPhep: this.infoBusiness.ngayCap != undefined ?  this.convert_date_string(this.infoBusiness.ngayCap) : null,
      GiayDangKyKinhDoanh: this.infoBusiness.giayDangKyKinhDoanh
    });
    
    this.subscriptions.push(
      this.formGroup.controls.cigaretteBusinessName.valueChanges.subscribe((x) => {
        const find_data = this.typeofbusinessData.find((x: any) => x.id == this.formGroup.controls.cigaretteBusinessName.value)
        if (find_data.id !== "00000000-0000-0000-0000-000000000000") {
          this.formGroup.patchValue({
            "GiayDangKyKinhDoanh": find_data.giayPhepSanXuat,
            "NgayCapPhep": find_data.ngayCapPhep,
          }, { emitEvent: false })
        }
        else {
          this.formGroup.patchValue({
            "GiayDangKyKinhDoanh": '',
            "NgayCapPhep": '',
          }, { emitEvent: false })
        }
      })
    );
    
    this.show = true;
  }

  clear_model() {
    EMPTY_CUSTOM.CigaretteBusinessId = '';
    EMPTY_CUSTOM.cigaretteBusinessName = '00000000-0000-0000-0000-000000000000';
    EMPTY_CUSTOM.cigaretteBusinessDetail = [];
    EMPTY_DETAIL.TenDoanhNghiep = '';
    EMPTY_DETAIL.NguoiDaiDien = '';
    EMPTY_DETAIL.SoDienThoai = '';
    EMPTY_DETAIL.Huyen = '00000000-0000-0000-0000-000000000000';
    EMPTY_DETAIL.Xa = '00000000-0000-0000-0000-000000000000';
    EMPTY_DETAIL.DiaChi = '';
    EMPTY_DETAIL.GiayPhepKinhDoanh = '';
    EMPTY_DETAIL.NgayHetHan = null;
    EMPTY_DETAIL.DonViCungCap = '';
    EMPTY_DETAIL.PhoneDonViCungCap = '';
    EMPTY_DETAIL.NgayCap = null;
    EMPTY_DETAIL.DiaChiDonViCungCap = '';
    EMPTY_DETAIL.GhiChu = '';
    this.cigaretteBusinessDetail = EMPTY_DETAIL
    this.cigaretteBusinessData = EMPTY_CUSTOM
  }
  clear_modelDetail() {
    EMPTY_DETAIL.NguoiDaiDien = '';
    EMPTY_DETAIL.SoDienThoai = '';
    EMPTY_DETAIL.Huyen = '00000000-0000-0000-0000-000000000000';
    EMPTY_DETAIL.Xa = '00000000-0000-0000-0000-000000000000';
    EMPTY_DETAIL.DiaChi = '';
    EMPTY_DETAIL.GiayPhepKinhDoanh = '';
    EMPTY_DETAIL.NgayHetHan = null;
    EMPTY_DETAIL.DonViCungCap = '';
    EMPTY_DETAIL.DonViCungCap = '';
    EMPTY_DETAIL.PhoneDonViCungCap = '';
    EMPTY_DETAIL.NgayCap = null;
    EMPTY_DETAIL.DiaChiDonViCungCap = '';
    EMPTY_DETAIL.GhiChu = '';
    this.cigaretteBusinessDetail = EMPTY_DETAIL
  }

  save() {
    this.prepareTypeOfEnergy();
    if (this.id) {
      this.edit();
    } else {
      this.cigaretteBusinessData.CigaretteBusinessId = this.default_value
      this.create();
    }
  }

  edit() {
    const sbUpdate = this.cigaretteBusinessService.update(this.cigaretteBusinessData).pipe(
      tap(() => {
        this.modal.close();
      }),
      catchError((errorMessage) => {
        this.modal.dismiss(errorMessage);
        return of(this.cigaretteBusinessData);
      }),
    ).subscribe((res: any) => {
      Swal.fire({
        icon: res.status == 1 ? 'success' : 'error',
        title: res.status == 1 ? 'Chỉnh sửa thành công' : 'Chỉnh sửa thất bại',
        confirmButtonText: 'Xác nhận',
        text: res.status == 1 ? 'Chỉnh sửa thành công' : res.status == 0 ? res.error.msg : 'Chỉnh sửa thất bại',
      });
    });
    this.subscriptions.push(sbUpdate);
  }

  create() {
    const sbCreate = this.cigaretteBusinessService.create(this.cigaretteBusinessData).pipe(
      tap(() => {
        this.modal.close();
      }),
      catchError((errorMessage) => {
        this.modal.dismiss(errorMessage);
        return of(this.cigaretteBusinessData);
      }),
    ).subscribe((res: any) => {
      Swal.fire({
        icon: res.status == 1 ? 'success' : 'error',
        title: res.status == 1 ? 'Thêm mới thành công' : 'Thêm mới thất bại',
        confirmButtonText: 'Xác nhận',
        text: res.status == 1 ? 'Thêm mới thành công' : res.status == 0 ? res.error.msg : 'Thêm mới thất bại',
      });
    });
    this.subscriptions.push(sbCreate);
  }

  private prepareTypeOfEnergy() {
    const formData = this.formGroup.value;
    this.cigaretteBusinessData.cigaretteBusinessName = formData.cigaretteBusinessName;
    this.cigaretteBusinessData.cigaretteBusinessDetail = this.dataSource;
  }

  loadBusiness() {
    this.cigaretteBusinessService.loadBusinesses().subscribe(res_business => {
      const data_typeofbusiness = [{
        id: "00000000-0000-0000-0000-000000000000",
        text: '-- Chọn --'
      }]
      for (var business of res_business.items) {
        let obj_business = {
          id: business.businessId,
          text: business.businessNameVi,
          giayPhepSanXuat: business.giayPhepSanXuat,
          ngayCapPhep: this.convert_date_string(business.ngayCapPhep)
        }
        data_typeofbusiness.push(obj_business)
      }
      this.typeofbusinessData = data_typeofbusiness
      this.loadDetail();
    })
  }

  addStore() {
    // store=this.formGroup.value.Store;
    let tenDoanhNghiep = this.formGroup.value.TenDoanhNghiep;
    let nguoiDaiDien = this.formGroup.value.NguoiDaiDien;
    let soDienThoai = this.formGroup.value.SoDienThoai;
    let huyen = this.formGroup.value.Huyen;
    let xa = this.formGroup.value.Xa;
    let diaChi = this.formGroup.value.DiaChi;
    let giayPhepKinhDoanh = this.formGroup.value.GiayPhepKinhDoanh;
    let ngayHetHan = this.convert_date(this.formGroup.value.NgayHetHan);
    let donViCungCap = this.formGroup.value.DonViCungCap;
    let ngayCap = this.convert_date(this.formGroup.value.NgayCap);
    let phoneDonViCungCap = this.formGroup.value.PhoneDonViCungCap;
    let diaChiDonViCungCap = this.formGroup.value.DiaChiDonViCungCap;
    let ghiChu = this.formGroup.value.GhiChu;

    if (tenDoanhNghiep == "" || nguoiDaiDien == "" || soDienThoai == "" || huyen == "00000000-0000-0000-0000-000000000000" || xa == "00000000-0000-0000-0000-000000000000") {
      this.formGroup.markAllAsTouched();
      return;
    }
    let item = {
      tenDoanhNghiep,
      nguoiDaiDien,
      soDienThoai,
      huyen,
      xa,
      diaChi,
      giayPhepKinhDoanh,
      ngayHetHan,
      donViCungCap,
      ngayCap,
      phoneDonViCungCap,
      diaChiDonViCungCap,
      ghiChu
    }
    this.lstStore.push(item);
    this.dataSource = this.lstStore;
    this.formGroup.controls.TenDoanhNghiep.reset('')
    this.formGroup.controls.NguoiDaiDien.reset('')
    this.formGroup.controls.SoDienThoai.reset('')
    this.formGroup.controls.Huyen.reset("00000000-0000-0000-0000-000000000000");
    this.formGroup.controls.Xa.reset("00000000-0000-0000-0000-000000000000");
    this.formGroup.controls.DiaChi.reset("");
    this.formGroup.controls.GiayPhepKinhDoanh.reset("");
    this.formGroup.controls.DonViCungCap.reset("");
    this.formGroup.controls.PhoneDonViCungCap.reset("");
    this.formGroup.controls.DiaChiDonViCungCap.reset("");
    this.formGroup.controls.GhiChu.reset("");
    this.formGroup.controls.NgayCap.reset(null);
    this.formGroup.controls.NgayHetHan.reset(null);
    this.changeDetectorRefs.detectChanges();
  }

  delStore(item: any) {
    const index: number = this.lstStore.indexOf(item);
    this.lstStore.splice(index, 1);
    this.dataSource = this.lstStore;
  }

  delDetail(item: any, index: any) {
    this.dataSource.splice(index, 1);
  }

  isDefaultValue(controlName: any)//: boolean 
  {
    const control = this.formGroup.controls[controlName];
    const isdefaultvalue = (control.value == "00000000-0000-0000-0000-000000000000")
    if (isdefaultvalue) {
      control.setErrors({ default: true })
    }
    return control.invalid && (control.dirty || control.touched)
  }

  ngOnDestroy(): void {
    this.subscriptions.forEach(sb => sb.unsubscribe());
  }

  // helpers for View
  isControlValid(controlName: any): boolean {
    const control = this.formGroup.controls[controlName];
    return control.valid && (control.dirty || control.touched);
  }

  isControlInvalid(controlName: any): boolean {
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

  check_formGroup() {
    let checkFullForm = (this.formGroup.value.TenDoanhNghiep !== "" && this.formGroup.value.NguoiDaiDien !== "" && this.formGroup.value.SoDienThoai !== "" && this.formGroup.value.Huyen !== "00000000-0000-0000-0000-000000000000" && this.formGroup.value.Xa !== "00000000-0000-0000-0000-000000000000");
    let checkEmptyForm = (this.formGroup.value.TenDoanhNghiep == "" && this.formGroup.value.NguoiDaiDien == "" && this.formGroup.value.SoDienThoai == "" && this.formGroup.value.Huyen == "00000000-0000-0000-0000-000000000000" && this.formGroup.value.Xa == "00000000-0000-0000-0000-000000000000");

    if (this.formGroup.value.cigaretteBusinessName == '00000000-0000-0000-0000-000000000000') {
      this.isDefaultValue('cigaretteBusinessName')
    } else if (checkFullForm) {
      this.remindSaveData();
    }
    else if (this.dataSource.length == 0 || !checkEmptyForm) {
      this.formGroup.markAllAsTouched();
    }
    else {
      this.save();
    }
  }

  loadCommuneByDistrict(event: any) {
    if (event != '00000000-0000-0000-0000-000000000000') {
      var result = this.communeData.filter((x: { id: string; districtId: any; }) => x.id == '00000000-0000-0000-0000-000000000000' || x.districtId == event);
      this.communeDataByDistrictId = result;
    }
    else {
      this.communeDataByDistrictId = this.communeData;
    }
    this.formGroup.controls['Xa'].setValue('00000000-0000-0000-0000-000000000000')
  }

  convert_date(string_date: string) {
    var result = moment.utc(string_date, "DD/MM/YYYY");
    return result
  }

  convert_date_string(string_date: string) {
    var date = string_date.split("T")[0];
    var list = date.split("-"); //["year", "month", "day"]
    var result = list[2] + "/" + list[1] + "/" + list[0]
    return result
  }

  prenventInputNonNumber(event: any) {
    if (event.which < 48 || event.which > 57) {
      event.preventDefault();
    }
  }

  remindSaveData() {
    Swal.fire({
      title: 'Bạn chưa lưu thông tin đã nhập',
      text: 'Vui lòng lưu thông tin thương nhân buôn bán',
      icon: 'warning',
      showCancelButton: true,
      confirmButtonColor: '#3085d6',
      cancelButtonColor: '#d33',
      cancelButtonText: 'Thoát',
      confirmButtonText: 'Xác nhận'
    })
  }
}
