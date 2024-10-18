using System;
using System.Collections.Generic;

namespace EF_Core.Models
{
    public partial class AdministrativeProcedure
    {
        /// <summary>
        /// Bảng thủ tục hành chính - Sở thanh tra
        /// </summary>
        public Guid AdministrativeProceduresId { get; set; }
        /// <summary>
        /// Lĩnh vực giải quyết
        /// </summary>
        public Guid AdministrativeProceduresField { get; set; }
        /// <summary>
        /// Mã thủ tục
        /// </summary>
        public string AdministrativeProceduresCode { get; set; } = null!;
        /// <summary>
        /// Trạng thái - 1: Chưa xử lý, 2: Đang xử lý, 3: Đã xử lý
        /// </summary>
        public int Status { get; set; }
        /// <summary>
        /// Hình thức tiếp nhận - 1: Trực tiếp, 2: Tiếp tuyến
        /// </summary>
        public int ReceptionForm { get; set; }
        public string AdministrativeProceduresName { get; set; } = null!;
        public int AmountOfRecords { get; set; }
        public DateTime DayReception { get; set; }
        public DateTime SettlementTerm { get; set; }
        public DateTime? FinishDay { get; set; }
        /// <summary>
        /// 1: Hoạt động 0: Không hoạt động
        /// </summary>
        public bool? IsAction { get; set; }
        /// <summary>
        /// 1: Đã xóa; 0: Chưa xóa
        /// </summary>
        public bool IsDel { get; set; }
        /// <summary>
        /// Thời gian tạo
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// Người tạo
        /// </summary>
        public Guid CreateUserId { get; set; }
        public DateTime? UpdateTime { get; set; }
        public Guid? UpdateUserId { get; set; }
    }
}
