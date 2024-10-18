using API_SoCongThuong.Models;
using EF_Core.Models;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Runtime.CompilerServices;

namespace API_SoCongThuong.Reponsitories
{
    public class ReportPromotionCommerceRepo
    {
        public SoHoa_SoCongThuongContext _context;
        public ReportPromotionCommerceRepo(SoHoa_SoCongThuongContext context)
        {
            _context = context;
        }
        public async Task Insert(ReportPromotionCommerceModel model)
        {
            ReportPromotionCommerce data = new ReportPromotionCommerce()
            {
                ReportPromotionCommerceId= model.ReportPromotionCommerceId,
                Business=model.Business,
                Chief=model.Chief,
                EndTime=model.EndTime,
                StartTime=model.StartTime,
                Fax=model.Fax,
                Host=model.Host,
                Location=model.Location,
                Organize=model.Organize,
                PhoneNumber=model.PhoneNumber,
                Position=model.Position,
                ProjectName=model.ProjectName,
                Represent=model.Represent,
                ResultNote=model.ResultNote,
                Scale=model.Scale,
                Note=model.Note,
                CreateUserId = model.CreateUserId,
                CreateTime = model.CreateTime,
            };
            await _context.ReportPromotionCommerces.AddAsync(data);
            await _context.SaveChangesAsync();
        }
        public async Task Update(ReportPromotionCommerceModel model)
        {
            var detailinfo = await _context.ReportPromotionCommerces.Where(d => d.ReportPromotionCommerceId == model.ReportPromotionCommerceId).FirstOrDefaultAsync();
            detailinfo.Business = model.Business;
            detailinfo.Chief = model.Chief;
            detailinfo.EndTime = model.EndTime;
            detailinfo.StartTime = model.StartTime;
            detailinfo.Fax = model.Fax;
            detailinfo.Host = model.Host;
            detailinfo.Location = model.Location;
            detailinfo.Organize = model.Organize;
            detailinfo.PhoneNumber = model.PhoneNumber;
            detailinfo.Position = model.Position;
            detailinfo.ProjectName = model.ProjectName;
            detailinfo.Represent = model.Represent;
            detailinfo.ResultNote = model.ResultNote;
            detailinfo.Scale = model.Scale;
            detailinfo.Note = model.Note;

            await _context.SaveChangesAsync();
        }
        public async Task Delete(Guid Id)
        {
            var detailinfo = await _context.ReportPromotionCommerces.Where(d => d.ReportPromotionCommerceId == Id).FirstOrDefaultAsync();
            detailinfo.IsDel = true;
            await _context.SaveChangesAsync();
        }
        //public async Task Deletes(List<Guid> Ids)
        //{
        //    List<CateManageAncolLocalBussine> items = new List<CateManageAncolLocalBussine>();
        //    foreach (var idremove in Ids)
        //    {
        //        CateManageAncolLocalBussine item = new CateManageAncolLocalBussine();
        //        var detailinfo = await _context.ReportPromotionCommerces.Where(d => d.ReportPromotionCommercessId == idremove).FirstOrDefaultAsync();
        //        item.ReportPromotionCommercessId = idremove;
        //        item.IsDel = true;
        //        items.Add(item);

        //        var lstworkers = _context.ReportPromotionCommercesDetails.Where(d => d.ReportPromotionCommercessId == idremove).ToList();
        //        var lstprofession = _context.ReportPromotionCommercesTypeOfProfessions.Where(d => d.ReportPromotionCommercessId == idremove).ToList();
        //        _context.ReportPromotionCommercesDetails.RemoveRange(lstworkers);
        //        _context.ReportPromotionCommercesTypeOfProfessions.RemoveRange(lstprofession);
        //    }
        //    _context.ReportPromotionCommerces.UpdateRange(items);
        //    await _context.SaveChangesAsync();
        //}
        public ReportPromotionCommerceModel FindById(Guid Id)
        {
            var result = _context.ReportPromotionCommerces.Where(x => x.ReportPromotionCommerceId == Id && !x.IsDel).Select(model => new ReportPromotionCommerceModel()
            {
                ReportPromotionCommerceId = model.ReportPromotionCommerceId,
                Business = model.Business,
                Chief = model.Chief,
                EndTime = model.EndTime,
                StartTime = model.StartTime,
                Fax = model.Fax,
                Host = model.Host,
                Location = model.Location,
                Organize = model.Organize,
                PhoneNumber = model.PhoneNumber,
                Position = model.Position,
                ProjectName = model.ProjectName,
                Represent = model.Represent,
                ResultNote = model.ResultNote,
                Scale = model.Scale,
                Note = model.Note,
            }).FirstOrDefault();


            return result;
        }
    }
}
