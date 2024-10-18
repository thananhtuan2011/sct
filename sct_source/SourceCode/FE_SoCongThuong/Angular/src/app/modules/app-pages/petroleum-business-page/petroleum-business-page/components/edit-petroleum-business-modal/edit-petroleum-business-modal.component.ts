import {
  ChangeDetectorRef,
  Component,
  Input,
  OnDestroy,
  OnInit,
} from '@angular/core';
import {
  FormBuilder,
  FormControl,
  FormGroup,
  Validators,
} from '@angular/forms';
import {
  NgbActiveModal,
  NgbDateAdapter,
  NgbDateParserFormatter,
} from '@ng-bootstrap/ng-bootstrap';
import { of, Subscription } from 'rxjs';
import { catchError, finalize, first, tap } from 'rxjs/operators';
import { SelectOptionData } from 'src/app/_metronic/shared/components/select-custom/select-custom.interface';
import Swal from 'sweetalert2';
import {
  PetroleumBusinessModel,
  PetroleumBusinessDetailModel,
} from '../../../_models/petroleum-business.model';
import { PetroleumBusinessPageService } from '../../../_services/petroleum-business-page.service';
import { Options } from 'select2';
import { CommonService } from 'src/app/_metronic/shared/services/common.service';
import * as moment from 'moment';

const EMPTY_CUSTOM: PetroleumBusinessModel = {
  id: '',
  petroleumBusinessId: '',
  petroleumBusinessName: '00000000-0000-0000-0000-000000000000',
  petroleumBusinessDetail: [],
  giayDangKyKinhDoanh: '',
  ngayCap: null
};

const EMPTY_DETAIL: PetroleumBusinessDetailModel = {
  TenCuaHang: '',
  NguoiDaiDien: '',
  SoDienThoai: '',
  Huyen: '00000000-0000-0000-0000-000000000000',
  Xa: '00000000-0000-0000-0000-000000000000',
  DiaChi: '',
  GiayPhepKinhDoanh: '',
  NgayHetHan: null,
  NgayCapPhep: null,
  ThoiHan5Nam: null,
  NguoiQuanLy: '',
  HinhThuc: '00000000-0000-0000-0000-000000000000',
  SoCotBomE5: null,
  SoCotBomA95: null,
  SoCotBomOil: null,
  SoBeChua: null,
  TongDungTich: null,
  ThoiGianBanHang: '',
  DienTichXayDung: '',
  TuyenPhucVu: '',
  DonViCungCap: '',
  LoaiGiayXacNhan: '00000000-0000-0000-0000-000000000000',
  ThoiHan1Nam: null,
  DiaChiDonViCungCap: '',
  NguoiLienHeDonViCungCap: '',
  SoDienThoaiDonViCungCap: '',
  HinhThucHopDong: '00000000-0000-0000-0000-000000000000',
  GhiChu: '',
  NgayCapPhepXayDung: null
};
@Component({
  selector: 'app-edit-petroleum-business-modal.component',
  templateUrl: './edit-petroleum-business-modal.component.html',
  styleUrls: ['./edit-petroleum-business-modal.component.scss'],
})
export class EditPetroleumBusinessModalComponent implements OnInit, OnDestroy {
  @Input() id: any;
  @Input() type: any;
  isLoading$: any;
  petroleumBusinessData: PetroleumBusinessModel;
  petroleumBusinessDetailData: PetroleumBusinessDetailModel;
  formGroup: FormGroup;
  public options: Options;
  businessData: any;
  contractFormData: any ;
  certificateTypeData: any;
  dataSource: any[] = [];
  lstStore: any[] = [];
  displayedColumns: string[] = ['stt', 'name', 'action'];
  communeDataByDistrictId: any = [
    {
      id: '00000000-0000-0000-0000-000000000000',
      text: '-- Chọn --',
    },
  ];
  private subscriptions: Subscription[] = [];
  public default_value = '00000000-0000-0000-0000-000000000000';
  public typeofbusinessData: any;
  public formalityData: any = [];
  detailData: any = [];
  public districtData: Array<any>;
  // public districtId: any;
  public communeData: Array<any>;
  public communeDataFilter: Array<any>;

  loaded: number = 0;
  constructor(
    private petroleumBusinessService: PetroleumBusinessPageService,
    private fb: FormBuilder,
    public modal: NgbActiveModal,
    private changeDetectorRefs: ChangeDetectorRef,
    public commonService: CommonService
  ) {}

  ngOnInit(): void {
    this.isLoading$ = this.petroleumBusinessService.isLoading$;
    (async () => {
      this.loadBusiness();
      this.loadContractForm();
      this.loadCertificateType();
      this.loadBusinessFormality();
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
      const sb = this.petroleumBusinessService
        .getItemById(this.id)
        .pipe(
          first(),
          catchError((errorMessage) => {
            this.modal.dismiss(errorMessage);
            return of(EMPTY_CUSTOM);
          })
        )
        .subscribe((res: any) => {
          this.petroleumBusinessData = res.items[0];
          // this.resetModell()
          this.dataSource = res.items[0].petroleumBusinessDetail;
          this.clear_modelDetail();
          this.lstStore = res.items[0].petroleumBusinessDetail;
          // this.detailData = res.items[0].petroleumBuDetail;
          this.loadForm();
        });
      this.subscriptions.push(sb);
    }
  }

  loadForm() {
    this.formGroup = this.fb.group({
      petroleumBusinessName: [
        this.petroleumBusinessData.petroleumBusinessName,
        Validators.compose([Validators.required]),
      ],
      TenCuaHang: [
        this.petroleumBusinessDetailData.TenCuaHang,
        Validators.compose([Validators.required]),
      ],
      NguoiDaiDien: [
        this.petroleumBusinessDetailData.NguoiDaiDien,
        Validators.compose([Validators.required]),
      ],
      SoDienThoai: [
        this.petroleumBusinessDetailData.SoDienThoai,
        Validators.compose([Validators.required]),
      ],
      Huyen: [
        this.petroleumBusinessDetailData.Huyen,
        Validators.compose([Validators.required]),
      ],
      Xa: [
        this.petroleumBusinessDetailData.Xa,
        Validators.compose([Validators.required]),
      ],
      DiaChi: [this.petroleumBusinessDetailData.DiaChi],
      GiayPhepKinhDoanh: [this.petroleumBusinessDetailData.GiayPhepKinhDoanh],
      NgayHetHan: [this.petroleumBusinessDetailData.NgayHetHan],
      DonViCungCap: [
        this.petroleumBusinessDetailData.DonViCungCap,
        Validators.compose([Validators.required]),
      ],
      NgayCapPhep: [this.petroleumBusinessDetailData.NgayCapPhep],
      ThoiHan5Nam: [this.petroleumBusinessDetailData.ThoiHan5Nam],
      NguoiQuanLy: [
        this.petroleumBusinessDetailData.NguoiQuanLy,
        Validators.compose([Validators.required]),
      ],
      HinhThuc: [this.petroleumBusinessDetailData.HinhThuc],
      SoCotBomE5: [this.petroleumBusinessDetailData.SoCotBomE5],
      SoCotBomA95: [this.petroleumBusinessDetailData.SoCotBomA95],
      SoCotBomOil: [this.petroleumBusinessDetailData.SoCotBomOil],
      SoBeChua: [this.petroleumBusinessDetailData.SoBeChua],
      TongDungTich: [this.petroleumBusinessDetailData.TongDungTich],
      ThoiGianBanHang: [this.petroleumBusinessDetailData.ThoiGianBanHang],
      DienTichXayDung: [this.petroleumBusinessDetailData.DienTichXayDung],
      TuyenPhucVu: [this.petroleumBusinessDetailData.TuyenPhucVu],
      ThoiHan1Nam: this.petroleumBusinessDetailData.ThoiHan1Nam,
      LoaiGiayXacNhan: this.petroleumBusinessDetailData.LoaiGiayXacNhan,
      DiaChiDonViCungCap: this.petroleumBusinessDetailData.DiaChiDonViCungCap,
      NguoiLienHeDonViCungCap: this.petroleumBusinessDetailData.NguoiLienHeDonViCungCap,
      SoDienThoaiDonViCungCap: this.petroleumBusinessDetailData.SoDienThoaiDonViCungCap,
      HinhThucHopDong: this.petroleumBusinessDetailData.HinhThucHopDong,
      GhiChu: this.petroleumBusinessDetailData.GhiChu,
      NgayCapPhepXayDung: this.petroleumBusinessDetailData.NgayCapPhepXayDung,
      GiayDangKyKinhDoanh: this.petroleumBusinessData.giayDangKyKinhDoanh,
      NgayCap: this.petroleumBusinessData.ngayCap != null ? this.convert_date_string(this.petroleumBusinessData.ngayCap): null
    });
    
    this.subscriptions.push(
      this.formGroup.controls.petroleumBusinessName.valueChanges.subscribe((x) => {
        const find_data = this.typeofbusinessData.find((x: any) => x.id == this.formGroup.controls.petroleumBusinessName.value)
        if (find_data.id !== "00000000-0000-0000-0000-000000000000") {
          this.formGroup.patchValue({
            "GiayDangKyKinhDoanh": find_data.giayPhepSanXuat,
            "NgayCap": find_data.ngayCapPhep,
          }, { emitEvent: false })
        }
        else {
          this.formGroup.patchValue({
            "GiayDangKyKinhDoanh": '',
            "NgayCap": '',
          }, { emitEvent: false })
        }
      })
    );
  }

  clear_model() {
    EMPTY_CUSTOM.petroleumBusinessId = '';
    EMPTY_CUSTOM.petroleumBusinessName = '00000000-0000-0000-0000-000000000000';
    EMPTY_CUSTOM.petroleumBusinessDetail = [];
    EMPTY_DETAIL.TenCuaHang = '';
    EMPTY_DETAIL.NguoiDaiDien = '';
    EMPTY_DETAIL.SoDienThoai = '';
    EMPTY_DETAIL.Huyen = '00000000-0000-0000-0000-000000000000';
    EMPTY_DETAIL.Xa = '00000000-0000-0000-0000-000000000000';
    EMPTY_DETAIL.DiaChi = '';
    EMPTY_DETAIL.GiayPhepKinhDoanh = '';
    EMPTY_DETAIL.NgayHetHan = null;
    EMPTY_DETAIL.DonViCungCap = '';
    EMPTY_DETAIL.NgayCapPhep = '';
    EMPTY_DETAIL.NguoiQuanLy = '';
    EMPTY_DETAIL.HinhThuc = '00000000-0000-0000-0000-000000000000';
    EMPTY_DETAIL.SoCotBomE5 = null;
    EMPTY_DETAIL.SoCotBomA95 = null;
    EMPTY_DETAIL.SoCotBomOil = null;
    EMPTY_DETAIL.SoBeChua = null;
    EMPTY_DETAIL.TongDungTich = null;
    EMPTY_DETAIL.ThoiGianBanHang = '';
    EMPTY_DETAIL.DienTichXayDung = '';
    EMPTY_DETAIL.TuyenPhucVu = '';
    EMPTY_DETAIL.ThoiHan5Nam = null;
    EMPTY_DETAIL.ThoiHan1Nam = null;
    EMPTY_DETAIL.LoaiGiayXacNhan = '00000000-0000-0000-0000-000000000000';
    EMPTY_DETAIL.DiaChiDonViCungCap = "";
    EMPTY_DETAIL.NguoiLienHeDonViCungCap = "";
    EMPTY_DETAIL.SoDienThoaiDonViCungCap = "";
    EMPTY_DETAIL.HinhThucHopDong = '00000000-0000-0000-0000-000000000000';
    EMPTY_DETAIL.GhiChu = "";
    EMPTY_DETAIL.NgayCapPhepXayDung = null;
    this.petroleumBusinessDetailData = EMPTY_DETAIL;
    this.petroleumBusinessData = EMPTY_CUSTOM;
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
    EMPTY_DETAIL.NgayCapPhep = '';
    EMPTY_DETAIL.NguoiQuanLy = '';
    EMPTY_DETAIL.HinhThuc = '00000000-0000-0000-0000-000000000000';
    EMPTY_DETAIL.SoCotBomE5 = null;
    EMPTY_DETAIL.SoCotBomA95 = null;
    EMPTY_DETAIL.SoCotBomOil = null;
    EMPTY_DETAIL.SoBeChua = null;
    EMPTY_DETAIL.TongDungTich = null;
    EMPTY_DETAIL.ThoiGianBanHang = '';
    EMPTY_DETAIL.DienTichXayDung = '';
    EMPTY_DETAIL.TuyenPhucVu = '';
    EMPTY_DETAIL.ThoiHan5Nam = null;
    EMPTY_DETAIL.ThoiHan1Nam = null;
    EMPTY_DETAIL.LoaiGiayXacNhan = '00000000-0000-0000-0000-000000000000';
    EMPTY_DETAIL.DiaChiDonViCungCap = "";
    EMPTY_DETAIL.NguoiLienHeDonViCungCap = "";
    EMPTY_DETAIL.SoDienThoaiDonViCungCap = "";
    EMPTY_DETAIL.HinhThucHopDong = '00000000-0000-0000-0000-000000000000';
    EMPTY_DETAIL.GhiChu = "";
    EMPTY_DETAIL.NgayCapPhepXayDung = null;
    this.petroleumBusinessDetailData = EMPTY_DETAIL;
  }

  save() {
    this.prepareTypeOfEnergy();
    if (this.petroleumBusinessData.petroleumBusinessId != '') {
      this.edit();
    } else {
      this.petroleumBusinessData.petroleumBusinessId = this.default_value;
      this.create();
    }
  }

  edit() {
    const sbUpdate = this.petroleumBusinessService
      .update(this.petroleumBusinessData)
      .pipe(
        tap(() => {
          this.modal.close();
        }),
        catchError((errorMessage) => {
          this.modal.dismiss(errorMessage);
          return of(this.petroleumBusinessData);
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
        // this.petroleumBusinessData = EMPTY_CUSTOM
      });
    this.subscriptions.push(sbUpdate);
  }

  create() {
    const sbCreate = this.petroleumBusinessService
      .create(this.petroleumBusinessData)
      .pipe(
        tap(() => {
          this.modal.close();
        }),
        catchError((errorMessage) => {
          this.modal.dismiss(errorMessage);
          return of(this.petroleumBusinessData);
        })
      )
      .subscribe((res: any) => {
        Swal.fire({
          icon: res.status == 1 ? 'success' : 'error',
          title: res.status == 1 ? 'Thêm mới thành công' : 'Thêm mới thất bại',
          confirmButtonText: 'Xác nhận',
          text: 'Thêm mới ' + (res.status == 1 ? 'thành công' : 'thất bại'),
        });
        this.petroleumBusinessData = EMPTY_CUSTOM;
      });
    this.subscriptions.push(sbCreate);
    EMPTY_CUSTOM.petroleumBusinessId = '';
    EMPTY_CUSTOM.petroleumBusinessName = '';
    this.petroleumBusinessData = EMPTY_CUSTOM;
  }

  private prepareTypeOfEnergy() {
    const formData = this.formGroup.value;
    this.petroleumBusinessData.petroleumBusinessName =
      formData.petroleumBusinessName;
    this.petroleumBusinessData.petroleumBusinessDetail = this.dataSource;
  }

  loadBusiness() {
    this.commonService.getBusiness().subscribe((res_business: any) => {
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
          representative: business.nguoiDaiDien,
          phoneNumber: business.soDienThoai,
          giayPhepSanXuat: business.giayPhepSanXuat,
          ngayCapPhep: this.convert_date_string(business.ngayCapPhep)
        };
        data_typeofbusiness.push(obj_business);
      }
      this.typeofbusinessData = data_typeofbusiness;
    });
    return this.typeofbusinessData;
  }
  
  loadContractForm() {
    this.commonService.getListContractForm().subscribe((res: any) => {
      const data = [
        {
          id: '00000000-0000-0000-0000-000000000000',
          text: '-- Chọn --',
        },
      ];
      for (var item of res.items) {
        let obj = {
          id: item.categoryId,
          text: item.categoryName,
        };
        data.push(obj);
      }
      this.contractFormData = data;
    });
  }
  
  loadCertificateType() {
    this.commonService.getListCertificateType().subscribe((res: any) => {
      const data = [
        {
          id: '00000000-0000-0000-0000-000000000000',
          text: '-- Chọn --',
        },
      ];
      for (var item of res.items) {
        let obj = {
          id: item.categoryId,
          text: item.categoryName,
        };
        data.push(obj);
      }
      this.certificateTypeData = data;
    });
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

  check_Store() {
    if (this.formGroup.invalid) {
      this.formGroup.markAllAsTouched();
    } else {
      this.addStore();
    }
  }

  addStore() {
    let tenCuaHang = this.formGroup.value.TenCuaHang;
    let nguoiDaiDien = this.formGroup.value.NguoiDaiDien;
    let soDienThoai = this.formGroup.value.SoDienThoai;
    let huyen = this.formGroup.value.Huyen;
    let xa = this.formGroup.value.Xa;
    let diaChi = this.formGroup.value.DiaChi;
    let giayPhepKinhDoanh = this.formGroup.value.GiayPhepKinhDoanh;
    let ngayHetHan = this.convert_date(this.formGroup.value.NgayHetHan);
    let donViCungCap = this.formGroup.value.DonViCungCap;
    let ngayCapPhep = this.convert_date(this.formGroup.value.NgayCapPhep);
    let thoiHan5Nam = this.convert_date(this.formGroup.value.ThoiHan5Nam);
    let nguoiQuanLy = this.formGroup.value.NguoiQuanLy;
    let hinhThuc = this.formGroup.value.HinhThuc;
    let soCotBomE5 = this.formGroup.value.SoCotBomE5;
    let soCotBomA95 = this.formGroup.value.SoCotBomA95;
    let soCotBomOil = this.formGroup.value.SoCotBomOil;
    let soBeChua = this.formGroup.value.SoBeChua;
    let tongDungTich = this.formGroup.value.TongDungTich;
    let thoiGianBanHang = this.formGroup.value.ThoiGianBanHang;
    let dienTichXayDung = this.formGroup.value.DienTichXayDung;
    let tuyenPhucVu = this.formGroup.value.TuyenPhucVu;
    let thoiHan1Nam = this.convert_date(this.formGroup.value.ThoiHan1Nam);
    let loaiGiayXacNhan = this.formGroup.value.LoaiGiayXacNhan;
    let diaChiDonViCungCap = this.formGroup.value.DiaChiDonViCungCap;
    let nguoiLienHeDonViCungCap = this.formGroup.value.NguoiLienHeDonViCungCap;
    let soDienThoaiDonViCungCap = this.formGroup.value.SoDienThoaiDonViCungCap;
    let hinhThucHopDong = this.formGroup.value.HinhThucHopDong;
    let ghiChu = this.formGroup.value.GhiChu;
    let ngayCapPhepXayDung = this.convert_date(this.formGroup.value.NgayCapPhepXayDung);
    
    if (
      tenCuaHang == '' ||
      nguoiDaiDien == '' ||
      nguoiQuanLy == '' ||
      soDienThoai == '' ||
      huyen == '00000000-0000-0000-0000-000000000000' ||
      xa == '00000000-0000-0000-0000-000000000000' ||
      hinhThuc == '00000000-0000-0000-0000-000000000000'
    ) {
      this.formGroup.markAllAsTouched();
      return;
    }
    let item = {
      tenCuaHang,
      nguoiDaiDien,
      soDienThoai,
      huyen,
      xa,
      diaChi,
      giayPhepKinhDoanh,
      ngayHetHan,
      donViCungCap,
      ngayCapPhep,
      thoiHan5Nam,
      nguoiQuanLy,
      hinhThuc,
      soCotBomE5,
      soCotBomA95,
      soCotBomOil,
      soBeChua,
      tongDungTich,
      thoiGianBanHang,
      dienTichXayDung,
      tuyenPhucVu,
      thoiHan1Nam,
      loaiGiayXacNhan,
      diaChiDonViCungCap,
      nguoiLienHeDonViCungCap,
      soDienThoaiDonViCungCap,
      hinhThucHopDong,
      ghiChu,
      ngayCapPhepXayDung
    };

    this.lstStore.push(item);
    this.dataSource = this.lstStore;
    this.formGroup.controls.TenCuaHang.reset('');
    this.formGroup.controls.NguoiDaiDien.reset('');
    this.formGroup.controls.SoDienThoai.reset('');
    this.formGroup.controls.Huyen.reset('00000000-0000-0000-0000-000000000000');
    this.formGroup.controls.Xa.reset('00000000-0000-0000-0000-000000000000');
    this.formGroup.controls.HinhThuc.reset(
      '00000000-0000-0000-0000-000000000000'
    );
    this.formGroup.controls.DiaChi.reset('');
    this.formGroup.controls.GiayPhepKinhDoanh.reset('');
    this.formGroup.controls.NgayHetHan.reset('');
    this.formGroup.controls.DonViCungCap.reset('');
    this.formGroup.controls.NgayCapPhep.reset('');
    this.formGroup.controls.SoCotBomOil.reset('');
    this.formGroup.controls.SoCotBomA95.reset('');
    this.formGroup.controls.SoCotBomE5.reset('');
    this.formGroup.controls.NguoiQuanLy.reset('');
    this.formGroup.controls.ThoiHan5Nam.reset(null);
    this.formGroup.controls.ThoiGianBanHang.reset('');
    this.formGroup.controls.TongDungTich.reset('');
    this.formGroup.controls.SoBeChua.reset('');
    this.formGroup.controls.DienTichXayDung.reset('');
    this.formGroup.controls.TuyenPhucVu.reset('');
    
    this.formGroup.controls.ThoiHan1Nam.reset(null);
    this.formGroup.controls.LoaiGiayXacNhan.reset('00000000-0000-0000-0000-000000000000');
    this.formGroup.controls.DiaChiDonViCungCap.reset('');
    this.formGroup.controls.SoDienThoaiDonViCungCap.reset('');
    this.formGroup.controls.NguoiLienHeDonViCungCap.reset('');
    this.formGroup.controls.HinhThucHopDong.reset('00000000-0000-0000-0000-000000000000');
    this.formGroup.controls.GhiChu.reset('');
    this.formGroup.controls.NgayCapPhepXayDung.reset(null);
    
    

    this.changeDetectorRefs.detectChanges();
  }

  delStore(item: any) {
    const index: number = this.lstStore.indexOf(item);
    this.lstStore.splice(index, 1);
    this.dataSource = this.lstStore;
  }

  delete_detail(data: any) {
    this.detailData = this.detailData.filter(
      (x: any) => x.businessNameVi !== data.businessNameVi
    );
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
      this.formGroup.value.TenCuaHang !== '' &&
      this.formGroup.value.NguoiDaiDien !== '' &&
      this.formGroup.value.SoDienThoai !== '' &&
      this.formGroup.value.Huyen !== '00000000-0000-0000-0000-000000000000' &&
      this.formGroup.value.Xa !== '00000000-0000-0000-0000-000000000000' &&
      this.formGroup.value.NguoiQuanLy !== '' &&
      this.formGroup.value.HinhThuc !== '00000000-0000-0000-0000-000000000000';
    let checkEmptyForm =
      this.formGroup.value.TenCuaHang == '' &&
      this.formGroup.value.NguoiDaiDien == '' &&
      this.formGroup.value.SoDienThoai == '' &&
      this.formGroup.value.Huyen == '00000000-0000-0000-0000-000000000000' &&
      this.formGroup.value.Xa == '00000000-0000-0000-0000-000000000000' &&
      this.formGroup.value.NguoiQuanLy == '' &&
      this.formGroup.value.HinhThuc == '00000000-0000-0000-0000-000000000000';

    if (
      this.formGroup.value.petroleumBusinessName ==
      '00000000-0000-0000-0000-000000000000'
    ) {
      this.formGroup.controls.petroleumBusinessName.markAsTouched();
    } else if (checkFullForm) {
      this.remindSaveData();
    } else if (this.dataSource.length == 0 || !checkEmptyForm) {
      this.formGroup.markAllAsTouched();
    } else {
      this.save();
    }
  }

  public loadBusinessFormality() {
    const sub = this.commonService
      .GetConfig('BUSINESS_FORMALITY')
      .subscribe((res: any) => {
        const data = [
          { id: '00000000-0000-0000-0000-000000000000', text: '-- Chọn --' },
          ...res.items.listConfig.map((item: any) => ({
            id: item.categoryId,
            text: item.categoryName,
            typeCode: item.categoryTypeCode,
            code: item.categoryCode,
            priority: item.priority,
          })),
        ];
        this.formalityData = data;
      });
    this.subscriptions.push(sub);
  }

  loaddistrict() {
    this.petroleumBusinessService.loadDistrict().subscribe((res: any) => {
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
    this.petroleumBusinessService.loadCommune().subscribe((res: any) => {
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

  checkBeforeLoadForm() {
    if (this.loaded > 3) {
      this.loadBusiness();
    }
  }

  prenventInputNonNumber(event: any) {
    if (event.which < 48 || event.which > 57) {
      event.preventDefault();
    }
  }

  changedistrict(event: any) {
    if (event != '00000000-0000-0000-0000-000000000000') {
      var result_commune = this.communeData.filter(
        (x) =>
          x.districtId == event ||
          x.id == '00000000-0000-0000-0000-000000000000'
      );
      this.communeDataFilter = result_commune;
    } else {
      this.communeDataFilter = this.communeData;
    }
    this.formGroup.controls['Xa'].setValue(
      '00000000-0000-0000-0000-000000000000'
    );
  }

  remindSaveData() {
    Swal.fire({
      title: 'Bạn chưa lưu thông tin đã nhập',
      text: 'Vui lòng lưu thông tin hệ thống cửa hàng',
      icon: 'warning',
      showCancelButton: true,
      confirmButtonColor: '#3085d6',
      cancelButtonColor: '#d33',
      cancelButtonText: 'Thoát',
      confirmButtonText: 'Xác nhận',
    });
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
}
