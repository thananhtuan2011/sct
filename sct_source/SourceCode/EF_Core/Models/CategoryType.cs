using System;
using System.Collections.Generic;

namespace EF_Core.Models
{
    public partial class CategoryType
    {
        public Guid CategoryTypeId { get; set; }
        /// <summary>
        /// Mã nhóm
        /// </summary>
        public string CategoryTypeCode { get; set; } = null!;
        /// <summary>
        /// Tên nhóm
        /// </summary>
        public string CategoryTypeName { get; set; } = null!;
        /// <summary>
        /// Mô tả
        /// </summary>
        public string? Description { get; set; }
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
