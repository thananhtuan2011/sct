import { BaseModel } from '../../../../_metronic/shared/crud-table/models/base.model';

export interface ManageArchiveRecordsModel extends BaseModel {
  manageArchiveRecordsId: string;
  recordsFinancePlanId: number;
  codeFile: string;
  title: string;
  receptionTime: any;
  storageTime: number;
  location: string;
  storeDocumentsAt: string;
  storeFilesAt: string;
  creator:string;
  note: string;
}
