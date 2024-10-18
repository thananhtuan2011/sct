using API_SoCongThuong.Models;
using EF_Core.Models;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Runtime.CompilerServices;

namespace API_SoCongThuong.Reponsitories
{
    public class CateInvestmentProjectRepo
    {
        public SoHoa_SoCongThuongContext _context;
        public CateInvestmentProjectRepo(SoHoa_SoCongThuongContext context)
        {
            _context = context;
        }
        public async Task Insert(CateInvestmentProjectModel model)
        {
            CateInvestmentProject data = new CateInvestmentProject()
            {
                CateInvestmentProjectId = model.CateInvestmentProjectId,
                BusinessName = model.BusinessName,
                InvestmentType = model.InvestmentType,
                Produce = model.Produce,
                ProductValue = model.ProductValue,
                ProjectArea = model.ProjectArea,
                Quantity = model.Quantity,
                Reality = model.Reality,
                Investment = model.Investment,
                NumberOfWorker = model.NumberOfWorker,
                CreateUserId = model.CreateUserId,
                CreateTime = model.CreateTime,
                Owner = model.Owner,
                Career = model.Career,
                PhoneNumber = model.PhoneNumber,
                District = model.District
            };
            await _context.CateInvestmentProjects.AddAsync(data);
            await _context.SaveChangesAsync();
        }
        public async Task Update(CateInvestmentProjectModel model)
        {
            var detailinfo = await _context.CateInvestmentProjects.Where(d => d.CateInvestmentProjectId == model.CateInvestmentProjectId).FirstOrDefaultAsync();
            detailinfo.BusinessName = model.BusinessName;
            detailinfo.InvestmentType = model.InvestmentType;
            detailinfo.Produce = model.Produce;
            detailinfo.ProductValue = model.ProductValue;
            detailinfo.ProjectArea = model.ProjectArea;
            detailinfo.Quantity = model.Quantity;
            detailinfo.Reality = model.Reality;
            detailinfo.Investment = model.Investment;
            detailinfo.NumberOfWorker = model.NumberOfWorker;
            detailinfo.UpdateUserId = model.UpdateUserId;
            detailinfo.UpdateTime = model.UpdateTime;
            detailinfo.Owner = model.Owner;
            detailinfo.Career = model.Career;
            detailinfo.PhoneNumber = model.PhoneNumber;
            detailinfo.District = model.District;

            await _context.SaveChangesAsync();
        }
        public async Task Delete(Guid Id)
        {
            var detailinfo = await _context.CateInvestmentProjects.Where(d => d.CateInvestmentProjectId == Id).FirstOrDefaultAsync();
            detailinfo.IsDel = true;
            await _context.SaveChangesAsync();
        }
        //public async Task Deletes(List<Guid> Ids)
        //{
        //    List<CateManageAncolLocalBussine> items = new List<CateManageAncolLocalBussine>();
        //    foreach (var idremove in Ids)
        //    {
        //        CateManageAncolLocalBussine item = new CateManageAncolLocalBussine();
        //        var detailinfo = await _context.CateInvestmentProjects.Where(d => d.CateInvestmentProjectssId == idremove).FirstOrDefaultAsync();
        //        item.CateInvestmentProjectssId = idremove;
        //        item.IsDel = true;
        //        items.Add(item);

        //        var lstworkers = _context.CateInvestmentProjectsDetails.Where(d => d.CateInvestmentProjectssId == idremove).ToList();
        //        var lstprofession = _context.CateInvestmentProjectsTypeOfProfessions.Where(d => d.CateInvestmentProjectssId == idremove).ToList();
        //        _context.CateInvestmentProjectsDetails.RemoveRange(lstworkers);
        //        _context.CateInvestmentProjectsTypeOfProfessions.RemoveRange(lstprofession);
        //    }
        //    _context.CateInvestmentProjects.UpdateRange(items);
        //    await _context.SaveChangesAsync();
        //}
        public CateInvestmentProjectModel FindById(Guid Id)
        {
            var result = _context.CateInvestmentProjects.Where(x => x.CateInvestmentProjectId == Id && !x.IsDel).Select(model => new CateInvestmentProjectModel()
            {
                CateInvestmentProjectId = model.CateInvestmentProjectId,
                BusinessName = model.BusinessName,
                InvestmentType = model.InvestmentType,
                Produce = model.Produce,
                ProductValue = model.ProductValue,
                ProjectArea = model.ProjectArea,
                Quantity = model.Quantity,
                Reality = model.Reality,
                Investment = model.Investment,
                NumberOfWorker = model.NumberOfWorker,
                Owner = model.Owner,
                Career = model.Career,
                PhoneNumber = model.PhoneNumber,
                District = model.District
            }).FirstOrDefault();


            return result;
        }
    }
}
