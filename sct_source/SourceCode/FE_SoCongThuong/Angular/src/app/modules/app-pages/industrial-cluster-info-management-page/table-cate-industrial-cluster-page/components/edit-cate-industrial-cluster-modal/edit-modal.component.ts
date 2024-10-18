import { selectModel } from './../../../../controls-page/controls-custom-page/controls-custom-page.component';
import { ChangeDetectorRef, Component, Input, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal, NgbDateAdapter, NgbDateParserFormatter } from '@ng-bootstrap/ng-bootstrap';
import { of, Subscription, filter } from 'rxjs';
import { catchError, finalize, first, tap, throwIfEmpty } from 'rxjs/operators';
import Swal from 'sweetalert2';
import { Options } from 'select2';

import { CateIndustrialClusterModel } from '../../../_models/cate-industrial-cluster.model';
import { CateIndustrialClusterPageService } from '../../../_services/cate-industrial-cluster-page.service';
import * as moment from 'moment';

const EMPTY_CUSTOM: CateIndustrialClusterModel = {
  id: '',
  cateIndustrialClustersId : '00000000-0000-0000-0000-000000000000',
  industrialClustersName: '', //Tên cụm công nghiệp
  originalArea: null, //Diện tích thành lập
  establishCode: '', //Quyết định thành lập
  expandedArea: null, //Diện tích mở rộng
  decisionExpandCode: '', //Quyết định mở rộng
  detailedArea: null, //Diện tích QH chi tiết
  approvalDecision: '', //Quyết định phê duyệt QHCT
  industrialArea: null, //Diện tích đất công nghiệp
  rentedArea: null, //Diện tích đất cho thuê
  occupancy: null, //Tỉ lệ lấp đấy
  note: '', //Ghi chú
  district : '00000000-0000-0000-0000-000000000000',
};

@Component({
  selector: 'app-edit-modal',
  templateUrl: './edit-modal.component.html',
  styleUrls: ['./edit-modal.component.scss'],

})
export class EditCateIndustrialClusterModalComponent implements OnInit, OnDestroy {
  @Input() id: any;
  @Input() districtData: any;
  isLoading$: any;
  cateinvestmentprojectData: CateIndustrialClusterModel;
  formGroup: FormGroup;
  options: Options;

  private subscriptions: Subscription[] = [];

  constructor(
    private cateinvestmentprojectService: CateIndustrialClusterPageService,
    private fb: FormBuilder, public modal: NgbActiveModal,
    private changeDetectorRefs: ChangeDetectorRef
  ) { }

  ngOnInit(): void {
    this.isLoading$ = this.cateinvestmentprojectService.isLoading$;
    (async () => {
      // this.loadBusiness();
      // this.loadTypeOfProfession();

      //Đợi load data 150ms
      await this.delay(150);

      this.loadCateIndustrialCluster();
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

  loadCateIndustrialCluster() {
    if (!this.id) {
      this.clear();
      this.loadForm();
    } else {
      const sb = this.cateinvestmentprojectService.getItemById(this.id).pipe(
        first(),
        catchError((errorMessage) => {
          this.modal.dismiss(errorMessage);
          return of(EMPTY_CUSTOM);
        })
      ).subscribe((res: any) => {
        this.cateinvestmentprojectData = res.data;
        this.loadForm();
        this.formGroup.controls.OriginalArea.setValue(this.f_currency(this.formGroup.controls.OriginalArea.value))
        this.formGroup.controls.ExpandedArea.setValue(this.f_currency(this.formGroup.controls.ExpandedArea.value))
        this.formGroup.controls.DetailedArea.setValue(this.f_currency(this.formGroup.controls.DetailedArea.value))
        this.formGroup.controls.IndustrialArea.setValue(this.f_currency(this.formGroup.controls.IndustrialArea.value))
        this.formGroup.controls.RentedArea.setValue(this.f_currency(this.formGroup.controls.RentedArea.value))
        this.formGroup.updateValueAndValidity()
      });
      this.subscriptions.push(sb);
    }
  }

  loadForm() {
    this.formGroup = this.fb.group({
      IndustrialClustersName: [this.cateinvestmentprojectData.industrialClustersName, Validators.required], 
      OriginalArea: [this.cateinvestmentprojectData.originalArea], 
      EstablishCode: [this.cateinvestmentprojectData.establishCode, Validators.required], 
      ExpandedArea: [this.cateinvestmentprojectData.expandedArea], 
      DecisionExpandCode: [this.cateinvestmentprojectData.decisionExpandCode, Validators.required], 
      DetailedArea: [this.cateinvestmentprojectData.detailedArea], 
      ApprovalDecision: [this.cateinvestmentprojectData.approvalDecision, Validators.required], 
      IndustrialArea: [this.cateinvestmentprojectData.industrialArea],
      RentedArea: [this.cateinvestmentprojectData.rentedArea], 
      Occupancy: [this.cateinvestmentprojectData.occupancy], 
      Note: [this.cateinvestmentprojectData.note], 
      district: [this.cateinvestmentprojectData.district]
    });
    this.subscriptions.push(
      this.formGroup.controls.OriginalArea.valueChanges.subscribe((x) => {
        this.formGroup.patchValue({
          "OriginalArea": x ? this.f_currency(x) : null
        }, { emitEvent: false })
      })
    );
    this.subscriptions.push(
      this.formGroup.controls.ExpandedArea.valueChanges.subscribe((x) => {
        this.formGroup.patchValue({
          "ExpandedArea": x ? this.f_currency(x) : null
        }, { emitEvent: false })
      })
    );
    this.subscriptions.push(
      this.formGroup.controls.DetailedArea.valueChanges.subscribe((x) => {
        this.formGroup.patchValue({
          "DetailedArea": x ? this.f_currency(x) : null
        }, { emitEvent: false })
      })
    );
    this.subscriptions.push(
      this.formGroup.controls.IndustrialArea.valueChanges.subscribe((x) => {
        this.formGroup.patchValue({
          "IndustrialArea": x ? this.f_currency(x) : null
        }, { emitEvent: false })
      })
    );
    this.subscriptions.push(
      this.formGroup.controls.RentedArea.valueChanges.subscribe((x) => {
        this.formGroup.patchValue({
          "RentedArea": x ? this.f_currency(x) : null
        }, { emitEvent: false })
      })
    );
  }

  clear() {
    EMPTY_CUSTOM.cateIndustrialClustersId = '00000000-0000-0000-0000-000000000000';
    EMPTY_CUSTOM.industrialClustersName = '';//Tên cụm công nghiệp
    EMPTY_CUSTOM.originalArea = null; //Diện tích thành lập
    EMPTY_CUSTOM.establishCode = ''; //Quyết định thành lập
    EMPTY_CUSTOM.expandedArea = null; //Diện tích mở rộng
    EMPTY_CUSTOM.decisionExpandCode = ''; //Quyết định mở rộng
    EMPTY_CUSTOM.detailedArea = null; //Diện tích QH chi tiết
    EMPTY_CUSTOM.approvalDecision = ''; //Quyết định phê duyệt QHCT
    EMPTY_CUSTOM.industrialArea = null; ///Diện tích đất công nghiệp
    EMPTY_CUSTOM.rentedArea = null; //Diện tích đất cho thuê
    EMPTY_CUSTOM.occupancy = null; //Tỉ lệ lấp đấy
    EMPTY_CUSTOM.note = ''; //Ghi chú
    EMPTY_CUSTOM.district = '00000000-0000-0000-0000-000000000000';
    this.cateinvestmentprojectData = EMPTY_CUSTOM;
  }

  private prepareCateIndustrialCluster() {
    const formData = this.formGroup.value;
    this.cateinvestmentprojectData.industrialClustersName = formData.IndustrialClustersName;
    this.cateinvestmentprojectData.originalArea = Number(formData.OriginalArea.replaceAll(',' , ''));
    this.cateinvestmentprojectData.establishCode = formData.EstablishCode;
    this.cateinvestmentprojectData.expandedArea = Number(formData.ExpandedArea.replaceAll(',' , ''));
    this.cateinvestmentprojectData.decisionExpandCode = formData.DecisionExpandCode;
    this.cateinvestmentprojectData.detailedArea = Number(formData.DetailedArea.replaceAll(',' , ''));
    this.cateinvestmentprojectData.approvalDecision = formData.ApprovalDecision;
    this.cateinvestmentprojectData.industrialArea = Number(formData.IndustrialArea.replaceAll(',' , ''));
    this.cateinvestmentprojectData.rentedArea = Number(formData.RentedArea.replaceAll(',' , ''));
    this.cateinvestmentprojectData.occupancy = formData.Occupancy;
    this.cateinvestmentprojectData.district = formData.district;
    this.cateinvestmentprojectData.note = formData.Note;
  }

  //Date
  convert_date(string_date: string) {
    var result = moment.utc(string_date, "DD/MM/YYYY");
    return result
  }

  convert_date_string(string_date: string) {
    var date = string_date.split("T")[0];
    var list = date.split("-"); //["year", "month", "day"]
    var result = list[2] + "/" + list[1] + "/" + list[0]
    return result
  }

  //Ô số
  f_currency(value: any, args?: any): any {
    let nbr = Number((value + '').replace(/,|-/g, ""));
    const result = (nbr + '').replace(/(\d)(?=(\d{3})+(?!\d))/g, "$1,");
    return result
  }

  prenventInputNonNumber(event: any) {
    if (event.which < 48 || event.which > 57) {
      event.preventDefault();
    }
  }

  //CURD
  save() {
    this.prepareCateIndustrialCluster();
    if (this.id) {
      this.edit();
    } else {
      this.create();
    }
  }

  edit() {
    const sbUpdate = this.cateinvestmentprojectService.update(this.cateinvestmentprojectData).pipe(
      tap(() => {
        this.modal.close();
      }),
      catchError((errorMessage) => {
        this.modal.dismiss(errorMessage);
        return of(this.cateinvestmentprojectData);
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
    const sbCreate = this.cateinvestmentprojectService.create(this.cateinvestmentprojectData).pipe(
      tap(() => {
        this.modal.close();
      }),
      catchError((errorMessage) => {
        this.modal.dismiss(errorMessage);
        return of(this.cateinvestmentprojectData);
      }),
    ).subscribe((res: any) => {
      Swal.fire({
        icon: res.status == 1 ? 'success' : 'error',
        title: res.status == 1 ? 'Thêm mới thành công' : 'Thêm mới thất bại',
        confirmButtonText: 'Xác nhận',
        text: 'Thêm mới ' + (res.status == 1 ? 'thành công' : 'thất bại'),
      });
      this.cateinvestmentprojectData = EMPTY_CUSTOM
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

  isDefaultValue(controlName: any) {
    const control = this.formGroup.controls[controlName];
    const value = control.value;
    if (value == '00000000-0000-0000-0000-000000000000' || value == 0 || value == null) {
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
}
