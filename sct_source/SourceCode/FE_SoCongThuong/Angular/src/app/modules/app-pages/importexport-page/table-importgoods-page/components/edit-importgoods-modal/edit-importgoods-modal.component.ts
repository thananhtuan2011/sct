import { ChangeDetectorRef, Component, Input, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal, NgbDateAdapter, NgbDateParserFormatter } from '@ng-bootstrap/ng-bootstrap';
import { of, Subscription } from 'rxjs';
import { catchError, finalize, first, tap } from 'rxjs/operators';
import Swal from 'sweetalert2';

import { Options } from 'select2';

import { ImportGoodsModel } from '../../../_models/importgoods.model';
import { ImportGoodsPageService } from '../../../_services/importgoods-page.service';

const EMPTY_CUSTOM: ImportGoodsModel = {
  id: '',
  importGoodsId: '',
  importGoodsName: '', //Tên mặt hàng
  itemGroupId: '00000000-0000-0000-0000-000000000000', //nhóm mặt hàng
  typeOfEconomicId: '00000000-0000-0000-0000-000000000000', //Loại hình kinh tế
  businessId: '00000000-0000-0000-0000-000000000000', //Doanh nghiệp
  countryId: '00000000-0000-0000-0000-000000000000', //Thị trường
  amount: '', //Số lượng
  amountUnitId: '00000000-0000-0000-0000-000000000000', //Đơn vị tính
  price: 0, //Trị giá
  importTime: '', //Thời gian nhập khẩu
};

@Component({
  selector: 'app-edit-importgoods-modal',
  templateUrl: './edit-importgoods-modal.component.html',
  styleUrls: ['./edit-importgoods-modal.component.scss'],
})

export class EditImportGoodsModalComponent implements OnInit, OnDestroy {
  @Input() id: any;
  isLoading$: any;
  importgoodsData: ImportGoodsModel;
  formGroup: FormGroup;
  options: Options;

  ItemGroupData: Array<any>;
  TypeOfEconomicData: Array<any>;
  BusinessData: Array<any>;
  CountryData: Array<any>;
  UnitData: Array<any>;

  private subscriptions: Subscription[] = [];
  show: boolean = false;
  apiLoaded: number = 0;

  constructor(
    private importgoodsService: ImportGoodsPageService,
    private fb: FormBuilder, public modal: NgbActiveModal,
  ) { }

  ngOnInit(): void {
    this.isLoading$ = this.importgoodsService.isLoading$;
    this.loadItemGroup();
    this.loadTypeOfEconomic();
    this.loadBusiness();
    this.loadCountry();
    this.loadUnit();

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

  loadItemGroup() {
    this.importgoodsService.loadItemGroup().subscribe((res: any) => {
      var itemgroups = [
        {
          id: '00000000-0000-0000-0000-000000000000',
          text: '-- Chọn --',
        },
      ];
      for (var item of res.items) {
        let itemgroup = {
          id: item.itemGroupId,
          text: item.itemGroupName
        }
        itemgroups.push(itemgroup)
      }
      this.ItemGroupData = itemgroups
      this.loadImportGoods();
    })
  }

  loadTypeOfEconomic() {
    this.importgoodsService.loadTypeOfEconomic().subscribe((res: any) => {
      var typeofeconomics = [
        {
          id: '00000000-0000-0000-0000-000000000000',
          text: '-- Chọn --',
        },
      ];
      for (var item of res.items) {
        let typeofeconomic = {
          id: item.typeOfEconomicId,
          text: item.typeOfEconomicName
        }
        typeofeconomics.push(typeofeconomic)
      }
      this.TypeOfEconomicData = typeofeconomics.sort((i1, i2) => {
        if (i1.text > i2.text) {
          return 1;
        }
        if (i1.text < i2.text) {
          return -1;
        }
        return 0;
      });
      this.loadImportGoods();
    })
  }

  loadBusiness() {
    this.importgoodsService.loadBusiness().subscribe((res: any) => {
      var businesses = [
        {
          id: '00000000-0000-0000-0000-000000000000',
          text: '-- Chọn --',
        },
      ];
      for (var item of res.items) {
        let business = {
          id: item.businessId,
          text: item.businessName
        }
        businesses.push(business)
      }
      this.BusinessData = businesses
      this.loadImportGoods();
    })
  }

  loadCountry() {
    this.importgoodsService.loadCountry().subscribe((res: any) => {
      var countries = [
        {
          id: '00000000-0000-0000-0000-000000000000',
          text: '-- Chọn --',
        },
      ];
      for (var item of res.items) {
        let country = {
          id: item.countryId,
          text: item.countryName
        }
        countries.push(country)
      }
      this.CountryData = countries
      this.loadImportGoods();
    })
  }

  loadUnit() {
    this.importgoodsService.loadUnit().subscribe((res: any) => {
      var units = [
        {
          id: '00000000-0000-0000-0000-000000000000',
          text: '-- Chọn --',
        },
      ];
      for (var item of res.items) {
        let unit = {
          id: item.unitId,
          text: item.unitName
        }
        units.push(unit)
      }
      this.UnitData = units.sort((i1, i2) => {
        if (i1.text > i2.text) {
          return 1;
        }
        if (i1.text < i2.text) {
          return -1;
        }
        return 0;
      });
      this.loadImportGoods();
    })
  }

  loadImportGoods() {
    if (this.apiLoaded < 4) {
      this.apiLoaded++;
      return
    }
    if (!this.id) {
      this.clearModel();
      this.loadForm();
    } else {
      const sb = this.importgoodsService.getItemById(this.id).pipe(
        first(),
        catchError((errorMessage) => {
          this.modal.dismiss(errorMessage);
          return of(EMPTY_CUSTOM);
        })
      ).subscribe((res: any) => {
        this.importgoodsData = res.items[0];
        this.importgoodsData.amount = this.f_currency(res.items[0].amount);
        this.importgoodsData.price = this.f_currency(res.items[0].price);
        this.loadForm();
      });
      this.subscriptions.push(sb);
    }
  }

  loadForm() {
    this.formGroup = this.fb.group({
      ImportGoodsName: [this.importgoodsData.importGoodsName, Validators.required],
      ItemGroupId: [this.importgoodsData.itemGroupId],
      TypeOfEconomicId: [this.importgoodsData.typeOfEconomicId],
      BusinessId: [this.importgoodsData.businessId],
      CountryId: [this.importgoodsData.countryId],
      Amount: [this.importgoodsData.amount, Validators.required],
      AmountUnitId: [this.importgoodsData.amountUnitId],
      Price: [this.importgoodsData.price, Validators.required],
      ImportTime: [this.importgoodsData.importTime, Validators.required]
    });
    
    this.formGroup.controls.Amount.valueChanges.subscribe((x) => {
      this.formGroup.patchValue({
        "Amount": this.f_currency(x)
      }, { emitEvent: false })
    })
    
    this.formGroup.controls.Price.valueChanges.subscribe((x) => {
      this.formGroup.patchValue({
        "Price": this.f_currency(x)
      }, { emitEvent: false })
    })
    this.show = true;
  }

  save() {
    this.prepareImportGoods();
    if (this.importgoodsData.importGoodsId != '00000000-0000-0000-0000-000000000000') {
      this.edit();
    } else {
      this.create();
    }
  }

  edit() {
    const sbUpdate = this.importgoodsService.update(this.importgoodsData).pipe(
      tap(() => {
        this.modal.close();
      }),
      catchError((errorMessage) => {
        this.modal.dismiss(errorMessage);
        return of(this.importgoodsData);
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
    const sbCreate = this.importgoodsService.create(this.importgoodsData).pipe(
      tap(() => {
        this.modal.close();
      }),
      catchError((errorMessage) => {
        this.modal.dismiss(errorMessage);
        return of(this.importgoodsData);
      }),
    ).subscribe((res: any) => {
      Swal.fire({
        icon: res.status == 1 ? 'success' : 'error',
        title: res.status == 1 ? 'Thêm mới thành công' : 'Thêm mới thất bại',
        confirmButtonText: 'Xác nhận',
        text: 'Thêm mới ' + (res.status == 1 ? 'thành công' : 'thất bại'),
      });
      this.importgoodsData = EMPTY_CUSTOM
    });
    this.subscriptions.push(sbCreate);
  }

  clearModel() {
    EMPTY_CUSTOM.importGoodsId = '00000000-0000-0000-0000-000000000000',
      EMPTY_CUSTOM.importGoodsName = '',
      EMPTY_CUSTOM.itemGroupId = '00000000-0000-0000-0000-000000000000',
      EMPTY_CUSTOM.typeOfEconomicId = '00000000-0000-0000-0000-000000000000',
      EMPTY_CUSTOM.businessId = '00000000-0000-0000-0000-000000000000',
      EMPTY_CUSTOM.countryId = '00000000-0000-0000-0000-000000000000',
      EMPTY_CUSTOM.amount = '',
      EMPTY_CUSTOM.amountUnitId = '00000000-0000-0000-0000-000000000000',
      EMPTY_CUSTOM.price = 0,
      EMPTY_CUSTOM.importTime = '',
      this.importgoodsData = EMPTY_CUSTOM;
  }

  private prepareImportGoods() {
    const formData = this.formGroup.value;
    this.importgoodsData.importGoodsName = formData.ImportGoodsName;
    this.importgoodsData.itemGroupId = formData.ItemGroupId;
    this.importgoodsData.typeOfEconomicId = formData.TypeOfEconomicId;
    this.importgoodsData.businessId = formData.BusinessId;
    this.importgoodsData.countryId = formData.CountryId;
    this.importgoodsData.amount = formData.Amount ? formData.Amount.replaceAll(',' , '') : null;
    this.importgoodsData.amountUnitId = formData.AmountUnitId;
    this.importgoodsData.price = formData.Price ? Number(formData.Price.replaceAll(',' , '')) : null;
    this.importgoodsData.importTime = formData.ImportTime;
  }

  prenventInputNonNumber(event: any) {
    if (event.which < 48 || event.which > 57) {
      event.preventDefault();
    }
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
    if (value == '00000000-0000-0000-0000-000000000000' || value < 0) {
      control.setErrors({ defaultvalue: true });
    }
    else {
      control.setErrors({ defaultvalue: null });
      control.updateValueAndValidity();
    }
    return control.invalid && (control.dirty || control.touched);
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
  
  f_currency(value: any, args?: any): any {
    let nbr = Number((value + '').replace(/,|-/g, ""));
    const result = (nbr + '').replace(/(\d)(?=(\d{3})+(?!\d))/g, "$1,");
    return result
  }
}
