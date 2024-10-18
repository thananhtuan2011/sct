import { BaseModel } from '../../../../_metronic/shared/crud-table/models/base.model';

export interface CommuneModel extends BaseModel {
  communeId : string;
  communeCode : string;
  communeName : string;
  districtId : string;
}
