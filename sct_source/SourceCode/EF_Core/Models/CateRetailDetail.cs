using System;
using System.Collections.Generic;

namespace EF_Core.Models
{
    public partial class CateRetailDetail
    {
        public Guid CateRetailDetailId { get; set; }
        public Guid CateRetailId { get; set; }
        /// <summary>
        /// Thực hiện tháng trước
        /// </summary>
        public long? PerformLastmonth { get; set; }
        /// <summary>
        /// Ước tính báo cáo tháng
        /// </summary>
        public long? EstimateReportingMonth { get; set; }
        /// <summary>
        /// Cộng dồn từ đầu năm đến tháng báo cáo
        /// </summary>
        public long? CumulativeToReportingMonth { get; set; }
        /// <summary>
        /// Thực hiện báo cáo
        /// </summary>
        public long? PerformReporting { get; set; }
        /// <summary>
        /// 1: Tổng mức bán lẻ hàng hóa theo năm báo cáo
        /// 2: Tổng mức bán lẻ hàng hóa theo năm trước
        /// </summary>
        public int? Type { get; set; }
        public Guid Criteria { get; set; }
    }
}
