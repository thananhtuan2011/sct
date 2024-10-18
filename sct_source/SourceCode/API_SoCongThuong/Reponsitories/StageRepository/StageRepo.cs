using EF_Core.Models;
using Microsoft.EntityFrameworkCore;

namespace API_SoCongThuong.Reponsitories.StageRepository
{
    public class StageRepo
    {
        public SoHoa_SoCongThuongContext _context;
        public StageRepo(SoHoa_SoCongThuongContext context)
        {
            _context = context;
        }
        public async Task Insert(Stage model)
        {
            await _context.Stages.AddAsync(model);
            await _context.SaveChangesAsync();
        }

        public async Task Update(Stage model)
        {
            _context.Stages.Update(model);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteStage(Stage model)
        {
            var db = await _context.Stages.Where(d => d.StageId == model.StageId).FirstOrDefaultAsync();
            db.IsDel = model.IsDel;
            await _context.SaveChangesAsync();
        }
        public async Task Delete(Guid id)
        {
            var itemRemove = await _context.Stages.Where(x => x.StageId == id).FirstOrDefaultAsync();
            _context.Stages.Remove(itemRemove);
            await _context.SaveChangesAsync();
        }

        public IQueryable<Stage> FindAll()
        {
            var result = _context.Stages.Select(d => new Stage()
            {
                StageId = d.StageId,
                StageName = d.StageName,
                StartYear = d.StartYear,
                EndYear = d.EndYear,
                IsDel = d.IsDel,
            });
            return result;
        }

        public IQueryable<Stage> FindById(Guid Id)
        {
            var result = _context.Stages.Where(x => x.StageId == Id).Select(d => new Stage()
            {
                StageId = d.StageId,
                StageName = d.StageName,
                StartYear = d.StartYear,
                EndYear = d.EndYear,
                IsDel = d.IsDel,
            });

            return result;
        }
    }
}
