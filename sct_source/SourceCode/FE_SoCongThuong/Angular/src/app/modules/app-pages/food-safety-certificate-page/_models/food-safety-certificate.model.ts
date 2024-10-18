import { BaseModel } from '../../../../_metronic/shared/crud-table/models/base.model';

export interface FoodSafetyCertificateModel extends BaseModel{
  profileCode: string;
  profileName: string;
  foodSafetyCertificateId: string;
  businessId: string;
  managerName: string;
  districtId: string;
  address: string;
  phoneNumber: string;
  num: string;
  validTill: any;
  licenseDate: any;
  note: string;
}