import { BaseModel } from '../../../../_metronic/shared/crud-table/models/base.model';

export interface BusinessLineModel extends BaseModel {
  businessLineId : string;
  businessLineCode : string;
  businessLineName : string;
}
