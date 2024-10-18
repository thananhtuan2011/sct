import { BaseModel } from '../../../../_metronic/shared/crud-table/models/base.model';

export interface ListOfKeyEnergyUsersModel extends BaseModel {
  listOfKeyEnergyUsersId: string;
  businessId: string;
  address: string;
  date: any;
  link: string;
  profession: string;
  manufactProfession: string;
  energyConsumption: number;
  note: string;
  district: string;
  decision: string;
}
