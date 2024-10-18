import { ChangeDetectorRef, Component, Input, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal, NgbDateAdapter, NgbDateParserFormatter } from '@ng-bootstrap/ng-bootstrap';
import { of, Subscription } from 'rxjs';
import { catchError, finalize, first, tap } from 'rxjs/operators';
import Swal from 'sweetalert2';

import { Options } from 'select2';

import { AdminFormalitiesModel } from '../../../_models/adminformalities.model';
import { AdminFormalitiesPageService } from '../../../_services/adminformalities-page.service';

const EMPTY_CUSTOM: AdminFormalitiesModel = {
  id: '',
  adminFormalitiesId: '00000000-0000-0000-0000-000000000000',
  adminFormalitiesCode: '',
  adminFormalitiesName: '',
  fieldId: '00000000-0000-0000-0000-000000000000',
  dvclevel: 0,
  docUrl: '',
};

@Component({
  selector: 'app-edit-adminformalities-modal',
  templateUrl: './edit-adminformalities-modal.component.html',
  styleUrls: ['./edit-adminformalities-modal.component.scss'],

})
export class EditAdminFormalitiesModalComponent implements OnInit, OnDestroy {
  @Input() id: any;
  isLoading$: any;
  adminformalitiesData: AdminFormalitiesModel;
  formGroup: FormGroup;
  options: Options;
  show: boolean = false;

  private subscriptions: Subscription[] = [];
  FieldData: any;
  DVCLevelData: any = [
    {
      id: 0,
      text: '-- Chọn --',
    },
    {
      id: 1,
      text: 'Toàn trình'
    },
    {
      id: 2,
      text: 'Còn lại',
    },
  ];

  constructor(
    private adminformalitiesService: AdminFormalitiesPageService,
    private fb: FormBuilder, public modal: NgbActiveModal,
  ) { }

  ngOnInit(): void {
    this.isLoading$ = this.adminformalitiesService.isLoading$;
    this.loadFields();
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

  loadAdminformalities() {
    if (!this.id) {
      this.clearModel();
      this.loadForm();
    } else {
      const sb = this.adminformalitiesService.getItemById(this.id).pipe(
        first(),
        catchError((errorMessage) => {
          this.modal.dismiss(errorMessage);
          return of(EMPTY_CUSTOM);
        })
      ).subscribe((res: any) => {
        this.adminformalitiesData = res.items[0];
        this.loadForm();
      });
      this.subscriptions.push(sb);
    }
  }

  loadFields() {
    this.adminformalitiesService.loadField().subscribe((res: any) => {
      var fields = [
        {
          id: '00000000-0000-0000-0000-000000000000',
          text: '-- Chọn --',
          priority: 0,
        },
      ];
      for (var item of res.items) {
        let field = {
          id: item.fieldId,
          text: item.fieldName,
          priority: item.priority,
        }
        fields.push(field)
      }
      this.FieldData = fields.sort((i1, i2) => {
        if (i1.priority > i2.priority) {
          return 1;
        }
        if (i1.priority < i2.priority) {
          return -1;
        }
        return 0;
      })
      this.loadAdminformalities();
    })
  }

  loadForm() {
    this.formGroup = this.fb.group({
      AdminFormalitiesCode: [this.adminformalitiesData.adminFormalitiesCode, Validators.required],
      AdminFormalitiesName: [this.adminformalitiesData.adminFormalitiesName, Validators.required],
      FieldId: [this.adminformalitiesData.fieldId],
      DVCLevel: [this.adminformalitiesData.dvclevel],
      DocUrl: [this.adminformalitiesData.docUrl, Validators.required]
    });
    this.show = true;
  }

  save() {
    this.prepareAdminFormalities();
    if (this.adminformalitiesData.adminFormalitiesId != '00000000-0000-0000-0000-000000000000') {
      this.edit();
    } else {
      this.create();
    }
  }

  edit() {
    const sbUpdate = this.adminformalitiesService.update(this.adminformalitiesData).pipe(
      tap(() => {
        this.modal.close();
      }),
      catchError((errorMessage) => {
        this.modal.dismiss(errorMessage);
        return of(this.adminformalitiesData);
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
    const sbCreate = this.adminformalitiesService.create(this.adminformalitiesData).pipe(
      tap(() => {
        this.modal.close();
      }),
      catchError((errorMessage) => {
        this.modal.dismiss(errorMessage);
        return of(this.adminformalitiesData);
      }),
    ).subscribe((res: any) => {
      Swal.fire({
        icon: res.status == 1 ? 'success' : 'error',
        title: res.status == 1 ? 'Thêm mới thành công' : 'Thêm mới thất bại',
        confirmButtonText: 'Xác nhận',
        text: res.status == 0 ? res.error.msg : 'Thêm mới ' + (res.status == 1 ? 'thành công' : 'thất bại'),
      });
      this.adminformalitiesData = EMPTY_CUSTOM
    });
    this.subscriptions.push(sbCreate);
  }

  private prepareAdminFormalities() {
    const formData = this.formGroup.value;
    this.adminformalitiesData.adminFormalitiesCode = formData.AdminFormalitiesCode;
    this.adminformalitiesData.adminFormalitiesName = formData.AdminFormalitiesName;
    this.adminformalitiesData.fieldId = formData.FieldId;
    this.adminformalitiesData.dvclevel = formData.DVCLevel;
    this.adminformalitiesData.docUrl = formData.docUrl;
  }

  clearModel() {
    EMPTY_CUSTOM.adminFormalitiesId = '00000000-0000-0000-0000-000000000000'
    EMPTY_CUSTOM.adminFormalitiesCode = '',
    EMPTY_CUSTOM.adminFormalitiesName = '',
    EMPTY_CUSTOM.fieldId = '00000000-0000-0000-0000-000000000000',
    EMPTY_CUSTOM.dvclevel = 0,
    EMPTY_CUSTOM.docUrl = '',
    this.adminformalitiesData = EMPTY_CUSTOM
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
