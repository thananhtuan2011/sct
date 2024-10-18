using System;
using System.Collections.Generic;

namespace EF_Core.Models
{
    public partial class EnvironmentProjectManagement
    {
        /// <summary>
        /// Quản lý đề án
        /// </summary>
        public Guid EnvironmentProjectManagementId { get; set; }
        /// <summary>
        /// Tên dự án / đề án
        /// </summary>
        public string ProjectName { get; set; } = null!;
        /// <summary>
        /// Nội dung thực hiện
        /// </summary>
        public string ImplementationContent { get; set; } = null!;
        /// <summary>
        /// Kinh phí được duyệt
        /// </summary>
        public string ApprovedFunding { get; set; } = null!;
        /// <summary>
        /// Kinh phí thực hiện
        /// </summary>
        public string ImplementationCost { get; set; } = null!;
        /// <summary>
        /// Đơn vị phối hợp
        /// </summary>
        public string CoordinationUnit { get; set; } = null!;
        /// <summary>
        /// Năm thực hiện
        /// </summary>
        public int YearOfImplementationFrom { get; set; }
        public int YearOfImplementationTo { get; set; }
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
