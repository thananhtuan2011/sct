using System;
using System.Collections.Generic;

namespace EF_Core.Models
{
    public partial class StateUnit
    {
        /// <summary>
        /// Bảng đơn vị
        /// </summary>
        public Guid StateUnitsId { get; set; }
        /// <summary>
        /// Mã đơn vị
        /// </summary>
        public string StateUnitsCode { get; set; } = null!;
        /// <summary>
        /// Tên đơn vị
        /// </summary>
        public string StateUnitsName { get; set; } = null!;
        /// <summary>
        /// Id đơn vị cha
        /// </summary>
        public Guid ParentId { get; set; }
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
