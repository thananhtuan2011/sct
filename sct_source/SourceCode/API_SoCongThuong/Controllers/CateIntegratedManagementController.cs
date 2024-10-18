using DpsLibs.Web;
using EF_Core.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.Design;
using API_SoCongThuong.Classes;
using API_SoCongThuong.Models;
using API_SoCongThuong.Reponsitories.BusinessIndustryRepository;
using API_SoCongThuong.Reponsitories.BusinessRepository;
using API_SoCongThuong.Reponsitories.IndustryRepository;
using API_SoCongThuong.Reponsitories.TypeOfBusinessRepository;
using API_SoCongThuong.Reponsitories.TypeOfProfessionRepository;
using API_SoCongThuong.Reponsitories.CategoryRepository;
using API_SoCongThuong.Reponsitories.DistrictRepository;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using API_SoCongThuong.Reponsitories;
using Microsoft.EntityFrameworkCore.Internal;

namespace API_SoCongThuong.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CateIntegratedManagementController : ControllerBase
    {
        private CateProjectRepo _repo;
        //private BusinessRepo _repoBusi;

        public CateIntegratedManagementController(SoHoa_SoCongThuongContext context)
        {
            _repo = new CateProjectRepo(context);

        }
        // Lấy danh sách 
        #region 
        [Route("find")]
        [HttpPost]
        public IActionResult ListItems_New([FromBody] QueryRequestBody query)//query truyền lên
        {

            BaseModels<CateIntegratedManagementModel> model = new BaseModels<CateIntegratedManagementModel>();
            //string _keywordSearch = ""; //Keyword tìm kiếm
            bool _orderBy_ASC = true;  //Khởi tạo sắp xếp dữ liệu acs hoặc desc khi tìm kiếm
            try
            {
                //Lấy Token, lấy model
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }

                Func<CateIntegratedManagementModel, object> _orderByExpression = x => x.ProjectType; //Khởi tạo mặc định sắp xếp dữ liệu
                Dictionary<string, Func<CateIntegratedManagementModel, object>> _sortableFields = new Dictionary<string, Func<CateIntegratedManagementModel, object>>   //Khởi tạo các trường để sắp xếp
                    {
                    { "ProjectTypeName", x => x.ProjectTypeName},
                    { "Investors", x => x.Investors },
                    { "ProjectName", x => x.ProjectName },
                    { "ProjectInvestment", x => x.ProjectInvestment },
                    { "AreaName", x => x.Area },
                    { "CreateName", x => x.CreateUserId },
                    { "CreateTimeDisplay", x => x.CreateTime },
                    { "IsAction", x => x.IsAction },
                    { "CheckHistory", x => x.CheckHistory },
                    };
                if (query.Sort != null
                    && !string.IsNullOrEmpty(query.Sort.ColumnName)
                    && _sortableFields.ContainsKey(query.Sort.ColumnName))
                {
                    _orderBy_ASC = ("desc".Equals(query.Sort.Direction.ToLower()) ? false : true);    //Sắp xếp asc hoặc desc
                    _orderByExpression = _sortableFields[query.Sort.ColumnName]; //Trường cần sắp xếp
                }


                //var lstData = _repoBusi._context.Businesses
                //                   .Where(c => !c.IsDel)
                //                   .ToList();

                IQueryable<CateIntegratedManagementModel> _data = _repo._context.CateProjects.Where(x => !x.IsDel).GroupJoin(
                    _repo._context.Users.DefaultIfEmpty(),
                    cc => cc.CreateUserId,
                    u => u.UserId,
                     (cc, u) => new { cc, u }).SelectMany(result => result.u.DefaultIfEmpty(), (info, us) => new CateIntegratedManagementModel
                     {
                         CateProjectId = info.cc.CateProjectId,
                         ProjectTypeName = (info.cc.ProjectType == 1 ? "Lũy kế DDI ngoài Khu công nghiệp" : (info.cc.ProjectType == 2 ? "Lũy kế FDI ngoài Khu công nghiệp" : (info.cc.ProjectType == 3 ? "Thu hồi lũy kế" : "Mua bán góp vốn"))),
                         ProjectName = info.cc.ProjectName,
                         Investors = info.cc.Investors,
                         Area = info.cc.Area,
                         AreaName = info.cc.Area == 1 ? "Trong nước" : "Ngoài nước",
                         ProjectInvestment = info.cc.ProjectInvestment,
                         ProjectPartnerNationality = info.cc.ProjectPartnerNationality,
                         InvestmentCertificateCode = info.cc.InvestmentCertificateCode,
                         InvestmentCertificateDate = info.cc.InvestmentCertificateDate,
                         ProjectDecisionToWithdrawDate = info.cc.ProjectDecisionToWithdrawDate,
                         ProjectLegalRepresent = info.cc.ProjectLegalRepresent,
                         CheckHistory = _repo._context.CateProjectHistories.Where(x=>x.CateProjectId== info.cc.CateProjectId).FirstOrDefault() == null ? false : true,
                     });


                //_data = _data.Where(x => !x.IsDel);

                //if (query.SearchValue != null && query.SearchValue != "") //Kiểm tra điều kiện tìm kiếm
                //{
                //    _keywordSearch = query.SearchValue.Trim().ToLower();
                //    _data = _data.Where(x => x.ProjectTypeName.ToLower().Contains(_keywordSearch)
                //    || x.ProjectName.ToLower().Contains(_keywordSearch)
                //    || x.ProjectName.ToLower().Contains(_keywordSearch)
                //    || x.Investors.ToString().Contains(_keywordSearch)
                //    || x.ProjectInvestment.ToString().ToLower().Contains(_keywordSearch)
                //    || x.AreaName.ToLower().Contains(_keywordSearch)
                //    );  //Lấy table đã select tìm kiếm theo keyword
                //}
                // model.items = _data.ToList();

                //Filter:
                //Loại dự án
                if (query.Filter != null && query.Filter.ContainsKey("ProjectType") && !string.IsNullOrEmpty(query.Filter["ProjectType"]))
                {
                    _data = _data.Where(x => x.ProjectTypeName.ToLower().Contains(string.Join("", query.Filter["ProjectType"]).ToLower()));
                }

                //Khu vực
                if (query.Filter != null && query.Filter.ContainsKey("Area") && !string.IsNullOrEmpty(query.Filter["Area"].ToString()))
                {
                    _data = _data.Where(x => x.Area.ToString() == query.Filter["Area"]);
                }

                //Tên nhà đầu tư
                if (query.Filter != null && query.Filter.ContainsKey("InvestorsName") && !string.IsNullOrEmpty(query.Filter["InvestorsName"]))
                {
                    _data = _data.Where(x => x.Investors.ToLower().Contains(string.Join("", query.Filter["InvestorsName"]).ToLower()));
                }

                //Quốc tịch / Đối tác
                if (query.Filter != null && query.Filter.ContainsKey("CountryName") && !string.IsNullOrEmpty(query.Filter["CountryName"]))
                {
                    _data = _data.Where(x => x.ProjectPartnerNationality.ToLower().Contains(string.Join("", query.Filter["CountryName"]).ToLower()));
                }

                //Số chứng nhận đầu tư
                if (query.Filter != null && query.Filter.ContainsKey("InvestmentCertificateCode") && !string.IsNullOrEmpty(query.Filter["InvestmentCertificateCode"]))
                {
                    _data = _data.Where(x => x.InvestmentCertificateCode.ToLower().Contains(string.Join("", query.Filter["InvestmentCertificateCode"]).ToLower()));
                }

                //Thời gian cấp chứng nhận đầu tư
                if (query.Filter != null && query.Filter.ContainsKey("InvestmentCertificateDate")
                    && !string.IsNullOrEmpty(query.Filter["InvestmentCertificateDate"]))
                {
                    _data = _data.Where(x => x.InvestmentCertificateDate == DateTime.ParseExact(query.Filter["InvestmentCertificateDate"], "dd/MM/yyyy", null));
                }

                //Người đại diện
                if (query.Filter != null && query.Filter.ContainsKey("ProjectLegalRepresent") && !string.IsNullOrEmpty(query.Filter["ProjectLegalRepresent"]))
                {
                    _data = _data.Where(x => x.ProjectLegalRepresent.ToLower().Contains(string.Join("", query.Filter["ProjectLegalRepresent"]).ToLower()));
                }

                //Thời gian thu hồi luỹ kế
                if (query.Filter != null && query.Filter.ContainsKey("ProjectDecisionToWithdrawDate")
                    && !string.IsNullOrEmpty(query.Filter["ProjectDecisionToWithdrawDate"]))
                {
                    _data = _data.Where(x => x.ProjectDecisionToWithdrawDate == DateTime.ParseExact(query.Filter["ProjectDecisionToWithdrawDate"], "dd/MM/yyyy", null));
                }

                int _countRows = _data.Count(); //Đếm số dòng của table đã select được
                if (_countRows == 0)    //nếu table = 0 thì trả về không có dữ liệu
                {
                    return NotFound("Không có dữ liệu");
                }
                if (query.Panigator.More)    //query more = true
                {
                    model.status = 1;
                    model.items = _data.ToList();
                    model.total = _countRows;
                    return Ok(model);
                }
                if (_orderBy_ASC) //Sắp xếp dữ liệu theo acs
                {
                    model.items = _data
                        .OrderBy(_orderByExpression)
                        .Skip((query.Panigator.PageIndex - 1) * query.Panigator.PageSize)
                        .Take(query.Panigator.PageSize)
                        .ToList();
                }
                else //Sắp xếp dữ liệu theo desc
                {
                    model.items = _data
                        .OrderByDescending(_orderByExpression)
                        .Skip((query.Panigator.PageIndex - 1) * query.Panigator.PageSize)
                        .Take(query.Panigator.PageSize)
                        .ToList();
                }
                ////Đoạn này lấy total đã tối ưu việc call DB nhiều lần
                //var listId = model.items.Select(x => x.CateCriteriaId).ToList();
                //var listTotal = _repo._context.CateCriteria.Where(x => listId.Contains(x.CateCriteriaId)).Select(x =>
                // new CateCriteriaModel
                // {
                //     CateCriteriaId = x.CateCriteriaId
                // }).ToList();
                //for (int i = 0; i < model.items.Count(); i++)
                //{
                //    int tt = listTotal.Where(x => x.CateCriterionId == model.items[i].CateCriterionId).Select(x => x.TotalStore).FirstOrDefault(0);
                //    model.items[i].TotalStore = tt;
                //}
                model.status = 1;
                model.total = _countRows;
                return Ok(model);
            }
            catch (Exception ex)
            {
                model.status = 0;
                model.error = new ErrorModel()
                {
                    Code = ErrCode_Const.EXCEPTION_API,
                    Msg = ex.Message
                };
                return BadRequest(model);
            }
        }
        #endregion

        [HttpGet("info/{id}")]
        public IActionResult getItemById(Guid id)
        {
            BaseModels<CateIntegratedManagementModel> model = new BaseModels<CateIntegratedManagementModel>();
            try
            {
                var info = (from p in _repo._context.CateProjects.Where(x => x.CateProjectId == id && !x.IsDel)
                           //join c in _repo._context.Countries.Where(x => !x.IsDel)
                           //    on p.Country equals c.CountryId
                           //join b in _repo._context.Businesses.Where(x => !x.IsDel)
                           //    on p.CompanySell equals b.BusinessId
                               //from p in _repo.FindById(id)
                           join u in _repo._context.Units.Where(x => !x.IsDel) 
                               on p.Units equals u.UnitId into res
                           from r in res.DefaultIfEmpty()
                           select new CateIntegratedManagementModel
                           {
                               CateProjectId = id,
                               ActualPurchase = p.ActualPurchase,
                               Address = p.Address,
                               Area = p.Area,
                               AreaName = p.Area == 1 ? "Trong nước" : "Ngoài nước",
                               CapitalContributionTradingTime = p.CapitalContributionTradingTime,
                               CapitalPurchase = p.CapitalPurchase,
                               CharterCapitalAfterPurchase = p.CharterCapitalAfterPurchase,
                               CompanyBuy = p.CompanyBuy,
                               CompanySell = p.CompanySell,
                               CompanySellName = _repo._context.Businesses.Where(x=> x.BusinessId == p.CompanySell && !x.IsDel).FirstOrDefault().BusinessNameVi == null ? "" : _repo._context.Businesses.Where(x => x.BusinessId == p.CompanySell && !x.IsDel).FirstOrDefault().BusinessNameVi,
                               Country = p.Country,
                               CountryName = _repo._context.Countries.Where(x=> x.CountryId == p.Country && !x.IsDel).FirstOrDefault().CountryName == null ? "" : _repo._context.Countries.Where(x => x.CountryId == p.Country && !x.IsDel).FirstOrDefault().CountryName,
                               InitialCharterCapital = p.InitialCharterCapital,
                               InvestmentCertificateCode = p.InvestmentCertificateCode,
                               InvestmentCertificateDate = p.InvestmentCertificateDate,
                               Investors = p.Investors,
                               PolicyDecisions = p.PolicyDecisions,
                               PolicyDecisionsDate = p.PolicyDecisionsDate,
                               Profession = p.Profession,
                               ProjectAddress = p.ProjectAddress,
                               ProjectDecisionToWithdraw = p.ProjectDecisionToWithdraw,
                               ProjectDecisionToWithdrawDate = p.ProjectDecisionToWithdrawDate,
                               ProjectFdi = p.ProjectFdi,
                               ProjectImplementationScale = p.ProjectImplementationScale,
                               ProjectImplementationYear = p.ProjectImplementationYear,
                               ProjectInvestment = p.ProjectInvestment,
                               ProjectInvestmentForm = p.ProjectInvestmentForm,
                               ProjectInvestmentUnits = p.ProjectInvestmentUnits,
                               ProjectLegalRepresent = p.ProjectLegalRepresent,
                               ProjectLicenseYear = p.ProjectLicenseYear,
                               ProjectLocalArea = p.ProjectLocalArea,
                               Units = r.UnitName ?? "",
                               ProjectName = p.ProjectName,
                               ProjectOperatingTime = p.ProjectOperatingTime,
                               ProjectPartnerNationality = p.ProjectPartnerNationality,
                               ProjectPhoneNumber = p.ProjectPhoneNumber,
                               ProjectProgress = p.ProjectProgress,
                               ProjectProgressActual = p.ProjectProgressActual,
                               ProjectType = p.ProjectType,
                               ProjectTypeName = p.ProjectType == 1 ? "Lũy kế DDI ngoài Khu công nghiệp" :
                                            (p.ProjectType == 2 ? "Lũy kế FDI ngoài Khu công nghiệp" :
                                            (p.ProjectType == 3 ? "Thu hồi lũy kế" : "Mua bán góp vốn")),
                               Note = p.Note,
                           }).FirstOrDefault();

                if (info == null)
                    return NotFound(ErrMsg_Const.GetMsg(ErrCode_Const.CANNOT_FIND_DATA_BY_QUERY));

                var Details = (from d in _repo._context.CateProjectDisbursements.Where(x => x.CateProjectId == id)
                               join u in _repo._context.Units.Where(x => !x.IsDel)
                                   on d.DisbursementUnits equals u.UnitId
                               select new CateIntegratedManagementDisbursementModel
                               {
                                   CateProjectDisbursementId = d.CateProjectDisbursementId,
                                   DisbursementDate = d.DisbursementDate,
                                   DisbursementMoney = d.DisbursementMoney,
                                   DisbursementUnits = d.DisbursementUnits,
                                   UnitName = u.UnitName,
                               }).ToList();

                List<CateIntegratedManagementDisbursementModel> Result = new List<CateIntegratedManagementDisbursementModel>();
                foreach (var item in Details)
                {
                    CateIntegratedManagementDisbursementModel i = new CateIntegratedManagementDisbursementModel()
                    {
                        CateProjectDisbursementId = item.CateProjectDisbursementId,
                        DisbursementDate = item.DisbursementDate,
                        DisbursementMoney = item.DisbursementMoney,
                        DisbursementUnits = item.DisbursementUnits,
                        UnitName = item.UnitName,
                        IsConfirm = item.IsConfirm,
                    };
                    Result.Add(i);
                }

                var Histories = _repo._context.CateProjectHistories.Where(x => x.CateProjectId == id).ToList();

                List<CateProjectHistoryModel> HistoriesData = new List<CateProjectHistoryModel>();
                foreach (var item in Histories)
                {
                    CateProjectHistoryModel i = new CateProjectHistoryModel()
                    {
                        CateProjectHistoryId = item.CateProjectHistoryId,
                        ContentAdjust = item.ContentAdjust.Replace("," , "<br>"),
                        UpdateUserId = item.UpdateUserId,
                        UpdateName = _repo._context.Users.Where(x => x.UserId == item.UpdateUserId).FirstOrDefault().UserName,
                    };
                    HistoriesData.Add(i);
                }

                info.Details = Result;
                info.Historys = HistoriesData;

                //Set data cho base model
                model.status = 1;
                model.data = info;
                return Ok(model);
            }
            catch (Exception ex)
            {

                model.status = 0;
                model.error = new ErrorModel()
                {
                    Code = ErrCode_Const.EXCEPTION_API,
                    Msg = ex.Message
                };
                return BadRequest(model);
            }
        }
    }
}
