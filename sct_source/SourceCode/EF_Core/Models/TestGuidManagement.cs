using System;
using System.Collections.Generic;

namespace EF_Core.Models
{
    public partial class TestGuidManagement
    {
        public Guid TestGuidManagementId { get; set; }
        public string InspectionAgency { get; set; } = null!;
        public DateTime Time { get; set; }
        public string CoordinationAgency { get; set; } = null!;
        public string Result { get; set; } = null!;
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
