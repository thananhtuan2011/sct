namespace API_SoCongThuong.Models
{
    public class MarketTargetSevenModel
    {
        public Guid MarketTargetSevenId { get; set; }
        public string MarketName { get; set; } = null!;
        public Guid DistrictId { get; set; }
        public Guid CommuneId { get; set; }
        public string DistrictName { get; set; } = "";
        public string CommuneName { get; set; } = "";
        public string? Address { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public string? Note { get; set; }
        public bool? IsAction { get; set; }
        public bool IsDel { get; set; }
        public DateTime CreateTime { get; set; }
        public Guid CreateUserId { get; set; }
        public DateTime? UpdateTime { get; set; }
        public Guid? UpdateUserId { get; set; }
        public string Date { get; set; } = null!;
    }
}
