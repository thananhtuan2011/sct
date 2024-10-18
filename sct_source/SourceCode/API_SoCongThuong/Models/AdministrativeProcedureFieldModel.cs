namespace API_SoCongThuong.Models
{
    public class AdministrativeProcedureFieldModel
    {
        public Guid CategoryId { get; set; }
        public string CategoryCode { get; set; } = null!;
        public string CategoryName { get; set; } = null!;
        public bool IsAction { get; set; } = true;
        public int Piority { get; set; } = 0;
    }
}