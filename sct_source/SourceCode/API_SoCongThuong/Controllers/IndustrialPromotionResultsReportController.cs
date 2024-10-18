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
using System.Drawing.Drawing2D;
using ClosedXML.Excel;
using System.Net;
using System.Net.Http.Headers;
using System.Collections;
using API_SoCongThuong.Logger;
using Newtonsoft.Json;
using System.Data;

namespace API_SoCongThuong.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IndustrialPromotionResultsReport : ControllerBase
    {
        private IndustrialPromotionResultsReportRepo _repo;
        private IConfiguration _configuration;
        private readonly ILogger<AsyncLogger> _logger;
        public SoHoa_SoCongThuongContext _context;
        private AsyncLogger _asyncLogger;
        public IndustrialPromotionResultsReport(SoHoa_SoCongThuongContext context, IConfiguration configuration, ILogger<AsyncLogger> logger)
        {
            _repo = new IndustrialPromotionResultsReportRepo(context);
            _logger = logger;
            _context = context;
            _asyncLogger = new AsyncLogger(_logger, _context);
            _configuration = configuration;

        }

        [Route("find")]
        [HttpPost]
        public IActionResult ListItems_New([FromBody] QueryRequestBody query)
        {
            BaseModels<IndustrialPromotionResultsReportModel> model = new BaseModels<IndustrialPromotionResultsReportModel>();
            string _keywordSearch = "";
            bool _orderBy_ASC = true;
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }

                Func<IndustrialPromotionResultsReportModel, object> _orderByExpression = x => x.RpIndustrialPromotionResultsId;
                Dictionary<string, Func<IndustrialPromotionResultsReportModel, object>> _sortableFields = new Dictionary<string, Func<IndustrialPromotionResultsReportModel, object>>
                    {
                        { "TargetsName", x => x.TargetsName },
                        { "YearReport", x => x.YearReport },
                        { "NationalReport", x => x.NationalReport },
                        { "LocalReport", x => x.LocalReport },
                        { "Unit", x => x.Unit },
                    };
                if (query.Sort != null && !string.IsNullOrEmpty(query.Sort.ColumnName) && _sortableFields.ContainsKey(query.Sort.ColumnName))
                {
                    _orderBy_ASC = ("desc".Equals(query.Sort.Direction.ToLower()) ? false : true);
                    _orderByExpression = _sortableFields[query.Sort.ColumnName];
                }

                var _data = from A in _repo._context.IndustrialPromotionResultsReports.Where(x => !x.IsDel)
                            join C in _repo._context.Categories.Select(x => new { x.CategoryId, x.CategoryName })
                                on A.Targets equals C.CategoryId
                            select (new IndustrialPromotionResultsReportModel
                            {
                                RpIndustrialPromotionResultsId = A.RpIndustrialPromotionResultsId,
                                YearReport = A.YearReport,
                                NationalReport = A.NationalReport,
                                LocalReport = A.LocalReport,
                                Targets = A.Targets,
                                TargetsName = C.CategoryName,
                                Unit = A.Unit,
                                IsDel = A.IsDel
                            }
                            );

                if (query.SearchValue != null && query.SearchValue != "")
                {
                    _keywordSearch = query.SearchValue.Trim().ToLower();
                    _data = _data.Where(x =>
                        x.YearReport.ToString().Contains(_keywordSearch)
                       || x.NationalReport.ToString().Contains(_keywordSearch)
                       || x.LocalReport.ToString().Contains(_keywordSearch)
                       || x.TargetsName.Contains(_keywordSearch)
                       || x.Unit.Contains(_keywordSearch)
                   );
                }

                if (query.Filter != null && query.Filter.ContainsKey("Year"))
                {
                    _data = _data.Where(x => x.YearReport.ToString().Equals(string.Join("", query.Filter["Year"])));
                }

                int _countRows = _data.Count();
                if (_countRows == 0)
                {
                    return NotFound("Không có dữ liệu");
                }

                if (_orderBy_ASC)
                {
                    model.items = _data
                        .OrderBy(_orderByExpression)
                        .Skip((query.Panigator.PageIndex - 1) * query.Panigator.PageSize)
                        .Take(query.Panigator.PageSize)
                        .ToList();
                }
                else
                {
                    model.items = _data
                        .OrderByDescending(_orderByExpression)
                        .Skip((query.Panigator.PageIndex - 1) * query.Panigator.PageSize)
                        .Take(query.Panigator.PageSize)
                        .ToList();
                }

                if (query.Panigator.More)
                {
                    model.status = 1;
                    model.items = _data.ToList();
                    model.total = _countRows;
                    return Ok(model);
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

        [Route("loadTargets")]
        [HttpGet]
        public IActionResult loadTargets()
        {
            BaseModels<Category> model = new BaseModels<Category>();

            try
            {
                //Lấy Token, lấy model
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                IQueryable<Category> _data = _repo._context.Categories.Where(x => x.CategoryTypeCode == "TARGETS_OF_INDUSTRIAL_PROMOTION_RESULT");

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

        [HttpPost()]
        public async Task<IActionResult> Create(IndustrialPromotionResultsReportModel data)
        {
            BaseModels<IndustrialPromotionResultsReportModel> model = new BaseModels<IndustrialPromotionResultsReportModel>();
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

                data = new Ulities().TrimModel(data);

                await _repo.Insert(data);
                datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.CREATE, NameCategory_Const.INDUSTRIAL_PROMOTION_RESULT, Action_Status.SUCCESS);
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
        public async Task<IActionResult> Update(IndustrialPromotionResultsReportModel data)
        {
            BaseModels<IndustrialPromotionResultsReportModel> model = new BaseModels<IndustrialPromotionResultsReportModel>();
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }

                var CheckData = _repo.FindById(data.RpIndustrialPromotionResultsId);
                SystemLog datalog = new SystemLog();
                if (CheckData == null)
                {
                    model.status = 0;
                    model.error = new ErrorModel()
                    {
                        Code = ErrCode_Const.PROPERTY_IS_NULL_OR_EMPTY
                    };
                    datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.UPDATE, NameCategory_Const.INDUSTRIAL_PROMOTION_RESULT, Action_Status.FAIL);
                    _asyncLogger.LogInfo(JsonConvert.SerializeObject(datalog));

                    return BadRequest(model);
                }
                else
                {
                    if (string.IsNullOrEmpty(data.RpIndustrialPromotionResultsId.ToString()))
                    {
                        model.status = 0;
                        model.error = new ErrorModel()
                        {
                            Code = ErrCode_Const.PROPERTY_IS_NULL_OR_EMPTY
                        };
                        datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.UPDATE, NameCategory_Const.INDUSTRIAL_PROMOTION_RESULT, Action_Status.FAIL);
                        _asyncLogger.LogInfo(JsonConvert.SerializeObject(datalog));

                        return BadRequest(model);
                    }

                    data.UpdateTime = DateTime.Now;
                    data.UpdateUserId = loginData.Userid;

                    data = new Ulities().TrimModel(data);

                    await _repo.Update(data);

                    datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.UPDATE, NameCategory_Const.INDUSTRIAL_PROMOTION_RESULT, Action_Status.SUCCESS);
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
                datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.DELETE, NameCategory_Const.INDUSTRIAL_PROMOTION_RESULT, Action_Status.SUCCESS);
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

        [HttpGet("{id}")]
        public IActionResult getItemById(Guid id)
        {
            BaseModels<IndustrialPromotionResultsReportModel> model = new BaseModels<IndustrialPromotionResultsReportModel>();
            try
            {
                var result = _repo.FindById(id);
                if (result == null)
                    return NotFound(ErrMsg_Const.GetMsg(ErrCode_Const.CANNOT_FIND_DATA_BY_QUERY));

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

        [HttpPost("export")]
        public IActionResult ExportFile([FromBody] QueryRequestBody query)
        {
            //Query data
            var data = from A in _repo._context.IndustrialPromotionResultsReports.Where(x => !x.IsDel)
                       join C in _repo._context.Categories.Select(x => new { x.CategoryId, x.CategoryName })
                           on A.Targets equals C.CategoryId
                       select (new IndustrialPromotionResultsReportModel
                       {
                           RpIndustrialPromotionResultsId = A.RpIndustrialPromotionResultsId,
                           NationalReport = A.NationalReport,
                           LocalReport = A.LocalReport,
                           Targets = A.Targets,
                           TargetsName = C.CategoryName,
                           Unit = A.Unit,
                           IsDel = A.IsDel,
                           YearReport = A.YearReport
                       });

            if (query.Filter != null && query.Filter.ContainsKey("Year"))
            {
                data = data.Where(x => x.YearReport.ToString().Equals(string.Join("", query.Filter["Year"])));
            }

            if (!data.Any())
            {
                return BadRequest();
            }

            try
            {
                using (var workbook = new XLWorkbook(@"Upload/Templates/Baocaoketquacongtackhuyencong.xlsx"))
                {
                    IXLWorksheet worksheet = workbook.Worksheets.Worksheet(1);
                    worksheet.Cell(3, 4).Value = $"Năm Báo Cáo: {query.Filter!["Year"]}";

                    foreach (var item in data.ToList())
                    {
                        var CellGroup = worksheet.Cells("B8:B37").FirstOrDefault(x => x.Value.ToString().Contains(item.TargetsName!));
                        if (CellGroup != null)
                        {
                            var index = CellGroup.Address.RowNumber;
                            worksheet.Cell(index, 3).Value = item.Unit;
                            worksheet.Cell(index, 4).Value = item.LocalReport;
                            worksheet.Cell(index, 5).Value = item.NationalReport;
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
