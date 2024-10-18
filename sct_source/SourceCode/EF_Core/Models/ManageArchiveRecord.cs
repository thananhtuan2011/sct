using System;
using System.Collections.Generic;

namespace EF_Core.Models
{
    public partial class ManageArchiveRecord
    {
        /// <summary>
        /// Bảng quản lý lưu trữ hồ sơ
        /// </summary>
        public Guid ManageArchiveRecordsId { get; set; }
        /// <summary>
        /// Mã nhóm hồ sơ
        /// </summary>
        public int RecordsFinancePlanId { get; set; }
        /// <summary>
        /// Số và ký hiệu hồ sơ
        /// </summary>
        public string CodeFile { get; set; } = null!;
        /// <summary>
        /// Tiêu đề hồ sơ
        /// </summary>
        public string Title { get; set; } = null!;
        /// <summary>
        /// Thơi gian tiếp nhận
        /// </summary>
        public DateTime ReceptionTime { get; set; }
        /// <summary>
        /// Thời gian bảo quản
        /// </summary>
        public int StorageTime { get; set; }
        /// <summary>
        /// Địa điểm
        /// </summary>
        public string Location { get; set; } = null!;
        public string StoreDocumentsAt { get; set; } = null!;
        public string StoreFilesAt { get; set; } = null!;
        /// <summary>
        /// Đơn vị, người lập hồ sơ
        /// </summary>
        public string Creator { get; set; } = null!;
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
