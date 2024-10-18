import { Component, Input, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal, NgbDateAdapter, NgbDateParserFormatter } from '@ng-bootstrap/ng-bootstrap';
import { of, Subscription } from 'rxjs';
import { catchError, finalize, first, tap } from 'rxjs/operators';
import Swal from 'sweetalert2';
import { RecordsManagerModel } from '../../../_models/records-manager-page.model';
import { RecordsManagerPageService } from '../../../_services/records-manager-page.service';
import { Options } from 'select2';
import * as moment from 'moment';

const EMPTY_CUSTOM: RecordsManagerModel = {
  id: '',
  recordsFinancePlanId: '00000000-0000-0000-0000-000000000000',
  recordsManagerId: '00000000-0000-0000-0000-000000000000',
  codeFile: '',
  title: '',
  receptionTime: null,
  storageTime: 0,
  creator:'',
  note: '',
};

@Component({
  selector: 'app-edit-records-finance-plan-modal',
  templateUrl: './edit-modal.component.html',
  styleUrls: ['./edit-modal.component.scss'],

})
export class EditRecordsManagerModalComponent implements OnInit, OnDestroy {
  @Input() id: any;
  @Input() itemData: any;
  @Input() type: any;
  isLoading$:any;
  data: RecordsManagerModel;
  formGroup: FormGroup;
  dataRecordsGroup: any = [];
  private subscriptions: Subscription[] = [];
  dataCreator: any = [];
  options: Options;
  show: boolean = false;

  constructor(
    public recordsManager: RecordsManagerPageService,
    private fb: FormBuilder, public modal: NgbActiveModal
    ) { }

  ngOnInit(): void {
    this.isLoading$ = this.recordsManager.isLoading$;
    this.loadRecordsFinance();
    this.options = {
      theme:'bootstrap5',
      templateSelection: this.templateSelection,
    };
  }
  
  public templateSelection = (state: any): JQuery | string => {
    if (!state.id) {
      return state.text;
    }
    return jQuery('<span class="form-select form-select-solid form-select-lg">'+ state.text + '</span>');
  }
  
  loadForm() {
    if(!this.id){
      this.clearModel();
    } else{
      this.data = this.itemData;
      this.dataCreator = this.data.creator.split(',');
    }
    this.formGroup = this.fb.group({
      RecordsGroup: [this.data.recordsFinancePlanId],
      CodeFile: [this.data.codeFile, Validators.required],
      Title: [this.data.title,Validators.required],
      ReceptionTime: [this.data.receptionTime != null ? this.convert_date_string(this.data.receptionTime) : '' ,Validators.required],
      StorageTime: [this.data.storageTime,Validators.required],
      CreatorX: ['',Validators.required],
      Note: this.data.note
    });
    if(this.type){
      this.formGroup.disable();
    }
    this.show = true;
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
    const sbUpdate = this.recordsManager.update(this.data).pipe(
      tap(() => {
        this.modal.close();
      }),
      catchError((errorMessage) => {
        this.modal.dismiss(errorMessage);
        return of(this.data);
      }),
    ).subscribe((res: any) => {
      Swal.fire({
        icon: res.status == 1 ? 'success' : 'error',
        title: res.status == 1 ? 'Chỉnh sửa thành công' : 'Chỉnh sửa thất bại',
        confirmButtonText: 'Xác nhận',
        text: (res.status == 1 ? 'Chỉnh sửa thành công' : res.status == 0 ? res.error.msg : "Chỉnh sửa thất bại" ),
      });
    });
    this.subscriptions.push(sbUpdate);
  }

  create() {
    const sbCreate = this.recordsManager.create(this.data).pipe(
      tap(() => {
        this.modal.close();
      }),
      catchError((errorMessage) => {
        this.modal.dismiss(errorMessage);
        return of(this.data);
      }),
    ).subscribe((res: any) => {
      Swal.fire({
        icon: res.status == 1 ? 'success' : 'error',
        title: res.status == 1 ? 'Thêm mới thành công' : 'Thêm mới thất bại',
        confirmButtonText: 'Xác nhận',
        text: (res.status == 1 ? 'Thêm mới thành công' : res.status == 0 ? res.error.msg : 'Thêm mới thất bại' ),
      });
      this.data = EMPTY_CUSTOM
    });
    this.subscriptions.push(sbCreate);
  }

  private prepareData() {
    let creator : string;
    if(this.dataCreator.length > 0 && this.formGroup.controls.CreatorX.value !== ''){
      creator = `${this.dataCreator.toString()},${this.formGroup.controls.CreatorX.value}`
    }else if(this.dataCreator.length > 0){
      creator = `${this.dataCreator.toString()}`
    } else{
      creator = `${this.formGroup.controls.CreatorX.value}`
    }
    const formData = this.formGroup.value;
    this.data.recordsFinancePlanId = formData.RecordsGroup;
    this.data.codeFile = formData.CodeFile;
    this.data.title = formData.Title;
    this.data.receptionTime = this.convert_date(formData.ReceptionTime);
    this.data.note = formData.Note;
    this.data.creator = creator;
    this.data.storageTime = formData.StorageTime;
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
    if (control.value == '' || control.value == '00000000-0000-0000-0000-000000000000') {
      control.setErrors({'default': true})
    } else {
      control.setErrors(null)
    }
    return control.hasError('default') && (control.dirty || control.touched);
  }

  check_formGroup() {
    if(this.dataCreator.length > 0)
    {
      this.formGroup.controls.CreatorX.clearValidators(); 
      this.formGroup.controls.CreatorX.updateValueAndValidity();
    }
    if (this.formGroup.invalid ) {
      this.formGroup.markAllAsTouched();
      this.formGroup.updateValueAndValidity();
    }
    else {
      this.save()
    }
  }
  
  clearModel(){
    EMPTY_CUSTOM.id =  '';
    EMPTY_CUSTOM.recordsFinancePlanId =  '00000000-0000-0000-0000-000000000000';
    EMPTY_CUSTOM.recordsManagerId =  '00000000-0000-0000-0000-000000000000';
    EMPTY_CUSTOM.codeFile =  '';
    EMPTY_CUSTOM.title =  '';
    EMPTY_CUSTOM.receptionTime =  null;
    EMPTY_CUSTOM.storageTime =  0;
    EMPTY_CUSTOM.creator = '';
    EMPTY_CUSTOM.note =  '';
    this.data = EMPTY_CUSTOM;
  }
  
  loadRecordsFinance(){
    this.recordsManager.getAllRecordsFianacePlan().subscribe(res => {
      const data = [
        {
          id: "00000000-0000-0000-0000-000000000000",
          text: '-- Chọn --'
        }
      ]
      
      for(var item of res.items) { 
        let obj = {
          id: item.recordsFinancePlanId,
          text: item.name
        }
        data.push(obj)
      }
      this.dataRecordsGroup = data

      this.loadForm();
    })
  }
  addCreator(){
    const formData = this.formGroup.controls.CreatorX.value;
    if(formData.trim() !== ''){
      this.dataCreator.push(formData)
      this.formGroup.controls.CreatorX.reset('')
      this.formGroup.controls.CreatorX.clearValidators(); 
      this.formGroup.controls.CreatorX.updateValueAndValidity();
    }
  }
  
  deleteCreator(value: any){
    this.dataCreator = this.dataCreator.filter((x: any) => x != value)
    if(this.dataCreator.length == 0)
      {
        this.formGroup.controls.CreatorX.addValidators(Validators.required)
        this.formGroup.controls.CreatorX.updateValueAndValidity();
        this.formGroup.controls.CreatorX.markAsTouched()
        this.formGroup.controls.CreatorX.updateValueAndValidity();

      }
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
  
  changeCreator(event: any, index: number) {
    this.dataCreator[index] = event.target.value;
    if(event.target.value == ''){
      this.dataCreator.splice(index,1)
    }
    if (this.dataCreator.length == 0) {
      this.formGroup.controls.CreatorX.addValidators(Validators.required)
      this.formGroup.controls.CreatorX.updateValueAndValidity();
      this.formGroup.controls.CreatorX.markAsTouched()
      this.formGroup.controls.CreatorX.updateValueAndValidity();
    }
  }
}
