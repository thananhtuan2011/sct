using System;
using System.Collections.Generic;

namespace EF_Core.Models
{
    public partial class ManageConfirmPromotion
    {
        public Guid ManageConfirmPromotionId { get; set; }
        public string ManageConfirmPromotionName { get; set; } = null!;
        public string GoodsServices { get; set; } = null!;
        public string? GoodsServicesPay { get; set; }
        public DateTime TimeStart { get; set; }
        public DateTime TimeEnd { get; set; }
        public string NumberOfDocuments { get; set; } = null!;
        public bool? IsAction { get; set; }
        public bool IsDel { get; set; }
        public DateTime CreateTime { get; set; }
        public Guid CreateUserId { get; set; }
        public DateTime? UpdateTime { get; set; }
        public Guid? UpdateUserId { get; set; }
    }
}
