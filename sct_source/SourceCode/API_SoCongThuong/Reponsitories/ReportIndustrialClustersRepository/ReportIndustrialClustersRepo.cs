using API_SoCongThuong.Models;
using EF_Core.Models;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Runtime.CompilerServices;

namespace API_SoCongThuong.Reponsitories
{
    public class ReportIndustrialClustersRepo
    {
        public SoHoa_SoCongThuongContext _context;
        public ReportIndustrialClustersRepo(SoHoa_SoCongThuongContext context)
        {
            _context = context;
        }
        public async Task Insert(ReportIndustrialClusterModel model)
        {
            ReportIndustrialCluster data = new ReportIndustrialCluster()
            {
                Criteria = model.Criteria,
                Note = model.Note,
                Quantity = model.Quantity,
                ReportIndustrialClustersId = model.ReportIndustrialClustersId,
                ReportingPeriod = model.ReportingPeriod,
                TypeReport = model.TypeReport,
                Units = model.Units,
                CreateUserId = model.CreateUserId,
                CreateTime = model.CreateTime,
                Year = model.Year,
                GroupId = model.GroupId,
                District = model.District
            };
            await _context.ReportIndustrialClusters.AddAsync(data);
            await _context.SaveChangesAsync();
        }
        public async Task Update(ReportIndustrialClusterModel model)
        {
            var detailinfo = await _context.ReportIndustrialClusters.Where(d => d.ReportIndustrialClustersId == model.ReportIndustrialClustersId).FirstOrDefaultAsync();
            detailinfo.Criteria = model.Criteria;
            detailinfo.Note = model.Note;
            detailinfo.Quantity = model.Quantity;
            detailinfo.ReportIndustrialClustersId = model.ReportIndustrialClustersId;
            detailinfo.ReportingPeriod = model.ReportingPeriod;
            detailinfo.TypeReport = model.TypeReport;
            detailinfo.Units = model.Units;
            detailinfo.UpdateUserId = model.UpdateUserId;
            detailinfo.UpdateTime = model.UpdateTime;
            detailinfo.Year = model.Year;
            detailinfo.GroupId = model.GroupId;
            detailinfo.District = model.District;

            await _context.SaveChangesAsync();
        }
        public async Task Delete(Guid Id)
        {
            var detailinfo = await _context.ReportIndustrialClusters.Where(d => d.ReportIndustrialClustersId == Id).FirstOrDefaultAsync();
            detailinfo.IsDel = true;

            await _context.SaveChangesAsync();
        }
        //public async Task Deletes(List<Guid> Ids)
        //{
        //    List<CateManageAncolLocalBussine> items = new List<CateManageAncolLocalBussine>();
        //    foreach (var idremove in Ids)
        //    {
        //        CateManageAncolLocalBussine item = new CateManageAncolLocalBussine();
        //        var detailinfo = await _context.ReportIndustrialClusters.Where(d => d.ReportIndustrialClustersId == idremove).FirstOrDefaultAsync();
        //        item.ReportIndustrialClusterssId = idremove;
        //        item.IsDel = true;
        //        items.Add(item);

        //        var lstworkers = _context.ReportIndustrialClustersDetails.Where(d => d.ReportIndustrialClusterssId == idremove).ToList();
        //        var lstprofession = _context.ReportIndustrialClustersTypeOfProfessions.Where(d => d.ReportIndustrialClusterssId == idremove).ToList();
        //        _context.ReportIndustrialClustersDetails.RemoveRange(lstworkers);
        //        _context.ReportIndustrialClustersTypeOfProfessions.RemoveRange(lstprofession);
        //    }
        //    _context.ReportIndustrialClusters.UpdateRange(items);
        //    await _context.SaveChangesAsync();
        //}
        public ReportIndustrialClusterModel FindById(Guid Id)
        {
            var result = _context.ReportIndustrialClusters.Where(x => x.ReportIndustrialClustersId == Id && !x.IsDel).Select(model => new ReportIndustrialClusterModel()
            {
                ReportIndustrialClustersId = model.ReportIndustrialClustersId,
                Criteria = model.Criteria,
                Note = model.Note,
                Quantity = model.Quantity,
                ReportingPeriod = model.ReportingPeriod,
                TypeReport = model.TypeReport,
                Units = model.Units,
                Year = model.Year,
                GroupId = model.GroupId,
                District = model.District
            }).FirstOrDefault();

            return result;
        }
    }
}
