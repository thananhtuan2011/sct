using System;
using System.Collections.Generic;

namespace EF_Core.Models
{
    public partial class ReportPromotionCommerce
    {
        public Guid ReportPromotionCommerceId { get; set; }
        /// <summary>
        /// đơn vị chủ trì
        /// </summary>
        public string Host { get; set; } = null!;
        public Guid Business { get; set; }
        /// <summary>
        /// thủ trưởng đơn vị
        /// </summary>
        public string Chief { get; set; } = null!;
        public string ProjectName { get; set; } = null!;
        /// <summary>
        /// đơn vị tổ chức
        /// </summary>
        public string Organize { get; set; } = null!;
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        /// <summary>
        /// địa điểm
        /// </summary>
        public string Location { get; set; } = null!;
        /// <summary>
        /// quy mô
        /// </summary>
        public string Scale { get; set; } = null!;
        public string? ResultNote { get; set; }
        public string? Note { get; set; }
        public string Represent { get; set; } = null!;
        public string Position { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string Fax { get; set; } = null!;
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
