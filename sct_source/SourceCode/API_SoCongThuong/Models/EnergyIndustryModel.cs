namespace API_SoCongThuong.Models
{
    public class EnergyIndustryModel
    {
        public Guid EnergyIndustryId { get; set; }
        public string EnergyIndustryCode { get; set; } = "";
        public string EnergyIndustryName { get; set; } = "";
        public bool IsDel { get; set; }
    }
    public class removeListEnergyIndustryItems
    {
        public List<Guid> EnergyIndustryIds { get; set; }
    }
}