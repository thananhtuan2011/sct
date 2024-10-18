import { BaseModel } from '../../../../_metronic/shared/crud-table/models/base.model';

export interface GroupUserModel extends BaseModel {
  groupId: string;
  groupName: string;
  priority: number;
}
