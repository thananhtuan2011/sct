namespace API_SoCongThuong.Models
{
    public class FoodSafetyCertificateModel
    {
        public Guid FoodSafetyCertificateId { get; set; }
        public string ProfileCode { get; set; } = null!;
        public string ProfileName { get; set; } = null!;
        public Guid BusinessId { get; set; }
        public string? BusinessName { get; set; }
        public string ManagerName { get; set; } = null!;
        public Guid DistrictId { get; set; }
        public string? DistrictName { get; set; }
        public string Address { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string? Num { get; set; }
        public string? ValidTill { get; set; }
        public string? LicenseDate { get; set; }
        public string? ProductionDataString { get; set; }
        public List<FoodSafetyCertificateItemModel>? ProductionData { get; set; }
        public int? Status { get; set; }
        public string? Note { get; set; }
        public List<FoodSafetyCertificateAttachFileModel>? Details { get; set; }
        public string? IdFiles { get; set; }
        public Guid? CreateUserId { get; set; }
        public Guid? UpdateUserId { get; set; }
    }

    public partial class FoodSafetyCertificateItemModel
    {
        public Guid? ItemId { get; set; }
        public Guid? FoodSafetyCertificateId { get; set; }
        public int Type { get; set; }
        public string ProductName { get; set; } = null!;
    }

    public partial class FoodSafetyCertificateAttachFileModel
    {
        public Guid FoodSafetyCertificateAttachFileId { get; set; }
        public Guid FoodSafetyCertificateId { get; set; }
        public string LinkFile { get; set; } = null!;
    }
}