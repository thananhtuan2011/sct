import { BaseModel } from '../../../../_metronic/shared/crud-table/models/base.model';

export interface ManagementSeminarModel extends BaseModel {
  managemenetSeminarId : string;
  profileCode: string;
  businessId : string;
  title : string;
  districtId: string;
  address: string;
  contact: string;
  phoneNumber: string;
  numberParticipant: number;
  note: string;
  listTime: any [];
}
