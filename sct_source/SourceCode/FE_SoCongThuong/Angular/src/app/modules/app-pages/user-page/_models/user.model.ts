import { BaseModel } from '../../../../_metronic/shared/crud-table/models/base.model';

export interface UserModel extends BaseModel {
  userId:string;
  fullname: string;
  username: string;
  password: string;
  deptId: string;
  roleId: string;
  phone:string;
  email:string;
  cccd:string;
  groupId:string;
  levelUser: number;
  areaId: string;
}
