import { ChangeDetectorRef, Component, Input, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal, NgbDateAdapter, NgbDateParserFormatter, NgbModal, NgbDateStruct } from '@ng-bootstrap/ng-bootstrap';
import * as moment from 'moment';
import { of, Subscription } from 'rxjs';
import { catchError, finalize, first, tap } from 'rxjs/operators';
import Swal from 'sweetalert2';

import { ManageConfirmPromotionModel } from '../../../_models/manage-confirm-promotion-page.model';
import { ManageConfirmPromotionPageService } from '../../../_services/manage-confirm-promotion-page.service';
import { Options } from 'select2';
import { SelectOptionData } from 'src/app/_metronic/shared/components/select-custom/select-custom.interface';

const EMPTY_CUSTOM: ManageConfirmPromotionModel = {
  id: '',
  manageConfirmPromotionId: '',
  manageConfirmPromotionName: '',
  goodsServices: '',
  goodsServicesPay: '',
  timeStart: null,
  timeEnd: null,
  numberOfDocuments: '',
};

@Component({
  selector: 'app-edit-manage-confirm-promotion-page',
  templateUrl: './edit-manage-confirm-promotion-page.component.html',
  styleUrls: ['./edit-manage-confirm-promotion-page.component.scss'],

})
export class EditManageConfirmPromotionModalComponent implements OnInit, OnDestroy {
  @Input() id: any;
  @Input() type: any;
  isLoading$: any;
  manageConfirmPromotionData: ManageConfirmPromotionModel;
  formGroup: FormGroup;
  file_documents: any[] = [];
  del_file_ids: string = "";
  options: Options;

  MinDate: NgbDateStruct = { day: 1, month: 1, year: 1975 };
  MaxDate: NgbDateStruct = { day: 1, month: 1, year: 2050 };

  private subscriptions: Subscription[] = [];
  public default_value = "00000000-0000-0000-0000-000000000000"

  constructor(
    private manageConfirmPromotionService: ManageConfirmPromotionPageService,
    private fb: FormBuilder, public modal: NgbActiveModal,
    private modalService: NgbModal,

  ) { }

  ngOnInit(): void {
    this.isLoading$ = this.manageConfirmPromotionService.isLoading$;
    this.loadManageConfirmPromotion();
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

  clearmodel() {
    EMPTY_CUSTOM.manageConfirmPromotionId = '00000000-0000-0000-0000-000000000000';
    EMPTY_CUSTOM.manageConfirmPromotionName = '';
    EMPTY_CUSTOM.goodsServices = '',
      EMPTY_CUSTOM.goodsServicesPay = '',
      EMPTY_CUSTOM.timeStart = null,
      EMPTY_CUSTOM.timeEnd = null,
      EMPTY_CUSTOM.numberOfDocuments = '',
      this.manageConfirmPromotionData = EMPTY_CUSTOM;
  }

  loadManageConfirmPromotion() {
    if (!this.id) {
      this.clearmodel();
      this.loadForm();
    } else {
      const sb = this.manageConfirmPromotionService.getItemById(this.id).pipe(
        first(),
        catchError((errorMessage) => {
          this.modal.dismiss(errorMessage);
          return of(EMPTY_CUSTOM);
        })
      ).subscribe((res: any) => {
        this.manageConfirmPromotionData = res.data;
        this.manageConfirmPromotionData.timeStart = (res.data.timeStart);
        this.MinDate = this.convert_to_ngbstruct(res.data.timeStart) ?? { day: 1, month: 1, year: 1975 };
        this.manageConfirmPromotionData.timeEnd = (res.data.timeEnd);
        this.MaxDate = res.data.timeEnd !== null ? this.convert_to_ngbstruct(res.data.timeEnd) ?? { day: 1, month: 1, year: 2050 } : { day: 1, month: 1, year: 2050 };
        this.file_documents = res.data.details;
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
      // ManageConfirmPromotionCode: [this.manageConfirmPromotionData.tradePromotionProjectCode, Validators.compose([Validators.required, Validators.pattern("[a-zA-Z0-9]{1,15}")])],
      ManageConfirmPromotionName: [this.manageConfirmPromotionData.manageConfirmPromotionName, Validators.required],
      GoodsServices: [this.manageConfirmPromotionData.goodsServices, Validators.required],
      GoodsServicesPay: [this.manageConfirmPromotionData.goodsServicesPay],
      TimeStart: [this.manageConfirmPromotionData.timeStart, Validators.required],
      TimeEnd: [this.manageConfirmPromotionData.timeEnd, Validators.required],
      NumberOfDocuments: [this.manageConfirmPromotionData.numberOfDocuments, Validators.required],
    });
    this.formGroup.controls.TimeStart.valueChanges.subscribe((x: string) => {
      this.MinDate = this.convert_to_ngbstruct(x) ?? { day: 1, month: 1, year: 1975 }
    })
    this.formGroup.controls.TimeEnd.valueChanges.subscribe((x: string) => {
      this.MaxDate = this.convert_to_ngbstruct(x) ?? { day: 1, month: 1, year: 2050 }
    })
  }

  convert_to_ngbstruct(string_date: string) {
    if (string_date.length > 0) {
      var date = string_date.split("T")[0];
      var list = date.split("-");
      return { year: Number(list[0]), month: Number(list[1]), day: Number(list[2]) } as NgbDateStruct
    }
  }

  save() {
    const model = this.prepareManageConfirmPromotion();
    if (this.id) {
      model.append('manageConfirmPromotionId', this.id);
      this.edit(model);
    } else {
      this.create(model);
    }
  }

  edit(item: any) {
    const sbUpdate = this.manageConfirmPromotionService.updateformdata(item).pipe(
      tap(() => {
        this.modal.close();
      }),
      catchError((errorMessage) => {
        this.modal.dismiss(errorMessage);
        return of(this.manageConfirmPromotionData);
      }),
    ).subscribe((res: any) => {
      Swal.fire({
        icon: res.status == 1 ? 'success' : 'error',
        title: res.status == 1 ? 'Chỉnh sửa thành công' : 'Chỉnh sửa thất bại',
        confirmButtonText: 'Xác nhận',
        text: res.status == 0 ? res.error.msg : 'Chỉnh sửa ' + (res.status == 1 ? 'thành công' : 'thất bại'),
      });
      this.manageConfirmPromotionData = EMPTY_CUSTOM
    });
    this.subscriptions.push(sbUpdate);
  }

  create(item: any) {
    const sbCreate = this.manageConfirmPromotionService.createformdata(item).pipe(
      tap(() => {
        this.modal.close();
      }),
      catchError((errorMessage) => {
        this.modal.dismiss(errorMessage);
        return of(this.manageConfirmPromotionData);
      }),
    ).subscribe((res: any) => {
      Swal.fire({
        icon: res.status == 1 ? 'success' : 'error',
        title: res.status == 1 ? 'Thêm mới thành công' : 'Thêm mới thất bại',
        confirmButtonText: 'Xác nhận',
        text: res.status == 0 ? res.error.msg : 'Thêm mới ' + (res.status == 1 ? 'thành công' : 'thất bại'),
      });
      this.manageConfirmPromotionData = EMPTY_CUSTOM
    });
    this.subscriptions.push(sbCreate);
  }

  private prepareManageConfirmPromotion() {
    const formValue = this.formGroup.value;
    let formData: any = new FormData();
    formData.append('ManageConfirmPromotionName', formValue.ManageConfirmPromotionName);
    formData.append('GoodsServices', formValue.GoodsServices);
    formData.append('GoodsServicesPay', formValue.GoodsServicesPay);
    formData.append('NumberOfDocuments', formValue.NumberOfDocuments);
    formData.append('TimeStartGet', JSON.stringify(this.convert_datetime(formValue.TimeStart)));
    formData.append('TimeEndGet', formValue.TimeEnd != null ? JSON.stringify(this.convert_datetime(formValue.TimeEnd)) : null);

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
    if (this.file_documents[index].manageConfirmPromotionAttachFileId) {
      this.del_file_ids += this.file_documents[index].manageConfirmPromotionAttachFileId + ','
      this.file_documents.splice(index, 1);
    }
    else {
      this.file_documents.splice(index, 1);
    }
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
