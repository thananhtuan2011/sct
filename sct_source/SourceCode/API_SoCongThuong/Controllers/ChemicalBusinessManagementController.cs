
using API_SoCongThuong.Classes;
using API_SoCongThuong.Logger;
using API_SoCongThuong.Models;
using API_SoCongThuong.Reponsitories.ChemicalBusinessManagementRepository;
using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.EMMA;
using DocumentFormat.OpenXml.Spreadsheet;
using EF_Core.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.ComponentModel.Design;
using System.Data;
using System.Numerics;
using System.Reflection.Metadata;
using System.Runtime.Intrinsics.X86;
using static System.Net.Mime.MediaTypeNames;

namespace API_SoCongThuong.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChemicalBusinessManagementController : ControllerBase
    {
        private ChemicalBusinessManagementRepo _repo;

        private IConfiguration _configuration;
        private readonly ILogger<AsyncLogger> _logger;
        public SoHoa_SoCongThuongContext _context;
        private AsyncLogger _asyncLogger;
        public ChemicalBusinessManagementController(SoHoa_SoCongThuongContext context, IConfiguration configuration, ILogger<AsyncLogger> logger)
        {
            _repo = new ChemicalBusinessManagementRepo(context);

            _logger = logger;
            _context = context;
            _asyncLogger = new AsyncLogger(_logger, _context);
            _configuration = configuration;
        }

        [Route("find")]
        [HttpPost]
        public IActionResult ListItems_New([FromBody] QueryRequestBody query)//query truyền lên
        {

            BaseModels<ChemicalBusinessManagementModel> model = new BaseModels<ChemicalBusinessManagementModel>();
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

                Func<ChemicalBusinessManagementModel, object> _orderByExpression = x => x.BusinessName; //Khởi tạo mặc định sắp xếp dữ liệu
                Dictionary<string, Func<ChemicalBusinessManagementModel, object>> _sortableFields = new Dictionary<string, Func<ChemicalBusinessManagementModel, object>>   //Khởi tạo các trường để sắp xếp
                    {
                        { "BusinessNameString", x => x.BusinessNameString },
                        { "Address", x => x.Address },
                        { "ChemicalStorage", x => x.ChemicalStorage },
                        { "PNUPSCHCmeasures", x => x.Pnupschcmeasures },
                        { "Status", x => x.Status },
                    };

                if (query.Sort != null && !string.IsNullOrEmpty(query.Sort.ColumnName) && _sortableFields.ContainsKey(query.Sort.ColumnName))
                {
                    _orderBy_ASC = ("desc".Equals(query.Sort.Direction.ToLower()) ? false : true);    //Sắp xếp asc hoặc desc
                    _orderByExpression = _sortableFields[query.Sort.ColumnName]; //Trường cần sắp xếp
                }

                IQueryable<ChemicalBusinessManagementModel> _data = (
                from cbm in _repo._context.ChemicalBusinessManagements
                where !cbm.IsDel
                join b in _repo._context.Businesses
                on cbm.BusinessName equals b.BusinessId
                join d in _repo._context.Districts
                        on cbm.DistrictId equals d.DistrictId
                join c in _repo._context.Communes
                    on cbm.CommuneId equals c.CommuneId
                select new ChemicalBusinessManagementModel
                {
                    ChemicalBusinessManagementId = cbm.ChemicalBusinessManagementId,
                    BusinessName = cbm.BusinessName,
                    BusinessNameString = b.BusinessNameVi,
                    Address = cbm.Address,
                    ChemicalStorage = cbm.ChemicalStorage,
                    Pnupschcmeasures = cbm.Pnupschcmeasures,
                    Status = cbm.Status,
                    IsDel = cbm.IsDel,
                    DistrictId = cbm.DistrictId,
                    CommuneId = cbm.CommuneId,
                    Represent = cbm.Represent,
                    DistrictName = d != null && !d.IsDel ? (d.DistrictName) : "",
                    CommuneName = c != null && !c.IsDel ? c.CommuneName : "",
                }
                ).ToList().AsQueryable();


                if (query.SearchValue != null && query.SearchValue != "")
                {
                    _keywordSearch = query.SearchValue.Trim().ToLower();
                    _data = _data.Where(x =>
                    x.BusinessNameString.ToLower().Contains(_keywordSearch)
                    || x.Address.ToLower().Contains(_keywordSearch)
                    || x.ChemicalStorage.ToLower().Contains(_keywordSearch)
               );
                }

                if (query.Filter != null && query.Filter.ContainsKey("Status") && !string.IsNullOrEmpty(query.Filter["Status"]))
                {
                    _data = _data.Where(x => x.Status.ToString() == query.Filter["Status"]);
                }

                if (query.Filter != null && query.Filter.ContainsKey("DistrictId") && !string.IsNullOrEmpty(query.Filter["DistrictId"]))
                {
                    _data = _data.Where(x => x.DistrictId.ToString() == query.Filter["DistrictId"]);
                }

                if (query.Filter != null && query.Filter.ContainsKey("CommuneId") && !string.IsNullOrEmpty(query.Filter["CommuneId"]))
                {
                    _data = _data.Where(x => x.CommuneId.ToString() == query.Filter["CommuneId"]);
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
            BaseModels<ChemicalBusinessManagement> model = new BaseModels<ChemicalBusinessManagement>();
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
        public async Task<IActionResult> Update(ChemicalBusinessManagement data)
        {
            BaseModels<ChemicalBusinessManagement> model = new BaseModels<ChemicalBusinessManagement>();
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                SystemLog datalog = new SystemLog();
                ChemicalBusinessManagement? SaveData = _repo._context.ChemicalBusinessManagements.Where(x => x.ChemicalBusinessManagementId == data.ChemicalBusinessManagementId && !x.IsDel).FirstOrDefault();
                if (SaveData != null)
                {
                    SaveData.ChemicalBusinessManagementId = Guid.Parse(data.ChemicalBusinessManagementId.ToString());
                    SaveData.BusinessName = data.BusinessName;
                    SaveData.Address = data.Address;
                    SaveData.ChemicalStorage = data.ChemicalStorage;
                    SaveData.Pnupschcmeasures = data.Pnupschcmeasures;
                    SaveData.Status = data.Status;
                    SaveData.CommuneId = data.CommuneId;
                    SaveData.DistrictId = data.DistrictId;
                    SaveData.Represent = data.Represent;

                    SaveData.UpdateUserId = loginData.Userid;
                    SaveData.UpdateTime = DateTime.Now;

                    await _repo.Update(SaveData);
                    datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.UPDATE, NameCategory_Const.CHEMICAL_BUSINESS_MANAGEMENT, Action_Status.SUCCESS);
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
                    datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.UPDATE, NameCategory_Const.CHEMICAL_BUSINESS_MANAGEMENT, Action_Status.FAIL);
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
        public async Task<IActionResult> create(ChemicalBusinessManagement data)
        {
            BaseModels<ChemicalBusinessManagement> model = new BaseModels<ChemicalBusinessManagement>();
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                SystemLog datalog = new SystemLog();
                ChemicalBusinessManagement SaveData = new ChemicalBusinessManagement();
                SaveData.BusinessName = data.BusinessName;
                SaveData.Address = data.Address;
                SaveData.ChemicalStorage = data.ChemicalStorage;
                SaveData.Pnupschcmeasures = data.Pnupschcmeasures;
                SaveData.Status = data.Status;
                SaveData.CommuneId = data.CommuneId;
                SaveData.DistrictId = data.DistrictId;
                SaveData.Represent = data.Represent;

                SaveData.CreateUserId = loginData.Userid;
                SaveData.CreateTime = DateTime.Now;

                await _repo.Insert(SaveData);
                datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.CREATE, NameCategory_Const.CHEMICAL_BUSINESS_MANAGEMENT, Action_Status.SUCCESS);
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
            BaseModels<ChemicalBusinessManagement> model = new BaseModels<ChemicalBusinessManagement>();
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                SystemLog datalog = new SystemLog();
                ChemicalBusinessManagement DeleteData = new ChemicalBusinessManagement();
                DeleteData.ChemicalBusinessManagementId = id;
                DeleteData.IsDel = true;
                await _repo.DeleteChemicalBusinessManagement(DeleteData);
                datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.DELETE, NameCategory_Const.CHEMICAL_BUSINESS_MANAGEMENT, Action_Status.SUCCESS);
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
