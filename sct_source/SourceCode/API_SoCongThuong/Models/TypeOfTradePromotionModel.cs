namespace API_SoCongThuong.Models
{
    public class TypeOfTradePromotionModel
    {
        public Guid TypeOfTradePromotionId { get; set; }
        public string TypeOfTradePromotionCode { get; set; } = "";
        public string TypeOfTradePromotionName { get; set; } = "";
        public bool IsDel { get; set; }
    }
    public class removeListTypeOfTradePromotionItems
    {
        public List<Guid> TypeOfTradePromotionIds { get; set; }
    }
}