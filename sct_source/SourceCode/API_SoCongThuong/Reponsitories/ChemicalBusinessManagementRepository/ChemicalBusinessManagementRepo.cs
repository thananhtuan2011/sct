using EF_Core.Models;
using Microsoft.EntityFrameworkCore;

namespace API_SoCongThuong.Reponsitories.ChemicalBusinessManagementRepository
{
    public class ChemicalBusinessManagementRepo
    {
        public SoHoa_SoCongThuongContext _context;
        public ChemicalBusinessManagementRepo(SoHoa_SoCongThuongContext context)
        {
            _context = context;
        }
        public async Task Insert(ChemicalBusinessManagement model)
        {
            await _context.ChemicalBusinessManagements.AddAsync(model);
            await _context.SaveChangesAsync();
        }

        public async Task Update(ChemicalBusinessManagement model)
        {
            _context.ChemicalBusinessManagements.Update(model);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteChemicalBusinessManagement(ChemicalBusinessManagement model)
        {
            var db = await _context.ChemicalBusinessManagements.Where(d => d.ChemicalBusinessManagementId == model.ChemicalBusinessManagementId).FirstOrDefaultAsync();
            db.IsDel = model.IsDel;
            await _context.SaveChangesAsync();
        }
        public async Task Delete(Guid id)
        {
            var itemRemove = await _context.ChemicalBusinessManagements.Where(x => x.ChemicalBusinessManagementId == id).FirstOrDefaultAsync();
            _context.ChemicalBusinessManagements.Remove(itemRemove);
            await _context.SaveChangesAsync();
        }

        public IQueryable<ChemicalBusinessManagement> FindAll()
        {
            var result = _context.ChemicalBusinessManagements.Select(d => new ChemicalBusinessManagement()
            {
                ChemicalBusinessManagementId = d.ChemicalBusinessManagementId,
                BusinessName = d.BusinessName,
                Address = d.Address,
                ChemicalStorage = d.ChemicalStorage,
                Pnupschcmeasures = d.Pnupschcmeasures,
                Status = d.Status,
                IsDel = d.IsDel,
            });
            return result;
        }

        public IQueryable<ChemicalBusinessManagement> FindById(Guid Id)
        {
            var result = _context.ChemicalBusinessManagements.Where(x => x.ChemicalBusinessManagementId == Id).Select(d => new ChemicalBusinessManagement()
            {
                ChemicalBusinessManagementId = d.ChemicalBusinessManagementId,
                BusinessName = d.BusinessName,
                Address = d.Address,
                ChemicalStorage = d.ChemicalStorage,
                Pnupschcmeasures = d.Pnupschcmeasures,
                Status = d.Status,
                IsDel = d.IsDel,
                CommuneId = d.CommuneId,
                DistrictId = d.DistrictId,
                Represent = d.Represent
            });

            return result;
        }
    }
}
