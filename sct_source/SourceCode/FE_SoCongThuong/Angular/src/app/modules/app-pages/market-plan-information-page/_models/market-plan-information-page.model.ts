import { BaseModel } from '../../../../_metronic/shared/crud-table/models/base.model';

export interface MarketPlanInformationModel extends BaseModel {
  marketPlanInformationId: string;
  marketName: string;
  districtId: string;
  communeId: string;
  address: string;
  year: number;
  landArea: number;
  businessLandArea: number;
  constructionProperty: string;
  constructionNeed: string;
  note: string;
}
