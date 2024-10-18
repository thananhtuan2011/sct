namespace API_SoCongThuong.Models
{
    public class StateTitleModel
    {
        public Guid StateTitlesId { get; set; }
        public string StateTitlesCode { get; set; } = "";
        public string StateTitlesName { get; set; } = "";
        public int Piority { get; set; }
        public bool IsDel { get; set; }
    }
    public class removeListStateTitleItems
    {
        public List<Guid> StateTitlesIds { get; set; }
    }
}