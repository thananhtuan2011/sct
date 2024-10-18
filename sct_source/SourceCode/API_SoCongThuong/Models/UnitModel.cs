namespace API_SoCongThuong.Models
{
    public class UnitModel
    {
        public Guid UnitId { get; set; }
        public string? UnitCode { get; set; }
        public string UnitName { get; set; } = null!;
        public string? UnitNameEn { get; set; }
        public decimal? Exchange { get; set; }
        public string? Note { get; set; }
        public bool IsDel { get; set; }
    }
    public class removeListUnitItems
    {
        public List<Guid> UnitIds { get; set; }
    }
}