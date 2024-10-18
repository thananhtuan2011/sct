using EF_Core.Models;

namespace API_SoCongThuong.Reponsitories.CommonRepository
{
    public class CommonRepo
    {
        public SoHoa_SoCongThuongContext _context;
        public CommonRepo(SoHoa_SoCongThuongContext context)
        {
            _context = context;
        }
    }
}
