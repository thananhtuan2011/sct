import { BaseModel } from '../../../../_metronic/shared/crud-table/models/base.model';

export interface ExportGoodsModel extends BaseModel {
  exportGoodsId : string;
  exportGoodsName : string; //Tên mặt hàng
  itemGroupId: string; //Nhóm mặt hàng
  typeOfEconomicId: string; //Loại hình kinh tế
  businessId: string; //Doanh nghiệp
  countryId: string; //Thị trường
  amount: string ; //Số lượng
  amountUnitId: string; //Đơn vị tính
  price: number | null; //Trị giá
  exportTime: string; //Thời gian nhập khẩu
}