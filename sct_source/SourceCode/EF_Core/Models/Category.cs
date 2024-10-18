using System;
using System.Collections.Generic;

namespace EF_Core.Models
{
    public partial class Category
    {
        public Guid CategoryId { get; set; }
        public string CategoryTypeCode { get; set; } = null!;
        public string CategoryCode { get; set; } = null!;
        public string CategoryName { get; set; } = null!;
        /// <summary>
        /// 1: Hoạt động 0: Không hoạt động
        /// </summary>
        public bool? IsAction { get; set; }
        public int Piority { get; set; }
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
        /// <summary>
        /// Thời gian chỉnh sửa
        /// </summary>
        public DateTime? UpdateTime { get; set; }
        /// <summary>
        /// Người chỉnh sửa
        /// </summary>
        public Guid? UpdateUserId { get; set; }
    }
}
