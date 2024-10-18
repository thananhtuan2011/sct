import { BaseModel } from '../../../../_metronic/shared/crud-table/models/base.model';

export interface MarketDevelopPlanModel extends BaseModel {
  marketDevelopPlanId: string;
  marketName: string;
  districtId: string;
  communeId: string;
  address: string;
  rankId: string;
  stage: string;
  typeOfPlanMarket: string;
  existLandArea: number;
  newLandArea: number;
  addLandArea: number;
  capital: number;
  note: string;
}
