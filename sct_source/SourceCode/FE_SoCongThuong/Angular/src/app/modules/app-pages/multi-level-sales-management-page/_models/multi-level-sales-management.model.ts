import { BaseModel } from '../../../../_metronic/shared/crud-table/models/base.model';

export interface MultiLevelSalesManagementModel extends BaseModel {
    //bán hàng đa cấp
    multiLevelSalesManagementId: string,
    
    //Cơ sở hoạt động
    businessId: string, //Doanh nghiệp
    startDate: any, //Ngày bắt đầu hoạt động
    yearReport: number //Năm báo cáo
    multiLevelSellingPlace: string, //Địa điểm hoạt động bán hàng đa cấp
    contactPersonName: string, //Người liên hệ
    contactPersonPhoneNumber: string, //Số điện thoại
    contactPersonAddress: string, //Địa chỉ người liên hệ

    //Kết quả hoạt động
    participants: number | null, //Số người tham gia bán hàng đa cấp
    newParticipants: number| null, //Số người tham gia bán hàng đa cấp phát sinh thêm
    terminations: number| null, //Số người tham gia bán hàng đa cấp kết thúc hợp đồng
    basicTrainings: number| null, //Số lượng đào tạo căn bản
    turnover: number| null, //Doanh thu bán hàng đa cấp trên địa bàn (Triệu đồng)
    commission: number| null, //Tổng hoa hồng, tiền thưởng, lợi ích kinh tế đã nhận (Triệu đồng)
    promotionalValue: number| null, //Giá trị khuyến mãi quy đổi thành tiền (Triệu đồng)
    taxDeduction: number| null, //Khấu trừ thuế thu nhập cá nhân
    buyBackGoods: number| null, //Mua lại hàng hoá từ người tham gia bán hàng đa cấp (Triệu đồng)
}
