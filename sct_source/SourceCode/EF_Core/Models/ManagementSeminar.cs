using System;
using System.Collections.Generic;

namespace EF_Core.Models
{
    public partial class ManagementSeminar
    {
        public Guid ManagementSeminarId { get; set; }
        public string ProfileCode { get; set; } = null!;
        public Guid BusinessId { get; set; }
        public string Title { get; set; } = null!;
        public Guid DistrictId { get; set; }
        public string? Address { get; set; }
        public string? Contact { get; set; }
        public string? PhoneNumber { get; set; }
        public int NumberParticipant { get; set; }
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
