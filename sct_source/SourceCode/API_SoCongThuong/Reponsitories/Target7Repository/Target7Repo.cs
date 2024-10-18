using EF_Core.Models;

namespace API_SoCongThuong.Reponsitories.Target7Repository
{
    public class Target7Repo
    {
        public SoHoa_SoCongThuongContext _context;

        public Target7Repo(SoHoa_SoCongThuongContext context)
        {
            _context = context;
        }

        public async Task Insert(Target7 model)
        {
            await _context.Target7s.AddAsync(model);
            await _context.SaveChangesAsync();
        }

        public async Task Update(Target7 model)
        {
            _context.Target7s.Update(model);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(Target7 model)
        {
            _context.Target7s.Update(model);
            await _context.SaveChangesAsync();
        }

        public IQueryable<Target7> FindById(Guid Id)
        {
            var result = _context.Target7s
                .Where(x => x.Target7Id == Id && !x.IsDel)
                .Select(f => new Target7()
                {
                    Target7Id = f.Target7Id,
                    StageId = f.StageId,
                    Year = f.Year,
                    DistrictId = f.DistrictId,
                    CommuneId = f.CommuneId,
                    MarketInPlaning = f.MarketInPlaning,
                    PlanCommercial = f.PlanCommercial,
                    PlanMarket = f.PlanMarket,
                    EstimateCommercial = f.EstimateCommercial,
                    EstimateMarket = f.EstimateMarket,
                    NewRuralCriteriaRaised = f.NewRuralCriteriaRaised,
                    Note = f.Note,
                });

            return result;
        }
    }
}
