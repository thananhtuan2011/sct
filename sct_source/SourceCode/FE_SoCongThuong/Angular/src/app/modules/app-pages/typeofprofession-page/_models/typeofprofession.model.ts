import { BaseModel } from '../../../../_metronic/shared/crud-table/models/base.model';

export interface TypeOfProfessionModel extends BaseModel {
  typeOfProfessionId : string;
  typeOfProfessionCode : string;
  typeOfProfessionName : string;
}
