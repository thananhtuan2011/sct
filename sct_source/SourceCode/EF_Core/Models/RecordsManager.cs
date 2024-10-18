using System;
using System.Collections.Generic;

namespace EF_Core.Models
{
    public partial class RecordsManager
    {
        public Guid RecordsManagerId { get; set; }
        public Guid RecordsFinancePlanId { get; set; }
        public string CodeFile { get; set; } = null!;
        public string Title { get; set; } = null!;
        public DateTime ReceptionTime { get; set; }
        public decimal StorageTime { get; set; }
        public string Creator { get; set; } = null!;
        public string? Note { get; set; }
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
