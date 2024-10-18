import { BaseModel } from '../../../../_metronic/shared/crud-table/models/base.model';

export interface RPIndustrialModel extends BaseModel {
  // CigaretteBusinessId : string;
  ReportOperationalStatusOfConstructionInvestmentProjectsId : string;
  reportingPeriod : string;
  typeReport : string;
  quantity : number | null;
  criteria : string;
  units : string;
  note : string;
  year: number;
  groupId: string;
  district: string;
}
