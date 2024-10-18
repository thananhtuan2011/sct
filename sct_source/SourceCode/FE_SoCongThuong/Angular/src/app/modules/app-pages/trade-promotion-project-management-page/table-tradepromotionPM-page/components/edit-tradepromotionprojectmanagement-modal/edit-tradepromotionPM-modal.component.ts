import { ChangeDetectorRef, Component, Input, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal, NgbDateAdapter, NgbDateParserFormatter, NgbModal, NgbDateStruct } from '@ng-bootstrap/ng-bootstrap';
import * as moment from 'moment';
import { of, Subscription } from 'rxjs';
import { catchError, finalize, first, tap } from 'rxjs/operators';
import Swal from 'sweetalert2';

import { TradePromotionProjectManagementModel } from '../../../_models/tradepromotionPM.model';
import { TradePromotionProjectManagementPageService } from '../../../_services/tradepromotionPM-page.service';
import { Options } from 'select2';
import { SelectOptionData } from 'src/app/_metronic/shared/components/select-custom/select-custom.interface';
import { AddEnterpriseInProvinceModalComponent } from '../edit-enterprises-in-province-modal/edit-modal.component';
import { AddEnterpriseOutsideProvinceModalComponent } from '../edit-enterprises-outside-province-modal/edit-modal.component';


const EMPTY_CUSTOM: TradePromotionProjectManagementModel = {
  id: '',
  tradePromotionProjectManagementId: '',
  tradePromotionProjectManagementName: '',
  implementingAgencies: '',
  cost: '',
  currencyUnit: '',
  timeStart: null,
  timeEnd: null,
  numberOfApprovalDocuments: '',
  implementationResults: 0,
  status: 0,
  reason: ''
};

@Component({
  selector: 'app-edit-tradepromotionproject-modal',
  templateUrl: './edit-tradepromotionPM-modal.component.html',
  styleUrls: ['./edit-tradepromotionPM-modal.component.scss'],

})
export class EditTradePromotionProjectModalComponent implements OnInit, OnDestroy {
  @Input() id: any;
  @Input() type: any;
  isLoading$: any;
  tradepromotionprojectManagementData: TradePromotionProjectManagementModel;
  formGroup: FormGroup;
  file_documents: any[] = [];
  del_file_ids: string = "";
  businessData: any = [];
  public currencyUnitData: any = [];
  public implementResults: any = [
    {
      id: 0,
      text: 'Không thực hiện'
    },
    {
      id: 1,
      text: 'Có thực hiện'
    }
  ]
  options: Options;

  //Min - Max Date:
  MinDate: NgbDateStruct = { day: 1, month: 1, year: 1975 };
  MaxDate: NgbDateStruct = { day: 1, month: 1, year: 2050 };
  show: boolean = false;

  private subscriptions: Subscription[] = [];
  public default_value = "00000000-0000-0000-0000-000000000000"

  constructor(
    private tradepromotionprojectManagementService: TradePromotionProjectManagementPageService,
    private fb: FormBuilder, public modal: NgbActiveModal,
    private modalService: NgbModal,
  ) { }

  ngOnInit(): void {
    this.isLoading$ = this.tradepromotionprojectManagementService.isLoading$;
    this.loadTradePromotionProject();
    this.loadCurrencyUnit();
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

  loadCurrencyUnit() {
    this.tradepromotionprojectManagementService.loadCurrencyUnit().subscribe((res: any) => {
      var currencyUnit = [
        {
          id: '00000000-0000-0000-0000-000000000000',
          text: '-- Chọn --',
        },
      ];
      for (var item of res.items) {
        let obj_currencyUnit = {
          id: item.unitId,
          text: item.unitName,
        }
        currencyUnit.push(obj_currencyUnit)
      }
      this.currencyUnitData = currencyUnit
    })
  }

  clearmodel() {
    EMPTY_CUSTOM.currencyUnit = '00000000-0000-0000-0000-000000000000'
    EMPTY_CUSTOM.tradePromotionProjectManagementId = '00000000-0000-0000-0000-000000000000';
    EMPTY_CUSTOM.tradePromotionProjectManagementName = '';
    EMPTY_CUSTOM.implementingAgencies = '',
    EMPTY_CUSTOM.cost = '',
    EMPTY_CUSTOM.timeStart = null,
    EMPTY_CUSTOM.timeEnd = null,
    EMPTY_CUSTOM.numberOfApprovalDocuments = '',
    EMPTY_CUSTOM.implementationResults = 0,
    EMPTY_CUSTOM.status = 0,
    EMPTY_CUSTOM.reason = ''
    this.tradepromotionprojectManagementData = EMPTY_CUSTOM;
  }

  loadTradePromotionProject() {
    if (!this.id) {
      this.clearmodel();
      this.loadForm();
    } else {
      const sb = this.tradepromotionprojectManagementService.getItemById(this.id).pipe(
        first(),
        catchError((errorMessage) => {
          this.modal.dismiss(errorMessage);
          return of(EMPTY_CUSTOM);
        })
      ).subscribe((res: any) => {
        this.tradepromotionprojectManagementData = res.data;
        this.tradepromotionprojectManagementData.timeStart = (res.data.timeStart);
        this.MinDate = this.convert_to_ngbstruct(res.data.timeStart) ?? { day: 1, month: 1, year: 1975 }
        res.data.timeEnd !== null ? this.tradepromotionprojectManagementData.timeEnd = (res.data.timeEnd) : null;
        this.MaxDate = res.data.timeEnd !== null ? this.convert_to_ngbstruct(res.data.timeEnd) ?? { day: 1, month: 1, year: 2050 } : { day: 1, month: 1, year: 2050 }
        this.file_documents = res.data.details;
        this.businessData = res.data.businessDetails;
        this.tradepromotionprojectManagementData.cost = res.data.cost != null ? this.f_currency(res.data.cost) : '0';
        this.tradepromotionprojectManagementData.numberOfApprovalDocuments = res.data.numberOfApprovalDocuments != null ? this.f_currency(res.data.numberOfApprovalDocuments) : '0';
        this.loadForm();
        if (this.type) {
          this.formGroup.disable();
          this.formGroup.updateValueAndValidity();
        }
      });
      this.subscriptions.push(sb);
    }
  }

  loadForm() {
    this.formGroup = this.fb.group({
      TradePromotionProjectManagementName: [this.tradepromotionprojectManagementData.tradePromotionProjectManagementName, Validators.required],
      ImplementingAgencies: [this.tradepromotionprojectManagementData.implementingAgencies, Validators.required],
      Cost: [this.tradepromotionprojectManagementData.cost],
      CurrencyUnit: [this.tradepromotionprojectManagementData.currencyUnit],
      TimeStart: [this.tradepromotionprojectManagementData.timeStart, Validators.required],
      TimeEnd: [this.tradepromotionprojectManagementData.timeEnd],
      NumberOfApprovalDocuments: [this.tradepromotionprojectManagementData.numberOfApprovalDocuments, Validators.required],
      ImplementationResults: [this.tradepromotionprojectManagementData.implementationResults],
      Status: [this.tradepromotionprojectManagementData.status],
      Reason: [this.tradepromotionprojectManagementData.reason]
    });
    this.subscriptions.push(
      this.formGroup.controls.TimeStart.valueChanges.subscribe((x: string) => {
        this.MinDate = this.convert_to_ngbstruct(x) ?? { day: 1, month: 1, year: 1975 }
      })
    )
    this.subscriptions.push(
      this.formGroup.controls.TimeEnd.valueChanges.subscribe((x: string) => {
        this.MaxDate = this.convert_to_ngbstruct(x) ?? { day: 1, month: 1, year: 2050 }
      })
    )
    this.subscriptions.push(
      this.formGroup.controls.Cost.valueChanges.subscribe((x: string) => {
        this.formGroup.patchValue({
          "Cost": this.f_currency(x)
        }, { emitEvent: false })
      })
    )
    this.subscriptions.push(
      this.formGroup.controls.NumberOfApprovalDocuments.valueChanges.subscribe((x: string) => {
        this.formGroup.patchValue({
          "NumberOfApprovalDocuments": this.f_currency(x)
        }, { emitEvent: false })
      })
    )
  }

  convert_to_ngbstruct(string_date: string) {
    if (string_date.length > 0) {
      var date = string_date.split("T")[0];
      var list = date.split("-");
      return { year: Number(list[0]), month: Number(list[1]), day: Number(list[2]) } as NgbDateStruct
    }
  }

  save() {
    const model = this.prepareTradePromotionProjectManagement();
    if (this.id) {
      model.append('tradePromotionProjectManagementId', this.id);
      this.edit(model);
    } else {
      this.create(model);
    }
  }

  edit(item: any) {
    const sbUpdate = this.tradepromotionprojectManagementService.updateformdata(item).pipe(
      tap(() => {
        this.modal.close();
      }),
      catchError((errorMessage) => {
        this.modal.dismiss(errorMessage);
        return of(this.tradepromotionprojectManagementData);
      }),
    ).subscribe((res: any) => {
      Swal.fire({
        icon: res.status == 1 ? 'success' : 'error',
        title: res.status == 1 ? 'Chỉnh sửa thành công' : 'Chỉnh sửa thất bại',
        confirmButtonText: 'Xác nhận',
        text: res.status == 0 ? res.error.msg : 'Chỉnh sửa ' + (res.status == 1 ? 'thành công' : 'thất bại'),
      });
      this.tradepromotionprojectManagementData = EMPTY_CUSTOM
    });
    this.subscriptions.push(sbUpdate);
  }

  create(item: any) {
    const sbCreate = this.tradepromotionprojectManagementService.createformdata(item).pipe(
      tap(() => {
        this.modal.close();
      }),
      catchError((errorMessage) => {
        this.modal.dismiss(errorMessage);
        return of(this.tradepromotionprojectManagementData);
      }),
    ).subscribe((res: any) => {
      Swal.fire({
        icon: res.status == 1 ? 'success' : 'error',
        title: res.status == 1 ? 'Thêm mới thành công' : 'Thêm mới thất bại',
        confirmButtonText: 'Xác nhận',
        text: res.status == 0 ? res.error.msg : 'Thêm mới ' + (res.status == 1 ? 'thành công' : 'thất bại'),
      });
      this.tradepromotionprojectManagementData = EMPTY_CUSTOM
    });
    this.subscriptions.push(sbCreate);
  }

  private prepareTradePromotionProjectManagement() {
    const formValue = this.formGroup.value;
    let formData: any = new FormData();
    formData.append('TradePromotionProjectManagementName', formValue.TradePromotionProjectManagementName);
    formData.append('ImplementingAgencies', formValue.ImplementingAgencies);
    formData.append('Cost', Number(formValue.Cost.replaceAll(',' , '')));
    formData.append('CurrencyUnit', formValue.CurrencyUnit);
    formData.append('NumberOfApprovalDocuments', Number(formValue.NumberOfApprovalDocuments.replaceAll(',' , '')));
    formData.append('TimeStartGet', JSON.stringify(this.convert_datetime(formValue.TimeStart)));
    formData.append('TimeEndGet', formValue.TimeEnd != null ? JSON.stringify(this.convert_datetime(formValue.TimeEnd)) : null);
    formData.append('ImplementationResults', formValue.ImplementationResults);
    formData.append('Reason', formValue.Reason);

    if (this.file_documents.length > 0) {
      let i = 1;
      for (var document of this.file_documents) {
        if (document.name) {
          formData.append("FileQuyetDinh" + i, document, document.name);
          i++;
        }
      }
    }

    if (this.del_file_ids != "") {
      //Id của file cần xoá
      formData.append("IdFiles", this.del_file_ids)
    }

    if (this.businessData.length > 0) {
      formData.append("BusinessDetail", JSON.stringify(this.businessData));
    }
    return formData;
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

  f_currency(value: any, args?: any): any {
    let nbr = Number((value + '').replace(/,|-/g, ''));
    return (nbr + '').replace(/(\d)(?=(\d{3})+(?!\d))/g, '$1,');
  }

  prenventInputNonNumber(event: any) {
    if (event.which < 48 || event.which > 57) {
      event.preventDefault();
    }
  }

  prepareFilesList(files: Array<any>) {
    for (const item of files) {
      item.progress = 0;
      this.file_documents.push(item);
    }
  }

  @ViewChild('fileDropRef') fileDropRef: any;

  onFileDropped($event: any) {
    this.prepareFilesList($event);
  }

  fileBrowseHandler(files: any) {
    this.prepareFilesList(files.target.files);
  }

  deleteFile(index: number) {
    this.fileDropRef.nativeElement.value = '';
    if (this.file_documents[index].tradePromotionProjectManagementAttachFileId) {
      this.del_file_ids += this.file_documents[index].tradePromotionProjectManagementAttachFileId + ','
      this.file_documents.splice(index, 1);
    }
    else {
      this.file_documents.splice(index, 1);
    }
  }

  add_enterprise_in_province(id: any) {
    const modalRef = this.modalService.open(AddEnterpriseInProvinceModalComponent, { size: '100px' });
    modalRef.result.then(({ ...res }) =>
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
    modalRef.result.then(({ ...res }) =>
      res,
      (res) => {
        if (res) {
          this.add_detail(res)
        }
      }
    );
  }

  add_detail(data: any) {
    if (this.businessData.findIndex((x: any) => x.businessNameVi === data.BusinessNameVi) === -1) {
      let obj_add = {
        tradepromotionprojectManagementId: this.id == '' ? '00000000-0000-0000-0000-000000000000' : this.id,
        businessId: data.BusinessId,
        businessCode: data.BusinessCode,
        businessNameVi: data.BusinessNameVi,
        nganhNghe: data.NganhNghe,
        diaChi: data.DiaChi,
        nguoiDaiDien: data.NguoiDaiDien,
      }
      this.businessData.push(obj_add);
    }
  }


  delete_detail(data: any) {
    this.businessData = this.businessData.filter((x: any) => x.businessNameVi !== data.businessNameVi)
  }

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

  convert_datetime(date: string) {
    if (date) {
      if (date.includes("+07:00")) {
        const result = moment.utc(date)
        return result
      }
      else {
        const result = moment.utc(date + "+07:00")
        return result
      }
    }
    else {
      return null
    }
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
