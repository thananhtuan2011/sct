namespace API_SoCongThuong.Models
{
    public class ElectricOperatingUnitsModel
    {
        public Guid ElectricOperatingUnitsId { get; set; }
        public Guid CustomerName { get; set; }
        public string? CustomerString { get; set; }
        public string Address { get; set; } = null!;
        public string PhoneNumber { get; set; }
        public string PresidentName { get; set; }
        public string NumOfGP { get; set; } = null!;
        public string SignDay { get; set; } = null!;
        public Guid Supplier { get; set; }
        public string? SupplierName { get; set; }
        public bool IsPowerGeneration { get; set; }
        public bool IsPowerDistribution { get; set; }
        public bool IsConsulting { get; set; }
        public bool IsSurveillance { get; set; }
        public bool IsElectricityRetail { get; set; }
        public string? FieldActivity { get; set; }
        public int Status { get; set; }
    }
}