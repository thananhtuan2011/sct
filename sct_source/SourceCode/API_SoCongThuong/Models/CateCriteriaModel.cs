namespace API_SoCongThuong.Models
{
    public class CateCriteriaModel
    {
        public Guid CateCriteriaId { get; set; }
        public string CateCriteriaName { get; set; }
        public string CreateName { get; set; } = "";
        public DateTime CreateTime { get; set; }
        public bool IsActive { get; set; }
        public bool IsDel { get; set; }
    }
    public class removeListCateCriteriaItems
    {
        public List<Guid> CateCriteriaIds { get; set; }
    }
}
