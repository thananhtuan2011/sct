import { BaseModel } from '../../../../_metronic/shared/crud-table/models/base.model';

export interface TradePromotionProjectManagementDetailModel extends BaseModel {
  tradePromotionProjectManagementDetailId: string;
  tradePromotionProjectManagementId: string;
  businessId: string;
  businessCode: string;
  businessNameVi: string;
  nganhNghe: string;
  diaChi: string;
  nguoiDaiDien: string;
}
