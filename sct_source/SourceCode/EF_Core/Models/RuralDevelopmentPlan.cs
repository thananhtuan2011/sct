using System;
using System.Collections.Generic;

namespace EF_Core.Models
{
    public partial class RuralDevelopmentPlan
    {
        /// <summary>
        /// Kế hoạch phát triển chợ nông thôn
        /// </summary>
        public Guid RuralDevelopmentPlanId { get; set; }
        /// <summary>
        /// Tên TTTM / Siêu thị
        /// </summary>
        public string SuperMarketShoppingMallName { get; set; } = null!;
        /// <summary>
        /// Địa chỉ
        /// </summary>
        public string Address { get; set; } = null!;
        /// <summary>
        /// Tổng vốn đầu tư
        /// </summary>
        public long? TotalInvestment { get; set; }
        /// <summary>
        /// Ngân sách
        /// </summary>
        public long? Budget { get; set; }
        /// <summary>
        /// Ngoài ngân sách
        /// </summary>
        public long? OutOfBudget { get; set; }
        /// <summary>
        /// Loại hình: 0 - Xây dựng, 1 - Nâng cấp
        /// </summary>
        public int? Type { get; set; }
        /// <summary>
        /// ///Giai đoạn
        /// </summary>
        public Guid? StageId { get; set; }
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
