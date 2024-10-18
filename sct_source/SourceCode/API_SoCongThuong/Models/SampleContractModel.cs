namespace API_SoCongThuong.Models
{
    public class SampleContractModel
    {
        
        /// Bảng hợp đồng mẫu - Sở thanh tra
        public Guid SampleContractId { get; set; }
        
        /// Lĩnh vực giải quyết
        public Guid SampleContractField { get; set; }
        public string? SampleContractFieldName { get; set; } = "";
        
        /// Thời gian đăng ký
        public DateTime RegistrationTime { get; set; }
        
        /// Số hồ sơ        
        public string ProfileNumber { get; set; } = null!;
        
        /// Tên người đăng ký        
        public string RegistrantName { get; set; } = null!;
        
        /// Số điện thoại liên hệ        
        public string PhoneNumber { get; set; } = null!;
        
        /// Tên cơ quan/ tổ chức        
        public string BusinessName { get; set; } = null!;
        
        /// Mã số thuế        
        public string TaxCode { get; set; } = null!;
        
        /// Số điện thoại cơ quan / tổ chức        
        public string? BusinessPhoneNumber { get; set; }
        
        /// Địa chỉ        
        public string? Address { get; set; }
        
        /// 1: Đã xóa; 0: Chưa xóa        
        public bool IsDel { get; set; }
    }
}