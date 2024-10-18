using API_SoCongThuong.Models;
using EF_Core.Models;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Runtime.CompilerServices;

namespace API_SoCongThuong.Reponsitories
{
    public class CateIndustrialClusterRepo
    {
        public SoHoa_SoCongThuongContext _context;
        public CateIndustrialClusterRepo(SoHoa_SoCongThuongContext context)
        {
            _context = context;
        }
        public async Task Insert(CateIndustrialClusterModel model)
        {
            CateIndustrialCluster data = new CateIndustrialCluster()
            {
                CateIndustrialClustersId = model.CateIndustrialClustersId,
                ApprovalDecision = model.ApprovalDecision,
                DecisionExpandCode = model.DecisionExpandCode,
                DetailedArea = model.DetailedArea,
                EstablishCode = model.EstablishCode,
                ExpandedArea = model.ExpandedArea,
                IndustrialArea = model.IndustrialArea,
                IndustrialClustersName = model.IndustrialClustersName,
                Occupancy = model.Occupancy,
                OriginalArea = model.OriginalArea,
                RentedArea = model.RentedArea,
                Note = model.Note,
                CreateUserId = model.CreateUserId,
                CreateTime = model.CreateTime,
                District = model.District
            };
            await _context.CateIndustrialClusters.AddAsync(data);
            await _context.SaveChangesAsync();
        }
        public async Task Update(CateIndustrialClusterModel model)
        {
            var detailinfo = await _context.CateIndustrialClusters.Where(d => d.CateIndustrialClustersId == model.CateIndustrialClustersId).FirstOrDefaultAsync();
            detailinfo.ApprovalDecision = model.ApprovalDecision;
            detailinfo.DecisionExpandCode = model.DecisionExpandCode;
            detailinfo.DetailedArea = model.DetailedArea;
            detailinfo.EstablishCode = model.EstablishCode;
            detailinfo.ExpandedArea = model.ExpandedArea;
            detailinfo.IndustrialArea = model.IndustrialArea;
            detailinfo.IndustrialClustersName = model.IndustrialClustersName;
            detailinfo.Occupancy = model.Occupancy;
            detailinfo.OriginalArea = model.OriginalArea;
            detailinfo.RentedArea = model.RentedArea;
            detailinfo.Note = model.Note;
            detailinfo.UpdateUserId = model.UpdateUserId;
            detailinfo.UpdateTime = model.UpdateTime;
            detailinfo.District = model.District;
            await _context.SaveChangesAsync();
        }
        public async Task Delete(Guid Id)
        {
            var detailinfo = await _context.CateIndustrialClusters.Where(d => d.CateIndustrialClustersId == Id).FirstOrDefaultAsync();
            detailinfo.IsDel = true;

            await _context.SaveChangesAsync();
        }
        //public async Task Deletes(List<Guid> Ids)
        //{
        //    List<CateManageAncolLocalBussine> items = new List<CateManageAncolLocalBussine>();
        //    foreach (var idremove in Ids)
        //    {
        //        CateManageAncolLocalBussine item = new CateManageAncolLocalBussine();
        //        var detailinfo = await _context.CateIndustrialClusters.Where(d => d.CateIndustrialClusterssId == idremove).FirstOrDefaultAsync();
        //        item.CateIndustrialClusterssId = idremove;
        //        item.IsDel = true;
        //        items.Add(item);

        //        var lstworkers = _context.CateIndustrialClustersDetails.Where(d => d.CateIndustrialClusterssId == idremove).ToList();
        //        var lstprofession = _context.CateIndustrialClustersTypeOfProfessions.Where(d => d.CateIndustrialClusterssId == idremove).ToList();
        //        _context.CateIndustrialClustersDetails.RemoveRange(lstworkers);
        //        _context.CateIndustrialClustersTypeOfProfessions.RemoveRange(lstprofession);
        //    }
        //    _context.CateIndustrialClusters.UpdateRange(items);
        //    await _context.SaveChangesAsync();
        //}
        public CateIndustrialClusterModel FindById(Guid Id)
        {
            var result = _context.CateIndustrialClusters.Where(x => x.CateIndustrialClustersId == Id && !x.IsDel)
                .Select(model => new CateIndustrialClusterModel()
                {
                    CateIndustrialClustersId = model.CateIndustrialClustersId,
                    ApprovalDecision = model.ApprovalDecision,
                    DecisionExpandCode = model.DecisionExpandCode,
                    DetailedArea = model.DetailedArea,
                    EstablishCode = model.EstablishCode,
                    ExpandedArea = model.ExpandedArea,
                    IndustrialArea = model.IndustrialArea,
                    IndustrialClustersName = model.IndustrialClustersName,
                    Occupancy = model.Occupancy,
                    OriginalArea = model.OriginalArea,
                    RentedArea = model.RentedArea,
                    Note = model.Note,
                    IsAction = model.IsAction,
                    District = model.District,
                }).FirstOrDefault();

            return result;
        }

    }
}
