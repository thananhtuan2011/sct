using EF_Core.Models;
using Microsoft.EntityFrameworkCore;

namespace API_SoCongThuong.Reponsitories
{
    public class RecordsFinancePlanRepo
    {
        public SoHoa_SoCongThuongContext _context;
        public RecordsFinancePlanRepo(SoHoa_SoCongThuongContext context)
        {
            _context = context;
        }
        public async Task Insert(RecordsFinancePlan model)
        {
            await _context.RecordsFinancePlans.AddAsync(model);
            await _context.SaveChangesAsync();
        }

        public async Task Update(RecordsFinancePlan model)
        {
            _context.RecordsFinancePlans.Update(model);
            await _context.SaveChangesAsync();
        }
        public async Task Delete(RecordsFinancePlan model)
        {
            var db = await _context.RecordsFinancePlans.Where(d => d.RecordsFinancePlanId == model.RecordsFinancePlanId).FirstOrDefaultAsync();
            if(db != null)
            {
                db.IsDel = model.IsDel;
            }
            await _context.SaveChangesAsync();
        }
    }
}
