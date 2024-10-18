import { BaseModel } from '../../../../_metronic/shared/crud-table/models/base.model';

export interface RuralDevelopmentPlanModel extends BaseModel {
    /// Kế hoạch phát triển chợ nông thôn
    ruralDevelopmentPlanId: string;
    /// Tên TTTM / Siêu thị
    superMarketShoppingMallName: string;
    /// Địa chỉ
    address: string;
    /// Tổng vốn đầu tư
    totalInvestment: number | null;
    /// Ngân sách
    budget: number | null;
    /// Ngoài ngân sách
    outOfBudget: number | null;
    /// Loại hình: 0 - Xây dựng, 1 - Nâng cấp
    type: number | null;
    /// Giai đoạn
    stageId: string;
    /// Dữ liệu giai đoạn
    stages: any;
}
