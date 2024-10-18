using System;
using System.Collections.Generic;

namespace API_SoCongThuong.Models
{
    public partial class CateProjectModel
    {
        public Guid CateProjectId { get; set; }
        /// <summary>
        /// lấy từ bảng Category,  type = TYPE_OF_PROJECT
        /// </summary>
        public int ProjectType { get; set; }
        public string ProjectTypeName { get; set; }
        /// <summary>
        /// 1: Trong nước, 2: ngoài nước
        /// </summary>
        public int Area { get; set; }
        public string Investors { get; set; } = "";
        public string Address { get; set; } = "";
        public string InvestmentCertificateCode { get; set; } = "";
        public DateTime? InvestmentCertificateDate { get; set; }
        public string PolicyDecisions { get; set; } = "";
        public DateTime? PolicyDecisionsDate { get; set; }
        public Guid ProjectId { get; set; }
        public string ProjectName { get; set; } = "";
        public long ProjectInvestment { get; set; }
        public Guid ProjectInvestmentUnits { get; set; }
        /// <summary>
        /// Địa điểm thực hiện dự án
        /// </summary>
        public string ProjectAddress { get; set; } = "";
        /// <summary>
        /// ngành nghề
        /// </summary>
        public string Profession { get; set; } = "";
        /// <summary>
        /// người đại diện pháp luật
        /// </summary>
        public string ProjectLegalRepresent { get; set; } = "";
        /// <summary>
        /// số diện thoại liên lạc
        /// </summary>
        public string ProjectPhoneNumber { get; set; } = "";
        /// <summary>
        /// tiến độ thực hiện dự án, tiến độ đã đăng ký
        /// </summary>
        public string ProjectProgress { get; set; } = "";
        /// <summary>
        /// thời gian thực hiện (năm)
        /// </summary>
        public int ProjectOperatingTime { get; set; }
        /// <summary>
        /// tiến độ thực tế
        /// </summary>
        public string ProjectProgressActual { get; set; } = "";
        /// <summary>
        /// địa bàn
        /// </summary>
        public string ProjectLocalArea { get; set; } = "";
        /// <summary>
        /// quốc tịch/đối tác
        /// </summary>
        public string ProjectPartnerNationality { get; set; } = "";
        /// <summary>
        /// hình thức đầu tư
        /// </summary>
        public string ProjectInvestmentForm { get; set; } = "";
        /// <summary>
        /// năm cấp phép
        /// </summary>
        public int ProjectLicenseYear { get; set; }
        /// <summary>
        /// Năm thực hiện
        /// </summary>
        public int ProjectImplementationYear { get; set; }
        /// <summary>
        /// mục tiêu, quy mô thực hiện dự án
        /// </summary>
        public string ProjectImplementationScale { get; set; } = "";
        /// <summary>
        /// quyết định thu hồi
        /// </summary>
        public string ProjectDecisionToWithdraw { get; set; } = "";
        public DateTime? ProjectDecisionToWithdrawDate { get; set; }
        /// <summary>
        /// FDI
        /// </summary>
        public string ProjectFdi { get; set; } = "";
        public string? Note { get; set; }
        /// <summary>
        /// công ty bán
        /// </summary>
        public string CompanyBuy { get; set; } = "";
        /// <summary>
        /// lấy từ danh mục quốc gia
        /// </summary>
        public Guid Country { get; set; }
        public string CountryName { get; set; } = "";
        /// <summary>
        /// Công ty bán/công ty nhận, lấy từ danh mục doanh nghiệp
        /// </summary>
        public Guid CompanySell { get; set; }
        public string CompanySellName { get; set; } = "";
        /// <summary>
        /// vốn điều lệ ban đầu
        /// </summary>
        public long InitialCharterCapital { get; set; }
        /// <summary>
        /// lấy từ bảng units
        /// </summary>
        public Guid Units { get; set; }
        /// <summary>
        /// vốn mua
        /// </summary>
        public long CapitalPurchase { get; set; }
        /// <summary>
        /// mua thực tế
        /// </summary>
        public long ActualPurchase { get; set; }
        /// <summary>
        /// vốn điều lệ sau khi mua
        /// </summary>
        public long CharterCapitalAfterPurchase { get; set; }
        public DateTime? CapitalContributionTradingTime { get; set; }
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
        public string CreateTimeDisplay { get; set; } = "";
        /// <summary>
        /// Người tạo
        /// </summary>
        public Guid CreateUserId { get; set; }
        public string CreateName { get; set; } = "";
        public DateTime? UpdateTime { get; set; }
        public Guid? UpdateUserId { get; set; }
        public List<CateProjectDisbursementModel> Details { get; set; } = new List<CateProjectDisbursementModel>();
        public List<CateProjectHistoryModel> Historys { get; set; } = new List<CateProjectHistoryModel>();
    }

    public partial class CateProjectDisbursementModel
    {
        public Guid? CateProjectDisbursementId { get; set; }
        public Guid CateProjectId { get; set; }
        public DateTime DisbursementDate { get; set; }
        public long DisbursementMoney { get; set; }
        public Guid DisbursementUnits { get; set; }
        public bool IsConfirm { get; set; }
    }

    public partial class CateProjectHistoryModel
    {
        public Guid? CateProjectHistoryId { get; set; }
        public Guid CateProjectId { get; set; }
        public string ContentAdjust { get; set; } = null!   ;
        public DateTime? UpdateTime { get; set; }
        public string UpdateTimeDisplay { get; set; } = "";
        public Guid? UpdateUserId { get; set; }
        public string UpdateName { get; set; } = "";
    }
}
