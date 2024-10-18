import { BaseModel } from '../../../../_metronic/shared/crud-table/models/base.model';

export interface ManagementFirePreventionModel extends BaseModel {
  managementFirePreventionId : string;
  businessName : string;
  address : string;
  reality : number;
}
