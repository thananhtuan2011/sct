using EF_Core.Models;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Runtime.CompilerServices;
using API_SoCongThuong.Models;
using DocumentFormat.OpenXml.Office2010.Excel;

namespace API_SoCongThuong.Reponsitories
{
    public class CateRPSoldAncolRepo
    {
        public SoHoa_SoCongThuongContext _context;
        public CateRPSoldAncolRepo(SoHoa_SoCongThuongContext context)
        {
            _context = context;
        }
        public async Task Insert(CateReportSoldAncolModel model)
        {
            CateReportSoldAncol data = new CateReportSoldAncol()
            {
                BusinessId = model.BusinessId,
                CateReportSoldAncolId = model.CateReportSoldAncolId,
                QuantityBoughtOfYear = model.QuantityBoughtOfYear,
                QuantitySoldOfYear = model.QuantitySoldOfYear,
                TotalPriceBoughtOfYear = model.TotalPriceBoughtOfYear,
                TotalPriceSoldOfYear = model.TotalPriceSoldOfYear,
                CreateUserId = model.CreateUserId,
                CreateTime = model.CreateTime,
                Year = model.YearId
            };
            await _context.CateReportSoldAncols.AddAsync(data);
            await _context.SaveChangesAsync();
        }
        public async Task Update(CateReportSoldAncolModel model)
        {
            var detailinfo = await _context.CateReportSoldAncols.Where(d => d.CateReportSoldAncolId == model.CateReportSoldAncolId).FirstOrDefaultAsync();
            detailinfo.BusinessId = model.BusinessId;
            detailinfo.QuantityBoughtOfYear = model.QuantityBoughtOfYear;
            detailinfo.QuantitySoldOfYear = model.QuantitySoldOfYear;
            detailinfo.TotalPriceBoughtOfYear = model.TotalPriceBoughtOfYear;
            detailinfo.TotalPriceSoldOfYear = model.TotalPriceSoldOfYear;
            detailinfo.UpdateUserId = model.UpdateUserId;
            detailinfo.UpdateTime = model.UpdateTime;
            detailinfo.Year = model.YearId;
            await _context.SaveChangesAsync();
        }
        public async Task Delete(Guid Id)
        {
            var detailinfo = await _context.CateReportSoldAncols.Where(d => d.CateReportSoldAncolId == Id).FirstOrDefaultAsync();
            detailinfo.IsDel = true;
            await _context.SaveChangesAsync();
        }
        public async Task Deletes(List<Guid> Ids)
        {
            List<CateReportSoldAncol> items = new List<CateReportSoldAncol>();
            foreach (var idremove in Ids)
            {
                CateReportSoldAncol item = new CateReportSoldAncol();
                var detailinfo = await _context.CateReportSoldAncols.Where(d => d.CateReportSoldAncolId == idremove).FirstOrDefaultAsync();
                item.CateReportSoldAncolId = idremove;
                item.IsDel = true;
                items.Add(item);
            }
            _context.CateReportSoldAncols.UpdateRange(items);
            await _context.SaveChangesAsync();
        }
        public CateReportSoldAncol FindById(Guid Id)
        {
            var result = _context.CateReportSoldAncols.Where(x => x.CateReportSoldAncolId == Id && !x.IsDel).Select(d => new CateReportSoldAncol()
            {
                CateReportSoldAncolId = d.CateReportSoldAncolId,
                BusinessId = d.BusinessId,
                QuantityBoughtOfYear = d.QuantityBoughtOfYear,
                QuantitySoldOfYear = d.QuantitySoldOfYear,
                TotalPriceBoughtOfYear = d.TotalPriceBoughtOfYear,
                TotalPriceSoldOfYear = d.TotalPriceSoldOfYear,
                IsAction = d.IsAction,
                Year = d.Year
            }).FirstOrDefault();

            return result;
        }

        public List<CateReportSoldAncolModel> FindData(QueryRequestBody query)
        {
            List<CateReportSoldAncolModel> result = new List<CateReportSoldAncolModel>();

            //IQueryable<CateReportSoldAncol> _data = _context.CateReportSoldAncols.Where(x => !x.IsDel).Select(d => new CateReportSoldAncol()
            //{
            //    CateReportSoldAncolId = d.CateReportSoldAncolId,
            //    Address = d.Address,
            //    AlcoholBusinessName = d.AlcoholBusinessName,
            //    LicenseCode = d.LicenseCode,
            //    LicenseDate = d.LicenseDate,
            //    PhoneNumber = d.PhoneNumber,
            //    QuantityBoughtOfYear = d.QuantityBoughtOfYear,
            //    QuantitySoldOfYear = d.QuantitySoldOfYear,
            //    TotalPriceBoughtOfYear = d.TotalPriceBoughtOfYear,
            //    TotalPriceSoldOfYear = d.TotalPriceSoldOfYear,
            //    IsAction = d.IsAction,
            //    BusinessId = d.BusinessId,
            //    Year = d.Year
            //}).ToList().AsQueryable();

            IQueryable<CateReportSoldAncolModel> _data = (from cc in _context.CateReportSoldAncols
                                                          where !cc.IsDel
                                                          join u in _context.Users on cc.CreateUserId equals u.UserId into cuGroup
                                                          from cu in cuGroup.DefaultIfEmpty()
                                                          join b in _context.Businesses on cc.BusinessId equals b.BusinessId into cuBusiness
                                                          from bu in cuBusiness.DefaultIfEmpty()
                                                          select new CateReportSoldAncolModel
                                                          {
                                                              CateReportSoldAncolId = cc.CateReportSoldAncolId,

                                                              BusinessId = cc.BusinessId,
                                                              AlcoholBusinessName = bu.BusinessNameVi,
                                                              Address = bu.DiaChiTruSo,
                                                              PhoneNumber = bu.SoDienThoai,
                                                              LicenseCode = bu.GiayPhepSanXuat ?? "",
                                                              LicenseDate = bu.NgayCapPhep,
                                                              LicenseDateDisplay = bu.NgayCapPhep.HasValue ? bu.NgayCapPhep.Value.ToString("dd'/'MM'/'yyyy") : "",

                                                              QuantityBoughtOfYear = cc.QuantityBoughtOfYear,
                                                              QuantitySoldOfYear = cc.QuantitySoldOfYear,
                                                              TotalPriceBoughtOfYear = cc.TotalPriceBoughtOfYear,
                                                              TotalPriceSoldOfYear = cc.TotalPriceSoldOfYear,
                                                              YearId = cc.Year,
                                                              CreateName = cu.FullName,
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
