using System;
using System.Collections.Generic;

namespace API_SoCongThuong.Models
{
    public partial class CateReportProduceCrafttAncolForEconomicModel
    {
        public Guid CateReportProduceCrafttAncolForEconomicId { get; set; }

        /// Thông tin doanh nghiệp
        public Guid BusinessId { get; set; }
        public string? AlcoholBusinessName { get; set; }
        public string? Representative { get; set; }
        public string? Address { get; set; }
        public string? PhoneNumber { get; set; }
        public string? LicenseCode { get; set; }
        public DateTime? LicenseDate { get; set; }
        public string? LicenseDateDisplay { get; set; }

        /// Huyện
        public Guid? DistrictId { get; set; }
        public string? DistrictName { get; set; }

        /// Chủng loại rượu
        public string TypeofWine { get; set; } = null!;
        /// Sản lượng tiêu thụ
        public int QuantityConsume { get; set; }
        /// Sản lượng sản xuất
        public int Quantity { get; set; }
        /// 1: Hoạt động 0: Không hoạt động
        public int YearReport { get; set; }
        public bool? IsAction { get; set; }
        /// 1: Đã xóa; 0: Chưa xóa
        public bool IsDel { get; set; }
        /// Thời gian tạo
        public DateTime CreateTime { get; set; }
        public string CreateTimeDisplay { get; set; } = "";
        /// Người tạo
        public Guid CreateUserId { get; set; }
        public string CreateName { get; set; } = "";
        public DateTime? UpdateTime { get; set; }
        public Guid? UpdateUserId { get; set; }
    }
}
