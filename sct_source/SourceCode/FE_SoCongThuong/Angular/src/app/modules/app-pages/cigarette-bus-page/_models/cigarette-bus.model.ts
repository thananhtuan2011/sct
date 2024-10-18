import { BaseModel } from '../../../../_metronic/shared/crud-table/models/base.model';

export interface CigaretteBusinessModel extends BaseModel {
  // CigaretteBusinessId : string;
  CigaretteBusinessId : string;
  cigaretteBusinessName : string;
  cigaretteBusinessDetail: CigaretteBusinessDetailModel[];
}

export interface CigaretteBusinessDetailModel {
  TenDoanhNghiep: string;
  NguoiDaiDien: string;
  SoDienThoai: string;
  Huyen: string;
  Xa: string;
  DiaChi: string;
  GiayPhepKinhDoanh: string;
  NgayHetHan: any;
  DonViCungCap: string;
  NgayCap: any;
  DiaChiDonViCungCap: string;
  PhoneDonViCungCap: string;
  GhiChu: string;
}
