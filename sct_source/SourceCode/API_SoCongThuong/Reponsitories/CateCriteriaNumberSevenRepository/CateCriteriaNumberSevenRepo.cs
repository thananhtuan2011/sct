using API_SoCongThuong.Models;
using EF_Core.Models;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Runtime.CompilerServices;

namespace API_SoCongThuong.Reponsitories
{
    public class CateCriteriaNumberSevenRepo
    {
        public SoHoa_SoCongThuongContext _context;
        public CateCriteriaNumberSevenRepo(SoHoa_SoCongThuongContext context)
        {
            _context = context;
        }
        public async Task Insert(CateCriteriaNumberSevenModel model)
        {
            CateCriteriaNumberSeven data = new CateCriteriaNumberSeven()
            {
                CateCriteriaNumberSevenId = model.CateCriteriaNumberSevenId,
                CateCriteriaNumberSevenCode = model.CateCriteriaNumberSevenCode,
                CheckUserId = model.CheckUserId,
                ConfirmTime = model.ConfirmTime,
                ConfirmUserId = model.ConfirmUserId,
                CreateUserId = model.CreateUserId,
                CreateTime = model.CreateTime,
                ReportMonth = model.ReportMonth
            };
            await _context.CateCriteriaNumberSevens.AddAsync(data);
            await _context.SaveChangesAsync();
            List<CateCriteriaNumberSevenDetail> details = new List<CateCriteriaNumberSevenDetail>();
            foreach (var item in model.Details)
            {
                CateCriteriaNumberSevenDetail detail = new CateCriteriaNumberSevenDetail()
                {
                    CateCriteriaNumberSevenId = data.CateCriteriaNumberSevenId,
                    DistrictId = item.DistrictId,
                    NumberOfQualifyingWard = item.NumberOfQualifyingWard,
                    NumberOfWard = item.NumberOfWard,
                    NumberOfWardCommercialInfrastructure = item.NumberOfWardCommercialInfrastructure,
                    NumberOfWardCommercialInfrastructureEstimate = item.NumberOfWardCommercialInfrastructureEstimate,
                    NumberOfWardCommercialInfrastructurePlan = item.NumberOfWardCommercialInfrastructurePlan,
                    NumberOfWardNewCountryside = item.NumberOfWardNewCountryside,
                    NumberOfWardNewCountrysideEstimate = item.NumberOfWardNewCountrysideEstimate,
                    NumberOfWardNewCountrysidePlan = item.NumberOfWardNewCountrysidePlan,
                    NumberOfWardWithMarket = item.NumberOfWardWithMarket
                };
                details.Add(detail);
            }
            await _context.CateCriteriaNumberSevenDetails.AddRangeAsync(details);
            await _context.SaveChangesAsync();
        }
        public async Task Update(CateCriteriaNumberSevenModel model)
        {
            var details = _context.CateCriteriaNumberSevenDetails.Where(d => d.CateCriteriaNumberSevenId == model.CateCriteriaNumberSevenId).ToList();
            _context.CateCriteriaNumberSevenDetails.RemoveRange(details);
            await _context.SaveChangesAsync();

            var detailinfo = await _context.CateCriteriaNumberSevens.Where(d => d.CateCriteriaNumberSevenId == model.CateCriteriaNumberSevenId).FirstOrDefaultAsync();
            detailinfo.CateCriteriaNumberSevenCode = model.CateCriteriaNumberSevenCode;
            detailinfo.CheckUserId = model.CheckUserId;
            detailinfo.ConfirmTime = model.ConfirmTime;
            detailinfo.ConfirmUserId = model.ConfirmUserId;
            detailinfo.CreateUserId = model.CreateUserId;
            detailinfo.CreateTime = model.CreateTime;
            detailinfo.UpdateUserId = model.UpdateUserId;
            detailinfo.UpdateTime = model.UpdateTime;
            detailinfo.ReportMonth = model.ReportMonth;

            List<CateCriteriaNumberSevenDetail> Lstdetails = new List<CateCriteriaNumberSevenDetail>();
            foreach (var item in model.Details)
            {
                CateCriteriaNumberSevenDetail detail = new CateCriteriaNumberSevenDetail()
                {
                    CateCriteriaNumberSevenId = model.CateCriteriaNumberSevenId,
                    DistrictId = item.DistrictId,
                    NumberOfQualifyingWard = item.NumberOfQualifyingWard,
                    NumberOfWard = item.NumberOfWard,
                    NumberOfWardCommercialInfrastructure = item.NumberOfWardCommercialInfrastructure,
                    NumberOfWardCommercialInfrastructureEstimate = item.NumberOfWardCommercialInfrastructureEstimate,
                    NumberOfWardCommercialInfrastructurePlan = item.NumberOfWardCommercialInfrastructurePlan,
                    NumberOfWardNewCountryside = item.NumberOfWardNewCountryside,
                    NumberOfWardNewCountrysideEstimate = item.NumberOfWardNewCountrysideEstimate,
                    NumberOfWardNewCountrysidePlan = item.NumberOfWardNewCountrysidePlan,
                    NumberOfWardWithMarket = item.NumberOfWardWithMarket
                };
                Lstdetails.Add(detail);
            }
            await _context.CateCriteriaNumberSevenDetails.AddRangeAsync(Lstdetails);
            await _context.SaveChangesAsync();
        }
        public async Task Delete(Guid Id)
        {
            var detailinfo = await _context.CateCriteriaNumberSevens.Where(d => d.CateCriteriaNumberSevenId == Id).FirstOrDefaultAsync();
            detailinfo.IsDel = true;

            var details = _context.CateCriteriaNumberSevenDetails.Where(d => d.CateCriteriaNumberSevenId == Id).ToList();
            _context.CateCriteriaNumberSevenDetails.RemoveRange(details);

            await _context.SaveChangesAsync();
        }
        //public async Task Deletes(List<Guid> Ids)
        //{
        //    List<CateManageAncolLocalBussine> items = new List<CateManageAncolLocalBussine>();
        //    foreach (var idremove in Ids)
        //    {
        //        CateManageAncolLocalBussine item = new CateManageAncolLocalBussine();
        //        var detailinfo = await _context.CateCriteriaNumberSevens.Where(d => d.CateCriteriaNumberSevenssId == idremove).FirstOrDefaultAsync();
        //        item.CateCriteriaNumberSevenssId = idremove;
        //        item.IsDel = true;
        //        items.Add(item);

        //        var details = _context.CateCriteriaNumberSevensDetails.Where(d => d.CateCriteriaNumberSevenssId == idremove).ToList();
        //        _context.CateCriteriaNumberSevensDetails.RemoveRange(details);
        //    }
        //    _context.CateCriteriaNumberSevens.UpdateRange(items);
        //    await _context.SaveChangesAsync();
        //}
        public CateCriteriaNumberSevenModel FindById(Guid Id)
        {
            var result = _context.CateCriteriaNumberSevens.Where(x => x.CateCriteriaNumberSevenId == Id && !x.IsDel).Select(d => new CateCriteriaNumberSevenModel()
            {
                CateCriteriaNumberSevenId = Id,
                CateCriteriaNumberSevenCode = d.CateCriteriaNumberSevenCode,
                CheckUserId = d.CheckUserId,
                ConfirmTime = d.ConfirmTime,
                ConfirmUserId = d.ConfirmUserId,
                CreateTime = d.CreateTime,
                CreateUserId = d.CreateUserId,
                ReportMonth = d.ReportMonth
            }).FirstOrDefault();

            if (result == null)
            {
                return new CateCriteriaNumberSevenModel();
            }

            var details = _context.CateCriteriaNumberSevenDetails.Where(x => x.CateCriteriaNumberSevenId == Id).Join(
                _context.Districts, cc => cc.DistrictId, dd => dd.DistrictId,
                (cc, dd) => new CateCriteriaNumberSevenDetailModel()
                {
                    DistrictId = cc.DistrictId,
                    CateCriteriaNumberSevenDetailId = cc.CateCriteriaNumberSevenDetailId,
                    DistrictName = dd.DistrictName,
                    NumberOfQualifyingWard = cc.NumberOfQualifyingWard,
                    NumberOfWard = cc.NumberOfWard,
                    NumberOfWardWithMarket = cc.NumberOfWardWithMarket,
                    NumberOfWardCommercialInfrastructure = cc.NumberOfWardCommercialInfrastructure,
                    NumberOfWardCommercialInfrastructureEstimate = cc.NumberOfWardCommercialInfrastructureEstimate,
                    NumberOfWardCommercialInfrastructurePlan = cc.NumberOfWardCommercialInfrastructurePlan,
                    NumberOfWardNewCountryside = cc.NumberOfWardNewCountryside,
                    NumberOfWardNewCountrysideEstimate = cc.NumberOfWardNewCountrysideEstimate,
                    NumberOfWardNewCountrysidePlan = cc.NumberOfWardNewCountrysidePlan,
                }
            ).ToList();

            result.Details = details;
            return result;
        }

        #region get danh sách district
        public IQueryable<District> FindDistrict()
        {
            var result = _context.Districts.Where(x => !x.IsDel).Select(d => new District()
            {
                DistrictId = d.DistrictId,
                DistrictName = d.DistrictName,
                IsDel = d.IsDel,
            });

            return result;
        }
        #endregion
    }
}
