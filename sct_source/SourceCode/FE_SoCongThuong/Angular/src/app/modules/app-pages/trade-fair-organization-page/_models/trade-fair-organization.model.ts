import { BaseModel } from '../../../../_metronic/shared/crud-table/models/base.model';

export interface TradeFairOrganizationCertificationModel extends BaseModel {
  tradeFairOrganizationCertificationId: string,
  tradeFairName: string,
  address: string,
  scale: string,
  textNumber: string,
}
