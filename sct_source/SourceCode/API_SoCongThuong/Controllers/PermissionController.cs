using API_SoCongThuong.Classes;
using API_SoCongThuong.Logger;
using API_SoCongThuong.Models;
using API_SoCongThuong.Models.PermissionModel;
using API_SoCongThuong.Models.TestModel;
using API_SoCongThuong.Reponsitories.PermissionRepository;
using API_SoCongThuong.Reponsitories.UserRepository;
using EF_Core.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace API_SoCongThuong.Controllers.Permission
{
    [Route("api/[controller]")]
    [ApiController]
    public class PermissionController : ControllerBase
    {
        private PermissionRepo _repo;
        private UserRepo _repoUser;

        private IConfiguration _configuration;
        private readonly ILogger<AsyncLogger> _logger;
        public SoHoa_SoCongThuongContext _context;
        private AsyncLogger _asyncLogger;
        public PermissionController(SoHoa_SoCongThuongContext context, IConfiguration configuration, IConnectionMultiplexer redisCache, ILogger<AsyncLogger> logger)
        {
            _repo = new PermissionRepo(context);
            _repoUser = new UserRepo(context, configuration, redisCache);

            _logger = logger;
            _context = context;
            _asyncLogger = new AsyncLogger(_logger, _context);
            _configuration = configuration;
        }

        #region Danh sách test
        /// <summary>
        /// Lấy danh sách thông tin test
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [Route("find")]
        [HttpPost]
        public IActionResult ListItems_New([FromBody] QueryRequestBody query)//query truyền lên
        {

            BaseModels<PermissionModel> model = new BaseModels<PermissionModel>();
            string _keywordSearch = ""; //Keyword tìm kiếm
            bool _orderBy_ASC = true;  //Khởi tạo sắp xếp dữ liệu acs hoặc desc khi tìm kiếm
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }

                Func<PermissionModel, object> _orderByExpression = x => x.Priority; //Khởi tạo mặc định sắp xếp dữ liệu
                Dictionary<string, Func<PermissionModel, object>> _sortableFields = new Dictionary<string, Func<PermissionModel, object>>   //Khởi tạo các trường để sắp xếp
                {
                    { "GroupId", x => x.GroupId },
                    { "GroupName", x => x.GroupName },
                    { "Status", x => x.Status },
                    { "Priority", x => x.Priority }
                };
                if (query.Sort != null && !string.IsNullOrEmpty(query.Sort.ColumnName) && _sortableFields.ContainsKey(query.Sort.ColumnName))
                {
                    _orderBy_ASC = ("desc".Equals(query.Sort.Direction.ToLower()) ? false : true);    //Sắp xếp asc hoặc desc
                    _orderByExpression = _sortableFields[query.Sort.ColumnName]; //Trường cần sắp xếp
                }
                //Cách 1 dùng entity
                IQueryable<PermissionModel> _data = _repo._context.Groups.Select(x => new PermissionModel
                {
                    GroupId = x.GroupId,
                    GroupName = x.GroupName ?? "",
                    Status = (int)x.Status,
                    Priority = (int)x.Priority,
                    CountGroupUser = _repo._context.Users.Where(t => t.GroupUserId.Equals(x.GroupId) && t.IsDel == false).Count(),
                    IsAdmin = x.GroupName == "Quản trị hệ thống" ? true : false
                }); ;
                if (query.SearchValue != null && query.SearchValue != "") //Kiểm tra điều kiện tìm kiếm
                {
                    _keywordSearch = query.SearchValue.Trim().ToLower();
                    _data = _data.Where(x =>
                       x.Priority.ToString().Contains(_keywordSearch)
                       || x.GroupName.ToLower().Contains(_keywordSearch)
                   );  //Lấy table đã select tìm kiếm theo keyword
                }
                if (query.Filter != null && query.Filter.ContainsKey("GroupId") && !string.IsNullOrEmpty(query.Filter["GroupId"]))
                {
                    _data = _data.Where(x => x.GroupId.ToString().Contains(string.Join("", query.Filter["GroupId"])));
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
            BaseModels<Group> model = new BaseModels<Group>();
            try
            {
                var result = _repo.FindById(id);
                if (result.ToList().Count() == 0)
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


        [HttpPut("{id}")]
        public async Task<IActionResult> Update(PermissionModel data)
        {
            BaseModels<Group> model = new BaseModels<Group>();
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                Group test = new Group();
                test.GroupId = data.GroupId;
                test.GroupName = data.GroupName;
                test.Priority = data.Priority;
                test.Status = data.Status;
                await _repo.Update(test);
                SystemLog datalog = new SystemLog();
                datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.UPDATE, NameCategory_Const.GROUP_USER, Action_Status.SUCCESS);
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
        [HttpPost()]
        public async Task<IActionResult> create(PermissionModel data)
        {
            BaseModels<Group> model = new BaseModels<Group>();
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                var dt = _repo.FindByName(data.GroupName);
                
                SystemLog datalog = new SystemLog();
                if (dt.ToList().Count() > 0)
                {
                    model.status = 0;
                    model.error = new ErrorModel()
                    {
                        Code = ErrCode_Const.EXCEPTION_API,
                        Msg = "Tên nhóm đã tồn tại"
                    };
                    datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.CREATE, NameCategory_Const.GROUP_USER, Action_Status.FAIL);
                    _asyncLogger.LogInfo(JsonConvert.SerializeObject(datalog));
                    return BadRequest(model);
                }
                Group test = new Group();
                test.GroupName = data.GroupName;
                test.Priority = data.Priority;
                await _repo.Insert(test);
                datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.CREATE, NameCategory_Const.GROUP_USER, Action_Status.SUCCESS);
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

        [HttpDelete("{id}")]
        public async Task<IActionResult> delete(Guid id)
        {
            BaseModels<TblTest> model = new BaseModels<TblTest>();
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }

                SystemLog datalog = new SystemLog();
                var count = _repo._context.Users.Where(t => t.GroupUserId.Equals(id) && t.IsDel == false).Count();
                if (count > 0)
                {
                    model.status = 0;
                    model.error = new ErrorModel()
                    {
                        Code = ErrCode_Const.SQL_QUERY_ERROR_RETURN_TBL,
                        Msg = "Đang tồn tại user trong nhóm! không thể xóa"
                    };
                    datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.DELETE, NameCategory_Const.GROUP_USER, Action_Status.FAIL);
                    _asyncLogger.LogInfo(JsonConvert.SerializeObject(datalog));
                    return BadRequest(model);
                }
                var result = _repo.FindById(id).FirstOrDefault();
                if (result != null)
                {
                    if (result.GroupName == "Quản trị hệ thống")
                    {
                        model.status = 0;
                        model.error = new ErrorModel()
                        {
                            Code = ErrCode_Const.EXCEPTION_API,
                            Msg = "Không thể xóa nhóm quản trị hệ thống"
                        };
                        datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.DELETE, NameCategory_Const.GROUP_USER, Action_Status.FAIL);
                        _asyncLogger.LogInfo(JsonConvert.SerializeObject(datalog));
                        return BadRequest(model);
                    }
                }
                await _repo.Delete(id);
                datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.DELETE, NameCategory_Const.GROUP_USER, Action_Status.SUCCESS);
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

        [Route("deleteItems")]
        [HttpPut()]
        public async Task<IActionResult> deleteItems(removeGroupListItems data)
        {
            BaseModels<TblTest> model = new BaseModels<TblTest>();
            try
            {
                foreach (Guid id in data.ids)
                {
                    await _repo.Delete(id);
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

        [HttpPost]
        [Route("LayTreePhanQuyen")]
        public object LayTreePhanQuyen(Guid IdGroup)
        {
            ErrorModel error = new ErrorModel();
            BaseModels<object> model = new BaseModels<object>();
            try
            {
                //List<EF_Core.Models.Permission> data_GroupRoles = (from item in _repo._context.Permissions
                //                                    where item.CodeGroup == null
                //                                           orderby item.Position ascending
                //                                           select new EF_Core.Models.Permission
                //                                           {
                //                                               Code = item.Code,
                //                                               Description = item.Description,
                //                                               PermitName = item.PermitName,
                //                                               Position = item.Position,
                //                                           }).OrderBy(x => x.Position).ToList();

                List<EF_Core.Models.Permission> data_GroupRoles_children = (from item in _repo._context.Permissions
                                                                            where item.CodeGroup == null && item.Disable == false
                                                                            orderby item.Position ascending
                                                                            select new EF_Core.Models.Permission
                                                                            {
                                                                                Code = item.Code,
                                                                                Description = item.Description,
                                                                                PermitName = item.PermitName,
                                                                                Position = item.Position,
                                                                                CodeGroup = item.CodeGroup

                                                                            }).OrderBy(x => x.Position).ToList();

                IQueryable<GroupPermit> data_permitRoles = _repo._context.GroupPermits.Where(item => item.GroupId == IdGroup).Select(item => new GroupPermit
                {

                    Code = item.Code
                });


                List<GroupRoleModel> data_Roles = (from item in _repo._context.Permissions
                                                   where item.Disable == false
                                                   orderby item.Position ascending
                                                   select new GroupRoleModel
                                                   {
                                                       Code = item.Code,
                                                       Description = item.Description,
                                                       Name = item.PermitName,
                                                       Position = (short?)item.Position,
                                                       CodeGroup = item.CodeGroup,
                                                       Permitted = data_permitRoles.Where(x => x.Code == item.Code).FirstOrDefault() == null ? false : true
                                                   }).OrderBy(x => x.Position).ToList();



                if (data_Roles == null || data_permitRoles == null)
                {
                    error = new ErrorModel();
                    model.status = 0;
                    model.data = string.Empty;
                    model.error = error;
                    return model;
                }

                //var echa = data_GroupRoles.AsEnumerable();
                var echa_child = data_GroupRoles_children.AsEnumerable();
                var econ = data_Roles.AsEnumerable();

                //var data = new
                //{
                //    text = "Tất cả",
                //    keytext = "Tất cả",
                //    data = new { },
                //    children = from goc in echa
                //               select new
                //               {
                //                   text = goc.PermitName.ToString(),
                //                   keytext = goc.PermitName.ToString() + goc.Code.ToString(),
                //                   data = new
                //                   {
                //                       IdGroup = goc.Code.ToString(),
                //                       GroupName = goc.PermitName,
                //                       IdRole = goc.Code.ToString(),
                //                   },
                //                   children = (from goc_child in echa_child
                //                               where goc_child.CodeGroup.ToString() == goc.Code
                //                               select new
                //                               {
                //                                   text = goc_child.PermitName.ToString(),
                //                                   keytext = goc_child.PermitName.ToString() + goc_child.Code.ToString(),
                //                                   data = new
                //                                   {
                //                                       IdGroup = goc_child.Code.ToString(),
                //                                       IdRole = goc_child.Code.ToString(),
                //                                       GroupName = goc_child.PermitName,
                //                                   },
                //                                   children = from con in econ
                //                                              where con.CodeGroup != null && con.CodeGroup == goc_child.Code
                //                                              select new
                //                                              {
                //                                                  text = con.Name.ToString(),
                //                                                  keytext = con.Name.ToString() + con.Code.ToString() + goc_child.Code.ToString(),
                //                                                  state = new
                //                                                  {
                //                                                      selected = (bool)con.Permitted
                //                                                  },
                //                                                  data = new
                //                                                  {
                //                                                      IdRole = con.Code.ToString(),
                //                                                      IdGroup = con.CodeGroup.ToString(),
                //                                                      RoleName = con.Name,
                //                                                      Permitted = con.Permitted,
                //                                                  },
                //                                                  children = from cons in econ
                //                                                             where cons.CodeGroup != null && cons.CodeGroup == con.Code
                //                                                             select new
                //                                                             {
                //                                                                 text = cons.Name.ToString(),
                //                                                                 keytext = cons.Name.ToString() + cons.Code.ToString() + goc_child.Code.ToString(),
                //                                                                 state = new
                //                                                                 {
                //                                                                     selected = (bool)cons.Permitted
                //                                                                 },
                //                                                                 data = new
                //                                                                 {
                //                                                                     IdRole = cons.Code.ToString(),
                //                                                                     IdGroup = cons.CodeGroup.ToString(),
                //                                                                     RoleName = cons.Name,
                //                                                                     Permitted = cons.Permitted,
                //                                                                 }
                //                                                             }
                //                                              }
                //                               }
                //                            )
                //               }
                //};

                var data = new
                {
                    text = "Tất cả",
                    keytext = "Tất cả",
                    data = new { },
                    children = (from goc_child in echa_child
                                where goc_child.CodeGroup is null
                                select new
                                {
                                    text = goc_child.PermitName.ToString(),
                                    keytext = goc_child.PermitName.ToString() + goc_child.Code.ToString(),
                                    data = new
                                    {
                                        IdGroup = goc_child.Code.ToString(),
                                        IdRole = goc_child.Code.ToString(),
                                        GroupName = goc_child.PermitName,
                                    },
                                    children = from con in econ
                                               where con.CodeGroup != null && con.CodeGroup == goc_child.Code
                                               select new
                                               {
                                                   text = con.Name.ToString(),
                                                   keytext = con.Name.ToString() + con.Code.ToString() + goc_child.Code.ToString(),
                                                   state = new
                                                   {
                                                       selected = (bool)con.Permitted
                                                   },
                                                   data = new
                                                   {
                                                       IdRole = con.Code.ToString(),
                                                       IdGroup = con.CodeGroup.ToString(),
                                                       RoleName = con.Name,
                                                       Permitted = con.Permitted,
                                                   },
                                                   children = from cons in econ
                                                              where cons.CodeGroup != null && cons.CodeGroup == con.Code
                                                              select new
                                                              {
                                                                  text = cons.Name.ToString(),
                                                                  keytext = cons.Name.ToString() + cons.Code.ToString() + goc_child.Code.ToString(),
                                                                  state = new
                                                                  {
                                                                      selected = (bool)cons.Permitted
                                                                  },
                                                                  data = new
                                                                  {
                                                                      IdRole = cons.Code.ToString(),
                                                                      IdGroup = cons.CodeGroup.ToString(),
                                                                      RoleName = cons.Name,
                                                                      Permitted = cons.Permitted
                                                                  },
                                                                  children = from conss in econ
                                                                             where conss.CodeGroup != null && conss.CodeGroup == cons.Code
                                                                             select new
                                                                             {
                                                                                 text = conss.Name.ToString(),
                                                                                 keytext = conss.Name.ToString() + conss.Code.ToString() + goc_child.Code.ToString(),
                                                                                 state = new
                                                                                 {
                                                                                     selected = (bool)conss.Permitted
                                                                                 },
                                                                                 data = new
                                                                                 {
                                                                                     IdRole = conss.Code.ToString(),
                                                                                     IdGroup = conss.CodeGroup.ToString(),
                                                                                     RoleName = conss.Name,
                                                                                     Permitted = conss.Permitted,
                                                                                 }
                                                                             }
                                                              }
                                               }
                                }
                                            )
                };
                return new
                {
                    data = data,
                    data_con = data_Roles,
                    data_role = data_permitRoles,
                };

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

        [HttpPost]
        [Route("UserGroupRoles_Update")]
        public object UserGroupRoles_Update([FromBody] UserGroupRoles request)
        {
            BaseModels<object> model = new BaseModels<object>();
            try
            {
                //var result = _repo.FindById(request.IdGroup).FirstOrDefault();
                //if (result != null)
                //{
                //    if (result.GroupName == "Quản trị hệ thống")
                //    {
                //        model.status = 0;
                //        model.error = new ErrorModel()
                //        {
                //            Code = ErrCode_Const.EXCEPTION_API,
                //            Msg = "Không thể chỉnh sửa nhóm quản trị hệ thống"
                //        };
                //        return BadRequest(model);
                //    }
                //}
                var exists = request.Code.Distinct().ToList();

                var grouproles = _repo._context.GroupPermits.Where(x => x.GroupId == request.IdGroup).ToList();

                if (grouproles.Count > 0)
                {
                    _repo._context.GroupPermits.RemoveRange(grouproles);
                    _repo._context.SaveChanges();
                }

                List<GroupPermit> lstpqGroupPermits = new List<GroupPermit>();
                foreach (var item in exists)
                {
                    GroupPermit pqGroupPermit = new GroupPermit();
                    pqGroupPermit.Code = item;
                    pqGroupPermit.GroupId = request.IdGroup;

                    lstpqGroupPermits.Add(pqGroupPermit);
                }

                if (lstpqGroupPermits.Count > 0)
                {
                    _repo._context.GroupPermits.AddRange(lstpqGroupPermits);
                    _repo._context.SaveChanges();
                }
                //lấy tất cả user trong group để logout từng user
                var users = _repo._context.Users.Where(x => x.GroupUserId == request.IdGroup).ToList();
                foreach (var item in users)
                {
                    var uuids = _repoUser.findUuidByUsername(item.UserName);
                    foreach (var it in uuids)
                    {
                        _repoUser.removeFromWhitelist(it.Uuid.ToString());
                    }
                }
                model.status = 1;
                return model;

            }
            catch (Exception ex)
            {
                model.status = 0;
                return model;
            }

        }
    }
}
