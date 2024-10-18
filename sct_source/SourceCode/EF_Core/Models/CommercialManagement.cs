using System;
using System.Collections.Generic;

namespace EF_Core.Models
{
    public partial class CommercialManagement
    {
        public Guid CommercialId { get; set; }
        public Guid Type { get; set; }
        public string Code { get; set; } = null!;
        public string Name { get; set; } = null!;
        public Guid? TypeOfMarketId { get; set; }
        public Guid DistrictId { get; set; }
        public Guid CommuneId { get; set; }
        public string Address { get; set; } = null!;
        public Guid? RankId { get; set; }
        public Guid? ConstructiveNatureId { get; set; }
        public Guid? BusinessNatureId { get; set; }
        public Guid? TypeOfEconomic { get; set; }
        public Guid? ManagementFormId { get; set; }
        public Guid? ManagementObjectId { get; set; }
        public string PhoneNumber { get; set; } = null!;
        public string? Email { get; set; }
        public string? Fax { get; set; }
        public string? Note { get; set; }
        public byte? TypeOfMarket { get; set; }
        public byte? TypeOfCenterLogistic { get; set; }
        public byte? FormMarket { get; set; }
        public byte? Form { get; set; }
        public byte? Area { get; set; }
        public byte? MarketCleared { get; set; }
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
