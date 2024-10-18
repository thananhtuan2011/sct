using EF_Core.Models;
using Microsoft.EntityFrameworkCore;

namespace API_SoCongThuong.Reponsitories.CommercialManagementRepository
{
    public class CommercialManagementRepo
    {
        public SoHoa_SoCongThuongContext _context;
        public CommercialManagementRepo(SoHoa_SoCongThuongContext context)
        {
            _context = context;
        }
        public async Task Insert(CommercialManagement model)
        {
            await _context.CommercialManagements.AddAsync(model);
            await _context.SaveChangesAsync();
        }

        public async Task Update(CommercialManagement model)
        {
            _context.CommercialManagements.Update(model);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteCommercialManagement(CommercialManagement model)
        {
            var db = await _context.CommercialManagements.Where(d => d.CommercialId == model.CommercialId).FirstOrDefaultAsync();
            db.IsDel = model.IsDel;
            await _context.SaveChangesAsync();
        }
        public async Task Delete(Guid id)
        {
            var itemRemove = await _context.CommercialManagements.Where(x => x.CommercialId == id).FirstOrDefaultAsync();
            _context.CommercialManagements.Remove(itemRemove);
            await _context.SaveChangesAsync();
        }

        public IQueryable<CommercialManagement> FindAll()
        {
            var result = _context.CommercialManagements.Select(d => new CommercialManagement()
            {
                CommercialId = d.CommercialId,
                Type = d.Type,
                Code = d.Code,
                Name = d.Name,
                TypeOfMarketId = d.TypeOfMarketId,
                DistrictId = d.DistrictId,
                CommuneId = d.CommuneId,
                Address = d.Address,
                RankId = d.RankId,
                ConstructiveNatureId = d.ConstructiveNatureId,
                BusinessNatureId = d.BusinessNatureId,
                TypeOfEconomic = d.TypeOfEconomic,
                ManagementFormId = d.ManagementFormId,
                ManagementObjectId = d.ManagementObjectId,
                PhoneNumber = d.PhoneNumber,
                Email = d.Email,
                Fax = d.Fax,
                Note = d.Note,
                IsDel = d.IsDel,
            });

            return result;
        }

        public IQueryable<CommercialManagement> FindById(Guid Id)
        {
            var result = _context.CommercialManagements.Where(x => x.CommercialId == Id).Select(d => new CommercialManagement()
            {
                CommercialId = d.CommercialId,
                Type = d.Type,
                Code = d.Code,
                Name = d.Name,
                TypeOfMarketId = d.TypeOfMarketId ?? Guid.Empty,
                DistrictId = d.DistrictId,
                CommuneId = d.CommuneId,
                Address = d.Address,
                RankId = d.RankId ?? Guid.Empty,
                ConstructiveNatureId = d.ConstructiveNatureId ?? Guid.Empty,
                BusinessNatureId = d.BusinessNatureId ?? Guid.Empty,
                TypeOfEconomic = d.TypeOfEconomic ?? Guid.Empty,
                ManagementFormId = d.ManagementFormId ?? Guid.Empty,
                ManagementObjectId = d.ManagementObjectId ?? Guid.Empty,
                PhoneNumber = d.PhoneNumber,
                Email = d.Email,
                Fax = d.Fax,
                Note = d.Note,
                IsDel = d.IsDel,
                TypeOfMarket = d.TypeOfMarket,
                TypeOfCenterLogistic = d.TypeOfCenterLogistic,
                FormMarket = d.FormMarket,
                Form = d.Form,
                Area = d.Area,
                MarketCleared = d.MarketCleared,
        });

            return result;
        }
    }
}
