using System;
using System.Collections.Generic;

namespace API_SoCongThuong.Models
{
    public partial class CateRetailModel
    {
        /// <summary>
        /// Bảng danh mục bán lẻ
        /// </summary>
        public Guid CateRetailId { get; set; }
        public string? CateRetailCode { get; set; }
        public Guid? CheckUserId { get; set; }
        public string CheckName { get; set; } = "";
        public Guid? ConfirmUserId { get; set; }
        public string ConfirmName { get; set; } = "";
        public DateTime? ConfirmTime { get; set; }
        public string ConfirmTimeDisplay { get; set; } = "";
        /// <summary>
        /// Thời gian tạo
        /// </summary>
        public DateTime CreateTime { get; set; }
        public string CreateTimeDisplay { get; set; } = "";
        /// <summary>
        /// Người tạo
        /// </summary>
        public Guid CreateUserId { get; set; }
        public string CreateName { get; set; } = "";
        public DateTime? UpdateTime { get; set; }
        public string UpdateTimeDisplay { get; set; } = "";
        public Guid? UpdateUserId { get; set; }
        public string UpdateName { get; set; } = "";
        public bool? IsAction { get; set; }
        public List<CateRetailDetailModel> Details { get; set; } = new List<CateRetailDetailModel>();
        public string? ReportMonth { get; set; }
    }

    public partial class CateRetailDetailModel
    {
        public Guid? CateRetailDetailId { get; set; }
        //public Guid? CateCriteriaId { get; set; }
        public Guid Criteria { get; set; }
        public string CriteriaName { get; set; }
        /// <summary>
        /// Thực hiện tháng trước
        /// </summary>
        public long? PerformLastmonth { get; set; }
        /// <summary>
        /// Ước tính báo cáo tháng
        /// </summary>
        public long? EstimateReportingMonth { get; set; }
        /// <summary>
        /// Cộng dồn từ đầu năm đến tháng báo cáo
        /// </summary>
        public long? CumulativeToReportingMonth { get; set; }
        /// <summary>
        /// Thực hiện báo cáo
        /// </summary>
        public long? PerformReporting { get; set; }
        /// <summary>
        /// 1: Tổng mức bán lẻ hàng hóa theo năm báo cáo
        /// 2: Tổng mức bán lẻ hàng hóa theo năm trước
        /// </summary>
        public int? Type { get; set; }
        public bool IsDel { get; set; } = false;
    }
}
