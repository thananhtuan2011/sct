import { BaseModel } from '../../../../_metronic/shared/crud-table/models/base.model';

export interface CountryModel extends BaseModel {
  countryId : string;
  countryCode : string;
  countryName : string;
}
