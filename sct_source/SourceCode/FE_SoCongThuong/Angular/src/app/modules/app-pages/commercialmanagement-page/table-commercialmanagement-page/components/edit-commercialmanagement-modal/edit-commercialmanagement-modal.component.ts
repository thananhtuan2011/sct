import { ChangeDetectorRef, Component, Input, OnDestroy, OnInit, Output } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal, NgbDateAdapter, NgbDateParserFormatter } from '@ng-bootstrap/ng-bootstrap';
import { of, Subscription } from 'rxjs';
import { catchError, finalize, first, tap } from 'rxjs/operators';
import Swal from 'sweetalert2';

import { SelectOptionData } from 'src/app/_metronic/shared/components/select-custom/select-custom.interface';
import { Options } from 'select2';

import { CommercialManagementModel } from '../../../_models/commercialmanagement.model';
import { CommercialManagementPageService } from '../../../_services/commercialmanagement-page.service';
import { CommonService } from 'src/app/_metronic/shared/services/common.service';

const EMPTY_CUSTOM = {
  id: '',
  type: '00000000-0000-0000-0000-000000000000',
  code: '',
  name: '',
};

@Component({
  selector: 'app-edit-commercialmanagement-modal',
  templateUrl: './edit-commercialmanagement-modal.component.html',
  styleUrls: ['./edit-commercialmanagement-modal.component.scss'],
})

export class EditCommercialManagementModalComponent implements OnInit, OnDestroy {
  @Input() id: any;
  isLoading$: any;
  commercialmanagementData: any;
  formGroup: FormGroup;
  show: boolean = false;

  private subscriptions: Subscription[] = [];
  public options: Options;

  public typeData: Array<any>;
  public type: any = '';
  public typeid: any = '';
  public apiLoaded: number = 0;

  constructor(
    private commercialmanagementService: CommercialManagementPageService,
    private fb: FormBuilder,
    public modal: NgbActiveModal,
    private commonService: CommonService,
  ) { }

  ngOnInit(): void {
    this.isLoading$ = this.commercialmanagementService.isLoading$;
    this.loadConfigMarket();
    this.options = {
      theme: 'bootstrap5',
      templateSelection: this.templateSelection,
    };

    this.commercialmanagementService.check_form.subscribe(item => {
      if (item == 'check') {
        this.formGroup.markAllAsTouched()
        this.formGroup.updateValueAndValidity();
      };
    });
  }

  public templateSelection = (state: any): JQuery | string => {
    if (!state.id) {
      return state.text;
    }
    return jQuery('<span class="form-select form-select-solid form-select-lg">' + state.text + '</span>');
  }

  public loadConfigMarket() {
    const sub = this.commonService.GetConfig("MARKET").subscribe((res: any) => {
      const data = [
        ...res.items.listConfig.map((item: any) => ({
          id: item.categoryId,
          text: item.categoryName,
          typeCode: item.categoryTypeCode,
          code: item.categoryCode,
          priority: item.priority,
        }))
      ]
      this.typeData = data;
      this.loadMarket();
    })
    this.subscriptions.push(sub);
  }

  loadMarket() {
    if (!this.id) {
      this.clearmodel();
      this.loadForm();
      this.type = this.typeData.find(x => x.text == "Chợ")?.text
      this.typeid = this.typeData.find(x => x.text == "Chợ")?.id
    } else {
      const sb = this.commercialmanagementService.getItemById(this.id).pipe(
        first(),
        catchError((errorMessage) => {
          this.modal.dismiss(errorMessage);
          return of(EMPTY_CUSTOM);
        })
      ).subscribe((res: any) => {
        this.commercialmanagementData = res.items[0];
        this.loadForm();
        this.type = this.typeData.find(x => x.id == res.items[0].type)?.text
        this.typeid = this.typeData.find(x => x.id == res.items[0].type)?.id
      });
      this.subscriptions.push(sb);
    }
  }

  loadForm() {
    this.formGroup = this.fb.group({
      Type: [this.commercialmanagementData.type],
      Code: [this.commercialmanagementData.code, Validators.required],
      Name: [this.commercialmanagementData.name, Validators.required],
    });
    this.show = true;
  }

  clearmodel() {
    EMPTY_CUSTOM.type = this.typeData[0].id;
    EMPTY_CUSTOM.code = '';
    EMPTY_CUSTOM.name = '';
    this.commercialmanagementData = EMPTY_CUSTOM;
  }

  gettype(event: any) {
    this.type = this.typeData.find(x => x.id == event)?.text
    this.typeid = this.typeData.find(x => x.id == event)?.id
    return this.type, this.typeid
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
}