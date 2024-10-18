using DpsLibs.Web;
using EF_Core.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.Design;
using API_SoCongThuong.Classes;
using API_SoCongThuong.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using API_SoCongThuong.Reponsitories;
using Microsoft.EntityFrameworkCore.Internal;
using ClosedXML.Excel;
using API_SoCongThuong.Logger;
using Newtonsoft.Json;
using System.Data;

namespace API_SoCongThuong.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CateProjectController : ControllerBase
    {
        private CateProjectRepo _repo;
        private IConfiguration _configuration;
        private readonly ILogger<AsyncLogger> _logger;
        public SoHoa_SoCongThuongContext _context;
        private AsyncLogger _asyncLogger;
        public CateProjectController(SoHoa_SoCongThuongContext context, IConfiguration configuration, ILogger<AsyncLogger> logger)
        {
            _repo = new CateProjectRepo(context);
            _logger = logger;
            _context = context;
            _asyncLogger = new AsyncLogger(_logger, _context);
            _configuration = configuration;
        }
        // Lấy danh sách 
        #region 
        [Route("find")]
        [HttpPost]
        public IActionResult ListItems_New([FromBody] QueryRequestBody query)//query truyền lên
        {

            BaseModels<CateProjectModel> model = new BaseModels<CateProjectModel>();
            string _keywordSearch = ""; //Keyword tìm kiếm
            bool _orderBy_ASC = false;  //Khởi tạo sắp xếp dữ liệu acs hoặc desc khi tìm kiếm
            try
            {
                //Lấy Token, lấy model
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }

                Func<CateProjectModel, object> _orderByExpression = x => x.ProjectType; //Khởi tạo mặc định sắp xếp dữ liệu
                Dictionary<string, Func<CateProjectModel, object>> _sortableFields = new Dictionary<string, Func<CateProjectModel, object>>   //Khởi tạo các trường để sắp xếp
                    {
                    { "projectTypeName", x => x.ProjectType },
                    { "Investors", x => x.Investors },
                    { "ProjectName", x => x.ProjectName },
                    { "ProjectInvestment", x => x.ProjectInvestment },
                    { "CreateName", x => x.CreateUserId },
                    { "CreateTimeDisplay", x => x.CreateTime },
                    { "IsAction", x => x.IsAction },
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

                IQueryable<CateProjectModel> _data = _repo._context.CateProjects.Where(x => !x.IsDel).GroupJoin(
                    _repo._context.Users,
                    cc => cc.CreateUserId,
                    u => u.UserId,
                     (cc, u) => new { cc, u }).SelectMany(result => result.u.DefaultIfEmpty(), (info, us) => new CateProjectModel
                     {
                         CateProjectId = info.cc.CateProjectId,
                         ProjectTypeName = (info.cc.ProjectType == 1 ? "Lũy kế DDI ngoài Khu công nghiệp" : (info.cc.ProjectType == 2 ? "Lũy kế FDI ngoài Khu công nghiệp" : (info.cc.ProjectType == 3 ? "Thu hồi lũy kế" : "Mua bán góp vốn"))),
                         ProjectName = info.cc.ProjectName,
                         Investors = info.cc.Investors,
                         ProjectInvestment = info.cc.ProjectInvestment,
                         CreateTime = info.cc.CreateTime,
                         ProjectType = info.cc.ProjectType,
                         CompanyBuy = info.cc.CompanyBuy,
                         CapitalPurchase = info.cc.CapitalPurchase
                     });

                //_data = _data.Where(x => !x.IsDel);

                if (query.Filter != null && query.Filter.ContainsKey("ProjectType")
                    && !string.IsNullOrEmpty(query.Filter["ProjectType"]))
                {
                    _data = _data.Where(x => x.ProjectType.ToString() == query.Filter["ProjectType"]);
                }

                if (query.SearchValue != null && query.SearchValue != "") //Kiểm tra điều kiện tìm kiếm
                {
                    _keywordSearch = query.SearchValue.Trim().ToLower();
                    _data = _data.Where(x => x.ProjectTypeName.ToLower().Contains(_keywordSearch)
                    || x.ProjectName.ToLower().Contains(_keywordSearch)
                    || x.Investors.ToLower().Contains(_keywordSearch)
                    || x.ProjectInvestment.ToString().Contains(_keywordSearch)
                    );  //Lấy table đã select tìm kiếm theo keyword
                }
                // model.items = _data.ToList();

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

        [HttpPost()]
        public async Task<IActionResult> create(CateProjectModel data)
        {
            BaseModels<CateProjectModel> model = new BaseModels<CateProjectModel>();
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                SystemLog datalog = new SystemLog();
                data.CreateUserId = loginData.Userid;
                data.CreateTime = DateTime.Now;
                await _repo.Insert(data);
                datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.CREATE, NameCategory_Const.CATE_PROJECT, Action_Status.SUCCESS);
                _asyncLogger.LogInfo(JsonConvert.SerializeObject(datalog));
                model.status = 1;
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
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(CateProjectModel data)
        {
            BaseModels<CateProjectModel> model = new BaseModels<CateProjectModel>();
            try
            {

                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }

                var CheckData = _repo.FindById(data.CateProjectId);
                SystemLog datalog = new SystemLog();
                if (CheckData == null)
                {
                    //chỗ này không tồn tại id
                    model.status = 0;
                    model.error = new ErrorModel()
                    {
                        Code = ErrCode_Const.PROPERTY_IS_NULL_OR_EMPTY
                    };
                    datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.UPDATE, NameCategory_Const.CATE_PROJECT, Action_Status.FAIL);
                    _asyncLogger.LogInfo(JsonConvert.SerializeObject(datalog));
                    return BadRequest(model);
                }
                else
                {

                    data.UpdateTime = DateTime.Now;
                    data.UpdateUserId = loginData.Userid;
                    await _repo.Update(data);
                    datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.UPDATE, NameCategory_Const.CATE_PROJECT, Action_Status.SUCCESS);
                    _asyncLogger.LogInfo(JsonConvert.SerializeObject(datalog));
                }
                model.status = 1;
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
        #region Danh sách đơn vị
        [Route("list-don-vi")]
        [HttpGet]
        public IActionResult LoadDanhSachDonVi()
        {
            BaseModels<Unit> model = new BaseModels<Unit>();

            try
            {
                //Lấy Token, lấy model
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                IQueryable<Unit> _data = _repo._context.Units.Where(x => !x.IsDel && x.UnitCode == "CUR").Select(d => new Unit()
                {
                    UnitId = d.UnitId,
                    UnitCode = d.UnitCode,
                    UnitName = d.UnitName,
                    IsDel = d.IsDel,
                });

                model.status = 1;
                model.items = _data.ToList();
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
        [HttpGet("{id}")]
        public IActionResult getItemById(Guid id)
        {
            BaseModels<CateProjectModel> model = new BaseModels<CateProjectModel>();
            try
            {
                var info = _repo.FindById(id);
                //var storelist = _repo.FindStoreId(id).Select(x => x.CateCriterionId).ToList().ToString();
                if (info == null)
                    return NotFound(ErrMsg_Const.GetMsg(ErrCode_Const.CANNOT_FIND_DATA_BY_QUERY));

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
        //[Route("deletes")]
        //[HttpPut()]
        //public async Task<IActionResult> deletes(List<Guid> IdRemoves)
        //{
        //    BaseModels<object> model = new BaseModels<object>();
        //    try
        //    {
        //        await _repo.Deletes(IdRemoves);
        //        model.status = 1;
        //        return Ok(model);
        //    }
        //    catch (Exception ex)
        //    {

        //        model.status = 0;
        //        model.error = new ErrorModel()
        //        {     
        //            Code = ErrCode_Const.EXCEPTION_API,
        //            Msg = ex.Message
        //        };
        //        return BadRequest(model);
        //    }
        //}
        [HttpPut("delete/{id}")]
        public async Task<IActionResult> delete(Guid id)
        {
            BaseModels<object> model = new BaseModels<object>();
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                SystemLog datalog = new SystemLog();
                await _repo.Delete(id);
                datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.DELETE, NameCategory_Const.CATE_PROJECT, Action_Status.SUCCESS);
                _asyncLogger.LogInfo(JsonConvert.SerializeObject(datalog));
                model.status = 1;
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

        [HttpGet("GetListProject")]
        public async Task<IActionResult> GetListProject()
        {
            BaseModels<List<CateProject>> model = new BaseModels<List<CateProject>>();
            try
            {
                var rs = _repo.GetListProject();
                model.data = rs;
                model.status = 1;
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

        [HttpGet("GetListById/{id}")]
        public async Task<IActionResult> GetListProjectById(Guid Id)
        {
            BaseModels<CateProject> model = new BaseModels<CateProject>();
            try
            {
                var rs = _repo.GetListProjectById(Id);
                model.data = rs;
                model.status = 1;
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

        private List<CateProjectModel> FindData([FromBody] QueryRequestBody query)//query truyền lên
        {
            bool _orderBy_ASC = true;  //Khởi tạo sắp xếp dữ liệu acs hoặc desc khi tìm kiếm
            string _keywordSearch = "";
            Func<CateProjectModel, object> _orderByExpression = x => x.ProjectType; //Khởi tạo mặc định sắp xếp dữ liệu
            Dictionary<string, Func<CateProjectModel, object>> _sortableFields = new Dictionary<string, Func<CateProjectModel, object>>   //Khởi tạo các trường để sắp xếp
                    {
                    { "projectTypeName", x => x.ProjectType },
                    { "Investors", x => x.Investors },
                    { "ProjectName", x => x.ProjectName },
                    { "ProjectInvestment", x => x.ProjectInvestment },
                    { "CreateName", x => x.CreateUserId },
                    { "CreateTimeDisplay", x => x.CreateTime },
                    { "IsAction", x => x.IsAction },
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

            IQueryable<CateProjectModel> _data = _repo._context.CateProjects.Where(x => !x.IsDel).GroupJoin(
                _repo._context.Users,
                cc => cc.CreateUserId,
                u => u.UserId,
                 (cc, u) => new { cc, u }).SelectMany(result => result.u.DefaultIfEmpty(), (info, us) => new CateProjectModel
                 {
                     CateProjectId = info.cc.CateProjectId,
                     ProjectTypeName = (info.cc.ProjectType == 1 ? "Lũy kế DDI ngoài Khu công nghiệp" : (info.cc.ProjectType == 2 ? "Lũy kế FDI ngoài Khu công nghiệp" : (info.cc.ProjectType == 3 ? "Thu hồi lũy kế" : "Mua bán góp vốn"))),
                     ProjectName = info.cc.ProjectName,
                     Investors = info.cc.Investors,

                     CreateTime = info.cc.CreateTime,
                     ProjectType = info.cc.ProjectType,
                     Address = info.cc.Address,
                     InvestmentCertificateCode = info.cc.InvestmentCertificateCode,
                     InvestmentCertificateDate = info.cc.InvestmentCertificateDate,
                     PolicyDecisions = info.cc.PolicyDecisions,
                     ProjectInvestment = info.cc.ProjectInvestment,
                     ProjectAddress = info.cc.ProjectAddress,
                     ProjectImplementationScale = info.cc.ProjectImplementationScale,
                     ProjectLegalRepresent = info.cc.ProjectLegalRepresent,
                     ProjectPhoneNumber = info.cc.ProjectPhoneNumber,
                     ProjectProgress = info.cc.ProjectProgress,
                     ProjectOperatingTime = info.cc.ProjectOperatingTime,
                     ProjectProgressActual = info.cc.ProjectProgressActual,
                     ProjectLocalArea = info.cc.ProjectLocalArea,
                     ProjectLicenseYear = info.cc.ProjectLicenseYear,
                     Profession = info.cc.Profession,
                     ProjectPartnerNationality = info.cc.ProjectPartnerNationality,
                     ProjectInvestmentForm = info.cc.ProjectInvestmentForm,
                     ProjectImplementationYear = info.cc.ProjectImplementationYear,
                     ProjectDecisionToWithdraw = info.cc.ProjectDecisionToWithdraw,
                     InitialCharterCapital = info.cc.InitialCharterCapital,
                     CapitalPurchase = info.cc.CapitalPurchase,
                     ActualPurchase = info.cc.ActualPurchase,
                     CharterCapitalAfterPurchase = info.cc.CharterCapitalAfterPurchase,
                     CompanyBuy = info.cc.CompanyBuy,
                     CountryName = info.cc.ProjectType == 4 ? _repo._context.Countries.Where( c => !c.IsDel && c.CountryId == info.cc.Country).Select ( c => c.CountryName).FirstOrDefault() : "",
                     CompanySellName = info.cc.ProjectType == 4 ? _repo._context.Businesses.Where( b => !b.IsDel && b.BusinessId == info.cc.CompanySell ).Select(b => b.BusinessNameVi).FirstOrDefault() : "",

                 });

            //_data = _data.Where(x => !x.IsDel);

            if (query.Filter != null && query.Filter.ContainsKey("ProjectType")
                && !string.IsNullOrEmpty(query.Filter["ProjectType"]))
            {
                _data = _data.Where(x => x.ProjectType.ToString() == query.Filter["ProjectType"]);
            }

            if (query.SearchValue != null && query.SearchValue != "") //Kiểm tra điều kiện tìm kiếm
            {
                _keywordSearch = query.SearchValue.Trim().ToLower();
                _data = _data.Where(x => x.ProjectTypeName.ToLower().Contains(_keywordSearch)
                || x.ProjectName.ToLower().Contains(_keywordSearch)
                || x.Investors.ToLower().Contains(_keywordSearch)
                || x.ProjectInvestment.ToString().Contains(_keywordSearch)
                );  //Lấy table đã select tìm kiếm theo keyword
            }
            // model.items = _data.ToList();

            return _data.ToList();
        }

        [HttpPost("Export")]
        public IActionResult Export([FromBody] QueryRequestBody query)
        {
            var data = FindData(query).GroupBy(x => x.ProjectType).ToList();

            if (!data.Any())
            {
                return BadRequest();
            }

            try
            {

                using (var workbook = new XLWorkbook(@"Upload/Templates/QuanLyDuAnFDIDDI.xlsx"))
                {
                    foreach (var group in data)
                    {
                        IXLWorksheet worksheet = workbook.Worksheets.Worksheet(group.Key);

                        int index = 4;
                        int row = 1;
                        if(group.Key == 1)
                        {

                            foreach (var item in group)
                            {
                                var addrow = worksheet.Row(index);
                                addrow.InsertRowsBelow(1);
                                worksheet.Cell(index, 1).Value = row;
                                worksheet.Cell(index, 2).Value = item.Investors;
                                worksheet.Cell(index, 3).Value = item.ProjectName;
                                worksheet.Cell(index, 4).Value = item.Address;

                                worksheet.Cell(index, 5).Value = item.InvestmentCertificateCode;
                                worksheet.Cell(index, 6).Value = item.InvestmentCertificateDate;
                                worksheet.Cell(index, 7).Value = item.PolicyDecisions;
                                worksheet.Cell(index, 8).Value = item.ProjectInvestment;
                                worksheet.Cell(index, 9).Value = item.ProjectAddress;
                                worksheet.Cell(index, 10).Value = item.ProjectImplementationScale;
                                worksheet.Cell(index, 11).Value = $"{item.ProjectLegalRepresent} \r\n {item.ProjectPhoneNumber}";
                                worksheet.Cell(index, 12).Value = item.ProjectProgress;
                                worksheet.Cell(index, 13).Value = item.ProjectOperatingTime;
                                worksheet.Cell(index, 14).Value = item.ProjectProgressActual;
                                worksheet.Cell(index, 15).Value = item.ProjectLocalArea;
                                worksheet.Cell(index, 16).Value = item.ProjectLicenseYear;



                                index++;
                                row++;
                            }
                           
                            worksheet.Row(index).Delete();
                        }
                        else if (group.Key == 2)
                        {

                            foreach (var item in group)
                            {
                                var addrow = worksheet.Row(index);
                                addrow.InsertRowsBelow(1);
                                worksheet.Cell(index, 1).Value = row;
                                worksheet.Cell(index, 2).Value = item.Investors;
                                worksheet.Cell(index, 3).Value = item.ProjectName;
                                worksheet.Cell(index, 4).Value = item.Address;

                                worksheet.Cell(index, 5).Value = item.InvestmentCertificateCode;
                                worksheet.Cell(index, 6).Value = item.InvestmentCertificateDate;
                                worksheet.Cell(index, 7).Value = item.PolicyDecisions;
                                worksheet.Cell(index, 8).Value = item.ProjectInvestment;
                                worksheet.Cell(index, 9).Value = item.ProjectAddress;
                                worksheet.Cell(index, 10).Value = item.Profession;
                                worksheet.Cell(index, 11).Value = $"{item.ProjectLegalRepresent} \r\n {item.ProjectPhoneNumber}";
                                worksheet.Cell(index, 12).Value = item.ProjectProgress;
                                worksheet.Cell(index, 13).Value = item.ProjectOperatingTime;
                                worksheet.Cell(index, 14).Value = item.ProjectProgressActual;
                                worksheet.Cell(index, 15).Value = item.ProjectLocalArea;
                                worksheet.Cell(index, 16).Value = item.ProjectPartnerNationality;
                                worksheet.Cell(index, 17).Value = item.ProjectInvestmentForm;
                                worksheet.Cell(index, 18).Value = item.ProjectLicenseYear;
                                worksheet.Cell(index, 19).Value = item.ProjectImplementationYear;

                                index++;
                                row++;
                            }

                            worksheet.Row(index).Delete();
                        }
                        else if (group.Key == 3)
                        {

                            foreach (var item in group)
                            {
                                var addrow = worksheet.Row(index);
                                addrow.InsertRowsBelow(1);
                                worksheet.Cell(index, 1).Value = row;
                                worksheet.Cell(index, 2).Value = item.Investors;
                                worksheet.Cell(index, 3).Value = item.ProjectName;
                                worksheet.Cell(index, 4).Value = item.Address;

                                worksheet.Cell(index, 5).Value = item.InvestmentCertificateCode;
                                worksheet.Cell(index, 6).Value = item.InvestmentCertificateDate;
                                worksheet.Cell(index, 7).Value = item.PolicyDecisions;
                                worksheet.Cell(index, 8).Value = item.ProjectInvestment;
                                worksheet.Cell(index, 9).Value = item.ProjectAddress;
                                worksheet.Cell(index, 10).Value = item.ProjectImplementationScale;
                                worksheet.Cell(index, 11).Value = $"{item.ProjectLegalRepresent} \r\n {item.ProjectPhoneNumber}";
                                worksheet.Cell(index, 12).Value = item.ProjectProgress;
                                worksheet.Cell(index, 13).Value = item.ProjectOperatingTime;
                                worksheet.Cell(index, 14).Value = item.ProjectLocalArea;
                                worksheet.Cell(index, 15).Value = item.ProjectLicenseYear;
                                worksheet.Cell(index, 16).Value = item.ProjectDecisionToWithdraw;

                                index++;
                                row++;
                            }

                            worksheet.Row(index).Delete();
                        }
                        else
                        {
                            foreach (var item in group)
                            {
                                var addrow = worksheet.Row(index);
                                addrow.InsertRowsBelow(1);
                                worksheet.Cell(index, 1).Value = row;
                                worksheet.Cell(index, 2).Value = item.CompanyBuy;
                                worksheet.Cell(index, 3).Value = item.CountryName; // Quoc gia
                                worksheet.Cell(index, 4).Value = item.CompanySellName; // Cty nhận 

                                worksheet.Cell(index, 5).Value = item.InitialCharterCapital; // 
                                worksheet.Cell(index, 6).Value = item.CapitalPurchase;
                                worksheet.Cell(index, 7).Value = item.ActualPurchase;
                                worksheet.Cell(index, 8).Value = item.CharterCapitalAfterPurchase;


                                index++;
                                row++;
                            }

                            worksheet.Row(index).Delete();
                        }
                    }

                    using (var stream = new MemoryStream())
                    {
                        workbook.SaveAs(stream);
                        stream.Flush();
                        stream.Position = 0;

                        return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "file.xlsx");
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
