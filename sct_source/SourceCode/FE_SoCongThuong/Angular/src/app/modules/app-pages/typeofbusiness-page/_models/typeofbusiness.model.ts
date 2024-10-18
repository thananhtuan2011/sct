import { BaseModel } from '../../../../_metronic/shared/crud-table/models/base.model';

export interface TypeOfBusinessModel extends BaseModel {
  typeOfBusinessId : string;
  typeOfBusinessCode : string;
  typeOfBusinessName : string;
}
