import { TypeOfProfessionModel } from './../../../../typeofprofession-page/_models/typeofprofession.model';
import { ChangeDetectorRef, Component, Input, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal, NgbDateAdapter, NgbDateParserFormatter } from '@ng-bootstrap/ng-bootstrap';
import { of, Subscription } from 'rxjs';
import { catchError, finalize, first, tap } from 'rxjs/operators';
import { CommonService } from 'src/app/_metronic/shared/services/common.service';
import Swal from 'sweetalert2';

import { Options } from 'select2';
import { BusinessLineModel } from 'src/app/modules/app-pages/business-line-page/_models/business-line.model';
import { MarketManagementPageService } from '../../../_services/marketmanagement-page.service';

const EMPTY_CUSTOM = {
  BusinessLineId : '00000000-0000-0000-0000-000000000000',
  Price : 0,
};

@Component({
  selector: 'app-edit-market-businessline-modal',
  templateUrl: './edit-businessline-modal.component.html',
  styleUrls: ['./edit-businessline-modal.component.scss'],

})
export class AddBusinessLineModalComponent implements OnInit, OnDestroy {
  @Input() id: any;
  isLoading$:any;
  formGroup: FormGroup;
  businessLineData: any = [];
  typeOfProfessionData: any;
  options: Options;
  businessLineId = '00000000-0000-0000-0000-000000000000'
  price: any = null
  data: any 

  private subscriptions: Subscription[] = [];
  
  constructor(
    private fb: FormBuilder, public modal: NgbActiveModal,
    public commonService: CommonService,
    public marketManagementService: MarketManagementPageService,
    private cd: ChangeDetectorRef
    ) { }

  ngOnInit(): void {
    this.loadBusinessLine();
    this.loadForm();


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

  loadBusinessLine() {
    this.marketManagementService.getBusinessLine().subscribe((res: any) => {
      const businesseLine = [
        {
          id: '00000000-0000-0000-0000-000000000000',
          text: '-- Chọn --',
        },
      ]
      for (var item of res.items) {
        let obj_businessLine = {
          id: item.businessLineId,
          text: item.businessLineName,
        }
        businesseLine.push(obj_businessLine)
      }
      this.businessLineData = businesseLine
      this.cd.detectChanges()
    })
  }

  loadForm() {
    this.formGroup = this.fb.group({
      BusinessLineId: this.businessLineId,
      Price: this.price
    });
  }

  save() {
    this.modal.dismiss(this.prepareData());
  }

  private prepareData() {
    const formData = this.formGroup.value;
    let result = {
      businessLineId: formData.BusinessLineId,
      price: formData.Price,
      businessLineName: this.businessLineData.filter((x: any) => x.id == formData.BusinessLineId)[0].text
    }
    return result
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
    if (value == '00000000-0000-0000-0000-000000000000' || value == null) {
      control.setErrors({ defaultvalue: true });
    }
    else {
      control.setErrors({ defaultvalue: null });
      control.updateValueAndValidity();
    }
    return control.invalid && (control.dirty || control.touched);
  }
  
  isInvalidValue(controlName: any){
    const control = this.formGroup.controls[controlName];
    const value = control.value;
    if (value < 0 || value == null) {
      control.setErrors({ defaultvalue: true });
    }
    else {
      control.setErrors({ defaultvalue: null });
      control.updateValueAndValidity();
    }
    return control.invalid && (control.dirty || control.touched);
  }
  
  check_formGroup() {
    if (this.formGroup.invalid ) {
      this.formGroup.markAllAsTouched();
    }
    else {
      this.save();
    }
  }
}