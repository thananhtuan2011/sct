import { BaseModel } from '../../../../_metronic/shared/crud-table/models/base.model';

export interface CateRetailModel extends BaseModel {
  cateRetailCode : string;
  cateRetailId : string;
  checkUserId : any;
  confirmUserId : any;
  createUserId : any;
  confirmTime : any;
  createTime : any;
  reportMonth: string
  details:[{
    criteria:string,
    cumulativeToReportingMonth :number,
    estimateReportingMonth :number,
    performLastmonth :number,
    performReporting :number,
    type :number,
  }];

}
export interface CateRetailDetailModel extends BaseModel {
  // CigaretteBusinessId : string;
  CateRetailId : string;
  estimateReportingMonth : number | null;
  cumulativeToReportingMonth : number | null;
  performReporting : number | null;
  performLastmonth : number | null;
  criteria : string;
  criteriaName : string;
  type : number;
  isDel : boolean;

}

