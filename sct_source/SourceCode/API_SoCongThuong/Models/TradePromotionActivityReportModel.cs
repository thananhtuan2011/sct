namespace API_SoCongThuong.Models
{
    public partial class TradePromotionActivityReportModel
    {
        /// <summary>
        /// Bảng báo cáo hoạt động xúc tiến thương mại
        /// </summary>
        public Guid TradePromotionActivityReportId { get; set; }
        /// <summary>
        /// Id quy mô
        /// </summary>
        public int ScaleId { get; set; }
        /// <summary>
        /// Tên đề án
        /// </summary>
        public string PlanName { get; set; } = null!;
        /// <summary>
        /// Kế hoạch tham gia
        /// </summary>
        public bool PlanToJoin { get; set; }
        /// <summary>
        /// Ngày bắt đầu
        /// </summary>
        public DateTime? StartDate { get; set; }
        public string StartDateDisplay { get; set; } = null!;
        /// <summary>
        /// Thời gian kết thúc
        /// </summary>
        public DateTime? EndDate { get; set; }
        public string? EndDateDisplay { get; set; }
        /// <summary>
        /// Id huyện
        /// </summary>
        public string? Time { get; set; }
        /// <summary>
        /// Id huyện
        /// </summary>
        public Guid DistrictId { get; set; }
        /// <summary>
        /// Địa chỉ
        /// </summary>
        public string Address { get; set; } = null!;
        /// <summary>
        /// Kinh phí thực hiện
        /// </summary>
        public long ImplementationCost { get; set; }
        /// <summary>
        /// Kinh phí hổ trợ
        /// </summary>
        public long FundingSupport { get; set; }
        /// <summary>
        /// Quy mô
        /// </summary>
        public string? Scale { get; set; }
        /// <summary>
        /// Số lượng doanh nghiệp tham gia
        /// </summary>
        public int NumParticipating { get; set; }
        /// <summary>
        /// Ghi chú
        /// </summary>
        public string? Note { get; set; }
        /// <summary>
        /// 1: Hoạt động 0: Không hoạt động
        /// </summary>
        public DateTime? CreateTime { get; set; }
        /// <summary>
        /// Người tạo
        /// </summary>
        public Guid? CreateUserId { get; set; }
        public DateTime? UpdateTime { get; set; }
        public Guid? UpdateUserId { get; set; }
        public string? ParticipatingBusinessesJson { get; set; }
        public List<TradePromotionActivityReportBusinessModel>? ParticipatingBusinesses { get; set; }
        public string? LIdDel { get; set; }
        public List<TradePromotionActivityReportAttachFileModel>? Files { get; set; }
        /// <summary>
        /// Danh sách doanh nghiệp tham gia
        /// </summary>
        public string? Participating { get; set; }
    }

    public partial class TradePromotionActivityReportBusinessModel
    {
        public Guid? TradePromotionActivityReportDetailId { get; set; }
        public Guid? TradePromotionActivityReportId { get; set; }
        public Guid? BusinessId { get; set; }
        public string BusinessName { get; set; }
        public string Address { get; set; }
    }

    public partial class TradePromotionActivityReportAttachFileModel
    {
        public Guid TradePromotionActivityReportAttachFileId { get; set; }
        public Guid TradePromotionActivityReportId { get; set; }
        public string LinkFile { get; set; } = null!;
    }
}
