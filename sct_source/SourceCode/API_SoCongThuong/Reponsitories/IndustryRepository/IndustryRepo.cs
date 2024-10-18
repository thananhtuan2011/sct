using EF_Core.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Immutable;
using static System.Net.Mime.MediaTypeNames;

namespace API_SoCongThuong.Reponsitories.IndustryRepository
{
    public class IndustryRepo
    {
        public SoHoa_SoCongThuongContext _context;
        public IndustryRepo(SoHoa_SoCongThuongContext context)
        {
            _context = context;
        }
        public async Task Insert(Industry model)
        {
            await _context.Industries.AddAsync(model);
            await _context.SaveChangesAsync();
        }

        public async Task Update(Industry model)
        {
            _context.Industries.Update(model);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(Guid IndustryId)
        {
            var itemRemove = await _context.Industries.Where(x => x.IndustryId == IndustryId).FirstOrDefaultAsync();
            _context.Industries.Remove(itemRemove);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteIndustry(Guid IndustryId)
        {
            var itemRemove = await _context.Industries.Where(x => x.IndustryId == IndustryId).FirstOrDefaultAsync();
            itemRemove.IsDel = true;
            _context.Industries.Update(itemRemove);
            await _context.SaveChangesAsync();
        }
        public IQueryable<Industry> FindAll()
        {
            var result = _context.Industries.Select(d => new Industry()
            {
                IndustryId = d.IndustryId,
                IndustryCode = d.IndustryCode,
                IndustryName = d.IndustryName,
                ParentIndustryId = d.ParentIndustryId,
                IsDel = d.IsDel
            }).OrderBy(x => x.IndustryCode);

            return result;
        }

        public IQueryable<Industry> FindByIndustryId(Guid IndustryId)
        {
            var result = _context.Industries.Where(x => x.IndustryId == IndustryId).Select(d => new Industry()
            {
                IndustryId = d.IndustryId,
                IndustryCode=d.IndustryCode,
                IndustryName = d.IndustryName,
                ParentIndustryId = d.ParentIndustryId ?? Guid.Empty,
                ///IndustryLevel =d.IndustryLevel,
                IsDel=d.IsDel
            });

            return result;
        }

        public bool findByIndustryCode(string industryCode, Guid? industryId)
        {
            if (industryId != null)
            {
                var IndustryCode = _context.Industries.Where(x => x.IndustryId == industryId && x.IndustryCode == industryCode && !x.IsDel).FirstOrDefault();
                if (IndustryCode != null)
                {
                    return false;
                }
            }
            var isIndustryCode = _context.Industries.Where(x => x.IndustryCode == industryCode && !x.IsDel).FirstOrDefault();
            if (isIndustryCode == null)
            {
                return false;
            }
            return true;
        }
    }
}
