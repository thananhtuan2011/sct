namespace API_SoCongThuong.Models
{
    public class MultiLevelSalesManagementModel
    {
        /// Quản lý cơ sở hoạt động bán hàng đa cấp
        public Guid MultiLevelSalesManagementId { get; set; }

        /// Doanh nghiệp
        public Guid BusinessId { get; set; }
        public string BusinessName { get; set; } = "";

        /// Ngày bắt đầu hoạt động
        public DateTime StartDate { get; set; }

        /// Năm báo cáo
        public int YearReport { get; set; }

        /// Địa điểm hoạt động bán hàng đa cấp
        public string? MultiLevelSellingPlace { get; set; }

        /// Người liên hệ
        public string? ContactPersonName { get; set; }

        /// Số điện thoại người liên hệ
        public string? ContactPersonPhoneNumber { get; set; }

        /// Địa chỉ người liên hệ
        public string? ContactPersonAddress { get; set; }

        /// Số người tham gia bán hàng đa cấp
        public int Participants { get; set; }

        /// Số người tham gia bán hàng đa cấp phát sinh mới
        public int NewParticipants { get; set; }

        /// Số người tham gia bán hàng đa cấp chấm dứt hợp đồng
        public int Terminations { get; set; }

        /// Số lượng đào tạo căn bản
        public int BasicTrainings { get; set; }

        /// Doanh thu bán hàng đa cấp trên địa bàn tỉnh (triệu đồng)
        public int Turnover { get; set; }

        /// Tổng hoa hồng, tiền thưởng, lợi ích kinh tế đã nhận (Triệu đồng)
        public int Commission { get; set; }

        /// Giá trị khuyến mãi quy đổi thành tiền (Triệu đồng)
        public int? PromotionalValue { get; set; }

        /// Khấu trừ thuế thu nhập cá nhân (Triệu đồng)
        public int? TaxDeduction { get; set; }

        /// Mua lại hàng hoá từ người tham gia bán hàng đa cấp (Triệu đồng)
        public int? BuyBackGoods { get; set; }

        /// 1: Đã xóa; 0: Chưa xóa
        public bool IsDel { get; set; }
    }
}