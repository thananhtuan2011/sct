using API_SoCongThuong.Classes;
using API_SoCongThuong.Logger;
using API_SoCongThuong.Models;
using API_SoCongThuong.Reponsitories;
using EF_Core.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;


namespace API_SoCongThuong.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecordsFinancePlanController : ControllerBase
    {
        private RecordsFinancePlanRepo _repo;
        private IConfiguration _configuration;
        private readonly ILogger<AsyncLogger> _logger;
        public SoHoa_SoCongThuongContext _context;
        private AsyncLogger _asyncLogger;

        public RecordsFinancePlanController(SoHoa_SoCongThuongContext context, IConfiguration configuration, ILogger<AsyncLogger> logger)
        {
            _repo = new RecordsFinancePlanRepo(context);
            _logger = logger;
            _context = context;
            _asyncLogger = new AsyncLogger(_logger, _context);
            _configuration = configuration;
        }

        [Route("find")]
        [HttpPost]
        public IActionResult ListItems_New([FromBody] QueryRequestBody query)//query truyền lên
        {

            BaseModels<RecordsFinancePlanModel> model = new BaseModels<RecordsFinancePlanModel>();
            string _keywordSearch = ""; //Keyword tìm kiếm
            bool _orderBy_ASC = true;  //Khởi tạo sắp xếp dữ liệu acs hoặc desc khi tìm kiếm
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                Func<RecordsFinancePlanModel, object> _orderByExpression = x => x.Code; //Khởi tạo mặc định sắp xếp dữ liệu
                Dictionary<string, Func<RecordsFinancePlanModel, object>> _sortableFields = new Dictionary<string, Func<RecordsFinancePlanModel, object>>
                {
                    { "Code", x => x.Code },
                    { "Name" , x => x.Name },
                };//Khởi tạo các trường để sắp xếp
                IQueryable<RecordsFinancePlanModel> _data = _repo._context.RecordsFinancePlans.Where(x => !x.IsDel).Select(x => new RecordsFinancePlanModel
                {
                    Code = x.Code,
                    Name = x.Name,
                    RecordsFinancePlanId = x.RecordsFinancePlanId
                });
                if (query.Sort != null
                   && !string.IsNullOrEmpty(query.Sort.ColumnName)
                   && _sortableFields.ContainsKey(query.Sort.ColumnName))
                {
                    _orderBy_ASC = ("desc".Equals(query.Sort.Direction.ToLower()) ? false : true);    //Sắp xếp asc hoặc desc
                    _orderByExpression = _sortableFields[query.Sort.ColumnName]; //Trường cần sắp xếp
                }
                if (query.SearchValue != null && query.SearchValue != "") //Kiểm tra điều kiện tìm kiếm
                {
                    _keywordSearch = query.SearchValue.Trim().ToLower();
                    _data = _data.Where(x =>
                       /* x.DistrictId.ToString().ToLower().Contains(_keywordSearch)
                        || */
                       x.Code.ToLower().Contains(_keywordSearch) || x.Name.ToLower().Contains(_keywordSearch)
                   );  //Lấy table đã select tìm kiếm theo keyword
                }
                int _countRows = _data.Count(); //Đếm số dòng của table đã select được
                if (_countRows == 0)    //nếu table = 0 thì trả về không có dữ liệu
                {
                    return NotFound("Không có dữ liệu");
                }
                if (query.Panigator.More)    //query more = true
                {
                    model.status = 1;
                    model.items = _data.ToList();
                    model.total = _countRows;
                    return Ok(model);
                }
                if (_orderBy_ASC) //Sắp xếp dữ liệu theo acs
                {
                    model.items = _data
                        .OrderBy(_orderByExpression)
                        .Skip((query.Panigator.PageIndex - 1) * query.Panigator.PageSize)
                        .Take(query.Panigator.PageSize)
                        .ToList();
                }
                else //Sắp xếp dữ liệu theo desc
                {
                    model.items = _data
                        .OrderByDescending(_orderByExpression)
                        .Skip((query.Panigator.PageIndex - 1) * query.Panigator.PageSize)
                        .Take(query.Panigator.PageSize)
                        .ToList();
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

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(RecordsFinancePlanModel data)
        {
            BaseModels<RecordsFinancePlanModel> model = new BaseModels<RecordsFinancePlanModel>();
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                SystemLog datalog = new SystemLog();
                int count = _repo._context.RecordsFinancePlans.Where(x => !x.IsDel && x.RecordsFinancePlanId != data.RecordsFinancePlanId && x.Code == data.Code).Count();
                if (count > 0)
                {
                    model.status = 0;
                    model.error = new ErrorModel()
                    {
                        Code = ErrCode_Const.EXCEPTION_API,
                        Msg = "Mã nhóm hồ sơ đã tồn tại!"
                    };
                    datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.UPDATE, NameCategory_Const.RECORDS_FINANCE_PLAN, Action_Status.FAIL);
                    _asyncLogger.LogInfo(JsonConvert.SerializeObject(datalog));
                    return Ok(model);
                }
                RecordsFinancePlan? _data = _repo._context.RecordsFinancePlans.Where(x => !x.IsDel && x.RecordsFinancePlanId == data.RecordsFinancePlanId).FirstOrDefault();
                if (_data != null)
                {
                    _data.Code = data.Code;
                    _data.Name = data.Name;
                    _data.UpdateTime = DateTime.Now;
                    _data.UpdateUserId = loginData.Userid;
                    datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.UPDATE, NameCategory_Const.RECORDS_FINANCE_PLAN, Action_Status.SUCCESS);
                    _asyncLogger.LogInfo(JsonConvert.SerializeObject(datalog));
                    await _repo.Update(_data);
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
        [HttpPost()]
        public async Task<IActionResult> create(RecordsFinancePlanModel data)
        {
            BaseModels<RecordsFinancePlanModel> model = new BaseModels<RecordsFinancePlanModel>();
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                SystemLog datalog = new SystemLog();
                int count = _repo._context.RecordsFinancePlans.Where(x => !x.IsDel && x.Code == data.Code).Count();
                if (count > 0)
                {
                    model.status = 0;
                    model.error = new ErrorModel()
                    {
                        Code = ErrCode_Const.EXCEPTION_API,
                        Msg = "Mã nhóm hồ sơ đã tồn tại!"
                    };
                    datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.CREATE, NameCategory_Const.RECORDS_FINANCE_PLAN, Action_Status.FAIL);
                    _asyncLogger.LogInfo(JsonConvert.SerializeObject(datalog));
                    return Ok(model);
                }
                RecordsFinancePlan SaveData = new RecordsFinancePlan();
                SaveData.Code = data.Code;
                SaveData.Name = data.Name;
                SaveData.CreateTime = DateTime.Now;
                SaveData.CreateUserId = loginData.Userid;
                await _repo.Insert(SaveData);
                datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.CREATE, NameCategory_Const.RECORDS_FINANCE_PLAN, Action_Status.SUCCESS);
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
        public async Task<IActionResult> Delete(Guid id)
        {
            BaseModels<RecordsFinancePlanModel> model = new BaseModels<RecordsFinancePlanModel>();
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                SystemLog datalog = new SystemLog();
                RecordsFinancePlan item = new RecordsFinancePlan();
                item.RecordsFinancePlanId = id;
                item.IsDel = true;
                await _repo.Delete(item);
                datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.DELETE, NameCategory_Const.RECORDS_FINANCE_PLAN, Action_Status.SUCCESS);
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

        [HttpGet("findAll")]
        public IActionResult FindAll()
        {
            BaseModels<RecordsFinancePlanModel> model = new BaseModels<RecordsFinancePlanModel>();
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                IQueryable<RecordsFinancePlanModel> _data = _repo._context.RecordsFinancePlans.Where(x => !x.IsDel).Select(x => new RecordsFinancePlanModel
                {
                    Code = x.Code,
                    Name = x.Name,
                    RecordsFinancePlanId = x.RecordsFinancePlanId
                }).OrderBy(x => x.Name);
                int _countRows = _data.Count();
                if(_countRows > 0)
                {
                    model.items = _data.ToList();
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
    }
}
