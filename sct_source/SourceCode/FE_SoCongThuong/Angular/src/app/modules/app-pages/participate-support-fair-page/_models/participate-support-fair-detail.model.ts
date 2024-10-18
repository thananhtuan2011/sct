import { BaseModel } from '../../../../_metronic/shared/crud-table/models/base.model';

export interface ParticipateSupportFairDetailModel extends BaseModel {
  participateSupportFairDetailId: string;
  participateSupportFairId: string;
  businessId: string;
  businessCode: string;
  businessNameVi: string;
  nganhNghe: string;
  diaChi: string;
  nguoiDaiDien: string;
  huyen: string;
  xa: string;
}
