using EF_Core.Models;
using Microsoft.EntityFrameworkCore;

namespace API_SoCongThuong.Reponsitories.ManagementFirePreventionRepository
{
    public class ManagementFirePreventionRepo
    {
        public SoHoa_SoCongThuongContext _context;
        public ManagementFirePreventionRepo(SoHoa_SoCongThuongContext context)
        {
            _context = context;
        }
        public async Task Insert(ManagementFirePrevention model)
        {
            await _context.ManagementFirePreventions.AddAsync(model);
            await _context.SaveChangesAsync();
        }

        public async Task Update(ManagementFirePrevention model)
        {
            _context.ManagementFirePreventions.Update(model);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteManagementFirePrevention(ManagementFirePrevention model)
        {
            var db = await _context.ManagementFirePreventions.Where(d => d.ManagementFirePreventionId == model.ManagementFirePreventionId).FirstOrDefaultAsync();
            db.IsDel = model.IsDel;
            await _context.SaveChangesAsync();
        }
        public async Task Delete(Guid id)
        {
            var itemRemove = await _context.ManagementFirePreventions.Where(x => x.ManagementFirePreventionId == id).FirstOrDefaultAsync();
            _context.ManagementFirePreventions.Remove(itemRemove);
            await _context.SaveChangesAsync();
        }

        public IQueryable<ManagementFirePrevention> FindAll()
        {
            var result = _context.ManagementFirePreventions.Select(d => new ManagementFirePrevention()
            {
                ManagementFirePreventionId = d.ManagementFirePreventionId,
                BusinessName = d.BusinessName,
                Address = d.Address,
                Reality = d.Reality,
                IsDel = d.IsDel,
            });
            return result;
        }

        public IQueryable<ManagementFirePrevention> FindById(Guid Id)
        {
            var result = _context.ManagementFirePreventions.Where(x => x.ManagementFirePreventionId == Id).Select(d => new ManagementFirePrevention()
            {
                ManagementFirePreventionId = d.ManagementFirePreventionId,
                BusinessName = d.BusinessName,
                Address = d.Address,
                Reality = d.Reality,
                IsDel = d.IsDel,
            });

            return result;
        }
    }
}
