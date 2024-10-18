using API_SoCongThuong.Models;
using EF_Core.Models;
using Microsoft.EntityFrameworkCore;

namespace API_SoCongThuong.Reponsitories
{
    public class MarketTargetSevenRepo
    {
        public SoHoa_SoCongThuongContext _context;
        public MarketTargetSevenRepo(SoHoa_SoCongThuongContext context)
        {
            _context = context;
        }
        public async Task Insert(MarketTargetSeven model)
        {
            await _context.MarketTargetSevens.AddAsync(model);
            await _context.SaveChangesAsync();
        }

        public async Task Update(MarketTargetSeven model)
        {
            _context.MarketTargetSevens.Update(model);
            await _context.SaveChangesAsync();
        }
        public async Task Delete(MarketTargetSeven model)
        {
            var db = await _context.MarketTargetSevens.Where(d => d.MarketTargetSevenId == model.MarketTargetSevenId).FirstOrDefaultAsync();
            db.IsDel = model.IsDel;
            await _context.SaveChangesAsync();
        }

        public IQueryable<MarketTargetSeven> FindAll()
        {
            var result = _context.MarketTargetSevens.Select(d => new MarketTargetSeven()
            {
                Note = d.Note,
                IsDel = d.IsDel,
            });

            return result;
        }

        public IQueryable<MarketTargetSevenModel> FindById(Guid Id)
        {
            var result = _context.MarketTargetSevens.Where(x => x.MarketTargetSevenId == Id).Select(d => new MarketTargetSevenModel()
            {
                MarketTargetSevenId = d.MarketTargetSevenId,
                MarketName = d.MarketName,
                DistrictId = d.DistrictId,
                CommuneId = d.CommuneId,
                Address = d.Address,
                Year = d.Year,
                Month = d.Month,
                Date = d.Year.ToString() + "-" + d.Month.ToString("D2"),
                Note = d.Note
        });
            return result;
        }
    }
}
