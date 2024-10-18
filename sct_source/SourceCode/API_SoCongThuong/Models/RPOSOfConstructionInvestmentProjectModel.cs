using System;
using System.Collections.Generic;

namespace API_SoCongThuong.Models
{
    public partial class RPOSOfConstructionInvestmentProjectModel
    {
        public Guid ReportOperationalStatusOfConstructionInvestmentProjectsId { get; set; }
        /// <summary>
        /// Kỳ báo cáo, 1: 6 tháng, 2: 1 năm
        /// </summary>
        public int ReportingPeriod { get; set; }
        /// <summary>
        /// Chỉ tiêu, lấy từ bảng CateCriteria
        /// </summary>
        ///
        public int Year { get; set; } = 2023;
        public Guid Criteria { get; set; }
        public string CriteriaName { get; set; } = "";
        /// <summary>
        /// đơn vị tính
        /// </summary>
        public string Units { get; set; } = null!;
        public int Quantity { get; set; }
        public string? Note { get; set; } = "";
        /// <summary>
        /// 1: Hoạt động 0: Không hoạt động
        /// </summary>
        public bool? IsAction { get; set; }
        /// <summary>
        /// 1: Đã xóa; 0: Chưa xóa
        /// </summary>
        public bool IsDel { get; set; }
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
        public Guid? UpdateUserId { get; set; }
    }
}
