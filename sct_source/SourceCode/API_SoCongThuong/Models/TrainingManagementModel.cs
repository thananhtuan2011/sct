using System;
using System.Collections.Generic;

namespace API_SoCongThuong.Models
{
    public partial class TrainingManagementModel
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
        public string? StartDateDisplay { get; set; }
        /// <summary>
        /// Ngày kết thúc
        /// </summary>
        public DateTime? EndDate { get; set; }
        public string? EndDateDisplay { get; set; }
        /// <summary>
        /// Thời lượng
        /// </summary>
        public string? Time { get; set; }
        /// <summary>
        /// Huyện
        /// </summary>
        public Guid DistrictId { get; set; }
        /// <summary>
        /// Tên huyện
        /// </summary>
        public string? DistrictName { get; set; }
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
        /// Ngày tạo
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// Người tạo
        /// </summary>
        public Guid CreateUserId { get; set; }
        /// <summary>
        /// Ngày cập nhật
        /// </summary>
        public DateTime? UpdateTime { get; set; }
        /// <summary>
        /// Người cập nhật
        /// </summary>
        public Guid? UpdateUserId { get; set; }
        public List<TrainingManagementAttachFileModel> Details { get; set; } = new List<TrainingManagementAttachFileModel>();

        public string? IdFiles { get; set; }
    }

    public partial class TrainingManagementAttachFileModel
    {
        public Guid TrainingManagementAttachFileId { get; set; }
        public Guid TrainingManagementId { get; set; }
        public string LinkFile { get; set; } = null!;
        public bool IsDel { get; set; } = false;
    }
}
