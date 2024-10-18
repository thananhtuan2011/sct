using EF_Core.Models;

namespace API_SoCongThuong.Reponsitories.StatisticalRepository
{
    public class StatisticalRepo
    {
        public SoHoa_SoCongThuongContext _context;
        public StatisticalRepo(SoHoa_SoCongThuongContext context)
        {
            _context = context;
        }
    }
}
