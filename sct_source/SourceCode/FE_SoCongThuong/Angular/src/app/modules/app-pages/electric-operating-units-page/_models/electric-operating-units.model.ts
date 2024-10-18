import { BaseModel } from '../../../../_metronic/shared/crud-table/models/base.model';

export interface ElectricOperatingUnitsModel extends BaseModel {
  electricOperatingUnitsId: string;
  customerName: string;
  address: string;
  phoneNumber: string;
  presidentName: string;
  numOfGP: string;
  signDay: any;
  supplier: string;
  isPowerGeneration: boolean;
  isPowerDistribution: boolean;
  isElectricityRetail: boolean;
  isConsulting: boolean;
  isSurveillance : boolean;
  status: number;
}
