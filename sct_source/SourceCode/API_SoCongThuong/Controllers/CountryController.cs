
using API_SoCongThuong.Classes;
using API_SoCongThuong.Models;
using API_SoCongThuong.Reponsitories.CountryRepository;
using API_SoCongThuong.Reponsitories.ImportGoodsRepository;
using API_SoCongThuong.Reponsitories.ExportGoodsRepository;
using API_SoCongThuong.Reponsitories;
using EF_Core.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.Design;
using static System.Net.Mime.MediaTypeNames;
using API_SoCongThuong.Logger;
using Newtonsoft.Json;
using static API_SoCongThuong.Classes.ErrMsg_Const;
using System.Data;

namespace API_SoCongThuong.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CountryController : ControllerBase
    {
        private CountryRepo _repoCountry;
        private ImportGoodsRepo _repoImportGoods;
        private ExportGoodsRepo _repoExportGoods;
        private ParticipateSupportFairRepo _repoParticipateSupportFair;

        private IConfiguration _configuration;
        private readonly ILogger<AsyncLogger> _logger;
        public SoHoa_SoCongThuongContext _context;
        private AsyncLogger _asyncLogger;
        public CountryController(SoHoa_SoCongThuongContext context, IConfiguration configuration, ILogger<AsyncLogger> logger)
        {
            _repoCountry = new CountryRepo(context);
            _repoImportGoods = new ImportGoodsRepo(context);
            _repoExportGoods = new ExportGoodsRepo(context);
            _repoParticipateSupportFair = new ParticipateSupportFairRepo(context);

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

            BaseModels<CountryModel> model = new BaseModels<CountryModel>();
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

                Func<CountryModel, object> _orderByExpression = x => x.CountryCode; //Khởi tạo mặc định sắp xếp dữ liệu
                Dictionary<string, Func<CountryModel, object>> _sortableFields = new Dictionary<string, Func<CountryModel, object>>   //Khởi tạo các trường để sắp xếp
                    {
                        { "CountryCode", x => x.CountryCode },
                        { "CountryName", x => x.CountryName }
                    };
                if (query.Sort != null
                    && !string.IsNullOrEmpty(query.Sort.ColumnName)
                    && _sortableFields.ContainsKey(query.Sort.ColumnName))
                {
                    _orderBy_ASC = ("desc".Equals(query.Sort.Direction.ToLower()) ? false : true);    //Sắp xếp asc hoặc desc
                    _orderByExpression = _sortableFields[query.Sort.ColumnName]; //Trường cần sắp xếp
                }

                //Cách 1 dùng entity
                IQueryable<CountryModel> _data = _repoCountry._context.Countries.Select(x => new CountryModel
                {
                    CountryId = x.CountryId,
                    CountryCode = x.CountryCode ?? "",
                    CountryName = x.CountryName ?? "",
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
                       x.CountryName.ToLower().Contains(_keywordSearch) || x.CountryCode.ToLower().Contains(_keywordSearch)
                   );  //Lấy table đã select tìm kiếm theo keyword
                }
                if (query.Filter != null && query.Filter.ContainsKey("idGroupParent") && !string.IsNullOrEmpty(query.Filter["idGroupParent"]))
                {
                    _data = _data.Where(x => x.CountryId.ToString().Contains(string.Join("", query.Filter["idGroupParent"])));
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
            BaseModels<Country> model = new BaseModels<Country>();
            try
            {
                var result = _repoCountry.FindById(id);
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
        public async Task<IActionResult> Update(Country data)
        {
            BaseModels<Country> model = new BaseModels<Country>();
            try
            {
                SystemLog datalog = new SystemLog();
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }

                Country? SaveData = _repoCountry._context.Countries.Where(x => x.CountryId == data.CountryId && !x.IsDel).FirstOrDefault();
                if (SaveData != null)
                {
                    var countryCode = _repoCountry.findByCountryCode(data.CountryCode, Guid.Parse(data.CountryId.ToString()));

                    if (countryCode)
                    {
                        model.status = 0;
                        model.error = new ErrorModel()
                        {
                            Code = ErrCode_Const.EXCEPTION_API,
                            Msg = "Mã quốc gia đã tồn tại"
                        };
                        datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.UPDATE, NameCategory_Const.COUNTRY, Action_Status.FAIL);
                        _asyncLogger.LogInfo(JsonConvert.SerializeObject(datalog));
                        return Ok(model);
                    }
                    SaveData.CountryId = Guid.Parse(data.CountryId.ToString());
                    SaveData.CountryCode = data.CountryCode;
                    SaveData.CountryName = data.CountryName;
                    SaveData.UpdateUserId = loginData.Userid;
                    SaveData.UpdateTime = DateTime.Now;

                    await _repoCountry.Update(SaveData);
                    datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.UPDATE, NameCategory_Const.COUNTRY, Action_Status.SUCCESS);
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
                    datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.UPDATE, NameCategory_Const.COUNTRY, Action_Status.FAIL);
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
        public async Task<IActionResult> create(Country data)
        {
            BaseModels<Country> model = new BaseModels<Country>();
            try
            {
                SystemLog datalog = new SystemLog();
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                var countryCode = _repoCountry.findByCountryCode(data.CountryCode, null);

                if (countryCode)
                {
                    model.status = 0;
                    model.error = new ErrorModel()
                    {
                        Code = ErrCode_Const.EXCEPTION_API,
                        Msg = "Mã quốc gia đã tồn tại"
                    };
                    datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.CREATE, NameCategory_Const.COUNTRY, Action_Status.FAIL);
                    _asyncLogger.LogInfo(JsonConvert.SerializeObject(datalog));
                    return BadRequest(model);
                }

                Country SaveData = new Country();
                SaveData.CountryCode = data.CountryCode;
                SaveData.CountryName = data.CountryName;
                SaveData.CreateUserId = loginData.Userid;
                SaveData.CreateTime = DateTime.Now;

                await _repoCountry.Insert(SaveData);
                datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.CREATE, NameCategory_Const.COUNTRY, Action_Status.SUCCESS);
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

        [HttpPut("deleteCountry/{id}")]
        public async Task<IActionResult> deleteCountry(Guid id)
        {
            BaseModels<Country> model = new BaseModels<Country>();

            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                SystemLog datalog = new SystemLog();
                var countImportGoods = _repoImportGoods._context.ImportGoods.Where(x => x.CountryId == id && !x.IsDel).Count();
                var countExportGoods = _repoExportGoods._context.ExportGoods.Where(x => x.CountryId == id && !x.IsDel).Count();
                var countParticipateSupportFair = _repoParticipateSupportFair._context.ParticipateSupportFairs.Where(x => x.Country == id && !x.IsDel).Count();
                if (countImportGoods > 0 || countExportGoods > 0)
                {
                    model.status = 0;
                    model.error = new ErrorModel()
                    {
                        Code = ErrCode_Const.EXCEPTION_API,
                        Msg = "Dữ liệu đang được sử dụng ở trang khác"
                    };
                    datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.DELETE, NameCategory_Const.COUNTRY, Action_Status.FAIL);
                    _asyncLogger.LogInfo(JsonConvert.SerializeObject(datalog));
                    return Ok(model);
                }
                Country DeleteData = new Country();
                DeleteData.CountryId = id;
                DeleteData.IsDel = true;
                await _repoCountry.DeleteCountry(DeleteData);
                datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.DELETE, NameCategory_Const.COUNTRY, Action_Status.SUCCESS);
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
