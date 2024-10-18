using API_SoCongThuong.Models;
using DocumentFormat.OpenXml.Office2010.Excel;
using EF_Core.Models;
using Microsoft.EntityFrameworkCore;

namespace API_SoCongThuong.Reponsitories.PetroleumBusinessRepository
{
    public class PetroleumRepo
    {
        public SoHoa_SoCongThuongContext _context;
        public PetroleumRepo(SoHoa_SoCongThuongContext context)
        {
            _context = context;
        }
        public async Task Insert(PetroleumBusinessModel model)
        {
            PetroleumBusiness petroBusinessData = new PetroleumBusiness
            {
                PetroleumBusinessName = model.PetroleumBusinessName,
                Supplier = model.Supplier,
                Address = model.Address,
                PhoneNumber = model.PhoneNumber,
                Representative = model.Representative,
                CreateUserId = model.CreateUserId,
                CreateTime = model.CreateTime
            };
            await _context.PetroleumBusinesses.AddAsync(petroBusinessData);
            await _context.SaveChangesAsync();
            if (model.PetroleumBusinessDetail.Count() > 0)
            {
                List<PetroleumBusinessStore> PetroleumBussinessDetailList = new List<PetroleumBusinessStore>();
                foreach (var item in model.PetroleumBusinessDetail)
                {
                    PetroleumBusinessStore detailItem = new PetroleumBusinessStore()
                    {
                        PetroleumBusinessId = petroBusinessData.PetroleumBusinessId,
                        TenCuaHang = item.TenCuaHang,
                        NguoiDaiDien = item.NguoiDaiDien,
                        SoDienThoai = item.SoDienThoai,
                        Huyen = item.Huyen,
                        Xa = item.Xa,
                        DiaChi = item.DiaChi,
                        GiayPhepKinhDoanh = item.GiayPhepKinhDoanh,
                        NgayHetHan = item.NgayHetHan,
                        DonViCungCap = item.DonViCungCap,
                        ThoiHan5Nam = item.ThoiHan5Nam,
                        NguoiQuanLy = item.NguoiQuanLy,
                        NgayCapPhep = item.NgayCapPhep,
                        SoBeChua = item.SoBeChua,
                        SoCotBomOil = item.SoCotBomOil,
                        SoCotBomE5 = item.SoCotBomE5,
                        SoCotBomA95 = item.SoCotBomA95,
                        DienTichXayDung = item.DienTichXayDung,
                        HinhThuc = item.HinhThuc,
                        ThoiGianBanHang = item.ThoiGianBanHang,
                        TongDungTich = item.TongDungTich,
                        TuyenPhucVu = item.TuyenPhucVu,
                        ThoiHan1Nam = item.ThoiHan1Nam,
                        LoaiGiayXacNhan = item.LoaiGiayXacNhan,
                        NguoiLienHeDonViCungCap = item.NguoiLienHeDonViCungCap,
                        SoDienThoaiDonViCungCap = item.SoDienThoaiDonViCungCap,
                        DiaChiDonViCungCap = item.DiaChiDonViCungCap,
                        HinhThucHopDong = item.HinhThucHopDong,
                        GhiChu = item.GhiChu,
                        NgayCapPhepXayDung = item.NgayCapPhepXayDung
                    };
                    PetroleumBussinessDetailList.Add(detailItem);
                }

                await _context.PetroleumBusinessStores.AddRangeAsync(PetroleumBussinessDetailList);
                await _context.SaveChangesAsync();
            }
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
        public async Task Update(PetroleumBusinessModel model)
        {
            var db = await _context.PetroleumBusinesses.Where(d => d.PetroleumBusinessId == model.PetroleumBusinessId).FirstOrDefaultAsync();
            db.PetroleumBusinessId = model.PetroleumBusinessId;
            db.PetroleumBusinessName = model.PetroleumBusinessName;
            db.Address = model.Address;
            db.PhoneNumber = model.PhoneNumber;
            db.Representative = model.Representative;
            db.Supplier = model.Supplier;
            db.UpdateUserId = model.UpdateUserId;
            db.UpdateTime = model.UpdateTime;
            await _context.SaveChangesAsync();

            Guid PetroleumBusinessName = db.PetroleumBusinessName;
            List<Guid> ListId = _context.PetroleumBusinesses.Where(x => x.PetroleumBusinessName == PetroleumBusinessName).Select(x => x.PetroleumBusinessId).ToList();

            var listItemDetail = _context.PetroleumBusinessStores.Where(x => ListId.Contains(x.PetroleumBusinessId)).ToList();
            _context.PetroleumBusinessStores.RemoveRange(listItemDetail);
            await _context.SaveChangesAsync();

            if (model.PetroleumBusinessDetail.Count() > 0)
            {
                List<PetroleumBusinessStore> PetroleumBussinessList = new List<PetroleumBusinessStore>();
                foreach (var item in model.PetroleumBusinessDetail)
                {
                    PetroleumBusinessStore detailItem = new PetroleumBusinessStore()
                    {
                        PetroleumBusinessId = model.PetroleumBusinessId,
                        TenCuaHang = item.TenCuaHang,
                        NguoiDaiDien = item.NguoiDaiDien,
                        SoDienThoai = item.SoDienThoai,
                        Huyen = item.Huyen,
                        Xa = item.Xa,
                        DiaChi = item.DiaChi,
                        GiayPhepKinhDoanh = item.GiayPhepKinhDoanh,
                        NgayHetHan = item.NgayHetHan,
                        DonViCungCap = item.DonViCungCap,
                        ThoiHan5Nam = item.ThoiHan5Nam,
                        NguoiQuanLy = item.NguoiQuanLy,
                        NgayCapPhep = item.NgayCapPhep,
                        SoBeChua = item.SoBeChua,
                        SoCotBomOil = item.SoCotBomOil,
                        SoCotBomE5 = item.SoCotBomE5,
                        SoCotBomA95 = item.SoCotBomA95,
                        DienTichXayDung = item.DienTichXayDung,
                        HinhThuc = item.HinhThuc,
                        ThoiGianBanHang = item.ThoiGianBanHang,
                        TongDungTich = item.TongDungTich,
                        TuyenPhucVu = item.TuyenPhucVu,
                        ThoiHan1Nam = item.ThoiHan1Nam,
                        LoaiGiayXacNhan = item.LoaiGiayXacNhan,
                        NguoiLienHeDonViCungCap = item.NguoiLienHeDonViCungCap,
                        SoDienThoaiDonViCungCap =  item.SoDienThoaiDonViCungCap,
                        DiaChiDonViCungCap = item.DiaChiDonViCungCap,
                        HinhThucHopDong = item.HinhThucHopDong,
                        GhiChu = item.GhiChu,
                        NgayCapPhepXayDung = item.NgayCapPhepXayDung
                    };
                    PetroleumBussinessList.Add(detailItem);
                }

                await _context.PetroleumBusinessStores.AddRangeAsync(PetroleumBussinessList);
                await _context.SaveChangesAsync();
            }
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
        public async Task DeleteById(Guid PetroleumBusinessId)
        {
            Guid PetroleumBusinessName = _context.PetroleumBusinesses.Where(x => !x.IsDel && x.PetroleumBusinessId == PetroleumBusinessId).Select(x => x.PetroleumBusinessName).FirstOrDefault();
            List<Guid> listId = _context.PetroleumBusinesses.Where(x => !x.IsDel && x.PetroleumBusinessName == PetroleumBusinessName).Select(x => x.PetroleumBusinessId).ToList();
            var itemRemove =  _context.PetroleumBusinesses.Where(x => listId.Contains(x.PetroleumBusinessId)).ToList();
            itemRemove.ForEach(item => item.IsDel = true);
            _context.UpdateRange(itemRemove);
            await _context.SaveChangesAsync();
            
            var listItemDetail = _context.PetroleumBusinessStores.Where(x => listId.Contains(x.PetroleumBusinessId)).ToList();
            _context.PetroleumBusinessStores.RemoveRange(listItemDetail);
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
        public PetroleumBusinessModel FindByPetroleumBusinessId(Guid PetroleumBusinessId)
        {
            var result = _context.PetroleumBusinesses.Where(x => x.PetroleumBusinessId == PetroleumBusinessId)
                .Join(_context.Businesses, x => x.PetroleumBusinessName, cd => cd.BusinessId, (x, cd) => new PetroleumBusinessModel
                {
                    PetroleumBusinessId = x.PetroleumBusinessId,
                    PetroleumBusinessName = x.PetroleumBusinessName,
                    Address = cd.DiaChiTruSo ?? "",
                    PhoneNumber = cd.SoDienThoai ?? "",
                    Supplier = x.Supplier,
                    Representative = cd.NguoiDaiDien ?? "",
                    IsDel = x.IsDel,
                    BusinessNameVi = cd.BusinessNameVi,
                    GiayDangKyKinhDoanh = cd.GiayPhepSanXuat,
                    NgayCap = cd.NgayCapPhep

                }).FirstOrDefault();

            Guid PetroleumBusinessName = result.PetroleumBusinessName;
            List<Guid> ListId = _context.PetroleumBusinesses.Where(x => x.PetroleumBusinessName == PetroleumBusinessName).Select(x => x.PetroleumBusinessId).ToList();
            var detail = (from pb in _context.PetroleumBusinessStores
                          where ListId.Contains(pb.PetroleumBusinessId)
                          join d in _context.Districts on pb.Huyen equals d.DistrictId
                          join c in _context.Communes on pb.Xa equals c.CommuneId
                          join cate in _context.Categories on pb.HinhThuc equals cate.CategoryId
                          join cate1 in _context.Categories on pb.LoaiGiayXacNhan equals cate1.CategoryId into pbcate1
                          from cate1 in pbcate1.DefaultIfEmpty()
                          join cate2 in _context.Categories on pb.HinhThucHopDong equals cate2.CategoryId into pbcate2
                          from cate2 in pbcate2.DefaultIfEmpty()
                          select new PetroleumBusinessDetailModel()
                          {
                              TenCuaHang = pb.TenCuaHang,
                              NguoiDaiDien = pb.NguoiDaiDien,
                              SoDienThoai = pb.SoDienThoai,
                              Huyen = pb.Huyen,
                              TenHuyen = d.DistrictName,
                              Xa = pb.Xa,
                              TenXa = c.CommuneName,
                              HinhThuc = pb.HinhThuc,
                              TenHinhThuc = cate.CategoryName,
                              DiaChi = pb.DiaChi,
                              GiayPhepKinhDoanh = pb.GiayPhepKinhDoanh,
                              NgayHetHan = pb.NgayHetHan,
                              DonViCungCap = pb.DonViCungCap,
                              ThoiHan5Nam = pb.ThoiHan5Nam,
                              NguoiQuanLy = pb.NguoiQuanLy,
                              NgayCapPhep = pb.NgayCapPhep,
                              SoBeChua = pb.SoBeChua,
                              SoCotBomOil = pb.SoCotBomOil,
                              SoCotBomE5 = pb.SoCotBomE5,
                              SoCotBomA95 = pb.SoCotBomA95,
                              DienTichXayDung = pb.DienTichXayDung,
                              ThoiGianBanHang = pb.ThoiGianBanHang,
                              TongDungTich = pb.TongDungTich,
                              TuyenPhucVu = pb.TuyenPhucVu,
                              ThoiHan1Nam = pb.ThoiHan1Nam,
                              NguoiLienHeDonViCungCap = pb.NguoiLienHeDonViCungCap,
                              DiaChiDonViCungCap = pb.DiaChiDonViCungCap,
                              SoDienThoaiDonViCungCap = pb.SoDienThoaiDonViCungCap,
                              NgayCapPhepXayDung = pb.NgayCapPhepXayDung,
                              GhiChu = pb.GhiChu,
                              TenLoaiGiayXacNhan = cate1.CategoryName,
                              TenHinhThucHopDong = cate2.CategoryName
                          }).ToList();

            result.PetroleumBusinessDetail = detail;
            return result;
        }

        // lay danh sach doanh nghiep
        public IQueryable<Business> FindAll()
        {
            var result = _context.Businesses.Select(d => new Business()
            {
                BusinessId = d.BusinessId,
                BusinessCode = d.BusinessCode,
                NguoiDaiDien = d.NguoiDaiDien,
                TenGiaoDich = d.TenGiaoDich,
                BusinessNameEn = d.BusinessNameEn,
                DiaChiTruSo = d.DiaChiTruSo,
                NgayCapPhep = d.NgayCapPhep,
                MaSoThue = d.MaSoThue,
                BusinessNameVi = d.BusinessNameVi,
                LoaiHinhDoanhNghiep = d.LoaiHinhDoanhNghiep,
                LoaiNganhNghe = d.LoaiNganhNghe,
                NgayHoatDong = d.NgayHoatDong,
                SoDienThoai = d.SoDienThoai,
                DiaChi = d.DiaChi,
                GiamDoc = d.GiamDoc,
                Email = d.Email,
                IsDel = d.IsDel,
            });
            return result;
        }
        public bool IsExistPetroleumBusiness(Guid id, Guid? PetroleumBusinessId)
        {
            if (PetroleumBusinessId != null)
            {
                var countItem = _context.PetroleumBusinesses.Where(x => !x.IsDel && x.PetroleumBusinessName == id && x.PetroleumBusinessId == PetroleumBusinessId).Count();
                if (countItem == 1)
                {
                    return false;
                }
            }
            var count = _context.PetroleumBusinesses.Where(x => !x.IsDel && x.PetroleumBusinessName == id).Count();
            if (count == 0)
                return false;
            return true;
        }

        public List<PetroleumBusinessModel> FindData(QueryRequestBody query)
        {
            List<PetroleumBusinessModel> result = new List<PetroleumBusinessModel>();
            IQueryable<PetroleumBusinessModel> _data = _context.PetroleumBusinesses.Where(x => !x.IsDel).Join(
                _context.Businesses,
                cc => cc.PetroleumBusinessName, cd => cd.BusinessId,
                (cc, cd) => new PetroleumBusinessModel

                {
                    PetroleumBusinessId = cc.PetroleumBusinessId,
                    PetroleumBusinessName = cc.PetroleumBusinessName,
                    Address = cd.DiaChi ?? "",
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
                _data = _data.Where(x => x.BusinessNameVi.ToLower().Contains(_keywordSearch) || x.Representative.ToLower().Contains(_keywordSearch) || x.Address.ToLower().Contains(_keywordSearch));  //Lấy table đã select tìm kiếm theo keyword
            }


            //Count total
            result = _data.ToList();

            var listId = _data.Select(x => x.PetroleumBusinessId).ToList();
            var listTotal = _context.PetroleumBusinesses.Where(x => listId.Contains(x.PetroleumBusinessId)).Select(x =>
                new PetroleumBusinessModel
                {
                    PetroleumBusinessId = x.PetroleumBusinessId,
                    TotalStore = _context.PetroleumBusinessStores.Where(z => z.PetroleumBusinessId == x.PetroleumBusinessId).Count()
                }).ToList();

            for (int i = 0; i < _data.Count(); i++)
            {
                int tt = listTotal.Where(x => x.PetroleumBusinessId == result[i].PetroleumBusinessId).Select(x => x.TotalStore).FirstOrDefault(0);
                result[i].TotalStore = tt;
            }

            return result;
        }
    }
}
