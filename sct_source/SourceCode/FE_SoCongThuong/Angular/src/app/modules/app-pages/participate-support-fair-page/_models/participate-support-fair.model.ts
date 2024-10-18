import { BaseModel } from '../../../../_metronic/shared/crud-table/models/base.model';

export interface ParticipateSupportFairModel extends BaseModel {
  participateSupportFairId : string;
  participateSupportFairName : string;
  address : string;
  country: string;
  scale: string;
  startTime: any;
  endTime: any;
  planJoin: number;
  details: any;
  districtId: string,
  communeId: string,
  implementCost: number,
}
