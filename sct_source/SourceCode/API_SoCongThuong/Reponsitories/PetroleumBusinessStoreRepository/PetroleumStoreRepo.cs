using EF_Core.Models;
using Microsoft.EntityFrameworkCore;

namespace API_SoCongThuong.Reponsitories.PetroleumBusinessStoreRepository
{
    public class PetroleumStoreRepo
    {
        public SoHoa_SoCongThuongContext _context;
        public PetroleumStoreRepo(SoHoa_SoCongThuongContext context)
        {
            _context = context;
        }
        public async Task Insert(PetroleumBusiness model)
        {
            await _context.PetroleumBusinesses.AddAsync(model);
            await _context.SaveChangesAsync();
        }
        public async Task InsertDetail(PetroleumBusinessStore model)
        {
            await _context.PetroleumBusinessStores.AddAsync(model);
            await _context.SaveChangesAsync();
        }
        public async Task InsertListDetail(List<PetroleumBusinessStore> model)
        {
            await _context.PetroleumBusinessStores.AddRangeAsync(model);
            await _context.SaveChangesAsync();
        }
        public async Task Update(PetroleumBusiness model)
        {
            var petroleum = await _context.PetroleumBusinesses.Where(d => d.PetroleumBusinessId == model.PetroleumBusinessId).FirstOrDefaultAsync();
            petroleum.PetroleumBusinessName = model.PetroleumBusinessName;
            petroleum.Address = model.Address;
            petroleum.PhoneNumber = model.PhoneNumber;
            petroleum.Supplier = model.Supplier;
            petroleum.Representative = model.Representative;
            await _context.SaveChangesAsync();
        }
        public async Task DeletePetroleumBu(PetroleumBusiness model)
        {
            var db = await _context.PetroleumBusinesses.Where(d => d.PetroleumBusinessId == model.PetroleumBusinessId).FirstOrDefaultAsync();
            db.IsDel = model.IsDel;
            await _context.SaveChangesAsync();
        }
        public List<PetroleumBusinessStore> getListPetroStore(Guid Id)
        {
           return _context.PetroleumBusinessStores.Where(x => x.PetroleumBusinessId == Id).ToList();
        }
        public async Task DeletePetroleumBusiness(PetroleumBusinessStore model)
        {
            _context.PetroleumBusinessStores.Remove(model);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteListStore(List<PetroleumBusinessStore> model)
        {
            _context.PetroleumBusinessStores.RemoveRange(model);
            await _context.SaveChangesAsync();
        }
        public async Task Delete(Guid PetroleumBusinessId)
        {
            var itemRemove = await _context.PetroleumBusinesses.Where(x => x.PetroleumBusinessId == PetroleumBusinessId).FirstOrDefaultAsync();
            _context.PetroleumBusinesses.Remove(itemRemove);
            await _context.SaveChangesAsync();
        }
        public PetroleumBusiness FindByPetroleumBusinessId(Guid PetroleumBusinessId)
        {
            var result = _context.PetroleumBusinesses.Where(x => x.PetroleumBusinessId == PetroleumBusinessId).Select(d => new PetroleumBusiness()
            {
                PetroleumBusinessId = d.PetroleumBusinessId,
                PetroleumBusinessName = d.PetroleumBusinessName,
                Address = d.Address,
                Supplier= d.Supplier,
                PhoneNumber = d.PhoneNumber,
                Representative = d.Representative,
                IsDel = d.IsDel
            }).FirstOrDefault();

            return result;
        }
        public IQueryable<PetroleumBusinessStore> FindStoreId(Guid Id)
        {
            var result = _context.PetroleumBusinessStores.Where(x => x.PetroleumBusinessId == Id).Select(d => new PetroleumBusinessStore()
            {
                PetroleumBusinessId = d.PetroleumBusinessId,
                TenCuaHang = d.TenCuaHang,
            });

            return result;
        }
        // lay danh sach doanh nghiep
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
            });
            return result;
        }
    }
}
