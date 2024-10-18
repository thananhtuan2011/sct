using API_SoCongThuong.Classes;
using API_SoCongThuong.Logger;
using API_SoCongThuong.Models;
using API_SoCongThuong.Reponsitories.NewRuralCriteriaRepository;
using EF_Core.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Data;

namespace API_SoCongThuong.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NewRuralCriteriaController : ControllerBase
    {
        public SoHoa_SoCongThuongContext _context;
        private NewRuralCriteriaRepo _repo;
        private IConfiguration _configuration;
        private readonly ILogger<AsyncLogger> _logger;
        private AsyncLogger _asyncLogger;

        public NewRuralCriteriaController(SoHoa_SoCongThuongContext context, IConfiguration configuration, ILogger<AsyncLogger> logger)
        {
            _repo = new NewRuralCriteriaRepo(context);
            _logger = logger;
            _context = context;
            _asyncLogger = new AsyncLogger(_logger, _context);
            _configuration = configuration;
        }

        #region Code cũ
        //[Route("find")]
        //[HttpPost]
        //public IActionResult Find([FromBody] QueryRequestBody query)
        //{
        //    BaseModels<NewRuralCriteriaModel> model = new BaseModels<NewRuralCriteriaModel>();
        //    string _keywordSearch = "";
        //    bool _orderBy_ASC = true;
        //    try
        //    {
        //        UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
        //        if (loginData == null)
        //        {
        //            return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
        //        }

        //        Func<NewRuralCriteriaModel, object> _orderByExpression = x => x.DistrictName;
        //        Dictionary<string, Func<NewRuralCriteriaModel, object>> _sortableFields =
        //            new Dictionary<string, Func<NewRuralCriteriaModel, object>>
        //            {
        //                { "DistrictName", x => x.DistrictName },
        //                { "CommuneName", x => x.CommuneName },
        //                { "Target4", x => x.Target4 },
        //                { "Target7", x => x.Target7 },
        //                { "Target1708", x => x.Target1708 },
        //                { "Target1708Raised", x => x.Target1708Raised },
        //            };

        //        if (query.Sort != null && !string.IsNullOrEmpty(query.Sort.ColumnName) && _sortableFields.ContainsKey(query.Sort.ColumnName))
        //        {
        //            _orderBy_ASC = ("desc".Equals(query.Sort.Direction.ToLower()) ? false : true);
        //            _orderByExpression = _sortableFields[query.Sort.ColumnName];
        //        }

        //        int Year = DateTime.Now.Year;
        //        if (query.Filter != null && query.Filter.ContainsKey("Year") && !string.IsNullOrEmpty(query.Filter["Year"]))
        //        {
        //            Year = int.Parse(query.Filter["Year"]);
        //        }

        //        var allCommune = (from c in _context.Communes
        //                          join d in _context.Districts
        //                             on c.DistrictId equals d.DistrictId
        //                          where !c.IsDel && !d.IsDel
        //                          select new
        //                          {
        //                              d.DistrictId,
        //                              d.DistrictName,
        //                              c.CommuneId,
        //                              c.CommuneName,
        //                          }).ToList();

        //        //Tiêu chí số 4
        //        var allTarget4 = (from c in _context.CommuneElectricityManagements
        //                          join s in _context.Stages
        //                             on c.StageId equals s.StageId
        //                          where !c.IsDel && !s.IsDel
        //                          select new
        //                          {
        //                              c.CommuneId,
        //                              s.StartYear,
        //                              s.EndYear,
        //                              c.Target4Start,
        //                              c.Target4End,
        //                              c.Content41Start,
        //                              c.Content42Start,
        //                              c.Content41End,
        //                              c.Content42End,
        //                          }
        //                         ).ToList();

        //        var passTarget4byYear = (from t in allTarget4
        //                                where t.StartYear <= Year
        //                                group t by t.CommuneId into g
        //                                select new
        //                                {
        //                                    CommuneId = g.Key,
        //                                    PassTarget4 = g.Where(x => x.Target4Start).Any()
        //                                }).ToList();

        //        //Tiêu chí số 7
        //        var passTarget7byYear = (from t in _context.Target7s
        //                                where t.Year == Year
        //                                group t by t.CommuneId into g
        //                                select new
        //                                {
        //                                    CommuneId = g.Key,
        //                                    PassTarget7 = g.Where(x => x.PlanCommercial || x.PlanMarket).Any()
        //                                }).ToList();

        //        //Tiêu chí số 7 nâng cao
        //        var passTarget7RaisebyYear = (from t in _context.Target7s
        //                                 where t.Year <= Year
        //                                 group t by t.CommuneId into g
        //                                 select new
        //                                 {
        //                                     CommuneId = g.Key,
        //                                     PassTarget7Raised = g.Where(x => x.NewRuralCriteriaRaised).Any()
        //                                 }).ToList();

        //        //Tiêu chí số 17.08
        //        var allTarget1708 = (from c in _context.Target1708s
        //                             join s in _context.Stages
        //                                on c.StageId equals s.StageId
        //                             where !c.IsDel && !s.IsDel
        //                             select new
        //                             {
        //                                 c.CommuneId,
        //                                 s.StartYear,
        //                                 s.EndYear,
        //                                 c.NewRuralCriteria,
        //                                 c.NewRuralCriteriaRaised
        //                             }
        //                         ).ToList();

        //        var passTarget1708byYear = (from t in allTarget1708
        //                                   where t.StartYear <= Year && Year <= t.EndYear
        //                                   group t by t.CommuneId into g
        //                                   select new
        //                                   {
        //                                       CommuneId = g.Key,
        //                                       PassTarget1708 = g.Where(x => x.NewRuralCriteria).Any()
        //                                   }).ToList();

        //        //Tiêu chí số 17.08 nâng cao
        //        var passTarget1708RaisebyYear = (from t in allTarget1708
        //                                        where t.StartYear <= Year
        //                                        group t by t.CommuneId into g
        //                                        select new
        //                                        {
        //                                            CommuneId = g.Key,
        //                                            PassTarget1708Raised = g.Where(x => x.NewRuralCriteriaRaised).Any()
        //                                        }).ToList();

        //        IQueryable<NewRuralCriteriaModel> _data = (from c in allCommune
        //                                                   join tg4 in passTarget4byYear
        //                                                     on c.CommuneId equals tg4.CommuneId into JoinT4
        //                                                   from t4 in JoinT4.DefaultIfEmpty()
        //                                                   join tg7 in passTarget7byYear
        //                                                     on c.CommuneId equals tg7.CommuneId into JoinT7
        //                                                   from t7 in JoinT7.DefaultIfEmpty()
        //                                                   join tg7R in passTarget7RaisebyYear
        //                                                     on c.CommuneId equals tg7R.CommuneId into JoinT7Raised
        //                                                   from tg7Raised in JoinT7Raised.DefaultIfEmpty()
        //                                                   join tg1708 in passTarget1708byYear
        //                                                     on c.CommuneId equals tg1708.CommuneId into JoinT1708
        //                                                   from t1708 in JoinT1708.DefaultIfEmpty()
        //                                                   join tg1708R in passTarget1708RaisebyYear
        //                                                     on c.CommuneId equals tg1708R.CommuneId into JoinT1708Raised
        //                                                   from t1708Raised in JoinT1708Raised.DefaultIfEmpty()
        //                                                   select new NewRuralCriteriaModel
        //                                                   {
        //                                                       DistrictId = c.DistrictId,
        //                                                       DistrictName = c.DistrictName,
        //                                                       CommuneId = c.CommuneId,
        //                                                       CommuneName = c.CommuneName,
        //                                                       Target4 = t4?.PassTarget4 ?? false,
        //                                                       Target7 = t7?.PassTarget7 ?? false,
        //                                                       Target7Raised = tg7Raised?.PassTarget7Raised ?? false,
        //                                                       Target1708 = t1708?.PassTarget1708 ?? false,
        //                                                       Target1708Raised = t1708Raised?.PassTarget1708Raised ?? false
        //                                                   }).ToList().AsQueryable();

        //        if (query.SearchValue != null && query.SearchValue != "")
        //        {
        //            _keywordSearch = query.SearchValue.Trim().ToLower();
        //            _data = _data.Where(x =>
        //                x.DistrictName.ToLower().Contains(_keywordSearch)
        //                || x.CommuneName.ToLower().Contains(_keywordSearch)
        //           );
        //        }

        //        if (query.Filter != null && query.Filter.ContainsKey("Commune") && !string.IsNullOrEmpty(query.Filter["Commune"]))
        //        {
        //            _data = _data.Where(x => x.CommuneId.ToString() == query.Filter["Commune"]);
        //        }

        //        if (query.Filter != null && query.Filter.ContainsKey("District") && !string.IsNullOrEmpty(query.Filter["District"]))
        //        {
        //            _data = _data.Where(x => x.DistrictId.ToString() == query.Filter["District"]);
        //        }

        //        int _countRows = _data.Count();
        //        if (_countRows == 0)
        //        {
        //            return NotFound("Không có dữ liệu");
        //        }

        //        if (_orderBy_ASC)
        //        {
        //            model.items = _data
        //                .OrderBy(_orderByExpression)
        //                .Skip((query.Panigator.PageIndex - 1) * query.Panigator.PageSize)
        //                .Take(query.Panigator.PageSize)
        //                .ToList();
        //        }
        //        else
        //        {
        //            model.items = _data
        //                .OrderByDescending(_orderByExpression)
        //                .Skip((query.Panigator.PageIndex - 1) * query.Panigator.PageSize)
        //                .Take(query.Panigator.PageSize)
        //                .ToList();
        //        }

        //        if (query.Panigator.More)
        //        {
        //            model.status = 1;
        //            model.items = _data.ToList();
        //            model.total = _countRows;
        //            return Ok(model);
        //        }

        //        model.status = 1;
        //        model.total = _countRows;
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
        #endregion

        [Route("find")]
        [HttpPost]
        public IActionResult Find([FromBody] QueryRequestBody query)
        {
            BaseModels<NewRuralCriteriaModel> model = new BaseModels<NewRuralCriteriaModel>();
            string _keywordSearch = "";
            bool _orderBy_ASC = true;
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }

                Func<NewRuralCriteriaModel, object> _orderByExpression = x => x.DistrictName;
                Dictionary<string, Func<NewRuralCriteriaModel, object>> _sortableFields =
                    new Dictionary<string, Func<NewRuralCriteriaModel, object>>
                    {
                        { "DistrictName", x => x.DistrictName },
                        { "CommuneName", x => x.CommuneName },
                        { "Title", x => x.TitleName },
                        { "Target4", x => x.Target4 },
                        { "Target7", x => x.Target7 },
                        { "Target1708", x => x.Target1708 },
                    };

                if (query.Sort != null && !string.IsNullOrEmpty(query.Sort.ColumnName) && _sortableFields.ContainsKey(query.Sort.ColumnName))
                {
                    _orderBy_ASC = ("desc".Equals(query.Sort.Direction.ToLower()) ? false : true);
                    _orderByExpression = _sortableFields[query.Sort.ColumnName];
                }

                IQueryable<NewRuralCriteriaModel> _data = (from c in _repo._context.NewRuralCriteria
                                                           where !c.IsDel
                                                           join district in _repo._context.Districts
                                                              on c.DistrictId equals district.DistrictId
                                                           join commune in _repo._context.Communes
                                                              on c.CommuneId equals commune.CommuneId
                                                           select new NewRuralCriteriaModel
                                                           {
                                                               NewRuralCriteriaId = c.NewRuralCriteriaId,
                                                               DistrictId = c.DistrictId,
                                                               DistrictName = district.DistrictName,
                                                               CommuneId = c.CommuneId,
                                                               CommuneName = commune.CommuneName,
                                                               Title = c.Title,
                                                               TitleName = c.Title.ToString() == "2" ? "Nông thôn mới nâng cao" : "Nông thôn mới",
                                                               Target4 = c.Target4,
                                                               Target7 = c.Target7,
                                                               Target1708 = c.Target1708
                                                           }).ToList().AsQueryable();

                if (query.SearchValue != null && query.SearchValue != "")
                {
                    _keywordSearch = query.SearchValue.Trim().ToLower();
                    _data = _data.Where(x =>
                        x.DistrictName.ToLower().Contains(_keywordSearch)
                        || x.CommuneName.ToLower().Contains(_keywordSearch)
                   );
                }

                if (query.Filter != null && query.Filter.ContainsKey("District") && !string.IsNullOrEmpty(query.Filter["District"]))
                {
                    _data = _data.Where(x => x.DistrictId.ToString() == query.Filter["District"]);
                }

                if (query.Filter != null && query.Filter.ContainsKey("Commune") && !string.IsNullOrEmpty(query.Filter["Commune"]))
                {
                    _data = _data.Where(x => x.CommuneId.ToString() == query.Filter["Commune"]);
                }

                if (query.Filter != null && query.Filter.ContainsKey("Title") && !string.IsNullOrEmpty(query.Filter["Title"]))
                {
                    _data = _data.Where(x => x.Title.ToString() == query.Filter["Title"]);
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
        public async Task<IActionResult> Create(NewRuralCriteriaModel data)
        {
            BaseModels<NewRuralCriterion> model = new BaseModels<NewRuralCriterion>();
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }

                var check = _repo._context.NewRuralCriteria.Where(x => !x.IsDel && x.CommuneId == data.CommuneId).Any();
                SystemLog datalog = new SystemLog();
                if (check)
                {
                    model.status = 0;
                    model.error = new ErrorModel()
                    {
                        Code = ErrCode_Const.EXCEPTION_API,
                        Msg = "Báo cáo của huyện này đã tồn tại."
                    };
                    datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.CREATE, NameCategory_Const.NEW_RURAL_CRITERIA, Action_Status.FAIL);
                    _asyncLogger.LogInfo(JsonConvert.SerializeObject(datalog));
                    return BadRequest(model);
                }

                data = new Ulities().TrimModel(data);
                NewRuralCriterion SaveData = new NewRuralCriterion();
                SaveData.DistrictId = data.DistrictId;
                SaveData.CommuneId = data.CommuneId;
                SaveData.Title = int.Parse(data.TitleIdStr);
                SaveData.Target4 = data.Target4;
                SaveData.Target7 = data.Target7;
                SaveData.Target1708 = data.Target1708;
                SaveData.Note = data.Note;

                SaveData.CreateUserId = loginData.Userid;
                SaveData.CreateTime = DateTime.Now;

                await _repo.Insert(SaveData);
                datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.CREATE, NameCategory_Const.NEW_RURAL_CRITERIA, Action_Status.SUCCESS);
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
            BaseModels<NewRuralCriteriaModel> model = new BaseModels<NewRuralCriteriaModel>();
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
        public async Task<IActionResult> Update(NewRuralCriteriaModel data)
        {
            BaseModels<NewRuralCriterion> model = new BaseModels<NewRuralCriterion>();
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                SystemLog datalog = new SystemLog();
                NewRuralCriterion? SaveData = _repo._context.NewRuralCriteria.Where(x => x.NewRuralCriteriaId == data.NewRuralCriteriaId && !x.IsDel).FirstOrDefault();
                if (SaveData != null)
                {
                    var Check = _repo._context.NewRuralCriteria.Where(x => !x.IsDel && x.CommuneId == data.CommuneId && x.NewRuralCriteriaId != data.NewRuralCriteriaId).Any();

                    if (Check)
                    {
                        model.status = 0;
                        model.error = new ErrorModel()
                        {
                            Code = ErrCode_Const.EXCEPTION_API,
                            Msg = "Báo cáo xã này đã tồn tại"
                        };
                        return Ok(model);
                    }

                    data = new Ulities().TrimModel(data);
                    SaveData.NewRuralCriteriaId = data.NewRuralCriteriaId;
                    SaveData.DistrictId = data.DistrictId;
                    SaveData.CommuneId = data.CommuneId;
                    SaveData.Title = int.Parse(data.TitleIdStr);
                    SaveData.Target4 = data.Target4;
                    SaveData.Target7 = data.Target7;
                    SaveData.Target1708 = data.Target1708;
                    SaveData.Note = data.Note;

                    SaveData.UpdateUserId = loginData.Userid;
                    SaveData.UpdateTime = DateTime.Now;

                    await _repo.Update(SaveData);
                    datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.UPDATE, NameCategory_Const.NEW_RURAL_CRITERIA, Action_Status.SUCCESS);
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
                    datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.UPDATE, NameCategory_Const.NEW_RURAL_CRITERIA, Action_Status.FAIL);
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
            BaseModels<NewRuralCriterion> model = new BaseModels<NewRuralCriterion>();
            try
            {
                NewRuralCriterion? DeleteData = _repo._context.NewRuralCriteria.Where(x => x.NewRuralCriteriaId == id && !x.IsDel).FirstOrDefault();
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
                    datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.DELETE, NameCategory_Const.NEW_RURAL_CRITERIA, Action_Status.SUCCESS);
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

        //[Route("ImportData")]
        //[HttpPost()]
        //public async Task<IActionResult> ImportData()
        //{
        //    BaseModels<NewRuralCriterion> model = new BaseModels<NewRuralCriterion>();
        //    try
        //    {
        //        List<NewRuralCriterion> adddata = new List<NewRuralCriterion>();
        //        var AllCommune = _context.Communes.Where(x => !x.IsDel).ToList();
        //        foreach (var Commune in AllCommune)
        //        {
        //            NewRuralCriterion item = new NewRuralCriterion()
        //            {
        //                DistrictId = Commune.DistrictId,
        //                CommuneId = Commune.CommuneId,
        //                Title = 1,
        //                Target4 = false,
        //                Target7 = false,
        //                Target1708 = false,
        //                Note = "",
        //                CreateTime = DateTime.Now,
        //                CreateUserId = Guid.Parse("bca51f82-8300-424e-a72f-3a0810fd8555"),
        //            };
        //            adddata.Add(item);
        //        }

        //        model.status = 1;
        //        await _context.NewRuralCriteria.AddRangeAsync(adddata);
        //        await _context.SaveChangesAsync();
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
    }
}
