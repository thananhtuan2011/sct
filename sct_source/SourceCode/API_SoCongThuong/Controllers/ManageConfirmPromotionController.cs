
using API_SoCongThuong.Classes;
using API_SoCongThuong.Models;
using API_SoCongThuong.Reponsitories.ManageConfirmPromotionRepository;
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
using System.Data;

namespace API_SoCongThuong.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ManageConfirmPromotionController : ControllerBase
    {
        private ManageConfirmPromotionRepo _repoManageConfirmPromotion;
        private UserRepo _repoUser;
        private IConfiguration _config;
        private IConnectionMultiplexer redisCache;

        private readonly ILogger<AsyncLogger> _logger;
        public SoHoa_SoCongThuongContext _context;
        private AsyncLogger _asyncLogger;

        public ManageConfirmPromotionController(SoHoa_SoCongThuongContext context, IConfiguration configuration, IConnectionMultiplexer redisCache, ILogger<AsyncLogger> logger)
        {
            _repoManageConfirmPromotion = new ManageConfirmPromotionRepo(context);
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

            BaseModels<ManageConfirmPromotionModel> model = new BaseModels<ManageConfirmPromotionModel>();
            string _keywordSearch = ""; //Keyword tìm kiếm
            bool _orderBy_ASC = true;  //Khởi tạo sắp xếp dữ liệu acs hoặc desc khi tìm kiếm
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }

                Func<ManageConfirmPromotionModel, object> _orderByExpression = x => x.ManageConfirmPromotionName; //Khởi tạo mặc định sắp xếp dữ liệu
                Dictionary<string, Func<ManageConfirmPromotionModel, object>> _sortableFields = new Dictionary<string, Func<ManageConfirmPromotionModel, object>>   //Khởi tạo các trường để sắp xếp
                {
                    { "ManageConfirmPromotionName", x => x.ManageConfirmPromotionName },
                    { "GoodsServices", x => x.GoodsServices },
                    { "GoodsServicesPay", x => x.GoodsServicesPay }
                };
                if (query.Sort != null
                    && !string.IsNullOrEmpty(query.Sort.ColumnName)
                    && _sortableFields.ContainsKey(query.Sort.ColumnName))
                {
                    _orderBy_ASC = ("desc".Equals(query.Sort.Direction.ToLower()) ? false : true);    //Sắp xếp asc hoặc desc
                    _orderByExpression = _sortableFields[query.Sort.ColumnName]; //Trường cần sắp xếp
                }
                //Cách 1 dùng entity
                IQueryable<ManageConfirmPromotionModel> _data = _repoManageConfirmPromotion._context.ManageConfirmPromotions.Select(x => new ManageConfirmPromotionModel
                {
                    ManageConfirmPromotionId = x.ManageConfirmPromotionId,
                    ManageConfirmPromotionName = x.ManageConfirmPromotionName ?? "",
                    GoodsServices = x.GoodsServices,
                    GoodsServicesPay = x.GoodsServicesPay ?? "",
                    TimeStart = x.TimeStart,
                    TimeEnd = x.TimeEnd,
                    NumberOfDocuments = x.NumberOfDocuments,
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
                       /* x.ManageConfirmPromotionId.ToString().ToLower().Contains(_keywordSearch)
                        || */
                       x.ManageConfirmPromotionName.ToLower().Contains(_keywordSearch) || x.GoodsServices.ToLower().Contains(_keywordSearch) || x.GoodsServicesPay.ToLower().Contains(_keywordSearch)
                   );  //Lấy table đã select tìm kiếm theo keyword
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
            BaseModels<ManageConfirmPromotionModel> model = new BaseModels<ManageConfirmPromotionModel>();
            try
            {
                var result = _repoManageConfirmPromotion.FindById(id, _config);
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
        public async Task<IActionResult> Update([FromForm] ManageConfirmPromotionModel data)
        {
            BaseModels<ManageConfirmPromotionModel> model = new BaseModels<ManageConfirmPromotionModel>();
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                SystemLog datalog = new SystemLog();
                var CheckData = _repoManageConfirmPromotion.FindById(data.ManageConfirmPromotionId, _config);
                if (CheckData == null)
                {
                    //chỗ này không tồn tại id
                    model.status = 0;
                    model.error = new ErrorModel()
                    {
                        Code = ErrCode_Const.PROPERTY_IS_NULL_OR_EMPTY
                    };
                    datalog = Ulities.WriteLog(_config, HttpContext, loginData.Username, ActionType_Const.UPDATE, NameCategory_Const.MANAGE_CONFIRM_PROMOTION, Action_Status.FAIL);
                    _asyncLogger.LogInfo(JsonConvert.SerializeObject(datalog));
                    return BadRequest(model);
                }
                else
                {
                    #region gắn hàm upload file
                    var Files = Request.Form.Files;
                    var LstFile = new List<ManageConfirmPromotionAttachFileModel>();
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
                                    Linkfile = "ManageConfirmPromotion"
                                };
                                var result = Ulities.UploadFile(up, _config);

                                ManageConfirmPromotionAttachFileModel fileSave = new ManageConfirmPromotionAttachFileModel();
                                fileSave.LinkFile = result.link;
                                LstFile.Add(fileSave);
                            }
                        }
                    }
                    data.Details = LstFile;
                    #endregion
                    data.TimeStart = Ulities.ConvertTimeZone(HttpContext.Request.Headers, JsonConvert.DeserializeObject<DateTime>(data.TimeStartGet));
                    data.TimeEnd = Ulities.ConvertTimeZone(HttpContext.Request.Headers, JsonConvert.DeserializeObject<DateTime>(data.TimeEndGet));
                    data.UpdateTime = DateTime.Now;
                    data.UpdateUserId = loginData.Userid;
                    await _repoManageConfirmPromotion.Update(data, _config);
                    datalog = Ulities.WriteLog(_config, HttpContext, loginData.Username, ActionType_Const.UPDATE, NameCategory_Const.MANAGE_CONFIRM_PROMOTION, Action_Status.SUCCESS);
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
        public async Task<IActionResult> create([FromForm] ManageConfirmPromotionModel data)
        {
            BaseModels<ManageConfirmPromotion> model = new BaseModels<ManageConfirmPromotion>();
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                SystemLog datalog = new SystemLog();
                #region gắn hàm upload file
                var Files = Request.Form.Files;
                var LstFile = new List<ManageConfirmPromotionAttachFileModel>();
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
                                Linkfile = "ManageConfirmPromotion"
                            };
                            var result = Ulities.UploadFile(up, _config);

                            ManageConfirmPromotionAttachFileModel fileSave = new ManageConfirmPromotionAttachFileModel();
                            fileSave.LinkFile = result.link;
                            LstFile.Add(fileSave);
                        }
                    }
                }
                data.Details = LstFile;
                #endregion
                data.TimeStart = Ulities.ConvertTimeZone(HttpContext.Request.Headers, JsonConvert.DeserializeObject<DateTime>(data.TimeStartGet));
                data.TimeEnd = Ulities.ConvertTimeZone(HttpContext.Request.Headers, JsonConvert.DeserializeObject<DateTime>(data.TimeEndGet));
                data.CreateTime = DateTime.Now;
                data.CreateUserId = loginData.Userid;

                await _repoManageConfirmPromotion.Insert(data);
                datalog = Ulities.WriteLog(_config, HttpContext, loginData.Username, ActionType_Const.CREATE, NameCategory_Const.MANAGE_CONFIRM_PROMOTION, Action_Status.SUCCESS);
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

        [HttpPut("deleteManageConfirmPromotion/{id}")]
        public async Task<IActionResult> deleteManageConfirmPromotion(Guid id)
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
                await _repoManageConfirmPromotion.Delete(id, _config);
                datalog = Ulities.WriteLog(_config, HttpContext, loginData.Username, ActionType_Const.DELETE, NameCategory_Const.MANAGE_CONFIRM_PROMOTION, Action_Status.SUCCESS);
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
