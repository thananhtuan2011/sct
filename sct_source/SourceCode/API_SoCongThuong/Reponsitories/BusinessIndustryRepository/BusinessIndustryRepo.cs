using EF_Core.Models;
using Microsoft.EntityFrameworkCore;

namespace API_SoCongThuong.Reponsitories.BusinessIndustryRepository
{
    public class BusinessIndustryRepo
    {
        public SoHoa_SoCongThuongContext _context;
        public BusinessIndustryRepo(SoHoa_SoCongThuongContext context)
        {
            _context = context;
        }
        public async Task Insert(BusinessIndustry model)
        {
            await _context.BusinessIndustries.AddAsync(model);
            await _context.SaveChangesAsync();
        }

        public async Task InsertNew(List<BusinessIndustry> model)
        {
            await _context.BusinessIndustries.AddRangeAsync(model);
            await _context.SaveChangesAsync();
        }

        public async Task Update(BusinessIndustry model)
        {
            //var db = await _context.BusinessIndustries.Where(d => d.Id == model.Id).FirstOrDefaultAsync();
            //db.BusinessId = model.BusinessId;
            //db.IndustryId = model.IndustryId;
            //db.IsDel = model.IsDel;
            _context.BusinessIndustries.Update(model);
            await _context.SaveChangesAsync();
        }
        //public async Task DeleteBusinessIndustry(BusinessIndustry model)
        //{
        //    var db = await _context.BusinessIndustries.Where(d => d.BusinessId == model.BusinessId && d.IndustryId == model.IndustryId).FirstOrDefaultAsync();
        //    db.IsDel = model.IsDel;
        //    await _context.SaveChangesAsync();
        //}
        public async Task DeleteOld(List<BusinessIndustry> model)
        {
            _context.BusinessIndustries.RemoveRange(model);
            await _context.SaveChangesAsync();
        }

        public IQueryable<BusinessIndustry> FindAll()
        {
            var result = _context.BusinessIndustries.Select(d => new BusinessIndustry()
            {
                Id = d.Id,
                BusinessId = d.BusinessId,
                IndustryId = d.IndustryId,
            });

            return result;
        }

        public IQueryable<BusinessIndustry> FindByBusinessId(Guid Id)
        {
            var result = _context.BusinessIndustries.Where(x => x.BusinessId == Id).Select(d => new BusinessIndustry()
            {
                Id = d.Id,
                BusinessId = d.BusinessId,
                IndustryId = d.IndustryId,
            });

            return result;
        }
    }
}
