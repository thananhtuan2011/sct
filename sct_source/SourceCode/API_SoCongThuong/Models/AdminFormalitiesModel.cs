namespace API_SoCongThuong.Models
{
    public class AdminFormalitiesModel
    {
        public Guid AdminFormalitiesId { get; set; }
        public string AdminFormalitiesCode { get; set; } = null!;
        public string AdminFormalitiesName { get; set; } = null!;
        public Guid FieldId { get; set; }
        public int DVCLevel { get; set; }
        public string DocUrl { get; set; } = null!;
        public bool IsDel { get; set; }
    }
    public class removeListAdminFormalitiesItems
    {
        public List<Guid> AdminFormalitiesIds { get; set; }
    }

    public class AdminFormalitiesViewModel
    {
        public Guid AdminFormalitiesId { get; set; }
        public string AdminFormalitiesCode { get; set; } = null!;
        public string AdminFormalitiesName { get; set; } = null!;
        public Guid FieldId { get; set; }
        public string FieldName { get; set; } = null!;
        public int DVCLevel { get; set; }
        public string DocUrl { get; set; } = null!;
        public bool IsDel { get; set; }
    }

    public class FieldView
    {
        public Guid FieldId { get; set; }
        public string FieldName { get; set; } = null!;
        public int Priority { get; set; }
    }
}