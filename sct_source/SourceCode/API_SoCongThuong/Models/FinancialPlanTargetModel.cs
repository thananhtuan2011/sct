namespace API_SoCongThuong.Models
{
    public class FinancialPlanTargetModel
    {
        /// Bảng các chỉ tiêu sản xuất kinh doanh, xuất khẩu chủ yếu
        public Guid FinancialPlanTargetsId { get; set; }
        /// Loại hình: 
        /// 1 - Giá trị sản xuất, 
        /// 2 - Sản phẩm chủ yếu, 
        /// 3 - Khối Doanh nghiệp (Xuất khẩu), 
        /// 4 - Nhóm hàng (Xuất khẩu), 
        /// 5 - Mặt hàng (Xuất khẩu), 
        /// 6 - Thị trường (Xuất khẩu), 
        /// 7 - Mặt hàng  chủ yếu (Nhập khẩu)
        public int Type { get; set; }
        /// Tên
        public string Name { get; set; } = null!;
        /// Đơn vị tính
        public string Unit { get; set; } = null!;
        /// Tháng báo cáo
        public string Date { get; set; } = null!;
        /// Năm
        public int? Year { get; set; }
        /// Tháng
        public int? Month { get; set; }
        /// Kế hoạch năm
        public decimal Plan { get; set; }
        /// Thực hiện cùng tháng năm trước
        public decimal ValueSameMonthLastYear { get; set; }
        /// Thực hiện tháng trước
        public decimal ValueLastMonth { get; set; }
        /// Ước tính tháng thực hiện
        public decimal EstimatedMonth { get; set; }
        /// Cộng dồn đến tháng
        public decimal CumulativeToMonth { get; set; }
        /// Cộng dồn đến tháng năm trước
        public decimal CumulativeToMonthLastYear { get; set; }
        /// So sánh tháng trước: = EstimatedMonth / ValueLastMonth
        public decimal? CompareLastMonth { get; set; } = 0;
        /// So sánh tháng cùng kỳ = EstimatedMonth / ValueSameMonthLastYear
        public decimal? ComparedSameMonth { get; set; } = 0;
        /// Luỹ kế so kế hoạch năm = EstimatedMonth / Plan
        public decimal? AccumulatedComparedYearPlan { get; set; } = 0;
        /// Luỹ kế so cùng kỳ = CumulativeToMonth / CumulativeToMonthLastYear
        public decimal? AccumulatedComparedPeriod { get; set; } = 0;
        /// 1: Hoạt động 0: Không hoạt động
        public bool? IsAction { get; set; }
        /// 1: Đã xóa; 0: Chưa xóa
        public bool IsDel { get; set; }
        /// Thời gian tạo
        public DateTime CreateTime { get; set; }
        /// Người tạo
        public Guid CreateUserId { get; set; }
        public DateTime? UpdateTime { get; set; }
        public Guid? UpdateUserId { get; set; }
    }
}