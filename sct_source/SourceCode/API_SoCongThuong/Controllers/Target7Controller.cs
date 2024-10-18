using API_SoCongThuong.Classes;
using API_SoCongThuong.Logger;
using API_SoCongThuong.Models;
using API_SoCongThuong.Reponsitories.Target7Repository;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using EF_Core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Data;

namespace API_SoCongThuong.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Target7Controller : ControllerBase
    {
        private Target7Repo _repo;
        private IConfiguration _configuration;
        private readonly ILogger<AsyncLogger> _logger;
        public SoHoa_SoCongThuongContext _context;
        private AsyncLogger _asyncLogger;

        public Target7Controller(SoHoa_SoCongThuongContext context, IConfiguration configuration, ILogger<AsyncLogger> logger)
        {
            _repo = new Target7Repo(context);
            _logger = logger;
            _context = context;
            _asyncLogger = new AsyncLogger(_logger, _context);
            _configuration = configuration;
        }

        [Route("find")]
        [HttpPost]
        public IActionResult Find([FromBody] QueryRequestBody query)
        {
            BaseModels<Target7Model> model = new BaseModels<Target7Model>();
            string _keywordSearch = "";
            bool _orderBy_ASC = true;
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }

                Func<Target7Model, object> _orderByExpression = x => x.DistrictName;
                Dictionary<string, Func<Target7Model, object>> _sortableFields =
                    new Dictionary<string, Func<Target7Model, object>>
                    {
                        { "DistrictName", x => x.DistrictName },
                        { "CommuneName", x => x.CommuneName },
                        { "PlanCommercial", x => x.PlanCommercial },
                        { "PlanMarket", x => x.PlanMarket },
                        { "EstimateCommercial", x => x.EstimateCommercial },
                        { "EstimateMarket", x => x.EstimateMarket },
                    };

                if (query.Sort != null && !string.IsNullOrEmpty(query.Sort.ColumnName) && _sortableFields.ContainsKey(query.Sort.ColumnName))
                {
                    _orderBy_ASC = ("desc".Equals(query.Sort.Direction.ToLower()) ? false : true);
                    _orderByExpression = _sortableFields[query.Sort.ColumnName];
                }

                IQueryable<Target7Model> _data = (from t in _repo._context.Target7s
                                                  where !t.IsDel
                                                  join s in _repo._context.Categories
                                                     on t.StageId equals s.CategoryId
                                                    into JoinStage
                                                  from stage in JoinStage.DefaultIfEmpty()
                                                  join district in _repo._context.Districts
                                                     on t.DistrictId equals district.DistrictId
                                                  join commune in _repo._context.Communes
                                                     on t.CommuneId equals commune.CommuneId
                                                  select new Target7Model
                                                  {
                                                      Target7Id = t.Target7Id,
                                                      StageId = t.StageId ?? Guid.Empty,
                                                      StageName = stage.CategoryName,
                                                      Year = t.Year,
                                                      DistrictId = t.DistrictId,
                                                      DistrictName = district.DistrictName,
                                                      CommuneId = t.CommuneId,
                                                      CommuneName = commune.CommuneName,
                                                      PlanCommercial = t.PlanCommercial,
                                                      PlanMarket = t.PlanMarket,
                                                      EstimateCommercial = t.EstimateCommercial,
                                                      EstimateMarket = t.EstimateMarket,
                                                      NewRuralCriteriaRaised = t.NewRuralCriteriaRaised,
                                                  })
                                                  .ToList()
                                                  .AsQueryable();

                if (query.SearchValue != null && query.SearchValue != "")
                {
                    _keywordSearch = query.SearchValue.Trim().ToLower();
                    _data = _data.Where(x =>
                        x.DistrictName.ToLower().Contains(_keywordSearch)
                        || x.CommuneName.ToLower().Contains(_keywordSearch)
                   );
                }

                if (query.Filter != null && query.Filter.ContainsKey("Year") && !string.IsNullOrEmpty(query.Filter["Year"]))
                {
                    _data = _data.Where(x => x.Year.ToString() == query.Filter["Year"]);
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
        public async Task<IActionResult> Create(Target7Model data)
        {
            BaseModels<Target7> model = new BaseModels<Target7>();
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                var check = _repo._context.Target7s.Where(x => !x.IsDel && x.Year == data.Year && x.StageId == data.StageId && x.CommuneId == data.CommuneId).Any();
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
                Target7 SaveData = new Target7();
                SaveData.StageId = data.StageId;
                SaveData.Year = data.Year;
                SaveData.DistrictId = data.DistrictId;
                SaveData.CommuneId = data.CommuneId;
                SaveData.MarketInPlaning = data.MarketInPlaning;
                SaveData.PlanCommercial = data.PlanCommercial;
                SaveData.PlanMarket = data.PlanMarket;
                SaveData.EstimateCommercial = data.EstimateCommercial;
                SaveData.EstimateMarket = data.EstimateMarket;
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
            BaseModels<Target7> model = new BaseModels<Target7>();
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
        public async Task<IActionResult> Update(Target7Model data)
        {
            BaseModels<Target7> model = new BaseModels<Target7>();
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                SystemLog datalog = new SystemLog();
                Target7? SaveData = _repo._context.Target7s.Where(x => x.Target7Id == data.Target7Id && !x.IsDel).FirstOrDefault();
                if (SaveData != null)
                {
                    var Check = _repo._context.Target7s.Where(x => x.Target7Id != data.Target7Id && x.Year == data.Year && x.StageId == data.StageId && x.CommuneId == data.CommuneId && !x.IsDel).Any();

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
                    SaveData.Target7Id = data.Target7Id;
                    SaveData.StageId = data.StageId;
                    SaveData.Year = data.Year;
                    SaveData.DistrictId = data.DistrictId;
                    SaveData.CommuneId = data.CommuneId;
                    SaveData.MarketInPlaning = data.MarketInPlaning;
                    SaveData.PlanCommercial = data.PlanCommercial;
                    SaveData.PlanMarket = data.PlanMarket;
                    SaveData.EstimateCommercial = data.EstimateCommercial;
                    SaveData.EstimateMarket = data.EstimateMarket;
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
            BaseModels<Target7> model = new BaseModels<Target7>();
            try
            {
                Target7? DeleteData = _repo._context.Target7s.Where(x => x.Target7Id == id && !x.IsDel).FirstOrDefault();
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

        //[Route("LoadStage")]
        //[HttpGet]
        //public IActionResult LoadStage()
        //{
        //    BaseModels<Stage> model = new BaseModels<Stage>();
        //    try
        //    {
        //        UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
        //        if (loginData == null)
        //        {
        //            return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
        //        }

        //        IQueryable<Stage> _data = _repo._context.Stages
        //            .Where(x => !x.IsDel
        //                && ((x.StartYear % 10 == 1 && x.EndYear % 10 == 5)
        //                || (x.StartYear % 10 == 6 && x.EndYear % 10 == 0)))
        //            .Select(x => new Stage
        //            {
        //                StageId = x.StageId,
        //                StageName = x.StageName,
        //                StartYear = x.StartYear,
        //                EndYear = x.EndYear,
        //            })
        //            .OrderBy(x => x.StartYear)
        //            .ThenBy(x => x.EndYear);

        //        model.status = 1;
        //        model.items = _data.ToList();
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

        [Route("ExportExcel")]
        [HttpPost]
        public IActionResult ExportExcel([FromBody] QueryRequestBody query)
        {
            try
            {
                Guid StageId = Guid.Empty;
                string StageName = "";
                if (query.Filter != null && query.Filter.ContainsKey("Stage") && !string.IsNullOrEmpty(query.Filter["Stage"]))
                {
                    StageId = Guid.Parse(query.Filter["Stage"]);
                    StageName = _repo._context.Categories.Where(x => x.CategoryId == StageId).Select(x => x.CategoryName).FirstOrDefault() ?? "";
                }

                int Year = 0;
                int PreviousYear = 0;
                if (query.Filter != null && query.Filter.ContainsKey("Year") && !string.IsNullOrEmpty(query.Filter["Year"]))
                {
                    Year = int.Parse(query.Filter["Year"]);
                    PreviousYear = Year - 1;
                }

                if (StageId == Guid.Empty || Year == 0)
                {
                    return BadRequest();
                }

                var data = (from c in _repo._context.Target7s
                            where !c.IsDel && c.Year == Year && c.StageId == StageId
                            join s in _repo._context.Categories
                              on c.StageId equals s.CategoryId into JoinStage
                            from stage in JoinStage.DefaultIfEmpty()
                            join district in _repo._context.Districts
                              on c.DistrictId equals district.DistrictId
                            join commune in _repo._context.Communes
                              on c.CommuneId equals commune.CommuneId
                            join previous in _repo._context.Target7s.Where(x => x.Year == PreviousYear && x.StageId == StageId)
                              on c.CommuneId equals previous.CommuneId into JoinPrevious
                            from p in JoinPrevious.DefaultIfEmpty()
                            select new
                            {
                                Target7Id = c.Target7Id,
                                StageId = c.StageId ?? Guid.Empty,
                                StageName = stage.CategoryName,
                                DistrictId = c.DistrictId,
                                DistrictName = district.DistrictName,
                                CommuneId = c.CommuneId,
                                CommuneName = commune.CommuneName,
                                PlanCommercial = c.PlanCommercial,
                                PlanMarket = c.PlanMarket,
                                EstimateCommercial = c.EstimateCommercial,
                                EstimateMarket = c.EstimateMarket,
                                PreviousCommercial = p.EstimateCommercial ? true : false,
                                PreviousMarket = p.EstimateMarket ? true : false,
                            }
                            ).ToList();

                if (!data.Any())
                {
                    return BadRequest();
                }

                var group = from i in data
                            group i by i.DistrictId into g
                            select new
                            {
                                DistrictId = g.Key,
                                DistrictName = g.Select(x => x.DistrictName).FirstOrDefault(),
                                NumPreviousCommercial = g.Count(x => x.PreviousCommercial),
                                NumPreviousMarket = g.Count(x => x.PreviousMarket),
                                NumPlanCommercial = g.Count(x => x.PreviousMarket),
                                NumPlanMarket = g.Count(x => x.PlanMarket),
                                NumEstimateCommercial = g.Count(x => x.EstimateCommercial),
                                NumEstimateMarket = g.Count(x => x.EstimateMarket),
                                NumCommunePassTarget = _repo._context.Target7s
                                                            .Where(x => x.DistrictId == g.Key)
                                                            .GroupBy(x => x.CommuneId)
                                                            .Select(x => new
                                                            {
                                                                CommuneId = x.Key,
                                                                PassTarget = x.Where(item => item.EstimateCommercial && item.EstimateMarket).Any(),
                                                            })
                                                            .Count(x => x.PassTarget),
                                NumCommuneHaveMarketPlanning = _repo._context.Target7s
                                                            .Where(x => x.DistrictId == g.Key)
                                                            .GroupBy(x => x.CommuneId)
                                                            .Select(x => new
                                                            {
                                                                CommuneId = x.Key,
                                                                PassTarget = x.Where(item => item.MarketInPlaning > 0).Any(),
                                                            })
                                                            .Count(x => x.PassTarget)
                            };

                using (var workbook = new XLWorkbook(@"Upload/Templates/Tieuchiso7.xlsx"))
                {
                    IXLWorksheet worksheet = workbook.Worksheets.Worksheet(1);
                    worksheet.Cell(7, 1).Value = StageName + " " + Year;
                    for (var i = 14; i < 23; i++)
                    {
                        string DistrictName = worksheet.Cell(i, 2).Value.ToString().ToLower();
                        var item = group.Where(x => DistrictName.Contains(x.DistrictName.ToLower())).FirstOrDefault();
                        if (item != null)
                        {
                            worksheet.Cell(i, 4).Value = item.NumCommunePassTarget;
                            worksheet.Cell(i, 5).Value = item.NumCommuneHaveMarketPlanning;
                            worksheet.Cell(i, 6).Value = item.NumPreviousCommercial;
                            worksheet.Cell(i, 7).Value = item.NumPreviousMarket;
                            worksheet.Cell(i, 8).Value = item.NumPlanCommercial;
                            worksheet.Cell(i, 9).Value = item.NumPlanMarket;
                            worksheet.Cell(i, 10).Value = item.NumEstimateCommercial;
                            worksheet.Cell(i, 11).Value = item.NumEstimateMarket;
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

        [Route("GetDataProvice")]
        [HttpPost]
        public IActionResult GetDataProvice([FromBody] QueryRequestBody query)
        {
            BaseModels<Target7ProviceModel> model = new BaseModels<Target7ProviceModel>();
            string _keywordSearch = "";
            bool _orderBy_ASC = true;
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                Guid StageId = Guid.Empty;
                string StageName = "";
                if (query.Filter != null && query.Filter.ContainsKey("Stage") && !string.IsNullOrEmpty(query.Filter["Stage"]))
                {
                    StageId = Guid.Parse(query.Filter["Stage"]);
                    StageName = _repo._context.Categories.Where(x => x.CategoryId == StageId).Select(x => x.CategoryName).FirstOrDefault() ?? "";
                }

                int Year = 0;
                int PreviousYear = 0;
                if (query.Filter != null && query.Filter.ContainsKey("Year") && !string.IsNullOrEmpty(query.Filter["Year"]))
                {
                    Year = int.Parse(query.Filter["Year"]);
                    PreviousYear = Year - 1;
                }

                if (StageId == Guid.Empty || Year == 0)
                {
                    return BadRequest();
                }

                var data = (from c in _repo._context.Target7s
                            where !c.IsDel && c.Year == Year && c.StageId == StageId
                            join s in _repo._context.Categories
                              on c.StageId equals s.CategoryId into JoinStage
                            from stage in JoinStage.DefaultIfEmpty()
                            join district in _repo._context.Districts
                              on c.DistrictId equals district.DistrictId
                            join commune in _repo._context.Communes
                              on c.CommuneId equals commune.CommuneId
                            join previous in _repo._context.Target7s.Where(x => x.Year == PreviousYear && x.StageId == StageId)
                              on c.CommuneId equals previous.CommuneId into JoinPrevious
                            from p in JoinPrevious.DefaultIfEmpty()
                            select new
                            {
                                Target7Id = c.Target7Id,
                                StageId = c.StageId ?? Guid.Empty,
                                StageName = stage.CategoryName,
                                DistrictId = c.DistrictId,
                                DistrictName = district.DistrictName,
                                CommuneId = c.CommuneId,
                                CommuneName = commune.CommuneName,
                                PlanCommercial = c.PlanCommercial,
                                PlanMarket = c.PlanMarket,
                                EstimateCommercial = c.EstimateCommercial,
                                EstimateMarket = c.EstimateMarket,
                                PreviousCommercial = p.EstimateCommercial ? true : false,
                                PreviousMarket = p.EstimateMarket ? true : false,
                            }
                            ).ToList();

                var group = (from i in data
                            group i by i.DistrictId into g
                            select new
                            {
                                DistrictId = g.Key,
                                DistrictName = g.Select(x => x.DistrictName).FirstOrDefault(),
                                NumPreviousCommercial = g.Count(x => x.PreviousCommercial),
                                NumPreviousMarket = g.Count(x => x.PreviousMarket),
                                NumPlanCommercial = g.Count(x => x.PreviousMarket),
                                NumPlanMarket = g.Count(x => x.PlanMarket),
                                NumEstimateCommercial = g.Count(x => x.EstimateCommercial),
                                NumEstimateMarket = g.Count(x => x.EstimateMarket),
                                NumCommunePassTarget = _repo._context.Target7s
                                                            .Where(x => x.DistrictId == g.Key)
                                                            .GroupBy(x => x.CommuneId)
                                                            .Select(x => new
                                                            {
                                                                CommuneId = x.Key,
                                                                PassTarget = x.Where(item => item.EstimateCommercial && item.EstimateMarket).Any(),
                                                            })
                                                            .Count(x => x.PassTarget),
                                NumCommuneHaveMarketPlanning = _repo._context.Target7s
                                                            .Where(x => x.DistrictId == g.Key)
                                                            .GroupBy(x => x.CommuneId)
                                                            .Select(x => new
                                                            {
                                                                CommuneId = x.Key,
                                                                PassTarget = x.Where(item => item.MarketInPlaning > 0).Any(),
                                                            })
                                                            .Count(x => x.PassTarget)
                            }).ToList().AsQueryable();
                var listDistrict = from d in _repo._context.Districts
                                   where !d.IsDel
                                   join commune in _repo._context.Communes on d.DistrictId equals commune.DistrictId
                                   group d by d.DistrictId into g
                                   select new
                                   {
                                       DistrictId = g.Key,
                                       DistrictName = g.Select(x => x.DistrictName).FirstOrDefault(),
                                       CountCommune = g.Count()
                                   };
                List<Target7ProviceModel> result = new List<Target7ProviceModel>();
                foreach (var itemDistrict in listDistrict)
                {
                    var itemResult = new Target7ProviceModel();
                    itemResult.DistrictId = itemDistrict.DistrictId;
                    itemResult.DistrictName = itemDistrict.DistrictName;
                    itemResult.NumberCommune = itemDistrict.CountCommune;
                    var item = group.Where(x => x.DistrictId == itemDistrict.DistrictId).FirstOrDefault();
                    if (item != null)
                    {
                        itemResult.NumCommunePassTarget = item.NumCommunePassTarget;
                        itemResult.NumCommuneHaveMarketPlanning = item.NumCommuneHaveMarketPlanning;
                        itemResult.NumPreviousCommercial = item.NumPreviousCommercial;
                        itemResult.NumPreviousMarket = item.NumPreviousMarket;
                        itemResult.NumPlanCommercial = item.NumPlanCommercial;
                        itemResult.NumPlanMarket = item.NumPlanMarket;
                        itemResult.NumEstimateCommercial = item.NumEstimateCommercial;
                        itemResult.NumEstimateMarket = item.NumEstimateMarket;
                    }
                    result.Add(itemResult);
                    
                }
                model.status = 1;
                model.items = result;
                model.total = result.Count();
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
