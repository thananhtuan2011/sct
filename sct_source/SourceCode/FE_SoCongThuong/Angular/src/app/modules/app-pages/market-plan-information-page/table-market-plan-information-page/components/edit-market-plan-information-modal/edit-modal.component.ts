import { ChangeDetectorRef, Component, Input, OnChanges, OnDestroy, OnInit, SimpleChanges } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal, NgbDateAdapter, NgbDateParserFormatter } from '@ng-bootstrap/ng-bootstrap';
import { of, Subscription } from 'rxjs';
import { catchError, finalize, first, tap } from 'rxjs/operators';
import Swal from 'sweetalert2';

import { MarketPlanInformationModel } from '../../../_models/market-plan-information-page.model';
import { MarketPlanInformationPageService } from '../../../_services/market-plan-information-page.service';
import { Options } from 'select2';
import * as moment from 'moment';

const EMPTY_CUSTOM: MarketPlanInformationModel = {
  id: '',
  marketPlanInformationId: '00000000-0000-0000-0000-000000000000',
  marketName: '',
  districtId: '00000000-0000-0000-0000-000000000000',
  communeId: '00000000-0000-0000-0000-000000000000',
  address: '',
  year: moment().year(),
  landArea: 0,
  businessLandArea: 0,
  constructionProperty: '00000000-0000-0000-0000-000000000000',
  constructionNeed: '00000000-0000-0000-0000-000000000000',
  note: ''
};

@Component({
  selector: 'app-edit-production-business-modal',
  templateUrl: './edit-modal.component.html',
  styleUrls: ['./edit-modal.component.scss'],
})
export class EditMarketPlanInformationModalComponent implements OnInit, OnDestroy {
  @Input() id: any;
  @Input() districtData: any;
  @Input() communeData: any;
  @Input() constructionPropertyData: any;
  @Input() constructionNeedData: any;
  isLoading$: any;
  marketPlanInformationData: MarketPlanInformationModel;
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
    private pageService: MarketPlanInformationPageService,
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
      MarketName: [this.marketPlanInformationData.marketName, Validators.required],
      DistrictId: [this.marketPlanInformationData.districtId],
      CommuneId: [this.marketPlanInformationData.communeId],
      Address: [this.marketPlanInformationData.address],
      Year: [this.marketPlanInformationData.year, Validators.required],
      LandArea: this.marketPlanInformationData.landArea == 0 ? '' :  this.marketPlanInformationData.landArea,
      BusinessLandArea: this.marketPlanInformationData.businessLandArea == 0 ? '' : this.marketPlanInformationData.businessLandArea,
      ConstructionProperty: this.marketPlanInformationData.constructionProperty,
      ConstructionNeed: this.marketPlanInformationData.constructionNeed,
      Note: this.marketPlanInformationData.note
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
    EMPTY_CUSTOM.marketPlanInformationId = '00000000-0000-0000-0000-000000000000';
    EMPTY_CUSTOM.marketName = '';
    EMPTY_CUSTOM.districtId = '00000000-0000-0000-0000-000000000000';
    EMPTY_CUSTOM.communeId = '00000000-0000-0000-0000-000000000000';
    EMPTY_CUSTOM.address = '';
    EMPTY_CUSTOM.year = moment().year();
    EMPTY_CUSTOM.landArea = 0;
    EMPTY_CUSTOM.businessLandArea = 0;
    EMPTY_CUSTOM.constructionProperty = '00000000-0000-0000-0000-000000000000';
    EMPTY_CUSTOM.constructionNeed = '00000000-0000-0000-0000-000000000000';
    EMPTY_CUSTOM.note = '';
    
    this.marketPlanInformationData = EMPTY_CUSTOM;
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
    this.marketPlanInformationData.marketName = formData.MarketName;
    this.marketPlanInformationData.districtId = formData.DistrictId;
    this.marketPlanInformationData.communeId = formData.CommuneId;
    this.marketPlanInformationData.address = formData.Address;
    this.marketPlanInformationData.year = formData.Year;
    this.marketPlanInformationData.landArea = formData.LandArea == '' ? 0 : formData.LandArea;
    this.marketPlanInformationData.businessLandArea = formData.BusinessLandArea == '' ? 0 : formData.BusinessLandArea;
    this.marketPlanInformationData.constructionProperty = formData.ConstructionProperty;
    this.marketPlanInformationData.constructionNeed = formData.ConstructionNeed;
    this.marketPlanInformationData.note = formData.Note;
 
  }

  edit() {
    const sbUpdate = this.pageService.update(this.marketPlanInformationData).pipe(
      tap(() => {
        this.modal.close();
      }),
      catchError((errorMessage) => {
        this.modal.dismiss(errorMessage);
        return of(this.marketPlanInformationData);
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
    const sbCreate = this.pageService.create(this.marketPlanInformationData).pipe(
      tap(() => {
        this.modal.close();
      }),
      catchError((errorMessage) => {
        this.modal.dismiss(errorMessage);
        return of(this.marketPlanInformationData);
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
          this.marketPlanInformationData = res.items[0];
          this.loadForm();
          this.show = true;
        });
      this.subscriptions.push(sb);
    }
  }
}
