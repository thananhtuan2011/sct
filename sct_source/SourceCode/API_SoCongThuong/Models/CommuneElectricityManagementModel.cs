namespace API_SoCongThuong.Models
{
    public class CommuneElectricityManagementModel
    {
        public Guid CommuneElectricityManagementId { get; set; }
        public Guid StageId { get; set; }
        public string? StageName { get; set; }
        public Guid DistrictId { get; set; }
        public string? DistrictName { get; set; }
        public Guid CommuneId { get; set; }
        public string? CommuneName { get; set; }
        public bool Content41Start { get; set; }
        public bool Content42Start { get; set; }
        public bool Target4Start { get; set; }
        public bool Content41End { get; set; }
        public bool Content42End { get; set; }
        public bool Target4End { get; set; }
        public string? Note { get; set; }
    }
}