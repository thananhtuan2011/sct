import { ChangeDetectorRef, Component, Input, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal, NgbDateAdapter, NgbDateParserFormatter } from '@ng-bootstrap/ng-bootstrap';
import { of, Subscription } from 'rxjs';
import { catchError, finalize, first, tap } from 'rxjs/operators';
import Swal from 'sweetalert2';

import { ChemicalBusinessManagementModel } from '../../../_models/chemical-business-management.model';
import { ChemicalBusinessManagementPageService } from '../../../_services/chemical-business-management-page.service';
import { Options } from 'select2';
import { CommonService } from 'src/app/_metronic/shared/services/common.service';

const EMPTY_CUSTOM: ChemicalBusinessManagementModel = {
  id: '',
  chemicalBusinessManagementId: '00000000-0000-0000-0000-000000000000',
  businessName: '',
  address: '',
  chemicalStorage: '',
  pnupschcmeasures: 2,
  status: 0,
  represent: '',
  districtId: '00000000-0000-0000-0000-000000000000',
  communeId: '00000000-0000-0000-0000-000000000000'
};

@Component({
  selector: 'app-edit-modal',
  templateUrl: './edit-modal.component.html',
  styleUrls: ['./edit-modal.component.scss'],

})
export class EditChemicalBusinessManagementModalComponent implements OnInit, OnDestroy {
  @Input() id: any;
  isLoading$:any;
  chemicalBusinessManagementData: ChemicalBusinessManagementModel;
  formGroup: FormGroup;
  public options: Options;

  private subscriptions: Subscription[] = [];
  public default_value = "00000000-0000-0000-0000-000000000000"
  districtData: any = [];
  communeData: any = [];
  communeDataByDistrictId : any = [];
  show: boolean = false;
  apiLoaded: number = 0;
  businessData: any[] = [];
  PNUPSCHC: any[] = [
    {
      id: 0,
      text: "-- Chọn --"
    },
    {
      id: 1,
      text: "Có"
    },
    {
      id: 2,
      text: "Không"
    },
  ];
  yearRange: any;
  
  constructor(
    private chemicalBusinessManagementService: ChemicalBusinessManagementPageService,
    private fb: FormBuilder, 
    public modal: NgbActiveModal,
    private commonService: CommonService,
    private changeDetectorRef: ChangeDetectorRef,
    ) { }

  ngOnInit(): void {
    this.isLoading$ = this.chemicalBusinessManagementService.isLoading$;
    this.loadDistrict();
    this.loadCommune();
    this.loadBusiness();
    this.getYearsList();
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

  getYearsList() {
    const currentYear = new Date().getFullYear();
    const yearsList: any = [{ id: 0, text: "-- Chọn --" }];
    for (let i = -10; i <= 10; i++) {
      const year = currentYear + i;
      yearsList.push({ id: year, text: "Kiểm tra năm " + year.toString() });
    }
    this.yearRange = yearsList;
  }

  loadChemicalBusinessManagement() {
    if (this.apiLoaded < 2) {
      this.apiLoaded++;
      return
    }
    if (!this.id) {
      this.clear();
      this.loadForm();
    } else {
      const sb = this.chemicalBusinessManagementService.getItemById(this.id).pipe(
        first(),
        catchError((errorMessage) => {
          this.modal.dismiss(errorMessage);
          return of(EMPTY_CUSTOM);
        })
      ).subscribe((res: any) => {
        this.chemicalBusinessManagementData = res.items[0];
        this.chemicalBusinessManagementData.pnupschcmeasures = res.items[0].pnupschcmeasures == true ? 1 : 2;
        this.loadForm();
      });
      this.subscriptions.push(sb);
    }
  }

  loadForm() {
    let result = this.communeData.filter((x: { id: string; districtId: any; }) => x.id == '00000000-0000-0000-0000-000000000000' || x.districtId == this.chemicalBusinessManagementData.districtId);
    this.communeDataByDistrictId = result;
    this.formGroup = this.fb.group({
      BusinessName: [this.chemicalBusinessManagementData.businessName, Validators.required],
      Address: [this.chemicalBusinessManagementData.address, Validators.required],
      ChemicalStorage: [this.chemicalBusinessManagementData.chemicalStorage, Validators.required],
      PNUPSCHCmeasures: [this.chemicalBusinessManagementData.pnupschcmeasures, Validators.required],
      Status: [this.chemicalBusinessManagementData.status, Validators.required],
      DistrictId: [this.chemicalBusinessManagementData.districtId],
      Represent: [this.chemicalBusinessManagementData.represent, Validators.required],
      CommuneId: [this.chemicalBusinessManagementData.communeId]
    });

    this.subscriptions.push(
      this.formGroup.controls.BusinessName.valueChanges.subscribe(x => {
        if (x !== '00000000-0000-0000-0000-000000000000') {
          let data = this.businessData.find(y => y.id == x);
          if (data) {
            this.formGroup.controls.Represent.setValue(data.represent, { onlySelf: true, emitEvent: false });
            this.formGroup.controls.Address.setValue(data.address, { onlySelf: true, emitEvent: false });
            this.formGroup.controls.DistrictId.setValue(data.district, { onlySelf: true, emitEvent: false });
            this.formGroup.controls.CommuneId.setValue(data.commune, { onlySelf: true, emitEvent: false });
          }
        } else {
          this.formGroup.controls.Represent.reset('');
          this.formGroup.controls.Address.reset('');
          this.formGroup.controls.DistrictId.reset('00000000-0000-0000-0000-000000000000');
          this.formGroup.controls.CommuneId.reset('00000000-0000-0000-0000-000000000000');
        }
      })
    )
    this.subscriptions.push(
      this.formGroup.controls.DistrictId.valueChanges.subscribe(x => {
        this.formGroup.controls.CommuneId.reset('00000000-0000-0000-0000-000000000000');
        this.loadCommuneByDistrict(x);
      })
    )

    this.show = true;
  }

  clear() {
    EMPTY_CUSTOM.chemicalBusinessManagementId = '00000000-0000-0000-0000-000000000000',
    EMPTY_CUSTOM.businessName = '00000000-0000-0000-0000-000000000000',
    EMPTY_CUSTOM.address = '',
    EMPTY_CUSTOM.chemicalStorage = '',
    EMPTY_CUSTOM.pnupschcmeasures = 2,
    EMPTY_CUSTOM.status = 0,
    EMPTY_CUSTOM.represent = '',
    EMPTY_CUSTOM.districtId = '00000000-0000-0000-0000-000000000000',
    EMPTY_CUSTOM.communeId = '00000000-0000-0000-0000-000000000000'
    this.chemicalBusinessManagementData = EMPTY_CUSTOM;
  }

  save() {
    this.prepareChemicalBusinessManagement();
    if (this.id) {
      this.edit();
    } else {
      this.create();
    }
  }

  edit() {
    const sbUpdate = this.chemicalBusinessManagementService.update(this.chemicalBusinessManagementData).pipe(
      tap(() => {
        this.modal.close();
      }),
      catchError((errorMessage) => {
        this.modal.dismiss(errorMessage);
        return of(this.chemicalBusinessManagementData);
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
    const sbCreate = this.chemicalBusinessManagementService.create(this.chemicalBusinessManagementData).pipe(
      tap(() => {
        this.modal.close();
      }),
      catchError((errorMessage) => {
        this.modal.dismiss(errorMessage);
        return of(this.chemicalBusinessManagementData);
      }),
    ).subscribe((res: any) => {
      Swal.fire({
        icon: res.status == 1 ? 'success' : 'error',
        title: res.status == 1 ? 'Thêm mới thành công' : 'Thêm mới thất bại',
        confirmButtonText: 'Xác nhận',
        text: res.status == 0 ? res.error.msg : 'Thêm mới ' + (res.status == 1 ? 'thành công' : 'thất bại'),
      });
      this.chemicalBusinessManagementData = EMPTY_CUSTOM
    });
    this.subscriptions.push(sbCreate);
  }

  private prepareChemicalBusinessManagement() {
    const formData = this.formGroup.value;
    this.chemicalBusinessManagementData.businessName = formData.BusinessName;
    this.chemicalBusinessManagementData.address = formData.Address;
    this.chemicalBusinessManagementData.chemicalStorage = formData.ChemicalStorage;
    this.chemicalBusinessManagementData.pnupschcmeasures = formData.PNUPSCHCmeasures == 2 ? false : true;
    this.chemicalBusinessManagementData.status = formData.Status;
    this.chemicalBusinessManagementData.represent = formData.Represent;
    this.chemicalBusinessManagementData.districtId = formData.DistrictId;
    this.chemicalBusinessManagementData.communeId = formData.CommuneId;
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

  check_formGroup() {
    if (this.formGroup.invalid) {
      this.formGroup.markAllAsTouched();
      this.formGroup.updateValueAndValidity();
    }
    else {
      this.save()
    }
  }
  
  isDefaultValue(controlName: any): boolean {
    const control = this.formGroup.controls[controlName];
    const value = control.value;
    const isdefaultvalue = (value == "00000000-0000-0000-0000-000000000000" || value == 0)
    if (isdefaultvalue) {
      control.setErrors({
        default_value : true
      })
    } 
    return control.invalid && (control.dirty || control.touched);
  }

  loadBusiness() {
    this.commonService.getBusiness().subscribe((res: any) => {
      const data = [
        { id: "00000000-0000-0000-0000-000000000000", text: "-- Chọn --" },
        ...res.items.map((item: any) => ({
          id: item.businessId,
          text: item.businessNameVi,
          businessCode: item.businessCode,
          district: item.districtId,
          commune: item.communeId,
          address: item.diaChiTruSo,
          phoneNumber: item.soDienThoai,
          represent: item.nguoiDaiDien,
        }))
      ]
      this.businessData = data;
      this.loadChemicalBusinessManagement();
    })
  }
  
  loadDistrict(){
    this.commonService.getDistrict().subscribe((res: any) => {
      const data = [
        {
          id: "00000000-0000-0000-0000-000000000000",
          text: '-- Chọn --'
        }
      ]
      for (var item of res.items) {
        let obj = {
          id: item.districtId,
          text: item.districtName,
        }
        data.push(obj)
      }
      this.districtData = data;
      this.loadChemicalBusinessManagement();
    })
  }
  
  loadCommune(){
    this.commonService.getCommune().subscribe((res: any) => {
      const data = [
        {
          id: "00000000-0000-0000-0000-000000000000",
          text: '-- Chọn --',
          districtId: '00000000-0000-0000-0000-000000000000'
        }
      ]
      for (let item of res.items) {
        let obj = {
          id: item.communeId,
          text: item.communeName,
          districtId: item.districtId
        }
        data.push(obj)
      }
      this.communeData = data;
      this.communeDataByDistrictId = data;
      this.loadChemicalBusinessManagement();
      // this.communeDataByDistrictId = [ 
      //   {
      //     id: "00000000-0000-0000-0000-000000000000",
      //     text: '-- Chọn --',
      //     districtId: '00000000-0000-0000-0000-000000000000'
      //   }]
    })
  }
  
  loadCommuneByDistrict(event: any){
    if (event != '00000000-0000-0000-0000-000000000000') {
      let result = this.communeData.filter((x: { id: string; districtId: any; }) => x.id == '00000000-0000-0000-0000-000000000000' || x.districtId == event);
      this.communeDataByDistrictId = result;
    }else{
      this.communeDataByDistrictId = [ 
      {
        id: "00000000-0000-0000-0000-000000000000",
        text: '-- Chọn --',
        districtId: '00000000-0000-0000-0000-000000000000'
      }]
    }
    this.formGroup.controls['CommuneId'].setValue('00000000-0000-0000-0000-000000000000')
  }
}
