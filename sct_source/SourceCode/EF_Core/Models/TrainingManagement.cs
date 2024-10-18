using System;
using System.Collections.Generic;

namespace EF_Core.Models
{
    public partial class TrainingManagement
    {
        /// <summary>
        /// Quản lý đào tạo tập huấn
        /// </summary>
        public Guid TrainingManagementId { get; set; }
        /// <summary>
        /// Nội dung tập huấn
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
        /// Thời lượng
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
        /// Đơn vị tham gia
        /// </summary>
        public string Participating { get; set; } = null!;
        /// <summary>
        /// Số người tham gia
        /// </summary>
        public int NumParticipating { get; set; }
        /// <summary>
        /// Kinh phí thực hiện
        /// </summary>
        public long ImplementationCost { get; set; }
        /// <summary>
        /// Báo cáo viên
        /// </summary>
        public string? Annunciator { get; set; }
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
