using System;
using System.Collections.Generic;

namespace EF_Core.Models
{
    public partial class User
    {
        public Guid UserId { get; set; }
        /// <summary>
        /// @import Tên người dùng
        /// </summary>
        public string UserName { get; set; } = null!;
        /// <summary>
        /// @Họ và tên@
        /// </summary>
        public string FullName { get; set; } = null!;
        /// <summary>
        /// 1: Hoạt động 0: Không hoạt động
        /// </summary>
        public bool? IsAction { get; set; }
        /// <summary>
        /// 1: Đã xóa; 0: Chưa xóa
        /// </summary>
        public bool? IsDel { get; set; }
        /// <summary>
        /// Thời gian tạo
        /// </summary>
        public DateTime? CreateTime { get; set; }
        /// <summary>
        /// Người tạo
        /// </summary>
        public Guid? CreateUserId { get; set; }
        public DateTime? UpdateTime { get; set; }
        public Guid? UpdateUserId { get; set; }
        public Guid? RoleId { get; set; }
        public Guid? DeptId { get; set; }
        public string? Avatar { get; set; }
        public string? Password { get; set; }
        public string? Email { get; set; }
        public string? Cccd { get; set; }
        public string? Phone { get; set; }
        public int? Status { get; set; }
        /// <summary>
        /// Cấp: 0 - Cấp Tỉnh, 1 - Cấp Huyện, 2 - Cấp Xã.
        /// </summary>
        public int LevelUser { get; set; }
        /// <summary>
        /// ID của xã huyện tỉnh dùng cột LevelUser để xác định bảng Join.
        /// </summary>
        public Guid AreaId { get; set; }
        public Guid? GroupUserId { get; set; }
        public bool? IsAdmin { get; set; }
        public int CountLoginFail { get; set; }
        public DateTime? TimeLock { get; set; }
    }
}
