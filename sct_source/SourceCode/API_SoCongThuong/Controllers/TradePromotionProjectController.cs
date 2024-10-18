﻿
using API_SoCongThuong.Classes;
using API_SoCongThuong.Logger;
using API_SoCongThuong.Models;
using API_SoCongThuong.Reponsitories.TradePromotionProjectRepository;
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
    public class TradePromotionProjectController : ControllerBase
    {
        private TradePromotionProjectRepo _repoTradePromotionProject;

        private IConfiguration _configuration;
        private readonly ILogger<AsyncLogger> _logger;
        public SoHoa_SoCongThuongContext _context;
        private AsyncLogger _asyncLogger;
        public TradePromotionProjectController(SoHoa_SoCongThuongContext context, IConfiguration configuration, ILogger<AsyncLogger> logger)
        {
            _repoTradePromotionProject = new TradePromotionProjectRepo(context);
            _logger = logger;
            _context = context;
            _asyncLogger = new AsyncLogger(_logger, _context);
            _configuration = configuration;
        }

        [Route("find")]
        [HttpPost]
        public IActionResult ListItems_New([FromBody] QueryRequestBody query)//query truyền lên
        {

            BaseModels<TradePromotionProjectModel> model = new BaseModels<TradePromotionProjectModel>();
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

                Func<TradePromotionProjectModel, object> _orderByExpression = x => x.TradePromotionProjectCode; //Khởi tạo mặc định sắp xếp dữ liệu
                Dictionary<string, Func<TradePromotionProjectModel, object>> _sortableFields = new Dictionary<string, Func<TradePromotionProjectModel, object>>   //Khởi tạo các trường để sắp xếp
                    {
                        { "TradePromotionProjectCode", x => x.TradePromotionProjectCode },
                        { "TradePromotionProjectName", x => x.TradePromotionProjectName }
                    };
                if (query.Sort != null
                    && !string.IsNullOrEmpty(query.Sort.ColumnName)
                    && _sortableFields.ContainsKey(query.Sort.ColumnName))
                {
                    _orderBy_ASC = ("desc".Equals(query.Sort.Direction.ToLower()) ? false : true);    //Sắp xếp asc hoặc desc
                    _orderByExpression = _sortableFields[query.Sort.ColumnName]; //Trường cần sắp xếp
                }

                //Cách 1 dùng entity
                IQueryable<TradePromotionProjectModel> _data = _repoTradePromotionProject._context.TradePromotionProjects.Select(x => new TradePromotionProjectModel
                {
                    TradePromotionProjectId = x.TradePromotionProjectId,
                    TradePromotionProjectCode = x.TradePromotionProjectCode ?? "",
                    TradePromotionProjectName = x.TradePromotionProjectName ?? "",
                    IsDel = x.IsDel
                });
                _data = _data.Where(x => !x.IsDel);


                //Cách 2 dùng linq
                //IQueryable<TestModel> _dataLinq = (from t in _repo._context.Districts
                //                                       //where
                //                                   select new TestModel
                //                                   {
                //                                       Id = t.Id,
                //                                       Name = t.Name,
                //                                   });


                if (query.SearchValue != null && query.SearchValue != "") //Kiểm tra điều kiện tìm kiếm
                {
                    _keywordSearch = query.SearchValue.Trim().ToLower();
                    _data = _data.Where(x =>
                       /* x.DistrictId.ToString().ToLower().Contains(_keywordSearch)
                        || */
                       x.TradePromotionProjectName.ToLower().Contains(_keywordSearch) || x.TradePromotionProjectCode.ToLower().Contains(_keywordSearch)
                   );  //Lấy table đã select tìm kiếm theo keyword
                }
                if (query.Filter != null && query.Filter.ContainsKey("idGroupParent") && !string.IsNullOrEmpty(query.Filter["idGroupParent"]))
                {
                    _data = _data.Where(x => x.TradePromotionProjectId.ToString().Contains(string.Join("", query.Filter["idGroupParent"])));
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

        [HttpGet("{id}")]
        public IActionResult getItemById(Guid id)
        {
            BaseModels<TradePromotionProject> model = new BaseModels<TradePromotionProject>();
            try
            {
                var result = _repoTradePromotionProject.FindById(id);
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
        public async Task<IActionResult> Update(TradePromotionProject data)
        {
            BaseModels<TradePromotionProject> model = new BaseModels<TradePromotionProject>();
            try
            {

                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                SystemLog datalog = new SystemLog();
                TradePromotionProject? SaveData = _repoTradePromotionProject._context.TradePromotionProjects.Where(x => x.TradePromotionProjectId == data.TradePromotionProjectId && !x.IsDel).FirstOrDefault();
                if (SaveData != null)
                {
                    var tradePromotionProject = _repoTradePromotionProject.findByTradePromotionProjectCode(data.TradePromotionProjectCode, Guid.Parse(data.TradePromotionProjectId.ToString()));

                    if (tradePromotionProject)
                    {
                        model.status = 0;
                        model.error = new ErrorModel()
                        {
                            Code = ErrCode_Const.EXCEPTION_API,
                            Msg = "Mã đề án đã tồn tại"
                        };
                        datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.UPDATE, NameCategory_Const.TRADE_PROMOTION_PROJECT, Action_Status.FAIL);
                        _asyncLogger.LogInfo(JsonConvert.SerializeObject(datalog));
                        return Ok(model);
                    }
                    SaveData.TradePromotionProjectId = Guid.Parse(data.TradePromotionProjectId.ToString());
                    SaveData.TradePromotionProjectCode = data.TradePromotionProjectCode;
                    SaveData.TradePromotionProjectName = data.TradePromotionProjectName;

                    SaveData.UpdateUserId = loginData.Userid;
                    SaveData.UpdateTime = DateTime.Now;

                    await _repoTradePromotionProject.Update(SaveData);
                    datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.UPDATE, NameCategory_Const.TRADE_PROMOTION_PROJECT, Action_Status.SUCCESS);
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
        public async Task<IActionResult> create(TradePromotionProject data)
        {
            BaseModels<TradePromotionProject> model = new BaseModels<TradePromotionProject>();
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                SystemLog datalog = new SystemLog();
                var tradePromotionProject = _repoTradePromotionProject.findByTradePromotionProjectCode(data.TradePromotionProjectCode, null);

                if (tradePromotionProject)
                {
                    model.status = 0;
                    model.error = new ErrorModel()
                    {
                        Code = ErrCode_Const.EXCEPTION_API,
                        Msg = "Mã đề án đã tồn tại"
                    };
                    datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.CREATE, NameCategory_Const.TRADE_PROMOTION_PROJECT, Action_Status.FAIL);
                    _asyncLogger.LogInfo(JsonConvert.SerializeObject(datalog));
                    return BadRequest(model);
                }

                TradePromotionProject SaveData = new TradePromotionProject();
                SaveData.TradePromotionProjectCode = data.TradePromotionProjectCode;
                SaveData.TradePromotionProjectName = data.TradePromotionProjectName;

                SaveData.CreateUserId = loginData.Userid;
                SaveData.CreateTime = DateTime.Now;

                await _repoTradePromotionProject.Insert(SaveData);
                datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.CREATE, NameCategory_Const.TRADE_PROMOTION_PROJECT, Action_Status.SUCCESS);
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

        [HttpPut("deleteTradePromotionProject/{id}")]
        public async Task<IActionResult> deleteTradePromotionProject(Guid id)
        {
            BaseModels<TradePromotionProject> model = new BaseModels<TradePromotionProject>();
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                SystemLog datalog = new SystemLog();
                TradePromotionProject DeleteData = new TradePromotionProject();
                DeleteData.TradePromotionProjectId = id;
                DeleteData.IsDel = true;
                await _repoTradePromotionProject.DeleteTradePromotionProject(DeleteData);
                datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.DELETE, NameCategory_Const.TRADE_PROMOTION_PROJECT, Action_Status.SUCCESS);
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

        [Route("deleteTradePromotionProjects")]
        [HttpPut()]
        public async Task<IActionResult> deleteTradePromotionProjects(removeListTradePromotionProjectItems data)
        {
            BaseModels<TradePromotionProject> model = new BaseModels<TradePromotionProject>();
            try
            {
                foreach (Guid id in data.TradePromotionProjectIds)
                {
                    TradePromotionProject DeleteData = new TradePromotionProject();
                    DeleteData.TradePromotionProjectId = id;
                    DeleteData.IsDel = true;
                    await _repoTradePromotionProject.DeleteTradePromotionProject(DeleteData);
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