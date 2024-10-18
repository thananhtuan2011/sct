import { BaseModel } from '../../../../_metronic/shared/crud-table/models/base.model';

export interface StageModel extends BaseModel {
  stageId : string;
  stageName : string;
  startYear : number | null;
  endYear : number | null;
}
