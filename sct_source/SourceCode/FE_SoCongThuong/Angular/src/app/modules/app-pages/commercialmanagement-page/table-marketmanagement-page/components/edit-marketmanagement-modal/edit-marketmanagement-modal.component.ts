import { ChangeDetectorRef, Component, Input, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal, NgbDateAdapter, NgbDateParserFormatter, NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { of, Subscription } from 'rxjs';
import { catchError, finalize, first, tap } from 'rxjs/operators';
import Swal from 'sweetalert2';

import { MarketManagementModel } from '../../../_models/marketmanagement.model';
import { MarketManagementPageService } from '../../../_services/marketmanagement-page.service';

import { Options } from 'select2';
import { AddBusinessLineModalComponent } from '../edit-businessline-modal/edit-businessline-modal.component';

const EMPTY_CUSTOM: MarketManagementModel = {
  id: '',
  marketManagementId: '00000000-0000-0000-0000-000000000000',
  districtId: '00000000-0000-0000-0000-000000000000',
  communeId: '00000000-0000-0000-0000-000000000000',
  marketId: '00000000-0000-0000-0000-000000000000',
  nganhHangKinhDoanh: '00000000-0000-0000-0000-000000000000',
  boothNumber: null,
  giaTrongNhaLong: null,
  giaNgoaiNhaLong: null,
  deXuatGiaMoi: null,
  note: '',
  matHang: []
};

@Component({
  selector: 'app-edit-marketmanagement-modal',
  templateUrl: './edit-marketmanagement-modal.component.html',
  styleUrls: ['./edit-marketmanagement-modal.component.scss'],

})
export class EditMarketManagementModalComponent implements OnInit, OnDestroy {
  @Input() id: any;
  @Input() type: any;
  isLoading$: any;
  marketmanagementData: MarketManagementModel;
  formGroup: FormGroup;
  options: Options;

  public districtData: Array<any>;
  // public districtId: any;
  public communeData: Array<any>;
  public communeDataFilter: Array<any>;
  // public communeId: any;
  public marketData: Array<any>;
  public marketDataFilter: Array<any>;
  // public marketId : any;
  public businessLineData: Array<any>;

  public businessLineDetail : any = []
  private subscriptions: Subscription[] = [];
  loaded: number = 0;

  constructor(
    private marketmanagementService: MarketManagementPageService,
    private fb: FormBuilder, public modal: NgbActiveModal,
    private modalService: NgbModal,
  ) { }

  ngOnInit(): void {
    this.isLoading$ = this.marketmanagementService.isLoading$;
    this.loaddistrict();
    this.loadcommune();
    this.loadmarket();
    this.loadbusinessLine();
    this.options = {
      theme: 'bootstrap5',
      templateSelection: this.templateSelection,
    };
  }

  checkBeforeLoadForm() {
    if (this.loaded > 3) {
      this.loadMarketManagement();
    }
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

  loadMarketManagement() {
    if (!this.id) {
      this.clearmodel();
      this.loadForm();
    } else {
      const sb = this.marketmanagementService.getItemById(this.id).pipe(
        first(),
        catchError((errorMessage) => {
          this.modal.dismiss(errorMessage);
          return of(EMPTY_CUSTOM);
        })
      ).subscribe((res: any) => {
        this.marketmanagementData = res.items[0];
        this.marketmanagementData.giaNgoaiNhaLong = this.f_currency(res.items[0].giaNgoaiNhaLong);
        this.marketmanagementData.giaTrongNhaLong = this.f_currency(res.items[0].giaTrongNhaLong);
        this.marketmanagementData.deXuatGiaMoi = this.f_currency(res.items[0].deXuatGiaMoi);
        this.marketmanagementData.boothNumber = this.f_currency(res.items[0].boothNumber);
        this.loadForm();
      });
      this.subscriptions.push(sb);
    }
  }

  //load data
  loaddistrict() {
    this.marketmanagementService.loadDistrict().subscribe((res: any) => {
      var districts = [
        {
          id: '00000000-0000-0000-0000-000000000000',
          text: '-- Chọn --',
        },
      ];
      for (var item of res.items) {
        let obj_district = {
          id: item.districtId,
          text: item.districtName,
        }
        districts.push(obj_district)
      }
      this.districtData = districts.sort((a, b) => {
        if (a.text < b.text) {
          return -1;
        }
        if (a.text > b.text) {
          return 1;
        }
        return 0;
      });

      this.loaded++;
      this.checkBeforeLoadForm();
    })
  }

  loadcommune() {
    this.marketmanagementService.loadCommune().subscribe((res: any) => {
      var communes = [
        {
          id: '00000000-0000-0000-0000-000000000000',
          text: '-- Chọn --',
          districtId: '00000000-0000-0000-0000-000000000000',
        },
      ];
      for (var item of res.items) {
        let obj_commune = {
          id: item.communeId,
          text: item.communeName,
          districtId: item.districtId,
        }
        communes.push(obj_commune)
      }
      this.communeData = communes.sort((a, b) => {
        if (a < b) {
          return -1;
        }
        if (a > b) {
          return 1;
        }
        return 0;
      });
      this.communeDataFilter = communes.sort((a, b) => {
        if (a < b) {
          return -1;
        }
        if (a > b) {
          return 1;
        }
        return 0;
      });

      this.loaded++;
      this.checkBeforeLoadForm();
    })
  }

  loadmarket() {
    this.marketmanagementService.loadMarket().subscribe((res: any) => {
      var markets = [
        {
          id: '00000000-0000-0000-0000-000000000000',
          text: '-- Chọn --',
          districtId: '00000000-0000-0000-0000-000000000000',
          communeId: '00000000-0000-0000-0000-000000000000',
        },
      ];
      for (var item of res.items) {
        let obj_market = {
          id: item.marketId,
          text: item.marketName,
          districtId: item.districtId,
          communeId: item.communeId,
        }
        markets.push(obj_market)
      }
      this.marketData = markets
      this.marketDataFilter = markets
      
      this.loaded++;
      this.checkBeforeLoadForm();
    })
  }

  loadbusinessLine() {
    this.marketmanagementService.loadBusinessLine().subscribe((res: any) => {
      var businesslines = [
        {
          id: '00000000-0000-0000-0000-000000000000',
          text: '-- Chọn --',
        },
      ];
      for (var item of res.items) {
        let obj_businessLine = {
          id: item.businessLineId,
          text: item.businessLineName,

        }
        businesslines.push(obj_businessLine)
      }
      this.businessLineData = businesslines

      this.loaded++;
      this.checkBeforeLoadForm();
    })
  }

  loadForm() {
    this.businessLineDetail = this.marketmanagementData.matHang
    if(this.marketmanagementData.marketManagementId != '00000000-0000-0000-0000-000000000000'){
      this.marketmanagementData.id = this.marketmanagementData.marketManagementId
    }
    this.formGroup = this.fb.group({
      DistrictId: [this.marketmanagementData.districtId],
      CommuneId: [this.marketmanagementData.communeId],
      MarketId: [this.marketmanagementData.marketId],
      BoothNumber: [this.marketmanagementData.boothNumber, Validators.required], //Số sạp
      GiaTrongNhaLong: [this.marketmanagementData.giaTrongNhaLong],
      GiaNgoaiNhaLong: [this.marketmanagementData.giaNgoaiNhaLong],
      DeXuatGiaMoi: [this.marketmanagementData.deXuatGiaMoi],
      Note: [this.marketmanagementData.note],
    });
    this.formGroup.controls.BoothNumber.valueChanges.subscribe((x) => {
      this.formGroup.patchValue({
        "BoothNumber": this.f_currency(x)
      }, { emitEvent: false })
    })
    
    this.formGroup.controls.GiaTrongNhaLong.valueChanges.subscribe((x) => {
      this.formGroup.patchValue({
        "GiaTrongNhaLong": this.f_currency(x)
      }, { emitEvent: false })
    })
    
    this.formGroup.controls.GiaNgoaiNhaLong.valueChanges.subscribe((x) => {
      this.formGroup.patchValue({
        "GiaNgoaiNhaLong": this.f_currency(x)
      }, { emitEvent: false })
    })
    
    this.formGroup.controls.DeXuatGiaMoi.valueChanges.subscribe((x) => {
      this.formGroup.patchValue({
        "DeXuatGiaMoi": this.f_currency(x)
      }, { emitEvent: false })
    })
  }

  //change data
  changedistrict(event: any) {
    if (event != '00000000-0000-0000-0000-000000000000') {
      var result_commune = this.communeData.filter(x => x.districtId == event || x.id == '00000000-0000-0000-0000-000000000000')
      var result_market = this.marketData.filter(x => x.districtId == event || x.id == '00000000-0000-0000-0000-000000000000')
      this.communeDataFilter = result_commune
      this.marketDataFilter = result_market
    }
    else {
      this.communeDataFilter = this.communeData
      this.marketDataFilter = this.marketData
    }
    this.formGroup.controls['CommuneId'].setValue('00000000-0000-0000-0000-000000000000')
  }

  changecommune(event: any) {
    if (event != '00000000-0000-0000-0000-000000000000') {
      var result_market = this.marketData.filter(x => x.communeId == event || x.id == '00000000-0000-0000-0000-000000000000')
      this.marketDataFilter = result_market
    }
    else {
      var result_market = this.marketData.filter(x => x.districtId == this.formGroup.controls['DistrictId'].value || x.id == '00000000-0000-0000-0000-000000000000')
      this.marketDataFilter = result_market
    }
    this.formGroup.controls['MarketId'].setValue('00000000-0000-0000-0000-000000000000')
  }

  clearmodel() {
      EMPTY_CUSTOM.marketManagementId = '00000000-0000-0000-0000-000000000000',
      EMPTY_CUSTOM.districtId = '00000000-0000-0000-0000-000000000000',
      EMPTY_CUSTOM.communeId = '00000000-0000-0000-0000-000000000000',
      EMPTY_CUSTOM.marketId = '00000000-0000-0000-0000-000000000000',
      EMPTY_CUSTOM.nganhHangKinhDoanh = '00000000-0000-0000-0000-000000000000',
      EMPTY_CUSTOM.boothNumber = null,
      EMPTY_CUSTOM.giaTrongNhaLong = null,
      EMPTY_CUSTOM.giaNgoaiNhaLong = null,
      EMPTY_CUSTOM.deXuatGiaMoi = null,
      EMPTY_CUSTOM.note = '',
      EMPTY_CUSTOM.matHang = []
      this.marketmanagementData = EMPTY_CUSTOM;
  }

  private prepareMarketManagementData() {
    const formData = this.formGroup.value;
    this.marketmanagementData.districtId = formData.DistrictId;
    this.marketmanagementData.communeId = formData.CommuneId;
    this.marketmanagementData.marketId = formData.MarketId;
    this.marketmanagementData.boothNumber = formData.BoothNumber ? Number(formData.BoothNumber.replaceAll(',' , '')) : null;
    this.marketmanagementData.giaTrongNhaLong = formData.GiaTrongNhaLong ? Number(formData.GiaTrongNhaLong.replaceAll(',' , '')) : null;
    this.marketmanagementData.giaNgoaiNhaLong = formData.GiaNgoaiNhaLong ? Number(formData.GiaNgoaiNhaLong.replaceAll(',' , '')) : null;
    this.marketmanagementData.deXuatGiaMoi = formData.DeXuatGiaMoi ? Number(formData.DeXuatGiaMoi.replaceAll(',' , '')) : null;
    this.marketmanagementData.note = formData.Note;
    this.marketmanagementData.nganhHangKinhDoanh = '00000000-0000-0000-0000-000000000000';
    this.marketmanagementData.matHang = this.businessLineDetail
  }

  save() {
    this.prepareMarketManagementData();
    if (this.marketmanagementData.marketManagementId != '00000000-0000-0000-0000-000000000000') {
      this.edit();
    } else {
      this.create();
    }
  }

  edit() {
    const sbUpdate = this.marketmanagementService.update(this.marketmanagementData).pipe(
      tap(() => {
        this.modal.close();
      }),
      catchError((errorMessage) => {
        this.modal.dismiss(errorMessage);
        return of(this.marketmanagementData);
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
    const sbCreate = this.marketmanagementService.create(this.marketmanagementData).pipe(
      tap(() => {
        this.modal.close();
      }),
      catchError((errorMessage) => {
        this.modal.dismiss(errorMessage);
        return of(this.marketmanagementData);
      }),
    ).subscribe((res: any) => {
      Swal.fire({
        icon: res.status == 1 ? 'success' : 'error',
        title: res.status == 1 ? 'Thêm mới thành công' : 'Thêm mới thất bại',
        confirmButtonText: 'Xác nhận',
        text: 'Thêm mới ' + (res.status == 1 ? 'thành công' : 'thất bại'),
      });
      this.marketmanagementData = EMPTY_CUSTOM
    });
    this.subscriptions.push(sbCreate);
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

  isdefaultvalue(controlName: any) {
    const control = this.formGroup.controls[controlName];
    const value = control.value;
    if (value == '00000000-0000-0000-0000-000000000000') {
      control.setErrors({ defaultvalue: true });
    }
    else {
      control.setErrors({ defaultvalue: null });
      control.updateValueAndValidity();
    }
    return control.invalid && (control.dirty || control.touched);
  }

  prenventInputNonNumber(event: any) {
    if (event.which < 48 || event.which > 57) {
      event.preventDefault();
    }
  }

  check_formGroup() {
    if (this.formGroup.invalid) {
      this.formGroup.markAllAsTouched();
    }
    else {
      this.save();
    }
  }
  
  addBusinessLine(){
    const modalRef = this.modalService.open(AddBusinessLineModalComponent, { size: '100px' });
    modalRef.result.then(({...res}) =>
      res,
      (res) => {
        if (res) {
          this.add_detail(res)
        }
      }
    );
  }
  
  add_detail(data: any){
    if (this.businessLineDetail.findIndex((x: any) => x.businessLineName === data.businessLineName) === -1) {
      let obj_add = {
        businessLineName: data.businessLineName,
        businessLineId: data.businessLineId,
        price: data.price
      }
      this.businessLineDetail.push(obj_add);
    }
  }
  
  delete_detail(data: any){
    this.businessLineDetail = this.businessLineDetail.filter((x: any) => x.businessLineId !== data.businessLineId)
  }
  
  f_currency(value: any, args?: any): any {
    let nbr = Number((value + '').replace(/,|-/g, ""));
    const result = (nbr + '').replace(/(\d)(?=(\d{3})+(?!\d))/g, "$1,");
    return result
  }
}
