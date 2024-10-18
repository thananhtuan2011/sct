namespace API_SoCongThuong.Models
{
    public class StateUnitModel
    {
        public Guid StateUnitsId { get; set; }
        public string StateUnitsCode { get; set; } = "";
        public string StateUnitsName { get; set; } = "";
        public Guid? ParentId { get; set; }
        public string ParentName { get; set; } = "";
        public bool IsDel { get; set; }
    }
}