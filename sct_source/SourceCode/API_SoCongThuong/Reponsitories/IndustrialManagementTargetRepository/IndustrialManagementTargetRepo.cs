using EF_Core.Models;
using Microsoft.EntityFrameworkCore;
using API_SoCongThuong.Models;
namespace API_SoCongThuong.Reponsitories
{
    public class IndustrialManagementTargetRepo
    {
        public SoHoa_SoCongThuongContext _context;
        public IndustrialManagementTargetRepo(SoHoa_SoCongThuongContext context)
        {
            _context = context;
        }
        public async Task Insert(IndustrialManagementTargetModel model)
        {
            List<IndustrialManagementTarget> listDataSave = new List<IndustrialManagementTarget>();
            List<IndustrialManagementTargetChildModel> tmp = new List<IndustrialManagementTargetChildModel>();
            foreach(var item  in model.listChild)
            {
                if(item.getChild.Count > 0)
                {
                    tmp.Add(item);
                }
                else
                {
                    IndustrialManagementTarget SaveData = new IndustrialManagementTarget();
                    SaveData.ParentTargetId = model.ParentTargetId;
                    SaveData.GroupTargetId = model.GroupTargetId;
                    SaveData.Name = item.Name.Trim();
                    SaveData.Unit = item.Unit.Trim();
                    SaveData.CreateTime = model.CreateTime;
                    SaveData.CreateUserId = model.CreateUserId;
                    listDataSave.Add(SaveData);
                }
            }
            _context.IndustrialManagementTargets.AddRange(listDataSave);
            await _context.SaveChangesAsync();
            foreach (var it in tmp)
            {
                IndustrialManagementTarget SaveData = new IndustrialManagementTarget();
                SaveData.ParentTargetId = model.ParentTargetId;
                SaveData.GroupTargetId = model.GroupTargetId;
                SaveData.Name = it.Name.Trim();
                SaveData.Unit = it.Unit.Trim();
                SaveData.CreateTime = model.CreateTime;
                SaveData.CreateUserId = model.CreateUserId;
                await _context.IndustrialManagementTargets.AddAsync(SaveData);
                await _context.SaveChangesAsync();
                List<IndustrialManagementTargetChild> listChild = new List<IndustrialManagementTargetChild>();
                foreach (var child in it.getChild)
                {
                    IndustrialManagementTargetChild data = new IndustrialManagementTargetChild();
                    data.IndustrialManagementTargetId = SaveData.IndustrialManagementTargetId;
                    data.Name = child.Name.Trim();
                    data.Unit = child.Unit.Trim();
                    data.CreateTime = model.CreateTime;
                    data.CreateUserId = model.CreateUserId;
                    listChild.Add(data);
                }
                if(listChild.Count > 0)
                {
                    await _context.IndustrialManagementTargetChildren.AddRangeAsync(listChild);
                    await _context.SaveChangesAsync();
                }
            }
            //SaveData.ParentTargetId = model.ParentTargetId;
            //SaveData.GroupTargetId = model.GroupTargetId;

            //await _context.Gas.AddAsync(model);
            //await _context.SaveChangesAsync();
        }

        public async Task Update(IndustrialManagementTargetModel model)
        {
            var data = _context.IndustrialManagementTargets.Where(x => !x.IsDel && x.IndustrialManagementTargetId == model.IndustrialManagementTargetId).Select(x => x).FirstOrDefault();
            if(data != null)
            {
                data.ParentTargetId = model.ParentTargetId;
                data.GroupTargetId = model.GroupTargetId;
                data.Name = model.listChild[0].Name.Trim();
                data.Unit = model.listChild[0].Unit.Trim();
                data.UpdateTime = model.UpdateTime;
                data.UpdateUserId = model.UpdateUserId;
                await _context.SaveChangesAsync();

                var listDel = _context.IndustrialManagementTargetChildren.Where(x => x.IndustrialManagementTargetId == model.IndustrialManagementTargetId).ToList();
                if(listDel.Count > 0)
                {
                    _context.IndustrialManagementTargetChildren.RemoveRange(listDel);
                    await _context.SaveChangesAsync();
                }

                List<IndustrialManagementTargetChild> listChild = new List<IndustrialManagementTargetChild>();
                foreach (var child in model.listChild[0].getChild)
                {
                    IndustrialManagementTargetChild tmp = new IndustrialManagementTargetChild();
                    tmp.IndustrialManagementTargetId = model.IndustrialManagementTargetId;
                    tmp.Name = child.Name.Trim();
                    tmp.Unit = child.Unit.Trim();
                    tmp.CreateTime = model.CreateTime;
                    tmp.CreateUserId = model.CreateUserId;
                    listChild.Add(tmp);
                }
                if (listChild.Count > 0)
                {
                    await _context.IndustrialManagementTargetChildren.AddRangeAsync(listChild);
                    await _context.SaveChangesAsync();
                }
            }

            //_context.Gas.Update(model);
        }
        public async Task Delete(Guid id)
        {

            var data = _context.IndustrialManagementTargets.Where(x => !x.IsDel && x.IndustrialManagementTargetId == id).Select(x => x).FirstOrDefault();

            //var db = await _context.Gas.Where(d => d.GasId == model.GasId).FirstOrDefaultAsync();
            if (data != null)
            {
                data.IsDel = true;
            }

            var listDel = _context.IndustrialManagementTargetChildren.Where(x => x.IndustrialManagementTargetId == id).ToList();
            if (listDel.Count > 0)
            {
                _context.IndustrialManagementTargetChildren.RemoveRange(listDel);
                await _context.SaveChangesAsync();
            }
            await _context.SaveChangesAsync();
        }

        public IndustrialManagementTargetChildModel FindById(Guid id)
        {
            //var result = from imt in _context.IndustrialManagementTargets where imt.IndustrialManagementTargetId equals id && !imt.IsDel

            var result = _context.IndustrialManagementTargets.Where(x => x.IndustrialManagementTargetId == id && !x.IsDel).Select(d => new IndustrialManagementTargetChildModel()
            {
                IndustrialManagementTargetId = d.IndustrialManagementTargetId,
                Name = d.Name,
                Unit = d.Unit,
                ParentTargetId = d.ParentTargetId,
                GroupTargetId = d.GroupTargetId,
            }).FirstOrDefault();

            var listChild = _context.IndustrialManagementTargetChildren.Where(x => x.IndustrialManagementTargetId == id && !x.IsDel).Select(d => new ChildModel
            {
                Id = d.Id,
                Name = d.Name,
                Unit = d.Unit
            }).ToList();
     
            result.getChild = listChild;

            return result;
        }
    }
}
