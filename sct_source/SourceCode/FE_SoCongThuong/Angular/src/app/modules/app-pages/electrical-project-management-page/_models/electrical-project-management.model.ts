import { BaseModel } from '../../../../_metronic/shared/crud-table/models/base.model';

export interface ElectricalProjectManagementModel extends BaseModel {
  electricalProjectManagementId: string;
  buildingCode: string;
  buildingName: string;
  district: string;
  address: string;
  represent: string;
  status: number;
  note: string;
  typeOfConstruction: string;
  voltageLevel: string;
  wattage: string;
  length: string;
  wireType: string;
}
