import { Component, Input, OnInit, OnChanges, ViewChild, SimpleChanges, ChangeDetectorRef, OnDestroy } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { MatTabChangeEvent } from '@angular/material/tabs';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Options } from 'select2';
import * as moment from 'moment';
import Swal from 'sweetalert2';
import { TotalRetailSaleModel } from '../../../_models/total-retail-sale-page.model';
import { TotalRetailSalePageService } from '../../../_services/total-retail-sale-page.service';
import { catchError, finalize, first, tap } from 'rxjs/operators';
import { of, Subscription } from 'rxjs';

const EMPTY_CUSTOM: TotalRetailSaleModel = {
  id: '',
  totalRetailSaleId: '00000000-0000-0000-0000-000000000000',
  target: 1,
  month: null,
  reportIndex: 0,
}

@Component({
  selector: 'app-edit-report-index-industry-modal',
  templateUrl: './edit-page-modal.component.html',
  styleUrls: ['./edit-page-modal.component.scss'],

})
export class EditTotalRetailSaleModalComponent implements OnInit, OnChanges, OnDestroy {
  @Input() id: any;
  @Input() type: any;
  @Input() dataItem: any;
  isLoading$: any;
  showTab0: boolean = true;
  showTab1: boolean = true;
  formGroup: FormGroup
  options: Options;
  data: TotalRetailSaleModel
  targetData: any = [
    {
      id: 1,
      text: 'Bán lẻ hàng hóa'
    },
    {
      id: 2,
      text: 'Lưu trú, ăn uống'
    },
    {
      id: 3,
      text: 'Du lịch'
    },
    {
      id: 4,
      text: 'Dịch vụ khác'
    }
  ]
  
  private subscriptions: Subscription[] = [];
  
  yearData: any
  yearId: any = moment().year() - 1;
  
  constructor (
    public modal: NgbActiveModal,
    private fb: FormBuilder, 
    private changeDetectorRefs: ChangeDetectorRef ,
    private totalRetailSaleService : TotalRetailSalePageService
  ) { }

  ngOnChanges(changes: SimpleChanges): void {
    
  }

  ngOnInit(): void { 
    this.loadForm();
    this.options = {
      theme: 'bootstrap5',
      templateSelection: this.templateSelection,
    };
   // this.changeDetectorRefs.detectChanges()
  }
  
  public templateSelection = (state: any): JQuery | string => {
    if (!state.id) {
      return state.text;
    }
    return jQuery('<span class="form-select form-select-solid form-select-lg">' + state.text + '</span>');
  }
  loadForm(){
    if(!this.id){
      this.clearForm()
    }else{
      this.data = this.dataItem
    }
    this.formGroup = this.fb.group({
      Target: this.data.target,
      Month: this.data.month,
      ReportIndex: [this.data.reportIndex == 0 ? "" : this.data.reportIndex, Validators.required]
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
    const sbUpdate = this.totalRetailSaleService.update(this.data).pipe(
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
    const sbCreate = this.totalRetailSaleService.create(this.data).pipe(
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
    this.data.target = formData.Target;
    this.data.month = formData.Month;
    this.data.reportIndex = formData.ReportIndex;
  }
  
  clearForm(){
    EMPTY_CUSTOM.id = ''
    EMPTY_CUSTOM.totalRetailSaleId = '00000000-0000-0000-0000-000000000000'
    EMPTY_CUSTOM.target = 1
    EMPTY_CUSTOM.month = null
    EMPTY_CUSTOM.reportIndex = 0
    this.data = EMPTY_CUSTOM
  }

  ngOnDestroy(): void {
    this.subscriptions.forEach(sb => sb.unsubscribe());
  }
  
  isDefaultValue(controlName: any) {
    const control = this.formGroup.controls[controlName];
    const value = control.value;
    if (value == null) {
      control.setErrors({ defaultvalue: true });
    }
    else {
      control.setErrors({ defaultvalue: null });
      control.updateValueAndValidity();
    }
    return control.invalid && (control.dirty || control.touched);
  }
  
  GetValueRatioAccumulation(){
    if(this.formGroup.value){
      const formControl = this.formGroup.value;
      if(formControl.PYAccumulation !== 0){
        return ((formControl.YORAccumulation/formControl.PYAccumulation)*100).toFixed(3)
      } 
    }
    return 0;
  }
  
  GetValueRatioImplementSamePeriod(){
    if(this.formGroup.value){    
      const formControl = this.formGroup.value;
      if(formControl.PYImplement !== 0){
        return ((formControl.YOREstimate/formControl.PYImplement)*100).toFixed(3)
      } 
    }
    return 0;
  }
  
  GetValueRatioImplementLastMonth(){
    if(this.formGroup.value){      
      const formControl = this.formGroup.value;
      if(formControl.YORImplement !== 0){
        return ((formControl.YOREstimate/formControl.YORImplement)*100).toFixed(3)
      } 
    }
    return 0;
  }
  
  check_formGroup(){
    if (this.formGroup.invalid) {
      this.formGroup.markAllAsTouched();
      this.formGroup.updateValueAndValidity();
    }
    else {
      this.save()
    }
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
  
}