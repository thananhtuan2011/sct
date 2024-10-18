import { BaseModel } from '../../../../_metronic/shared/crud-table/models/base.model';

export interface InterCommerceModel extends BaseModel {
  // CigaretteBusinessId : string;
  InternationalCommerceId : string;
  internationalCommerceName : string;
  investorName : string;
  licensingActivity : string;
  address : string;
  diaChiDoanhNghiep: string;
  giayDangKyKinhDoanh: string;
  ngayCapPhep: any;
  nguoiDaiDien: string;
  soDienThoai: string;
  tenCoSoBanLe: string;
  diaChiCoSoBanLe: string;
  giayPhepKinhDoanh: string;
  ngayCapGiayPhepKinhDoanh: any;
  giayPhepBanLe: string;
  ngayCapGiayPhepBanLe: any;
  ngayHetHanGiayPhepBanLe: any;
  dienTichSuDung: number;
  dienTichSan: number;
  dienTichBanHang: number;
  dienTichKinhDoanh: number;
  ghiChu: string;
  loaiHinhCoSo: string;
}
