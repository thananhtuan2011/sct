import { Component, Input, OnDestroy, OnInit } from '@angular/core';
import {
  FormBuilder,
  FormControl,
  FormGroup,
  Validators,
} from '@angular/forms';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { of, Subscription } from 'rxjs';
import { catchError, finalize, first, tap } from 'rxjs/operators';
import Swal from 'sweetalert2';

import { ListOfKeyEnergyUsersModel } from '../../../_models/list-of-key-energy-users.model';
import { ListOfKeyEnergyUsersPageService } from '../../../_services/list-of-key-energy-users-page.service';
import { Options } from 'select2';
import { CommonService } from 'src/app/_metronic/shared/services/common.service';
import * as moment from 'moment';

@Component({
  selector: 'app-edit-modal',
  templateUrl: './edit-modal.component.html',
  styleUrls: ['./edit-modal.component.scss'],
})
export class EditListOfKeyEnergyUsersModalComponent
  implements OnInit, OnDestroy {
  @Input() id: any;
  private subscriptions: Subscription[] = [];
  isLoading$: any;
  options: Options;
  editData: ListOfKeyEnergyUsersModel;
  formGroup: FormGroup;

  businessData: any;
  districtData: any;
  typeofprofessionData: any;
  show: boolean = false;

  constructor(
    private service: ListOfKeyEnergyUsersPageService,
    private fb: FormBuilder,
    public modal: NgbActiveModal,
    private commonService: CommonService
  ) { }

  ngOnInit(): void {
    this.isLoading$ = this.service.isLoading$;
    this.options = {
      theme: 'bootstrap5',
      templateSelection: this.templateSelection,
    };
    this.loadDistrict();
    this.loadTypeOfProfession();
    this.loadBusiness();
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

  loadBusiness() {
    this.commonService.getBusiness().subscribe((res: any) => {
      const data = [
        {
          id: '00000000-0000-0000-0000-000000000000',
          text: '-- Chọn --',
          districtId: '00000000-0000-0000-0000-000000000000',
          diaChiTruSo: '',
        },
        ...res.items.map((item: any) => ({
          id: item.businessId,
          text: item.businessNameVi,
          districtId: item.districtId,
          diaChiTruSo: item.diaChiTruSo,
        })),
      ];
      this.businessData = data;
      this.loadData();
    });
  }

  loadDistrict() {
    this.commonService.getDistrict().subscribe((res: any) => {
      const data = [
        {
          id: '00000000-0000-0000-0000-000000000000',
          text: '-- Chọn --',
        },
        ...res.items.map((item: any) => ({
          id: item.districtId,
          text: item.districtName,
        })),
      ];
      this.districtData = data;
    });
  }

  loadTypeOfProfession() {
    this.commonService.getTypeOfProfession().subscribe((res: any) => {
      const data = [
        {
          id: '00000000-0000-0000-0000-000000000000',
          text: '-- Chọn --',
        },
        ...res.items.map((item: any) => ({
          id: item.typeOfProfessionId,
          text: item.typeOfProfessionName,
        })),
      ];
      this.typeofprofessionData = data;
    });
  }

  loadData() {
    if (!this.id) {
      this.clear();
      this.loadForm();
    } else {
      const sb = this.service
        .getItemById(this.id)
        .pipe(
          first(),
          catchError((errorMessage) => {
            this.modal.dismiss(errorMessage);
            return of(this.clear());
          })
        )
        .subscribe((res: any) => {
          this.editData = res.items[0] as ListOfKeyEnergyUsersModel;
          this.loadForm();
        });
      this.subscriptions.push(sb);
    }
  }

  clear() {
    const EmptyModel = {
      listOfKeyEnergyUsersId: '00000000-0000-0000-0000-000000000000',
      businessId: '00000000-0000-0000-0000-000000000000',
      address: '',
      date: moment().year(),
      link: '',
      profession: '00000000-0000-0000-0000-000000000000',
      manufactProfession: '',
      note: '',
      district: '00000000-0000-0000-0000-000000000000',
      decision: '',
    } as ListOfKeyEnergyUsersModel;
    this.editData = EmptyModel;
    return EmptyModel;
  }

  loadForm() {
    this.formGroup = this.fb.group({
      BusinessId: [this.editData.businessId],
      Address: [this.editData.address, Validators.required],
      Link: [this.editData.link],
      District: [this.editData.district],
      Decision:  [this.editData.decision],
      Profession: [this.editData.profession],
      ManufactProfession: [this.editData.manufactProfession, Validators.required],
      Date: [this.editData.date],
      EnergyConsumption: [this.editData.energyConsumption],
      Note: [this.editData.note],
    });
    this.loadAdressBusiness();
    this.subscriptions.push(
      this.formGroup.controls.BusinessId.valueChanges.subscribe((x) => {
        this.loadAdressBusiness();
      })
    );
    this.show = true;
  }

  loadAdressBusiness() {
    const business = this.businessData.find(
      (x: any) => x.id === this.formGroup.controls.BusinessId.value
    );
    if (
      business &&
      business.districtId !== '00000000-0000-0000-0000-000000000000'
    ) {
      const districtName = this.districtData.find(
        (x: any) => x.id === business.districtId
      );
      this.formGroup.patchValue(
        {
          District: districtName.text,
          Address: business.diaChiTruSo,
        },
        { emitEvent: false }
      );
    } else {
      this.formGroup.patchValue(
        {
          District: '',
          Address: '',
        },
        { emitEvent: false }
      );
    }
  }

  private prepare() {
    const formData = this.formGroup.value;
    this.editData.businessId = formData.BusinessId;
    this.editData.address = formData.Address;
    this.editData.date = formData.Date;
    this.editData.link = formData.Link;
    this.editData.profession = formData.Profession;
    this.editData.manufactProfession = formData.ManufactProfession;
    this.editData.energyConsumption = formData.EnergyConsumption;
    this.editData.note = formData.Note;
    this.editData.decision = formData.Decision;
  }

  save() {
    this.prepare();
    if (this.id) {
      this.edit();
    } else {
      this.create();
    }
  }

  edit() {
    const sbUpdate = this.service
      .update(this.editData)
      .pipe(
        tap(() => {
          this.modal.close();
        }),
        catchError((errorMessage) => {
          this.modal.dismiss(errorMessage);
          return of(this.editData);
        })
      )
      .subscribe((res: any) => {
        Swal.fire({
          icon: res.status == 1 ? 'success' : 'error',
          title:
            res.status == 1 ? 'Chỉnh sửa thành công' : 'Chỉnh sửa thất bại',
          confirmButtonText: 'Xác nhận',
          text:
            res.status == 0
              ? res.error.msg
              : 'Chỉnh sửa ' + (res.status == 1 ? 'thành công' : 'thất bại'),
        });
      });
    this.subscriptions.push(sbUpdate);
  }

  create() {
    const sbCreate = this.service
      .create(this.editData)
      .pipe(
        tap(() => {
          this.modal.close();
        }),
        catchError((errorMessage) => {
          this.modal.dismiss(errorMessage);
          return of(this.editData);
        })
      )
      .subscribe((res: any) => {
        Swal.fire({
          icon: res.status == 1 ? 'success' : 'error',
          title: res.status == 1 ? 'Thêm mới thành công' : 'Thêm mới thất bại',
          confirmButtonText: 'Xác nhận',
          text:
            res.status == 0
              ? res.error.msg
              : 'Thêm mới ' + (res.status == 1 ? 'thành công' : 'thất bại'),
        });
        this.clear();
      });
    this.subscriptions.push(sbCreate);
  }

  ngOnDestroy(): void {
    this.subscriptions.forEach((sb) => sb.unsubscribe());
  }

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
    if (
      value == '00000000-0000-0000-0000-000000000000' ||
      value == null ||
      value == 0
    ) {
      control.setErrors({ defaultvalue: true });
    } else {
      control.setErrors(null);
    }
    return control.invalid && (control.touched || !control.pristine);
  }

  check_formGroup() {
    this.formGroup.markAllAsTouched();
    if (!this.formGroup.invalid) {
      this.save();
    }
  }
}
