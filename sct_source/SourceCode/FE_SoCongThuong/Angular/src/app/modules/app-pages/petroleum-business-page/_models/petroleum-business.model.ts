import { BaseModel } from '../../../../_metronic/shared/crud-table/models/base.model';

export interface PetroleumBusinessModel extends BaseModel {
  // petroleumBusinessId : string;
  petroleumBusinessId : string;
  petroleumBusinessName : string; //Tên doanh nghiệp
  petroleumBusinessDetail : PetroleumBusinessDetailModel[];
  giayDangKyKinhDoanh: string;
  ngayCap: any;
}
export interface PetroleumBusinessDetailModel {
  TenCuaHang: string;
  NguoiDaiDien: string;
  SoDienThoai: string;
  Huyen: string;
  Xa: string;
  DiaChi: string;
  GiayPhepKinhDoanh: string;
  NgayHetHan: any;
  NgayCapPhep : any;
  ThoiHan5Nam : any;
  NguoiQuanLy : string;
  HinhThuc : string;
  SoCotBomE5 : number | null;
  SoCotBomA95 : number | null;
  SoCotBomOil : number | null;
  SoBeChua : number | null;
  TongDungTich : number | null;
  ThoiGianBanHang : string;
  DienTichXayDung : string;
  TuyenPhucVu : string;
  DonViCungCap: string;
  LoaiGiayXacNhan: string;
  ThoiHan1Nam: any;
  DiaChiDonViCungCap: string;
  NguoiLienHeDonViCungCap: string;
  SoDienThoaiDonViCungCap: string;
  HinhThucHopDong: string;
  GhiChu: string;
  NgayCapPhepXayDung: any;
}

