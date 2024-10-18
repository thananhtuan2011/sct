using EF_Core.Models;
using Microsoft.EntityFrameworkCore;

namespace API_SoCongThuong.Reponsitories
{
    public class RecordsManagerRepo
    {
        public SoHoa_SoCongThuongContext _context;
        public RecordsManagerRepo(SoHoa_SoCongThuongContext context)
        {
            _context = context;
        }
        public async Task Insert(RecordsManager model)
        {
            await _context.RecordsManagers.AddAsync(model);
            await _context.SaveChangesAsync();
        }

        public async Task Update(RecordsManager model)
        {
            _context.RecordsManagers.Update(model);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(RecordsManager model)
        {
            var db = await _context.RecordsManagers.Where(d => d.RecordsManagerId == model.RecordsManagerId).FirstOrDefaultAsync();
            if (db != null)
            {
                db.IsDel = model.IsDel;
            }
            await _context.SaveChangesAsync();
        }
    }
}
