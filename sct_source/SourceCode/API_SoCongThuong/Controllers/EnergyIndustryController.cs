
using API_SoCongThuong.Classes;
using API_SoCongThuong.Logger;
using API_SoCongThuong.Models;
using API_SoCongThuong.Reponsitories.EnergyIndustryRepository;
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
    public class EnergyIndustryController : ControllerBase
    {
        private EnergyIndustryRepo _repoEnergyIndustry;
        private IConfiguration _configuration;
        private readonly ILogger<AsyncLogger> _logger;
        public SoHoa_SoCongThuongContext _context;
        private AsyncLogger _asyncLogger;
        public EnergyIndustryController(SoHoa_SoCongThuongContext context, IConfiguration configuration, ILogger<AsyncLogger> logger)
        {
            _repoEnergyIndustry = new EnergyIndustryRepo(context);

            _logger = logger;
            _context = context;
            _asyncLogger = new AsyncLogger(_logger, _context);
            _configuration = configuration;
        }

        [Route("find")]
        [HttpPost]
        public IActionResult ListItems_New([FromBody] QueryRequestBody query)//query truyền lên
        {

            BaseModels<EnergyIndustryModel> model = new BaseModels<EnergyIndustryModel>();
            string _keywordSearch = ""; //Keyword tìm kiếm
            bool _orderBy_ASC = true;  //Khởi tạo sắp xếp dữ liệu acs hoặc desc khi tìm kiếm
            try
            {
                //Lấy Token, lấy model
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }

                Func<EnergyIndustryModel, object> _orderByExpression = x => x.EnergyIndustryCode; //Khởi tạo mặc định sắp xếp dữ liệu
                Dictionary<string, Func<EnergyIndustryModel, object>> _sortableFields = new Dictionary<string, Func<EnergyIndustryModel, object>>   //Khởi tạo các trường để sắp xếp
                    {
                        { "EnergyIndustryCode", x => x.EnergyIndustryCode },
                        { "EnergyIndustryName", x => x.EnergyIndustryName }
                    };

                if (query.Sort != null && !string.IsNullOrEmpty(query.Sort.ColumnName) && _sortableFields.ContainsKey(query.Sort.ColumnName))
                {
                    //Sắp xếp asc hoặc desc
                    _orderBy_ASC = ("desc".Equals(query.Sort.Direction.ToLower()) ? false : true);

                    //Trường cần sắp xếp
                    _orderByExpression = _sortableFields[query.Sort.ColumnName];
                }

                //Cách 1 dùng entity
                IQueryable<EnergyIndustryModel> _data = _repoEnergyIndustry._context.EnergyIndustries.Select(x => new EnergyIndustryModel
                {
                    EnergyIndustryId = x.EnergyIndustryId,
                    EnergyIndustryCode = x.EnergyIndustryCode ?? "",
                    EnergyIndustryName = x.EnergyIndustryName ?? "",
                    IsDel = x.IsDel
                });
                _data = _data.Where(x => !x.IsDel);

                //Kiểm tra điều kiện tìm kiếm
                if (query.SearchValue != null && query.SearchValue != "")
                {
                    _keywordSearch = query.SearchValue.Trim().ToLower();
                    //Lấy table đã select tìm kiếm theo keyword
                    _data = _data.Where(x =>
                       x.EnergyIndustryName.ToLower().Contains(_keywordSearch)
                       || x.EnergyIndustryCode.ToLower().Contains(_keywordSearch)
                   );
                }
                if (query.Filter != null && query.Filter.ContainsKey("idGroupParent") && !string.IsNullOrEmpty(query.Filter["idGroupParent"]))
                {
                    _data = _data.Where(x => x.EnergyIndustryId.ToString().Contains(string.Join("", query.Filter["idGroupParent"])));
                }
                int _countRows = _data.Count(); //Đếm số dòng của table đã select được
                if (_countRows == 0)    //nếu table = 0 thì trả về không có dữ liệu
                {
                    return NotFound("Không có dữ liệu");
                }
                //query more = true
                if (query.Panigator.More)
                {
                    model.status = 1;
                    model.items = _data.ToList();
                    model.total = _countRows;
                    return Ok(model);
                }
                //Sắp xếp dữ liệu theo acs
                if (_orderBy_ASC)
                {
                    model.items = _data
                        .OrderBy(_orderByExpression)
                        .Skip((query.Panigator.PageIndex - 1) * query.Panigator.PageSize)
                        .Take(query.Panigator.PageSize)
                        .ToList();
                }
                //Sắp xếp dữ liệu theo desc
                else
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

        [HttpGet("{id}")]
        public IActionResult getItemById(Guid id)
        {
            BaseModels<EnergyIndustry> model = new BaseModels<EnergyIndustry>();
            try
            {
                var result = _repoEnergyIndustry.FindById(id);
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
        public async Task<IActionResult> Update(EnergyIndustry data)
        {
            BaseModels<EnergyIndustry> model = new BaseModels<EnergyIndustry>();
            try
            {

                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                SystemLog datalog = new SystemLog();
                EnergyIndustry? SaveData = _repoEnergyIndustry._context.EnergyIndustries.Where(x => x.EnergyIndustryId == data.EnergyIndustryId && !x.IsDel).FirstOrDefault();
                if (SaveData != null)
                {
                    var energyIndustry = _repoEnergyIndustry.findByEnergyIndustryCode(data.EnergyIndustryCode, Guid.Parse(data.EnergyIndustryId.ToString()));

                    if (energyIndustry)
                    {
                        model.status = 0;
                        model.error = new ErrorModel()
                        {
                            Code = ErrCode_Const.EXCEPTION_API,
                            Msg = "Mã lĩnh vực đã tồn tại"
                        };
                        datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.UPDATE, NameCategory_Const.ENERGY_INDUSTRY, Action_Status.FAIL);
                        _asyncLogger.LogInfo(JsonConvert.SerializeObject(datalog));
                        return Ok(model);
                    }
                    SaveData.EnergyIndustryId = Guid.Parse(data.EnergyIndustryId.ToString());
                    SaveData.EnergyIndustryCode = data.EnergyIndustryCode;
                    SaveData.EnergyIndustryName = data.EnergyIndustryName;

                    SaveData.UpdateUserId = loginData.Userid;
                    SaveData.UpdateTime = DateTime.Now;

                    await _repoEnergyIndustry.Update(SaveData);
                    datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.UPDATE, NameCategory_Const.ENERGY_INDUSTRY, Action_Status.SUCCESS);
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

        [HttpPost()]
        public async Task<IActionResult> create(EnergyIndustry data)
        {
            BaseModels<EnergyIndustry> model = new BaseModels<EnergyIndustry>();
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                SystemLog datalog = new SystemLog();
                var energyIndustry = _repoEnergyIndustry.findByEnergyIndustryCode(data.EnergyIndustryCode, null);

                if (energyIndustry)
                {
                    model.status = 0;
                    model.error = new ErrorModel()
                    {
                        Code = ErrCode_Const.EXCEPTION_API,
                        Msg = "Mã lĩnh vực đã tồn tại"
                    };
                    datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.CREATE, NameCategory_Const.ENERGY_INDUSTRY, Action_Status.FAIL);
                    _asyncLogger.LogInfo(JsonConvert.SerializeObject(datalog));
                    return BadRequest(model);
                }
                EnergyIndustry SaveData = new EnergyIndustry();
                SaveData.EnergyIndustryCode = data.EnergyIndustryCode;
                SaveData.EnergyIndustryName = data.EnergyIndustryName;
                SaveData.CreateUserId = loginData.Userid;
                SaveData.CreateTime = DateTime.Now;

                await _repoEnergyIndustry.Insert(SaveData);
                datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.CREATE, NameCategory_Const.ENERGY_INDUSTRY, Action_Status.SUCCESS);
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

        [HttpPut("deleteEnergyIndustry/{id}")]
        public async Task<IActionResult> deleteEnergyIndustry(Guid id)
        {
            BaseModels<EnergyIndustry> model = new BaseModels<EnergyIndustry>();
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                SystemLog datalog = new SystemLog();
                EnergyIndustry DeleteData = new EnergyIndustry();
                DeleteData.EnergyIndustryId = id;
                DeleteData.IsDel = true;
                await _repoEnergyIndustry.DeleteEnergyIndustry(DeleteData);
                datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.DELETE, NameCategory_Const.ENERGY_INDUSTRY, Action_Status.SUCCESS);
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

        [Route("deleteEnergyIndustries")]
        [HttpPut()]
        public async Task<IActionResult> deleteEnergyIndustries(removeListEnergyIndustryItems data)
        {
            BaseModels<EnergyIndustry> model = new BaseModels<EnergyIndustry>();
            try
            {
                foreach (Guid id in data.EnergyIndustryIds)
                {
                    EnergyIndustry DeleteData = new EnergyIndustry();
                    DeleteData.EnergyIndustryId = id;
                    DeleteData.IsDel = true;
                    await _repoEnergyIndustry.DeleteEnergyIndustry(DeleteData);
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
