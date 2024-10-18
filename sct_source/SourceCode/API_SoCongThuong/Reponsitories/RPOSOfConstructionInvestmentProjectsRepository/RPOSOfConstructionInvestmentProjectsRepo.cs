using API_SoCongThuong.Models;
using EF_Core.Models;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Runtime.CompilerServices;

namespace API_SoCongThuong.Reponsitories
{
    public class RPOSOfConstructionInvestmentProjectsRepo
    {
        public SoHoa_SoCongThuongContext _context;
        public RPOSOfConstructionInvestmentProjectsRepo(SoHoa_SoCongThuongContext context)
        {
            _context = context;
        }
        public async Task Insert(RPOSOfConstructionInvestmentProjectModel model)
        {
            ReportOperationalStatusOfConstructionInvestmentProject data = new ReportOperationalStatusOfConstructionInvestmentProject()
            {
                ReportOperationalStatusOfConstructionInvestmentProjectsId = model.ReportOperationalStatusOfConstructionInvestmentProjectsId,
                Criteria= model.Criteria,
                Note= model.Note,
                Quantity= model.Quantity,
                ReportingPeriod= model.ReportingPeriod,
                Units= model.Units,
                CreateUserId = model.CreateUserId,
                CreateTime = model.CreateTime,
                Year = model.Year
            };
            await _context.ReportOperationalStatusOfConstructionInvestmentProjects.AddAsync(data);
            await _context.SaveChangesAsync();
        }
        public async Task Update(RPOSOfConstructionInvestmentProjectModel model)
        {
            var detailinfo = await _context.ReportOperationalStatusOfConstructionInvestmentProjects.Where(d => d.ReportOperationalStatusOfConstructionInvestmentProjectsId == model.ReportOperationalStatusOfConstructionInvestmentProjectsId).FirstOrDefaultAsync();
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
            var detailinfo = await _context.ReportOperationalStatusOfConstructionInvestmentProjects.Where(d => d.ReportOperationalStatusOfConstructionInvestmentProjectsId == Id).FirstOrDefaultAsync();
            detailinfo.IsDel = true;

            await _context.SaveChangesAsync();
        }
        //public async Task Deletes(List<Guid> Ids)
        //{
        //    List<CateManageAncolLocalBussine> items = new List<CateManageAncolLocalBussine>();
        //    foreach (var idremove in Ids)
        //    {
        //        CateManageAncolLocalBussine item = new CateManageAncolLocalBussine();
        //        var detailinfo = await _context.ReportOperationalStatusOfConstructionInvestmentProjects.Where(d => d.ReportOperationalStatusOfConstructionInvestmentProjectssId == idremove).FirstOrDefaultAsync();
        //        item.ReportOperationalStatusOfConstructionInvestmentProjectssId = idremove;
        //        item.IsDel = true;
        //        items.Add(item);

        //        var lstworkers = _context.ReportOperationalStatusOfConstructionInvestmentProjectsDetails.Where(d => d.ReportOperationalStatusOfConstructionInvestmentProjectssId == idremove).ToList();
        //        var lstprofession = _context.ReportOperationalStatusOfConstructionInvestmentProjectsTypeOfProfessions.Where(d => d.ReportOperationalStatusOfConstructionInvestmentProjectssId == idremove).ToList();
        //        _context.ReportOperationalStatusOfConstructionInvestmentProjectsDetails.RemoveRange(lstworkers);
        //        _context.ReportOperationalStatusOfConstructionInvestmentProjectsTypeOfProfessions.RemoveRange(lstprofession);
        //    }
        //    _context.ReportOperationalStatusOfConstructionInvestmentProjects.UpdateRange(items);
        //    await _context.SaveChangesAsync();
        //}
        public RPOSOfConstructionInvestmentProjectModel FindById(Guid Id)
        {
            var result = _context.ReportOperationalStatusOfConstructionInvestmentProjects.Where(x => x.ReportOperationalStatusOfConstructionInvestmentProjectsId == Id && !x.IsDel).Select(model => new RPOSOfConstructionInvestmentProjectModel()
            {
                ReportOperationalStatusOfConstructionInvestmentProjectsId = model.ReportOperationalStatusOfConstructionInvestmentProjectsId,
                Criteria = model.Criteria,
                Note = model.Note,
                Quantity = model.Quantity,
                ReportingPeriod = model.ReportingPeriod,
                Units = model.Units,
                Year = model.Year
            }).FirstOrDefault();

            return result;
        }
    }
}
