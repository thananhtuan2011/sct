using API_SoCongThuong.Models;
using EF_Core.Models;
using Microsoft.EntityFrameworkCore;
namespace API_SoCongThuong.Reponsitories.InterCommerceRepository
{
    public class InterCommerceRepo
    {
        public SoHoa_SoCongThuongContext _context;
        public InterCommerceRepo(SoHoa_SoCongThuongContext context)
        {
            _context = context;
        }
        public async Task Insert(InternationalCommerce model)
        {
            await _context.InternationalCommerces.AddAsync(model);
            await _context.SaveChangesAsync();
        }
        public InternationalCommerce FindByInternationalCommerceId(Guid InternationalCommerceId)
        {
            var result = _context.InternationalCommerces.Where(x =>!x.IsDel &&  x.InternationalCommerceId == InternationalCommerceId).Select(d => new InternationalCommerce()
            {
                InternationalCommerceId = d.InternationalCommerceId,
                InternationalCommerceName = d.InternationalCommerceName,
                Address = d.Address,
                InvestorName = d.InvestorName,
                LicensingActivity = d.LicensingActivity,
                TenCoSoBanLe = d.TenCoSoBanLe,
                DiaChiCoSoBanLe = d.DiaChiCoSoBanLe,
                LoaiHinhCoSo = d.LoaiHinhCoSo,
                GiayPhepKinhDoanh = d.GiayPhepKinhDoanh,
                NgayCapGiayPhepKinhDoanh = d.NgayCapGiayPhepKinhDoanh,
                GiayPhepBanLe = d.GiayPhepBanLe,
                NgayCapGiayPhepBanLe = d.NgayCapGiayPhepBanLe,
                NgayHetHanGiayPhepBanLe = d.NgayHetHanGiayPhepBanLe,
                DienTichSuDung = d.DienTichSuDung,
                DienTichSan = d.DienTichSan,
                DienTichBanHang = d.DienTichBanHang,
                DienTichKinhDoanh = d.DienTichKinhDoanh,
                GhiChu = d.GhiChu
            }).FirstOrDefault();

            return result;
        }
        public async Task Update(InternationalCommerce model)
        {
            var db = await _context.InternationalCommerces.Where(d => d.InternationalCommerceId == model.InternationalCommerceId).FirstOrDefaultAsync();
            if(db != null)
            {
                db.InternationalCommerceId = model.InternationalCommerceId;
                db.InternationalCommerceName = model.InternationalCommerceName;
                db.Address = model.Address;
                db.InvestorName = model.InvestorName;
                db.LicensingActivity = model.LicensingActivity;
                db.TenCoSoBanLe = model.TenCoSoBanLe;
                db.DiaChiCoSoBanLe = model.DiaChiCoSoBanLe;
                db.LoaiHinhCoSo = model.LoaiHinhCoSo;
                db.GiayPhepKinhDoanh = model.GiayPhepKinhDoanh;
                db.NgayCapGiayPhepKinhDoanh = model.NgayCapGiayPhepKinhDoanh;
                db.GiayPhepBanLe = model.GiayPhepBanLe;
                db.NgayCapGiayPhepBanLe = model.NgayCapGiayPhepBanLe;
                db.NgayHetHanGiayPhepBanLe = model.NgayHetHanGiayPhepBanLe;
                db.DienTichSuDung = model.DienTichSuDung;
                db.DienTichSan = model.DienTichSan;
                db.DienTichBanHang = model.DienTichBanHang;
                db.DienTichKinhDoanh = model.DienTichKinhDoanh;
                db.GhiChu = model.GhiChu;
                db.UpdateTime = model.UpdateTime;
                db.UpdateUserId = model.UpdateUserId;
            }
            await _context.SaveChangesAsync();
        }

        public async Task Delete(InternationalCommerce model)
        {
            var db = await _context.InternationalCommerces.Where(d => d.InternationalCommerceId == model.InternationalCommerceId).FirstOrDefaultAsync();
            db.IsDel = model.IsDel;
            await _context.SaveChangesAsync();
        }
        public async Task DeleteById(Guid InternationalCommerceId)
        {
            var itemRemove = await _context.InternationalCommerces.Where(x => x.InternationalCommerceId == InternationalCommerceId).FirstOrDefaultAsync();
            _context.InternationalCommerces.Remove(itemRemove);
            await _context.SaveChangesAsync();
        }

        public List<InterCommerceModel> FindData(QueryRequestBody query)
        {
            List<InterCommerceModel> result = new List<InterCommerceModel>();
            var _data = from ic in _context.InternationalCommerces
                        where !ic.IsDel
                        join b in _context.Businesses on ic.InternationalCommerceName equals b.BusinessId
                        join c in _context.Categories on ic.LoaiHinhCoSo equals c.CategoryId into icc
                        from c in icc.DefaultIfEmpty()

                        select new InterCommerceModel()
                        {
                            InternationalCommerceName = ic.InternationalCommerceName,
                            InternationalCommerceId = ic.InternationalCommerceId,
                            Address = b.DiaChiTruSo ?? "",
                            InvestorName = ic.InvestorName ?? "",
                            LicensingActivity = ic.LicensingActivity ?? "",
                            IsDel = ic.IsDel,
                            PhoneNumber = b.SoDienThoai,
                            Representative = b.NguoiDaiDien,
                            BusinessNameVi = b.BusinessNameVi,
                            TenCoSoBanLe = ic.TenCoSoBanLe,
                            GiayPhepKinhDoanh = b.GiayPhepSanXuat,
                            NgayCapGiayPhepKinhDoanh = b.NgayCapPhep,
                            TenLoaiHinhCoSo = c.CategoryName,
                            DienTichSuDung = ic.DienTichSuDung,
                            DienTichKinhDoanh = ic.DienTichKinhDoanh,
                            DienTichBanHang = ic.DienTichBanHang,
                            DienTichSan = ic.DienTichSan,
                            GhiChu = ic.GhiChu,
                            GiayPhepBanLe = ic.GiayPhepBanLe,
                            NgayCapGiayPhepBanLe = ic.NgayCapGiayPhepBanLe
                        };

            //Search
            string _keywordSearch = "";
            if (query.SearchValue != null && query.SearchValue != "")
            {
                _keywordSearch = query.SearchValue.Trim().ToLower();
                _data = _data.Where(x => x.BusinessNameVi.ToLower().Contains(_keywordSearch)
                || x.InvestorName.ToLower().Contains(_keywordSearch));
            }

            result = _data.ToList();

            return result;
        }
    }
}

