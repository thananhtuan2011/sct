import { BaseModel } from '../../../../_metronic/shared/crud-table/models/base.model';

export interface MarketTargetSevenModel extends BaseModel {
  marketTargetSevenId: string;
  marketName: string;
  districtId: string;
  communeId: string;
  address: string;
  date: string;
  note: string;
}
