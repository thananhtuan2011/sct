namespace API_SoCongThuong.Models
{
    public class ApprovedPowerProjectModel
    {
        /// <summary>
        /// Quản lý dự án nguồn điện được phê duyệt
        /// </summary>
        public Guid ApprovedPowerProjectId { get; set; }
        /// <summary>
        /// Id lĩnh vực
        /// </summary>
        public Guid EnergyIndustryId { get; set; }
        /// <summary>
        /// Tên lĩnh vực
        /// </summary>
        public string? EnergyIndustryName { get; set; }
        /// <summary>
        /// Tên dự án
        /// </summary>
        public string ProjectName { get; set; } = null!;
        /// <summary>
        /// Nhà đầu tư
        /// </summary>
        public string InvestorName { get; set; } = null!;
        /// <summary>
        /// Huyện
        /// </summary>
        public Guid DistrictId { get; set; }
        /// <summary>
        /// Tên huyện
        /// </summary>
        public string? DistrictName { get; set; }
        /// <summary>
        /// Địa chỉ
        /// </summary>
        public string Address { get; set; } = null!;
        /// <summary>
        /// Quyết định chủ trương
        /// </summary>
        public string PolicyDecision { get; set; } = null!;
        /// <summary>
        /// Công suất
        /// </summary>
        public int Wattage { get; set; }
        /// <summary>
        /// Số tua bin
        /// </summary>
        public int Turbines { get; set; }
        /// <summary>
        /// Trạm biến áp
        /// </summary>
        public int Substation { get; set; }
        /// <summary>
        /// Sản lượng điện phát (MWh)
        /// </summary>
        public int? PowerOutput { get; set; }
        /// <summary>
        /// Diện tích
        /// </summary>
        public decimal Area { get; set; }
        /// <summary>
        /// Năm
        /// </summary>
        public int Year { get; set; }
        /// <summary>
        /// Trạng thái
        /// </summary>
        public Guid Status { get; set; }
        /// <summary>
        /// Tên trạng thái
        /// </summary>
        public string? StatusName { get; set; }
        /// <summary>
        /// Ghi chú
        /// </summary>
        public string? Note { get; set; }
        public bool IsDel { get; set; }
        public DateTime CreateTime { get; set; }
        public Guid CreateUserId { get; set; }
        public DateTime? UpdateTime { get; set; }
        public Guid? UpdateUserId { get; set; }
    }
}