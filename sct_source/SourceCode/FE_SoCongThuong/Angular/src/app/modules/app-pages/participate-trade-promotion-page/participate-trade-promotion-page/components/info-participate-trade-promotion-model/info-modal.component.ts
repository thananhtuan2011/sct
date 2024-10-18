import { ChangeDetectorRef, Component, Input, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal, NgbDateAdapter, NgbDateParserFormatter } from '@ng-bootstrap/ng-bootstrap';
import { Observable, of, Subscription } from 'rxjs';
import { catchError, finalize, first, tap } from 'rxjs/operators';
import { SelectOptionData } from 'src/app/_metronic/shared/components/select-custom/select-custom.interface';
import Swal from 'sweetalert2';
import { Options } from 'select2';
import { CommonService } from 'src/app/_metronic/shared/services/common.service';
import * as moment from 'moment';
import { environment } from 'src/environments/environment';
import { ParticipateTradePromotionModel } from '../../../_models/participate-trade-promotion.model';
import { ParticipateTradePromotionService } from '../../../_services/participate-trade-promotion-page.service';

const EMPTY_CUSTOM: ParticipateTradePromotionModel = {
  id: '',
  businessId: '',
  businessNameVi: '',
  diaChiTruSo: '',
  maSoThue: '',
  soDienThoai: '',
  email:'',
  nguoiDaiDien:'',
  ngayCapPhep: null,
  details: [],
  detail2s: [],
};
@Component({
  selector: 'app-info-modal.component',
  templateUrl: './info-modal.component.html',
  styleUrls: ['./info-modal.component.scss'],

})
export class InfoParticipateTradePromotionComponent implements OnInit, OnDestroy {
  @Input() id: any;
  isLoading$:any;
  participateTradePromotionData: ParticipateTradePromotionModel;
  formGroup: FormGroup;
  public options: Options;
  displayedColumns: string[] = ['stt','name', 'action'];
  public datKyBaoCao: Array<SelectOptionData>;
  files: any[] = [];
  private subscriptions: Subscription[] = [];
  public default_value = "00000000-0000-0000-0000-000000000000"
  public data_busi: Array<SelectOptionData>;
  ListFileDinhKemDel:any='';
  public dataIndustryPromotion : any = [];
  public dataSource: { code: string, name: string }[] = [];
  public dataSource1: { name : string, address: string ,country : string, scale: string,planJoin : number,startTime : any, endTime: any }[] = [];
  infoData: any;
  Search: string="";
  Search2: string="";
  Search3: string=""; 
  filtered: Observable<any[]>;
  constructor(
    private participateTradePromotionService: ParticipateTradePromotionService,
    private fb: FormBuilder, public modal: NgbActiveModal,
    private changeDetectorRefs: ChangeDetectorRef,
    private commonService: CommonService,
    ) { }

  ngOnInit(): void {
    this.isLoading$ = this.participateTradePromotionService.isLoading$;
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
    } else {
      const sb = this.participateTradePromotionService.getItemByCode(this.id).pipe(
        first(),
        catchError((errorMessage) => {
          this.modal.dismiss(errorMessage);
          return of(EMPTY_CUSTOM);
        })
      ).subscribe((res: any) => {
        this.infoData= res.data;
        if(this.infoData.ngayCapPhep){
          this.infoData.ngayCapPhep=this.convert_date_string(res.data.ngayCapPhep);
        }
        res.data.detail2s.forEach((element: any) => {
          const data = {
            code: element.industryCode,
            name: element.industryName
          }
          this.dataSource.push(data)
        });

        res.data.detail1s.forEach((element: any) => {
          const data = {
            name: element.participateSupportFairName,
            address: element.address,
            country: element.country,
            scale: element.scale,
            planJoin: element.planJoin,
            startTime: this.convert_date_string(element.startTime),
            endTime: this.convert_date_string(element.endTime),
          }
          this.dataSource1.push(data)
        });
        
        res.data.detail3s.forEach((item: any) => {
          const data = {
            Name: item.participateSupportFairName,
            StartDate: item.startDate,
            EndDate: item.endDate,
            CapitalName: item.capitalName,
            Funding: item.funding
          }
          this.dataIndustryPromotion.push(data);
        })
        this.changeDetectorRefs.detectChanges();

      });
      this.subscriptions.push(sb);
    }
  }

  f_currency(value: any, args?: any): any {
    let nbr = Number((value + '').replace(/,|-/g, ""));
    const result = (nbr + '').replace(/(\d)(?=(\d{3})+(?!\d))/g, "$1,");
    return result
  }
  
  clear_model() {
    EMPTY_CUSTOM.businessId = '',   
    EMPTY_CUSTOM.businessNameVi = '',   
    EMPTY_CUSTOM.nguoiDaiDien = '',   
    EMPTY_CUSTOM.soDienThoai = '',   
    EMPTY_CUSTOM.email = '',
    EMPTY_CUSTOM.ngayCapPhep = null,
    
    EMPTY_CUSTOM.details = [],
    this.participateTradePromotionData = EMPTY_CUSTOM
  }

  convert_date(string_date: string){
    var result = moment.utc(string_date, "DD/MM/YYYY");
    return result
  }

  convert_date_string(string_date: string){
    var date = string_date.split("T")[0];
    var list = date.split("-"); //["year", "month", "day"]
    var result = list[2] + "/" + list[1] + "/" + list[0]
    return result
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
  prenventInputNonNumber(event: any) {
    if (event.which < 48 || event.which > 57) {
      event.preventDefault();
    }
  }

}
