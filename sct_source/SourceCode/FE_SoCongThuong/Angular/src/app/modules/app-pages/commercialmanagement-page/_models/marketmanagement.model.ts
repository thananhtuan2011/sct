import { BaseModel } from '../../../../_metronic/shared/crud-table/models/base.model';

export interface MarketManagementModel extends BaseModel {
  marketManagementId: string,
  districtId: string,
  communeId: string,
  marketId: string,
  nganhHangKinhDoanh: string,
  boothNumber: number | null,
  giaTrongNhaLong: number | null,
  giaNgoaiNhaLong: number | null,
  deXuatGiaMoi: number | null,
  note: string,
  matHang: any
}

