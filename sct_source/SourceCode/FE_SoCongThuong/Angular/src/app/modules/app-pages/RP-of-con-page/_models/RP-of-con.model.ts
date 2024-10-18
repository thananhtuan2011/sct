import { BaseModel } from '../../../../_metronic/shared/crud-table/models/base.model';

export interface RPOSOfConstructionModel extends BaseModel {
  // CigaretteBusinessId : string;
  ReportOperationalStatusOfConstructionInvestmentProjectsId : string;
  reportingPeriod : string;
  quantity : number | null;
  criteria : string;
  units : string;
  note : string;
  year: number;
}
