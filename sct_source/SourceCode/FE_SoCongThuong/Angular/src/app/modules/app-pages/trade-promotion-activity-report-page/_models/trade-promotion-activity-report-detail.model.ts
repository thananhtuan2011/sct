import { BaseModel } from '../../../../_metronic/shared/crud-table/models/base.model';

export interface TradePromotionActivityReportDetailModel extends BaseModel {
  tradePromotionActivityReportDetailId: string;
  tradePromotionActivityReportId: string;
  businessId: string;
  businessName: string;
  address: string;
}