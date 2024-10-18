import { selectModel } from './../../../../controls-page/controls-custom-page/controls-custom-page.component';
import { ChangeDetectorRef, Component, Input, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal, NgbDateAdapter, NgbDateParserFormatter } from '@ng-bootstrap/ng-bootstrap';
import { of, Subscription, filter, EMPTY } from 'rxjs';
import { catchError, finalize, first, tap, throwIfEmpty } from 'rxjs/operators';
import Swal from 'sweetalert2';
import { Options } from 'select2';

import { CateInvestmentProjectModel } from '../../../_models/cate-investment-project.model';
import { CateInvestmentProjectPageService } from '../../../_services/cate-investment-project-page.service';
import * as moment from 'moment';

const EMPTY_CUSTOM: CateInvestmentProjectModel = {
  id: '',
  cateInvestmentProjectId : '00000000-0000-0000-0000-000000000000',
  investmentType: null, //Loại đầu tư 1. Trong nước / 2. Ngoài nước
  businessName : '', //Tên doanh nghiệp
  investment : null, //Vốn đăng ký
  numberOfWorker: null, //Số lượng nhân viên
  projectArea: null, //Diện tích dự án
  quantity: null, //Sản lượng (SP / ngày)
  produce: null, //Công suất
  productValue: null,//Giá trị sản phẩm
  reality: '', //Thực trạng
  owner: '',
  phoneNumber: '',
  career: '',
  district: '00000000-0000-0000-0000-000000000000'
};

@Component({
  selector: 'app-edit-modal',
  templateUrl: './edit-modal.component.html',
  styleUrls: ['./edit-modal.component.scss'],

})
export class EditCateInvestmentProjectModalComponent implements OnInit, OnDestroy {
  @Input() id: any;
  @Input() districtData: any;
  isLoading$: any;
  cateinvestmentprojectData: CateInvestmentProjectModel;
  formGroup: FormGroup;
  options: Options;

  private subscriptions: Subscription[] = [];

  constructor(
    private cateinvestmentprojectService: CateInvestmentProjectPageService,
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

      this.loadCateInvestmentProject();
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

  loadCateInvestmentProject() {
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
        this.formGroup.controls.Investment.setValue(this.f_currency(this.formGroup.controls.Investment.value))
        this.formGroup.controls.NumberOfWorker.setValue(this.f_currency(this.formGroup.controls.NumberOfWorker.value))
        this.formGroup.controls.ProjectArea.setValue(this.f_currency(this.formGroup.controls.ProjectArea.value))
        this.formGroup.controls.Quantity.setValue(this.f_currency(this.formGroup.controls.Quantity.value))
        this.formGroup.controls.Produce.setValue(this.f_currency(this.formGroup.controls.Produce.value))
        this.formGroup.controls.ProductValue.setValue(this.f_currency(this.formGroup.controls.ProductValue.value))
        this.formGroup.updateValueAndValidity()
      });
      this.subscriptions.push(sb);
    }
  }

  loadForm() {
    this.formGroup = this.fb.group({
      InvestmentType: [this.cateinvestmentprojectData.investmentType], //Loại đầu tư 1. Trong nước / 2. Ngoài nước
      BusinessName: [this.cateinvestmentprojectData.businessName, Validators.required], //Tên doanh nghiệp
      Investment: [this.cateinvestmentprojectData.investment], //Vốn đăng ký
      NumberOfWorker: [this.cateinvestmentprojectData.numberOfWorker], //Số lượng nhân viên
      ProjectArea: [this.cateinvestmentprojectData.projectArea], //Diện tích dự án
      Quantity: [this.cateinvestmentprojectData.quantity], //Sản lượng (SP / ngày)
      Produce: [this.cateinvestmentprojectData.produce], //Công suất
      ProductValue: [this.cateinvestmentprojectData.productValue], //Giá trị sản phẩm
      Reality: [this.cateinvestmentprojectData.reality], //Thực trạng
      Owner: [this.cateinvestmentprojectData.owner, Validators.required],
      PhoneNumber: [this.cateinvestmentprojectData.phoneNumber],
      Career: [this.cateinvestmentprojectData.career],
      district: this.cateinvestmentprojectData.district
    });
    this.subscriptions.push(
      this.formGroup.controls.Investment.valueChanges.subscribe((x) => {
        this.formGroup.patchValue({
          "Investment": x ? this.f_currency(x) : null
        }, { emitEvent: false })
      })
    );
    this.subscriptions.push(
      this.formGroup.controls.NumberOfWorker.valueChanges.subscribe((x) => {
        this.formGroup.patchValue({
          "NumberOfWorker": x ? this.f_currency(x) : null
        }, { emitEvent: false })
      })
    );
    this.subscriptions.push(
      this.formGroup.controls.ProjectArea.valueChanges.subscribe((x) => {
        this.formGroup.patchValue({
          "ProjectArea": x ? this.f_currency(x) : null
        }, { emitEvent: false })
      })
    );
    this.subscriptions.push(
      this.formGroup.controls.Quantity.valueChanges.subscribe((x) => {
        this.formGroup.patchValue({
          "Quantity": x ? this.f_currency(x) : null
        }, { emitEvent: false })
      })
    );
    this.subscriptions.push(
      this.formGroup.controls.Produce.valueChanges.subscribe((x) => {
        this.formGroup.patchValue({
          "Produce": x ? this.f_currency(x) : null
        }, { emitEvent: false })
      })
    );
    this.subscriptions.push(
      this.formGroup.controls.ProductValue.valueChanges.subscribe((x) => {
        this.formGroup.patchValue({
          "ProductValue": x ? this.f_currency(x) : null
        }, { emitEvent: false })
      })
    );
  }

  clear() {
    EMPTY_CUSTOM.cateInvestmentProjectId = '00000000-0000-0000-0000-000000000000';
    EMPTY_CUSTOM.investmentType = null; //Loại đầu tư 1. Trong nước / 2. Ngoài nước
    EMPTY_CUSTOM.businessName = ''; //Tên doanh nghiệp
    EMPTY_CUSTOM.investment = null; //Vốn đăng ký
    EMPTY_CUSTOM.numberOfWorker = null; //Số lượng nhân viên
    EMPTY_CUSTOM.projectArea = null; //Diện tích dự án
    EMPTY_CUSTOM.quantity = null; //Sản lượng (SP / ngày)
    EMPTY_CUSTOM.produce = null; //Công suất
    EMPTY_CUSTOM.productValue = null; //Giá trị sản phẩm
    EMPTY_CUSTOM.reality = ''; //Thực trạng
    EMPTY_CUSTOM.career = '';
    EMPTY_CUSTOM.phoneNumber = '';
    EMPTY_CUSTOM.owner = '';
    EMPTY_CUSTOM.district = '00000000-0000-0000-0000-000000000000';
    this.cateinvestmentprojectData = EMPTY_CUSTOM;
  }

  private prepareCateInvestmentProject() {
    // this.prepare_listworkers();
    // this.formGroup.updateValueAndValidity();
    const formData = this.formGroup.value;
    this.cateinvestmentprojectData.investmentType = Number(formData.InvestmentType);
    this.cateinvestmentprojectData.businessName = formData.BusinessName;
    this.cateinvestmentprojectData.investment = Number(formData.Investment.replaceAll(',' , ''));
    this.cateinvestmentprojectData.numberOfWorker = Number(formData.NumberOfWorker.replaceAll(',' , ''));
    this.cateinvestmentprojectData.projectArea = Number(formData.ProjectArea.replaceAll(',' , ''));
    this.cateinvestmentprojectData.quantity = Number(formData.Quantity.replaceAll(',' , ''));
    this.cateinvestmentprojectData.produce = Number(formData.Produce.replaceAll(',' , ''));
    this.cateinvestmentprojectData.productValue = Number(formData.ProductValue.replaceAll(',' , ''));
    this.cateinvestmentprojectData.reality = formData.Reality;
    this.cateinvestmentprojectData.owner = formData.Owner;
    this.cateinvestmentprojectData.phoneNumber = formData.PhoneNumber;
    this.cateinvestmentprojectData.career = formData.Career;
    this.cateinvestmentprojectData.district = formData.district;
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
    this.prepareCateInvestmentProject();
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
    if (this.formGroup.invalid || this.formGroup.controls['InvestmentType'].value === 0) {
      this.formGroup.markAllAsTouched();
      this.formGroup.updateValueAndValidity();
    }
    else {
      this.save()
    }
  }
}
