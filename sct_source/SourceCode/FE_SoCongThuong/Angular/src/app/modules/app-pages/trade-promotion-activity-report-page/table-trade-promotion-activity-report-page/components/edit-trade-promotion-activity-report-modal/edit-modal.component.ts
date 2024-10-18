import { ChangeDetectorRef, Component, Input, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal, NgbDateAdapter, NgbDateParserFormatter, NgbModal } from '@ng-bootstrap/ng-bootstrap';
import * as moment from 'moment';
import { of, Subscription, filter } from 'rxjs';
import { catchError, finalize, first, tap } from 'rxjs/operators';
import { CommonService } from 'src/app/_metronic/shared/services/common.service';
import Swal from 'sweetalert2';

import { TradePromotionActivityReportModel } from '../../../_models/trade-promotion-activity-report.model';
import { TradePromotionActivityReportService } from '../../../_services/trade-promotion-activity-report-page.service';
import { AddEnterpriseInProvinceModalComponent } from '../edit-enterprises-in-province-modal/edit-modal.component';
import { AddEnterpriseOutsideProvinceModalComponent } from '../edit-enterprises-outside-province-modal/edit-modal.component';
import { Options } from 'select2';
import { TradePromotionActivityReportDetailModel } from '../../../_models/trade-promotion-activity-report-detail.model';

@Component({
  selector: 'app-edit-modal',
  templateUrl: './edit-modal.component.html',
  styleUrls: ['./edit-modal.component.scss'],
})

export class EditTradePromotionActivityReportModalComponent implements OnInit, OnDestroy {
  @Input() id: any;
  @Input() type: any;
  isLoading$: any;
  editData: TradePromotionActivityReportModel;
  formGroup: FormGroup;
  searchGroup: FormGroup;
  options: Options;
  show: boolean = false;
  showPlanToJoin: boolean = false;

  participatingBusinessesData: any = [];
  districtData: any = [];
  scaleData: any = [
    {
      id: '0',
      text: "Trong tỉnh"
    },
    {
      id: '1',
      text: "Ngoài tỉnh"
    },
    {
      id: '2',
      text: "Ngoài nước"
    },
  ]

  file_documents: any[] = [];
  del_file_ids: string = "";

  private subscriptions: Subscription[] = [];

  constructor(
    private pageService: TradePromotionActivityReportService,
    private fb: FormBuilder, public modal: NgbActiveModal,
    public commonService: CommonService,
    private modalService: NgbModal,
    private cd: ChangeDetectorRef,
  ) { }

  ngOnInit(): void {
    this.isLoading$ = this.pageService.isLoading$;
    this.loadDistrict();
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

  loadDistrict() {
    this.commonService.getDistrict().subscribe((res: any) => {
      const data = [
        { id: "00000000-0000-0000-0000-000000000000", text: "-- Chọn --" },
        ...res.items.map((item: any) => ({
          id: item.districtId,
          text: item.districtName,
        }))
      ]
      this.districtData = data;
      this.loadPage();
    })
  }

  loadPage() {
    if (!this.id) {
      this.clear();
      this.loadForm();
    } else {
      const sb = this.pageService.getItemById(this.id).pipe(
        first(),
        catchError((errorMessage) => {
          this.modal.dismiss(errorMessage);
          return of(this.clear());
        })
      ).subscribe((res: any) => {
        this.editData = res.data;
        this.editData.scaleId = res.data.scaleId + "";
        this.editData.startDate = res.data.startDateDisplay;
        this.editData.endDate = res.data.endDateDisplay;
        this.editData.implementationCost = this.f_currency(res.data.implementationCost);
        this.editData.fundingSupport = this.f_currency(res.data.fundingSupport);
        this.editData.numParticipating = this.f_currency(res.data.numParticipating);

        if (res.data.participatingBusinesses) {
          this.participatingBusinessesData = res.data.participatingBusinesses;
        }

        if (res.data.files) {
          this.file_documents = res.data.files;
        }

        this.loadForm();

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
      ScaleId: [this.editData.scaleId], //Quy mô đề án
      PlanName: [this.editData.planName, Validators.required], //Tên đề án
      PlanToJoin: [this.editData.planToJoin], //Kế hoạch tham gia
      StartDate: [this.editData.startDate, Validators.required], //Thời gian bắt đầu
      EndDate: [this.editData.endDate], //Thời gian kết thúc
      Time: [this.editData.time], //Thời lượng
      DistrictId: [this.editData.districtId], //Id huyện
      Address: [this.editData.address, Validators.required], //Địa điểm tổ chức
      ImplementationCost: [this.editData.implementationCost + "", Validators.compose([Validators.required, Validators.pattern('^[0-9,]+$')])], //Kinh phí thực hiện
      FundingSupport: [this.editData.fundingSupport + "", Validators.compose([Validators.required, Validators.pattern('^[0-9,]+$')])], //Kinh phí hổ trợ doanh nghiệp
      Scale: [this.editData.scale], //Quy mô
      NumParticipating: [this.editData.numParticipating + "", Validators.pattern('^[0-9,]+$')], //Số lượng doanh nghiệp tham gia
      Note: [this.editData.note], //Ghi chú
      Search: ['']
    });
    this.show = true;
    this.subscriptions.push(
      this.formGroup.controls.ImplementationCost.valueChanges.subscribe((x: string) => {
        this.formGroup.patchValue({
          "ImplementationCost": this.f_currency(x)
        }, { emitEvent: false })
      })
    );
    this.subscriptions.push(
      this.formGroup.controls.FundingSupport.valueChanges.subscribe((x: string) => {
        this.formGroup.patchValue({
          "FundingSupport": this.f_currency(x)
        }, { emitEvent: false })
      })
    );
    this.subscriptions.push(
      this.formGroup.controls.NumParticipating.valueChanges.subscribe((x: string) => {
        this.formGroup.patchValue({
          "NumParticipating": this.f_currency(x)
        }, { emitEvent: false })
      })
    );
  }

  clear() {
    const EMPTY_CUSTOM: TradePromotionActivityReportModel = {
      id: "00000000-0000-0000-0000-000000000000",
      tradePromotionActivityReportId: "00000000-0000-0000-0000-000000000000",
      scaleId: '0', //Id Quy mô
      planName: '', //Tên đề án
      planToJoin: false, //Kế hoạch tham gia
      startDate: "", //Thời gian bắt đầu
      endDate: "", //Thời gian kết thúc
      time: "", //Thời lượng
      districtId: "00000000-0000-0000-0000-000000000000", //Huyện
      address: "", //Địa điểm
      implementationCost: 0, //kinh phí thực hiện
      fundingSupport: 0, //Kinh phí hổ trợ
      scale: "", //Quy mô
      numParticipating: 0, //Số lượng doanh nghiệp tham gia
      participatingBusinesses: [], //Doanh nghiệp tham gia
      note: "", //Ghi chú
    };
    this.editData = EMPTY_CUSTOM;
    return EMPTY_CUSTOM;
  }

  private prepareTradePromotionActivityReport() {
    const formValue = this.formGroup.value;
    const formData: any = new FormData();

    formData.append('ScaleId', formValue.ScaleId);
    formData.append('PlanName', formValue.PlanName);
    formData.append('PlanToJoin', formValue.PlanToJoin);
    formData.append('StartDateDisplay', formValue.StartDate);
    formData.append('EndDateDisplay', formValue.EndDate);
    formData.append('Time', formValue.Time);
    formData.append('DistrictId', formValue.DistrictId);
    formData.append('Address', formValue.Address);
    formData.append('ImplementationCost', Number(formValue.ImplementationCost.replaceAll(',','')));
    formData.append('FundingSupport', Number(formValue.FundingSupport.replaceAll(',','')));
    formData.append('Scale', formValue.Scale);
    formData.append('NumParticipating', Number(formValue.NumParticipating.replaceAll(',','')));
    formData.append('Note', formValue.Note);

    formData.append('ParticipatingBusinessesJson', JSON.stringify(this.participatingBusinessesData));

    //File documents
    if (this.file_documents.length > 0) {
      let i = 1;
      for (var document of this.file_documents) {
        if (document.name) {
          formData.append("File" + i, document, document.name);
          i++;
        }
      }
    }

    //Id của file cần xoá
    if (this.del_file_ids != "") {
      formData.append("LIdDel", this.del_file_ids)
    }
    
    return formData
  }

  diffDate(start: string, end: string) {
    if (!start && !end) {
      return "";
    }

    if (start.length === 0 || end.length === 0) {
      return "";
    }
  
    const dateStart = moment(start, 'DD/MM/YYYY');
    const dateEnd = moment(end, 'DD/MM/YYYY');
    const time = dateEnd.diff(dateStart, "day");
  
    if (time < 0) {
      return time + " ngày"
    } else if (time === 0) {
      return "Trong ngày"
    } else {
      return time + " ngày"
    }
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
    const model = this.prepareTradePromotionActivityReport();
    if (this.id) {
      model.append("TradePromotionActivityReportId", this.id)
      this.edit(model);
    } else {
      this.create(model);
    }
  }

  edit(item: any) {
    const sbUpdate = this.pageService.updateFormData(item).pipe(
      tap(() => {
        this.modal.close();
      }),
      catchError((errorMessage) => {
        this.modal.dismiss(errorMessage);
        return of(this.editData);
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

  create(item: any) {
    const sbCreate = this.pageService.createFormData(item).pipe(
      tap(() => {
        this.modal.close();
      }),
      catchError((errorMessage) => {
        this.modal.dismiss(errorMessage);
        return of(this.editData);
      }),
    ).subscribe((res: any) => {
      Swal.fire({
        icon: res.status == 1 ? 'success' : 'error',
        title: res.status == 1 ? 'Thêm mới thành công' : 'Thêm mới thất bại',
        confirmButtonText: 'Xác nhận',
        text: 'Thêm mới ' + (res.status == 1 ? 'thành công' : 'thất bại'),
      });
      this.clear();
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
    if (value == '00000000-0000-0000-0000-000000000000') {
      control.setErrors({ defaultvalue: true });
    }
    else {
      control.setErrors({ defaultvalue: null });
      control.updateValueAndValidity();
    }
    return control.invalid && (control.dirty || control.touched);
  }

  //Enterprise
  add_enterprise_in_province() {
    const modalRef = this.modalService.open(AddEnterpriseInProvinceModalComponent, { size: 'lg' });
    modalRef.result.then(({ ...res }) =>
      res,
      (res) => {
        if (res) {
          this.add_business(res)
        }
      }
    );
  }

  add_enterprise_outside_province() {
    const modalRef = this.modalService.open(AddEnterpriseOutsideProvinceModalComponent, { size: 'lg' });
    modalRef.result.then(({ ...res }) =>
      res,
      (res) => {
        if (res) {
          this.add_business(res)
        }
      }
    );
  }

  add_business(data: any) {
    if (this.participatingBusinessesData.findIndex((x: any) => x.businessName === data.BusinessName) === -1) {
      let obj_add = {
        tradePromotionActivityReportId: !!this.id ? this.id : '00000000-0000-0000-0000-000000000000',
        businessId: data.BusinessId,
        businessName: data.BusinessName,
        address: data.Address,
      } as TradePromotionActivityReportDetailModel
      this.participatingBusinessesData.push(obj_add);
    }
  }

  delete_business(index: any) {
    this.participatingBusinessesData.splice(index, 1)
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

  /**Files */
  //Upload File
  @ViewChild('fileDropRef') fileDropRef: any;
  /**
   * on file drop handler
   */
  onFileDropped($event: any) {
    this.prepareFilesList($event);
  }

  /**
   * handle file from browsing
   */
  fileBrowseHandler(files: any) {
    this.prepareFilesList(files.target.files);
  }

  /**
   * Delete file from files list
   * @param index (File index)
   */
  deleteFile(index: number) {
    this.fileDropRef.nativeElement.value = '';
    if (this.file_documents[index].tradePromotionActivityReportAttachFileId) {
      this.del_file_ids += this.file_documents[index].tradePromotionActivityReportAttachFileId + ','
      this.file_documents.splice(index, 1);
    }
    else {
      this.file_documents.splice(index, 1);
    }
  }

  /**
   * Convert Files list to normal array list
   * @param files (Files List)
   */
  prepareFilesList(files: Array<any>) {
    for (const item of files) {
      this.file_documents.push(item);
    }
  }

  /**
   * format bytes
   * @param bytes (File size in bytes)
   * @param decimals (Decimals point)
   */
  formatBytes(bytes: any, decimals: any) {
    if (bytes === 0) {
      return '0 Bytes';
    }
    const k = 1024;
    const dm = decimals <= 0 ? 0 : decimals || 2;
    const sizes = ['Bytes', 'KB', 'MB', 'GB', 'TB', 'PB', 'EB', 'ZB', 'YB'];
    const i = Math.floor(Math.log(bytes) / Math.log(k));
    return parseFloat((bytes / Math.pow(k, i)).toFixed(dm)) + ' ' + sizes[i];
  }
}
