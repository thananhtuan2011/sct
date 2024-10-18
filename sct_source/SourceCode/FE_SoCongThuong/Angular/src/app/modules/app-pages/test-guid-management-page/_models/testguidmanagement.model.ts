import { BaseModel } from '../../../../_metronic/shared/crud-table/models/base.model';

export interface TestGuidManagementModel extends BaseModel {
  testGuidManagementId : string;
  inspectionAgency : string;
  coordinationAgency: string;
  result: string;
  time: any;
}