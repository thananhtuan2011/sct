import { BaseModel } from '../../../../_metronic/shared/crud-table/models/base.model';

export interface ParticipateTradePromotionModel extends BaseModel {
  businessId : string;
  diaChiTruSo : string;
  businessNameVi : string;
  maSoThue : string;
  nguoiDaiDien : string;
  soDienThoai: string;
  email: string;

  ngayCapPhep : any;
  details: any;
  detail2s: any;
}
