using API_SoCongThuong.Classes;
using API_SoCongThuong.Logger;
using API_SoCongThuong.Models;
using API_SoCongThuong.Reponsitories.CommuneElectricityManagementRepository;
using ClosedXML.Excel;
using EF_Core.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.ComponentModel.Design;
using System.Data;
using System.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace API_SoCongThuong.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommuneElectricityManagementController : ControllerBase
    {
        private CommuneElectricityManagementRepo _repo;
        private IConfiguration _configuration;
        private readonly ILogger<AsyncLogger> _logger;
        public SoHoa_SoCongThuongContext _context;
        private AsyncLogger _asyncLogger;

        public CommuneElectricityManagementController(SoHoa_SoCongThuongContext context, IConfiguration configuration, ILogger<AsyncLogger> logger)
        {
            _repo = new CommuneElectricityManagementRepo(context);
            _logger = logger;
            _context = context;
            _asyncLogger = new AsyncLogger(_logger, _context);
            _configuration = configuration;
        }

        [Route("find")]
        [HttpPost]
        public IActionResult Find([FromBody] QueryRequestBody query)
        {
            BaseModels<CommuneElectricityManagementModel> model = new BaseModels<CommuneElectricityManagementModel>();
            string _keywordSearch = "";
            bool _orderBy_ASC = true;
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }

                Func<CommuneElectricityManagementModel, object> _orderByExpression = x => x.DistrictName;
                Dictionary<string, Func<CommuneElectricityManagementModel, object>> _sortableFields =
                    new Dictionary<string, Func<CommuneElectricityManagementModel, object>>
                    {
                        { "DistrictName", x => x.DistrictName },
                        { "CommuneName", x => x.CommuneName },
                        { "Content41Start", x => x.Content41Start },
                        { "Content42Start", x => x.Content42Start },
                        { "Target4Start", x => x.Target4Start },
                        { "Content41End", x => x.Content41End },
                        { "Content42End", x => x.Content42End },
                        { "Target4End", x => x.Target4End },
                    };
                if (query.Sort != null && !string.IsNullOrEmpty(query.Sort.ColumnName) && _sortableFields.ContainsKey(query.Sort.ColumnName))
                {
                    _orderBy_ASC = ("desc".Equals(query.Sort.Direction.ToLower()) ? false : true);
                    _orderByExpression = _sortableFields[query.Sort.ColumnName];
                }

                IQueryable<CommuneElectricityManagementModel> _data = (from c in _repo._context.CommuneElectricityManagements
                                                                       where !c.IsDel
                                                                       join stage in _repo._context.Stages
                                                                       on c.StageId equals stage.StageId
                                                                       join district in _repo._context.Districts
                                                                       on c.DistrictId equals district.DistrictId
                                                                       join commune in _repo._context.Communes
                                                                       on c.CommuneId equals commune.CommuneId
                                                                       select new CommuneElectricityManagementModel
                                                                       {
                                                                           CommuneElectricityManagementId = c.CommuneElectricityManagementId,
                                                                           StageId = c.StageId,
                                                                           StageName = stage.StageName,
                                                                           DistrictId = c.DistrictId,
                                                                           DistrictName = district.DistrictName,
                                                                           CommuneId = c.CommuneId,
                                                                           CommuneName = commune.CommuneName,
                                                                           Content41Start = c.Content41Start,
                                                                           Content42Start = c.Content42Start,
                                                                           Target4Start = c.Target4Start,
                                                                           Content41End = c.Content41End,
                                                                           Content42End = c.Content42End,
                                                                           Target4End = c.Target4End,
                                                                       }).ToList().AsQueryable();

                if (query.SearchValue != null && query.SearchValue != "")
                {
                    _keywordSearch = query.SearchValue.Trim().ToLower();
                    _data = _data.Where(x =>
                        x.DistrictName.ToLower().Contains(_keywordSearch)
                        || x.CommuneName.ToLower().Contains(_keywordSearch)
                   );
                }

                if (query.Filter != null && query.Filter.ContainsKey("Stage") && !string.IsNullOrEmpty(query.Filter["Stage"]))
                {
                    _data = _data.Where(x => x.StageId.ToString() == query.Filter["Stage"]);
                }

                if (query.Filter != null && query.Filter.ContainsKey("District") && !string.IsNullOrEmpty(query.Filter["District"]))
                {
                    _data = _data.Where(x => x.DistrictId.ToString() == query.Filter["District"]);
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

        [HttpPost()]
        public async Task<IActionResult> create(CommuneElectricityManagement data)
        {
            BaseModels<CommuneElectricityManagement> model = new BaseModels<CommuneElectricityManagement>();
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                var check = _repo._context.CommuneElectricityManagements.Where(x => !x.IsDel && x.StageId == data.StageId && x.CommuneId == data.CommuneId).Any();
                SystemLog datalog = new SystemLog();
                if (check)
                {
                    model.status = 0;
                    model.error = new ErrorModel()
                    {
                        Code = ErrCode_Const.EXCEPTION_API,
                        Msg = "Báo cáo giai đoạn này đã tồn tại"
                    }; 
                    datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.CREATE, NameCategory_Const.COMMUNE_ELECTRICITY_MANAGEMENT, Action_Status.FAIL);
                    _asyncLogger.LogInfo(JsonConvert.SerializeObject(datalog));
                    return BadRequest(model);
                }

                CommuneElectricityManagement SaveData = new CommuneElectricityManagement();

                SaveData.StageId = data.StageId;
                SaveData.DistrictId = data.DistrictId;
                SaveData.CommuneId = data.CommuneId;
                SaveData.Content41Start = data.Content41Start;
                SaveData.Content42Start = data.Content42Start;
                SaveData.Target4Start = data.Target4Start;
                SaveData.Content41End = data.Content41End;
                SaveData.Content42End = data.Content42End;
                SaveData.Target4End = data.Target4End;
                SaveData.Note = data.Note;

                SaveData.CreateUserId = loginData.Userid;
                SaveData.CreateTime = DateTime.Now;

                await _repo.Insert(SaveData);
                datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.CREATE, NameCategory_Const.COMMUNE_ELECTRICITY_MANAGEMENT, Action_Status.SUCCESS);
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
            BaseModels<CommuneElectricityManagement> model = new BaseModels<CommuneElectricityManagement>();
            try
            {
                var result = _repo.FindById(id);
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
        public async Task<IActionResult> Update(CommuneElectricityManagement data)
        {
            BaseModels<CommuneElectricityManagement> model = new BaseModels<CommuneElectricityManagement>();
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                SystemLog datalog = new SystemLog();
                CommuneElectricityManagement? SaveData = _repo._context.CommuneElectricityManagements.Where(x => x.CommuneElectricityManagementId == data.CommuneElectricityManagementId && !x.IsDel).FirstOrDefault();
                if (SaveData != null)
                {
                    var Check = _repo._context.CommuneElectricityManagements.Where(x => x.CommuneElectricityManagementId != data.CommuneElectricityManagementId && x.StageId == data.StageId && x.CommuneId == data.CommuneId && !x.IsDel).Any();

                    if (Check)
                    {
                        model.status = 0;
                        model.error = new ErrorModel()
                        {
                            Code = ErrCode_Const.EXCEPTION_API,
                            Msg = "Báo cáo giai đoạn này đã tồn tại"
                        };
                        return Ok(model);
                    }

                    SaveData.CommuneElectricityManagementId = data.CommuneElectricityManagementId;
                    SaveData.StageId = data.StageId;
                    SaveData.DistrictId = data.DistrictId;
                    SaveData.CommuneId = data.CommuneId;
                    SaveData.Content41Start = data.Content41Start;
                    SaveData.Content42Start = data.Content42Start;
                    SaveData.Target4Start = data.Target4Start;
                    SaveData.Content41End = data.Content41End;
                    SaveData.Content42End = data.Content42End;
                    SaveData.Target4End = data.Target4End;
                    SaveData.Note = data.Note;

                    SaveData.UpdateUserId = loginData.Userid;
                    SaveData.UpdateTime = DateTime.Now;

                    await _repo.Update(SaveData);
                    datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.UPDATE, NameCategory_Const.COMMUNE_ELECTRICITY_MANAGEMENT, Action_Status.SUCCESS);
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
                    datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.UPDATE, NameCategory_Const.COMMUNE_ELECTRICITY_MANAGEMENT, Action_Status.FAIL);
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
        public async Task<IActionResult> delete(Guid id)
        {
            BaseModels<CommuneElectricityManagement> model = new BaseModels<CommuneElectricityManagement>();
            try
            {
                CommuneElectricityManagement? DeleteData = _repo._context.CommuneElectricityManagements.Where(x => x.CommuneElectricityManagementId == id && !x.IsDel).FirstOrDefault();
                if (DeleteData != null)
                {
                    UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                    if (loginData == null)
                    {
                        return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                    }
                    SystemLog datalog = new SystemLog();
                    DeleteData.IsDel = true;
                    await _repo.Delete(DeleteData);
                    datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.DELETE, NameCategory_Const.COMMUNE_ELECTRICITY_MANAGEMENT, Action_Status.SUCCESS);
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

        [Route("LoadStage")]
        [HttpGet]
        public IActionResult LoadStage()
        {
            BaseModels<Stage> model = new BaseModels<Stage>();
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }

                IQueryable<Stage> _data = _repo._context.Stages.Where(x => !x.IsDel).Select(x => new Stage
                {
                    StageId = x.StageId,
                    StageName = x.StageName,
                    StartYear = x.StartYear,
                    EndYear = x.EndYear,
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

        [Route("LoadDistrict")]
        [HttpGet]
        public IActionResult LoadDistrict()
        {
            BaseModels<District> model = new BaseModels<District>();
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }

                IQueryable<District> _data = _repo._context.Districts.Where(x => !x.IsDel).Select(x => new District
                {
                    DistrictId = x.DistrictId,
                    DistrictName = x.DistrictName,
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

        [Route("LoadCommune")]
        [HttpGet]
        public IActionResult LoadCommune()
        {
            BaseModels<Commune> model = new BaseModels<Commune>();
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }

                IQueryable<Commune> _data = _repo._context.Communes.Where(x => !x.IsDel).Select(x => new Commune
                {
                    CommuneId = x.CommuneId,
                    CommuneName = x.CommuneName,
                    DistrictId = x.DistrictId,
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

        [HttpPost("ExportExcel/{id}")]
        public IActionResult ExportExcel(Guid id)
        {
            try
            {
                IQueryable<CommuneElectricityManagementModel> query = (from c in _repo._context.CommuneElectricityManagements
                                                                       where !c.IsDel
                                                                       join stage in _repo._context.Stages
                                                                       on c.StageId equals stage.StageId
                                                                       join district in _repo._context.Districts
                                                                       on c.DistrictId equals district.DistrictId
                                                                       join commune in _repo._context.Communes
                                                                       on c.CommuneId equals commune.CommuneId
                                                                       select new CommuneElectricityManagementModel
                                                                       {
                                                                           CommuneElectricityManagementId = c.CommuneElectricityManagementId,
                                                                           StageId = c.StageId,
                                                                           StageName = stage.StageName,
                                                                           DistrictId = c.DistrictId,
                                                                           DistrictName = district.DistrictName,
                                                                           CommuneId = c.CommuneId,
                                                                           CommuneName = commune.CommuneName,
                                                                           Content41Start = c.Content41Start,
                                                                           Content42Start = c.Content42Start,
                                                                           Target4Start = c.Target4Start,
                                                                           Content41End = c.Content41End,
                                                                           Content42End = c.Content42End,
                                                                           Target4End = c.Target4End,
                                                                       }).ToList().AsQueryable();

                query = query.Where(x => x.StageId == id);

                List<CommuneElectricityManagementModel> data = query.ToList();

                if (!data.Any())
                {
                    return BadRequest();
                }

                var StageItem = _repo._context.Stages.Where(x => x.StageId == id && !x.IsDel).FirstOrDefault();

                using (var workbook = new XLWorkbook(@"Upload/Templates/Baocaothuchientieuchidiennongthon.xlsx"))
                {
                    IXLWorksheet worksheet = workbook.Worksheets.Worksheet(1);
                    worksheet.Cell(3, 3).Value = "Lũy kế đến hết năm " + StageItem?.StartYear.ToString();
                    worksheet.Cell(3, 6).Value = "Dự kiến đến hết năm " + StageItem?.EndYear.ToString();
                    worksheet.Cell(157, 1).Value = "Lũy kế đến hết năm " + StageItem?.StartYear.ToString() + ":";

                    for (int i = 6; i < 157; i ++)
                    {
                        if ( i == 25 || i == 36 ||  i == 59 ||  i == 67 || i ==88 || i == 104 || i == 118 || i == 136)
                        {
                            continue;
                        }

                        var CommuneData = data.Where(x => x.CommuneName.ToLower().Contains(worksheet.Cell(i, 2).Value.ToString().ToLower())).FirstOrDefault();
                        if ( CommuneData != null )
                        {
                            worksheet.Cell(i, 3).Value = CommuneData.Content41Start == true ? "x" : "";
                            worksheet.Cell(i, 4).Value = CommuneData.Content42Start == true ? "x" : "";
                            worksheet.Cell(i, 5).Value = CommuneData.Target4Start == true ? "x" : "";
                            worksheet.Cell(i, 6).Value = CommuneData.Content41End == true ? "x" : "";
                            worksheet.Cell(i, 7).Value = CommuneData.Content42End == true ? "x" : "";
                            worksheet.Cell(i, 8).Value = CommuneData.Target4End == true ? "x" : "";
                            worksheet.Cell(i, 9).Value = CommuneData.Note;
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
