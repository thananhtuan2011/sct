using EF_Core.Models;
using Microsoft.EntityFrameworkCore;

namespace API_SoCongThuong.Reponsitories.TypeOfMarketRepository
{
    public class TypeOfMarketRepo
    {
        public SoHoa_SoCongThuongContext _context;
        public TypeOfMarketRepo(SoHoa_SoCongThuongContext context)
        {
            _context = context;
        }
        public async Task Insert(TypeOfMarket model)
        {
            await _context.TypeOfMarkets.AddAsync(model);
            await _context.SaveChangesAsync();
        }

        public async Task Update(TypeOfMarket model)
        {
            _context.Update(model);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteTypeOfMarket(TypeOfMarket model)
        {
            var db = await _context.TypeOfMarkets.Where(d => d.TypeOfMarketId == model.TypeOfMarketId).FirstOrDefaultAsync();
            db.IsDel = model.IsDel;
            await _context.SaveChangesAsync();
        }
        public async Task Delete(Guid id)
        {
            var itemRemove = await _context.TypeOfMarkets.Where(x => x.TypeOfMarketId == id).FirstOrDefaultAsync();
            _context.TypeOfMarkets.Remove(itemRemove);
            await _context.SaveChangesAsync();
        }

        public IQueryable<TypeOfMarket> FindAll()
        {
            var result = _context.TypeOfMarkets.Select(d => new TypeOfMarket()
            {
                TypeOfMarketId = d.TypeOfMarketId,
                TypeOfMarketCode = d.TypeOfMarketCode,
                TypeOfMarketName = d.TypeOfMarketName,
                IsDel = d.IsDel,
            });

            return result;
        }

        public IQueryable<TypeOfMarket> FindById(Guid Id)
        {
            var result = _context.TypeOfMarkets.Where(x => x.TypeOfMarketId == Id).Select(d => new TypeOfMarket()
            {
                TypeOfMarketId = d.TypeOfMarketId,
                TypeOfMarketCode = d.TypeOfMarketCode,
                TypeOfMarketName = d.TypeOfMarketName,
                IsDel = d.IsDel,
            });

            return result;
        }

        public bool findByTypeOfMarketCode(string typeOfMarketCode, Guid? typeOfMarketId)
        {
            if (typeOfMarketId != null)
            {
                var TypeOfMarketCode = _context.TypeOfMarkets.Where(x => x.TypeOfMarketId == typeOfMarketId && x.TypeOfMarketCode == typeOfMarketCode && !x.IsDel).FirstOrDefault();
                if (TypeOfMarketCode != null)
                {
                    return false;
                }
            }
            var isTypeOfMarketCode = _context.TypeOfMarkets.Where(x => x.TypeOfMarketCode == typeOfMarketCode && !x.IsDel).FirstOrDefault();
            if (isTypeOfMarketCode == null)
            {
                return false;
            }
            return true;
        }
    }
}
