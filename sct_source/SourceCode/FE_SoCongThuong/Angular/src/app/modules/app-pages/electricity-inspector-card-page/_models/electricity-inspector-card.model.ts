import { BaseModel } from '../../../../_metronic/shared/crud-table/models/base.model';

export interface ElectricityInspectorCardModel extends BaseModel {
  electricityInspectorCardId: string,
  inspectorName: string,
  birthday: any,
  licenseDate: any,
  degree: string,
  unit: string,
  seniority: string,
  cardColor: string,
}
