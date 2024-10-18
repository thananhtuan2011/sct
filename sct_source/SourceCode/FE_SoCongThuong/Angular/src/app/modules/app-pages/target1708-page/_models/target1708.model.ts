import { BaseModel } from '../../../../_metronic/shared/crud-table/models/base.model';

export interface Target1708Model extends BaseModel {
  target1708Id : string;
  stageId : string;
  districtId: string;
  communeId: string;
  newRuralCriteria: boolean;
  newRuralCriteriaRaised: boolean;
  note: string;
}
