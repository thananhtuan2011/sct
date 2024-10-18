import { BaseModel } from '../../../../_metronic/shared/crud-table/models/base.model';

export interface CateIndustrialClusterModel extends BaseModel {
  cateIndustrialClustersId : string;
  industrialClustersName: string; //Tên cụm công nghiệp
  originalArea: number | null; //Diện tích thành lập
  establishCode: string; //Quyết định thành lập
  expandedArea: number | null; //Diện tích mở rộng
  decisionExpandCode: string; //Quyết định mở rộng
  detailedArea: number | null; //Diện tích QH chi tiết
  approvalDecision: string; //Quyết định phê duyệt QHCT
  industrialArea: number | null; //Diện tích đất công nghiệp
  rentedArea: number | null; //Diện tích đất cho thuê
  occupancy: number | null; //Tỉ lệ lấp đấy
  note: string; //Ghi chú
  district: string;
}
