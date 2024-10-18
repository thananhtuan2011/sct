using API_SoCongThuong.Classes;
using API_SoCongThuong.Logger;
using API_SoCongThuong.Models;
using API_SoCongThuong.Reponsitories.IndustryRepository;
using EF_Core.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Data;
using System.Reflection.Metadata;
using System.Security.Principal;
using static API_SoCongThuong.Classes.ErrMsg_Const;

namespace API_SoCongThuong.Controllers
{
    [EnableCors("AllowOrigin")]
    [Route("api/[controller]")]
    [ApiController]
    public class IndustryController : ControllerBase
    {
        private IndustryRepo _repo;

        private IConfiguration _configuration;
        private readonly ILogger<AsyncLogger> _logger;
        public SoHoa_SoCongThuongContext _context;
        private AsyncLogger _asyncLogger;
        public IndustryController(SoHoa_SoCongThuongContext context, IConfiguration configuration, ILogger<AsyncLogger> logger)
        {
            _repo = new IndustryRepo(context);

            _logger = logger;
            _context = context;
            _asyncLogger = new AsyncLogger(_logger, _context);
            _configuration = configuration;
        }

        [Route("find")]
        [HttpPost]
        public IActionResult ListItems_New([FromBody] QueryRequestBody query)//query truyền lên
        {

            BaseModels<IndustryModel> model = new BaseModels<IndustryModel>();
            //Keyword tìm kiếm
            string _keywordSearch = "";

            //Khởi tạo sắp xếp dữ liệu acs hoặc desc khi tìm kiếm
            bool _orderBy_ASC = true;
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                //Khởi tạo mặc định sắp xếp dữ liệu
                Func<IndustryModel, object> _orderByExpression = x => x.IndustryCode;
                Func<IndustryModel, object> _thenByExpression = x => x.IndustryCode;
                Dictionary<string, Func<IndustryModel, object>> _sortableFields = new Dictionary<string, Func<IndustryModel, object>>   //Khởi tạo các trường để sắp xếp
                {
                    { "IndustryId", x => x.IndustryId },
                    { "IndustryCode", x => x.IndustryCode },
                    { "IndustryName", x => x.IndustryName },
                    { "ParentIndustryId", x => x.ParentIndustryId },
                    { "IndustryLevel", x => x.IndustryLevel},

                };

                //Check có sort hay không, nếu không thì sắp xếp theo 2 trường mặc định
                if (query.Sort != null && !string.IsNullOrEmpty(query.Sort.ColumnName) && _sortableFields.ContainsKey(query.Sort.ColumnName))
                {
                    _orderBy_ASC = ("desc".Equals(query.Sort.Direction.ToLower()) ? false : true);    //Sắp xếp asc hoặc desc
                    _orderByExpression = _sortableFields[query.Sort.ColumnName]; //Trường cần sắp xếp
                }

                //Cách 1 dùng entity
                IQueryable<IndustryModel> _data = _repo._context.Industries.Where(n => !n.IsDel).Select(x => new IndustryModel
                {
                    IndustryId = x.IndustryId,
                    IndustryCode = x.IndustryCode,
                    IndustryName = x.IndustryName ?? "",
                    ParentIndustryId = x.ParentIndustryId ?? Guid.Empty,
                    IsDel = x.IsDel,
                }).ToList().AsQueryable();

                //Kiểm tra điều kiện tìm kiếm
                if (query.SearchValue != null && query.SearchValue != "")
                {
                    //Lấy table đã select tìm kiếm theo keyword
                    _keywordSearch = query.SearchValue.Trim().ToLower();
                    _data = _data.Where(x =>
                       x.IndustryCode.ToLower().Contains(_keywordSearch)
                       || x.IndustryName.ToLower().Contains(_keywordSearch)
                   );
                }

                //Đếm số dòng của table đã select được
                int _countRows = _data.Count();

                //nếu table = 0 thì trả về không có dữ liệu
                if (_countRows == 0)
                {
                    return NotFound("Không có dữ liệu");
                }

                // Đệ quy tính cấp
                model.items = _data.ToList();
                var lstData = _repo._context.Industries.Where(c => !c.IsDel).ToList();
                for (int i = 0; i < model.items.Count(); i++)
                {
                    model.items[i].IndustryLevel = countLevel(model.items[i].IndustryId, lstData, 1);
                }
                //var level = model.items.ToList();

                //Sắp xếp dữ liệu theo acs
                if (_orderBy_ASC)
                {
                    model.items = model.items
                        .OrderBy(_orderByExpression)
                        .Skip((query.Panigator.PageIndex - 1) * query.Panigator.PageSize)
                        .Take(query.Panigator.PageSize)
                        .ToList();
                }

                //Sắp xếp dữ liệu theo desc
                else
                {
                    model.items = model.items
                        .OrderByDescending(_orderByExpression)
                        .Skip((query.Panigator.PageIndex - 1) * query.Panigator.PageSize)
                        .Take(query.Panigator.PageSize)
                        .ToList();
                }

                //query more = true
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


        // Funtion tính cấp
        private int countLevel(Guid id, List<Industry> lstData, int curLevel)
        {
            if (lstData != null && lstData.Count() > 0)
            {
                var data = lstData.Where(x => x.IndustryId == id).FirstOrDefault();
                if (data == null)
                {
                    return 0;
                }
                else
                {
                    if (data.ParentIndustryId != null && data.ParentIndustryId.HasValue) // && data.ParentIndustryId != Guid.Empty
                    {
                        int nextLevel = curLevel + 1;
                        return countLevel(data.ParentIndustryId.Value, lstData, nextLevel);
                    }
                    else
                    {
                        return curLevel;
                    }
                }
            }
            else
            {
                return 0;
            }
        }

        // Load data cho dropdown
        #region
        [Route("loadindustry/{id}")]
        [HttpGet]
        public IActionResult LoadIndustry(Guid id)
        {
            BaseModels<Industry> model = new BaseModels<Industry>();
            try
            {
                //Lấy Token, lấy model
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }


                if (id != Guid.Empty)
                {
                    List<Guid> ChildData = _repo._context.Industries.Where(x => x.ParentIndustryId == id).Select(x => x.IndustryId).ToList();
                    List<Guid> Result = ChildData.ToList();
                    Result.Add(id);

                    if (ChildData.Any())
                    {
                        var ListChild = GetListIdChild(ChildData, Result);
                        if (ListChild.Any())
                        {
                            IQueryable<Industry> _data = _repo._context.Industries.Where(x => !ListChild.Contains(x.IndustryId)).Select(d => new Industry()
                            {
                                IndustryId = d.IndustryId,
                                IndustryCode = d.IndustryCode,
                                IndustryName = d.IndustryName,
                                ParentIndustryId = d.ParentIndustryId,
                                IsDel = d.IsDel
                            }).OrderBy(x => x.IndustryCode);
                            model.items = _data.ToList();
                        }
                    }
                    else
                    {
                        IQueryable<Industry> _data = _repo.FindAll();
                        model.items = _data.ToList();
                    }
                }
                else
                {
                    IQueryable<Industry> _data = _repo.FindAll();
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
        #endregion

        [HttpGet("{id}")]
        public IActionResult getItemById(Guid id)
        {
            BaseModels<Industry> model = new BaseModels<Industry>();
            try
            {
                var result = _repo.FindByIndustryId(id);
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
        public async Task<IActionResult> Update(IndustryModel data)
        {
            BaseModels<Industry> model = new BaseModels<Industry>();
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }

                Industry? test = _repo._context.Industries.Where(x => x.IndustryId == data.IndustryId && !x.IsDel).FirstOrDefault();
                SystemLog datalog = new SystemLog();
                if (test != null)
                {
                    var industryCode = _repo.findByIndustryCode(data.IndustryCode, data.IndustryId);
                    if (industryCode)
                    {
                        model.status = 0;
                        model.error = new ErrorModel()
                        {
                            Code = ErrCode_Const.EXCEPTION_API,
                            Msg = "Mã ngành hàng đã tồn tại"
                        };
                        datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.UPDATE, NameCategory_Const.INDUSTRY, Action_Status.FAIL);
                        _asyncLogger.LogInfo(JsonConvert.SerializeObject(datalog));
                        return Ok(model);
                    }

                    //List<Guid> ChildData = _repo._context.Industries.Where(x => x.ParentIndustryId == data.IndustryId).Select(x => x.IndustryId).ToList();
                    //List<Guid> Result = ChildData.ToList();

                    //if (ChildData.Any())
                    //{
                    //    var ListChild = GetListIdChild(ChildData, Result);
                    //    if (ListChild.Any())
                    //    {
                    //        if (ListChild.Contains(data.ParentIndustryId))
                    //        {
                    //            model.status = 0;
                    //            model.error = new ErrorModel()
                    //            {
                    //                Code = ErrCode_Const.EXCEPTION_API,
                    //                Msg = "Không thể chỉnh sửa ngành nghề hiện tại thành ngành nghề con của chính nó."
                    //            };
                    //            return Ok(model);
                    //        }
                    //    }
                    //}

                    test.IndustryId = data.IndustryId;
                    test.IndustryCode = data.IndustryCode;
                    test.IndustryName = data.IndustryName;
                    test.ParentIndustryId = data.ParentIndustryId != Guid.Empty ? data.ParentIndustryId : null;

                    test.UpdateUserId = loginData.Userid;
                    test.UpdateTime = DateTime.Now;
                    test.IsDel = data.IsDel;
                    datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.UPDATE, NameCategory_Const.INDUSTRY, Action_Status.SUCCESS);
                    _asyncLogger.LogInfo(JsonConvert.SerializeObject(datalog));
                    //await _repo.Update(test);
                    model.status = 1;
                    return Ok(model);
                }
                else
                {
                    model.status = 0;
                    datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.UPDATE, NameCategory_Const.INDUSTRY, Action_Status.FAIL);
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

        private List<Guid> GetListIdChild(List<Guid> list, List<Guid> result)
        {
            var currentList = new List<Guid>();

            foreach (var id in list)
            {
                var childIds = _repo._context.Industries.Where(x => x.ParentIndustryId == id)
                                                        .Select(x => x.IndustryId)
                                                        .ToList();

                if (childIds.Any())
                {
                    currentList.AddRange(childIds);
                    result.AddRange(childIds);
                }
            }

            if (currentList.Any())
            {
                return GetListIdChild(currentList, result);
            }
            else
            {
                return result;
            }
        }


        [HttpPost()]
        public async Task<IActionResult> create(IndustryModel data)
        {
            BaseModels<Industry> model = new BaseModels<Industry>();
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                var industryCode = _repo.findByIndustryCode(data.IndustryCode, null);
                SystemLog datalog = new SystemLog();
                if (industryCode)
                {
                    model.status = 0;
                    model.error = new ErrorModel()
                    {
                        Code = ErrCode_Const.EXCEPTION_API,
                        Msg = "Mã ngành hàng đã tồn tại"
                    };
                    datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.CREATE, NameCategory_Const.INDUSTRY, Action_Status.FAIL);
                    _asyncLogger.LogInfo(JsonConvert.SerializeObject(datalog));
                    return BadRequest(model);
                }

                Industry test = new Industry();
                test.IndustryCode = data.IndustryCode;
                test.IndustryName = data.IndustryName;
                test.ParentIndustryId = data.ParentIndustryId != Guid.Empty ? data.ParentIndustryId : null;

                test.CreateUserId = loginData.Userid;
                test.CreateTime = DateTime.Now;

                await _repo.Insert(test);
                datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.CREATE, NameCategory_Const.INDUSTRY, Action_Status.SUCCESS);
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
            BaseModels<Industry> model = new BaseModels<Industry>();
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                SystemLog datalog = new SystemLog();
                if (_repo._context.Industries.Where(x => x.ParentIndustryId == id && !x.IsDel).Any())
                {
                    model.status = 0;
                    model.error = new ErrorModel()
                    {
                        Code = ErrCode_Const.PROPERTY_IS_INVALID,
                        Msg = "Vui lòng xoá hết ngành nghề con trước khi xoá nghành nghề cha."
                    };
                    datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.DELETE, NameCategory_Const.INDUSTRY, Action_Status.FAIL);
                    _asyncLogger.LogInfo(JsonConvert.SerializeObject(datalog));
                    return BadRequest(model);
                }
                await _repo.DeleteIndustry(id);
                datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.DELETE, NameCategory_Const.INDUSTRY, Action_Status.SUCCESS);
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
        public async Task<IActionResult> deleteItems(removeListIndustryItems data)
        {
            BaseModels<Industry> model = new BaseModels<Industry>();
            try
            {
                foreach (Guid id in data.IndustryIds)
                {
                    await _repo.DeleteIndustry(id);
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
