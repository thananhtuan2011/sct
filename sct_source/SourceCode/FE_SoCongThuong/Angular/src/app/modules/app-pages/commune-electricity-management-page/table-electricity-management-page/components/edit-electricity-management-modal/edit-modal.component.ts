import { ChangeDetectorRef, Component, Input, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { of, Subscription } from 'rxjs';
import { catchError, finalize, first, tap } from 'rxjs/operators';
import Swal from 'sweetalert2';

import { CommuneElectricityManagementModel } from '../../../_models/commune-electricity-management.model';
import { EditCommuneElectricityManagementPageService } from '../../../_services/commune-electricity-management-page.service';
import { Options } from 'select2';
import { CommonService } from 'src/app/_metronic/shared/services/common.service';

@Component({
  selector: 'app-edit-modal',
  templateUrl: './edit-modal.component.html',
  styleUrls: ['./edit-modal.component.scss'],

})
export class EditCommuneElectricityManagementModalComponent implements OnInit, OnDestroy {
  @Input() id: any;
  @Input() type: any;
  private subscriptions: Subscription[] = [];
  isLoading$: any;
  options: Options;
  editData: CommuneElectricityManagementModel;
  formGroup: FormGroup;

  stageData: any = [];
  districtData: any = [];
  communeData: any = [];
  communeDataFilter: any = [];
  statusAPI: number = 0;

  titleStart: string = '';
  titleEnd: string = '';
  show: boolean = false;
  apiComplete: number = 0;

  constructor(
    private service: EditCommuneElectricityManagementPageService,
    private commonService: CommonService,
    private fb: FormBuilder,
    public modal: NgbActiveModal,
    private cd: ChangeDetectorRef,
  ) { }

  ngOnInit(): void {
    this.isLoading$ = this.service.isLoading$;
    this.options = {
      theme: 'bootstrap5',
      templateSelection: this.templateSelection,
    };

    this.loadStage();
    this.loadDistrict();
    this.loadCommune();
  }

  public templateSelection = (state: any): JQuery | string => {
    if (!state.id) {
      return state.text;
    }
    return jQuery('<span class="form-select form-select-solid form-select-lg">' + state.text + '</span>');
  }

  loadStage() {
    this.service.loadStage().subscribe((res: any) => {
      const data = [
        { id: "00000000-0000-0000-0000-000000000000", text: "-- Chọn --" },
        ...res.items.map((item: any) => ({
          id: item.stageId,
          text: item.stageName,
          yearStart: item.startYear,
          yearEnd: item.endYear,
        }))
      ]
      this.stageData = data.sort((a, b) => a.text.localeCompare(b.text));
      this.loadData();
    })
  }

  loadDistrict() {
    this.service.loadDistrict().subscribe((res: any) => {
      const data = [
        { id: "00000000-0000-0000-0000-000000000000", text: "-- Chọn --" },
        ...res.items.map((item: any) => ({
          id: item.districtId,
          text: item.districtName,
        }))
      ]
      this.districtData = data.sort((a, b) => a.text.localeCompare(b.text));
      this.loadData();
    })
  }

  loadCommune() {
    this.service.loadCommune().subscribe((res: any) => {
      const data = [
        { id: "00000000-0000-0000-0000-000000000000", text: "-- Chọn --" },
        ...res.items.map((item: any) => ({
          id: item.communeId,
          text: item.communeName,
          districtId: item.districtId
        }))
      ]
      this.communeData = data.sort((a, b) => a.text.localeCompare(b.text));
      this.communeDataFilter = data.sort((a, b) => a.text.localeCompare(b.text));
      this.loadData();
    })
  }

  loadData() {
    this.apiComplete++
    if(this.apiComplete != 3) {
      return
    }
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
        this.editData = res.items[0] as CommuneElectricityManagementModel;
        const Index = this.stageData.findIndex((y: any) => y.id == this.editData.stageId);
        const StageItem = this.stageData[Index];
        this.titleStart = ` đến hết năm ${StageItem.yearStart}`;
        this.titleEnd = ` đến hết năm ${StageItem.yearEnd}`;
        this.loadForm();
      });
      this.subscriptions.push(sb);
    }
  }

  clear() {
    const EmptyModel = {
      communeElectricityManagementId: '00000000-0000-0000-0000-000000000000',
      stageId: '00000000-0000-0000-0000-000000000000',
      districtId: '00000000-0000-0000-0000-000000000000',
      communeId: '00000000-0000-0000-0000-000000000000',
      content41Start: false,
      content42Start: false,
      target4Start: false,
      content41End: false,
      content42End: false,
      target4End: false,
      note: '',
    } as CommuneElectricityManagementModel;
    this.editData = EmptyModel;
    return EmptyModel;
  }

  loadForm() {
    this.formGroup = this.fb.group({
      StageId: [this.editData.stageId],
      DistrictId: [this.editData.districtId],
      CommuneId: [this.editData.communeId],
      Content41Start: [this.editData.content41Start],
      Content42Start: [this.editData.content42Start],
      Target4Start: [this.editData.target4Start],
      Content41End: [this.editData.content41End],
      Content42End: [this.editData.content42End],
      Target4End: [this.editData.target4End],
      Note: [this.editData.note]
    })

    this.show = true;

    const districtChange = this.formGroup.controls.DistrictId.valueChanges.subscribe(x => {
      if (x != '00000000-0000-0000-0000-000000000000' && !this.type) {
        this.communeDataFilter = this.communeData.filter((y: any) => y.districtId == x || y.id == '00000000-0000-0000-0000-000000000000');
      } else {
        this.communeDataFilter = this.communeData;
      }
      if (!this.type) {
        this.formGroup.controls.CommuneId.setValue('00000000-0000-0000-0000-000000000000', { emitEvent: false });
      }
    });
    this.subscriptions.push(districtChange)

    const stageChange = this.formGroup.controls.StageId.valueChanges.subscribe(x => {
      if (x != '00000000-0000-0000-0000-000000000000' && !this.type) {
        const Index = this.stageData.findIndex((y: any) => y.id == x);
        const StageItem = this.stageData[Index];
        this.titleStart = ` đến hết năm ${StageItem.yearStart}`;
        this.titleEnd = ` đến hết năm ${StageItem.yearEnd}`;
      } else {
        this.titleStart = '';
        this.titleEnd = '';
      }
    })
    this.subscriptions.push(stageChange)

    if (this.type == "view") {
      this.formGroup.disable();
    }
  }

  private prepare() {
    const formData = this.formGroup.value;
    this.editData.stageId = formData.StageId;
    this.editData.districtId = formData.DistrictId;
    this.editData.communeId = formData.CommuneId;
    this.editData.content41Start = formData.Content41Start;
    this.editData.content42Start = formData.Content42Start;
    this.editData.target4Start = formData.Target4Start;
    this.editData.content41End = formData.Content41End;
    this.editData.content42End = formData.Content42End;
    this.editData.target4End = formData.Target4End;
    this.editData.note = formData.Note;
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
    if (value == '00000000-0000-0000-0000-000000000000') {
      control.setErrors({ defaultvalue: true });
      return control.invalid && (control.touched || control.dirty);
    }
    else {
      control.setErrors(null);
      return false;
    }
  }

  // isCheckBox(Type: string) {
  //   const Content41 = this.formGroup.controls[`Content41${Type}`];
  //   const Content42 = this.formGroup.controls[`Content42${Type}`];
  //   const Target4 = this.formGroup.controls[`Target4${Type}`];
  //   if (!Content41.value && !Content42.value && !Target4.value) {
  //     this.formGroup.setErrors({ noCheckBox: true })
  //     return this.formGroup.invalid && (Content41.touched || Content42.touched || Target4.touched);
  //   } else {
  //     this.formGroup.setErrors(null)
  //     return false;
  //   }
  // }

  check_formGroup() {
    this.formGroup.markAllAsTouched();
    if (!this.formGroup.invalid) {
      this.save()
    }
  }
}
