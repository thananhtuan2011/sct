using EF_Core.Models;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Runtime.CompilerServices;
using API_SoCongThuong.Models;
using DocumentFormat.OpenXml.Office2010.Excel;

namespace API_SoCongThuong.Reponsitories
{
    public class CateRPProduceIndustlAncolRepo
    {
        public SoHoa_SoCongThuongContext _context;
        public CateRPProduceIndustlAncolRepo(SoHoa_SoCongThuongContext context)
        {
            _context = context;
        }

        public async Task Insert(CateReportProduceIndustlAncolModel model)
        {
            CateReportProduceIndustlAncol data = new CateReportProduceIndustlAncol()
            {
                CateReportProduceIndustlAncolId = model.CateReportProduceIndustlAncolId,
                BusinessId = model.BusinessId,
                TypeofWine = model.TypeofWine,
                DesignCapacity = model.DesignCapacity,
                Investment = model.Investment,
                QuantityConsume = model.QuantityConsume,
                QuantityProduction = model.QuantityProduction,
                CreateUserId = model.CreateUserId,
                CreateTime = model.CreateTime,
                YearReport = model.YearReport,
            };
            await _context.CateReportProduceIndustlAncols.AddAsync(data);
            await _context.SaveChangesAsync();
        }

        public async Task Update(CateReportProduceIndustlAncolModel model)
        {
            var detailinfo = await _context.CateReportProduceIndustlAncols.Where(d => d.CateReportProduceIndustlAncolId == model.CateReportProduceIndustlAncolId).FirstOrDefaultAsync();
            detailinfo.BusinessId = model.BusinessId;
            detailinfo.TypeofWine = model.TypeofWine;
            detailinfo.DesignCapacity = model.DesignCapacity;
            detailinfo.Investment = model.Investment;
            detailinfo.QuantityConsume = model.QuantityConsume;
            detailinfo.QuantityProduction = model.QuantityProduction;
            detailinfo.UpdateUserId = model.UpdateUserId;
            detailinfo.UpdateTime = model.UpdateTime;
            detailinfo.YearReport = model.YearReport;
            await _context.SaveChangesAsync();
        }

        public async Task Delete(Guid Id)
        {
            var detailinfo = await _context.CateReportProduceIndustlAncols.Where(d => d.CateReportProduceIndustlAncolId == Id).FirstOrDefaultAsync();
            detailinfo.IsDel = true;
            await _context.SaveChangesAsync();
        }

        public async Task Deletes(List<Guid> Ids)
        {
            List<CateReportProduceIndustlAncol> items = new List<CateReportProduceIndustlAncol>();
            foreach (var idremove in Ids)
            {
                CateReportProduceIndustlAncol item = new CateReportProduceIndustlAncol();
                var detailinfo = await _context.CateReportProduceIndustlAncols.Where(d => d.CateReportProduceIndustlAncolId == idremove).FirstOrDefaultAsync();
                item.CateReportProduceIndustlAncolId = idremove;
                item.IsDel = true;
                items.Add(item);
            }
            _context.CateReportProduceIndustlAncols.UpdateRange(items);
            await _context.SaveChangesAsync();
        }

        public CateReportProduceIndustlAncolModel FindById(Guid Id)
        {
            var result = _context.CateReportProduceIndustlAncols.Where(x => x.CateReportProduceIndustlAncolId == Id && !x.IsDel).Select(d => new CateReportProduceIndustlAncolModel()
            {
                CateReportProduceIndustlAncolId = d.CateReportProduceIndustlAncolId,
                BusinessId = d.BusinessId,
                DesignCapacity = d.DesignCapacity,
                QuantityConsume = d.QuantityConsume,
                QuantityProduction = d.QuantityProduction,
                Investment = d.Investment,
                TypeofWine = d.TypeofWine,
                YearReport = d.YearReport,
            }).FirstOrDefault();



            return result;
        }

        public List<CateReportProduceIndustlAncolModel> FindData(QueryRequestBody query)
        {
            List<CateReportProduceIndustlAncolModel> result = new List<CateReportProduceIndustlAncolModel>();

            IQueryable<CateReportProduceIndustlAncolModel> _data = (from crp in _context.CateReportProduceIndustlAncols
                                                                    where !crp.IsDel
                                                                    join b in _context.Businesses on crp.BusinessId equals b.BusinessId into JoinBu
                                                                    from bu in JoinBu.DefaultIfEmpty()
                                                                    join d in _context.Districts on bu.DistrictId equals d.DistrictId into JoinDis
                                                                    from dis in JoinDis.DefaultIfEmpty()
                                                                    select new CateReportProduceIndustlAncolModel
                                                                    {
                                                                        CateReportProduceIndustlAncolId = crp.CateReportProduceIndustlAncolId,

                                                                        BusinessId = crp.BusinessId,
                                                                        AlcoholBusinessName = bu.BusinessNameVi,
                                                                        Address = bu.DiaChiTruSo ?? "",
                                                                        Representative = bu.NguoiDaiDien,
                                                                        PhoneNumber = bu.SoDienThoai ?? "",
                                                                        LicenseCode = bu.GiayPhepSanXuat ?? "",
                                                                        LicenseDate = bu.NgayCapPhep,


                                                                        DesignCapacity = crp.DesignCapacity,
                                                                        QuantityConsume = crp.QuantityConsume,
                                                                        QuantityProduction = crp.QuantityProduction,
                                                                        Investment = crp.Investment,
                                                                        TypeofWine = crp.TypeofWine,
                                                                        YearReport = crp.YearReport,
                                                                    }).ToList().AsQueryable();

            string _keywordSearch = "";
            if (query.SearchValue != null && query.SearchValue != "")
            {
                _keywordSearch = query.SearchValue.Trim().ToLower();
                _data = _data.Where(x => x.AlcoholBusinessName.ToLower().Contains(_keywordSearch)
                || x.Address.ToLower().Contains(_keywordSearch)
                || x.PhoneNumber.ToLower().Contains(_keywordSearch)
                || x.DesignCapacity.ToLower().Contains(_keywordSearch)
                || x.QuantityConsume.ToString().Equals(_keywordSearch)
                || x.QuantityProduction.ToString().Equals(_keywordSearch)
                || x.Investment.ToString().Contains(_keywordSearch)
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
    }
}
