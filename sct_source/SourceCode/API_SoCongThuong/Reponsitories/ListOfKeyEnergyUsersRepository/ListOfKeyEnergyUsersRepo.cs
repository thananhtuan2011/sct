using API_SoCongThuong.Models;
using EF_Core.Models;
using Microsoft.EntityFrameworkCore;

namespace API_SoCongThuong.Reponsitories.CountryRepository
{
    public class ListOfKeyEnergyUsersRepo
    {
        public SoHoa_SoCongThuongContext _context;
        public ListOfKeyEnergyUsersRepo(SoHoa_SoCongThuongContext context)
        {
            _context = context;
        }
        public async Task Insert(ListOfKeyEnergyUser model)
        {
            await _context.ListOfKeyEnergyUsers.AddAsync(model);
            await _context.SaveChangesAsync();
        }

        public async Task Update(ListOfKeyEnergyUser model)
        {
            _context.ListOfKeyEnergyUsers.Update(model);
            await _context.SaveChangesAsync();
        }
        public async Task Delete(ListOfKeyEnergyUser model)
        {
            _context.ListOfKeyEnergyUsers.Update(model);
            await _context.SaveChangesAsync();
        }

        public IQueryable<ListOfKeyEnergyUsersModel> FindById(Guid Id)
        {
            var result = _context.ListOfKeyEnergyUsers.Where(x => x.ListOfKeyEnergyUsersId == Id && !x.IsDel).Select(f => new ListOfKeyEnergyUsersModel()
            {
                ListOfKeyEnergyUsersId = f.ListOfKeyEnergyUsersId,
                BusinessId = f.BusinessId,
                Address = f.Address,
                Date = f.Date,
                Link = f.Link ?? "",
                Profession = f.Profession,
                ManufactProfession = f.ManufactProfession ?? "",
                Note = f.Note ?? "",
                EnergyConsumption = f.EnergyConsumption,
                Decision = f.Decision ?? ""
            });

            return result;
        }
    }
}
