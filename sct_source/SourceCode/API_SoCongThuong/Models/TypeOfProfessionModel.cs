namespace API_SoCongThuong.Models
{
    public class TypeOfProfessionModel
    {
        public Guid TypeOfProfessionId { get; set; }
        public string TypeOfProfessionCode { get; set; } = "";
        public string TypeOfProfessionName { get; set; } = "";
        public bool IsDel { get; set; }
    }
    public class removeListTypeOfProfessionItems
    {
        public List<Guid> TypeOfProfessionIds { get; set; }
    }
}