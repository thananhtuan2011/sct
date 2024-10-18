import { BaseModel } from '../../../../_metronic/shared/crud-table/models/base.model';

export interface MarketInvestEnterpriseModel extends BaseModel {
  marketInvestEnterpriseId: string;
  marketName: string;
  districtId: string;
  communeId: string;
  address: string;
  investment: boolean;
  businessName: string;
  capital: number;
  evaluate: string;
  note: string;
}
