import { BaseModel } from '../../../../_metronic/shared/crud-table/models/base.model';

export interface CommercialManagementModel extends BaseModel {
  commercialId: string,
  type: string,
  code: string,
  name: string,
  typeOfMarketId: string,
  districtId: string,
  communeId: string,
  address: string,
  rankId: string,
  constructiveNatureId: string,
  businessNatureId: string,
  typeOfEconomic: string,
  managementFormId: string,
  managementObjectId: string,
  phoneNumber: string,
  email: string,
  fax: string,
  note: string,
  typeOfMarket: number| null,
  typeOfCenterLogistic: number| null,
  formMarket: number| null,
  form: number| null,
  area: number| null,
  marketCleared: number| null
}
