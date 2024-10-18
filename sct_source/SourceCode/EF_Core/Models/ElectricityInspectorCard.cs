using System;
using System.Collections.Generic;

namespace EF_Core.Models
{
    public partial class ElectricityInspectorCard
    {
        /// <summary>
        /// Danh sách thẻ kiểm tra viên điện lực
        /// </summary>
        public Guid ElectricityInspectorCardId { get; set; }
        /// <summary>
        /// Tên Kiểm tra viên
        /// </summary>
        public string InspectorName { get; set; } = null!;
        /// <summary>
        /// Ngày sinh
        /// </summary>
        public DateTime Birthday { get; set; }
        /// <summary>
        /// Ngày cấp thẻ
        /// </summary>
        public DateTime LicenseDate { get; set; }
        /// <summary>
        /// Trình độ
        /// </summary>
        public string Degree { get; set; } = null!;
        /// <summary>
        /// Đơn vị
        /// </summary>
        public string Unit { get; set; } = null!;
        /// <summary>
        /// Thâm niên
        /// </summary>
        public string Seniority { get; set; } = null!;
        public Guid CardColor { get; set; }
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
