using System;
using System.Collections.Generic;

namespace EF_Core.Models
{
    public partial class CateReportSoldAncolForFactoryLicense
    {
        public Guid CateReportSoldAncolForFactoryLicenseId { get; set; }
        /// <summary>
        /// Id tổ chức / cá nhân
        /// </summary>
        public Guid BusinessId { get; set; }
        /// <summary>
        /// Loại rượu
        /// </summary>
        public string TypeofWine { get; set; } = null!;
        /// <summary>
        /// Sản lượng
        /// </summary>
        public int Quantity { get; set; }
        /// <summary>
        /// Nhà máy mua rượu để chế biến lại
        /// </summary>
        public string WineFactory { get; set; } = null!;
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
        /// <summary>
        /// Người tạo
        /// </summary>
        public Guid CreateUserId { get; set; }
        public DateTime? UpdateTime { get; set; }
        public Guid? UpdateUserId { get; set; }
    }
}
