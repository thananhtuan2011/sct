import { ChangeDetectorRef, Component, Input, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal, NgbDateAdapter, NgbDateParserFormatter } from '@ng-bootstrap/ng-bootstrap';
import { of, Subscription } from 'rxjs';
import { catchError, finalize, first, tap } from 'rxjs/operators';
import { SelectOptionData } from 'src/app/_metronic/shared/components/select-custom/select-custom.interface';
import Swal from 'sweetalert2';
import { Options } from 'select2';
import { CateCriteriaNumberSevenService } from '../../../_services/cate-criteria.service';
import { CateCriteriaNumberSevenDetailModel, CateCriteriaNumberSevenModel } from '../../../_models/cate-criteria.model';

const EMPTY_CUSTOM: CateCriteriaNumberSevenDetailModel = {
  id: '',
  cateCriteriaNumberSevenId: '',
  districtId: '00000000-0000-0000-0000-000000000000',
  districtName: '',
  numberOfWard: 0,
  numberOfQualifyingWard: 0,
  numberOfWardCommercialInfrastructure: 0,
  numberOfWardWithMarket: 0,
  numberOfWardCommercialInfrastructureEstimate: 0,
  numberOfWardCommercialInfrastructurePlan: 0,
  numberOfWardNewCountryside: 0,
  numberOfWardNewCountrysideEstimate: 0,
  numberOfWardNewCountrysidePlan: 0,
};

@Component({
  selector: 'app-edit-cate-criteria-detail1-modal.component',
  templateUrl: './edit-cate-criteria-detail1-modal.component.html',
  styleUrls: ['./edit-cate-criteria-detail1-modal.component.scss'],

})
export class EditCateCriteriaDetail1ModalComponent implements OnInit, OnDestroy {
  @Input() id: any;
  isLoading$: any;
  CateCriNumber7DetailModel: CateCriteriaNumberSevenDetailModel;
  formGroup: FormGroup;
  public options: Options;
  dataSource: any[] = [];
  lstStore: any[] = [];

  private subscriptions: Subscription[] = [];
  public default_value = "00000000-0000-0000-0000-000000000000"
  public dataDistrict: any = [];

  constructor(
    private CateRetailService: CateCriteriaNumberSevenService,
    private fb: FormBuilder, public modal: NgbActiveModal,
    private changeDetectorRefs: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.isLoading$ = this.CateRetailService.isLoading$;
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

  loadDetail() {
    if (!this.id) {
      this.clear_model();
      this.loadForm();
    } else {
      const sb = this.CateRetailService.getItemById(this.id).pipe(
        first(),
        catchError((errorMessage) => {
          this.modal.dismiss(errorMessage);
          return of(EMPTY_CUSTOM);
        })
      ).subscribe((res: any) => {
        this.CateCriNumber7DetailModel = res.items[0];
        this.loadForm();
      });
      this.subscriptions.push(sb);
    }
  }

  loadForm() {
    this.formGroup = this.fb.group({
      DistrictId: [this.CateCriNumber7DetailModel.districtId, Validators.compose([Validators.required])],
      NumberOfWard: [this.CateCriNumber7DetailModel.numberOfWard, Validators.compose([Validators.pattern(/^[0-9]\d*$/)])],
      NumberOfQualifyingWard: [this.CateCriNumber7DetailModel.numberOfQualifyingWard, Validators.compose([Validators.pattern(/^[0-9]\d*$/)])],
      NumberOfWardWithMarket: [this.CateCriNumber7DetailModel.numberOfWardWithMarket, Validators.compose([Validators.pattern(/^[0-9]\d*$/)])],
      NumberOfWardCommercialInfrastructure: [this.CateCriNumber7DetailModel.numberOfWardCommercialInfrastructure, Validators.compose([Validators.pattern(/^[0-9]\d*$/)])],
      NumberOfWardNewCountryside: [this.CateCriNumber7DetailModel.numberOfWardNewCountryside, Validators.compose([, Validators.pattern(/^[0-9]\d*$/)])],
      NumberOfWardCommercialInfrastructure_Plan: [this.CateCriNumber7DetailModel.numberOfWardCommercialInfrastructurePlan, Validators.compose([Validators.pattern(/^[0-9]\d*$/)])],
      NumberOfWardNewCountryside_Plan: [this.CateCriNumber7DetailModel.numberOfWardNewCountrysidePlan, Validators.compose([Validators.pattern(/^[0-9]\d*$/)])],
      NumberOfWardCommercialInfrastructure_Estimate: [this.CateCriNumber7DetailModel.numberOfWardCommercialInfrastructureEstimate, Validators.compose([Validators.pattern(/^[0-9]\d*$/)])],
      NumberOfWardNewCountryside_Estimate: [this.CateCriNumber7DetailModel.numberOfWardNewCountrysideEstimate, Validators.compose([Validators.pattern(/^[0-9]\d*$/)])],
    });
    this.formGroup.controls.DistrictId.valueChanges.subscribe(id => {
      this.formGroup.patchValue({
        "NumberOfWard": this.dataDistrict.find((x: any) => x.id == id).commune ?? 0
      }, {emitEvent: false})
    })
  }

  clear_model() {
    EMPTY_CUSTOM.cateCriteriaNumberSevenId = '',
      EMPTY_CUSTOM.districtId = '00000000-0000-0000-0000-000000000000',
      EMPTY_CUSTOM.numberOfWard = 0,
      EMPTY_CUSTOM.numberOfQualifyingWard = 0,
      EMPTY_CUSTOM.numberOfWardWithMarket = 0,
      EMPTY_CUSTOM.numberOfWardCommercialInfrastructure = 0,
      EMPTY_CUSTOM.numberOfWardNewCountryside = 0,
      EMPTY_CUSTOM.numberOfWardCommercialInfrastructurePlan = 0,
      EMPTY_CUSTOM.numberOfWardNewCountrysidePlan = 0,
      EMPTY_CUSTOM.numberOfWardCommercialInfrastructureEstimate = 0,
      EMPTY_CUSTOM.numberOfWardNewCountrysideEstimate = 0,
      this.CateCriNumber7DetailModel = EMPTY_CUSTOM
  }

  save() {
    const formData = this.formGroup.value;
    this.CateCriNumber7DetailModel.districtId = formData.DistrictId;
    this.CateCriNumber7DetailModel.numberOfWard = +formData.NumberOfWard;
    this.CateCriNumber7DetailModel.numberOfQualifyingWard = +formData.NumberOfQualifyingWard;
    this.CateCriNumber7DetailModel.numberOfWardWithMarket = +formData.NumberOfWardWithMarket;
    this.CateCriNumber7DetailModel.numberOfWardCommercialInfrastructure = +formData.NumberOfWardCommercialInfrastructure;
    this.CateCriNumber7DetailModel.numberOfWardNewCountryside = +formData.NumberOfWardNewCountryside;
    this.CateCriNumber7DetailModel.numberOfWardCommercialInfrastructurePlan = +formData.NumberOfWardCommercialInfrastructure_Plan;
    this.CateCriNumber7DetailModel.numberOfWardNewCountrysidePlan = +formData.NumberOfWardNewCountryside_Plan;
    this.CateCriNumber7DetailModel.numberOfWardCommercialInfrastructureEstimate = +formData.NumberOfWardCommercialInfrastructure_Estimate;
    this.CateCriNumber7DetailModel.numberOfWardNewCountrysideEstimate = +formData.NumberOfWardNewCountryside_Estimate
    this.dataDistrict.forEach((x: any) => {
      if (x.id == this.CateCriNumber7DetailModel.districtId) {
        this.CateCriNumber7DetailModel.districtName = x.text;
      }
    });
    this.modal.close(this.CateCriNumber7DetailModel);
  }

  loadDistrict() {
    this.CateRetailService.loaddistrict().subscribe(res => {
      const data = [{
        id: "00000000-0000-0000-0000-000000000000",
        text: '-- Chọn --'
      }]
      for (var dis of res.items) {
        let obj_business = {
          id: dis.districtId,
          text: dis.districtName,
          commune: dis.communeNumber,
        }
        data.push(obj_business);
      }
      this.dataDistrict = data.sort((a, b) => {
        if (a.text < b.text) {
          return -1;
        }
        if (a.text > b.text) {
          return 1;
        }
        return 0;
      });
      this.loadDetail();
    })
  }

  //Chuyển output của ngbDate thành dạng Date
  convertNgbDateToDate(dateToConvert: any) {
    return new Date(Date.UTC(dateToConvert.year, dateToConvert.month - 1, dateToConvert.day))
  }

  //Chuyển Date thành định dạng của ngbDate
  converDateToNbgDate(dateToConvert: any) {
    const data = new Date(dateToConvert)
    let NgbDate = {
      year: data.getFullYear(),
      month: data.getMonth() + 1,
      day: data.getDate(),
    }
    return NgbDate
  }

  prenventInputNonNumber(event: any) {
    if (event.which < 48 || event.which > 57) {
      event.preventDefault();
    }
  }

  isDefaultValue(controlName: any)//: boolean 
  {
    const control = this.formGroup.controls[controlName];
    const isdefaultvalue = (control.value == "00000000-0000-0000-0000-000000000000")
    if (isdefaultvalue) {
      control.setErrors({ default: true })
    }
    return control.invalid && (control.dirty || control.touched)
  }

  ngOnDestroy(): void {
    this.subscriptions.forEach(sb => sb.unsubscribe());
  }

  // helpers for View
  isControlValid(controlName: any): boolean {
    const control = this.formGroup.controls[controlName];
    return control.valid && (control.dirty || control.touched);
  }

  isControlInvalid(controlName: any): boolean {
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
    }
    else {
      this.save();
    }
  }
}
