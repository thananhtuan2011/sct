import { BaseModel } from '../../../../_metronic/shared/crud-table/models/base.model';

export interface Target7Model extends BaseModel {
  target7Id : string;
  year: number;
  stageId : string;
  districtId: string;
  communeId: string;
  marketInPlaning: number;
  planCommercial: boolean;
  planMarket: boolean;
  estimateCommercial: boolean;
  estimateMarket: boolean;
  newRuralCriteriaRaised: boolean;
  note: string;
}
