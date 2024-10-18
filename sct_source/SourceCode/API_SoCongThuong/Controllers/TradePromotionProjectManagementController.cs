
using API_SoCongThuong.Classes;
using API_SoCongThuong.Models;
using API_SoCongThuong.Reponsitories.TradePromotionProjectManagementRepository;
using API_SoCongThuong.Reponsitories.UnitRepository;
using API_SoCongThuong.Reponsitories.UserRepository;
using EF_Core.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.Design;
using System;
using System.Text;
using Newtonsoft.Json;
using StackExchange.Redis;


using static API_SoCongThuong.Classes.Ulities;
using Microsoft.Extensions.Configuration;
using API_SoCongThuong.Logger;

namespace API_SoCongThuong.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TradePromotionProjectManagementController : ControllerBase
    {
        private TradePromotionProjectManagementRepo _repoTradePromotionProjectManagement;
        private UnitRepo _repoUnit;
        private UserRepo _repoUser;
        private IConfiguration _config;
        private IConnectionMultiplexer redisCache;

        private readonly ILogger<AsyncLogger> _logger;
        public SoHoa_SoCongThuongContext _context;
        private AsyncLogger _asyncLogger;

        public TradePromotionProjectManagementController(SoHoa_SoCongThuongContext context, IConfiguration configuration, IConnectionMultiplexer redisCache, ILogger<AsyncLogger> logger)
        {
            _repoTradePromotionProjectManagement = new TradePromotionProjectManagementRepo(context);
            _repoUnit = new UnitRepo(context);
            _repoUser = new UserRepo(context, configuration, redisCache);
            _config = configuration;
            _logger = logger;
            _context = context;
            _asyncLogger = new AsyncLogger(_logger, _context);
        }

        [Route("find")]
        [HttpPost]
        public IActionResult ListItems_New([FromBody] QueryRequestBody query)//query truyền lên
        {

            BaseModels<TradePromotionProjectManagementModel> model = new BaseModels<TradePromotionProjectManagementModel>();
            string _keywordSearch = ""; //Keyword tìm kiếm
            bool _orderBy_ASC = true;  //Khởi tạo sắp xếp dữ liệu acs hoặc desc khi tìm kiếm
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }

                Func<TradePromotionProjectManagementModel, object> _orderByExpression = x => x.TradePromotionProjectManagementName; //Khởi tạo mặc định sắp xếp dữ liệu
                Dictionary<string, Func<TradePromotionProjectManagementModel, object>> _sortableFields = new Dictionary<string, Func<TradePromotionProjectManagementModel, object>>   //Khởi tạo các trường để sắp xếp
                {
                    { "TradePromotionProjectManagementName", x => x.TradePromotionProjectManagementName },
                };
                if (query.Sort != null
                    && !string.IsNullOrEmpty(query.Sort.ColumnName)
                    && _sortableFields.ContainsKey(query.Sort.ColumnName))
                {
                    _orderBy_ASC = ("desc".Equals(query.Sort.Direction.ToLower()) ? false : true);    //Sắp xếp asc hoặc desc
                    _orderByExpression = _sortableFields[query.Sort.ColumnName]; //Trường cần sắp xếp
                }
                //Cách 1 dùng entity
                IQueryable<TradePromotionProjectManagementModel> _data = _repoTradePromotionProjectManagement._context.TradePromotionProjectManagements.Select(x => new TradePromotionProjectManagementModel
                {
                    TradePromotionProjectManagementId = x.TradePromotionProjectManagementId,
                    TradePromotionProjectManagementName = x.TradePromotionProjectManagementName ?? "",
                    ImplementingAgencies = x.ImplementingAgencies,
                    Cost = x.Cost,
                    CurrencyUnit = x.CurrencyUnit,
                    TimeStart = x.TimeStart,
                    TimeEnd = x.TimeEnd,
                    NumberOfApprovalDocuments = x.NumberOfApprovalDocuments,
                    ImplementationResults = x.ImplementationResults,
                    Status = x.Status,
                    Reason = x.Reason,
                    IsDel = x.IsDel,
                    CreateUserId = x.CreateUserId,
                    InputDataPerson = "",
                }); ;
                _data = _data.Where(x => !x.IsDel);
                var userData = _repoUser.FindAll();
                if (query.SearchValue != null && query.SearchValue != "") //Kiểm tra điều kiện tìm kiếm
                {
                    _keywordSearch = query.SearchValue.Trim().ToLower();
                    _data = _data.Where(x =>
                       /* x.TradePromotionProjectManagementId.ToString().ToLower().Contains(_keywordSearch)
                        || */
                       x.TradePromotionProjectManagementName.ToLower().Contains(_keywordSearch)
                   );  //Lấy table đã select tìm kiếm theo keyword
                }

                if (query.Filter != null && query.Filter.ContainsKey("ImplementResult"))
                {
                    var implementResultFilter = Int32.Parse(query.Filter["ImplementResult"]);
                    _data = _data.Where(x => x.ImplementationResults == implementResultFilter);
                }

                if (query.Filter != null && query.Filter.ContainsKey("InputDataPersonId"))
                {
                    var inputDataPersonIdFilter = query.Filter["InputDataPersonId"];
                    _data = _data.Where(x => x.CreateUserId == Guid.Parse(inputDataPersonIdFilter));
                }

                if (query.Filter != null && query.Filter.ContainsKey("MinTime")
                    && !string.IsNullOrEmpty(query.Filter["MinTime"]))
                {
                    _data = _data.Where(x =>
                                (x.TimeStart) >=
                                DateTime.ParseExact(query.Filter["MinTime"], "dd/MM/yyyy", null));
                }

                if (query.Filter != null && query.Filter.ContainsKey("MaxTime")
                    && !string.IsNullOrEmpty(query.Filter["MaxTime"]))
                {
                    _data = _data.Where(x =>
                               x.TimeStart <=
                                DateTime.ParseExact(query.Filter["MaxTime"], "dd/MM/yyyy", null));
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

                for (int i = 0; i < model.items.Count; i++)
                {
                    foreach (User item in userData)
                    {
                        if (model.items[i].CreateUserId == item.UserId)
                        {
                            model.items[i].InputDataPerson = item.FullName;
                            break;
                        }
                    }
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
            BaseModels<TradePromotionProjectManagementModel> model = new BaseModels<TradePromotionProjectManagementModel>();
            try
            {
                var result = _repoTradePromotionProjectManagement.FindById(id, _config);
                if (result != null)
                {
                    model.status = 1;
                    model.data = result;
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
        public async Task<IActionResult> Update([FromForm] TradePromotionProjectManagementModel data)
        {
            BaseModels<TradePromotionProjectManagementModel> model = new BaseModels<TradePromotionProjectManagementModel>();
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                SystemLog datalog = new SystemLog();
                var CheckData = _repoTradePromotionProjectManagement.FindById(data.TradePromotionProjectManagementId, _config);
                if (CheckData == null)
                {
                    //chỗ này không tồn tại id
                    model.status = 0;
                    model.error = new ErrorModel()
                    {
                        Code = ErrCode_Const.PROPERTY_IS_NULL_OR_EMPTY
                    };
                    datalog = Ulities.WriteLog(_config, HttpContext, loginData.Username, ActionType_Const.UPDATE, NameCategory_Const.TRADE_PROMOTION_PROJECT_MANAGEMENT, Action_Status.FAIL);
                    _asyncLogger.LogInfo(JsonConvert.SerializeObject(datalog));
                    return BadRequest(model);
                }
                else
                {
                    #region gắn hàm upload file
                    var Files = Request.Form.Files;
                    var LstFile = new List<TradePromotionProjectManagementAttachFileModel>();
                    foreach (var f in Files)
                    {
                        if (f.Length > 0)
                        {
                            using (var ms = new MemoryStream())
                            {
                                f.CopyTo(ms);
                                upLoadFileModel up = new upLoadFileModel()
                                {
                                    bs = ms.ToArray(),
                                    FileName = f.FileName.Replace(" ", ""),
                                    Linkfile = "TradePromotionProjectManagement"
                                };
                                var result = Ulities.UploadFile(up, _config);

                                TradePromotionProjectManagementAttachFileModel fileSave = new TradePromotionProjectManagementAttachFileModel();
                                fileSave.LinkFile = result.link;
                                LstFile.Add(fileSave);
                            }
                        }
                    }
                    data.Details = LstFile;
                    #endregion
                    data.TimeStart = Ulities.ConvertTimeZone(HttpContext.Request.Headers, JsonConvert.DeserializeObject<DateTime>(data.TimeStartGet));
                    data.TimeEnd = data.TimeEndGet != "null" ? Ulities.ConvertTimeZone(HttpContext.Request.Headers, JsonConvert.DeserializeObject<DateTime>(data.TimeEndGet)) : null;
                    data.UpdateTime = DateTime.Now;
                    data.UpdateUserId = loginData.Userid;
                    data.BusinessDetails = JsonConvert.DeserializeObject<List<TradePromotionProjectManagementDetailModel>>(data.BusinessDetail);
                    await _repoTradePromotionProjectManagement.Update(data, _config);
                    datalog = Ulities.WriteLog(_config, HttpContext, loginData.Username, ActionType_Const.UPDATE, NameCategory_Const.TRADE_PROMOTION_PROJECT_MANAGEMENT, Action_Status.SUCCESS);
                    _asyncLogger.LogInfo(JsonConvert.SerializeObject(datalog));
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
        public async Task<IActionResult> create([FromForm] TradePromotionProjectManagementModel data)
        {
            BaseModels<TradePromotionProjectManagement> model = new BaseModels<TradePromotionProjectManagement>();
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                SystemLog datalog = new SystemLog();
                data.BusinessDetails = JsonConvert.DeserializeObject<List<TradePromotionProjectManagementDetailModel>>(data.BusinessDetail);


                #region gắn hàm upload file
                var Files = Request.Form.Files;
                var LstFile = new List<TradePromotionProjectManagementAttachFileModel>();
                foreach (var f in Files)
                {
                    if (f.Length > 0)
                    {
                        using (var ms = new MemoryStream())
                        {
                            f.CopyTo(ms);
                            upLoadFileModel up = new upLoadFileModel()
                            {
                                bs = ms.ToArray(),
                                FileName = f.FileName.Replace(" ", ""),
                                Linkfile = "TradePromotionProjectManagement"
                            };
                            var result = Ulities.UploadFile(up, _config);

                            TradePromotionProjectManagementAttachFileModel fileSave = new TradePromotionProjectManagementAttachFileModel();
                            fileSave.LinkFile = result.link;
                            LstFile.Add(fileSave);
                        }
                    }
                }
                data.Details = LstFile;
                #endregion
                data.TimeStart = Ulities.ConvertTimeZone(HttpContext.Request.Headers, JsonConvert.DeserializeObject<DateTime>(data.TimeStartGet));
                data.TimeEnd = data.TimeEndGet != "null" ? Ulities.ConvertTimeZone(HttpContext.Request.Headers, JsonConvert.DeserializeObject<DateTime>(data.TimeEndGet)) : null;
                data.CreateTime = DateTime.Now;
                data.CreateUserId = loginData.Userid;

                await _repoTradePromotionProjectManagement.Insert(data);
                datalog = Ulities.WriteLog(_config, HttpContext, loginData.Username, ActionType_Const.CREATE, NameCategory_Const.TRADE_PROMOTION_PROJECT_MANAGEMENT, Action_Status.SUCCESS);
                _asyncLogger.LogInfo(JsonConvert.SerializeObject(datalog));
                //await _repoTradePromotionProjectManagement.Insert(SaveData);
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

        [HttpPut("deleteTradePromotionProjectManagement/{id}")]
        public async Task<IActionResult> deleteTradePromotionProjectManagement(Guid id)
        {
            BaseModels<object> model = new BaseModels<object>();
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                SystemLog datalog = new SystemLog();
                await _repoTradePromotionProjectManagement.Delete(id, _config);
                datalog = Ulities.WriteLog(_config, HttpContext, loginData.Username, ActionType_Const.DELETE, NameCategory_Const.TRADE_PROMOTION_PROJECT_MANAGEMENT, Action_Status.SUCCESS);
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

        [Route("deleteTradePromotionProjectManagements")]
        [HttpPut()]
        public async Task<IActionResult> deleteTradePromotionProjectManagements(removeListTradePromotionProjectManagementItems data)
        {
            BaseModels<TradePromotionProjectManagement> model = new BaseModels<TradePromotionProjectManagement>();
            try
            {
                foreach (Guid id in data.TradePromotionProjectManagementIds)
                {
                    TradePromotionProjectManagement DeleteData = new TradePromotionProjectManagement();
                    DeleteData.TradePromotionProjectManagementId = id;
                    DeleteData.IsDel = true;
                    await _repoTradePromotionProjectManagement.DeleteTradePromotionProjectManagement(DeleteData);
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



        [Route("loadCurrencyUnit")]
        [HttpGet]
        public IActionResult LoadCurrencyUnit()
        {
            BaseModels<Unit> unit_model = new BaseModels<Unit>();

            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }

                IQueryable<Unit> _data = _repoUnit._context.Units.Where(x => x.UnitCode == "CUR" && !x.IsDel).Select(x => new Unit
                {
                    UnitId = x.UnitId,
                    UnitName = x.UnitName,
                    UnitNameEn = x.UnitNameEn
                });

                unit_model.status = 1;
                unit_model.items = _data.ToList();
                return Ok(unit_model);

            }
            catch (Exception ex)
            {
                unit_model.status = 0;
                unit_model.error = new ErrorModel()
                {
                    Code = ErrCode_Const.EXCEPTION_API,
                    Msg = ex.Message
                };
                return BadRequest(unit_model);
            }
        }

        [Route("loadUser")]
        [HttpGet]
        public IActionResult LoadUser()
        {
            BaseModels<User> user_model = new BaseModels<User>();

            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }

                IQueryable<User> _data = _repoUser._context.Users.Where(x => x.IsDel == false).Select(x => new User
                {
                    UserId = x.UserId,
                    FullName = x.FullName,
                });

                user_model.status = 1;
                user_model.items = _data.ToList();
                return Ok(user_model);

            }
            catch (Exception ex)
            {
                user_model.status = 0;
                user_model.error = new ErrorModel()
                {
                    Code = ErrCode_Const.EXCEPTION_API,
                    Msg = ex.Message
                };
                return BadRequest(user_model);
            }
        }
    }
}
