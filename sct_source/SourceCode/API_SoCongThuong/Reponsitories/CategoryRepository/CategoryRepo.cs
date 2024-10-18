using API_SoCongThuong.Models;
using EF_Core.Models;
using Microsoft.EntityFrameworkCore;

namespace API_SoCongThuong.Reponsitories.CategoryRepository
{
    public class CategoryRepo
    {
        public SoHoa_SoCongThuongContext _context;
        public CategoryRepo(SoHoa_SoCongThuongContext context)
        {
            _context = context;
        }

        public IQueryable<Category> FindAll()
        {
            var result = _context.Categories.Where(x => !x.IsDel).Select(d => new Category()
            {
                CategoryId = d.CategoryId,
                CategoryCode = d.CategoryCode,
                CategoryName = d.CategoryName,
                CategoryTypeCode = d.CategoryTypeCode,
                Piority = d.Piority,
                IsAction = d.IsAction,
            });

            return result;
        }

        public IQueryable<ConfigGroupModel> FindById(Guid Id)
        {
            var result = _context.CategoryTypes.Where(x => x.CategoryTypeId == Id && !x.IsDel).Select(d => new ConfigGroupModel()
            {
                CategoryTypeId = d.CategoryTypeId,
                CategoryTypeCode = d.CategoryTypeCode,
                CategoryTypeName = d.CategoryTypeName,
                Description = d.Description,
            });

            return result;
        }

        public async Task Insert(CategoryType model)
        {
            await _context.CategoryTypes.AddAsync(model);
            await _context.SaveChangesAsync();
        }

        public async Task Update(CategoryType model, string OldCode)
        {
            _context.CategoryTypes.Update(model);
            await _context.SaveChangesAsync();

            List<Category> LChild = _context.Categories.Where(x => x.CategoryTypeCode == OldCode).ToList();
            if (LChild.Count > 0)
            {
                foreach (var lChild in LChild)
                {
                    lChild.CategoryTypeCode = model.CategoryTypeCode;
                }
                _context.Categories.UpdateRange(LChild);
                await _context.SaveChangesAsync();
            }
        }

        public async Task Delete(CategoryType model)
        {
            var db = await _context.CategoryTypes.Where(d => d.CategoryTypeId == model.CategoryTypeId).FirstOrDefaultAsync();
            if (db != null)
            {
                db.IsDel = model.IsDel;
            }
            await _context.SaveChangesAsync();
        }

        public IQueryable<ConfigModel> FindConfigById(string TypeCode)
        {
            var result = _context.Categories
                .Where(x => x.CategoryTypeCode == TypeCode && !x.IsDel)
                .Select(f => new ConfigModel()
                {
                    CategoryId = f.CategoryId,
                    CategoryTypeCode = f.CategoryTypeCode,
                    CategoryCode = f.CategoryCode,
                    CategoryName = f.CategoryName,
                    Priority = f.Piority,
                    IsAction = f.IsAction ?? false,
                })
                .OrderBy(x => x.Priority);

            return result;
        }

        public async Task InsertListConfig(List<Category> model)
        {
            await _context.Categories.AddRangeAsync(model);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateListConfig(List<Category> model)
        {
            _context.Categories.UpdateRange(model);
            await _context.SaveChangesAsync();
        }
    }
}
