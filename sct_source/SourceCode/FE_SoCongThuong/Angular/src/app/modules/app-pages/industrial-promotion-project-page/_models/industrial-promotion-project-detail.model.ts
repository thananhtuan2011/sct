import { BaseModel } from '../../../../_metronic/shared/crud-table/models/base.model';

export interface IndustrialPromotionProjectDetailModel extends BaseModel {
  industrialPromotionProjectDetailId: string;
  industrialPromotionProjectId: string;
  businessId: string;
  businessCode: string;
  businessNameVi: string;
  nganhNghe: string;
  diaChi: string;
  nguoiDaiDien: string;
}
