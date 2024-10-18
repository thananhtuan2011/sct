using System;
using System.Collections.Generic;

namespace EF_Core.Models
{
    public partial class ProcessAdministrativeProcedure
    {
        /// <summary>
        /// Quy trình nội bộ giải quyết thủ tục hành chính - Thủ tục hành chính - Sở thanh tra
        /// </summary>
        public Guid ProcessAdministrativeProceduresId { get; set; }
        /// <summary>
        /// Lĩnh vực giải quyết
        /// </summary>
        public Guid ProcessAdministrativeProceduresField { get; set; }
        /// <summary>
        /// Mã quy trình
        /// </summary>
        public string ProcessAdministrativeProceduresCode { get; set; } = null!;
        /// <summary>
        /// Tên quy trình
        /// </summary>
        public string ProcessAdministrativeProceduresName { get; set; } = null!;
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
