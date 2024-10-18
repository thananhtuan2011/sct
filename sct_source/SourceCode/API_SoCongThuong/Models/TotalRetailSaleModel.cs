namespace API_SoCongThuong.Models
{
    public class TotalRetailSaleModel
    {
        public Guid TotalRetailSaleId { get; set; }
        public byte Target { get; set; } = 1;
        public string Month { get; set; } = "";
        public string TargetName { get; set; } = "";
        public decimal ReportIndex { get; set; } = 0;
        public decimal? YORImplement { get; set; } = decimal.Zero;
        public decimal? YOREstimate { get; set; } = decimal.Zero;
        public decimal? YORAccumulation { get; set; } = decimal.Zero;
        public decimal? PYImplement { get; set; } = decimal.Zero;
        public decimal? PYAccumulation { get; set; } = decimal.Zero;
        public decimal? RatioImplementLastMonth { get; set; } = decimal.Zero;
        public decimal? RatioImplementSamePeriod { get; set; } = decimal.Zero;
        public decimal? RatioAccumulation { get; set; } = decimal.Zero;
        public bool? IsAction { get; set; }
        public bool IsDel { get; set; }
        public DateTime CreateTime { get; set; }
        public Guid CreateUserId { get; set; }
        public DateTime? UpdateTime { get; set; }
        public Guid? UpdateUserId { get; set; }
    }
}
