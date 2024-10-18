import { BaseModel } from '../../../../_metronic/shared/crud-table/models/base.model';

export interface TypeOfEnergyModel extends BaseModel {
  typeOfEnergyId : string;
  typeOfEnergyCode : string;
  typeOfEnergyName : string;
}
