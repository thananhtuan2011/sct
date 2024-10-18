import { BaseModel } from '../../../../_metronic/shared/crud-table/models/base.model';

export interface CateRPTurnOverIndustAncolModel extends BaseModel {
  cateReportTurnOverIndustAncolId: string,
  businessId: string,
  quantityBoughtOfYear: number | null, //Số lượng mua trong năm
  totalPriceBoughtOfYear: number | null, //Tổng giá trị mua trong năm
  quantitySoldOfYear: number | null, //Số lượng bán trong năm
  totalPriceSoldOfYear: number | null, //Tổng giá trị bán trong năm
  yearId: any
}
