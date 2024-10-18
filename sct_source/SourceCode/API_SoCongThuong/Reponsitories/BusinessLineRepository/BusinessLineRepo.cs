using EF_Core.Models;
using Microsoft.EntityFrameworkCore;

namespace API_SoCongThuong.Reponsitories.BusinessLineRepository
{
    public class BusinessLineRepo
    {
        public SoHoa_SoCongThuongContext _context;
        public BusinessLineRepo(SoHoa_SoCongThuongContext context)
        {
            _context = context;
        }
        public async Task Insert(BusinessLine model)
        {
            await _context.BusinessLines.AddAsync(model);
            await _context.SaveChangesAsync();
        }

        public async Task Update(BusinessLine model)
        {
            var db = await _context.BusinessLines.Where(d => d.BusinessLineId == model.BusinessLineId).FirstOrDefaultAsync();
            db.BusinessLineName = model.BusinessLineName;
            db.BusinessLineCode = model.BusinessLineCode;
            db.IsDel = model.IsDel;
            await _context.SaveChangesAsync();
        }
        public async Task DeleteBusinessLine(BusinessLine model)
        {
            var db = await _context.BusinessLines.Where(d => d.BusinessLineId == model.BusinessLineId).FirstOrDefaultAsync();
            db.IsDel = model.IsDel;
            await _context.SaveChangesAsync();
        }
        public async Task Delete(Guid id)
        {
            var itemRemove = await _context.BusinessLines.Where(x => x.BusinessLineId == id).FirstOrDefaultAsync();
            _context.BusinessLines.Remove(itemRemove);
            await _context.SaveChangesAsync();
        }

        public IQueryable<BusinessLine> FindAll()
        {
            var result = _context.BusinessLines.Select(d => new BusinessLine()
            {
                BusinessLineId = d.BusinessLineId,
                BusinessLineCode = d.BusinessLineCode,
                BusinessLineName = d.BusinessLineName,
                IsDel = d.IsDel,
            });

            return result;
        }

        public IQueryable<BusinessLine> FindById(Guid Id)
        {
            var result = _context.BusinessLines.Where(x => x.BusinessLineId == Id).Select(d => new BusinessLine()
            {
                BusinessLineId = d.BusinessLineId,
                BusinessLineCode = d.BusinessLineCode,
                BusinessLineName = d.BusinessLineName,
                IsDel = d.IsDel,
            });

            return result;
        }

        public bool findByBusinessLineCode(string typeOfEnergyCode, Guid? typeOfEnergyId)
        {
            if (typeOfEnergyId != null)
            {
                var BusinessLineCode = _context.BusinessLines.Where(x => x.BusinessLineId == typeOfEnergyId && x.BusinessLineCode == typeOfEnergyCode && !x.IsDel).FirstOrDefault();
                if (BusinessLineCode != null)
                {
                    return false;
                }
            }
            var isBusinessLineCode = _context.BusinessLines.Where(x => x.BusinessLineCode == typeOfEnergyCode && !x.IsDel).FirstOrDefault();
            if (isBusinessLineCode == null)
            {
                return false;
            }
            return true;
        }
    }
}
