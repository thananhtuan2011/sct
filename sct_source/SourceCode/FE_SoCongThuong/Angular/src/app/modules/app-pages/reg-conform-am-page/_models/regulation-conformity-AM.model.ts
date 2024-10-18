import { BaseModel } from '../../../../_metronic/shared/crud-table/models/base.model';

export interface RegulationConformityAMModel extends BaseModel {
  regulationConformityAMId: string;
  dayReception: string;
  establishmentId: string;
  districtId: string;
  address: string;
  phone: string;
  num: string;
  productName: string;
  content: string;
  dateOfPublication: string;
  note: string
}