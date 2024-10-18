using System;
using System.Collections.Generic;

namespace EF_Core.Models
{
    public partial class MultiLevelSalesManagement
    {
        /// <summary>
        /// Quản lý cơ sở hoạt động bán hàng đa cấp
        /// </summary>
        public Guid MultiLevelSalesManagementId { get; set; }
        /// <summary>
        /// Doanh nghiệp
        /// </summary>
        public Guid BusinessId { get; set; }
        /// <summary>
        /// Ngày bắt đầu hoạt động
        /// </summary>
        public DateTime StartDate { get; set; }
        /// <summary>
        /// Năm báo cáo
        /// </summary>
        public int YearReport { get; set; }
        /// <summary>
        /// Địa điểm hoạt động bán hàng đa cấp
        /// </summary>
        public string? MultiLevelSellingPlace { get; set; }
        /// <summary>
        /// Người liên hệ
        /// </summary>
        public string? ContactPersonName { get; set; }
        /// <summary>
        /// Số điện thoại người liên hệ
        /// </summary>
        public string? ContactPersonPhoneNumber { get; set; }
        /// <summary>
        /// Địa chỉ người liên hệ
        /// </summary>
        public string? ContactPersonAddress { get; set; }
        /// <summary>
        /// Số người tham gia bán hàng đa cấp
        /// </summary>
        public int Participants { get; set; }
        /// <summary>
        /// Số người tham gia bán hàng đa cấp phát sinh mới
        /// </summary>
        public int NewParticipants { get; set; }
        /// <summary>
        /// Số người tham gia bán hàng đa cấp chấm dứt hợp đồng
        /// </summary>
        public int Terminations { get; set; }
        /// <summary>
        /// Số lượng đào tạo căn bản
        /// </summary>
        public int BasicTrainings { get; set; }
        /// <summary>
        /// Doanh thu bán hàng đa cấp trên địa bàn tỉnh (triệu đồng)
        /// </summary>
        public int Turnover { get; set; }
        /// <summary>
        /// Tổng hoa hồng, tiền thưởng, lợi ích kinh tế đã nhận (Triệu đồng)
        /// </summary>
        public int Commission { get; set; }
        /// <summary>
        /// Giá trị khuyến mãi quy đổi thành tiền (Triệu đồng)
        /// </summary>
        public int? PromotionalValue { get; set; }
        /// <summary>
        /// Khấu trừ thuế thu nhập cá nhân (Triệu đồng)
        /// </summary>
        public int? TaxDeduction { get; set; }
        /// <summary>
        /// Mua lại hàng hoá từ người tham gia bán hàng đa cấp (Triệu đồng)
        /// </summary>
        public int? BuyBackGoods { get; set; }
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
