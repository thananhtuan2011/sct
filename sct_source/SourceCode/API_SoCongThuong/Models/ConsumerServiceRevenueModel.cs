using System;
using System.Collections.Generic;

namespace API_SoCongThuong.Models
{
    public partial class ConsumerServiceRevenueModel
    {
        /// Bảng danh mục bán lẻ
        public Guid ConsumerServiceRevenueId { get; set; }
        public string? ConsumerServiceRevenueCode { get; set; }
        public Guid? CheckUserId { get; set; }
        public string CheckName { get; set; } = "";
        public Guid? ConfirmUserId { get; set; }
        public string ConfirmName { get; set; } = "";
        public DateTime? ConfirmTime { get; set; }
        public string ConfirmTimeDisplay { get; set; } = "";
        /// Thời gian tạo
        public DateTime CreateTime { get; set; }
        public string CreateTimeDisplay { get; set; } = "";
        /// Người tạo
        public Guid CreateUserId { get; set; }
        public string CreateName { get; set; } = "";
        public DateTime? UpdateTime { get; set; }
        public string UpdateTimeDisplay { get; set; } = "";
        public Guid? UpdateUserId { get; set; }
        public string UpdateName { get; set; } = "";
        public bool? IsAction { get; set; }
        public List<ConsumerServiceRevenueDetailModel> Details { get; set; } = new List<ConsumerServiceRevenueDetailModel>();
        public string? ReportMonth { get; set; }
    }

    public partial class ConsumerServiceRevenueDetailModel
    {
        public Guid? ConsumerServiceRevenueDetailId { get; set; }
        public Guid Criteria { get; set; }
        public string CriteriaName { get; set; }
        /// Thực hiện tháng trước
        public long? PerformLastmonth { get; set; }
        /// Ước tính báo cáo tháng
        public long? EstimateReportingMonth { get; set; }
        /// Cộng dồn từ đầu năm đến tháng báo cáo
        public long? CumulativeToReportingMonth { get; set; }
        /// Thực hiện báo cáo
        public long? PerformReporting { get; set; }
        /// 1: Tổng mức bán lẻ hàng hóa theo năm báo cáo
        /// 2: Tổng mức bán lẻ hàng hóa theo năm trước
        public int? Type { get; set; }
        public bool IsDel { get; set; } = false;
    }
}
