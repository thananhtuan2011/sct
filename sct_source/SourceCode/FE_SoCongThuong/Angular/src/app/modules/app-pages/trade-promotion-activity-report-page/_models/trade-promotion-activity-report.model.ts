import { BaseModel } from '../../../../_metronic/shared/crud-table/models/base.model';

export interface TradePromotionActivityReportModel extends BaseModel {
  tradePromotionActivityReportId: string;
  scaleId: string; //Id Quy mô
  planName: string; //Tên đề án
  planToJoin: boolean; //Kế hoạch tham gia
  startDate: string; //Thời gian bắt đầu
  endDate: string; //Thời gian kết thúc
  time: string; //Thời lượng
  districtId: string; //Huyện
  address: string; //Địa điểm
  implementationCost: number; //kinh phí thực hiện
  fundingSupport: number; //Kinh phí hổ trợ
  scale: string; //Quy mô
  numParticipating: number; //Số lượng doanh nghiệp tham gia
  participatingBusinesses: any[]; //Doanh nghiệp tham gia
  note: string; //Ghi chú
}