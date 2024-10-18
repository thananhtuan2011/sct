using System;
using System.Collections.Generic;

namespace EF_Core.Models
{
    public partial class ParticipateSupportFair
    {
        public Guid ParticipateSupportFairId { get; set; }
        public string ParticipateSupportFairName { get; set; } = null!;
        public Guid? DistrictId { get; set; }
        public Guid? CommuneId { get; set; }
        public string Address { get; set; } = null!;
        public Guid Country { get; set; }
        public string Scale { get; set; } = null!;
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        /// <summary>
        /// 1: Sở tham gia
        /// 2: Hỗ trợ doanh nghiệp tham gia
        /// </summary>
        public int PlanJoin { get; set; }
        /// <summary>
        /// Kinh phí thực hiện
        /// </summary>
        public long ImplementCost { get; set; }
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
