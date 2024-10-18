using API_SoCongThuong.Models;
using EF_Core.Models;
using Microsoft.EntityFrameworkCore;

namespace API_SoCongThuong.Reponsitories.AdministrativeProcedureFieldRepository
{
    public class AdministrativeProcedureFieldRepo
    {
        public SoHoa_SoCongThuongContext _context;
        public AdministrativeProcedureFieldRepo(SoHoa_SoCongThuongContext context)
        {
            _context = context;
        }
        public async Task Insert(Category model)
        {
            await _context.Categories.AddAsync(model);
            await _context.SaveChangesAsync();
        }

        public async Task Update(Category model)
        {
            _context.Categories.Update(model);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(Category model)
        {
            var db = await _context.Categories.Where(d => d.CategoryId == model.CategoryId).FirstOrDefaultAsync();
            if (db != null)
            {
                db.IsAction = model.IsAction;
                await _context.SaveChangesAsync();
            }
        }

        public IQueryable<Category> FindAll()
        {
            var result = _context.Categories.Select(f => new Category()
            {
                CategoryId = f.CategoryId,
                CategoryCode = f.CategoryCode,
                CategoryName = f.CategoryName,
                IsAction = f.IsAction,
                Piority = f.Piority,
            });

            return result;
        }

        public IQueryable<AdministrativeProcedureFieldModel> FindById(Guid Id)
        {
            var result = _context.Categories.Where(x => x.CategoryId == Id).Select(d => new AdministrativeProcedureFieldModel()
            {
                CategoryId = d.CategoryId,
                CategoryCode = d.CategoryCode,
                CategoryName = d.CategoryName,
                IsAction = d.IsAction ?? false,
                Piority = d.Piority,
            });

            return result;
        }
    }
}
