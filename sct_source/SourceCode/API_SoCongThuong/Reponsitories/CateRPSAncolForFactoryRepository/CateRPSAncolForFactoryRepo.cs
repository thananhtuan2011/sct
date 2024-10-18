using EF_Core.Models;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;
using API_SoCongThuong.Models;

namespace API_SoCongThuong.Reponsitories
{
    public class CateRPSAncolForFactoryRepo
    {
        public SoHoa_SoCongThuongContext _context;
        public CateRPSAncolForFactoryRepo(SoHoa_SoCongThuongContext context)
        {
            _context = context;
        }

        public async Task Insert(CateReportSoldAncolForFactoryLicenseModel model)
        {
            CateReportSoldAncolForFactoryLicense data = new CateReportSoldAncolForFactoryLicense()
            {
                CateReportSoldAncolForFactoryLicenseId = model.CateReportSoldAncolForFactoryLicenseId,
                BusinessId = model.BusinessId,
                Quantity = model.Quantity,
                TypeofWine = model.TypeofWine,
                WineFactory = model.WineFactory,
                CreateUserId = model.CreateUserId,
                CreateTime = model.CreateTime,
                YearReport = model.YearReport,
            };
            await _context.CateReportSoldAncolForFactoryLicenses.AddAsync(data);
            await _context.SaveChangesAsync();
        }

        public async Task Update(CateReportSoldAncolForFactoryLicenseModel model)
        {
            var detailinfo = await _context.CateReportSoldAncolForFactoryLicenses.Where(d => d.CateReportSoldAncolForFactoryLicenseId == model.CateReportSoldAncolForFactoryLicenseId).FirstOrDefaultAsync();
            detailinfo.BusinessId = model.BusinessId;
            detailinfo.Quantity = model.Quantity;
            detailinfo.TypeofWine = model.TypeofWine;
            detailinfo.WineFactory = model.WineFactory;
            detailinfo.UpdateUserId = model.UpdateUserId;
            detailinfo.UpdateTime = model.UpdateTime;
            detailinfo.YearReport = model.YearReport;
            await _context.SaveChangesAsync();
        }

        public async Task Delete(Guid Id)
        {
            var detailinfo = await _context.CateReportSoldAncolForFactoryLicenses.Where(d => d.CateReportSoldAncolForFactoryLicenseId == Id).FirstOrDefaultAsync();
            detailinfo.IsDel = true;
            await _context.SaveChangesAsync();
        }

        public async Task Deletes(List<Guid> Ids)
        {
            List<CateReportSoldAncolForFactoryLicense> items = new List<CateReportSoldAncolForFactoryLicense>();
            foreach (var idremove in Ids)
            {
                CateReportSoldAncolForFactoryLicense item = new CateReportSoldAncolForFactoryLicense();
                var detailinfo = await _context.CateReportSoldAncolForFactoryLicenses.Where(d => d.CateReportSoldAncolForFactoryLicenseId == idremove).FirstOrDefaultAsync();
                item.CateReportSoldAncolForFactoryLicenseId = idremove;
                item.IsDel = true;
                items.Add(item);
            }
            _context.CateReportSoldAncolForFactoryLicenses.UpdateRange(items);
            await _context.SaveChangesAsync();
        }

        public CateReportSoldAncolForFactoryLicense FindById(Guid Id)
        {
            var result = _context.CateReportSoldAncolForFactoryLicenses.Where(x => x.CateReportSoldAncolForFactoryLicenseId == Id && !x.IsDel).Select(d => new CateReportSoldAncolForFactoryLicense()
            {
                CateReportSoldAncolForFactoryLicenseId = d.CateReportSoldAncolForFactoryLicenseId,
                Quantity = d.Quantity,
                TypeofWine = d.TypeofWine,
                WineFactory = d.WineFactory,
                IsAction = d.IsAction,
                BusinessId = d.BusinessId,
                YearReport = d.YearReport,
            }).FirstOrDefault();

            return result;
        }

        public List<CateReportSoldAncolForFactoryLicenseModel> FindData(QueryRequestBody query)
        {
            List<CateReportSoldAncolForFactoryLicenseModel> result = new List<CateReportSoldAncolForFactoryLicenseModel>();

            IQueryable<CateReportSoldAncolForFactoryLicenseModel> _data = _context.CateReportSoldAncolForFactoryLicenses
            .Where(x => !x.IsDel).GroupJoin(
                _context.Users,
                cc => cc.CreateUserId,
                u => u.UserId,
                (cc, u) => new { cc, u })
            .SelectMany(
                result => result.u.DefaultIfEmpty(),
                (info, us) => new { info, us })
            .GroupJoin(
                _context.Businesses, ifo => ifo.info.cc.BusinessId,
                bu => bu.BusinessId,
                (ifo, bu) => new { ifo, bu })
            .SelectMany(
                result => result.bu.DefaultIfEmpty(),
                (ifo1, bu) => new CateReportSoldAncolForFactoryLicenseModel
                {
                    CateReportSoldAncolForFactoryLicenseId = ifo1.ifo.info.cc.CateReportSoldAncolForFactoryLicenseId,
                    OrganizationName = bu.BusinessNameVi,
                    PhoneNumber = bu.SoDienThoai,
                    Address = bu.DiaChi,
                    TypeofWine = ifo1.ifo.info.cc.TypeofWine,
                    Quantity = ifo1.ifo.info.cc.Quantity,
                    WineFactory = ifo1.ifo.info.cc.WineFactory,
                    CreateName = ifo1.ifo.us.FullName,
                    BusinessId = ifo1.ifo.info.cc.BusinessId,
                    YearReport = ifo1.ifo.info.cc.YearReport,
                    CreateTimeDisplay = ifo1.ifo.info.cc.CreateTime.ToString("dd/MM/yyyy hh:mm")
                }
            ).ToList().AsQueryable();

            string _keywordSearch = "";
            if (query.SearchValue != null && query.SearchValue != "")
            {
                _keywordSearch = query.SearchValue.Trim().ToLower();
                _data = _data.Where(x => x.OrganizationName.ToLower().Contains(_keywordSearch)
                || x.Address.ToLower().Contains(_keywordSearch)
                || x.PhoneNumber.ToLower().Contains(_keywordSearch)
                || x.Quantity.ToString().Contains(_keywordSearch)
                || x.WineFactory.ToLower().Contains(_keywordSearch));
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
