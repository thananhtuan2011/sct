namespace API_SoCongThuong.Models
{
    public class SysLogsModel
    {
        public Guid LogId { get; set; } = Guid.Empty;
        public string TenDangNhap { get; set; }
        public string Ip { get; set; }
        public string ActionName { get; set; }
        public string ActionType { get; set; }
        public string Content { get; set; }
        public string Time { get; set; }
        public DateTime? Date { get; set; }
    }
}
