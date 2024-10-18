using System;
using System.Collections.Generic;

namespace EF_Core.Models
{
    public partial class GasBusiness
    {
        public Guid GasBusinessId { get; set; }
        public byte TypeBusiness { get; set; }
        public Guid BusinessName { get; set; }
        public Guid GasId { get; set; }
        public string Licensors { get; set; } = null!;
        public string? Fax { get; set; }
        public string? NumDoc { get; set; }
        public DateTime? DateStart { get; set; }
        public DateTime? DateEnd { get; set; }
        public Guid? ComplianceStatus { get; set; }
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
