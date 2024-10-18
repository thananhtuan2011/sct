using API_SoCongThuong.Models;
using EF_Core.Models;
using Microsoft.EntityFrameworkCore;

namespace API_SoCongThuong.Reponsitories.ProcessAdministrativeProceduresRepository
{
    public class ProcessAdministrativeProceduresRepo
    {
        public SoHoa_SoCongThuongContext _context;
        public ProcessAdministrativeProceduresRepo(SoHoa_SoCongThuongContext context)
        {
            _context = context;
        }
        public async Task Insert(ProcessAdministrativeProceduresModel model)
        {
            ProcessAdministrativeProcedure data = new ProcessAdministrativeProcedure()
            {
                ProcessAdministrativeProceduresId = model.ProcessAdministrativeProceduresId,
                ProcessAdministrativeProceduresField = model.ProcessAdministrativeProceduresField,
                ProcessAdministrativeProceduresCode = model.ProcessAdministrativeProceduresCode,
                ProcessAdministrativeProceduresName = model.ProcessAdministrativeProceduresName,
                CreateUserId = model.CreateUserId,
                CreateTime = model.CreateTime,
            };
            await _context.ProcessAdministrativeProcedures.AddAsync(data);
            await _context.SaveChangesAsync();

            List<ProcessAdministrativeProceduresStep> steps = new List<ProcessAdministrativeProceduresStep>();
            foreach (var item in model.ProcessStep)
            {
                ProcessAdministrativeProceduresStep step = new ProcessAdministrativeProceduresStep()
                {
                    ProcessAdministrativeProceduresId = data.ProcessAdministrativeProceduresId,
                    Step = item.Step,
                    ImplementingAgencies = item.ImplementingAgencies,
                    ProcessingTime = item.ProcessingTime,
                    ContentImplementation = item.ContentImplementation,
                };
                steps.Add(step);
            }
            await _context.ProcessAdministrativeProceduresSteps.AddRangeAsync(steps);
            await _context.SaveChangesAsync();
        }

        public async Task Update(ProcessAdministrativeProceduresModel model)
        {
            var detailinfo = await _context.ProcessAdministrativeProcedures.Where(d => d.ProcessAdministrativeProceduresId == model.ProcessAdministrativeProceduresId).FirstOrDefaultAsync();
            detailinfo.ProcessAdministrativeProceduresField = model.ProcessAdministrativeProceduresField;
            detailinfo.ProcessAdministrativeProceduresCode = model.ProcessAdministrativeProceduresCode;
            detailinfo.ProcessAdministrativeProceduresName = model.ProcessAdministrativeProceduresName;
            detailinfo.UpdateUserId = model.UpdateUserId;
            detailinfo.UpdateTime = model.UpdateTime;

            var del_steps = _context.ProcessAdministrativeProceduresSteps.Where(d => d.ProcessAdministrativeProceduresId == model.ProcessAdministrativeProceduresId).ToList();
            _context.RemoveRange(del_steps);

            List<ProcessAdministrativeProceduresStep> steps = new List<ProcessAdministrativeProceduresStep>();
            foreach (var item in model.ProcessStep)
            {
                ProcessAdministrativeProceduresStep step = new ProcessAdministrativeProceduresStep()
                {
                    ProcessAdministrativeProceduresId = model.ProcessAdministrativeProceduresId,
                    Step = item.Step,
                    ImplementingAgencies = item.ImplementingAgencies,
                    ProcessingTime = item.ProcessingTime,
                    ContentImplementation = item.ContentImplementation,
                };
                steps.Add(step);
            }
            await _context.ProcessAdministrativeProceduresSteps.AddRangeAsync(steps);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(Guid Id)
        {
            var db = await _context.ProcessAdministrativeProcedures.Where(d => d.ProcessAdministrativeProceduresId == Id).FirstOrDefaultAsync();
            db.IsDel = true;

            var del_steps = _context.ProcessAdministrativeProceduresSteps.Where(d => d.ProcessAdministrativeProceduresId == Id).ToList();
            _context.RemoveRange(del_steps);

            await _context.SaveChangesAsync();
        }

        //public async Task Delete(Guid id)
        //{
        //    var itemRemove = await _context.ProcessAdministrativeProcedures.Where(x => x.ProcessAdministrativeProceduresId == id).FirstOrDefaultAsync();
        //    _context.ProcessAdministrativeProcedures.Remove(itemRemove);
        //    await _context.SaveChangesAsync();
        //}

        //public IQueryable<ProcessAdministrativeProcedure> FindAll()
        //{
        //    var result = _context.ProcessAdministrativeProcedures.Select(d => new ProcessAdministrativeProcedure()
        //    {
        //        ProcessAdministrativeProceduresId = d.ProcessAdministrativeProceduresId,
        //        ProcessAdministrativeProceduresCode = d.ProcessAdministrativeProceduresCode,
        //        ProcessAdministrativeProceduresName = d.ProcessAdministrativeProceduresName,
        //        IsDel = d.IsDel,
        //    });

        //    return result;
        //}

        public IQueryable<ProcessAdministrativeProceduresModel> FindById(Guid Id)
        {
            var result = _context.ProcessAdministrativeProcedures.Where(x => x.ProcessAdministrativeProceduresId == Id).Select(d => new ProcessAdministrativeProceduresModel()
            {
                ProcessAdministrativeProceduresId = d.ProcessAdministrativeProceduresId,
                ProcessAdministrativeProceduresField = d.ProcessAdministrativeProceduresField,
                ProcessAdministrativeProceduresFieldName = _context.Categories.Where(res => res.CategoryId == d.ProcessAdministrativeProceduresField).FirstOrDefault().CategoryName ?? "",
                ProcessAdministrativeProceduresCode = d.ProcessAdministrativeProceduresCode,
                ProcessAdministrativeProceduresName = d.ProcessAdministrativeProceduresName,
                IsDel = d.IsDel,
                ProcessStep = _context.ProcessAdministrativeProceduresSteps.Where(x => x.ProcessAdministrativeProceduresId == Id).ToList(),
        });
            return result;
        }

        public bool findByProcessAdministrativeProceduresCode(string Code, Guid? Id)
        {
            if (Id != null)
            {
                var ProcessAdministrativeProceduresCode = _context.ProcessAdministrativeProcedures.Where(x => x.ProcessAdministrativeProceduresId == Id && x.ProcessAdministrativeProceduresCode == Code && !x.IsDel).FirstOrDefault();
                if (ProcessAdministrativeProceduresCode != null)
                {
                    return false;
                }
            }
            var isProcessAdministrativeProceduresCode = _context.ProcessAdministrativeProcedures.Where(x => x.ProcessAdministrativeProceduresCode == Code && !x.IsDel).FirstOrDefault();
            if (isProcessAdministrativeProceduresCode == null)
            {
                return false;
            }
            return true;
        }
    }
}
