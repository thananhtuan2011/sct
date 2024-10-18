namespace API_SoCongThuong.Models
{
    public class TypeOfEnergyModel
    {
        public Guid TypeOfEnergyId { get; set; }
        public string TypeOfEnergyCode { get; set; } = "";
        public string TypeOfEnergyName { get; set; } = "";
        public bool IsDel { get; set; }
    }
    public class removeListTypeOfEnergyItems
    {
        public List<Guid> TypeOfEnergyIds { get; set; }
    }
}