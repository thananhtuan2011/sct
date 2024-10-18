using DpsLibs.Web;
using EF_Core.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.Design;
using API_SoCongThuong.Classes;
using API_SoCongThuong.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using API_SoCongThuong.Reponsitories;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.AspNetCore.Mvc.Filters;
using static API_SoCongThuong.Classes.Ulities;
using Newtonsoft.Json;
using System.Collections.Generic;
using API_SoCongThuong.Logger;
using System.Data;

namespace API_SoCongThuong.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TradeFairOrganizationCertificationController : ControllerBase
    {
        private TradeFairOrganizationCertificationRepo _repo;
        private IConfiguration _config;
        private readonly ILogger<AsyncLogger> _logger;
        public SoHoa_SoCongThuongContext _context;
        private AsyncLogger _asyncLogger;
        //private BusinessRepo _repoBusi;

        public TradeFairOrganizationCertificationController(SoHoa_SoCongThuongContext context, IConfiguration configuration, ILogger<AsyncLogger> logger)
        {
            _repo = new TradeFairOrganizationCertificationRepo(context);
            _config = configuration;
            _logger = logger;
            _context = context;
            _asyncLogger = new AsyncLogger(_logger, _context);
        }

        [Route("find")]
        [HttpPost]
        public IActionResult ListItems_New([FromBody] QueryRequestBody query)//query truyền lên
        {

            BaseModels<TradeFairOrganizationCertificationModel> model = new BaseModels<TradeFairOrganizationCertificationModel>();
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

                Func<TradeFairOrganizationCertificationModel, object> _orderByExpression = x => x.TradeFairOrganizationCertificationId; //Khởi tạo mặc định sắp xếp dữ liệu
                Dictionary<string, Func<TradeFairOrganizationCertificationModel, object>> _sortableFields = new Dictionary<string, Func<TradeFairOrganizationCertificationModel, object>>   //Khởi tạo các trường để sắp xếp
                    {
                    { "TradeFairName", x => x.TradeFairName },
                    { "Address", x => x.Address },
                    { "Scale", x => x.Scale },
                    { "TextNumber", x => x.TextNumber },
                    { "CreateUserName", x => x.CreateUserName },
                    };
                if (query.Sort != null
                    && !string.IsNullOrEmpty(query.Sort.ColumnName)
                    && _sortableFields.ContainsKey(query.Sort.ColumnName))
                {
                    _orderBy_ASC = ("desc".Equals(query.Sort.Direction.ToLower()) ? false : true);    //Sắp xếp asc hoặc desc
                    _orderByExpression = _sortableFields[query.Sort.ColumnName]; //Trường cần sắp xếp
                }

                IQueryable<TradeFairOrganizationCertificationModel> _data = _repo._context.TradeFairOrganizationCertifications.Where(x => !x.IsDel)
                    .GroupJoin(_repo._context.Users,
                                tf => tf.CreateUserId,
                                u => u.UserId,
                                (tf, u) => new { tf, u })
                    .SelectMany(r => r.u.DefaultIfEmpty(), (tfo, ur) => new TradeFairOrganizationCertificationModel
                    {
                        TradeFairOrganizationCertificationId = tfo.tf.TradeFairOrganizationCertificationId,
                        TradeFairName = tfo.tf.TradeFairName,
                        Address = tfo.tf.Address,
                        Scale = tfo.tf.Scale,
                        TextNumber = tfo.tf.TextNumber,
                        CreateUserName = ur.FullName,
                        CreateTime = tfo.tf.CreateTime,
                    });

                if (query.SearchValue != null && query.SearchValue != "") //Kiểm tra điều kiện tìm kiếm
                {
                    _keywordSearch = query.SearchValue.Trim().ToLower();
                    _data = _data.Where(x =>
                    x.Address.ToLower().Contains(_keywordSearch)
                    || x.TradeFairName.ToLower().Contains(_keywordSearch)
                    || x.Scale.ToLower().Contains(_keywordSearch)
                    || x.TextNumber.ToLower().Contains(_keywordSearch)
                    || x.CreateUserName.ToLower().Contains(_keywordSearch)
                    );
                }

                if (query.Filter != null && query.Filter.ContainsKey("MinTime")
                                         && !string.IsNullOrEmpty(query.Filter["MinTime"]))
                {
                    _data = _data.Where(x =>
                                (x.CreateTime) >=
                                DateTime.ParseExact(query.Filter["MinTime"], "dd/MM/yyyy", null));
                }

                if (query.Filter != null && query.Filter.ContainsKey("MaxTime")
                    && !string.IsNullOrEmpty(query.Filter["MaxTime"]))
                {
                    _data = _data.Where(x =>
                               x.CreateTime <=
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

        [HttpPost()]
        public async Task<IActionResult> create([FromForm] TradeFairOrganizationCertificationModel data)
        {
            BaseModels<TradeFairOrganizationCertificationModel> model = new BaseModels<TradeFairOrganizationCertificationModel>();
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                SystemLog datalog = new SystemLog();
                var Files = Request.Form.Files;
                var LstFile = new List<TradeFairOrganizationCertificationAttachFileModel>();
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
                                Linkfile = "TradeFairOrganizationCertification"
                            };
                            var result = Ulities.UploadFile(up, _config);

                            TradeFairOrganizationCertificationAttachFileModel fileSave = new TradeFairOrganizationCertificationAttachFileModel();
                            fileSave.LinkFile = result.link;
                            LstFile.Add(fileSave);
                        }
                    }
                }
                data.ListFiles = LstFile;

                var LstTimes = new List<TradeFairOrganizationCertificationTimeModel>();
                var _ = JsonConvert.DeserializeObject<List<TradeFairOrganizationCertificationTimeModel>>(data.ListTimeString);
                foreach (var i in _)
                    if (i != null)
                    {
                        TradeFairOrganizationCertificationTimeModel time = new TradeFairOrganizationCertificationTimeModel();
                        time.TradeFairOrganizationCertificationId = i.TradeFairOrganizationCertificationId;
                        time.StartTime = Ulities.ConvertTimeZone(HttpContext.Request.Headers, i.StartTime);
                        time.EndTime = Ulities.ConvertTimeZone(HttpContext.Request.Headers, i.EndTime);
                        LstTimes.Add(time);
                    }
                data.ListTimes = LstTimes;

                data.CreateTime = DateTime.Now;
                data.CreateUserId = loginData.Userid;

                await _repo.Insert(data);
                datalog = Ulities.WriteLog(_config, HttpContext, loginData.Username, ActionType_Const.CREATE, NameCategory_Const.TRADE_FAIR_ORGANIZATION_CERTIFICATION, Action_Status.SUCCESS);
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
        public async Task<IActionResult> Update([FromForm] TradeFairOrganizationCertificationModel data)
        {
            BaseModels<TradeFairOrganizationCertificationModel> model = new BaseModels<TradeFairOrganizationCertificationModel>();
            try
            {

                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                SystemLog datalog = new SystemLog();
                var CheckData = _repo.FindById(data.TradeFairOrganizationCertificationId, _config);
                if (CheckData == null)
                {
                    //chỗ này không tồn tại id
                    model.status = 0;
                    model.error = new ErrorModel()
                    {
                        Code = ErrCode_Const.PROPERTY_IS_NULL_OR_EMPTY
                    };
                    datalog = Ulities.WriteLog(_config, HttpContext, loginData.Username, ActionType_Const.UPDATE, NameCategory_Const.TRADE_FAIR_ORGANIZATION_CERTIFICATION, Action_Status.FAIL);
                    _asyncLogger.LogInfo(JsonConvert.SerializeObject(datalog));
                    return BadRequest(model);
                }
                else
                {
                    var Files = Request.Form.Files;
                    var LstFile = new List<TradeFairOrganizationCertificationAttachFileModel>();
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
                                    Linkfile = "TradeFairOrganizationCertification"
                                };
                                var result = Ulities.UploadFile(up, _config);

                                TradeFairOrganizationCertificationAttachFileModel fileSave = new TradeFairOrganizationCertificationAttachFileModel();
                                fileSave.LinkFile = result.link;
                                LstFile.Add(fileSave);
                            }
                        }
                    }
                    data.ListFiles = LstFile;

                    var LstTimes = new List<TradeFairOrganizationCertificationTimeModel>();
                    var _ = JsonConvert.DeserializeObject<List<TradeFairOrganizationCertificationTimeModel>>(data.ListTimeString);
                    foreach (var i in _)
                        if (i != null)
                        {
                            TradeFairOrganizationCertificationTimeModel time = new TradeFairOrganizationCertificationTimeModel();
                            time.TradeFairOrganizationCertificationId = i.TradeFairOrganizationCertificationId;
                            time.StartTime = i.StartTime;
                            time.EndTime = i.EndTime;
                            LstTimes.Add(time);
                        }
                    data.ListTimes = LstTimes;

                    data.UpdateTime = DateTime.Now;
                    data.UpdateUserId = loginData.Userid;
                    await _repo.Update(data, _config);
                    datalog = Ulities.WriteLog(_config, HttpContext, loginData.Username, ActionType_Const.UPDATE, NameCategory_Const.TRADE_FAIR_ORGANIZATION_CERTIFICATION, Action_Status.SUCCESS);
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

        [HttpGet("{id}")]
        public IActionResult getItemById(Guid id)
        {
            BaseModels<TradeFairOrganizationCertificationModel> model = new BaseModels<TradeFairOrganizationCertificationModel>();
            try
            {
                var info = _repo.FindById(id, _config);

                if (info == null)
                    return NotFound(ErrMsg_Const.GetMsg(ErrCode_Const.CANNOT_FIND_DATA_BY_QUERY));

                //Set data cho base model
                model.status = 1;
                model.data = info;
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
            BaseModels<object> model = new BaseModels<object>();
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                SystemLog datalog = new SystemLog();
                await _repo.Delete(id, _config);
                datalog = Ulities.WriteLog(_config, HttpContext, loginData.Username, ActionType_Const.DELETE, NameCategory_Const.TRADE_FAIR_ORGANIZATION_CERTIFICATION, Action_Status.SUCCESS);
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
