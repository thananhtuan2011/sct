namespace API_SoCongThuong.Models
{
    public class TradePromotionProjectModel
    {
        public Guid TradePromotionProjectId { get; set; }
        public string TradePromotionProjectCode { get; set; } = "";
        public string TradePromotionProjectName { get; set; } = "";
        public bool IsDel { get; set; }
    }
    public class removeListTradePromotionProjectItems
    {
        public List<Guid> TradePromotionProjectIds { get; set; }
    }
}