using API_SoCongThuong.Models;
using EF_Core.Models;
using Microsoft.EntityFrameworkCore;

namespace API_SoCongThuong.Reponsitories
{
    public class TotalRetailSaleRepo
    {
        public SoHoa_SoCongThuongContext _context;
        public TotalRetailSaleRepo(SoHoa_SoCongThuongContext context)
        {
            _context = context;
        }

        public async Task Insert(TotalRetailSale model)
        {
            await _context.TotalRetailSales.AddAsync(model);
            await _context.SaveChangesAsync();
        }

        public async Task Update(TotalRetailSale model)
        {
            _context.TotalRetailSales.Update(model);
            await _context.SaveChangesAsync();
        }

        public IQueryable<TotalRetailSaleModel> FindById(Guid Id)
        {
            var result = _context.TotalRetailSales.Where(x => !x.IsDel && x.TotalRetailSaleId == Id).Select(x => new TotalRetailSaleModel
            {
                TotalRetailSaleId = x.TotalRetailSaleId,
                Target = x.Target,
            });
            return result;
        }

        public async Task Delete(TotalRetailSale model)
        {
            var db = await _context.TotalRetailSales.Where(x => x.TotalRetailSaleId == model.TotalRetailSaleId).FirstOrDefaultAsync();
            if(db != null)
            {
                db.IsDel = model.IsDel;
            }
            await _context.SaveChangesAsync();
        }
    }
}
