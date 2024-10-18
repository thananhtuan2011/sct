import { ChangeDetectorRef, Component, Input, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal, NgbDateAdapter, NgbDateParserFormatter } from '@ng-bootstrap/ng-bootstrap';
import { of, Subscription } from 'rxjs';
import { catchError, finalize, first, tap } from 'rxjs/operators';
import Swal from 'sweetalert2';

import { SelectOptionData } from 'src/app/_metronic/shared/components/select-custom/select-custom.interface';
import { Options } from 'select2';

import { CommercialManagementModel } from '../../../_models/commercialmanagement.model';
import { CommercialManagementPageService } from '../../../_services/commercialmanagement-page.service';
import { CommonService } from 'src/app/_metronic/shared/services/common.service';

const EMPTY_CUSTOM: CommercialManagementModel = {
  id: '',
  commercialId: '00000000-0000-0000-0000-000000000000',
  type: '',
  code: '',
  name: '',
  typeOfMarketId: '',
  districtId: '',
  communeId: '',
  address: '',
  rankId: '',
  constructiveNatureId: '',
  businessNatureId: '',
  typeOfEconomic: '',
  managementFormId: '',
  managementObjectId: '',
  phoneNumber: '',
  email: '',
  fax: '',
  note: '',
  typeOfMarket: null,
  typeOfCenterLogistic: 0,
  formMarket: null,
  form: null,
  area: null,
  marketCleared: null
};

@Component({
  selector: 'app-edit-logistics-modal',
  templateUrl: './edit-logistics-modal.component.html',
  styleUrls: ['./edit-logistics-modal.component.scss'],
})

export class EditCommercialManagementLogisticsModalComponent implements OnInit, OnDestroy {
  @Input() id: any;
  @Input() typeId: any;
  @Input() code: any;
  @Input() name: any;
  isLoading$: any;
  commercialmanagementData: CommercialManagementModel;
  formGroup: FormGroup;


  private subscriptions: Subscription[] = [];
  public options: Options;

  public type: any;

  public districtData: Array<any>;
  // public district: any;
  public communeData: Array<any>;
  public communeDataFilter: Array<any>;

  public rankData: Array<any>;
  public businessnatureData: Array<any>;
  public typeofeconomicData: Array<any>;
  public typeOfCenterLogisticData: any = [
    {
      id: 0,
      text: '-- Chọn --'
    },
    {
      id: 1,
      text: 'Trung tâm Logistics cấp tính'
    },
    {
      id: 2,
      text: 'Trung tâm Logistic chuyên dùng hàng không'
    }
  ]

  constructor(
    private commercialmanagementService: CommercialManagementPageService,
    private commonService: CommonService,
    private fb: FormBuilder, public modal: NgbActiveModal,
  ) { }

  ngOnInit(): void {
    this.isLoading$ = this.commercialmanagementService.isLoading$;

    (async () => {
      this.loadDistrict();
      this.loadCommune();
      this.loadCategory();
      await this.delay(300);
      this.loadMarket();
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

  loadDistrict() {
    this.commonService.getDistrict().subscribe((res: any) => {
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
    })
  }

  loadCommune() {
    this.commonService.getCommune().subscribe((res: any) => {
      var communes = [
        {
          id: '00000000-0000-0000-0000-000000000000',
          text: '-- Chọn --',
        },
      ];
      for (var item of res.items) {
        let obj_commune = {
          id: item.communeId,
          text: item.communeName,
          districtId: item.districtId,
        };
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
      });;
      this.communeDataFilter = communes.sort((a, b) => {
        if (a < b) {
          return -1;
        }
        if (a > b) {
          return 1;
        }
        return 0;
      });;
    })
  }

  loadCommuneByDistrict(event: any) {
    if (event != '00000000-0000-0000-0000-000000000000') {
      var result = this.communeData.filter(x => x.id == '00000000-0000-0000-0000-000000000000' || x.districtId == event);
      this.communeDataFilter = result;
    }
    else {
      this.communeDataFilter = this.communeData;
    }
    this.formGroup.controls['CommuneId'].setValue('00000000-0000-0000-0000-000000000000')
  }

  loadCategory() {
    this.commercialmanagementService.loadCategory().subscribe((res: any) => {
      var ranks = [
        {
          id: '00000000-0000-0000-0000-000000000000',
          text: '-- Chọn --',
          piority: '0',
        },
      ];
      var businessnatures = [
        {
          id: '00000000-0000-0000-0000-000000000000',
          text: '-- Chọn --',
          piority: '0',
        },
      ];
      var typeofeconomic = [
        {
          id: '00000000-0000-0000-0000-000000000000',
          text: '-- Chọn --',
          piority: '0',
        },
      ];

      for (var item of res.items) {
        if (item.categoryTypeCode == 'LEVEL_MARKET') {
          let obj_rank = {
            id: item.categoryId,
            text: item.categoryName,
            piority: item.piority,
          };
          ranks.push(obj_rank);
        }
        if (item.categoryTypeCode == 'BUSINESS_NATURE_SUPERMARKET') {
          let obj_businessnature = {
            id: item.categoryId,
            text: item.categoryName,
            piority: item.piority,
          };
          businessnatures.push(obj_businessnature);
        }
        if (item.categoryTypeCode == 'TYPE_OF_ECONOMIC_COMMERCIAL') {
          let obj_typeofeconomic = {
            id: item.categoryId,
            text: item.categoryName,
            piority: item.piority,
          };
          typeofeconomic.push(obj_typeofeconomic);
        }
      }
      this.rankData = ranks.sort((i1, i2) => {
        if (i1.piority > i2.piority) {
          return 1;
        }
        if (i1.piority < i2.piority) {
          return -1;
        }
        return 0;
      });
      this.businessnatureData = businessnatures.sort((i1, i2) => {
        if (i1.piority > i2.piority) {
          return 1;
        }
        if (i1.piority < i2.piority) {
          return -1;
        }
        return 0;
      });
      this.typeofeconomicData = typeofeconomic.sort((i1, i2) => {
        if (i1.piority > i2.piority) {
          return 1;
        }
        if (i1.piority < i2.piority) {
          return -1;
        }
        return 0;
      });
    })
  }

  loadMarket() {
    if (!this.id) {
      this.clearmodel();
      this.loadForm();
    } else {
      const sb = this.commercialmanagementService.getItemById(this.id).pipe(
        first(),
        catchError((errorMessage) => {
          this.modal.dismiss(errorMessage);
          return of(EMPTY_CUSTOM);
        })
      ).subscribe((res: any) => {
        this.commercialmanagementData = res.items[0];
        this.loadForm();
      });
      this.subscriptions.push(sb);
    }
  }

  loadForm() {
    this.formGroup = this.fb.group({
      TypeOfMarketId: [this.commercialmanagementData.typeOfMarketId],
      DistrictId: [this.commercialmanagementData.districtId],
      CommuneId: [this.commercialmanagementData.communeId],
      Address: [this.commercialmanagementData.address, Validators.required],
      RankId: [this.commercialmanagementData.rankId],
      ConstructiveNatureId: [this.commercialmanagementData.constructiveNatureId],
      BusinessNatureId: [this.commercialmanagementData.businessNatureId],
      TypeOfEconomic: [this.commercialmanagementData.typeOfEconomic],
      ManagementFormId: [this.commercialmanagementData.managementFormId],
      ManagementObjectId: [this.commercialmanagementData.managementObjectId],
      PhoneNumber: [this.commercialmanagementData.phoneNumber, Validators.required],
      Email: [this.commercialmanagementData.email, Validators.email],
      Fax: [this.commercialmanagementData.fax],
      Note: [this.commercialmanagementData.note],
      TypeOfCenterLogistic: this.commercialmanagementData.typeOfCenterLogistic
    });
  }

  save() {
    this.prepareCommercialManagement();
    if (this.commercialmanagementData.commercialId != '00000000-0000-0000-0000-000000000000') {
      this.edit();
    } else {
      this.create();
    }
  }

  edit() {
    const sbUpdate = this.commercialmanagementService.update(this.commercialmanagementData).pipe(
      tap(() => {
        this.modal.close();
      }),
      catchError((errorMessage) => {
        this.modal.dismiss(errorMessage);
        return of(this.commercialmanagementData);
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
    const sbCreate = this.commercialmanagementService.create(this.commercialmanagementData).pipe(
      tap(() => {
        this.modal.close();
      }),
      catchError((errorMessage) => {
        this.modal.dismiss(errorMessage);
        return of(this.commercialmanagementData);
      }),
    ).subscribe((res: any) => {
      Swal.fire({
        icon: res.status == 1 ? 'success' : 'error',
        title: res.status == 1 ? 'Thêm mới thành công' : 'Thêm mới thất bại',
        confirmButtonText: 'Xác nhận',
        text: 'Thêm mới ' + (res.status == 1 ? 'thành công' : 'thất bại'),
      });
      this.commercialmanagementData = EMPTY_CUSTOM
    });
    this.subscriptions.push(sbCreate);
  }

  clearmodel() {
    EMPTY_CUSTOM.commercialId = '00000000-0000-0000-0000-000000000000';
    EMPTY_CUSTOM.type = '';
    EMPTY_CUSTOM.code = '';
    EMPTY_CUSTOM.name = '';
    EMPTY_CUSTOM.typeOfMarketId = '00000000-0000-0000-0000-000000000000';
    EMPTY_CUSTOM.districtId = '00000000-0000-0000-0000-000000000000';
    EMPTY_CUSTOM.communeId = '00000000-0000-0000-0000-000000000000';
    EMPTY_CUSTOM.address = '';
    EMPTY_CUSTOM.rankId = '00000000-0000-0000-0000-000000000000';
    EMPTY_CUSTOM.constructiveNatureId = '00000000-0000-0000-0000-000000000000';
    EMPTY_CUSTOM.businessNatureId = '00000000-0000-0000-0000-000000000000';
    EMPTY_CUSTOM.typeOfEconomic = '00000000-0000-0000-0000-000000000000';
    EMPTY_CUSTOM.managementFormId = '00000000-0000-0000-0000-000000000000';
    EMPTY_CUSTOM.managementObjectId = '00000000-0000-0000-0000-000000000000';
    EMPTY_CUSTOM.phoneNumber = '';
    EMPTY_CUSTOM.email = '';
    EMPTY_CUSTOM.fax = '';
    EMPTY_CUSTOM.note = '';
    EMPTY_CUSTOM.typeOfCenterLogistic = 0,
    this.commercialmanagementData = EMPTY_CUSTOM;
  }

  private prepareCommercialManagement() {
    const formData = this.formGroup.value;
    this.commercialmanagementData.type = this.typeId;
    this.commercialmanagementData.code = this.code;
    this.commercialmanagementData.name = this.name;
    this.commercialmanagementData.typeOfMarketId = '00000000-0000-0000-0000-000000000000';
    this.commercialmanagementData.districtId = formData.DistrictId;
    this.commercialmanagementData.communeId = formData.CommuneId;
    this.commercialmanagementData.address = formData.Address;
    this.commercialmanagementData.rankId = formData.RankId;
    this.commercialmanagementData.constructiveNatureId = '00000000-0000-0000-0000-000000000000';
    this.commercialmanagementData.businessNatureId = formData.BusinessNatureId;
    this.commercialmanagementData.typeOfEconomic = '00000000-0000-0000-0000-000000000000';
    this.commercialmanagementData.managementFormId = '00000000-0000-0000-0000-000000000000';
    this.commercialmanagementData.managementObjectId = '00000000-0000-0000-0000-000000000000';
    this.commercialmanagementData.phoneNumber = formData.PhoneNumber;
    this.commercialmanagementData.email = formData.Email;
    this.commercialmanagementData.fax = formData.Fax;
    this.commercialmanagementData.note = formData.Note;
    this.commercialmanagementData.typeOfCenterLogistic = formData.TypeOfCenterLogistic;
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

  isDefaultValue(controlName: any): boolean {
    const control = this.formGroup.controls[controlName];
    const value = control.value;
    const isdefaultvalue = (value == "00000000-0000-0000-0000-000000000000") || (value == 0)
    if (isdefaultvalue) {
      control.setErrors(
        {
          default_value: true
        }
      )
    }
    return control.invalid && (control.dirty || control.touched);
  }

  check_formGroup() {
    if (this.formGroup.invalid || this.code == '' || this.name == '') {
      this.formGroup.markAllAsTouched();
      this.formGroup.updateValueAndValidity();
      this.commercialmanagementService.check_form.next('check');
    }
    else {
      this.save()
    }
  }
}