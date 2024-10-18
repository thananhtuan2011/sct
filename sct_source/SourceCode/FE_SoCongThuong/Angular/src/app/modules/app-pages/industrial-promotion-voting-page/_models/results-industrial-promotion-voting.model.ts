import { BaseModel } from '../../../../_metronic/shared/crud-table/models/base.model';

export interface ResultsIndustrialPromotionVotingModel extends BaseModel {

  resultsIndustrialPromotionVotingId: string,
  locallity: boolean|null; //Địa bàn
  numbersRegister: number | null, //Số lượng sản phẩm đăng ký
  numberCertified: number | null, //Số lượng sản phẩm được cấp giấy chứng nhận
  targets: string,  //Số sản phẩm đăng ký tham gia bình chọn
  unit: string,
}
