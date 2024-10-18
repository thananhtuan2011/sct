using API_SoCongThuong.Models;
using EF_Core.Models;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Runtime.CompilerServices;

namespace API_SoCongThuong.Reponsitories
{
    public class IndustrialPromotionResultsReportRepo
    {
        public SoHoa_SoCongThuongContext _context;
        public IndustrialPromotionResultsReportRepo(SoHoa_SoCongThuongContext context)
        {
            _context = context;
        }
        public async Task Insert(IndustrialPromotionResultsReportModel model)
        {
            IndustrialPromotionResultsReport data = new IndustrialPromotionResultsReport()
            {
                RpIndustrialPromotionResultsId = model.RpIndustrialPromotionResultsId,
                YearReport = model.YearReport,
                NationalReport = model.NationalReport,
                LocalReport = model.LocalReport,
                Targets = model.Targets,
                Unit = model.Unit,
                CreateUserId = model.CreateUserId,
                CreateTime = model.CreateTime,
            };

            await _context.IndustrialPromotionResultsReports.AddAsync(data);
            await _context.SaveChangesAsync();
        }
        public async Task Update(IndustrialPromotionResultsReportModel model)
        {
            var detailinfo = await _context.IndustrialPromotionResultsReports.Where(d => d.RpIndustrialPromotionResultsId == model.RpIndustrialPromotionResultsId).FirstOrDefaultAsync();
            detailinfo.RpIndustrialPromotionResultsId = model.RpIndustrialPromotionResultsId;
            detailinfo.YearReport = model.YearReport;
            detailinfo.NationalReport = model.NationalReport;
            detailinfo.LocalReport = model.LocalReport;
            detailinfo.Targets = model.Targets;
            detailinfo.Unit = model.Unit;
            detailinfo.UpdateUserId = model.UpdateUserId;
            detailinfo.UpdateTime = model.UpdateTime;

            await _context.SaveChangesAsync();
        }
        public async Task Delete(Guid Id)
        {
            var detailinfo = await _context.IndustrialPromotionResultsReports.Where(d => d.RpIndustrialPromotionResultsId == Id).FirstOrDefaultAsync();
            detailinfo.IsDel = true;

            await _context.SaveChangesAsync();
        }
        public IndustrialPromotionResultsReportModel FindById(Guid Id)
        {
            var result = _context.IndustrialPromotionResultsReports.Where(x => x.RpIndustrialPromotionResultsId == Id && !x.IsDel).Select(d => new IndustrialPromotionResultsReportModel()
            {
                RpIndustrialPromotionResultsId = d.RpIndustrialPromotionResultsId,
                YearReport = d.YearReport,
                NationalReport = d.NationalReport,
                LocalReport = d.LocalReport,
                Targets = d.Targets,
                Unit = d.Unit,
            }).FirstOrDefault();

            return result;
        }
    }
}
