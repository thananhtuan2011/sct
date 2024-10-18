using EF_Core.Models;
using Microsoft.EntityFrameworkCore;

namespace API_SoCongThuong.Reponsitories.CountryRepository
{
    public class CountryRepo
    {
        public SoHoa_SoCongThuongContext _context;
        public CountryRepo(SoHoa_SoCongThuongContext context)
        {
            _context = context;
        }
        public async Task Insert(Country model)
        {
            await _context.Countries.AddAsync(model);
            await _context.SaveChangesAsync();
        }

        public async Task Update(Country model)
        {
            _context.Countries.Update(model);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteCountry(Country model)
        {
            var db = await _context.Countries.Where(d => d.CountryId == model.CountryId).FirstOrDefaultAsync();
            db.IsDel = model.IsDel;
            await _context.SaveChangesAsync();
        }
        public async Task Delete(Guid id)
        {
            var itemRemove = await _context.Countries.Where(x => x.CountryId == id).FirstOrDefaultAsync();
            _context.Countries.Remove(itemRemove);
            await _context.SaveChangesAsync();
        }

        public IQueryable<Country> FindAll()
        {
            var result = _context.Countries.Select(d => new Country()
            {
                CountryId = d.CountryId,
                CountryCode = d.CountryCode,
                CountryName = d.CountryName,
                IsDel = d.IsDel,
            });

            return result;
        }

        public IQueryable<Country> FindById(Guid Id)
        {
            var result = _context.Countries.Where(x => x.CountryId == Id).Select(d => new Country()
            {
                CountryId = d.CountryId,
                CountryCode = d.CountryCode,
                CountryName = d.CountryName,
                IsDel = d.IsDel,
            });

            return result;
        }

        public bool findByCountryCode(string countryCode, Guid? countryId)
        {
            if (countryId != null)
            {
                var CountryCode = _context.Countries.Where(x => x.CountryId == countryId && x.CountryCode == countryCode && !x.IsDel).FirstOrDefault();
                if (CountryCode != null)
                {
                    return false;
                }
            }
            var isCountryCode = _context.Countries.Where(x => x.CountryCode == countryCode && !x.IsDel).FirstOrDefault();
            if (isCountryCode == null)
            {
                return false;
            }
            return true;
        }
    }
}
