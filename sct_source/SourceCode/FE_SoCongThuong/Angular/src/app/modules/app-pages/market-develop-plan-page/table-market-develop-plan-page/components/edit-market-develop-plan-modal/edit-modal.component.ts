import { ChangeDetectorRef, Component, Input, OnChanges, OnDestroy, OnInit, SimpleChanges } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal, NgbDateAdapter, NgbDateParserFormatter } from '@ng-bootstrap/ng-bootstrap';
import { of, Subscription } from 'rxjs';
import { catchError, finalize, first, tap } from 'rxjs/operators';
import Swal from 'sweetalert2';

import { MarketDevelopPlanModel } from '../../../_models/market-develop-plan-page.model';
import { MarketDevelopPlanPageService } from '../../../_services/market-develop-plan-page.service';
import { Options } from 'select2';
import * as moment from 'moment';

const EMPTY_CUSTOM: MarketDevelopPlanModel = {
  id: '',
  marketDevelopPlanId: '00000000-0000-0000-0000-000000000000',
  marketName: '',
  districtId: '00000000-0000-0000-0000-000000000000',
  communeId: '00000000-0000-0000-0000-000000000000',
  address: '',
  rankId: '00000000-0000-0000-0000-000000000000',
  stage: '00000000-0000-0000-0000-000000000000',
  typeOfPlanMarket: '00000000-0000-0000-0000-000000000000',
  existLandArea: 0,
  newLandArea: 0,
  addLandArea: 0,
  capital: 0,
  note: ''
};

@Component({
  selector: 'app-edit-production-business-modal',
  templateUrl: './edit-modal.component.html',
  styleUrls: ['./edit-modal.component.scss'],
})
export class EditMarketDevelopPlanModalComponent implements OnInit, OnDestroy {
  @Input() id: any;
  @Input() districtData: any;
  @Input() communeData: any;
  @Input() typeOfMarketPlanData: any;
  @Input() rankData: any;
  @Input() stageData: any;
  
  isLoading$: any;
  marketDevelopPlanData: MarketDevelopPlanModel;
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
    private pageService: MarketDevelopPlanPageService,
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
      MarketName: [this.marketDevelopPlanData.marketName, Validators.required],
      DistrictId: [this.marketDevelopPlanData.districtId],
      CommuneId: [this.marketDevelopPlanData.communeId],
      Address: [this.marketDevelopPlanData.address],
      RankId: this.marketDevelopPlanData.rankId,
      Stage: this.marketDevelopPlanData.stage,
      TypeOfMarketPlan: this.marketDevelopPlanData.typeOfPlanMarket,
      ExistLandArea: this.marketDevelopPlanData.existLandArea == 0 ? '' :  this.marketDevelopPlanData.existLandArea,
      NewLandArea: this.marketDevelopPlanData.newLandArea == 0 ? '' : this.marketDevelopPlanData.newLandArea,
      AddLandArea: this.marketDevelopPlanData.addLandArea == 0 ? '' : this.marketDevelopPlanData.addLandArea,
      Capital: this.marketDevelopPlanData.capital == 0 ? '' : this.marketDevelopPlanData.capital,
      Note: this.marketDevelopPlanData.note
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
    EMPTY_CUSTOM.marketDevelopPlanId = '00000000-0000-0000-0000-000000000000';
    EMPTY_CUSTOM.marketName = '';;
    EMPTY_CUSTOM.districtId = '00000000-0000-0000-0000-000000000000';
    EMPTY_CUSTOM.communeId = '00000000-0000-0000-0000-000000000000';
    EMPTY_CUSTOM.address = '';;
    EMPTY_CUSTOM.rankId = '00000000-0000-0000-0000-000000000000';
    EMPTY_CUSTOM.stage = '00000000-0000-0000-0000-000000000000';
    EMPTY_CUSTOM.typeOfPlanMarket = '00000000-0000-0000-0000-000000000000';
    EMPTY_CUSTOM.existLandArea = 0;;
    EMPTY_CUSTOM.newLandArea = 0;;
    EMPTY_CUSTOM.addLandArea = 0;;
    EMPTY_CUSTOM.capital = 0;;
    EMPTY_CUSTOM.note = '';
    
    this.marketDevelopPlanData = EMPTY_CUSTOM;
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
    this.marketDevelopPlanData.marketName = formData.MarketName;
    this.marketDevelopPlanData.districtId = formData.DistrictId;
    this.marketDevelopPlanData.communeId = formData.CommuneId;
    this.marketDevelopPlanData.address = formData.Address;
    this.marketDevelopPlanData.rankId = formData.RankId;
    this.marketDevelopPlanData.stage = formData.Stage;
    this.marketDevelopPlanData.typeOfPlanMarket = formData.TypeOfMarketPlan;
    
    this.marketDevelopPlanData.existLandArea = formData.ExistLandArea == '' ? 0 : formData.ExistLandArea;
    this.marketDevelopPlanData.newLandArea = formData.NewLandArea == '' ? 0 : formData.NewLandArea;
    this.marketDevelopPlanData.addLandArea = formData.AddLandArea == '' ? 0 : formData.AddLandArea;
    this.marketDevelopPlanData.capital = formData.Capital == '' ? 0 : formData.Capital;
    this.marketDevelopPlanData.note = formData.Note;
 
  }

  edit() {
    const sbUpdate = this.pageService.update(this.marketDevelopPlanData).pipe(
      tap(() => {
        this.modal.close();
      }),
      catchError((errorMessage) => {
        this.modal.dismiss(errorMessage);
        return of(this.marketDevelopPlanData);
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
    const sbCreate = this.pageService.create(this.marketDevelopPlanData).pipe(
      tap(() => {
        this.modal.close();
      }),
      catchError((errorMessage) => {
        this.modal.dismiss(errorMessage);
        return of(this.marketDevelopPlanData);
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
          this.marketDevelopPlanData = res.items[0];
          this.loadForm();
          this.show = true;
        });
      this.subscriptions.push(sb);
    }
  }
}
