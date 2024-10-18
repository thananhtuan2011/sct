import { BaseModel } from '../../../../_metronic/shared/crud-table/models/base.model';

export interface IndustryPromotionReportModel extends BaseModel {
  rpIndustrialPromotionFundingId: string,
  yearReport: number | null
  nationalReport: number | null,
  localReport: number | null,
  targets: string,
  unit: string,
}
