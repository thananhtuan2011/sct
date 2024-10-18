using EF_Core.Models;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;

namespace API_SoCongThuong.Reponsitories
{
    public class CateCriteriaRepo
    {
        public SoHoa_SoCongThuongContext _context;
        public CateCriteriaRepo(SoHoa_SoCongThuongContext context)
        {
            _context = context;
        }
        public async Task Insert(CateCriterion model)
        {
            await _context.CateCriteria.AddAsync(model);
            await _context.SaveChangesAsync();
        }
        public async Task Update(CateCriterion model)
        {
            var detailinfo = await _context.CateCriteria.Where(d => d.CateCriteriaId == model.CateCriteriaId).FirstOrDefaultAsync();
            detailinfo.CateCriteriaName = model.CateCriteriaName;
            await _context.SaveChangesAsync();
        }
        public async Task Delete(Guid CateCriteriaId)
        {
            var detailinfo = await _context.CateCriteria.Where(d => d.CateCriteriaId == CateCriteriaId).FirstOrDefaultAsync();
            detailinfo.IsDel = true;
            await _context.SaveChangesAsync();
        }
        public async Task Deletes(List<Guid> CateCriteriaIds)
        {
            List<CateCriterion> items = new List<CateCriterion>();
            foreach (var idremove in CateCriteriaIds)
            {
                CateCriterion item = new CateCriterion();
                var detailinfo = await _context.CateCriteria.Where(d => d.CateCriteriaId == idremove).FirstOrDefaultAsync();
                item.CateCriteriaId = idremove;
                item.IsDel = true;
                items.Add(item);
            }
            _context.CateCriteria.UpdateRange(items);
            await _context.SaveChangesAsync();
        }
        public CateCriterion FindById(Guid CateCriteriaId)
        {
            var result = _context.CateCriteria.Where(x => x.CateCriteriaId == CateCriteriaId && !x.IsDel).Select(d => new CateCriterion()
            {
                CateCriteriaId = d.CateCriteriaId,
                CateCriteriaName = d.CateCriteriaName,
                IsDel = d.IsDel
            }).FirstOrDefault();

            return result;
        }
    }
}
