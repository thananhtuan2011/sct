import { BaseModel } from '../../../../_metronic/shared/crud-table/models/base.model';

export interface IndustrialPromotionResultsModel extends BaseModel {
  rpIndustrialPromotionResultsId: string,
  yearReport: number | null
  nationalReport: number | null,
  localReport: number | null,
  targets: string,
  unit: string,
}
