import { BaseModel } from '../../../../_metronic/shared/crud-table/models/base.model';

export interface CateRPSoldAncolModel extends BaseModel {
  cateReportSoldAncolId: string,
  businessId: string,
  quantityBoughtOfYear: number | null, //Số lượng mua trong năm
  totalPriceBoughtOfYear: number | null, //Tổng giá trị mua trong năm
  quantitySoldOfYear: number | null, //Số lượng bán trong năm
  totalPriceSoldOfYear: number | null, //Tổng giá trị bán trong năm
  yearId: number
}
