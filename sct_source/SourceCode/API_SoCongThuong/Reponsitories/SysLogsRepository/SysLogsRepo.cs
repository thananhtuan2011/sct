using EF_Core.Models;
using Microsoft.EntityFrameworkCore;

namespace API_SoCongThuong.Reponsitories.SysLogsRepository
{
    public class SysLogsRepo
    {
        public SoHoa_SoCongThuongContext _context;
        public SysLogsRepo(SoHoa_SoCongThuongContext context)
        {
            _context = context;
        }

        public async Task DeleteLog(SystemLog model)
        {
            var db = await _context.SystemLogs.Where(d => d.LogId == model.LogId).FirstOrDefaultAsync();
            db.IsDel = model.IsDel;
            await _context.SaveChangesAsync();
        }
    }
}
