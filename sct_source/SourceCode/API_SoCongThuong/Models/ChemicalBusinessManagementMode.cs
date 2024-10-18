namespace API_SoCongThuong.Models
{
    public class ChemicalBusinessManagementModel
    {
        public Guid ChemicalBusinessManagementId { get; set; }
        public Guid BusinessName { get; set; }
        public string? BusinessNameString { get; set; }
        public string Address { get; set; } = null!;
        public string ChemicalStorage { get; set; } = null!;
        public bool Pnupschcmeasures { get; set; }
        public int Status { get; set; }
        public bool IsDel { get; set; }
        public DateTime CreateTime { get; set; }
        public Guid CreateUserId { get; set; }
        public DateTime? UpdateTime { get; set; }
        public Guid? UpdateUserId { get; set; }
        public Guid? DistrictId { get; set; }
        public Guid? CommuneId { get; set; }
        public string? Represent { get; set; }
        public string? DistrictName { get; set; }
        public string? CommuneName { get; set; }

    }
}