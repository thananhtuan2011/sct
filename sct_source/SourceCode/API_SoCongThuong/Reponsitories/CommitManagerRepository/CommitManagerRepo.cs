using API_SoCongThuong.Models;
using DocumentFormat.OpenXml.Drawing.Diagrams;
using EF_Core.Models;
using Microsoft.EntityFrameworkCore;
namespace API_SoCongThuong.Reponsitories
{
    public class CommitManagerRepo
    {
        public SoHoa_SoCongThuongContext _context;
        public CommitManagerRepo(SoHoa_SoCongThuongContext context)
        {
            _context = context;
        }
        public async Task Insert(CommitManagerModel model)
        {
            CommitManager SaveData = new CommitManager();
            SaveData.MaHoSo = model.MaHoSo;
            SaveData.TenThuTuc = model.TenThuTuc;
            SaveData.TenToChuc = model.TenToChuc;
            SaveData.CoSo = model.CoSo;
            SaveData.Huyen = model.Huyen;
            SaveData.DiaChi = model.DiaChi;
            SaveData.SoDienThoai = model.SoDienThoai;
            SaveData.NgayNhanHoSo = model.NgayNhanHoSo;
            SaveData.NgayCamKet = model.NgayCamKet;
            SaveData.NguoiLamCamKet = model.NguoiLamCamKet;
            SaveData.GhiChu = model.GhiChu;
            SaveData.CreateUserId = model.CreateUserId;
            SaveData.CreateTime = model.CreateTime;
            await _context.CommitManagers.AddAsync(SaveData);
            await _context.SaveChangesAsync();
            if(model.ListItems.Count > 0)
            {
                List<CommitManagerListItem> list = new List<CommitManagerListItem>();
                foreach(var item in model.ListItems)
                {
                    CommitManagerListItem data = new CommitManagerListItem();
                    data.CommitManagerId = SaveData.CommitManagerId;
                    data.LoaiHinh = item.LoaiHinh;
                    data.TenMatHang = item.TenMatHang;
                    data.CreateUserId = model.CreateUserId;
                    data.CreateTime = model.CreateTime;
                    list.Add(data);
                }
                await _context.CommitManagerListItems.AddRangeAsync(list);
                await _context.SaveChangesAsync();
            }
        }

        public async Task Update(CommitManagerModel model)
        {
            var SaveData = await _context.CommitManagers.Where(x => !x.IsDel && x.CommitManagerId == model.CommitManagerId).FirstOrDefaultAsync();
            if(SaveData != null)
            {
                SaveData.MaHoSo = model.MaHoSo;
                SaveData.TenThuTuc = model.TenThuTuc;
                SaveData.TenToChuc = model.TenToChuc;
                SaveData.CoSo = model.CoSo;
                SaveData.Huyen = model.Huyen;
                SaveData.DiaChi = model.DiaChi;
                SaveData.SoDienThoai = model.SoDienThoai;
                SaveData.NgayNhanHoSo = model.NgayNhanHoSo;
                SaveData.NgayCamKet = model.NgayCamKet;
                SaveData.NguoiLamCamKet = model.NguoiLamCamKet;
                SaveData.GhiChu = model.GhiChu;
                SaveData.UpdateTime = model.UpdateTime;
                SaveData.UpdateUserId = model.UpdateUserId;
                await _context.SaveChangesAsync();
            }
            var listItemDetail = _context.CommitManagerListItems.Where(x => x.CommitManagerId == model.CommitManagerId).ToList();
            _context.CommitManagerListItems.RemoveRange(listItemDetail);
            await _context.SaveChangesAsync();
            if (model.ListItems.Count > 0)
            {
                List<CommitManagerListItem> list = new List<CommitManagerListItem>();
                foreach (var item in model.ListItems)
                {
                    CommitManagerListItem data = new CommitManagerListItem();
                    data.CommitManagerId = SaveData.CommitManagerId;
                    data.LoaiHinh = item.LoaiHinh;
                    data.TenMatHang = item.TenMatHang;
                    data.CreateUserId = model.CreateUserId;
                    data.CreateTime = model.CreateTime;
                    list.Add(data);
                }
                await _context.CommitManagerListItems.AddRangeAsync(list);
                await _context.SaveChangesAsync();
            }

        }
        public async Task Delete(CommitManager model)
        {
            var db = await _context.CommitManagers.Where(d => d.CommitManagerId == model.CommitManagerId).FirstOrDefaultAsync();
            if (db != null)
            {
                db.IsDel = model.IsDel;
            }
            var listItemDetail = _context.CommitManagerListItems.Where(x => x.CommitManagerId == model.CommitManagerId).ToList();
            _context.CommitManagerListItems.RemoveRange(listItemDetail);
            await _context.SaveChangesAsync();
        }
    }
}
