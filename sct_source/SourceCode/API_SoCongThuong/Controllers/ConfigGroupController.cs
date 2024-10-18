using API_SoCongThuong.Classes;
using API_SoCongThuong.Logger;
using API_SoCongThuong.Models;
using API_SoCongThuong.Reponsitories.CategoryRepository;
using EF_Core.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace API_SoCongThuong.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConfigGroupController : ControllerBase
    {
        private CategoryRepo _repo;
        private IConfiguration _configuration;
        private readonly ILogger<AsyncLogger> _logger;
        public SoHoa_SoCongThuongContext _context;
        private AsyncLogger _asyncLogger;
        public ConfigGroupController(SoHoa_SoCongThuongContext context, IConfiguration configuration, ILogger<AsyncLogger> logger)
        {
            _repo = new CategoryRepo(context);
            _logger = logger;
            _context = context;
            _asyncLogger = new AsyncLogger(_logger, _context);
            _configuration = configuration;
        }

        [Route("find")]
        [HttpPost]
        public IActionResult Find([FromBody] QueryRequestBody query)
        {

            BaseModels<ConfigGroupModel> model = new BaseModels<ConfigGroupModel>();
            string _keywordSearch = "";
            bool _orderBy_ASC = true;
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }

                Func<ConfigGroupModel, object> _orderByExpression = x => x.Description;
                Dictionary<string, Func<ConfigGroupModel, object>> _sortableFields = new Dictionary<string, Func<ConfigGroupModel, object>>
                {
                    { "CategoryTypeCode", x => x.CategoryTypeCode },
                    { "CategoryTypeName", x => x.CategoryTypeName },
                    { "Description", x => x.Description }
                };
                if (query.Sort != null && !string.IsNullOrEmpty(query.Sort.ColumnName) && _sortableFields.ContainsKey(query.Sort.ColumnName))
                {
                    _orderBy_ASC = ("desc".Equals(query.Sort.Direction.ToLower()) ? false : true);
                    _orderByExpression = _sortableFields[query.Sort.ColumnName];
                }

                IQueryable<ConfigGroupModel> _data = _repo._context.CategoryTypes
                    .Where(x => x.IsAction == true)
                    .Select(x => new ConfigGroupModel
                    {
                        CategoryTypeId = x.CategoryTypeId,
                        CategoryTypeCode = x.CategoryTypeCode,
                        CategoryTypeName = x.CategoryTypeName,
                        Description = x.Description,
                    }
                );

                if (query.SearchValue != null && query.SearchValue != "")
                {
                    _keywordSearch = query.SearchValue.Trim().ToLower();
                    _data = _data.Where(x =>
                    x.CategoryTypeName.ToLower().Contains(_keywordSearch) ||
                    x.Description.ToLower().Contains(_keywordSearch)
                   );
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
        public async Task<IActionResult> Create(ConfigGroupModel data)
        {
            BaseModels<ConfigGroupModel> model = new BaseModels<ConfigGroupModel>();
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }

                SystemLog datalog = new SystemLog();
                var Check = _repo._context.CategoryTypes.Where(x => x.CategoryTypeId != data.CategoryTypeId && x.CategoryTypeCode == data.CategoryTypeCode).Any();

                if (Check)
                {
                    model.status = 0;
                    model.error = new ErrorModel()
                    {
                        Code = ErrCode_Const.EXCEPTION_API,
                        Msg = $"Mã nhóm cấu hình {data.CategoryTypeCode} đã tồn tại"
                    };
                    datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.CREATE, NameCategory_Const.CONFIGS_GROUP, Action_Status.FAIL);
                    _asyncLogger.LogInfo(JsonConvert.SerializeObject(datalog));
                    return BadRequest(model);
                }

                CategoryType SaveData = new CategoryType();
                SaveData.CategoryTypeId = data.CategoryTypeId;
                SaveData.CategoryTypeCode = data.CategoryTypeCode;
                SaveData.CategoryTypeName = data.CategoryTypeName;
                SaveData.Description = data.Description;
                SaveData.IsAction = true;
                SaveData.IsDel = false;

                await _repo.Insert(SaveData);
                datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.CREATE, NameCategory_Const.CONFIGS_GROUP, Action_Status.SUCCESS);
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
            BaseModels<ConfigGroupModel> model = new BaseModels<ConfigGroupModel>();
            try
            {
                var result = _repo.FindById(id);

                if (result.Any())
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
        public async Task<IActionResult> Update(ConfigGroupModel data)
        {
            BaseModels<Category> model = new BaseModels<Category>();
            try
            {

                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }

                var util = new Ulities();
                data = util.TrimModel(data);
                SystemLog datalog = new SystemLog();
                CategoryType? SaveData = _repo._context.CategoryTypes.Where(x => x.CategoryTypeId == data.CategoryTypeId).FirstOrDefault();
                if (SaveData != null)
                {
                    var Check = _repo._context.CategoryTypes.Where(x => x.CategoryTypeId != data.CategoryTypeId && x.CategoryTypeCode == data.CategoryTypeCode).Any();

                    if (Check)
                    {
                        model.status = 0;
                        model.error = new ErrorModel()
                        {
                            Code = ErrCode_Const.EXCEPTION_API,
                            Msg = $"Mã nhóm cấu hình {data.CategoryTypeCode} đã tồn tại"
                        };
                        datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.UPDATE, NameCategory_Const.CONFIGS_GROUP, Action_Status.FAIL);
                        _asyncLogger.LogInfo(JsonConvert.SerializeObject(datalog));
                        return BadRequest(model);
                    }

                    string OldTypeCode = SaveData.CategoryTypeCode;
                    SaveData.CategoryTypeId = data.CategoryTypeId;
                    SaveData.CategoryTypeCode = data.CategoryTypeCode;
                    SaveData.CategoryTypeName = data.CategoryTypeName;
                    SaveData.Description = data.Description;
                    SaveData.IsAction = true;

                    await _repo.Update(SaveData, OldTypeCode);
                    datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.UPDATE, NameCategory_Const.CONFIGS_GROUP, Action_Status.SUCCESS);
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
                    datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.UPDATE, NameCategory_Const.CONFIGS_GROUP, Action_Status.FAIL);
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
            BaseModels<CategoryType> model = new BaseModels<CategoryType>();
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }

                CategoryType DeleteData = new CategoryType();
                DeleteData.CategoryTypeId = id;
                DeleteData.IsDel = true;
                await _repo.Delete(DeleteData);
                SystemLog datalog = new SystemLog();
                datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.DELETE, NameCategory_Const.CONFIGS_GROUP, Action_Status.SUCCESS);
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
    }
}

