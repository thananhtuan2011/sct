using EF_Core.Models;
using Microsoft.EntityFrameworkCore;

namespace API_SoCongThuong.Reponsitories
{
    public class GasRepo
    {
        public SoHoa_SoCongThuongContext _context;
        public GasRepo(SoHoa_SoCongThuongContext context)
        {
            _context = context;
        }
        public async Task Insert(Ga model)
        {
            await _context.Gas.AddAsync(model);
            await _context.SaveChangesAsync();
        }

        public async Task Update(Ga model)
        {
            _context.Gas.Update(model);
            await _context.SaveChangesAsync();
        }
        public async Task Delete(Ga model)
        {
            var db = await _context.Gas.Where(d => d.GasId == model.GasId).FirstOrDefaultAsync();
            if (db != null)
            {
                db.IsDel = model.IsDel;
            }
            await _context.SaveChangesAsync();
        }
    }
}
