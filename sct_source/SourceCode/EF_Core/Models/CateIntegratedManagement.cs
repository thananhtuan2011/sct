using System;
using System.Collections.Generic;

namespace EF_Core.Models
{
    public partial class CateIntegratedManagement
    {
        public Guid IntegratedManagementId { get; set; }
        public int ProjectType { get; set; }
        /// <summary>
        /// 1: Trong nước, 2: ngoài nước
        /// </summary>
        public int Area { get; set; }
        public string Investors { get; set; } = null!;
        public string Address { get; set; } = null!;
        public string InvestmentCertificateCode { get; set; } = null!;
        public DateTime InvestmentCertificateDate { get; set; }
        public string PolicyDecisions { get; set; } = null!;
        public DateTime PolicyDecisionsDate { get; set; }
        public string ProjectName { get; set; } = null!;
        public int ProjectInvestment { get; set; }
        public Guid ProjectInvestmentUnits { get; set; }
        /// <summary>
        /// Địa điểm thực hiện dự án
        /// </summary>
        public string ProjectAddress { get; set; } = null!;
        /// <summary>
        /// mục tiêu, quy mô thực hiện dự án
        /// </summary>
        public string ProjectImplementationScale { get; set; } = null!;
        /// <summary>
        /// người đại diện pháp luật
        /// </summary>
        public string ProjectLegalRepresent { get; set; } = null!;
        /// <summary>
        /// số diện thoại liên lạc
        /// </summary>
        public string ProjectPhoneNumber { get; set; } = null!;
        /// <summary>
        /// tiến độ thực hiện dự án, tiến độ đã đăng ký
        /// </summary>
        public string ProjectProgress { get; set; } = null!;
        /// <summary>
        /// thời gian thực hiện (năm)
        /// </summary>
        public int ProjectOperatingTime { get; set; }
        /// <summary>
        /// tiến độ thực tế
        /// </summary>
        public string ProjectProgressActual { get; set; } = null!;
        /// <summary>
        /// địa bàn
        /// </summary>
        public string ProjectLocalArea { get; set; } = null!;
        /// <summary>
        /// năm cấp phép
        /// </summary>
        public int ProjectLicenseYear { get; set; }
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
