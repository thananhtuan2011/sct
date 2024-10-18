using EF_Core.Models;
using Microsoft.EntityFrameworkCore;

namespace API_SoCongThuong.Reponsitories.CommuneElectricityManagementRepository
{
    public class CommuneElectricityManagementRepo
    {
        public SoHoa_SoCongThuongContext _context;
        public CommuneElectricityManagementRepo(SoHoa_SoCongThuongContext context)
        {
            _context = context;
        }

        public async Task Insert(CommuneElectricityManagement model)
        {
            await _context.CommuneElectricityManagements.AddAsync(model);
            await _context.SaveChangesAsync();
        }

        public async Task Update(CommuneElectricityManagement model)
        {
            _context.CommuneElectricityManagements.Update(model);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(CommuneElectricityManagement model)
        {
            _context.CommuneElectricityManagements.Update(model);
            await _context.SaveChangesAsync();
        }

        public IQueryable<CommuneElectricityManagement> FindById(Guid Id)
        {
            var result = _context.CommuneElectricityManagements
                .Where(x => x.CommuneElectricityManagementId == Id && !x.IsDel)
                .Select(f => new CommuneElectricityManagement()
                {
                    CommuneElectricityManagementId = f.CommuneElectricityManagementId,
                    StageId = f.StageId,
                    DistrictId = f.DistrictId,
                    CommuneId = f.CommuneId,
                    Content41Start = f.Content41Start,
                    Content42Start = f.Content42Start,
                    Target4Start = f.Target4Start,
                    Content41End = f.Content41End,
                    Content42End = f.Content42End,
                    Target4End = f.Target4End,
                    Note = f.Note,
                });

            return result;
        }
    }
}
