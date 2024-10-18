
using API_SoCongThuong.Classes;
using API_SoCongThuong.Logger;
using API_SoCongThuong.Models;
using API_SoCongThuong.Reponsitories.AdministrativeProcedureFieldRepository;
using EF_Core.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.ComponentModel.Design;
using System.Data;
using static System.Net.Mime.MediaTypeNames;

namespace API_SoCongThuong.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdministrativeProcedureFieldController : ControllerBase
    {
        private AdministrativeProcedureFieldRepo _repo;
        private IConfiguration _configuration;
        private readonly ILogger<AsyncLogger> _logger;
        public SoHoa_SoCongThuongContext _context;
        private AsyncLogger _asyncLogger;
        public AdministrativeProcedureFieldController(SoHoa_SoCongThuongContext context, IConfiguration configuration, ILogger<AsyncLogger> logger)
        {
            _repo = new AdministrativeProcedureFieldRepo(context);
            _logger = logger;
            _context = context;
            _asyncLogger = new AsyncLogger(_logger, _context);
            _configuration = configuration;

        }

        [Route("find")]
        [HttpPost]
        public IActionResult Find([FromBody] QueryRequestBody query)
        {
            BaseModels<AdministrativeProcedureFieldModel> model = new BaseModels<AdministrativeProcedureFieldModel>();
            string _keywordSearch = "";
            bool _orderBy_ASC = true;
            try
            {
                //Lấy Token, lấy model
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }

                Func<AdministrativeProcedureFieldModel, object> _orderByExpression = x => x.Piority;
                Dictionary<string, Func<AdministrativeProcedureFieldModel, object>> _sortableFields = new Dictionary<string, Func<AdministrativeProcedureFieldModel, object>>
                    {
                        { "CategoryCode", x => x.CategoryCode },
                        { "CategoryName", x => x.CategoryName }
                    };
                if (query.Sort != null
                    && !string.IsNullOrEmpty(query.Sort.ColumnName)
                    && _sortableFields.ContainsKey(query.Sort.ColumnName))
                {
                    _orderBy_ASC = ("desc".Equals(query.Sort.Direction.ToLower()) ? false : true);
                    _orderByExpression = _sortableFields[query.Sort.ColumnName];
                }

                IQueryable<AdministrativeProcedureFieldModel> _data = _repo._context.Categories
                    .Where(x => x.IsAction == true && x.CategoryTypeCode == "ADMINISTRATIVE_PROCEDURE_FIELD")
                    .Select(x => new AdministrativeProcedureFieldModel
                    {
                        CategoryId = x.CategoryId,
                        CategoryCode = x.CategoryCode,
                        CategoryName = x.CategoryName,
                        Piority = x.Piority,
                    }).ToList().AsQueryable();


                _data = _data.Where(x => x.IsAction);

                if (query.SearchValue != null && query.SearchValue != "")
                {
                    _keywordSearch = query.SearchValue.Trim().ToLower();
                    _data = _data.Where(x =>
                       x.CategoryName.ToLower().Contains(_keywordSearch)
                       || x.CategoryCode.ToLower().Contains(_keywordSearch)
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

        [HttpGet("{id}")]
        public IActionResult getItemById(Guid id)
        {
            BaseModels<AdministrativeProcedureFieldModel> model = new BaseModels<AdministrativeProcedureFieldModel>();
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
        public async Task<IActionResult> Update(AdministrativeProcedureFieldModel data)
        {
            BaseModels<AdministrativeProcedureFieldModel> model = new BaseModels<AdministrativeProcedureFieldModel>();
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                SystemLog datalog = new SystemLog();
                Category? SaveData = _repo._context.Categories.Where(x => x.CategoryId == data.CategoryId && x.IsAction == true).FirstOrDefault();
                if (SaveData != null)
                {
                    var IsCodeExist = _repo._context.Categories.Where(x => x.CategoryId != data.CategoryId && x.CategoryCode == data.CategoryCode && x.IsAction == true);

                    if (IsCodeExist.Any())
                    {
                        model.status = 0;
                        model.error = new ErrorModel()
                        {
                            Code = ErrCode_Const.EXCEPTION_API,
                            Msg = "Mã lĩnh vực giải quyết đã tồn tại"
                        };
                        datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.UPDATE, NameCategory_Const.ADMIN_IS_TRATIVE_PRODUCE_FIELD, Action_Status.FAIL);
                        _asyncLogger.LogInfo(JsonConvert.SerializeObject(datalog));
                        return BadRequest(model);
                    }

                    SaveData.CategoryId = Guid.Parse(data.CategoryId.ToString());
                    SaveData.CategoryCode = data.CategoryCode;
                    SaveData.CategoryName = data.CategoryName;

                    await _repo.Update(SaveData);
                    datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.UPDATE, NameCategory_Const.ADMIN_IS_TRATIVE_PRODUCE_FIELD, Action_Status.SUCCESS);
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
                    datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.UPDATE, NameCategory_Const.ADMIN_IS_TRATIVE_PRODUCE_FIELD, Action_Status.FAIL);
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

        [HttpPost()]
        public async Task<IActionResult> create(AdministrativeProcedureFieldModel data)
        {
            BaseModels<Category> model = new BaseModels<Category>();
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }

                var IsCodeExist = _repo._context.Categories.Where(x => x.CategoryCode == data.CategoryCode && x.IsAction == true);
                SystemLog datalog = new SystemLog();
                if (IsCodeExist.Any())
                {
                    model.status = 0;
                    model.error = new ErrorModel()
                    {
                        Code = ErrCode_Const.EXCEPTION_API,
                        Msg = "Mã lĩnh vực giải quyết đã tồn tại"
                    };
                    datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.CREATE, NameCategory_Const.ADMIN_IS_TRATIVE_PRODUCE_FIELD, Action_Status.FAIL);
                    _asyncLogger.LogInfo(JsonConvert.SerializeObject(datalog));
                    return BadRequest(model);
                }


                Category SaveData = new Category();
                SaveData.CategoryCode = data.CategoryCode;
                SaveData.CategoryName = data.CategoryName;
                SaveData.CategoryTypeCode = "ADMINISTRATIVE_PROCEDURE_FIELD";
                SaveData.Piority = _repo._context.Categories.Where(x => x.CategoryTypeCode == "ADMINISTRATIVE_PROCEDURE_FIELD" && x.IsAction == true).Max(x => x.Piority) + 1;
                SaveData.IsAction = true;

                await _repo.Insert(SaveData);
                datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.CREATE, NameCategory_Const.ADMIN_IS_TRATIVE_PRODUCE_FIELD, Action_Status.SUCCESS);
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

        [HttpPut("delete/{id}")]
        public async Task<IActionResult> delete(Guid id)
        {
            BaseModels<Category> model = new BaseModels<Category>();
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                SystemLog datalog = new SystemLog();
                var CheckAdministrativeProcedures = _repo._context.AdministrativeProcedures.Where(x => x.AdministrativeProceduresField == id && !x.IsDel).Any();
                var CheckProcessAdministrativeProcedures = _repo._context.ProcessAdministrativeProcedures.Where(x => x.ProcessAdministrativeProceduresField == id && !x.IsDel).Any();
                var CheckSampleContract = _repo._context.SampleContracts.Where(x => x.SampleContractField == id && !x.IsDel).Any();

                if (CheckAdministrativeProcedures || CheckProcessAdministrativeProcedures || CheckSampleContract)
                {
                    model.status = 0;
                    model.error = new ErrorModel()
                    {
                        Code = ErrCode_Const.EXCEPTION_API,
                        Msg = "Dữ liệu đang được sử dụng ở trang khác"
                    };
                    datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.DELETE, NameCategory_Const.ADMIN_IS_TRATIVE_PRODUCE_FIELD, Action_Status.FAIL);
                    _asyncLogger.LogInfo(JsonConvert.SerializeObject(datalog));
                    return BadRequest(model);
                }

                Category DeleteData = new Category();
                DeleteData.CategoryId = id;
                DeleteData.IsAction = false;

                await _repo.Delete(DeleteData);
                datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.DELETE, NameCategory_Const.ADMIN_IS_TRATIVE_PRODUCE_FIELD, Action_Status.SUCCESS);
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
