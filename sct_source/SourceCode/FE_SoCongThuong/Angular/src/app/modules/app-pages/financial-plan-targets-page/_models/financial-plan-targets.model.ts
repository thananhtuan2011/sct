import { BaseModel } from '../../../../_metronic/shared/crud-table/models/base.model';

export interface FinancialPlanTargetsModel extends BaseModel {
  financialPlanTargetsId: string;
  //Tên
  name: string;
  /** Phân loại - Type
   * 1: Giá trị sản xuất
   * 2: Sản phẩm chủ yếu
   * Xuất khẩu
   * 3: Tổng kim ngạch xuất khẩu
   * 4: Phân theo khối Doanh nghiệp
   * 5: Phân theo nhóm hàng
   * 6: Mặt hàng xuất khẩu chủ yếu
   * 7: Thị trường xuất khẩu
   * Nhập khẩu:
   * 8: Tổng kim ngạch nhập khẩu
   * 9: Mặt hàng nhập khẩu chủ yếu
   * 10: Tổng MBLHH-DVXH
   */
  type: number;
  //Đơn vị
  unit: string;
  //Năm tháng báo cáo "YYYY-MM"
  date: string;
  // (1) Kế hoạch năm
  plan: number;
  // (2) Thực hiện cùng tháng năm trước
  // valueSameMonthLastYear: number;
  // (3) Thực hiện tháng trước
  // valueLastMonth: number;
  // (4) Ước tính tháng thực hiện
  estimatedMonth: number;
  // (5) Cộng dồn đến tháng
  cumulativeToMonth: number;
  // (6) Cộng dồn đến tháng năm trước
  // cumulativeToMonthLastYear: number;
  // (7)=(4)/(3) So sánh tháng trước: = estimatedMonth / valueLastMonth
  // compareLastMonth: number;
  // (8)=(4)/(2) So sánh tháng cùng kỳ = estimatedMonth / valueSameMonthLastYear
  // comparedSameMonth: number;
  // (9)=(4)/(1) Luỹ kế so kế hoạch năm = estimatedMonth / plan
  // accumulatedComparedYearPlan: number;
  // (10)=(5)/(6) Luỹ kế so cùng kỳ = cumulativeToMonth / cumulativeToMonthLastYear
  // accumulatedComparedPeriod: number;
}
