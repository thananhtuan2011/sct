namespace API_SoCongThuong.Models
{
    public class Target1708Model
    {
        public Guid Target1708Id { get; set; }
        public Guid StageId { get; set; }
        public string? StageName { get; set; }
        public int? StartYear { get; set; }
        public int? EndYear { get; set; }
        public Guid DistrictId { get; set; }
        public string? DistrictName { get; set; }
        public Guid CommuneId { get; set; }
        public string? CommuneName { get; set; }
        public bool NewRuralCriteria { get; set; }
        public bool? PreviousNewRuralCriteria { get; set; }
        public bool NewRuralCriteriaRaised { get; set; }
        public bool? PreviousNewRuralCriteriaRaised { get; set; }
        public string? Note { get; set; }
    }
}
