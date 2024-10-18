using System;
using System.Collections.Generic;

namespace EF_Core.Models
{
    public partial class AdministrativeFormality
    {
        public Guid AdminFormalitiesId { get; set; }
        public string AdminFormalitiesCode { get; set; } = null!;
        public string AdminFormalitiesName { get; set; } = null!;
        public Guid FieldId { get; set; }
        /// <summary>
        /// Cấp DVC: 1 - Toàn trình, 2 - Còn lại
        /// </summary>
        public int Dvclevel { get; set; }
        /// <summary>
        /// Url của Sở
        /// </summary>
        public string DocUrl { get; set; } = null!;
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
