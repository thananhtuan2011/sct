using API_SoCongThuong.Classes;
using API_SoCongThuong.Models;
using API_SoCongThuong.Reponsitories.RegulationConformityAMRepository;
using EF_Core.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.Design;
using System;
using System.Text;
using Newtonsoft.Json;
using StackExchange.Redis;


using static API_SoCongThuong.Classes.Ulities;
using ClosedXML.Excel;
using API_SoCongThuong.Logger;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Wordprocessing;
using System.Globalization;

namespace API_SoCongThuong.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegulationConformityAMController : ControllerBase
    {
        private RegulationConformityAMRepo _repo;
        private IConfiguration _configuration;
        private readonly ILogger<AsyncLogger> _logger;
        public SoHoa_SoCongThuongContext _context;
        private AsyncLogger _asyncLogger;

        public RegulationConformityAMController(SoHoa_SoCongThuongContext context, IConfiguration configuration, ILogger<AsyncLogger> logger)
        {
            _repo = new RegulationConformityAMRepo(context);

            _logger = logger;
            _context = context;
            _asyncLogger = new AsyncLogger(_logger, _context);
            _configuration = configuration;
        }

        [Route("find")]
        [HttpPost]
        public IActionResult ListItems_New([FromBody] QueryRequestBody query)
        {
            BaseModels<RegulationConformityAMModel> model = new BaseModels<RegulationConformityAMModel>();
            string _keywordSearch = "";
            bool _orderBy_ASC = true;
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }

                Func<RegulationConformityAMModel, object> _orderByExpression = x => x.EstablishmentName;
                Dictionary<string, Func<RegulationConformityAMModel, object>> _sortableFields = new Dictionary<string, Func<RegulationConformityAMModel, object>>
                {
                    { "EstablishmentName", x => x.EstablishmentName },
                    { "Phone", x => x.Phone },
                    { "DayReception", x => x.DayReception },
                    { "Num", x => x.Num },
                };
                if (query.Sort != null
                    && !string.IsNullOrEmpty(query.Sort.ColumnName)
                    && _sortableFields.ContainsKey(query.Sort.ColumnName))
                {
                    _orderBy_ASC = ("desc".Equals(query.Sort.Direction.ToLower()) ? false : true);
                    _orderByExpression = _sortableFields[query.Sort.ColumnName];
                }

                IQueryable<RegulationConformityAMModel> _data = (from x in _repo._context.RegulationConformityAms
                                                                 where !x.IsDel
                                                                 join b in _repo._context.Businesses on x.EstablishmentId equals b.BusinessId into bx
                                                                 from res in bx.DefaultIfEmpty()
                                                                 select new RegulationConformityAMModel
                                                                 {
                                                                     RegulationConformityAMId = x.RegulationConformityAmid,
                                                                     EstablishmentName = res.BusinessNameVi ?? "",
                                                                     DistrictId = x.DistrictId,
                                                                     ProductName = x.ProductName,
                                                                     Num = x.Num,
                                                                     DayReception = x.DayReception.ToString("dd'/'MM'/'yyyy"),
                                                                     DayReceptionDate = x.DayReception
                                                                 }).ToList().AsQueryable();

                if (query.SearchValue != null && query.SearchValue != "")
                {
                    _keywordSearch = query.SearchValue.Trim().ToLower();
                    _data = _data.Where(x =>
                       x.EstablishmentName.ToLower().Contains(_keywordSearch)
                       || x.ProductName.ToLower().Contains(_keywordSearch)
                       || x.Num.ToLower().Contains(_keywordSearch)
                   );
                }

                if (query.Filter != null && query.Filter.ContainsKey("DistrictId") && !string.IsNullOrEmpty(query.Filter["DistrictId"]))
                {
                    _data = _data.Where(x => x.DistrictId == Guid.Parse(query.Filter["DistrictId"]));
                }

                if (query.Filter != null && query.Filter.ContainsKey("MinDate") && !string.IsNullOrEmpty(query.Filter["MinDate"]))
                {
                    _data = _data.Where(x => x.DayReceptionDate >= DateTime.ParseExact(query.Filter["MinDate"], "dd'/'MM'/'yyyy", CultureInfo.InvariantCulture).Date);
                }

                if (query.Filter != null && query.Filter.ContainsKey("MaxDate") && !string.IsNullOrEmpty(query.Filter["MaxDate"]))
                {
                    _data = _data.Where(x => x.DayReceptionDate <= DateTime.ParseExact(query.Filter["MaxDate"], "dd'/'MM'/'yyyy", CultureInfo.InvariantCulture).Date);
                }

                int _countRows = _data.Count();
                if (_countRows == 0)
                {
                    return NotFound("Không có dữ liệu");
                }
                if (query.Panigator.More)
                {
                    model.status = 1;
                    model.items = _data.ToList();
                    model.total = _countRows;
                    return Ok(model);
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

        [Route("getlogs/{id}")]
        [HttpGet]
        public IActionResult GetLogsById(Guid id)
        {
            BaseModels<RegulationConformityAmLogModel> model = new BaseModels<RegulationConformityAmLogModel>();
            try
            {
                var result = _repo.FindLogsById(id);
                if (result != null)
                {
                    model.status = 1;
                    model.items = result;
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

        [HttpGet("{id}")]
        public IActionResult GetItemById(Guid id)
        {
            BaseModels<RegulationConformityAMModel> model = new BaseModels<RegulationConformityAMModel>();
            try
            {
                var result = _repo.FindById(id);
                if (result != null)
                {
                    model.status = 1;
                    model.data = result;
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
        public async Task<IActionResult> Update([FromBody] RegulationConformityAMModel data)
        {
            BaseModels<RegulationConformityAMModel> model = new BaseModels<RegulationConformityAMModel>();
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                SystemLog datalog = new SystemLog();
                var CheckData = _repo.FindById(data.RegulationConformityAMId);
                if (CheckData == null)
                {
                    model.status = 0;
                    model.error = new ErrorModel()
                    {
                        Code = ErrCode_Const.PROPERTY_IS_NULL_OR_EMPTY
                    };
                    datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.UPDATE, NameCategory_Const.REGULATION_CONFORMITY_ANNOUNCEMENT_MANAGEMENT, Action_Status.FAIL);
                    _asyncLogger.LogInfo(JsonConvert.SerializeObject(datalog));
                    return BadRequest(model);
                }
                else
                {
                    data.UpdateTime = DateTime.Now;
                    data.UpdateUserId = loginData.Userid;

                    var util = new Ulities();
                    data = util.TrimModel(data);

                    await _repo.Update(data);
                    datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.UPDATE, NameCategory_Const.REGULATION_CONFORMITY_ANNOUNCEMENT_MANAGEMENT, Action_Status.SUCCESS);
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

        [HttpPost()]
        public async Task<IActionResult> Create([FromBody] RegulationConformityAMModel data)
        {
            BaseModels<RegulationConformityAm> model = new BaseModels<RegulationConformityAm>();
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                SystemLog datalog = new SystemLog();
                data.CreateTime = DateTime.Now;
                data.CreateUserId = loginData.Userid;

                var util = new Ulities();
                data = util.TrimModel(data);

                await _repo.Insert(data);
                datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.CREATE, NameCategory_Const.REGULATION_CONFORMITY_ANNOUNCEMENT_MANAGEMENT, Action_Status.SUCCESS);
                _asyncLogger.LogInfo(JsonConvert.SerializeObject(datalog));
                //await _repo.Insert(SaveData);
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

        [HttpPut("deleteRegulationConformityAM/{id}")]
        public async Task<IActionResult> Delete(Guid id)
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
                datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.DELETE, NameCategory_Const.REGULATION_CONFORMITY_ANNOUNCEMENT_MANAGEMENT, Action_Status.SUCCESS);
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

        private List<RegulationConformityAMModel> FindData(QueryRequestBody query)
        {
            string _keywordSearch = ""; //Keyword tìm kiếm
            bool _orderBy_ASC = true;  //Khởi tạo sắp xếp dữ liệu acs hoặc desc khi tìm kiếm
            Func<RegulationConformityAMModel, object> _orderByExpression = x => x.EstablishmentName;
            Dictionary<string, Func<RegulationConformityAMModel, object>> _sortableFields = new Dictionary<string, Func<RegulationConformityAMModel, object>>
                {
                    { "EstablishmentName", x => x.EstablishmentName },
                    { "Address", x => x.Address },
                    { "Phone", x => x.Phone },
                    { "DayReception", x => x.DayReception },
                    { "Num", x => x.Num },
                };
            if (query.Sort != null
                && !string.IsNullOrEmpty(query.Sort.ColumnName)
                && _sortableFields.ContainsKey(query.Sort.ColumnName))
            {
                _orderBy_ASC = ("desc".Equals(query.Sort.Direction.ToLower()) ? false : true);
                _orderByExpression = _sortableFields[query.Sort.ColumnName];
            }

            IQueryable<RegulationConformityAMModel> _data = (from x in _repo._context.RegulationConformityAms
                                                             where !x.IsDel
                                                             join b in _repo._context.Businesses on x.EstablishmentId equals b.BusinessId into bx
                                                             from res in bx.DefaultIfEmpty()
                                                             select new RegulationConformityAMModel
                                                             {
                                                                 RegulationConformityAMId = x.RegulationConformityAmid,
                                                                 DayReception = x.DayReception.ToString("dd'/'MM'/'yyyy"),
                                                                 EstablishmentName = res.BusinessNameVi ?? "",
                                                                 Address = x.Address,
                                                                 Phone = x.Phone,
                                                                 Num = x.Num,
                                                                 ProductName = x.ProductName,
                                                                 Content = x.Content,
                                                                 Note = x.Note,
                                                                 DateOfPublication = x.DateOfPublication.ToString("dd'/'MM'/'yyyy"),
                                                             }).ToList().AsQueryable();

            if (query.SearchValue != null && query.SearchValue != "")
            {
                _keywordSearch = query.SearchValue.Trim().ToLower();
                _data = _data.Where(x =>
                   x.EstablishmentName.ToLower().Contains(_keywordSearch)
                   || x.Phone.ToLower().Contains(_keywordSearch)
                   || x.Num.ToLower().Contains(_keywordSearch)
               );
            }

            if (query.Filter != null && query.Filter.ContainsKey("DistrictId") && !string.IsNullOrEmpty(query.Filter["DistrictId"]))
            {
                _data = _data.Where(x => x.DistrictId == Guid.Parse(query.Filter["DistrictId"]));
            }

            if (query.Filter != null && query.Filter.ContainsKey("DayReception") && !string.IsNullOrEmpty(query.Filter["DayReception"]))
            {
                _data = _data.Where(x => x.DayReception == query.Filter["DayReception"]);
            }

            return _data.ToList();
        }

        [HttpPost("Export")]
        public IActionResult Export([FromBody] QueryRequestBody query)
        {
            var data = FindData(query);

            if (!data.Any() || data.Count == 0)
            {
                return BadRequest();
            }

            string Title = "";
            if (query.Filter != null && query.Filter.ContainsKey("DistrictId") && !string.IsNullOrEmpty(query.Filter["DistrictId"]))
            {
                var districtName = _context.Districts.Where(x => x.DistrictId.ToString() == query.Filter["DistrictId"]).FirstOrDefault()!.DistrictName ?? "";
                if (string.IsNullOrEmpty(districtName))
                {
                    Title += " " + districtName;
                }
                
            }

            if (query.Filter != null && query.Filter.ContainsKey("DayReception") && !string.IsNullOrEmpty(query.Filter["DayReception"]))
            {
                Title += " NGÀY " + query.Filter["DayReception"];
            }

            var logHistory = (from l in _context.RegulationConformityAmLogs
                              join u in _context.Users
                              on l.UserId equals u.UserId
                              select new RegulationConformityAmLogModel
                              {
                                  LogId = l.LogId,
                                  ItemId = l.ItemId,
                                  UserId = l.UserId,
                                  UserName = u.FullName,
                                  LogTime = l.LogTime,
                                  LogTimeDisplay = l.LogTime.AddHours(7).ToString("dd'/'MM'/'yyyy HH:mm"),
                                  Property = l.Property,
                                  OldValue = l.OldValue,
                                  NewValue = l.NewValue,
                              }).OrderBy(x => x.LogTime).ToList();
            try
            {
                using (var workbook = new XLWorkbook(@"Upload/Templates/QuanLyCongBoHopQuy.xlsx"))
                {
                    IXLWorksheet worksheet = workbook.Worksheets.Worksheet(1);
                    int index = 5;
                    int row = 1;

                    worksheet.Cell(2, 1).Value = "QUẢN LÝ CÔNG BỐ HỢP QUY" + Title;

                    foreach (var item in data)
                    {
                        if (row < data.Count())
                        {
                            var addrow = worksheet.Row(index);
                            addrow.InsertRowsBelow(1);
                        }

                        worksheet.Cell(index, 1).Value = row;
                        worksheet.Cell(index, 2).Value = item.EstablishmentName;
                        worksheet.Cell(index, 3).Value = item.DayReception;
                        worksheet.Cell(index, 4).Value = item.Address;
                        worksheet.Cell(index, 5).Value = item.Phone;
                        worksheet.Cell(index, 6).Value = item.Num;
                        worksheet.Cell(index, 7).Value = item.ProductName;
                        worksheet.Cell(index, 8).Value = item.Content;
                        worksheet.Cell(index, 9).Value = item.Note;
                        worksheet.Cell(index, 10).Value = item.DateOfPublication;

                        var itemLog = logHistory.Where(x => x.ItemId == item.RegulationConformityAMId).ToList();
                        if (itemLog.Any())
                        {
                            var cellLog = worksheet.Cell(index, 11);
                            var logContent = cellLog.GetRichText().ClearText();

                            int lIndex = 1;
                            foreach (var l in itemLog)
                            {
                                if (lIndex < itemLog.Count())
                                {
                                    logContent.AddText("-" + l.LogTimeDisplay + ", " + l.OldValue + " -> " + l.NewValue + ".").AddNewLine();
                                }
                                else
                                {
                                    logContent.AddText("-" + l.LogTimeDisplay + ", " + l.OldValue + " -> " + l.NewValue + ".");
                                }
                                lIndex++;
                            }
                        }

                        index++;
                        row++;
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
