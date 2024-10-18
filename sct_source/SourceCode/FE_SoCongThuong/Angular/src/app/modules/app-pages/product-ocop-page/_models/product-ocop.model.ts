import { BaseModel } from '../../../../_metronic/shared/crud-table/models/base.model';

export interface ProductOcopModel extends BaseModel {
  productOcopid: string;
  productName: string; //Tên sản phẩm
  productOwner: string; //Chủ thể sản xuất
  phoneNumber: string; //Số điện thoại
  districtId: string; //Huyện
  address: string; //Địa chỉ
  ingredients: string; //Thành phần
  expiry: number | null; //Hạn sử dụng
  preserve: string; //Bảo quản
  approvalDecision: string; //Quyết định phê duyệt
  ratings: number; //Số sao đánh giá
}
