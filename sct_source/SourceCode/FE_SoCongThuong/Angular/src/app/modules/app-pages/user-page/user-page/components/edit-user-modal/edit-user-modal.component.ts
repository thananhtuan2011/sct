import { Component, Input, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators, FormControl } from '@angular/forms';
import { NgbActiveModal, NgbDateAdapter, NgbDateParserFormatter } from '@ng-bootstrap/ng-bootstrap';
import { of, Subscription } from 'rxjs';
import { catchError, finalize, first, tap } from 'rxjs/operators';
import { Options } from 'select2';
import { SelectOptionData } from 'src/app/_metronic/shared/components/select-custom/select-custom.interface';
import { CommonService } from 'src/app/_metronic/shared/services/common.service';
import Swal from 'sweetalert2';
import { UserModel } from '../../../_models/user.model';
import { UserService } from '../../../_services/user.service';
import { ConfirmPasswordValidator } from './confirm-password.validator';
import { PasswordValidator } from '../edit-user-modal/validators-password.validator';

const EMPTY_CUSTOM: UserModel = {
  id: "00000000-0000-0000-0000-000000000000",
  fullname: '',
  username: '',
  password: '',
  deptId: "00000000-0000-0000-0000-000000000000",
  roleId: "00000000-0000-0000-0000-000000000000",
  phone: '',
  email: '',
  cccd: '',
  userId: "00000000-0000-0000-0000-000000000000",
  groupId: "00000000-0000-0000-0000-000000000000",
  levelUser: 0,
  areaId: "00000000-0000-0000-0000-000000000000",
};

@Component({
  selector: 'app-edit-user-modal',
  templateUrl: './edit-user-modal.component.html',
  styleUrls: ['./edit-user-modal.component.scss'],

})
export class EditUserModalComponent implements OnInit, OnDestroy {
  @Input() id: number;
  isLoading$: any;
  userData: UserModel;
  public formGroup: FormGroup;
  default_value = "00000000-0000-0000-0000-000000000000"
  public positonData: Array<SelectOptionData>;
  public departmemtData: Array<SelectOptionData>;
  public groupUserData: Array<SelectOptionData>;
  public districtData: any = [];
  public options: Options;
  public status: number = 0;
  private subscriptions: Subscription[] = [];
  constructor(
    private UserService: UserService,
    private fb: FormBuilder,
    public modal: NgbActiveModal,
    private common: CommonService,
  ) { }

  ngOnInit(): void {
    this.isLoading$ = this.UserService.isLoading$;
    this.loadPostion()
    this.loadDepartmemt()
    this.loadGroupUser()
    this.loadDistrict()
    this.options = {
      theme: 'bootstrap5',
      templateSelection: this.templateSelection
    }
  }

  public templateSelection = (state: any): JQuery | string => {
    if (!state.id) {
      return state.text;
    }
    return jQuery('<span class="form-select form-select-solid form-select-lg">' + state.text + '</span>');
  }

  loadPostion() {
    this.common.getListPostion().subscribe((res: any) => {
      if (res && res.status == 1) {
        const data = [
          {
            id: "00000000-0000-0000-0000-000000000000",
            text: '-- Chọn --'
          }
        ]
        for (var it of res.items) {
          let obj = {
            id: it.stateTitlesId,
            text: it.stateTitlesName
          }
          data.push(obj)
        }
        this.positonData = data
      }
      this.status += 1;
      this.checkStatus();
    }
    );
  }

  loadDepartmemt() {
    this.common.getListDepartmemt().subscribe((res: any) => {
      if (res && res.status == 1) {
        const data = [
          {
            id: "00000000-0000-0000-0000-000000000000",
            text: '-- Chọn --'
          }
        ]
        for (var it of res.items) {
          let obj = {
            id: it.stateUnitsId,
            text: it.stateUnitsName
          }
          data.push(obj)
        }
        this.departmemtData = data
      }
      this.status += 1;
      this.checkStatus();
    }
    );
  }

  loadGroupUser() {
    this.UserService.getListGroupUser().subscribe((res: any) => {
      if (res && res.status == 1) {
        const data = [
          {
            id: "00000000-0000-0000-0000-000000000000",
            text: '-- Chọn --'
          }
        ]
        for (var it of res.items) {
          let obj = {
            id: it.groupId,
            text: it.groupName
          }
          data.push(obj)
        }
        this.groupUserData = data
      }
      this.status += 1;
      this.checkStatus();
    }
    );
  }

  loadDistrict() {
    this.common.getDistrict().subscribe((res: any) => {
      if (res && res.status == 1) {
        const data = [
          {
            id: "00000000-0000-0000-0000-000000000000",
            text: '-- Chọn --'
          }
        ]
        for (var item of res.items) {
          let obj = {
            id: item.districtId,
            text: item.districtName,
          }
          data.push(obj)
        }
        this.districtData = data
      }
      this.status += 1;
      this.checkStatus();
    })
  }

  loadCommune() {
  }

  checkStatus() {
    if (this.status == 4) {
      this.loadCustom()
    }
  }

  clear() {
    EMPTY_CUSTOM.id = "00000000-0000-0000-0000-000000000000",
    EMPTY_CUSTOM.fullname = '',
    EMPTY_CUSTOM.username = '',
    EMPTY_CUSTOM.password = '',
    EMPTY_CUSTOM.deptId = "00000000-0000-0000-0000-000000000000",
    EMPTY_CUSTOM.roleId = "00000000-0000-0000-0000-000000000000",
    EMPTY_CUSTOM.phone = '',
    EMPTY_CUSTOM.email = '',
    EMPTY_CUSTOM.cccd = '',
    EMPTY_CUSTOM.userId = "00000000-0000-0000-0000-000000000000",
    EMPTY_CUSTOM.groupId = "00000000-0000-0000-0000-000000000000",
    EMPTY_CUSTOM.levelUser = 0,
    EMPTY_CUSTOM.areaId = "00000000-0000-0000-0000-000000000000"
  }

  loadCustom() {
    if (!this.id) {
      this.userData = EMPTY_CUSTOM;
      this.loadForm();
    } else {
      const sb = this.UserService.getItemById(this.id).pipe(
        first(),
        catchError((errorMessage) => {
          this.modal.dismiss(errorMessage);
          return of(EMPTY_CUSTOM);
        })
      ).subscribe((res: any) => {
        this.userData = res.items[0];
        this.loadForm();
      });
      this.subscriptions.push(sb);
    }
  }

  loadForm() {
    this.formGroup = this.fb.group({
      HoVaTen: [this.userData.fullname, Validators.compose([Validators.required, Validators.minLength(3), Validators.maxLength(50)])],
      TenDangNhap: [this.userData.username, Validators.compose([Validators.required, Validators.minLength(3), Validators.maxLength(100)])],
      MatKhau: [this.userData.password, [Validators.minLength(8), Validators.required, PasswordValidator()]],
      ChucVu: [this.userData.roleId, [Validators.required]],
      DonVi: [this.userData.deptId, [Validators.required]],
      SoDienThoai: [this.userData.phone, Validators.compose([Validators.required, Validators.minLength(10), Validators.pattern(/^-?(0|[0-9]\d*)?$/)])],
      Email: [this.userData.email, [Validators.minLength(3), Validators.email]],
      CCCD: [this.userData.cccd, Validators.compose([Validators.minLength(9), Validators.pattern(/^-?(0|[0-9]\d*)?$/)])],
      NhapLaiMatKhau: [this.userData.password, [Validators.minLength(8), Validators.required]],
      NhomNguoiDung: [this.userData.groupId, [Validators.required]],
      Cap: [this.userData.levelUser],
      Huyen: [this.userData.areaId]
    },
    {
      validator: ConfirmPasswordValidator.MatchPassword
    }
    );
  }

  save() {
    this.prepareCustomer();
    if (this.userData.userId && this.userData.userId !== this.default_value) {
      this.edit();
    } else {
      this.create();
    }
  }

  edit() {
    this.userData.id = this.id + "";
    const sbUpdate = this.UserService.update(this.userData).pipe(
      tap(() => {
        this.modal.close();
      }),
      catchError((errorMessage) => {
        this.modal.dismiss(errorMessage);
        return of(this.userData);
      }),
    ).subscribe((res: any) => {
      Swal.fire({
        icon: res.status == 1 ? 'success' : 'error',
        title: res.status == 1 ? 'Chỉnh sửa thành công' : 'Chỉnh sửa thất bại',
        confirmButtonText: 'Xác nhận',
        text: '' + (res.status == 1 ? 'Chỉnh sửa thành công' : res.error.msg),
      });
    });
    this.subscriptions.push(sbUpdate);
  }

  create() {
    const sbCreate = this.UserService.create(this.userData).pipe(
      tap(() => {
        this.modal.close();
      }),
      catchError((errorMessage) => {

        this.modal.dismiss(errorMessage);
        return of(this.userData);
      }),
    ).subscribe((res: any) => {
      Swal.fire({
        icon: res.status == 1 ? 'success' : 'error',
        title: res.status == 1 ? 'Thêm mới thành công' : 'Thêm mới thất bại',
        confirmButtonText: 'Xác nhận',
        text: (res.status == 1 ? 'Thêm mới thành công' : res.error.msg),
      });
      this.clear()
      this.userData = EMPTY_CUSTOM
    });
    this.subscriptions.push(sbCreate);
  }

  private prepareCustomer() {
    const formData = this.formGroup.value;
    this.userData.fullname = formData.HoVaTen;
    this.userData.username = formData.TenDangNhap;
    this.userData.password = formData.MatKhau;
    this.userData.deptId = formData.DonVi;
    this.userData.roleId = formData.ChucVu;
    this.userData.phone = formData.SoDienThoai;
    this.userData.email = formData.Email;
    this.userData.cccd = formData.CCCD;
    this.userData.groupId = formData.NhomNguoiDung;
    this.userData.levelUser = formData.Cap;
    this.userData.areaId = formData.Huyen;
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

  controlHasErrorSelect(controlName: any): boolean {
    const control = this.formGroup.controls[controlName];
    if (control.value === this.default_value) {
      control?.setErrors({ select: true });
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

  showDistrict(){
    const condition = this.formGroup.controls.Cap.value;
    return condition === 1 ? true : false;
  }

  selectProvinceLevel(){
    this.formGroup.controls.Huyen.setValue("00000000-0000-0000-0000-000000000000");
    this.formGroup.controls.Huyen.updateValueAndValidity();
  }
}
