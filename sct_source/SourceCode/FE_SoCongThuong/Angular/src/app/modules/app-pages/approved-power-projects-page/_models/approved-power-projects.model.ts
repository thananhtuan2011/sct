import { BaseModel } from '../../../../_metronic/shared/crud-table/models/base.model';

export interface ApprovedPowerProjectsModel extends BaseModel {
  energyIndustryId: string;
  approvedPowerProjectId: string;
  projectName: string;
  investorName: string;
  districtId: string;
  address: string;
  policyDecision: string;
  wattage: number | null;
  turbines: number | null;
  substation: number | null;
  powerOutput: number | null;
  area: number | null;
  year: number;
  status: string;
  note: string;
}
