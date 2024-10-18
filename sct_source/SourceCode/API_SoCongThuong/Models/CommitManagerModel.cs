namespace API_SoCongThuong.Models
{
    public class CommitManagerModel
    {
        public Guid CommitManagerId { get; set; }
        public string MaHoSo { get; set; } = null!;
        public string TenThuTuc { get; set; } = null!;
        public string TenToChuc { get; set; } = null!;
        public string CoSo { get; set; } = null!;
        public string DiaChi { get; set; } = null!;
        public string? SoDienThoai { get; set; }
        public Guid Huyen { get; set; }
        public DateTime NgayNhanHoSo { get; set; }
        public DateTime NgayCamKet { get; set; }
        public string NguoiLamCamKet { get; set; } = null!;
        public string? GhiChu { get; set; }
        public bool? IsAction { get; set; }
        public bool IsDel { get; set; }
        public DateTime CreateTime { get; set; }
        public Guid CreateUserId { get; set; }
        public DateTime? UpdateTime { get; set; }
        public Guid? UpdateUserId { get; set; }
        public List<CommitManagerListitemModel> ListItems { get; set; } = new List<CommitManagerListitemModel>();
        public List<CommitManagerListitemModel> ListItemProduct { get; set; } = new List<CommitManagerListitemModel>();
        public List<CommitManagerListitemModel> ListItemBusiness { get; set; } = new List<CommitManagerListitemModel>();
        public string TenHuyen { get; set; } = "";

    }

    public class CommitManagerListitemModel 
    {
        public Guid Id { get; set; }
        public Guid CommitManagerId { get; set; } = Guid.Empty;
        public Guid LoaiHinh { get; set; } = Guid.Empty;
        public string TenMatHang { get; set; } = "";
        public string TenLoaiHinh { get; set; } = "";
        public bool? IsAction { get; set; }
        public bool IsDel { get; set; }
        public DateTime CreateTime { get; set; }
        public Guid CreateUserId { get; set; }
        public DateTime? UpdateTime { get; set; }
        public Guid? UpdateUserId { get; set; }

    }

}
