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
    public class CateRPSAncolForFactoryController : ControllerBase
    {
        private CateRPSAncolForFactoryRepo _repo;
        private IConfiguration _configuration;
        private readonly ILogger<AsyncLogger> _logger;
        public SoHoa_SoCongThuongContext _context;
        private AsyncLogger _asyncLogger;

        public CateRPSAncolForFactoryController(SoHoa_SoCongThuongContext context, IConfiguration configuration, ILogger<AsyncLogger> logger
)
        {
            _repo = new CateRPSAncolForFactoryRepo(context);
            _logger = logger;
            _context = context;
            _asyncLogger = new AsyncLogger(_logger, _context);
            _configuration = configuration;
        }

        [Route("find")]
        [HttpPost]
        public IActionResult ListItems_New([FromBody] QueryRequestBody query)
        {
            BaseModels<CateReportSoldAncolForFactoryLicenseModel> model = new BaseModels<CateReportSoldAncolForFactoryLicenseModel>();
            string _keywordSearch = "";
            bool _orderBy_ASC = false;
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }

                Func<CateReportSoldAncolForFactoryLicenseModel, object> _orderByExpression = x => x.CateReportSoldAncolForFactoryLicenseId;
                Dictionary<string, Func<CateReportSoldAncolForFactoryLicenseModel, object>> _sortableFields = new Dictionary<string, Func<CateReportSoldAncolForFactoryLicenseModel, object>>
                    {
                        { "OrganizationName", x => x.OrganizationName },
                        { "Address", x => x.Address },
                        { "PhoneNumber", x => x.PhoneNumber },
                        { "Quantity", x => x.Quantity },
                        { "WineFactory", x => x.WineFactory },
                        { "CreateName", x => x.CreateUserId },
                        { "CreateTimeDisplay", x => x.CreateTime },
                        { "IsAction", x => x.IsAction },
                        { "YearReport", x => x.YearReport }
                    };
                if (query.Sort != null && !string.IsNullOrEmpty(query.Sort.ColumnName) && _sortableFields.ContainsKey(query.Sort.ColumnName))
                {
                    _orderBy_ASC = ("desc".Equals(query.Sort.Direction.ToLower()) ? false : true);
                    _orderByExpression = _sortableFields[query.Sort.ColumnName];
                }

                IQueryable<CateReportSoldAncolForFactoryLicenseModel> _data = _repo._context.CateReportSoldAncolForFactoryLicenses
                .Where(x => !x.IsDel).GroupJoin(
                    _repo._context.Users,
                    cc => cc.CreateUserId,
                    u => u.UserId,
                    (cc, u) => new { cc, u })
                .SelectMany(
                    result => result.u.DefaultIfEmpty(),
                    (info, us) => new { info, us })
                .GroupJoin(
                    _repo._context.Businesses, ifo => ifo.info.cc.BusinessId,
                    bu => bu.BusinessId,
                    (ifo, bu) => new { ifo, bu })
                .SelectMany(
                    result => result.bu.DefaultIfEmpty(),
                    (ifo1, bu) => new CateReportSoldAncolForFactoryLicenseModel
                    {
                        CateReportSoldAncolForFactoryLicenseId = ifo1.ifo.info.cc.CateReportSoldAncolForFactoryLicenseId,
                        OrganizationName = bu.BusinessNameVi,
                        PhoneNumber = bu.SoDienThoai,
                        Address = bu.DiaChiTruSo,
                        Quantity = ifo1.ifo.info.cc.Quantity,
                        WineFactory = ifo1.ifo.info.cc.WineFactory,
                        CreateName = ifo1.ifo.us.FullName,
                        BusinessId = ifo1.ifo.info.cc.BusinessId,
                        YearReport = ifo1.ifo.info.cc.YearReport,
                        CreateTimeDisplay = ifo1.ifo.info.cc.CreateTime.ToString("dd/MM/yyyy hh:mm")
                    }
                ).ToList().AsQueryable();

                if (query.SearchValue != null && query.SearchValue != "")
                {
                    _keywordSearch = query.SearchValue.Trim().ToLower();
                    _data = _data.Where(x => x.OrganizationName.ToLower().Contains(_keywordSearch)
                    || x.Address.ToLower().Contains(_keywordSearch)
                    || x.PhoneNumber.ToLower().Contains(_keywordSearch)
                    || x.Quantity.ToString().Contains(_keywordSearch)
                    || x.WineFactory.ToLower().Contains(_keywordSearch));
                }

                if (query.Filter != null && query.Filter.ContainsKey("YearReport"))
                {
                    _data = _data.Where(x => x.YearReport.ToString() == query.Filter["YearReport"]);
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
        public async Task<IActionResult> Create(CateReportSoldAncolForFactoryLicenseModel data)
        {
            BaseModels<CateReportSoldAncolForFactoryLicenseModel> model = new BaseModels<CateReportSoldAncolForFactoryLicenseModel>();
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                SystemLog datalog = new SystemLog();

                if (string.IsNullOrEmpty(data.TypeofWine))
                {
                    model.status = 0;
                    model.error = new ErrorModel()
                    {
                        Code = ErrCode_Const.PROPERTY_IS_NULL_OR_EMPTY
                    };

                    datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.CREATE, NameCategory_Const.CATE_RP_ANCOL_FOR_FACTORY, Action_Status.FAIL);
                    _asyncLogger.LogInfo(JsonConvert.SerializeObject(datalog));
                    return BadRequest(model);
                }

                if (string.IsNullOrEmpty(data.WineFactory))
                {
                    model.status = 0;
                    model.error = new ErrorModel()
                    {
                        Code = ErrCode_Const.PROPERTY_IS_NULL_OR_EMPTY
                    };

                    datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.CREATE, NameCategory_Const.CATE_RP_ANCOL_FOR_FACTORY, Action_Status.FAIL);
                    _asyncLogger.LogInfo(JsonConvert.SerializeObject(datalog));
                    return BadRequest(model);
                }

                var check = _repo._context.CateReportSoldAncolForFactoryLicenses.Where(x => x.YearReport == data.YearReport && x.BusinessId == data.BusinessId && !x.IsDel).Any();
                if (check)
                {
                    model.status = 0;
                    model.error = new ErrorModel()
                    {
                        Code = ErrCode_Const.SQL_INSERT_FAILED,
                        Msg = "Báo cáo đã tồn tại"
                    };

                    datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.CREATE, NameCategory_Const.CATE_RP_ANCOL_FOR_FACTORY, Action_Status.SUCCESS);
                    _asyncLogger.LogInfo(JsonConvert.SerializeObject(datalog));
                    return BadRequest(model);
                }

                if (data.YearReport == 0)
                {
                    model.status = 0;
                    model.error = new ErrorModel()
                    {
                        Code = ErrCode_Const.PROPERTY_IS_NULL_OR_EMPTY
                    };

                    datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.CREATE, NameCategory_Const.CATE_RP_ANCOL_FOR_FACTORY, Action_Status.FAIL);
                    _asyncLogger.LogInfo(JsonConvert.SerializeObject(datalog));
                    return BadRequest(model);
                }

                data.CreateUserId = loginData.Userid;
                data.CreateTime = DateTime.Now;
                data.CreateTimeDisplay = data.CreateTime.ToString("'dd'/'MM'/'yyyy'");
                data.CreateName = loginData.Fullname;

                await _repo.Insert(data);

                datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.CREATE, NameCategory_Const.CATE_RP_ANCOL_FOR_FACTORY, Action_Status.SUCCESS);
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
        public async Task<IActionResult> Update(CateReportSoldAncolForFactoryLicenseModel data)
        {
            BaseModels<CateReportSoldAncolForFactoryLicense> model = new BaseModels<CateReportSoldAncolForFactoryLicense>();
            try
            {

                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                SystemLog datalog = new SystemLog();
                var CheckData = _repo.FindById(data.CateReportSoldAncolForFactoryLicenseId);
                if (CheckData == null)
                {
                    model.status = 0;
                    model.error = new ErrorModel()
                    {
                        Code = ErrCode_Const.PROPERTY_IS_NULL_OR_EMPTY
                    };

                    datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.UPDATE, NameCategory_Const.CATE_RP_ANCOL_FOR_FACTORY, Action_Status.FAIL);
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

                        datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.UPDATE, NameCategory_Const.CATE_RP_ANCOL_FOR_FACTORY, Action_Status.FAIL);
                        _asyncLogger.LogInfo(JsonConvert.SerializeObject(datalog));
                        return BadRequest(model);
                    }

                    if (string.IsNullOrEmpty(data.WineFactory))
                    {
                        model.status = 0;
                        model.error = new ErrorModel()
                        {
                            Code = ErrCode_Const.PROPERTY_IS_NULL_OR_EMPTY
                        };

                        datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.UPDATE, NameCategory_Const.CATE_RP_ANCOL_FOR_FACTORY, Action_Status.FAIL);
                        _asyncLogger.LogInfo(JsonConvert.SerializeObject(datalog));
                        return BadRequest(model);
                    }

                    var check = _repo._context.CateReportSoldAncolForFactoryLicenses.Where(x => x.YearReport == data.YearReport && x.BusinessId == data.BusinessId && x.CateReportSoldAncolForFactoryLicenseId != data.CateReportSoldAncolForFactoryLicenseId && !x.IsDel).Any();
                    if (check)
                    {
                        model.status = 0;
                        model.error = new ErrorModel()
                        {
                            Code = ErrCode_Const.SQL_INSERT_FAILED,
                            Msg = "Báo cáo đã tồn tại"
                        };

                        datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.UPDATE, NameCategory_Const.CATE_RP_ANCOL_FOR_FACTORY, Action_Status.FAIL);
                        _asyncLogger.LogInfo(JsonConvert.SerializeObject(datalog));
                        return BadRequest(model);
                    }

                    data.UpdateTime = DateTime.Now;
                    data.UpdateUserId = loginData.Userid;
                    await _repo.Update(data);

                    datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.UPDATE, NameCategory_Const.CATE_RP_ANCOL_FOR_FACTORY, Action_Status.SUCCESS);
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
            BaseModels<CateReportSoldAncolForFactoryLicenseModel> model = new BaseModels<CateReportSoldAncolForFactoryLicenseModel>();
            try
            {
                var info = _repo.FindById(id);
                if (info == null)
                {
                    return NotFound(ErrMsg_Const.GetMsg(ErrCode_Const.CANNOT_FIND_DATA_BY_QUERY));
                }

                CateReportSoldAncolForFactoryLicenseModel result = new CateReportSoldAncolForFactoryLicenseModel()
                {
                    CateReportSoldAncolForFactoryLicenseId = info.CateReportSoldAncolForFactoryLicenseId,
                    BusinessId = info.BusinessId,
                    Quantity = info.Quantity,
                    TypeofWine = info.TypeofWine,
                    WineFactory = info.WineFactory,
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
            BaseModels<CateReportSoldAncolForFactoryLicense> model = new BaseModels<CateReportSoldAncolForFactoryLicense>();
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
            UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
            if (loginData == null)
            {
                return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
            }
            SystemLog datalog = new SystemLog();
            BaseModels<CateReportSoldAncolForFactoryLicense> model = new BaseModels<CateReportSoldAncolForFactoryLicense>();
            try
            {
                await _repo.Delete(id);

                datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.DELETE, NameCategory_Const.CATE_RP_ANCOL_FOR_FACTORY, Action_Status.SUCCESS);
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
                using (var workbook = new XLWorkbook(@"Upload/Templates/Baocaoruouthucongbanchocosocogiayphepsanxuatruou.xlsx"))
                {
                    IXLWorksheet worksheet = workbook.Worksheets.Worksheet(1);
                    worksheet.Cell(1, 1).Value = $"Báo cáo tình hình rượu thủ công bán cho cơ sở có giấy phép sản xuất rượu trên địa bàn năm {currentYear}";

                    int row = 1;
                    int index = 10;

                    foreach (var item in data)
                    {
                        if (row == 1)
                        {
                            worksheet.Cell(index, 1).Value = row;
                            worksheet.Cell(index, 2).Value = item.OrganizationName;
                            worksheet.Cell(index, 3).Value = item.Address;
                            worksheet.Cell(index, 4).Value = item.PhoneNumber;
                            worksheet.Cell(index, 5).Value = item.TypeofWine;
                            worksheet.Cell(index, 6).Value = item.Quantity;
                            worksheet.Cell(index, 7).Value = item.WineFactory;

                            index++;
                            row++;
                        }
                        else
                        {
                            var addrow = worksheet.Row(index - 1);
                            addrow.InsertRowsBelow(1);

                            worksheet.Cell(index, 1).Value = row;
                            worksheet.Cell(index, 2).Value = item.OrganizationName;
                            worksheet.Cell(index, 3).Value = item.Address;
                            worksheet.Cell(index, 4).Value = item.PhoneNumber;
                            worksheet.Cell(index, 5).Value = item.TypeofWine;
                            worksheet.Cell(index, 6).Value = item.Quantity;
                            worksheet.Cell(index, 7).Value = item.WineFactory;
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
