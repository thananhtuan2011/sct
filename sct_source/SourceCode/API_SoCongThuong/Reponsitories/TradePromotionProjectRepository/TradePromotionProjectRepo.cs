using EF_Core.Models;
using Microsoft.EntityFrameworkCore;

namespace API_SoCongThuong.Reponsitories.TradePromotionProjectRepository
{
    public class TradePromotionProjectRepo
    {
        public SoHoa_SoCongThuongContext _context;
        public TradePromotionProjectRepo(SoHoa_SoCongThuongContext context)
        {
            _context = context;
        }
        public async Task Insert(TradePromotionProject model)
        {
            await _context.TradePromotionProjects.AddAsync(model);
            await _context.SaveChangesAsync();
        }

        public async Task Update(TradePromotionProject model)
        {
            _context.TradePromotionProjects.Update(model);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteTradePromotionProject(TradePromotionProject model)
        {
            var db = await _context.TradePromotionProjects.Where(d => d.TradePromotionProjectId == model.TradePromotionProjectId).FirstOrDefaultAsync();
            db.IsDel = model.IsDel;
            await _context.SaveChangesAsync();
        }
        public async Task Delete(Guid id)
        {
            var itemRemove = await _context.TradePromotionProjects.Where(x => x.TradePromotionProjectId == id).FirstOrDefaultAsync();
            _context.TradePromotionProjects.Remove(itemRemove);
            await _context.SaveChangesAsync();
        }

        public IQueryable<TradePromotionProject> FindAll()
        {
            var result = _context.TradePromotionProjects.Select(d => new TradePromotionProject()
            {
                TradePromotionProjectId = d.TradePromotionProjectId,
                TradePromotionProjectCode = d.TradePromotionProjectCode,
                TradePromotionProjectName = d.TradePromotionProjectName,
                IsDel = d.IsDel,
            });

            return result;
        }

        public IQueryable<TradePromotionProject> FindById(Guid Id)
        {
            var result = _context.TradePromotionProjects.Where(x => x.TradePromotionProjectId == Id).Select(d => new TradePromotionProject()
            {
                TradePromotionProjectId = d.TradePromotionProjectId,
                TradePromotionProjectCode = d.TradePromotionProjectCode,
                TradePromotionProjectName = d.TradePromotionProjectName,
                IsDel = d.IsDel,
            });

            return result;
        }

        public bool findByTradePromotionProjectCode(string tradePromotionProjectCode, Guid? tradePromotionProjectId)
        {
            if (tradePromotionProjectId != null)
            {
                var TradePromotionProjectCode = _context.TradePromotionProjects.Where(x => x.TradePromotionProjectId == tradePromotionProjectId && x.TradePromotionProjectCode == tradePromotionProjectCode && !x.IsDel).FirstOrDefault();
                if (TradePromotionProjectCode != null)
                {
                    return false;
                }
            }
            var isTradePromotionProjectCode = _context.TradePromotionProjects.Where(x => x.TradePromotionProjectCode == tradePromotionProjectCode && !x.IsDel).FirstOrDefault();
            if (isTradePromotionProjectCode == null)
            {
                return false;
            }
            return true;
        }
    }
}
