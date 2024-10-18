using System;
using System.Collections.Generic;

namespace EF_Core.Models
{
    public partial class FoodSafetyCertificate
    {
        /// <summary>
        /// Bảng giấy chứng nhận - An toàn thực phẩm
        /// </summary>
        public Guid FoodSafetyCertificateId { get; set; }
        public string ProfileCode { get; set; } = null!;
        public string ProfileName { get; set; } = null!;
        /// <summary>
        /// Id doanh nghiệp
        /// </summary>
        public Guid BusinessId { get; set; }
        /// <summary>
        /// Tên chủ sở hữu
        /// </summary>
        public string ManagerName { get; set; } = null!;
        public Guid DistrictId { get; set; }
        /// <summary>
        /// Địa chỉ sản xuất
        /// </summary>
        public string Address { get; set; } = null!;
        /// <summary>
        /// Số điện thoại
        /// </summary>
        public string PhoneNumber { get; set; } = null!;
        /// <summary>
        /// Số cấp
        /// </summary>
        public string? Num { get; set; }
        /// <summary>
        /// Hiệu lực đến
        /// </summary>
        public DateTime? ValidTill { get; set; }
        public DateTime? LicenseDate { get; set; }
        /// <summary>
        /// Trạng thái: 0 - Chưa đủ điều kiện; 1 - Đủ điều kiện (Chưa cấp giấy chứng nhận); 2 - Đã cấp giấy chứng nhận
        /// </summary>
        public int? Status { get; set; }
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
