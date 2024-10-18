import { BaseModel } from '../../../../_metronic/shared/crud-table/models/base.model';

export interface StateTitlesModel extends BaseModel {
  stateTitlesId : string;
  stateTitlesCode : string;
  stateTitlesName : string;
  piority : number;
}
