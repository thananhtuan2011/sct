import { ChangeDetectorRef, Component, Input, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal, NgbDateAdapter, NgbDateParserFormatter } from '@ng-bootstrap/ng-bootstrap';
import * as moment from 'moment';
import { of, Subscription } from 'rxjs';
import { catchError, finalize, first, tap } from 'rxjs/operators';
import { Options } from 'select2';
import Swal from 'sweetalert2';

import { MultiLevelSalesParticipantsModel } from '../../../_models/multi-level-sales-participants.model';
import { MultiLevelSalesParticipantsPageService } from '../../../_services/multi-level-sales-participants-page.service';

const EMPTY_CUSTOM: MultiLevelSalesParticipantsModel = {
  id: '',
  multiLevelSalesParticipantsId: '00000000-0000-0000-0000-000000000000',
  multiLevelSalesParticipantsCode: '',
  participantsName: '',
  birthday: null,
  phoneNumber: '',
  identityCardNumber: null,
  dateOfIssuance: null,
  placeOfIssue: '',
  gender: 0,
  joinDate: null,
  province: '',
  address: '',
};

@Component({
  selector: 'app-edit-modal',
  templateUrl: './edit-modal.component.html',
  styleUrls: ['./edit-modal.component.scss'],

})
export class EditMultiLevelSalesParticipantsModalComponent implements OnInit, OnDestroy {
  @Input() id: any;
  isLoading$: any;
  multiLevelSalesParticipantsData: MultiLevelSalesParticipantsModel;
  formGroup: FormGroup;
  options: Options;

  private subscriptions: Subscription[] = [];
  GenderData: any = [
    {
      id: 0,
      text: '-- Chọn --'
    },
    {
      id: 1,
      text: 'Nam'
    },
    {
      id: 2,
      text: 'Nữ'
    },
  ];

  mindate: any = {year: 1960, month: 1, day: 1}
  maxdate: any = {year: moment().year(), month: moment().month(), day: moment().date()}

  constructor(
    private multiLevelSalesParticipantsService: MultiLevelSalesParticipantsPageService,
    private fb: FormBuilder, public modal: NgbActiveModal,
  ) { }

  ngOnInit(): void {
    this.isLoading$ = this.multiLevelSalesParticipantsService.isLoading$;
    this.loadMultiLevelSalesParticipants();
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

  convert_date_string(string_date: string) {
    var date = string_date.split("T")[0];
    var list = date.split("-"); //["year", "month", "day"]
    var result = list[2] + "/" + list[1] + "/" + list[0]
    return result
  }

  loadMultiLevelSalesParticipants() {
    if (!this.id) {
      this.clear();
      this.loadForm();
    } else {
      const sb = this.multiLevelSalesParticipantsService.getItemById(this.id).pipe(
        first(),
        catchError((errorMessage) => {
          this.modal.dismiss(errorMessage);
          return of(EMPTY_CUSTOM);
        })
      ).subscribe((res: any) => {
        this.multiLevelSalesParticipantsData = res.items[0];
        this.loadForm();
        this.formGroup.controls['Birthday'].setValue(this.convert_date_string(this.formGroup.controls['Birthday'].value));
        this.formGroup.controls['DateOfIssuance'].setValue(this.convert_date_string(this.formGroup.controls['DateOfIssuance'].value));
        this.formGroup.controls['JoinDate'].setValue(this.convert_date_string(this.formGroup.controls['JoinDate'].value));
        this.formGroup.updateValueAndValidity();
      });
      this.subscriptions.push(sb);
    }
  }

  loadForm() {
    this.formGroup = this.fb.group({
      MultiLevelSalesParticipantsCode: [this.multiLevelSalesParticipantsData.multiLevelSalesParticipantsCode, Validators.required],
      ParticipantsName: [this.multiLevelSalesParticipantsData.participantsName, Validators.required],
      Birthday: [this.multiLevelSalesParticipantsData.birthday, Validators.required],
      PhoneNumber: [this.multiLevelSalesParticipantsData.phoneNumber, Validators.required],
      IdentityCardNumber: [this.multiLevelSalesParticipantsData.identityCardNumber, Validators.required],
      DateOfIssuance: [this.multiLevelSalesParticipantsData.dateOfIssuance, Validators.required],
      PlaceOfIssue: [this.multiLevelSalesParticipantsData.placeOfIssue, Validators.required],
      Gender: [this.multiLevelSalesParticipantsData.gender, Validators.required],
      JoinDate: [this.multiLevelSalesParticipantsData.joinDate, Validators.required],
      Province: [this.multiLevelSalesParticipantsData.province, Validators.required],
      Address: [this.multiLevelSalesParticipantsData.address, Validators.required],
    });
  }

  clear() {
    EMPTY_CUSTOM.multiLevelSalesParticipantsId = '00000000-0000-0000-0000-000000000000',
    EMPTY_CUSTOM.multiLevelSalesParticipantsCode = '',
    EMPTY_CUSTOM.participantsName = '',
    EMPTY_CUSTOM.birthday = null,
    EMPTY_CUSTOM.phoneNumber = '',
    EMPTY_CUSTOM.identityCardNumber = null,
    EMPTY_CUSTOM.dateOfIssuance = null,
    EMPTY_CUSTOM.placeOfIssue = '',
    EMPTY_CUSTOM.gender = 0,
    EMPTY_CUSTOM.joinDate = null,
    EMPTY_CUSTOM.province = ''
    EMPTY_CUSTOM.address = '',
    this.multiLevelSalesParticipantsData = EMPTY_CUSTOM;
  }

  private prepareData() {
    const formData = this.formGroup.value;
    this.multiLevelSalesParticipantsData.multiLevelSalesParticipantsCode = formData.MultiLevelSalesParticipantsCode;
    this.multiLevelSalesParticipantsData.participantsName = formData.ParticipantsName;
    this.multiLevelSalesParticipantsData.birthday = this.convert_date(formData.Birthday);
    this.multiLevelSalesParticipantsData.phoneNumber = formData.PhoneNumber;
    this.multiLevelSalesParticipantsData.identityCardNumber = formData.IdentityCardNumber;
    this.multiLevelSalesParticipantsData.dateOfIssuance = this.convert_date(formData.DateOfIssuance);
    this.multiLevelSalesParticipantsData.placeOfIssue = formData.PlaceOfIssue;
    this.multiLevelSalesParticipantsData.gender = formData.Gender;
    this.multiLevelSalesParticipantsData.joinDate = this.convert_date(formData.JoinDate);
    this.multiLevelSalesParticipantsData.province = formData.Province;
    this.multiLevelSalesParticipantsData.address = formData.Address;
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
    const sbUpdate = this.multiLevelSalesParticipantsService.update(this.multiLevelSalesParticipantsData).pipe(
      tap(() => {
        this.modal.close();
      }),
      catchError((errorMessage) => {
        this.modal.dismiss(errorMessage);
        return of(this.multiLevelSalesParticipantsData);
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
    const sbCreate = this.multiLevelSalesParticipantsService.create(this.multiLevelSalesParticipantsData).pipe(
      tap(() => {
        this.modal.close();
      }),
      catchError((errorMessage) => {
        this.modal.dismiss(errorMessage);
        return of(this.multiLevelSalesParticipantsData);
      }),
    ).subscribe((res: any) => {
      Swal.fire({
        icon: res.status == 1 ? 'success' : 'error',
        title: res.status == 1 ? 'Thêm mới thành công' : 'Thêm mới thất bại',
        confirmButtonText: 'Xác nhận',
        text: res.status == 0 ? res.error.msg : 'Thêm mới ' + (res.status == 1 ? 'thành công' : 'thất bại'),
      });
      this.multiLevelSalesParticipantsData = EMPTY_CUSTOM
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
}
