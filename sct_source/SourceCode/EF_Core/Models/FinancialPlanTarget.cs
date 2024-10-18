using System;
using System.Collections.Generic;

namespace EF_Core.Models
{
    public partial class FinancialPlanTarget
    {
        /// <summary>
        /// Bảng các chỉ tiêu sản xuất kinh doanh, xuất khẩu chủ yếu
        /// </summary>
        public Guid FinancialPlanTargetsId { get; set; }
        /// <summary>
        /// Loại hình: 1 - Giá trị sản xuất, 2 - Sản phẩm chủ yếu, 3 - Khối Doanh nghiệp (Xuất khẩu), 4 - Nhóm hàng (Xuất khẩu), 5 - Mặt hàng (Xuất khẩu), 6 - Thị trường (Xuất khẩu), 7 - Mặt hàng  chủ yếu (Nhập khẩu)
        /// </summary>
        public int Type { get; set; }
        /// <summary>
        /// Tên
        /// </summary>
        public string Name { get; set; } = null!;
        /// <summary>
        /// Đơn vị tính
        /// </summary>
        public string Unit { get; set; } = null!;
        /// <summary>
        /// Năm báo cáo
        /// </summary>
        public int Year { get; set; }
        public int Month { get; set; }
        public decimal Planning { get; set; }
        /// <summary>
        /// Ước tính tháng thực hiện
        /// </summary>
        public decimal EstimatedMonth { get; set; }
        /// <summary>
        /// Cộng dồn đến tháng
        /// </summary>
        public decimal CumulativeToMonth { get; set; }
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
