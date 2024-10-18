using System;
using System.Collections.Generic;

namespace API_SoCongThuong.Models
{
    public partial class CateCriteriaNumberSevenModel
    {
        public Guid CateCriteriaNumberSevenId { get; set; }
        public string CateCriteriaNumberSevenCode { get; set; }
        public Guid CheckUserId { get; set; }
        public string CheckName { get; set; } = "";
        public Guid ConfirmUserId { get; set; }
        public string ConfirmName { get; set; } = "";
        public DateTime? ConfirmTime { get; set; }
        public string ConfirmTimeDisplay { get; set; } = "";
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
        public List<CateCriteriaNumberSevenDetailModel> Details { get; set; } = new List<CateCriteriaNumberSevenDetailModel>();
        public string? ReportMonth { get; set; }
    }

    public partial class CateCriteriaNumberSevenDetailModel
    {
        public Guid? CateCriteriaNumberSevenDetailId { get; set; }
        public Guid CateCriteriaNumberSevenId { get; set; }
        public Guid DistrictId { get; set; }
        public string DistrictName { get; set; }
        /// <summary>
        /// số lượng xã trong huyện
        /// </summary>
        public int NumberOfWard { get; set; }
        /// <summary>
        /// số xã đạt tiêu chuẩn tiêu chí số 7
        /// </summary>
        public int NumberOfQualifyingWard { get; set; }
        public int NumberOfWardWithMarket { get; set; }
        /// <summary>
        /// số lượng xã đạt tiêu chuẩn hạ tầng thương mại 
        /// </summary>
        public int NumberOfWardCommercialInfrastructure { get; set; }
        /// <summary>
        /// số xã đạt nông thôn mới
        /// </summary>
        public int NumberOfWardNewCountryside { get; set; }
        /// <summary>
        /// số lượng xã đạt tiêu chuẩn hạ tầng thương mại 
        /// </summary>
        public int NumberOfWardCommercialInfrastructurePlan { get; set; }
        /// <summary>
        /// số xã đạt nông thôn mới
        /// </summary>
        public int NumberOfWardNewCountrysidePlan { get; set; }
        /// <summary>
        /// số lượng xã đạt tiêu chuẩn hạ tầng thương mại 
        /// </summary>
        public int NumberOfWardCommercialInfrastructureEstimate { get; set; }
        /// <summary>
        /// số xã đạt nông thôn mới
        /// </summary>
        public int NumberOfWardNewCountrysideEstimate { get; set; }
    }
}
