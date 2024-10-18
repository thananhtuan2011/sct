import { BaseModel } from '../../../../_metronic/shared/crud-table/models/base.model';

export interface RooftopSolarProjectManagementModel extends BaseModel {
  rooftopSolarProjectManagementId: string;
  projectName: string; //Tên dự án
  investorName: string; //Chủ đầu tư
  address: string; //Vị trí
  area: number | null; //Diện tích
  surveyPolicy: string; //Chủ trương khảo
  wattage: number | null; //Công suất
  progress: string; //Tiến độ
  district: string;
  operationDay: any;
}
