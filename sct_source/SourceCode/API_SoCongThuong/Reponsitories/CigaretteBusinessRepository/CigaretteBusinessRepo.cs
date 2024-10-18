using EF_Core.Models;
using Microsoft.EntityFrameworkCore;
using API_SoCongThuong.Models;

namespace API_SoCongThuong.Reponsitories.CigaretteBusinessRepository
{
    public class CigaretteBusinessRepo
    {
        public SoHoa_SoCongThuongContext _context;
        public CigaretteBusinessRepo(SoHoa_SoCongThuongContext context)
        {
            _context = context;
        }
        public async Task Insert(CigaretteBusinessModel model)
        {
            CigaretteBusiness cigaBusinessData = new CigaretteBusiness
            {
                //insert thong tin moi
                CigaretteBusinessName = model.CigaretteBusinessName,
                Supplier = model.Supplier,
                Address = model.Address,
                PhoneNumber = model.PhoneNumber,
                Representative = model.Representative,
                CreateUserId = model.CreateUserId,
                CreateTime = model.CreateTime
            };
            await _context.CigaretteBusinesses.AddAsync(cigaBusinessData);
            await _context.SaveChangesAsync();
            if (model.CigaretteBusinessDetail.Count() > 0)
            {
                List<CigaretteBusinessStore> CigaretteBusinessStoreList = new List<CigaretteBusinessStore>();
                foreach (var item in model.CigaretteBusinessDetail)
                {
                    CigaretteBusinessStore detailItem = new CigaretteBusinessStore()
                    {
                        CigaretteBusinessId = cigaBusinessData.CigaretteBusinessId,
                        TenDoanhNghiep = item.TenDoanhNghiep,
                        NguoiDaiDien = item.NguoiDaiDien,
                        SoDienThoai = item.SoDienThoai,
                        Huyen = item.Huyen,
                        Xa = item.Xa,
                        DiaChi = item.DiaChi,
                        GiayPhepKinhDoanh = item.GiayPhepKinhDoanh,
                        NgayHetHan = item.NgayHetHan,
                        DonViCungCap = item.DonViCungCap,
                        NgayCap = item.NgayCap,
                        PhoneDonViCungCap = item.PhoneDonViCungCap,
                        DiaChiDonViCungCap = item.DiaChiDonViCungCap,
                        GhiChu = item.GhiChu,
                    };
                    CigaretteBusinessStoreList.Add(detailItem);
                }

                await _context.CigaretteBusinessStores.AddRangeAsync(CigaretteBusinessStoreList);
                await _context.SaveChangesAsync();
            }
        }

        public CigaretteBusinessModel FindByCigaretteBusinessId(Guid CigaretteBusinessId)
        {
            var result = _context.CigaretteBusinesses.Where(x => x.CigaretteBusinessId == CigaretteBusinessId)
                .Join(_context.Businesses, x => x.CigaretteBusinessName, cd => cd.BusinessId , (x, cd) => new CigaretteBusinessModel
                {
                    CigaretteBusinessId = x.CigaretteBusinessId,
                    CigaretteBusinessName = x.CigaretteBusinessName,
                    Address = cd.DiaChiTruSo ?? "",
                    PhoneNumber = cd.SoDienThoai ?? "",
                    Supplier = x.Supplier,
                    Representative = cd.NguoiDaiDien ?? "",
                    IsDel = x.IsDel,
                    BusinessNameVi = cd.BusinessNameVi,
                    GiayDangKyKinhDoanh = cd.GiayPhepSanXuat,
                    NgayCap = cd.NgayCapPhep
                }).FirstOrDefault();
            Guid CigaretteBusinessName = result.CigaretteBusinessName;
            List<Guid> ListId = _context.CigaretteBusinesses.Where(x => !x.IsDel && x.CigaretteBusinessName == CigaretteBusinessName).Select(x => x.CigaretteBusinessId).ToList();
            var detail = _context.CigaretteBusinessStores.Where(x => ListId.Contains(x.CigaretteBusinessId))
                .Select(x => new CigaretteBusinessDetailModel()
            {
                TenDoanhNghiep = x.TenDoanhNghiep,
                NguoiDaiDien = x.NguoiDaiDien,
                SoDienThoai = x.SoDienThoai,
                Huyen = x.Huyen,
                TenHuyen = _context.Districts.Where(d => !d.IsDel && d.DistrictId == x.Huyen).Select(d=> d.DistrictName).FirstOrDefault(),
                Xa = x.Xa,
                TenXa = _context.Communes.Where(d => !d.IsDel && d.CommuneId == x.Xa).Select(d => d.CommuneName).FirstOrDefault(),
                DiaChi = x.DiaChi,
                GiayPhepKinhDoanh = x.GiayPhepKinhDoanh,
                NgayHetHan = x.NgayHetHan,
                DonViCungCap = x.DonViCungCap,
                NgayCap = x.NgayCap,
                DiaChiDonViCungCap = x.DiaChiDonViCungCap,
                PhoneDonViCungCap = x.PhoneDonViCungCap,
                GhiChu = x.GhiChu
            }).ToList();
            result.CigaretteBusinessDetail = detail;
            return result;
        }
        public async Task Update(CigaretteBusinessModel model)
        {
            var db = await _context.CigaretteBusinesses.Where(d => d.CigaretteBusinessId == model.CigaretteBusinessId).FirstOrDefaultAsync();
            db.CigaretteBusinessId = model.CigaretteBusinessId;
            db.CigaretteBusinessName = model.CigaretteBusinessName;
            db.Address = model.Address;
            db.PhoneNumber = model.PhoneNumber;
            db.Representative = model.Representative;
            db.Supplier = model.Supplier;
            db.UpdateUserId = model.UpdateUserId;
            db.UpdateTime = model.UpdateTime;
            await _context.SaveChangesAsync();

            Guid CigaretteBusinessName = db.CigaretteBusinessName;
            List<Guid> ListId = _context.CigaretteBusinesses.Where(x => !x.IsDel && x.CigaretteBusinessName == CigaretteBusinessName).Select(x => x.CigaretteBusinessId).ToList();

            var listItemDetail = _context.CigaretteBusinessStores.Where(x => ListId.Contains(x.CigaretteBusinessId)).ToList();
            _context.CigaretteBusinessStores.RemoveRange(listItemDetail);
            await _context.SaveChangesAsync();
            if (model.CigaretteBusinessDetail.Count() > 0)
            {
                List<CigaretteBusinessStore> CigaretteBusinessStoreList = new List<CigaretteBusinessStore>();
                foreach (var item in model.CigaretteBusinessDetail)
                {
                    CigaretteBusinessStore detailItem = new CigaretteBusinessStore()
                    {
                        CigaretteBusinessId = model.CigaretteBusinessId,
                        TenDoanhNghiep = item.TenDoanhNghiep,
                        NguoiDaiDien = item.NguoiDaiDien,
                        SoDienThoai = item.SoDienThoai,
                        Huyen = item.Huyen,
                        Xa = item.Xa,
                        DiaChi = item.DiaChi,
                        GiayPhepKinhDoanh = item.GiayPhepKinhDoanh,
                        NgayHetHan = item.NgayHetHan,
                        DonViCungCap = item.DonViCungCap,
                        NgayCap = item.NgayCap,
                        PhoneDonViCungCap = item.PhoneDonViCungCap,
                        DiaChiDonViCungCap = item.DiaChiDonViCungCap,
                        GhiChu = item.GhiChu,
                    };
                    CigaretteBusinessStoreList.Add(detailItem);
                }

                await _context.CigaretteBusinessStores.AddRangeAsync(CigaretteBusinessStoreList);
                await _context.SaveChangesAsync();
            }
        }
        public async Task Delete(CigaretteBusiness model)
        {
            var db = await _context.CigaretteBusinesses.Where(d => d.CigaretteBusinessId == model.CigaretteBusinessId).FirstOrDefaultAsync();
            db.IsDel = model.IsDel;
            await _context.SaveChangesAsync();
        }
        public async Task DeleteById(Guid CigaretteBusinessId)
        {
            Guid CigaretteBusinessName = _context.CigaretteBusinesses.Where(x => !x.IsDel && x.CigaretteBusinessId == CigaretteBusinessId).Select(x => x.CigaretteBusinessName).FirstOrDefault();
            List<Guid> listId = _context.CigaretteBusinesses.Where(x => !x.IsDel && x.CigaretteBusinessName == CigaretteBusinessName).Select(x => x.CigaretteBusinessId).ToList();
            var itemRemove = _context.CigaretteBusinesses.Where(x => listId.Contains(x.CigaretteBusinessId)).ToList();
            itemRemove.ForEach(item => item.IsDel = true);
            _context.UpdateRange(itemRemove);
            await _context.SaveChangesAsync();

            var listItemDetail = _context.CigaretteBusinessStores.Where(x => listId.Contains(x.CigaretteBusinessId)).ToList();
            _context.CigaretteBusinessStores.RemoveRange(listItemDetail);
            await _context.SaveChangesAsync();
        }

        public bool IsExistCigaretteBusiness(Guid id, Guid? CigaretteBusinessId)
        {
            if(CigaretteBusinessId != null)
            {
                var countItem = _context.CigaretteBusinesses.Where(x => !x.IsDel && x.CigaretteBusinessName == id && x.CigaretteBusinessId == CigaretteBusinessId).Count();
                if(countItem == 1)
                {
                    return false;
                }
            }
            var count = _context.CigaretteBusinesses.Where(x => !x.IsDel && x.CigaretteBusinessName == id).Count();
            if ( count == 0)
                return false;
            return true;
        }

        public List<CigaretteBusinessModel> FindData(QueryRequestBody query)
        {
            List<CigaretteBusinessModel> result = new List<CigaretteBusinessModel>();
            IQueryable<CigaretteBusinessModel> _data = _context.CigaretteBusinesses.Where(x => !x.IsDel).Join(
                _context.Businesses,
                cc => cc.CigaretteBusinessName, cd => cd.BusinessId,
                 (cc, cd) => new CigaretteBusinessModel
                 {
                     CigaretteBusinessId = cc.CigaretteBusinessId,
                     CigaretteBusinessName = cc.CigaretteBusinessName,
                     Address = cc.Address ?? "",
                     PhoneNumber = cd.SoDienThoai ?? "",
                     Supplier = cc.Supplier,
                     Representative = cd.NguoiDaiDien ?? "",
                     IsDel = cc.IsDel,
                     BusinessNameVi = cd.BusinessNameVi,
                 });

            //Search
            string _keywordSearch = "";
            if (query.SearchValue != null && query.SearchValue != "") //Kiểm tra điều kiện tìm kiếm
            {
                _keywordSearch = query.SearchValue.Trim().ToLower();
                _data = _data.Where(x => x.BusinessNameVi.ToLower().Contains(_keywordSearch) || x.Representative.ToLower().Contains(_keywordSearch));  //Lấy table đã select tìm kiếm theo keyword
            }

            result = _data.ToList();

            return result;
        }
    }
}
