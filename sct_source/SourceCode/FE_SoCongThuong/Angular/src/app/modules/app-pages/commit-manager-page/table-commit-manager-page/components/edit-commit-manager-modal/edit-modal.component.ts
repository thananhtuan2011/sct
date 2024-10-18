import { Component, Input, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal, NgbDateAdapter, NgbDateParserFormatter, NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { of, Subscription } from 'rxjs';
import { catchError, finalize, first, tap } from 'rxjs/operators';
import Swal from 'sweetalert2';
import { CommitManagerModel } from '../../../_models/commit-manager-page.model';
import { CommitManagerPageService } from '../../../_services/commit-manager-page.service';
import { Options } from 'select2';
import * as moment from 'moment';
import { CommonService } from 'src/app/_metronic/shared/services/common.service';
import { EditItemsComponent } from '../edit-items-modal/edit-modal.component';

const EMPTY_CUSTOM: CommitManagerModel = {
  id: '',
  commitManagerId: '00000000-0000-0000-0000-000000000000',
  maHoSo: '',
  tenThuTuc: '',
  tenToChuc: '',
  coSo: '',
  diaChi: '',
  soDienThoai: '',
  ngayNhanHoSo: null,
  ngayCamKet: null,
  nguoiLamCamKet: '',
  ghiChu: '',
  huyen: '00000000-0000-0000-0000-000000000000',
  listItems: []
};

@Component({
  selector: 'app-edit-commit-finance-plan-modal',
  templateUrl: './edit-modal.component.html',
  styleUrls: ['./edit-modal.component.scss'],

})
export class EditCommitManagerModalComponent implements OnInit, OnDestroy {
  @Input() id: any;
  @Input() itemData: any;
  @Input() type: any;
  @Input() districtData: any;
  typeData: any;
  isLoading$: any;
  data: CommitManagerModel;
  formGroup: FormGroup;
  dataCommitGroup: any = [];
  private subscriptions: Subscription[] = [];
  dataCreator: any = [];
  options: Options
  businessData: any = [];
  listItems: any = []
  show: boolean = false;
  constructor(
    public commitManager: CommitManagerPageService,
    private fb: FormBuilder, public modal: NgbActiveModal,
    private commonService: CommonService,
    private modalService: NgbModal,

  ) { }

  ngOnInit(): void {
    this.isLoading$ = this.commitManager.isLoading$;
    this.loadBusiness();
    this.loadTypeItemsData();
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

  loadForm() {
    if (!this.id) {
      this.clearModel();
    } else {
      this.data = this.itemData;
      this.listItems = this.itemData.listItems;
    }
    this.formGroup = this.fb.group({
      MaHoSo: [this.data.maHoSo, Validators.required],
      TenThuTuc: [this.data.tenThuTuc, Validators.required],
      TenToChuc: [this.data.tenToChuc, Validators.required],
      CoSo: [this.data.coSo, Validators.required],
      DiaChi: [this.data.diaChi, Validators.required],
      SoDienThoai: [this.data.soDienThoai, Validators.compose([Validators.minLength(10), Validators.maxLength(11), Validators.pattern("^0[0-9]{9,10}$")])],
      NgayNhanHoSo: [this.convert_date_string(this.data.ngayNhanHoSo), Validators.required],
      NgayCamKet: [this.convert_date_string(this.data.ngayCamKet), Validators.required],
      NguoiLamCamKet: [this.data.nguoiLamCamKet, Validators.required],
      GhiChu: this.data.ghiChu,
      Huyen: this.data.huyen
    });
    if (this.type) {
      this.formGroup.disable();
    }
    this.show = true;
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
    const sbUpdate = this.commitManager.update(this.data).pipe(
      tap(() => {
        this.modal.close();
      }),
      catchError((errorMessage) => {
        this.modal.dismiss(errorMessage);
        return of(this.data);
      }),
    ).subscribe((res: any) => {
      Swal.fire({
        icon: res.status == 1 ? 'success' : 'error',
        title: res.status == 1 ? 'Chỉnh sửa thành công' : 'Chỉnh sửa thất bại',
        confirmButtonText: 'Xác nhận',
        text: (res.status == 1 ? 'Chỉnh sửa thành công' : res.status == 0 ? res.error.msg : "Chỉnh sửa thất bại"),
      });
    });
    this.subscriptions.push(sbUpdate);
  }

  create() {
    const sbCreate = this.commitManager.create(this.data).pipe(
      tap(() => {
        this.modal.close();
      }),
      catchError((errorMessage) => {
        this.modal.dismiss(errorMessage);
        return of(this.data);
      }),
    ).subscribe((res: any) => {
      Swal.fire({
        icon: res.status == 1 ? 'success' : 'error',
        title: res.status == 1 ? 'Thêm mới thành công' : 'Thêm mới thất bại',
        confirmButtonText: 'Xác nhận',
        text: (res.status == 1 ? 'Thêm mới thành công' : res.status == 0 ? res.error.msg : 'Thêm mới thất bại'),
      });
      this.data = EMPTY_CUSTOM
    });
    this.subscriptions.push(sbCreate);
  }

  private prepareData() {
    const formData = this.formGroup.value;
    this.data.maHoSo = formData.MaHoSo;
    this.data.tenThuTuc = formData.TenThuTuc;
    this.data.tenToChuc = formData.TenToChuc;
    this.data.coSo = formData.CoSo;
    this.data.diaChi = formData.DiaChi;
    this.data.soDienThoai = formData.SoDienThoai;
    this.data.ngayNhanHoSo = this.convert_date(formData.NgayNhanHoSo);
    this.data.ngayCamKet = this.convert_date(formData.NgayCamKet);
    this.data.nguoiLamCamKet = formData.NguoiLamCamKet;
    this.data.ghiChu = formData.GhiChu;
    this.data.huyen = formData.Huyen;
    this.data.listItems = this.listItems;
  }

  prenventInputNonNumber(event: any) {
    if (event.which < 48 || event.which > 57) {
      event.preventDefault();
    }
  }

  ngOnDestroy(): void {
    this.subscriptions.forEach(sb => sb.unsubscribe());
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
    if (control.value == '' || control.value == '00000000-0000-0000-0000-000000000000') {
      control.setErrors({ 'default': true })
    } else {
      control.setErrors(null)
    }
    return control.hasError('default') && (control.dirty || control.touched);
  }

  check_formGroup() {
    if (this.formGroup.invalid) {
      this.formGroup.markAllAsTouched();
      this.formGroup.updateValueAndValidity();
    }
    else {
      this.save()
    }
  }

  clearModel() {
    EMPTY_CUSTOM.id = '';
    EMPTY_CUSTOM.commitManagerId = '00000000-0000-0000-0000-000000000000';
    EMPTY_CUSTOM.maHoSo = '';
    EMPTY_CUSTOM.tenThuTuc = '';
    EMPTY_CUSTOM.tenToChuc = '';
    EMPTY_CUSTOM.coSo = '';
    EMPTY_CUSTOM.diaChi = '';
    EMPTY_CUSTOM.soDienThoai = '';
    EMPTY_CUSTOM.ngayNhanHoSo = null;
    EMPTY_CUSTOM.ngayCamKet = null;
    EMPTY_CUSTOM.nguoiLamCamKet = '';
    EMPTY_CUSTOM.ghiChu = '';
    EMPTY_CUSTOM.listItems = [];
    EMPTY_CUSTOM.huyen = '00000000-0000-0000-0000-000000000000';
    this.data = EMPTY_CUSTOM;
  }

  convert_date(string_date: string) {
    var result = moment.utc(string_date, "DD/MM/YYYY");
    return result
  }

  convert_date_string(string_date: string) {
    if (string_date == null) {
      return null;
    }
    let date = string_date.split("T")[0];
    let list = date.split("-"); //["year", "month", "day"]
    let result = list[2] + "/" + list[1] + "/" + list[0]
    return result
  }


  loadBusiness() {
    this.commonService.getBusiness().subscribe((res: any) => {
      const businesses = [
        {
          id: '00000000-0000-0000-0000-000000000000',
          text: '-- Chọn --',
        },
      ]
      for (var item of res.items) {
        let obj_business = {
          id: item.businessId,
          text: item.businessNameVi,
          address: item.diaChiTruSo,
          phoneNumber: item.soDienThoai,
          nguoiDaiDien: item.nguoiDaiDien,
          giayPhepSanXuat: item.giayPhepSanXuat,
          ngayCapPhep: item.ngayCapPhep
        }
        businesses.push(obj_business)
      }
      this.businessData = businesses
      this.loadForm();
    })
  }

  openModal() {
    const modalRef = this.modalService.open(EditItemsComponent, { size: 'lg' });
    modalRef.componentInstance.typeData = this.typeData;
    modalRef.result.then(({ ...res }) =>
      res,
      (res) => {
        if (typeof (res) == "object" && res) {
          this.listItems.push(res);
        }
      }
    );
  }

  loadTypeItemsData() {
    const sb = this.commonService.GetConfig("TYPE_ITEMS").subscribe((res: any) => {
      const data = [
        {
          id: '00000000-0000-0000-0000-000000000000',
          text: '-- Chọn --',
        },
        ...res.items.listConfig.map((item: any) => ({
          id: item.categoryId,
          text: item.categoryName,
        }))
      ]
      this.typeData = data;
    })

    this.subscriptions.push(sb)
  }

  deleteItem(index: any) {
    this.listItems.splice(index, 1)
  }
}
