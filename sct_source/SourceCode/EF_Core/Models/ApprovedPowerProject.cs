using System;
using System.Collections.Generic;

namespace EF_Core.Models
{
    public partial class ApprovedPowerProject
    {
        /// <summary>
        /// Quản lý dự án nguồn điện được phê duyệt
        /// </summary>
        public Guid ApprovedPowerProjectId { get; set; }
        public Guid EnergyIndustryId { get; set; }
        /// <summary>
        /// Tên dự án
        /// </summary>
        public string ProjectName { get; set; } = null!;
        /// <summary>
        /// Nhà đầu tư
        /// </summary>
        public string InvestorName { get; set; } = null!;
        /// <summary>
        /// Huyện
        /// </summary>
        public Guid DistrictId { get; set; }
        /// <summary>
        /// Địa chỉ
        /// </summary>
        public string Address { get; set; } = null!;
        /// <summary>
        /// Quyết định chủ trương
        /// </summary>
        public string PolicyDecision { get; set; } = null!;
        /// <summary>
        /// Công suất
        /// </summary>
        public int Wattage { get; set; }
        /// <summary>
        /// Số lượng tuabin
        /// </summary>
        public int Turbines { get; set; }
        /// <summary>
        /// Trạm biến áp
        /// </summary>
        public int Substation { get; set; }
        /// <summary>
        /// Sản lượng điện phát
        /// </summary>
        public int? PowerOutput { get; set; }
        /// <summary>
        /// Diện tích
        /// </summary>
        public decimal Area { get; set; }
        public int Year { get; set; }
        public Guid Status { get; set; }
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
