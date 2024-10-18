import { BaseModel } from '../../../../_metronic/shared/crud-table/models/base.model';

export interface CateRPSAncolForFactoryModel extends BaseModel {
  cateReportSoldAncolForFactoryLicenseId: string,
  businessId: string,
  typeofWine: string, //Loại rượu
  quantity: number | null, //Sản lượng
  wineFactory: string, //Nhà máy mua rượu để chế biến lại
  yearReport: number,
}
