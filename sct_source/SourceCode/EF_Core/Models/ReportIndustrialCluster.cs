using System;
using System.Collections.Generic;

namespace EF_Core.Models
{
    public partial class ReportIndustrialCluster
    {
        public Guid ReportIndustrialClustersId { get; set; }
        /// <summary>
        /// Kỳ báo cáo, 1: 6 tháng, 2: 1 năm
        /// </summary>
        public int ReportingPeriod { get; set; }
        public int Year { get; set; }
        /// <summary>
        /// 1: Phương án phát triển cụm công nghiệp
        /// 2: Thành lập, đầu tư xây dựng hạ tầng kỹ thuật cụm công nghiệp
        /// 3: Hoạt động của các cụm công nghiệp
        /// </summary>
        public int TypeReport { get; set; }
        /// <summary>
        /// Chỉ tiêu, lấy từ bảng CateCriteria
        /// </summary>
        public Guid Criteria { get; set; }
        public Guid? GroupId { get; set; }
        /// <summary>
        /// đơn vị tính
        /// </summary>
        public string Units { get; set; } = null!;
        public int Quantity { get; set; }
        public string? Note { get; set; }
        public Guid? District { get; set; }
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
        /// <summary>
        /// Người tạo
        /// </summary>
        public Guid CreateUserId { get; set; }
        public DateTime? UpdateTime { get; set; }
        public Guid? UpdateUserId { get; set; }
    }
}
