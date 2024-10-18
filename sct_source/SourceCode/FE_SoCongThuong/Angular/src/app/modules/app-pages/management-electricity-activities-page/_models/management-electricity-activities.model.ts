import { BaseModel } from '../../../../_metronic/shared/crud-table/models/base.model';

export interface ManagementElectricityActivitiesModel extends BaseModel {
  managementElectricityActivitiesId: string;
  projectName: string;
  districtId: string;
  wattage: number;
  maxWattage: number;
  type: number | null;
  dateOfAcceptance: string;
  connectorAgreement: string;
  powerPurchaseAgreement: string;
  anotherContent:string;
}
