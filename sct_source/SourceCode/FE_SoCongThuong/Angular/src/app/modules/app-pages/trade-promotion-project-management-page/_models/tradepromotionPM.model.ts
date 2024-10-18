import { BaseModel } from '../../../../_metronic/shared/crud-table/models/base.model';

export interface TradePromotionProjectManagementModel extends BaseModel {
  tradePromotionProjectManagementId : string;
  tradePromotionProjectManagementName : string;
  implementingAgencies: string;
  cost: string;
  currencyUnit: string;
  timeStart: any;
  timeEnd: any;
  numberOfApprovalDocuments: string;
  implementationResults: number;
  status: number;
  reason: string;
}