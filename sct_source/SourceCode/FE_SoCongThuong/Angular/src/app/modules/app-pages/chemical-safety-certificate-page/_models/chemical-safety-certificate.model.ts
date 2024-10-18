import { BaseModel } from '../../../../_metronic/shared/crud-table/models/base.model';

export interface ChemicalSafetyCertificateModel extends BaseModel{
  chemicalSafetyCertificateId: string;
  businessId: string;
  address: string;
  phoneNumber: string;
  fax: string;
  businessAddress: string;
  businessCode: string;
  provider: string;
  businessCertificateDate: string;
  num: string;
  validTill: any;
  licenseDate: string;
  listChemical: any[];
}