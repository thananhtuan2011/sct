import { ChangeDetectorRef, Component, Input, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal, NgbDateAdapter, NgbDateParserFormatter } from '@ng-bootstrap/ng-bootstrap';
import { of, Subscription } from 'rxjs';
import { catchError, finalize, first, tap } from 'rxjs/operators';
import { SelectOptionData } from 'src/app/_metronic/shared/components/select-custom/select-custom.interface';
import Swal from 'sweetalert2';
import { Options } from 'select2';
import { CateCriteriaPageService } from '../../../_services/cate-criteria-page.service';
import { CateCriteriaModel } from '../../../_models/cate-criteria.model';

const EMPTY_CUSTOM: CateCriteriaModel = {
  id: '',
  CateCriteriaId: '',
  cateCriteriaName: '',
};

@Component({
  selector: 'app-edit-cate-criteria-modal.component',
  templateUrl: './edit-cate-criteria-modal.component.html',
  styleUrls: ['./edit-cate-criteria-modal.component.scss'],

})
export class EditCateCriteriaModalComponent implements OnInit, OnDestroy {
  @Input() id: any;
  isLoading$:any;
  cateCriteriData: CateCriteriaModel;
  formGroup: FormGroup;
  public options: Options;
  private subscriptions: Subscription[] = [];
  public default_value = "00000000-0000-0000-0000-000000000000"
  public typeofbusinessData: Array<SelectOptionData>;

  constructor(
    private cateCriteriaPageService: CateCriteriaPageService,
    private fb: FormBuilder, public modal: NgbActiveModal,
    private changeDetectorRefs: ChangeDetectorRef

    ) { }

  ngOnInit(): void {
    this.isLoading$ = this.cateCriteriaPageService.isLoading$;
    (async () => { 
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
      const sb = this.cateCriteriaPageService.getItemById(this.id).pipe(
        first(),
        catchError((errorMessage) => {
          this.modal.dismiss(errorMessage);
          return of(EMPTY_CUSTOM);
        })
      ).subscribe((res: any) => {
        this.cateCriteriData = res.items[0];
        this.loadForm();
      });
      this.subscriptions.push(sb);
    }
  }

  loadForm() {
    this.formGroup = this.fb.group({
      cateCriteriaName: [this.cateCriteriData.cateCriteriaName, Validators.compose([Validators.required ])],
    });
  }
clear_model() {
    EMPTY_CUSTOM.CateCriteriaId = '',
    EMPTY_CUSTOM.cateCriteriaName = '',
    this.cateCriteriData = EMPTY_CUSTOM
  }
  save() {
    this.prepare();
    if (this.cateCriteriData.CateCriteriaId!= '') {
      this.edit();
    } else {
      this.cateCriteriData.CateCriteriaId = this.default_value
      this.create();
    }
  }

  edit() {
    const sbUpdate = this.cateCriteriaPageService.update(this.cateCriteriData).pipe(
      tap(() => {
        this.modal.close();
      }),
      catchError((errorMessage) => {
        this.modal.dismiss(errorMessage);
        return of(this.cateCriteriData);
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
    const sbCreate = this.cateCriteriaPageService.create(this.cateCriteriData).pipe(
      tap(() => {
        this.modal.close();
      }),
      catchError((errorMessage) => {
        this.modal.dismiss(errorMessage);
        return of(this.cateCriteriData);
      }),
    ).subscribe((res: any) => {
      Swal.fire({
        icon: res.status == 1 ? 'success' : 'error',
        title: res.status == 1 ? 'Thêm mới thành công' : 'Thêm mới thất bại',
        confirmButtonText: 'Xác nhận',
        text: 'Thêm mới ' + (res.status == 1 ? 'thành công' : 'thất bại'),
      });
      this.cateCriteriData = EMPTY_CUSTOM
    });
    this.subscriptions.push(sbCreate);
    EMPTY_CUSTOM.CateCriteriaId='';
    EMPTY_CUSTOM.cateCriteriaName='';
    this.cateCriteriData = EMPTY_CUSTOM;
  }

  private prepare() {
    const formData = this.formGroup.value;
    this.cateCriteriData.cateCriteriaName = formData.cateCriteriaName;
   
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

  check_formGroup() {
    if (this.formGroup.invalid) {
      this.formGroup.markAllAsTouched();
    }
    else {
      this.save();
    }
  }
}
