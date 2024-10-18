import { BaseModel } from '../../../../_metronic/shared/crud-table/models/base.model';

export interface IndustrialPromotionProjectModel extends BaseModel {
  industrialPromotionProjectId : string;
  projectName : string;
  startDate: string; //Thời gian bắt đầu thực hiện
  endDate: string; //Thời gian kết thúc
  capital: any; //nguồn vốn
  funding: any; //tổng kinh phí
  industrialPromotionFunding: any; //kinh phí khuyến công hỗ trợ
  reciprocalEnterpriseFunding: any; //Kinh phí doanh nghiệp đối ứng
  details: any;
}
