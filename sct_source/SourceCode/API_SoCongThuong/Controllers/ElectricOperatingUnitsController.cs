using API_SoCongThuong.Classes;
using API_SoCongThuong.Logger;
using API_SoCongThuong.Models;
using API_SoCongThuong.Reponsitories.ElectricOperatingUnitsRepository;
using ClosedXML.Excel;
using EF_Core.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using System.ComponentModel.Design;
using System.Data;
using System.Globalization;
using static System.Net.Mime.MediaTypeNames;

namespace API_SoCongThuong.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ElectricOperatingUnitsController : ControllerBase
    {
        private ElectricOperatingUnitsRepo _repo;
        private IConfiguration _configuration;
        private readonly ILogger<AsyncLogger> _logger;
        public SoHoa_SoCongThuongContext _context;
        private AsyncLogger _asyncLogger;
        public ElectricOperatingUnitsController(SoHoa_SoCongThuongContext context, IConfiguration configuration, ILogger<AsyncLogger> logger)
        {
            _repo = new ElectricOperatingUnitsRepo(context);
            _logger = logger;
            _context = context;
            _asyncLogger = new AsyncLogger(_logger, _context);
            _configuration = configuration;

        }

        [Route("find")]
        [HttpPost]
        public IActionResult Find([FromBody] QueryRequestBody query)
        {
            BaseModels<ElectricOperatingUnitsModel> model = new BaseModels<ElectricOperatingUnitsModel>();
            string _keywordSearch = "";
            bool _orderBy_ASC = true;
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }

                Func<ElectricOperatingUnitsModel, object> _orderByExpression = x => x.CustomerName;
                Dictionary<string, Func<ElectricOperatingUnitsModel, object>> _sortableFields =
                    new Dictionary<string, Func<ElectricOperatingUnitsModel, object>>
                    {
                        { "CustomerString", x => x.CustomerString },
                        { "Address", x => x.Address },
                        { "NumOfGP", x => x.NumOfGP },
                        { "Supplier", x => x.Supplier },
                        { "Status", x => x.Status },
                    };
                if (query.Sort != null && !string.IsNullOrEmpty(query.Sort.ColumnName) && _sortableFields.ContainsKey(query.Sort.ColumnName))
                {
                    _orderBy_ASC = ("desc".Equals(query.Sort.Direction.ToLower()) ? false : true);
                    _orderByExpression = _sortableFields[query.Sort.ColumnName];
                }

                IQueryable<ElectricOperatingUnitsModel> _data = (from e in _repo._context.ElectricOperatingUnits
                                                                 where !e.IsDel
                                                                 join b in _repo._context.Businesses on e.CustomerName equals b.BusinessId
                                                                 join c in _repo._context.Categories on e.Supplier equals c.CategoryId
                                                                 select new ElectricOperatingUnitsModel
                                                                 {
                                                                     ElectricOperatingUnitsId = e.ElectricOperatingUnitsId,
                                                                     CustomerName = e.CustomerName,
                                                                     CustomerString = b.BusinessNameVi,
                                                                     Address = e.Address,
                                                                     NumOfGP = e.NumOfGp,
                                                                     SignDay = e.SignDay.ToString("dd'/'MM'/'yyyy"),
                                                                     Supplier = e.Supplier,
                                                                     SupplierName = c.CategoryName,
                                                                     IsPowerGeneration = e.IsPowerGeneration,
                                                                     IsPowerDistribution = e.IsPowerDistribution,
                                                                     IsElectricityRetail = e.IsElectricityRetail,
                                                                     IsConsulting = e.IsConsulting,
                                                                     IsSurveillance = e.IsSurveillance,
                                                                     // FieldActivity = FieldActivity(e.IsPowerGeneration, e.IsPowerDistribution, e.IsElectricityRetail, e.IsConsulting, e.IsSurveillance),
                                                                     Status = e.Status,
                                                                 }).ToList().AsQueryable();

                if (query.SearchValue != null && query.SearchValue != "")
                {
                    _keywordSearch = query.SearchValue.Trim().ToLower();
                    _data = _data.Where(x =>
                        x.CustomerString.ToLower().Contains(_keywordSearch)
                        || x.Address.ToLower().Contains(_keywordSearch)
                        || x.NumOfGP.ToLower().Contains(_keywordSearch)
                        || x.SignDay.ToLower().Contains(_keywordSearch)
                   );
                }

                if (query.Filter != null && query.Filter.ContainsKey("SupplierId") && !string.IsNullOrEmpty(query.Filter["SupplierId"]))
                {
                    _data = _data.Where(x => x.Supplier.ToString() == query.Filter["SupplierId"]);
                }

                if (query.Filter != null && query.Filter.ContainsKey("Type") && !string.IsNullOrEmpty(query.Filter["Type"]))
                {
                    if (query.Filter["Type"] == "1")
                    {
                        _data = _data.Where(x => x.IsPowerGeneration == true);
                    }
                    if (query.Filter["Type"] == "2")
                    {
                        _data = _data.Where(x => x.IsPowerDistribution == true);
                    }
                    if (query.Filter["Type"] == "3")
                    {
                        _data = _data.Where(x => x.IsElectricityRetail == true);
                    }
                    if (query.Filter["Type"] == "4")
                    {
                        _data = _data.Where(x => x.IsConsulting == true);
                    }
                    if (query.Filter["Type"] == "5")
                    {
                        _data = _data.Where(x => x.IsSurveillance == true);
                    }
                }

                if (query.Filter != null && query.Filter.ContainsKey("StatusId") && !string.IsNullOrEmpty(query.Filter["StatusId"]))
                {
                    _data = _data.Where(x => x.Status.ToString() == query.Filter["StatusId"]);
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

                for (var i = 0; i < model.items.Count; i ++)
                {
                    model.items[i].FieldActivity = FieldActivity(model.items[i].IsPowerGeneration, model.items[i].IsPowerDistribution, model.items[i].IsElectricityRetail, model.items[i].IsConsulting, model.items[i].IsSurveillance);
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

        private string FieldActivity(bool IsPowerGeneration, bool IsPowerDistribution, bool IsElectricityRetail, bool IsConsulting, bool IsSurveillance)
        {
            List<string> Result = new List<string>();

            if (IsPowerGeneration)
                Result.Add("Phát điện");

            if (IsPowerDistribution)
                Result.Add("Phân phối điện");

            if (IsElectricityRetail)
                Result.Add("Bán lẻ điện");

            if (IsConsulting)
                Result.Add("Tư vấn thiết kế");

            if (IsSurveillance)
                Result.Add("Tư vấn giám sát");

            return Result.Count > 0 ? string.Join(", ", Result) : "";
        }

        [HttpPost()]
        public async Task<IActionResult> create(ElectricOperatingUnitsModel data)
        {
            BaseModels<ElectricOperatingUnit> model = new BaseModels<ElectricOperatingUnit>();
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                SystemLog datalog = new SystemLog();
                ElectricOperatingUnit SaveData = new ElectricOperatingUnit();

                SaveData.CustomerName = data.CustomerName;
                SaveData.Address = data.Address;
                SaveData.PhoneNumber = data.PhoneNumber;
                SaveData.PresidentName = data.PresidentName;
                SaveData.NumOfGp = data.NumOfGP;
                SaveData.SignDay = DateTime.ParseExact(data.SignDay, "dd'/'MM'/'yyyy", CultureInfo.InvariantCulture);
                SaveData.Supplier = data.Supplier;
                SaveData.IsPowerGeneration = data.IsPowerGeneration;
                SaveData.IsPowerDistribution = data.IsPowerDistribution;
                SaveData.IsConsulting = data.IsConsulting;
                SaveData.IsSurveillance = data.IsSurveillance;
                SaveData.IsElectricityRetail = data.IsElectricityRetail;
                SaveData.Status = data.Status;

                SaveData.CreateUserId = loginData.Userid;
                SaveData.CreateTime = DateTime.Now;

                await _repo.Insert(SaveData);
                datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.CREATE, NameCategory_Const.ELECTRIC_OPERATING_UNIT, Action_Status.SUCCESS);
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

        [HttpGet("{id}")]
        public IActionResult getItemById(Guid id)
        {
            BaseModels<ElectricOperatingUnitsModel> model = new BaseModels<ElectricOperatingUnitsModel>();
            try
            {
                var result = _repo.FindById(id);
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
        public async Task<IActionResult> Update(ElectricOperatingUnitsModel data)
        {
            BaseModels<ElectricOperatingUnit> model = new BaseModels<ElectricOperatingUnit>();
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                SystemLog datalog = new SystemLog();
                ElectricOperatingUnit? SaveData = _repo._context.ElectricOperatingUnits.Where(x => x.ElectricOperatingUnitsId == data.ElectricOperatingUnitsId && !x.IsDel).FirstOrDefault();
                if (SaveData != null)
                {
                    SaveData.CustomerName = data.CustomerName;
                    SaveData.Address = data.Address;
                    SaveData.PhoneNumber = data.PhoneNumber;
                    SaveData.PresidentName = data.PresidentName;
                    SaveData.NumOfGp = data.NumOfGP;
                    SaveData.SignDay = DateTime.ParseExact(data.SignDay, "dd'/'MM'/'yyyy", CultureInfo.InvariantCulture);
                    SaveData.Supplier = data.Supplier;
                    SaveData.IsPowerGeneration = data.IsPowerGeneration;
                    SaveData.IsPowerDistribution = data.IsPowerDistribution;
                    SaveData.IsConsulting = data.IsConsulting;
                    SaveData.IsSurveillance = data.IsSurveillance;
                    SaveData.IsElectricityRetail = data.IsElectricityRetail;
                    SaveData.Status = data.Status;

                    SaveData.UpdateUserId = loginData.Userid;
                    SaveData.UpdateTime = DateTime.Now;

                    await _repo.Update(SaveData);
                    datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.UPDATE, NameCategory_Const.ELECTRIC_OPERATING_UNIT, Action_Status.SUCCESS);
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
                    datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.UPDATE, NameCategory_Const.ELECTRIC_OPERATING_UNIT, Action_Status.FAIL);
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

        [HttpPut("delete/{id}")]
        public async Task<IActionResult> delete(Guid id)
        {
            BaseModels<ElectricOperatingUnit> model = new BaseModels<ElectricOperatingUnit>();
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                SystemLog datalog = new SystemLog();
                ElectricOperatingUnit? DeleteData = _repo._context.ElectricOperatingUnits.Where(x => x.ElectricOperatingUnitsId == id && !x.IsDel).FirstOrDefault();
                if (DeleteData != null)
                {
                    DeleteData.IsDel = true;
                    await _repo.Delete(DeleteData);
                    datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.DELETE, NameCategory_Const.ELECTRIC_OPERATING_UNIT, Action_Status.SUCCESS);
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
                    datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.DELETE, NameCategory_Const.ELECTRIC_OPERATING_UNIT, Action_Status.FAIL);
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

        [HttpPost("ExportExcel")]
        public IActionResult ExportExcel([FromBody] QueryRequestBody query)
        {
            try
            {
                IQueryable<ElectricOperatingUnitsModel> query_str = (from e in _repo._context.ElectricOperatingUnits
                                                                     where !e.IsDel
                                                                     join b in _repo._context.Businesses on e.CustomerName equals b.BusinessId
                                                                     join c in _repo._context.Categories on e.Supplier equals c.CategoryId
                                                                     select new ElectricOperatingUnitsModel
                                                                     {
                                                                         ElectricOperatingUnitsId = e.ElectricOperatingUnitsId,
                                                                         CustomerName = e.CustomerName,
                                                                         CustomerString = b.BusinessNameVi,
                                                                         Address = e.Address,
                                                                         NumOfGP = e.NumOfGp,
                                                                         SignDay = e.SignDay.ToString("dd'/'MM'/'yyyy"),
                                                                         Supplier = e.Supplier,
                                                                         SupplierName = c.CategoryName,
                                                                         IsPowerGeneration = e.IsPowerGeneration,
                                                                         IsPowerDistribution = e.IsPowerDistribution,
                                                                         IsElectricityRetail = e.IsElectricityRetail,
                                                                         IsConsulting = e.IsConsulting,
                                                                         IsSurveillance = e.IsSurveillance,
                                                                         Status = e.Status,
                                                                     }).ToList().AsQueryable();

                string _keywordSearch = "";
                if (query.SearchValue != null && query.SearchValue != "")
                {
                    _keywordSearch = query.SearchValue.Trim().ToLower();
                    query_str = query_str.Where(x =>
                        x.CustomerString.ToLower().Contains(_keywordSearch)
                        || x.Address.ToLower().Contains(_keywordSearch)
                        || x.NumOfGP.ToLower().Contains(_keywordSearch)
                        || x.SignDay.ToLower().Contains(_keywordSearch)
                   );
                }

                string FilterType = "";
                if (query.Filter != null && query.Filter.ContainsKey("SupplierId") && !string.IsNullOrEmpty(query.Filter["SupplierId"]))
                {
                    query_str = query_str.Where(x => x.Supplier.ToString() == query.Filter["SupplierId"]);
                    string SupplierName = query_str.Where(x => x.Supplier.ToString() == query.Filter["SupplierId"]).Select(x => x.SupplierName).FirstOrDefault() ?? "";
                    if (!string.IsNullOrEmpty(SupplierName))
                    {
                        FilterType += " " + SupplierName.ToUpper();
                    }
                }

                if (query.Filter != null && query.Filter.ContainsKey("Type") && !string.IsNullOrEmpty(query.Filter["Type"]))
                {
                    if (query.Filter["Type"] == "1")
                    {
                        query_str = query_str.Where(x => x.IsConsulting == true);
                        FilterType += " LĨNH VỰC PHÁT ĐIỆN";
                    }
                    if (query.Filter["Type"] == "2")
                    {
                        query_str = query_str.Where(x => x.IsSurveillance == true);
                        FilterType += " LĨNH VỰC PHÂN PHỐI ĐIỆN";
                    }
                    if (query.Filter["Type"] == "3")
                    {
                        query_str = query_str.Where(x => x.IsElectricityRetail == true);
                        FilterType += " LĨNH VỰC BÁN LẺ ĐIỆN";
                    }
                    if (query.Filter["Type"] == "4")
                    {
                        query_str = query_str.Where(x => x.IsElectricityRetail == true);
                        FilterType += " LĨNH VỰC TƯ VẤN THIẾT KẾ";
                    }
                    if (query.Filter["Type"] == "5")
                    {
                        query_str = query_str.Where(x => x.IsElectricityRetail == true);
                        FilterType += " LĨNH VỰC TƯ VẤN GIÁM SÁT";
                    }
                }

                if (query.Filter != null && query.Filter.ContainsKey("StatusId") && !string.IsNullOrEmpty(query.Filter["StatusId"]))
                {
                    query_str = query_str.Where(x => x.Status.ToString() == query.Filter["StatusId"]);
                    if (query.Filter["StatusId"] == "0")
                    {
                        FilterType += " CÒN HOẠT ĐỘNG";
                    }
                    if (query.Filter["StatusId"] == "1")
                    {
                        FilterType += " NGỪNG HOẠT ĐỘNG";
                    }
                }

                string Title = "CÁC ĐƠN VỊ ĐƯỢC CẤP GIẤY PHÉP HOẠT ĐỘNG ĐIỆN LỰC";

                List<ElectricOperatingUnitsModel> data = query_str.ToList();

                if (!data.Any())
                {
                    return BadRequest();
                }

                using (var workbook = new XLWorkbook(@"Upload/Templates/Cacdonvihoatdongdienluc.xlsx"))
                {
                    IXLWorksheet worksheet = workbook.Worksheets.Worksheet(1);
                    worksheet.Cell(6, 1).Value = Title + FilterType;
                    int index = 9;

                    foreach (var item in data)
                    {
                        worksheet.Cell(index, 1).Value = index - 8;
                        worksheet.Cell(index, 2).Value = item.CustomerString.ToUpper();
                        worksheet.Cell(index, 3).Value = item.Address;
                        worksheet.Cell(index, 4).Value = item.NumOfGP;
                        worksheet.Cell(index, 5).Value = item.SignDay;
                        worksheet.Cell(index, 6).Value = item.SupplierName;
                        worksheet.Cell(index, 7).Value = item.IsPowerGeneration ? "x" : "";
                        worksheet.Cell(index, 8).Value = item.IsPowerDistribution ? "x" : "";
                        worksheet.Cell(index, 9).Value = item.IsElectricityRetail ? "x" : "";
                        worksheet.Cell(index, 10).Value = item.IsConsulting ? "x" : "";
                        worksheet.Cell(index, 11).Value = item.IsSurveillance ? "x" : "";
                        worksheet.Cell(index, 12).Value = item.Status == 0 ? "Còn hoạt động" : "Ngừng hoạt động";
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
