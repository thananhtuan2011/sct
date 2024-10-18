using System;
using System.Collections.Generic;

namespace EF_Core.Models
{
    public partial class CateReportProduceCrafttAncolForEconomic
    {
        public Guid CateReportProduceCrafttAncolForEconomicId { get; set; }
        public Guid BusinessId { get; set; }
        /// <summary>
        /// chủng loại rượu
        /// </summary>
        public string TypeofWine { get; set; } = null!;
        /// <summary>
        /// Sản lượng
        /// </summary>
        public int Quantity { get; set; }
        /// <summary>
        /// Sản lượng tiêu thụ
        /// </summary>
        public int QuantityConsume { get; set; }
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
        /// <summary>
        /// Người tạo
        /// </summary>
        public Guid CreateUserId { get; set; }
        public DateTime? UpdateTime { get; set; }
        public Guid? UpdateUserId { get; set; }
    }
}
