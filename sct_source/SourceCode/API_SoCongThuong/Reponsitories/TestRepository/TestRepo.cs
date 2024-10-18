using EF_Core.Models;
using Microsoft.EntityFrameworkCore;

namespace API_SoCongThuong.Reponsitories.TestRepository
{
    public class TestRepo
    {
        public SoHoa_SoCongThuongContext _context;
        public TestRepo(SoHoa_SoCongThuongContext context)
        {
            _context = context;
        }
        public async Task Insert(TblTest model)
        {
            await _context.TblTests.AddAsync(model);
            await _context.SaveChangesAsync();
        }

        public async Task Update(TblTest model)
        {
            var db = await _context.TblTests.Where(d => d.Id == model.Id).FirstOrDefaultAsync();
            db.Name = model.Name;
            await _context.SaveChangesAsync();
        }

        public async Task Delete(long id)
        {
            var itemRemove = await _context.TblTests.Where(x => x.Id == id).FirstOrDefaultAsync();
            _context.TblTests.Remove(itemRemove);
            await _context.SaveChangesAsync();
        }

        public IQueryable<TblTest> FindAll()
        {
            var result = _context.TblTests.Select(d => new TblTest()
            {
                Id = d.Id,
                Name = d.Name,
            });

            return result;
        }

        public IQueryable<TblTest> FindById(long Id)
        {
            var result = _context.TblTests.Where(x =>x.Id == Id).Select(d => new TblTest()
            {
                Id = d.Id,
                Name = d.Name,
            });

            return result;
        }
    }
}
