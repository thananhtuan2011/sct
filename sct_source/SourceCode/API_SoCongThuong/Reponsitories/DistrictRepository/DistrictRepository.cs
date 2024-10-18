using EF_Core.Models;
using Microsoft.EntityFrameworkCore;
using static System.Net.Mime.MediaTypeNames;

namespace API_SoCongThuong.Reponsitories.DistrictRepository
{
    public class DistrictRepository
    {
        public SoHoa_SoCongThuongContext _context;
        public DistrictRepository(SoHoa_SoCongThuongContext context)
        {
            _context = context;
        }
        public async Task Insert(District model)
        {
            await _context.Districts.AddAsync(model);
            await _context.SaveChangesAsync();
        }

        public async Task Update(District model)
        {
            _context.Districts.Update(model);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteDistrict(District model)
        {
            var db = await _context.Districts.Where(d => d.DistrictId == model.DistrictId).FirstOrDefaultAsync();
            db.IsDel = model.IsDel;
            await _context.SaveChangesAsync();
        }
        public async Task Delete(Guid id)
        {
            var itemRemove = await _context.Districts.Where(x => x.DistrictId == id).FirstOrDefaultAsync();
            _context.Districts.Remove(itemRemove);
            await _context.SaveChangesAsync();
        }

        public IQueryable<District> FindAll()
        {
            var result = _context.Districts.Select(d => new District()
            {
                DistrictId = d.DistrictId,
                DistrictCode = d.DistrictCode,
                DistrictName = d.DistrictName,
                CommuneNumber = d.CommuneNumber,
                IsDel = d.IsDel,
            });

            return result;
        }

        public IQueryable<District> FindById(Guid Id)
        {
            var result = _context.Districts.Where(x =>x.DistrictId == Id).Select(d => new District()
            {
                DistrictId = d.DistrictId,
                DistrictCode = d.DistrictCode,
                DistrictName = d.DistrictName,
                CommuneNumber = d.CommuneNumber,
                IsDel = d.IsDel,
            });

            return result;
        }

        public bool findByDistrictCode (string districtCode, Guid? districtId )
        {
            if (districtId != null)
            {
                var DistrictCode = _context.Districts.Where(x => x.DistrictId == districtId && x.DistrictCode == districtCode && !x.IsDel).FirstOrDefault();
                if (DistrictCode != null)
                {
                    return false;
                }
            }
            var isDistrictCode = _context.Districts.Where(x => x.DistrictCode == districtCode && !x.IsDel).FirstOrDefault();
            if (isDistrictCode == null)
            {
                return false;
            }
            return true;
        }
    }
}
