import { ChangeDetectorRef, Component, Input, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal, NgbDateAdapter, NgbDateParserFormatter, NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { BehaviorSubject, Observable, of, Subscription } from 'rxjs';
import { catchError, finalize, first, tap } from 'rxjs/operators';
import { SelectOptionData } from 'src/app/_metronic/shared/components/select-custom/select-custom.interface';
import Swal from 'sweetalert2';
import { Options } from 'select2';
import { NgIf } from '@angular/common';
import * as moment from 'moment';
import { UserModel } from 'src/app/modules/auth/models/user.model';
import { AuthService } from 'src/app/modules/auth/services/auth.service';
import { CateCriteriaNumberSevenModel } from '../../../_models/cate-criteria.model';
import { CateCriteriaNumberSevenService } from '../../../_services/cate-criteria.service';
import { EditCateCriteriaDetail1ModalComponent } from '../edit-cate-criteria-detail1/edit-cate-criteria-detail1-modal.component';
export type UserType = UserModel | undefined;

const EMPTY_CUSTOM: CateCriteriaNumberSevenModel = {
  id: '',
  cateCriteriaNumberSevenId: '',
  cateCriteriaNumberSevenCode: '',
  confirmUserId: '00000000-0000-0000-0000-000000000000',
  checkUserId: '00000000-0000-0000-0000-000000000000',
  createUserId: '00000000-0000-0000-0000-000000000000',
  confirmTime: null,
  createTime: null,
  reportMonth: '',
  details: [{
    cateCriteriaNumberSevenId: '',
    districtId: '00000000-0000-0000-0000-000000000000',
    districtName: '',
    numberOfWard: 0,
    numberOfQualifyingWard: 0,
    numberOfWardCommercialInfrastructure: 0,
    numberOfWardWithMarket: 0,
    numberOfWardCommercialInfrastructureEstimate: 0,
    numberOfWardCommercialInfrastructurePlan: 0,
    numberOfWardNewCountryside: 0,
    numberOfWardNewCountrysideEstimate: 0,
    numberOfWardNewCountrysidePlan: 0,
  }],
};

@Component({
  selector: 'app-edit-cate-criteria-modal.component',
  templateUrl: './edit-cate-criteria-modal.component.html',
  styleUrls: ['./edit-cate-criteria-modal.component.scss'],

})
export class EditCateCriteriaModalComponent implements OnInit, OnDestroy {
  @Input() id: any;
  @Input() view: any;
  isLoading$: any;
  cateretailData: CateCriteriaNumberSevenModel;
  formGroup: FormGroup;
  public options: Options;
  dataSourceDetail: any[] = [];
  lstbaocaonam: any[] = [];
  lstbaocaonamtruoc: any[] = [];
  displayedColumns: string[] = ['stt', 'name', 'action'];
  dataSource: any[] = [];
  datauser: any;
  show: boolean = false;


  private subscriptions: Subscription[] = [];
  public default_value = "00000000-0000-0000-0000-000000000000"
  public dataUser: Array<SelectOptionData>;

  constructor(
    private CateCriteriaService: CateCriteriaNumberSevenService,
    private authService: AuthService,
    private fb: FormBuilder, public modal: NgbActiveModal,
    private changeDetectorRefs: ChangeDetectorRef,
    private modalService: NgbModal
  ) { }

  ngOnInit(): void {
    this.isLoading$ = this.CateCriteriaService.isLoading$;
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
      this.show = true;
    } else {
      const sb = this.CateCriteriaService.getItemById(this.id).pipe(
        first(),
        catchError((errorMessage) => {
          this.modal.dismiss(errorMessage);
          return of(EMPTY_CUSTOM);
        })
      ).subscribe((res: any) => {
        this.cateretailData = res.data;
        this.cateretailData.confirmTime = res.data.confirmTime != null ? this.convert_date_string(res.data.confirmTime) : null;
        this.cateretailData.createTime = this.convert_date_string(res.data.createTime);
        res.data.details.forEach((x: any) => {
          this.lstbaocaonam.push(x);
          this.dataSourceDetail = this.lstbaocaonam;
        });
        this.loadForm();
        if (this.view) {
          this.formGroup.disable();
        }
        this.formGroup.updateValueAndValidity();
        this.show = true;
      });
      this.subscriptions.push(sb);
    }
  }

  loadForm() {
    this.formGroup = this.fb.group({
      cateCriteriaNumberSevenCode: [this.cateretailData.cateCriteriaNumberSevenCode, Validators.required],
      CheckName: [this.cateretailData.checkUserId],
      ConfirmName: [this.cateretailData.confirmUserId],
      CreateName: [this.cateretailData.createUserId],
      NgayDuyet: [this.cateretailData.confirmTime],
      NgayTao: [this.cateretailData.createTime],
      ReportMonth: [this.cateretailData.reportMonth],
    });
  }

  clear_model() {
    EMPTY_CUSTOM.cateCriteriaNumberSevenId = '',
      EMPTY_CUSTOM.confirmUserId = '00000000-0000-0000-0000-000000000000',
      EMPTY_CUSTOM.checkUserId = '00000000-0000-0000-0000-000000000000',
      // EMPTY_CUSTOM.createUserId = '00000000-0000-0000-0000-000000000000',
      EMPTY_CUSTOM.confirmTime = '',
      // EMPTY_CUSTOM.createTime = '',
      EMPTY_CUSTOM.cateCriteriaNumberSevenCode = '',
      EMPTY_CUSTOM.reportMonth = '',
      EMPTY_CUSTOM.details = [{
        cateCriteriaNumberSevenId: '',
        districtId: '00000000-0000-0000-0000-000000000000',
        districtName: '',
        numberOfWard: 0,
        numberOfQualifyingWard: 0,
        numberOfWardCommercialInfrastructure: 0,
        numberOfWardWithMarket: 0,
        numberOfWardCommercialInfrastructureEstimate: 0,
        numberOfWardCommercialInfrastructurePlan: 0,
        numberOfWardNewCountryside: 0,
        numberOfWardNewCountrysideEstimate: 0,
        numberOfWardNewCountrysidePlan: 0,
      }],
      this.cateretailData = EMPTY_CUSTOM
  }

  save() {
    this.prepare();
    if (this.cateretailData.cateCriteriaNumberSevenId != '') {
      this.edit();
    } else {
      this.cateretailData.cateCriteriaNumberSevenId = this.default_value
      this.create();
    }
  }

  edit() {
    const sbUpdate = this.CateCriteriaService.update(this.cateretailData).pipe(
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
    const sbCreate = this.CateCriteriaService.create(this.cateretailData).pipe(
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
      this.cateretailData = EMPTY_CUSTOM
    });
    this.subscriptions.push(sbCreate);
  }

  createCate() {
    const modalRef = this.modalService.open(EditCateCriteriaDetail1ModalComponent, { size: 'xl' });
    modalRef.result.then(({ ...res }) =>
      this.afterClose(res),
      () => { }
    );
  }

  afterClose(ob: any) {
    var check = 0;
    this.dataSourceDetail.forEach(x => {
      if (x.districtId == ob.districtId) {
        check = 1;
      }
    });
    if (check == 1) {
      return;
    }
    this.lstbaocaonam.unshift(ob),
      this.dataSourceDetail = this.lstbaocaonam;
    this.changeDetectorRefs.detectChanges();
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
    this.cateretailData.cateCriteriaNumberSevenCode = formData.cateCriteriaNumberSevenCode;
    this.cateretailData.confirmUserId = formData.ConfirmName;
    this.cateretailData.checkUserId = formData.CheckName;
    this.cateretailData.createUserId = formData.CreateName;
    this.cateretailData.confirmTime = this.convert_date(formData.NgayDuyet);
    this.cateretailData.createTime = this.convert_date(formData.NgayTao);
    this.cateretailData.reportMonth = formData.ReportMonth;
    let detail: {
      cateCriteriaNumberSevenId: '00000000-0000-0000-0000-000000000000';
      districtId: string;
      districtName: string;
      numberOfWard: number;
      numberOfQualifyingWard: number;
      numberOfWardWithMarket: number;
      numberOfWardCommercialInfrastructure: number;
      numberOfWardCommercialInfrastructureEstimate: number;
      numberOfWardCommercialInfrastructurePlan: number;
      numberOfWardNewCountryside: number;
      numberOfWardNewCountrysideEstimate: number;
      numberOfWardNewCountrysidePlan: number;
    };

    this.cateretailData.details = EMPTY_CUSTOM.details;
    if (this.cateretailData.details.length > 0) {
      this.cateretailData.details.splice(0, 1);
    }

    this.dataSourceDetail.forEach(x => {
      detail = {
        cateCriteriaNumberSevenId: '00000000-0000-0000-0000-000000000000',
        districtId: x.districtId,
        districtName: x.districtName,
        numberOfWard: x.numberOfWard,
        numberOfQualifyingWard: x.numberOfQualifyingWard,
        numberOfWardWithMarket: x.numberOfWardWithMarket,
        numberOfWardCommercialInfrastructure: x.numberOfWardCommercialInfrastructure,
        numberOfWardCommercialInfrastructureEstimate: x.numberOfWardCommercialInfrastructureEstimate,
        numberOfWardCommercialInfrastructurePlan: x.numberOfWardCommercialInfrastructurePlan,
        numberOfWardNewCountryside: x.numberOfWardNewCountryside,
        numberOfWardNewCountrysideEstimate: x.numberOfWardNewCountrysideEstimate,
        numberOfWardNewCountrysidePlan: x.numberOfWardNewCountrysidePlan,
      };
      this.cateretailData.details.push(detail);
    });
  }

  loadUser() {
    this.CateCriteriaService.loaduser().subscribe(res => {
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
      // this.datauser = this.authService.getDataUser();
      // this.cateretailData.createUserId = this.datauser.userid;
      // var today = new Date();
      // var dd = String(today.getDate()).padStart(2, '0');
      // var mm = String(today.getMonth() + 1).padStart(2, '0'); //January is 0!
      // var yyyy = today.getFullYear();
      // var date;
      // date = dd + '/' + mm + '/' + yyyy;
      // this.cateretailData.createTime = date;
    })
  }

  //Chuyển output của ngbDate thành dạng Date
  convertNgbDateToDate(dateToConvert: any) {
    return new Date(Date.UTC(dateToConvert.year, dateToConvert.month - 1, dateToConvert.day))
  }

  //Chuyển Date thành định dạng của ngbDate
  converDateToNbgDate(dateToConvert: any) {
    const data = new Date(dateToConvert)
    let NgbDate = {
      year: data.getFullYear(),
      month: data.getMonth() + 1,
      day: data.getDate(),
    }
    return NgbDate
  }

  delDetail1(item: any) {
    const index: number = this.lstbaocaonam.indexOf(item);
    this.lstbaocaonam.splice(index, 1);
    this.dataSourceDetail = this.lstbaocaonam;
  }

  isDefaultValue(controlName: any)//: boolean 
  {
    const control = this.formGroup.controls[controlName];
    const isdefaultvalue = (control.value == "00000000-0000-0000-0000-000000000000")
    if (isdefaultvalue) {
      control.setErrors({ default: true })
    }
    return control.invalid && (control.dirty || control.touched)
  }

  ngOnDestroy(): void {
    this.subscriptions.forEach(sb => sb.unsubscribe());
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
    }
    else {
      this.save();
    }
  }
}
