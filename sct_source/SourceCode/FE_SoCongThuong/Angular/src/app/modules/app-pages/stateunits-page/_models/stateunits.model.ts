import { BaseModel } from '../../../../_metronic/shared/crud-table/models/base.model';

export interface StateUnitsModel extends BaseModel {
  stateUnitsId : string;
  stateUnitsCode : string;
  stateUnitsName : string;
  parentId: string;
}
