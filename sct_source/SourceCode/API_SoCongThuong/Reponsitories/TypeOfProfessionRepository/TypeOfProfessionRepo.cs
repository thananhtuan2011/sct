using EF_Core.Models;
using Microsoft.EntityFrameworkCore;

namespace API_SoCongThuong.Reponsitories.TypeOfProfessionRepository
{
    public class TypeOfProfessionRepo
    {
        public SoHoa_SoCongThuongContext _context;
        public TypeOfProfessionRepo(SoHoa_SoCongThuongContext context)
        {
            _context = context;
        }
        public async Task Insert(TypeOfProfession model)
        {
            await _context.TypeOfProfessions.AddAsync(model);
            await _context.SaveChangesAsync();
        }

        public async Task Update(TypeOfProfession model)
        {
            _context.TypeOfProfessions.Update(model);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteTypeOfProfession(TypeOfProfession model)
        {
            var db = await _context.TypeOfProfessions.Where(d => d.TypeOfProfessionId == model.TypeOfProfessionId).FirstOrDefaultAsync();
            db.IsDel = model.IsDel;
            await _context.SaveChangesAsync();
        }
        public async Task Delete(Guid id)
        {
            var itemRemove = await _context.TypeOfProfessions.Where(x => x.TypeOfProfessionId == id).FirstOrDefaultAsync();
            _context.TypeOfProfessions.Remove(itemRemove);
            await _context.SaveChangesAsync();
        }

        public IQueryable<TypeOfProfession> FindAll()
        {
            var result = _context.TypeOfProfessions.Select(d => new TypeOfProfession()
            {
                TypeOfProfessionId = d.TypeOfProfessionId,
                TypeOfProfessionCode = d.TypeOfProfessionCode,
                TypeOfProfessionName = d.TypeOfProfessionName,
                IsDel = d.IsDel,
            });

            return result;
        }

        public IQueryable<TypeOfProfession> FindById(Guid Id)
        {
            var result = _context.TypeOfProfessions.Where(x => x.TypeOfProfessionId == Id).Select(d => new TypeOfProfession()
            {
                TypeOfProfessionId = d.TypeOfProfessionId,
                TypeOfProfessionCode = d.TypeOfProfessionCode,
                TypeOfProfessionName = d.TypeOfProfessionName,
                IsDel = d.IsDel,
            });

            return result;
        }

        public bool findByTypeOfProfessionCode(string typeOfProfessionCode, Guid? typeOfProfessionId)
        {
            if (typeOfProfessionId != null)
            {
                var TypeOfProfessionCode = _context.TypeOfProfessions.Where(x => x.TypeOfProfessionId == typeOfProfessionId && x.TypeOfProfessionCode == typeOfProfessionCode && !x.IsDel).FirstOrDefault();
                if (TypeOfProfessionCode != null)
                {
                    return false;
                }
            }
            var isTypeOfProfessionCode = _context.TypeOfProfessions.Where(x => x.TypeOfProfessionCode == typeOfProfessionCode && !x.IsDel).FirstOrDefault();
            if (isTypeOfProfessionCode == null)
            {
                return false;
            }
            return true;
        }
    }
}
