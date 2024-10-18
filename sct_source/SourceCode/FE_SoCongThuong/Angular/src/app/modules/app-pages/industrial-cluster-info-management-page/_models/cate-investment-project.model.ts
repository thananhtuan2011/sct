import { BaseModel } from '../../../../_metronic/shared/crud-table/models/base.model';

export interface CateInvestmentProjectModel extends BaseModel {
  cateInvestmentProjectId : string;
  investmentType: number | null; //Loại đầu tư 1. Trong nước / 2. Ngoài nước
  businessName : string; //Tên doanh nghiệp
  investment : number | null; //Vốn đăng ký
  numberOfWorker: number | null; //Số lượng nhân viên
  projectArea: number | null; //Diện tích dự án
  quantity: number | null; //Sản lượng (SP / ngày)
  produce: number | null; //Công suất
  productValue: number | null;//Giá trị sản phẩm
  reality: string; //Thực trạng
  owner: string; // chủ doanh nghiệp
  phoneNumber: string;
  career: string; //ngành nghề
  district: string;
}
