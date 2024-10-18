import { ChangeDetectorRef, Component, Input, OnChanges, OnDestroy, OnInit, SimpleChanges } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal, NgbDateAdapter, NgbDateParserFormatter } from '@ng-bootstrap/ng-bootstrap';
import { of, Subscription } from 'rxjs';
import { catchError, finalize, first, tap } from 'rxjs/operators';
import Swal from 'sweetalert2';

import { FinancialPlanTargetsModel } from '../../../_models/financial-plan-targets.model';
import { FinancialPlanTargetsPageService } from '../../../_services/financial-plan-targets-page.service';
import { Options } from 'select2';
import * as moment from 'moment';

const EMPTY_CUSTOM: FinancialPlanTargetsModel = {
  id: '',
  financialPlanTargetsId: '00000000-0000-0000-0000-000000000000',
  //Tên
  name: '',
  /** Phân loại - Type
   * 1: Giá trị sản xuất
   * 2: Sản phẩm chủ yếu
   * Xuất khẩu
   * 3: Tổng kim ngạch xuất khẩu
   * 4: Phân theo khối doanh nghiệp
   * 5: Phân theo nhóm hàng
   * 6: Mặt hàng xuất khẩu chủ yếu
   * 7: Thị trường xuất khẩu
   * Nhập khẩu:
   * 8: Tổng kim ngạch nhập khẩu
   * 9: Mặt hàng nhập khẩu chủ yếu
   * 10: Tổng MBLHH-DVXH
   */
  type: 0,
  //Đơn vị
  unit: '',
  //Năm
  date: moment().format('YYYY-MM'),
  // (1) Kế hoạch năm
  plan: 0,
  // (2) Thực hiện cùng tháng năm trước
  //valueSameMonthLastYear: 0,
  // (3) Thực hiện tháng trước
  // valueLastMonth: 0,
  // (4) Ước tính tháng thực hiện
  estimatedMonth: 0,
  // (5) Cộng dồn đến tháng
  cumulativeToMonth: 0,
  // (6) Cộng dồn đến tháng năm trước
  // cumulativeToMonthLastYear: 0,
  // (7)=(4)/(3) So sánh tháng trước: = estimatedMonth / valueLastMonth
  // compareLastMonth: 0,
  // (8)=(4)/(2) So sánh tháng cùng kỳ = estimatedMonth / valueSameMonthLastYear
  // comparedSameMonth: 0,
  // (9)=(4)/(1) Luỹ kế so kế hoạch năm = estimatedMonth / plan
  // accumulatedComparedYearPlan: 0,
  // (10)=(5)/(6) Luỹ kế so cùng kỳ = cumulativeToMonth / cumulativeToMonthLastYear
  // accumulatedComparedPeriod: 0,
};

@Component({
  selector: 'app-edit-import-export-business-modal',
  templateUrl: './edit-modal.component.html',
  styleUrls: ['./edit-modal.component.scss'],
})

export class EditImportExportBusinessModalComponent implements OnInit, OnDestroy, OnChanges {
  @Input() id: any;
  isLoading$: any;
  financialPlanTargetsData: FinancialPlanTargetsModel;
  formGroup: FormGroup;
  options: Options;
  private subscriptions: Subscription[] = [];
  exportData: { id: string, text: string }[] = [];

  //Nhập khẩu - Mặt hàng chủ yếu
  importedGoods: { id: string, text: string }[] = [
    {
      id: "Nguyên liệu dược, dược phẩm",
      text: "Nguyên liệu dược, dược phẩm"
    },
    {
      id: "Nguyên phụ liệu dệt may, da giày, vải",
      text: "Nguyên phụ liệu dệt may, da giày, vải"
    },
    {
      id: "Sản phẩm điện tử và linh kiện, máy vi tính",
      text: "Sản phẩm điện tử và linh kiện, máy vi tính"
    },
    {
      id: "Máy móc, thiết bị, dụng cụ",
      text: "Máy móc, thiết bị, dụng cụ"
    }
  ]

  /**Chỉ tiêu xuất khẩu */
  targetsExport: { id: any, text: string }[] = [
    {
      id: "Tổng kim ngạch xuất khẩu",
      text: "Tổng kim ngạch xuất khẩu"
    },
    {
      id: "Phân theo khối doanh nghiệp",
      text: "Phân theo khối doanh nghiệp"
    },
    {
      id: "Phân theo nhóm hàng",
      text: "Phân theo nhóm hàng"
    },
    {
      id: "Mặt hàng xuất khẩu chủ yếu",
      text: "Mặt hàng xuất khẩu chủ yếu"
    },
    {
      id: "Thị trường xuất khẩu",
      text: "Thị trường xuất khẩu"
    },
  ]

  /**Xuất khẩu */

  /**Chỉ tiêu nhập khẩu */
  targetsImport: { id: any, text: string }[] = [
    {
      id: "Tổng kim ngạch nhập khẩu",
      text: "Tổng kim ngạch nhập khẩu"
    },
    {
      id: "Mặt hàng nhập khẩu chủ yếu",
      text: "Mặt hàng nhập khẩu chủ yếu"
    },
    {
      id: "Tổng MBLHH-DVXH",
      text: "Tổng MBLHH-DVXH"
    },
  ]
  // Khối doanh nghiệp
  businessBlock: { id: string, text: string }[] = [
    {
      id: "Doanh nghiệp FDI",
      text: "Doanh nghiệp FDI"
    },
    {
      id: "Doanh nghiệp trong nước",
      text: "Doanh nghiệp trong nước"
    },
  ]
  // Nhóm hàng
  groupOfGoods: { id: string, text: string }[] = [
    {
      id: "Hàng thủy sản",
      text: "Hàng thủy sản"
    },
    {
      id: "Hàng rau quả",
      text: "Hàng rau quả"
    },
    {
      id: "Hàng công nghiệp - tiểu thủ công nghiệp",
      text: "Hàng công nghiệp - tiểu thủ công nghiệp"
    },
    {
      id: "KNXK SP từ dừa",
      text: "KNXK SP từ dừa"
    }
  ]

  //Mặt hàng chủ yếu
  // exportedGoods: { id: string, text: string }[] = [
  //   {
  //     id: "Thuỷ sản các loại",
  //     text: "Thuỷ sản các loại"
  //   },
  //   {
  //     id: "Cơm dừa nạo sấy",
  //     text: "Cơm dừa nạo sấy"
  //   },
  //   {
  //     id: "Nước cốt dừa",
  //     text: "Nước cốt dừa"
  //   },
  //   {
  //     id: "Nước dừa đóng lon",
  //     text: "Nước dừa đóng lon"
  //   },
  //   {
  //     id: "Than hoạt tính",
  //     text: "Than hoạt tính"
  //   },
  //   {
  //     id: "Chỉ xơ dừa",
  //     text: "Chỉ xơ dừa"
  //   },
  //   {
  //     id: "Dệt may",
  //     text: "Dệt may"
  //   },
  //   {
  //     id: "Túi xách",
  //     text: "Túi xách"
  //   },
  //   {
  //     id: "Điện tử và linh kiện",
  //     text: "Điện tử và linh kiện"
  //   },
  // ]

  // Thị trường xuất khẩu
  // market: { id: string, text: string }[] = [
  //   {
  //     id: "Châu Á",
  //     text: "Châu Á"
  //   },
  //   {
  //     id: "Châu Mỹ",
  //     text: "Châu Mỹ"
  //   },
  //   {
  //     id: "Châu Âu",
  //     text: "Châu Âu"
  //   },
  //   {
  //     id: "Châu Phi",
  //     text: "Châu Phi"
  //   },
  //   {
  //     id: "Châu Đại Dương",
  //     text: "Châu Đại Dương"
  //   },
  // ]

  show: boolean = false;

  constructor(
    private financialPlanTargetsService: FinancialPlanTargetsPageService,
    private fb: FormBuilder,
    public modal: NgbActiveModal,
    private changeDetectorRef: ChangeDetectorRef
  ) {
    this.exportData = this.businessBlock
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes.id.currentValue) {
      const sb = this.financialPlanTargetsService.getItemById(this.id).pipe(
        first(),
        catchError((errorMessage) => {
          this.modal.dismiss(errorMessage);
          return of(EMPTY_CUSTOM);
        })
      ).subscribe((res: any) => {
        this.financialPlanTargetsData = res.items[0];
        this.loadForm();
        if (this.financialPlanTargetsData.type == 3) {
          this.formGroup.controls.Classify.reset(2);
          this.formGroup.controls.Targets.reset("Tổng kim ngạch xuất khẩu", { onlySelf: true, emitEvent: false })
          this.formGroup.controls.Name.reset(this.financialPlanTargetsData.name, { onlySelf: true, emitEvent: false })
        } else if (this.financialPlanTargetsData.type == 4) {
          this.formGroup.controls.Classify.reset(2);
          this.formGroup.controls.Targets.reset("Phân theo khối doanh nghiệp", { onlySelf: true, emitEvent: false })
          this.formGroup.controls.Name.reset(this.financialPlanTargetsData.name, { onlySelf: true, emitEvent: false })
        } else if (this.financialPlanTargetsData.type == 5) {
          this.formGroup.controls.Classify.reset(2);
          this.formGroup.controls.Targets.reset("Phân theo nhóm hàng", { onlySelf: true, emitEvent: false })
          this.formGroup.controls.Name.reset(this.financialPlanTargetsData.name, { onlySelf: true, emitEvent: false })
        } else if (this.financialPlanTargetsData.type == 6) {
          this.formGroup.controls.Classify.reset(2);
          this.formGroup.controls.Targets.reset("Mặt hàng xuất khẩu chủ yếu", { onlySelf: true, emitEvent: false })
          this.formGroup.controls.Name.reset(this.financialPlanTargetsData.name, { onlySelf: true, emitEvent: false })
        } else if (this.financialPlanTargetsData.type == 7) {
          this.formGroup.controls.Classify.reset(2);
          this.formGroup.controls.Targets.reset("Thị trường xuất khẩu", { onlySelf: true, emitEvent: false })
          this.formGroup.controls.Name.reset(this.financialPlanTargetsData.name, { onlySelf: true, emitEvent: false })
        } else if (this.financialPlanTargetsData.type == 8) {
          this.formGroup.controls.Classify.reset(1);
          this.formGroup.controls.Targets.reset("Tổng kim ngạch nhập khẩu", { onlySelf: true, emitEvent: false })
          this.formGroup.controls.Name.reset(this.financialPlanTargetsData.name, { onlySelf: true, emitEvent: false })
        } else if (this.financialPlanTargetsData.type == 9) {
          this.formGroup.controls.Classify.reset(1);
          this.formGroup.controls.Targets.reset("Mặt hàng nhập khẩu chủ yếu", { onlySelf: true, emitEvent: false })
          this.formGroup.controls.Name.reset(this.financialPlanTargetsData.name, { onlySelf: true, emitEvent: false })
        } else if (this.financialPlanTargetsData.type == 10) {
          this.formGroup.controls.Classify.reset(1);
          this.formGroup.controls.Targets.reset("Tổng MBLHH-DVXH", { onlySelf: true, emitEvent: false })
          this.formGroup.controls.Name.reset(this.financialPlanTargetsData.name, { onlySelf: true, emitEvent: false })
        }
        this.show = true;
      });
      this.subscriptions.push(sb);
    } else {
      this.financialPlanTargetsData = EMPTY_CUSTOM;
      this.loadForm();
      this.resetForm();
      this.show = true;
    }
  }

  ngOnInit(): void {
    this.isLoading$ = this.financialPlanTargetsService.isLoading$;

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

  GetValue(control1: string, control2: string) {
    if (this.formGroup.value[control1] !== 0 && this.formGroup.value[control2] !== 0) {
      const result = this.formGroup.value[control1] / this.formGroup.value[control2];
      return Math.floor(result * 100) / 100;
    }
    return 0
  }

  GetYearMonth(type: string) {
    const result = this.formGroup.controls.Date.value.split("-") // ["YYYY", "MM"]

    if (type == "Year") {
      return result[0]
    }

    if (type == "Month") {
      return result[1]
    }
  }

  loadForm() {
    this.formGroup = this.fb.group({
      Classify: [1],
      Name: ["Nguyên liệu dược, dược phẩm"],
      Targets: ["Mặt hàng nhập khẩu chủ yếu"],
      Unit: [this.financialPlanTargetsData.unit, Validators.required],
      Date: [this.financialPlanTargetsData.date],
      Plan: [this.financialPlanTargetsData.plan],
      // ValueSameMonthLastYear: [this.financialPlanTargetsData.valueSameMonthLastYear],
      // ValueLastMonth: [this.financialPlanTargetsData.valueLastMonth],
      EstimatedMonth: [this.financialPlanTargetsData.estimatedMonth],
      CumulativeToMonth: [this.financialPlanTargetsData.cumulativeToMonth],
      // CumulativeToMonthLastYear: [this.financialPlanTargetsData.cumulativeToMonthLastYear],
    });

    // Change data by control Classify:
    this.formGroup.controls.Classify.valueChanges.subscribe((x: number) => {
      if (x == 1) {
        this.formGroup.controls.Targets.reset("Mặt hàng nhập khẩu chủ yếu")
        this.formGroup.controls.Name.reset("Nguyên liệu dược, dược phẩm")
      } else {
        this.formGroup.controls.Targets.reset("Phân theo khối doanh nghiệp")
        this.formGroup.controls.Name.reset("Doanh nghiệp FDI")
      }
    });

    // Set control Name theo control Target:
    this.formGroup.controls.Targets.valueChanges.subscribe((x: string) => {
      /** Xuất khẩu */
      // Name không nhập
      if (x == "Tổng kim ngạch xuất khẩu") {
        this.formGroup.controls.Name.reset("Tổng kim ngạch nhập khẩu");
      }
      // Name chọn từ businessBlock
      else if (x == "Phân theo khối doanh nghiệp") {
        this.exportData = this.businessBlock;
        this.formGroup.controls.Name.reset("Doanh nghiệp FDI");
      }
      // Name chọn từ groupOfGoods
      else if (x == "Phân theo nhóm hàng") {
        this.exportData = this.groupOfGoods;
        this.formGroup.controls.Name.reset("Hàng thủy sản");
      }
      // Name nhập text
      else if (x == "Mặt hàng xuất khẩu chủ yếu") {
        this.formGroup.controls.Name.reset("");
      } 
      // Name nhập text
      else if (x == "Thị trường xuất khẩu") {
        this.formGroup.controls.Name.reset("");
      }

      /** Nhập khẩu */
      // Name không nhập
      else if (x == "Tổng kim ngạch nhập khẩu") {
        this.formGroup.controls.Name.reset("Tổng kim ngạch nhập khẩu");
      }
      else if (x == "Mặt hàng nhập khẩu chủ yếu") {
        this.formGroup.controls.Name.reset("Nguyên liệu dược, dược phẩm");
      }
      // Name không nhập
      else if (x == "Tổng MBLHH-DVXH") {
        this.formGroup.controls.Name.reset("Tổng MBLHH-DVXH");
      }
    });
  }

  resetForm() {
    this.formGroup.controls.Classify.reset(1);
    this.formGroup.controls.Name.reset("Nguyên liệu dược, dược phẩm", { onlySelf: true, emitEvent: false });
    this.formGroup.controls.Targets.reset("Mặt hàng nhập khẩu chủ yếu", { onlySelf: true, emitEvent: false });
    this.formGroup.controls.Unit.reset("");
    this.formGroup.controls.Date.reset(moment().format('YYYY-MM'));
    this.formGroup.controls.Plan.reset(0);
    // this.formGroup.controls.ValueSameMonthLastYear.reset(0);
    // this.formGroup.controls.ValueLastMonth.reset(0);
    this.formGroup.controls.EstimatedMonth.reset(0);
    this.formGroup.controls.CumulativeToMonth.reset(0);
    // this.formGroup.controls.CumulativeToMonthLastYear.reset(0);
  }

  save() {
    this.prepareData();
    if (this.id) {
      this.edit();
    } else {
      this.create();
    }
  }

  private prepareData() {
    const formData = this.formGroup.value;
    this.financialPlanTargetsData.name = formData.Name;
    this.financialPlanTargetsData.type = this.getTypeOfTarget(formData.Classify, formData.Targets);
    this.financialPlanTargetsData.unit = formData.Unit;
    this.financialPlanTargetsData.date = formData.Date;
    this.financialPlanTargetsData.plan = formData.Plan;
    // this.financialPlanTargetsData.valueSameMonthLastYear = formData.ValueSameMonthLastYear;
    // this.financialPlanTargetsData.valueLastMonth = formData.ValueLastMonth;
    this.financialPlanTargetsData.estimatedMonth = formData.EstimatedMonth;
    this.financialPlanTargetsData.cumulativeToMonth = formData.CumulativeToMonth;
    // this.financialPlanTargetsData.cumulativeToMonthLastYear = formData.CumulativeToMonthLastYear;
  }

  getTypeOfTarget(Classification: number, Target: string) {
    switch (Classification) {
      case 1:
        switch (Target) {
          case "Tổng kim ngạch nhập khẩu":
            return 8;
          case "Mặt hàng nhập khẩu chủ yếu":
            return 9;
          case "Tổng MBLHH-DVXH":
            return 10;
          default:
            return 0;
        }
      case 2:
        switch (Target) {
          case "Tổng kim ngạch xuất khẩu":
            return 3;
          case "Phân theo khối doanh nghiệp":
            return 4;
          case "Phân theo nhóm hàng":
            return 5;
          case "Mặt hàng xuất khẩu chủ yếu":
            return 6;
          case "Thị trường xuất khẩu":
            return 7;
          default:
            return 0;
        }
      default:
        return 0;
    }
  }

  edit() {
    const sbUpdate = this.financialPlanTargetsService.update(this.financialPlanTargetsData).pipe(
      tap(() => {
        this.modal.close();
      }),
      catchError((errorMessage) => {
        this.modal.dismiss(errorMessage);
        return of(this.financialPlanTargetsData);
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
    const sbCreate = this.financialPlanTargetsService.create(this.financialPlanTargetsData).pipe(
      tap(() => {
        this.modal.close();
      }),
      catchError((errorMessage) => {
        this.modal.dismiss(errorMessage);
        return of(this.financialPlanTargetsData);
      }),
    ).subscribe((res: any) => {
      Swal.fire({
        icon: res.status == 1 ? 'success' : 'error',
        title: res.status == 1 ? 'Thêm mới thành công' : 'Thêm mới thất bại',
        confirmButtonText: 'Xác nhận',
        text: res.status == 0 ? res.error.msg : 'Thêm mới ' + (res.status == 1 ? 'thành công' : 'thất bại'),
      });
      this.financialPlanTargetsData = EMPTY_CUSTOM;
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
      this.formGroup.updateValueAndValidity();
    }
    else {
      this.save()
    }
  }
}
