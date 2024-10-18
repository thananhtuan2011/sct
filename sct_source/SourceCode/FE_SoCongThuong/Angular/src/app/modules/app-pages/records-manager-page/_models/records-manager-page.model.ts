import { BaseModel } from '../../../../_metronic/shared/crud-table/models/base.model';

export interface RecordsManagerModel extends BaseModel {
  recordsManagerId: string;
  recordsFinancePlanId: string;
  codeFile: string;
  title: string;
  receptionTime: any;
  storageTime: number;
  creator:string;
  note: string;
}
