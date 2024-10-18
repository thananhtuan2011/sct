import { ChangeDetectorRef, Component, Input, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal, NgbDateAdapter, NgbDateParserFormatter, NgbModal } from '@ng-bootstrap/ng-bootstrap';
import * as moment from 'moment';
import { of, Subscription } from 'rxjs';
import { catchError, finalize, first, tap } from 'rxjs/operators';
import Swal from 'sweetalert2';

import { TrainingClassManagementModel } from '../../../_models/trainingclassmanagement.model';
import { TrainingClassManagementPageService } from '../../../_services/trainingclassmanagement.service';
import { Options } from 'select2';
import { SelectOptionData } from 'src/app/_metronic/shared/components/select-custom/select-custom.interface';


const EMPTY_CUSTOM: TrainingClassManagementModel = {
  id: '',
  trainingClassManagementId : '',
  topic: '',
  location: '',
  participant: '',
  timeStart: null,
  numberOfAttendees: null,
};

@Component({
  selector: 'app-edit-trainclassmanage-modal',
  templateUrl: './edit-modal.component.html',
  styleUrls: ['./edit-modal.component.scss'],

})
export class EditTrainingClassManagementModalComponent implements OnInit, OnDestroy {
  @Input() id: any;
  @Input() type: any;
  isLoading$:any;
  trainingClassManagementData: TrainingClassManagementModel;
  formGroup: FormGroup;
  file_documents: any[] = [];
  del_file_ids: string = "";
    
  private subscriptions: Subscription[] = [];
  public default_value = "00000000-0000-0000-0000-000000000000"
  
  constructor(
    private trainingClassManagementService: TrainingClassManagementPageService,
    private fb: FormBuilder, public modal: NgbActiveModal,
    private modalService: NgbModal,
    ) { }

  ngOnInit(): void {
    this.isLoading$ = this.trainingClassManagementService.isLoading$;
    this.loadTrainingClassManagement();
  }
  
  public templateSelection = (state: any): JQuery | string => {
    if (!state.id) {
      return state.text;
    }
    return jQuery('<span class="form-select form-select-solid form-select-lg">' + state.text + '</span>');
  }
  
  clearmodel() {
    EMPTY_CUSTOM.trainingClassManagementId='00000000-0000-0000-0000-000000000000';
    EMPTY_CUSTOM.topic='';
    EMPTY_CUSTOM.location =  '',
    EMPTY_CUSTOM.participant = '',
    EMPTY_CUSTOM.timeStart = null,
    EMPTY_CUSTOM.numberOfAttendees = null,
    this.trainingClassManagementData = EMPTY_CUSTOM;
  }
  
  loadTrainingClassManagement() {
    if (!this.id) {
      this.clearmodel();
      this.loadForm();
    } else {
      const sb = this.trainingClassManagementService.getItemById(this.id).pipe(
        first(),
        catchError((errorMessage) => {
          this.modal.dismiss(errorMessage);
          return of(EMPTY_CUSTOM);
        })
      ).subscribe((res: any) => {
        this.trainingClassManagementData = res.data;
        this.trainingClassManagementData.numberOfAttendees = this.f_currency(res.data.numberOfAttendees)
        this.trainingClassManagementData.timeStart = (res.data.timeStart);
        this.file_documents = res.data.details;
        this.loadForm();
        if (this.type == 'view') {
          this.formGroup.disable();
          this.formGroup.updateValueAndValidity();
        }
      });
      this.subscriptions.push(sb);
    }
  }
  
  loadForm() {
    this.formGroup = this.fb.group({
      Topic: [this.trainingClassManagementData.topic, Validators.required],
      Location : [this.trainingClassManagementData.location, Validators.required],
      Participant : [this.trainingClassManagementData.participant, Validators.required],
      NumberOfAttendees: [this.trainingClassManagementData.numberOfAttendees, Validators.required],
      TimeStart: [this.trainingClassManagementData.timeStart != null ? this.convert_date_string(this.trainingClassManagementData.timeStart) : null, Validators.required],
    });
    this.formGroup.controls.NumberOfAttendees.valueChanges.subscribe(x => {
      this.formGroup.patchValue({
        'NumberOfAttendees' : this.f_currency(x)
      }, { emitEvent: false })
    })
  }

  f_currency(value: any, args?: any): any {
    let nbr = Number((value + '').replace(/,|-/g, ''));
    return (nbr + '').replace(/(\d)(?=(\d{3})+(?!\d))/g, '$1,');
  }

  save() {
    const model = this.prepareTrainingClassManagement();
    if(this.id){
      model.append('GasTrainingClassManagementId', this.id);
      this.edit(model);
    }else{
      this.create(model);
    }
  }

  edit(item: any) {
    const sbUpdate = this.trainingClassManagementService.updateformdata(item).pipe(
      tap(() => {
        this.modal.close();
      }),
      catchError((errorMessage) => {
        this.modal.dismiss(errorMessage);
        return of(this.trainingClassManagementData);
      }),
    ).subscribe((res: any) => {
      Swal.fire({
        icon: res.status == 1 ? 'success' : 'error',
        title: res.status == 1 ? 'Chỉnh sửa thành công' : 'Chỉnh sửa thất bại',
        confirmButtonText: 'Xác nhận',
        text: res.status == 0 ? res.error.msg : 'Chỉnh sửa ' + (res.status == 1 ? 'thành công' : 'thất bại'),
      });
      this.trainingClassManagementData = EMPTY_CUSTOM
    });
    this.subscriptions.push(sbUpdate);
  }

  create(item: any) {
    const sbCreate = this.trainingClassManagementService.createformdata(item).pipe(
      tap(() => {
        this.modal.close();
      }),
      catchError((errorMessage) => {
        this.modal.dismiss(errorMessage);
        return of(this.trainingClassManagementData);
      }),
    ).subscribe((res: any) => {
      Swal.fire({
        icon: res.status == 1 ? 'success' : 'error',
        title: res.status == 1 ? 'Thêm mới thành công' : 'Thêm mới thất bại',
        confirmButtonText: 'Xác nhận',
        text: res.status == 0 ? res.error.msg : 'Thêm mới ' + (res.status == 1 ? 'thành công' : 'thất bại'),
      });
      this.trainingClassManagementData = EMPTY_CUSTOM
    });
    this.subscriptions.push(sbCreate);
  }

  private prepareTrainingClassManagement() {
    const formValue = this.formGroup.value;
    let formData: any = new FormData();
    formData.append('Topic', formValue.Topic);
    formData.append('Location', formValue.Location);
    formData.append('Participant', formValue.Participant);
    formData.append('NumberOfAttendees', Number(formValue.NumberOfAttendees.replaceAll(',', ''))); 
    formData.append('TimeStartGet', JSON.stringify(this.convert_date(formValue.TimeStart)));
    
    if (this.file_documents.length > 0) {
      let i = 1;
      for (var document of this.file_documents) {
        if (document.name) {
          formData.append("File" + i, document, document.name);
          i++;
        }
      }
    }
    
    if (this.del_file_ids != "") {
      //Id của file cần xoá
      formData.append("IdFiles", this.del_file_ids)
    }
    
    return formData;
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

  prepareFilesList(files: Array<any>) {
    for (const item of files) {
      item.progress = 0;
      this.file_documents.push(item);
    }
  }

  @ViewChild('fileDropRef') fileDropRef: any;
  onFileDropped($event: any) {
    this.prepareFilesList($event);
  }
  
  fileBrowseHandler(files: any) {
    this.prepareFilesList(files.target.files);
  }
  
  deleteFile(index: number) {
    this.fileDropRef.nativeElement.value = '';
    if (this.file_documents[index].gasTrainingClassManagementAttachFileId) {
      this.del_file_ids += this.file_documents[index].gasTrainingClassManagementAttachFileId + ','
      this.file_documents.splice(index, 1);
    }
    else {
      this.file_documents.splice(index, 1);
    }
  }
  
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
  
  check_formGroup() {
    if (this.formGroup.invalid) {
      this.formGroup.markAllAsTouched();
    }
    else {
      this.save();
    }
  }
}
