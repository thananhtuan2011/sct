using System;
using System.Collections.Generic;

namespace API_SoCongThuong.Models
{
    public partial class EnvironmentProjectManagementModel
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
        public string ApprovedFunding { get; set; }
        /// <summary>
        /// Kinh phí thực hiện
        /// </summary>
        public string ImplementationCost { get; set; }
        /// <summary>
        /// Đơn vị phối hợp
        /// </summary>
        public string CoordinationUnit { get; set; } = null!;
        /// <summary>
        /// Năm thực hiện, từ năm
        /// </summary>
        public int YearOfImplementationFrom { get; set; }
        /// <summary>
        /// Năm thực hiện, đến năm
        /// </summary>
        public int YearOfImplementationTo { get; set; }
        /// <summary>
        /// 1: Hoạt động 0: Không hoạt động
        /// </summary>
        public bool IsDel { get; set; }
        /// <summary>
        /// 1: Hoạt động 0: Không hoạt động
        /// </summary>
        public DateTime CreateTime { get; set; }
        public Guid CreateUserId { get; set; }
        public DateTime? UpdateTime { get; set; }
        public Guid? UpdateUserId { get; set; }
        public List<EnvironmentProjectManagementAttachFileModel> FileUpload { get; set; } = new List<EnvironmentProjectManagementAttachFileModel>();

        public string? IdFiles { get; set; }
    }

    public partial class EnvironmentProjectManagementAttachFileModel
    {
        public Guid EnvironmentProjectManagementAttachFileId { get; set; }
        public Guid EnvironmentProjectManagementId { get; set; }
        public string LinkFile { get; set; } = null!;
    }
}
