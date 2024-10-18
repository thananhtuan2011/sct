import { BaseModel } from '../../../../_metronic/shared/crud-table/models/base.model';

export interface ProposedPowerProjectsModel extends BaseModel {
  proposedPowerProjectId: string;
  energyIndustryId: string;
  projectName: string;
  statusId: string;
  investorName: string;
  address: string;
  wattage: number | null;
  policyDecision: string;
  note: string;
  // proposedDate: string;
}
