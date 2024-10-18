using EF_Core.Models;
using Microsoft.EntityFrameworkCore;

namespace API_SoCongThuong.Reponsitories.TypeOfBusinessRepository
{
    public class TypeOfBusinessRepo
    {
        public SoHoa_SoCongThuongContext _context;
        public TypeOfBusinessRepo(SoHoa_SoCongThuongContext context)
        {
            _context = context;
        }
        public async Task Insert(TypeOfBusiness model)
        {
            await _context.TypeOfBusinesses.AddAsync(model);
            await _context.SaveChangesAsync();
        }

        public async Task Update(TypeOfBusiness model)
        {
            _context.TypeOfBusinesses.Update(model);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteTypeOfBusiness(TypeOfBusiness model)
        {
            var db = await _context.TypeOfBusinesses.Where(d => d.TypeOfBusinessId == model.TypeOfBusinessId).FirstOrDefaultAsync();
            db.IsDel = model.IsDel;
            await _context.SaveChangesAsync();
        }
        public async Task Delete(Guid id)
        {
            var itemRemove = await _context.TypeOfBusinesses.Where(x => x.TypeOfBusinessId == id).FirstOrDefaultAsync();
            _context.TypeOfBusinesses.Remove(itemRemove);
            await _context.SaveChangesAsync();
        }

        public IQueryable<TypeOfBusiness> FindAll()
        {
            var result = _context.TypeOfBusinesses.Select(d => new TypeOfBusiness()
            {
                TypeOfBusinessId = d.TypeOfBusinessId,
                TypeOfBusinessCode = d.TypeOfBusinessCode,
                TypeOfBusinessName = d.TypeOfBusinessName,
                IsDel = d.IsDel,
            });

            return result;
        }

        public IQueryable<TypeOfBusiness> FindById(Guid Id)
        {
            var result = _context.TypeOfBusinesses.Where(x => x.TypeOfBusinessId == Id).Select(d => new TypeOfBusiness()
            {
                TypeOfBusinessId = d.TypeOfBusinessId,
                TypeOfBusinessCode = d.TypeOfBusinessCode,
                TypeOfBusinessName = d.TypeOfBusinessName,
                IsDel = d.IsDel,
            });

            return result;
        }

        public bool findByTypeOfBusinessCode(string typeOfBusinessCode, Guid? typeOfBusinessId)
        {
            if (typeOfBusinessId != null)
            {
                var TypeOfBusinessCode = _context.TypeOfBusinesses.Where(x => x.TypeOfBusinessId == typeOfBusinessId && x.TypeOfBusinessCode == typeOfBusinessCode && !x.IsDel).FirstOrDefault();
                if (TypeOfBusinessCode != null)
                {
                    return false;
                }
            }
            var isTypeOfBusinessCode = _context.TypeOfBusinesses.Where(x => x.TypeOfBusinessCode == typeOfBusinessCode && !x.IsDel).FirstOrDefault();
            if (isTypeOfBusinessCode == null)
            {
                return false;
            }
            return true;
        }
    }
}
