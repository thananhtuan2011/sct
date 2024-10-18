using EF_Core.Models;
using Microsoft.EntityFrameworkCore;
using API_SoCongThuong.Models;

namespace API_SoCongThuong.Reponsitories.AlcoholBusinessRepository
{
    public class AlcoholBusinessRepo
    {
        public SoHoa_SoCongThuongContext _context;
        public AlcoholBusinessRepo(SoHoa_SoCongThuongContext context)
        {
            _context = context;
        }
        public async Task Insert(AlcoholBusinessModel model)
        {
            AlcoholBusiness alcoBusinessData = new AlcoholBusiness
            {
                AlcoholBusinessName = model.AlcoholBusinessName,
                Supplier = model.Supplier,
                Address = model.Address,
                PhoneNumber = model.PhoneNumber,
                Representative = model.Representative,
                CreateUserId = model.CreateUserId,
                CreateTime = model.CreateTime
            };
            await _context.AlcoholBusinesses.AddAsync(alcoBusinessData);
            await _context.SaveChangesAsync();

            if (model.GiayPhepBanBuon != "")
            {
                var temp = _context.AlcoholWholesaleLicenses.Where(x => x.BusinessId == model.AlcoholBusinessName).FirstOrDefault();
                if (temp != null)
                {
                    temp.LicenseNumber = model.GiayPhepBanBuon;
                    temp.LicenseDate = model.NgayCapGiayPhepBanBuon;
                    temp.ExpirationDate = model.NgayHetHanGiayPhepBanBuon;
                    temp.UpdateUserId = model.CreateUserId;
                    temp.UpdateTime = model.CreateTime;
                }
                else
                {
                    AlcoholWholesaleLicense alcoholWholesaleLicense = new AlcoholWholesaleLicense
                    {
                        BusinessId = model.AlcoholBusinessName,
                        LicenseNumber = model.GiayPhepBanBuon,
                        LicenseDate = model.NgayCapGiayPhepBanBuon,
                        ExpirationDate = model.NgayHetHanGiayPhepBanBuon,
                        CreateUserId = model.CreateUserId,
                        CreateTime = model.CreateTime
                    };
                    await _context.AlcoholWholesaleLicenses.AddAsync(alcoholWholesaleLicense);
                }
                //await _context.SaveChangesAsync();
            }
            if (model.AlcoholBusinessDetail.Count() > 0)
            {
                List<AlcoholBussinessDetail> AlcoholBussinessDetailList = new List<AlcoholBussinessDetail>();
                foreach (var item in model.AlcoholBusinessDetail)
                {
                    AlcoholBussinessDetail detailItem = new AlcoholBussinessDetail()
                    {
                        AlcoholBusinessId = alcoBusinessData.AlcoholBusinessId,
                        TenDoanhNghiep = item.TenDoanhNghiep,
                        NguoiDaiDien = item.NguoiDaiDien,
                        SoDienThoai = item.SoDienThoai,
                        Huyen = item.Huyen,
                        Xa = item.Xa,
                        DiaChi = item.DiaChi,
                        GiayPhepKinhDoanh = item.GiayPhepKinhDoanh,
                        NgayHetHan = item.NgayHetHan,
                        DonViCungCap = item.DonViCungCap,
                        NgayCapGiayPhepBanLe = item.NgayCapGiayPhepBanLe,
                        DiaChiDonViCungCap = item.DiaChiDonViCungCap,
                        SoDienThoaiDonViCungCap = item.SoDienThoaiDonViCungCap,
                        GhiChu = item.GhiChu,
                        CreateUserId = model.CreateUserId,
                    };
                    AlcoholBussinessDetailList.Add(detailItem);
                }

                await _context.AlcoholBussinessDetails.AddRangeAsync(AlcoholBussinessDetailList);
                await _context.SaveChangesAsync();
            }
        }
        public AlcoholBusinessModel FindByAlcoholBusinessId(Guid AlcoholBusinessId)
        {
            var result = (from ab in _context.AlcoholBusinesses
                          where ab.AlcoholBusinessId == AlcoholBusinessId
                          join b in _context.Businesses on ab.AlcoholBusinessName equals b.BusinessId
                          join wl in _context.AlcoholWholesaleLicenses on ab.AlcoholBusinessName equals wl.BusinessId into abwl
                          from wl in abwl.DefaultIfEmpty()
                          select new AlcoholBusinessModel()
                          {
                              AlcoholBusinessId = ab.AlcoholBusinessId,
                              AlcoholBusinessName = ab.AlcoholBusinessName,
                              Address = b.DiaChiTruSo ?? "",
                              PhoneNumber = b.SoDienThoai ?? "",
                              Supplier = ab.Supplier,
                              Representative = b.NguoiDaiDien ?? "",
                              IsDel = ab.IsDel,
                              BusinessNameVi = b.BusinessNameVi,
                              GiayDangKyKinhDoanh = b.GiayPhepSanXuat,
                              NgayCapPhep = b.NgayCapPhep,
                              GiayPhepBanBuon = wl.LicenseNumber,
                              NgayCapGiayPhepBanBuon = wl.LicenseDate,
                              NgayHetHanGiayPhepBanBuon = wl.ExpirationDate
                          }).FirstOrDefault();

            Guid AlcoholBusinessName = result.AlcoholBusinessName;
            List<Guid> ListId = _context.AlcoholBusinesses.Where(x => !x.IsDel && x.AlcoholBusinessName == AlcoholBusinessName).Select(x => x.AlcoholBusinessId).ToList();
            var detail = (from abd in _context.AlcoholBussinessDetails
                          where ListId.Contains(abd.AlcoholBusinessId)
                          join d in _context.Districts on abd.Huyen equals d.DistrictId
                          join c in _context.Communes on abd.Xa equals c.CommuneId
                          select new AlcoholBusinessDetailModel()
                          {
                              TenDoanhNghiep = abd.TenDoanhNghiep,
                              NguoiDaiDien = abd.NguoiDaiDien,
                              SoDienThoai = abd.SoDienThoai,
                              Huyen = abd.Huyen,
                              TenHuyen = d.DistrictName,
                              Xa = abd.Xa,
                              TenXa = c.CommuneName,
                              DiaChi = abd.DiaChi,
                              GiayPhepKinhDoanh = abd.GiayPhepKinhDoanh,
                              NgayHetHan = abd.NgayHetHan,
                              DonViCungCap = abd.DonViCungCap,
                              DiaChiDonViCungCap = abd.DiaChiDonViCungCap,
                              SoDienThoaiDonViCungCap = abd.SoDienThoaiDonViCungCap,
                              NgayCapGiayPhepBanLe = abd.NgayCapGiayPhepBanLe,
                              GhiChu = abd.GhiChu
                          }).ToList();

            result.AlcoholBusinessDetail = detail;
            return result;
        }
        public async Task Update(AlcoholBusinessModel model)
        {
            var db = await _context.AlcoholBusinesses.Where(d => d.AlcoholBusinessId == model.AlcoholBusinessId).FirstOrDefaultAsync();
            db.AlcoholBusinessId = model.AlcoholBusinessId;
            db.AlcoholBusinessName = model.AlcoholBusinessName;
            db.Address = model.Address;
            db.PhoneNumber = model.PhoneNumber;
            db.Representative = model.Representative;
            db.Supplier = model.Supplier;
            db.UpdateUserId = model.UpdateUserId;
            db.UpdateTime = model.UpdateTime;
            await _context.SaveChangesAsync();
            if (model.GiayPhepBanBuon != "")
            {
                var temp = await _context.AlcoholWholesaleLicenses.Where(x => x.BusinessId == model.AlcoholBusinessName).FirstOrDefaultAsync();
                if (temp != null)
                {
                    temp.LicenseNumber = model.GiayPhepBanBuon;
                    temp.LicenseDate = model.NgayCapGiayPhepBanBuon;
                    temp.ExpirationDate = model.NgayHetHanGiayPhepBanBuon;
                    temp.UpdateUserId = model.UpdateUserId;
                    temp.UpdateTime = model.UpdateTime;
                    await _context.SaveChangesAsync();

                }
                else
                {
                    AlcoholWholesaleLicense alcoholWholesaleLicense = new AlcoholWholesaleLicense
                    {
                        BusinessId = model.AlcoholBusinessName,
                        LicenseNumber = model.GiayPhepBanBuon,
                        LicenseDate = model.NgayCapGiayPhepBanBuon,
                        ExpirationDate = model.NgayHetHanGiayPhepBanBuon,
                        CreateUserId = new Guid(model.UpdateUserId.ToString()),
                        CreateTime = new DateTime()
                    };
                    await _context.AlcoholWholesaleLicenses.AddAsync(alcoholWholesaleLicense);
                }
            }


            Guid AlcoholBusinessName = db.AlcoholBusinessName;
            List<Guid> ListId = _context.AlcoholBusinesses.Where(x => !x.IsDel && x.AlcoholBusinessName == AlcoholBusinessName).Select(x => x.AlcoholBusinessId).ToList();

            var listItemDetail = _context.AlcoholBussinessDetails.Where(x => ListId.Contains(x.AlcoholBusinessId)).ToList();
            _context.AlcoholBussinessDetails.RemoveRange(listItemDetail);
            await _context.SaveChangesAsync();
            if (model.AlcoholBusinessDetail.Count() > 0)
            {
                List<AlcoholBussinessDetail> AlcoholBussinessList = new List<AlcoholBussinessDetail>();
                foreach (var item in model.AlcoholBusinessDetail)
                {
                    AlcoholBussinessDetail detailItem = new AlcoholBussinessDetail()
                    {
                        AlcoholBusinessId = model.AlcoholBusinessId,
                        TenDoanhNghiep = item.TenDoanhNghiep,
                        NguoiDaiDien = item.NguoiDaiDien,
                        SoDienThoai = item.SoDienThoai,
                        Huyen = item.Huyen,
                        Xa = item.Xa,
                        DiaChi = item.DiaChi,
                        GiayPhepKinhDoanh = item.GiayPhepKinhDoanh,
                        NgayHetHan = item.NgayHetHan,
                        DonViCungCap = item.DonViCungCap,
                        NgayCapGiayPhepBanLe = item.NgayCapGiayPhepBanLe,
                        DiaChiDonViCungCap = item.DiaChiDonViCungCap,
                        SoDienThoaiDonViCungCap = item.SoDienThoaiDonViCungCap,
                        GhiChu = item.GhiChu
                    };
                    AlcoholBussinessList.Add(detailItem);
                }

                await _context.AlcoholBussinessDetails.AddRangeAsync(AlcoholBussinessList);
                await _context.SaveChangesAsync();
            }
        }

        public async Task Delete(AlcoholBusiness model)
        {
            var db = await _context.AlcoholBusinesses.Where(d => d.AlcoholBusinessId == model.AlcoholBusinessId).FirstOrDefaultAsync();
            db.IsDel = model.IsDel;
            await _context.SaveChangesAsync();
        }
        public async Task DeleteById(Guid AlcoholBusinessId)
        {
            Guid AlcoholBusinessName = _context.AlcoholBusinesses.Where(x => !x.IsDel && x.AlcoholBusinessId == AlcoholBusinessId).Select(x => x.AlcoholBusinessName).FirstOrDefault();
            List<Guid> listId = _context.AlcoholBusinesses.Where(x => !x.IsDel && x.AlcoholBusinessName == AlcoholBusinessName).Select(x => x.AlcoholBusinessId).ToList();
            var itemRemove = _context.AlcoholBusinesses.Where(x => listId.Contains(x.AlcoholBusinessId)).ToList();
            itemRemove.ForEach(item => item.IsDel = true);
            _context.UpdateRange(itemRemove);
            await _context.SaveChangesAsync();

            var listItemDetail = _context.AlcoholBussinessDetails.Where(x => listId.Contains(x.AlcoholBusinessId)).ToList();
            _context.AlcoholBussinessDetails.RemoveRange(listItemDetail);
            await _context.SaveChangesAsync();

        }
        public bool IsExistAlcoholBusiness(Guid id, Guid? AlcoholBusinessId)
        {
            if (AlcoholBusinessId != null)
            {
                var countItem = _context.AlcoholBusinesses.Where(x => !x.IsDel && x.AlcoholBusinessName == id && x.AlcoholBusinessId == AlcoholBusinessId).Count();
                if (countItem == 1)
                {
                    return false;
                }
            }
            var count = _context.AlcoholBusinesses.Where(x => !x.IsDel && x.AlcoholBusinessName == id).Count();
            if (count == 0)
                return false;
            return true;
        }

        public List<AlcoholBusinessModel> FindData(QueryRequestBody query)
        {
            List<AlcoholBusinessModel> result = new List<AlcoholBusinessModel>();
            IQueryable<AlcoholBusinessModel> _data = _context.AlcoholBusinesses.Where(x => !x.IsDel).Join(
                _context.Businesses,
                cc => cc.AlcoholBusinessName, cd => cd.BusinessId,
                 (cc, cd) => new AlcoholBusinessModel
                 {
                     AlcoholBusinessName = cc.AlcoholBusinessName,
                     AlcoholBusinessId = cc.AlcoholBusinessId,
                     Address = cc.Address ?? "",
                     PhoneNumber = cd.SoDienThoai ?? "",
                     Supplier = cc.Supplier,
                     Representative = cd.NguoiDaiDien ?? "",
                     IsDel = cc.IsDel,
                     BusinessNameVi = cd.BusinessNameVi,
                 });

            //Search
            string _keywordSearch = "";
            if (query.SearchValue != null && query.SearchValue != "")
            {
                _keywordSearch = query.SearchValue.Trim().ToLower();
                _data = _data.Where(x => x.BusinessNameVi.ToLower().Contains(_keywordSearch)
                || x.Representative.ToLower().Contains(_keywordSearch));
            }

            result = _data.ToList();

            return result;
        }
    }
}
