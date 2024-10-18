using EF_Core.Models;
using Microsoft.EntityFrameworkCore;

namespace API_SoCongThuong.Reponsitories.CommuneRepository
{
    public class CommuneRepo
    {
        public SoHoa_SoCongThuongContext _context;
        public CommuneRepo(SoHoa_SoCongThuongContext context)
        {
            _context = context;
        }
        public async Task Insert(Commune model)
        {
            await _context.Communes.AddAsync(model);
            await _context.SaveChangesAsync();
        }

        public async Task Update(Commune model)
        {
            _context.Communes.Update(model);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteCommune(Commune model)
        {
            var db = await _context.Communes.Where(d => d.CommuneId == model.CommuneId).FirstOrDefaultAsync();
            db.IsDel = model.IsDel;
            await _context.SaveChangesAsync();
        }
        public async Task Delete(Guid id)
        {
            var itemRemove = await _context.Communes.Where(x => x.CommuneId == id).FirstOrDefaultAsync();
            _context.Communes.Remove(itemRemove);
            await _context.SaveChangesAsync();
        }

        public IQueryable<Commune> FindAll()
        {
            var result = _context.Communes.Select(d => new Commune()
            {
                CommuneId = d.CommuneId,
                CommuneCode = d.CommuneCode,
                CommuneName = d.CommuneName,
                DistrictId = d.DistrictId,
                IsDel = d.IsDel,
            });

            return result;
        }

        public IQueryable<Commune> FindById(Guid Id)
        {
            var result = _context.Communes.Where(x => x.CommuneId == Id).Select(d => new Commune()
            {
                CommuneId = d.CommuneId,
                CommuneCode = d.CommuneCode,
                CommuneName = d.CommuneName,
                DistrictId = d.DistrictId,
                IsDel = d.IsDel,
            });

            return result;
        }

        public bool findByCommuneCode(string communeCode, Guid? communeId)
        {
            if (communeId != null)
            {
                var CommuneCode = _context.Communes.Where(x => x.CommuneId == communeId && x.CommuneCode == communeCode && !x.IsDel).FirstOrDefault();
                if (CommuneCode != null)
                {
                    return false;
                }
            }
            var isCommuneCode = _context.Communes.Where(x => x.CommuneCode == communeCode && !x.IsDel).FirstOrDefault();
            if (isCommuneCode == null)
            {
                return false;
            }
            return true;
        }
    }
}
