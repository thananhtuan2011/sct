namespace API_SoCongThuong.Models
{
    public class TypeOfMarketModel
    {
        public Guid TypeOfMarketId { get; set; }
        public string TypeOfMarketCode { get; set; } = "";
        public string TypeOfMarketName { get; set; } = "";
        public bool IsDel { get; set; }
    }
    public class removeListTypeOfMarketItems
    {
        public List<Guid> TypeOfMarketIds { get; set; }
    }
}