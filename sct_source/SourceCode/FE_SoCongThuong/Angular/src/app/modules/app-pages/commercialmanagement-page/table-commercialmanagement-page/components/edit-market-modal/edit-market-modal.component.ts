import { CommonService } from 'src/app/_metronic/shared/services/common.service';
import { Component, EventEmitter, Input, OnDestroy, OnInit, Output } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { of, Subscription } from 'rxjs';
import { catchError, first, tap } from 'rxjs/operators';
import Swal from 'sweetalert2';

import { SelectOptionData } from 'src/app/_metronic/shared/components/select-custom/select-custom.interface';
import { Options } from 'select2';

import { CommercialManagementModel } from '../../../_models/commercialmanagement.model';
import { CommercialManagementPageService } from '../../../_services/commercialmanagement-page.service';

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
  typeOfCenterLogistic: null,
  formMarket: null,
  form: null,
  area: null,
  marketCleared: null
};

@Component({
  selector: 'app-edit-market-modal',
  templateUrl: './edit-market-modal.component.html',
  styleUrls: ['./edit-market-modal.component.scss'],
})

export class EditCommercialManagementMarketModalComponent implements OnInit, OnDestroy {
  @Input() id: any;
  @Input() typeId: any;
  @Input() code: any;
  @Input() name: any;

  isLoading$: any;
  commercialmanagementData: CommercialManagementModel;
  formGroup: FormGroup;

  private subscriptions: Subscription[] = [];
  public options: Options;

  // public typeofmarketData: Array<any>;
  // public typeofmarket: any;

  public districtData: Array<any>;
  // public district: any;
  public communeData: Array<any>;
  public communeDataFilter: Array<any>;

  public rankData: Array<any>;
  public typeOfMarketData: any = [
    {
      id: 0,
      text: '-- Chọn --'
    },
    {
      id: 1,
      text: 'Chợ trong quy hoạch'
    },
    {
      id: 2,
      text: 'Chợ khác'
    }
  ]
  public areaData: any = [
    {
      id: 0,
      text: '-- Chọn --'
    },
    {
      id: 1,
      text: 'Thành thị'
    },
    {
      id: 2,
      text: 'Nông thôn'
    }
  ]
  
  public formMarketData: any = [
    {
      id: 0,
      text: 'Không có'
    },
    {
      id: 1,
      text: 'Chợ được chuyển đổi mô hình quản lý'
    },
    {
      id: 2,
      text: 'Chợ xây mới'
    },
    {
      id: 3,
      text: 'Chợ nâng cấp, cải tạo'
    }
  ]
  
  public marketClearedData: any = [
    {
      id: 0,
      text: 'Không có'
    },
    {
      id: 1,
      text: 'Chợ đã giải toả, di dời'
    },
    {
      id: 2,
      text: 'Chợ có kế hoạch giải toả, di dời trong thời gian tới'
    }
  ]
  
  public formDataMarket: any = [
    {
      id: 0,
      text: '-- Chọn --'
    },
    {
      id: 1,
      text: 'Chợ ngoài quy hoạch'
    },
    {
      id: 2,
      text: 'Chợ đệm'
    },
    {
      id: 3,
      text: 'Chợ nổi'
    }
  ]
  public constructivenatureData: Array<any>;
  public businessnatureData: Array<any>;
  // public typewholesalemarketData: Array<any>;
  public managementformData: Array<any>;
  public managementobjectData: Array<any>;
  public managementobjectDataFilter: Array<any>;
  public typeOfMarketId: any = 0;
  
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
      // this.loadTypeOfMarket();
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

  // loadTypeOfMarket() {
  //   this.commercialmanagementService.loadTypeOfMarket().subscribe((res: any) => {
  //     var typeofmarkets = [
  //       {
  //         id: '00000000-0000-0000-0000-000000000000',
  //         text: '-- Chọn --',
  //       },
  //     ];
  //     for (var item of res.items) {
  //       let obj_typeofmarket = {
  //         id: item.typeOfMarketId,
  //         text: item.typeOfMarketName
  //       }
  //       typeofmarkets.push(obj_typeofmarket)
  //     }
  //     this.typeofmarketData = typeofmarkets
  //   })
  //   return this.typeofmarketData
  // }

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
      var constructivenatures = [
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
      // var typewholesalemarkets = [
      //   {
      //     id: '00000000-0000-0000-0000-000000000000',
      //     text: '-- Chọn --',
      //     piority: '0',
      //   },
      // ];
      var managementforms = [
        {
          id: '00000000-0000-0000-0000-000000000000',
          text: '-- Chọn --',
          piority: '0',
        },
      ];
      var managementobjects = [
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

        if (item.categoryTypeCode == 'CONSTRUCTIVE_NATURE') {
          let obj_constructivenature = {
            id: item.categoryId,
            text: item.categoryName,
            piority: item.piority,
          };
          constructivenatures.push(obj_constructivenature);
        }

        if (item.categoryTypeCode == 'BUSINESS_NATURE_MARKET') {
          let obj_businessnature = {
            id: item.categoryId,
            text: item.categoryName,
            piority: item.piority,
          };
          businessnatures.push(obj_businessnature);
        }

        // if (item.categoryTypeCode == 'TYPE_WHOLESALE_MARKET') {
        //   let obj_typewholesalemarket = {
        //     id: item.categoryId,
        //     text: item.categoryName,
        //     piority: item.piority,
        //   };
        //   typewholesalemarkets.push(obj_typewholesalemarket);
        // }

        if (item.categoryTypeCode == 'MANAGEMENT_FORM') {
          let obj_managementform = {
            id: item.categoryId,
            text: item.categoryName,
            piority: item.piority,
          };
          managementforms.push(obj_managementform);
        }

        if (item.categoryTypeCode == 'MANAGEMENT_OBJECT') {
          let obj_managementobject = {
            id: item.categoryId,
            text: item.categoryName,
            piority: item.piority,
          };
          managementobjects.push(obj_managementobject);
        }
      }

      //Hạng
      this.rankData = ranks.sort((i1, i2) => {
        if (i1.piority > i2.piority) {
          return 1;
        }
        if (i1.piority < i2.piority) {
          return -1;
        }
        return 0;
      });

      //Tính chất xây dựng
      this.constructivenatureData = constructivenatures.sort((i1, i2) => {
        if (i1.piority > i2.piority) {
          return 1;
        }
        if (i1.piority < i2.piority) {
          return -1;
        }
        return 0;
      });

      //Tính chất kinh doanh
      this.businessnatureData = businessnatures.sort((i1, i2) => {
        if (i1.piority > i2.piority) {
          return 1;
        }
        if (i1.piority < i2.piority) {
          return -1;
        }
        return 0;
      });

      //Loại chợ đầu mối - bỏ
      // this.typewholesalemarketData = typewholesalemarkets;

      //Hình thức quản lý
      this.managementformData = managementforms.sort((i1, i2) => {
        if (i1.piority > i2.piority) {
          return 1;
        }
        if (i1.piority < i2.piority) {
          return -1;
        }
        return 0;
      });

      //Đối tượng quản lý
      this.managementobjectData = managementobjects.sort((i1, i2) => {
        if (i1.piority > i2.piority) {
          return 1;
        }
        if (i1.piority < i2.piority) {
          return -1;
        }
        return 0;
      });

      this.managementobjectDataFilter = this.managementobjectData
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
        // this.typeofmarket = this.typeofmarketData.find(x => x.id == res.items[0].typeOfMarketId)?.text
        this.loadForm();
      });
      this.subscriptions.push(sb);
    }
  }

  loadForm() {
    this.typeOfMarketId = this.commercialmanagementData.typeOfMarket;
    this.formGroup = this.fb.group({
      TypeOfMarketId: [this.commercialmanagementData.typeOfMarketId],
      DistrictId: [this.commercialmanagementData.districtId],
      CommuneId: [this.commercialmanagementData.communeId],
      Address: [this.commercialmanagementData.address, Validators.required],
      RankId: [this.commercialmanagementData.rankId],
      ConstructiveNatureId: [this.commercialmanagementData.constructiveNatureId],
      BusinessNatureId: [this.commercialmanagementData.businessNatureId],
      // TypeOfEconomicId: [this.commercialmanagementData.typeOfEconomicId],
      ManagementFormId: [this.commercialmanagementData.managementFormId],
      ManagementObjectId: [this.commercialmanagementData.managementObjectId],
      PhoneNumber: [this.commercialmanagementData.phoneNumber, Validators.required],
      Email: [this.commercialmanagementData.email, Validators.email],
      Fax: [this.commercialmanagementData.fax],
      Note: [this.commercialmanagementData.note],
      TypeOfMarket : this.commercialmanagementData.typeOfMarket,
      Area: this.commercialmanagementData.area,
      FormMarket: this.commercialmanagementData.formMarket,
      MarketCleared: this.commercialmanagementData.marketCleared,
      Form: this.commercialmanagementData.form
    });
    this.subscriptions.push(
      this.formGroup.controls.ManagementFormId.valueChanges.subscribe((data: any) => {
        if (this.managementformData.find(x => x.id == data).piority == 1) {
          this.managementobjectDataFilter = this.managementobjectData.filter(x => x.piority <= 3 || x.id == '00000000-0000-0000-0000-000000000000');
        }
        else if (this.managementformData.find(x => x.id == data).piority == 2) {
          this.managementobjectDataFilter = this.managementobjectData.filter(x => x.piority > 3 || x.id == '00000000-0000-0000-0000-000000000000');
        }
        else {
          this.managementobjectDataFilter = this.managementobjectData;
        }
      })
    );
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
    EMPTY_CUSTOM.typeOfMarket = 0,
    EMPTY_CUSTOM.formMarket = 0,
    EMPTY_CUSTOM.form = 0,
    EMPTY_CUSTOM.area = 0,
    EMPTY_CUSTOM.marketCleared = 0,
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
    this.commercialmanagementData.constructiveNatureId = formData.ConstructiveNatureId;
    this.commercialmanagementData.businessNatureId = formData.BusinessNatureId;
    this.commercialmanagementData.typeOfEconomic = '00000000-0000-0000-0000-000000000000',
    this.commercialmanagementData.managementFormId = formData.ManagementFormId;
    this.commercialmanagementData.managementObjectId = formData.ManagementObjectId;
    this.commercialmanagementData.phoneNumber = formData.PhoneNumber;
    this.commercialmanagementData.email = formData.Email;
    this.commercialmanagementData.fax = formData.Fax;
    this.commercialmanagementData.note = formData.Note;
    this.commercialmanagementData.typeOfMarket = formData.TypeOfMarket;
    this.commercialmanagementData.formMarket = formData.FormMarket,
    this.commercialmanagementData.form = formData.Form,
    this.commercialmanagementData.area = formData.Area,
    this.commercialmanagementData.marketCleared = formData.MarketCleared
  }

  // gettypeofmarket(event: any) {
  //   this.typeofmarket = this.typeofmarketData.find(x => x.id == event)?.text
  //   if (this.typeofmarket == 'Chợ ngoài quy hoạch') {
  //     this.formGroup.controls['RankId'].setValue('00000000-0000-0000-0000-000000000000');
  //     this.formGroup.controls['ConstructiveNatureId'].setValue('00000000-0000-0000-0000-000000000000');
  //     this.formGroup.controls['BusinessNatureId'].setValue('00000000-0000-0000-0000-000000000000');
  //     this.formGroup.controls['TypeWholesaleMarketId'].setValue('00000000-0000-0000-0000-000000000000');
  //     this.formGroup.controls['ManagementFormId'].setValue('00000000-0000-0000-0000-000000000000');
  //     this.formGroup.controls['ManagementObjectId'].setValue('00000000-0000-0000-0000-000000000000');
  //   }
  //   return this.typeofmarket
  // }

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
  
  getTypeOfMarket(event: any){
    this.typeOfMarketId = this.typeOfMarketData.find((x: { id: any; }) => x.id == event)?.id
    //this.typeid = this.typeData.find(x => x.id == event)?.id
    return this.typeOfMarketId;
  }
}