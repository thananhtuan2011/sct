import { Component, Input, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal, NgbDateAdapter, NgbDateParserFormatter } from '@ng-bootstrap/ng-bootstrap';
import { of, Subscription } from 'rxjs';
import { catchError, finalize, first, tap } from 'rxjs/operators';
import Swal from 'sweetalert2';
import { IndustryModel } from '../../../_models/industry.model';
import { IndustryPageService } from '../../../_services/industry-page.service';

import { SelectOptionData } from 'src/app/_metronic/shared/components/select-custom/select-custom.interface';
import { Options } from 'select2';

const EMPTY_CUSTOM: IndustryModel = {
  id: '',
  industryId: '00000000-0000-0000-0000-000000000000',
  industryName: '',
  industryCode: '',
  parentIndustryId: '00000000-0000-0000-0000-000000000000',
};

@Component({
  selector: 'app-edit-industry-modal',
  templateUrl: './edit-industry-modal.component.html',
  styleUrls: ['./edit-industry-modal.component.scss'],

})
export class EditCustomModalComponent implements OnInit, OnDestroy {
  @Input() id: any;
  isLoading$:any;
  industryData: IndustryModel;
  formGroup: FormGroup;
  private subscriptions: Subscription[] = [];

  public selectData: Array<SelectOptionData> ;
  public options: Options;
  show: boolean = false;

  constructor(
    private industryService: IndustryPageService,
    private fb: FormBuilder, 
    public modal: NgbActiveModal,
    ) { }

  ngOnInit(): void {
    this.isLoading$ = this.industryService.isLoading$;
    this.loadIndustry(this.id ?? '00000000-0000-0000-0000-000000000000');

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

  loadIndustry(id: any) {
    this.industryService.loadIndustry(id).subscribe(val => {
      const data = [
        {
          id: '00000000-0000-0000-0000-000000000000',
          text: '--- Chọn / Không có ---',
        }
      ]
      for(var industry of val.items){
        if (industry.isDel == false) {
          let obj = {
            id: industry.industryId,
            text: industry.industryName,
          }
          data.push(obj)
        }
      }
      this.selectData = data;
      this.loadCustom();
    })
  }

  // selectdatainedit(id: string){
  //   this.industryService.loadIndustry().subscribe(val => {
  //     const data = [
  //       {
  //         id: '00000000-0000-0000-0000-000000000000',
  //         text: 'Không có',
  //       }
  //     ]
  //     for(var industry of val.items){
  //       if (industry.industryId != id && industry.isDel == false) {
  //         let obj = {
  //           id: industry.industryId,
  //           text: industry.industryName,
  //         }
  //         data.push(obj)
  //       }
  //     }
  //     this.selectData = data
  //   })
  // }

  loadCustom() {
    if (!this.id) {
      this.clear();
      this.loadForm();
    } else {
      const sb = this.industryService.getItemById(this.id).pipe(
        first(),
        catchError((errorMessage) => {
          this.modal.dismiss(errorMessage);
          return of(EMPTY_CUSTOM);
        })
      ).subscribe((res: any) => {
        this.industryData = res.items[0];
        this.loadForm();
        
      });
      this.subscriptions.push(sb);
    }
  }

  loadForm() {
    this.formGroup = this.fb.group({
      industryName: [this.industryData.industryName, Validators.required],
      industryCode: [this.industryData.industryCode, Validators.compose([Validators.required, Validators.pattern("[a-zA-Z0-9]{1,15}")])],
      parentIndustryId: [this.industryData.parentIndustryId]
    });
    this.formGroup.controls.parentIndustryId.valueChanges.subscribe(x => console.log(x))
    this.show = true;
  }

  save() {
    this.prepareCustomer();
    if (this.industryData.industryId != '00000000-0000-0000-0000-000000000000') {
      this.edit();
    } else {
      this.create();
    }
  }

  edit() {
    const sbUpdate = this.industryService.update(this.industryData).pipe(
      tap(() => {
        this.modal.close();
      }),
      catchError((errorMessage) => {
        this.modal.dismiss(errorMessage);
        return of(this.industryData);
      }),
    ).subscribe((res: any) => {
      Swal.fire({
        icon: res.status == 1 ? 'success' : 'error',
        title: res.status == 1 ? 'Chỉnh sửa thành công' : 'Chỉnh sửa thất bại',
        confirmButtonText: 'Xác nhận',
        text: res.status == 1 ? 'Chỉnh sửa thành công' : res.status == 0 ? res.error.msg : 'Chỉnh sửa thất bại',
      });
    });
    this.subscriptions.push(sbUpdate);
  }

  create() {
    const sbCreate = this.industryService.create(this.industryData).pipe(
      tap(() => {
        this.modal.close();
      }),
      catchError((errorMessage) => {
        this.modal.dismiss(errorMessage);
        return of(this.industryData);
      }),
    ).subscribe((res: any) => {
      Swal.fire({
        icon: res.status == 1 ? 'success' : 'error',
        title: res.status == 1 ? 'Thêm mới thành công' : 'Thêm mới thất bại',
        confirmButtonText: 'Xác nhận',
        text: res.status == 1 ? 'Thêm mới thành công' : res.status == 0 ? res.error.msg : 'Thêm mới thất bại',
      });
      this.industryData = EMPTY_CUSTOM
    });
    this.subscriptions.push(sbCreate);
  }

  clear(){
    EMPTY_CUSTOM.industryId='00000000-0000-0000-0000-000000000000'
    EMPTY_CUSTOM.industryName=''
    EMPTY_CUSTOM.industryCode=''
    EMPTY_CUSTOM.parentIndustryId='00000000-0000-0000-0000-000000000000'
    this.industryData = EMPTY_CUSTOM
  }

  private prepareCustomer() {
    const formData = this.formGroup.value;
    this.industryData.industryName = formData.industryName;
    this.industryData.industryCode = formData.industryCode;
    this.industryData.parentIndustryId= formData.parentIndustryId;
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

  check_formGroup() {
    if (this.formGroup.invalid) {
      this.formGroup.markAllAsTouched();
      this.formGroup.updateValueAndValidity();
    }
    else {
      this.save()
    }
  }
}
