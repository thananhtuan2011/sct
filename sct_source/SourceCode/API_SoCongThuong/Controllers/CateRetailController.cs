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
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using System.Runtime.Intrinsics.X86;
using ClosedXML.Excel;
using API_SoCongThuong.Logger;
using Newtonsoft.Json;
using System.Data;

namespace API_SoCongThuong.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CateRetailController : ControllerBase
    {
        private CateRetailRepo _repo;
        private IConfiguration _configuration;
        private readonly ILogger<AsyncLogger> _logger;
        public SoHoa_SoCongThuongContext _context;
        private AsyncLogger _asyncLogger;
        public CateRetailController(SoHoa_SoCongThuongContext context, IConfiguration configuration, ILogger<AsyncLogger> logger)
        {
            _repo = new CateRetailRepo(context);
            _logger = logger;
            _context = context;
            _asyncLogger = new AsyncLogger(_logger, _context);
            _configuration = configuration;
        }

        [Route("find")]
        [HttpPost]
        public IActionResult ListItems_New([FromBody] QueryRequestBody query)
        {

            BaseModels<CateRetailModel> model = new BaseModels<CateRetailModel>();
            string _keywordSearch = "";
            bool _orderBy_ASC = false;
            try
            {
                //Lấy Token, lấy model
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }

                Func<CateRetailModel, object> _orderByExpression = x => x.CateRetailCode;
                Dictionary<string, Func<CateRetailModel, object>> _sortableFields = new Dictionary<string, Func<CateRetailModel, object>>
                    {
                        { "cateRetailCode", x => x.CateRetailCode },
                        { "ConfirmName", x => x.ConfirmName },
                        { "CheckName", x => x.CheckName },
                        { "ConfirmTime", x => x.ConfirmTime },
                        { "CreateTime", x => x.CreateTime },
                        { "IsAction", x => x.IsAction },
                    };

                if (query.Sort != null
                    && !string.IsNullOrEmpty(query.Sort.ColumnName)
                    && _sortableFields.ContainsKey(query.Sort.ColumnName))
                {
                    _orderBy_ASC = ("desc".Equals(query.Sort.Direction.ToLower()) ? false : true);
                    _orderByExpression = _sortableFields[query.Sort.ColumnName];
                }

                IQueryable<CateRetailModel> _data = _repo._context.CateRetails.Where(x => !x.IsDel)
                    .GroupJoin(
                        _repo._context.Users,
                        cc => cc.CreateUserId,
                        u => u.UserId,
                        (cc, u) => new { cc, u })
                    .SelectMany(rs => rs.u.DefaultIfEmpty(), (info, use) => new { info, use })
                    .GroupJoin(
                        _repo._context.Users,
                        query1 => query1.info.cc.ConfirmUserId,
                        u2 => u2.UserId,
                        (query1, u2) => new { query1, u2 })
                    .SelectMany(rs => rs.u2.DefaultIfEmpty(), (info1, use1) => new { info1, use1 })
                    .GroupJoin(
                        _repo._context.Users,
                        query2 => query2.info1.query1.info.cc.CheckUserId,
                        u3 => u3.UserId,
                        (query2, u3) => new { query2, u3 })
                    .SelectMany(rs => rs.u3.DefaultIfEmpty(), (info2, use2) => new CateRetailModel
                    {
                        CateRetailId = info2.query2.info1.query1.info.cc.CateRetailId,
                        CateRetailCode = info2.query2.info1.query1.info.cc.CateRetailCode,
                        CreateName = info2.query2.info1.query1.use.FullName,
                        CreateTimeDisplay = info2.query2.info1.query1.info.cc.CreateTime.ToString("dd'/'MM'/'yyyy"),
                        ConfirmTimeDisplay = info2.query2.info1.query1.info.cc.ConfirmTime.HasValue ? info2.query2.info1.query1.info.cc.ConfirmTime.Value.ToString("dd'/'MM'/'yyyy") : "",
                        ConfirmName = info2.query2.use1.FullName,
                        CheckName = use2.FullName,
                        CreateTime = info2.query2.info1.query1.info.cc.CreateTime,
                        CreateUserId = info2.query2.info1.query1.info.cc.CreateUserId
                    }).ToList().AsQueryable();

                if (query.SearchValue != null && query.SearchValue != "")
                {
                    _keywordSearch = query.SearchValue.Trim().ToLower();
                    _data = _data.Where(x => x.CateRetailCode.ToLower().Contains(_keywordSearch)
                    || x.CreateName.ToLower().Contains(_keywordSearch)
                    || x.ConfirmName.ToLower().Contains(_keywordSearch)
                    || x.CheckName.ToLower().Contains(_keywordSearch)
                    );
                }

                if (query.Filter != null && query.Filter.ContainsKey("InputDataPersonId"))
                {
                    var inputDataPersonIdFilter = query.Filter["InputDataPersonId"];
                    _data = _data.Where(x => Guid.Parse(inputDataPersonIdFilter) == x.CreateUserId);
                }

                if (query.Filter != null && query.Filter.ContainsKey("MinTime")
                  && !string.IsNullOrEmpty(query.Filter["MinTime"]))
                {
                    _data = _data.Where(x =>
                                (x.CreateTime) >=
                                DateTime.ParseExact(query.Filter["MinTime"], "dd/MM/yyyy", null));
                }

                if (query.Filter != null && query.Filter.ContainsKey("MaxTime")
                    && !string.IsNullOrEmpty(query.Filter["MaxTime"]))
                {
                    _data = _data.Where(x =>
                               x.CreateTime <=
                                DateTime.ParseExact(query.Filter["MaxTime"], "dd/MM/yyyy", null));
                }

                int _countRows = _data.Count();
                if (_countRows == 0)
                {
                    return NotFound("Không có dữ liệu");
                }
                if (query.Panigator.More)
                {
                    model.status = 1;
                    model.items = _data.ToList();
                    model.total = _countRows;
                    return Ok(model);
                }
                if (_orderBy_ASC)
                {
                    model.items = _data
                        .OrderBy(x => x.CreateTime)
                        .Skip((query.Panigator.PageIndex - 1) * query.Panigator.PageSize)
                        .Take(query.Panigator.PageSize)
                        .ToList();
                }
                else
                {
                    model.items = _data
                        .OrderByDescending(x => x.CreateTime)
                        .Skip((query.Panigator.PageIndex - 1) * query.Panigator.PageSize)
                        .Take(query.Panigator.PageSize)
                        .ToList();
                }

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

        [HttpPost()]
        public async Task<IActionResult> create(CateRetailModel data)
        {
            BaseModels<CateRetailModel> model = new BaseModels<CateRetailModel>();
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                SystemLog datalog = new SystemLog();
                await _repo.Insert(data);
                datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.CREATE, NameCategory_Const.CATE_RETAIL, Action_Status.SUCCESS);
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
        public async Task<IActionResult> Update(CateRetailModel data)
        {
            BaseModels<CateCriterion> model = new BaseModels<CateCriterion>();
            try
            {

                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }

                var CheckData = _repo.FindById(data.CateRetailId);
                SystemLog datalog = new SystemLog();
                if (CheckData == null)
                {
                    model.status = 0;
                    model.error = new ErrorModel()
                    {
                        Code = ErrCode_Const.PROPERTY_IS_NULL_OR_EMPTY
                    };
                    datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.UPDATE, NameCategory_Const.CATE_RETAIL, Action_Status.FAIL);
                    _asyncLogger.LogInfo(JsonConvert.SerializeObject(datalog));
                    return BadRequest(model);
                }
                else
                {
                    if (string.IsNullOrEmpty(data.CateRetailCode))
                    {
                        model.status = 0;
                        model.error = new ErrorModel()
                        {
                            Code = ErrCode_Const.PROPERTY_IS_NULL_OR_EMPTY
                        };
                        datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.UPDATE, NameCategory_Const.CATE_RETAIL, Action_Status.FAIL);
                        _asyncLogger.LogInfo(JsonConvert.SerializeObject(datalog));
                        return BadRequest(model);
                    }

                    data.UpdateTime = DateTime.Now;
                    data.UpdateUserId = loginData.Userid;
                    await _repo.Update(data);
                    datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.UPDATE, NameCategory_Const.CATE_RETAIL, Action_Status.SUCCESS);
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

        [Route("deletes")]
        [HttpPut()]
        public async Task<IActionResult> deletes(List<Guid> IdRemoves)
        {
            BaseModels<CateCriterion> model = new BaseModels<CateCriterion>();
            try
            {
                await _repo.Deletes(IdRemoves);
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

        [HttpPut("delete/{id}")]
        public async Task<IActionResult> delete(Guid id)
        {
            BaseModels<CateCriterion> model = new BaseModels<CateCriterion>();
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                SystemLog datalog = new SystemLog();
                await _repo.Delete(id);
                datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.DELETE, NameCategory_Const.CATE_RETAIL, Action_Status.SUCCESS);
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

        [Route("loaduser")]
        [HttpGet]
        public IActionResult LoadUser()
        {
            BaseModels<User> model = new BaseModels<User>();
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }

                IQueryable<User> _data = _repo.FindUser();

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

        [Route("loadcategory")]
        [HttpGet]
        public IActionResult LoadCategory()
        {
            BaseModels<Category> model = new BaseModels<Category>();
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }

                IQueryable<Category> _data = _repo.FindCriteria();

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

        [HttpGet("{id}")]
        public IActionResult getItemById(Guid id)
        {
            BaseModels<CateRetailModel> model = new BaseModels<CateRetailModel>();
            try
            {
                var info = _repo.FindById(id);
                if (info == null)
                    return NotFound(ErrMsg_Const.GetMsg(ErrCode_Const.CANNOT_FIND_DATA_BY_QUERY));

                var MonthString = info.ReportMonth;
                var LDate = MonthString.Split('-');
                var LastYearString = (int.Parse(LDate[0]) - 1).ToString() + "-" + LDate[1];
                var old_id = _repo._context.CateRetails.Where(x => x.ReportMonth == LastYearString && !x.IsDel).FirstOrDefault();

                CateRetailModel result = new CateRetailModel()
                {
                    CateRetailCode = info.CateRetailCode,
                    CateRetailId = info.CateRetailId,
                    CheckUserId = info.CheckUserId,
                    ConfirmUserId = info.ConfirmUserId,
                    CreateUserId = info.CreateUserId,
                    ConfirmTime = info.ConfirmTime,
                    CreateTime = info.CreateTime,
                    ReportMonth = info.ReportMonth,
                };

                if (old_id == null)
                {
                    result.Details = _repo.FindDetailById(id);
                }
                else
                {
                    result.Details = _repo.FindDetailById(id, old_id.CateRetailId);
                };

                model.status = 1;
                model.data = result;
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

        [HttpPost("ExportExcel/{id}")]
        public IActionResult ExportExcel(Guid id)
        {
            try
            {
                var data = _repo.FindById(id);

                if (data == null)
                {
                    return BadRequest();
                }

                var MonthString = data.ReportMonth;
                var LDate = MonthString.Split('-');
                var LastYearString = (int.Parse(LDate[0]) - 1).ToString() + "-" + LDate[1];
                var old_id = _repo._context.CateRetails.Where(x => x.ReportMonth == LastYearString && !x.IsDel).FirstOrDefault();

                CateRetailModel result = new CateRetailModel()
                {
                    CateRetailCode = data.CateRetailCode,
                    CateRetailId = data.CateRetailId,
                    CheckUserId = data.CheckUserId,
                    ConfirmUserId = data.ConfirmUserId,
                    CreateUserId = data.CreateUserId,
                    ConfirmTime = data.ConfirmTime,
                    CreateTime = data.CreateTime,
                    ReportMonth = data.ReportMonth,
                };

                if (old_id == null)
                {
                    result.Details = _repo.FindDetailById(id);
                }
                else
                {
                    result.Details = _repo.FindDetailById(id, old_id.CateRetailId);
                };

                using (var workbook = new XLWorkbook(@"Upload/Templates/Quanlytongmucbanlehanghoa.xlsx"))
                {
                    IXLWorksheet worksheet = workbook.Worksheets.Worksheet(1);

                    string Month = LDate[1];
                    string Year = LDate[0];
                    worksheet.Cell(7, 1).Value = $"Tháng {Month} năm {Year}";

                    var range = worksheet.Cells("B13:B16");

                    foreach (var item in result.Details)
                    {
                        var SearchKeywork = Ulities.RemoveUnicode(item.CriteriaName).ToLower().Trim();

                        var cell = range.FirstOrDefault(x => Ulities.RemoveUnicode(x.Value.ToString().ToLower().Trim()).Contains(SearchKeywork));

                        if (cell != null)
                        {
                            int row = cell.Address.RowNumber;
                            if (item.Type == 1)
                            {
                                if (cell != null)
                                {
                                    worksheet.Cell(row, 3).Value = item.PerformLastmonth;
                                    worksheet.Cell(row, 4).Value = item.EstimateReportingMonth;
                                    worksheet.Cell(row, 5).Value = item.CumulativeToReportingMonth;
                                }
                            }
                            else if (item.Type == 2)
                            {
                                if (cell != null)
                                {
                                    worksheet.Cell(row, 6).Value = item.EstimateReportingMonth;
                                    worksheet.Cell(row, 7).Value = item.CumulativeToReportingMonth;
                                }
                            }
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
