import { BaseModel } from '../../../../_metronic/shared/crud-table/models/base.model';

export interface IndustrialManagementTargetModel extends BaseModel {
  industrialManagementTargetId: string;
  parentTargetId: string;
  groupTargetId: string;
  name: string;
  unit: string;
  listChild: any[];
}


