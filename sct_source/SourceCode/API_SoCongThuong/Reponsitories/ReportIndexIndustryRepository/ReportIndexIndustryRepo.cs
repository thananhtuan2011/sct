using EF_Core.Models;
using API_SoCongThuong.Models;
using Microsoft.EntityFrameworkCore;

namespace API_SoCongThuong.Reponsitories
{
    public class ReportIndexIndustryRepo
    {
        public SoHoa_SoCongThuongContext _context;
        public ReportIndexIndustryRepo(SoHoa_SoCongThuongContext context)
        {
            _context = context;
        }

        public async Task Insert(ReportIndexIndustry model)
        {
            await _context.ReportIndexIndustries.AddAsync(model);
            await _context.SaveChangesAsync();
        }

        public async Task Update(ReportIndexIndustry model)
        {
             _context.ReportIndexIndustries.Update(model);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(ReportIndexIndustry model)
        {
            var db = await _context.ReportIndexIndustries.Where(x => x.ReportIndexIndustryId == model.ReportIndexIndustryId).FirstOrDefaultAsync();
            if (db != null)
            {
                db.IsDel = model.IsDel;
            }
            await _context.SaveChangesAsync();
        }
    }
}
