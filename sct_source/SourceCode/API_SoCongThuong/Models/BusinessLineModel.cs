namespace API_SoCongThuong.Models
{
    public class BusinessLineModel
    {
        public Guid BusinessLineId { get; set; }
        public string BusinessLineCode { get; set; } = "";
        public string BusinessLineName { get; set; } = "";
        public bool IsDel { get; set; }
    }
}