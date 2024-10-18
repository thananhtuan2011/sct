import { BaseModel } from '../../../../_metronic/shared/crud-table/models/base.model';

export interface SampleContractModel extends BaseModel {
  sampleContractId: string,
  sampleContractField: string,
  registrationTime: any,
  profileNumber: string,
  registrantName: string,
  phoneNumber: string,
  businessName: string,
  taxCode: string,
  businessPhoneNumber: string | null,
  address: string | null,
}
