using System;
using System.Collections.Generic;

namespace EF_Core.Models
{
    public partial class RooftopSolarProjectManagement
    {
        /// <summary>
        /// Quản lý dự án điện mặt trời áp mái
        /// </summary>
        public Guid RooftopSolarProjectManagementId { get; set; }
        /// <summary>
        /// Tên dự án
        /// </summary>
        public string ProjectName { get; set; } = null!;
        /// <summary>
        /// Chủ đầu tư
        /// </summary>
        public string InvestorName { get; set; } = null!;
        /// <summary>
        /// Vị trí
        /// </summary>
        public string Address { get; set; } = null!;
        /// <summary>
        /// Diện tích
        /// </summary>
        public decimal Area { get; set; }
        /// <summary>
        /// Chủ trương khảo
        /// </summary>
        public string SurveyPolicy { get; set; } = null!;
        /// <summary>
        /// Công suất
        /// </summary>
        public decimal Wattage { get; set; }
        /// <summary>
        /// Tiến độ
        /// </summary>
        public string Progress { get; set; } = null!;
        public Guid? District { get; set; }
        public DateTime? OperationDay { get; set; }
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
