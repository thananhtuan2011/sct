namespace API_SoCongThuong.Models
{
    public class RooftopSolarProjectManagementModel
    {
        /// <summary>
        /// Quản lý dự án điện mặt trời áp mái
        /// </summary>
        public Guid RooftopSolarProjectManagementId { get; set; }
        /// <summary>
        /// Tên dự án
        /// </summary>
        public string ProjectName { get; set; } = null!;
        /// <summary>
        /// Nhà đầu tư
        /// </summary>
        public string InvestorName { get; set; } = null!;
        /// <summary>
        /// Vị trí
        /// </summary>
        public string Address { get; set; } = null!;
        /// <summary>
        /// Diện tích
        /// </summary>
        public decimal Area { get; set; }
        /// <summary>
        /// Chủ trương khảo
        /// </summary>
        public string SurveyPolicy { get; set; } = null!;
        /// <summary>
        /// Công suất
        /// </summary>
        public decimal Wattage { get; set; }
        /// <summary>
        /// Tiến độ
        /// </summary>
        public string Progress { get; set; } = null!;
        public bool IsDel { get; set; }
        public DateTime CreateTime { get; set; }
        public Guid CreateUserId { get; set; }
        public DateTime? UpdateTime { get; set; }
        public Guid? UpdateUserId { get; set; }
        public Guid? District { get; set; } = Guid.Empty;
        public string DistrictName { get; set; } = "";
        public DateTime? OperationDay { get; set; }
    }
}