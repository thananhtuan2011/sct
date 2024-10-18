
using API_SoCongThuong.Classes;
using API_SoCongThuong.Models;
using API_SoCongThuong.Reponsitories.UnitRepository;
using API_SoCongThuong.Reponsitories.ImportGoodsRepository;
using API_SoCongThuong.Reponsitories.ExportGoodsRepository;
using API_SoCongThuong.Reponsitories.TradePromotionProjectManagementRepository;
using EF_Core.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.Design;
using static System.Net.Mime.MediaTypeNames;
using API_SoCongThuong.Logger;
using Newtonsoft.Json;

namespace API_SoCongThuong.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UnitController : ControllerBase
    {
        private UnitRepo _repoUnit;
        private ImportGoodsRepo _repoImportGoods;
        private ExportGoodsRepo _repoExportGoods;
        private TradePromotionProjectManagementRepo _repoTradePromotionProjectManagement;
        private IConfiguration _configuration;
        private readonly ILogger<AsyncLogger> _logger;
        public SoHoa_SoCongThuongContext _context;
        private AsyncLogger _asyncLogger;

        public UnitController(SoHoa_SoCongThuongContext context, IConfiguration configuration, ILogger<AsyncLogger> logger)
        {
            _repoUnit = new UnitRepo(context);
            _repoImportGoods = new ImportGoodsRepo(context);
            _repoExportGoods = new ExportGoodsRepo(context);
            _repoTradePromotionProjectManagement = new TradePromotionProjectManagementRepo(context);
            _logger = logger;
            _context = context;
            _asyncLogger = new AsyncLogger(_logger, _context);
            _configuration = configuration;
        }

        [Route("find")]
        [HttpPost]
        public IActionResult ListItems_New([FromBody] QueryRequestBody query)//query truyền lên
        {

            BaseModels<UnitModel> model = new BaseModels<UnitModel>();
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

                Func<UnitModel, object> _orderByExpression = x => x.UnitName; //Khởi tạo mặc định sắp xếp dữ liệu
                Dictionary<string, Func<UnitModel, object>> _sortableFields = new Dictionary<string, Func<UnitModel, object>>   //Khởi tạo các trường để sắp xếp
                    {
                        { "UnitName", x => x.UnitName },
                        { "UnitNameEN", x => x.UnitNameEn },
                        { "UnitCode", x => x.UnitCode },
                        { "Exchange", x => x.Exchange },
                        { "Note", x => x.Note},
                    };
                if (query.Sort != null
                    && !string.IsNullOrEmpty(query.Sort.ColumnName)
                    && _sortableFields.ContainsKey(query.Sort.ColumnName))
                {
                    _orderBy_ASC = ("desc".Equals(query.Sort.Direction.ToLower()) ? false : true);    //Sắp xếp asc hoặc desc
                    _orderByExpression = _sortableFields[query.Sort.ColumnName]; //Trường cần sắp xếp
                }

                //Cách 1 dùng entity
                IQueryable<UnitModel> _data = _repoUnit._context.Units.Select(x => new UnitModel
                {
                    UnitId = x.UnitId,
                    UnitCode = x.UnitCode,
                    UnitName = x.UnitName,
                    UnitNameEn = x.UnitNameEn,
                    Exchange = x.Exchange,
                    Note = x.Note,
                    IsDel = x.IsDel,
                });
                _data = _data.Where(x => !x.IsDel);


                if (query.SearchValue != null && query.SearchValue != "") //Kiểm tra điều kiện tìm kiếm
                {
                    _keywordSearch = query.SearchValue.Trim().ToLower();
                    _data = _data.Where(x =>
                       /* x.DistrictId.ToString().ToLower().Contains(_keywordSearch)
                        || */
                       x.UnitName.ToLower().Contains(_keywordSearch) || x.UnitCode.ToLower().Contains(_keywordSearch)
                       || x.UnitNameEn.ToLower().Contains(_keywordSearch) || x.Exchange.ToString().Contains(_keywordSearch) 
                       || x.Note.ToLower().Contains(_keywordSearch)
                   );  //Lấy table đã select tìm kiếm theo keyword
                }
                if (query.Filter != null && query.Filter.ContainsKey("idGroupParent") && !string.IsNullOrEmpty(query.Filter["idGroupParent"]))
                {
                    _data = _data.Where(x => x.UnitId.ToString().Contains(string.Join("", query.Filter["idGroupParent"])));
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
            BaseModels<Unit> model = new BaseModels<Unit>();
            try
            {
                var result = _repoUnit.FindById(id);
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

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Unit data)
        {
            BaseModels<Unit> model = new BaseModels<Unit>();
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                SystemLog datalog = new SystemLog();
                var SaveData = _repoUnit._context.Units.Where(x => x.UnitId == data.UnitId).FirstOrDefault();
                if (SaveData != null)
                {
                    var unit = _repoUnit.findByUnitCode(data.UnitCode, Guid.Parse(data.UnitId.ToString()));

                    if (unit)
                    {
                        model.status = 0;
                        model.error = new ErrorModel()
                        {
                            Code = ErrCode_Const.EXCEPTION_API,
                            Msg = "Mã/Ký hiệu đơn vị tính đã tồn tại"
                        };
                        datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.UPDATE, NameCategory_Const.UNITS, Action_Status.FAIL);
                        _asyncLogger.LogInfo(JsonConvert.SerializeObject(datalog));
                        return Ok(model);
                    }
                    SaveData.UnitId = Guid.Parse(data.UnitId.ToString());
                    SaveData.UnitCode = data.UnitCode;
                    SaveData.UnitName = data.UnitName;
                    SaveData.UnitNameEn = data.UnitNameEn;
                    SaveData.Exchange = data.Exchange;
                    SaveData.Note = data.Note;
                    SaveData.UpdateUserId = loginData.Userid;
                    SaveData.UpdateTime = DateTime.Now;

                    await _repoUnit.Update(SaveData);
                    datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.UPDATE, NameCategory_Const.UNITS, Action_Status.SUCCESS);
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
        public async Task<IActionResult> create(Unit data)
        {
            BaseModels<Unit> model = new BaseModels<Unit>();
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }

                var unit = _repoUnit.findByUnitCode(data.UnitCode, null);
                SystemLog datalog = new SystemLog();
                if (unit)
                {
                    model.status = 0;
                    model.error = new ErrorModel()
                    {
                        Code = ErrCode_Const.EXCEPTION_API,
                        Msg = "Mã/Ký hiệu đơn vị tính đã tồn tại"
                    };
                    datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.CREATE, NameCategory_Const.UNITS, Action_Status.FAIL);
                    _asyncLogger.LogInfo(JsonConvert.SerializeObject(datalog));
                    return BadRequest(model);
                }

                Unit SaveData = new Unit();
                SaveData.UnitCode = data.UnitCode;
                SaveData.UnitName = data.UnitName;
                SaveData.UnitNameEn = data.UnitNameEn;
                SaveData.Exchange = data.Exchange;
                SaveData.Note = data.Note;
                SaveData.CreateUserId = loginData.Userid;
                SaveData.CreateTime = DateTime.Now;

                await _repoUnit.Insert(SaveData);
                datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.CREATE, NameCategory_Const.UNITS, Action_Status.SUCCESS);
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

        [HttpPut("deleteUnit/{id}")]
        public async Task<IActionResult> deleteUnit(Guid id)
        {
            BaseModels<Unit> model = new BaseModels<Unit>();
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                SystemLog datalog = new SystemLog();
                var countImportGoods = _repoImportGoods._context.ImportGoods.Where(x => x.AmountUnit == id && !x.IsDel).Count();
                var countExportGoods = _repoExportGoods._context.ExportGoods.Where(x => x.AmountUnit == id && !x.IsDel).Count();
                var count = _repoTradePromotionProjectManagement._context.TradePromotionProjectManagements.Where(x => x.CurrencyUnit == id && !x.IsDel).Count();
                if (countImportGoods > 0 || countExportGoods > 0 || count > 0)
                {
                    model.status = 0;
                    model.error = new ErrorModel()
                    {
                        Code = ErrCode_Const.EXCEPTION_API,
                        Msg = "Dữ liệu đang được sử dụng ở trang khác"
                    };
                    datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.DELETE, NameCategory_Const.UNITS, Action_Status.FAIL);
                    _asyncLogger.LogInfo(JsonConvert.SerializeObject(datalog));
                    return Ok(model);
                }
                Unit DeleteData = new Unit();
                DeleteData.UnitId = id;
                DeleteData.IsDel = true;
                await _repoUnit.DeleteUnit(DeleteData);
                datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.DELETE, NameCategory_Const.UNITS, Action_Status.SUCCESS);
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

        [Route("deleteUnits")]
        [HttpPut()]
        public async Task<IActionResult> deleteUnits(removeListUnitItems data)
        {
            BaseModels<Unit> model = new BaseModels<Unit>();
            try
            {
                foreach (Guid id in data.UnitIds)
                {
                    Unit DeleteData = new Unit();
                    DeleteData.UnitId = id;
                    DeleteData.IsDel = true;
                    await _repoUnit.DeleteUnit(DeleteData);
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
