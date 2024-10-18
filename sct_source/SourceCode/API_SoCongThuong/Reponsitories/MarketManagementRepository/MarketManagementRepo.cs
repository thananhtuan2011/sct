using API_SoCongThuong.Models;
using EF_Core.Models;
using Microsoft.EntityFrameworkCore;

namespace API_SoCongThuong.Reponsitories.MarketManagementRepository
{
    public class MarketManagementRepo
    {
        public SoHoa_SoCongThuongContext _context;
        public MarketManagementRepo(SoHoa_SoCongThuongContext context)
        {
            _context = context;
        }
        public async Task Insert(MarketManagementModel model)
        {
            MarketManagement SaveData = new MarketManagement();
            SaveData.DistrictId = model.DistrictId;
            SaveData.CommuneId = model.CommuneId;
            SaveData.MarketId = model.MarketId;
            SaveData.NganhHangKinhDoanh = model.NganhHangKinhDoanh;
            SaveData.BoothNumber = model.BoothNumber;
            SaveData.GiaTrongNhaLong = model.GiaTrongNhaLong;
            SaveData.GiaNgoaiNhaLong = model.GiaNgoaiNhaLong;
            SaveData.DeXuatGiaMoi = model.DeXuatGiaMoi;
            SaveData.Note = model.Note;

            SaveData.CreateUserId = model.CreateUserId;
            SaveData.CreateTime = model.CreateTime;

            await _context.MarketManagements.AddAsync(SaveData);
            List<MarketManagementDetail> dataDetails = new List<MarketManagementDetail>();
            foreach(var item in model.MatHang)
            {
                MarketManagementDetail detail = new MarketManagementDetail();
                detail.BusinessLineId = item.BusinessLineId;
                detail.BusinessLineName = item.BusinessLineName;
                detail.MarketId = model.MarketId;
                detail.Price = item.Price;
                dataDetails.Add(detail);
            }
            await _context.MarketManagementDetails.AddRangeAsync(dataDetails);
            await _context.SaveChangesAsync();
        }

        public async Task Update(MarketManagementModel model)
        {
            MarketManagement? SaveData = _context.MarketManagements.Where(x => x.MarketManagementId == model.MarketManagementId && x.IsDel == false).FirstOrDefault();

            if(SaveData != null)
            {
                SaveData.MarketManagementId = Guid.Parse(model.MarketManagementId.ToString());
                SaveData.DistrictId = model.DistrictId;
                SaveData.CommuneId = model.CommuneId;
                SaveData.MarketId = model.MarketId;
                SaveData.NganhHangKinhDoanh = model.NganhHangKinhDoanh;
                SaveData.BoothNumber = model.BoothNumber;
                SaveData.GiaTrongNhaLong = model.GiaTrongNhaLong;
                SaveData.GiaNgoaiNhaLong = model.GiaNgoaiNhaLong;
                SaveData.DeXuatGiaMoi = model.DeXuatGiaMoi;
                SaveData.Note = model.Note;

                SaveData.UpdateUserId = model.UpdateUserId;
                SaveData.UpdateTime = model.UpdateTime;
                _context.MarketManagements.Update(SaveData);

                List<MarketManagementDetail> deleteData = _context.MarketManagementDetails.Where(x => x.MarketId == model.MarketId).ToList();
                _context.MarketManagementDetails.RemoveRange(deleteData);
                await _context.SaveChangesAsync();

                List<MarketManagementDetail> dataDetails = new List<MarketManagementDetail>();
                foreach (var item in model.MatHang)
                {
                    MarketManagementDetail detail = new MarketManagementDetail();
                    detail.BusinessLineId = item.BusinessLineId;
                    detail.BusinessLineName = item.BusinessLineName;
                    detail.MarketId = model.MarketId;
                    detail.Price = item.Price;
                    dataDetails.Add(detail);
                }
                await _context.MarketManagementDetails.AddRangeAsync(dataDetails);
                await _context.SaveChangesAsync();
            }
        }
        public async Task DeleteMarketManagement(MarketManagement model)
        {
            var db = await _context.MarketManagements.Where(d => d.MarketManagementId == model.MarketManagementId).FirstOrDefaultAsync();
            Guid marketId = Guid.Empty;
            if (db != null)
            {
                db.IsDel = model.IsDel;
                marketId = db.MarketId;
            }

            List<MarketManagementDetail> details = _context.MarketManagementDetails.Where(x => x.MarketId == marketId && !x.IsDel).ToList();
            _context.MarketManagementDetails.RemoveRange(details);
            await _context.SaveChangesAsync();
        }
        public async Task Delete(Guid id)
        {
            var itemRemove = await _context.MarketManagements.Where(x => x.MarketManagementId == id).FirstOrDefaultAsync();
            _context.MarketManagements.Remove(itemRemove);
            await _context.SaveChangesAsync();
        }

        public IQueryable<MarketManagement> FindAll()
        {
            var result = _context.MarketManagements.Select(d => new MarketManagement()
            {
                MarketManagementId = d.MarketManagementId,
                DistrictId = d.DistrictId,
                CommuneId = d.CommuneId,
                MarketId = d.MarketId,
                NganhHangKinhDoanh = d.NganhHangKinhDoanh,
                BoothNumber = d.BoothNumber,
                GiaTrongNhaLong = d.GiaTrongNhaLong,
                GiaNgoaiNhaLong = d.GiaNgoaiNhaLong,
                DeXuatGiaMoi = d.DeXuatGiaMoi,
                Note = d.Note,
                IsDel = d.IsDel,
            });

            return result;
        }

        public IQueryable<MarketManagementModel> FindById(Guid Id)
        {
            var result = _context.MarketManagements.Where(x => x.MarketManagementId == Id).Select(d => new MarketManagementModel()
            {
                MarketManagementId = d.MarketManagementId,
                DistrictId = d.DistrictId,
                CommuneId = d.CommuneId,
                MarketId = d.MarketId,
                NganhHangKinhDoanh = d.NganhHangKinhDoanh,
                BoothNumber = d.BoothNumber,
                GiaTrongNhaLong = d.GiaTrongNhaLong,
                GiaNgoaiNhaLong = d.GiaNgoaiNhaLong,
                DeXuatGiaMoi = d.DeXuatGiaMoi,
                Note = d.Note,
                IsDel = d.IsDel,
                MatHang = (_context.MarketManagementDetails.Where(x => !x.IsDel && x.MarketId == d.MarketId).Select(item => new BusinessLineDetailModel()
                {
                    BusinessLineId = item.BusinessLineId,
                    BusinessLineName = item.BusinessLineName,
                    Price = item.Price,
                })).ToList()
            });

            return result;
        }
    }
}
