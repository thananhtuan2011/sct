using System;
using System.Collections.Generic;

namespace EF_Core.Models
{
    public partial class TradePromotionOther
    {
        /// <summary>
        /// Bảng quản lý xúc tiến thương mại khác
        /// </summary>
        public Guid TradePromotionOtherId { get; set; }
        /// <summary>
        /// Loại hình hoạt động
        /// </summary>
        public int TypeOfActivity { get; set; }
        /// <summary>
        /// Nội dung hoạt động
        /// </summary>
        public string Content { get; set; } = null!;
        /// <summary>
        /// Ngày bắt đầu
        /// </summary>
        public DateTime StartDate { get; set; }
        /// <summary>
        /// Ngày kết thúc
        /// </summary>
        public DateTime? EndDate { get; set; }
        /// <summary>
        /// Thời lượng / Số lượng, thời lượng phát sóng
        /// </summary>
        public string? Time { get; set; }
        /// <summary>
        /// Huyện
        /// </summary>
        public Guid DistrictId { get; set; }
        /// <summary>
        /// Địa chỉ
        /// </summary>
        public string Address { get; set; } = null!;
        /// <summary>
        /// Kinh phí thực hiện
        /// </summary>
        public long ImplementationCost { get; set; }
        /// <summary>
        /// Đơn vị tham gia, kinh phí thực hiện
        /// </summary>
        public string? Participating { get; set; }
        /// <summary>
        /// Đơn vị kết nối
        /// </summary>
        public string? Coordination { get; set; }
        /// <summary>
        /// Kết quả
        /// </summary>
        public string? Result { get; set; }
        /// <summary>
        /// Ghi chú
        /// </summary>
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
    }
}
