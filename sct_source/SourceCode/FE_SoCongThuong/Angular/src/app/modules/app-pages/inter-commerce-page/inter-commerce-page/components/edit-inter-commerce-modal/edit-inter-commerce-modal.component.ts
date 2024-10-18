import { ChangeDetectorRef, Component, Input, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal, NgbDateAdapter, NgbDateParserFormatter } from '@ng-bootstrap/ng-bootstrap';
import { of, Subscription } from 'rxjs';
import { catchError, finalize, first, tap } from 'rxjs/operators';
import { SelectOptionData } from 'src/app/_metronic/shared/components/select-custom/select-custom.interface';
import Swal from 'sweetalert2';
import { Options } from 'select2';
import { InterCommercePageService } from '../../../_services/inter-commerce-page.service';
import { InterCommerceModel } from '../../../_models/inter-commerce.model';
import { CommonService } from 'src/app/_metronic/shared/services/common.service';
import * as moment from 'moment';

const EMPTY_CUSTOM: InterCommerceModel = {
  id: '',
  InternationalCommerceId: '',
  internationalCommerceName: '00000000-0000-0000-0000-000000000000',
  investorName: '',
  licensingActivity: '',
  address: '',
  diaChiDoanhNghiep: '',
  giayDangKyKinhDoanh: '',
  ngayCapPhep: null,
  nguoiDaiDien: '',
  soDienThoai: '',
  tenCoSoBanLe: '',
  diaChiCoSoBanLe: '',
  giayPhepKinhDoanh: '',
  ngayCapGiayPhepKinhDoanh: null,
  giayPhepBanLe: '',
  ngayCapGiayPhepBanLe: null,
  ngayHetHanGiayPhepBanLe: null,
  dienTichSuDung: 0,
  dienTichSan: 0,
  dienTichBanHang: 0,
  dienTichKinhDoanh: 0,
  ghiChu: '',
  loaiHinhCoSo: '00000000-0000-0000-0000-000000000000'
};

@Component({
  selector: 'app-edit-inter-commerce-modal.component',
  templateUrl: './edit-inter-commerce-modal.component.html',
  styleUrls: ['./edit-inter-commerce-modal.component.scss'],

})
export class EditInterCommerceModalComponent implements OnInit, OnDestroy {
  @Input() id: any;
  @Input() type: any;
  isLoading$: any;
  internationalCommerceData: InterCommerceModel;
  formGroup: FormGroup;
  public options: Options;
  dataSource: any[] = [];
  typeMarketData : any;
  lstStore: any[] = [];
  displayedColumns: string[] = ['stt', 'name', 'action'];

  private subscriptions: Subscription[] = [];
  public default_value = "00000000-0000-0000-0000-000000000000"
  public typeofbusinessData: any;

  constructor(
    private InternationalCommerceService: InterCommercePageService,
    private fb: FormBuilder, public modal: NgbActiveModal,
    private changeDetectorRefs: ChangeDetectorRef,
    private commonService: CommonService
  ) { }

  ngOnInit(): void {
    this.isLoading$ = this.InternationalCommerceService.isLoading$;
    (async () => {
      this.loadBusiness();
      this.loadTypeMarket();
      await this.delay(500)
      this.loadDetail();
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

  loadDetail() {

    if (!this.id) {
      this.clear_model();
      this.loadForm();
    } else {
      const sb = this.InternationalCommerceService.getItemById(this.id).pipe(
        first(),
        catchError((errorMessage) => {
          this.modal.dismiss(errorMessage);
          return of(EMPTY_CUSTOM);
        })
      ).subscribe((res: any) => {
        this.internationalCommerceData = res.items[0];
        this.loadForm();
      });
      this.subscriptions.push(sb);
    }
  }

  loadForm() {
    this.formGroup = this.fb.group({
      InternationalCommerceName: [this.internationalCommerceData.internationalCommerceName, Validators.compose([Validators.required])],
      InvestorName: [this.internationalCommerceData.investorName],
      Address: [this.internationalCommerceData.address],
      LicensingActivity: [this.internationalCommerceData.licensingActivity],
      DiaChiDoanhNghiep: this.internationalCommerceData.diaChiDoanhNghiep,
      GiayDangKyKinhDoanh: this.internationalCommerceData.giayDangKyKinhDoanh,
      NguoiDaiDien: this.internationalCommerceData.nguoiDaiDien,
      SoDienThoai: this.internationalCommerceData.soDienThoai,
      TenCoSoBanLe: [this.internationalCommerceData.tenCoSoBanLe, Validators.required],
      DiaChiCoSoBanLe: [this.internationalCommerceData.diaChiCoSoBanLe, Validators.required],
      GiayPhepKinhDoanh: [this.internationalCommerceData.giayPhepKinhDoanh, Validators.required],
      NgayCapGiayPhepKinhDoanh: [this.convert_date_string(this.internationalCommerceData.ngayCapGiayPhepKinhDoanh), Validators.required],
      GiayPhepBanLe: [this.internationalCommerceData.giayPhepBanLe, Validators.required],
      NgayCapGiayPhepBanLe: [this.convert_date_string(this.internationalCommerceData.ngayCapGiayPhepBanLe), Validators.required],
      NgayHetHanGiayPhepBanLe: [this.convert_date_string(this.internationalCommerceData.ngayHetHanGiayPhepBanLe), Validators.required ],
      DienTichSuDung: this.internationalCommerceData.dienTichSuDung === 0 ? '' : this.internationalCommerceData.dienTichSuDung ,
      DienTichSan: this.internationalCommerceData.dienTichSan === 0 ? '' : this.internationalCommerceData.dienTichSan,
      DienTichBanHang: this.internationalCommerceData.dienTichBanHang === 0 ? '' : this.internationalCommerceData.dienTichBanHang,
      DienTichKinhDoanh: this.internationalCommerceData.dienTichKinhDoanh === 0 ? '' : this.internationalCommerceData.dienTichKinhDoanh,
      GhiChu: this.internationalCommerceData.ghiChu,
      LoaiHinhCoSo: this.internationalCommerceData.loaiHinhCoSo,
      NgayCapPhep: this.internationalCommerceData.ngayCapPhep
    });
    this.getInfoBusiness();
    this.subscriptions.push(
      this.formGroup.controls.InternationalCommerceName.valueChanges.subscribe(
        (x) => {
          this.getInfoBusiness();
        }
      )
    );
    if(this.type){
      this.formGroup.disable();
    }
  }
  
  getInfoBusiness(){
    const find_data = this.typeofbusinessData.find(
      (x: any) =>
        x.id == this.formGroup.controls.InternationalCommerceName.value
    );
    if (find_data.id !== '00000000-0000-0000-0000-000000000000') {
        this.formGroup.patchValue(
          {
            DiaChiDoanhNghiep: find_data.diaChi,
            GiayDangKyKinhDoanh: find_data.giayDangKyKinhDoanh,
            NgayCapPhep: this.convert_date_string(
              find_data.ngayCapPhep
            ),
            NguoiDaiDien: find_data.nguoiDaiDienPhapLuat,
            SoDienThoai: find_data.soDienThoai,
          },
          { emitEvent: false }
        );
    } else{
      this.formGroup.patchValue(
        {
          DiaChiDoanhNghiep: '',
          GiayDangKyKinhDoanh: '',
          NgayCapPhep: null,
          NguoiDaiDien: '',
          SoDienThoai: '',
        },
        { emitEvent: false }
      );
    }
  }
  clear_model() {
    EMPTY_CUSTOM.InternationalCommerceId = '';
      EMPTY_CUSTOM.internationalCommerceName = '00000000-0000-0000-0000-000000000000';
      EMPTY_CUSTOM.address = '';
      EMPTY_CUSTOM.licensingActivity = '';
      EMPTY_CUSTOM.diaChiDoanhNghiep = '';
      EMPTY_CUSTOM.giayDangKyKinhDoanh = '';
      EMPTY_CUSTOM.ngayCapPhep = null;
      EMPTY_CUSTOM.nguoiDaiDien = '';
      EMPTY_CUSTOM.soDienThoai = '';
      EMPTY_CUSTOM.tenCoSoBanLe = '';
      EMPTY_CUSTOM.diaChiCoSoBanLe = '';
      EMPTY_CUSTOM.giayPhepKinhDoanh = '';
      EMPTY_CUSTOM.ngayCapGiayPhepKinhDoanh = null;
      EMPTY_CUSTOM.giayPhepBanLe = '';
      EMPTY_CUSTOM.ngayCapGiayPhepBanLe = null;
      EMPTY_CUSTOM.ngayHetHanGiayPhepBanLe = null;
      EMPTY_CUSTOM.dienTichSuDung = 0;
      EMPTY_CUSTOM.dienTichSan = 0;
      EMPTY_CUSTOM.dienTichBanHang = 0;
      EMPTY_CUSTOM.dienTichKinhDoanh = 0;
      EMPTY_CUSTOM.ghiChu = '';
      EMPTY_CUSTOM.loaiHinhCoSo = '00000000-0000-0000-0000-000000000000';
      this.internationalCommerceData = EMPTY_CUSTOM;
  }
  save() {
    this.prepareTypeOfEnergy();
    if (this.internationalCommerceData.InternationalCommerceId != '') {
      this.edit();
    } else {
      this.internationalCommerceData.InternationalCommerceId = this.default_value
      this.create();
    }
  }

  edit() {
    const sbUpdate = this.InternationalCommerceService.update(this.internationalCommerceData).pipe(
      tap(() => {
        this.modal.close();
      }),
      catchError((errorMessage) => {
        this.modal.dismiss(errorMessage);
        return of(this.internationalCommerceData);
      }),
    ).subscribe((res: any) => {
      Swal.fire({
        icon: res.status == 1 ? 'success' : 'error',
        title: res.status == 1 ? 'Chỉnh sửa thành công' : 'Chỉnh sửa thất bại',
        confirmButtonText: 'Xác nhận',
        text: 'Chỉnh sửa ' + (res.status == 1 ? 'thành công' : 'thất bại'),
      });
    //  this.internationalCommerceData = EMPTY_CUSTOM
    });
    this.subscriptions.push(sbUpdate);
  }

  create() {
    const sbCreate = this.InternationalCommerceService.create(this.internationalCommerceData).pipe(
      tap(() => {
        this.modal.close();
      }),
      catchError((errorMessage) => {
        this.modal.dismiss(errorMessage);
        return of(this.internationalCommerceData);
      }),
    ).subscribe((res: any) => {
      Swal.fire({
        icon: res.status == 1 ? 'success' : 'error',
        title: res.status == 1 ? 'Thêm mới thành công' : 'Thêm mới thất bại',
        confirmButtonText: 'Xác nhận',
        text: 'Thêm mới ' + (res.status == 1 ? 'thành công' : 'thất bại'),
      });
      this.internationalCommerceData = EMPTY_CUSTOM
    });
    this.subscriptions.push(sbCreate);
    EMPTY_CUSTOM.InternationalCommerceId = '';
    EMPTY_CUSTOM.internationalCommerceName = '';
    this.internationalCommerceData = EMPTY_CUSTOM;
  }

  private prepareTypeOfEnergy() {

    const formData = this.formGroup.value;
    this.internationalCommerceData.internationalCommerceName = formData.InternationalCommerceName;
    this.internationalCommerceData.investorName = formData.InvestorName;
    this.internationalCommerceData.address = formData.Address;
    this.internationalCommerceData.licensingActivity = formData.LicensingActivity;
    
    this.internationalCommerceData.tenCoSoBanLe = formData.TenCoSoBanLe;
    this.internationalCommerceData.diaChiCoSoBanLe = formData.DiaChiCoSoBanLe;
    this.internationalCommerceData.giayPhepKinhDoanh = formData.GiayPhepKinhDoanh;
    this.internationalCommerceData.ngayCapGiayPhepKinhDoanh = this.convert_date(formData.NgayCapGiayPhepKinhDoanh);
    this.internationalCommerceData.giayPhepBanLe = formData.GiayPhepBanLe;
    this.internationalCommerceData.ngayCapGiayPhepBanLe = this.convert_date(formData.NgayCapGiayPhepBanLe);
    this.internationalCommerceData.ngayHetHanGiayPhepBanLe = this.convert_date(formData.NgayHetHanGiayPhepBanLe);
    this.internationalCommerceData.dienTichSuDung = formData.DienTichSuDung;
    this.internationalCommerceData.dienTichSan = formData.DienTichSan;
    this.internationalCommerceData.dienTichBanHang = formData.DienTichBanHang;
    this.internationalCommerceData.dienTichKinhDoanh = formData.DienTichKinhDoanh;
    this.internationalCommerceData.ghiChu = formData.GhiChu;
    this.internationalCommerceData.loaiHinhCoSo = formData.LoaiHinhCoSo;
  }
  loadBusiness() {
    this.InternationalCommerceService.loadBusinesses().subscribe(res_business => {
      const data_typeofbusiness = [{
        id: "00000000-0000-0000-0000-000000000000",
        text: '-- Chọn --'
      }]
      for (var business of res_business.items) {
        let obj_business = {
          id: business.businessId,
          text: business.businessNameVi,
          diaChi: business.diaChiTruSo,
          giayDangKyKinhDoanh: business.giayPhepSanXuat,
          ngayCapPhep: business.ngayCapPhep,
          nguoiDaiDienPhapLuat: business.nguoiDaiDien,
          soDienThoai: business.soDienThoai
        }
        data_typeofbusiness.push(obj_business)
      }
      this.typeofbusinessData = data_typeofbusiness
    })
    return this.typeofbusinessData
  }
  addStore() {
    var store = "";
    store = this.formGroup.value.Store;
    if (store == "") {
      return;
    }
    this.lstStore.push(store);
    this.dataSource = this.lstStore;
    this.formGroup.controls.Store.setValue("");
    this.formGroup.controls.Store.clearValidators();
    this.formGroup.controls.Store.updateValueAndValidity();
    this.changeDetectorRefs.detectChanges();

  }
  delStore(item: any) {
    const index: number = this.lstStore.indexOf(item);
    this.lstStore.splice(index, 1);
    this.dataSource = this.lstStore;
  }


  isDefaultValue(controlName: any)//: boolean 
  {
    const control = this.formGroup.controls[controlName];
    const isdefaultvalue = (control.value == "00000000-0000-0000-0000-000000000000");
    if (isdefaultvalue) {
      control.setErrors({ default: true });
    }
    // control.updateValueAndValidity();
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
   // const check
    // if (this.formGroup.controls.InternationalCommerceName.value == "00000000-0000-0000-0000-000000000000") {
    //   this.formGroup.controls.InternationalCommerceName.setErrors({ default: true });
    //   this.formGroup.markAllAsTouched();
    //   this.formGroup.updateValueAndValidity();
    // }
    if (this.formGroup.invalid) {
      this.formGroup.markAllAsTouched();
    }
    else {
      this.save()
    }
  }
  
  loadTypeMarket(){
    const sb = this.commonService.GetConfig('RETAIL_FORM').subscribe((res: any) => {
      const data = [
        {          
          id: '00000000-0000-0000-0000-000000000000',
          text: '-- Chọn --'
        },
        ...res.items.listConfig.map((item: any ) => ({
          id: item.categoryId,
          text: item.categoryName
        }))
      ]
      this.typeMarketData = data;
    })
    this.subscriptions.push(sb)
  }
  
  convert_date_string(string_date: string | null) {
    if (string_date === null) {
      return null;
    }
    var date = string_date.split('T')[0];
    var list = date.split('-'); //["year", "month", "day"]
    var result = list[2] + '/' + list[1] + '/' + list[0];
    return result;
  }
  
  convert_date(string_date: string | null) {
    if (string_date === null) {
      return null;
    }
    var result = moment.utc(string_date, 'DD/MM/YYYY');
    return result;
  }
}
