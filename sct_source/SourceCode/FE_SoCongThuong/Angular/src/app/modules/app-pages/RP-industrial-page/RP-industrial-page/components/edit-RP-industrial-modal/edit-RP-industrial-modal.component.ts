import {
  ChangeDetectorRef,
  Component,
  Input,
  OnDestroy,
  OnInit,
} from '@angular/core';
import {
  FormBuilder,
  FormControl,
  FormGroup,
  Validators,
} from '@angular/forms';
import {
  NgbActiveModal,
  NgbDateAdapter,
  NgbDateParserFormatter,
} from '@ng-bootstrap/ng-bootstrap';
import { of, Subscription } from 'rxjs';
import { catchError, finalize, first, tap } from 'rxjs/operators';
import { SelectOptionData } from 'src/app/_metronic/shared/components/select-custom/select-custom.interface';
import Swal from 'sweetalert2';
import { Options } from 'select2';
import { CommonService } from 'src/app/_metronic/shared/services/common.service';
import { RPIndustrialPageService } from '../../../_services/RP-industrial-page.service';
import { RPIndustrialModel } from '../../../_models/RP-industrial.model';
import * as moment from 'moment';

const EMPTY_CUSTOM: RPIndustrialModel = {
  id: '',
  ReportOperationalStatusOfConstructionInvestmentProjectsId: '',
  criteria: '00000000-0000-0000-0000-000000000000',
  typeReport: '00000000-0000-0000-0000-000000000000',
  units: '',
  note: '',
  quantity: null,
  year: moment().year(),
  reportingPeriod: '00000000-0000-0000-0000-000000000000',
  groupId: '00000000-0000-0000-0000-000000000000',
  district: '00000000-0000-0000-0000-000000000000',
};
@Component({
  selector: 'app-edit-rp-industrial-modal.component',
  templateUrl: './edit-RP-industrial-modal.component.html',
  styleUrls: ['./edit-RP-industrial-modal.component.scss'],
})
export class EditRPIndustrialModalComponent implements OnInit, OnDestroy {
  @Input() id: any;
  @Input() groupType: any;
  @Input() districtData: any;
  isLoading$: any;
  yearData: any = [];
  rPIndustrialData: RPIndustrialModel;
  formGroup: FormGroup;
  public options: Options;
  dataSource: any[] = [];
  lstStore: any[] = [];
  listTarget: any = [
    {
      id: '00000000-0000-0000-0000-000000000000',
      text: '-- Chọn --',
    },
  ];
  displayedColumns: string[] = ['stt', 'name', 'action'];
  public datKyBaoCao: Array<SelectOptionData>;
  public dataType: Array<SelectOptionData>;

  private subscriptions: Subscription[] = [];
  public default_value = '00000000-0000-0000-0000-000000000000';
  public CriteriaData: any = [];

  constructor(
    private rPIndustrialPageService: RPIndustrialPageService,
    private fb: FormBuilder,
    public modal: NgbActiveModal,
    private changeDetectorRefs: ChangeDetectorRef,
    private commonService: CommonService
  ) {}

  ngOnInit(): void {
    this.isLoading$ = this.rPIndustrialPageService.isLoading$;
    (async () => {
      this.loadKyBaoCao();
      this.loadYear();
      this.loadCi();
      this.loadType();
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
    return jQuery(
      '<span class="form-select form-select-solid form-select-lg">' +
        state.text +
        '</span>'
    );
  };

  delay(ms: number) {
    return new Promise((resolve) => setTimeout(resolve, ms));
  }

  loadDetail() {
    if (!this.id) {
      this.clear_model();
      this.loadForm();
    } else {
      const sb = this.rPIndustrialPageService
        .getItemById(this.id)
        .pipe(
          first(),
          catchError((errorMessage) => {
            this.modal.dismiss(errorMessage);
            return of(EMPTY_CUSTOM);
          })
        )
        .subscribe((res: any) => {
          this.rPIndustrialData = res.data;
          this.rPIndustrialData.quantity = this.f_currency(res.data.quantity);
          this.loadForm();
        });
      this.subscriptions.push(sb);
    }
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

  loadForm() {
    this.formGroup = this.fb.group({
      criteria: [
        this.rPIndustrialData.criteria,
        Validators.compose([Validators.required]),
      ],
      units: [
        this.rPIndustrialData.units,
        Validators.compose([Validators.required]),
      ],
      quantity: [
        this.rPIndustrialData.quantity,
        Validators.compose([Validators.required]),
      ],
      typeReport: ['1'],
      reportingPeriod: [this.rPIndustrialData.reportingPeriod],
      note: [this.rPIndustrialData.note],
      year: [this.rPIndustrialData.year],
      groupId: [this.rPIndustrialData.groupId],
      district: [this.rPIndustrialData.district],
    });
    this.loadGroup();
    this.loadUnit();

    this.subscriptions.push(
      this.formGroup.controls.groupId.valueChanges.subscribe((x) => {
        this.loadTargetByParentId();
      })
    );

    this.subscriptions.push(
      this.formGroup.controls.criteria.valueChanges.subscribe((x) => {
        this.loadUnit();
      })
    );

    this.formGroup.controls.quantity.valueChanges.subscribe((x) => {
      this.formGroup.patchValue(
        {
          quantity: this.f_currency(x),
        },
        { emitEvent: false }
      );
    });
  }

  loadGroup() {
    const find_data = this.groupType.find(
      (x: any) => x.id == this.rPIndustrialData.groupId
    );
    if (find_data && find_data.id !== '00000000-0000-0000-0000-000000000000') {
      this.formGroup.patchValue(
        {
          groupId: find_data.id,
        },
        { emitEvent: false }
      );
    } else {
      this.formGroup.patchValue(
        {
          groupId: '00000000-0000-0000-0000-000000000000',
        },
        { emitEvent: false }
      );
    }
  }

  loadUnit() {
    const find_data = this.CriteriaData.find(
      (x: any) => x.id == this.formGroup.controls.criteria.value
    );
    if (find_data && find_data.id !== '00000000-0000-0000-0000-000000000000') {
      this.formGroup.patchValue(
        {
          units: find_data.unit,
        },
        { emitEvent: false }
      );
    } else {
      this.formGroup.patchValue(
        {
          unit: '',
        },
        { emitEvent: false }
      );
    }
  }

  loadTargetByParentId() {
    this.listTarget = this.CriteriaData.filter(
      (item: any) =>
        item.groupId == this.formGroup.controls.groupId.value ||
        item.groupId == '00000000-0000-0000-0000-000000000000'
    );
    this.formGroup.patchValue(
      {
        criteria: '00000000-0000-0000-0000-000000000000',
      },
      { emitEvent: false }
    );
  }

  clear_model() {
    EMPTY_CUSTOM.ReportOperationalStatusOfConstructionInvestmentProjectsId = '';
    EMPTY_CUSTOM.criteria = '00000000-0000-0000-0000-000000000000';
    EMPTY_CUSTOM.units = '';
    EMPTY_CUSTOM.note = '';
    EMPTY_CUSTOM.quantity = null;
    EMPTY_CUSTOM.year = moment().year();
    EMPTY_CUSTOM.reportingPeriod = '00000000-0000-0000-0000-000000000000';
    EMPTY_CUSTOM.groupId = '00000000-0000-0000-0000-000000000000';
    EMPTY_CUSTOM.district = '00000000-0000-0000-0000-000000000000';
    this.rPIndustrialData = EMPTY_CUSTOM;
  }

  save() {
    this.prepareTypeOfEnergy();
    if (
      this.rPIndustrialData
        .ReportOperationalStatusOfConstructionInvestmentProjectsId != ''
    ) {
      this.edit();
    } else {
      this.rPIndustrialData.ReportOperationalStatusOfConstructionInvestmentProjectsId =
        this.default_value;
      this.create();
    }
  }

  edit() {
    const sbUpdate = this.rPIndustrialPageService
      .update(this.rPIndustrialData)
      .pipe(
        tap(() => {
          this.modal.close();
        }),
        catchError((errorMessage) => {
          this.modal.dismiss(errorMessage);
          return of(this.rPIndustrialData);
        })
      )
      .subscribe((res: any) => {
        Swal.fire({
          icon: res.status == 1 ? 'success' : 'error',
          title:
            res.status == 1 ? 'Chỉnh sửa thành công' : 'Chỉnh sửa thất bại',
          confirmButtonText: 'Xác nhận',
          text: 'Chỉnh sửa ' + (res.status == 1 ? 'thành công' : 'thất bại'),
        });
      });
    this.subscriptions.push(sbUpdate);
  }

  create() {
    const sbCreate = this.rPIndustrialPageService
      .create(this.rPIndustrialData)
      .pipe(
        tap(() => {
          this.modal.close();
        }),
        catchError((errorMessage) => {
          this.modal.dismiss(errorMessage);
          return of(this.rPIndustrialData);
        })
      )
      .subscribe((res: any) => {
        Swal.fire({
          icon: res.status == 1 ? 'success' : 'error',
          title: res.status == 1 ? 'Thêm mới thành công' : 'Thêm mới thất bại',
          confirmButtonText: 'Xác nhận',
          text: 'Thêm mới ' + (res.status == 1 ? 'thành công' : 'thất bại'),
        });
        this.rPIndustrialData = EMPTY_CUSTOM;
      });
    this.subscriptions.push(sbCreate);
    EMPTY_CUSTOM.ReportOperationalStatusOfConstructionInvestmentProjectsId = '';
    this.rPIndustrialData = EMPTY_CUSTOM;
  }

  private prepareTypeOfEnergy() {
    const formData = this.formGroup.value;
    this.rPIndustrialData.criteria = formData.criteria;
    this.rPIndustrialData.typeReport = '1';
    this.rPIndustrialData.reportingPeriod = formData.reportingPeriod;
    this.rPIndustrialData.quantity = Number(
      formData.quantity.replaceAll(',', '')
    );
    this.rPIndustrialData.units = formData.units;
    this.rPIndustrialData.note = formData.note;
    this.rPIndustrialData.year = formData.year;
    this.rPIndustrialData.groupId = formData.groupId;
    this.rPIndustrialData.district = formData.district;
  }

  loadType() {
    const data = [
      {
        id: '00000000-0000-0000-0000-000000000000',
        text: '-- Chọn --',
      },
    ];
    let obj1 = {
      id: '1',
      text: 'Thành lập, đầu tư xây dựng hạ tầng kỹ thuật CCN',
    };
    let obj2 = {
      id: '2',
      text: 'Phương án phát triển cụm công nghiệp',
    };
    let obj3 = {
      id: '3',
      text: 'Hoạt động của các cụm công nghiệp',
    };
    data.push(obj1);
    data.push(obj2);
    data.push(obj3);
    this.dataType = data;
    return this.dataType;
  }
  loadCi() {
    this.commonService.getListTargetCCN3().subscribe((res: any) => {
      const data_typeofitem = [
        {
          id: '00000000-0000-0000-0000-000000000000',
          text: '-- Chọn --',
          unit: '',
          groupId: '00000000-0000-0000-0000-000000000000',
        },
        ...res.items.map((item: any) => ({
          id: item.industrialManagementTargetId,
          text: item.name,
          unit: item.unit,
          groupId: item.groupTargetId,
        })),
      ];
      this.CriteriaData = data_typeofitem;
      this.listTarget = this.CriteriaData;
    });
    return this.CriteriaData;
  }

  addStore() {
    var store = '';
    store = this.formGroup.value.Store;
    if (store == '') {
      return;
    }
    this.lstStore.push(store);
    this.dataSource = this.lstStore;
    this.formGroup.controls.Store.setValue('');
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
    const isdefaultvalue =
      control.value == '00000000-0000-0000-0000-000000000000' || control.value == null;
    if (isdefaultvalue) {
      control.setErrors({ default: true });
    }
    return control.invalid && (control.dirty || control.touched);
  }

  ngOnDestroy(): void {
    this.subscriptions.forEach((sb) => sb.unsubscribe());
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
    } else {
      this.save();
    }
  }

  loadKyBaoCao() {
    const data = [
      {
        id: '00000000-0000-0000-0000-000000000000',
        text: '-- Chọn --',
      },
    ];
    let obj1 = {
      id: '1',
      text: '6 tháng đầu năm',
    };
    let obj2 = {
      id: '2',
      text: 'Cả năm',
    };
    data.push(obj1);
    data.push(obj2);
    this.datKyBaoCao = data;
    return this.datKyBaoCao;
  }

  loadYear() {
    const data = [
      {
        id: 0,
        text: '-- Chọn --',
      },
    ];
    for (let i = 0; i < 30; i++) {
      let obj = {
        id: moment().year() - 15 + i,
        text: (moment().year() - 15 + i).toString(),
      };
      data.push(obj);
    }
    this.yearData = data;
  }
}
