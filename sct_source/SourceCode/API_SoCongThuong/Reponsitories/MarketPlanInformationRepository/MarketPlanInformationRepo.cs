using EF_Core.Models;
using Microsoft.EntityFrameworkCore;

namespace API_SoCongThuong.Reponsitories
{
    public class MarketPlanInformationRepo
    {
        public SoHoa_SoCongThuongContext _context;
        public MarketPlanInformationRepo(SoHoa_SoCongThuongContext context)
        {
            _context = context;
        }
        public async Task Insert(MarketPlanInformation model)
        {
            await _context.MarketPlanInformations.AddAsync(model);
            await _context.SaveChangesAsync();
        }

        public async Task Update(MarketPlanInformation model)
        {
            _context.MarketPlanInformations.Update(model);
            await _context.SaveChangesAsync();
        }
        public async Task Delete(MarketPlanInformation model)
        {
            var db = await _context.MarketPlanInformations.Where(d => d.MarketPlanInformationId == model.MarketPlanInformationId).FirstOrDefaultAsync();
            db.IsDel = model.IsDel;
            await _context.SaveChangesAsync();
        }

        public IQueryable<MarketPlanInformation> FindAll()
        {
            var result = _context.MarketPlanInformations.Select(d => new MarketPlanInformation()
            {
                Note = d.Note,
                IsDel = d.IsDel,
            });

            return result;
        }

        public IQueryable<MarketPlanInformation> FindById(Guid Id)
        {
            var result = _context.MarketPlanInformations.Where(x => x.MarketPlanInformationId == Id).Select(d => new MarketPlanInformation()
            {
                MarketPlanInformationId = d.MarketPlanInformationId,
                MarketName = d.MarketName,
                DistrictId = d.DistrictId,
                CommuneId = d.CommuneId,
                Address = d.Address,
                Year = d.Year,
                LandArea = d.LandArea,
                BusinessLandArea = d.BusinessLandArea,
                ConstructionProperty = d.ConstructionProperty,
                ConstructionNeed = d.ConstructionNeed,
                Note = d.Note
        });
            return result;
        }
    }
}
