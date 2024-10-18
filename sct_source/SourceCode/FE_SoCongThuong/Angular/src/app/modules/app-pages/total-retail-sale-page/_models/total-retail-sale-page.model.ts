import { BaseModel } from '../../../../_metronic/shared/crud-table/models/base.model';

export interface TotalRetailSaleModel extends BaseModel {
  totalRetailSaleId: string;
  target: number;
  month: any;
  reportIndex: number;
}
