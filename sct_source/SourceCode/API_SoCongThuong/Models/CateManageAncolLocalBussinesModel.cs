using System;
using System.Collections.Generic;

namespace API_SoCongThuong.Models
{
    public partial class CateManageAncolLocalBussinesModel
    {
        public Guid CateManageAncolLocalBussinessId { get; set; }
        public bool? IsActive { get; set; }
        public Guid BusinessId { get; set; }
        public string BusinessName { get; set; } = "";
        public string BusinessCode { get; set; } = "";
        public long Investment { get; set; }
        public int NumberOfWorker { get; set; }
        /// <summary>
        /// lấy từ bảng TypeOfProfession, một loại ngành nghề chính
        /// </summary>
        public Guid TypeOfProfessionId { get; set; }
        public string TypeOfProfessionName { get; set; } = "";
        public DateTime DateRelease { get; set; }
        public string DateReleaseDisplay { get; set; } = "";
        public DateTime? DateChange { get; set; }
        public string DateChangeDisplay { get; set; } = "";
        /// <summary>
        /// 1: Hoạt động 0: Không hoạt động
        /// </summary>
        public bool? IsAction { get; set; }
        public string ActionName { get; set; } = "";
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
        public string CreateName { get;set; } = "";
        public DateTime? UpdateTime { get; set; }
        public Guid? UpdateUserId { get; set; }
        public string RepresentName { get; set; } = "";
        public List<CateManageAncolLocalBussinesDetailModel> LstWorkers { get; set; } = new List<CateManageAncolLocalBussinesDetailModel>();
        public List<CateManageAncolLocalBussinesTypeOfProfessionModel> LstProfession { get; set; } = new List<CateManageAncolLocalBussinesTypeOfProfessionModel>();
    }
    public partial class CateManageAncolLocalBussinesDetailModel
    {
        public Guid? CateManageAncolLocalBussinesDetailId { get; set; }
        public string Fullname { get; set; } = null!;
        /// <summary>
        /// 1: thành viên góp vốn
        /// 2: cổ đông
        /// </summary>
        public int Type { get; set; }
        public bool IsDel { get; set; }
    }
    public partial class CateManageAncolLocalBussinesTypeOfProfessionModel
    {
        public Guid? CateManageAncolLocalBussinesTypeProfessionId { get; set; }
        public Guid TypeOfProfessionId { get; set; }
        public string? TypeOfProfessionName { get; set; } = "";
        public bool IsDel { get; set; }
    }

    public partial class ExportCateManageAncolLocalBussinesModel
    {
        public Guid CateManageAncolLocalBussinessId { get; set; }
        public string BusinessCode { get; set; } = null!;
        public string BusinessName { get; set; } = null!;
        public string? Address { get; set; }
        public string District { get; set; } = null!;
        public string Commune { get; set; } = null!;
        public long Investment { get; set; }
        public string ActionName { get; set; } = null!;
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public string? RepresentName { get; set; }
        public string? RepresentBirthDay { get; set; }
        public string? CitizenId { get; set; }
        public string? IssuerCitizenId { get; set; }
        public string? CitizenIdDate { get; set; }
        public string? Owner { get; set; }
        public int NumberOfWorker { get; set; }
        public string? TypeOfProfessionName { get; set; }
        public string DateReleaseDisplay { get; set; } = null!;
        public DateTime DateRelease { get; set; }
        public string? DateChangeDisplay { get; set; }
        public string TypeOfBusinessName { get; set; } = null!;
        public string? LstProfession { get; set; }
        public string? LstCapitalContributing  { get; set; }
        public string? LstShareholder { get; set; }
    }
}
