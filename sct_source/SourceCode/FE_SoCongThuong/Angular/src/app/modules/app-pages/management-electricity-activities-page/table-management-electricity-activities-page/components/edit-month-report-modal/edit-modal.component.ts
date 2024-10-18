import { ChangeDetectorRef, Component, Input, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { of, Subscription } from 'rxjs';
import { catchError, first, tap } from 'rxjs/operators';
import Swal from 'sweetalert2';

import { MonthReportModel } from '../../../_models/month-report.model';
import { MonthReportPageService } from '../../../_services/month-report-page.service';
import { Options } from 'select2';
import * as moment from 'moment';

@Component({
  selector: 'app-edit-month-report-modal',
  templateUrl: './edit-modal.component.html',
  styleUrls: ['./edit-modal.component.scss'],
})

export class EditMonthReportModalComponent implements OnInit, OnDestroy {
  @Input() id: any;
  @Input() type: any;
  private subscriptions: Subscription[] = [];
  isLoading$: any;
  options: Options;
  editData: MonthReportModel;
  formGroup: FormGroup;
  monthInit: string = '';
  show: boolean = false;

  files: any[] = [];
  delFileIds: string = "";

  constructor(
    private service: MonthReportPageService,
    private fb: FormBuilder,
    public modal: NgbActiveModal,
    public cdr: ChangeDetectorRef,
  ) { }

  ngOnInit(): void {
    this.isLoading$ = this.service.isLoading$;
    this.options = {
      theme: 'bootstrap5',
      templateSelection: this.templateSelection,
    };
    this.loadData();
  }

  public templateSelection = (state: any): JQuery | string => {
    if (!state.id) {
      return state.text;
    }
    return jQuery('<span class="form-select form-select-solid form-select-lg">' + state.text + '</span>');
  }

  loadData() {
    if (!this.id) {
      this.clear();
      this.loadForm();
    } else {
      const sb = this.service.getItemById(this.id).pipe(
        first(),
        catchError((errorMessage) => {
          this.modal.dismiss(errorMessage);
          return of(this.clear());
        })
      ).subscribe((res: any) => {
        this.files = res.items[0].allFile;
        this.editData = res.items[0] as MonthReportModel;
        this.monthInit = `${res.items[0].year}-${String(res.items[0].month).padStart(2, "0")}`
        this.loadForm();

        if (this.type) {
          this.formGroup.disable();
        }
      });
      this.subscriptions.push(sb);
    }
  }

  clear() {
    const EmptyModel = {
      monthReportId: '00000000-0000-0000-0000-000000000000',
      updateDate: '',
      month: '',
      year: '',
      lFiles: [],
    } as MonthReportModel;
    this.editData = EmptyModel;
    return EmptyModel;
  }

  loadForm() {
    this.formGroup = this.fb.group({
      UpdateDate: [this.editData.updateDate, Validators.required],
      Month: [this.monthInit, Validators.required],
    })

    this.show = true;
  }

  private prepare() {
    var formValue = this.formGroup.value;
    var formData: any = new FormData();
    formData.append("UpdateDate", formValue.UpdateDate);
    var MonthYear = formValue.Month.split('-');
    formData.append("Month", MonthYear[1]);
    formData.append("Year", MonthYear[0]);
    if (this.files.length > 0) {
      let i = 1;
      for (var file of this.files) {
        if (file.name) {
          formData.append("Files" + i, file, file.name);
          i++;
        }
      }
    };
    if (this.delFileIds.length > 0) {
      formData.append("DelFileIds", this.delFileIds)
    };
    return formData;
  }

  save() {
    const model = this.prepare();
    if (!!this.id) {
      model.append("MonthReportId", this.id)
      this.edit(model);
    } else {
      this.create(model);
    }
  }

  edit(items: any) {
    const sbUpdate = this.service.updateformdata(items).pipe(
      tap(() => {
        this.modal.close();
      }),
      catchError((errorMessage) => {
        this.modal.dismiss(errorMessage);
        return of(items);
      }),
    ).subscribe((res: any) => {
      Swal.fire({
        icon: res.status == 1 ? 'success' : 'error',
        title: res.status == 1 ? 'Chỉnh sửa thành công' : 'Chỉnh sửa thất bại',
        confirmButtonText: 'Xác nhận',
        text: res.status == 0 ? res.error.msg : 'Chỉnh sửa ' + (res.status == 1 ? 'thành công' : 'thất bại'),
      });
    });
    this.subscriptions.push(sbUpdate);
  }

  create(items: any) {
    const sbCreate = this.service.createformdata(items).pipe(
      tap(() => {
        this.modal.close();
      }),
      catchError((errorMessage) => {
        this.modal.dismiss(errorMessage);
        return of(items);
      }),
    ).subscribe((res: any) => {
      Swal.fire({
        icon: res.status == 1 ? 'success' : 'error',
        title: res.status == 1 ? 'Thêm mới thành công' : 'Thêm mới thất bại',
        confirmButtonText: 'Xác nhận',
        text: res.status == 0 ? res.error.msg : 'Thêm mới ' + (res.status == 1 ? 'thành công' : 'thất bại'),
      });
      this.clear();
    });
    this.subscriptions.push(sbCreate);
  }

  ngOnDestroy(): void {
    this.subscriptions.forEach(sb => sb.unsubscribe());
  }

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
      return control.invalid && (control.touched || control.dirty);
    }
    else {
      control.setErrors(null);
      return false;
    }
  }

  //Upload File
  @ViewChild('fileDropRef') fileDropRef: any;

  /** on file drop handler */
  onFileDropped($event: any) {
    this.prepareFilesList($event);
  }

  /** handle file from browsing */
  fileBrowseHandler(files: any) {
    this.prepareFilesList(files.target.files);
  }

  /** 
   * Delete file from files list
   * @param index (File index) 
   */
  deleteFile(index: number) {
    this.fileDropRef.nativeElement.value = '';
    if (this.files[index].fileId) {
      this.delFileIds += this.files[index].fileId + ','
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
    this.formGroup.markAllAsTouched();
    if (!this.formGroup.invalid) {
      this.save()
    }
  }

  downloadAllFiles() {
    let LFile = [...this.files];
    var interval = setInterval(download, 300, LFile);
    
    function download(urls: any) {
      let url = LFile.pop().linkFile;
      let a = document.createElement("a");
      a.setAttribute('href', url);
      a.click();
      if (urls.length == 0) {
        clearInterval(interval);
      }
    }
  }

  getCreateTime(time: string) {
    if (!!time) {
      return time;
    } else {
      return moment().format('DD/MM/YYYY');
    }
  }
}
