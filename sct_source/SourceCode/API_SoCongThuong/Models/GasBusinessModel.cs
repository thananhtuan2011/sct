namespace API_SoCongThuong.Models
{
    public class GasBusinessModel
    {
        public Guid GasBusinessId { get; set; }
        public byte TypeBusiness { get; set; } = 0;
        public Guid BusinessId { get; set; }
        public string BusinessName { get; set; } = "";
        public string BusinessCode { get; set; } = "";
        public Guid GasBusiness { get; set; }
        public string? ForeignTransactionName { get; set; } = "";
        public string? Address { get; set; } = "";
        public string? PhoneNumber { get; set; } = "";
        public string? Fax { get; set; }
        public string? BusinessCertificate { get; set; } = "";
        public DateTime? LicenseDate { get; set; } = DateTime.Now;
        public string Licensors { get; set; } = "";
        public string? TaxId { get; set; }
        public string? NumDoc { get; set; }
        public DateTime? DateEnd { get; set; }
        public DateTime? DateStart { get; set; }
        public Guid? ComplianceStatus { get; set; }
        public bool? IsAction { get; set; }
        public bool IsDel { get; set; }
        public DateTime CreateTime { get; set; }
        public Guid CreateUserId { get; set; }
        public DateTime? UpdateTime { get; set; }
        public Guid? UpdateUserId { get; set; }
        public string ComplianceStatusName { get; set; } = "";
        public Guid? District { get; set; }
    }
}
