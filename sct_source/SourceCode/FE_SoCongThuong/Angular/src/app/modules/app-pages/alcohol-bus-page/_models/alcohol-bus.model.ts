import { BaseModel } from '../../../../_metronic/shared/crud-table/models/base.model';

export interface AlcoholBusinessModel extends BaseModel {
  // petroleumBusinessId : string;
  alcoholBusinessId : string;
  alcoholBusinessName : string;
  // supplier : string;
  // representative : string;
  // phoneNumber : string;
  // address : string;
  // details: any;
  alcoholBusinessDetail: AlcoholBusinessDetailModel[];
  giayDangKyKinhDoanh: string;
  ngayCapPhep: any;
  giayPhepBanBuon: string;
  ngayCapGiayPhepBanBuon: any;
  ngayHetHanGiayPhepBanBuon: any;
}
export interface AlcoholBusinessDetailModel {
  TenDoanhNghiep: string;
  NguoiDaiDien: string;
  SoDienThoai: string;
  Huyen: string;
  Xa: string;
  DiaChi: string;
  GiayPhepKinhDoanh: string;
  NgayHetHan: any;
  DonViCungCap: string;
  DiaChiDonViCungCap: string;
  SoDienThoaiDonViCungCap: string;
  NgayCapGiayPhepBanLe: any;
  GhiChu: string;
}
