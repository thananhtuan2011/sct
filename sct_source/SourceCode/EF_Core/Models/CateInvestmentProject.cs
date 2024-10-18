using System;
using System.Collections.Generic;

namespace EF_Core.Models
{
    public partial class CateInvestmentProject
    {
        public Guid CateInvestmentProjectId { get; set; }
        /// <summary>
        /// 1: trong nước, 2 ngoài nước
        /// </summary>
        public int InvestmentType { get; set; }
        public string BusinessName { get; set; } = null!;
        public string? Owner { get; set; }
        public Guid? District { get; set; }
        public string? Career { get; set; }
        public string? PhoneNumber { get; set; }
        public long Investment { get; set; }
        public int NumberOfWorker { get; set; }
        /// <summary>
        /// Diện tích dự án
        /// </summary>
        public int ProjectArea { get; set; }
        /// <summary>
        /// Sản lượng 1 ngày
        /// </summary>
        public int Quantity { get; set; }
        /// <summary>
        /// năng suất sản xuất
        /// </summary>
        public int Produce { get; set; }
        public long ProductValue { get; set; }
        public string? Reality { get; set; }
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
