namespace API_SoCongThuong.Models
{
    public class StatisticalMultiLevelSalesModel
    {
        public Guid BusinessMultiLevelId { get; set; } = Guid.Empty;
        public string BusinessId { get; set; } = null!;
        public string BusinessName { get; set; } = null!;
        public int YearReport { get; set; }
        public string DistrictId { get; set; } = null!;
        public string DistrictName { get; set; } = null!;
        public long Revenue { get; set; } = 0;
        public long Scale { get; set; } = 0;
        public string Representative { get; set; } = null!;
        public string PhoneNumber { get; set; }
    }

    public class TotalStatisticalMultiLevelSalesModel
    {
        public long totalRevenue { get; set; } = 0;
        public long totalScale { get; set; } = 0;
    }

    public class ReturnStatisticalMultiLevelSalesModel
    {
        public List<StatisticalMultiLevelSalesModel> Data { get; set; } = new List<StatisticalMultiLevelSalesModel>();
        public TotalStatisticalMultiLevelSalesModel Total { get; set; } = new TotalStatisticalMultiLevelSalesModel();
    }

}
