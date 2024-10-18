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
using System.Net;
using ClosedXML.Excel;
using API_SoCongThuong.Logger;
using Newtonsoft.Json;
using System.Data;

namespace API_SoCongThuong.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CateRPPCrafttAncolForEconomicController : ControllerBase
    {
        private CateRPPCrafttAncolForEconomicRepo _repo;
        private CateRPProduceIndustlAncolRepo _repoIndustryAncol;
        private IConfiguration _configuration;
        private readonly ILogger<AsyncLogger> _logger;
        public SoHoa_SoCongThuongContext _context;
        private AsyncLogger _asyncLogger;
        // private CateRPProduceIndustlAncolRepo _repoIndustryAncol;

        public CateRPPCrafttAncolForEconomicController(SoHoa_SoCongThuongContext context, IConfiguration configuration, ILogger<AsyncLogger> logger)
        {
            _repo = new CateRPPCrafttAncolForEconomicRepo(context);
            _repoIndustryAncol = new CateRPProduceIndustlAncolRepo(context);
            _logger = logger;
            _context = context;
            _asyncLogger = new AsyncLogger(_logger, _context);
            _configuration = configuration;
        }

        [Route("find")]
        [HttpPost]
        public IActionResult ListItems_New([FromBody] QueryRequestBody query)//query truyền lên
        {

            BaseModels<CateReportProduceCrafttAncolForEconomicModel> model = new BaseModels<CateReportProduceCrafttAncolForEconomicModel>();
            string _keywordSearch = ""; //Keyword tìm kiếm
            bool _orderBy_ASC = false;  //Khởi tạo sắp xếp dữ liệu acs hoặc desc khi tìm kiếm
            try
            {
                ////Lấy Token, lấy model
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }

                Func<CateReportProduceCrafttAncolForEconomicModel, object> _orderByExpression = x => x.CateReportProduceCrafttAncolForEconomicId; //Khởi tạo mặc định sắp xếp dữ liệu
                Dictionary<string, Func<CateReportProduceCrafttAncolForEconomicModel, object>> _sortableFields = new Dictionary<string, Func<CateReportProduceCrafttAncolForEconomicModel, object>>   //Khởi tạo các trường để sắp xếp
                    {
                        { "AlcoholBusinessName", x => x.AlcoholBusinessName },
                        { "Address", x => x.Address },
                        { "DistrictId", x=> x.DistrictId},
                        { "PhoneNumber", x => x.PhoneNumber },
                        { "Quantity", x => x.Quantity },
                        { "YearReport", x => x.YearReport },
                        { "Representative", x => x.Representative },
                        { "LicenseDateDisplay", x => x.LicenseDateDisplay },
                        { "LicenseCode", x => x.LicenseCode },
                        { "TypeofWine", x => x.TypeofWine },
                        { "QuantityConsume", x => x.QuantityConsume },
                        { "CreateName", x => x.CreateUserId },
                        { "CreateTimeDisplay", x => x.CreateTime },
                        { "IsAction", x => x.IsAction },
                    };
                if (query.Sort != null
                    && !string.IsNullOrEmpty(query.Sort.ColumnName)
                    && _sortableFields.ContainsKey(query.Sort.ColumnName))
                {
                    _orderBy_ASC = ("desc".Equals(query.Sort.Direction.ToLower()) ? false : true);    //Sắp xếp asc hoặc desc
                    _orderByExpression = _sortableFields[query.Sort.ColumnName]; //Trường cần sắp xếp
                }

                //Linq Entity Syntax
                //IQueryable<CateReportProduceCrafttAncolForEconomicModel> _data = _repo._context.CateReportProduceCrafttAncolForEconomics.Where(x => !x.IsDel)
                //    .GroupJoin(_repo._context.Districts,
                //             x => x.DistrictId,
                //             d => d.DistrictId,
                //             (x, d) => new { x, d }).SelectMany(r => r.d.DefaultIfEmpty(), (cc, u) => new { cc, u })
                //    .GroupJoin(_repo._context.Users,
                //    x1 => x1.u.CreateUserId,
                //    u => u.UserId,
                //     (x1, u) => new { x1, u }).SelectMany(result => result.u.DefaultIfEmpty(), (info, us)
                //     => new CateReportProduceCrafttAncolForEconomicModel
                //     {
                //         CateReportProduceCrafttAncolForEconomicId = info.x1.cc.x.CateReportProduceCrafttAncolForEconomicId,
                //         AlcoholBusinessName = info.x1.cc.x.AlcoholBusinessName,
                //         PhoneNumber = info.x1.cc.x.PhoneNumber,
                //         Address = info.x1.cc.x.Address,
                //         DistrictId = info.x1.cc.x.DistrictId,
                //         DistrictName = info.x1.u.DistrictName,
                //         QuantityConsume = info.x1.cc.x.QuantityConsume,
                //         Representative = info.x1.cc.x.Representative,
                //         ProductionForm = info.x1.cc.x.ProductionForm,
                //         Quantity = info.x1.cc.x.Quantity,
                //         TypeofWine = info.x1.cc.x.TypeofWine,
                //         LicenseCode = info.x1.cc.x.LicenseCode,
                //         LicenseDateDisplay = info.x1.cc.x.LicenseDate.ToString("dd'/'MM'/'yyyy"),
                //         LicenseDate = info.x1.cc.x.LicenseDate,
                //         CreateName = us.FullName,
                //         CreateTimeDisplay = info.x1.cc.x.CreateTime.ToString("dd/MM/yyyy hh:mm"),
                //         BusinessId = info.x1.cc.x.BusinessId,
                //         YearReport = info.x1.cc.x.YearReport,
                //     });

                //Linq Query Syntax
                IQueryable<CateReportProduceCrafttAncolForEconomicModel> _data = (from cc in _repo._context.CateReportProduceCrafttAncolForEconomics
                                                                                  where !cc.IsDel
                                                                                  join u in _repo._context.Users on cc.CreateUserId equals u.UserId into JoinUser
                                                                                  from us in JoinUser.DefaultIfEmpty()
                                                                                  join b in _repo._context.Businesses on cc.BusinessId equals b.BusinessId into JoinBusinesses
                                                                                  from bu in JoinBusinesses.DefaultIfEmpty()
                                                                                  join d in _repo._context.Districts on bu.DistrictId equals d.DistrictId into JoinDistricts
                                                                                  from di in JoinDistricts.DefaultIfEmpty()
                                                                                  select new CateReportProduceCrafttAncolForEconomicModel
                                                                                  {
                                                                                      CateReportProduceCrafttAncolForEconomicId = cc.CateReportProduceCrafttAncolForEconomicId,

                                                                                      AlcoholBusinessName = bu.BusinessNameVi,
                                                                                      PhoneNumber = bu.SoDienThoai,
                                                                                      Address = bu.DiaChiTruSo,
                                                                                      LicenseCode = bu.GiayPhepSanXuat ?? "",
                                                                                      LicenseDateDisplay = bu.NgayCapPhep.HasValue ? bu.NgayCapPhep.Value.ToString("dd'/'MM'/'yyyy") : "",
                                                                                      Representative = bu.NguoiDaiDien,
                                                                                      DistrictName = di.DistrictName,
                                                                                      Quantity = cc.Quantity,
                                                                                      QuantityConsume = cc.QuantityConsume,
                                                                                      TypeofWine = cc.TypeofWine,
                                                                                      YearReport = cc.YearReport,
                                                                                      CreateName = us.FullName,
                                                                                      CreateTimeDisplay = cc.CreateTime.ToString("dd/MM/yyyy hh:mm")
                                                                                  })
                                                                                  .ToList().AsQueryable();

                if (query.SearchValue != null && query.SearchValue != "")
                {
                    _keywordSearch = query.SearchValue.Trim().ToLower();
                    _data = _data.Where(x => x.AlcoholBusinessName.ToLower().Contains(_keywordSearch)
                    || x.Address.ToLower().Contains(_keywordSearch)
                    || x.PhoneNumber.ToLower().Contains(_keywordSearch)
                    || x.Quantity.ToString().Contains(_keywordSearch)
                    || x.TypeofWine.ToLower().Contains(_keywordSearch)
                    || x.LicenseCode.ToLower().Contains(_keywordSearch)
                    );
                }

                //Filter
                if (query.Filter != null && query.Filter.ContainsKey("YearReport"))
                {
                    _data = _data.Where(x => x.YearReport.ToString().Equals(string.Join("", query.Filter["YearReport"])));
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

        [HttpPost()]
        public async Task<IActionResult> Create(CateReportProduceCrafttAncolForEconomicModel data)
        {
            BaseModels<CateReportProduceCrafttAncolForEconomicModel> model = new BaseModels<CateReportProduceCrafttAncolForEconomicModel>();
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }

                var util = new Ulities();
                data = util.TrimModel(data);
                SystemLog datalog = new SystemLog();

                if (string.IsNullOrEmpty(data.TypeofWine))
                {
                    model.status = 0;
                    model.error = new ErrorModel()
                    {
                        Code = ErrCode_Const.PROPERTY_IS_NULL_OR_EMPTY
                    };

                    datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.CREATE, NameCategory_Const.CATE_RP_CRAFT_ANCOL_FOR_ECONOMIC, Action_Status.FAIL);
                    _asyncLogger.LogInfo(JsonConvert.SerializeObject(datalog));
                    return BadRequest(model);
                }

                data.CreateTime = DateTime.Now;
                data.CreateUserId = loginData.Userid;
                await _repo.Insert(data);

                datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.CREATE, NameCategory_Const.CATE_RP_CRAFT_ANCOL_FOR_ECONOMIC, Action_Status.SUCCESS);
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
        public async Task<IActionResult> Update(CateReportProduceCrafttAncolForEconomicModel data)
        {
            BaseModels<CateReportProduceCrafttAncolForEconomicModel> model = new BaseModels<CateReportProduceCrafttAncolForEconomicModel>();
            try
            {

                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }

                var util = new Ulities();
                data = util.TrimModel(data);
                SystemLog datalog = new SystemLog();

                var CheckData = _repo.FindById(data.CateReportProduceCrafttAncolForEconomicId);
                if (CheckData == null)
                {
                    //chỗ này không tồn tại id
                    model.status = 0;
                    model.error = new ErrorModel()
                    {
                        Code = ErrCode_Const.PROPERTY_IS_NULL_OR_EMPTY
                    };

                    datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.UPDATE, NameCategory_Const.CATE_RP_CRAFT_ANCOL_FOR_ECONOMIC, Action_Status.FAIL);
                    _asyncLogger.LogInfo(JsonConvert.SerializeObject(datalog));
                    return BadRequest(model);
                }
                else
                {
                    if (string.IsNullOrEmpty(data.TypeofWine))
                    {
                        model.status = 0;
                        model.error = new ErrorModel()
                        {
                            Code = ErrCode_Const.PROPERTY_IS_NULL_OR_EMPTY
                        };

                        datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.UPDATE, NameCategory_Const.CATE_RP_CRAFT_ANCOL_FOR_ECONOMIC, Action_Status.FAIL);
                        _asyncLogger.LogInfo(JsonConvert.SerializeObject(datalog));
                        return BadRequest(model);
                    }

                    data.UpdateTime = DateTime.Now;
                    data.UpdateUserId = loginData.Userid;
                    await _repo.Update(data);

                    datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.UPDATE, NameCategory_Const.CATE_RP_CRAFT_ANCOL_FOR_ECONOMIC, Action_Status.SUCCESS);
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
            BaseModels<CateReportProduceCrafttAncolForEconomicModel> model = new BaseModels<CateReportProduceCrafttAncolForEconomicModel>();
            try
            {
                var info = _repo.FindById(id);
                if (info == null)
                    return NotFound(ErrMsg_Const.GetMsg(ErrCode_Const.CANNOT_FIND_DATA_BY_QUERY));

                CateReportProduceCrafttAncolForEconomicModel result = new CateReportProduceCrafttAncolForEconomicModel()
                {
                    CateReportProduceCrafttAncolForEconomicId = info.CateReportProduceCrafttAncolForEconomicId,
                    Quantity = info.Quantity,
                    QuantityConsume = info.QuantityConsume,
                    TypeofWine = info.TypeofWine,
                    BusinessId = info.BusinessId,
                    YearReport = info.YearReport
                };

                model.status = 1;
                model.data = result;
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
        [Route("deletes")]
        [HttpPut()]
        public async Task<IActionResult> deletes(List<Guid> IdRemoves)
        {
            BaseModels<object> model = new BaseModels<object>();
            try
            {
                await _repo.Deletes(IdRemoves);
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

                await _repo.Delete(id);

                datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.DELETE, NameCategory_Const.CATE_RP_CRAFT_ANCOL_FOR_ECONOMIC, Action_Status.SUCCESS);
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

        [Route("getDataAlcoholProduct")]
        [HttpGet]

        public object getDataAlcoholProduct()
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

                var craftAncol = _repo._context.CateReportProduceCrafttAncolForEconomics.Where(x => !x.IsDel).Sum(x => x.Quantity);
                var industryAncol = _repoIndustryAncol._context.CateReportProduceIndustlAncols.Where(x => !x.IsDel).Sum(x => x.QuantityProduction);

                var result = new { craftAncol, industryAncol };
                return result;
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

        [HttpPost("Export")]
        public IActionResult Export([FromBody] QueryRequestBody query)
        {
            var dataForEconomic = _repo.FindDataForEconomic(query);
            var dataForSoldAncols = _repo.FindDataForSoldAncols(query);

            if (!dataForEconomic.Any() && !dataForSoldAncols.Any())
            {
                return BadRequest();
            }

            int currentYear = DateTime.Now.Year;

            if (query.Filter != null && query.Filter.ContainsKey("YearReport") && !string.IsNullOrEmpty(query.Filter["YearReport"]))
            {
                currentYear = int.Parse(query.Filter["YearReport"]);
            };

            try
            {
                using (var workbook = new XLWorkbook(@"Upload/Templates/Baocaosanxuatruouthucongmucdichkinhdoanh.xlsx"))
                {
                    IXLWorksheet worksheet = workbook.Worksheets.Worksheet(1);
                    worksheet.Cell(1, 1).Value = $"Báo cáo tình hình sản xuất kinh doanh trên địa bàn năm {currentYear}";
                    int indexTable1 = 10;
                    int indexTable2 = 18;

                    int row = 1;
                    foreach (var item in dataForSoldAncols)
                    {
                        worksheet.Cell(indexTable2, 1).Value = row;
                        worksheet.Cell(indexTable2, 2).Value = item.AlcoholBusinessName.ToUpper();
                        worksheet.Cell(indexTable2, 3).Value = item.Address;
                        worksheet.Cell(indexTable2, 4).Value = item.PhoneNumber;
                        worksheet.Cell(indexTable2, 5).Value = item.LicenseCode;
                        worksheet.Cell(indexTable2, 6).Value = item.LicenseDate;
                        worksheet.Cell(indexTable2, 7).Value = item.QuantityBoughtOfYear;
                        worksheet.Cell(indexTable2, 8).Value = item.TotalPriceBoughtOfYear;
                        worksheet.Cell(indexTable2, 9).Value = item.QuantitySoldOfYear;
                        worksheet.Cell(indexTable2, 10).Value = item.TotalPriceSoldOfYear;

                        if(row < dataForSoldAncols.Count)
                        {
                            worksheet.Row(indexTable2).InsertRowsBelow(1);
                            indexTable2++;
                            row++;
                        }
                    }

                    row = 1;
                    foreach (var item in dataForEconomic)
                    {
                        worksheet.Cell(indexTable1, 1).Value = row;
                        worksheet.Cell(indexTable1, 2).Value = item.AlcoholBusinessName.ToUpper();
                        worksheet.Cell(indexTable1, 3).Value = item.Address;
                        worksheet.Cell(indexTable1, 4).Value = item.PhoneNumber;
                        worksheet.Cell(indexTable1, 5).Value = item.LicenseCode;
                        worksheet.Cell(indexTable1, 6).Value = item.LicenseDate;
                        worksheet.Cell(indexTable1, 7).Value = item.TypeofWine;
                        worksheet.Cell(indexTable1, 8).Value = item.Quantity;
                        worksheet.Cell(indexTable1, 9).Value = item.QuantityConsume;

                        if (row < dataForEconomic.Count)
                        {
                            worksheet.Row(indexTable1).InsertRowsBelow(1);
                            indexTable1++;
                            row++;
                        }

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
