using EF_Core.Models;
using Microsoft.EntityFrameworkCore;

namespace API_SoCongThuong.Reponsitories.TypeOfEnergyRepository
{
    public class TypeOfEnergyRepo
    {
        public SoHoa_SoCongThuongContext _context;
        public TypeOfEnergyRepo(SoHoa_SoCongThuongContext context)
        {
            _context = context;
        }
        public async Task Insert(TypeOfEnergy model)
        {
            await _context.TypeOfEnergies.AddAsync(model);
            await _context.SaveChangesAsync();
        }

        public async Task Update(TypeOfEnergy model)
        {
            var db = await _context.TypeOfEnergies.Where(d => d.TypeOfEnergyId == model.TypeOfEnergyId).FirstOrDefaultAsync();
            db.TypeOfEnergyName = model.TypeOfEnergyName;
            db.TypeOfEnergyCode = model.TypeOfEnergyCode;
            db.IsDel = model.IsDel;
            await _context.SaveChangesAsync();
        }
        public async Task DeleteTypeOfEnergy(TypeOfEnergy model)
        {
            var db = await _context.TypeOfEnergies.Where(d => d.TypeOfEnergyId == model.TypeOfEnergyId).FirstOrDefaultAsync();
            db.IsDel = model.IsDel;
            await _context.SaveChangesAsync();
        }
        public async Task Delete(Guid id)
        {
            var itemRemove = await _context.TypeOfEnergies.Where(x => x.TypeOfEnergyId == id).FirstOrDefaultAsync();
            _context.TypeOfEnergies.Remove(itemRemove);
            await _context.SaveChangesAsync();
        }

        public IQueryable<TypeOfEnergy> FindAll()
        {
            var result = _context.TypeOfEnergies.Select(d => new TypeOfEnergy()
            {
                TypeOfEnergyId = d.TypeOfEnergyId,
                TypeOfEnergyCode = d.TypeOfEnergyCode,
                TypeOfEnergyName = d.TypeOfEnergyName,
                IsDel = d.IsDel,
            });

            return result;
        }

        public IQueryable<TypeOfEnergy> FindById(Guid Id)
        {
            var result = _context.TypeOfEnergies.Where(x => x.TypeOfEnergyId == Id).Select(d => new TypeOfEnergy()
            {
                TypeOfEnergyId = d.TypeOfEnergyId,
                TypeOfEnergyCode = d.TypeOfEnergyCode,
                TypeOfEnergyName = d.TypeOfEnergyName,
                IsDel = d.IsDel,
            });

            return result;
        }

        public bool findByTypeOfEnergyCode(string typeOfEnergyCode, Guid? typeOfEnergyId)
        {
            if (typeOfEnergyId != null)
            {
                var TypeOfEnergyCode = _context.TypeOfEnergies.Where(x => x.TypeOfEnergyId == typeOfEnergyId && x.TypeOfEnergyCode == typeOfEnergyCode && !x.IsDel).FirstOrDefault();
                if (TypeOfEnergyCode != null)
                {
                    return false;
                }
            }
            var isTypeOfEnergyCode = _context.TypeOfEnergies.Where(x => x.TypeOfEnergyCode == typeOfEnergyCode && !x.IsDel).FirstOrDefault();
            if (isTypeOfEnergyCode == null)
            {
                return false;
            }
            return true;
        }
    }
}
