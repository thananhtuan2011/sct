using System;
using System.Collections.Generic;

namespace API_SoCongThuong.Models
{
    public partial class TradeFairOrganizationCertificationModel
    {

        /// Quản lý xác nhận tổ chức hội chợ triển lãm thương mại - Xúc tiến thương mại 
        public Guid TradeFairOrganizationCertificationId { get; set; }

        /// Tên hội chợ / Triển lãm thương mại 
        public string TradeFairName { get; set; } = null!;

        /// Địa điểm tổ chức 
        public string Address { get; set; } = null!;

        /// Quy mô
        public string? Scale { get; set; }

        /// Số văn bản
        public string TextNumber { get; set; } = null!;

        /// 1: Đã xóa; 0: Chưa xóa 
        public bool IsDel { get; set; }

        /// Thời gian tạo
        public DateTime CreateTime { get; set; }
        public Guid CreateUserId { get; set; }
        public string? CreateUserName { get; set; }
        public DateTime? UpdateTime { get; set; }
        public Guid? UpdateUserId { get; set; }
        public string? ListTimeString { get; set; }
        public List<TradeFairOrganizationCertificationAttachFileModel> ListFiles { get; set; } = new List<TradeFairOrganizationCertificationAttachFileModel>();
        public List<TradeFairOrganizationCertificationTimeModel> ListTimes { get; set; } = new List<TradeFairOrganizationCertificationTimeModel>();
        public string? IdFiles { get; set; }
    }

    public partial class TradeFairOrganizationCertificationAttachFileModel
    {
        public Guid TradeFairFileId { get; set; }
        public Guid TradeFairOrganizationCertificationId { get; set; }
        public string LinkFile { get; set; } = null!;
    }

    public partial class TradeFairOrganizationCertificationTimeModel
    {
        public Guid TradeFairTimeId { get; set; }
        public Guid TradeFairOrganizationCertificationId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }
}
