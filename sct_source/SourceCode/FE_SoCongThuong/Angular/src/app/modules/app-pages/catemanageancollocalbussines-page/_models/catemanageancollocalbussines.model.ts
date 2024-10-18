import { BaseModel } from '../../../../_metronic/shared/crud-table/models/base.model';

export interface CateManageAncolLocalBussinesModel extends BaseModel {
  cateManageAncolLocalBussinessId : string;
  businessId : string; //Tên doanh nghiệp
  investment : any; //Vốn điều lệ
  numberOfWorker : any; //Số nhân viên
  typeOfProfessionId: string; //Nghành nghề kinh doanh chính
  lstProfession: Array<any>; //List ngành nghề phụ
  dateRelease: any | null; //Ngày cấp
  dateChange: any | null; //Ngày đăng ký thay đổi
  lstWorkers: Array<any>; //List danh sách thành viên góp vốn, cổ đông
  isActive: boolean | null; //Trạng thái 1: Hoạt động 0: Không hoạt động
}
