using System;
using System.Collections.Generic;

namespace API_SoCongThuong.Models
{
    public partial class ResultsIndustrialPromotionVotingModel
    {
        public Guid ResultsIndustrialPromotionVotingId { get; set; }
        public bool Locallity { get; set; }
        /// <summary>
        /// Chỉ tiêu
        /// </summary>
        public Guid Targets { get; set; }
        public int NumbersRegister { get; set; }
        public int NumberCertified { get; set; }
        /// <summary>
        /// Đơn vị tính
        /// </summary>
        public string? Unit { get; set; }
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
        public string ActionName { get; set; } = "";
        /// <summary>
        /// Người tạo
        /// </summary>
        public Guid CreateUserId { get; set; }
        public DateTime? UpdateTime { get; set; }
        public DateTime DateRelease { get; set; }
        public Guid? UpdateUserId { get; set; }
        public string? TargetsName { get; set; } = "";
    }
}
