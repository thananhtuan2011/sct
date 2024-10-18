import { BaseModel } from '../../../../_metronic/shared/crud-table/models/base.model';

export interface TrainingManagementModel extends BaseModel {
  trainingManagementId: string;
  content: string;
  startDate: string;
  endDate: string;
  time: string;
  districtId: string;
  address: string;
  participating: string;
  numParticipating: number;
  implementationCost: number;
  annunciator: string;
  note: string;
  details: any;
}
