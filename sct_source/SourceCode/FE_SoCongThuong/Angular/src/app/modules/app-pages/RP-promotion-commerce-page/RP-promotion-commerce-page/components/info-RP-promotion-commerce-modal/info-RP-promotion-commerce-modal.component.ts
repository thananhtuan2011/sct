import { ChangeDetectorRef, Component, Input, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal, NgbDateAdapter, NgbDateParserFormatter } from '@ng-bootstrap/ng-bootstrap';
import { of, Subscription } from 'rxjs';
import { catchError, finalize, first, tap } from 'rxjs/operators';
import { SelectOptionData } from 'src/app/_metronic/shared/components/select-custom/select-custom.interface';
import Swal from 'sweetalert2';
import { Options } from 'select2';
import { CommonService } from 'src/app/_metronic/shared/services/common.service';
import { ReportPromotionCommerceModel } from '../../../_models/RP-promotion-commerce.model';
import { ReportPromotionCommercePageService } from '../../../_services/RP-promotion-commerce-page.service';
import * as moment from 'moment';

const EMPTY_CUSTOM: ReportPromotionCommerceModel = {
  id: '',
  ReportPromotionCommerceId: '',
  business: '00000000-0000-0000-0000-000000000000',
  host: '',
  projectName: '',
  chief: '',
  organize: '',
  location: '',
  scale: '',
  resultNote: '',
  position: '', 
  represent: '',
  phoneNumber: '',
  fax: '',
  note: '',
  startTime : null,
  endTime : null,
}
@Component({
  selector: 'app-info-rp-promotion-commerce-modal.component',
  templateUrl: './info-RP-promotion-commerce-modal.component.html',
  styleUrls: ['./info-RP-promotion-commerce-modal.component.scss'],

})
export class InfoReportPromotionCommerceModalComponent implements OnInit, OnDestroy {
  @Input() id: any;
  isLoading$:any;
  ReportPromotionCommerceData: ReportPromotionCommerceModel;
  formGroup: FormGroup;
  public options: Options;
  dataSource: any[] = [];
  displayedColumns: string[] = ['stt','name', 'action'];
  public datKyBaoCao: Array<SelectOptionData>;

  private subscriptions: Subscription[] = [];
  public default_value = "00000000-0000-0000-0000-000000000000"
  public data_busi: Array<SelectOptionData>;

  constructor(
    private reportPromotionCommercePageService: ReportPromotionCommercePageService,
    private fb: FormBuilder, public modal: NgbActiveModal,
    private changeDetectorRefs: ChangeDetectorRef,
    private commonService: CommonService,
    ) { }

  ngOnInit(): void {
    this.isLoading$ = this.reportPromotionCommercePageService.isLoading$;
    (async () => { 
    this.loadBusi();
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
    return jQuery('<span class="form-select form-select-solid form-select-lg">' + state.text + '</span>');
  }

  loadDetail() {
    
    if (!this.id) {
      this.clear_model();
      this.loadForm();
    } else {
      const sb = this.reportPromotionCommercePageService.getItemById(this.id).pipe(
        first(),
        catchError((errorMessage) => {
          this.modal.dismiss(errorMessage);
          return of(EMPTY_CUSTOM);
        })
      ).subscribe((res: any) => {
        this.ReportPromotionCommerceData = res.data;
        this.ReportPromotionCommerceData.startTime =this.convert_date_string(res.data.startTime);
        this.ReportPromotionCommerceData.endTime =this.convert_date_string(res.data.endTime);
        this.loadForm();
        const value=this.dataSource.findIndex(x => x.businessId ==res.data.business);
        var list:any;
        list=this.dataSource[value];
        this.formGroup.controls["address"].setValue(list.diaChi);
        this.formGroup.updateValueAndValidity();
      });
      this.subscriptions.push(sb);
    }
  }

  loadForm() {
    this.formGroup = this.fb.group({
      business: [this.ReportPromotionCommerceData.business, Validators.compose([Validators.required ])],
      host: [this.ReportPromotionCommerceData.host, Validators.compose([Validators.required])],
      chief: [this.ReportPromotionCommerceData.chief, Validators.compose([Validators.required ])],
      organize: [this.ReportPromotionCommerceData.organize, Validators.compose([Validators.required ])],
      address: [''],
      location: [this.ReportPromotionCommerceData.location, Validators.compose([Validators.required ])],
      scale: [this.ReportPromotionCommerceData.scale, Validators.compose([Validators.required ])],
      resultNote: [this.ReportPromotionCommerceData.resultNote, Validators.compose([Validators.required ])],
      projectName: [this.ReportPromotionCommerceData.projectName, Validators.compose([Validators.required ])],
      position: [this.ReportPromotionCommerceData.position, Validators.compose([Validators.required ])],
      represent: [this.ReportPromotionCommerceData.represent, Validators.compose([Validators.required])],
      phonenumber: [this.ReportPromotionCommerceData.phoneNumber, Validators.compose([Validators.required ])],
      startTime: [this.ReportPromotionCommerceData.startTime, Validators.compose([Validators.required ])],
      endTime: [this.ReportPromotionCommerceData.endTime, Validators.compose([Validators.required ])],
      fax: [this.ReportPromotionCommerceData.fax],
      note: [this.ReportPromotionCommerceData.note],
    });
    this.formGroup.disable();
  }
clear_model() {
    EMPTY_CUSTOM.ReportPromotionCommerceId = '',
    EMPTY_CUSTOM.business = '00000000-0000-0000-0000-000000000000',
    EMPTY_CUSTOM.host = '',
    EMPTY_CUSTOM.chief = '',
    EMPTY_CUSTOM.organize = '',
    EMPTY_CUSTOM.location = '',
    EMPTY_CUSTOM.resultNote = '',
    EMPTY_CUSTOM.note = '',
    EMPTY_CUSTOM.position = '',
    EMPTY_CUSTOM.represent = '',
    EMPTY_CUSTOM.phoneNumber = '',
    EMPTY_CUSTOM.startTime = null,
    EMPTY_CUSTOM.endTime = null,
    EMPTY_CUSTOM.fax = '',
    EMPTY_CUSTOM.note = '',
    this.ReportPromotionCommerceData = EMPTY_CUSTOM
  }
  save() {
    this.prepareTypeOfEnergy();
    if (this.ReportPromotionCommerceData.ReportPromotionCommerceId!= '') {
      this.edit();
    } else {
      this.ReportPromotionCommerceData.ReportPromotionCommerceId = this.default_value
      this.create();
    }
  }

  edit() {
    const sbUpdate = this.reportPromotionCommercePageService.update(this.ReportPromotionCommerceData).pipe(
      tap(() => {
        this.modal.close();
      }),
      catchError((errorMessage) => {
        this.modal.dismiss(errorMessage);
        return of(this.ReportPromotionCommerceData);
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
    const sbCreate = this.reportPromotionCommercePageService.create(this.ReportPromotionCommerceData).pipe(
      tap(() => {
        this.modal.close();
      }),
      catchError((errorMessage) => {
        this.modal.dismiss(errorMessage);
        return of(this.ReportPromotionCommerceData);
      }),
    ).subscribe((res: any) => {
      Swal.fire({
        icon: res.status == 1 ? 'success' : 'error',
        title: res.status == 1 ? 'Thêm mới thành công' : 'Thêm mới thất bại',
        confirmButtonText: 'Xác nhận',
        text: 'Thêm mới ' + (res.status == 1 ? 'thành công' : 'thất bại'),
      });
      this.ReportPromotionCommerceData = EMPTY_CUSTOM
    });
    this.subscriptions.push(sbCreate);
    EMPTY_CUSTOM.ReportPromotionCommerceId='';
    this.ReportPromotionCommerceData = EMPTY_CUSTOM;
  }
  convert_date(string_date: string){
    var result = moment.utc(string_date, "DD/MM/YYYY");
    return result
  }

  convert_date_string(string_date: string){
    var date = string_date.split("T")[0];
    var list = date.split("-"); //["year", "month", "day"]
    var result = list[2] + "/" + list[1] + "/" + list[0]
    return result
  }
  private prepareTypeOfEnergy() {
    const formData = this.formGroup.value;
    this.ReportPromotionCommerceData.business = formData.business;
    this.ReportPromotionCommerceData.host = formData.host;
    this.ReportPromotionCommerceData.projectName = formData.projectName;
    this.ReportPromotionCommerceData.chief = formData.chief;
    this.ReportPromotionCommerceData.location = formData.location;
    this.ReportPromotionCommerceData.organize = formData.organize;
    this.ReportPromotionCommerceData.resultNote = formData.resultNote;
    this.ReportPromotionCommerceData.position = formData.position;
    this.ReportPromotionCommerceData.represent = formData.represent;
    this.ReportPromotionCommerceData.phoneNumber = formData.phonenumber;
    this.ReportPromotionCommerceData.startTime = this.convert_date(formData.startTime);
    this.ReportPromotionCommerceData.endTime = this.convert_date(formData.endTime);
    this.ReportPromotionCommerceData.fax = formData.fax;
    this.ReportPromotionCommerceData.scale = formData.scale;
    this.ReportPromotionCommerceData.note = formData.note;
  }
  loadProjectById($event:any) {
    const value=this.dataSource.findIndex(x => x.businessId ==$event);
    var list:any;
    list=this.dataSource[value];
    this.formGroup.controls["address"].setValue(list.diaChi);
    this.formGroup.updateValueAndValidity();
  }
loadBusi() {
    this.commonService.getBusiness().subscribe((res:any) => {
      this.dataSource=res.items;
      const data_busi = [{
        id: "00000000-0000-0000-0000-000000000000",
        text: '-- Chọn --'
      }]
      for (var item of res.items) {
        let obj_item = {
          id: item.businessId,
          text: item.businessNameVi,
        }
        data_busi.push(obj_item)
      }
      this.data_busi = data_busi;
    })
    return this.data_busi;
  }

  isDefaultValue(controlName: any)//: boolean 
  {
    const control = this.formGroup.controls[controlName];
    const isdefaultvalue = (control.value == "00000000-0000-0000-0000-000000000000")
    if (isdefaultvalue){
      control.setErrors({default: true})
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

}
