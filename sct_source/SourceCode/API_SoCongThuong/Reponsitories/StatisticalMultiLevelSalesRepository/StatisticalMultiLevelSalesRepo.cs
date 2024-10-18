using EF_Core.Models;

namespace API_SoCongThuong.Reponsitories.StatisticalMultiLevelSalesRepository
{
    public class StatisticalMultiLevelSalesRepo
    {
        public SoHoa_SoCongThuongContext _context;
        public StatisticalMultiLevelSalesRepo(SoHoa_SoCongThuongContext context)
        {
            _context = context;
        }
    }
}
