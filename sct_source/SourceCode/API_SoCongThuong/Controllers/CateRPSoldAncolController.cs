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
using ClosedXML.Excel;
using API_SoCongThuong.Logger;
using Newtonsoft.Json;
using System.Data;

namespace API_SoCongThuong.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CateRPSoldAncolController : ControllerBase
    {
        private CateRPSoldAncolRepo _repo;
        private IConfiguration _configuration;
        private readonly ILogger<AsyncLogger> _logger;
        public SoHoa_SoCongThuongContext _context;
        private AsyncLogger _asyncLogger;

        public CateRPSoldAncolController(SoHoa_SoCongThuongContext context, IConfiguration configuration, ILogger<AsyncLogger> logger)
        {
            _repo = new CateRPSoldAncolRepo(context);
            _logger = logger;
            _context = context;
            _asyncLogger = new AsyncLogger(_logger, _context);
            _configuration = configuration;
        }

        [Route("find")]
        [HttpPost]
        public IActionResult ListItems_New([FromBody] QueryRequestBody query)
        {
            BaseModels<CateReportSoldAncolModel> model = new BaseModels<CateReportSoldAncolModel>();
            string _keywordSearch = "";
            bool _orderBy_ASC = false;
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }

                Func<CateReportSoldAncolModel, object> _orderByExpression = x => x.CateReportSoldAncolId;
                Dictionary<string, Func<CateReportSoldAncolModel, object>> _sortableFields = new Dictionary<string, Func<CateReportSoldAncolModel, object>>
                    {
                        { "AlcoholBusinessName", x => x.AlcoholBusinessName },
                        { "Address", x => x.Address },
                        { "PhoneNumber", x => x.PhoneNumber },
                        { "LicenseCode", x => x.LicenseCode },
                        { "LicenseDateDisplay", x => x.LicenseDate },
                        { "QuantityBoughtOfYear", x => x.QuantityBoughtOfYear },
                        { "QuantitySoldOfYear", x => x.QuantitySoldOfYear },
                        { "TotalPriceBoughtOfYear", x => x.TotalPriceBoughtOfYear },
                        { "TotalPriceSoldOfYear", x => x.TotalPriceSoldOfYear },
                        { "CreateName", x => x.CreateUserId },
                        { "CreateTimeDisplay", x => x.CreateTime },
                        { "IsAction", x => x.IsAction },
                    };
                if (query.Sort != null && !string.IsNullOrEmpty(query.Sort.ColumnName) && _sortableFields.ContainsKey(query.Sort.ColumnName))
                {
                    _orderBy_ASC = ("desc".Equals(query.Sort.Direction.ToLower()) ? false : true);
                    _orderByExpression = _sortableFields[query.Sort.ColumnName];
                }

                //IQueryable<CateReportSoldAncolModel> _data = _repo._context.CateReportSoldAncols
                //.Where(x => !x.IsDel)
                //.GroupJoin(
                //    _repo._context.Users,
                //    cc => cc.CreateUserId,
                //    u => u.UserId,
                //    (cc, u) => new { cc, u })
                //.SelectMany(
                //    result => result.u.DefaultIfEmpty(),
                //    (info, us) => new CateReportSoldAncolModel
                //    {
                //        CateReportSoldAncolId = info.cc.CateReportSoldAncolId,
                //        AlcoholBusinessName = info.cc.AlcoholBusinessName,
                //        PhoneNumber = info.cc.PhoneNumber,
                //        Address = info.cc.Address,
                //        LicenseCode = info.cc.LicenseCode,
                //        LicenseDateDisplay = string.Format("{0:dd/MM/yyyy}", info.cc.LicenseDate),
                //        ProductionForm = info.cc.ProductionForm,
                //        QuantityBoughtOfYear = info.cc.QuantityBoughtOfYear,
                //        QuantitySoldOfYear = info.cc.QuantitySoldOfYear,
                //        TotalPriceBoughtOfYear = info.cc.TotalPriceBoughtOfYear,
                //        TotalPriceSoldOfYear = info.cc.TotalPriceSoldOfYear,
                //        CreateName = us.FullName,
                //        CreateTimeDisplay = info.cc.CreateTime.ToString("dd/MM/yyyy hh:mm"),
                //        BusinessId = info.cc.BusinessId,
                //        YearId = info.cc.Year,
                //    });

                //Linq Query Syntax
                IQueryable<CateReportSoldAncolModel> _data = (from cc in _repo._context.CateReportSoldAncols
                                                              where !cc.IsDel
                                                              join u in _repo._context.Users on cc.CreateUserId equals u.UserId into cuGroup
                                                              from result in cuGroup.DefaultIfEmpty()
                                                              join b in _repo._context.Businesses on cc.BusinessId equals b.BusinessId into cuBusiness
                                                              from bu in cuBusiness.DefaultIfEmpty()
                                                              select new CateReportSoldAncolModel
                                                              {
                                                                  CateReportSoldAncolId = cc.CateReportSoldAncolId,

                                                                  BusinessId = cc.BusinessId,
                                                                  AlcoholBusinessName = bu.BusinessNameVi,
                                                                  Address = bu.DiaChiTruSo,
                                                                  PhoneNumber = bu.SoDienThoai,
                                                                  LicenseCode = bu.GiayPhepSanXuat ?? "",
                                                                  LicenseDateDisplay = bu.NgayCapPhep.HasValue ? bu.NgayCapPhep.Value.ToString("dd'/'MM'/'yyyy") : "",

                                                                  QuantityBoughtOfYear = cc.QuantityBoughtOfYear,
                                                                  QuantitySoldOfYear = cc.QuantitySoldOfYear,
                                                                  TotalPriceBoughtOfYear = cc.TotalPriceBoughtOfYear,
                                                                  TotalPriceSoldOfYear = cc.TotalPriceSoldOfYear,
                                                                  CreateName = result.FullName,
                                                                  CreateTimeDisplay = cc.CreateTime.ToString("dd/MM/yyyy hh:mm"),
                                                                  YearId = cc.Year,
                                                              }).ToList().AsQueryable();

                if (query.SearchValue != null && query.SearchValue != "")
                {
                    _keywordSearch = query.SearchValue.Trim().ToLower();
                    _data = _data.Where(x => x.AlcoholBusinessName.ToLower().Contains(_keywordSearch)
                    || x.Address.ToLower().Contains(_keywordSearch)
                    || x.PhoneNumber.ToLower().Contains(_keywordSearch)
                    || x.QuantityBoughtOfYear.ToString().Contains(_keywordSearch)
                    || x.QuantitySoldOfYear.ToString().Contains(_keywordSearch)
                    || x.TotalPriceBoughtOfYear.ToString().Contains(_keywordSearch)
                    || x.TotalPriceSoldOfYear.ToString().Contains(_keywordSearch)
                    || x.LicenseCode.ToLower().Contains(_keywordSearch)
                    );
                }

                if (query.Filter != null && query.Filter.ContainsKey("YearReport"))
                {
                    _data = _data.Where(x => x.YearId.ToString() == query.Filter["YearReport"]);
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
        public async Task<IActionResult> Create(CateReportSoldAncolModel data)
        {
            BaseModels<CateReportSoldAncolModel> model = new BaseModels<CateReportSoldAncolModel>();
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

                data.CreateUserId = loginData.Userid;
                await _repo.Insert(data);
                datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.CREATE, NameCategory_Const.CATE_RP_SOLD_ANCOL, Action_Status.SUCCESS);
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
        public async Task<IActionResult> Update(CateReportSoldAncolModel data)
        {
            BaseModels<CateReportSoldAncolModel> model = new BaseModels<CateReportSoldAncolModel>();
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
                var CheckData = _repo.FindById(data.CateReportSoldAncolId);
                if (CheckData == null)
                {
                    //chỗ này không tồn tại id
                    model.status = 0;
                    model.error = new ErrorModel()
                    {
                        Code = ErrCode_Const.PROPERTY_IS_NULL_OR_EMPTY
                    };
                    datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.UPDATE, NameCategory_Const.CATE_RP_SOLD_ANCOL, Action_Status.FAIL);
                    _asyncLogger.LogInfo(JsonConvert.SerializeObject(datalog));
                    return BadRequest(model);
                }
                else
                {
                    data.UpdateTime = DateTime.Now;
                    data.UpdateUserId = loginData.Userid;
                    await _repo.Update(data);
                    datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.UPDATE, NameCategory_Const.CATE_RP_SOLD_ANCOL, Action_Status.SUCCESS);
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
            BaseModels<CateReportSoldAncolModel> model = new BaseModels<CateReportSoldAncolModel>();
            try
            {
                var info = _repo.FindById(id);
                if (info == null)
                    return NotFound(ErrMsg_Const.GetMsg(ErrCode_Const.CANNOT_FIND_DATA_BY_QUERY));

                CateReportSoldAncolModel result = new CateReportSoldAncolModel()
                {
                    CateReportSoldAncolId = info.CateReportSoldAncolId,
                    QuantityBoughtOfYear = info.QuantityBoughtOfYear,
                    QuantitySoldOfYear = info.QuantitySoldOfYear,
                    TotalPriceBoughtOfYear = info.TotalPriceBoughtOfYear,
                    TotalPriceSoldOfYear = info.TotalPriceSoldOfYear,
                    BusinessId = info.BusinessId,
                    YearId = info.Year
                };

                //Set data cho base model
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
                datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.DELETE, NameCategory_Const.CATE_RP_SOLD_ANCOL, Action_Status.SUCCESS);
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

        [HttpPost("Export")]
        public IActionResult Export([FromBody] QueryRequestBody query)
        {
            var data = _repo.FindData(query);

            if (!data.Any())
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
                using (var workbook = new XLWorkbook(@"Upload/Templates/Baocaotinhhinhbanleruou.xlsx"))
                {
                    IXLWorksheet worksheet = workbook.Worksheets.Worksheet(1);
                    worksheet.Cell(1, 1).Value = $"Báo cáo tình hình sản xuất kinh doanh trên địa bàn năm {currentYear}";

                    int row = 1;
                    int index = 12;

                    foreach (var item in data)
                    {
                        if (row == 1)
                        {
                            worksheet.Cell(index, 1).Value = row;
                            worksheet.Cell(index, 2).Value = item.AlcoholBusinessName.ToUpper();
                            worksheet.Cell(index, 3).Value = item.Address;
                            worksheet.Cell(index, 4).Value = item.PhoneNumber;
                            worksheet.Cell(index, 5).Value = item.LicenseCode;
                            worksheet.Cell(index, 6).Value = item.LicenseDate;
                            worksheet.Cell(index, 7).Value = item.QuantityBoughtOfYear;
                            worksheet.Cell(index, 8).Value = item.TotalPriceBoughtOfYear;
                            worksheet.Cell(index, 9).Value = item.QuantitySoldOfYear;
                            worksheet.Cell(index, 10).Value = item.TotalPriceSoldOfYear;

                            index++;
                            row++;
                        }
                        else
                        {
                            var addrow = worksheet.Row(index - 1);
                            addrow.InsertRowsBelow(1);

                            worksheet.Cell(index, 1).Value = row;
                            worksheet.Cell(index, 2).Value = item.AlcoholBusinessName.ToUpper();
                            worksheet.Cell(index, 3).Value = item.Address;
                            worksheet.Cell(index, 4).Value = item.PhoneNumber;
                            worksheet.Cell(index, 5).Value = item.LicenseCode;
                            worksheet.Cell(index, 6).Value = item.LicenseDate;
                            worksheet.Cell(index, 7).Value = item.QuantityBoughtOfYear;
                            worksheet.Cell(index, 8).Value = item.TotalPriceBoughtOfYear;
                            worksheet.Cell(index, 9).Value = item.QuantitySoldOfYear;
                            worksheet.Cell(index, 10).Value = item.TotalPriceSoldOfYear;

                            index++;
                            row++;
                        }
                    }

                    var delRow = worksheet.Row(index);
                    delRow.Delete();

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
