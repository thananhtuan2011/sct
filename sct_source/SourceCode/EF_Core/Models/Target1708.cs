using System;
using System.Collections.Generic;

namespace EF_Core.Models
{
    public partial class Target1708
    {
        /// <summary>
        /// Bảng tiêu chí 17.08
        /// </summary>
        public Guid Target1708Id { get; set; }
        /// <summary>
        /// ID giai đoạn lấy từ bảng Stage
        /// </summary>
        public Guid StageId { get; set; }
        /// <summary>
        /// ID Huyện lấy từ bảng District
        /// </summary>
        public Guid DistrictId { get; set; }
        /// <summary>
        /// Mã xã lấy từ bảng Commune
        /// </summary>
        public Guid CommuneId { get; set; }
        /// <summary>
        /// Đạt tiêu chí nông thôn mới
        /// </summary>
        public bool NewRuralCriteria { get; set; }
        /// <summary>
        /// Đạt tiêu chí nông thôn mới nâng cao
        /// </summary>
        public bool NewRuralCriteriaRaised { get; set; }
        /// <summary>
        /// Ghi chú
        /// </summary>
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
