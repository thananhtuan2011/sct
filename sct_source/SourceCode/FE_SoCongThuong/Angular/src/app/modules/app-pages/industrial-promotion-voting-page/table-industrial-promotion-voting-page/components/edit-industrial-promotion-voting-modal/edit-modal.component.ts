import { ChangeDetectorRef, Component, Input, OnDestroy, OnInit } from '@angular/core';
import { AbstractControl, FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal, NgbDateAdapter, NgbDateParserFormatter } from '@ng-bootstrap/ng-bootstrap';
import { of, Subscription } from 'rxjs';
import { catchError, finalize, first, tap } from 'rxjs/operators';
import Swal from 'sweetalert2';
import { ResultsIndustrialPromotionVotingModel } from '../../../_models/results-industrial-promotion-voting.model';
import { ResultsIndustrialPromotionVotingPageService } from '../../../_services/results-industrial-promotion-voting-page.service';

import { SelectOptionData } from 'src/app/_metronic/shared/components/select-custom/select-custom.interface';
import { Options } from 'select2';
import { data } from 'jquery';

const EMPTY_CUSTOM: ResultsIndustrialPromotionVotingModel = {
  id: '',
  resultsIndustrialPromotionVotingId: '',
  locallity:null,
  numbersRegister: null,
  numberCertified: null,
  targets: '',
  unit: '',
};

@Component({
  selector: 'app-edit-modal',
  templateUrl: './edit-modal.component.html',
  styleUrls: ['./edit-modal.component.scss'],

})
export class EditResultsIndustrialPromotionVotingModalComponent implements OnInit, OnDestroy {
  @Input() id: any;
  isLoading$:any;
  resultsIndustrialPromotionVotingData: ResultsIndustrialPromotionVotingModel;
  formGroup: FormGroup;


  private subscriptions: Subscription[] = [];

  public selectData: any = [] ;
  public options: Options;


  constructor(
    private resultsIndustrialPromotionVoting: ResultsIndustrialPromotionVotingPageService,
    private fb: FormBuilder, public modal: NgbActiveModal,
    ) { }


  ngOnInit(): void {
    this.isLoading$ = this.resultsIndustrialPromotionVoting.isLoading$;

    (async () => {
      this.loadTargets()
      //Đợi load data 100ms
      await this.delay(100);
      this.loadIndustryPromotionReport();
      //this.loadIndustryPromotionReport();
    })();

    this.options = {
      theme:'bootstrap5',
      templateSelection: this.templateSelection,
    };
  }

  public templateSelection = (state: any): JQuery | string => {
    if (!state.id) {
      return state.text;
    }
    return jQuery('<span class="form-select form-select-solid form-select-lg">'+ state.text + '</span>');
  }

  delay(ms: number) {
    return new Promise( resolve => setTimeout(resolve, ms) );
  }

  loadTargets()
  {
    this.resultsIndustrialPromotionVoting.loadTargets().subscribe(res=>{
      const data = [
        {
          id: "00000000-0000-0000-0000-000000000000",
          text: '-- Chọn --',
          piority:0
        }
      ]
      for(var targets of res.items) {
        let obj = {
          id: targets.categoryId,
          text: targets.categoryName,
          piority: targets.piority
        }
        data.push(obj)
      }
      this.selectData = data.sort((i1, i2) => {
        if (i1.piority > i2.piority) {
          return 1;
        }
        if (i1.piority < i2.piority) {
          return -1;
        }
        return 0;
      });
      }
    );
  }

  loadIndustryPromotionReport() {
    if (!this.id) {
      this.clear();
      this.loadForm();
    } else {
      const sb = this.resultsIndustrialPromotionVoting.getItemById(this.id).pipe(
        first(),
        catchError((errorMessage) => {
          this.modal.dismiss(errorMessage);
          return of(EMPTY_CUSTOM);
        })
      ).subscribe((res: any) => {
        this.resultsIndustrialPromotionVotingData = res.data;
        this.loadForm();
      });
      this.subscriptions.push(sb);
    }
  }

  loadForm() {
    this.formGroup = this.fb.group({
      NumbersRegister: [this.resultsIndustrialPromotionVotingData.numbersRegister, Validators.required],  //,Validators.minLength(3), Validators.maxLength(100)
      NumberCertified: [this.resultsIndustrialPromotionVotingData.numberCertified, Validators.required],  //Validators.minLength(3), Validators.maxLength(100)
      Targets: [this.resultsIndustrialPromotionVotingData.targets],
      Unit: [this.resultsIndustrialPromotionVotingData.unit],
      Locallity: [this.resultsIndustrialPromotionVotingData.locallity,  Validators.required],
    });
  }

  save() {
    this.prepareCommune();
    if (this.id) {
      this.edit();
    } else {
      this.create();
    }
  }
  prenventInputNonNumber(event: any) {
    if (event.which < 48 || event.which > 57) {
      event.preventDefault();
    }
  }
  edit() {
    const sbUpdate = this.resultsIndustrialPromotionVoting.update(this.resultsIndustrialPromotionVotingData).pipe(
      tap(() => {
        this.modal.close();
      }),
      catchError((errorMessage) => {
        this.modal.dismiss(errorMessage);
        return of(this.resultsIndustrialPromotionVotingData);
      }),
    ).subscribe((res: any) => {
      Swal.fire({
        icon: res.status == 1 ? 'success' : 'error',
        title: res.status == 1 ? 'Chỉnh sửa thành công' : 'Chỉnh sửa thất bại',
        confirmButtonText: 'Xác nhận',
        text: res.status == 1 ? 'Chỉnh sửa thành công' : res.status == 0 ? res.error.msg:  'Chỉnh sửa thất bại',
      });
    });
    this.subscriptions.push(sbUpdate);
  }

  create() {
    const sbCreate = this.resultsIndustrialPromotionVoting.create(this.resultsIndustrialPromotionVotingData).pipe(
      tap(() => {
        this.modal.close();
      }),
      catchError((errorMessage) => {
        this.modal.dismiss(errorMessage);
        return of(this.resultsIndustrialPromotionVotingData);
      }),
    ).subscribe((res: any) => {
      Swal.fire({
        icon: res.status == 1 ? 'success' : 'error',
        title: res.status == 1 ? 'Thêm mới thành công' : 'Thêm mới thất bại',
        confirmButtonText: 'Xác nhận',
        text: res.status == 1 ? 'Thêm mới thành công' : res.status == 0 ? res.error.msg : 'Thêm mới thất bại',
      });
      this.resultsIndustrialPromotionVotingData = EMPTY_CUSTOM
    });
    this.subscriptions.push(sbCreate);

  }

  private prepareCommune() {
    const formData = this.formGroup.value;
    this.resultsIndustrialPromotionVotingData.numbersRegister = Number(formData.NumbersRegister);
    this.resultsIndustrialPromotionVotingData.numberCertified = Number(formData.NumberCertified);
    this.resultsIndustrialPromotionVotingData.targets = formData.Targets;
    this.resultsIndustrialPromotionVotingData.unit = formData.Unit;
    this.resultsIndustrialPromotionVotingData.locallity = formData.Locallity;
  }

  ngOnDestroy(): void {
    this.subscriptions.forEach(sb => sb.unsubscribe());
  }

  clear() {
    EMPTY_CUSTOM.resultsIndustrialPromotionVotingId = '00000000-0000-0000-0000-000000000000',
    EMPTY_CUSTOM.numbersRegister = null,
    EMPTY_CUSTOM.numberCertified = null,
    EMPTY_CUSTOM.locallity=null,
    EMPTY_CUSTOM.unit = '',
    EMPTY_CUSTOM.targets = '00000000-0000-0000-0000-000000000000',
    this.resultsIndustrialPromotionVotingData = EMPTY_CUSTOM;
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
    if (value == '00000000-0000-0000-0000-000000000000' || value == 0 || value == null) {
      control.setErrors({ defaultvalue: true });
    }
    else {
      control.setErrors({ defaultvalue: null });
      control.updateValueAndValidity();
    }
    return control.invalid && (control.dirty || control.touched);
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
}
