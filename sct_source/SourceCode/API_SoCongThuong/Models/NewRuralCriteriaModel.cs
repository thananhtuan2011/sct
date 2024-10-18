namespace API_SoCongThuong.Models
{
    //public class NewRuralCriteriaModel
    //{
    //    public Guid DistrictId { get; set; }
    //    public string? DistrictName { get; set; }
    //    public Guid CommuneId { get; set; }
    //    public string? CommuneName { get; set; }
    //    public int Year { get; set; }
    //    public bool? Target4 { get; set; } = false;
    //    public bool? Target7 { get; set; } = false;
    //    public bool? Target7Raised { get; set; } = false;
    //    public bool? Target1708 { get; set; } = false;
    //    public bool? Target1708Raised { get; set; } = false;
    //}

    public class NewRuralCriteriaModel
    {
        public Guid NewRuralCriteriaId { get; set; }
        public Guid DistrictId { get; set; }
        public string? DistrictName { get; set; }
        public Guid CommuneId { get; set; }
        public string? CommuneName { get; set; }
        public string TitleIdStr { get; set; } = null!;
        public int? Title { get; set; }
        public string? TitleName { get; set; }
        public bool Target4 { get; set; } = false;
        public bool Target7 { get; set; } = false;
        public bool Target1708 { get; set; } = false;
        public string? Note { get; set; }
    }
}
