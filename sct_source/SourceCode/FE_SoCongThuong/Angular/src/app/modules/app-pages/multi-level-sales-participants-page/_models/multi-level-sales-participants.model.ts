import { BaseModel } from '../../../../_metronic/shared/crud-table/models/base.model';

export interface MultiLevelSalesParticipantsModel extends BaseModel {
  multiLevelSalesParticipantsId: string,
  multiLevelSalesParticipantsCode: string,
  participantsName: string,
  birthday: any,
  phoneNumber : string,
  identityCardNumber: number | null,
  dateOfIssuance: any,
  placeOfIssue: string,
  gender: number,
  joinDate: any,
  province: string,
  address: string,
}
