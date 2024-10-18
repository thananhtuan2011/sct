namespace API_SoCongThuong.Models
{
    public class ElectricityInspectorCardModel
    {
        /// Danh sách thẻ kiểm tra viên điện lực
        public Guid ElectricityInspectorCardId { get; set; }
        /// Tên Kiểm tra viên
        public string InspectorName { get; set; } = null!;
        /// Ngày sinh
        public string Birthday { get; set; }
        public DateTime? BirthdayDate { get; set; }
        /// Ngày cấp thẻ
        public string LicenseDate { get; set; }
        public DateTime? LicenseDateDate { get; set; }
        /// Trình độ
        public string Degree { get; set; } = null!;
        /// Đơn vị
        public string Unit { get; set; } = null!;
        /// Thâm niên
        public string Seniority { get; set; } = null!;
        /// Màu thẻ: 0 - Cam, 1 - Vàng, 2 - Hồng
        public Guid CardColor { get; set; }
        public string CardColorName { get; set; } = "";
        public bool IsDel { get; set; }
        public DateTime CreateTime { get; set; }
        public Guid CreateUserId { get; set; }
        public DateTime? UpdateTime { get; set; }
        public Guid? UpdateUserId { get; set; }
    }
}