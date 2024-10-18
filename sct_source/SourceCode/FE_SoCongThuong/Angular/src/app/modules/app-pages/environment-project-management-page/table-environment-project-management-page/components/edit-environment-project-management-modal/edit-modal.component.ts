import { ChangeDetectorRef, Component, Input, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal, NgbDateAdapter, NgbDateParserFormatter } from '@ng-bootstrap/ng-bootstrap';
import * as moment from 'moment';
import { of, Subscription } from 'rxjs';
import { catchError, finalize, first, tap } from 'rxjs/operators';
import { Options } from 'select2';
import Swal from 'sweetalert2';

import { EnvironmentProjectManagementModel } from '../../../_models/environment-project-management.model';
import { EnvironmentProjectManagementPageService } from '../../../_services/environment-project-management-page.service';

const EMPTY_CUSTOM: EnvironmentProjectManagementModel = {
  id: '',
  environmentProjectManagementId: '00000000-0000-0000-0000-000000000000',
  projectName : '',
  implementationContent : '',
  approvedFunding : null,
  implementationCost : null,
  coordinationUnit : '',
  yearOfImplementationFrom : moment().year(),
  yearOfImplementationTo : moment().year(),
};

@Component({
  selector: 'app-edit-modal',
  templateUrl: './edit-modal.component.html',
  styleUrls: ['./edit-modal.component.scss'],

})

export class EditEnvironmentProjectManagementModalComponent implements OnInit, OnDestroy {
  @Input() id: any;
  @Input() type: any;
  isLoading$: any;
  environmentProjectManagementData: EnvironmentProjectManagementModel;
  formGroup: FormGroup;
  options: Options;
  
  files: any[] = [];
  del_file_ids: string = ""

  private subscriptions: Subscription[] = [];

  yearRange: any = [
    {
      id: 0,
      text: '-- Chọn --'
    }
  ]

  constructor(
    private environmentProjectManagementService: EnvironmentProjectManagementPageService,
    private fb: FormBuilder, public modal: NgbActiveModal,
  ) { }

  ngOnInit(): void {
    this.isLoading$ = this.environmentProjectManagementService.isLoading$;
    this.loadEnvironmentProjectManagement();
    this.options = {
      theme: 'bootstrap5',
      templateSelection: this.templateSelection,
    };
    this.getYearsList();
  }

  public templateSelection = (state: any): JQuery | string => {
    if (!state.id) {
      return state.text;
    }
    return jQuery('<span class="form-select form-select-solid form-select-lg">' + state.text + '</span>');
  }

  getYearsList() {
    const currentYear = new Date().getFullYear();
    const yearsList: any = [{ id: 0, text: "-- Chọn --" }];

    for (let i = -10; i <= 10; i++) {
      const year = currentYear + i;
      yearsList.push({ id: year, text: year });
    }

    this.yearRange = yearsList;
  }

  loadEnvironmentProjectManagement() {
    if (!this.id) {
      this.clear();
      this.loadForm();
    } else {
      const sb = this.environmentProjectManagementService.getItemById(this.id).pipe(
        first(),
        catchError((errorMessage) => {
          this.modal.dismiss(errorMessage);
          return of(EMPTY_CUSTOM);
        })
      ).subscribe((res: any) => {
        this.environmentProjectManagementData = res.data;
        this.environmentProjectManagementData.approvedFunding = this.f_currency(res.data.approvedFunding);
        this.environmentProjectManagementData.implementationCost = this.f_currency(res.data.implementationCost);
        this.files = res.data.fileUpload;
        this.loadForm();
        if (this.type) {
          this.formGroup.disable()
        }
      });
      this.subscriptions.push(sb);
    }
  }

  loadForm() {
    this.formGroup = this.fb.group({
      ProjectName: [this.environmentProjectManagementData.projectName, Validators.required],
      ImplementationContent: [this.environmentProjectManagementData.implementationContent, Validators.required],
      ApprovedFunding: [this.environmentProjectManagementData.approvedFunding, Validators.required],
      ImplementationCost: [this.environmentProjectManagementData.implementationCost, Validators.required],
      CoordinationUnit: [this.environmentProjectManagementData.coordinationUnit, Validators.required],
      YearOfImplementationFrom: [this.environmentProjectManagementData.yearOfImplementationFrom, Validators.required],
      YearOfImplementationTo: [this.environmentProjectManagementData.yearOfImplementationTo, Validators.required],
    });
    // this.subscriptions.push(
    //   this.formGroup.controls.ApprovedFunding.valueChanges.subscribe((x) => {
    //     this.formGroup.patchValue({
    //       "ApprovedFunding": this.f_currency(x)
    //     }, { emitEvent: false })
    //   })
    // );
    // this.subscriptions.push(
    //   this.formGroup.controls.ImplementationCost.valueChanges.subscribe((x) => {
    //     this.formGroup.patchValue({
    //       "ImplementationCost": this.f_currency(x)
    //     }, { emitEvent: false })
    //   })
    // );
  }

  clear() {
    EMPTY_CUSTOM.environmentProjectManagementId = '00000000-0000-0000-0000-000000000000',
    EMPTY_CUSTOM.projectName = '',
    EMPTY_CUSTOM.implementationContent = '',
    EMPTY_CUSTOM.approvedFunding = null,
    EMPTY_CUSTOM.implementationCost = null,
    EMPTY_CUSTOM.coordinationUnit = '',
    EMPTY_CUSTOM.yearOfImplementationFrom = moment().year(),
    EMPTY_CUSTOM.yearOfImplementationTo = moment().year(),
    this.environmentProjectManagementData = EMPTY_CUSTOM;
  }

  private prepareEnvironmentProjectManagement() {
    var formValue = this.formGroup.value;
    var formData: any = new FormData();
    formData.append("ProjectName", formValue.ProjectName)
    formData.append("ImplementationContent", formValue.ImplementationContent)
    formData.append("ApprovedFunding", Number(formValue.ApprovedFunding.replaceAll(',' , '')))
    formData.append("ImplementationCost", Number(formValue.ImplementationCost.replaceAll(',' , '')))
    formData.append("CoordinationUnit", formValue.CoordinationUnit)
    formData.append("ImplementationCost", formValue.ImplementationCost)
    formData.append("YearOfImplementationFrom", formValue.YearOfImplementationFrom)
    formData.append("YearOfImplementationTo", formValue.YearOfImplementationTo)
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
    const model = this.prepareEnvironmentProjectManagement();
    if (this.id) {
      model.append("EnvironmentProjectManagementId", this.id)
      this.edit(model);
    } else {
      this.create(model);
    }
  }

  edit(item: any) {
    const sbUpdate = this.environmentProjectManagementService.updateFormData(item).pipe(
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
      this.environmentProjectManagementData = EMPTY_CUSTOM
    });
    this.subscriptions.push(sbUpdate);
  }

  create(item:any) {
    const sbCreate = this.environmentProjectManagementService.createFormData(item).pipe(
      tap(() => {
        this.modal.close();
      }),
      catchError((errorMessage) => {
        this.modal.dismiss(errorMessage);
        return of(this.prepareEnvironmentProjectManagement());
      }),
    ).subscribe((res: any) => {
      Swal.fire({
        icon: res.status == 1 ? 'success' : 'error',
        title: res.status == 1 ? 'Thêm mới thành công' : 'Thêm mới thất bại',
        confirmButtonText: 'Xác nhận',
        text: 'Thêm mới ' + (res.status == 1 ? 'thành công' : 'thất bại'),
      });
      this.environmentProjectManagementData = EMPTY_CUSTOM
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
    if (value == '00000000-0000-0000-0000-000000000000' || value == 0) {
      control.setErrors({ defaultvalue: true });
    }
    else {
      control.setErrors({ defaultvalue: null });
      control.updateValueAndValidity();
    }
    return control.invalid && (control.dirty || control.touched);
  }

  convert_date(string_date: string) {
    var result = moment.utc(string_date, "DD/MM/YYYY");
    return result
  }

  convert_date_string(string_date: string | null) {
    if (string_date) {
      var date = string_date.split("T")[0];
      var list = date.split("-"); //["year", "month", "day"]
      var result = list[2] + "/" + list[1] + "/" + list[0]
      return result
    }
    return null
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
    if (this.files[index].environmentProjectManagementAttachFileId) {
      this.del_file_ids += this.files[index].environmentProjectManagementAttachFileId + ','
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

  checkYear() {
    const control = this.formGroup.controls.YearOfImplementationTo;
    const From = this.formGroup.controls.YearOfImplementationFrom.value;
    const To = control.value;
    if (From > To) {
      control.setErrors({ FromTo: true });
    } else {
      control.setErrors({ FromTo: null });
      control.updateValueAndValidity();
    }
    return control.invalid && (control.dirty || control.touched);
  }
}
