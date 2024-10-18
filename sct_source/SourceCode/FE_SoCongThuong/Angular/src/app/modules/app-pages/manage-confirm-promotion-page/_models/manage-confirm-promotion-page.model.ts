import { BaseModel } from '../../../../_metronic/shared/crud-table/models/base.model';

export interface ManageConfirmPromotionModel extends BaseModel {
  manageConfirmPromotionId : string;
  manageConfirmPromotionName : string;
  goodsServices: string;
  goodsServicesPay: string;
  timeStart: any;
  timeEnd: any;
  numberOfDocuments: string;
}