using System;
using System.Collections.Generic;

namespace EF_Core.Models
{
    public partial class ChemicalSafetyCertificate
    {
        /// <summary>
        /// Bảng giấy chứng nhận - An toàn hóa chất
        /// </summary>
        public Guid ChemicalSafetyCertificateId { get; set; }
        /// <summary>
        /// Id doanh nghiệp
        /// </summary>
        public Guid BusinessId { get; set; }
        /// <summary>
        /// Địa chỉ trụ sở chính
        /// </summary>
        public string Address { get; set; } = null!;
        /// <summary>
        /// Số điện thoại
        /// </summary>
        public string PhoneNumber { get; set; } = null!;
        /// <summary>
        /// Fax
        /// </summary>
        public string? Fax { get; set; }
        /// <summary>
        /// Địa chỉ kinh doanh hóa chất
        /// </summary>
        public string BusinessAddress { get; set; } = null!;
        /// <summary>
        /// Mã doanh nghiệp
        /// </summary>
        public string BusinessCode { get; set; } = null!;
        /// <summary>
        /// Người cấp
        /// </summary>
        public string Provider { get; set; } = null!;
        /// <summary>
        /// Ngày cấp giấy chứng nhận đăng ký doanh nghiệp
        /// </summary>
        public DateTime? BusinessCertificateDate { get; set; }
        /// <summary>
        /// Ngày cấp
        /// </summary>
        public DateTime? LicenseDate { get; set; }
        /// <summary>
        /// Số cấp
        /// </summary>
        public string? Num { get; set; }
        /// <summary>
        /// Hiệu lực đến
        /// </summary>
        public DateTime? ValidTill { get; set; }
        /// <summary>
        /// Trạng thái: 0 - Chưa đủ điều kiện; 1 - Đủ điều kiện (Chưa cấp giấy chứng nhận); 2 - Đã cấp giấy chứng nhận
        /// </summary>
        public int? Status { get; set; }
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
