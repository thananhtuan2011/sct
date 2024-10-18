namespace API_SoCongThuong.Models
{
    public class CommuneModel
    {
        public Guid CommuneId { get; set; }
        public string CommuneCode { get; set; } = "";
        public string CommuneName { get; set; } = "";
        public Guid DistrictId { get; set; }
        public string DistrictName { get; set; } = "";
        public bool IsDel { get; set; }
    }
    public class removeListCommuneItems
    {
        public List<Guid> CommuneIds { get; set; }
    }
}
