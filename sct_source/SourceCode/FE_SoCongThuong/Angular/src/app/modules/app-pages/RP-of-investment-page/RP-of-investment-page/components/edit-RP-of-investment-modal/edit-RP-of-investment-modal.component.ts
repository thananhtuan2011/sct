import { ChangeDetectorRef, Component, Input, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal, NgbDateAdapter, NgbDateParserFormatter } from '@ng-bootstrap/ng-bootstrap';
import { of, Subscription } from 'rxjs';
import { catchError, finalize, first, tap } from 'rxjs/operators';
import { SelectOptionData } from 'src/app/_metronic/shared/components/select-custom/select-custom.interface';
import Swal from 'sweetalert2';
import { Options } from 'select2';
import { CommonService } from 'src/app/_metronic/shared/services/common.service';
import { RPOSOfInvestmentnModel } from '../../../_models/RP-of-investment.model';
import { RPOSOfInvestmentnPageService } from '../../../_services/RP-of-investment-page.service';
import * as moment from 'moment';

const EMPTY_CUSTOM: RPOSOfInvestmentnModel = {
  id: '',
  ReportOperationalStatusOfInvestmentProjectsId: '',
  criteria: '00000000-0000-0000-0000-000000000000',
  units: '',
  note: '',
  quantity: null,
  reportingPeriod: '00000000-0000-0000-0000-000000000000',
  year: moment().year()
}
@Component({
  selector: 'app-edit-rp-of-investment-modal.component',
  templateUrl: './edit-RP-of-investment-modal.component.html',
  styleUrls: ['./edit-RP-of-investment-modal.component.scss'],

})
export class EditRPOSOfInvestmentnModalComponent implements OnInit, OnDestroy {
  @Input() id: any;
  isLoading$: any;
  RPOSOfInvestmentnData: RPOSOfInvestmentnModel;
  formGroup: FormGroup;
  public options: Options;
  dataSource: any[] = [];
  lstStore: any[] = [];
  yearData: any = []
  displayedColumns: string[] = ['stt', 'name', 'action'];
  public datKyBaoCao: Array<SelectOptionData>;

  private subscriptions: Subscription[] = [];
  public default_value = "00000000-0000-0000-0000-000000000000"
  public CriteriaData: any = [];

  constructor(
    private RPOSOfInvestmentnPageService: RPOSOfInvestmentnPageService,
    private fb: FormBuilder, public modal: NgbActiveModal,
    private changeDetectorRefs: ChangeDetectorRef,
    private commonService: CommonService,
  ) { }

  ngOnInit(): void {
    this.isLoading$ = this.RPOSOfInvestmentnPageService.isLoading$;
    (async () => {
      this.loadYear();
      this.loadCi();
      this.loadKyBaoCao();
      await this.delay(200);
      this.loadDetail();
    })();
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

  f_currency(value: any, args?: any): any {
    let nbr = Number((value + '').replace(/,|-/g, ''));
    return (nbr + '').replace(/(\d)(?=(\d{3})+(?!\d))/g, '$1,');
  }

  prenventInputNonNumber(event: any) {
    if (event.which < 48 || event.which > 57) {
      event.preventDefault();
    }
  }

  loadDetail() {
    if (!this.id) {
      this.clear_model();
      this.loadForm();
    } else {
      const sb = this.RPOSOfInvestmentnPageService.getItemById(this.id).pipe(
        first(),
        catchError((errorMessage) => {
          this.modal.dismiss(errorMessage);
          return of(EMPTY_CUSTOM);
        })
      ).subscribe((res: any) => {
        this.RPOSOfInvestmentnData = res.data;
        this.RPOSOfInvestmentnData.quantity = this.f_currency(res.data.quantity)
        this.loadForm();
      });
      this.subscriptions.push(sb);
    }
  }
  loadForm() {
    this.formGroup = this.fb.group({
      criteria: [this.RPOSOfInvestmentnData.criteria, Validators.compose([Validators.required])],
      units: [this.RPOSOfInvestmentnData.units, Validators.compose([Validators.required])],
      quantity: [this.RPOSOfInvestmentnData.quantity, Validators.compose([Validators.required])],
      reportingPeriod: [this.RPOSOfInvestmentnData.reportingPeriod],
      note: [this.RPOSOfInvestmentnData.note],
      year: [this.RPOSOfInvestmentnData.year]
    });
    this.loadUnit();
    
    this.subscriptions.push(
      this.formGroup.controls.criteria.valueChanges.subscribe((x) => {
        this.loadUnit();
      })
    );
        
    this.formGroup.controls.quantity.valueChanges.subscribe(x => {
      this.formGroup.patchValue({
        'quantity' : this.f_currency(x)
      }, { emitEvent: false })
    })
  }
  clear_model() {
    EMPTY_CUSTOM.ReportOperationalStatusOfInvestmentProjectsId = '',
      EMPTY_CUSTOM.criteria = '00000000-0000-0000-0000-000000000000',
      EMPTY_CUSTOM.units = '',
      EMPTY_CUSTOM.note = '',
      EMPTY_CUSTOM.quantity = null,
      EMPTY_CUSTOM.reportingPeriod = '00000000-0000-0000-0000-000000000000',
      EMPTY_CUSTOM.year = moment().year(),
      this.RPOSOfInvestmentnData = EMPTY_CUSTOM
  }
  
  loadUnit(){
    const find_data = this.CriteriaData.find((x: any) => x.id == this.formGroup.controls.criteria.value)
      if (find_data && find_data.id !== "00000000-0000-0000-0000-000000000000") {
        this.formGroup.patchValue({
          "units": find_data.unit,
        }, { emitEvent: false })
      }
      else {
        this.formGroup.patchValue({
          "unit": '',
        }, { emitEvent: false })
      }
  }
  
  save() {
    this.prepareTypeOfEnergy();
    if (this.RPOSOfInvestmentnData.ReportOperationalStatusOfInvestmentProjectsId != '') {
      this.edit();
    } else {
      this.RPOSOfInvestmentnData.ReportOperationalStatusOfInvestmentProjectsId = this.default_value
      this.create();
    }
  }

  edit() {
    const sbUpdate = this.RPOSOfInvestmentnPageService.update(this.RPOSOfInvestmentnData).pipe(
      tap(() => {
        this.modal.close();
      }),
      catchError((errorMessage) => {
        this.modal.dismiss(errorMessage);
        return of(this.RPOSOfInvestmentnData);
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
    const sbCreate = this.RPOSOfInvestmentnPageService.create(this.RPOSOfInvestmentnData).pipe(
      tap(() => {
        this.modal.close();
      }),
      catchError((errorMessage) => {
        this.modal.dismiss(errorMessage);
        return of(this.RPOSOfInvestmentnData);
      }),
    ).subscribe((res: any) => {
      Swal.fire({
        icon: res.status == 1 ? 'success' : 'error',
        title: res.status == 1 ? 'Thêm mới thành công' : 'Thêm mới thất bại',
        confirmButtonText: 'Xác nhận',
        text: (res.status == 1 ? 'Thêm mới thành công' : res.status == 0 ? res.error.msg : "Thêm mới thất bại" ),
      });
      this.RPOSOfInvestmentnData = EMPTY_CUSTOM
    });
    this.subscriptions.push(sbCreate);
    EMPTY_CUSTOM.ReportOperationalStatusOfInvestmentProjectsId = '';
    this.RPOSOfInvestmentnData = EMPTY_CUSTOM;
  }

  private prepareTypeOfEnergy() {
    const formData = this.formGroup.value;
    this.RPOSOfInvestmentnData.criteria = formData.criteria;
    this.RPOSOfInvestmentnData.reportingPeriod = formData.reportingPeriod;
    this.RPOSOfInvestmentnData.quantity = Number(formData.quantity.replaceAll(',', ''));
    this.RPOSOfInvestmentnData.units = formData.units;
    this.RPOSOfInvestmentnData.note = formData.note;
    this.RPOSOfInvestmentnData.year = formData.year;
  }

  loadKyBaoCao() {
    const data = [{
      id: "00000000-0000-0000-0000-000000000000",
      text: '-- Chọn --'
    }];
    let obj1 = {
      id: "1",
      text: "6 tháng đầu năm",
    };
    let obj2 = {
      id: "2",
      text: "Cả năm",
    }
    data.push(obj1);
    data.push(obj2);
    this.datKyBaoCao = data;
    return this.datKyBaoCao;
  }
  loadCi() {

    this.commonService.getListTargetCC1().subscribe((res: any) => {
      const data_typeofitem = [{
        id: "00000000-0000-0000-0000-000000000000",
        text: '-- Chọn --',
        unit: ''
      },
      ...res.items.map((item: any) => ({
        id: item.industrialManagementTargetId,
        text: item.name,
        unit: item.unit
      }))
    ]
      this.CriteriaData = data_typeofitem;
    })
    return this.CriteriaData;
  }
  addStore() {
    var store = "";
    store = this.formGroup.value.Store;
    if (store == "") {
      return;
    }
    this.lstStore.push(store);
    this.dataSource = this.lstStore;
    this.formGroup.controls.Store.setValue("");
    this.formGroup.controls.Store.clearValidators();
    this.formGroup.controls.Store.updateValueAndValidity();
    this.changeDetectorRefs.detectChanges();

  }
  delStore(item: any) {
    const index: number = this.lstStore.indexOf(item);
    this.lstStore.splice(index, 1);
    this.dataSource = this.lstStore;
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
  
  loadYear(){
    const data = [
      {
        id: 0,
        text: "-- Chọn --"
      }
    ];
    for(let i = 0; i < 30; i++){
      let obj = {
        id: moment().year()- 15 + i,
        text: (moment().year()- 15 + i).toString()
      }
      data.push(obj);
    }
    this.yearData = data;
  }
  
}
