import { ChangeDetectorRef, Component, Input, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal, NgbDateAdapter, NgbDateParserFormatter, NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { of, Subscription } from 'rxjs';
import { catchError, finalize, first, tap } from 'rxjs/operators';
import Swal from 'sweetalert2';

import { BuildAndUpgradeModel } from '../../../_models/buildandupgrade.model';
import { BuildAndUpgradePageService } from '../../../_services/buildandupgrade-page.service';
import { EditCommercialManagementModalComponent } from '../../../table-commercialmanagement-page/components/edit-commercialmanagement-modal/edit-commercialmanagement-modal.component';
import { Options } from 'select2';
import * as moment from 'moment';

const EMPTY_CUSTOM: BuildAndUpgradeModel = {
  id: '',
  buildAndUpgradeId: '00000000-0000-0000-0000-000000000000', //Id
  buildAndUpgradeName: '', //Tên danh mục
  address: '', //Địa chỉ
  districtId: '00000000-0000-0000-0000-000000000000',
  communeId: '00000000-0000-0000-0000-000000000000',
  commercialId: '00000000-0000-0000-0000-000000000000',

  totalInvestment: null, //Tổng vốn đầu tư
  realizedCapital: null, // Vốn đã thực hiện
  budgetCapital: null, //Vốn ngân sách
  landUseCapital: null, // Vốn chính quyền sử dụng đất
  loans: null, //Vốn vay
  anotherCapital: null, //Vốn khác
  isBuild: false,  //Có phải xây dựng không
  isUpgrade: false, //Có phải nâng cấp không
  note: '', //Ghi chú

  //Unit
  totalInvestmentUnit: 'null',
  realizedCapitalUnit: 'null',
  budgetCapitalUnit: 'null',
  landUseCapitalUnit: 'null',
  loansUnit: 'null',
  anotherCapitalUnit: 'null',
  year: null
};

@Component({
  selector: 'app-edit-buildandupgrade-modal',
  templateUrl: './edit-buildandupgrade-modal.component.html',
  styleUrls: ['./edit-buildandupgrade-modal.component.scss'],

})
export class EditBuildAndUpgradeModalComponent implements OnInit, OnDestroy {
  @Input() id: any;
  isLoading$: any;
  buildandupgradeData: BuildAndUpgradeModel;
  formGroup: FormGroup;
  options: Options;
  toggleBool: boolean=true;
  textBool: boolean = true;
  unitData: Array<any>;
  reportYear: any = moment().year();
  public marketData: any = [];
  public marketDataFilter: Array<any>;

  private subscriptions: Subscription[] = [];

  constructor(
    private buildandupgradeService: BuildAndUpgradePageService,
    private fb: FormBuilder, public modal: NgbActiveModal,
    private modalService: NgbModal,
  ) { }

  ngOnInit(): void {
    this.isLoading$ = this.buildandupgradeService.isLoading$;
    this.loadBuildAndUpgrade();
    this.loadmarket();
    this.options = {
      theme: 'bootstrap5',
      templateSelection: this.templateSelection,
    };

    this.unitData = [
      {
        id: 'null',
        text: 'Chọn',
        value: null,
      },
      {
        id: 'TY',
        text: 'Tỷ',
        value: 1_000_000_000,
      },
      {
        id: 'TRIEU',
        text: 'Triệu',
        value: 1_000_000,
      },
    ]
  }

  public templateSelection = (state: any): JQuery | string => {
    if (!state.id) {
      return state.text;
    }
    return jQuery('<span class="form-select form-select-solid form-select-lg">' + state.text + '</span>');
  }

  loadBuildAndUpgrade() {
    if (!this.id) {
      this.clearmodel();
      this.loadForm();
    } else {
      const sb = this.buildandupgradeService.getItemById(this.id).pipe(
        first(),
        catchError((errorMessage) => {
          this.modal.dismiss(errorMessage);
          return of(EMPTY_CUSTOM);
        })
      ).subscribe((res: any) => {
        this.buildandupgradeData = res.items[0];
        this.buildandupgradeData.totalInvestment = this.setvalue(res.items[0].totalInvestmentUnit, res.items[0].totalInvestment);
        this.buildandupgradeData.realizedCapital = this.setvalue(res.items[0].realizedCapitalUnit, res.items[0].realizedCapital);
        this.buildandupgradeData.budgetCapital = this.setvalue(res.items[0].budgetCapitalUnit, res.items[0].budgetCapital);
        this.buildandupgradeData.landUseCapital = this.setvalue(res.items[0].landUseCapitalUnit, res.items[0].landUseCapital);
        this.buildandupgradeData.loans = this.setvalue(res.items[0].loansUnit, res.items[0].loans);
        this.buildandupgradeData.anotherCapital = this.setvalue(res.items[0].anotherCapitalUnit, res.items[0].anotherCapital);
        this.loadForm();
      });
      this.subscriptions.push(sb);
    }
  }

  setvalue(IdUnit: any, value: any) {
    if (IdUnit == 'TY') {
      return value / 1_000_000_000
    }
    if (IdUnit == 'TRIEU') {
      return value / 1_000_000
    }
    else {
      return null
    }
  }

  loadForm() {
    this.formGroup = this.fb.group({
      BuildAndUpgradeName: [this.buildandupgradeData.buildAndUpgradeName],
      Address: [this.buildandupgradeData.address],
      CommercialId: [this.buildandupgradeData.commercialId],
      DistrictId: [this.buildandupgradeData.districtId],
      CommuneId: [this.buildandupgradeData.communeId],
      TotalInvestment: [this.buildandupgradeData.totalInvestment, Validators.pattern(/^[0-9]+(\.[0-9]{1,2})?$/)],
      RealizedCapital: [this.buildandupgradeData.realizedCapital, Validators.pattern(/^[0-9]+(\.[0-9]{1,2})?$/)],
      BudgetCapital: [this.buildandupgradeData.budgetCapital, Validators.pattern(/^[0-9]+(\.[0-9]{1,2})?$/)],
      LandUseCapital: [this.buildandupgradeData.landUseCapital, Validators.pattern(/^[0-9]+(\.[0-9]{1,2})?$/)],
      Loans: [this.buildandupgradeData.loans, Validators.pattern(/^[0-9]+(\.[0-9]{1,2})?$/)],
      AnotherCapital: [this.buildandupgradeData.anotherCapital, Validators.pattern(/^[0-9]+(\.[0-9]{1,2})?$/)],
      IsBuild: [this.buildandupgradeData.isBuild],
      IsUpgrade: [this.buildandupgradeData.isUpgrade],
      Note: [this.buildandupgradeData.note],

      //unit
      TotalInvestmentUnit: [this.buildandupgradeData.totalInvestmentUnit],
      RealizedCapitalUnit: [this.buildandupgradeData.realizedCapitalUnit],
      BudgetCapitalUnit: [this.buildandupgradeData.budgetCapitalUnit],
      LandUseCapitalUnit: [this.buildandupgradeData.landUseCapitalUnit],
      LoansUnit: [this.buildandupgradeData.loansUnit],
      AnotherCapitalUnit: [this.buildandupgradeData.anotherCapitalUnit],
      Year: [this.buildandupgradeData.year, Validators.required],
      District: '',
      Commune: '',
      AddressMarket: ''
    });
    this.loadInfoMarket();
    this.subscriptions.push(
      this.formGroup.controls.CommercialId.valueChanges.subscribe((x) => {
        this.loadInfoMarket();
      })
    );
  }
  
  loadInfoMarket(){
    const find_data = this.marketData.find((x: any) => x.id == this.formGroup.controls.CommercialId.value)
    if (find_data && find_data.id !== "00000000-0000-0000-0000-000000000000") {
      this.formGroup.patchValue({
        "District": find_data.districtName,
        "Commune": find_data.communeName,
        "AddressMarket": find_data.address
      }, { emitEvent: false })
    }
    else {
      this.formGroup.patchValue({
        "District": '',
        "Commune": '',
        "AddressMarket": ''
      }, { emitEvent: false })
    }
  }

  clearmodel() {
    EMPTY_CUSTOM.buildAndUpgradeId = '00000000-0000-0000-0000-000000000000';
    EMPTY_CUSTOM.buildAndUpgradeName = '';
    EMPTY_CUSTOM.address = '';
    EMPTY_CUSTOM.commercialId = '00000000-0000-0000-0000-000000000000';
    EMPTY_CUSTOM.districtId = '00000000-0000-0000-0000-000000000000',
    EMPTY_CUSTOM.communeId = '00000000-0000-0000-0000-000000000000',
    EMPTY_CUSTOM.totalInvestment = null;
    EMPTY_CUSTOM.realizedCapital = null;
    EMPTY_CUSTOM.budgetCapital = null;
    EMPTY_CUSTOM.landUseCapital = null;
    EMPTY_CUSTOM.loans = null;
    EMPTY_CUSTOM.anotherCapital = null;
    EMPTY_CUSTOM.isBuild = false;
    EMPTY_CUSTOM.isUpgrade = false;
    EMPTY_CUSTOM.note = '';

    //Unit
    EMPTY_CUSTOM.totalInvestmentUnit = 'null',
    EMPTY_CUSTOM.realizedCapitalUnit = 'null',
    EMPTY_CUSTOM.budgetCapitalUnit = 'null',
    EMPTY_CUSTOM.landUseCapitalUnit = 'null',
    EMPTY_CUSTOM.loansUnit = 'null',
    EMPTY_CUSTOM.anotherCapitalUnit = 'null',
    EMPTY_CUSTOM.year = null

    this.buildandupgradeData = EMPTY_CUSTOM;
  }

  buildorupgrade(controlName: any) {
    const value = this.formGroup.controls[controlName].value
    if (controlName == 'IsBuild' && value == true) {
      this.toggleBool = false;
      // this.textBool = true;
      this.formGroup.controls['IsUpgrade'].setValue(!value)
    }
    if (controlName == 'IsUpgrade' && value == true) {
      this.toggleBool = true;
      // this.textBool = false;
      this.formGroup.controls['IsBuild'].setValue(!value)
    }
  }

  save() {
    this.prepareBuildAndUpgradeData();
    if (this.buildandupgradeData.buildAndUpgradeId != '00000000-0000-0000-0000-000000000000') {
      this.edit();
    } else {
      this.create();
    }
  }
  getDistrictValue(id: any){
    if (id){
      const districtId = this.marketDataFilter.find(x => x.id == id).districtId
      return districtId;
    }
    else {
      return '00000000-0000-0000-0000-000000000000'
    }
  }

  getCommuneValue(id: any){
    if (id){
      const communeId = this.marketDataFilter.find(x => x.id == id).communeId
      return communeId;
    }
    else {
      return '00000000-0000-0000-0000-000000000000'
    }
  }

  edit() {
    const sbUpdate = this.buildandupgradeService.update(this.buildandupgradeData).pipe(
      tap(() => {
        this.modal.close();
      }),
      catchError((errorMessage) => {
        this.modal.dismiss(errorMessage);
        return of(this.buildandupgradeData);
      }),
    ).subscribe((res: any) => {
      Swal.fire({
        icon: res.status == 1 ? 'success' : 'error',
        title: res.status == 1 ? 'Chỉnh sửa thành công' : 'Chỉnh sửa thất bại',
        confirmButtonText: 'Xác nhận',
        text: 'Chỉnh sửa ' + (res.status == 1 ? 'thành công' : 'thất bại'),
      });
    });
    this.subscriptions.push(sbUpdate);
  }

  create() {
    const sbCreate = this.buildandupgradeService.create(this.buildandupgradeData).pipe(
      tap(() => {
        this.modal.close();
      }),
      catchError((errorMessage) => {
        this.modal.dismiss(errorMessage);
        return of(this.buildandupgradeData);
      }),
    ).subscribe((res: any) => {
      Swal.fire({
        icon: res.status == 1 ? 'success' : 'error',
        title: res.status == 1 ? 'Thêm mới thành công' : 'Thêm mới thất bại',
        confirmButtonText: 'Xác nhận',
        text: 'Thêm mới ' + (res.status == 1 ? 'thành công' : 'thất bại'),
      });
    });
    this.subscriptions.push(sbCreate);
  }

  getsavevalue(Value:any, IdUnit: any) {
    if (Value){
      const UnitValue = this.unitData.find(x => x.id == IdUnit).value
      return Value * UnitValue
    }
    else {
      return null
    }
  }

  private prepareBuildAndUpgradeData() {
    const formData = this.formGroup.value;
    this.buildandupgradeData.buildAndUpgradeName = formData.BuildAndUpgradeName;
    this.buildandupgradeData.address = formData.Address;
    this.buildandupgradeData.commercialId = formData.CommercialId;
    this.buildandupgradeData.districtId = this.getDistrictValue(formData.CommercialId);
    this.buildandupgradeData.communeId = this.getCommuneValue(formData.CommercialId);
    this.buildandupgradeData.totalInvestment = this.getsavevalue(formData.TotalInvestment, formData.TotalInvestmentUnit);
    this.buildandupgradeData.realizedCapital = this.getsavevalue(formData.RealizedCapital, formData.RealizedCapitalUnit);
    this.buildandupgradeData.budgetCapital = this.getsavevalue(formData.BudgetCapital, formData.BudgetCapitalUnit);
    this.buildandupgradeData.landUseCapital = this.getsavevalue(formData.LandUseCapital, formData.LandUseCapitalUnit);
    this.buildandupgradeData.loans = this.getsavevalue(formData.Loans, formData.LoansUnit);
    this.buildandupgradeData.anotherCapital = this.getsavevalue(formData.AnotherCapital, formData.AnotherCapitalUnit);
    this.buildandupgradeData.isBuild = formData.IsBuild;
    this.buildandupgradeData.isUpgrade = formData.IsUpgrade;
    this.buildandupgradeData.note = formData.Note;

    this.buildandupgradeData.totalInvestmentUnit = formData.TotalInvestmentUnit;
    this.buildandupgradeData.realizedCapitalUnit = formData.RealizedCapitalUnit;
    this.buildandupgradeData.budgetCapitalUnit = formData.BudgetCapitalUnit;
    this.buildandupgradeData.landUseCapitalUnit = formData.LandUseCapitalUnit;
    this.buildandupgradeData.loansUnit = formData.LoansUnit;
    this.buildandupgradeData.anotherCapitalUnit = formData.AnotherCapitalUnit;
    this.buildandupgradeData.year = formData.Year;
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
  isCheckBoxChecked(): boolean {
    const control_build = this.formGroup.controls['IsBuild']
    const control_upgrade = this.formGroup.controls['IsUpgrade']
    const check_build = control_build.value;
    const check_upgrade = control_upgrade.value;
    if (check_build == check_upgrade) {
      control_build.setErrors({ none_checked: true })
    }
    return control_build.invalid && (control_build.dirty || control_build.touched || control_upgrade.dirty || control_upgrade.touched);
  }

  isnullcapitalunit(controlName: any, controlNameUnit: any) {
    const control = this.formGroup.controls[controlName];
    const controlUnit = this.formGroup.controls[controlNameUnit];
    const value = control.value;
    const unit_value = controlUnit.value;
    if (value && (unit_value == 'null')) {
      controlUnit.setErrors({noneUnit : true});
    }
    else {
      controlUnit.setErrors({noneUnit : null});
      controlUnit.updateValueAndValidity();
    }
    return controlUnit.invalid;
  }

  isnullcapital(controlName: any, controlNameUnit: any) {
    const control = this.formGroup.controls[controlName];
    const controlUnit = this.formGroup.controls[controlNameUnit];
    const value = control.value;
    const unit_value = controlUnit.value;
    if (!value && (unit_value != 'null')) {
      control.setErrors({noneValue : true});
    }
    else {
      control.setErrors({noneValue : null});
      control.updateValueAndValidity();
    };
    return control.invalid;
  }

  prenventInputNonNumber(event: any) {
    if (event.which < 48 || event.which > 57) {
      event.preventDefault();
    }
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

  add_comercial_management() {
    this.edit_commercial(0);
  }

  edit_commercial(status: any) {
    // const modalRef = this.modalService.open(LoginModalComponent, { size: 'md',centered: true });
    const modalRef = this.modalService.open(EditCommercialManagementModalComponent, { size: 'lg' });
    modalRef.componentInstance.status = status;
    modalRef.result.then(() =>
    this.loadmarket(),
    () => { }
  );
  }

  loadmarket() {
    this.buildandupgradeService.loadMarket().subscribe((res: any) => {
      var markets = [
        {
          id: '00000000-0000-0000-0000-000000000000',
          text: '-- Chọn --',
          districtId: '00000000-0000-0000-0000-000000000000',
          communeId: '00000000-0000-0000-0000-000000000000',
          districtName: '',
          communeName: '',
          address: ''
        },
      ];
      for (var item of res.items) {
        let obj_market = {
          id: item.commercialId,
          text: item.commercialName,
          districtId: item.districtId,
          communeId: item.communeId,
          districtName: item.districtName,
          communeName: item.communeName,
          address: item.address
        }
        markets.push(obj_market)
      }
      this.marketData = markets
      this.marketDataFilter = markets
    })
  }
}
