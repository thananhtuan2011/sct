import { BaseModel } from '../../../../_metronic/shared/crud-table/models/base.model';

export interface UnitsModel extends BaseModel {
  unitId :string;
  unitName: string;
  unitNameEn: string | null;
  unitCode: string | null;
  exchange: number | null;
  note: string | null;
}
