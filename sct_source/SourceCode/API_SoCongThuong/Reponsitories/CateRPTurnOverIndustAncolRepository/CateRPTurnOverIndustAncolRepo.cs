using EF_Core.Models;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.Xml;
using API_SoCongThuong.Models;
using DocumentFormat.OpenXml.Office2010.Excel;

namespace API_SoCongThuong.Reponsitories
{
    public class CateRPTurnOverIndustAncolRepo
    {
        public SoHoa_SoCongThuongContext _context;
        public CateRPTurnOverIndustAncolRepo(SoHoa_SoCongThuongContext context)
        {
            _context = context;
        }
        public async Task Insert(CateReportTurnOverIndustAncolModel model)
        {
            CateReportTurnOverIndustAncol data = new CateReportTurnOverIndustAncol()
            {
                CateReportTurnOverIndustAncolId = model.CateReportTurnOverIndustAncolId,
                QuantityBoughtOfYear = model.QuantityBoughtOfYear,
                QuantitySoldOfYear = model.QuantitySoldOfYear,
                TotalPriceBoughtOfYear = model.TotalPriceBoughtOfYear,
                TotalPriceSoldOfYear = model.TotalPriceSoldOfYear,
                CreateUserId = model.CreateUserId,
                CreateTime = model.CreateTime,
                BusinessId = model.BusinessId,
                Year = model.YearId
            };
            await _context.CateReportTurnOverIndustAncols.AddAsync(data);
            await _context.SaveChangesAsync();
        }
        public async Task Update(CateReportTurnOverIndustAncolModel model)
        {
            var detailinfo = await _context.CateReportTurnOverIndustAncols.Where(d => d.CateReportTurnOverIndustAncolId == model.CateReportTurnOverIndustAncolId).FirstOrDefaultAsync();
            detailinfo.QuantityBoughtOfYear = model.QuantityBoughtOfYear;
            detailinfo.QuantitySoldOfYear = model.QuantitySoldOfYear;
            detailinfo.TotalPriceBoughtOfYear = model.TotalPriceBoughtOfYear;
            detailinfo.TotalPriceSoldOfYear = model.TotalPriceSoldOfYear;
            detailinfo.UpdateUserId = model.UpdateUserId;
            detailinfo.UpdateTime = model.UpdateTime;
            detailinfo.BusinessId = model.BusinessId;
            detailinfo.Year = model.YearId;
            await _context.SaveChangesAsync();
        }
        public async Task Delete(Guid Id)
        {
            var detailinfo = await _context.CateReportTurnOverIndustAncols.Where(d => d.CateReportTurnOverIndustAncolId == Id).FirstOrDefaultAsync();
            detailinfo.IsDel = true;
            await _context.SaveChangesAsync();
        }
        public async Task Deletes(List<Guid> Ids)
        {
            List<CateReportTurnOverIndustAncol> items = new List<CateReportTurnOverIndustAncol>();
            foreach (var idremove in Ids)
            {
                CateReportTurnOverIndustAncol item = new CateReportTurnOverIndustAncol();
                var detailinfo = await _context.CateReportTurnOverIndustAncols.Where(d => d.CateReportTurnOverIndustAncolId == idremove).FirstOrDefaultAsync();
                item.CateReportTurnOverIndustAncolId = idremove;
                item.IsDel = true;
                items.Add(item);
            }
            _context.CateReportTurnOverIndustAncols.UpdateRange(items);
            await _context.SaveChangesAsync();
        }
        public CateReportTurnOverIndustAncol FindById(Guid Id)
        {
            var result = _context.CateReportTurnOverIndustAncols.Where(x => x.CateReportTurnOverIndustAncolId == Id && !x.IsDel).Select(d => new CateReportTurnOverIndustAncol()
            {
                CateReportTurnOverIndustAncolId = d.CateReportTurnOverIndustAncolId,
                QuantityBoughtOfYear = d.QuantityBoughtOfYear,
                QuantitySoldOfYear = d.QuantitySoldOfYear,
                TotalPriceBoughtOfYear = d.TotalPriceBoughtOfYear,
                TotalPriceSoldOfYear = d.TotalPriceSoldOfYear,
                IsAction = d.IsAction,
                BusinessId = d.BusinessId,
                Year = d.Year
            }).FirstOrDefault();

            return result;
        }

        public List<CateReportTurnOverIndustAncolModel> FindData(QueryRequestBody query)
        {
            List<CateReportTurnOverIndustAncolModel> result = new List<CateReportTurnOverIndustAncolModel>();

            var _data = (from cc in _context.CateReportTurnOverIndustAncols
                         where !cc.IsDel
                         join u in _context.Users on cc.CreateUserId equals u.UserId into cu
                         from us in cu.DefaultIfEmpty()
                         join b in _context.Businesses on cc.BusinessId equals b.BusinessId into JoinBusiness
                         from bu in JoinBusiness.DefaultIfEmpty()
                         select new CateReportTurnOverIndustAncolModel
                         {
                             CateReportTurnOverIndustAncolId = cc.CateReportTurnOverIndustAncolId,
                             BusinessId = cc.BusinessId,
                             AlcoholBusinessName = bu.BusinessNameVi,
                             PhoneNumber = bu.SoDienThoai,
                             Address = bu.DiaChi,
                             LicenseCode = bu.GiayPhepSanXuat ?? "",
                             LicenseDate = bu.NgayCapPhep,
                             LicenseDateDisplay = bu.NgayCapPhep.HasValue ? bu.NgayCapPhep.Value.ToString("dd'/'MM'/'yyyy") : "",

                             QuantityBoughtOfYear = cc.QuantityBoughtOfYear,
                             QuantitySoldOfYear = cc.QuantitySoldOfYear,
                             TotalPriceBoughtOfYear = cc.TotalPriceBoughtOfYear,
                             TotalPriceSoldOfYear = cc.TotalPriceSoldOfYear,
                             YearId = cc.Year,

                             CreateName = us.FullName,
                             CreateTimeDisplay = cc.CreateTime.ToString("dd/MM/yyyy hh:mm"),
                         }).ToList().AsQueryable();

            string _keywordSearch = "";
            if (query.SearchValue != null && query.SearchValue != "")
            {
                _keywordSearch = query.SearchValue.Trim().ToLower();
                _data = _data.Where(x => x.AlcoholBusinessName.ToLower().Contains(_keywordSearch)
                || x.Address.ToLower().Contains(_keywordSearch)
                || x.PhoneNumber.ToLower().Contains(_keywordSearch)
                || x.QuantityBoughtOfYear.ToString().Contains(_keywordSearch)
                || x.QuantitySoldOfYear.ToString().Contains(_keywordSearch)
                || x.TotalPriceBoughtOfYear.ToString().Contains(_keywordSearch)
                || x.TotalPriceSoldOfYear.ToString().Contains(_keywordSearch)
                || x.LicenseCode.ToLower().Contains(_keywordSearch)
                );
            }

            //Filter
            if (query.Filter != null && query.Filter.ContainsKey("YearReport"))
            {
                _data = _data.Where(x => x.YearId.ToString() == query.Filter["YearReport"]);
            }
            else
            {
                _data = _data.Where(x => x.YearId == DateTime.Now.Year);
            }

            result = _data.ToList();

            return result;
        }
    }
}
