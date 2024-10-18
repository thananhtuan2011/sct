using System;
using System.Collections.Generic;

namespace EF_Core.Models
{
    public partial class Stage
    {
        /// <summary>
        /// Giai đoạn
        /// </summary>
        public Guid StageId { get; set; }
        /// <summary>
        /// Tên giai đoạn
        /// </summary>
        public string StageName { get; set; } = null!;
        /// <summary>
        /// Năm bắt đầu
        /// </summary>
        public int StartYear { get; set; }
        /// <summary>
        /// Năm kết thúc
        /// </summary>
        public int EndYear { get; set; }
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
