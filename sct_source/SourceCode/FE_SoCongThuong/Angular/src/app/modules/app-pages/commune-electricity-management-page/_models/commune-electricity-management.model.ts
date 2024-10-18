import { BaseModel } from '../../../../_metronic/shared/crud-table/models/base.model';

export interface CommuneElectricityManagementModel extends BaseModel {
  communeElectricityManagementId : string;
  stageId : string;
  districtId: string;
  communeId: string;
  content41Start: boolean;
  content42Start: boolean;
  target4Start: boolean;
  content41End: boolean;
  content42End: boolean;
  target4End: boolean;
  note: string;
}
