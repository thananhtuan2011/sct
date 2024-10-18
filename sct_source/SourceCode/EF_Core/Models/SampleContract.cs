using System;
using System.Collections.Generic;

namespace EF_Core.Models
{
    public partial class SampleContract
    {
        /// <summary>
        /// Bảng hợp đồng mẫu - Sở thanh tra
        /// </summary>
        public Guid SampleContractId { get; set; }
        /// <summary>
        /// Lĩnh vực giải quyết
        /// </summary>
        public Guid SampleContractField { get; set; }
        /// <summary>
        /// Thời gian đăng ký
        /// </summary>
        public DateTime RegistrationTime { get; set; }
        /// <summary>
        /// Số hồ sơ
        /// </summary>
        public string ProfileNumber { get; set; } = null!;
        /// <summary>
        /// Tên người đăng ký
        /// </summary>
        public string RegistrantName { get; set; } = null!;
        /// <summary>
        /// Số điện thoại liên hệ
        /// </summary>
        public string PhoneNumber { get; set; } = null!;
        /// <summary>
        /// Tên cơ quan/ tổ chức
        /// </summary>
        public string BusinessName { get; set; } = null!;
        /// <summary>
        /// Mã số thuế
        /// </summary>
        public string TaxCode { get; set; } = null!;
        /// <summary>
        /// Số điện thoại cơ quan / tổ chức
        /// </summary>
        public string? BusinessPhoneNumber { get; set; }
        /// <summary>
        /// Địa chỉ
        /// </summary>
        public string? Address { get; set; }
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
