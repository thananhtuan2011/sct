import { ChangeDetectorRef, Component, Input, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal, NgbDateAdapter, NgbDateParserFormatter } from '@ng-bootstrap/ng-bootstrap';
import { of, Subscription } from 'rxjs';
import { catchError, finalize, first, tap } from 'rxjs/operators';
import Swal from 'sweetalert2';

import { TypeOfMarketModel } from '../../../_models/typeofmarket.model';
import { TypeOfMarketPageService } from '../../../_services/typeofmarket-page.service';

const EMPTY_CUSTOM: TypeOfMarketModel = {
  id: '',
  typeOfMarketId: '',
  typeOfMarketCode: '',
  typeOfMarketName: '',
};

@Component({
  selector: 'app-edit-typeofmarket-modal',
  templateUrl: './edit-typeofmarket-modal.component.html',
  styleUrls: ['./edit-typeofmarket-modal.component.scss'],

})
export class EditTypeOfMarketModalComponent implements OnInit, OnDestroy {
  @Input() id: any;
  isLoading$:any;
  typeofmarketData: TypeOfMarketModel;
  formGroup: FormGroup;

  private subscriptions: Subscription[] = [];
  public default_value = "00000000-0000-0000-0000-000000000000"
  
  constructor(
    private typeofmarketService: TypeOfMarketPageService,
    private fb: FormBuilder, public modal: NgbActiveModal,
    ) { }

  ngOnInit(): void {
    this.isLoading$ = this.typeofmarketService.isLoading$;
    this.loadMarket();
  }

  loadMarket() {
    if (!this.id) {
      EMPTY_CUSTOM.typeOfMarketCode='';
      EMPTY_CUSTOM.typeOfMarketName='';
      this.typeofmarketData = EMPTY_CUSTOM;
      this.loadForm();
    } else {
      const sb = this.typeofmarketService.getItemById(this.id).pipe(
        first(),
        catchError((errorMessage) => {
          this.modal.dismiss(errorMessage);
          return of(EMPTY_CUSTOM);
        })
      ).subscribe((res: any) => {
        this.typeofmarketData = res.items[0];
        this.loadForm();
      });
      this.subscriptions.push(sb);
    }
  }

  loadForm() {
    this.formGroup = this.fb.group({
      TypeOfMarketCode: [this.typeofmarketData.typeOfMarketCode, Validators.compose([Validators.required, Validators.pattern("[a-zA-Z0-9]{1,15}")])],
      TypeOfMarketName: [this.typeofmarketData.typeOfMarketName, Validators.required],
    });
  }

  save() {
    this.prepareTypeOfMarket();
    if (this.typeofmarketData.typeOfMarketId) {
      this.edit();
    } else {
      this.typeofmarketData.typeOfMarketId = "00000000-0000-0000-0000-000000000000"
      this.create();
    }
  }

  edit() {
    const sbUpdate = this.typeofmarketService.update(this.typeofmarketData).pipe(
      tap(() => {
        this.modal.close();
      }),
      catchError((errorMessage) => {
        this.modal.dismiss(errorMessage);
        return of(this.typeofmarketData);
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
    const sbCreate = this.typeofmarketService.create(this.typeofmarketData).pipe(
      tap(() => {
        this.modal.close();
      }),
      catchError((errorMessage) => {
        this.modal.dismiss(errorMessage);
        return of(this.typeofmarketData);
      }),
    ).subscribe((res: any) => {
      Swal.fire({
        icon: res.status == 1 ? 'success' : 'error',
        title: res.status == 1 ? 'Thêm mới thành công' : 'Thêm mới thất bại',
        confirmButtonText: 'Xác nhận',
        text: res.status == 0 ? res.error.msg : 'Thêm mới ' + (res.status == 1 ? 'thành công' : 'thất bại'),
      });
      this.typeofmarketData = EMPTY_CUSTOM
    });
    this.subscriptions.push(sbCreate);
    EMPTY_CUSTOM.typeOfMarketId='';
    EMPTY_CUSTOM.typeOfMarketCode='';
    EMPTY_CUSTOM.typeOfMarketName='';
    this.typeofmarketData = EMPTY_CUSTOM;
  }

  private prepareTypeOfMarket() {
    const formData = this.formGroup.value;
    this.typeofmarketData.typeOfMarketCode = formData.TypeOfMarketCode;
    this.typeofmarketData.typeOfMarketName = formData.TypeOfMarketName;
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

  check_formGroup() {
    if (this.formGroup.invalid) {
      this.formGroup.markAllAsTouched();
    }
    else {
      this.save();
    }
  }
}
