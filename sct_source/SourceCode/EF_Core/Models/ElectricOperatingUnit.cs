using System;
using System.Collections.Generic;

namespace EF_Core.Models
{
    public partial class ElectricOperatingUnit
    {
        /// <summary>
        /// Bảng các đơn vị hoạt động điện lực
        /// </summary>
        public Guid ElectricOperatingUnitsId { get; set; }
        /// <summary>
        /// Id doanh nghiệp
        /// </summary>
        public Guid CustomerName { get; set; }
        /// <summary>
        /// Địa chỉ
        /// </summary>
        public string Address { get; set; } = null!;
        /// <summary>
        /// Số điện thoại
        /// </summary>
        public string? PhoneNumber { get; set; }
        /// <summary>
        /// Tên giám đốc
        /// </summary>
        public string? PresidentName { get; set; }
        /// <summary>
        /// Số của GP hoạt động điện lực
        /// </summary>
        public string NumOfGp { get; set; } = null!;
        /// <summary>
        /// Ngày ký văn bản
        /// </summary>
        public DateTime SignDay { get; set; }
        /// <summary>
        /// Đơn vị cấp
        /// </summary>
        public Guid Supplier { get; set; }
        /// <summary>
        /// Phát điện
        /// </summary>
        public bool IsPowerGeneration { get; set; }
        /// <summary>
        /// Phân phối điện
        /// </summary>
        public bool IsPowerDistribution { get; set; }
        /// <summary>
        /// Tư vấn thiết kế
        /// </summary>
        public bool IsConsulting { get; set; }
        /// <summary>
        /// Tư vấn giám sát
        /// </summary>
        public bool IsSurveillance { get; set; }
        /// <summary>
        /// Bán lẻ điện
        /// </summary>
        public bool IsElectricityRetail { get; set; }
        /// <summary>
        /// Tình trạng hoạt động: 0 - Còn hoạt động; 1 - Ngừng hoạt động
        /// </summary>
        public int Status { get; set; }
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
