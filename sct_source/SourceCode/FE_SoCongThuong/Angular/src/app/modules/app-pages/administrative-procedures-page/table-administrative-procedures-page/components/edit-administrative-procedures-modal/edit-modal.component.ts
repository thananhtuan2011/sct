import { ChangeDetectorRef, Component, Input, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, ValidatorFn, Validators } from '@angular/forms';
import { NgbActiveModal, NgbDateAdapter, NgbDateParserFormatter, NgbDateStruct } from '@ng-bootstrap/ng-bootstrap';
import { of, Subscription } from 'rxjs';
import { catchError, finalize, first, tap } from 'rxjs/operators';
import { Options } from 'select2';
import Swal from 'sweetalert2';
import * as moment from 'moment';

import { AdministrativeProceduresModel } from '../../../_models/administrative-procedures.model';
import { AdministrativeProceduresPageService } from '../../../_services/administrative-procedures-page.service';
import { DateValidator } from '../../../../../../_metronic/shared/pipe/CustomNgbDate/ValidatorDate';

const EMPTY_CUSTOM: AdministrativeProceduresModel = {
  id: '',
  administrativeProceduresId: '00000000-0000-0000-0000-000000000000',
  administrativeProceduresField: '00000000-0000-0000-0000-000000000000',
  administrativeProceduresCode: '',
  status: 0,
  receptionForm: 0,
  administrativeProceduresName: '',
  amountOfRecords: null,
  dayReception: null,
  settlementTerm: null,
  finishDay: null,
};

@Component({
  selector: 'app-edit-modal',
  templateUrl: './edit-modal.component.html',
  styleUrls: ['./edit-modal.component.scss'],

})
export class EditAdministrativeProceduresModalComponent implements OnInit, OnDestroy {
  @Input() id: any;
  @Input() type: any = null;
  @Input() page: any ;
  isLoading$: any;
  administrativeProceduresData: AdministrativeProceduresModel;
  formGroup: FormGroup;
  options: Options;

  administrativeProceduresFieldData: any = [];

  statusData: any = [
    {
      id: 0,
      text: "-- Chọn --",
    },
    {
      id: 1,
      text: "Chưa xử lý",
    },
    {
      id: 2,
      text: "Đang xử lý",
    },
    {
      id: 3,
      text: "Đã xử lý",
    },
  ];

  receptionFormData: any = [
    {
      id: 0,
      text: "-- Chọn --",
    },
    {
      id: 1,
      text: "Trực tiếp",
    },
    {
      id: 2,
      text: "Trực tuyến",
    },
  ];

  MinDateReception: NgbDateStruct = { day: 1, month: 1, year: 1975 };
  MinDateSettlementTerm: NgbDateStruct = { day: 1, month: 1, year: 1975 };
  MinDateFinishDay: NgbDateStruct = { day: 1, month: 1, year: 1975 };
  MaxDateFinishDay: NgbDateStruct = { day: Number(moment().format('D')), month: Number(moment().format('M')), year: Number(moment().format('YYYY')) };

  private subscriptions: Subscription[] = [];

  constructor(
    private administrativeProceduresPageService: AdministrativeProceduresPageService,
    private fb: FormBuilder, public modal: NgbActiveModal,
    private cd: ChangeDetectorRef,
  ) { }

  ngOnInit(): void {
    this.isLoading$ = this.administrativeProceduresPageService.isLoading$;
    this.loadFieldData();
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

  loadFieldData() {
    this.administrativeProceduresPageService.loadField().subscribe((res: any) => {
      const field_data = [
        {
          id: '00000000-0000-0000-0000-000000000000',
          text: "-- Chọn --",
          piority: 0
        },
      ]

      for (var i of res.items) {
        let obj = {
          id: i.categoryId,
          text: i.categoryName,
          piority: i.piority,
        };
        field_data.push(obj);
      }

      this.administrativeProceduresFieldData = field_data.sort((i1, i2) => {
        if (i1.piority > i2.piority) {
          return 1;
        }
        if (i1.piority < i2.piority) {
          return -1;
        }
        return 0;
      });

      this.loadAdministrativeProcedures();
    })
  }

  loadAdministrativeProcedures() {
    if (!this.id) {
      this.clear();
      this.loadForm();
    } else {
      const sb = this.administrativeProceduresPageService.getItemById(this.id).pipe(
        first(),
        catchError((errorMessage) => {
          this.modal.dismiss(errorMessage);
          return of(EMPTY_CUSTOM);
        })
      ).subscribe((res: any) => {
        this.administrativeProceduresData = res.items[0];
        if(this.administrativeProceduresData.dayReception) {
          const [day, month, year] = this.administrativeProceduresData.dayReception.split('/').map(Number);
          this.MinDateSettlementTerm = { day, month, year };
          this.MinDateFinishDay = { day, month, year };
        }
        this.loadForm();
        
        if (this.type) {
          this.formGroup.updateValueAndValidity();
          this.formGroup.disable();
        }
      });
      this.subscriptions.push(sb);
    }
  }

  loadForm() {
    this.formGroup = this.fb.group({
      AdministrativeProceduresField: [this.administrativeProceduresData.administrativeProceduresField],
      AdministrativeProceduresCode: [this.administrativeProceduresData.administrativeProceduresCode, Validators.required],
      Status: [this.administrativeProceduresData.status],
      ReceptionForm: [this.administrativeProceduresData.receptionForm],
      AdministrativeProceduresName: [this.administrativeProceduresData.administrativeProceduresName, Validators.required],
      AmountOfRecords: [this.administrativeProceduresData.amountOfRecords, Validators.required],
      DayReception: [this.administrativeProceduresData.dayReception, Validators.required],
      SettlementTerm: [this.administrativeProceduresData.settlementTerm, Validators.required],
      FinishDay: [this.administrativeProceduresData.finishDay],
    });
    this.formGroup.controls.DayReception.valueChanges.subscribe(x => {
      if (x && !this.type) {
        const [day, month, year] = x.split('/').map(Number);
        this.MinDateSettlementTerm = { day, month, year };
        this.MinDateFinishDay = { day, month, year };
        this.formGroup.controls.SettlementTerm.setValue(null);
        this.formGroup.controls.FinishDay.setValue(null);
        this.formGroup.updateValueAndValidity();
      } else {
        this.MinDateSettlementTerm = { day: 1, month: 1, year: 1975 };
        this.MinDateFinishDay = { day: 1, month: 1, year: 1975 };
      }
    })
  }

  clear() {
    EMPTY_CUSTOM.administrativeProceduresId = '00000000-0000-0000-0000-000000000000',
    EMPTY_CUSTOM.administrativeProceduresField = '00000000-0000-0000-0000-000000000000',
    EMPTY_CUSTOM.administrativeProceduresCode = '',
    EMPTY_CUSTOM.status = 0,
    EMPTY_CUSTOM.receptionForm = 0,
    EMPTY_CUSTOM.administrativeProceduresName = '',
    EMPTY_CUSTOM.amountOfRecords = null,
    EMPTY_CUSTOM.dayReception = null,
    EMPTY_CUSTOM.settlementTerm = null,
    EMPTY_CUSTOM.finishDay = null,
    this.administrativeProceduresData = EMPTY_CUSTOM;
  }

  private prepareData() {
    const formData = this.formGroup.value;
    this.administrativeProceduresData.administrativeProceduresField = formData.AdministrativeProceduresField,
    this.administrativeProceduresData.administrativeProceduresCode = formData.AdministrativeProceduresCode,
    this.administrativeProceduresData.status = formData.FinishDay ? 3 : formData.Status,
    this.administrativeProceduresData.receptionForm = formData.ReceptionForm,
    this.administrativeProceduresData.administrativeProceduresName = formData.AdministrativeProceduresName,
    this.administrativeProceduresData.amountOfRecords = formData.AmountOfRecords,
    this.administrativeProceduresData.dayReception = formData.DayReception,
    this.administrativeProceduresData.settlementTerm = formData.SettlementTerm,
    this.administrativeProceduresData.finishDay = formData.Status == 3 && !formData.FinishDay ? moment().format('DD/MM/YYYY') : formData.FinishDay
  }

  save() {
    this.prepareData();
    if (this.id) {
      this.edit();
    } else {
      this.create();
    }
  }

  check_formGroup() {
    if (this.formGroup.invalid) {
      this.formGroup.markAllAsTouched();
    }
    else {
      this.save();
    }
  }

  edit() {
    const sbUpdate = this.administrativeProceduresPageService.update(this.administrativeProceduresData).pipe(
      tap(() => {
        this.modal.close();
      }),
      catchError((errorMessage) => {
        this.modal.dismiss(errorMessage);
        return of(this.administrativeProceduresData);
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
    const sbCreate = this.administrativeProceduresPageService.create(this.administrativeProceduresData).pipe(
      tap(() => {
        this.modal.close();
      }),
      catchError((errorMessage) => {
        this.modal.dismiss(errorMessage);
        return of(this.administrativeProceduresData);
      }),
    ).subscribe((res: any) => {
      Swal.fire({
        icon: res.status == 1 ? 'success' : 'error',
        title: res.status == 1 ? 'Thêm mới thành công' : 'Thêm mới thất bại',
        confirmButtonText: 'Xác nhận',
        text: res.status == 0 ? res.error.msg : 'Thêm mới ' + (res.status == 1 ? 'thành công' : 'thất bại'),
      });
      this.administrativeProceduresData = EMPTY_CUSTOM
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

  prenventInputNonNumber(event: any) {
    if (event.which < 48 || event.which > 57) {
      event.preventDefault();
    }
  }

  onKeyDown(event: KeyboardEvent) {
    // Ngăn người dùng nhập bất kỳ phím nào vào ô input trừ Backspace
    if (event.key !== 'Backspace') {
      event.preventDefault();
    }
  }

  onPaste(event: ClipboardEvent) {
    // Ngăn người dùng dán bất kỳ nội dung nào vào ô input
    event.preventDefault();
  }
}
