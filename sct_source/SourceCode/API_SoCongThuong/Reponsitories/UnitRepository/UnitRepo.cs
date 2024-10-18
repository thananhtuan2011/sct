using EF_Core.Models;
using Microsoft.EntityFrameworkCore;

namespace API_SoCongThuong.Reponsitories.UnitRepository
{
    public class UnitRepo
    {
        public SoHoa_SoCongThuongContext _context;
        public UnitRepo(SoHoa_SoCongThuongContext context)
        {
            _context = context;
        }
        public async Task Insert(Unit model)
        {
            await _context.Units.AddAsync(model);
            await _context.SaveChangesAsync();
        }

        public async Task Update(Unit model)
        {
            _context.Units.Update(model);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteUnit(Unit model)
        {
            var db = await _context.Units.Where(d => d.UnitId == model.UnitId).FirstOrDefaultAsync();
            db.IsDel = model.IsDel;
            await _context.SaveChangesAsync();
        }
        public async Task Delete(Guid id)
        {
            var itemRemove = await _context.Units.Where(x => x.UnitId == id).FirstOrDefaultAsync();
            _context.Units.Remove(itemRemove);
            await _context.SaveChangesAsync();
        }

        public IQueryable<Unit> FindAll()
        {
            var result = _context.Units.Select(d => new Unit()
            {
                UnitId = d.UnitId,
                UnitCode = d.UnitCode,
                UnitName = d.UnitName,
                UnitNameEn = d.UnitNameEn,
                Exchange = d.Exchange,
                Note = d.Note,
                IsDel = d.IsDel,
            });

            return result;
        }

        public IQueryable<Unit> FindById(Guid Id)
        {
            var result = _context.Units.Where(x => x.UnitId == Id).Select(d => new Unit()
            {
                UnitId = d.UnitId,
                UnitCode = d.UnitCode,
                UnitName = d.UnitName,
                UnitNameEn = d.UnitNameEn,
                Exchange = d.Exchange,
                Note = d.Note,
                IsDel = d.IsDel,
            });

            return result;
        }

        public bool findByUnitCode(string unitCode, Guid? unitId)
        {
            if (unitId != null)
            {
                var UnitCode = _context.Units.Where(x => x.UnitId == unitId && x.UnitCode == unitCode && !x.IsDel).FirstOrDefault();
                if (UnitCode != null)
                {
                    return false;
                }
            }
            var isUnitCode = _context.Units.Where(x => x.UnitCode == unitCode && !x.IsDel).FirstOrDefault();
            if (isUnitCode == null)
            {
                return false;
            }
            return true;
        }
    }
}
