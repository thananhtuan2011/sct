
using API_SoCongThuong.Classes;
using API_SoCongThuong.Models;
using API_SoCongThuong.Reponsitories.MarketManagementRepository;
using API_SoCongThuong.Reponsitories.CommercialManagementRepository;
using API_SoCongThuong.Reponsitories.DistrictRepository;
using API_SoCongThuong.Reponsitories.CommuneRepository;
using API_SoCongThuong.Reponsitories.CategoryRepository;
using EF_Core.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.Design;
using API_SoCongThuong.Reponsitories.BusinessLineRepository;
using ClosedXML.Excel;
using API_SoCongThuong.Logger;
using Newtonsoft.Json;

namespace API_SoCongThuong.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MarketManagementController : ControllerBase
    {
        private MarketManagementRepo _repoMarketManagement;
        private CommercialManagementRepo _repoCommercialManagement;
        private DistrictRepository _repoDistrictRepo;
        private CommuneRepo _repoCommuneRepo;
        private CategoryRepo _repoCategory;
        private BusinessLineRepo _repoBussinessLine;

        private IConfiguration _configuration;
        private readonly ILogger<AsyncLogger> _logger;
        public SoHoa_SoCongThuongContext _context;
        private AsyncLogger _asyncLogger;
        public MarketManagementController(SoHoa_SoCongThuongContext context, IConfiguration configuration, ILogger<AsyncLogger> logger)
        {
            _repoBussinessLine = new BusinessLineRepo(context);
            _repoMarketManagement = new MarketManagementRepo(context);
            _repoCommercialManagement = new CommercialManagementRepo(context);
            _repoDistrictRepo = new DistrictRepository(context);
            _repoCommuneRepo = new CommuneRepo(context);
            _repoCategory = new CategoryRepo(context);

            _logger = logger;
            _context = context;
            _asyncLogger = new AsyncLogger(_logger, _context);
            _configuration = configuration;
        }

        [Route("loaddistrict")]
        [HttpGet]
        public IActionResult LoadDistrict()
        {
            BaseModels<DistrictMarketModel> district_model = new BaseModels<DistrictMarketModel>();

            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }

                IQueryable<DistrictMarketModel> _data = _repoDistrictRepo._context.Districts.Where(x => !x.IsDel).Select(x => new DistrictMarketModel
                {
                    DistrictId = x.DistrictId,
                    DistrictCode = x.DistrictCode,
                    DistrictName = x.DistrictName,
                });

                district_model.status = 1;
                district_model.items = _data.ToList();
                return Ok(district_model);
            }
            catch (Exception ex)
            {
                district_model.status = 0;
                district_model.error = new ErrorModel()
                {
                    Code = ErrCode_Const.EXCEPTION_API,
                    Msg = ex.Message
                };
                return BadRequest(district_model);
            }
        }

        [Route("loadcommune")]
        [HttpGet]
        public IActionResult LoadCommune()
        {
            BaseModels<CommuneMarketModel> Commune_model = new BaseModels<CommuneMarketModel>();

            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }

                IQueryable<CommuneMarketModel> _data = _repoCommuneRepo._context.Communes.Where(x => !x.IsDel).Select(x => new CommuneMarketModel
                {
                    CommuneId = x.CommuneId,
                    CommuneCode = x.CommuneCode,
                    CommuneName = x.CommuneName,
                    DistrictId = x.DistrictId,
                });

                Commune_model.status = 1;
                Commune_model.items = _data.ToList();
                return Ok(Commune_model);
            }
            catch (Exception ex)
            {
                Commune_model.status = 0;
                Commune_model.error = new ErrorModel()
                {
                    Code = ErrCode_Const.EXCEPTION_API,
                    Msg = ex.Message
                };
                return BadRequest(Commune_model);
            }
        }

        [Route("loadmarket")]
        [HttpGet]
        public IActionResult LoadMarket()
        {
            BaseModels<MarketModel> Market_model = new BaseModels<MarketModel>();

            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }

                var _data = from c in _repoCommercialManagement._context.CommercialManagements
                            join t in _repoCategory._context.Categories
                                on c.Type equals t.CategoryId
                            where (t.CategoryTypeCode == "MARKET" && !c.IsDel)
                            select new MarketModel
                            {
                                MarketId = c.CommercialId,
                                MarketName = c.Name,
                                DistrictId = c.DistrictId,
                                CommuneId = c.CommuneId,
                            };

                Market_model.status = 1;
                Market_model.items = _data.ToList();
                return Ok(Market_model);
            }
            catch (Exception ex)
            {
                Market_model.status = 0;
                Market_model.error = new ErrorModel()
                {
                    Code = ErrCode_Const.EXCEPTION_API,
                    Msg = ex.Message
                };
                return BadRequest(Market_model);
            }
        }

        [Route("loadbusinessline")]
        [HttpGet]
        public IActionResult LoadBusinessLine()
        {
            BaseModels<BusinessLine> model = new BaseModels<BusinessLine>();

            try
            {
                //Lấy Token, lấy model
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                IQueryable<BusinessLine> _data = _repoBussinessLine.FindAll().Where(x => !x.IsDel);

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

        [Route("find")]
        [HttpPost]
        public IActionResult ListItems_New([FromBody] QueryRequestBody query)//query truyền lên
        {

            BaseModels<MarketManagementModel> model = new BaseModels<MarketManagementModel>();
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

                Func<MarketManagementModel, object> _orderByExpression = x => x.MarketManagementId; //Khởi tạo mặc định sắp xếp dữ liệu
                Dictionary<string, Func<MarketManagementModel, object>> _sortableFields = new Dictionary<string, Func<MarketManagementModel, object>>   //Khởi tạo các trường để sắp xếp
                    {
                        { "TenNganhHangKinhDoanh", x => x.TenNganhHangKinhDoanh },
                        { "SoSap", x => x.BoothNumber },
                        { "MarketName", x => x.MarketName },
                        { "GiaTrongNhaLong", x => x.GiaTrongNhaLong },
                        { "GiaNgoaiNhaLong", x => x.GiaNgoaiNhaLong },
                        { "DeXuatGiaMoi", x => x.DeXuatGiaMoi },
                        { "Note", x => x.Note },
                    };
                if (query.Sort != null
                    && !string.IsNullOrEmpty(query.Sort.ColumnName)
                    && _sortableFields.ContainsKey(query.Sort.ColumnName))
                {
                    _orderBy_ASC = ("desc".Equals(query.Sort.Direction.ToLower()) ? false : true);    //Sắp xếp asc hoặc desc
                    _orderByExpression = _sortableFields[query.Sort.ColumnName]; //Trường cần sắp xếp
                }

                //Cách 1 dùng entity
                IQueryable<MarketManagementModel> _data = from mm in _repoMarketManagement._context.MarketManagements
                                                          where !mm.IsDel
                                                          join cm in _repoMarketManagement._context.CommercialManagements
                                                          on mm.MarketId equals cm.CommercialId into JoinCm
                                                          from com in JoinCm.DefaultIfEmpty()
                                                          join bl in _repoMarketManagement._context.BusinessLines
                                                          on mm.NganhHangKinhDoanh equals bl.BusinessLineId into JoinBl
                                                          from bul in JoinBl.DefaultIfEmpty()
                                                          select new MarketManagementModel
                                                          {
                                                              DistrictId = mm.DistrictId,
                                                              CommuneId = mm.CommuneId,
                                                              MarketId = mm.MarketId,
                                                              MarketName = com.Name,
                                                              MarketManagementId = mm.MarketManagementId,
                                                              NganhHangKinhDoanh = mm.NganhHangKinhDoanh,
                                                              TenNganhHangKinhDoanh = bul.BusinessLineName,
                                                              BoothNumber = mm.BoothNumber,
                                                              GiaTrongNhaLong = mm.GiaTrongNhaLong,
                                                              GiaNgoaiNhaLong = mm.GiaNgoaiNhaLong,
                                                              DeXuatGiaMoi = mm.DeXuatGiaMoi,
                                                              Note = mm.Note,
                                                              IsDel = mm.IsDel,
                                                          };

                if (query.SearchValue != null && query.SearchValue != "")
                {
                    _keywordSearch = query.SearchValue.Trim().ToLower();
                    _data = _data.Where(x =>
                       x.TenNganhHangKinhDoanh.ToLower().Contains(_keywordSearch)
                       || x.GiaTrongNhaLong.ToString().Contains(_keywordSearch)
                       || x.GiaNgoaiNhaLong.ToString().Contains(_keywordSearch)
                       || x.DeXuatGiaMoi.ToString().Contains(_keywordSearch)
                       || x.MarketName.ToLower().Contains(_keywordSearch)
                   );
                }

                if (query.Filter != null && query.Filter.ContainsKey("DistrictId"))
                {
                    _data = _data.Where(x => x.DistrictId.ToString() == query.Filter["DistrictId"]);
                }
                if (query.Filter != null && query.Filter.ContainsKey("CommuneId"))
                {
                    _data = _data.Where(x => x.CommuneId.ToString() == query.Filter["CommuneId"]);
                }
                if (query.Filter != null && query.Filter.ContainsKey("MarketId"))
                {
                    _data = _data.Where(x => x.MarketId.ToString() == query.Filter["MarketId"]);
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
            BaseModels<MarketManagementModel> model = new BaseModels<MarketManagementModel>();
            try
            {
                var result = _repoMarketManagement.FindById(id);
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
        public async Task<IActionResult> Update(MarketManagementModel data)
        {
            BaseModels<MarketManagementModel> model = new BaseModels<MarketManagementModel>();
            try
            {

                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                SystemLog datalog = new SystemLog();

                MarketManagement? SaveData = _repoMarketManagement._context.MarketManagements.Where(x => x.MarketManagementId == data.MarketManagementId && !x.IsDel).FirstOrDefault();
                if (SaveData != null)
                {
                    data.UpdateTime = DateTime.Now;
                    data.UpdateUserId = loginData.Userid;

                    await _repoMarketManagement.Update(data);
                    datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.UPDATE, NameCategory_Const.MARKET_MANAGERMENT, Action_Status.SUCCESS);
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
                    datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.UPDATE, NameCategory_Const.MARKET_MANAGERMENT, Action_Status.FAIL);
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
        public async Task<IActionResult> create(MarketManagementModel data)
        {
            BaseModels<MarketManagementModel> model = new BaseModels<MarketManagementModel>();
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                SystemLog datalog = new SystemLog();
                data.CreateUserId = loginData.Userid;
                data.CreateTime = DateTime.Now;


                await _repoMarketManagement.Insert(data);
                datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.CREATE, NameCategory_Const.MARKET_MANAGERMENT, Action_Status.SUCCESS);
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

        [HttpPut("deleteMarketManagement/{id}")]
        public async Task<IActionResult> deleteMarketManagement(Guid id)
        {
            BaseModels<MarketManagement> model = new BaseModels<MarketManagement>();
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                SystemLog datalog = new SystemLog();
                MarketManagement DeleteData = new MarketManagement();
                DeleteData.MarketManagementId = id;
                DeleteData.IsDel = true;
                await _repoMarketManagement.DeleteMarketManagement(DeleteData);
                datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.DELETE, NameCategory_Const.MARKET_MANAGERMENT, Action_Status.SUCCESS);
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

        [Route("getDataMarket")]
        [HttpGet]
        public object getDataMarket()
        {
            BaseModels<object> model = new BaseModels<object>();
            try
            {
                //Lấy Token, lấy model
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                //string id = "F982DDCF-FC12-4B7F-915A-356B20424800";
                //IQueryable<AdministrativeProceduresModel> CheckData = _repo._context.AdministrativeProcedures.Where(x => x.AdministrativeProceduresId == Guid.Parse(id) && !x.IsDel);
                //IQueryable<AdministrativeProcedure> _data = _repo._context.AdministrativeProcedures.Where(x => !x.IsDel).GroupBy(d => d.Status);

                //IQueryable<Category> _data = _repo._context.Categories.Where(x => x.CategoryTypeCode == "ADMINISTRATIVE_PROCEDURE_FIELD");
                //var query = from p in _repoCommercialManagement._context.CommercialManagements
                //            where !p.IsDel &&
                //            group p by p.Status
                //            into g
                //            select new { g.Key, Count = g.Count() };
                //model.status = 1;
                //model.data = query;
                //return Ok(model);

                var _data = from c in _repoCommercialManagement._context.CommercialManagements
                            join t in _repoCategory._context.Categories
                                on c.Type equals t.CategoryId
                            where (t.CategoryName == "Chợ" && !c.IsDel)
                            group c by c.DistrictId
                            into g
                            select new { g.Key, Count = g.Count() };
                //select new MarketModel
                //{
                //    MarketId = c.CommercialId,
                //    MarketName = c.Name,
                //    DistrictId = c.DistrictId,
                //    CommuneId = c.CommuneId,
                //};
                return _data;
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

        [HttpPost("ExportExcel")]
        public IActionResult ExportExcel([FromBody] QueryRequestBody query)
        {
            try
            {
                IQueryable<MarketManagementModel> _data = from mm in _repoMarketManagement._context.MarketManagements
                                                          where !mm.IsDel
                                                          join cm in _repoMarketManagement._context.CommercialManagements
                                                          on mm.MarketId equals cm.CommercialId into JoinCm
                                                          from com in JoinCm.DefaultIfEmpty()
                                                          join bl in _repoMarketManagement._context.BusinessLines
                                                          on mm.NganhHangKinhDoanh equals bl.BusinessLineId into JoinBl
                                                          from bul in JoinBl.DefaultIfEmpty()
                                                          join d in _repoMarketManagement._context.Districts
                                                          on mm.DistrictId equals d.DistrictId into JoinDis
                                                          from dis in JoinDis.DefaultIfEmpty()
                                                          join c in _repoMarketManagement._context.Communes
                                                          on mm.CommuneId equals c.CommuneId into JoinCom
                                                          from comu in JoinCom.DefaultIfEmpty()
                                                          select new MarketManagementModel
                                                          {
                                                              DistrictId = mm.DistrictId,
                                                              DistrictName = dis.DistrictName,
                                                              CommuneId = mm.CommuneId,
                                                              CommuneName = comu.CommuneName,
                                                              MarketId = mm.MarketId,
                                                              MarketName = com.Name,
                                                              MarketManagementId = mm.MarketManagementId,
                                                              NganhHangKinhDoanh = mm.NganhHangKinhDoanh,
                                                              TenNganhHangKinhDoanh = bul.BusinessLineName,
                                                              BoothNumber = mm.BoothNumber,
                                                              GiaTrongNhaLong = mm.GiaTrongNhaLong,
                                                              GiaNgoaiNhaLong = mm.GiaNgoaiNhaLong,
                                                              DeXuatGiaMoi = mm.DeXuatGiaMoi,
                                                              Note = mm.Note,
                                                              IsDel = mm.IsDel,
                                                          };

                string _keywordSearch = "";
                if (query.SearchValue != null && query.SearchValue != "")
                {
                    _keywordSearch = query.SearchValue.Trim().ToLower();
                    _data = _data.Where(x =>
                       x.TenNganhHangKinhDoanh.ToLower().Contains(_keywordSearch)
                       || x.GiaTrongNhaLong.ToString().Contains(_keywordSearch)
                       || x.GiaNgoaiNhaLong.ToString().Contains(_keywordSearch)
                       || x.DeXuatGiaMoi.ToString().Contains(_keywordSearch)
                       || x.MarketName.ToLower().Contains(_keywordSearch)
                   );
                }

                //string DistrictName = "";
                //string CommuneName = "";
                if (query.Filter != null && query.Filter.ContainsKey("DistrictId"))
                {
                    //DistrictName = _repoMarketManagement._context.Districts.Where(x => x.DistrictId.ToString() == query.Filter["DistrictId"]).Select(x => x.DistrictName).FirstOrDefault() ?? "";
                    _data = _data.Where(x => x.DistrictId.ToString() == query.Filter["DistrictId"]);
                }
                if (query.Filter != null && query.Filter.ContainsKey("CommuneId"))
                {
                    //DistrictName = _repoMarketManagement._context.Communes.Where(x => x.DistrictId.ToString() == query.Filter["CommuneId"]).Select(x => x.CommuneName).FirstOrDefault() ?? "";
                    _data = _data.Where(x => x.CommuneId.ToString() == query.Filter["CommuneId"]);
                }
                if (query.Filter != null && query.Filter.ContainsKey("MarketId"))
                {
                    _data = _data.Where(x => x.MarketId.ToString() == query.Filter["MarketId"]);
                }

                string Title = "BẢNG KÊ GIÁ THU PHÍ QUẢN LÝ CHỢ";

                List<MarketManagementModel> data = _data.ToList();

                if (!data.Any())
                {
                    return BadRequest();
                }

                using (var workbook = new XLWorkbook(@"Upload/Templates/Danhsachthongtincho.xlsx"))
                {
                    IXLWorksheet worksheet = workbook.Worksheets.Worksheet(1);
                    worksheet.Cell(1, 1).Value = Title;
                    int index = 5;

                    foreach (var item in data)
                    {
                        worksheet.Cell(index, 1).Value = index - 4;
                        worksheet.Cell(index, 2).Value = item.MarketName;
                        worksheet.Cell(index, 3).Value = item.DistrictName;
                        worksheet.Cell(index, 4).Value = item.CommuneName;
                        worksheet.Cell(index, 5).Value = item.BoothNumber;
                        worksheet.Cell(index, 6).Value = item.GiaTrongNhaLong;
                        worksheet.Cell(index, 7).Value = item.GiaNgoaiNhaLong;
                        worksheet.Cell(index, 8).Value = item.DeXuatGiaMoi;
                        worksheet.Cell(index, 9).Value = item.Note;
                        worksheet.Row(index).InsertRowsBelow(1);
                        index++;
                    }
                    worksheet.Row(index).Delete();

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
