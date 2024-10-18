import { BaseModel } from '../../../../_metronic/shared/crud-table/models/base.model';

export interface TypeOfMarketModel extends BaseModel {
  typeOfMarketId : string;
  typeOfMarketCode : string;
  typeOfMarketName : string;
}
