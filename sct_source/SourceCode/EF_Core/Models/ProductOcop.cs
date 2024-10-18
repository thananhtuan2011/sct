using System;
using System.Collections.Generic;

namespace EF_Core.Models
{
    public partial class ProductOcop
    {
        public Guid ProductOcopid { get; set; }
        public string ProductName { get; set; } = null!;
        public string ProductOwner { get; set; } = null!;
        /// <summary>
        /// Số điện thoại
        /// </summary>
        public string? PhoneNumber { get; set; }
        public string Address { get; set; } = null!;
        /// <summary>
        /// Thành phần
        /// </summary>
        public string Ingredients { get; set; } = null!;
        /// <summary>
        /// Hạn sử dụng
        /// </summary>
        public int Expiry { get; set; }
        /// <summary>
        /// Quyết định phê duyệt
        /// </summary>
        public string? ApprovalDecision { get; set; }
        /// <summary>
        /// Bảo Quản
        /// </summary>
        public string Preserve { get; set; } = null!;
        public int Ratings { get; set; }
        /// <summary>
        /// Huyện
        /// </summary>
        public Guid DistrictId { get; set; }
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
