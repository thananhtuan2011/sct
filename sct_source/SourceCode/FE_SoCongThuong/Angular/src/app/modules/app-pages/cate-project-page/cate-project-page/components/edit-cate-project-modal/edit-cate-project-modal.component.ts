import { ChangeDetectorRef, Component, Input, OnDestroy, OnInit, ChangeDetectionStrategy, ViewChild } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal, NgbDateAdapter, NgbDateParserFormatter } from '@ng-bootstrap/ng-bootstrap';
import { of, Subscription } from 'rxjs';
import { catchError, finalize, first, tap } from 'rxjs/operators';
import { SelectOptionData } from 'src/app/_metronic/shared/components/select-custom/select-custom.interface';
import Swal from 'sweetalert2';
import { Options } from 'select2';
import { CateProjectModel } from '../../../_models/cate-project.model';
import { CateProjectPageService } from '../../../_services/cate-project-page.service';
import { CommonService } from 'src/app/_metronic/shared/services/common.service';
import * as moment from 'moment';
import { MatStepper } from '@angular/material/stepper';

const EMPTY_CUSTOM: CateProjectModel = {
  id: '',
  cateProjectId: '00000000-0000-0000-0000-000000000000',
  units: '00000000-0000-0000-0000-000000000000',
  country: '00000000-0000-0000-0000-000000000000',
  companySell: '00000000-0000-0000-0000-000000000000',
  investmentCertificateDate: null,
  policyDecisionsDate: null,
  projectDecisionToWithdrawDate: null,
  capitalContributionTradingTime: null,
  address: '',
  projectId: '00000000-0000-0000-0000-000000000000',
  projectName: '',
  projectInvestment: 0,
  projectInvestmentUnits: '00000000-0000-0000-0000-000000000000',
  projectType: 0,
  projectTypeName: '',
  area: 0,
  investors: '',
  investmentCertificateCode: '',
  policyDecisions: '',

  /// Địa điểm thực hiện dự án
  projectAddress: '',

  /// ngành nghề
  profession: '',

  /// người đại diện pháp luật
  projectLegalRepresent: '',

  /// số diện thoại liên lạc
  projectPhoneNumber: '',

  /// tiến độ thực hiện dự án, tiến độ đã đăng ký
  projectProgress: '',

  /// thời gian thực hiện (năm)
  projectOperatingTime: 0,

  /// tiến độ thực tế
  projectProgressActual: '',

  /// địa bàn
  projectLocalArea: '',

  /// quốc tịch/đối tác
  projectPartnerNationality: '',

  /// hình thức đầu tư
  projectInvestmentForm: '',

  /// năm cấp phép
  projectLicenseYear: 0,

  /// Năm thực hiện
  projectImplementationYear: 0,

  /// mục tiêu, quy mô thực hiện dự án
  projectImplementationScale: '',

  /// quyết định thu hồi
  projectDecisionToWithdraw: '',

  /// FDI
  projectFdi: '',
  note: '',

  /// công ty bán
  companyBuy: '',

  /// vốn điều lệ ban đầu
  initialCharterCapital: 0,

  /// vốn mua
  capitalPurchase: 0,

  /// mua thực tế
  actualPurchase: 0,

  /// vốn điều lệ sau khi mua
  charterCapitalAfterPurchase: 0,

  details: [],
  historys: [],
};

@Component({
  selector: 'app-edit-cate-project-modal.component',
  templateUrl: './edit-cate-project-modal.component.html',
  styleUrls: ['./edit-cate-project-modal.component.scss'],

})
export class EditCateProjectModalComponent implements OnInit, OnDestroy {
  @Input() id: any;
  isLoading$: any;
  CateProjectData: CateProjectModel;
  formGroup: FormGroup;
  public options: Options;
  private subscriptions: Subscription[] = [];
  public typeofdonviData: Array<SelectOptionData>;
  public countryData: Array<SelectOptionData>;
  public businesData: Array<SelectOptionData>;
  public projectData: Array<SelectOptionData>;
  public projectDataDetail: any[] = [];
  firstFormGroup: FormGroup;
  secondFormGroup: FormGroup;
  thirdFormGroup: FormGroup;
  fourthFormGroup: FormGroup;
  isLinear = false;
  type: number = 1;
  total: number = 0;
  public dataProjectType: Array<SelectOptionData>;
  public dataCountryType: Array<SelectOptionData>;
  dataSource: any[] = [];
  lstGiaiNgan: any[] = [];
  
  check: any;
  cols_check: any = [
    "InvestmentCertificateCode", 
    "InvestmentCertificateDate", 
    "PolicyDecisions", 
    "PolicyDecisionsDate", 
    "ProjectInvestment"
  ];
  change_data: any = "";

  constructor(
    private cateProjectService: CateProjectPageService,
    private commonService: CommonService,
    private fb: FormBuilder, public modal: NgbActiveModal,
    private changeDetectorRefs: ChangeDetectorRef
  ) { }

  ngOnInit(): void {
    this.isLoading$ = this.cateProjectService.isLoading$;
    (async () => {
      this.loadTypeofCountry();
      this.loadTypeProject();
      this.loaddonvi();
      this.loadDetail();
      this.loadBusiness();
      this.loadCountry();
      await this.delay(300);
      this.loadProject();
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
    this.dataSource = [];
    if (!this.id) {
      this.clear_model();
      this.loadForm();
    } else {
      const sb = this.cateProjectService.getItemById(this.id).pipe(
        first(),
        catchError((errorMessage) => {
          this.modal.dismiss(errorMessage);
          return of(EMPTY_CUSTOM);
        })
      ).subscribe((res: any) => {
        this.CateProjectData = res.data;
        this.CateProjectData.projectInvestment = this.f_currency(res.data.projectInvestment)
        if (res.data.capitalContributionTradingTime) {
          this.CateProjectData.capitalContributionTradingTime = this.convert_date_string(res.data.capitalContributionTradingTime);
        }
        if (res.data.investmentCertificateDate) {
          this.CateProjectData.investmentCertificateDate = this.convert_date_string(res.data.investmentCertificateDate);
        }
        if (res.data.projectDecisionToWithdrawDate) {
          this.CateProjectData.projectDecisionToWithdrawDate = this.convert_date_string(res.data.projectDecisionToWithdrawDate);
        }
        if (res.data.policyDecisionsDate) {
          this.CateProjectData.policyDecisionsDate = this.convert_date_string(res.data.policyDecisionsDate);
        }
        this.type = res.data.projectType;
        res.data.details.forEach((x: any) => {
          let detail = {
            disbursementUnits: x.disbursementUnits,
            disbursementMoney: x.disbursementMoney,
            disbursementDate: this.convert_date_string(x.disbursementDate),
            isCheck: x.isConfirm,
          }
          if (detail.disbursementUnits == 'ae0f8d52-5843-4446-887d-174025469add') {
            this.total += +detail.disbursementMoney;
          }
          else {
            this.total += +detail.disbursementMoney * 23000;
          }
          this.lstGiaiNgan.push(detail);
          this.dataSource = this.lstGiaiNgan;
        });

        //Create data check
        this.check = this.CateProjectData

        this.loadForm();
      });
      this.subscriptions.push(sb);
    }
  }

  delay(ms: number) {
    return new Promise(resolve => setTimeout(resolve, ms));
  }

  active(item: any) {
    this.type = item;
    this.firstFormGroup.reset();
    this.secondFormGroup.reset();
    this.thirdFormGroup.reset();
    this.clear_model();
    this.loadForm();
    this.thirdFormGroup.updateValueAndValidity();
    var config = document.getElementById('active' + item);
    config?.classList.add("active");
    this.firstFormGroup.controls["type"].setValue(item);
    this.firstFormGroup.controls["type"].updateValueAndValidity();

    this.secondFormGroup.controls["Area"].setValue(item);
    this.secondFormGroup.controls["ProjectType"].setValue(item);
    this.secondFormGroup.controls["Area"].updateValueAndValidity();
    this.secondFormGroup.controls["ProjectType"].updateValueAndValidity();
    if (item == 3) {
      this.secondFormGroup.controls["Area"].setValue("1");
      this.secondFormGroup.controls["Area"].updateValueAndValidity();
    }
    if (item == 4) {
      this.secondFormGroup.controls["Area"].setValue("0");
      this.secondFormGroup.controls.ProjectInvestment.setValue(0);
      this.secondFormGroup.controls.ProjectInvestment.updateValueAndValidity();
      this.secondFormGroup.controls["Area"].updateValueAndValidity();
    }
    for (let i = 0; i < 5; i++) {
      if (i != item) {
        var config = document.getElementById('active' + i);
        config?.classList.remove("active");
      }
    }
  }

  f_currency(value: any, args?: any): any {
    let nbr = Number((value + '').replace(/,|-/g, ''));
    return (nbr + '').replace(/(\d)(?=(\d{3})+(?!\d))/g, '$1,');
  }

  prenventInputNonNumber(event: any) {
    if (event.which < 48 || event.which > 57) {
      event.preventDefault();
    }
  }

  loadForm() {
    this.firstFormGroup = this.fb.group({
      type: [this.type, Validators.required]
    });

    this.secondFormGroup = this.fb.group({
      ProjectType: [this.type],
      Area: [(this.type == 3) ? 1 : this.type],
      Address: [this.CateProjectData.address],
      Investors: [this.CateProjectData.investors, (this.type == 1 || this.type == 2 || this.type == 3) ? Validators.required : ''],
      InvestmentCertificateDate: [this.CateProjectData.investmentCertificateDate],
      InvestmentCertificateCode: [this.CateProjectData.investmentCertificateCode],//, (this.type == 1 || this.type == 2) ? Validators.required : ''],
      PolicyDecisions: [this.CateProjectData.policyDecisions],//, (this.type == 1 || this.type == 2) ? Validators.required : ''],
      PolicyDecisionsDate: [this.CateProjectData.policyDecisionsDate],

      //Tên dự án-thu hồi lũy kế
      ProjectName: [this.CateProjectData.projectName],
      ProjectId: [this.CateProjectData.projectId],

      //type==4
      Units: [this.CateProjectData.units],
      CompanyBuy: [this.CateProjectData.companyBuy, (this.type == 4) ? Validators.required : ''],
      Country: [this.CateProjectData.country],
      CompanySell: [this.CateProjectData.companySell],
      InitialCharterCapital: [this.CateProjectData.initialCharterCapital],//Vốn điều lệ ban hành
      CapitalPurchase: [this.CateProjectData.capitalPurchase],//Vốn mua
      ActualPurchase: [this.CateProjectData.actualPurchase],// mua thực tế
      CharterCapitalAfterPurchase: [this.CateProjectData.charterCapitalAfterPurchase],// vốn điều lệ sau khi mua
      CapitalContributionTradingTime: [this.CateProjectData.capitalContributionTradingTime],//thời gian  mua bán góp vốn
      Note: [this.CateProjectData.note],
    });

    this.thirdFormGroup = this.fb.group({
      //DDI FDI
      ProjectName: [this.CateProjectData.projectName, (this.type == 1 || this.type == 2) ? Validators.required : ''],
      ProjectInvestment: [this.CateProjectData.projectInvestment, (this.type == 1 || this.type == 2) ? Validators.required : ''],
      Units: [this.CateProjectData.units, (this.type == 1 || this.type == 2) ? Validators.required : ''],
      ProjectAddress: [this.CateProjectData.projectAddress],
      ProjectInvestmentUnits: [this.CateProjectData.projectInvestmentUnits],//cũng là unit nhưng ở tab thông tin dự án
      Profession: [this.CateProjectData.profession],
      ProjectPhoneNumber: [this.CateProjectData.projectPhoneNumber],
      ProjectLegalRepresent: [this.CateProjectData.projectLegalRepresent],//người đại diện pháp luật
      ProjectProgress: [this.CateProjectData.projectProgress],//Tiến độ
      ProjectOperatingTime: [this.CateProjectData.projectOperatingTime],//thời gian thực hiện (năm)
      ProjectProgressActual: [this.CateProjectData.projectProgressActual],//tiến độ thực tế
      ProjectLocalArea: [this.CateProjectData.projectLocalArea],//địa bàn
      ProjectPartnerNationality: [this.CateProjectData.projectPartnerNationality],//quốc tịch
      ProjectInvestmentForm: [this.CateProjectData.projectInvestmentForm],//hình thức đầu tư
      ProjectImplementationYear: [this.CateProjectData.projectImplementationYear],//năm thực hiện
      ProjectLicenseYear: [this.CateProjectData.projectLicenseYear],// năm cấp phép
      ProjectImplementationScale: [this.CateProjectData.projectImplementationScale],// mục tiêu, quy mô thực hiện dự án
      ProjectDecisionToWithdraw: [this.CateProjectData.projectDecisionToWithdraw, (this.type == 3) ? Validators.required : ''],
      ProjectDecisionToWithdrawDate: [this.CateProjectData.projectDecisionToWithdrawDate, (this.type == 3) ? Validators.required : ''],
      ProjectFdi: [this.CateProjectData.projectFdi],
      Note: [this.CateProjectData.note],
    });
    this.thirdFormGroup.controls.ProjectInvestment.valueChanges.subscribe((x) => {
      this.thirdFormGroup.patchValue({
        "ProjectInvestment": this.f_currency(x)
      }, { emitEvent: false })
    })

    this.fourthFormGroup = this.fb.group({
      //Info giải ngân
      DisbursementUnits: [this.CateProjectData.units],
      DisbursementMoney: [''],
      DisbursementDate: [this.CateProjectData.projectDecisionToWithdrawDate],
    });
  }

  addGiaiNgan() {
    var _unitname = "";
    if (this.fourthFormGroup.value.DisbursementUnits == "00000000-0000-0000-0000-000000000000"
      || this.fourthFormGroup.value.DisbursementDate == null
      || this.fourthFormGroup.value.DisbursementMoney == '') {
      return;
    }
    this.typeofdonviData.forEach(x => {
      if (x.id == this.fourthFormGroup.value.DisbursementUnits) {
        _unitname = x.text;
      }
    });

    let four = {
      disbursementUnits: this.fourthFormGroup.value.DisbursementUnits,
      unitname: _unitname,
      disbursementMoney: this.fourthFormGroup.value.DisbursementMoney,
      disbursementDate: this.fourthFormGroup.value.DisbursementDate,
      isCheck: false,
    }
    if (four.disbursementUnits == 'ae0f8d52-5843-4446-887d-174025469add') {
      this.total += +four.disbursementMoney;
    }
    else {
      this.total += +four.disbursementMoney * 23000;
    }
    this.lstGiaiNgan.push(four);
    this.dataSource = this.lstGiaiNgan;
    // this.fourthFormGroup.controls.DisbursementUnits.setValue("");
    this.fourthFormGroup.controls.DisbursementMoney.setValue("");
    this.fourthFormGroup.controls.DisbursementDate.setValue("");
    this.fourthFormGroup.updateValueAndValidity();
    this.changeDetectorRefs.detectChanges();
  }

  ChangInfo(element: any) {
    if (element.isCheck == false) {
      element.isCheck = true;
    }
    else {
      element.isCheck = false;
    }
  }

  delStore(item: any) {
    this.total -= +item.disbursementMoney;
    const index: number = this.lstGiaiNgan.indexOf(item);
    this.lstGiaiNgan.splice(index, 1);
    this.dataSource = this.lstGiaiNgan;
  }

  clear_model() {
    EMPTY_CUSTOM.address = '',
    EMPTY_CUSTOM.cateProjectId = '00000000-0000-0000-0000-000000000000',
    EMPTY_CUSTOM.units = '00000000-0000-0000-0000-000000000000',
    EMPTY_CUSTOM.country = '00000000-0000-0000-0000-000000000000',
    EMPTY_CUSTOM.companySell = '00000000-0000-0000-0000-000000000000',
    EMPTY_CUSTOM.investmentCertificateDate = null,
    EMPTY_CUSTOM.policyDecisionsDate = null,
    EMPTY_CUSTOM.projectDecisionToWithdrawDate = null,
    EMPTY_CUSTOM.capitalContributionTradingTime = null,
    EMPTY_CUSTOM.address = '',
    EMPTY_CUSTOM.projectId = '00000000-0000-0000-0000-000000000000',
    EMPTY_CUSTOM.projectName = '',
    EMPTY_CUSTOM.projectInvestment = 0,
    EMPTY_CUSTOM.projectInvestmentUnits = '00000000-0000-0000-0000-000000000000',
    EMPTY_CUSTOM.projectType = 0,
    EMPTY_CUSTOM.projectTypeName = '',
    EMPTY_CUSTOM.area = 0,
    EMPTY_CUSTOM.investors = '',
    EMPTY_CUSTOM.investmentCertificateCode = '',
    EMPTY_CUSTOM.policyDecisions = '',
    EMPTY_CUSTOM.projectAddress = '',
    EMPTY_CUSTOM.profession = '',
    EMPTY_CUSTOM.projectLegalRepresent = '',
    EMPTY_CUSTOM.projectPhoneNumber = '',
    EMPTY_CUSTOM.projectProgress = '',
    EMPTY_CUSTOM.projectOperatingTime = 0,
    EMPTY_CUSTOM.projectProgressActual = '',
    EMPTY_CUSTOM.projectLocalArea = '',
    EMPTY_CUSTOM.projectPartnerNationality = '',
    EMPTY_CUSTOM.projectInvestmentForm = '',
    EMPTY_CUSTOM.projectLicenseYear = 0,
    EMPTY_CUSTOM.projectImplementationYear = 0,
    EMPTY_CUSTOM.projectImplementationScale = '',
    EMPTY_CUSTOM.projectDecisionToWithdraw = '',
    EMPTY_CUSTOM.projectFdi = '',
    EMPTY_CUSTOM.note = '',
    EMPTY_CUSTOM.companyBuy = '',
    EMPTY_CUSTOM.initialCharterCapital = 0,
    EMPTY_CUSTOM.capitalPurchase = 0,
    EMPTY_CUSTOM.actualPurchase = 0,
    EMPTY_CUSTOM.charterCapitalAfterPurchase = 0,
    EMPTY_CUSTOM.details = [],
    EMPTY_CUSTOM.historys = [],
    this.dataSource = [];

    this.CateProjectData = EMPTY_CUSTOM
  }
  save() {
    this.prepareData();
    if (this.change_data) {
      let obj = {
        ContentAdjust: this.change_data,
      }
      this.CateProjectData.historys.push(obj)
    }
    if (this.id) {
      this.edit();
    } else {
      this.create();
    }
  }

  edit() {
    const sbUpdate = this.cateProjectService.update(this.CateProjectData).pipe(
      tap(() => {
        this.modal.close();
      }),
      catchError((errorMessage) => {
        this.modal.dismiss(errorMessage);
        return of(this.CateProjectData);
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
    const sbCreate = this.cateProjectService.create(this.CateProjectData).pipe(
      tap(() => {
        this.modal.close();
      }),
      catchError((errorMessage) => {
        this.modal.dismiss(errorMessage);
        return of(this.CateProjectData);
      }),
    ).subscribe((res: any) => {
      Swal.fire({
        icon: res.status == 1 ? 'success' : 'error',
        title: res.status == 1 ? 'Thêm mới thành công' : 'Thêm mới thất bại',
        confirmButtonText: 'Xác nhận',
        text: 'Thêm mới ' + (res.status == 1 ? 'thành công' : 'thất bại'),
      });
      this.CateProjectData = EMPTY_CUSTOM
    });
    this.subscriptions.push(sbCreate);
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

  //---------------------------------------------------------------------------------------------------
  private prepareData() {
    const formData1 = this.firstFormGroup.value;
    const formData2 = this.secondFormGroup.value;
    const formData3 = this.thirdFormGroup.value;

    this.CateProjectData.projectType = formData1.type;
    this.dataProjectType.forEach(x => {
      if (x.id == formData1.type) {
        this.CateProjectData.projectTypeName = x.text;
      }
    });

    this.CateProjectData.area = formData2.Area;
    this.CateProjectData.address = formData2.Address;
    this.CateProjectData.investors = formData2.Investors;
    this.CateProjectData.investmentCertificateDate = this.convert_date(formData2.InvestmentCertificateDate);
    this.CateProjectData.investmentCertificateCode = formData2.InvestmentCertificateCode;
    this.CateProjectData.policyDecisions = formData2.PolicyDecisions;
    this.CateProjectData.policyDecisionsDate = this.convert_date(formData2.PolicyDecisionsDate);
    this.CateProjectData.projectLicenseYear = formData3.ProjectLicenseYear;

    if (this.type == 4) {
      this.CateProjectData.units = formData2.Units;
      this.CateProjectData.companyBuy = formData2.CompanyBuy;
      this.CateProjectData.country = formData2.Country;
      this.CateProjectData.companySell = formData2.CompanySell;
      this.CateProjectData.initialCharterCapital = formData2.InitialCharterCapital;
      this.CateProjectData.capitalPurchase = formData2.CapitalPurchase;
      this.CateProjectData.charterCapitalAfterPurchase = formData2.CharterCapitalAfterPurchase;
      this.CateProjectData.actualPurchase = formData2.ActualPurchase;
      this.CateProjectData.capitalContributionTradingTime = this.convert_date(formData2.CapitalContributionTradingTime);
      this.CateProjectData.note = formData2.Note;
    }

    if (this.type != 4) {
      this.CateProjectData.projectName = formData3.ProjectName;
      this.CateProjectData.projectInvestment = Number(formData3.ProjectInvestment.replaceAll(',' , ''));
      this.CateProjectData.projectInvestmentUnits = formData3.ProjectInvestmentUnits;
      this.CateProjectData.profession = formData3.Profession;
      this.CateProjectData.projectAddress = formData3.ProjectAddress;
      this.CateProjectData.profession = formData3.Profession;
      this.CateProjectData.projectPhoneNumber = formData3.ProjectPhoneNumber;
      this.CateProjectData.projectLegalRepresent = formData3.ProjectLegalRepresent;
      this.CateProjectData.projectProgress = formData3.ProjectProgress;
      this.CateProjectData.projectOperatingTime = formData3.ProjectOperatingTime;
      this.CateProjectData.projectPartnerNationality = formData3.ProjectPartnerNationality;
      this.CateProjectData.projectLocalArea = formData3.ProjectLocalArea;
      this.CateProjectData.projectProgressActual = formData3.ProjectProgressActual;
      this.CateProjectData.projectInvestmentForm = formData3.ProjectInvestmentForm;
      this.CateProjectData.projectImplementationYear = formData3.ProjectImplementationYear;
      this.CateProjectData.projectImplementationScale = formData3.ProjectImplementationScale;
      this.CateProjectData.projectDecisionToWithdraw = formData3.ProjectDecisionToWithdraw;
      this.CateProjectData.projectDecisionToWithdrawDate = this.convert_date(formData3.ProjectDecisionToWithdrawDate);
      this.CateProjectData.projectFdi = formData3.ProjectFdi;
      this.CateProjectData.projectId = formData2.ProjectId;
      this.CateProjectData.note = formData3.Note;

      if (this.type == 3) {
        this.projectData.forEach(x => {
          if (x.id == this.CateProjectData.projectId) {
            this.CateProjectData.projectName = x.text;
          }
        });
      }

      let detail: {
        CateProjectId: '00000000-0000-0000-0000-000000000000',
        disbursementDate: any,
        disbursementMoney: number,
        disbursementUnits: string,
        isConfirm: boolean,
      };

      this.CateProjectData.details = [];
      this.dataSource.forEach(x => {
        detail = {
          CateProjectId: '00000000-0000-0000-0000-000000000000',
          disbursementDate: this.convert_date(x.disbursementDate),
          disbursementMoney: +x.disbursementMoney,
          disbursementUnits: x.disbursementUnits,
          isConfirm: x.isCheck,
        };
        this.CateProjectData.details.push(detail);
      });
    }

    this.CateProjectData.historys = [];
    this.getDirtyValues(this.firstFormGroup);
    this.getDirtyValues(this.secondFormGroup);
    this.getDirtyValues(this.thirdFormGroup);
  }
  //---------------------------------------------------------------------------------------------------
  
  getDirtyValues(form: any) {
    const dirtyValues: Record<string, any> = {}
    Object.keys(form.controls)
      .forEach(key => {
        let currentControl = form.controls[key];

        if (currentControl.dirty) {
          if (currentControl.controls)
            dirtyValues[key] = this.getDirtyValues(currentControl);
          else
            dirtyValues[key] = currentControl.value;

            //Check 5 trường cần check
            if (this.cols_check.includes(key) && this.check) {

              //Số chứng nhận đầu tư:
              if (key === "InvestmentCertificateCode") {
                if (this.check.investmentCertificateCode && currentControl.value) {
                  this.change_data += "Số chứng nhận đầu tư từ " + this.check.investmentCertificateCode + " thành " + currentControl.value + ","
                }
                if (!this.check.investmentCertificateCode && currentControl.value) {
                  this.change_data += "Thêm mới số chứng nhận đầu tư " + currentControl.value + ","
                }
                if (!this.check.investmentCertificateCode && !currentControl.value) {
                  this.change_data += "Xoá Số chứng nhận đầu tư " + this.check.investmentCertificateCode + ","
                }
              }

              //Ngày cấp chứng nhận đầu tư:
              if (key === "InvestmentCertificateDate") {
                if (this.check.investmentCertificateDate && currentControl.value) {
                this.change_data += "Ngày cấp chứng nhận đầu tư từ ngày " + this.check.investmentCertificateDate + " thành ngày " + currentControl.value + ","
                }
                if (!this.check.investmentCertificateDate && currentControl.value) {
                  this.change_data += "Thêm mới ngày cấp chứng nhận đầu tư " + currentControl.value + ","
                }
                if (!this.check.policyDecisions && !currentControl.value) {
                  this.change_data += "Xoá ngày cấp chứng nhận đầu tư " + this.check.investmentCertificateDate + ","
                }
              }

              //Quyết định chủ trương
              if (key === "PolicyDecisions") {
                if (this.check.policyDecisions && currentControl.value) {
                this.change_data += "Quyết định chủ trương từ " + this.check.policyDecisions + " thành " + currentControl.value + ","
                }
                if (!this.check.policyDecisions && currentControl.value) {
                  this.change_data += "Thêm mới quyết định chủ trương " + currentControl.value + ","
                }
                if (!this.check.policyDecisions && !currentControl.value) {
                  this.change_data += "Xoá quyết định chủ trương " + this.check.policyDecisions + ","
                }
              }

              //Ngày ra quyết định chủ trương
              if (key === "PolicyDecisionsDate") {
                if (this.check.policyDecisionsDate && currentControl.value) {
                this.change_data += "Ngày quyết định từ " + this.check.policyDecisionsDate + " thành " + currentControl.value + ","
                }
                if (!this.check.policyDecisions && currentControl.value) {
                  this.change_data += "Thêm mới quyết định " + currentControl.value + ","
                }
                if (!this.check.policyDecisions && !currentControl.value) {
                  this.change_data += "Xoá ngày quyết định " + this.check.policyDecisions + ","
                }
              }

              //Tổng vốn đầu tư
              if (key === "ProjectInvestment") {
                if (this.check.projectInvestment && currentControl.value) {
                this.change_data += "Tổng vốn đầu tư từ " + this.check.projectInvestment + " thành " + currentControl.value + ","
                }
                if (!this.check.projectInvestment && currentControl.value) {
                  this.change_data += "Thêm mới tổng vốn đầu tư " + currentControl.value + ","
                }
                if (!this.check.projectInvestment && !currentControl.value){
                  this.change_data += "Xoá tổng vốn đầu tư " + this.check.projectInvestment + ","
                }
              }
            }
        }
      });
    return this.change_data
  }

  loaddonvi() {
    this.cateProjectService.loadDV().subscribe(res_donvi => {
      const data = [{
        id: "00000000-0000-0000-0000-000000000000",
        text: '-- Chọn --'
      }]
      for (var item of res_donvi.items) {
        let obj_donvi = {
          id: item.unitId,
          text: item.unitName,
        }
        data.push(obj_donvi)
      }
      this.typeofdonviData = data
    })
    return this.typeofdonviData
  }

  loadTypeofCountry() {
    const data = [{
      id: "00000000-0000-0000-0000-000000000000",
      text: '-- Chọn --'
    }];
    let obj1 = {
      id: "1",
      text: "Trong nước ",
    };
    let obj2 = {
      id: "2",
      text: "Ngoài nước ",
    }
    data.push(obj1);
    data.push(obj2);
    this.dataCountryType = data;
    return this.dataCountryType
  }

  loadCountry() {
    this.commonService.getListCountry().subscribe((res: any) => {
      const data = [{
        id: "00000000-0000-0000-0000-000000000000",
        text: '-- Chọn --'
      }]
      for (var item of res.items) {
        let obj_coun = {
          id: item.countryId,
          text: item.countryName,
        }
        data.push(obj_coun)
      }
      this.countryData = data
    })
    return this.countryData
  }

  loadProject() {
    this.cateProjectService.loadProject().subscribe((res: any) => {
      const data = [{
        id: "00000000-0000-0000-0000-000000000000",
        text: '-- Chọn --'
      }]
      for (var item of res.data) {
        let obj_coun = {
          id: item.cateProjectId,
          text: item.projectName,
        }
        data.push(obj_coun)
      }
      this.projectData = data
    })
    return this.projectData;
  }

  loadProjectById($event: any) {
    this.cateProjectService.loadProjectbyId($event).subscribe((res: any) => {
      this.secondFormGroup.controls["Address"].setValue(res.data.address);
      this.secondFormGroup.controls["InvestmentCertificateCode"].setValue(res.data.investmentCertificateCode);
      this.secondFormGroup.controls["InvestmentCertificateDate"].setValue(this.convert_date_string(res.data.investmentCertificateDate));
      this.secondFormGroup.controls["PolicyDecisions"].setValue(res.data.policyDecisions);
      this.secondFormGroup.controls["PolicyDecisionsDate"].setValue(this.convert_date_string(res.data.policyDecisionsDate));
      this.secondFormGroup.updateValueAndValidity();
    })
    return this.projectDataDetail;
  }

  loadBusiness() {
    this.commonService.getBusiness().subscribe((res: any) => {
      const data = [{
        id: "00000000-0000-0000-0000-000000000000",
        text: '-- Chọn --'
      }]
      for (var item of res.items) {
        let obj_bus = {
          id: item.businessId,
          text: item.businessNameVi,
        }
        data.push(obj_bus)
      }
      this.businesData = data
    })
    return this.businesData;
  }

  loadTypeProject() {
    const data = [{
      id: "00000000-0000-0000-0000-000000000000",
      text: '-- Chọn --'
    }];
    let obj1 = {
      id: "1",
      text: "Lũy kế DDI ngoài Khu công nghiệp",
    };
    let obj2 = {
      id: "2",
      text: "Lũy kế FDI ngoài Khu công nghiệp",
    }
    let obj3 = {
      id: "3",
      text: "Thu hồi lũy kế",
    };
    let obj4 = {
      id: "4",
      text: "Mua bán góp vốn",
    }
    data.push(obj1);
    data.push(obj2);
    data.push(obj3);
    data.push(obj4);
    this.dataProjectType = data;
    return this.dataProjectType
  }

  isDefaultValue(controlName: any, form: any)//: boolean 
  {
    if (form == 2) {
      const control = this.secondFormGroup.controls[controlName];
      const isdefaultvalue = (control.value == "00000000-0000-0000-0000-000000000000" || control.value == "0")
      if (isdefaultvalue) {
        control.setErrors({ default: true })
      }
      return control.invalid && (control.touched || control.dirty)
    }
    else if (form == 3) {
      const control = this.thirdFormGroup.controls[controlName];
      const isdefaultvalue = (control.value == "00000000-0000-0000-0000-000000000000" || control.value == "0")
      if (isdefaultvalue) {
        control.setErrors({ default: true })
      }
      return control.invalid && (control.touched || control.dirty)
    }
    else if (form == 4) {
      const control = this.fourthFormGroup.controls[controlName];
      const isdefaultvalue = (control.value == "00000000-0000-0000-0000-000000000000" || control.value == "0")
      if (isdefaultvalue) {
        control.setErrors({ default: true })
      }
      return control.invalid && (control.touched || control.dirty)
    }
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

  fourthFormHasError(validation: any, controlName: any): boolean {
    const control = this.fourthFormGroup.controls[controlName];
    return control.hasError(validation) && (control.dirty || control.touched);
  }

  thirdFormHasError(validation: any, controlName: any): boolean {
    const control = this.thirdFormGroup.controls[controlName];
    return control.hasError(validation) && (control.dirty || control.touched);
  }

  secondFormHasError(validation: any, controlName: any): boolean {
    const control = this.secondFormGroup.controls[controlName];
    return control.hasError(validation) && (control.dirty || control.touched);
  }

  isControlTouched(controlName: any): boolean {
    const control = this.formGroup.controls[controlName];
    return control.dirty || control.touched;
  }
  
  @ViewChild("stepper", { static: false }) stepper: MatStepper;
  check_second_form(type: any){
    if (type != 4) {
      if (this.secondFormGroup.invalid) {
        this.secondFormGroup.markAllAsTouched();
      }
      else {
        this.stepper.next();
      }
    }
    if (type == 4) {
      if (this.secondFormGroup.invalid) {
        this.secondFormGroup.markAllAsTouched();
      }
      else {
        this.save();
      }
    }
  }

  check_third_form(){
    if (this.thirdFormGroup.invalid) {
      if (this.thirdFormGroup.controls.ProjectInvestmentUnits.value == "00000000-0000-0000-0000-000000000000") {
        this.thirdFormGroup.controls.ProjectInvestmentUnits.setErrors({'default': true});
      }
      this.thirdFormGroup.markAllAsTouched();
      this.thirdFormGroup.updateValueAndValidity();
    }
    else {
      this.stepper.next();
    }
  }
}
