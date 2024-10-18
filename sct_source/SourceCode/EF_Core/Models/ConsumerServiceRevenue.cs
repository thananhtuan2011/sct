using System;
using System.Collections.Generic;

namespace EF_Core.Models
{
    public partial class ConsumerServiceRevenue
    {
        /// <summary>
        /// Bảng danh mục bán lẻ
        /// </summary>
        public Guid ConsumerServiceRevenueId { get; set; }
        public string? ConsumerServiceRevenueCode { get; set; }
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
        public Guid? CheckUserId { get; set; }
        public Guid? ConfirmUserId { get; set; }
        public DateTime? ConfirmTime { get; set; }
        public string? ReportMonth { get; set; }
        public DateTime? Importtime { get; set; }
    }
}
