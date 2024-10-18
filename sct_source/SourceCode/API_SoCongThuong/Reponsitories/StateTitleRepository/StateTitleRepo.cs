using EF_Core.Models;
using Microsoft.EntityFrameworkCore;

namespace API_SoCongThuong.Reponsitories.StateTitleRepository
{
    public class StateTitleRepo
    {
        public SoHoa_SoCongThuongContext _context;
        public StateTitleRepo(SoHoa_SoCongThuongContext context)
        {
            _context = context;
        }
        public async Task Insert(StateTitle model)
        {
            await _context.StateTitles.AddAsync(model);
            await _context.SaveChangesAsync();
        }

        public async Task Update(StateTitle model)
        {
            _context.StateTitles.Update(model);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteStateTitles(StateTitle model)
        {
            var db = await _context.StateTitles.Where(d => d.StateTitlesId == model.StateTitlesId).FirstOrDefaultAsync();
            db.IsDel = model.IsDel;
            await _context.SaveChangesAsync();
        }
        public async Task Delete(Guid id)
        {
            var itemRemove = await _context.StateTitles.Where(x => x.StateTitlesId == id).FirstOrDefaultAsync();
            _context.StateTitles.Remove(itemRemove);
            await _context.SaveChangesAsync();
        }

        public IQueryable<StateTitle> FindAll()
        {
            var result = _context.StateTitles.Select(d => new StateTitle()
            {
                StateTitlesId = d.StateTitlesId,
                StateTitlesCode = d.StateTitlesCode,
                StateTitlesName = d.StateTitlesName,
                Piority = d.Piority,
                IsDel = d.IsDel,
            });

            return result;
        }

        public IQueryable<StateTitle> FindById(Guid Id)
        {
            var result = _context.StateTitles.Where(x => x.StateTitlesId == Id).Select(d => new StateTitle()
            {
                StateTitlesId = d.StateTitlesId,
                StateTitlesCode = d.StateTitlesCode,
                StateTitlesName = d.StateTitlesName,
                Piority = d.Piority,
                IsDel = d.IsDel,
            });

            return result;
        }

        public bool findByStateTitlesCode(string stateTitlesCode, Guid? stateTitlesId)
        {
            if (stateTitlesId != null)
            {
                var StateTitlesCode = _context.StateTitles.Where(x => x.StateTitlesId == stateTitlesId && x.StateTitlesCode == stateTitlesCode && !x.IsDel).FirstOrDefault();
                if (StateTitlesCode != null)
                {
                    return false;
                }
            }
            var isStateTitlesCode = _context.StateTitles.Where(x => x.StateTitlesCode == stateTitlesCode && !x.IsDel).FirstOrDefault();
            if (isStateTitlesCode == null)
            {
                return false;
            }
            return true;
        }
    }
}
