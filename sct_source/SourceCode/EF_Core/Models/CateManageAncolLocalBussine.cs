using System;
using System.Collections.Generic;

namespace EF_Core.Models
{
    public partial class CateManageAncolLocalBussine
    {
        public Guid CateManageAncolLocalBussinessId { get; set; }
        public bool? IsActive { get; set; }
        public Guid BusinessId { get; set; }
        public long Investment { get; set; }
        public int NumberOfWorker { get; set; }
        /// <summary>
        /// lấy từ bảng TypeOfProfession, một loại ngành nghề chính
        /// </summary>
        public Guid TypeOfProfessionId { get; set; }
        public DateTime DateRelease { get; set; }
        public DateTime? DateChange { get; set; }
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
