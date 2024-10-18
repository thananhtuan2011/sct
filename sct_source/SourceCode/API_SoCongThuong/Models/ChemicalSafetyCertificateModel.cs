namespace API_SoCongThuong.Models
{
    public class ChemicalSafetyCertificateModel
    {
        public Guid ChemicalSafetyCertificateId { get; set; }
        public Guid BusinessId { get; set; }
        public string? BusinessName { get; set; }
        public string Address { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string? Fax { get; set; }
        public string BusinessAddress { get; set; } = null!;
        public string BusinessCode { get; set; } = null!;
        public string Provider { get; set; } = null!;
        public string? BusinessCertificateDate { get; set; }
        public string? LicenseDate { get; set; }
        public string? Num { get; set; }
        public string? ValidTill { get; set; }
        public int? Status { get; set; }
        public string? JsonListChemical { get; set; }
        public List<ChemicalInfoModel>? ListChemical { get; set; }
        public Guid? CreateUserId { get; set; }
        public Guid? UpdateUserId { get; set; }
        public List<ChemicalSafetyCertificateAttachFileModel>? Details { get; set; }
        public string? IdFiles { get; set; }
    }

    public class ChemicalInfoModel
    {
        public string TradeName { get; set; } = null!;
        public string NameOfChemical { get; set; } = null!;
        public string? Cascode { get; set; }
        public string? ChemicalFormula { get; set; }
        public string? Content { get; set; }
        public string? Mass { get; set; }
    }

    public partial class ChemicalSafetyCertificateAttachFileModel
    {
        public Guid ChemicalSafetyCertificateAttachFileId { get; set; }
        public Guid ChemicalSafetyCertificateId { get; set; }
        public string LinkFile { get; set; } = null!;
    }
}