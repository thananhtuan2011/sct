import {
  ChangeDetectorRef,
  Component,
  Input,
  OnDestroy,
  OnInit,
} from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { of, Subscription } from 'rxjs';
import { catchError, first, tap } from 'rxjs/operators';
import Swal from 'sweetalert2';
import {
  AlcoholBusinessModel,
  AlcoholBusinessDetailModel,
} from '../../../_models/alcohol-bus.model';
import { AlcoholBusinessPageService } from '../../../_services/alcohol-bus-page.service';
import { Options } from 'select2';
import { CommonService } from 'src/app/_metronic/shared/services/common.service';
import * as moment from 'moment';

const EMPTY_CUSTOM: AlcoholBusinessModel = {
  id: '',
  alcoholBusinessId: '',
  alcoholBusinessName: '00000000-0000-0000-0000-000000000000',
  alcoholBusinessDetail: [],
  giayDangKyKinhDoanh: '',
  ngayCapPhep: null,
  giayPhepBanBuon: '',
  ngayCapGiayPhepBanBuon: null,
  ngayHetHanGiayPhepBanBuon: null,
};

const EMPTY_DETAIL: AlcoholBusinessDetailModel = {
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
  SoDienThoaiDonViCungCap: '',
  NgayCapGiayPhepBanLe: null,
  GhiChu: '',
};

@Component({
  selector: 'app-edit-alcohol-bus-modal.component',
  templateUrl: './edit-alcohol-bus-modal.component.html',
  styleUrls: ['./edit-alcohol-bus-modal.component.scss'],
})
export class EditAlcoholBusinessModalComponent implements OnInit, OnDestroy {
  @Input() id: any;
  @Input() type: any;
  isLoading$: any;
  alcoholBusinessData: AlcoholBusinessModel;
  alcoholBusinessDetailData: AlcoholBusinessDetailModel;
  formGroup: FormGroup;
  dataSource: any[] = [];
  lstStore: any[] = [];
  public districtData: Array<any>;
  communeDataByDistrictId: any = [
    {
      id: '00000000-0000-0000-0000-000000000000',
      text: '-- Chọn --',
    },
  ];
  public communeData: Array<any>;
  public communeDataFilter: Array<any>;
  businessData: any;
  licenseData: any;
  loaded: number = 0;
  public options: Options;
  private subscriptions: Subscription[] = [];
  public default_value = '00000000-0000-0000-0000-000000000000';
  public typeofbusinessData: any;
  typeOfProfessionData: any;
  constructor(
    private alcoholBusinessService: AlcoholBusinessPageService,
    private fb: FormBuilder,
    public modalService: NgbActiveModal,
    private changeDetectorRefs: ChangeDetectorRef,
    public commonService: CommonService
  ) {}

  ngOnInit(): void {
    this.isLoading$ = this.alcoholBusinessService.isLoading$;
    (async () => {
      this.loadListAlcoholWholesaleLicense();
      this.loadBusiness();
      this.loaddistrict();
      this.loadcommune();
      this.getBusiness();
      await this.delay(200);
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
    return jQuery(
      '<span class="form-select form-select-solid form-select-lg">' +
        state.text +
        '</span>'
    );
  };

  delay(ms: number) {
    return new Promise((resolve) => setTimeout(resolve, ms));
  }

  loadDetail() {
    if (!this.id) {
      this.clear_model();
      this.loadForm();
    } else {
      const sb = this.alcoholBusinessService
        .getItemById(this.id)
        .pipe(
          first(),
          catchError((errorMessage) => {
            this.modalService.dismiss(errorMessage);
            return of(EMPTY_CUSTOM);
          })
        )
        .subscribe((res: any) => {
          this.alcoholBusinessData = res.items[0];
          // this.dataSource = res.data.details;
          this.dataSource = res.items[0].alcoholBusinessDetail;
          this.clear_modelDetail();
          this.lstStore = res.items[0].alcoholBusinessDetail;
          this.loadForm();
        });
      this.subscriptions.push(sb);
    }
  }

  loadForm() {
    this.formGroup = this.fb.group({
      alcoholBusinessName: [
        this.alcoholBusinessData.alcoholBusinessName,
        Validators.compose([Validators.required]),
      ],
      TenDoanhNghiep: [
        this.alcoholBusinessDetailData.TenDoanhNghiep,
        Validators.compose([Validators.required]),
      ],
      NguoiDaiDien: [
        this.alcoholBusinessDetailData.NguoiDaiDien,
        Validators.compose([Validators.required]),
      ],
      SoDienThoai: [
        this.alcoholBusinessDetailData.SoDienThoai,
        Validators.compose([Validators.required]),
      ],
      Huyen: [
        this.alcoholBusinessDetailData.Huyen,
        Validators.compose([Validators.required]),
      ],
      Xa: [
        this.alcoholBusinessDetailData.Xa,
        Validators.compose([Validators.required]),
      ],
      DiaChi: [this.alcoholBusinessDetailData.DiaChi],
      GiayPhepKinhDoanh: [this.alcoholBusinessDetailData.GiayPhepKinhDoanh],
      NgayHetHan: [this.alcoholBusinessDetailData.NgayHetHan],
      DonViCungCap: [this.alcoholBusinessDetailData.DonViCungCap],
      GiayDangKyKinhDoanh: this.alcoholBusinessData.giayDangKyKinhDoanh,
      NgayCapPhep: this.convert_date_string(
        this.alcoholBusinessData.ngayCapPhep
      ),
      GiayPhepBanBuon: this.alcoholBusinessData.giayPhepBanBuon,
      NgayCapGiayPhepBanBuon: this.convert_date_string(
        this.alcoholBusinessData.ngayCapGiayPhepBanBuon
      ),
      NgayHetHanGiayPhepBanBuon: this.convert_date_string(
        this.alcoholBusinessData.ngayHetHanGiayPhepBanBuon
      ),
      NgayCapGiayPhepBanLe: this.alcoholBusinessDetailData.NgayCapGiayPhepBanLe,
      DiaChiDonViCungCap: this.alcoholBusinessDetailData.DiaChiDonViCungCap,
      SoDienThoaiDonViCungCap:
        this.alcoholBusinessDetailData.SoDienThoaiDonViCungCap,
      GhiChu: this.alcoholBusinessDetailData.GhiChu,
    });
    this.subscriptions.push(
      this.formGroup.controls.alcoholBusinessName.valueChanges.subscribe(
        (x) => {
          const find_data = this.typeofbusinessData.find(
            (x: any) =>
              x.id == this.formGroup.controls.alcoholBusinessName.value
          );
          if (find_data.id !== '00000000-0000-0000-0000-000000000000') {
            const value = this.licenseData.find(
              (item: any) => item.BusinessId === find_data.id
            );
            if (value) {
              this.formGroup.patchValue(
                {
                  GiayPhepBanBuon: value.LicenseNumber,
                  NgayCapGiayPhepBanBuon: this.convert_date_string(
                    value.LicenseDate
                  ),
                  NgayHetHanGiayPhepBanBuon: this.convert_date_string(
                    value.ExpirationDate
                  ),
                },
                { emitEvent: false }
              );
            } else {
              this.formGroup.patchValue(
                {
                  GiayPhepBanBuon: '',
                  NgayCapGiayPhepBanBuon: null,
                  NgayHetHanGiayPhepBanBuon: null,
                },
                { emitEvent: false }
              );
            }
            this.formGroup.patchValue(
              {
                GiayDangKyKinhDoanh: find_data.giayPhepSanXuat,
                NgayCapPhep: find_data.ngayCapPhep,
              },
              { emitEvent: false }
            );
          } else {
            this.formGroup.patchValue(
              {
                GiayDangKyKinhDoanh: '',
                NgayCapPhep: '',
              },
              { emitEvent: false }
            );
          }
        }
      )
    );
  }
  clear_model() {
    EMPTY_CUSTOM.alcoholBusinessId = '';
    EMPTY_CUSTOM.alcoholBusinessName = '00000000-0000-0000-0000-000000000000';
    EMPTY_CUSTOM.alcoholBusinessDetail = [];
    EMPTY_DETAIL.TenDoanhNghiep = '';
    EMPTY_DETAIL.NguoiDaiDien = '';
    EMPTY_DETAIL.SoDienThoai = '';
    EMPTY_DETAIL.Huyen = '00000000-0000-0000-0000-000000000000';
    EMPTY_DETAIL.Xa = '00000000-0000-0000-0000-000000000000';
    EMPTY_DETAIL.DiaChi = '';
    EMPTY_DETAIL.GiayPhepKinhDoanh = '';
    EMPTY_DETAIL.NgayHetHan = null;
    EMPTY_DETAIL.DonViCungCap = '';
    EMPTY_DETAIL.DiaChiDonViCungCap = '';
    EMPTY_DETAIL.SoDienThoaiDonViCungCap = '';
    EMPTY_DETAIL.NgayCapGiayPhepBanLe = null;
    EMPTY_DETAIL.GhiChu = '';
    this.alcoholBusinessDetailData = EMPTY_DETAIL;
    this.alcoholBusinessData = EMPTY_CUSTOM;
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
    EMPTY_DETAIL.DiaChiDonViCungCap = '';
    EMPTY_DETAIL.SoDienThoaiDonViCungCap = '';
    EMPTY_DETAIL.NgayCapGiayPhepBanLe = null;
    EMPTY_DETAIL.GhiChu = '';
    this.alcoholBusinessDetailData = EMPTY_DETAIL;
  }
  save() {
    this.prepareTypeOfEnergy();
    if (this.alcoholBusinessData.alcoholBusinessId != '') {
      this.edit();
    } else {
      this.alcoholBusinessData.alcoholBusinessId = this.default_value;
      this.create();
    }
  }

  edit() {
    const sbUpdate = this.alcoholBusinessService
      .update(this.alcoholBusinessData)
      .pipe(
        tap(() => {
          this.modalService.close();
        }),
        catchError((errorMessage) => {
          this.modalService.dismiss(errorMessage);
          return of(this.alcoholBusinessData);
        })
      )
      .subscribe((res: any) => {
        Swal.fire({
          icon: res.status == 1 ? 'success' : 'error',
          title:
            res.status == 1 ? 'Chỉnh sửa thành công' : 'Chỉnh sửa thất bại',
          confirmButtonText: 'Xác nhận',
          text: 'Chỉnh sửa ' + (res.status == 1 ? 'thành công' : 'thất bại'),
        });
      });
    this.subscriptions.push(sbUpdate);
  }

  create() {
    const sbCreate = this.alcoholBusinessService
      .create(this.alcoholBusinessData)
      .pipe(
        tap(() => {
          this.modalService.close();
        }),
        catchError((errorMessage) => {
          this.modalService.dismiss(errorMessage);
          return of(this.alcoholBusinessData);
        })
      )
      .subscribe((res: any) => {
        Swal.fire({
          icon: res.status == 1 ? 'success' : 'error',
          title: res.status == 1 ? 'Thêm mới thành công' : 'Thêm mới thất bại',
          confirmButtonText: 'Xác nhận',
          text: 'Thêm mới ' + (res.status == 1 ? 'thành công' : 'thất bại'),
        });
        this.alcoholBusinessData = EMPTY_CUSTOM;
      });
    this.subscriptions.push(sbCreate);
    EMPTY_CUSTOM.alcoholBusinessId = '';
    EMPTY_CUSTOM.alcoholBusinessName = '';
    this.alcoholBusinessData = EMPTY_CUSTOM;
  }

  private prepareTypeOfEnergy() {
    const formData = this.formGroup.value;
    this.alcoholBusinessData.alcoholBusinessName = formData.alcoholBusinessName;
    this.alcoholBusinessData.giayPhepBanBuon = formData.GiayPhepBanBuon;
    this.alcoholBusinessData.ngayCapGiayPhepBanBuon =
      formData.NgayCapGiayPhepBanBuon !== null
        ? this.convert_date(formData.NgayCapGiayPhepBanBuon)
        : null;
    this.alcoholBusinessData.ngayHetHanGiayPhepBanBuon =
      formData.NgayHetHanGiayPhepBanBuon !== null
        ? this.convert_date(formData.NgayHetHanGiayPhepBanBuon)
        : null;
    this.alcoholBusinessData.alcoholBusinessDetail = this.dataSource;
  }
  loadBusiness() {
    this.alcoholBusinessService.loadBusinesses().subscribe((res_business) => {
      const data_typeofbusiness = [
        {
          id: '00000000-0000-0000-0000-000000000000',
          text: '-- Chọn --',
        },
      ];
      for (var business of res_business.items) {
        let obj_business = {
          id: business.businessId,
          text: business.businessNameVi,
          giayPhepSanXuat: business.giayPhepSanXuat,
          ngayCapPhep: this.convert_date_string(business.ngayCapPhep),
        };
        data_typeofbusiness.push(obj_business);
      }
      this.typeofbusinessData = data_typeofbusiness;
    });
    return this.typeofbusinessData;
  }
  getBusiness() {
    this.commonService.getBusiness().subscribe((res: any) => {
      const businesses = [
        {
          id: '00000000-0000-0000-0000-000000000000',
          text: '-- Chọn --',
        },
      ];
      for (var item of res.items) {
        let obj_business = {
          id: item.businessId,
          text: item.businessNameVi,
          address: item.diaChi,
          phoneNumber: item.soDienThoai,
          representative: item.nguoiDaiDien,
        };
        businesses.push(obj_business);
      }
      this.businessData = businesses;
    });
  }

  loadListAlcoholWholesaleLicense() {
    this.alcoholBusinessService
      .loadListAlcoholWholesaleLicense()
      .subscribe((res: any) => {
        const data = [];
        for (var item of res.items) {
          let tmp = {
            BusinessId: item.businessId,
            LicenseNumber: item.licenseNumber,
            LicenseDate: item.licenseDate,
            ExpirationDate: item.expirationDate,
          };
          data.push(tmp);
        }
        this.licenseData = data;
      });
  }

  isDefaultValue(
    controlName: any //: boolean
  ) {
    const control = this.formGroup.controls[controlName];
    const isdefaultvalue =
      control.value == '00000000-0000-0000-0000-000000000000';
    if (isdefaultvalue) {
      control.setErrors({ default: true });
    }
    return control.invalid && (control.dirty || control.touched);
  }

  ngOnDestroy(): void {
    this.subscriptions.forEach((sb) => sb.unsubscribe());
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
    let checkFullForm =
      this.formGroup.value.TenDoanhNghiep !== '' &&
      this.formGroup.value.NguoiDaiDien !== '' &&
      this.formGroup.value.SoDienThoai !== '' &&
      this.formGroup.value.Huyen !== '00000000-0000-0000-0000-000000000000' &&
      this.formGroup.value.Xa !== '00000000-0000-0000-0000-000000000000';
    let checkEmptyForm =
      this.formGroup.value.TenDoanhNghiep == '' &&
      this.formGroup.value.NguoiDaiDien == '' &&
      this.formGroup.value.SoDienThoai == '' &&
      this.formGroup.value.Huyen == '00000000-0000-0000-0000-000000000000' &&
      this.formGroup.value.Xa == '00000000-0000-0000-0000-000000000000';

    if (
      this.formGroup.value.alcoholBusinessName ==
      '00000000-0000-0000-0000-000000000000'
    ) {
      this.formGroup.controls.alcoholBusinessName.markAsTouched();
    } else if (checkFullForm) {
      this.remindSaveData();
    } else if (this.dataSource.length == 0 || !checkEmptyForm) {
      this.formGroup.markAllAsTouched();
    } else {
      this.save();
    }
  }
  loaddistrict() {
    this.alcoholBusinessService.loadDistrict().subscribe((res: any) => {
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
        };
        districts.push(obj_district);
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

      this.loaded++;
      //    this.checkBeforeLoadForm();
    });
  }
  loadcommune() {
    this.alcoholBusinessService.loadCommune().subscribe((res: any) => {
      var communes = [
        {
          id: '00000000-0000-0000-0000-000000000000',
          text: '-- Chọn --',
          districtId: '00000000-0000-0000-0000-000000000000',
        },
      ];
      for (var item of res.items) {
        let obj_commune = {
          id: item.communeId,
          text: item.communeName,
          districtId: item.districtId,
        };
        communes.push(obj_commune);
      }
      this.communeData = communes.sort((a, b) => {
        if (a < b) {
          return -1;
        }
        if (a > b) {
          return 1;
        }
        return 0;
      });
      this.communeDataFilter = communes.sort((a, b) => {
        if (a < b) {
          return -1;
        }
        if (a > b) {
          return 1;
        }
        return 0;
      });

      this.loaded++;
      //this.checkBeforeLoadForm();
    });
  }
  loadCommuneByDistrict(event: any) {
    if (event != '00000000-0000-0000-0000-000000000000') {
      var result = this.communeData.filter(
        (x: { id: string; districtId: any }) =>
          x.id == '00000000-0000-0000-0000-000000000000' ||
          x.districtId == event
      );
      this.communeDataByDistrictId = result;
    } else {
      this.communeDataByDistrictId = this.communeData;
    }
    this.formGroup.controls['Xa'].setValue(
      '00000000-0000-0000-0000-000000000000'
    );
  }

  add_detail() {
    let tenDoanhNghiep = this.formGroup.value.TenDoanhNghiep;
    let nguoiDaiDien = this.formGroup.value.NguoiDaiDien;
    let soDienThoai = this.formGroup.value.SoDienThoai;
    let huyen = this.formGroup.value.Huyen;
    let xa = this.formGroup.value.Xa;
    let diaChi = this.formGroup.value.DiaChi;
    let giayPhepKinhDoanh = this.formGroup.value.GiayPhepKinhDoanh;
    let ngayHetHan = this.convert_date(this.formGroup.value.NgayHetHan);
    let donViCungCap = this.formGroup.value.DonViCungCap;
    let diaChiDonViCungCap = this.formGroup.value.DiaChiDonViCungCap;
    let soDienThoaiDonViCungCap = this.formGroup.value.SoDienThoaiDonViCungCap;
    let ngayCapGiayPhepBanLe = this.convert_date(
      this.formGroup.value.NgayCapGiayPhepBanLe
    );
    let ghiChu = this.formGroup.value.GhiChu;
    if (
      tenDoanhNghiep == '' ||
      nguoiDaiDien == '' ||
      soDienThoai == '' ||
      huyen == '00000000-0000-0000-0000-000000000000' ||
      xa == '00000000-0000-0000-0000-000000000000'
    ) {
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
      diaChiDonViCungCap,
      soDienThoaiDonViCungCap,
      ngayCapGiayPhepBanLe,
      ghiChu,
    };

    this.lstStore.push(item);
    this.dataSource = this.lstStore;
    this.formGroup.controls.TenDoanhNghiep.reset('');

    this.formGroup.controls.NguoiDaiDien.reset('');
    this.formGroup.controls.SoDienThoai.reset('');
    this.formGroup.controls.Huyen.reset('00000000-0000-0000-0000-000000000000');

    this.formGroup.controls.Xa.reset('00000000-0000-0000-0000-000000000000');
    this.formGroup.controls.DiaChi.reset('');
    this.formGroup.controls.GiayPhepKinhDoanh.reset('');
    this.formGroup.controls.NgayHetHan.reset('');
    this.formGroup.controls.DonViCungCap.reset('');
    this.formGroup.controls.DiaChiDonViCungCap.reset('');
    this.formGroup.controls.SoDienThoaiDonViCungCap.reset('');
    this.formGroup.controls.NgayCapGiayPhepBanLe.reset(null);
    this.formGroup.controls.GhiChu.reset('');
    this.changeDetectorRefs.detectChanges();
  }
  delete_detail(item: any) {
    const index: number = this.lstStore.indexOf(item);
    this.lstStore.splice(index, 1);
    this.dataSource = this.lstStore;
  }
  convert_date(string_date: string | null) {
    if (string_date === null) {
      return null;
    }
    var result = moment.utc(string_date, 'DD/MM/YYYY');
    return result;
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
      confirmButtonText: 'Xác nhận',
    });
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
}
