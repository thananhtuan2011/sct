import { BaseModel } from '../../../../_metronic/shared/crud-table/models/base.model';

export interface EnvironmentProjectManagementModel extends BaseModel {
  environmentProjectManagementId : string;
  projectName : string;
  implementationContent : any;
  approvedFunding : number | null;
  implementationCost : number | null;
  coordinationUnit : string;
  yearOfImplementationFrom : number;
  yearOfImplementationTo : number;
}
