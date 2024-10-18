using System;
using System.Collections.Generic;

namespace API_SoCongThuong.Models
{
    public partial class CateReportSoldAncolForFactoryLicenseModel
    {
        public Guid CateReportSoldAncolForFactoryLicenseId { get; set; }
        public Guid BusinessId { get; set; }
        public string? OrganizationName { get; set; }
        public string? Address { get; set; }
        public string? PhoneNumber { get; set; }
        /// <summary>
        /// Loại rượu đăng ký
        /// </summary>
        public string TypeofWine { get; set; } = null!;
        /// <summary>
        /// Sản lượng
        /// </summary>
        public int Quantity { get; set; }
        /// <summary>
        /// Tên doanh nghiệp mua lại rượu
        /// </summary>
        public string WineFactory { get; set; } = null!;
        /// <summary>
        /// Năm báo cáo
        /// </summary>
        public int YearReport { get; set; }
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
        public string CreateTimeDisplay { get; set; } = "";
        /// <summary>
        /// Người tạo
        /// </summary>
        public Guid CreateUserId { get; set; }
        public string CreateName { get; set; } = "";
        public DateTime? UpdateTime { get; set; }
        public Guid? UpdateUserId { get; set; }
    }
}
