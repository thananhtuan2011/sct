using EF_Core.Models;
using Microsoft.EntityFrameworkCore;

namespace API_SoCongThuong.Reponsitories.TypeOfTradePromotionRepository
{
    public class TypeOfTradePromotionRepo
    {
        public SoHoa_SoCongThuongContext _context;
        public TypeOfTradePromotionRepo(SoHoa_SoCongThuongContext context)
        {
            _context = context;
        }
        public async Task Insert(TypeOfTradePromotion model)
        {
            await _context.TypeOfTradePromotions.AddAsync(model);
            await _context.SaveChangesAsync();
        }

        public async Task Update(TypeOfTradePromotion model)
        {
            _context.TypeOfTradePromotions.Update(model);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteTypeOfTradePromotion(TypeOfTradePromotion model)
        {
            var db = await _context.TypeOfTradePromotions.Where(d => d.TypeOfTradePromotionId == model.TypeOfTradePromotionId).FirstOrDefaultAsync();
            db.IsDel = model.IsDel;
            await _context.SaveChangesAsync();
        }
        public async Task Delete(Guid id)
        {
            var itemRemove = await _context.TypeOfTradePromotions.Where(x => x.TypeOfTradePromotionId == id).FirstOrDefaultAsync();
            _context.TypeOfTradePromotions.Remove(itemRemove);
            await _context.SaveChangesAsync();
        }

        public IQueryable<TypeOfTradePromotion> FindAll()
        {
            var result = _context.TypeOfTradePromotions.Select(d => new TypeOfTradePromotion()
            {
                TypeOfTradePromotionId = d.TypeOfTradePromotionId,
                TypeOfTradePromotionCode = d.TypeOfTradePromotionCode,
                TypeOfTradePromotionName = d.TypeOfTradePromotionName,
                IsDel = d.IsDel,
            });

            return result;
        }

        public IQueryable<TypeOfTradePromotion> FindById(Guid Id)
        {
            var result = _context.TypeOfTradePromotions.Where(x => x.TypeOfTradePromotionId == Id).Select(d => new TypeOfTradePromotion()
            {
                TypeOfTradePromotionId = d.TypeOfTradePromotionId,
                TypeOfTradePromotionCode = d.TypeOfTradePromotionCode,
                TypeOfTradePromotionName = d.TypeOfTradePromotionName,
                IsDel = d.IsDel,
            });

            return result;
        }

        public bool findByTypeOfTradePromotionCode(string typeOfTradePromotionCode, Guid? typeOfTradePromotionId)
        {
            if (typeOfTradePromotionId != null)
            {
                var TypeOfTradePromotionCode = _context.TypeOfTradePromotions.Where(x => x.TypeOfTradePromotionId == typeOfTradePromotionId && x.TypeOfTradePromotionCode == typeOfTradePromotionCode && !x.IsDel).FirstOrDefault();
                if (TypeOfTradePromotionCode != null)
                {
                    return false;
                }
            }
            var isTypeOfTradePromotionCode = _context.TypeOfTradePromotions.Where(x => x.TypeOfTradePromotionCode == typeOfTradePromotionCode && !x.IsDel).FirstOrDefault();
            if (isTypeOfTradePromotionCode == null)
            {
                return false;
            }
            return true;
        }
    }
}
