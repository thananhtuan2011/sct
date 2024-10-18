using System;
using System.Collections.Generic;

namespace EF_Core.Models
{
    public partial class NewRuralCriterion
    {
        /// <summary>
        /// BảngTiêu chí nông thôn mới nông thôn mới nâng cao
        /// </summary>
        public Guid NewRuralCriteriaId { get; set; }
        /// <summary>
        /// ID Huyện lấy từ bảng District
        /// </summary>
        public Guid DistrictId { get; set; }
        /// <summary>
        /// Mã xã lấy từ bảng Commune
        /// </summary>
        public Guid CommuneId { get; set; }
        /// <summary>
        /// Danh hiệu: 1 - Nông thôn mới, 2 - Nông thôn mới nâng cao
        /// </summary>
        public int Title { get; set; }
        /// <summary>
        /// Đạt tiêu chí số 4
        /// </summary>
        public bool Target4 { get; set; }
        /// <summary>
        /// Đạt tiêu chí số 7
        /// </summary>
        public bool Target7 { get; set; }
        /// <summary>
        /// Đạt tiêu chí 1708
        /// </summary>
        public bool Target1708 { get; set; }
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
