import { BaseModel } from '../../../../_metronic/shared/crud-table/models/base.model';

export interface IndustryModel extends BaseModel {
  industryId :string;
  industryCode :string;
  industryName: string;
  parentIndustryId: string;
}
