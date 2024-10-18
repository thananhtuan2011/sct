namespace API_SoCongThuong.Models
{
    public class MultiLevelSalesParticipantsModel
    {
        /// Quản lý người tham gia bán hàng đa cấp
        public Guid MultiLevelSalesParticipantsId { get; set; }
        /// Mã số
        public string MultiLevelSalesParticipantsCode { get; set; } = null!;
        /// Tên đơn vị
        public string ParticipantsName { get; set; } = null!;
        /// Ngày sinh
        public DateTime Birthday { get; set; }
        public string BirthdayDisplay { get; set; } = "";
        /// Số điện thoại
        public string PhoneNumber { get; set; } = null!;
        /// Số CMND / CCCD
        public string IdentityCardNumber { get; set; } = null!;
        /// Ngày cấp CMND / CCCD
        public DateTime DateOfIssuance { get; set; }
        public string DateOfIssuanceDisplay { get; set; } = "";
        /// Nơi cấp CMND / CCCD
        public string PlaceOfIssue { get; set; } = null!;
        /// Giới tính: 1 - Nam, 2 - Nữ.
        public int Gender { get; set; }
        public string GenderDisplay { get; set; } = "";
        /// Ngày tham gia
        public DateTime JoinDate { get; set; }
        public string JoinDateDisplay { get; set; } = "";
        /// Tỉnh
        public string Province { get; set; } = null!;
        /// Địa chỉ cư trú
        public string Address { get; set; } = null!;

        public bool IsDel { get; set; }
    }
}