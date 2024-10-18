import { ChangeDetectorRef, Component, Input, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { of, Subscription } from 'rxjs';
import { catchError, first, tap } from 'rxjs/operators';
import Swal from 'sweetalert2';
import { Options } from 'select2';
import { CommonService } from 'src/app/_metronic/shared/services/common.service';
import { TradePromotionOtherModel } from '../../../_models/trade-promotion-other.model';
import { TradePromotionOtherPageService } from '../../../_services/trade-promotion-other-page.service';

const EMPTY_CUSTOM: TradePromotionOtherModel = {
  id: '',
  tradePromotionOtherId: '00000000-0000-0000-0000-000000000000',
  typeOfActivity: 0,
  content: '',
  startDate: '',
  endDate: '',
  time: '',
  districtId: '00000000-0000-0000-0000-000000000000',
  address: '',
  implementationCost: 0,
  participating: '',
  coordination: '',
  result: '',
  note: '',
  details: [],
};

@Component({
  selector: 'app-edit-trade-promotion-other-modal.component',
  templateUrl: './edit-trade-promotion-other-modal.component.html',
  styleUrls: ['./edit-trade-promotion-other-modal.component.scss'],
})

export class EditTradePromotionOtherModalComponent implements OnInit, OnDestroy {
  @Input() id: any;
  @Input() type: any;
  isLoading$: any;
  tradePromotionOtherData: TradePromotionOtherModel;
  formGroup: FormGroup;
  showForm: boolean = false;
  public options: Options;
  private subscriptions: Subscription[] = [];

  ListFileDel: any = '';
  files: any[] = [];
  districtData: any[];
  typeData: any = [
    {
      id: "0",
      text: "Hội nghị kết nối cung - cầu hàng hóa"
    },
    {
      id: "1",
      text: "Hội nghị, hội thao"
    },
    {
      id: "2",
      text: "Truyền thông thông tin"
    },
    {
      id: "3",
      text: "Hoạt động khác"
    },
  ]


  constructor(
    private tradePromotionOtherPageService: TradePromotionOtherPageService,
    private fb: FormBuilder, public modal: NgbActiveModal,
    private changeDetectorRefs: ChangeDetectorRef,
    private commonService: CommonService,
  ) { }

  ngOnInit(): void {
    this.isLoading$ = this.tradePromotionOtherPageService.isLoading$;
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
      this.clear_model();
      this.loadForm();
    } else {
      const sb = this.tradePromotionOtherPageService.getItemById(this.id).pipe(
        first(),
        catchError((errorMessage) => {
          this.modal.dismiss(errorMessage);
          return of(EMPTY_CUSTOM);
        })
      ).subscribe((res: any) => {
        this.tradePromotionOtherData = res.data;
        this.tradePromotionOtherData.startDate = res.data.startDateDisplay;
        this.tradePromotionOtherData.endDate = res.data.endDateDisplay;
        this.tradePromotionOtherData.implementationCost = this.f_currency(res.data.implementationCost);
        
        if (res.data.details) {
          this.files = res.data.details;
        }

        // let detail: {
        //   tradePromotionOtherAttachFileId: string;
        //   linkFile: string;
        //   name: string;
        // };

        // res.data.details.forEach((x: any) => {
        //   detail = {
        //     tradePromotionOtherAttachFileId: x.tradePromotionOtherAttachFileId,
        //     linkFile: x.linkFile,
        //     name: x.linkFile.split('/sct/TradePromotionOtherDetail/')[1]
        //   }
        //   this.files.push(detail);
        // });
        this.loadForm();
      });
      this.subscriptions.push(sb);
    }
  }

  loadForm() {
    this.formGroup = this.fb.group({
      TypeOfActivity: [this.tradePromotionOtherData.typeOfActivity + ""],
      Content: [this.tradePromotionOtherData.content, Validators.required],
      StartDate: [this.tradePromotionOtherData.startDate, Validators.required],
      EndDate: [this.tradePromotionOtherData.endDate],
      Time: [this.tradePromotionOtherData.time],
      DistrictId: [this.tradePromotionOtherData.districtId],
      Address: [this.tradePromotionOtherData.address, Validators.required],
      ImplementationCost: [this.tradePromotionOtherData.implementationCost + "", Validators.required],
      Participating: [this.tradePromotionOtherData.participating],
      Coordination: [this.tradePromotionOtherData.coordination],
      Result: [this.tradePromotionOtherData.result],
      Note: [this.tradePromotionOtherData.note],
    });
    if (!!this.type) {
      this.formGroup.disable();
    }
    this.showForm = true;
    this.formGroup.controls.ImplementationCost.valueChanges.subscribe(x => {
      this.formGroup.patchValue({
        'ImplementationCost': this.f_currency(x)
      }, { emitEvent: false })
    })
  }

  f_currency(value: any, args?: any): any {
    let nbr = Number((value + '').replace(/,|-/g, ''));
    return (nbr + '').replace(/(\d)(?=(\d{3})+(?!\d))/g, '$1,');
  }

  clear_model() {
    EMPTY_CUSTOM.tradePromotionOtherId = '00000000-0000-0000-0000-000000000000';
    EMPTY_CUSTOM.typeOfActivity = 0;
    EMPTY_CUSTOM.content = '';
    EMPTY_CUSTOM.startDate = '';
    EMPTY_CUSTOM.endDate = '';
    EMPTY_CUSTOM.time = '';
    EMPTY_CUSTOM.districtId = '00000000-0000-0000-0000-000000000000';
    EMPTY_CUSTOM.address = '';
    EMPTY_CUSTOM.implementationCost = 0;
    EMPTY_CUSTOM.participating = '';
    EMPTY_CUSTOM.coordination = '';
    EMPTY_CUSTOM.result = '';
    EMPTY_CUSTOM.note = '';
    EMPTY_CUSTOM.details = [];
    this.tradePromotionOtherData = EMPTY_CUSTOM
  }

  save() {
    const model = this.prepareData();
    if (this.id) {
      this.edit(model);
    } else {
      this.tradePromotionOtherData.tradePromotionOtherId = '00000000-0000-0000-0000-000000000000';
      this.create(model);
    }
  }

  private prepareData() {
    const controls = this.formGroup.value;
    var formData: any = new FormData();

    formData.append("TradePromotionOtherId", this.tradePromotionOtherData.tradePromotionOtherId);
    formData.append("TypeOfActivity", Number(controls.TypeOfActivity));
    formData.append("Content", controls.Content);
    formData.append("StartDateDisplay", controls.StartDate);
    formData.append("EndDateDisplay", controls.EndDate);
    formData.append("Time", controls.Time);
    formData.append("DistrictId", controls.DistrictId);
    formData.append("Address", controls.Address);
    formData.append("ImplementationCost", Number(controls.ImplementationCost.replaceAll(',', '')));
    formData.append("Participating", controls.Participating);
    formData.append("Coordination", controls.Coordination);
    formData.append("Result", controls.Result);
    formData.append("Note", controls.Note);

    if (this.files.length > 0) {
      let i = 1;
      for (var document of this.files) {
        if (document.name) {
          formData.append("File" + i, document, document.name);
          i++;
        }
      }
    }

    //Id của file cần xoá
    if (this.ListFileDel != "") {
      formData.append("LIdDel", this.ListFileDel)
    }

    return formData;
  }

  edit(item: any) {
    const sbUpdate = this.tradePromotionOtherPageService.updateformdata(item, this.tradePromotionOtherData.tradePromotionOtherId).pipe(
      tap(() => {
        this.modal.close();
      }),
      catchError((errorMessage) => {
        this.modal.dismiss(errorMessage);
        return of(this.tradePromotionOtherData);
      }),
    ).subscribe((res: any) => {
      Swal.fire({
        icon: res.status == 1 ? 'success' : 'error',
        title: res.status == 1 ? 'Chỉnh sửa thành công' : 'Chỉnh sửa thất bại',
        confirmButtonText: 'Xác nhận',
        text: 'Chỉnh sửa ' + (res.status == 1 ? 'thành công' : 'thất bại'),
      });
      this.tradePromotionOtherData = EMPTY_CUSTOM
    });
    this.subscriptions.push(sbUpdate);
  }

  create(item: any) {
    const sbCreate = this.tradePromotionOtherPageService.createformdata(item).pipe(
      tap(() => {
        this.modal.close();
      }),
      catchError((errorMessage) => {
        this.modal.dismiss(errorMessage);
        return of(this.tradePromotionOtherData);
      }),
    ).subscribe((res: any) => {
      Swal.fire({
        icon: res.status == 1 ? 'success' : 'error',
        title: res.status == 1 ? 'Thêm mới thành công' : 'Thêm mới thất bại',
        confirmButtonText: 'Xác nhận',
        text: 'Thêm mới ' + (res.status == 1 ? 'thành công' : 'thất bại'),
      });
      this.tradePromotionOtherData = EMPTY_CUSTOM
    });
    this.subscriptions.push(sbCreate);
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

  isDefaultValue(controlName: any)//: boolean 
  {
    const control = this.formGroup.controls[controlName];
    const isdefaultvalue = (control.value == "00000000-0000-0000-0000-000000000000")
    if (isdefaultvalue) {
      control.setErrors({ default: true })
    }
    return control.invalid && (control.dirty || control.touched)
  }

  prenventInputNonNumber(event: any) {
    if (event.which < 48 || event.which > 57) {
      event.preventDefault();
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
    if (this.files[index].tradePromotionActivityReportAttachFileId) {
      this.ListFileDel += this.files[index].tradePromotionActivityReportAttachFileId + ','
      this.files.splice(index, 1);
    }
    else {
      this.files.splice(index, 1);
    }
  }

  /**
   * Convert Files list to normal array list
   * @param files (Files List)
   */
  prepareFilesList(files: Array<any>) {
    for (const item of files) {
      this.files.push(item);
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

  check_formGroup() {
    if (this.formGroup.invalid) {
      this.formGroup.markAllAsTouched();
    }
    else {
      this.save();
    }
  }
}
