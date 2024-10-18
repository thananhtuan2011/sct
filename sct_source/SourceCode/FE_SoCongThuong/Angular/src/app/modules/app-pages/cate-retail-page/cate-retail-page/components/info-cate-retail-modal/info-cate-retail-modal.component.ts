import { ChangeDetectorRef, Component, Input, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal, NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { of, Subscription } from 'rxjs';
import { catchError, first } from 'rxjs/operators';
import { SelectOptionData } from 'src/app/_metronic/shared/components/select-custom/select-custom.interface';
import { Options } from 'select2';
import { CateRetailModel } from '../../../_models/cate-retail.model';
import { CateRetailPageService } from '../../../_services/cate-retail.service';
import { UserModel } from 'src/app/modules/auth/models/user.model';
import { AuthService } from 'src/app/modules/auth/services/auth.service';
import Swal from 'sweetalert2';
import { environment } from 'src/environments/environment';
import { HttpClient } from '@angular/common/http';

export type UserType = UserModel | undefined;

const EMPTY_CUSTOM: CateRetailModel = {
  id: '',
  cateRetailId: '00000000-0000-0000-0000-000000000000',
  cateRetailCode: '',
  confirmUserId: '00000000-0000-0000-0000-000000000000',
  checkUserId: '00000000-0000-0000-0000-000000000000',
  createUserId: '00000000-0000-0000-0000-000000000000',
  confirmTime: null,
  createTime: null,
  reportMonth: '',
  details: [{
    criteria: '00000000-0000-0000-0000-000000000000',
    cumulativeToReportingMonth: 0,
    estimateReportingMonth: 0,
    performLastmonth: 0,
    performReporting: 0,
    type: 0,
  }],
};

@Component({
  selector: 'app-info-cate-retail-modal.component',
  templateUrl: './info-cate-retail-modal.component.html',
  styleUrls: ['./info-cate-retail-modal.component.scss'],

})
export class InfoCateRetailModalComponent implements OnInit, OnDestroy {
  @Input() id: any;
  isLoading$: any;
  cateretailData: CateRetailModel;
  formGroup: FormGroup;
  public options: Options;
  dataSourceTheoNam: any[] = [];
  dataSourceTheoNamTruoc: any[] = [];
  lstbaocaonam: any[] = [];
  lstbaocaonamtruoc: any[] = [];
  tyle: any[] = [];
  displayedColumns: string[] = ['stt', 'name', 'action'];
  totaldetail1: number = 0;
  totaldetail2: number = 0;
  totaldetail3: number = 0;
  totaldetail4: number = 0;
  totaldetail5: number = 0;
  totaldetail6: number = 0;
  totaldetail7: number = 0;
  totaldetail8: number = 0;
  status_tab_2: boolean = false;
  status_tab_3: boolean = false;

  private subscriptions: Subscription[] = [];
  public default_value = "00000000-0000-0000-0000-000000000000"
  public dataUser: Array<SelectOptionData>;
  show: boolean = false;


  constructor(
    private CateRetailService: CateRetailPageService,
    private fb: FormBuilder, 
    public modal: NgbActiveModal,
    private http: HttpClient
  ) { }

  ngOnInit(): void {
    this.isLoading$ = this.CateRetailService.isLoading$;
    this.loadUser();
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
    const sb = this.CateRetailService.getItemById(this.id).pipe(
      first(),
      catchError((errorMessage) => {
        this.modal.dismiss(errorMessage);
        return of(EMPTY_CUSTOM);
      })
    ).subscribe((res: any) => {
      this.cateretailData = res.data;
      this.cateretailData.confirmTime = this.convert_date_string(res.data.confirmTime);
      this.cateretailData.createTime = this.convert_date_string(res.data.createTime);

      res.data.details.forEach((x: any) => {
        if (x.type == 1) {
          this.lstbaocaonam.push(x);
          this.dataSourceTheoNam = this.lstbaocaonam;
          this.totaldetail1 += +x.performLastmonth;
          this.totaldetail2 += +x.estimateReportingMonth;
          this.totaldetail3 += +x.cumulativeToReportingMonth;
        }
        else {
          this.lstbaocaonamtruoc.push(x);
          this.dataSourceTheoNamTruoc = this.lstbaocaonamtruoc;
          this.totaldetail4 += +x.estimateReportingMonth;
          this.totaldetail5 += +x.cumulativeToReportingMonth;
        }
      });

      if (this.dataSourceTheoNamTruoc.length === 0) {
        this.status_tab_2 = true;
      }

      let detail: {
        criterianame: string;
        col1: number;
        col2: number;
        col3: number;
      };

      this.dataSourceTheoNam.forEach(x => {
        this.dataSourceTheoNamTruoc.forEach(y => {
          if (x.criteria == y.criteria) {
            detail = {
              criterianame: x.criteriaName,
              col1: +x.estimateReportingMonth / +x.performLastmonth,
              col2: +x.estimateReportingMonth / +y.estimateReportingMonth,
              col3: +x.cumulativeToReportingMonth / +y.cumulativeToReportingMonth,
            };
            this.totaldetail6 += +detail.col1;
            this.totaldetail7 += +detail.col2;
            this.totaldetail8 += +detail.col3;
            this.tyle.push(detail);
          }
        });
      });

      if (this.tyle.length === 0) {
        this.status_tab_3 = true;
      }

      this.loadForm();
      this.formGroup.disable();
    });

    this.subscriptions.push(sb);
  }

  loadForm() {
    this.formGroup = this.fb.group({
      CateRetailCode: [this.cateretailData.cateRetailCode, Validators.compose([Validators.required])],
      CheckName: [this.cateretailData.checkUserId],
      ConfirmName: [this.cateretailData.confirmUserId],
      CreateName: [this.cateretailData.createUserId],
      NgayDuyet: [this.cateretailData.confirmTime, Validators.required],
      NgayTao: [this.cateretailData.createTime],
      ReportMonth: [this.cateretailData.reportMonth, Validators.required],
    });
    this.show = true;
  }

  loadUser() {
    this.CateRetailService.loaduser().subscribe(res => {
      const data = [{
        id: "00000000-0000-0000-0000-000000000000",
        text: '-- Chọn --'
      }]
      for (var user of res.items) {
        let obj_business = {
          id: user.userId,
          text: user.fullName,
        }
        data.push(obj_business);
      }
      this.dataUser = data;
      this.loadDetail();
    })
  }

  round_number(num: number) {
    return (num > 0) ? num.toFixed(3).replace(/\.000$/, '') : '';
  }

  convert_date_string(string_date: string) {
    var date = string_date.split("T")[0];
    var list = date.split("-"); //["year", "month", "day"]
    var result = list[2] + "/" + list[1] + "/" + list[0]
    return result
  }

  ngOnDestroy(): void {
    this.subscriptions.forEach(sb => sb.unsubscribe());
  }

  exportFile() {
    const moment = require("moment");
    const timeString = moment().format("DDMMYYYYHHmmss");
    const fileName = "Quanlytongmucbanlehanghoa_" + timeString + ".xlsx"
    Swal.fire({
      icon: 'info',
      title: 'Đang xuất File...',
      // text: 'Vui lòng đợi một lúc trước khi file của bạn sẵn sàng!',
      didOpen: () => {
        Swal.showLoading()
      },
    })
    this.http.post(`${environment.apiUrl}/CateRetail/ExportExcel/${this.id}`, {},
    {
      responseType: 'blob',
    }).pipe(
      catchError((errorMessage: any) => {
        console.error(errorMessage)
        Swal.fire({
          icon: 'error',
          title: 'Xuất File thất bại',
          confirmButtonText: 'Xác nhận',
        });
        return of();
      }),
    ).subscribe(
      (res) => {
        const file = new Blob([res], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' });
        const fileURL = URL.createObjectURL(file);
        const a = document.createElement('a');
        a.href = fileURL;
        a.download = fileName;
        document.body.append(a);
        a.click();
        a.remove();
        URL.revokeObjectURL(fileURL);
        Swal.fire({
          icon: 'success',
          title: 'Xuất File thành công',
          confirmButtonText: 'Xác nhận',
        });
      },
    );
  }
}
