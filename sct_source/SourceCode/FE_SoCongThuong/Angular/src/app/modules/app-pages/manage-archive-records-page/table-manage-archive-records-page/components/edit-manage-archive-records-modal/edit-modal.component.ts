import { Component, Input, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { of, Subscription } from 'rxjs';
import { catchError, first, tap } from 'rxjs/operators';
import Swal from 'sweetalert2';
import { ManageArchiveRecordsModel } from '../../../_models/manage-archive-records-page.model';
import { ManageArchiveRecordsPageService } from '../../../_services/manage-archive-records-page.service';
import { Options } from 'select2';
import * as moment from 'moment';

@Component({
  selector: 'app-edit-modal',
  templateUrl: './edit-modal.component.html',
  styleUrls: ['./edit-modal.component.scss'],
})

export class EditManageArchiveRecordsModalComponent implements OnInit, OnDestroy {
  @Input() id: any;
  @Input() view: any;
  isLoading$: any;
  options: Options;
  data: ManageArchiveRecordsModel;
  formGroup: FormGroup;
  dataCreator: any = [];
  profileGroup: any = [
    {
      id: 0,
      text: "-- Chọn --"
    },
    {
      id: 1,
      text: "An toàn thực phẩm"
    },
    {
      id: 2,
      text: "Bảo vệ môi trường"
    },
    {
      id: 3,
      text: "An toàn hóa chất"
    },
    {
      id: 4,
      text: "Công tác phòng chống cháy nổ"
    },
    {
      id: 5,
      text: "Lĩnh vực kinh doanh khí"
    },
  ];
  show: boolean = false;
  private subscriptions: Subscription[] = [];

  constructor(
    public pageService: ManageArchiveRecordsPageService,
    private fb: FormBuilder,
    public modal: NgbActiveModal
  ) { }

  ngOnInit(): void {
    this.isLoading$ = this.pageService.isLoading$;
    this.loadData();
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

  loadData() {
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
        this.data = res.items[0] as ManageArchiveRecordsModel;
        if(this.data.creator.length > 0) {
          this.dataCreator = this.data.creator.split(',');
        }
        this.loadForm();
        if (this.view) {
          this.formGroup.disable();
        }
      });
      this.subscriptions.push(sb);
    }
  }

  clear() {
    const EmptyModel = {
      id: '',
      manageArchiveRecordsId: '00000000-0000-0000-0000-000000000000',
      recordsFinancePlanId: 0,
      codeFile: '',
      title: '',
      receptionTime: null,
      storageTime: 0,
      creator: '',
      note: '',
    } as ManageArchiveRecordsModel;
    this.data = EmptyModel;
    return EmptyModel;
  }

  loadForm() {
    this.formGroup = this.fb.group({
      RecordsGroup: [this.data.recordsFinancePlanId],
      CodeFile: [this.data.codeFile, Validators.required],
      Title: [this.data.title, Validators.required],
      ReceptionTime: [this.data.receptionTime != null ? this.convert_date_string(this.data.receptionTime) : '', Validators.required],
      StorageTime: [this.data.storageTime, Validators.required],
      Location: [this.data.location, Validators.required],
      StoreDocumentsAt: [this.data.storeDocumentsAt, Validators.required],
      StoreFilesAt: [this.data.storeFilesAt, Validators.required],
      CreatorX: ['', Validators.required],
      Note: this.data.note
    });

    this.show = true;
  }

  save() {
    this.prepareData();
    if (this.id) {
      this.edit();
    } else {
      this.create();
    }
  }

  edit() {
    const sbUpdate = this.pageService.update(this.data).pipe(
      tap(() => {
        this.modal.close();
      }),
      catchError((errorMessage) => {
        this.modal.dismiss(errorMessage);
        return of(this.data);
      }),
    ).subscribe((res: any) => {
      Swal.fire({
        icon: res.status == 1 ? 'success' : 'error',
        title: res.status == 1 ? 'Chỉnh sửa thành công' : 'Chỉnh sửa thất bại',
        confirmButtonText: 'Xác nhận',
        text: (res.status == 1 ? 'Chỉnh sửa thành công' : res.status == 0 ? res.error.msg : "Chỉnh sửa thất bại"),
      });
    });
    this.subscriptions.push(sbUpdate);
  }

  create() {
    const sbCreate = this.pageService.create(this.data).pipe(
      tap(() => {
        this.modal.close();
      }),
      catchError((errorMessage) => {
        this.modal.dismiss(errorMessage);
        return of(this.data);
      }),
    ).subscribe((res: any) => {
      Swal.fire({
        icon: res.status == 1 ? 'success' : 'error',
        title: res.status == 1 ? 'Thêm mới thành công' : 'Thêm mới thất bại',
        confirmButtonText: 'Xác nhận',
        text: (res.status == 1 ? 'Thêm mới thành công' : res.status == 0 ? res.error.msg : 'Thêm mới thất bại'),
      });
    });
    this.subscriptions.push(sbCreate);
  }

  private prepareData() {
    let creator: string;
    if (this.dataCreator.length > 0 && this.formGroup.controls.CreatorX.value !== '') {
      creator = `${this.dataCreator.toString()},${this.formGroup.controls.CreatorX.value}`
    } else if (this.dataCreator.length > 0) {
      creator = `${this.dataCreator.toString()}`
    } else {
      creator = `${this.formGroup.controls.CreatorX.value}`
    }
    const formData = this.formGroup.value;
    this.data.recordsFinancePlanId = formData.RecordsGroup;
    this.data.codeFile = formData.CodeFile;
    this.data.title = formData.Title;
    this.data.receptionTime = this.convert_date(formData.ReceptionTime);
    this.data.note = formData.Note;
    this.data.creator = creator;
    this.data.storageTime = formData.StorageTime;
    this.data.location = formData.Location;
    this.data.storeDocumentsAt = formData.StoreDocumentsAt;
    this.data.storeFilesAt = formData.StoreFilesAt;
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

  isDefaultValue(controlName: any): boolean {
    const control = this.formGroup.controls[controlName];
    if (control.value == '' || control.value == '00000000-0000-0000-0000-000000000000') {
      control.setErrors({ 'default': true })
    } else {
      control.setErrors(null)
    }
    return control.hasError('default') && (control.dirty || control.touched);
  }

  check_formGroup() {
    if (this.dataCreator.length > 0) {
      this.formGroup.controls.CreatorX.clearValidators();
      this.formGroup.controls.CreatorX.updateValueAndValidity();
    }
    if (this.formGroup.invalid) {
      this.formGroup.markAllAsTouched();
      this.formGroup.updateValueAndValidity();
    }
    else {
      this.save()
    }
  }

  addCreator() {
    const formData = this.formGroup.controls.CreatorX.value;
    if (formData.trim() !== '') {
      this.dataCreator.push(formData)
      this.formGroup.controls.CreatorX.reset('')
      this.formGroup.controls.CreatorX.clearValidators();
      this.formGroup.controls.CreatorX.updateValueAndValidity();
    }
  }

  changeCreator(event: any, index: number) {
    this.dataCreator[index] = event.target.value;
    if(event.target.value == ''){
      this.dataCreator.splice(index,1)
    }
    if (this.dataCreator.length == 0) {
      this.formGroup.controls.CreatorX.addValidators(Validators.required)
      this.formGroup.controls.CreatorX.updateValueAndValidity();
      this.formGroup.controls.CreatorX.markAsTouched()
      this.formGroup.controls.CreatorX.updateValueAndValidity();
    }
  }

  deleteCreator(value: any) {
    this.dataCreator = this.dataCreator.filter((x: any) => x != value)
    if (this.dataCreator.length == 0) {
      this.formGroup.controls.CreatorX.addValidators(Validators.required)
      this.formGroup.controls.CreatorX.updateValueAndValidity();
      this.formGroup.controls.CreatorX.markAsTouched()
      this.formGroup.controls.CreatorX.updateValueAndValidity();
    }
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
}
