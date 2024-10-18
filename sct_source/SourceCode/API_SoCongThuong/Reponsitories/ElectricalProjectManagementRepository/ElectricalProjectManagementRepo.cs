using API_SoCongThuong.Models;
using EF_Core.Models;
using Microsoft.EntityFrameworkCore;

namespace API_SoCongThuong.Reponsitories.CountryRepository
{
    public class ElectricalProjectManagementRepo
    {
        public SoHoa_SoCongThuongContext _context;
        public ElectricalProjectManagementRepo(SoHoa_SoCongThuongContext context)
        {
            _context = context;
        }
        public async Task Insert(ElectricalProjectManagement model)
        {
            await _context.ElectricalProjectManagements.AddAsync(model);
            await _context.SaveChangesAsync();
        }

        public async Task Update(ElectricalProjectManagement model)
        {
            _context.ElectricalProjectManagements.Update(model);
            await _context.SaveChangesAsync();
        }
        public async Task Delete(ElectricalProjectManagement model)
        {
            _context.ElectricalProjectManagements.Update(model);
            await _context.SaveChangesAsync();
        }

        public IQueryable<ElectricalProjectManagementModel> FindById(Guid Id)
        {
            var result = _context.ElectricalProjectManagements.Where(x => x.ElectricalProjectManagementId == Id && !x.IsDel)
                .Join(_context.Categories, x => x.TypeOfConstruction , cate => cate.CategoryId , (x, cate) => new ElectricalProjectManagementModel
                {
                ElectricalProjectManagementId = x.ElectricalProjectManagementId,
                BuildingCode = x.BuildingCode,
                BuildingName = x.BuildingName,
                District = x.District,
                Address = x.Address,
                Represent = x.Represent,
                Status = x.Status,
                Note = x.Note,
                VoltageLevel = x.VoltageLevel,
                TypeOfConstruction = x.TypeOfConstruction,
                Wattage = x.Wattage,
                Length = x.Length,
                WireType = x.WireType,
                TypeOfConstructionCode = cate.CategoryCode
                });

            return result;
        }
    }
}
