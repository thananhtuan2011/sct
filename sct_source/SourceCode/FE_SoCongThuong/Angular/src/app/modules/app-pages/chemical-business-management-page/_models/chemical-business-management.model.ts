import { BaseModel } from '../../../../_metronic/shared/crud-table/models/base.model';

export interface ChemicalBusinessManagementModel extends BaseModel {
  chemicalBusinessManagementId : string;
  businessName: string;
  address: string;
  chemicalStorage: string;
  pnupschcmeasures: any;
  status: number;
  represent: string;
  districtId: string;
  communeId: string;
}
