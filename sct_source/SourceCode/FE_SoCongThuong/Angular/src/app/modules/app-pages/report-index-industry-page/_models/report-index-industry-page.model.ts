import { BaseModel } from '../../../../_metronic/shared/crud-table/models/base.model';

export interface ReportIndexIndustryModel extends BaseModel {
  reportIndexIndustryId: string;
  target: number;
  reportIndex: number;
  month: any;
  dataTarget: Array<any>;
  comparedPreviousMonth: number;
  samePeriod: number;
  accumulation: number;
}
