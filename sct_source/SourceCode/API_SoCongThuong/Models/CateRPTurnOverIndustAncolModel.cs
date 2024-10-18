using System;
using System.Collections.Generic;

namespace API_SoCongThuong.Models
{
    public partial class CateReportTurnOverIndustAncolModel
    {
        public Guid CateReportTurnOverIndustAncolId { get; set; }
        /// <summary>
        /// 1: Buôn rượu 2: Bán lẻ
        /// </summary>
        public Guid BusinessId { get; set; }
        public string? AlcoholBusinessName { get; set; }
        public string? Address { get; set; }
        public string? PhoneNumber { get; set; }
        public string? LicenseCode { get; set; }
        public DateTime? LicenseDate { get; set; }
        public string? LicenseDateDisplay { get; set; }
        public long QuantityBoughtOfYear { get; set; }
        public long TotalPriceBoughtOfYear { get; set; }
        public long QuantitySoldOfYear { get; set; }
        public long TotalPriceSoldOfYear { get; set; }
        public int YearId { get; set; }
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
