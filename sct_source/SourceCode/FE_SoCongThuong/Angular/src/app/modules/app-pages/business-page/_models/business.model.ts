import { List } from 'lodash';
import { BaseModel } from '../../../../_metronic/shared/crud-table/models/base.model';

export interface BusinessModel extends BaseModel {
  businessId : string;
  
  //Thông tin doanh nghiệp
  businessCode : string;
  tenGiaoDich : string;
  businessNameEn : string;
  diaChiTruSo : string;
  ngayCapPhep : any;
  maSoThue : string;
  businessNameVi : string;
  districtId: string;
  communeId: string;
  loaiHinhDoanhNghiep : string;
  loaiNganhNghe : string;
  ngayHoatDong : any;
  giayPhepSanXuat: string;

  //Thông tin liên lạc
  nguoiDaiDien : string;
  soDienThoai : string;
  ngaySinh: string;
  cccd: string;
  ngayCap: string;
  noiCap: string;
  diaChi : string;
  giamDoc : string;
  email : string;

  //Thông tin ngành nghề
  industryId : string[];
}
