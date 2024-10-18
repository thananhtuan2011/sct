namespace API_SoCongThuong.Models
{
    public class ManagementElectricityActivitiesModel
    {
        public Guid ManagementElectricityActivitiesId { get; set; }
        public string ProjectName { get; set; }
        public Guid DistrictId { get; set; }
        public string? DistrictName { get; set; }
        public double Wattage { get; set; }
        public double MaxWattage { get; set; }
        public int Type { get; set; }
        public string? TypeName { get; set; }
        public DateTime DateOfAcceptance { get; set; }
        public string ConnectorAgreement { get; set; }
        public string PowerPurchaseAgreement { get; set; }
        public string AnotherContent { get; set; }
    }
}