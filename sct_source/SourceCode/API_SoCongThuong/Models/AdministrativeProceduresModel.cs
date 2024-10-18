namespace API_SoCongThuong.Models
{
    public class AdministrativeProceduresModel
    {
        /// Bảng thủ tục hành chính - Sở thanh tra
        public Guid AdministrativeProceduresId { get; set; }

        /// Lĩnh vực giải quyết
        public Guid AdministrativeProceduresField { get; set; }
        public string AdministrativeProceduresFieldName { get; set; } = "";

        /// Mã thủ tục
        public string AdministrativeProceduresCode { get; set; } = null!;

        /// Trạng thái - 1: Chưa xử lý, 2: Đang xử lý, 3: Đã xử lý
        public int Status { get; set; }
        public string StatusName { get; set; } = "";

        /// Hình thức tiếp nhận - 1: Trực tiếp, 2: Tiếp tuyến
        public int ReceptionForm { get; set; }
        public string ReceptionFormName { get; set; } = "";
        public string AdministrativeProceduresName { get; set; } = null!;
        public int AmountOfRecords { get; set; }
        public string DayReception { get; set; }
        public string SettlementTerm { get; set; }
        public string? FinishDay { get; set; }        
        /// 1: Đã xóa; 0: Chưa xóa
        public bool IsDel { get; set; }
        public DateTime CreateTime { get; set; }
        public Guid CreateUserId { get; set; }
        public DateTime? UpdateTime { get; set; }
        public Guid? UpdateUserId { get; set; }
    }
}