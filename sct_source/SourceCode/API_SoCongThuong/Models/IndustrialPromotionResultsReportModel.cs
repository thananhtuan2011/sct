using System;
using System.Collections.Generic;

namespace API_SoCongThuong.Models
{
    public partial class IndustrialPromotionResultsReportModel
    {
        public Guid RpIndustrialPromotionResultsId { get; set; }
        /// <summary>
        /// Khuyến công quốc gia
        /// </summary>
        public int YearReport { get; set; }
        /// <summary>
        /// Khuyến công quốc gia
        /// </summary>
        public int NationalReport { get; set; }
        /// <summary>
        /// Khuyến công địa phương
        /// </summary>
        public int LocalReport { get; set; }
        /// <summary>
        /// Chỉ tiêu
        /// </summary>
        public Guid Targets { get; set; }
        /// <summary>
        /// Đơn vị tính
        /// </summary>
        public string? Unit { get; set; }
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
        public DateTime DateRelease { get; set; }
        public Guid? UpdateUserId { get; set; }
        public string? TargetsName { get; set; } = "";
    }
}
