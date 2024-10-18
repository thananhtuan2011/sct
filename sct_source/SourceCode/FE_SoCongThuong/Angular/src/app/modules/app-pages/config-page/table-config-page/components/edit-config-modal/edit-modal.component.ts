import { ChangeDetectorRef, Component, Input, OnDestroy, OnInit } from '@angular/core';
import { FormArray, FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal, NgbDateAdapter, NgbDateParserFormatter } from '@ng-bootstrap/ng-bootstrap';
import { of, Subscription } from 'rxjs';
import { catchError, finalize, first, tap } from 'rxjs/operators';
import { Options } from 'select2';
import Swal from 'sweetalert2';

import { ConfigDefault, ConfigModel, ListConfigModel } from '../../../_models/config.model';
import { ConfigPageService } from '../../../_services/config-page.service';

@Component({
  selector: 'app-edit-modal',
  templateUrl: './edit-modal.component.html',
  styleUrls: ['./edit-modal.component.scss'],
})

export class EditConfigModalComponent implements OnInit, OnDestroy {
  @Input() id: any;
  @Input() typeCode: any;
  @Input() title: any;
  isLoading$: any;

  data: ListConfigModel = { id: 0, ListConfig: [] };
  dataDefault: ConfigDefault;

  formGroup: FormGroup;
  options: Options;

  ListConfig: any = [];
  show: boolean = false;

  private subscriptions: Subscription[] = [];

  constructor(
    private pageService: ConfigPageService,
    private fb: FormBuilder,
    public modal: NgbActiveModal,
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
    const sb = this.pageService.getItemById(this.id).pipe(
      first(),
      catchError((errorMessage) => {
        this.modal.dismiss(errorMessage);
        return of([this.dataDefault.Config]);
      })
    ).subscribe((res: any) => {
      if (res.items.listConfig != null) {
        this.data.ListConfig = res.items.listConfig;

        // Tạo array từ dữ liệu API
        this.ListConfig = res.items.listConfig.map((element: any) => ({
          CategoryId: element.categoryId,
          CategoryCode: element.categoryCode,
          CategoryName: element.categoryName,
          CategoryTypeCode: element.categoryTypeCode,
          Priority: element.priority,
          IsAction: element.isAction,
          IsDel: !element.isAction,
        })).sort((i1: any, i2: any) => i1.Priority - i2.Priority);
  
        this.loadForm();
  
        //Thêm các FormGroup vào FormArray
        if (this.ListConfig.length > 1) {
          for (var i = 1; i < this.ListConfig.length; i++) {
            this.addConfig()
          }
        }
  
        //Gán dữ liệu API vào FormArray
        this.formGroup.controls.ListConfig.setValue(this.ListConfig)
      } else {
        this.loadForm();
      }

      this.show = true;
    });
    this.subscriptions.push(sb);
  }

  loadForm() {
    this.formGroup = this.fb.group({
      ListConfig: this.fb.array([this.fb.group({
        CategoryId: ['00000000-0000-0000-0000-000000000000', Validators.required],
        CategoryCode: ['', Validators.required],
        CategoryName: ['', Validators.required],
        CategoryTypeCode: [this.typeCode, Validators.required],
        Priority: [1, Validators.required],
        IsAction: [true, Validators.required],
        IsDel: [false, Validators.required]
      })]),
    });
  }

  //Thao tác với FormArray
  // nhận FormArray
  get GetConfig(): FormArray {
    return this.formGroup.controls.ListConfig as FormArray;
  }

  //Thêm FormGroup vào FormArray
  addConfig() {
    this.GetConfig.push(this.fb.group({
      CategoryId: ['00000000-0000-0000-0000-000000000000', Validators.required],
      CategoryCode: ['', Validators.required],
      CategoryName: ['', Validators.required],
      CategoryTypeCode: [this.typeCode, Validators.required],
      Priority: [1, Validators.required],
      IsAction: [true, Validators.required],
      IsDel: [false, Validators.required]
    }))
  }

  //Xoá FormGroup ra khỏi FormArray
  delConfig(index: any) {
    this.GetConfig.removeAt(index)
  }

  //Thêm Value cho control Step
  setStepValue(index: any) {
    const control = this.GetConfig.controls[index].get('Priority');
    control?.setValue(index + 1)
    return index + 1
  }

  // Check Validation
  arrayControlHasError(validation: any, controlName: any, index: any): boolean {
    const control = this.GetConfig.controls[index].get(controlName);
    if (control) {
      return control.hasError(validation) && (control.dirty || control.touched);
    }
    else {
      return false
    }
  }

  private prepareData() {
    const formData = this.formGroup.value;
    this.data.id = this.id;
    this.data.ListConfig = formData.ListConfig;
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
        text: res.status == 0 ? res.error.msg : 'Chỉnh sửa ' + (res.status == 1 ? 'thành công' : 'thất bại'),
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
        text: res.status == 0 ? res.error.msg : 'Thêm mới ' + (res.status == 1 ? 'thành công' : 'thất bại'),
      });
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

  check_formGroup() {
    if (this.formGroup.invalid) {
      this.formGroup.markAllAsTouched();
    }
    else {
      Swal.fire({
        title: 'Bạn có chắc muốn lưu cấu hình này không?',
        text: 'Hành động này không thể hoàn tác và có thể ảnh hướng đến trang sử dụng cấu hình!',
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Xác nhận',
        cancelButtonText: 'Thoát'
      }).then((result) => {
        if (result.isConfirmed) {
          this.save();
        }
      })
    }
  }
}
