import { BaseModel } from '../../../../_metronic/shared/crud-table/models/base.model';

export interface MonthReportModel extends BaseModel {
  monthReportId: string;
  updateDate: string;
  month: string;
  year: string;
  lFiles: any;
}
