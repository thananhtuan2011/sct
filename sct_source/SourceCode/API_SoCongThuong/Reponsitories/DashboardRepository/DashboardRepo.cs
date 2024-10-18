using EF_Core.Models;

namespace API_SoCongThuong.Reponsitories.DashboardRepository
{
    public class DashboardRepo
    {
        public SoHoa_SoCongThuongContext _context;
        public DashboardRepo(SoHoa_SoCongThuongContext context)
        {
            _context = context;
        }
    }
}
