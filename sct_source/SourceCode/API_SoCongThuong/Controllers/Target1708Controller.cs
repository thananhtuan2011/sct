using API_SoCongThuong.Classes;
using API_SoCongThuong.Logger;
using API_SoCongThuong.Models;
using API_SoCongThuong.Reponsitories.Target1708Repository;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using EF_Core.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Data;

namespace API_SoCongThuong.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Target1708Controller : ControllerBase
    {
        private Target1708Repo _repo;
        private IConfiguration _configuration;
        private readonly ILogger<AsyncLogger> _logger;
        public SoHoa_SoCongThuongContext _context;
        private AsyncLogger _asyncLogger;

        public Target1708Controller(SoHoa_SoCongThuongContext context, IConfiguration configuration, ILogger<AsyncLogger> logger)
        {
            _repo = new Target1708Repo(context);
            _logger = logger;
            _context = context;
            _asyncLogger = new AsyncLogger(_logger, _context);
            _configuration = configuration;
        }

        [Route("find")]
        [HttpPost]
        public IActionResult Find([FromBody] QueryRequestBody query)
        {
            BaseModels<Target1708Model> model = new BaseModels<Target1708Model>();
            string _keywordSearch = "";
            bool _orderBy_ASC = true;
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }

                Func<Target1708Model, object> _orderByExpression = x => x.DistrictName;
                Dictionary<string, Func<Target1708Model, object>> _sortableFields =
                    new Dictionary<string, Func<Target1708Model, object>>
                    {
                        { "DistrictName", x => x.DistrictName },
                        { "CommuneName", x => x.CommuneName },
                        { "NewRuralCriteria", x => x.NewRuralCriteria },
                        { "NewRuralCriteriaRaised", x => x.NewRuralCriteriaRaised },
                        { "PreviousNewRuralCriteria", x => x.PreviousNewRuralCriteria },
                        { "PreviousNewRuralCriteriaRaised", x => x.PreviousNewRuralCriteriaRaised },
                    };

                if (query.Sort != null && !string.IsNullOrEmpty(query.Sort.ColumnName) && _sortableFields.ContainsKey(query.Sort.ColumnName))
                {
                    _orderBy_ASC = ("desc".Equals(query.Sort.Direction.ToLower()) ? false : true);
                    _orderByExpression = _sortableFields[query.Sort.ColumnName];
                }

                List<Stage> Lstage = _repo._context.Stages
                    .Where(x => !x.IsDel
                        && ((x.StartYear % 10 == 1 && x.EndYear % 10 == 5)
                        || (x.StartYear % 10 == 6 && x.EndYear % 10 == 0)))
                    .Select(x => new Stage
                    {
                        StageId = x.StageId,
                        StageName = x.StageName,
                        StartYear = x.StartYear,
                        EndYear = x.EndYear,
                    })
                    .OrderBy(x => x.StartYear)
                    .ThenBy(x => x.EndYear)
                    .ToList();

                Guid currentStageId = Guid.Empty;
                Guid previousStageId = Guid.Empty;
                if (query.Filter != null && query.Filter.ContainsKey("Stage") && !string.IsNullOrEmpty(query.Filter["Stage"]))
                {
                    Stage currentStage = Lstage.Where(x => x.StageId.ToString() == query.Filter["Stage"]).FirstOrDefault()!;
                    if (currentStage != null)
                    {
                        currentStageId = currentStage.StageId;
                        int currentIndex = Lstage.IndexOf(currentStage);
                        if (currentIndex != -1 && currentIndex > 0)
                        {
                            previousStageId = Lstage[currentIndex - 1].StageId;
                        }
                    }
                    else
                    {
                        model.status = 0;
                        model.error = new ErrorModel()
                        {
                            Code = ErrCode_Const.EXCEPTION_API,
                            Msg = "Không tìm thấy dữ liệu của giai đoạn này",
                        };
                        return BadRequest(model);
                    }
                }
                else
                {
                    Stage currentStage = Lstage.Where(x => x.StartYear <= DateTime.Now.Year && DateTime.Now.Year <= x.EndYear).FirstOrDefault()!;
                    if (currentStage != null)
                    {
                        currentStageId = currentStage.StageId;
                        int currentIndex = Lstage.IndexOf(currentStage);
                        if (currentIndex != -1 && currentIndex > 0)
                        {
                            previousStageId = Lstage[currentIndex - 1].StageId;
                        }
                    }
                    else
                    {
                        model.status = 0;
                        model.error = new ErrorModel()
                        {
                            Code = ErrCode_Const.EXCEPTION_API,
                            Msg = "Không tìm thấy dữ liệu của giai đoạn này",
                        };
                        return BadRequest(model);
                    }
                }

                IQueryable<Target1708Model> _data = (from c in _repo._context.Target1708s
                                                     where !c.IsDel && c.StageId == currentStageId
                                                     join stage in _repo._context.Stages
                                                        on c.StageId equals stage.StageId
                                                     join district in _repo._context.Districts
                                                        on c.DistrictId equals district.DistrictId
                                                     join commune in _repo._context.Communes
                                                        on c.CommuneId equals commune.CommuneId
                                                     join p in _repo._context.Target1708s.Where(x => !x.IsDel && x.StageId == previousStageId)
                                                        on new { c.DistrictId, c.CommuneId } equals new { p.DistrictId, p.CommuneId }
                                                        into JoinPrevious
                                                     from previous in JoinPrevious.DefaultIfEmpty()
                                                     select new Target1708Model
                                                     {
                                                         Target1708Id = c.Target1708Id,
                                                         StageId = c.StageId,
                                                         StageName = stage.StageName,
                                                         DistrictId = c.DistrictId,
                                                         DistrictName = district.DistrictName,
                                                         CommuneId = c.CommuneId,
                                                         CommuneName = commune.CommuneName,
                                                         NewRuralCriteria = c.NewRuralCriteria,
                                                         PreviousNewRuralCriteria = previous.NewRuralCriteria,
                                                         NewRuralCriteriaRaised = c.NewRuralCriteriaRaised,
                                                         PreviousNewRuralCriteriaRaised = previous.NewRuralCriteriaRaised,
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

                if (query.Filter != null && query.Filter.ContainsKey("Commune") && !string.IsNullOrEmpty(query.Filter["Commune"]))
                {
                    _data = _data.Where(x => x.CommuneId.ToString() == query.Filter["Commune"]);
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
        public async Task<IActionResult> Create(Target1708Model data)
        {
            BaseModels<Target1708> model = new BaseModels<Target1708>();
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                var check = _repo._context.Target1708s.Where(x => !x.IsDel && x.StageId == data.StageId && x.CommuneId == data.CommuneId).Any();
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

                data = new Ulities().TrimModel(data);
                Target1708 SaveData = new Target1708();
                SaveData.StageId = data.StageId;
                SaveData.DistrictId = data.DistrictId;
                SaveData.CommuneId = data.CommuneId;
                SaveData.NewRuralCriteria = data.NewRuralCriteria;
                SaveData.NewRuralCriteriaRaised = data.NewRuralCriteriaRaised;
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
        public IActionResult GetItemById(Guid id)
        {
            BaseModels<Target1708> model = new BaseModels<Target1708>();
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
        public async Task<IActionResult> Update(Target1708Model data)
        {
            BaseModels<Target1708> model = new BaseModels<Target1708>();
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                SystemLog datalog = new SystemLog();
                Target1708? SaveData = _repo._context.Target1708s.Where(x => x.Target1708Id == data.Target1708Id && !x.IsDel).FirstOrDefault();
                if (SaveData != null)
                {
                    var Check = _repo._context.Target1708s.Where(x => x.Target1708Id != data.Target1708Id && x.StageId == data.StageId && x.CommuneId == data.CommuneId && !x.IsDel).Any();

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

                    data = new Ulities().TrimModel(data);
                    SaveData.Target1708Id = data.Target1708Id;
                    SaveData.StageId = data.StageId;
                    SaveData.DistrictId = data.DistrictId;
                    SaveData.CommuneId = data.CommuneId;
                    SaveData.NewRuralCriteria = data.NewRuralCriteria;
                    SaveData.NewRuralCriteriaRaised = data.NewRuralCriteriaRaised;
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
        public async Task<IActionResult> Delete(Guid id)
        {
            BaseModels<Target1708> model = new BaseModels<Target1708>();
            try
            {
                Target1708? DeleteData = _repo._context.Target1708s.Where(x => x.Target1708Id == id && !x.IsDel).FirstOrDefault();
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

                IQueryable<Stage> _data = _repo._context.Stages
                    .Where(x => !x.IsDel
                        && ((x.StartYear % 10 == 1 && x.EndYear % 10 == 5)
                        || (x.StartYear % 10 == 6 && x.EndYear % 10 == 0)))
                    .Select(x => new Stage
                    {
                        StageId = x.StageId,
                        StageName = x.StageName,
                        StartYear = x.StartYear,
                        EndYear = x.EndYear,
                    })
                    .OrderBy(x => x.StartYear)
                    .ThenBy(x => x.EndYear);

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

        [Route("ExportExcel")]
        [HttpPost]
        public IActionResult ExportExcel([FromBody] QueryRequestBody query)
        {
            try
            {
                List<Stage> Lstage = _repo._context.Stages
                .Where(x => !x.IsDel
                    && ((x.StartYear % 10 == 1 && x.EndYear % 10 == 5)
                    || (x.StartYear % 10 == 6 && x.EndYear % 10 == 0)))
                .Select(x => new Stage
                {
                    StageId = x.StageId,
                    StageName = x.StageName,
                    StartYear = x.StartYear,
                    EndYear = x.EndYear,
                })
                .OrderBy(x => x.StartYear)
                .ThenBy(x => x.EndYear)
                .ToList();

                // Current
                Guid currentStageId = Guid.Empty;
                string currentStageName = string.Empty;
                // Previous
                Guid previousStageId = Guid.Empty;
                string previousStageName = string.Empty;
                if (query.Filter != null && query.Filter.ContainsKey("Stage") && !string.IsNullOrEmpty(query.Filter["Stage"]))
                {
                    Stage currentStage = Lstage.Where(x => x.StageId.ToString() == query.Filter["Stage"]).FirstOrDefault()!;
                    if (currentStage != null)
                    {
                        currentStageId = currentStage.StageId;
                        currentStageName = currentStage.StageName;
                        int currentIndex = Lstage.IndexOf(currentStage);
                        if (currentIndex != -1 && currentIndex > 0)
                        {
                            previousStageId = Lstage[currentIndex - 1].StageId;
                            previousStageName = Lstage[currentIndex - 1].StageName;
                        }
                    }
                    else
                    {
                        return BadRequest();
                    }
                }
                else
                {
                    Stage currentStage = Lstage.Where(x => x.StartYear <= DateTime.Now.Year && DateTime.Now.Year <= x.EndYear).FirstOrDefault()!;
                    if (currentStage != null)
                    {
                        currentStageId = currentStage.StageId;
                        currentStageName = currentStage.StageName;
                        int currentIndex = Lstage.IndexOf(currentStage);
                        if (currentIndex != -1 && currentIndex > 0)
                        {
                            previousStageId = Lstage[currentIndex - 1].StageId;
                            previousStageName = Lstage[currentIndex - 1].StageName;
                        }
                    }
                    else
                    {
                        return BadRequest();
                    }
                }

                IQueryable<Target1708Model> queryData = (from c in _repo._context.Target1708s
                                                         where !c.IsDel && c.StageId == currentStageId
                                                         join stage in _repo._context.Stages
                                                            on c.StageId equals stage.StageId
                                                         join district in _repo._context.Districts
                                                            on c.DistrictId equals district.DistrictId
                                                         join commune in _repo._context.Communes
                                                            on c.CommuneId equals commune.CommuneId
                                                         join p in _repo._context.Target1708s.Where(x => !x.IsDel && x.StageId == previousStageId)
                                                            on new { c.DistrictId, c.CommuneId } equals new { p.DistrictId, p.CommuneId }
                                                            into JoinPrevious
                                                         from previous in JoinPrevious.DefaultIfEmpty()
                                                         select new Target1708Model
                                                         {
                                                             Target1708Id = c.Target1708Id,
                                                             StageId = c.StageId,
                                                             StageName = stage.StageName,
                                                             DistrictId = c.DistrictId,
                                                             DistrictName = district.DistrictName,
                                                             CommuneId = c.CommuneId,
                                                             CommuneName = commune.CommuneName,
                                                             NewRuralCriteria = c.NewRuralCriteria,
                                                             PreviousNewRuralCriteria = previous.NewRuralCriteria,
                                                             NewRuralCriteriaRaised = c.NewRuralCriteriaRaised,
                                                             PreviousNewRuralCriteriaRaised = previous.NewRuralCriteriaRaised,
                                                         }).ToList().AsQueryable();

                List<Target1708Model> data = queryData.ToList();

                if (!data.Any())
                {
                    return BadRequest();
                }

                using (var workbook = new XLWorkbook(@"Upload/Templates/TieuChi1708.xlsx"))
                {
                    IXLWorksheet worksheet = workbook.Worksheets.Worksheet(1);
                    worksheet.Cell(1, 1).Value = "Bảng theo dõi tiến độ thực hiện chỉ tiêu ATTP ngành Công thương trên địa bàn tỉnh giai đoạn " + currentStageName;
                    worksheet.Cell(5, 4).Value = previousStageName;
                    worksheet.Cell(5, 5).Value = currentStageName;
                    worksheet.Cell(5, 6).Value = previousStageName;
                    worksheet.Cell(5, 7).Value = currentStageName;

                    for (int i = 7; i < 153; i++)
                    {
                        if (i == 13 || i == 33 || i == 54 || i == 74 || i == 85 || i == 98 || i == 114 || i == 136)
                        {
                            continue;
                        }

                        var CommuneData = data.Where(x => worksheet.Cell(i, 3).Value.ToString().ToLower().Contains(x.CommuneName.ToLower())).FirstOrDefault();
                        if (CommuneData != null)
                        {
                            worksheet.Cell(i, 4).Value = CommuneData.PreviousNewRuralCriteria == true ? "Đạt" : "Chưa đạt";
                            worksheet.Cell(i, 5).Value = CommuneData.NewRuralCriteria == true ? "Đạt" : "Chưa đạt";
                            worksheet.Cell(i, 6).Value = CommuneData.PreviousNewRuralCriteriaRaised == true ? "Đạt" : "Chưa đạt";
                            worksheet.Cell(i, 7).Value = CommuneData.NewRuralCriteriaRaised == true ? "Đạt" : "Chưa đạt";
                            worksheet.Cell(i, 8).Value = CommuneData.Note;
                        }
                    }

                    IXLWorksheet worksheet2 = workbook.Worksheets.Worksheet(2);
                    var TotalData = data.GroupBy(x => x.DistrictId).Select(x => new
                    {
                        DistrictName = x.FirstOrDefault()!.DistrictName,
                        NumNewRuralCriteria = x.Count(x => x.NewRuralCriteria),
                        NumNewRuralCriteriaRaised = x.Count(x => x.NewRuralCriteriaRaised)
                    });

                    for (int i = 7; i < 16; i++)
                    {
                        var District = TotalData.Where(x => worksheet2.Cell(i, 3).Value.ToString().ToLower().Contains(x.DistrictName!.ToLower())).FirstOrDefault();
                        if (District != null)
                        {
                            worksheet2.Cell(i, 5).Value = District.NumNewRuralCriteria;
                            worksheet2.Cell(i, 7).Value = District.NumNewRuralCriteriaRaised;
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
                return BadRequest(ex);
            }
        }
    }
}
