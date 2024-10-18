import { BaseModel } from '../../../../_metronic/shared/crud-table/models/base.model';

export interface GasBusinessModel extends BaseModel {
  gasBusinessId: string;
  typeBusiness: number;
  businessName: string;
  businessId: string;
  foreignTransactionName: string;
  gasBusiness: string;
  address: string;
  phoneNumber: string;
  fax: string;
  businessCertificate: string;
  licensors: string;
  licenseDate: any;
  taxId: string;
  numDoc: string;
  dateEnd: any;
  dateStart: any;
  complianceStatus: string;
}
