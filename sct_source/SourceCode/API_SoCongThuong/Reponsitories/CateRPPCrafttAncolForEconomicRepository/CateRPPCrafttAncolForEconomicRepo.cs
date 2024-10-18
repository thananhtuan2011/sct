using API_SoCongThuong.Models;
using EF_Core.Models;
using Microsoft.EntityFrameworkCore;

namespace API_SoCongThuong.Reponsitories
{
    public class CateRPPCrafttAncolForEconomicRepo
    {
        public SoHoa_SoCongThuongContext _context;
        public CateRPPCrafttAncolForEconomicRepo(SoHoa_SoCongThuongContext context)
        {
            _context = context;
        }

        public async Task Insert(CateReportProduceCrafttAncolForEconomicModel model)
        {
            CateReportProduceCrafttAncolForEconomic data = new CateReportProduceCrafttAncolForEconomic()
            {
                CateReportProduceCrafttAncolForEconomicId = model.CateReportProduceCrafttAncolForEconomicId,
                BusinessId = model.BusinessId,
                Quantity = model.Quantity,
                QuantityConsume = model.QuantityConsume,
                TypeofWine = model.TypeofWine,
                YearReport = model.YearReport,
                CreateUserId = model.CreateUserId,
                CreateTime = model.CreateTime,

            };
            await _context.CateReportProduceCrafttAncolForEconomics.AddAsync(data);
            await _context.SaveChangesAsync();
        }

        public async Task Update(CateReportProduceCrafttAncolForEconomicModel model)
        {
            var detailinfo = await _context.CateReportProduceCrafttAncolForEconomics.Where(d => d.CateReportProduceCrafttAncolForEconomicId == model.CateReportProduceCrafttAncolForEconomicId).FirstOrDefaultAsync();
            detailinfo.BusinessId = model.BusinessId;
            detailinfo.Quantity = model.Quantity;
            detailinfo.QuantityConsume = model.QuantityConsume;
            detailinfo.TypeofWine = model.TypeofWine;
            detailinfo.YearReport = model.YearReport;
            detailinfo.UpdateUserId = model.UpdateUserId;
            detailinfo.UpdateTime = model.UpdateTime;
            await _context.SaveChangesAsync();
        }

        public async Task Delete(Guid Id)
        {
            var detailinfo = await _context.CateReportProduceCrafttAncolForEconomics.Where(d => d.CateReportProduceCrafttAncolForEconomicId == Id).FirstOrDefaultAsync();
            detailinfo.IsDel = true;
            await _context.SaveChangesAsync();
        }

        public async Task Deletes(List<Guid> Ids)
        {
            List<CateReportProduceCrafttAncolForEconomic> items = new List<CateReportProduceCrafttAncolForEconomic>();
            foreach (var idremove in Ids)
            {
                CateReportProduceCrafttAncolForEconomic item = new CateReportProduceCrafttAncolForEconomic();
                var detailinfo = await _context.CateReportProduceCrafttAncolForEconomics.Where(d => d.CateReportProduceCrafttAncolForEconomicId == idremove).FirstOrDefaultAsync();
                item.CateReportProduceCrafttAncolForEconomicId = idremove;
                item.IsDel = true;
                items.Add(item);
            }
            _context.CateReportProduceCrafttAncolForEconomics.UpdateRange(items);
            await _context.SaveChangesAsync();
        }
        public CateReportProduceCrafttAncolForEconomic FindById(Guid Id)
        {
            var result = _context.CateReportProduceCrafttAncolForEconomics.Where(x => x.CateReportProduceCrafttAncolForEconomicId == Id && !x.IsDel).Select(d => new CateReportProduceCrafttAncolForEconomic()
            {
                CateReportProduceCrafttAncolForEconomicId = d.CateReportProduceCrafttAncolForEconomicId,
                BusinessId = d.BusinessId,
                TypeofWine = d.TypeofWine,
                Quantity = d.Quantity,
                QuantityConsume = d.QuantityConsume,
                YearReport = d.YearReport,
            }).FirstOrDefault();

            return result;
        }

        public List<CateReportProduceCrafttAncolForEconomicModel> FindDataForEconomic(QueryRequestBody query)
        {
            List<CateReportProduceCrafttAncolForEconomicModel> result = new List<CateReportProduceCrafttAncolForEconomicModel>();

            IQueryable<CateReportProduceCrafttAncolForEconomicModel> _data = (from cc in _context.CateReportProduceCrafttAncolForEconomics
                                                                              where !cc.IsDel
                                                                              join u in _context.Users on cc.CreateUserId equals u.UserId into JoinUser
                                                                              from us in JoinUser.DefaultIfEmpty()
                                                                              join b in _context.Businesses on cc.BusinessId equals b.BusinessId into JoinBusinesses
                                                                              from bu in JoinBusinesses.DefaultIfEmpty()
                                                                              join d in _context.Districts on bu.DistrictId equals d.DistrictId into JoinDistricts
                                                                              from di in JoinDistricts.DefaultIfEmpty()
                                                                              select new CateReportProduceCrafttAncolForEconomicModel
                                                                              {
                                                                                  CateReportProduceCrafttAncolForEconomicId = cc.CateReportProduceCrafttAncolForEconomicId,

                                                                                  AlcoholBusinessName = bu.BusinessNameVi,
                                                                                  PhoneNumber = bu.SoDienThoai,
                                                                                  Address = bu.DiaChiTruSo,
                                                                                  LicenseCode = bu.GiayPhepSanXuat ?? "",
                                                                                  LicenseDate = bu.NgayCapPhep,
                                                                                  LicenseDateDisplay = bu.NgayCapPhep.HasValue ? bu.NgayCapPhep.Value.ToString("dd'/'MM'/'yyyy") : "",
                                                                                  Representative = bu.NguoiDaiDien,
                                                                                  DistrictName = di.DistrictName,

                                                                                  BusinessId = cc.BusinessId,
                                                                                  Quantity = cc.Quantity,
                                                                                  QuantityConsume = cc.QuantityConsume,
                                                                                  TypeofWine = cc.TypeofWine,
                                                                                  YearReport = cc.YearReport,

                                                                                  CreateName = us.FullName,
                                                                                  CreateTimeDisplay = cc.CreateTime.ToString("dd/MM/yyyy hh:mm")
                                                                              })
                                                                              .ToList().AsQueryable();

            string _keywordSearch = "";
            if (query.SearchValue != null && query.SearchValue != "")
            {
                _keywordSearch = query.SearchValue.Trim().ToLower();
                _data = _data.Where(x => x.AlcoholBusinessName.ToLower().Contains(_keywordSearch)
                || x.Address.ToLower().Contains(_keywordSearch)
                || x.PhoneNumber.ToLower().Contains(_keywordSearch)
                || x.Quantity.ToString().Contains(_keywordSearch)
                || x.TypeofWine.ToLower().Contains(_keywordSearch)
                || x.LicenseCode.ToLower().Contains(_keywordSearch)
                );
            }

            //Filter
            if (query.Filter != null && query.Filter.ContainsKey("YearReport"))
            {
                _data = _data.Where(x => x.YearReport.ToString() == query.Filter["YearReport"]);
            }
            else
            {
                _data = _data.Where(x => x.YearReport == DateTime.Now.Year);
            }

            result = _data.ToList();

            return result;
        }

        public List<CateReportSoldAncolModel> FindDataForSoldAncols(QueryRequestBody query)
        {
            List<CateReportSoldAncolModel> result = new List<CateReportSoldAncolModel>();

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
