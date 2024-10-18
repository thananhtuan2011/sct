using EF_Core.Models;
using Microsoft.EntityFrameworkCore;

namespace API_SoCongThuong.Reponsitories.EnergyIndustryRepository
{
    public class EnergyIndustryRepo
    {
        public SoHoa_SoCongThuongContext _context;
        public EnergyIndustryRepo(SoHoa_SoCongThuongContext context)
        {
            _context = context;
        }
        public async Task Insert(EnergyIndustry model)
        {
            await _context.EnergyIndustries.AddAsync(model);
            await _context.SaveChangesAsync();
        }

        public async Task Update(EnergyIndustry model)
        {
            _context.EnergyIndustries.Update(model);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteEnergyIndustry(EnergyIndustry model)
        {
            var db = await _context.EnergyIndustries.Where(d => d.EnergyIndustryId == model.EnergyIndustryId).FirstOrDefaultAsync();
            db.IsDel = model.IsDel;
            await _context.SaveChangesAsync();
        }
        public async Task Delete(Guid id)
        {
            var itemRemove = await _context.EnergyIndustries.Where(x => x.EnergyIndustryId == id).FirstOrDefaultAsync();
            _context.EnergyIndustries.Remove(itemRemove);
            await _context.SaveChangesAsync();
        }

        public IQueryable<EnergyIndustry> FindAll()
        {
            var result = _context.EnergyIndustries.Select(d => new EnergyIndustry()
            {
                EnergyIndustryId = d.EnergyIndustryId,
                EnergyIndustryCode = d.EnergyIndustryCode,
                EnergyIndustryName = d.EnergyIndustryName,
                IsDel = d.IsDel,
            });

            return result;
        }

        public IQueryable<EnergyIndustry> FindById(Guid Id)
        {
            var result = _context.EnergyIndustries.Where(x => x.EnergyIndustryId == Id).Select(d => new EnergyIndustry()
            {
                EnergyIndustryId = d.EnergyIndustryId,
                EnergyIndustryCode = d.EnergyIndustryCode,
                EnergyIndustryName = d.EnergyIndustryName,
                IsDel = d.IsDel,
            });

            return result;
        }

        public bool findByEnergyIndustryCode(string energyIndustryCode, Guid? energyIndustryId)
        {
            if (energyIndustryId != null)
            {
                var EnergyIndustryCode = _context.EnergyIndustries.Where(x => x.EnergyIndustryId == energyIndustryId && x.EnergyIndustryCode == energyIndustryCode && !x.IsDel).FirstOrDefault();
                if (EnergyIndustryCode != null)
                {
                    return false;
                }
            }
            var isEnergyIndustryCode = _context.EnergyIndustries.Where(x => x.EnergyIndustryCode == energyIndustryCode && !x.IsDel).FirstOrDefault();
            if (isEnergyIndustryCode == null)
            {
                return false;
            }
            return true;
        }
    }
}
