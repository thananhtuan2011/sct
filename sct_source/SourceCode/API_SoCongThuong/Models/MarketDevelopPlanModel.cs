namespace API_SoCongThuong.Models
{
    public class MarketDevelopPlanModel
    {
        public Guid MarketDevelopPlanId { get; set; }
        public string MarketName { get; set; } = null!;
        public Guid RankId { get; set; }
        public string RankName { get; set; } = "";
        public Guid DistrictId { get; set; }
        public string DistrictName { get; set; } = "";
        public Guid CommuneId { get; set; }
        public string CommuneName { get; set; } = "";
        public string? Address { get; set; }
        public Guid Stage { get; set; }
        public string StageName { get; set; } = "";
        public Guid TypeOfPlanMarket { get; set; }
        public string TypeOfPlanMarketName { get; set; } = "";

        public decimal? ExistLandArea { get; set; } = 0;
        public decimal? NewLandArea { get; set; } = 0;
        public decimal? AddLandArea { get; set; } = 0;
        public decimal? Capital { get; set; } = 0;
        public string? Note { get; set; }
        public bool? IsAction { get; set; }
        public bool IsDel { get; set; }
        public DateTime CreateTime { get; set; }
        public Guid CreateUserId { get; set; }
        public DateTime? UpdateTime { get; set; }
        public Guid? UpdateUserId { get; set; }
        public string TypeOfPlanMarketCode { get; set; } = "";
    }
}
