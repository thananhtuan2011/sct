using System;
using System.Collections.Generic;

namespace EF_Core.Models
{
    public partial class ManagementFirePrevention
    {
        /// <summary>
        /// Quản lý công tác phòng chống cháy nổ thuộc ngành công thương
        /// </summary>
        public Guid ManagementFirePreventionId { get; set; }
        /// <summary>
        /// Tên doanh nghiệp
        /// </summary>
        public string BusinessName { get; set; } = null!;
        /// <summary>
        /// Địa chỉ
        /// </summary>
        public string Address { get; set; } = null!;
        /// <summary>
        /// Thực trạng: 2 - Tốt, 1 - Trung Bình, 0 - Không đạt
        /// </summary>
        public int Reality { get; set; }
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
