using API_SoCongThuong.Classes;
using API_SoCongThuong.Logger;
using API_SoCongThuong.Models;
using API_SoCongThuong.Reponsitories.UserRepository;
using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Office2010.Excel;
using EF_Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using StackExchange.Redis;
using System.Data;
using System.Reactive.Concurrency;
using static API_SoCongThuong.Models.CreateUser;

namespace API_SoCongThuong.Controllers.Identity
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private UserRepo _repo;
        private IConfiguration _configuration;
        public SoHoa_SoCongThuongContext _context;
        private readonly ILogger<AsyncLogger> _logger;
        private AsyncLogger _asyncLogger;

        public UserController(SoHoa_SoCongThuongContext context, IConfiguration configuration, IConnectionMultiplexer redisCache, ILogger<AsyncLogger> logger)
        {
            _repo = new UserRepo(context, configuration, redisCache);
            _configuration = configuration;
            _context = context;
            _logger = logger;
            _asyncLogger = new AsyncLogger(_logger, _context);
        }

        [Route("login")]
        [HttpPost]
        public async Task<IActionResult> Login([FromBody] AuthenticateRequest _user)
        {
            var ipAddress = Ulities.GetIPAddress(HttpContext);

            BaseModels<object> model = new BaseModels<object>();
            try
            {
                var response = await _repo.Authenticate(_user);

                if (response == null)
                {
                    var userLoginFail = _repo._context.Users.FirstOrDefault(x => x.IsDel == false && x.UserName == _user.UserName);
                    if (userLoginFail == null)
                    {
                        return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.USER_NOT_FOUND));
                    }

                    LoginFailedModel FailModel = new LoginFailedModel()
                    {
                        CountLoginFail = userLoginFail.CountLoginFail,
                        TimeLock = userLoginFail.TimeLock,
                        Error = ErrMsg_Const.GetMsg(ErrCode_Const.USER_NOT_FOUND)
                    };

                    var loginfaillog = new SystemLog()
                    {
                        ApplicationCode = _configuration.GetValue<string>("ApplicationCode"),
                        ServiceCode = _configuration.GetValue<string>("ModuleCode"),
                        SessionId = "",
                        IpPortParentNode = "",
                        IpPortCurrentNode = ipAddress,
                        ReponseConent = "",
                        RequestConent = "",
                        StartTime = DateTime.Now,
                        EndTime = DateTime.Now.AddDays(7),
                        Duration = 0,
                        ErrorCode = 0,
                        ErrorDescription = "",
                        TransactionStatus = "",
                        ActionType = ActionType_Const.LOGIN,
                        ActionName = "Đăng nhập",
                        UserName = _user.UserName,
                        Account = "",
                        ContentLog = "Đăng nhập thất bại"
                    };
                    _asyncLogger.LogInfo(JsonConvert.SerializeObject(loginfaillog));

                    return BadRequest(FailModel);
                }

                await _repo.setCurrentRefreshToken(response.uuid, _user.UserName);
                await _repo.addToWhitelist(response.uuid, response.RefreshToken);

                var dtlog = new SystemLog()
                {
                    ApplicationCode = _configuration.GetValue<string>("ApplicationCode"),
                    ServiceCode = _configuration.GetValue<string>("ModuleCode"),
                    SessionId = response.uuid,
                    IpPortParentNode = "",
                    IpPortCurrentNode = ipAddress,
                    ReponseConent = "",
                    RequestConent = "",
                    StartTime = DateTime.Now,
                    EndTime = DateTime.Now.AddDays(7),
                    Duration = 0,
                    ErrorCode = 0,
                    ErrorDescription = "",
                    TransactionStatus = "",
                    ActionType = ActionType_Const.LOGIN,
                    ActionName = "Đăng nhập",
                    UserName = _user.UserName,
                    Account = "",
                    ContentLog = "Đăng nhập thành công"
                };
                _asyncLogger.LogInfo(JsonConvert.SerializeObject(dtlog));

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("refresh")]
        [HttpPost]
        public IActionResult Rrefresh([FromBody] AuthenticateRequest _user)
        {
            return Ok();
        }

        [Route("changePassword")]
        [HttpPost]
        public IActionResult ChangePassword([FromBody] AuthenticateRequest _user)
        {
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> create([FromBody] CreateUser _user)
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
                var us = await _repo.findByUsername(_user.Username);
                if (us != null)
                {
                    model.status = 0;
                    model.error = new ErrorModel()
                    {
                        Code = ErrCode_Const.EXCEPTION_API,
                        Msg = "Username đã tồn tại"
                    };
                    datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.CREATE, NameCategory_Const.USER, Action_Status.FAIL);
                    _asyncLogger.LogInfo(JsonConvert.SerializeObject(datalog));
                    return BadRequest(model);
                }

                var count = _repo.FindAll().Count();
                var data = new User
                {
                    UserName = _user.Username.Trim(),
                    FullName = _user.Fullname.Trim(),
                    Password = DpsLibs.Common.EncDec.Encrypt(_user.PassWord, _configuration.GetValue<string>("PASSWORD_ED")),
                    RoleId = _user.RoleId,
                    DeptId = _user.DeptId,
                    GroupUserId = _user.GroupUserId,
                    Phone = _user.Phone,
                    Email = _user.Email,
                    Cccd = _user.CCCD,
                    LevelUser = _user.leveluser,
                    AreaId = _user.areaId,

                    Status = 0,
                    CountLoginFail = 0,
                    TimeLock = null,
                    CreateUserId = loginData.Userid,
                };

                await _repo.Insert(data);
                datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.CREATE, NameCategory_Const.USER, Action_Status.SUCCESS);
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
        public async Task<IActionResult> Update(DefaultUser _user)
        {
            BaseModels<Group> model = new BaseModels<Group>();
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                User test = new User()
                {
                    UserName = _user.Username.Trim(),
                    FullName = _user.Fullname.Trim(),
                    //Password = DpsLibs.Common.EncDec.Encrypt(_user.PassWord, _configuration.GetValue<string>("PASSWORD_ED")),
                    UserId = _user.UserId,
                    RoleId = _user.RoleId,
                    DeptId = _user.DeptId,
                    GroupUserId = _user.GroupUserId,
                    Phone = _user.Phone,
                    Email = _user.Email,
                    Cccd = _user.CCCD,
                    LevelUser = _user.levelUser,
                    AreaId = _user.areaId,
                    UpdateUserId = loginData.Userid,
                    Status = _user.Status,
                    IsDel = _user.IsDel,
                    CountLoginFail = _user.CountLoginFail,
                    TimeLock = _user.TimeLock,
                };
                await _repo.Update(test);
                SystemLog datalog = new SystemLog();
                if (_user.IsDel == true)
                {
                    datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.DELETE, NameCategory_Const.USER, Action_Status.SUCCESS);
                    _asyncLogger.LogInfo(JsonConvert.SerializeObject(datalog));
                }
                else
                {
                    datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.UPDATE, NameCategory_Const.USER, Action_Status.SUCCESS);
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

        #region 
        [Route("find")]
        [HttpPost]
        public IActionResult ListItems_New([FromBody] QueryRequestBody query)//query truyền lên
        {

            BaseModels<DefaultUser> model = new BaseModels<DefaultUser>();
            string _keywordSearch = ""; //Keyword tìm kiếm
            bool _orderBy_ASC = false;  //Khởi tạo sắp xếp dữ liệu acs hoặc desc khi tìm kiếm
            try
            {
                //Lấy Token, lấy model
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }

                Func<DefaultUser, object> _orderByExpression = x => x.Fullname; //Khởi tạo mặc định sắp xếp dữ liệu
                Dictionary<string, Func<DefaultUser, object>> _sortableFields = new Dictionary<string, Func<DefaultUser, object>>   //Khởi tạo các trường để sắp xếp
                {
                    { "Username", x => x.Username },
                    { "Fullname", x => x.Fullname },
                    { "Status", x => x.Status },
                    { "GroupUserName", x => x.GroupUserName },
                    { "DeptName", x => x.DeptName }
                };
                if (query.Sort != null
                    && !string.IsNullOrEmpty(query.Sort.ColumnName)
                    && _sortableFields.ContainsKey(query.Sort.ColumnName))
                {
                    _orderBy_ASC = ("desc".Equals(query.Sort.Direction.ToLower()) ? false : true);    //Sắp xếp asc hoặc desc
                    _orderByExpression = _sortableFields[query.Sort.ColumnName]; //Trường cần sắp xếp
                }

                IQueryable<DefaultUser> _data = from u in _repo._context.Users
                                                where u.IsDel == false
                                                join ps in _repo._context.StateTitles on u.RoleId equals ps.StateTitlesId
                                                join dept in _repo._context.StateUnits on u.DeptId equals dept.StateUnitsId
                                                join g in _repo._context.Groups on u.GroupUserId equals g.GroupId
                                                select new DefaultUser
                                                {
                                                    UserId = u.UserId,
                                                    Username = u.UserName,
                                                    Fullname = u.FullName,
                                                    PassWord = "baomat",
                                                    DeptId = (Guid)u.DeptId,
                                                    RoleId = (Guid)u.RoleId,
                                                    Phone = u.Phone,
                                                    //Avatar = u.Avatar,
                                                    Email = u.Email,
                                                    CCCD = u.Cccd,
                                                    GroupUserId = (Guid)u.GroupUserId,
                                                    DeptName = dept.StateUnitsName,
                                                    RoleName = ps.StateTitlesName,
                                                    GroupUserName = g.GroupName,
                                                    Status = (int)u.Status,
                                                    IsDel = u.IsDel
                                                };


                if (query.SearchValue != null && query.SearchValue != "") //Kiểm tra điều kiện tìm kiếm
                {
                    _keywordSearch = query.SearchValue.Trim().ToLower();
                    _data = _data.Where(x =>
                       /* x.DistrictId.ToString().ToLower().Contains(_keywordSearch)
                        || */
                       x.Fullname.ToLower().Contains(_keywordSearch) || x.Username.ToLower().Contains(_keywordSearch)
                   );  //Lấy table đã select tìm kiếm theo keyword
                }
                if (query.Filter != null && query.Filter.ContainsKey("idGroupParent") && !string.IsNullOrEmpty(query.Filter["idGroupParent"]))
                {
                    _data = _data.Where(x => x.UserId.ToString().Contains(string.Join("", query.Filter["idGroupParent"])));
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
        #endregion

        [HttpGet("{id}")]
        public IActionResult getItemById(Guid id)
        {
            BaseModels<DefaultUser> model = new BaseModels<DefaultUser>();
            try
            {
                IQueryable<DefaultUser> result = from u in _repo._context.Users
                                                 where u.UserId.Equals(id)
                                                 join ps in _repo._context.StateTitles on u.RoleId equals ps.StateTitlesId
                                                 join dept in _repo._context.StateUnits on u.DeptId equals dept.StateUnitsId
                                                 join g in _repo._context.Groups on u.GroupUserId equals g.GroupId
                                                 select new DefaultUser
                                                 {
                                                     UserId = u.UserId,
                                                     Username = u.UserName,
                                                     Fullname = u.FullName,
                                                     PassWord = "Baomat@123",
                                                     DeptId = (Guid)u.DeptId,
                                                     RoleId = (Guid)u.RoleId,
                                                     Phone = u.Phone,
                                                     Email = u.Email,
                                                     CCCD = u.Cccd,
                                                     GroupUserId = (Guid)u.GroupUserId,
                                                     DeptName = dept.StateUnitsName,
                                                     RoleName = ps.StateTitlesName,
                                                     GroupUserName = g.GroupName,
                                                     levelUser = u.LevelUser,
                                                     areaId = u.AreaId,
                                                     Status = (int)u.Status,
                                                     IsDel = u.IsDel
                                                 };
                if (result == null)
                    return NotFound(ErrMsg_Const.GetMsg(ErrCode_Const.CANNOT_FIND_DATA_BY_QUERY));

                model.status = 1;
                model.items = result.ToList();
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

        [Authorize]
        [Route("getUserByToken")]
        [HttpPost]
        public async Task<object> getUserByToken()
        {
            UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
            if (loginData == null)
            {
                return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
            }

            var refreshToken = await _repo.getFromWhitelist(loginData.Uuid);
            if (string.IsNullOrEmpty(refreshToken))
            {
                return Unauthorized(null);
            }
            BaseModels<object> model = new BaseModels<object>();
            model.error = new ErrorModel();
            model.data = loginData;
            model.status = model.data == null ? 0 : 1;
            model.error.Msg = model.status == 1 ? "" : "Thất bại";
            return model;
        }

        [Route("getMenu")]
        [HttpPost]
        public object getMenu()
        {
            BaseModels<object> model = new BaseModels<object>();
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                List<SubMenu> _submenu = new List<SubMenu>();
                List<MainMenu> _mainmenu = new List<MainMenu>();
                List<Module> _module = new List<Module>();
                DataTable _moduleTB;
                DataTable _mainmenuTB;
                DataTable _submenuTB;
                List<string> roles = loginData.Roles.Split(",").ToList();
                // Lấysubmenu
                _submenu = (from sub in _repo._context.SubMenus
                            where roles.Contains(sub.AllowPermit)
                            select sub
                                ).ToList();

                // Lấy mainmenu
                _mainmenu = (from mm in _repo._context.MainMenus
                             where roles.Contains(mm.AllowPermit)
                             //where _submenu.Select(x => x.IdMainMenu).Contains(mm.IdMain.ToString())
                             select mm
                               ).ToList();

                //Lấy module
                //_module = (from md in _repo._context.Modules
                //               //where _config._menu_ids.Count() > 0 ? _config._menu_ids.Contains(md.IdModule) : true
                //               //where _mainmenu.Select(x => x.IdModule).Contains(md.IdModule) && loginData.Roles.Contains(md.AllowPermit
                //           where loginData.Roles.Contains(md.AllowPermit)
                //           select md ).ToList();

                //_moduleTB = _repo.ConvertToDataTable(_module);
                _mainmenuTB = _repo.ConvertToDataTable(_mainmenu);
                _submenuTB = _repo.ConvertToDataTable(_submenu);

                if (_mainmenuTB == null || _submenu == null)
                {
                    model.status = 0;
                    model.data = false;
                    return model;
                }

                //var data = from md in _moduleTB.Select()
                //           orderby md["Position"]
                //           select new
                //           {
                //               IdModule = md["IdModule"],
                //               Title = md["Title"].ToString(),
                //               Href = md["Link"].ToString(),
                //               Position = md["Position"],
                //               Icon = md["Icon"].ToString(),
                //               Target = md["Target"].ToString(),
                //               Open = false,
                //               Child = from r in _mainmenuTB.Select()
                //                           where md["IdModule"].ToString().Equals(r["IdModule"].ToString())
                //                           orderby r["Position"]
                //                           select new
                //                           {
                //                               IdMain = r["IdMain"],
                //                               Title = r["Title"].ToString(),
                //                               Summary = r["Summary"].ToString(),
                //                               Href = r["Link"].ToString(),
                //                               Position = r["Position"],
                //                               Icon = r["Icon"].ToString(),
                //                               GroupName = r["GroupName"].ToString(),
                //                               Open = false,
                //                               Target = r["Target"].ToString(),
                //                               Child = from sm in _submenuTB.Select()
                //                                           where sm["IdMainMenu"].ToString().Equals(r["IdMain"].ToString())
                //                                           orderby sm["Position"]
                //                                           select new
                //                                           {
                //                                               IdSub = sm["IdSub"],
                //                                               Title = sm["Title"].ToString(),
                //                                               Summary = sm["Summary"].ToString(),
                //                                               Href = sm["Link"].ToString(),
                //                                               PageKey = sm["PageKey"].ToString(),
                //                                               Position = sm["Position"],
                //                                               Icon = sm["Icon"].ToString(),
                //                                               Open = false,
                //                                               Target = sm["Target"].ToString(),
                //                                           }

                //                           }
                //           };
                var data = from r in _mainmenuTB.Select()
                           orderby r["Position"]
                           select new
                           {
                               IdMain = r["IdMain"],
                               Title = r["Title"].ToString(),
                               Summary = r["Summary"].ToString(),
                               Href = r["Link"].ToString(),
                               Position = r["Position"],
                               Icon = r["Icon"].ToString(),
                               GroupName = r["GroupName"].ToString(),
                               Open = false,
                               Target = r["Target"].ToString(),
                               Child = from sm in _submenuTB.Select()
                                       where sm["Target"] == DBNull.Value && sm["IdMainMenu"].ToString().Equals(r["IdMain"].ToString())
                                       orderby sm["Position"]
                                       select new
                                       {
                                           IdSub = sm["IdSub"],
                                           Title = sm["Title"].ToString(),
                                           Summary = sm["Summary"].ToString(),
                                           Href = sm["Link"].ToString(),
                                           PageKey = sm["PageKey"].ToString(),
                                           Position = sm["Position"],
                                           Icon = sm["Icon"].ToString(),
                                           Open = false,
                                           Target = sm["Target"].ToString(),
                                           Child = from scm in _submenuTB.Select()
                                                   where scm["Target"] != DBNull.Value && scm["Target"].ToString().Equals(sm["IdSub"].ToString())
                                                   orderby scm["Position"]
                                                   select new
                                                   {
                                                       IdSub = scm["IdSub"],
                                                       Title = scm["Title"].ToString(),
                                                       Summary = scm["Summary"].ToString(),
                                                       Href = scm["Link"].ToString(),
                                                       PageKey = scm["PageKey"].ToString(),
                                                       Position = scm["Position"],
                                                       Icon = scm["Icon"].ToString(),
                                                       Open = false,
                                                       Target = scm["Target"].ToString(),
                                                   }
                                       }

                           };
                model.data = data;
                model.status = 1;
                return model;
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        #region Danh sách nhóm người dùng
        [Route("group-user")]
        [HttpGet]
        public IActionResult loadNhomNguoiDung()
        {
            BaseModels<Group> model = new BaseModels<Group>();

            try
            {
                //Lấy Token, lấy model
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                IQueryable<Group> _data = _repo._context.Groups.Where(x => x.Status == 0).Select(x => new Group
                {
                    GroupId = x.GroupId,
                    GroupName = x.GroupName,
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
        #endregion

        [Route("change-password-user")]
        [HttpPost]
        [CusAuthorize(Roles = "3")]
        public IActionResult ChangePasswordUser([FromBody] AuthenticateRequest _user)
        {
            BaseModels<object> model = new BaseModels<object>();
            try
            {
                //Lấy Token
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }

                _repo.UpdatePassword(_user);
                model.status = 1;
                model.total = 1;
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

        [Route("change-password-user-current")]
        [HttpPost]
        [CusAuthorize(Roles = "3")]
        //public IActionResult ChangePasswordUserCurrent([FromBody] ChangePasswordModel changPasswordModel)
        //{
        //    BaseModels<object> model = new BaseModels<object>();
        //    try
        //    {
        //        //Lấy Token
        //        UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
        //        if (loginData == null)
        //        {
        //            return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
        //        }
        //        if (!changPasswordModel.MatKhau.Equals(changPasswordModel.NhapLaiMatKhau))
        //            return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.SQL_INSERT_FAILED));
        //        if (changPasswordModel.MatKhau.Trim().Length < 8)
        //            return BadRequest("Độ dài mật khẩu không đúng quy định");
        //        var userDB = _context.Users.Where(t => t.UserId == loginData.Userid).FirstOrDefault();
        //        if (userDB == null)
        //            return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.SQL_INSERT_FAILED));
        //        if (!changPasswordModel.MatKhauHienTai.Equals(userDB.Password))
        //            return BadRequest("Mật khẩu hiện tại không chính xác");

        //       _repo.UpdatePassword(changPasswordModel);

        //        model.status = 1;
        //        model.total = 1;
        //        return Ok(model);

        //        //_repo.UpdatePassword(_user);
        //        //return Ok();
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
        public async Task<IActionResult> ChangePasswordUserCurrent([FromBody] ChangePasswordModel changePasswordModel)
        {
            BaseModels<object> model = new BaseModels<object>();
            try
            {
                //Lấy Token
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }

                changePasswordModel = _repo.CheckPasswordCurrent(changePasswordModel);
                var userDB = _context.Users.Where(t => t.UserId == loginData.Userid).FirstOrDefault();

                if (userDB == null)
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.SQL_INSERT_FAILED));
                if (!changePasswordModel.PassWordCurrent.Equals(userDB.Password))
                    return BadRequest("Mật khẩu hiện tại không chính xác");

                if (!changePasswordModel.PassWord.Equals(changePasswordModel.ConfirmPassword))
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.SQL_INSERT_FAILED));

                if (changePasswordModel.PassWord.Trim().Length < 8)
                    return BadRequest("Độ dài mật khẩu không đúng quy định");

                await _repo.UpdatePasswords(changePasswordModel);
                model.status = 1;
                model.total = 1;
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

        [HttpGet("loadtree")]
        public IActionResult LoadTree()
        {
            BaseModels<TreeUserModel> model = new BaseModels<TreeUserModel>();
            try
            {
                //Lấy Token, lấy model
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }

                //Query lấy data
                List<TreeUserModel> _data = _repo.GetUserTree();

                model.status = 1;
                model.items = _data;

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

        [Route("logout")]
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            var ipAddress = Ulities.GetIPAddress(HttpContext);

            BaseModels<object> model = new BaseModels<object>();
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }

                var uuids = loginData.Uuid;
                await _repo.removeFromWhitelist(uuids);
                SystemLog datalog = new SystemLog();

                //data log
                datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.LOGOUT, "Đăng xuất", Action_Status.SUCCESS);
                _asyncLogger.LogInfo(JsonConvert.SerializeObject(datalog));
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
