namespace API_SoCongThuong.Models
{
    public class ProposedPowerProjectModel
    {
        /// <summary>
        /// Quản lý dự án nguồn điện được phê duyệt
        /// </summary>
        public Guid ProposedPowerProjectId { get; set; }
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
        /// Id trạng thái
        /// </summary>
        public Guid StatusId { get; set; }
        /// <summary>
        /// Tên trạng thái
        /// </summary>
        public string? StatusName { get; set; }
        /// <summary>
        /// Nhà đầu tư
        /// </summary>
        public string InvestorName { get; set; } = null!;
        /// <summary>
        /// Văn bản pháp lý
        /// </summary>
        public string PolicyDecision { get; set; } = null!;
        /// <summary>
        /// Công suất
        /// </summary>
        public int Wattage { get; set; }
        /// <summary>
        /// Vị trí
        /// </summary>
        public string Address { get; set; } = null!;
        /// <summary>
        /// Ghi chú
        /// </summary>
        public string? Note { get; set; }
        //public string ProposedDate { get; set; } = null!;
        public bool IsDel { get; set; }
        public DateTime CreateTime { get; set; }
        public Guid CreateUserId { get; set; }
        public DateTime? UpdateTime { get; set; }
        public Guid? UpdateUserId { get; set; }
    }
}