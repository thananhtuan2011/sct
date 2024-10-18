using System;
using System.Collections.Generic;

namespace EF_Core.Models
{
    public partial class CommuneElectricityManagement
    {
        /// <summary>
        /// Bảng quản lý điện cấp xã
        /// </summary>
        public Guid CommuneElectricityManagementId { get; set; }
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
        /// Đạt nội dung 4.1
        /// </summary>
        public bool Content41Start { get; set; }
        /// <summary>
        /// Đạt nội dung 4.2
        /// </summary>
        public bool Content42Start { get; set; }
        /// <summary>
        /// Đạt tiêu chí số 4
        /// </summary>
        public bool Target4Start { get; set; }
        /// <summary>
        /// Đạt nội dung 4.1
        /// </summary>
        public bool Content41End { get; set; }
        /// <summary>
        /// Đạt nội dung 4.2
        /// </summary>
        public bool Content42End { get; set; }
        /// <summary>
        /// Đạt tiêu chí số 4
        /// </summary>
        public bool Target4End { get; set; }
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
