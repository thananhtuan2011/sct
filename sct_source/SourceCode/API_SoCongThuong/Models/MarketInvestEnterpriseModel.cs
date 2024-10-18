namespace API_SoCongThuong.Models
{
    public class MarketInvestEnterpriseModel
    {
        public Guid MarketInvestEnterpriseId { get; set; }
        public string MarketName { get; set; } = null!;
        public Guid DistrictId { get; set; }
        public Guid CommuneId { get; set; }
        public string DistrictName { get; set; } = "";
        public string CommuneName { get; set; } = "";
        public string InvestmentName { get; set; } = "";
        public string? Address { get; set; }
        public bool Investment { get; set; }
        public string BusinessName { get; set; } = null!;
        public int? Capital { get; set; }
        public string? Evaluate { get; set; }
        public string? Note { get; set; }
        public bool? IsAction { get; set; }
        public bool IsDel { get; set; }
        public DateTime CreateTime { get; set; }
        public Guid CreateUserId { get; set; }
        public DateTime? UpdateTime { get; set; }
        public Guid? UpdateUserId { get; set; }
    }
}
