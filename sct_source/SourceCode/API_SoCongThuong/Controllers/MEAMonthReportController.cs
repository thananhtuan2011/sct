
using API_SoCongThuong.Classes;
using API_SoCongThuong.Logger;
using API_SoCongThuong.Models;
using API_SoCongThuong.Reponsitories.ManagementElectricityActivitiesMonthReportRepo;
using ClosedXML.Excel;
using EF_Core.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Data;
using static API_SoCongThuong.Classes.Ulities;

namespace API_SoCongThuong.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ManagementElectricityActivitiesMonthReportController : ControllerBase
    {
        private ManagementElectricityActivitiesMonthReportRepo _repo;
        private IConfiguration _configuration;
        private readonly ILogger<AsyncLogger> _logger;
        public SoHoa_SoCongThuongContext _context;
        private AsyncLogger _asyncLogger;

        public ManagementElectricityActivitiesMonthReportController(SoHoa_SoCongThuongContext context, IConfiguration configuration, ILogger<AsyncLogger> logger)
        {
            _repo = new ManagementElectricityActivitiesMonthReportRepo(context);
            _logger = logger;
            _context = context;
            _asyncLogger = new AsyncLogger(_logger, _context);
            _configuration = configuration;
        }

        [Route("find")]
        [HttpPost]
        public IActionResult Find([FromBody] QueryRequestBody query)
        {
            BaseModels<ManagementElectricityActivitiesMonthReportModel> model = new BaseModels<ManagementElectricityActivitiesMonthReportModel>();
            string _keywordSearch = "";
            bool _orderBy_ASC = true;
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }

                Func<ManagementElectricityActivitiesMonthReportModel, object> _orderByExpression = x => x.Month;
                Dictionary<string, Func<ManagementElectricityActivitiesMonthReportModel, object>> _sortableFields = new Dictionary<string, Func<ManagementElectricityActivitiesMonthReportModel, object>>
                    {
                        { "Month", x => x.Month },
                        { "Year", x => x.Year },
                    };
                if (query.Sort != null && !string.IsNullOrEmpty(query.Sort.ColumnName) && _sortableFields.ContainsKey(query.Sort.ColumnName))
                {
                    _orderBy_ASC = ("desc".Equals(query.Sort.Direction.ToLower()) ? false : true);
                    _orderByExpression = _sortableFields[query.Sort.ColumnName];
                }

                IQueryable<ManagementElectricityActivitiesMonthReportModel> _data = (from m in _repo._context.MeaMonthReports
                                                                                     where !m.IsDel
                                                                                     select new ManagementElectricityActivitiesMonthReportModel
                                                                                     {
                                                                                         MonthReportId = m.ReportMonthId,
                                                                                         UpdateDate = m.UpdateDate.ToString("dd'/'MM'/'yyyy"),
                                                                                         Month = m.Month,
                                                                                         Year = m.Year
                                                                                     }).ToList().AsQueryable();

                if (query.SearchValue != null && query.SearchValue != "")
                {
                    _keywordSearch = query.SearchValue.Trim().ToLower();
                    _data = _data.Where(x => x.UpdateDate.ToLower().Contains(_keywordSearch)
                            || x.Month.ToString().Contains(_keywordSearch)
                            || x.Year.ToString().Contains(_keywordSearch));
                }

                if (query.Filter != null && query.Filter.ContainsKey("Month") && !string.IsNullOrEmpty(query.Filter["Month"]))
                {
                    _data = _data.Where(x => x.Month.ToString() == query.Filter["Month"]);
                }

                if (query.Filter != null && query.Filter.ContainsKey("Year") && !string.IsNullOrEmpty(query.Filter["Year"]))
                {
                    _data = _data.Where(x => x.Year.ToString() == query.Filter["Year"]);
                }

                int _countRows = _data.Count();
                if (_countRows == 0)
                {
                    return NotFound("Không có dữ liệu");
                }

                if (_orderBy_ASC)
                {
                    model.items = _data
                        .OrderBy(x => x.Month)
                        .ThenBy(x => x.Year)
                        .Skip((query.Panigator.PageIndex - 1) * query.Panigator.PageSize)
                        .Take(query.Panigator.PageSize)
                        .ToList();
                }
                else
                {
                    model.items = _data
                        .OrderByDescending(x => x.Month)
                        .ThenByDescending(x => x.Year)
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

        [HttpPost()]
        public async Task<IActionResult> Create([FromForm] ManagementElectricityActivitiesMonthReportModel data)
        {
            BaseModels<MeaMonthReport> model = new BaseModels<MeaMonthReport>();
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                SystemLog datalog = new SystemLog();

                var util = new Ulities();
                data = util.TrimModel(data);

                data.CreateUserId = loginData.Userid;
                data.CreateTime = DateTime.Now;

                var Files = Request.Form.Files;
                var LstFile = new List<MonthReportAttchFileModel>();
                if (Files.Any())
                {
                    foreach (var f in Files)
                    {
                        if (f.Length > 0)
                        {
                            using (var ms = new MemoryStream())
                            {
                                f.CopyTo(ms);
                                upLoadFileModel up = new upLoadFileModel()
                                {
                                    bs = ms.ToArray(),
                                    FileName = f.FileName.Replace(" ", ""),
                                    Linkfile = "ManagementElectricityActivitiesMonthReport"
                                };
                                var result = Ulities.UploadFile(up, _configuration);

                                MonthReportAttchFileModel fileSave = new MonthReportAttchFileModel();
                                fileSave.LinkFile = result.link;
                                LstFile.Add(fileSave);
                            }
                        }
                    }
                }

                data.AllFile = LstFile;

                await _repo.Insert(data);

                datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.CREATE, NameCategory_Const.MANAGEMENT_ELECTRICITY_ACTIVITIES_MONTH_REPORT, Action_Status.SUCCESS);
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
        public IActionResult GetItemById(Guid id)
        {
            BaseModels<ManagementElectricityActivitiesMonthReportModel> model = new BaseModels<ManagementElectricityActivitiesMonthReportModel>();
            try
            {
                var result = _repo.FindById(id, _configuration);
                if (result != null)
                {
                    model.status = 1;
                    model.items = result.ToList();
                    return Ok(model);
                }
                else
                {
                    model.status = 0;
                    model.error = new ErrorModel()
                    {
                        Code = ErrCode_Const.CANNOT_FIND_DATA_BY_QUERY,
                        Msg = "Không có dữ liệu này trên DB",
                    };
                    return NotFound(model);
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
        public async Task<IActionResult> Update([FromForm] ManagementElectricityActivitiesMonthReportModel data)
        {
            BaseModels<MeaMonthReport> model = new BaseModels<MeaMonthReport>();
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                SystemLog datalog = new SystemLog();
                MeaMonthReport? SaveData = _repo._context.MeaMonthReports.Where(x => x.ReportMonthId == data.MonthReportId && !x.IsDel).FirstOrDefault();
                if (SaveData != null)
                {
                    var util = new Ulities();
                    data = util.TrimModel(data);

                    data.CreateUserId = loginData.Userid;
                    data.CreateTime = DateTime.Now;

                    var Files = Request.Form.Files;
                    var LstFile = new List<MonthReportAttchFileModel>();
                    if (Files.Any())
                    {
                        foreach (var f in Files)
                        {
                            if (f.Length > 0)
                            {
                                using (var ms = new MemoryStream())
                                {
                                    f.CopyTo(ms);
                                    upLoadFileModel up = new upLoadFileModel()
                                    {
                                        bs = ms.ToArray(),
                                        FileName = f.FileName.Replace(" ", ""),
                                        Linkfile = "ManagementElectricityActivitiesMonthReport"
                                    };
                                    var result = Ulities.UploadFile(up, _configuration);

                                    MonthReportAttchFileModel fileSave = new MonthReportAttchFileModel();
                                    fileSave.LinkFile = result.link;
                                    LstFile.Add(fileSave);
                                }
                            }
                        }
                    }
                    data.AllFile = LstFile;

                    await _repo.Update(SaveData, data, _configuration);

                    datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.UPDATE, NameCategory_Const.MANAGEMENT_ELECTRICITY_ACTIVITIES_MONTH_REPORT, Action_Status.SUCCESS);
                    _asyncLogger.LogInfo(JsonConvert.SerializeObject(datalog));
                    model.status = 1;
                    return Ok(model);
                }
                else
                {
                    model.status = 0;
                    model.error = new ErrorModel()
                    {
                        Code = ErrCode_Const.CANNOT_FIND_DATA_BY_QUERY,
                        Msg = "Không có dữ liệu này trên DB",
                    };
                    datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.UPDATE, NameCategory_Const.MANAGEMENT_ELECTRICITY_ACTIVITIES_MONTH_REPORT, Action_Status.FAIL);
                    _asyncLogger.LogInfo(JsonConvert.SerializeObject(datalog));
                    return NotFound(model);
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

        [HttpPut("delete/{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            BaseModels<MeaMonthReport> model = new BaseModels<MeaMonthReport>();
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                SystemLog datalog = new SystemLog();
                MeaMonthReport? DeleteData = _repo._context.MeaMonthReports.Where(x => x.ReportMonthId == id && !x.IsDel).FirstOrDefault();
                if (DeleteData != null)
                {
                    DeleteData.IsDel = true;
                    await _repo.Delete(DeleteData, _configuration);
                    datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.DELETE, NameCategory_Const.MANAGEMENT_ELECTRICITY_ACTIVITIES_MONTH_REPORT, Action_Status.SUCCESS);
                    _asyncLogger.LogInfo(JsonConvert.SerializeObject(datalog));
                    model.status = 1;
                    return Ok(model);
                }
                else
                {
                    model.status = 0;
                    model.error = new ErrorModel()
                    {
                        Code = ErrCode_Const.CANNOT_FIND_DATA_BY_QUERY,
                        Msg = "Không có dữ liệu này trên DB",
                    };
                    datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.DELETE, NameCategory_Const.MANAGEMENT_ELECTRICITY_ACTIVITIES_MONTH_REPORT, Action_Status.FAIL);
                    _asyncLogger.LogInfo(JsonConvert.SerializeObject(datalog));
                    return NotFound(model);
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
    }
}
