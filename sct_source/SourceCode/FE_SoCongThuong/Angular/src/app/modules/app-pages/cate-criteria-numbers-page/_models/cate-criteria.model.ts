import { BaseModel } from '../../../../_metronic/shared/crud-table/models/base.model';

export interface CateCriteriaNumberSevenModel extends BaseModel {
  cateCriteriaNumberSevenCode : string;
  cateCriteriaNumberSevenId : string;
  checkUserId : string;
  confirmUserId : string;
  createUserId : string;
  confirmTime : any;
  createTime : any;
  reportMonth: any;
  details:[{
    cateCriteriaNumberSevenId:string,
    districtId:string,
    districtName : string,
    numberOfWard : number;
    numberOfQualifyingWard : number,
    numberOfWardWithMarket : number;
    numberOfWardCommercialInfrastructure : number,
    numberOfWardCommercialInfrastructureEstimate : number,
    numberOfWardCommercialInfrastructurePlan : number,
    numberOfWardNewCountryside : number,
    numberOfWardNewCountrysideEstimate : number,
    numberOfWardNewCountrysidePlan : number,
  }];

}
export interface CateCriteriaNumberSevenDetailModel extends BaseModel {
  // CigaretteBusinessId : string;
  cateCriteriaNumberSevenId : string;
  districtId : string;
  districtName : string;
  numberOfWard : number;
  numberOfQualifyingWard : number;
  numberOfWardWithMarket : number;
  numberOfWardCommercialInfrastructure : number;
  numberOfWardCommercialInfrastructureEstimate : number;
  numberOfWardCommercialInfrastructurePlan : number;
  numberOfWardNewCountryside : number;
  numberOfWardNewCountrysideEstimate : number;
  numberOfWardNewCountrysidePlan : number;
}

