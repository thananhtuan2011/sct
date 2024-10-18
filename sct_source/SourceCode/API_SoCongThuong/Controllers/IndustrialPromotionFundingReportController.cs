using EF_Core.Models;
using Microsoft.AspNetCore.Mvc;
using API_SoCongThuong.Classes;
using API_SoCongThuong.Models;
using API_SoCongThuong.Reponsitories;
using ClosedXML.Excel;
using API_SoCongThuong.Logger;
using Newtonsoft.Json;
using System.Data;

namespace API_SoCongThuong.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IndustrialPromotionFundingReport : ControllerBase
    {
        private IndustrialPromotionFundingReportRepo _repo;
        private IConfiguration _configuration;
        private readonly ILogger<AsyncLogger> _logger;
        public SoHoa_SoCongThuongContext _context;
        private AsyncLogger _asyncLogger;

        public IndustrialPromotionFundingReport(SoHoa_SoCongThuongContext context, IConfiguration configuration, ILogger<AsyncLogger> logger)
        {
            _repo = new IndustrialPromotionFundingReportRepo(context);
            _logger = logger;
            _context = context;
            _asyncLogger = new AsyncLogger(_logger, _context);
            _configuration = configuration;

        }

        [Route("find")]
        [HttpPost]
        public IActionResult ListItems_New([FromBody] QueryRequestBody query)
        {
            BaseModels<IndustrialPromotionFundingReportModel> model = new BaseModels<IndustrialPromotionFundingReportModel>();
            string _keywordSearch = "";
            bool _orderBy_ASC = false;
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }

                Func<IndustrialPromotionFundingReportModel, object> _orderByExpression = x => x.RpIndustrialPromotionFundingId;
                Dictionary<string, Func<IndustrialPromotionFundingReportModel, object>> _sortableFields = new Dictionary<string, Func<IndustrialPromotionFundingReportModel, object>>
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

                var _data = from A in _repo._context.IndustrialPromotionFundingReports.Where(x => !x.IsDel)
                            where !A.IsDel
                            join C in _repo._context.Categories.Select(x => new { x.CategoryId, x.CategoryName })
                                on A.Targets equals C.CategoryId
                            select (new IndustrialPromotionFundingReportModel
                            {
                                RpIndustrialPromotionFundingId = A.RpIndustrialPromotionFundingId,
                                YearReport = A.YearReport,
                                NationalReport = A.NationalReport,
                                LocalReport = A.LocalReport,
                                Targets = A.Targets,
                                TargetsName = C.CategoryName,
                                Unit = A.Unit,
                                IsDel = A.IsDel
                            });

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
        public IActionResult LoadTargets()
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
                IQueryable<Category> _data = _repo._context.Categories.Where(x => x.CategoryTypeCode == "TARGETS_OF_INDUSTRY_PROMOTION");

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
        public async Task<IActionResult> Create(IndustrialPromotionFundingReportModel data)
        {
            BaseModels<IndustrialPromotionFundingReportModel> model = new BaseModels<IndustrialPromotionFundingReportModel>();
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }

                var Check = _repo._context.IndustrialPromotionFundingReports.Where(x => x.Targets == data.Targets && x.YearReport == data.YearReport);
                if (!Check.Any())
                {
                    SystemLog datalog = new SystemLog();
                    data.CreateUserId = loginData.Userid;
                    data.CreateTime = DateTime.Now;

                    data = new Ulities().TrimModel(data);

                    await _repo.Insert(data);

                    datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.CREATE, NameCategory_Const.INDUSTRIAL_PROMOTION_FUNDING, Action_Status.SUCCESS);
                    _asyncLogger.LogInfo(JsonConvert.SerializeObject(datalog));
                    model.status = 1;

                    return Ok(model);
                }
                else
                {
                    String targetName = _repo._context.Categories.Where(x => x.CategoryId == Check.Select(i => i.Targets).FirstOrDefault()).Select(x => x.CategoryName).FirstOrDefault()!;

                    model.status = 0;
                    model.error = new ErrorModel()
                    {
                        Code = ErrCode_Const.EXCEPTION_API,
                        Msg = "Báo cáo chỉ tiêu " + targetName + " năm " + data.YearReport + " đã tồn tại",
                    };

                    return BadRequest(model);
                }
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
        public async Task<IActionResult> Update(IndustrialPromotionFundingReportModel data)
        {
            BaseModels<IndustrialPromotionFundingReportModel> model = new BaseModels<IndustrialPromotionFundingReportModel>();
            try
            {

                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }

                var CheckData = _repo.FindById(data.RpIndustrialPromotionFundingId);
                SystemLog datalog = new SystemLog();

                if (CheckData == null)
                {
                    model.status = 0;
                    model.error = new ErrorModel()
                    {
                        Code = ErrCode_Const.PROPERTY_IS_NULL_OR_EMPTY
                    };
                    datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.UPDATE, NameCategory_Const.INDUSTRIAL_PROMOTION_FUNDING, Action_Status.FAIL);
                    _asyncLogger.LogInfo(JsonConvert.SerializeObject(datalog));

                    return BadRequest(model);
                }
                else
                {
                    if (string.IsNullOrEmpty(data.RpIndustrialPromotionFundingId.ToString()))
                    {
                        model.status = 0;
                        model.error = new ErrorModel()
                        {
                            Code = ErrCode_Const.PROPERTY_IS_NULL_OR_EMPTY
                        };
                        datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.UPDATE, NameCategory_Const.INDUSTRIAL_PROMOTION_FUNDING, Action_Status.FAIL);
                        _asyncLogger.LogInfo(JsonConvert.SerializeObject(datalog));

                        return BadRequest(model);
                    }

                    var Check = _repo._context.IndustrialPromotionFundingReports.Where(x => x.Targets == data.Targets && x.YearReport == data.YearReport);
                    if (!Check.Any())
                    {
                        data.UpdateTime = DateTime.Now;
                        data.UpdateUserId = loginData.Userid;

                        data = new Ulities().TrimModel(data);

                        await _repo.Update(data);

                        datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.UPDATE, NameCategory_Const.INDUSTRIAL_PROMOTION_FUNDING, Action_Status.SUCCESS);
                        _asyncLogger.LogInfo(JsonConvert.SerializeObject(datalog));
                    }
                    else
                    {
                        String targetName = _repo._context.Categories.Where(x => x.CategoryId == Check.Select(i => i.Targets).FirstOrDefault()).Select(x => x.CategoryName).FirstOrDefault()!;

                        model.status = 0;
                        model.error = new ErrorModel()
                        {
                            Code = ErrCode_Const.EXCEPTION_API,
                            Msg = "Báo cáo chỉ tiêu " + targetName + " năm " + data.YearReport + " đã tồn tại",
                        };

                        return BadRequest(model);
                    }
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
                datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.DELETE, NameCategory_Const.INDUSTRIAL_PROMOTION_FUNDING, Action_Status.SUCCESS);
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
            BaseModels<IndustrialPromotionFundingReportModel> model = new BaseModels<IndustrialPromotionFundingReportModel>();
            try
            {
                var result = _repo.FindById(id);
                if (result == null)
                {
                    return NotFound(ErrMsg_Const.GetMsg(ErrCode_Const.CANNOT_FIND_DATA_BY_QUERY));
                }

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
            var data = from A in _repo._context.IndustrialPromotionFundingReports.Where(x => !x.IsDel)
                       join C in _repo._context.Categories.Select(x => new { x.CategoryId, x.CategoryName })
                           on A.Targets equals C.CategoryId
                       select (new IndustrialPromotionFundingReportModel
                       {
                           RpIndustrialPromotionFundingId = A.RpIndustrialPromotionFundingId,
                           YearReport = A.YearReport,
                           NationalReport = A.NationalReport,
                           LocalReport = A.LocalReport,
                           Targets = A.Targets,
                           TargetsName = C.CategoryName,
                           Unit = A.Unit,
                           IsDel = A.IsDel
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
                using (var workbook = new XLWorkbook(@"Upload/Templates/Baocaokinhphikhuyencong.xlsx"))
                {
                    IXLWorksheet worksheet = workbook.Worksheets.Worksheet(1);
                    worksheet.Cell(3, 4).Value = $"Năm Báo Cáo: {query.Filter!["Year"]}";

                    foreach (var item in data.ToList())
                    {
                        var CellGroup = worksheet.Cells("B7:B30").FirstOrDefault(x => x.Value.ToString().Contains(item.TargetsName!));
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
