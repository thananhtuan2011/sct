import { BaseModel } from '../../../../_metronic/shared/crud-table/models/base.model';

export interface ReportAdministrativeProceduresModel extends BaseModel {
    //Id của báo cáo
    reportId: string,
    //Id Quý
    period: string,
    //Năm
    year: number,
    //Id lĩnh vực
    administrativeProceduresField: string,
    //Tổng tiếp nhận
    totalReceive: number,
    //Online trong kỳ
    onlineInPeriod: number,
    //Offline trong kỳ
    offlineInPeriod: number,
    //Từ kỳ trước
    fromPreviousPeriod: number,
    //Tổng - đã giải quyết
    totalProcessed: number,
    //Đúng hạn - đã giải quyết
    onTimeProcessed: number,
    //Quá hạn - đã giải quyết
    outOfDateProcessed: number,
    //Trước hạn - đã giải quyết
    beforeDeadlineProcessed: number,
    //Tổng - đang xử lý
    totalProcessing: number,
    //Trong hạng - đang xử lý
    onTimeProcessing: number,
    //Quá hạn - đang xử lý
    outOfDateProcessing: number,
}


