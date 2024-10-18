import { BaseModel } from '../../../../_metronic/shared/crud-table/models/base.model';

export interface GasModel extends BaseModel {
  gasId: string;
  code: string;
  name: string;
}
