import { BaseModel } from '../../../../_metronic/shared/crud-table/models/base.model';

export interface CateRPPCrafttAncolForEconomicModel extends BaseModel {
  cateReportProduceCrafttAncolForEconomicId: string,
  typeofWine: string, //Loại rượu
  quantity: number | null, //Sản lượng
  businessId: string,
  quantityConsume: number | null, //Sản lượng tiêu thụ
  yearReport: number,
}
