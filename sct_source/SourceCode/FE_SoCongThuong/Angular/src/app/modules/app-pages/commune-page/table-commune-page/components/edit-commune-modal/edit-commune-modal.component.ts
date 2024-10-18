import { ChangeDetectorRef, Component, Input, OnDestroy, OnInit } from '@angular/core';
import { AbstractControl, FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal, NgbDateAdapter, NgbDateParserFormatter } from '@ng-bootstrap/ng-bootstrap';
import { of, Subscription } from 'rxjs';
import { catchError, finalize, first, tap } from 'rxjs/operators';
import Swal from 'sweetalert2';
import { CommuneModel } from '../../../_models/commune.model';
import { CommunePageService } from '../../../_services/commune-page.service';

import { SelectOptionData } from 'src/app/_metronic/shared/components/select-custom/select-custom.interface';
import { Options } from 'select2';

const EMPTY_CUSTOM: CommuneModel = {
  id: '',
  communeId: '',
  communeCode: '',
  communeName: '',
  districtId: '00000000-0000-0000-0000-000000000000',
};

@Component({
  selector: 'app-edit-commune-modal',
  templateUrl: './edit-commune-modal.component.html',
  styleUrls: ['./edit-commune-modal.component.scss'],

})
export class EditCommuneModalComponent implements OnInit, OnDestroy {
  @Input() id: any;
  isLoading$: any;
  communeData: CommuneModel;
  formGroup: FormGroup;


  private subscriptions: Subscription[] = [];

  public selectData: Array<SelectOptionData>;
  public options: Options;

  public default_value = "00000000-0000-0000-0000-000000000000"

  constructor(
    private communeService: CommunePageService,
    private fb: FormBuilder, public modal: NgbActiveModal,
  ) { }


  ngOnInit(): void {
    this.isLoading$ = this.communeService.isLoading$;
    this.loadDistrict()
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

  loadDistrict() {
    this.communeService.loadDistricts().subscribe(res => {
      const data = [
        {
          id: "00000000-0000-0000-0000-000000000000",
          text: '-- Chọn --'
        }
      ]
      for (var district of res.items) {
        let obj = {
          id: district.districtId,
          text: district.districtName
        }
        data.push(obj)
      }
      this.selectData = data.sort((a, b) => {
        if (a.text < b.text) {
          return -1;
        }
        if (a.text > b.text) {
          return 1;
        }
        return 0;
      });
      this.loadCommune();
    }
    );
  }

  loadCommune() {
    if (!this.id) {
      EMPTY_CUSTOM.communeCode = '';
      EMPTY_CUSTOM.communeName = '';
      EMPTY_CUSTOM.districtId = '00000000-0000-0000-0000-000000000000';
      this.communeData = EMPTY_CUSTOM;
      this.loadForm();
    } else {
      const sb = this.communeService.getItemById(this.id).pipe(
        first(),
        catchError((errorMessage) => {
          this.modal.dismiss(errorMessage);
          return of(EMPTY_CUSTOM);
        })
      ).subscribe((res: any) => {
        this.communeData = res.items[0];
        this.default_value = res.items[0].districtId;
        this.loadForm();
      });
      this.subscriptions.push(sb);
    }
  }

  loadForm() {
    this.formGroup = this.fb.group({
      CommuneCode: [this.communeData.communeCode, Validators.compose([Validators.required, Validators.pattern("[a-zA-Z0-9]{1,15}")])], //,Validators.minLength(3), Validators.maxLength(100)
      CommuneName: [this.communeData.communeName, Validators.required],  //Validators.minLength(3), Validators.maxLength(100)
      DistrictId: [this.communeData.districtId],
    });
  }

  save() {
    this.prepareCommune();
    if (this.id) {
      this.edit();
    } else {
      this.communeData.communeId = "00000000-0000-0000-0000-000000000000"
      this.create();
    }
  }

  edit() {
    const sbUpdate = this.communeService.update(this.communeData).pipe(
      tap(() => {
        this.modal.close();
      }),
      catchError((errorMessage) => {
        this.modal.dismiss(errorMessage);
        return of(this.communeData);
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
    const sbCreate = this.communeService.create(this.communeData).pipe(
      tap(() => {
        this.modal.close();
      }),
      catchError((errorMessage) => {
        this.modal.dismiss(errorMessage);
        return of(this.communeData);
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

  private prepareCommune() {
    const formData = this.formGroup.value;
    this.communeData.communeName = formData.CommuneName;
    this.communeData.communeCode = formData.CommuneCode;
    this.communeData.districtId = formData.DistrictId;
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

  isDefaultValue(controlName: any): boolean {
    const control = this.formGroup.controls[controlName];
    const value = control.value;
    const isdefaultvalue = (value == "00000000-0000-0000-0000-000000000000")
    if (isdefaultvalue) {
      control.setErrors({
        default_value: true
      })
    }
    return control.invalid && (control.dirty || control.touched);
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
}
