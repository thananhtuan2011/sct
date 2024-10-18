using System;
using System.Collections.Generic;

namespace EF_Core.Models
{
    public partial class CateReportProduceIndustlAncol
    {
        public Guid CateReportProduceIndustlAncolId { get; set; }
        /// <summary>
        /// Id doanh nghiệp
        /// </summary>
        public Guid BusinessId { get; set; }
        /// <summary>
        /// Chủng loại rượu
        /// </summary>
        public string TypeofWine { get; set; } = null!;
        /// <summary>
        /// Sản lượng sản xuất
        /// </summary>
        public int QuantityProduction { get; set; }
        /// <summary>
        /// Sản lượng tiêu thụ
        /// </summary>
        public int QuantityConsume { get; set; }
        /// <summary>
        /// Công suất thiết kế
        /// </summary>
        public string DesignCapacity { get; set; } = null!;
        /// <summary>
        /// Vốn đầu tư
        /// </summary>
        public long Investment { get; set; }
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
