import { ChangeDetectorRef, Component, Input, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal, NgbDateAdapter, NgbDateParserFormatter } from '@ng-bootstrap/ng-bootstrap';
import { of, Subscription } from 'rxjs';
import { catchError, finalize, first, tap } from 'rxjs/operators';
import { SelectOptionData } from 'src/app/_metronic/shared/components/select-custom/select-custom.interface';
import Swal from 'sweetalert2';
import { Options } from 'select2';
import { RPOSOfConstructionPageService } from '../../../_services/RP-of-con-page.service';
import { RPOSOfConstructionModel } from '../../../_models/RP-of-con.model';
import { CommonService } from 'src/app/_metronic/shared/services/common.service';
import * as moment from 'moment';

const EMPTY_CUSTOM: RPOSOfConstructionModel = {
  id: '',
  ReportOperationalStatusOfConstructionInvestmentProjectsId: '',
  criteria: '00000000-0000-0000-0000-000000000000',
  units: '',
  note: '',
  quantity: null,
  reportingPeriod: '00000000-0000-0000-0000-000000000000',
  year: moment().year()
}
@Component({
  selector: 'app-edit-rp-of-con-modal.component',
  templateUrl: './edit-RP-of-con-modal.component.html',
  styleUrls: ['./edit-RP-of-con-modal.component.scss'],

})
export class EditRPOSOfConstructionModalComponent implements OnInit, OnDestroy {
  @Input() id: any;
  isLoading$: any;
  rPOSOfConstructionData: RPOSOfConstructionModel;
  formGroup: FormGroup;
  public options: Options;
  yearData: any = [];
  dataSource: any[] = [];
  lstStore: any[] = [];
  displayedColumns: string[] = ['stt', 'name', 'action'];
  public datKyBaoCao: Array<SelectOptionData>;

  private subscriptions: Subscription[] = [];
  public default_value = "00000000-0000-0000-0000-000000000000"
  public CriteriaData: any = [];

  constructor(
    private rPOSOfConstructionPageService: RPOSOfConstructionPageService,
    private fb: FormBuilder, public modal: NgbActiveModal,
    private changeDetectorRefs: ChangeDetectorRef,
    private commonService: CommonService,
  ) { }

  ngOnInit(): void {
    this.isLoading$ = this.rPOSOfConstructionPageService.isLoading$;
    (async () => {
      this.loadYear();
      this.loadKyBaoCao();
      this.loadCi();
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
      const sb = this.rPOSOfConstructionPageService.getItemById(this.id).pipe(
        first(),
        catchError((errorMessage) => {
          this.modal.dismiss(errorMessage);
          return of(EMPTY_CUSTOM);
        })
      ).subscribe((res: any) => {
        this.rPOSOfConstructionData = res.data;
        this.rPOSOfConstructionData.quantity = this.f_currency(res.data.quantity);
        this.loadForm();
      });
      this.subscriptions.push(sb);
    }
  }

  loadForm() {
    this.formGroup = this.fb.group({
      criteria: [this.rPOSOfConstructionData.criteria, Validators.compose([Validators.required])],
      units: [this.rPOSOfConstructionData.units, Validators.compose([Validators.required])],
      quantity: [this.rPOSOfConstructionData.quantity, Validators.compose([Validators.required])],
      reportingPeriod: [this.rPOSOfConstructionData.reportingPeriod],
      note: [this.rPOSOfConstructionData.note],
      year: [this.rPOSOfConstructionData.year]
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

  clear_model() {
    EMPTY_CUSTOM.ReportOperationalStatusOfConstructionInvestmentProjectsId = '',
      EMPTY_CUSTOM.criteria = '00000000-0000-0000-0000-000000000000',
      EMPTY_CUSTOM.units = '',
      EMPTY_CUSTOM.note = '',
      EMPTY_CUSTOM.quantity = null,
      EMPTY_CUSTOM.reportingPeriod = '00000000-0000-0000-0000-000000000000',
      EMPTY_CUSTOM.year = moment().year(),
      this.rPOSOfConstructionData = EMPTY_CUSTOM
  }

  save() {
    this.prepareTypeOfEnergy();
    if (this.rPOSOfConstructionData.ReportOperationalStatusOfConstructionInvestmentProjectsId != '') {
      this.edit();
    } else {
      this.rPOSOfConstructionData.ReportOperationalStatusOfConstructionInvestmentProjectsId = this.default_value
      this.create();
    }
  }

  edit() {
    const sbUpdate = this.rPOSOfConstructionPageService.update(this.rPOSOfConstructionData).pipe(
      tap(() => {
        this.modal.close();
      }),
      catchError((errorMessage) => {
        this.modal.dismiss(errorMessage);
        return of(this.rPOSOfConstructionData);
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
    const sbCreate = this.rPOSOfConstructionPageService.create(this.rPOSOfConstructionData).pipe(
      tap(() => {
        this.modal.close();
      }),
      catchError((errorMessage) => {
        this.modal.dismiss(errorMessage);
        return of(this.rPOSOfConstructionData);
      }),
    ).subscribe((res: any) => {
      Swal.fire({
        icon: res.status == 1 ? 'success' : 'error',
        title: res.status == 1 ? 'Thêm mới thành công' : 'Thêm mới thất bại',
        confirmButtonText: 'Xác nhận',
        text: (res.status == 1 ? 'Thêm mới thành công' : res.status == 0 ? res.error.msg : "Thêm mới thất bại" ),
      });
      this.rPOSOfConstructionData = EMPTY_CUSTOM
    });
    this.subscriptions.push(sbCreate);
    EMPTY_CUSTOM.ReportOperationalStatusOfConstructionInvestmentProjectsId = '';
    this.rPOSOfConstructionData = EMPTY_CUSTOM;
  }

  private prepareTypeOfEnergy() {
    const formData = this.formGroup.value;
    this.rPOSOfConstructionData.criteria = formData.criteria;
    this.rPOSOfConstructionData.reportingPeriod = formData.reportingPeriod;
    this.rPOSOfConstructionData.quantity = Number(formData.quantity.replaceAll(',', ''));
    this.rPOSOfConstructionData.units = formData.units;
    this.rPOSOfConstructionData.note = formData.note;
    this.rPOSOfConstructionData.year = formData.year;
  }

  loadKyBaoCao() {
    const data = [{
      id: "00000000-0000-0000-0000-000000000000",
      text: '-- Chọn --'
    }];
    let obj1 = {
      id: "1",
      text: "6 tháng ",
    };
    let obj2 = {
      id: "2",
      text: "Năm",
    }
    data.push(obj1);
    data.push(obj2);
    this.datKyBaoCao = data;
    return this.datKyBaoCao;
  }

  loadCi() {
    this.commonService.getListTargetCCN2().subscribe((res: any) => {
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

  isDefaultValue(controlName: any) {
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
