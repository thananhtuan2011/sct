import { ChangeDetectorRef, Component, Input, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal, NgbDateAdapter, NgbDateParserFormatter } from '@ng-bootstrap/ng-bootstrap';
import * as moment from 'moment';
import { of, Subscription } from 'rxjs';
import { catchError, finalize, first, tap } from 'rxjs/operators';
import Swal from 'sweetalert2';

import { TrainingManagementModel } from '../../../_models/training-management.model';
import { TrainingManagementPageService } from '../../../_services/training-management-page.service';
import { CommonService } from 'src/app/_metronic/shared/services/common.service';
import { Options } from 'select2';

const EMPTY_CUSTOM: TrainingManagementModel = {
  id: '',
  trainingManagementId: '00000000-0000-0000-0000-000000000000',
  content: '',
  startDate: '',
  endDate: '',
  time: '',
  districtId: '00000000-0000-0000-0000-000000000000',
  address: '',
  participating: '',
  numParticipating: 0,
  implementationCost: 0,
  annunciator: '',
  note: '',
  details: [],
};

@Component({
  selector: 'app-edit-modal',
  templateUrl: './edit-modal.component.html',
  styleUrls: ['./edit-modal.component.scss'],

})

export class EditTrainingManagementModalComponent implements OnInit, OnDestroy {
  @Input() id: any;
  @Input() type: any;
  isLoading$: any;
  trainingManagementData: TrainingManagementModel;
  formGroup: FormGroup;
  showForm: boolean = false;
  options: Options;

  files: any[] = [];
  del_file_ids: string = ""

  private subscriptions: Subscription[] = [];
  districtData: any[];
  

  constructor(
    private trainingManagementService: TrainingManagementPageService,
    private fb: FormBuilder, 
    public modal: NgbActiveModal,
    private commonService: CommonService,
  ) { }

  ngOnInit(): void {
    this.isLoading$ = this.trainingManagementService.isLoading$;
    this.loadDistrict();
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
    this.commonService.getDistrict().subscribe((res: any) => {
      const data = [
        { id: "00000000-0000-0000-0000-000000000000", text: "-- Chọn --" },
        ...res.items.map((item: any) => ({
          id: item.districtId,
          text: item.districtName,
        }))
      ]
      this.districtData = data;
      this.loadTrainingManagement();
    })
  }

  loadTrainingManagement() {
    if (!this.id) {
      this.clear();
      this.loadForm();
    } else {
      const sb = this.trainingManagementService.getItemById(this.id).pipe(
        first(),
        catchError((errorMessage) => {
          this.modal.dismiss(errorMessage);
          return of(EMPTY_CUSTOM);
        })
      ).subscribe((res: any) => {
        this.trainingManagementData = res.data;
        this.trainingManagementData.startDate = res.data.startDateDisplay;
        this.trainingManagementData.endDate = res.data.endDateDisplay;
        this.trainingManagementData.numParticipating = this.f_currency(res.data.numParticipating);
        this.trainingManagementData.implementationCost = this.f_currency(res.data.implementationCost);
        this.files = res.data.details;
        this.loadForm();

        if (this.type) {
          this.formGroup.disable()
        }
      });
      this.subscriptions.push(sb);
    }
  }

  loadForm() {
    this.formGroup = this.fb.group({
      Content: [this.trainingManagementData.content, Validators.required],
      StartDate: [this.trainingManagementData.startDate, Validators.required],
      EndDate: [this.trainingManagementData.endDate],
      Time: [this.trainingManagementData.time],
      DistrictId: [this.trainingManagementData.districtId],
      Address: [this.trainingManagementData.address, Validators.required],
      Participating: [this.trainingManagementData.participating],
      NumParticipating: [this.trainingManagementData.numParticipating + ""],
      ImplementationCost: [this.trainingManagementData.implementationCost + ""],
      Annunciator: [this.trainingManagementData.annunciator],
      Note: [this.trainingManagementData.note],
    });
    
    this.showForm = true;

    this.subscriptions.push(
      this.formGroup.controls.NumParticipating.valueChanges.subscribe((x) => {
        this.formGroup.patchValue({
          "NumParticipating": this.f_currency(x)
        }, { emitEvent: false })
      })
    );

    this.subscriptions.push(
      this.formGroup.controls.ImplementationCost.valueChanges.subscribe((x) => {
        this.formGroup.patchValue({
          "ImplementationCost": this.f_currency(x)
        }, { emitEvent: false })
      })
    );
  }

  clear() {
    EMPTY_CUSTOM.trainingManagementId = '00000000-0000-0000-0000-000000000000',
    EMPTY_CUSTOM.content = '',
    EMPTY_CUSTOM.startDate = '',
    EMPTY_CUSTOM.endDate = '',
    EMPTY_CUSTOM.time = '',
    EMPTY_CUSTOM.districtId = '00000000-0000-0000-0000-000000000000',
    EMPTY_CUSTOM.address = '',
    EMPTY_CUSTOM.participating = '',
    EMPTY_CUSTOM.numParticipating = 0,
    EMPTY_CUSTOM.implementationCost = 0,
    EMPTY_CUSTOM.annunciator = '',
    EMPTY_CUSTOM.note = '',
    EMPTY_CUSTOM.details = [],
    this.trainingManagementData = EMPTY_CUSTOM;
  }

  private prepareTrainingManagement() {
    var formValue = this.formGroup.value;
    var formData: any = new FormData();
    formData.append("Content", formValue.Content);
    formData.append("StartDateDisplay", formValue.StartDate);
    formData.append("EndDateDisplay", formValue.EndDate);
    formData.append("Time", formValue.Time);
    formData.append("DistrictId", formValue.DistrictId);
    formData.append("Address", formValue.Address);
    formData.append("Participating", formValue.Participating);
    formData.append("NumParticipating", Number(formValue.NumParticipating.replaceAll(',', '')));
    formData.append("ImplementationCost", Number(formValue.ImplementationCost.replaceAll(',', '')));
    formData.append("Annunciator", formValue.Annunciator);
    formData.append("Note", formValue.Note);

    if (this.files.length > 0) {
      let i = 1;
      for (var file of this.files) {
        if (file.name) {
          formData.append("Files" + i, file, file.name);
          i++;
        }
      }
    }
    formData.append("IdFiles", this.del_file_ids)
    return formData
  }

  save() {
    const model = this.prepareTrainingManagement();
    if (this.id) {
      model.append("TrainingManagementId", this.id)
      this.edit(model);
    } else {
      this.create(model);
    }
  }

  edit(item: any) {
    const sbUpdate = this.trainingManagementService.updateformdata(item).pipe(
      tap(() => {
        this.modal.close();
      }),
      catchError((errorMessage) => {
        this.modal.dismiss(errorMessage);
        return of(item);
      }),
    ).subscribe((res: any) => {
      Swal.fire({
        icon: res.status == 1 ? 'success' : 'error',
        title: res.status == 1 ? 'Chỉnh sửa thành công' : 'Chỉnh sửa thất bại',
        confirmButtonText: 'Xác nhận',
        text: 'Chỉnh sửa ' + (res.status == 1 ? 'thành công' : 'thất bại'),
      });
      this.trainingManagementData = EMPTY_CUSTOM
    });
    this.subscriptions.push(sbUpdate);
  }

  create(item: any) {
    const sbCreate = this.trainingManagementService.createformdata(item).pipe(
      tap(() => {
        this.modal.close();
      }),
      catchError((errorMessage) => {
        this.modal.dismiss(errorMessage);
        return of(this.prepareTrainingManagement());
      }),
    ).subscribe((res: any) => {
      Swal.fire({
        icon: res.status == 1 ? 'success' : 'error',
        title: res.status == 1 ? 'Thêm mới thành công' : 'Thêm mới thất bại',
        confirmButtonText: 'Xác nhận',
        text: 'Thêm mới ' + (res.status == 1 ? 'thành công' : 'thất bại'),
      });
      this.trainingManagementData = EMPTY_CUSTOM
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

  isDefaultValue(controlName: any)
  {
    const control = this.formGroup.controls[controlName];
    const isdefaultvalue = (control.value == "00000000-0000-0000-0000-000000000000")
    if (isdefaultvalue) {
      control.setErrors({ default: true })
    }
    return control.invalid && (control.dirty || control.touched)
  }

  f_currency(value: any, args?: any): any {
    let nbr = Number((value + '').replace(/,|-/g, ""));
    const result = (nbr + '').replace(/(\d)(?=(\d{3})+(?!\d))/g, "$1,");
    return result
  }

  prenventInputNonNumber(event: any) {
    if (event.which < 48 || event.which > 57) {
      event.preventDefault();
    }
  }

  //Upload File
  @ViewChild('fileDropRef') fileDropRef: any;
  /**
   * on file drop handler
   */
  onFileDropped($event: any) {
    this.prepareFilesList($event);
  }

  /**
   * handle file from browsing
   */
  fileBrowseHandler(files: any) {
    this.prepareFilesList(files.target.files);
  }

  /**
   * Delete file from files list
   * @param index (File index)
   */
  deleteFile(index: number) {
    this.fileDropRef.nativeElement.value = '';
    if (this.files[index].trainingManagementAttachFileId) {
      this.del_file_ids += this.files[index].trainingManagementAttachFileId + ','
      this.files.splice(index, 1);
    }
    else {
      this.files.splice(index, 1);
    }
  }

  /**
   * Convert Files list to normal array list
   * @param files (Files List)
   */
  prepareFilesList(files: Array<any>) {
    for (const item of files) {
      this.files.push(item);
    }
  }

  /**
   * format bytes
   * @param bytes (File size in bytes)
   * @param decimals (Decimals point)
   */
  formatBytes(bytes: any, decimals: any) {
    if (bytes === 0) {
      return '0 Bytes';
    }
    const k = 1024;
    const dm = decimals <= 0 ? 0 : decimals || 2;
    const sizes = ['Bytes', 'KB', 'MB', 'GB', 'TB', 'PB', 'EB', 'ZB', 'YB'];
    const i = Math.floor(Math.log(bytes) / Math.log(k));
    return parseFloat((bytes / Math.pow(k, i)).toFixed(dm)) + ' ' + sizes[i];
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
