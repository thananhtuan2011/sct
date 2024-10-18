import { ChangeDetectorRef, Component, Input, OnDestroy, OnInit } from '@angular/core';
import { FormArray, FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal, NgbDateAdapter, NgbDateParserFormatter } from '@ng-bootstrap/ng-bootstrap';
import { of, Subscription } from 'rxjs';
import { catchError, finalize, first, tap } from 'rxjs/operators';
import { Options } from 'select2';
import Swal from 'sweetalert2';

import { ProcessAdministrativeProceduresModel } from '../../../_models/process-administrative-procedures.model';
import { ProcessAdministrativeProceduresPageService } from '../../../_services/process-administrative-procedures-page.service';
import { environment } from 'src/environments/environment';
import { HttpClient } from '@angular/common/http';

const EMPTY_CUSTOM: ProcessAdministrativeProceduresModel = {
  id: '',
  processAdministrativeProceduresId: '00000000-0000-0000-0000-000000000000',
  processAdministrativeProceduresField: '00000000-0000-0000-0000-000000000000',
  processAdministrativeProceduresCode: '',
  processAdministrativeProceduresName: '',
  processStep: [],
};

@Component({
  selector: 'app-edit-modal',
  templateUrl: './edit-modal.component.html',
  styleUrls: ['./edit-modal.component.scss'],
})
export class EditProcessAdministrativeProceduresModalComponent implements OnInit, OnDestroy {
  @Input() id: any;
  @Input() type: any;
  isLoading$: any;
  processAdministrativeProceduresData: ProcessAdministrativeProceduresModel;
  formGroup: FormGroup;
  options: Options;

  processAdministrativeProceduresFieldData: any = [];

  ProcessStepData: any = [];
  ProcessAdministrativeProceduresStepId: any = '00000000-0000-0000-0000-000000000000';

  private subscriptions: Subscription[] = [];

  constructor(
    private processAdministrativeProceduresPageService: ProcessAdministrativeProceduresPageService,
    private fb: FormBuilder, 
    public modal: NgbActiveModal,
    public http: HttpClient
  ) { }

  ngOnInit(): void {
    this.isLoading$ = this.processAdministrativeProceduresPageService.isLoading$;
    (async () => {
      this.loadFieldData()
      await this.delay(150);
      this.loadProcessAdministrativeProcedures();
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
    return jQuery('<span class="form-select form-select-solid form-select-lg">' + state.text + '</span>');
  }

  delay(ms: number) {
    return new Promise(resolve => setTimeout(resolve, ms));
  }

  loadFieldData() {
    this.processAdministrativeProceduresPageService.loadField().subscribe((res: any) => {
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

      this.processAdministrativeProceduresFieldData = field_data.sort((i1, i2) => {
        if (i1.piority > i2.piority) {
          return 1;
        }
        if (i1.piority < i2.piority) {
          return -1;
        }
        return 0;
      });
    })
  }

  loadProcessAdministrativeProcedures() {
    if (!this.id) {
      this.clear();
      this.loadForm();
    } else {
      const sb = this.processAdministrativeProceduresPageService.getItemById(this.id).pipe(
        first(),
        catchError((errorMessage) => {
          this.modal.dismiss(errorMessage);
          return of(EMPTY_CUSTOM);
        })
      ).subscribe((res: any) => {
        this.processAdministrativeProceduresData = res.items[0];

        // Tạo array từ dữ liệu API
        res.items[0].processStep.forEach((element: any) => {
          let obj = {
            Step: element.step,
            ImplementingAgencies: element.implementingAgencies,
            ProcessingTime: element.processingTime,
            ContentImplementation: element.contentImplementation,
          }
          this.ProcessStepData.push(obj)
          this.ProcessStepData = this.ProcessStepData.sort((i1: any, i2: any) => {
            if (i1.Step > i2.Step) {
              return 1;
            }
            if (i1.Step < i2.Step) {
              return -1;
            }
            return 0;
          });
        });

        this.loadForm();

        //Thêm các FormGroup vào FormArray
        if (this.ProcessStepData.length > 1) {
          for (var i = 1; i < this.ProcessStepData.length; i++) {
            this.addStep()
          }
        }

        //Gán dữ liệu API vào FormArray
        this.formGroup.controls.ProcessStep.setValue(this.ProcessStepData)

        //Check type để load edit hay view
        if (this.type == "view") {
          this.formGroup.disable();
        }
        this.formGroup.updateValueAndValidity();
      });
      this.subscriptions.push(sb);
    }
  }

  loadForm() {
    this.formGroup = this.fb.group({
      ProcessAdministrativeProceduresField: [this.processAdministrativeProceduresData.processAdministrativeProceduresField],
      ProcessAdministrativeProceduresCode: [this.processAdministrativeProceduresData.processAdministrativeProceduresCode, Validators.required],
      ProcessAdministrativeProceduresName: [this.processAdministrativeProceduresData.processAdministrativeProceduresName, Validators.required],
      ProcessStep: this.fb.array([this.fb.group({
        Step: [''],
        ImplementingAgencies: ['', Validators.required],
        ProcessingTime: ['', Validators.required],
        ContentImplementation: ['', Validators.required]
      })]),
    });
  }

  clear() {
    EMPTY_CUSTOM.processAdministrativeProceduresId = '00000000-0000-0000-0000-000000000000',
      EMPTY_CUSTOM.processAdministrativeProceduresField = '00000000-0000-0000-0000-000000000000',
      EMPTY_CUSTOM.processAdministrativeProceduresCode = '',
      EMPTY_CUSTOM.processAdministrativeProceduresName = '',
      EMPTY_CUSTOM.processStep = [],
      this.processAdministrativeProceduresData = EMPTY_CUSTOM;
  }

  private prepareData() {
    const formData = this.formGroup.value;
    this.processAdministrativeProceduresData.processAdministrativeProceduresField = formData.ProcessAdministrativeProceduresField,
      this.processAdministrativeProceduresData.processAdministrativeProceduresCode = formData.ProcessAdministrativeProceduresCode,
      this.processAdministrativeProceduresData.processAdministrativeProceduresName = formData.ProcessAdministrativeProceduresName,
      this.processAdministrativeProceduresData.processStep = formData.ProcessStep
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
    const sbUpdate = this.processAdministrativeProceduresPageService.update(this.processAdministrativeProceduresData).pipe(
      tap(() => {
        this.modal.close();
      }),
      catchError((errorMessage) => {
        this.modal.dismiss(errorMessage);
        return of(this.processAdministrativeProceduresData);
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
    const sbCreate = this.processAdministrativeProceduresPageService.create(this.processAdministrativeProceduresData).pipe(
      tap(() => {
        this.modal.close();
      }),
      catchError((errorMessage) => {
        this.modal.dismiss(errorMessage);
        return of(this.processAdministrativeProceduresData);
      }),
    ).subscribe((res: any) => {
      Swal.fire({
        icon: res.status == 1 ? 'success' : 'error',
        title: res.status == 1 ? 'Thêm mới thành công' : 'Thêm mới thất bại',
        confirmButtonText: 'Xác nhận',
        text: res.status == 0 ? res.error.msg : 'Thêm mới ' + (res.status == 1 ? 'thành công' : 'thất bại'),
      });
      this.processAdministrativeProceduresData = EMPTY_CUSTOM
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

  // Thao tác với FormArray
  // nhận FormArray
  get GetProcess(): FormArray {
    return this.formGroup.controls.ProcessStep as FormArray;
  }

  // Thêm FormGroup vào FormArray
  addStep() {
    this.GetProcess.push(this.fb.group({
      Step: [''],
      ImplementingAgencies: ['', Validators.required],
      ProcessingTime: ['', Validators.required],
      ContentImplementation: ['', Validators.required]
    }))
  }

  // Xoá FormGroup ra khỏi FormArray
  delStep(index: any) {
    this.GetProcess.removeAt(index)
  }

  // Thêm Value cho control Step
  setStepValue(index: any) {
    const control = this.GetProcess.controls[index].get('Step');
    control?.setValue(index + 1)
    return index + 1
  }

  // Check Validation
  arrayControlHasError(validation: any, controlName: any, index: any): boolean {
    const control = this.GetProcess.controls[index].get(controlName);
    if (control) {
      return control.hasError(validation) && (control.dirty || control.touched);
    }
    else {
      return false
    }
  }

  // del_step(item: any, index: number) {
  //   if (this.ProcessStepData.length && this.ProcessStepData.length > index) {
  //     for (let i = index + 1; i < this.ProcessStepData.length; i++)
  //       this.ProcessStepData[i].Step = this.ProcessStepData[i].Step - 1
  //   }
  //   this.ProcessStepData = this.ProcessStepData.filter((x: any) => x != item);
  //   this.formGroup.controls.ImplementingAgencies.setValidators(Validators.required)
  //   this.formGroup.controls.ProcessingTime.setValidators(Validators.required)
  //   this.formGroup.controls.ContentImplementation.setValidators(Validators.required)
  // }

  // add_step() {
  //   var implementingAgencies = "";
  //   var processingTime = 0;
  //   var contentImplementation = "";

  //   implementingAgencies = this.formGroup.value.ImplementingAgencies;
  //   processingTime = this.formGroup.value.ProcessingTime;
  //   contentImplementation = this.formGroup.value.ContentImplementation;

  //   if (implementingAgencies && processingTime && contentImplementation) {
  //     let obj = {
  //       Step: !this.ProcessStepData.length ? 1 : this.ProcessStepData.length + 1,
  //       ImplementingAgencies: implementingAgencies,
  //       ProcessingTime: processingTime,
  //       ContentImplementation: contentImplementation,
  //     }
  //     this.ProcessStepData.push(obj);

  //     this.formGroup.controls.ImplementingAgencies.reset();
  //     this.formGroup.controls.ProcessingTime.reset();
  //     this.formGroup.controls.ContentImplementation.reset();

  //     this.formGroup.controls.ImplementingAgencies.removeValidators(Validators.required)
  //     this.formGroup.controls.ProcessingTime.removeValidators(Validators.required)
  //     this.formGroup.controls.ContentImplementation.removeValidators(Validators.required)

  //     this.formGroup.controls.ImplementingAgencies.updateValueAndValidity();
  //     this.formGroup.controls.ProcessingTime.updateValueAndValidity();
  //     this.formGroup.controls.ContentImplementation.updateValueAndValidity();
  //   }
  //   else {
  //     this.formGroup.controls.ImplementingAgencies.setValidators(Validators.required)
  //     this.formGroup.controls.ProcessingTime.setValidators(Validators.required)
  //     this.formGroup.controls.ContentImplementation.setValidators(Validators.required)

  //     this.formGroup.controls.ImplementingAgencies.updateValueAndValidity();
  //     this.formGroup.controls.ProcessingTime.updateValueAndValidity();
  //     this.formGroup.controls.ContentImplementation.updateValueAndValidity();
  //   }
  // }

  check_formGroup() {
    // if (this.ProcessStepData.length == 0) {
    //   this.formGroup.controls.ImplementingAgencies.setValidators(Validators.required)
    //   this.formGroup.controls.ProcessingTime.setValidators(Validators.required)
    //   this.formGroup.controls.ContentImplementation.setValidators(Validators.required)

    //   this.formGroup.controls.ImplementingAgencies.updateValueAndValidity();
    //   this.formGroup.controls.ProcessingTime.updateValueAndValidity();
    //   this.formGroup.controls.ContentImplementation.updateValueAndValidity();
    // }
    if (this.formGroup.invalid) {
      this.formGroup.markAllAsTouched();
    }
    else {
      this.save();
    }
  }

  exportFile() {
    const moment = require("moment");
    const timeString = moment().format("DDMMYYYYHHmmss");
    const fileName = "ChiTietQuyTrinhNoiBoGiaiQuyetThuTucHanhChinh_" + timeString + ".xlsx"

    Swal.fire({
      title: 'Đang xuất File...',
      // text: 'Vui lòng đợi một lúc trước khi file của bạn sẵn sàng!',
      didOpen: () => {
        Swal.showLoading()
      },
    })

    this.http.get(`${environment.apiUrl}/ProcessAdministrativeProcedures/ExportById/${this.id}`,
    {
      responseType: 'blob',
    }).pipe(
      catchError((errorMessage: any) => {
        console.error(errorMessage)
        Swal.fire({
          icon: 'error',
          title: 'Xuất File thất bại',
          confirmButtonText: 'Xác nhận',
        });
        return of();
      }),
    ).subscribe(
      (res) => {
        const file = new Blob([res], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' });
        const fileURL = URL.createObjectURL(file);
        const a = document.createElement('a');
        a.href = fileURL;
        a.download = fileName;
        document.body.append(a);
        a.click();
        a.remove();
        URL.revokeObjectURL(fileURL);
        Swal.fire({
          icon: 'success',
          title: 'Xuất File thành công',
          confirmButtonText: 'Xác nhận',
        });
      },
    );
  }
}
