using API_SoCongThuong.Classes;
using API_SoCongThuong.Logger;
using API_SoCongThuong.Models;
using API_SoCongThuong.Reponsitories.SysLogsRepository;
using API_SoCongThuong.Reponsitories.TestRepository;
using EF_Core.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace API_SoCongThuong.Controllers
{
    [EnableCors("AllowOrigin")]
    [Route("api/[controller]")]
    [ApiController]
    public class SysLogsController : ControllerBase
    {
        private SysLogsRepo _repo;
        private IConfiguration _config;
        private readonly IConnectionMultiplexer _redisCache;
        public SysLogsController(SoHoa_SoCongThuongContext context, IConfiguration configuration, IConnectionMultiplexer redisCache)
        {
            _repo = new SysLogsRepo(context);
            _config = configuration;
            _redisCache = redisCache;
        }

        [Route("find")]
        [HttpPost]
        public IActionResult ListItems_New([FromBody] QueryRequestBody query)
        {
            BaseModels<SysLogsModel> model = new BaseModels<SysLogsModel>();
            bool _orderBy_ASC = false; 
            try
            {
                //Lấy Token, lấy model
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }

                Func<SysLogsModel, object> _orderByExpression = x => x.Date;
                Dictionary<string, Func<SysLogsModel, object>> _sortableFields = new Dictionary<string, Func<SysLogsModel, object>> 
                    {
                        { "Time", x => x.Time },
                        { "Method", x => x.ActionType },
                        { "TenDangNhap", x => x.TenDangNhap },
                        { "Ip", x => x.Ip },
                        { "ActionName", x => x.ActionName },
                        { "Content", x => x.Content },
                    };

                if (query.Sort != null
                    && !string.IsNullOrEmpty(query.Sort.ColumnName)
                    && _sortableFields.ContainsKey(query.Sort.ColumnName))
                {
                    _orderBy_ASC = ("desc".Equals(query.Sort.Direction.ToLower()) ? false : true);
                    _orderByExpression = _sortableFields[query.Sort.ColumnName];
                }

                IQueryable<SysLogsModel> _data = (from sys in _repo._context.SystemLogs
                                                  join us in _repo._context.Users on sys.UserName equals us.UserName where !sys.IsDel
                                                  select (new SysLogsModel
                                                  {
                                                      LogId = sys.LogId,
                                                      TenDangNhap = us.UserName,
                                                      Ip = sys.IpPortCurrentNode,
                                                      ActionName = sys.ActionName,
                                                      ActionType = sys.ActionType,
                                                      Content = sys.ContentLog,
                                                      Time = sys.StartTime.AddHours(7).ToString("dd/MM/yyyy HH:mm"),
                                                      Date = sys.StartTime
                                                  })).ToList().AsQueryable();
                //Filter
                if (query.Filter != null && query.Filter.ContainsKey("Method") && !string.IsNullOrEmpty(query.Filter["Method"].ToString()))
                {
                    _data = _data.Where(x => x.ActionType.ToLower().Contains(string.Join("", query.Filter["Method"]).ToLower()));
                }

                if (query.Filter != null && query.Filter.ContainsKey("Keyword") && !string.IsNullOrEmpty(query.Filter["Keyword"].ToString()))
                {
                    _data = _data.Where(x => x.TenDangNhap.ToLower().Contains(query.Filter["Keyword"].ToLower()));
                }

                if (query.Filter != null && query.Filter.ContainsKey("MinTime")
                    && !string.IsNullOrEmpty(query.Filter["MinTime"]))
                {
                    _data = _data.Where(x => x.Date >= DateTime.ParseExact(query.Filter["MinTime"], "dd/MM/yyyy", null));
                }

                if (query.Filter != null && query.Filter.ContainsKey("MaxTime")
                    && !string.IsNullOrEmpty(query.Filter["MaxTime"]))
                {
                    _data = _data.Where(x => x.Date <= DateTime.ParseExact(query.Filter["MaxTime"], "dd/MM/yyyy", null));
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

        [HttpPut("deleteLog/{id}")]
        public async Task<IActionResult> DeleteLog (Guid id)
        {
            BaseModels<SysLogsModel> model = new BaseModels<SysLogsModel>();
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }

                SystemLog data = new SystemLog();
                data.LogId = id;
                data.IsDel = true;
                await _repo.DeleteLog(data);
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
