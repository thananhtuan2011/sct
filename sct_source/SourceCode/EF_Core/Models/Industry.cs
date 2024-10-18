using System;
using System.Collections.Generic;

namespace EF_Core.Models
{
    public partial class Industry
    {
        public Guid IndustryId { get; set; }
        /// <summary>
        /// @Mã ngành Nghề@ $exist$
        /// </summary>
        public string IndustryCode { get; set; } = null!;
        /// <summary>
        /// @Tên ngành Nghề@
        /// </summary>
        public string IndustryName { get; set; } = null!;
        /// <summary>
        /// @Ngành nghề cha *@ #IndustryName#
        /// </summary>
        public Guid? ParentIndustryId { get; set; }
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
