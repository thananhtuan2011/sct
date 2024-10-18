using EF_Core.Models;
using Microsoft.EntityFrameworkCore;
namespace API_SoCongThuong.Reponsitories.PermissionRepository
{
    public class PermissionRepo
    {
        public SoHoa_SoCongThuongContext _context;
        public PermissionRepo(SoHoa_SoCongThuongContext context)
        {
            _context = context;
        }

        public async Task Insert(Group model)
        {
            await _context.Groups.AddAsync(model);
            await _context.SaveChangesAsync();
        }

        public async Task Update(Group model)
        {
            var db = await _context.Groups.Where(d => d.GroupId == model.GroupId).FirstOrDefaultAsync();
            db.GroupName = model.GroupName;
            db.Priority = model.Priority;
            db.Status = model.Status;
            await _context.SaveChangesAsync();
        }

        public async Task Delete(Guid id)
        {
            var itemRemove = await _context.Groups.Where(x => x.GroupId == id).FirstOrDefaultAsync();
            _context.Groups.Remove(itemRemove);
            await _context.SaveChangesAsync();
        }

        public IQueryable<Group> FindAll()
        {
            var result = _context.Groups.Select(d => new Group()
            {
                GroupName = d.GroupName,
                Priority = d.Priority,
            });

            return result;
        }

        public IQueryable<Group> FindById(Guid id)
        {
            var result = _context.Groups.Where(x => x.GroupId == id).Select(d => new Group()
            {
                GroupName = d.GroupName,
                Priority = d.Priority,
                GroupId = d.GroupId,
                Description =d.Description ?? "",
                Status = d.Status ?? 0,
            });

            return result;
        }

        public IQueryable<Group> FindByName(string name)
        {
            var result = _context.Groups.Where(x => x.GroupName.ToLower().Equals(name.ToLower())).Select(d => new Group()
            {
                GroupName = d.GroupName,
                Priority = d.Priority,
                GroupId = d.GroupId,
                Description = d.Description ?? "",
                Status = d.Status ?? 0,
            });

            return result;
        }
    }
}
