namespace API_SoCongThuong.Models
{
    public class MarketPlanInformationModel
    {
        public Guid MarketPlanInformationId { get; set; }
        public string MarketName { get; set; } = null!;
        public Guid DistrictId { get; set; }
        public Guid CommuneId { get; set; }
        public string DistrictName { get; set; } = "";
        public string CommuneName { get; set; } = "";
        public string? Address { get; set; }
        public int Year { get; set; }
        public decimal? LandArea { get; set; }
        public decimal? BusinessLandArea { get; set; }
        public Guid ConstructionProperty { get; set; }
        public string ConstructionPropertyName { get; set; } = "";
        public Guid ConstructionNeed { get; set; }
        public string ConstructionNeedName { get; set; } = "";
        public string ConstructionPropertyCode { get; set; } = "";
        public string ConstructionNeedCode { get; set; } = "";

        public string? Note { get; set; }
        public bool? IsAction { get; set; }
        public bool IsDel { get; set; }
        public DateTime CreateTime { get; set; }
        public Guid CreateUserId { get; set; }
        public DateTime? UpdateTime { get; set; }
        public Guid? UpdateUserId { get; set; }
    }
}
