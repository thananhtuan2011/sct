namespace API_SoCongThuong.Models
{
    public class ElectricalProjectManagementModel
    {
        public Guid ElectricalProjectManagementId { get; set; }
        public string BuildingCode { get; set; } = null!;
        public string BuildingName { get; set; } = null!;
        public Guid District { get; set; }
        public string? DistrictName { get; set; }
        public string Address { get; set; } = null!;
        public string Represent { get; set; } = null!;
        public int Status { get; set; }
        public string? Note { get; set; }
        public Guid? TypeOfConstruction { get; set; } = Guid.Empty;
        public Guid? VoltageLevel { get; set; } = Guid.Empty;
        public string VoltageLevelName { get; set; } = "";
        public string? Wattage { get; set; }
        public string? Length { get; set; }
        public string? WireType { get; set; }
        public string TypeOfConstructionCode { get; set; } = "";
    }
}