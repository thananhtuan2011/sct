import { BaseModel } from '../../../../_metronic/shared/crud-table/models/base.model';

export interface BuildAndUpgradeModel extends BaseModel {
  buildAndUpgradeId: string, //Id
  buildAndUpgradeName: string, //Tên danh mục
  address: string, //Địa chỉ
  commercialId: string;
  districtId: string,
  communeId: string,
  totalInvestment : number | null, //Tổng vốn đầu tư
  realizedCapital: number| null, // Vốn đã thực hiện
  budgetCapital: number| null, //Vốn ngân sách
  landUseCapital: number| null, // Vốn chính quyền sử dụng đất
  loans: number| null, //Vốn vay
  anotherCapital: number| null, //Vốn khác
  isBuild: boolean,  //Có phải xây dựng không
  isUpgrade: boolean, //Có phải nâng cấp không
  note: string, //Ghi chú

  //Unit
  totalInvestmentUnit: string,
  realizedCapitalUnit: string,
  budgetCapitalUnit: string,
  landUseCapitalUnit: string,
  loansUnit: string,
  anotherCapitalUnit: string,
  year: any;
}
