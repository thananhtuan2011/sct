using API_SoCongThuong.Models;
using EF_Core.Models;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Runtime.CompilerServices;

namespace API_SoCongThuong.Reponsitories
{
    public class ParticipateSupportFairRepo
    {
        public SoHoa_SoCongThuongContext _context;
        public ParticipateSupportFairRepo(SoHoa_SoCongThuongContext context)
        {
            _context = context;
        }
        public async Task Insert(ParticipateSupportFairModel model)
        {
            ParticipateSupportFair data = new ParticipateSupportFair()
            {
                ParticipateSupportFairId= model.ParticipateSupportFairId,
                ParticipateSupportFairName= model.ParticipateSupportFairName,
                Address= model.Address,
                Country= model.Country,
                EndTime= model.EndTime,
                PlanJoin= model.PlanJoin,
                Scale= model.Scale,
                StartTime= model.StartTime,
                CreateUserId = model.CreateUserId,
                CreateTime = model.CreateTime,
                DistrictId = model.DistrictId,
                CommuneId = model.CommuneId,
                ImplementCost= model.ImplementCost,
            };
            await _context.ParticipateSupportFairs.AddAsync(data);
            await _context.SaveChangesAsync();

            List<ParticipateSupportFairDetail> details = new List<ParticipateSupportFairDetail>();
            foreach (var item in model.Details)
            {
                ParticipateSupportFairDetail detail = new ParticipateSupportFairDetail()
                {
                    ParticipateSupportFairId=data.ParticipateSupportFairId,
                    BusinessCode=item.BusinessCode,
                    BusinessId=item.BusinessId,
                    BusinessNameVi= item.BusinessNameVi,
                    DiaChi= item.DiaChi,
                    NganhNghe= item.NganhNghe,
                    NguoiDaiDien = item.NguoiDaiDien,
                    Huyen = item.Huyen,
                    Xa = item.Xa
                };
                details.Add(detail);
            }
            await _context.ParticipateSupportFairDetails.AddRangeAsync(details);
            await _context.SaveChangesAsync();
        }
        public async Task Update(ParticipateSupportFairModel model)
        {
            var del = _context.ParticipateSupportFairDetails.Where(d => d.ParticipateSupportFairId == model.ParticipateSupportFairId).ToList();
            _context.ParticipateSupportFairDetails.RemoveRange(del);
            await _context.SaveChangesAsync();

            var detailinfo = await _context.ParticipateSupportFairs.Where(d => d.ParticipateSupportFairId == model.ParticipateSupportFairId).FirstOrDefaultAsync();
            detailinfo.Address = model.Address;
            detailinfo.Country = model.Country;
            detailinfo.EndTime = model.EndTime;
            detailinfo.PlanJoin = model.PlanJoin;
            detailinfo.Scale = model.Scale;
            detailinfo.StartTime = model.StartTime;
            detailinfo.UpdateUserId = model.UpdateUserId;
            detailinfo.UpdateTime = model.UpdateTime;
            detailinfo.DistrictId = model.DistrictId;
            detailinfo.CommuneId = model.CommuneId;
            detailinfo.ImplementCost = model.ImplementCost;
            detailinfo.ParticipateSupportFairName = model.ParticipateSupportFairName;

            List<ParticipateSupportFairDetail> details = new List<ParticipateSupportFairDetail>();
            foreach (var item in model.Details)
            {
                ParticipateSupportFairDetail detail = new ParticipateSupportFairDetail()
                {
                    ParticipateSupportFairId = model.ParticipateSupportFairId,
                    BusinessCode = item.BusinessCode,
                    BusinessId = item.BusinessId,
                    BusinessNameVi = item.BusinessNameVi,
                    DiaChi = item.DiaChi,
                    NganhNghe = item.NganhNghe,
                    NguoiDaiDien = item.NguoiDaiDien,
                    Huyen = item.Huyen,
                    Xa = item.Xa
                };
                details.Add(detail);
            }
            await _context.ParticipateSupportFairDetails.AddRangeAsync(details);
            await _context.SaveChangesAsync();
        }
        public async Task Delete(Guid Id)
        {
            var detailinfo = await _context.ParticipateSupportFairs.Where(d => d.ParticipateSupportFairId == Id).FirstOrDefaultAsync();
            if (detailinfo != null)
            {
                detailinfo.IsDel = true;
                await _context.SaveChangesAsync();

                var del = _context.ParticipateSupportFairDetails.Where(d => d.ParticipateSupportFairId == Id).ToList();
                _context.ParticipateSupportFairDetails.RemoveRange(del);
                await _context.SaveChangesAsync();
            }

        }
        //public async Task Deletes(List<Guid> Ids)
        //{
        //    List<CateManageAncolLocalBussine> items = new List<CateManageAncolLocalBussine>();
        //    foreach (var idremove in Ids)
        //    {
        //        CateManageAncolLocalBussine item = new CateManageAncolLocalBussine();
        //        var detailinfo = await _context.ParticipateSupportFairs.Where(d => d.ParticipateSupportFairssId == idremove).FirstOrDefaultAsync();
        //        item.ParticipateSupportFairssId = idremove;
        //        item.IsDel = true;
        //        items.Add(item);

        //        var lstworkers = _context.ParticipateSupportFairsDetails.Where(d => d.ParticipateSupportFairssId == idremove).ToList();
        //        var lstprofession = _context.ParticipateSupportFairsTypeOfProfessions.Where(d => d.ParticipateSupportFairssId == idremove).ToList();
        //        _context.ParticipateSupportFairsDetails.RemoveRange(lstworkers);
        //        _context.ParticipateSupportFairsTypeOfProfessions.RemoveRange(lstprofession);
        //    }
        //    _context.ParticipateSupportFairs.UpdateRange(items);
        //    await _context.SaveChangesAsync();
        //}
        public ParticipateSupportFairModel FindById(Guid Id)
        {
            var result = _context.ParticipateSupportFairs.Where(x => x.ParticipateSupportFairId == Id && !x.IsDel).Select(model => new ParticipateSupportFairModel()
            {
                ParticipateSupportFairId = model.ParticipateSupportFairId,
                ParticipateSupportFairName = model.ParticipateSupportFairName,
                Address = model.Address,
                Country = model.Country,
                EndTime = model.EndTime,
                PlanJoin = model.PlanJoin,
                Scale = model.Scale,
                StartTime = model.StartTime,
                DistrictId = model.DistrictId,
                CommuneId = model.CommuneId,
                ImplementCost = model.ImplementCost,
            }).FirstOrDefault();

            if (result == null)
            {
                return new ParticipateSupportFairModel();
            }

            var details = _context.ParticipateSupportFairDetails.Where(x => x.ParticipateSupportFairId == Id).Select(x => new ParticipateSupportFairDetailModel { 
                BusinessCode= x.BusinessCode,
                BusinessId= x.BusinessId,
                BusinessNameVi=x.BusinessNameVi,
                DiaChi=x.DiaChi,
                NganhNghe=x.NganhNghe,
                NguoiDaiDien=x.NguoiDaiDien,
                ParticipateSupportFairDetailId=x.ParticipateSupportFairDetailId,
                ParticipateSupportFairId=x.ParticipateSupportFairId,
                Huyen = x.Huyen ?? "",
                Xa = x.Xa ?? ""
            });
            result.Details = details.ToList();

            return result;
        }
    }
}
