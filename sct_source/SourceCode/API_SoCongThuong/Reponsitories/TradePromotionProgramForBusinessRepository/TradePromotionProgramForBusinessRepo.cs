using API_SoCongThuong.Models;
using EF_Core.Models;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Runtime.CompilerServices;

namespace API_SoCongThuong.Reponsitories
{
    public class TradePromotionProgramForBusinessRepo
    {
        public SoHoa_SoCongThuongContext _context;
        public TradePromotionProgramForBusinessRepo(SoHoa_SoCongThuongContext context)
        {
            _context = context;
        }
        public async Task Insert(TradePromotionProgramForBusinessModel model)
        {
            TradePromotionProgramForBusiness data = new TradePromotionProgramForBusiness()
            {
                TradePromotionProgramBusinessId = model.TradePromotionProgramBusinessId,
                Business = model.Business,
                Country = model.Country,
                CreateUserId = model.CreateUserId,
                CreateTime = model.CreateTime,
            };
            await _context.TradePromotionProgramForBusinesses.AddAsync(data);
            await _context.SaveChangesAsync();
            List<TradePromotionProgramForBusinessDetail> details = new List<TradePromotionProgramForBusinessDetail>();
            foreach (var item in model.Details)
            {
                TradePromotionProgramForBusinessDetail detail = new TradePromotionProgramForBusinessDetail()
                {
                    TradePromotionProgramBusinessId = data.TradePromotionProgramBusinessId,
                    Profession = item.Profession
                };
                details.Add(detail);
            }
            await _context.TradePromotionProgramForBusinessDetails.AddRangeAsync(details);
            await _context.SaveChangesAsync();
        }
        public async Task Update(TradePromotionProgramForBusinessModel model)
        {
            var del = _context.TradePromotionProgramForBusinessDetails.Where(d => d.TradePromotionProgramBusinessId == model.TradePromotionProgramBusinessId).ToList();
            _context.TradePromotionProgramForBusinessDetails.RemoveRange(del);
            await _context.SaveChangesAsync();

            var detailinfo = await _context.TradePromotionProgramForBusinesses.Where(d => d.TradePromotionProgramBusinessId == model.TradePromotionProgramBusinessId).FirstOrDefaultAsync();
            detailinfo.Business = model.Business;
            detailinfo.Country = model.Country;
            detailinfo.UpdateUserId = model.UpdateUserId;
            detailinfo.UpdateTime = model.UpdateTime;

            List<TradePromotionProgramForBusinessDetail> details = new List<TradePromotionProgramForBusinessDetail>();
            foreach (var item in model.Details)
            {
                TradePromotionProgramForBusinessDetail detail = new TradePromotionProgramForBusinessDetail()
                {
                    TradePromotionProgramBusinessId = model.TradePromotionProgramBusinessId,
                    Profession = item.Profession
                };
                details.Add(detail);
            }
            await _context.TradePromotionProgramForBusinessDetails.AddRangeAsync(details);
            await _context.SaveChangesAsync();
        }
        public async Task Delete(Guid Id)
        {
            var detailinfo = await _context.TradePromotionProgramForBusinesses.Where(d => d.TradePromotionProgramBusinessId == Id).FirstOrDefaultAsync();
            detailinfo.IsDel = true;

            var del = _context.TradePromotionProgramForBusinessDetails.Where(d => d.TradePromotionProgramBusinessId == Id).ToList();
            _context.TradePromotionProgramForBusinessDetails.RemoveRange(del);
            await _context.SaveChangesAsync();
        }
        //public async Task Deletes(List<Guid> Ids)
        //{
        //    List<CateManageAncolLocalBussine> items = new List<CateManageAncolLocalBussine>();
        //    foreach (var idremove in Ids)
        //    {
        //        CateManageAncolLocalBussine item = new CateManageAncolLocalBussine();
        //        var detailinfo = await _context.TradePromotionProgramForBusinesses.Where(d => d.TradePromotionProgramForBusinessessId == idremove).FirstOrDefaultAsync();
        //        item.TradePromotionProgramForBusinessessId = idremove;
        //        item.IsDel = true;
        //        items.Add(item);

        //        var lstworkers = _context.TradePromotionProgramForBusinessesDetails.Where(d => d.TradePromotionProgramForBusinessessId == idremove).ToList();
        //        var lstprofession = _context.TradePromotionProgramForBusinessesTypeOfProfessions.Where(d => d.TradePromotionProgramForBusinessessId == idremove).ToList();
        //        _context.TradePromotionProgramForBusinessesDetails.RemoveRange(lstworkers);
        //        _context.TradePromotionProgramForBusinessesTypeOfProfessions.RemoveRange(lstprofession);
        //    }
        //    _context.TradePromotionProgramForBusinesses.UpdateRange(items);
        //    await _context.SaveChangesAsync();
        //}
        public TradePromotionProgramForBusinessModel FindById(Guid Id)
        {
            var result = _context.TradePromotionProgramForBusinesses.Where(x => x.TradePromotionProgramBusinessId == Id && !x.IsDel).Select(model => new TradePromotionProgramForBusinessModel()
            {
                TradePromotionProgramBusinessId = model.TradePromotionProgramBusinessId,
                Business = model.Business,
                Country = model.Country,
            }).FirstOrDefault();

            if (result == null)
            {
                return new TradePromotionProgramForBusinessModel();
            }

            var details = _context.TradePromotionProgramForBusinessDetails.Where(x => x.TradePromotionProgramBusinessId == Id).GroupJoin(
                _context.TypeOfProfessions, trade => trade.Profession, profess => profess.TypeOfProfessionId,
                (trade, profess) => new { trade, profess }).SelectMany(result => result.profess.DefaultIfEmpty(), (info, prof)
                => new TradePromotionProgramForBusinessDetailModel
                {
                    TradePromotionProgramForBusinessDetailId = info.trade.TradePromotionProgramForBusinessDetailId,
                    Profession = info.trade.Profession,
                    ProfessionName = prof.TypeOfProfessionName,
                    ProfessionCode = prof.TypeOfProfessionCode
                });
            result.Details = details.ToList();

            return result;
        }
    }
}
