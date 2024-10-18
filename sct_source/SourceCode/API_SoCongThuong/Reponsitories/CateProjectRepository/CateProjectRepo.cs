using API_SoCongThuong.Models;
using EF_Core.Models;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Runtime.CompilerServices;

namespace API_SoCongThuong.Reponsitories
{
    public class CateProjectRepo
    {
        public SoHoa_SoCongThuongContext _context;
        public CateProjectRepo(SoHoa_SoCongThuongContext context)
        {
            _context = context;
        }
        public async Task Insert(CateProjectModel model)
        {
            try
            {
                CateProject data = new CateProject()
                {
                    CateProjectId = model.CateProjectId,
                    ActualPurchase = model.ActualPurchase,
                    Address = model.Address,
                    Area = model.Area,
                    ProjectId = model.ProjectId,
                    CapitalPurchase = model.CapitalPurchase,
                    CharterCapitalAfterPurchase = model.CharterCapitalAfterPurchase,
                    CompanyBuy = model.CompanyBuy,
                    CompanySell = model.CompanySell,
                    Country = model.Country,
                    InitialCharterCapital = model.InitialCharterCapital,
                    InvestmentCertificateCode = model.InvestmentCertificateCode,
                    Investors = model.Investors,
                    PolicyDecisions = model.PolicyDecisions,
                    Profession = model.Profession,
                    ProjectAddress = model.ProjectAddress,
                    ProjectDecisionToWithdraw = model.ProjectDecisionToWithdraw,
                    ProjectFdi = model.ProjectFdi,
                    ProjectImplementationScale = model.ProjectImplementationScale,
                    ProjectImplementationYear = model.ProjectImplementationYear,
                    ProjectInvestment = model.ProjectInvestment,
                    ProjectInvestmentForm = model.ProjectInvestmentForm,
                    ProjectInvestmentUnits = model.ProjectInvestmentUnits,
                    ProjectLegalRepresent = model.ProjectLegalRepresent,
                    ProjectLicenseYear = model.ProjectLicenseYear,
                    ProjectLocalArea = model.ProjectLocalArea,
                    Units = model.Units,
                    ProjectName = model.ProjectName,
                    ProjectOperatingTime = model.ProjectOperatingTime,
                    ProjectPartnerNationality = model.ProjectPartnerNationality,
                    ProjectPhoneNumber = model.ProjectPhoneNumber,
                    ProjectProgress = model.ProjectProgress,
                    ProjectProgressActual = model.ProjectProgressActual,
                    ProjectType = model.ProjectType,
                    Note = model.Note,
                    CreateUserId = model.CreateUserId,
                    CreateTime = model.CreateTime,
                };
                if (model.CapitalContributionTradingTime != null)
                    data.CapitalContributionTradingTime = (DateTime)model.CapitalContributionTradingTime;
                if (model.ProjectDecisionToWithdrawDate != null)
                    data.ProjectDecisionToWithdrawDate = (DateTime)model.ProjectDecisionToWithdrawDate;
                if (model.InvestmentCertificateDate != null)
                    data.InvestmentCertificateDate = (DateTime)model.InvestmentCertificateDate;
                if (model.PolicyDecisionsDate != null)
                    data.PolicyDecisionsDate = (DateTime)model.PolicyDecisionsDate;
                await _context.CateProjects.AddAsync(data);
                await _context.SaveChangesAsync();
                List<CateProjectDisbursement> details = new List<CateProjectDisbursement>();
                foreach (var item in model.Details)
                {
                    CateProjectDisbursement detail = new CateProjectDisbursement()
                    {
                        CateProjectId = data.CateProjectId,
                        DisbursementDate = item.DisbursementDate,
                        DisbursementMoney = item.DisbursementMoney,
                        DisbursementUnits = item.DisbursementUnits,
                        IsConfirm = item.IsConfirm,
                    };
                    details.Add(detail);
                }
                await _context.CateProjectDisbursements.AddRangeAsync(details);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {

            }
        }
        public async Task Update(CateProjectModel model)
        {
            var detais = _context.CateProjectDisbursements.Where(x => x.CateProjectId == model.CateProjectId).ToList();
            _context.CateProjectDisbursements.RemoveRange(detais);
            _context.SaveChanges();

            var detailinfo = await _context.CateProjects.Where(d => d.CateProjectId == model.CateProjectId).FirstOrDefaultAsync();
            detailinfo.ActualPurchase = model.ActualPurchase;
            detailinfo.Address = model.Address;
            detailinfo.Area = model.Area;
            //detailinfo.CapitalContributionTradingTime = model.CapitalContributionTradingTime;
            detailinfo.CapitalPurchase = model.CapitalPurchase;
            detailinfo.CharterCapitalAfterPurchase = model.CharterCapitalAfterPurchase;
            detailinfo.CompanyBuy = model.CompanyBuy;
            detailinfo.CompanySell = model.CompanySell;
            detailinfo.Country = model.Country;
            detailinfo.InitialCharterCapital = model.InitialCharterCapital;
            detailinfo.InvestmentCertificateCode = model.InvestmentCertificateCode;
            //detailinfo.InvestmentCertificateDate = model.InvestmentCertificateDate;
            detailinfo.Investors = model.Investors;
            detailinfo.PolicyDecisions = model.PolicyDecisions;
            //detailinfo.PolicyDecisionsDate = model.PolicyDecisionsDate;
            detailinfo.Profession = model.Profession;
            detailinfo.ProjectAddress = model.ProjectAddress;
            detailinfo.ProjectDecisionToWithdraw = model.ProjectDecisionToWithdraw;
            //detailinfo.ProjectDecisionToWithdrawDate = model.ProjectDecisionToWithdrawDate;
            detailinfo.ProjectFdi = model.ProjectFdi;
            detailinfo.ProjectImplementationScale = model.ProjectImplementationScale;
            detailinfo.ProjectImplementationYear = model.ProjectImplementationYear;
            detailinfo.ProjectInvestment = model.ProjectInvestment;
            detailinfo.ProjectInvestmentForm = model.ProjectInvestmentForm;
            detailinfo.ProjectInvestmentUnits = model.ProjectInvestmentUnits;
            detailinfo.ProjectLegalRepresent = model.ProjectLegalRepresent;
            detailinfo.ProjectLicenseYear = model.ProjectLicenseYear;
            detailinfo.ProjectLocalArea = model.ProjectLocalArea;
            detailinfo.Units = model.Units;
            detailinfo.ProjectName = model.ProjectName;
            detailinfo.ProjectOperatingTime = model.ProjectOperatingTime;
            detailinfo.ProjectPartnerNationality = model.ProjectPartnerNationality;
            detailinfo.ProjectPhoneNumber = model.ProjectPhoneNumber;
            detailinfo.ProjectProgress = model.ProjectProgress;
            detailinfo.ProjectProgressActual = model.ProjectProgressActual;
            detailinfo.ProjectType = model.ProjectType;
            detailinfo.Note = model.Note;
            detailinfo.UpdateUserId = model.UpdateUserId;
            detailinfo.UpdateTime = model.UpdateTime;

            if (model.CapitalContributionTradingTime != null)
                detailinfo.CapitalContributionTradingTime = (DateTime)model.CapitalContributionTradingTime;
            if (model.ProjectDecisionToWithdrawDate != null)
                detailinfo.ProjectDecisionToWithdrawDate = (DateTime)model.ProjectDecisionToWithdrawDate;
            if (model.InvestmentCertificateDate != null)
                detailinfo.InvestmentCertificateDate = (DateTime)model.InvestmentCertificateDate;
            if (model.PolicyDecisionsDate != null)
                detailinfo.PolicyDecisionsDate = (DateTime)model.PolicyDecisionsDate;

            List<CateProjectDisbursement> details = new List<CateProjectDisbursement>();
            foreach (var item in model.Details)
            {
                CateProjectDisbursement detail = new CateProjectDisbursement()
                {
                    CateProjectId = model.CateProjectId,
                    DisbursementDate = item.DisbursementDate,
                    DisbursementMoney = item.DisbursementMoney,
                    DisbursementUnits = item.DisbursementUnits,
                    IsConfirm = item.IsConfirm,
                };
                details.Add(detail);
            }
            await _context.CateProjectDisbursements.AddRangeAsync(details);
            await _context.SaveChangesAsync();

            List<CateProjectHistory> historys = new List<CateProjectHistory>();
            foreach (var item in model.Historys)
            {
                CateProjectHistory history = new CateProjectHistory()
                {
                    CateProjectId = model.CateProjectId,
                    ContentAdjust = item.ContentAdjust,
                    UpdateTime = DateTime.Now,
                    UpdateUserId = model.UpdateUserId,
                };
                historys.Add(history);
            }
            await _context.CateProjectHistories.AddRangeAsync(historys);
            await _context.SaveChangesAsync();
        }
        public async Task Delete(Guid Id)
        {
            var detailinfo = await _context.CateProjects.Where(d => d.CateProjectId == Id).FirstOrDefaultAsync();

            detailinfo.IsDel = true;

            var histo = _context.CateProjectHistories.Where(d => d.CateProjectId == Id).ToList();
            _context.CateProjectHistories.RemoveRange(histo);

            var lstworkers = _context.CateProjectDisbursements.Where(d => d.CateProjectId == Id).ToList();

            _context.CateProjectDisbursements.RemoveRange(lstworkers);

            await _context.SaveChangesAsync();
        }
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
        public CateProjectModel FindById(Guid Id)
        {
            var result = _context.CateProjects.Where(x => x.CateProjectId == Id && !x.IsDel).Select(model => new CateProjectModel()
            {
                CateProjectId = Id,
                ActualPurchase = model.ActualPurchase,
                Address = model.Address,
                Area = model.Area,
                CapitalContributionTradingTime = model.CapitalContributionTradingTime,
                CapitalPurchase = model.CapitalPurchase,
                CharterCapitalAfterPurchase = model.CharterCapitalAfterPurchase,
                CompanyBuy = model.CompanyBuy,
                CompanySell = model.CompanySell,
                Country = model.Country,
                InitialCharterCapital = model.InitialCharterCapital,
                InvestmentCertificateCode = model.InvestmentCertificateCode,
                InvestmentCertificateDate = model.InvestmentCertificateDate,
                Investors = model.Investors,
                PolicyDecisions = model.PolicyDecisions,
                PolicyDecisionsDate = model.PolicyDecisionsDate,
                Profession = model.Profession,
                ProjectAddress = model.ProjectAddress,
                ProjectDecisionToWithdraw = model.ProjectDecisionToWithdraw,
                ProjectDecisionToWithdrawDate = model.ProjectDecisionToWithdrawDate,
                ProjectFdi = model.ProjectFdi,
                ProjectImplementationScale = model.ProjectImplementationScale,
                ProjectImplementationYear = model.ProjectImplementationYear,
                ProjectInvestment = model.ProjectInvestment,
                ProjectInvestmentForm = model.ProjectInvestmentForm,
                ProjectInvestmentUnits = model.ProjectInvestmentUnits,
                ProjectLegalRepresent = model.ProjectLegalRepresent,
                ProjectLicenseYear = model.ProjectLicenseYear,
                ProjectLocalArea = model.ProjectLocalArea,
                Units = model.Units,
                ProjectName = model.ProjectName,
                ProjectId = model.ProjectId,
                ProjectOperatingTime = model.ProjectOperatingTime,
                ProjectPartnerNationality = model.ProjectPartnerNationality,
                ProjectPhoneNumber = model.ProjectPhoneNumber,
                ProjectProgress = model.ProjectProgress,
                ProjectProgressActual = model.ProjectProgressActual,
                ProjectType = model.ProjectType,
                Note = model.Note
            }).FirstOrDefault();

            if (result == null)
            {
                return new CateProjectModel();
            }

            var Details = _context.CateProjectDisbursements.Where(x => x.CateProjectId == Id).ToList();

            List<CateProjectDisbursementModel> Result = new List<CateProjectDisbursementModel>();
            foreach (var item in Details)
            {
                CateProjectDisbursementModel i = new CateProjectDisbursementModel()
                {
                    CateProjectDisbursementId = item.CateProjectDisbursementId,
                    CateProjectId = item.CateProjectId,
                    DisbursementDate = item.DisbursementDate,
                    DisbursementMoney = item.DisbursementMoney,
                    DisbursementUnits = item.DisbursementUnits,
                    IsConfirm = item.IsConfirm,
                };
                Result.Add(i);
            }

            var Histories = _context.CateProjectHistories.Where(x => x.CateProjectId == Id).ToList();

            List<CateProjectHistoryModel> HistoriesData = new List<CateProjectHistoryModel>();
            foreach (var item in HistoriesData)
            {
                CateProjectHistoryModel i = new CateProjectHistoryModel()
                {
                    CateProjectHistoryId = item.CateProjectHistoryId,
                    ContentAdjust = item.ContentAdjust,
                    UpdateUserId = item.UpdateUserId,
                };
                HistoriesData.Add(i);
            }

            result.Details = Result;
            result.Historys = HistoriesData;

            return result;
        }

        public List<CateProject> GetListProject()
        {
            var lst = _context.CateProjects.Where(x => (x.ProjectType == 1 || x.ProjectType == 2) && !x.IsDel).ToList();

            return lst;
        }
        public CateProject GetListProjectById(Guid Id)
        {
            var lst = _context.CateProjects.Where(x => (x.ProjectType == 1 || x.ProjectType == 2) && x.CateProjectId == Id).FirstOrDefault();

            return lst;
        }
    }
}
