using EF_Core.Models;
using Microsoft.EntityFrameworkCore;

namespace API_SoCongThuong.Reponsitories
{
    public class MarketInvestEnterpriseRepo
    {
        public SoHoa_SoCongThuongContext _context;
        public MarketInvestEnterpriseRepo(SoHoa_SoCongThuongContext context)
        {
            _context = context;
        }
        public async Task Insert(MarketInvestEnterprise model)
        {
            await _context.MarketInvestEnterprises.AddAsync(model);
            await _context.SaveChangesAsync();
        }

        public async Task Update(MarketInvestEnterprise model)
        {
            _context.MarketInvestEnterprises.Update(model);
            await _context.SaveChangesAsync();
        }
        public async Task Delete(MarketInvestEnterprise model)
        {
            var db = await _context.MarketInvestEnterprises.Where(d => d.MarketInvestEnterpriseId == model.MarketInvestEnterpriseId).FirstOrDefaultAsync();
            db.IsDel = model.IsDel;
            await _context.SaveChangesAsync();
        }

        public IQueryable<MarketInvestEnterprise> FindAll()
        {
            var result = _context.MarketInvestEnterprises.Select(d => new MarketInvestEnterprise()
            {
                Note = d.Note,
                IsDel = d.IsDel,
            });

            return result;
        }

        public IQueryable<MarketInvestEnterprise> FindById(Guid Id)
        {
            var result = _context.MarketInvestEnterprises.Where(x => x.MarketInvestEnterpriseId == Id).Select(d => new MarketInvestEnterprise()
            {
                MarketInvestEnterpriseId = d.MarketInvestEnterpriseId,
                MarketName = d.MarketName,
                DistrictId = d.DistrictId,
                CommuneId = d.CommuneId,
                Address = d.Address,
                Investment = d.Investment,
                BusinessName = d.BusinessName,
                Capital = d.Capital,
                Evaluate = d.Evaluate,
                Note = d.Note
        });
            return result;
        }
    }
}
