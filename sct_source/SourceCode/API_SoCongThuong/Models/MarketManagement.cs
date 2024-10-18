namespace API_SoCongThuong.Models
{
    public class MarketManagementModel
    {
        public Guid MarketManagementId { get; set; }
        public Guid DistrictId { get; set; }
        public string DistrictName { get; set; } = "";
        public Guid CommuneId { get; set; }
        public string CommuneName { get; set; } = "";
        public Guid MarketId { get; set; }
        public string MarketName { get; set; } = "";
        public Guid NganhHangKinhDoanh { get; set; }
        public string? TenNganhHangKinhDoanh { get; set; }
        public int? BoothNumber { get; set; }
        public decimal? GiaTrongNhaLong { get; set; }
        public decimal? GiaNgoaiNhaLong { get; set; }
        public decimal? DeXuatGiaMoi { get; set; }
        public string? Note { get; set; }
        public bool IsDel { get; set; }
        public Guid CreateUserId { get; set; } = Guid.Empty;
        public DateTime CreateTime { get; set; }
        public DateTime? UpdateTime { get; set; }
        public Guid? UpdateUserId { get; set; }
        public List<BusinessLineDetailModel> MatHang { get; set; } = new List<BusinessLineDetailModel>();
    }
    public class removeMarketManagementModelItems
    {
        public List<Guid> MarketManagementIds { get; set; }
    }

    public class DistrictMarketModel 
    {
        public Guid DistrictId { get; set; }
        public string DistrictCode { get; set; }
        public string DistrictName { get; set; }
    }

    public class CommuneMarketModel
    {
        public Guid CommuneId { get; set; }
        public string CommuneCode { get; set; }
        public string CommuneName { get; set; }
        public Guid DistrictId { get; set; }
    }

    public class MarketModel
    {
        public Guid MarketId { get; set; }
        public string MarketName { get; set; }
        public Guid DistrictId { get; set; }
        public Guid CommuneId { get; set; }
    }
        
    public class BusinessLineDetailModel
    {
        public Guid BusinessLineId { get; set; }
        public string BusinessLineName { get; set; } = "";
        public decimal Price { get; set; }
    }
}