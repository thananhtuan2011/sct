using EF_Core.Models;

namespace API_SoCongThuong.Reponsitories.Target1708Repository
{
    public class Target1708Repo
    {
        public SoHoa_SoCongThuongContext _context;

        public Target1708Repo(SoHoa_SoCongThuongContext context)
        {
            _context = context;
        }

        public async Task Insert(Target1708 model)
        {
            await _context.Target1708s.AddAsync(model);
            await _context.SaveChangesAsync();
        }

        public async Task Update(Target1708 model)
        {
            _context.Target1708s.Update(model);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(Target1708 model)
        {
            _context.Target1708s.Update(model);
            await _context.SaveChangesAsync();
        }

        public IQueryable<Target1708> FindById(Guid Id)
        {
            var result = _context.Target1708s
                .Where(x => x.Target1708Id == Id && !x.IsDel)
                .Select(f => new Target1708()
                {
                    Target1708Id = f.Target1708Id,
                    StageId = f.StageId,
                    DistrictId = f.DistrictId,
                    CommuneId = f.CommuneId,
                    NewRuralCriteria = f.NewRuralCriteria,
                    NewRuralCriteriaRaised = f.NewRuralCriteriaRaised,
                    Note = f.Note,
                });

            return result;
        }
    }
}
