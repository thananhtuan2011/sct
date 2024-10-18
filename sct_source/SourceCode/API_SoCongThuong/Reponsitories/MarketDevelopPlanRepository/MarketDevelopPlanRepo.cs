using API_SoCongThuong.Models;
using EF_Core.Models;
using Microsoft.EntityFrameworkCore;

namespace API_SoCongThuong.Reponsitories
{
    public class MarketDevelopPlanRepo
    {
        public SoHoa_SoCongThuongContext _context;
        public MarketDevelopPlanRepo(SoHoa_SoCongThuongContext context)
        {
            _context = context;
        }
        public async Task Insert(MarketDevelopPlan model)
        {
            await _context.MarketDevelopPlans.AddAsync(model);
            await _context.SaveChangesAsync();
        }

        public async Task Update(MarketDevelopPlan model)
        {
            _context.MarketDevelopPlans.Update(model);
            await _context.SaveChangesAsync();
        }
        public async Task Delete(MarketDevelopPlan model)
        {
            var db = await _context.MarketDevelopPlans.Where(d => d.MarketDevelopPlanId == model.MarketDevelopPlanId).FirstOrDefaultAsync();
            db.IsDel = model.IsDel;
            await _context.SaveChangesAsync();
        }

        public IQueryable<MarketDevelopPlan> FindAll()
        {
            var result = _context.MarketDevelopPlans.Select(d => new MarketDevelopPlan()
            {
                Note = d.Note,
                IsDel = d.IsDel,
            });

            return result;
        }

        public IQueryable<MarketDevelopPlan> FindById(Guid Id)
        {
            var result = _context.MarketDevelopPlans.Where(x => x.MarketDevelopPlanId == Id).Select(d => new MarketDevelopPlan()
            {
                MarketDevelopPlanId = d.MarketDevelopPlanId,
                MarketName = d.MarketName,
                RankId = d.RankId,
                DistrictId = d.DistrictId,
                CommuneId = d.CommuneId,
                Address = d.Address,
                Stage = d.Stage,
                TypeOfPlanMarket = d.TypeOfPlanMarket,
                ExistLandArea = d.ExistLandArea,
                NewLandArea = d.NewLandArea,
                AddLandArea = d.AddLandArea,
                Capital = d.Capital,
                Note = d.Note
            });
            return result;
        }
    }
}
