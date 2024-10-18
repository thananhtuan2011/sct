import { ChangeDetectorRef, Component, Input, OnChanges, OnDestroy, OnInit, SimpleChanges } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal, NgbDateAdapter, NgbDateParserFormatter } from '@ng-bootstrap/ng-bootstrap';
import { of, Subscription } from 'rxjs';
import { catchError, finalize, first, tap } from 'rxjs/operators';
import Swal from 'sweetalert2';

import { MarketTargetSevenModel } from '../../../_models/market-target-seven-page.model';
import { MarketTargetSevenPageService } from '../../../_services/market-target-seven-page.service';
import { Options } from 'select2';
import * as moment from 'moment';

const EMPTY_CUSTOM: MarketTargetSevenModel = {
  id: '',
  marketTargetSevenId: '00000000-0000-0000-0000-000000000000',
  marketName: '',
  districtId: '00000000-0000-0000-0000-000000000000',
  communeId: '00000000-0000-0000-0000-000000000000',
  address: '',
  date: '',
  note: ''
};

@Component({
  selector: 'app-edit-production-business-modal',
  templateUrl: './edit-modal.component.html',
  styleUrls: ['./edit-modal.component.scss'],
})
export class EditMarketTargetSevenModalComponent implements OnInit, OnDestroy {
  @Input() id: any;
  @Input() districtData: any;
  @Input() communeData: any;

  isLoading$: any;
  marketTargerSevenData: MarketTargetSevenModel;
  formGroup: FormGroup;
  options: Options;
  communeByDistrictData: any = [
    {
      id: '00000000-0000-0000-0000-000000000000',
      text: '-- Chọn --',
      districtId: '00000000-0000-0000-0000-000000000000'
    }
  ]
  private subscriptions: Subscription[] = [];

  show: boolean = false;

  constructor(
    private pageService: MarketTargetSevenPageService,
    private fb: FormBuilder,
    public modal: NgbActiveModal,
    private changeDetectorRef: ChangeDetectorRef,
  ) { }


  ngOnInit(): void {
    this.loadData();
    this.isLoading$ = this.pageService.isLoading$;
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

  loadForm() {
    this.formGroup = this.fb.group({
      MarketName: [this.marketTargerSevenData.marketName, Validators.required],
      DistrictId: [this.marketTargerSevenData.districtId],
      CommuneId: [this.marketTargerSevenData.communeId],
      Address: [this.marketTargerSevenData.address],
      Date: [this.marketTargerSevenData.date, Validators.required],
      Note: this.marketTargerSevenData.note
    });
    this.loadCommunData();
    this.subscriptions.push(
      this.formGroup.controls.DistrictId.valueChanges.subscribe((x) => {
      this.loadCommunData();
    }))
  }
  
  loadCommunData(){
    const find_data = this.communeData.filter((item: any) =>  item.districtId == this.formGroup.controls['DistrictId'].value || item.districtId == '00000000-0000-0000-0000-000000000000')
    this.communeByDistrictData = find_data;
    const data = find_data.find((x: any) => x.id == this.formGroup.controls['CommuneId'].value);
    if(!data){
      this.formGroup.patchValue(
        {
          CommuneId : '00000000-0000-0000-0000-000000000000',
        },
        { emitEvent: false }
      );
    }
  }

  clear_model() {
    EMPTY_CUSTOM.marketTargetSevenId = '00000000-0000-0000-0000-000000000000';
    EMPTY_CUSTOM.marketName = '';
    EMPTY_CUSTOM.districtId = '00000000-0000-0000-0000-000000000000';
    EMPTY_CUSTOM.communeId = '00000000-0000-0000-0000-000000000000';
    EMPTY_CUSTOM.date = '';
    EMPTY_CUSTOM.note = '';
    EMPTY_CUSTOM.address = '';
    this.marketTargerSevenData = EMPTY_CUSTOM;
  }

  save() {
    this.prepareData();
    if (this.id) {
      this.edit();
    } else {
      this.create();
    }
  }

  private prepareData() {
    const formData = this.formGroup.value;
    this.marketTargerSevenData.marketName = formData.MarketName;
    this.marketTargerSevenData.districtId = formData.DistrictId;
    this.marketTargerSevenData.communeId = formData.CommuneId;
    this.marketTargerSevenData.address = formData.Address;
    this.marketTargerSevenData.date = formData.Date;
    this.marketTargerSevenData.note = formData.Note;
 
  }

  edit() {
    const sbUpdate = this.pageService.update(this.marketTargerSevenData).pipe(
      tap(() => {
        this.modal.close();
      }),
      catchError((errorMessage) => {
        this.modal.dismiss(errorMessage);
        return of(this.marketTargerSevenData);
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
    const sbCreate = this.pageService.create(this.marketTargerSevenData).pipe(
      tap(() => {
        this.modal.close();
      }),
      catchError((errorMessage) => {
        this.modal.dismiss(errorMessage);
        return of(this.marketTargerSevenData);
      }),
    ).subscribe((res: any) => {
      Swal.fire({
        icon: res.status == 1 ? 'success' : 'error',
        title: res.status == 1 ? 'Thêm mới thành công' : 'Thêm mới thất bại',
        confirmButtonText: 'Xác nhận',
        text: res.status == 0 ? res.error.msg : 'Thêm mới ' + (res.status == 1 ? 'thành công' : 'thất bại'),
      });
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

  check_formGroup() {
    if (this.formGroup.invalid) {
      this.formGroup.markAllAsTouched();
      this.formGroup.updateValueAndValidity();
    }
    else {
      this.save()
    }
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
  
  loadData(){
    this.clear_model();
    if (!this.id) {
      this.loadForm();
      this.show = true;
      
    } else {
      const sb = this.pageService
        .getItemById(this.id)
        .pipe(
          first(),
          catchError((errorMessage) => {
            this.modal.dismiss(errorMessage);
            return of(EMPTY_CUSTOM);
          })
        )
        .subscribe((res: any) => {
          this.marketTargerSevenData = res.items[0];
          this.loadForm();
          this.show = true;
        });
      this.subscriptions.push(sb);
    }
  }
}
