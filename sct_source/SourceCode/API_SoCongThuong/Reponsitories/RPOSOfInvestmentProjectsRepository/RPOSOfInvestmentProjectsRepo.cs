using API_SoCongThuong.Models;
using EF_Core.Models;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Runtime.CompilerServices;

namespace API_SoCongThuong.Reponsitories
{
    public class RPOSOfInvestmentProjectsRepo
    {
        public SoHoa_SoCongThuongContext _context;
        public RPOSOfInvestmentProjectsRepo(SoHoa_SoCongThuongContext context)
        {
            _context = context;
        }
        public async Task Insert(RPOSOfInvestmentProjectModel model)
        {
            ReportOperationalStatusOfInvestmentProject data = new ReportOperationalStatusOfInvestmentProject()
            {
                ReportOperationalStatusOfInvestmentProjectsId = model.ReportOperationalStatusOfInvestmentProjectsId,
                Criteria= model.Criteria,
                Note= model.Note,
                Quantity= model.Quantity,
                ReportingPeriod= model.ReportingPeriod,
                Units= model.Units,
                CreateUserId = model.CreateUserId,
                CreateTime = model.CreateTime,
                Year = model.Year
            };
            await _context.ReportOperationalStatusOfInvestmentProjects.AddAsync(data);
            await _context.SaveChangesAsync();
        }
        public async Task Update(RPOSOfInvestmentProjectModel model)
        {
            var detailinfo = await _context.ReportOperationalStatusOfInvestmentProjects.Where(d => d.ReportOperationalStatusOfInvestmentProjectsId == model.ReportOperationalStatusOfInvestmentProjectsId).FirstOrDefaultAsync();
            detailinfo.ReportOperationalStatusOfInvestmentProjectsId = model.ReportOperationalStatusOfInvestmentProjectsId;
            detailinfo.Criteria = model.Criteria;
            detailinfo.Note = model.Note;
            detailinfo.Quantity = model.Quantity;
            detailinfo.ReportingPeriod = model.ReportingPeriod;
            detailinfo.Units = model.Units;
            detailinfo.UpdateUserId = model.UpdateUserId;
            detailinfo.UpdateTime = model.UpdateTime;
            detailinfo.Year = model.Year;

            await _context.SaveChangesAsync();
        }
        public async Task Delete(Guid Id)
        {
            var detailinfo = await _context.ReportOperationalStatusOfInvestmentProjects.Where(d => d.ReportOperationalStatusOfInvestmentProjectsId == Id).FirstOrDefaultAsync();
            detailinfo.IsDel = true;
            await _context.SaveChangesAsync();
        }
        //public async Task Deletes(List<Guid> Ids)
        //{
        //    List<CateManageAncolLocalBussine> items = new List<CateManageAncolLocalBussine>();
        //    foreach (var idremove in Ids)
        //    {
        //        CateManageAncolLocalBussine item = new CateManageAncolLocalBussine();
        //        var detailinfo = await _context.ReportOperationalStatusOfInvestmentProjects.Where(d => d.ReportOperationalStatusOfInvestmentProjectssId == idremove).FirstOrDefaultAsync();
        //        item.ReportOperationalStatusOfInvestmentProjectssId = idremove;
        //        item.IsDel = true;
        //        items.Add(item);

        //        var lstworkers = _context.ReportOperationalStatusOfInvestmentProjectsDetails.Where(d => d.ReportOperationalStatusOfInvestmentProjectssId == idremove).ToList();
        //        var lstprofession = _context.ReportOperationalStatusOfInvestmentProjectsTypeOfProfessions.Where(d => d.ReportOperationalStatusOfInvestmentProjectssId == idremove).ToList();
        //        _context.ReportOperationalStatusOfInvestmentProjectsDetails.RemoveRange(lstworkers);
        //        _context.ReportOperationalStatusOfInvestmentProjectsTypeOfProfessions.RemoveRange(lstprofession);
        //    }
        //    _context.ReportOperationalStatusOfInvestmentProjects.UpdateRange(items);
        //    await _context.SaveChangesAsync();
        //}
        public RPOSOfInvestmentProjectModel FindById(Guid Id)
        {
            var result = _context.ReportOperationalStatusOfInvestmentProjects.Where(x => x.ReportOperationalStatusOfInvestmentProjectsId == Id && !x.IsDel).Select(model => new RPOSOfInvestmentProjectModel()
            {
                ReportOperationalStatusOfInvestmentProjectsId = model.ReportOperationalStatusOfInvestmentProjectsId,
                Criteria = model.Criteria,
                Note = model.Note,
                Quantity = model.Quantity,
                ReportingPeriod = model.ReportingPeriod,
                Units = model.Units,
                Year = model.Year,
            }).FirstOrDefault();

            return result;
        }
    }
}
