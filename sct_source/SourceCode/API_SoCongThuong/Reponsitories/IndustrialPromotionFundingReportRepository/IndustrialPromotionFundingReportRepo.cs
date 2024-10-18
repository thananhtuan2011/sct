using API_SoCongThuong.Models;
using EF_Core.Models;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Runtime.CompilerServices;

namespace API_SoCongThuong.Reponsitories
{
    public class IndustrialPromotionFundingReportRepo
    {
        public SoHoa_SoCongThuongContext _context;
        public IndustrialPromotionFundingReportRepo(SoHoa_SoCongThuongContext context)
        {
            _context = context;
        }
        public async Task Insert(IndustrialPromotionFundingReportModel model)
        {
            IndustrialPromotionFundingReport data = new IndustrialPromotionFundingReport()
            {
                RpIndustrialPromotionFundingId = model.RpIndustrialPromotionFundingId,
                YearReport = model.YearReport,
                NationalReport = model.NationalReport,
                LocalReport = model.LocalReport,
                Targets = model.Targets,
                Unit = model.Unit,
                CreateUserId = model.CreateUserId,
                CreateTime = model.CreateTime,
            };
            await _context.IndustrialPromotionFundingReports.AddAsync(data);
            await _context.SaveChangesAsync();
        }
        public async Task Update(IndustrialPromotionFundingReportModel model)
        {
            var detailinfo = await _context.IndustrialPromotionFundingReports.Where(d => d.RpIndustrialPromotionFundingId == model.RpIndustrialPromotionFundingId).FirstOrDefaultAsync();
            detailinfo.RpIndustrialPromotionFundingId = model.RpIndustrialPromotionFundingId;
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
            var detailinfo = await _context.IndustrialPromotionFundingReports.Where(d => d.RpIndustrialPromotionFundingId == Id).FirstOrDefaultAsync();
            detailinfo.IsDel = true;
            await _context.SaveChangesAsync();

        }
        public IndustrialPromotionFundingReportModel FindById(Guid Id)
        {
            var result = _context.IndustrialPromotionFundingReports.Where(x => x.RpIndustrialPromotionFundingId == Id && !x.IsDel).Select(d => new IndustrialPromotionFundingReportModel()
            {
                RpIndustrialPromotionFundingId = d.RpIndustrialPromotionFundingId,
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
