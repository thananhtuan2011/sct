using System;
using System.Collections.Generic;

namespace EF_Core.Models
{
    public partial class CateReportSoldAncol
    {
        public Guid CateReportSoldAncolId { get; set; }
        /// <summary>
        /// Id doanh nghiệp
        /// </summary>
        public Guid BusinessId { get; set; }
        /// <summary>
        /// Số lượng mua trong năm
        /// </summary>
        public long QuantityBoughtOfYear { get; set; }
        /// <summary>
        /// Tổng giá trị mua trong năm
        /// </summary>
        public long TotalPriceBoughtOfYear { get; set; }
        /// <summary>
        /// Số lượng bán trong năm
        /// </summary>
        public long QuantitySoldOfYear { get; set; }
        /// <summary>
        /// Tổng số lượng bán trong năm
        /// </summary>
        public long TotalPriceSoldOfYear { get; set; }
        /// <summary>
        /// Năm báo cáo
        /// </summary>
        public int Year { get; set; }
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
