using EF_Core.Models;
using Microsoft.EntityFrameworkCore;

namespace API_SoCongThuong.Reponsitories.StateUnitsRepository
{
    public class StateUnitRepo
    {
        public SoHoa_SoCongThuongContext _context;
        public StateUnitRepo(SoHoa_SoCongThuongContext context)
        {
            _context = context;
        }
        public async Task Insert(StateUnit model)
        {
            await _context.StateUnits.AddAsync(model);
            await _context.SaveChangesAsync();
        }

        public async Task Update(StateUnit model)
        {
            _context.StateUnits.Update(model);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteStateUnits(StateUnit model)
        {
            var db = await _context.StateUnits.Where(d => d.StateUnitsId == model.StateUnitsId).FirstOrDefaultAsync();
            db.IsDel = model.IsDel;
            await _context.SaveChangesAsync();
        }

        public IQueryable<StateUnit> FindById(Guid Id)
        {
            var result = _context.StateUnits.Where(x => x.StateUnitsId == Id).Select(d => new StateUnit()
            {
                StateUnitsId = d.StateUnitsId,
                StateUnitsCode = d.StateUnitsCode,
                StateUnitsName = d.StateUnitsName,
                ParentId = d.ParentId,
                IsDel = d.IsDel,
            });

            return result;
        }

        public bool findByStateUnitsCode(string stateUnitsCode, Guid? stateUnitsId)
        {
            if (stateUnitsId != null)
            {
                var StateUnitsCode = _context.StateUnits.Where(x => x.StateUnitsId == stateUnitsId && x.StateUnitsCode == stateUnitsCode && !x.IsDel).FirstOrDefault();
                if (StateUnitsCode != null)
                {
                    return false;
                }
            }
            var isStateUnitsCode = _context.StateUnits.Where(x => x.StateUnitsCode == stateUnitsCode && !x.IsDel).FirstOrDefault();
            if (isStateUnitsCode == null)
            {
                return false;
            }
            return true;
        }
    }
}
