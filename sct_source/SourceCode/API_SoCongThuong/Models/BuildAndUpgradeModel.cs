namespace API_SoCongThuong.Models
{
    public class BuildAndUpgradeModel
    {
        public Guid BuildAndUpgradeId { get; set; }
        public Guid CommercialId { get; set; }
        public Guid DistrictId { get; set; }
        public Guid CommuneId { get; set; }
        public string BuildAndUpgradeName { get; set; } = null!; 
        public string Name { get; set; } = null!;
        public string DistrictsName { get; set; } = null!;
        public string CommuneName { get; set; } = null!;
        public string Address { get; set; } = null!;
        public decimal? TotalInvestment { get; set; } = 0;
        public string? TotalInvestmentUnit { get; set; }
        public decimal? RealizedCapital { get; set; } = 0;
        public string? RealizedCapitalUnit { get; set; }
        public decimal? BudgetCapital { get; set; } = 0;
        public string? BudgetCapitalUnit { get; set; }
        public decimal? LandUseCapital { get; set; } = 0;
        public string? LandUseCapitalUnit { get; set; }
        public decimal? Loans { get; set; } = 0;
        public string? LoansUnit { get; set; }
        public decimal? AnotherCapital { get; set; } = 0;
        public string? AnotherCapitalUnit { get; set; }
        public bool IsBuild { get; set; }
        public bool IsUpgrade { get; set; }
        public string? Note { get; set; }
        public bool IsDel { get; set; }
        public int? Year { get; set; } 
    }
    public class removeListBuildAndUpgradeModelItems
    {
        public List<Guid> BuildAndUpgradeIds { get; set; }
    }
    public class MarketBuildModel
    {
        public Guid CommercialId { get; set; }
        public string CommercialName { get; set; }
        public Guid DistrictId { get; set; }
        public string DistrictName { get; set; }
        public Guid CommuneId { get; set; }
        public string CommuneName { get; set; }
        public string Address { get; set; } = "";
    }
}