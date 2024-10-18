using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace API_SoCongThuong.Models
{
    public class QueryReportAdministrativeProceduresBody
    {
        [JsonPropertyName("filter")]//for derelize of NEWTON.JSON
        [JsonProperty("filter")]//for serilize of NEWTON.JSON
        public Dictionary<string, string> Filter { get; set; }
    }

    public class FieldAdministrativeProceduresModel
    {
        public Guid FieldId { get; set; }
        public string FieldName { get; set; } = null!;
    }

    public class ReturnReportAdministrativeProcedures
    {
        public List<ResultReportAdministrativeProceduresModel> Data { get; set; }
        public TotalReportAdministrativeProceduresModel Total { get; set; }
    }

    public class ResultReportAdministrativeProceduresModel
    {
        //Id lĩnh vực
        public Guid AdministrativeProceduresField { get; set; }
        //Tên lĩnh vực
        public string FieldName { get; set; } = null!;
        //Tổng tiếp nhận
        public int TotalReceive { get; set; }
        //Online trong kỳ
        public int OnlineInPeriod { get; set; }
        //Offline trong kỳ
        public int OfflineInPeriod { get; set; }
        //Từ kỳ trước
        public int FromPreviousPeriod { get; set; }
        //Tổng - đã giải quyết
        public int TotalProcessed { get; set; }
        //Đúng hạn - đã giải quyết
        public int OnTimeProcessed { get; set; }
        //Quá hạn - đã giải quyết
        public int OutOfDateProcessed { get; set; }
        //Trước hạn - đã giải quyết
        public int BeforeDeadlineProcessed { get; set; }
        //Tổng - đang xử lý
        public int TotalProcessing { get; set; }
        //Trong hạng - đang xử lý
        public int OnTimeProcessing { get; set; }
        //Quá hạn - đang xử lý
        public int OutOfDateProcessing { get; set; }
    }

    public class TotalReportAdministrativeProceduresModel
    {
        //Tổng tiếp nhận
        public int TotalReceive { get; set; }
        //Online trong kỳ
        public int OnlineInPeriod { get; set; }
        //Offline trong kỳ
        public int OfflineInPeriod { get; set; }
        //Từ kỳ trước
        public int FromPreviousPeriod { get; set; }
        //Tổng - đã giải quyết
        public int TotalProcessed { get; set; }
        //Đúng hạn - đã giải quyết
        public int OnTimeProcessed { get; set; }
        //Quá hạn - đã giải quyết
        public int OutOfDateProcessed { get; set; }
        //Trước hạn - đã giải quyết
        public int BeforeDeadlineProcessed { get; set; }
        //Tổng - đang xử lý
        public int TotalProcessing { get; set; }
        //Trong hạng - đang xử lý
        public int OnTimeProcessing { get; set; }
        //Quá hạn - đang xử lý
        public int OutOfDateProcessing { get; set; }
    }

    public class ReportAdministrativeProceduresModel
    {
        //Id báo cáo
        public Guid ReportId { get; set; }
        //Id kỳ báo cáo
        public Guid Period { get; set; }
        //Kỳ báo cáo
        public string? PeriodName { get; set; }
        //Năm báo cáo
        public int Year { get; set; }
        //Id lĩnh vực
        public Guid AdministrativeProceduresField { get; set; }
        //Tên lĩnh vực
        public string? FieldName { get; set; }
        //Tổng tiếp nhận
        public int TotalReceive { get; set; } = 0;
        //Online trong kỳ
        public int OnlineInPeriod { get; set; }
        //Offline trong kỳ
        public int OfflineInPeriod { get; set; }
        //Từ kỳ trước
        public int FromPreviousPeriod { get; set; }
        //Tổng - đã giải quyết
        public int TotalProcessed { get; set; } = 0;
        //Đúng hạn - đã giải quyết
        public int OnTimeProcessed { get; set; }
        //Quá hạn - đã giải quyết
        public int OutOfDateProcessed { get; set; }
        //Trước hạn - đã giải quyết
        public int BeforeDeadlineProcessed { get; set; }
        //Tổng - đang xử lý
        public int TotalProcessing { get; set; } = 0;
        //Trong hạng - đang xử lý
        public int OnTimeProcessing { get; set; }
        //Quá hạn - đang xử lý
        public int OutOfDateProcessing { get; set; }
        //Thời gian tạo
        public DateTime? CreateTime { get; set; }
        //Người tạo mới
        public Guid? CreateUserId { get; set; }
        //Thời gian update
        public DateTime? UpdateTime { get; set; }
        //Người chỉnh sửa
        public Guid? UpdateUserId { get; set; }
    }
}
