using API_SoCongThuong.Models;
using EF_Core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API_SoCongThuong.Reponsitories.BusinessRepository
{
    public class BusinessRepo
    {
        public SoHoa_SoCongThuongContext _context;
        public BusinessRepo(SoHoa_SoCongThuongContext context)
        {
            _context = context;
        }
        public async Task Insert(Business model)
        {
            await _context.Businesses.AddAsync(model);
            await _context.SaveChangesAsync();
        }

        public async Task InsertLogo(List<BusinessLogo> model)
        {
            if (model != null && model.Count() > 0)
            {
                await _context.BusinessLogos.AddRangeAsync(model);
                await _context.SaveChangesAsync();
            }
        }

        public async Task Update(Business model)
        {
            _context.Businesses.Update(model);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteBusiness(Business model)
        {
            var db = await _context.Businesses.Where(d => d.BusinessId == model.BusinessId).FirstOrDefaultAsync();
            db.IsDel = model.IsDel;
            await _context.SaveChangesAsync();
        }
        public async Task Delete(Guid id)
        {
            var itemRemove = await _context.Businesses.Where(x => x.BusinessId == id).FirstOrDefaultAsync();
            _context.Businesses.Remove(itemRemove);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteLogo(Guid id)
        {
            List<BusinessLogo> itemRemove = await _context.BusinessLogos.Where(x => x.BusinessId == id).ToListAsync();
            _context.BusinessLogos.RemoveRange(itemRemove);
            await _context.SaveChangesAsync();
        }

        public IQueryable<Business> FindAll()
        {
            var result = _context.Businesses.Select(d => new Business()
            {
                BusinessId = d.BusinessId,
                BusinessCode = d.BusinessCode,
                TenGiaoDich = d.TenGiaoDich,
                BusinessNameEn = d.BusinessNameEn,
                DiaChiTruSo = d.DiaChiTruSo,
                NgayCapPhep = d.NgayCapPhep,
                MaSoThue = d.MaSoThue,
                BusinessNameVi = d.BusinessNameVi,
                LoaiHinhDoanhNghiep = d.LoaiHinhDoanhNghiep,
                LoaiNganhNghe = d.LoaiNganhNghe,
                NgayHoatDong = d.NgayHoatDong,
                NguoiDaiDien = d.NguoiDaiDien,
                SoDienThoai = d.SoDienThoai,
                DiaChi = d.DiaChi,
                GiamDoc = d.GiamDoc,
                Email = d.Email,
                IsDel = d.IsDel,
                GiayPhepSanXuat = d.GiayPhepSanXuat
            });

            return result;
        }

        public IQueryable<Business> FindById(Guid Id)
        {
            var result = _context.Businesses.Where(x => x.BusinessId == Id).Select(d => new Business()
            {
                BusinessId = d.BusinessId,
                BusinessCode = d.BusinessCode,
                TenGiaoDich = d.TenGiaoDich,
                BusinessNameEn = d.BusinessNameEn,
                DistrictId = d.DistrictId,
                CommuneId = d.CommuneId,
                DiaChiTruSo = d.DiaChiTruSo,
                NgayCapPhep = d.NgayCapPhep,
                MaSoThue = d.MaSoThue,
                BusinessNameVi = d.BusinessNameVi,
                LoaiHinhDoanhNghiep = d.LoaiHinhDoanhNghiep,
                LoaiNganhNghe = d.LoaiNganhNghe,
                NgayHoatDong = d.NgayHoatDong,
                NguoiDaiDien = d.NguoiDaiDien,
                SoDienThoai = d.SoDienThoai,
                NgaySinh = d.NgaySinh,
                Cccd = d.Cccd,
                NgayCap = d.NgayCap,
                NoiCap = d.NoiCap,
                DiaChi = d.DiaChi,
                GiamDoc = d.GiamDoc,
                Email = d.Email,
                IsDel = d.IsDel,
                GiayPhepSanXuat = d.GiayPhepSanXuat
            });

            return result;
        }

        public bool findByBusinessCode(string businessCode, Guid? businessId)
        {
            if (businessId != null)
            {
                var BusinessCode = _context.Businesses.Where(x => x.BusinessId == businessId && x.BusinessCode == businessCode && !x.IsDel).FirstOrDefault();
                if (BusinessCode != null)
                {
                    return false;
                }
            }
            var isBusinessCode = _context.Businesses.Where(x => x.BusinessCode == businessCode && !x.IsDel).FirstOrDefault();
            if (isBusinessCode == null)
            {
                return false;
            }
            return true;
        }

        public List<BusinessModel> FindData(QueryRequestBody query)
        {
            List<BusinessModel> result = new List<BusinessModel>();

            IQueryable<BusinessModel> _data = _context.Businesses.Where(x => !x.IsDel).Select(x => new BusinessModel
            {
                BusinessId = x.BusinessId,
                BusinessCode = x.BusinessCode ?? "",
                TenGiaoDich = x.TenGiaoDich ?? "",
                BusinessNameEn = x.BusinessNameEn ?? "",
                DiaChiTruSo = x.DiaChiTruSo ?? "",
                NgayCapPhep = x.NgayCapPhep,
                MaSoThue = x.MaSoThue ?? "",
                BusinessNameVi = x.BusinessNameVi ?? "",
                LoaiHinhDoanhNghiep = x.LoaiHinhDoanhNghiep,
                LoaiNganhNghe = x.LoaiNganhNghe ?? Guid.Empty,
                NgayHoatDong = x.NgayHoatDong,
                NguoiDaiDien = x.NguoiDaiDien ?? "",
                SoDienThoai = x.SoDienThoai ?? "",
                DiaChi = x.DiaChi ?? "",
                GiamDoc = x.GiamDoc ?? "",
                Email = x.Email ?? "",
                IsDel = x.IsDel,
                GiayPhepSanXuat = x.GiayPhepSanXuat
            });

            //Search
            string _keywordSearch = "";
            if (query.SearchValue != null && query.SearchValue != "")
            {
                _keywordSearch = query.SearchValue.Trim().ToLower();
                _data = _data.Where(x =>
                   x.BusinessNameVi.ToLower().Contains(_keywordSearch)
                   || x.BusinessCode.ToLower().Contains(_keywordSearch)
               );
            }

            //Order
            bool _orderBy_ASC = true;
            Func<BusinessModel, object> _orderByExpression = x => x.BusinessCode;

            Dictionary<string, Func<BusinessModel, object>> _sortableFields = new Dictionary<string, Func<BusinessModel, object>>
                {
                    { "BusinessCode", x => x.BusinessCode },
                    { "TenGiaoDich", x => x.TenGiaoDich },
                    { "BusinessNameEn", x => x.BusinessNameEn },
                    { "DiaChiTruSo", x => x.DiaChiTruSo },
                    { "NgayCapPhep", x => x.NgayCapPhep },
                    { "MaSoThue", x => x.MaSoThue },
                    { "BusinessNameVi", x => x.BusinessNameVi },
                    { "LoaiHinhDoanhNghiep", x => x.LoaiHinhDoanhNghiep },
                    { "LoaiNganhNghe", x => x.LoaiNganhNghe },
                    { "NgayHoatDong", x => x.NgayHoatDong },
                    { "NguoiDaiDien", x => x.NguoiDaiDien },
                    { "SoDienThoai", x => x.SoDienThoai },
                    { "DiaChi", x => x.DiaChi },
                    { "GiamDoc", x => x.GiamDoc },
                    { "Email", x => x.Email },
                };

            if (query.Sort != null && !string.IsNullOrEmpty(query.Sort.ColumnName) && _sortableFields.ContainsKey(query.Sort.ColumnName))
            {
                _orderBy_ASC = ("desc".Equals(query.Sort.Direction.ToLower()) ? false : true);
                _orderByExpression = _sortableFields[query.Sort.ColumnName];
            }

            if (_orderBy_ASC)
            {
                result = _data
                    .OrderBy(_orderByExpression)
                    .ToList();
            }
            else
            {
                result = _data
                    .OrderByDescending(_orderByExpression)
                    .ToList();
            }

            return result;
        }
    }
}
