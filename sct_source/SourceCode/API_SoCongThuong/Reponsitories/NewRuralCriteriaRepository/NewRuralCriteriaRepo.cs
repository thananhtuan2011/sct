using API_SoCongThuong.Models;
using EF_Core.Models;

namespace API_SoCongThuong.Reponsitories.NewRuralCriteriaRepository
{
    public class NewRuralCriteriaRepo
    {
        public SoHoa_SoCongThuongContext _context;

        public NewRuralCriteriaRepo(SoHoa_SoCongThuongContext context)
        {
            _context = context;
        }

        public async Task Insert(NewRuralCriterion model)
        {
            await _context.NewRuralCriteria.AddAsync(model);
            await _context.SaveChangesAsync();
        }

        public async Task Update(NewRuralCriterion model)
        {
            _context.NewRuralCriteria.Update(model);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(NewRuralCriterion model)
        {
            _context.NewRuralCriteria.Update(model);
            await _context.SaveChangesAsync();
        }

        public IQueryable<NewRuralCriteriaModel> FindById(Guid Id)
        {
            var result = _context.NewRuralCriteria
                .Where(x => x.NewRuralCriteriaId == Id && !x.IsDel)
                .Select(f => new NewRuralCriteriaModel()
                {
                    NewRuralCriteriaId = f.NewRuralCriteriaId,
                    DistrictId = f.DistrictId,
                    CommuneId = f.CommuneId,
                    TitleIdStr = f.Title.ToString(),
                    Target4 = f.Target4,
                    Target7 = f.Target7,
                    Target1708 = f.Target1708,
                    Note = f.Note,
                });

            return result;
        }
    }
}
