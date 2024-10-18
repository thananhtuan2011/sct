import { BaseModel } from '../../../../_metronic/shared/crud-table/models/base.model';

export interface AdminFormalitiesModel extends BaseModel {
  adminFormalitiesId : string;
  adminFormalitiesCode : string;
  adminFormalitiesName : string;
  fieldId: string;
  dvclevel: number;
  docUrl: string;
}
