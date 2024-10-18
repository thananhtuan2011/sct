import { BaseModel } from '../../../../_metronic/shared/crud-table/models/base.model';

export interface TradePromotionOtherModel extends BaseModel {
  tradePromotionOtherId: string;
  typeOfActivity: number;
  content: string;
  startDate: any;
  endDate: any;
  time: string;
  districtId: string;
  address: string;
  implementationCost: number | null;
  participating: string;
  coordination: string;
  result: string;
  note: string;
  details: any;
}
