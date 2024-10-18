using EF_Core.Models;
using Microsoft.EntityFrameworkCore;
namespace API_SoCongThuong.Reponsitories
{
    public class GasBusinessRepo
    {
        public SoHoa_SoCongThuongContext _context;
        public GasBusinessRepo(SoHoa_SoCongThuongContext context)
        {
            _context = context;
        }
        public async Task Insert(GasBusiness model)
        {
            await _context.GasBusinesses.AddAsync(model);
            await _context.SaveChangesAsync();
        }

        public async Task Update(GasBusiness model)
        {
            _context.GasBusinesses.Update(model);
            await _context.SaveChangesAsync();
        }
        public async Task Delete(GasBusiness model)
        {
            var db = await _context.GasBusinesses.Where(d => d.GasBusinessId == model.GasBusinessId).FirstOrDefaultAsync();
            if (db != null)
            {
                db.IsDel = model.IsDel;
            }
            await _context.SaveChangesAsync();
        }
    }
}
