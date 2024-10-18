import { BaseModel } from '../../../../_metronic/shared/crud-table/models/base.model';

export interface CateRPProduceIndustlAncolModel extends BaseModel {
  cateReportProduceIndustlAncolId: string,
  businessId: string,
  typeofWine: string, //Loại rượu
  designCapacity: string, //Công suất thiết kế
  quantityProduction: number | null, //Sản lượng sản xuất
  quantityConsume: number | null, //Sản lượng tiêu thụ
  investment: number | null, //vốn đầu tư
  yearReport: number,
}
