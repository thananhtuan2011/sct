using System;
using System.Collections.Generic;

namespace EF_Core.Models
{
    public partial class ProposedPowerProject
    {
        /// <summary>
        /// Quản lý dự án nguồn điện đang đề xuất
        /// </summary>
        public Guid ProposedPowerProjectId { get; set; }
        /// <summary>
        /// Id lĩnh vực
        /// </summary>
        public Guid EnergyIndustryId { get; set; }
        /// <summary>
        /// Tên dự án
        /// </summary>
        public string ProjectName { get; set; } = null!;
        /// <summary>
        /// Id trạng thái - Lấy từ config
        /// </summary>
        public Guid StatusId { get; set; }
        /// <summary>
        /// Nhà đầu tư
        /// </summary>
        public string InvestorName { get; set; } = null!;
        /// <summary>
        /// Văn bản pháp ly
        /// </summary>
        public string PolicyDecision { get; set; } = null!;
        /// <summary>
        /// Công suất
        /// </summary>
        public int Wattage { get; set; }
        /// <summary>
        /// Vị trí
        /// </summary>
        public string Address { get; set; } = null!;
        /// <summary>
        /// Ghi chú
        /// </summary>
        public string? Note { get; set; }
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
