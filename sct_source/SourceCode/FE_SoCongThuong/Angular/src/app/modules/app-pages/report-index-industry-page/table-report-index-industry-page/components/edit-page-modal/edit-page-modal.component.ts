import { Component, Input, OnInit, OnChanges, ViewChild, SimpleChanges, ChangeDetectorRef, OnDestroy } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { MatTabChangeEvent } from '@angular/material/tabs';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Options } from 'select2';
import * as moment from 'moment';
import Swal from 'sweetalert2';
import { ReportIndexIndustryModel } from '../../../_models/report-index-industry-page.model';
import { ReportIndexIndustryPageService } from '../../../_services/report-index-industry-page.service';
import { catchError, finalize, first, tap } from 'rxjs/operators';
import { of, Subscription } from 'rxjs';

const EMPTY_CUSTOM: ReportIndexIndustryModel = {
  id: '',
  reportIndexIndustryId: '00000000-0000-0000-0000-000000000000',
  target: 0,
  month: null,
  reportIndex: 0,
  dataTarget: [],
  comparedPreviousMonth: 0,
  samePeriod: 0,
  accumulation: 0
}

@Component({
  selector: 'app-edit-report-index-industry-modal',
  templateUrl: './edit-page-modal.component.html',
  styleUrls: ['./edit-page-modal.component.scss'],

})
export class EditReportIndexIndustryModalComponent implements OnInit, OnChanges, OnDestroy {
  @Input() id: any;
  @Input() type: any;
  @Input() itemData: any;
  isLoading$: any;
  showTab0: boolean = true;
  showTab1: boolean = true;
  formGroup: FormGroup
  options: Options;
  data: ReportIndexIndustryModel
  targetData: any = [
    {
      id: 0,
      text: 'Toàn ngành công nghiệp'
    },
    {
      id: 1,
      text: 'Khai khoáng'
    },
    {
      id: 2,
      text: 'Công nghiệp chế biến, chế tạo'
    },
    {
      id: 3,
      text: 'Sản xuất và phân phối điện, khí đốt, nước nóng, hơi nước và điều hòa không khí'
    },
    {
      id: 4,
      text: 'Cung cấp nước, hoạt động quản lý và xử lý rác thải, nước thải'
    }
  ]
  
  private subscriptions: Subscription[] = [];
  
  yearData: any
  yearId: any = moment().year() - 1;
  
  constructor (
    public modal: NgbActiveModal,
    private fb: FormBuilder, 
    private changeDetectorRefs: ChangeDetectorRef ,
    private reportIndexIndustryService : ReportIndexIndustryPageService
  ) { }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes.id.currentValue && changes.type.currentValue) {
      
    }
  }

  ngOnInit(): void { 
    this.loadForm();
    this.options = {
      theme: 'bootstrap5',
      templateSelection: this.templateSelection,
    };
    this.changeDetectorRefs.detectChanges()
  }
  
  public templateSelection = (state: any): JQuery | string => {
    if (!state.id) {
      return state.text;
    }
    return jQuery('<span class="form-select form-select-solid form-select-lg">' + state.text + '</span>');
  }
  loadForm(){
    
    if(!this.id){
      this.clearModel()
    } else{
      this.data = this.itemData;
   //   this.yearId = this.data.year;
    }
    this.formGroup = this.fb.group({
      Target: [this.data.target],
      Month: [this.data.month, Validators.required],
      ReportIndex: [this.data.reportIndex == 0 ? '' : this.data.reportIndex, Validators.required ],
    })
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
    const sbUpdate = this.reportIndexIndustryService.update(this.data).pipe(
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
    const sbCreate = this.reportIndexIndustryService.create(this.data).pipe(
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

  prepareData() {
    const formData = this.formGroup.value;
    this.data.target = Number(formData.Target)
    this.data.month = formData.Month
    this.data.reportIndex = formData.ReportIndex
  }

  loadYear(){
    const data : any = [];
    for(let i = 0; i < 30; i++){
      let obj = {
        id: moment().year()- 15 + i,
        text: (moment().year()- 15 + i).toString()
      }
      data.push(obj);
    }
    this.yearData = data;
  }
  
  ngOnDestroy(): void {
    this.subscriptions.forEach(sb => sb.unsubscribe());
  }
  
  isDefaultValue(controlName: any) {
    const control = this.formGroup.controls[controlName];
    const value = control.value;
    if (value == '00000000-0000-0000-0000-000000000000' || value == '') {
      control.setErrors({ defaultvalue: true });
    }
    else {
      control.setErrors({ defaultvalue: null });
      control.updateValueAndValidity();
    }
    return control.invalid && (control.dirty || control.touched);
  }
  
  clearModel(){
    EMPTY_CUSTOM.reportIndexIndustryId = '00000000-0000-0000-0000-000000000000'
    EMPTY_CUSTOM.target = 0;
    EMPTY_CUSTOM.month =  null;
    EMPTY_CUSTOM.reportIndex =  0;
    EMPTY_CUSTOM.dataTarget =  [];
    EMPTY_CUSTOM.comparedPreviousMonth =  0;
    EMPTY_CUSTOM.samePeriod =  0;
    EMPTY_CUSTOM.accumulation =  0;
    this.data = EMPTY_CUSTOM
  }
  
  changeYear(value: any){
    this.yearId = value;
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
  
  check_formGroup() {
    if (this.formGroup.invalid ) {
      this.formGroup.markAllAsTouched();
      this.formGroup.updateValueAndValidity();
    }
    else {
      this.save()
    }
  }
}