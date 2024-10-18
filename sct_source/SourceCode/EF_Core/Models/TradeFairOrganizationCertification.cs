using System;
using System.Collections.Generic;

namespace EF_Core.Models
{
    public partial class TradeFairOrganizationCertification
    {
        /// <summary>
        /// Quản lý xác nhận tổ chức hội chợ triển lãm thương mại - Xúc tiến thương mại
        /// </summary>
        public Guid TradeFairOrganizationCertificationId { get; set; }
        /// <summary>
        /// Tên hội chợ / Triển lãm thương mại
        /// </summary>
        public string TradeFairName { get; set; } = null!;
        /// <summary>
        /// Địa điểm tổ chức
        /// </summary>
        public string Address { get; set; } = null!;
        /// <summary>
        /// Quy mô
        /// </summary>
        public string? Scale { get; set; }
        /// <summary>
        /// Số văn bản
        /// </summary>
        public string TextNumber { get; set; } = null!;
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
