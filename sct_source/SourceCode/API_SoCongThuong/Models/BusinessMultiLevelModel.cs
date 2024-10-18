namespace API_SoCongThuong.Models
{
    public class BusinessMultiLevelModel
    {
        public Guid BusinessMultiLevelId { get; set; }
        public Guid BusinessId { get; set; }
        public string BusinessCode { get; set; } = "";
        public string BusinessName { get; set; } = "";
        public Guid DistrictId { get; set; }
        public string? Address { get; set; }
        public DateTime StartDate { get; set; }
        public Guid Status { get; set; }
        public string NumCert { get; set; } = null!;
        public DateTime CertDate { get; set; }
        public DateTime? CertExp { get; set; }
        public string? Contact { get; set; }
        public string? PhoneNumber { get; set; }
        public string? AddressContact { get; set; }
        public string? Goods { get; set; }
        public string? LocalConfirm { get; set; }
        public string? Note { get; set; }
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
        public string StatusName { get; set; } = "";
        public string DistrictName { get; set; } = "";
    }
}
