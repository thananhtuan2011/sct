
using API_SoCongThuong.Classes;
using API_SoCongThuong.Logger;
using API_SoCongThuong.Models;
using API_SoCongThuong.Reponsitories.StateUnitsRepository;
using EF_Core.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.ComponentModel.Design;
using static API_SoCongThuong.Classes.ErrMsg_Const;

namespace API_SoCongThuong.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StateUnitController : ControllerBase
    {
        private StateUnitRepo _repoStateUnit;
        private IConfiguration _configuration;
        private readonly ILogger<AsyncLogger> _logger;
        public SoHoa_SoCongThuongContext _context;
        private AsyncLogger _asyncLogger;
        public StateUnitController(SoHoa_SoCongThuongContext context, IConfiguration configuration, ILogger<AsyncLogger> logger)
        {
            _repoStateUnit = new StateUnitRepo(context);
            _logger = logger;
            _context = context;
            _asyncLogger = new AsyncLogger(_logger, _context);
            _configuration = configuration;
        }

        [Route("find")]
        [HttpPost]
        public IActionResult ListItems_New([FromBody] QueryRequestBody query)
        {
            BaseModels<StateUnitModel> model = new BaseModels<StateUnitModel>();
            string _keywordSearch = "";
            bool _orderBy_ASC = true;
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }

                Func<StateUnitModel, object> _orderByExpression = x => x.StateUnitsCode;
                Dictionary<string, Func<StateUnitModel, object>> _sortableFields = new Dictionary<string, Func<StateUnitModel, object>>
                {
                    { "StateUnitsCode", x => x.StateUnitsCode },
                    { "StateUnitsName", x => x.StateUnitsName }
                };
                if (query.Sort != null && !string.IsNullOrEmpty(query.Sort.ColumnName) && _sortableFields.ContainsKey(query.Sort.ColumnName))
                {
                    _orderBy_ASC = ("desc".Equals(query.Sort.Direction.ToLower()) ? false : true);
                    _orderByExpression = _sortableFields[query.Sort.ColumnName];
                }

                IQueryable<StateUnitModel> _data = _repoStateUnit._context.StateUnits
                    .Where(x => !x.IsDel)
                    .Select(x => new StateUnitModel
                    {
                        StateUnitsId = x.StateUnitsId,
                        StateUnitsCode = x.StateUnitsCode ?? "",
                        StateUnitsName = x.StateUnitsName ?? "",
                        ParentName = _repoStateUnit._context.StateUnits.FirstOrDefault(i => i.StateUnitsId == x.ParentId && !i.IsDel).StateUnitsName ?? "",
                        IsDel = x.IsDel
                    });

                if (query.SearchValue != null && query.SearchValue != "")
                {
                    _keywordSearch = query.SearchValue.Trim().ToLower();
                    _data = _data.Where(x => x.StateUnitsName.ToLower().Contains(_keywordSearch) || x.StateUnitsCode.ToLower().Contains(_keywordSearch));
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

        [HttpGet("{id}")]
        public IActionResult getItemById(Guid id)
        {
            BaseModels<StateUnit> model = new BaseModels<StateUnit>();
            try
            {
                var result = _repoStateUnit.FindById(id);
                if (result == null)
                {
                    model.status = 0;
                    model.error = new ErrorModel()
                    {
                        Code = ErrCode_Const.CANNOT_FIND_DATA_BY_QUERY,
                        Msg = "Không có dữ liệu này trên DB",
                    };
                    return NotFound(model);
                }
                else
                {
                    model.status = 1;
                    model.items = result.ToList();
                    return Ok(model);
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

        [HttpPost()]
        public async Task<IActionResult> create(StateUnit data)
        {
            BaseModels<StateUnit> model = new BaseModels<StateUnit>();
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                SystemLog datalog = new SystemLog();
                var stateUnits = _repoStateUnit._context.StateUnits.Any(x => x.StateUnitsCode == data.StateUnitsCode && !x.IsDel);

                if (stateUnits)
                {
                    model.status = 0;
                    model.error = new ErrorModel()
                    {
                        Code = ErrCode_Const.EXCEPTION_API,
                        Msg = "Mã đơn vị đã tồn tại"
                    };
                    datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.CREATE, NameCategory_Const.STATE_UNITS, Action_Status.FAIL);
                    _asyncLogger.LogInfo(JsonConvert.SerializeObject(datalog));
                    return BadRequest(model);
                }

                StateUnit SaveData = new StateUnit();
                SaveData.StateUnitsCode = data.StateUnitsCode;
                SaveData.StateUnitsName = data.StateUnitsName;
                SaveData.ParentId = data.ParentId;

                SaveData.CreateUserId = loginData.Userid;
                SaveData.CreateTime = DateTime.Now;

                await _repoStateUnit.Insert(SaveData);
                datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.CREATE, NameCategory_Const.STATE_UNITS, Action_Status.SUCCESS);
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
        public async Task<IActionResult> Update(StateUnit data)
        {
            BaseModels<StateUnit> model = new BaseModels<StateUnit>();
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                SystemLog datalog = new SystemLog();
                StateUnit? SaveData = _repoStateUnit._context.StateUnits.FirstOrDefault(x => x.StateUnitsId == data.StateUnitsId && !x.IsDel);
                if (SaveData != null)
                {
                    var stateUnits = _repoStateUnit._context.StateUnits.Any(x => x.StateUnitsId != data.StateUnitsId && x.StateUnitsCode == data.StateUnitsCode && !x.IsDel);

                    if (stateUnits)
                    {
                        model.status = 0;
                        model.error = new ErrorModel()
                        {
                            Code = ErrCode_Const.EXCEPTION_API,
                            Msg = "Mã đơn vị đã tồn tại"
                        };
                        datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.UPDATE, NameCategory_Const.STATE_UNITS, Action_Status.FAIL);
                        _asyncLogger.LogInfo(JsonConvert.SerializeObject(datalog));
                        return Ok(model);
                    }

                    SaveData.StateUnitsId = Guid.Parse(data.StateUnitsId.ToString());
                    SaveData.StateUnitsCode = data.StateUnitsCode;
                    SaveData.StateUnitsName = data.StateUnitsName;
                    SaveData.ParentId = data.ParentId;

                    SaveData.UpdateUserId = loginData.Userid;
                    SaveData.UpdateTime = DateTime.Now;

                    await _repoStateUnit.Update(SaveData);
                    datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.UPDATE, NameCategory_Const.STATE_UNITS, Action_Status.SUCCESS);
                    _asyncLogger.LogInfo(JsonConvert.SerializeObject(datalog));
                    model.status = 1;
                    return Ok(model);
                }
                else
                {
                    model.status = 0;
                    datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.UPDATE, NameCategory_Const.STATE_UNITS, Action_Status.FAIL);
                    _asyncLogger.LogInfo(JsonConvert.SerializeObject(datalog));
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

        [HttpPut("deleteStateUnit/{id}")]
        public async Task<IActionResult> deleteStateUnit(Guid id)
        {
            BaseModels<StateUnit> model = new BaseModels<StateUnit>();
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                SystemLog datalog = new SystemLog();
                StateUnit DeleteData = new StateUnit();
                DeleteData.StateUnitsId = id;
                DeleteData.IsDel = true;
                await _repoStateUnit.DeleteStateUnits(DeleteData);
                datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.DELETE, NameCategory_Const.STATE_UNITS, Action_Status.SUCCESS);
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

        [HttpGet("loadparent")]
        public IActionResult LoadParent(Guid id)
        {
            BaseModels<StateUnit> model = new BaseModels<StateUnit>();
            try
            {
                //Lấy Token, lấy model
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }

                //Query lấy data
                IQueryable<StateUnit> _data = _repoStateUnit._context.StateUnits.Where(x => !x.IsDel).Select(x => new StateUnit
                {
                    StateUnitsId = x.StateUnitsId,
                    StateUnitsCode = x.StateUnitsCode,
                    StateUnitsName = x.StateUnitsName,
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
    }
}
