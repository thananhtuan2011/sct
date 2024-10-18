import { ChangeDetectorRef, Component, Input, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal, NgbDateAdapter, NgbDateParserFormatter, NgbModal } from '@ng-bootstrap/ng-bootstrap';
import * as moment from 'moment';
import { of, Subscription, filter } from 'rxjs';
import { catchError, finalize, first, tap } from 'rxjs/operators';
import { CommonService } from 'src/app/_metronic/shared/services/common.service';
import Swal from 'sweetalert2';

import { IndustrialPromotionProjectModel } from '../../../_models/industrial-promotion-project.model';
import { IndustrialPromotionProjectService } from '../../../_services/industrial-promotion-project-page.service';
import { AddEnterpriseInProvinceModalComponent } from '../edit-enterprises-in-province-modal/edit-modal.component';
import { AddEnterpriseOutsideProvinceModalComponent } from '../edit-enterprises-outside-province-modal/edit-modal.component';
import { Options } from 'select2';

const EMPTY_CUSTOM: IndustrialPromotionProjectModel = {
  id: '',
  industrialPromotionProjectId: '00000000-0000-0000-0000-000000000000',
  projectName: '',
  startDate: '',
  endDate: '',
  capital: null, //nguồn vốn
  funding: null, //tổng kinh phí
  industrialPromotionFunding: null, //kinh phí khuyến công hỗ trợ
  reciprocalEnterpriseFunding: null, //Kinh phí doanh nghiệp đối ứng
  details: [],
};

@Component({
  selector: 'app-edit-modal',
  templateUrl: './edit-modal.component.html',
  styleUrls: ['./edit-modal.component.scss'],

})
export class EditIndustrialPromotionProjectModalComponent implements OnInit, OnDestroy {
  @Input() id: any;
  @Input() type: any;
  isLoading$: any;
  industrialPromotionProjectData: IndustrialPromotionProjectModel;
  formGroup: FormGroup;
  searchGroup: FormGroup;
  options: Options;
  detailData: any = [];
  capitalData: any = [
    {
      id: 0,
      text: "-- Chọn --"
    },
    {
      id: 1,
      text: "Trung ương"
    },
    {
      id: 2,
      text: "Địa phương"
    },
  ]

  private subscriptions: Subscription[] = [];
  public default_value = "00000000-0000-0000-0000-000000000000"

  constructor(
    private industrialPromotionProjectService: IndustrialPromotionProjectService,
    private fb: FormBuilder, public modal: NgbActiveModal,
    public commonService: CommonService,
    private modalService: NgbModal,
    private changeDetectorRefs: ChangeDetectorRef,
  ) { }

  ngOnInit(): void {
    this.isLoading$ = this.industrialPromotionProjectService.isLoading$;
    (async () => {
      await this.delay(150);
      this.loadIndustrialPromotionProject();
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

  loadIndustrialPromotionProject() {
    if (!this.id) {
      this.clear();
      this.loadForm();
    } else {
      const sb = this.industrialPromotionProjectService.getItemById(this.id).pipe(
        first(),
        catchError((errorMessage) => {
          this.modal.dismiss(errorMessage);
          return of(EMPTY_CUSTOM);
        })
      ).subscribe((res: any) => {
        this.industrialPromotionProjectData = res.data;
        this.detailData = res.data.details;
        this.loadForm();
        this.formGroup.controls.Funding.setValue(this.f_currency(this.formGroup.controls.Funding.value))
        this.formGroup.controls.IndustrialPromotionFunding.setValue(this.f_currency(this.formGroup.controls.IndustrialPromotionFunding.value))
        this.formGroup.controls.ReciprocalEnterpriseFunding.setValue(this.f_currency(this.formGroup.controls.ReciprocalEnterpriseFunding.value))
        if (this.type) {
          this.formGroup.disable();
          this.formGroup.controls['Search'].enable();
          this.formGroup.updateValueAndValidity();
        }
      });
      this.subscriptions.push(sb);
    }
  }

  loadForm() {
    this.formGroup = this.fb.group({
      ProjectName: [this.industrialPromotionProjectData.projectName, Validators.required],
      StartDate : [this.industrialPromotionProjectData.startDate, Validators.required], //thời gian bắt đầu
      EndDate : [this.industrialPromotionProjectData.endDate, Validators.required], //thời gian kết thúc
      Capital: [this.industrialPromotionProjectData.capital], //nguồn vốn
      Funding: [this.industrialPromotionProjectData.funding, Validators.required], //tổng kinh phí
      IndustrialPromotionFunding: [this.industrialPromotionProjectData.industrialPromotionFunding, Validators.required], //kinh phí khuyến công hỗ trợ
      ReciprocalEnterpriseFunding: [this.industrialPromotionProjectData.reciprocalEnterpriseFunding, Validators.required], //Kinh phí doanh nghiệp đối ứng
      Search: ['']
    });
    this.subscriptions.push(
      this.formGroup.controls.Funding.valueChanges.subscribe((x) => {
        this.formGroup.patchValue({
          "Funding": this.f_currency(x)
        }, { emitEvent: false })
      })
    );
    this.subscriptions.push(
      this.formGroup.controls.IndustrialPromotionFunding.valueChanges.subscribe((x) => {
        this.formGroup.patchValue({
          "IndustrialPromotionFunding": this.f_currency(x)
        }, { emitEvent: false })
      })
    );
    this.subscriptions.push(
      this.formGroup.controls.ReciprocalEnterpriseFunding.valueChanges.subscribe((x) => {
        this.formGroup.patchValue({
          "ReciprocalEnterpriseFunding": this.f_currency(x)
        }, { emitEvent: false })
      })
    );
  }

  clear() {
    EMPTY_CUSTOM.industrialPromotionProjectId = '00000000-0000-0000-0000-000000000000',
    EMPTY_CUSTOM.projectName = '',
    EMPTY_CUSTOM.startDate = '', //thời gian thực hiện
    EMPTY_CUSTOM.endDate = '', //thời gian thực hiện
    EMPTY_CUSTOM.capital = null, //nguồn vốn
    EMPTY_CUSTOM.funding = null, //tổng kinh phí
    EMPTY_CUSTOM.industrialPromotionFunding = null, //kinh phí khuyến công hỗ trợ
    EMPTY_CUSTOM.reciprocalEnterpriseFunding = null, //Kinh phí doanh nghiệp đối ứng
    EMPTY_CUSTOM.details = []
    this.industrialPromotionProjectData = EMPTY_CUSTOM;
  }

  private prepareIndustrialPromotionProject() {
    const formData = this.formGroup.value;
    this.industrialPromotionProjectData.projectName = formData.ProjectName;
    this.industrialPromotionProjectData.startDate = formData.StartDate;
    this.industrialPromotionProjectData.endDate = formData.EndDate;
    this.industrialPromotionProjectData.capital = formData.Capital;
    this.industrialPromotionProjectData.funding = Number(formData.Funding.replaceAll(',' , ''));
    this.industrialPromotionProjectData.industrialPromotionFunding = Number(formData.IndustrialPromotionFunding.replaceAll(',' , ''));
    this.industrialPromotionProjectData.reciprocalEnterpriseFunding = Number(formData.ReciprocalEnterpriseFunding.replaceAll(',' , ''));
    this.industrialPromotionProjectData.details = this.detailData;
  }

  convert_date(string_date: string) {
    var result = moment.utc(string_date, "DD/MM/YYYY");
    return result
  }

  convert_date_string(string_date: string | null) {
    if (string_date) {
      var date = string_date.split("T")[0];
      var list = date.split("-"); //["year", "month", "day"]
      var result = list[2] + "/" + list[1] + "/" + list[0]
      return result
    }
    return null
  }

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

  save() {
    this.prepareIndustrialPromotionProject();
    if (this.id) {
      this.edit();
    } else {
      this.create();
    }
  }

  edit() {
    const sbUpdate = this.industrialPromotionProjectService.update(this.industrialPromotionProjectData).pipe(
      tap(() => {
        this.modal.close();
      }),
      catchError((errorMessage) => {
        this.modal.dismiss(errorMessage);
        return of(this.industrialPromotionProjectData);
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
    const sbCreate = this.industrialPromotionProjectService.create(this.industrialPromotionProjectData).pipe(
      tap(() => {
        this.modal.close();
      }),
      catchError((errorMessage) => {
        this.modal.dismiss(errorMessage);
        return of(this.industrialPromotionProjectData);
      }),
    ).subscribe((res: any) => {
      Swal.fire({
        icon: res.status == 1 ? 'success' : 'error',
        title: res.status == 1 ? 'Thêm mới thành công' : 'Thêm mới thất bại',
        confirmButtonText: 'Xác nhận',
        text: 'Thêm mới ' + (res.status == 1 ? 'thành công' : 'thất bại'),
      });
      this.industrialPromotionProjectData = EMPTY_CUSTOM
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

  //Enterprise
  add_enterprise_in_province(id: any) {
    const modalRef = this.modalService.open(AddEnterpriseInProvinceModalComponent, { size: '100px' });
    modalRef.result.then(({...res}) =>
      res,
      (res) => {
        if (res) {
          this.add_detail(res)
        }
      }
    );
  }

  add_enterprise_outside_province(id: any) {
    const modalRef = this.modalService.open(AddEnterpriseOutsideProvinceModalComponent, { size: '100px' });
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
    if (this.detailData.findIndex((x: any) => x.businessNameVi === data.BusinessNameVi) === -1) {
      let obj_add = {
        industrialPromotionProjectId : this.id == '' ? '00000000-0000-0000-0000-000000000000' : this.id,
        businessId : data.BusinessId,
        businessCode : data.BusinessCode,
        businessNameVi : data.BusinessNameVi,
        nganhNghe : data.NganhNghe,
        diaChi : data.DiaChi,
        nguoiDaiDien : data.NguoiDaiDien,
      }
      this.detailData.push(obj_add);
    }
  }

  delete_detail(data: any){
    this.detailData = this.detailData.filter((x: any) => x.businessNameVi !== data.businessNameVi)
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
