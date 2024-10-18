import { BaseModel } from '../../../../_metronic/shared/crud-table/models/base.model';

export interface RPOSOfInvestmentnModel extends BaseModel {
  // CigaretteBusinessId : string;
  ReportOperationalStatusOfInvestmentProjectsId: string;
  reportingPeriod: string;
  quantity: number | null;
  criteria: string;
  units: string;
  note: string;
  year: number;
}
