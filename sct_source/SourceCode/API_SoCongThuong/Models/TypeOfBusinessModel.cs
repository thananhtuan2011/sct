namespace API_SoCongThuong.Models
{
    public class TypeOfBusinessModel
    {
        public Guid TypeOfBusinessId { get; set; }
        public string TypeOfBusinessCode { get; set; } = "";
        public string TypeOfBusinessName { get; set; } = "";
        public bool IsDel { get; set; }
    }
    public class removeListTypeOfBusinessItems
    {
        public List<Guid> TypeOfBusinessIds { get; set; }
    }
}