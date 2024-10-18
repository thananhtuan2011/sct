using System;
using System.Collections.Generic;

namespace API_SoCongThuong.Models
{
    public partial class TradePromotionOtherModel
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
        public string? StartDateDisplay { get; set; }
        /// <summary>
        /// Ngày kết thúc
        /// </summary>
        public DateTime? EndDate { get; set; }
        public string? EndDateDisplay { get; set; }
        /// <summary>
        /// Thời lượng / Số lượng, thời lượng phát sóng
        /// </summary>
        public string? Time { get; set; }
        /// <summary>
        /// Huyện
        /// </summary>
        public Guid DistrictId { get; set; }
        public string? DistrictName { get; set; }
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
        /// Thời gian tạo
        /// </summary>
        public DateTime? CreateTime { get; set; }
        /// <summary>
        /// Người tạo
        /// </summary>
        public Guid? CreateUserId { get; set; }
        /// <summary>
        /// Thời gian cập nhật
        /// </summary>
        public DateTime? UpdateTime { get; set; }
        /// <summary>
        /// Người cập nhật
        /// </summary>
        public Guid? UpdateUserId { get; set; }
        public List<TradePromotionOtherDetailModel> Details { get; set; } = new List<TradePromotionOtherDetailModel>();
        public string LIdDel { get; set; } = "";
    }
    public partial class TradePromotionOtherDetailModel
    {
        public Guid TradePromotionOtherAttachFileId { get; set; }
        public Guid TradePromotionOtherId { get; set; }
        public string LinkFile { get; set; } = null!;
    }
}

