import { Component, Input, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { of, Subscription } from 'rxjs';
import { catchError, finalize, first, tap } from 'rxjs/operators';
import Swal from 'sweetalert2';

import { ElectricalProjectManagementModel } from '../../../_models/electrical-project-management.model';
import { ElectricalProjectManagementPageService } from '../../../_services/electrical-project-management-page.service';
import { Options } from 'select2';
import { CommonService } from 'src/app/_metronic/shared/services/common.service';

@Component({
  selector: 'app-edit-modal',
  templateUrl: './edit-modal.component.html',
  styleUrls: ['./edit-modal.component.scss'],
})

export class EditElectricalProjectManagementModalComponent implements OnInit, OnDestroy {
  @Input() id: any;
  @Input() type: any;
  @Input() listVoltageLevel: any;
  @Input() listListTypeOfConstruction: any;
  private subscriptions: Subscription[] = [];
  isLoading$: any;
  options: Options;
  editData: ElectricalProjectManagementModel;
  formGroup: FormGroup;

  districtData: { id: string, text: string }[];
  statusData: { id: any, text: string }[] = [
    {
      id: 0,
      text: "-- Chọn --"
    },
    {
      id: 1,
      text: "Đang vận hành"
    },
    {
      id: 2,
      text: "Chưa vận hành"
    },
    {
      id: 3,
      text: "Tạm ngưng vận hành"
    },
  ];
  show: boolean = false;
  typeOfConstruction: any = '';

  constructor(
    private service: ElectricalProjectManagementPageService,
    private fb: FormBuilder,
    public modal: NgbActiveModal,
    private commonService: CommonService,
  ) { }

  ngOnInit(): void {
    this.isLoading$ = this.service.isLoading$;
    this.options = {
      theme: 'bootstrap5',
      templateSelection: this.templateSelection,
    };
    this.loadDistrict();
  }

  public templateSelection = (state: any): JQuery | string => {
    if (!state.id) {
      return state.text;
    }
    return jQuery('<span class="form-select form-select-solid form-select-lg">' + state.text + '</span>');
  }

  loadDistrict() {
    this.commonService.getDistrict().subscribe((res: any) => {
      const data = [
        { id: "00000000-0000-0000-0000-000000000000", text: "-- Chọn --" },
        ...res.items.map((item: any) => ({
          id: item.districtId,
          text: item.districtName,
        }))
      ]
      this.districtData = data;
      this.loadData();
    })
  }
  
  loadData() {
    if (!this.id) {
      this.clear();
      this.loadForm();
    } else {
      const sb = this.service.getItemById(this.id).pipe(
        first(),
        catchError((errorMessage) => {
          this.modal.dismiss(errorMessage);
          return of(this.clear());
        })
      ).subscribe((res: any) => {
        this.editData = res.items[0] as ElectricalProjectManagementModel;
        this.typeOfConstruction = res.items[0].typeOfConstructionCode; 
        this.loadForm();
      });
      this.subscriptions.push(sb);
    }
  }

  clear() {
    const EmptyModel = {
      electricalProjectManagementId: '00000000-0000-0000-0000-000000000000',
      buildingCode: '',
      buildingName: '',
      district: '00000000-0000-0000-0000-000000000000',
      address: '',
      represent: '',
      status: 0,
      note: '',
      typeOfConstruction: '00000000-0000-0000-0000-000000000000',
      voltageLevel: '00000000-0000-0000-0000-000000000000',
      wattage: '',
      length: '',
      wireType: ''
    } as ElectricalProjectManagementModel;
    this.editData = EmptyModel;
    return EmptyModel;
  }

  loadForm() {
    this.formGroup = this.fb.group({
      BuildingCode: [this.editData.buildingCode, Validators.required],
      BuildingName: [this.editData.buildingName, Validators.required],
      District: [this.editData.district],
      Address: [this.editData.address, Validators.required],
      Represent: [this.editData.represent, Validators.required],
      Status: [this.editData.status],
      Note: [this.editData.note],
      VoltageLevel: [this.editData.voltageLevel],
      TypeOfConstruction: [this.editData.typeOfConstruction],
      Wattage: [this.editData.wattage],
      Length: [this.editData.length],
      WireType: [this.editData.wireType]
    })
    if(this.type) {
      this.formGroup.disable();
    }
    this.subscriptions.push(
      this.formGroup.controls.TypeOfConstruction.valueChanges.subscribe((x: any) => {
        const type = this.listListTypeOfConstruction.find((item: any) => item.id === x).code
        if (type == 'LINE') {
          this.formGroup.controls.Length.addValidators(Validators.required);
          this.formGroup.controls.Length.updateValueAndValidity();
          this.formGroup.controls.WireType.addValidators(Validators.required);
          this.formGroup.controls.WireType.updateValueAndValidity();
          this.formGroup.controls.Wattage.removeValidators(Validators.required);
          this.formGroup.controls.Wattage.updateValueAndValidity();
        } else {
          this.formGroup.controls.Length.removeValidators(Validators.required);
          this.formGroup.controls.Length.updateValueAndValidity();
          this.formGroup.controls.WireType.removeValidators(Validators.required);
          this.formGroup.controls.WireType.updateValueAndValidity();
          this.formGroup.controls.Wattage.addValidators(Validators.required);
          this.formGroup.controls.Wattage.updateValueAndValidity();
        }
      })
    )
    this.show = true;
  }

  private prepare() {
    const formData = this.formGroup.value;
    this.editData.buildingCode = formData.BuildingCode;
    this.editData.buildingName = formData.BuildingName;
    this.editData.district = formData.District;
    this.editData.address = formData.Address;
    this.editData.represent = formData.Represent;
    this.editData.status = formData.Status;
    this.editData.note = formData.Note;
    this.editData.voltageLevel = formData.VoltageLevel;
    this.editData.typeOfConstruction = formData.TypeOfConstruction;
    if(this.typeOfConstruction === 'LINE'){
      this.editData.wattage = '';
      this.editData.length = formData.Length;
      this.editData.wireType = formData.WireType;
    } else{    
      this.editData.length = '';
      this.editData.wireType = '';
      this.editData.wattage = formData.Wattage;
    }
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
    const sbUpdate = this.service.update(this.editData).pipe(
      tap(() => {
        this.modal.close();
      }),
      catchError((errorMessage) => {
        this.modal.dismiss(errorMessage);
        return of(this.editData);
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
    const sbCreate = this.service.create(this.editData).pipe(
      tap(() => {
        this.modal.close();
      }),
      catchError((errorMessage) => {
        this.modal.dismiss(errorMessage);
        return of(this.editData);
      }),
    ).subscribe((res: any) => {
      Swal.fire({
        icon: res.status == 1 ? 'success' : 'error',
        title: res.status == 1 ? 'Thêm mới thành công' : 'Thêm mới thất bại',
        confirmButtonText: 'Xác nhận',
        text: res.status == 0 ? res.error.msg : 'Thêm mới ' + (res.status == 1 ? 'thành công' : 'thất bại'),
      });
      this.clear();
    });
    this.subscriptions.push(sbCreate);
  }

  ngOnDestroy(): void {
    this.subscriptions.forEach(sb => sb.unsubscribe());
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
    if (value == '00000000-0000-0000-0000-000000000000' || value == 0) {
      control.setErrors({ defaultvalue: true });
    }
    else {
      control.setErrors(null);
    }
    return control.invalid && (control.touched || !control.pristine);
  }

  check_formGroup() {
    this.formGroup.markAllAsTouched();
    if (!this.formGroup.invalid) {
      this.save()
    }
  }

  gettype(event: any){
    this.typeOfConstruction = this.listListTypeOfConstruction.find((x: any) => x.id == event)?.code
  }
}