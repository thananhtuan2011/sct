namespace API_SoCongThuong.Models
{
    public class IndustryModel
    {
        public Guid IndustryId { get; set; }
        public string IndustryCode { get; set; } = "";
        public string IndustryName { get; set; } = "";
        public int IndustryLevel { get; set; }
        public Guid ParentIndustryId { get; set; }
        public bool IsDel { get; set; }
    }
    public class removeListIndustryItems
    {
        public List<Guid> IndustryIds { get; set; }
    }
}
