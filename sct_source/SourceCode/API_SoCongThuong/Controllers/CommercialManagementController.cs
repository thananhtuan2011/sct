
using API_SoCongThuong.Classes;
using API_SoCongThuong.Models;
using API_SoCongThuong.Reponsitories.CommercialManagementRepository;
using API_SoCongThuong.Reponsitories.TypeOfMarketRepository;
using API_SoCongThuong.Reponsitories.DistrictRepository;
using API_SoCongThuong.Reponsitories.CommuneRepository;
using API_SoCongThuong.Reponsitories.CategoryRepository;
using API_SoCongThuong.Reponsitories.MarketManagementRepository;
using EF_Core.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.Design;
using ClosedXML.Excel;
using API_SoCongThuong.Logger;
using Newtonsoft.Json;
using System.Data;
using DocumentFormat.OpenXml.Spreadsheet;

namespace API_SoCongThuong.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommercialManagementController : ControllerBase
    {
        private CommercialManagementRepo _repoCommercialManagement;
        private TypeOfMarketRepo _repoTypeOfMarket;
        private DistrictRepository _repoDistrict;
        private CategoryRepo _repoCategory;
        private MarketManagementRepo _repoMarketManagement;

        private IConfiguration _configuration;
        private readonly ILogger<AsyncLogger> _logger;
        public SoHoa_SoCongThuongContext _context;
        private AsyncLogger _asyncLogger;

        public CommercialManagementController(SoHoa_SoCongThuongContext context, IConfiguration configuration, ILogger<AsyncLogger> logger)
        {
            _repoCommercialManagement = new CommercialManagementRepo(context);
            _repoTypeOfMarket = new TypeOfMarketRepo(context);
            _repoDistrict = new DistrictRepository(context);
            _repoCategory = new CategoryRepo(context);
            _repoMarketManagement = new MarketManagementRepo(context);

            _logger = logger;
            _context = context;
            _asyncLogger = new AsyncLogger(_logger, _context);
            _configuration = configuration;
        }

        [Route("loadtypeofmarket")]
        [HttpGet]
        public IActionResult LoadTypeOfMarket()
        {
            BaseModels<TypeOfMarketModel> model = new BaseModels<TypeOfMarketModel>();

            try
            {
                //Lấy Token, lấy model
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }

                //Query lấy data
                IQueryable<TypeOfMarketModel> _data = _repoTypeOfMarket.FindAll().Where(x => !x.IsDel).Select(x => new TypeOfMarketModel
                {
                    TypeOfMarketId = x.TypeOfMarketId,
                    TypeOfMarketCode = x.TypeOfMarketCode,
                    TypeOfMarketName = x.TypeOfMarketName,
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

        [Route("loadcategory")]
        [HttpGet]
        public IActionResult LoadCategory()
        {
            BaseModels<Category> model = new BaseModels<Category>();

            try
            {
                //Lấy Token, lấy model
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }

                //Query lấy data
                IQueryable<Category> _data = _repoCategory.FindAll();

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

        // Lấy danh sách
        #region
        [Route("find")]
        [HttpPost]
        public IActionResult ListItems_New([FromBody] QueryRequestBody query)//query truyền lên
        {

            BaseModels<ViewCommercialManagementModel> model = new BaseModels<ViewCommercialManagementModel>();
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

                Func<ViewCommercialManagementModel, object> _orderByExpression = x => x.Code; //Khởi tạo mặc định sắp xếp dữ liệu
                Dictionary<string, Func<ViewCommercialManagementModel, object>> _sortableFields = new Dictionary<string, Func<ViewCommercialManagementModel, object>>   //Khởi tạo các trường để sắp xếp
                    {
                        { "Id", x => x.Id },
                        { "Code", x => x.Code },
                        { "Name", x => x.Name },
                        { "DistrictName", x => x.DistrictName},
                        { "PhoneNumber", x => x.PhoneNumber },
                    };
                if (query.Sort != null
                    && !string.IsNullOrEmpty(query.Sort.ColumnName)
                    && _sortableFields.ContainsKey(query.Sort.ColumnName))
                {
                    _orderBy_ASC = ("desc".Equals(query.Sort.Direction.ToLower()) ? false : true);    //Sắp xếp asc hoặc desc
                    _orderByExpression = _sortableFields[query.Sort.ColumnName]; //Trường cần sắp xếp
                }

                //Cách 1 dùng entity

                var _data = from c in _repoCommercialManagement._context.CommercialManagements
                            join d in _repoDistrict._context.Districts on c.DistrictId equals d.DistrictId
                            where !c.IsDel
                            select new ViewCommercialManagementModel
                            {
                                Id = c.CommercialId,
                                Code = c.Code,
                                Name = c.Name,
                                DistrictName = d.DistrictName,
                                PhoneNumber = c.PhoneNumber,
                                Type = c.Type,
                                DistrictId = c.DistrictId
                            };

                if (query.SearchValue != null && query.SearchValue != "") //Kiểm tra điều kiện tìm kiếm
                {
                    _keywordSearch = query.SearchValue.Trim().ToLower();
                    _data = _data.Where(x =>
                       /* x.DistrictId.ToString().ToLower().Contains(_keywordSearch)
                        || */
                       x.Name.ToLower().Contains(_keywordSearch) || x.Code.ToLower().Contains(_keywordSearch)
                   );  //Lấy table đã select tìm kiếm theo keyword
                }
                if (query.Filter != null && query.Filter.ContainsKey("TypeMarket") && !string.IsNullOrEmpty(query.Filter["TypeMarket"]))
                {
                    _data = _data.Where(x => x.Type == Guid.Parse(query.Filter["TypeMarket"]));
                }

                if (query.Filter != null && query.Filter.ContainsKey("District") && !string.IsNullOrEmpty(query.Filter["District"]))
                {
                    _data = _data.Where(x => x.DistrictId == Guid.Parse(query.Filter["District"]));
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
            BaseModels<CommercialManagement> model = new BaseModels<CommercialManagement>();
            try
            {
                var result = _repoCommercialManagement.FindById(id);
                if (result == null)
                {
                    model.status = 0;
                    model.error = new ErrorModel()
                    {
                        Code = ErrCode_Const.CANNOT_FIND_DATA_BY_QUERY,
                        Msg = "Không có dữ liệu này trên DB",
                    };
                    return NotFound(model);
                }
                else
                {
                    model.status = 1;
                    model.items = result.ToList();
                    return Ok(model);
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
        public async Task<IActionResult> Update(CommercialManagement data)
        {
            BaseModels<CommercialManagement> model = new BaseModels<CommercialManagement>();
            try
            {

                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                SystemLog datalog = new SystemLog();
                CommercialManagement? SaveData = _repoCommercialManagement._context.CommercialManagements.Where(x => x.CommercialId == data.CommercialId && !x.IsDel).FirstOrDefault();
                if (SaveData != null)
                {
                    SaveData.CommercialId = Guid.Parse(data.CommercialId.ToString());
                    SaveData.Type = data.Type;
                    SaveData.Code = data.Code;
                    SaveData.Name = data.Name;
                    SaveData.TypeOfMarketId = data.TypeOfMarketId != Guid.Empty ? data.TypeOfMarketId : null;
                    SaveData.DistrictId = data.DistrictId;
                    SaveData.CommuneId = data.CommuneId;
                    SaveData.Address = data.Address;
                    SaveData.RankId = data.RankId != Guid.Empty ? data.RankId : null;
                    SaveData.ConstructiveNatureId = data.ConstructiveNatureId != Guid.Empty ? data.ConstructiveNatureId : null;
                    SaveData.BusinessNatureId = data.BusinessNatureId != Guid.Empty ? data.BusinessNatureId : null;
                    SaveData.TypeOfEconomic = data.TypeOfEconomic != Guid.Empty ? data.TypeOfEconomic : null;
                    SaveData.ManagementFormId = data.ManagementFormId != Guid.Empty ? data.ManagementFormId : null;
                    SaveData.ManagementObjectId = data.ManagementObjectId != Guid.Empty ? data.ManagementObjectId : null;
                    SaveData.PhoneNumber = data.PhoneNumber;
                    SaveData.Email = data.Email;
                    SaveData.Fax = data.Fax;
                    SaveData.Note = data.Note;
                    SaveData.UpdateTime = DateTime.Now;
                    SaveData.UpdateUserId = loginData.Userid;
                    SaveData.TypeOfMarket = data.TypeOfMarket;
                    SaveData.TypeOfCenterLogistic = data.TypeOfCenterLogistic;
                    SaveData.FormMarket = data.FormMarket;
                    SaveData.Form = data.Form;
                    SaveData.Area = data.Area;
                    SaveData.MarketCleared = data.MarketCleared;

                    await _repoCommercialManagement.Update(SaveData);
                    datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.UPDATE, NameCategory_Const.COMMERCIAL_MANAGEMENT, Action_Status.SUCCESS);
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
                    datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.UPDATE, NameCategory_Const.COMMERCIAL_MANAGEMENT, Action_Status.FAIL);
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
        public async Task<IActionResult> create(CommercialManagement data)
        {
            BaseModels<CommercialManagement> model = new BaseModels<CommercialManagement>();
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                SystemLog datalog = new SystemLog();
                CommercialManagement SaveData = new CommercialManagement();
                SaveData.Type = data.Type;
                SaveData.Code = data.Code;
                SaveData.Name = data.Name;
                SaveData.TypeOfMarketId = data.TypeOfMarketId != Guid.Empty ? data.TypeOfMarketId : null;
                SaveData.DistrictId = data.DistrictId;
                SaveData.CommuneId = data.CommuneId;
                SaveData.Address = data.Address;
                SaveData.RankId = data.RankId != Guid.Empty ? data.RankId : null;
                SaveData.ConstructiveNatureId = data.ConstructiveNatureId != Guid.Empty ? data.ConstructiveNatureId : null;
                SaveData.BusinessNatureId = data.BusinessNatureId != Guid.Empty ? data.BusinessNatureId : null;
                SaveData.TypeOfEconomic = data.TypeOfEconomic != Guid.Empty ? data.TypeOfEconomic : null;
                SaveData.ManagementFormId = data.ManagementFormId != Guid.Empty ? data.ManagementFormId : null;
                SaveData.ManagementObjectId = data.ManagementObjectId != Guid.Empty ? data.ManagementObjectId : null;
                SaveData.PhoneNumber = data.PhoneNumber;
                SaveData.Email = data.Email;
                SaveData.Fax = data.Fax;
                SaveData.Note = data.Note;
                SaveData.CreateUserId = loginData.Userid;
                SaveData.CreateTime = new DateTime();
                SaveData.TypeOfMarket = data.TypeOfMarket;
                SaveData.TypeOfCenterLogistic = data.TypeOfCenterLogistic;
                SaveData.FormMarket = data.FormMarket;
                SaveData.Form = data.Form;
                SaveData.Area = data.Area;
                SaveData.MarketCleared = data.MarketCleared;

                await _repoCommercialManagement.Insert(SaveData);
                datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.CREATE, NameCategory_Const.COMMERCIAL_MANAGEMENT, Action_Status.SUCCESS);
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

        [HttpPut("deleteCommercialManagement/{id}")]
        public async Task<IActionResult> deleteCommercialManagement(Guid id)
        {
            BaseModels<CommercialManagement> model = new BaseModels<CommercialManagement>();
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                SystemLog datalog = new SystemLog();
                var countMarketManagement = _repoMarketManagement._context.MarketManagements.Where(x => x.MarketId == id && !x.IsDel).Count();
                if(countMarketManagement > 0)
                {
                    model.status = 0;
                    model.error = new ErrorModel()
                    {
                        Code = ErrCode_Const.EXCEPTION_API,
                        Msg = "Dữ liệu đang được sử dụng ở trang khác"
                    };
                    return Ok(model);
                }
                CommercialManagement DeleteData = new CommercialManagement();
                DeleteData.CommercialId = id;
                DeleteData.IsDel = true;
                await _repoCommercialManagement.DeleteCommercialManagement(DeleteData);
                datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.DELETE, NameCategory_Const.COMMERCIAL_MANAGEMENT, Action_Status.SUCCESS);
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

        [Route("deleteCommercialManagements")]
        [HttpPut()]
        public async Task<IActionResult> deleteCommercialManagementes(removeListCommercialManagementItems data)
        {
            BaseModels<CommercialManagement> model = new BaseModels<CommercialManagement>();
            try
            {
                foreach (Guid id in data.CommercialManagementIds)
                {
                    CommercialManagement DeleteData = new CommercialManagement();
                    DeleteData.CommercialId = id;
                    DeleteData.IsDel = true;
                    await _repoCommercialManagement.DeleteCommercialManagement(DeleteData);
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

        private List<CommercialManagementModel> FindData([FromBody] QueryRequestBody query)//query truyền lên
        {
            string _keywordSearch = "";
            //Cách 1 dùng entity

            var _data = from c in _repoCommercialManagement._context.CommercialManagements
                        join cate in _repoCategory._context.Categories on c.Type equals cate.CategoryId where !cate.IsDel
                        join d in _repoDistrict._context.Districts on c.DistrictId equals d.DistrictId
                        where !c.IsDel 
                        select new CommercialManagementModel()
                        {
                            CommercialId = c.CommercialId,
                            DistrictId = c.DistrictId,
                            CommuneId = c.CommuneId,
                            Type = c.Type,
                            Name = c.Name,
                            RankId = c.RankId,
                            TypeOfMarket = c.TypeOfMarket,
                            TypeOfEconomic = c.TypeOfEconomic,
                            BusinessNatureId = c.BusinessNatureId
                        };

            return _data.ToList();
        }

        [HttpPost("Export")]
        public IActionResult Export([FromBody] QueryRequestBody query)
        {
            var data = FindData(query);

            if (!data.Any())
            {
                return BadRequest();
            }

            try
            {
              //  var listMarket;
                var listIdRank = _repoCategory._context.Categories.Where(x => !x.IsDel && x.CategoryTypeCode == "LEVEL_MARKET").OrderBy(x => x.Piority).Select(x => x.CategoryId).ToList();
                var idTypeOfMarket = _repoCategory._context.Categories.Where(x => !x.IsDel && x.CategoryTypeCode == "MARKET" && x.CategoryCode == "M1").Select(x => x.CategoryId).FirstOrDefault();
                var listDistrict = _repoDistrict._context.Districts.Where(x => !x.IsDel).Select(x => new District
                {
                    DistrictId = x.DistrictId,
                    DistrictName = x.DistrictName
                }).OrderBy(x => x.DistrictName).ToList();

                // id Siêu thị
                var idTypeOfSuperMarket = _repoCategory._context.Categories.Where(x => !x.IsDel && x.CategoryTypeCode == "MARKET" && x.CategoryCode == "M2").Select(x => x.CategoryId).FirstOrDefault();
                // id trung tâm thương mại
                var idTypeOfSuperCenter = _repoCategory._context.Categories.Where(x => !x.IsDel && x.CategoryTypeCode == "MARKET" && x.CategoryCode == "M3").Select(x => x.CategoryId).FirstOrDefault();

                // danh sách loại siêu thị
                var listTypeOfSuperMarket = _repoCategory._context.Categories.Where(x => !x.IsDel && x.CategoryTypeCode == "BUSINESS_NATURE_SUPERMARKET").OrderBy(x => x.Piority).Select(x => x.CategoryId).ToList();
                // danh sách loại hình kinh tế
                var listTypeOfEconomic = _repoCategory._context.Categories.Where(x => !x.IsDel && x.CategoryTypeCode == "TYPE_OF_ECONOMIC_COMMERCIAL").OrderBy(x => x.Piority).Select(x => x.CategoryId).ToList();
                using (var workbook = new XLWorkbook(@"Upload/Templates/QuanLyThongTin_Cho_ST_TTTM.xlsx"))
                {
                    if (idTypeOfSuperMarket != null && idTypeOfSuperCenter!= null)
                    {
                        var listSuperMarket = from d in data.Where(x => x.Type == idTypeOfSuperMarket)
                                              group d by d.DistrictId into g
                                              select new
                                              {
                                                  DistrictId = g.Key,
                                                  // đếm loại hình kinh tế nhà nước theo hạng 1, 2, 3 
                                                  NNH1 = g.Where(x => x.TypeOfEconomic == listTypeOfEconomic[0] && x.RankId == listIdRank[0]).Count(),
                                                  NNH2 = g.Where(x => x.TypeOfEconomic == listTypeOfEconomic[0] && x.RankId == listIdRank[1]).Count(),
                                                  NNH3 = g.Where(x => x.TypeOfEconomic == listTypeOfEconomic[0] && x.RankId == listIdRank[2]).Count(),
                                                  // đếm loại hình kinh tế tập thể theo hạng 1, 2, 3 
                                                  TTH1 = g.Where(x => x.TypeOfEconomic == listTypeOfEconomic[1] && x.RankId == listIdRank[0]).Count(),
                                                  TTH2 = g.Where(x => x.TypeOfEconomic == listTypeOfEconomic[1] && x.RankId == listIdRank[1]).Count(),
                                                  TTH3 = g.Where(x => x.TypeOfEconomic == listTypeOfEconomic[1] && x.RankId == listIdRank[2]).Count(),
                                                  // đếm loại hình kinh tế vốn đầu tư nước ngoài theo hạng 1, 2, 3 
                                                  DTH1 = g.Where(x => x.TypeOfEconomic == listTypeOfEconomic[2] && x.RankId == listIdRank[0]).Count(),
                                                  DTH2 = g.Where(x => x.TypeOfEconomic == listTypeOfEconomic[2] && x.RankId == listIdRank[1]).Count(),
                                                  DTH3 = g.Where(x => x.TypeOfEconomic == listTypeOfEconomic[2] && x.RankId == listIdRank[2]).Count(),

                                                  // đếm loại hình kinh tế khác theo hạng 1, 2, 3 
                                                  KH1 = g.Where(x => x.TypeOfEconomic == listTypeOfEconomic[3] && x.RankId == listIdRank[0]).Count(),
                                                  KH2 = g.Where(x => x.TypeOfEconomic == listTypeOfEconomic[3] && x.RankId == listIdRank[1]).Count(),
                                                  KH3 = g.Where(x => x.TypeOfEconomic == listTypeOfEconomic[3] && x.RankId == listIdRank[2]).Count(),

                                                  // đếm loại hình siêu thị tổng hợp theo hạng 1, 2, 3 
                                                  THH1 = g.Where(x => x.BusinessNatureId == listTypeOfSuperMarket[0] && x.RankId == listIdRank[0]).Count(),
                                                  THH2 = g.Where(x => x.BusinessNatureId == listTypeOfSuperMarket[0] && x.RankId == listIdRank[1]).Count(),
                                                  THH3 = g.Where(x => x.BusinessNatureId == listTypeOfSuperMarket[0] && x.RankId == listIdRank[2]).Count(),
                                                  // đếm loại hình siêu thị chuyên doanh theo hạng 1, 2, 3 
                                                  CDH1 = g.Where(x => x.BusinessNatureId == listTypeOfSuperMarket[1] && x.RankId == listIdRank[0]).Count(),
                                                  CDH2 = g.Where(x => x.BusinessNatureId == listTypeOfSuperMarket[1] && x.RankId == listIdRank[1]).Count(),
                                                  CDH3 = g.Where(x => x.BusinessNatureId == listTypeOfSuperMarket[1] && x.RankId == listIdRank[2]).Count(),
                                              };
                        var listSuperCenter = from d in data.Where(x => x.Type == idTypeOfSuperCenter)
                                              group d by d.DistrictId into g
                                              select new
                                              {
                                                  DistrictId = g.Key,
                                                  // đếm loại hình kinh tế nhà nước theo hạng 1, 2, 3 
                                                  NNH1 = g.Where(x => x.TypeOfEconomic == listTypeOfEconomic[0] && x.RankId == listIdRank[0]).Count(),
                                                  NNH2 = g.Where(x => x.TypeOfEconomic == listTypeOfEconomic[0] && x.RankId == listIdRank[1]).Count(),
                                                  NNH3 = g.Where(x => x.TypeOfEconomic == listTypeOfEconomic[0] && x.RankId == listIdRank[2]).Count(),
                                                  // đếm loại hình kinh tế tập thể theo hạng 1, 2, 3 
                                                  TTH1 = g.Where(x => x.TypeOfEconomic == listTypeOfEconomic[1] && x.RankId == listIdRank[0]).Count(),
                                                  TTH2 = g.Where(x => x.TypeOfEconomic == listTypeOfEconomic[1] && x.RankId == listIdRank[1]).Count(),
                                                  TTH3 = g.Where(x => x.TypeOfEconomic == listTypeOfEconomic[1] && x.RankId == listIdRank[2]).Count(),
                                                  // đếm loại hình kinh tế vốn đầu tư nước ngoài theo hạng 1, 2, 3 
                                                  DTH1 = g.Where(x => x.TypeOfEconomic == listTypeOfEconomic[2] && x.RankId == listIdRank[0]).Count(),
                                                  DTH2 = g.Where(x => x.TypeOfEconomic == listTypeOfEconomic[2] && x.RankId == listIdRank[1]).Count(),
                                                  DTH3 = g.Where(x => x.TypeOfEconomic == listTypeOfEconomic[2] && x.RankId == listIdRank[2]).Count(),

                                                  // đếm loại hình kinh tế khác theo hạng 1, 2, 3 
                                                  KH1 = g.Where(x => x.TypeOfEconomic == listTypeOfEconomic[3] && x.RankId == listIdRank[0]).Count(),
                                                  KH2 = g.Where(x => x.TypeOfEconomic == listTypeOfEconomic[3] && x.RankId == listIdRank[1]).Count(),
                                                  KH3 = g.Where(x => x.TypeOfEconomic == listTypeOfEconomic[3] && x.RankId == listIdRank[2]).Count(),
                                              };
                        IXLWorksheet worksheet = workbook.Worksheets.Worksheet(2);
                        int index = 16;
                        int row = 1;

                        int sumNNH1 = 0;
                        int sumTTH1 = 0;
                        int sumDTH1 = 0;
                        int sumKH1 = 0;

                        int sumNNH2 = 0;
                        int sumTTH2 = 0;
                        int sumDTH2 = 0;
                        int sumKH2 = 0;

                        int sumNNH3 = 0;
                        int sumTTH3 = 0;
                        int sumDTH3 = 0;
                        int sumKH3 = 0;

                        int sumTHH1 = 0;
                        int sumCDH1 = 0;

                        int sumTHH2 = 0;
                        int sumCDH2 = 0;

                        int sumTHH3 = 0;
                        int sumCDH3 = 0;

                        int sumTTTMNNH1 = 0;
                        int sumTTTMTTH1 = 0;
                        int sumTTTMDTH1 = 0;
                        int sumTTTMKH1 = 0;

                        int sumTTTMNNH2 = 0;
                        int sumTTTMTTH2 = 0;
                        int sumTTTMDTH2 = 0;
                        int sumTTTMKH2 = 0;

                        int sumTTTMNNH3 = 0;
                        int sumTTTMTTH3 = 0;
                        int sumTTTMDTH3 = 0;
                        int sumTTTMKH3 = 0;
                        for(int i = 0; i<listDistrict.Count; i++)
                        {
                          //  worksheet.Cell(index, 2).Value = row;
                            worksheet.Cell(index, 2).Value = listDistrict[i].DistrictName;
                            int totalST = 0;
                            int totalTTTM = 0;
                            foreach (var item in listSuperMarket)
                            {
                                if (listDistrict[i].DistrictId == item.DistrictId)
                                {
                                    worksheet.Cell(index, 6).Value = item.NNH1 + item.NNH2 + item.NNH3;
                                    worksheet.Cell(index, 7).Value = item.TTH1 + item.TTH2 + item.TTH3;
                                    worksheet.Cell(index, 8).Value = item.DTH1 + item.DTH2 + item.DTH3;
                                    worksheet.Cell(index, 9).Value = item.KH1 + item.KH2 + item.KH3; 
                                    worksheet.Cell(index, 10).Value = item.THH1 + item.THH2 + item.THH3;
                                    worksheet.Cell(index, 11).Value = item.CDH1 + item.CDH2 + item.CDH3;

                                    sumNNH1 += item.NNH1;
                                    sumTTH1 += item.TTH1;
                                    sumDTH1 += item.DTH1;
                                    sumKH1 += item.KH1;

                                    sumNNH2 += item.NNH2;
                                    sumTTH2 += item.TTH2;
                                    sumDTH2 += item.DTH2;
                                    sumKH2 += item.KH2;

                                    sumNNH3 += item.NNH3;
                                    sumTTH3 += item.TTH3;
                                    sumDTH3 += item.DTH3;
                                    sumKH3 += item.KH3;

                                    sumTHH1 += item.THH1;
                                    sumCDH1 += item.CDH1;

                                    sumTHH2 += item.THH2;
                                    sumCDH2 += item.CDH2;

                                    sumTHH3 += item.THH3;
                                    sumCDH3 += item.CDH3;

                                    totalST = item.NNH1 + item.NNH2 + item.NNH3
                                    + item.TTH1 + item.TTH2 + item.TTH3
                                    + item.DTH1 + item.DTH2 + item.DTH3
                                    + item.KH1 + item.KH2 + item.KH3
                                    + item.THH1 + item.THH2 + item.THH3
                                    + item.CDH1 + item.CDH2 + item.CDH3;
                                }
                            }
                            foreach (var item in listSuperCenter)
                            {
                                if (listDistrict[i].DistrictId == item.DistrictId)
                                {
                                    worksheet.Cell(index, 13).Value = item.NNH1 + item.NNH2 + item.NNH3;
                                    worksheet.Cell(index, 14).Value = item.TTH1 + item.TTH2 + item.TTH3;
                                    worksheet.Cell(index, 15).Value = item.DTH1 + item.DTH2 + item.DTH3;
                                    worksheet.Cell(index, 16).Value = item.KH1 + item.KH2 + item.KH3;


                                    sumTTTMNNH1 += item.NNH1;
                                    sumTTTMTTH1 += item.TTH1;
                                    sumTTTMDTH1 += item.DTH1;
                                    sumTTTMKH1 += item.KH1;

                                    sumTTTMNNH2 += item.NNH2;
                                    sumTTTMTTH2 += item.TTH2;
                                    sumTTTMDTH2 += item.DTH2;
                                    sumTTTMKH2 += item.KH2;

                                    sumTTTMNNH3 += item.NNH3;
                                    sumTTTMTTH3 += item.TTH3;
                                    sumTTTMDTH3 += item.DTH3;
                                    sumTTTMKH3 += item.KH3;

                                    totalTTTM = item.NNH1 + item.NNH2 + item.NNH3
                                    + item.TTH1 + item.TTH2 + item.TTH3
                                    + item.DTH1 + item.DTH2 + item.DTH3
                                    + item.KH1 + item.KH2 + item.KH3;
                                }
                            }

                            worksheet.Cell(index, 4).Value = totalST + totalTTTM;
                            worksheet.Cell(index, 5).Value = totalST;
                            worksheet.Cell(index, 12).Value = totalTTTM;
                            worksheet.Cell(index, 4).Style.Font.FontColor = XLColor.Red;
                            worksheet.Cell(index, 5).Style.Font.FontColor = XLColor.Red;
                            worksheet.Cell(index, 12).Style.Font.FontColor = XLColor.Red;

                            var addrow = worksheet.Row(index);
                            addrow.InsertRowsBelow(1);
                            index++;
                            row++;
                        }

                        worksheet.Cell(11, 6).Value = sumNNH1;
                        worksheet.Cell(12, 6).Value = sumNNH2;
                        worksheet.Cell(13, 6).Value = sumNNH3;

                        worksheet.Cell(11, 7).Value = sumTTH1;
                        worksheet.Cell(12, 7).Value = sumTTH2;
                        worksheet.Cell(13, 7).Value = sumTTH3;


                        worksheet.Cell(11, 8).Value = sumDTH1;
                        worksheet.Cell(12, 8).Value = sumDTH2;
                        worksheet.Cell(13, 8).Value = sumDTH3;

                        worksheet.Cell(11, 9).Value = sumKH1;
                        worksheet.Cell(12, 9).Value = sumKH2;
                        worksheet.Cell(13, 9).Value = sumKH3;


                        worksheet.Cell(11, 10).Value = sumTHH1;
                        worksheet.Cell(12, 10).Value = sumTHH2;
                        worksheet.Cell(13, 10).Value = sumTHH3;

                        worksheet.Cell(11, 11).Value = sumCDH1;
                        worksheet.Cell(12, 11).Value = sumCDH2;
                        worksheet.Cell(13, 11).Value = sumCDH3;

                        worksheet.Cell(11, 13).Value = sumTTTMNNH1;
                        worksheet.Cell(12, 13).Value = sumTTTMNNH2;
                        worksheet.Cell(13, 13).Value = sumTTTMNNH3;

                        worksheet.Cell(11, 14).Value = sumTTTMTTH1;
                        worksheet.Cell(12, 14).Value = sumTTTMTTH2;
                        worksheet.Cell(13, 14).Value = sumTTTMTTH3;


                        worksheet.Cell(11, 15).Value = sumTTTMDTH1;
                        worksheet.Cell(12, 15).Value = sumTTTMDTH2;
                        worksheet.Cell(13, 15).Value = sumTTTMDTH3;

                        worksheet.Cell(11, 16).Value = sumTTTMKH1;
                        worksheet.Cell(12, 16).Value = sumTTTMKH2;
                        worksheet.Cell(13, 16).Value = sumTTTMKH3;

                        var delrow = worksheet.Row(index);
                        delrow.Delete();
                    }
                    if (idTypeOfMarket != null)
                    {
                        var listMarket = from d in data.Where(x => x.Type == idTypeOfMarket) group d by d.DistrictId into g
                                         select new
                                         {
                                             DistrictId = g.Key,
                                             CountRank1 = g.Where(x => x.TypeOfMarket == 1 && x.RankId == listIdRank[0]).Count(),
                                             CountRank2 = g.Where(x => x.TypeOfMarket == 1 && x.RankId == listIdRank[1]).Count(),
                                             CountRank3 = g.Where(x => x.TypeOfMarket == 1 && x.RankId == listIdRank[2]).Count(),
                                             CountTypeOther = g.Where(x => x.TypeOfMarket == 2 ).Count(),
                                         };

                    
                            IXLWorksheet worksheet = workbook.Worksheets.Worksheet(1);
                            int index = 8;
                            int row = 1;
                            int sumR1 = 0;
                            int sumR2 = 0;
                            int sumR3 = 0;
                            int sumOther = 0;
                            for(int i = 0; i< listDistrict.Count; i++)
                            {
                                worksheet.Cell(index, 1).Value = row;
                                worksheet.Cell(index, 2).Value = listDistrict[i].DistrictName;
                                int total = 0;
                                foreach (var item in listMarket)
                                {
                                    if (listDistrict[i].DistrictId == item.DistrictId)
                                    {
                                        worksheet.Cell(index, 5).Value = item.CountRank1;
                                        worksheet.Cell(index, 6).Value = item.CountRank2;
                                        worksheet.Cell(index, 7).Value = item.CountRank3;
                                        worksheet.Cell(index, 8).Value = item.CountTypeOther;
                                        sumR1 += item.CountRank1;
                                        sumR2 += item.CountRank2;
                                        sumR3 += item.CountRank3;
                                        sumOther += item.CountTypeOther;
                                        total = item.CountRank1 + item.CountRank2 + item.CountRank3 + item.CountTypeOther;
                                    }
                                }
                                worksheet.Cell(index, 4).Value = total;
                                var addrow = worksheet.Row(index);
                                addrow.InsertRowsBelow(1);
                                index++;
                                row++;
                            }
                            worksheet.Cell(7, 5).Value = sumR1;
                            worksheet.Cell(7, 6).Value = sumR2;
                            worksheet.Cell(7, 7).Value = sumR3;
                            worksheet.Cell(7, 8).Value = sumOther;

                            var delrow = worksheet.Row(index);
                            delrow.Delete();
                    }
                    else
                    {
                        return StatusCode(500, "ERROR");
                    }
                    using (var stream = new MemoryStream())
                    {
                        workbook.SaveAs(stream);
                        stream.Flush();
                        stream.Position = 0;

                        return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "file.xlsx");
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}