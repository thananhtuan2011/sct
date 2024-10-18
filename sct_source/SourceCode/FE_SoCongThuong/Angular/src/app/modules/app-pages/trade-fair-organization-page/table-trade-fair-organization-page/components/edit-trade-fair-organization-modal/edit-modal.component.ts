import { SaasComponent } from '../../../../../../_metronic/layout/components/toolbar/saas/saas.component';
import { ChangeDetectorRef, Component, ElementRef, Input, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal, NgbDateAdapter, NgbDateParserFormatter, NgbDateStruct } from '@ng-bootstrap/ng-bootstrap';
import * as moment from 'moment';
import { of, Subscription } from 'rxjs';
import { catchError, finalize, first, tap } from 'rxjs/operators';
import Swal from 'sweetalert2';

import { TradeFairOrganizationCertificationModel } from '../../../_models/trade-fair-organization.model';
import { TradeFairOrganizationCertificationPageService } from '../../../_services/trade-fair-organization-page.service';

const EMPTY_CUSTOM: TradeFairOrganizationCertificationModel = {
  id: '',
  tradeFairOrganizationCertificationId: '00000000-0000-0000-0000-000000000000',
  tradeFairName: '',
  address: '',
  scale: '',
  textNumber: '',
};

@Component({
  selector: 'app-edit-modal',
  templateUrl: './edit-modal.component.html',
  styleUrls: ['./edit-modal.component.scss'],

})

export class EditTradeFairOrganizationCertificationModalComponent implements OnInit, OnDestroy {
  @Input() id: any;
  @Input() type: any;
  @ViewChild('fileDropRef') fileDropRef: any;
  
  isLoading$: any;
  tradeFairOrganizationCertificationData: TradeFairOrganizationCertificationModel;
  formGroup: FormGroup;
  
  files: any[] = [];
  del_file_ids: string = "";

  list_times: any[] = [];
  startTime: any;
  endTime: any;

  MinDate: NgbDateStruct = { day: 1, month: 1, year: 1975 };
  MaxDate: NgbDateStruct = { day: 1, month: 1, year: 2050 };

  private subscriptions: Subscription[] = [];

  constructor(
    private tradeFairOrganizationCertificationService: TradeFairOrganizationCertificationPageService,
    private fb: FormBuilder, public modal: NgbActiveModal,
    private cdr: ChangeDetectorRef
  ) { }

  ngOnInit(): void {
    this.isLoading$ = this.tradeFairOrganizationCertificationService.isLoading$;
    this.loadTradeFairOrganizationCertification();
  }

  add_time(StartTime: any, EndTime: any) {
    if (StartTime && EndTime) {
      let obj = {
        TradeFairOrganizationCertificationId: this.tradeFairOrganizationCertificationData.tradeFairOrganizationCertificationId,
        StartTime: this.convert_datetime(StartTime),
        EndTime: this.convert_datetime(EndTime)
      }
      this.list_times.push(obj);

      this.formGroup.controls.StartTime.reset();
      this.formGroup.controls.EndTime.reset();

      this.MinDate = { day: 1, month: 1, year: 1975 };
      this.MaxDate = { day: 1, month: 1, year: 2050 };

      this.formGroup.controls.StartTime.removeValidators(Validators.required);
      this.formGroup.controls.EndTime.removeValidators(Validators.required);

      this.formGroup.controls.StartTime.updateValueAndValidity();
      this.formGroup.controls.EndTime.updateValueAndValidity();
    }
    else {
      this.formGroup.controls.StartTime.setValidators(Validators.required);
      this.formGroup.controls.EndTime.setValidators(Validators.required);
      
      this.formGroup.controls.StartTime.markAsTouched();
      this.formGroup.controls.EndTime.markAsTouched();
      
      this.formGroup.controls.StartTime.updateValueAndValidity();
      this.formGroup.controls.EndTime.updateValueAndValidity();
    }
  }

  del_time(Index: any){
    this.list_times.splice(Index, 1);
    if (this.list_times.length == 0) {
      this.formGroup.controls.StartTime.setValidators(Validators.required);
      this.formGroup.controls.EndTime.setValidators(Validators.required);
      
      this.formGroup.controls.StartTime.markAsTouched();
      this.formGroup.controls.EndTime.markAsTouched();
      
      this.formGroup.controls.StartTime.updateValueAndValidity();
      this.formGroup.controls.EndTime.updateValueAndValidity();
    }
  }

  loadTradeFairOrganizationCertification() {
    if (!this.id) {
      this.clear();
      this.loadForm();
    } else {
      const sb = this.tradeFairOrganizationCertificationService.getItemById(this.id).pipe(
        first(),
        catchError((errorMessage) => {
          this.modal.dismiss(errorMessage);
          return of(EMPTY_CUSTOM);
        })
      ).subscribe((res: any) => {
        this.tradeFairOrganizationCertificationData = res.data;
        this.files = res.data.listFiles;
        if (res.data.listTimes.length > 0) {
          for (var t of res.data.listTimes) {
            let time = {
              TradeFairOrganizationCertificationId: t.tradeFairOrganizationCertificationId,
              StartTime: t.startTime,
              EndTime: t.endTime
            }
            this.list_times.push(time);
          };
        }
        this.loadForm();
        if (this.type) {
          this.formGroup.disable()
        }
        this.formGroup.controls.StartTime.removeValidators(Validators.required);
        this.formGroup.controls.EndTime.removeValidators(Validators.required);
  
        this.formGroup.controls.StartTime.updateValueAndValidity();
        this.formGroup.controls.EndTime.updateValueAndValidity();
      });
      this.subscriptions.push(sb);
    }
  }

  loadForm() {
    this.formGroup = this.fb.group({
      TradeFairName: [this.tradeFairOrganizationCertificationData.tradeFairName, Validators.required],
      Address: [this.tradeFairOrganizationCertificationData.address, Validators.required],
      Scale: [this.tradeFairOrganizationCertificationData.scale],
      TextNumber: [this.tradeFairOrganizationCertificationData.textNumber, Validators.required],
      StartTime: [this.startTime, Validators.required],
      EndTime: [this.endTime, Validators.required],
    });
    this.formGroup.controls.StartTime.valueChanges.subscribe((x: string) => {
      this.MinDate = this.convert_to_ngbstruct(x) ?? { day: 1, month: 1, year: 1975 }
    })
    this.formGroup.controls.EndTime.valueChanges.subscribe((x: string) => {
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

  clear() {
    EMPTY_CUSTOM.tradeFairOrganizationCertificationId = '00000000-0000-0000-0000-000000000000'
    EMPTY_CUSTOM.tradeFairName = '',
    EMPTY_CUSTOM.address = '',
    EMPTY_CUSTOM.scale = '',
    EMPTY_CUSTOM.textNumber = '',
    this.list_times = [],
    this.tradeFairOrganizationCertificationData = EMPTY_CUSTOM;
  }

  private prepareTradeFairOrganizationCertification() {
    var formValue = this.formGroup.value;
    var formData: any = new FormData();

    formData.append("TradeFairName", String(formValue.TradeFairName))
    formData.append("Address", formValue.Address)
    formData.append("Scale", String(formValue.Scale))
    formData.append("TextNumber", String(formValue.TextNumber))

    formData.append("ListTimeString", JSON.stringify(this.list_times))
    
    if (this.files.length > 0) {
      let i = 1;
      for (var file of this.files) {
        if (file.name) {
          formData.append("Files" + i, file, file.name);
          i++;
        }
      }
    }

    formData.append("IdFiles", this.del_file_ids)

    return formData
  }

  save() {
    const model = this.prepareTradeFairOrganizationCertification();
    if (this.id) {
      model.append("TradeFairOrganizationCertificationId", this.id)
      this.edit(model);
    } else {
      this.create(model);
    }
  }

  edit(item: any) {
    const sbUpdate = this.tradeFairOrganizationCertificationService.update_Formdata(item).pipe(
      tap(() => {
        this.modal.close();
      }),
      catchError((errorMessage) => {
        this.modal.dismiss(errorMessage);
        return of(item);
      }),
    ).subscribe((res: any) => {
      Swal.fire({
        icon: res.status == 1 ? 'success' : 'error',
        title: res.status == 1 ? 'Chỉnh sửa thành công' : 'Chỉnh sửa thất bại',
        confirmButtonText: 'Xác nhận',
        text: 'Chỉnh sửa ' + (res.status == 1 ? 'thành công' : 'thất bại'),
      });
      this.tradeFairOrganizationCertificationData = EMPTY_CUSTOM
    });
    this.subscriptions.push(sbUpdate);
  }

  create(item:any) {
    const sbCreate = this.tradeFairOrganizationCertificationService.create_Formdata(item).pipe(
      tap(() => {
        this.modal.close();
      }),
      catchError((errorMessage) => {
        this.modal.dismiss(errorMessage);
        return of(this.prepareTradeFairOrganizationCertification());
      }),
    ).subscribe((res: any) => {
      Swal.fire({
        icon: res.status == 1 ? 'success' : 'error',
        title: res.status == 1 ? 'Thêm mới thành công' : 'Thêm mới thất bại',
        confirmButtonText: 'Xác nhận',
        text: 'Thêm mới ' + (res.status == 1 ? 'thành công' : 'thất bại'),
      });
      this.tradeFairOrganizationCertificationData = EMPTY_CUSTOM
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

  // convert_date(string_date: string) {
  //   if (string_date) {
  //     var result = moment.utc(string_date, "DD/MM/YYYY");
  //     return result
  //   }
  //   return null
  // }

  // convert_date_string(string_date: string | null) {
  //   if (string_date) {
  //     var date = string_date.split("T")[0];
  //     var list = date.split("-"); //["year", "month", "day"]
  //     var result = list[2] + "/" + list[1] + "/" + list[0]
  //     return result
  //   }
  //   return null
  // }

  convert_datetime(date: string){
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

  //Upload File
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
    if (this.files[index].tradeFairOrganizationCertificationAttachFileId) {
      this.del_file_ids += this.files[index].tradeFairOrganizationCertificationAttachFileId + ','
      this.files.splice(index, 1);
    }
    else {
      this.files.splice(index, 1);
    }
  }

  /**
   * Simulate the upload process
   */
  // uploadFilesSimulator(index: number) {
  //   setTimeout(() => {
  //     if (index === this.files.length) {
  //       return;
  //     } else {
  //       const progressInterval = setInterval(() => {
  //         if (this.files.length === 0) {
  //           clearInterval(progressInterval);
  //         }
  //         else {
  //           if (this.files[index].progress === 100) {
  //             clearInterval(progressInterval);
  //             this.uploadFilesSimulator(index + 1);
  //           }
  //           else {
  //             this.files[index].progress += 5;
  //           }
  //         }
  //       }, 200);
  //     }
  //   }, 200);
  // }

  /**
   * Convert Files list to normal array list
   * @param files (Files List)
   */
  prepareFilesList(files: Array<any>) {
    for (const item of files) {
      // item.progress = 0;
      this.files.push(item);
    }
    // this.uploadFilesSimulator(0);
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
