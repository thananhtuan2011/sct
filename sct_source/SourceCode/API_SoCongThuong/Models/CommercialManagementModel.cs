namespace API_SoCongThuong.Models
{
    public class CommercialManagementModel
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
        public bool IsDel { get; set; }
        public byte? TypeOfMarket { get; set; } /// 1: Chợ quy hoạch, 2: Chợ khác
        public byte? TypeOfCenterLogistic { get; set; } /// 1: Trung tâm Logistics cấp tính, 2:Trung tâm Logistic chuyên dùng hàng không
        public byte? FormMarket { get; set; } // 0: Không có , 1: Chợ được chuyển đổi mô hình quản lý, 2: Chợ xây mới, 3: Chợ nâng cấp cải tạo
        public byte? Form { get; set; } // 1: Chợ ngoài quy hoạch, 2: Chợ đệm, 3: Chợ nổi
        public byte? Area { get; set; } // 1: Thành thị 2: Nông thôn
        public byte? MarketCleared { get; set; } // 0: Không có , 1: Chợ đã giải toả, di dời, 2: Chợ có kế hoạch giải toả, di dời trong thời gian tới
    }
    public class removeListCommercialManagementItems
    {
        public List<Guid> CommercialManagementIds { get; set; }
    }

    public class CategoryModel
    {
        public Guid CategoryId { get; set; }
    }

    public class ViewCommercialManagementModel
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string DistrictName { get; set; }
        public string PhoneNumber { get; set; }
        public string LoaiHinh { get; set; } = "";
        public Guid Type { get; set; } = Guid.Empty;
        public Guid DistrictId { get; set; } = Guid.Empty;
    }
}