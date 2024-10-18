using API_SoCongThuong.Models;
using EF_Core.Models;
using Microsoft.EntityFrameworkCore;

namespace API_SoCongThuong.Reponsitories.ElectricOperatingUnitsRepository
{
    public class ElectricOperatingUnitsRepo
    {
        public SoHoa_SoCongThuongContext _context;
        public ElectricOperatingUnitsRepo(SoHoa_SoCongThuongContext context)
        {
            _context = context;
        }

        public async Task Insert(ElectricOperatingUnit model)
        {
            await _context.ElectricOperatingUnits.AddAsync(model);
            await _context.SaveChangesAsync();
        }

        public async Task Update(ElectricOperatingUnit model)
        {
            _context.ElectricOperatingUnits.Update(model);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(ElectricOperatingUnit model)
        {
            _context.ElectricOperatingUnits.Update(model);
            await _context.SaveChangesAsync();
        }

        public IQueryable<ElectricOperatingUnitsModel> FindById(Guid Id)
        {
            var result = _context.ElectricOperatingUnits
                .Where(x => x.ElectricOperatingUnitsId == Id && !x.IsDel)
                .Select(f => new ElectricOperatingUnitsModel()
                {
                    ElectricOperatingUnitsId = f.ElectricOperatingUnitsId,
                    CustomerName = f.CustomerName,
                    Address = f.Address,
                    PhoneNumber = f.PhoneNumber,
                    PresidentName = f.PresidentName,
                    NumOfGP = f.NumOfGp,
                    SignDay = f.SignDay.ToString("dd'/'MM'/'yyyy"),
                    Supplier = f.Supplier,
                    IsPowerGeneration = f.IsPowerGeneration,
                    IsPowerDistribution = f.IsPowerDistribution,
                    IsConsulting = f.IsConsulting,
                    IsSurveillance = f.IsSurveillance,
                    IsElectricityRetail = f.IsElectricityRetail,
                    Status = f.Status
                });

            return result;
        }
    }
}
