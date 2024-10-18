
using API_SoCongThuong.Classes;
using API_SoCongThuong.Logger;
using API_SoCongThuong.Models;
using API_SoCongThuong.Reponsitories.TypeOfTradePromotionRepository;
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
    public class TypeOfTradePromotionController : ControllerBase
    {
        private TypeOfTradePromotionRepo _repoTypeOfTradePromotion;
        private IConfiguration _configuration;
        private readonly ILogger<AsyncLogger> _logger;
        public SoHoa_SoCongThuongContext _context;
        private AsyncLogger _asyncLogger;
        public TypeOfTradePromotionController(SoHoa_SoCongThuongContext context, IConfiguration configuration, ILogger<AsyncLogger> logger)
        {
            _repoTypeOfTradePromotion = new TypeOfTradePromotionRepo(context);
            _logger = logger;
            _context = context;
            _asyncLogger = new AsyncLogger(_logger, _context);
            _configuration = configuration;
        }

        // Lấy danh sách danh mục loại hình chợ
        #region Danh mục loại hình chợ
        [Route("find")]
        [HttpPost]
        public IActionResult ListItems_New([FromBody] QueryRequestBody query)//query truyền lên
        {

            BaseModels<TypeOfTradePromotionModel> model = new BaseModels<TypeOfTradePromotionModel>();
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

                Func<TypeOfTradePromotionModel, object> _orderByExpression = x => x.TypeOfTradePromotionCode; //Khởi tạo mặc định sắp xếp dữ liệu
                Dictionary<string, Func<TypeOfTradePromotionModel, object>> _sortableFields = new Dictionary<string, Func<TypeOfTradePromotionModel, object>>   //Khởi tạo các trường để sắp xếp
                    {
                        { "TypeOfTradePromotionCode", x => x.TypeOfTradePromotionCode },
                        { "TypeOfTradePromotionName", x => x.TypeOfTradePromotionName }
                    };
                if (query.Sort != null
                    && !string.IsNullOrEmpty(query.Sort.ColumnName)
                    && _sortableFields.ContainsKey(query.Sort.ColumnName))
                {
                    _orderBy_ASC = ("desc".Equals(query.Sort.Direction.ToLower()) ? false : true);    //Sắp xếp asc hoặc desc
                    _orderByExpression = _sortableFields[query.Sort.ColumnName]; //Trường cần sắp xếp
                }

                //Cách 1 dùng entity
                IQueryable<TypeOfTradePromotionModel> _data = _repoTypeOfTradePromotion._context.TypeOfTradePromotions.Select(x => new TypeOfTradePromotionModel
                {
                    TypeOfTradePromotionId = x.TypeOfTradePromotionId,
                    TypeOfTradePromotionCode = x.TypeOfTradePromotionCode ?? "",
                    TypeOfTradePromotionName = x.TypeOfTradePromotionName ?? "",
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
                       x.TypeOfTradePromotionName.ToLower().Contains(_keywordSearch) || x.TypeOfTradePromotionCode.ToLower().Contains(_keywordSearch)
                   );  //Lấy table đã select tìm kiếm theo keyword
                }
                if (query.Filter != null && query.Filter.ContainsKey("idGroupParent") && !string.IsNullOrEmpty(query.Filter["idGroupParent"]))
                {
                    _data = _data.Where(x => x.TypeOfTradePromotionId.ToString().Contains(string.Join("", query.Filter["idGroupParent"])));
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
            BaseModels<TypeOfTradePromotion> model = new BaseModels<TypeOfTradePromotion>();
            try
            {
                var result = _repoTypeOfTradePromotion.FindById(id);
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
        public async Task<IActionResult> Update(TypeOfTradePromotion data)
        {
            BaseModels<TypeOfTradePromotion> model = new BaseModels<TypeOfTradePromotion>();
            try
            {

                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                SystemLog datalog = new SystemLog();
                TypeOfTradePromotion? SaveData = _repoTypeOfTradePromotion._context.TypeOfTradePromotions.Where(x => x.TypeOfTradePromotionId == data.TypeOfTradePromotionId && !x.IsDel).FirstOrDefault();
                if ( SaveData != null )
                {
                    var typeOfTradePromotion = _repoTypeOfTradePromotion.findByTypeOfTradePromotionCode(data.TypeOfTradePromotionCode, Guid.Parse(data.TypeOfTradePromotionId.ToString()));

                    if (typeOfTradePromotion)
                    {
                        model.status = 0;
                        model.error = new ErrorModel()
                        {
                            Code = ErrCode_Const.EXCEPTION_API,
                            Msg = "Mã hình thức đã tồn tại"
                        };
                        datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.UPDATE, NameCategory_Const.TYPE_OF_TRADE_PROMOTION, Action_Status.FAIL);
                        _asyncLogger.LogInfo(JsonConvert.SerializeObject(datalog));
                        return Ok(model);
                    }
                    SaveData.TypeOfTradePromotionId = Guid.Parse(data.TypeOfTradePromotionId.ToString());
                    SaveData.TypeOfTradePromotionCode = data.TypeOfTradePromotionCode;
                    SaveData.TypeOfTradePromotionName = data.TypeOfTradePromotionName;
                    
                    SaveData.UpdateUserId = loginData.Userid;
                    SaveData.UpdateTime = DateTime.Now;

                    await _repoTypeOfTradePromotion.Update(SaveData);
                    datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.UPDATE, NameCategory_Const.TYPE_OF_TRADE_PROMOTION, Action_Status.SUCCESS);
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
                    datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.UPDATE, NameCategory_Const.TYPE_OF_TRADE_PROMOTION, Action_Status.FAIL);
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
        public async Task<IActionResult> create(TypeOfTradePromotion data)
        {
            BaseModels<TypeOfTradePromotion> model = new BaseModels<TypeOfTradePromotion>();
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }

                var typeOfTradePromotion = _repoTypeOfTradePromotion.findByTypeOfTradePromotionCode(data.TypeOfTradePromotionCode, null);
                SystemLog datalog = new SystemLog();
                if (typeOfTradePromotion)
                {
                    model.status = 0;
                    model.error = new ErrorModel()
                    {
                        Code = ErrCode_Const.EXCEPTION_API,
                        Msg = "Mã hình thức đã tồn tại"
                    };
                    datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.CREATE, NameCategory_Const.TYPE_OF_TRADE_PROMOTION, Action_Status.FAIL);
                    _asyncLogger.LogInfo(JsonConvert.SerializeObject(datalog));
                    return BadRequest(model);
                }

                TypeOfTradePromotion SaveData = new TypeOfTradePromotion();
                SaveData.TypeOfTradePromotionCode = data.TypeOfTradePromotionCode;
                SaveData.TypeOfTradePromotionName = data.TypeOfTradePromotionName;
                SaveData.CreateUserId = loginData.Userid;
                SaveData.CreateTime = DateTime.Now;

                await _repoTypeOfTradePromotion.Insert(SaveData);
                datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.CREATE, NameCategory_Const.TYPE_OF_TRADE_PROMOTION, Action_Status.SUCCESS);
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

        [HttpPut("deleteTypeOfTradePromotion/{id}")]
        public async Task<IActionResult> deleteTypeOfTradePromotion(Guid id)
        {
            BaseModels<TypeOfTradePromotion> model = new BaseModels<TypeOfTradePromotion>();
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                SystemLog datalog = new SystemLog();
                TypeOfTradePromotion DeleteData = new TypeOfTradePromotion();
                DeleteData.TypeOfTradePromotionId = id;
                DeleteData.IsDel = true;
                await _repoTypeOfTradePromotion.DeleteTypeOfTradePromotion(DeleteData);
                datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.DELETE, NameCategory_Const.TYPE_OF_TRADE_PROMOTION, Action_Status.SUCCESS);
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

        [Route("deleteTypeOfTradePromotions")]
        [HttpPut()]
        public async Task<IActionResult> deleteTypeOfTradePromotions(removeListTypeOfTradePromotionItems data)
        {
            BaseModels<TypeOfTradePromotion> model = new BaseModels<TypeOfTradePromotion>();
            try
            {
                foreach (Guid id in data.TypeOfTradePromotionIds)
                {
                    TypeOfTradePromotion DeleteData = new TypeOfTradePromotion();
                    DeleteData.TypeOfTradePromotionId = id;
                    DeleteData.IsDel = true;
                    await _repoTypeOfTradePromotion.DeleteTypeOfTradePromotion(DeleteData);
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
