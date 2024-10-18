import { BaseModel } from '../../../../_metronic/shared/crud-table/models/base.model';

export interface DistrictModel extends BaseModel {
  districtCode : string;
  districtName : string;
  communeNumber : number | null;
}
