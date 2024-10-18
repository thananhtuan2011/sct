import { ChangeDetectorRef, Component, Input, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal, NgbDateAdapter, NgbDateParserFormatter, NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { BehaviorSubject, Observable, of, Subscription } from 'rxjs';
import { catchError, finalize, first, tap } from 'rxjs/operators';
import { SelectOptionData } from 'src/app/_metronic/shared/components/select-custom/select-custom.interface';
import Swal from 'sweetalert2';
import { Options } from 'select2';
import { CateRetailModel } from '../../../_models/cate-retail.model';
import { CateRetailPageService } from '../../../_services/cate-retail.service';
import { EditCateRetailDetail1ModalComponent } from '../edit-cate-retail-detail1/edit-cate-retail-detail1-modal.component';
import * as moment from 'moment';
import { UserModel } from 'src/app/modules/auth/models/user.model';
import { AuthService } from 'src/app/modules/auth/services/auth.service';
export type UserType = UserModel | undefined;

const EMPTY_CUSTOM: CateRetailModel = {
  id: '',
  cateRetailId: '',
  cateRetailCode: '',
  confirmUserId: '00000000-0000-0000-0000-000000000000',
  checkUserId: '00000000-0000-0000-0000-000000000000',
  createUserId: '00000000-0000-0000-0000-000000000000',
  confirmTime: null,
  createTime: null,
  reportMonth: '',
  details: [{
    criteria: '00000000-0000-0000-0000-000000000000',
    cumulativeToReportingMonth: 0,
    estimateReportingMonth: 0,
    performLastmonth: 0,
    performReporting: 0,
    type: 0,
  }],
};

@Component({
  selector: 'app-edit-cate-retail-modal.component',
  templateUrl: './edit-cate-retail-modal.component.html',
  styleUrls: ['./edit-cate-retail-modal.component.scss'],
})

export class EditCateRetailModalComponent implements OnInit, OnDestroy {
  @Input() id: any;
  isLoading$: any;
  cateretailData: CateRetailModel;
  formGroup: FormGroup;
  options: Options;
  dataSourceTheoNam: any[] = [];
  lstbaocaonam: any[] = [];
  lstbaocaonamtruoc: any[] = [];
  displayedColumns: string[] = ['stt', 'name', 'action'];
  totaldetail1: number = 0;
  totaldetail2: number = 0;
  totaldetail3: number = 0;
  show: boolean = false;

  private subscriptions: Subscription[] = [];
  public dataUser: Array<SelectOptionData>;

  constructor(
    private pageService: CateRetailPageService,
    private authService: AuthService,
    private fb: FormBuilder, 
    public modal: NgbActiveModal,
    private changeDetectorRefs: ChangeDetectorRef,
    private modalService: NgbModal
  ) { }

  ngOnInit(): void {
    this.isLoading$ = this.pageService.isLoading$;
    this.loadUser();
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

  loadDetail() {
    if (!this.id) {
      this.clear_model();
      this.loadForm();

      const date = new Date();
      const dateString = moment(date).format("DD/MM/YYYY");
      this.formGroup.controls.NgayTao.setValue(dateString);

      this.formGroup.controls.CreateName.setValue(this.authService.getDataUser().userid);
      this.formGroup.updateValueAndValidity();
    } else {
      const sb = this.pageService.getItemById(this.id).pipe(
        first(),
        catchError((errorMessage) => {
          this.modal.dismiss(errorMessage);
          return of(EMPTY_CUSTOM);
        })
      ).subscribe((res: any) => {
        this.cateretailData = res.data;
        this.cateretailData.confirmTime = this.convert_date_string(res.data.confirmTime);
        this.cateretailData.createTime = this.convert_date_string(res.data.createTime);
        this.loadForm();
        res.data.details.forEach((x: any) => {
          if (x.type == 1) {
            this.lstbaocaonam.push(x);
            this.dataSourceTheoNam = this.lstbaocaonam;
            this.totaldetail1 += +x.performLastmonth;
            this.totaldetail2 += +x.estimateReportingMonth;
            this.totaldetail3 += +x.cumulativeToReportingMonth;
          }
        });
      });
      this.subscriptions.push(sb);
    }
  }

  loadForm() {
    this.formGroup = this.fb.group({
      CateRetailCode: [this.cateretailData.cateRetailCode, Validators.compose([Validators.required])],
      CheckName: [this.cateretailData.checkUserId],
      ConfirmName: [this.cateretailData.confirmUserId],
      CreateName: [this.cateretailData.createUserId],
      NgayDuyet: [this.cateretailData.confirmTime, Validators.required],
      NgayTao: [this.cateretailData.createTime],
      ReportMonth: [this.cateretailData.reportMonth, Validators.required],
    });

    this.show = true;
  }

  clear_model() {
    EMPTY_CUSTOM.cateRetailId = '00000000-0000-0000-0000-000000000000',
    EMPTY_CUSTOM.confirmUserId = '00000000-0000-0000-0000-000000000000',
    EMPTY_CUSTOM.checkUserId = '00000000-0000-0000-0000-000000000000',
    EMPTY_CUSTOM.confirmTime = '',
    EMPTY_CUSTOM.cateRetailCode = '',
    EMPTY_CUSTOM.reportMonth = '',
    this.cateretailData = EMPTY_CUSTOM
  }

  save() {
    this.prepare();
    if (this.id) {
      this.edit();
    } else {
      this.create();
    }
  }

  edit() {
    const sbUpdate = this.pageService.update(this.cateretailData).pipe(
      tap(() => {
        this.modal.close();
      }),
      catchError((errorMessage) => {
        this.modal.dismiss(errorMessage);
        return of(this.cateretailData);
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
    const sbCreate = this.pageService.create(this.cateretailData).pipe(
      tap(() => {
        this.modal.close();
      }),
      catchError((errorMessage) => {
        this.modal.dismiss(errorMessage);
        return of(this.cateretailData);
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

  createCate() {
    const modalRef = this.modalService.open(EditCateRetailDetail1ModalComponent, { size: 'lg' });
    modalRef.result.then(({ ...res }) =>
      this.afterClose(res),
      () => { }
    );
  }

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

  private prepare() {
    const formData = this.formGroup.value;
    this.cateretailData.cateRetailCode = formData.CateRetailCode;
    this.cateretailData.confirmUserId = formData.ConfirmName;
    this.cateretailData.checkUserId = formData.CheckName;
    this.cateretailData.createUserId = formData.CreateName;
    this.cateretailData.confirmTime = this.convert_date(formData.NgayDuyet);
    this.cateretailData.createTime = this.convert_date(formData.NgayTao);
    this.cateretailData.reportMonth = formData.ReportMonth;

    let detail: {
      cateRetailDetailId: '00000000-0000-0000-0000-000000000000',
      criteria: string;
      criterianame: string;
      cumulativeToReportingMonth: number;
      estimateReportingMonth: number;
      performLastmonth: number;
      performReporting: number;
      type: number;
      isDel: boolean;
    };

    if (this.cateretailData.cateRetailId == '00000000-0000-0000-0000-000000000000') {
      this.cateretailData.details.splice(0, 1);
    }

    this.dataSourceTheoNam.forEach(x => {
      if (x.id == '') {
        detail = {
          cateRetailDetailId: '00000000-0000-0000-0000-000000000000',
          criteria: x.criteria,
          criterianame: '',
          cumulativeToReportingMonth: x.cumulativeToReportingMonth,
          estimateReportingMonth: x.estimateReportingMonth,
          performLastmonth: x.performLastmonth,
          performReporting: x.performReporting,
          type: x.type,
          isDel: false,
        };
        this.cateretailData.details.push(detail);
      }
    });
  }

  loadUser() {
    this.pageService.loaduser().subscribe(res => {
      const data = [{
        id: "00000000-0000-0000-0000-000000000000",
        text: '-- Chọn --'
      }]
      for (var user of res.items) {
        let obj_business = {
          id: user.userId,
          text: user.fullName,
        }
        data.push(obj_business);
      }
      this.dataUser = data;
      this.loadDetail();
    })
  }

  afterClose(ob: any) {
    var check = 0;
    this.dataSourceTheoNam.forEach(x => {
      if(x.criteriaName == ob.criteriaName) {
        check = 1;
      }
    });

    if (check == 1) {
      return;
    }

    this.lstbaocaonam.unshift(ob),
    this.dataSourceTheoNam = this.lstbaocaonam.filter((x: any) => !x.isDel);
    this.totaldetail1 += +ob.performLastmonth;
    this.totaldetail2 += +ob.estimateReportingMonth;
    this.totaldetail3 += +ob.cumulativeToReportingMonth;
    this.changeDetectorRefs.detectChanges();
  }

  delDetail(item: any, index: any) {
    this.totaldetail1 -= +item.performLastmonth;
    this.totaldetail2 -= +item.estimateReportingMonth;
    this.totaldetail3 -= +item.cumulativeToReportingMonth;
    if (this.cateretailData.cateRetailId != '') {
      this.lstbaocaonam.forEach((x: any) => {
        if (x.cateRetailDetailId == item.cateRetailDetailId) {
          x.isDel = true;
        }
      });
      this.dataSourceTheoNam = this.lstbaocaonam.filter((x: any) => !x.isDel);
    }
    else {
      this.lstbaocaonam.splice(index, 1);
      this.dataSourceTheoNam = this.lstbaocaonam;
    }
  }

  isDefaultValue(controlName: any): boolean {
    const control = this.formGroup.controls[controlName];
    if (control.value === "00000000-0000-0000-0000-000000000000") {
      control.setErrors({ default: true });
    }
    return control.invalid && (control.dirty || control.touched);
  }

  ngOnDestroy(): void {
    this.subscriptions.forEach(sb => sb.unsubscribe());
  }

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
    }
    else {
      this.save();
    }
  }

  round_number(num: number) {
    return (num > 0) ? num.toFixed(3).replace(/\.000$/, '') : '';
  }  
}