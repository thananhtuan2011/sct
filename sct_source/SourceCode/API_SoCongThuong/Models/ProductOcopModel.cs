namespace API_SoCongThuong.Models
{
    public partial class ProductOcopModel
    {
        public Guid ProductOcopid { get; set; }
        public string ProductName { get; set; } = null!;
        public string ProductOwner { get; set; } = null!;
        public string? PhoneNumber { get; set; }
        public string Address { get; set; } = null!;
        /// <summary>
        /// Thành phần
        /// </summary>
        public string Ingredients { get; set; } = null!;
        /// <summary>
        /// Hạn sử dụng
        /// </summary>
        public int Expiry { get; set; }
        /// <summary>
        /// Bảo Quản
        /// </summary>
        public string Preserve { get; set; } = null!;
        /// <summary>
        /// Quyết định phê duyệt
        /// </summary>
        public string? ApprovalDecision { get; set; }
        public int Ratings { get; set; }
        public Guid DistrictId { get; set; }
        public string? DistrictName { get; set; }
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
        public List<ProductOcopAttachFileModel> Details { get; set; } = new List<ProductOcopAttachFileModel>();
        public string IdFiles { get; set; } = "";
        public string LinkFileDisplay { get; set; } = "";
    }
    public partial class ProductOcopAttachFileModel
    {
        public Guid ProductOcopattachFileId { get; set; }
        public Guid ProductOcopid { get; set; }
        public string LinkFile { get; set; } = null!;
        public int Type { get; set; } = 0;
    }
}