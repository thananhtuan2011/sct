import { BaseModel } from '../../../../_metronic/shared/crud-table/models/base.model';

export interface TrainingClassManagementModel extends BaseModel {
  trainingClassManagementId : string;
  topic : string;
  location: string;
  participant: string;
  numberOfAttendees: number | null;
  timeStart: any;
}