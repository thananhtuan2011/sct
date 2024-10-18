import { BaseModel } from '../../../../_metronic/shared/crud-table/models/base.model';

export interface CommitManagerModel extends BaseModel {
  commitManagerId: string;
  // businessId: string;
  // businessName: string;
  // representative: string;
  // address: string;
  // businessCertificate: string;
  // licenseDate: any;
  // licensors: string;
  // phoneNumber: string;
  // power: string;
  // staff: number;
  // committer: string;
  // noDoc: string;
  // items: string;
  maHoSo: string;
  tenThuTuc: string;
  tenToChuc: string; 
  coSo: string;
  diaChi: string; 
  soDienThoai: string;
  ngayNhanHoSo: any;
  ngayCamKet: any;
  nguoiLamCamKet: string;
  ghiChu: string;
  huyen: string;
  listItems: any;
}
