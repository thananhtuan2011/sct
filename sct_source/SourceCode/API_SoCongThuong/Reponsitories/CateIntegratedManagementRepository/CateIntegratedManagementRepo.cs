using API_SoCongThuong.Models;
using EF_Core.Models;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Runtime.CompilerServices;

namespace API_SoCongThuong.Reponsitories
{
    public class CateIntegratedManagementRepo
    {
        public SoHoa_SoCongThuongContext _context;
        public CateIntegratedManagementRepo(SoHoa_SoCongThuongContext context)
        {
            _context = context;
        }
        //public async Task Insert(CateIntegratedManagementModel model)
        //{
        //    CateIntegratedManagement data = new CateIntegratedManagement()
        //    {
        //        IntegratedManagementId = model.IntegratedManagementId,
        //        Address = model.Address,
        //        Area = model.Area,
        //        InvestmentCertificateCode = model.InvestmentCertificateCode,
        //        InvestmentCertificateDate = model.InvestmentCertificateDate,
        //        Investors = model.Investors,
        //        PolicyDecisions = model.PolicyDecisions,
        //        PolicyDecisionsDate = model.PolicyDecisionsDate,
        //        ProjectAddress = model.ProjectAddress,
        //        ProjectImplementationScale = model.ProjectImplementationScale,
        //        ProjectInvestment = model.ProjectInvestment,
        //        ProjectInvestmentUnits = model.ProjectInvestmentUnits,
        //        ProjectLegalRepresent = model.ProjectLegalRepresent,
        //        ProjectLicenseYear = model.ProjectLicenseYear,
        //        ProjectLocalArea = model.ProjectLocalArea,
        //        ProjectName = model.ProjectName,
        //        ProjectOperatingTime = model.ProjectOperatingTime,
        //        ProjectPhoneNumber = model.ProjectPhoneNumber,
        //        ProjectProgress = model.ProjectProgress,
        //        ProjectProgressActual = model.ProjectProgressActual,
        //        ProjectType = model.ProjectType,
        //        Note = model.Note,
        //        CreateUserId = model.CreateUserId,
        //        CreateTime = model.CreateTime,
        //    };
        //    await _context.CateIntegratedManagements.AddAsync(data);
        //    await _context.SaveChangesAsync();
        //    List<CateIntegratedManagementDisbursement> details = new List<CateIntegratedManagementDisbursement>();
        //    foreach (var item in model.Disbursements)
        //    {
        //        CateIntegratedManagementDisbursement detail = new CateIntegratedManagementDisbursement()
        //        {
        //            CateIntegratedManagementId = data.IntegratedManagementId,
        //            DisbursementDate = item.DisbursementDate,
        //            DisbursementMoney = item.DisbursementMoney,
        //            DisbursementUnits = item.DisbursementUnits,
        //            IsConfirm= item.IsConfirm,
        //        };
        //        details.Add(detail);
        //    }
        //    await _context.CateIntegratedManagementDisbursements.AddRangeAsync(details);
        //    await _context.SaveChangesAsync();
        //}
        //public async Task Update(CateIntegratedManagementModel model)
        //{
        //    var dt = _context.CateIntegratedManagementDisbursements.Where(x => x.CateIntegratedManagementId == model.IntegratedManagementId).ToList();
        //    _context.CateIntegratedManagementDisbursements.RemoveRange(dt);
        //    var his = _context.CateIntegratedManagementHistories.Where(x => x.CateIntegratedManagementId == model.IntegratedManagementId).ToList();
        //    _context.CateIntegratedManagementHistories.RemoveRange(his);
        //    _context.SaveChanges();

        //    var detailinfo = await _context.CateIntegratedManagements.Where(d => d.IntegratedManagementId == model.IntegratedManagementId).FirstOrDefaultAsync();
        //    detailinfo.Address = model.Address;
        //    detailinfo.Area = model.Area;
        //    detailinfo.InvestmentCertificateCode = model.InvestmentCertificateCode;
        //    detailinfo.InvestmentCertificateDate = model.InvestmentCertificateDate;
        //    detailinfo.Investors = model.Investors;
        //    detailinfo.PolicyDecisions = model.PolicyDecisions;
        //    detailinfo.PolicyDecisionsDate = model.PolicyDecisionsDate;
        //    detailinfo.ProjectAddress = model.ProjectAddress;
        //    detailinfo.ProjectImplementationScale = model.ProjectImplementationScale;
        //    detailinfo.ProjectInvestment = model.ProjectInvestment;
        //    detailinfo.ProjectInvestmentUnits = model.ProjectInvestmentUnits;
        //    detailinfo.ProjectLegalRepresent = model.ProjectLegalRepresent;
        //    detailinfo.ProjectLicenseYear = model.ProjectLicenseYear;
        //    detailinfo.ProjectLocalArea = model.ProjectLocalArea;
        //    detailinfo.ProjectName = model.ProjectName;
        //    detailinfo.ProjectOperatingTime = model.ProjectOperatingTime;
        //    detailinfo.ProjectPhoneNumber = model.ProjectPhoneNumber;
        //    detailinfo.ProjectProgress = model.ProjectProgress;
        //    detailinfo.ProjectProgressActual = model.ProjectProgressActual;
        //    detailinfo.ProjectType = model.ProjectType;
        //    detailinfo.Note = model.Note;
        //    detailinfo.UpdateUserId = model.UpdateUserId;
        //    detailinfo.UpdateTime = model.UpdateTime;

        //    List<CateIntegratedManagementDisbursement> disb = new List<CateIntegratedManagementDisbursement>();
        //    foreach (var item in model.Disbursements)
        //    {
        //        CateIntegratedManagementDisbursement detail = new CateIntegratedManagementDisbursement()
        //        {
        //            CateIntegratedManagementId = model.IntegratedManagementId,
        //            DisbursementDate = item.DisbursementDate,
        //            DisbursementMoney = item.DisbursementMoney,
        //            DisbursementUnits = item.DisbursementUnits,
        //            IsConfirm = item.IsConfirm,
        //        };
        //        disb.Add(detail);
        //    }
        //    await _context.CateIntegratedManagementDisbursements.AddRangeAsync(disb);

        //    List<CateIntegratedManagementHistory> historys = new List<CateIntegratedManagementHistory>();
        //    foreach (var item in model.Historys)
        //    {
        //        CateIntegratedManagementHistory history = new CateIntegratedManagementHistory()
        //        {
        //            CateIntegratedManagementId = model.IntegratedManagementId,
        //            ContentAdjust = item.ContentAdjust,
        //        };
        //        historys.Add(history);
        //    }
        //    await _context.CateIntegratedManagementHistories.AddRangeAsync(historys);
        //    await _context.SaveChangesAsync();
        //}
        //public async Task Delete(Guid Id)
        //{
        //    var detailinfo = await _context.CateIntegratedManagements.Where(d => d.IntegratedManagementId == Id).FirstOrDefaultAsync();
        //    detailinfo.IsDel = true;

        //    var details = _context.CateIntegratedManagementDisbursements.Where(d => d.CateIntegratedManagementId == Id).ToList();
        //    _context.CateIntegratedManagementDisbursements.RemoveRange(details);
        //    var histo = _context.CateIntegratedManagementHistories.Where(d => d.CateIntegratedManagementId == Id).ToList();
        //    _context.CateIntegratedManagementHistories.RemoveRange(histo);

        //    await _context.SaveChangesAsync();
        //}
        //public async Task Deletes(List<Guid> Ids)
        //{
        //    List<CateProject> items = new List<CateProject>();
        //    foreach (var idremove in Ids)
        //    {
        //        CateProject item = new CateProject();
        //        var detailinfo = await _context.CateProjects.Where(d => d.CateProjectssId == idremove).FirstOrDefaultAsync();
        //        item.CateProjectssId = idremove;
        //        item.IsDel = true;
        //        items.Add(item);

        //        var lstworkers = _context.CateProjectsDetails.Where(d => d.CateProjectssId == idremove).ToList();
        //        var lstprofession = _context.CateProjectsTypeOfProfessions.Where(d => d.CateProjectssId == idremove).ToList();
        //        _context.CateProjectsDetails.RemoveRange(lstworkers);
        //        _context.CateProjectsTypeOfProfessions.RemoveRange(lstprofession);
        //    }
        //    _context.CateProjects.UpdateRange(items);
        //    await _context.SaveChangesAsync();
        //}
        //public CateIntegratedManagementModel FindById(Guid Id)
        //{
        //    var result = _context.CateIntegratedManagements.Where(x => x.IntegratedManagementId == Id && !x.IsDel).Select(model => new CateIntegratedManagementModel()
        //    {
        //        IntegratedManagementId = Id,
        //        Address = model.Address,
        //        Area = model.Area,
        //        InvestmentCertificateCode = model.InvestmentCertificateCode,
        //        InvestmentCertificateDate = model.InvestmentCertificateDate,
        //        Investors = model.Investors,
        //        PolicyDecisions = model.PolicyDecisions,
        //        PolicyDecisionsDate = model.PolicyDecisionsDate,
        //        ProjectAddress = model.ProjectAddress,
        //        ProjectImplementationScale = model.ProjectImplementationScale,
        //        ProjectInvestment = model.ProjectInvestment,
        //        ProjectInvestmentUnits = model.ProjectInvestmentUnits,
        //        ProjectLegalRepresent = model.ProjectLegalRepresent,
        //        ProjectLicenseYear = model.ProjectLicenseYear,
        //        ProjectLocalArea = model.ProjectLocalArea,
        //        ProjectName = model.ProjectName,
        //        ProjectOperatingTime = model.ProjectOperatingTime,
        //        ProjectPhoneNumber = model.ProjectPhoneNumber,
        //        ProjectProgress = model.ProjectProgress,
        //        ProjectProgressActual = model.ProjectProgressActual,
        //        ProjectType = model.ProjectType,
        //        Note = model.Note
        //    }).FirstOrDefault();

        //    var Details = _context.CateIntegratedManagementDisbursements.Where(x => x.CateIntegratedManagementId == Id).ToList();
        //    List<CateIntegratedManagementDisbursementModel> Result = new List<CateIntegratedManagementDisbursementModel>();
        //    foreach (var item in Details)
        //    {
        //        CateIntegratedManagementDisbursementModel i = new CateIntegratedManagementDisbursementModel()
        //        {
        //            CateIntegratedManagementDisbursementId=item.CateIntegratedManagementDisbursementId,
        //            DisbursementDate=item.DisbursementDate,
        //            DisbursementMoney=item.DisbursementMoney,
        //            DisbursementUnits=item.DisbursementUnits,
        //            IsConfirm = item.IsConfirm,
        //        };
        //        Result.Add(i);
        //    }
        //    result.Disbursements = Result;

        //    var Historys = _context.CateIntegratedManagementHistories.Where(x => x.CateIntegratedManagementId == Id).DefaultIfEmpty().Join(
        //        _context.Users, his => his.UpdateUserId, us => us.UserId,
        //        (his, us) => new CateIntegratedManagementHistoryModel
        //        {
        //            CateIntegratedManagementHistoryId = his.CateIntegratedManagementHistoryId,
        //            ContentAdjust = his.ContentAdjust,
        //            UpdateTimeDisplay = string.Format("{0:dd/MM/yyyy}", his.UpdateTime),
        //        }).ToList();
        //    result.Historys = Historys;


        //    return result;
        //}
    }
}
