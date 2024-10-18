using EF_Core.Models;
using Microsoft.AspNetCore.Mvc;
using API_SoCongThuong.Classes;
using API_SoCongThuong.Models;
using API_SoCongThuong.Reponsitories;
using Newtonsoft.Json;
using System.Data;
using API_SoCongThuong.Logger;
using static API_SoCongThuong.Classes.Ulities;
using ClosedXML.Excel;

namespace API_SoCongThuong.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TradePromotionActivityReportController : ControllerBase
    {
        private TradePromotionActivityReportRepo _repo;
        private IConfiguration _configuration;
        private readonly ILogger<AsyncLogger> _logger;
        public SoHoa_SoCongThuongContext _context;
        private AsyncLogger _asyncLogger;

        public TradePromotionActivityReportController(SoHoa_SoCongThuongContext context, IConfiguration configuration, ILogger<AsyncLogger> logger)
        {
            _repo = new TradePromotionActivityReportRepo(context);

            _logger = logger;
            _context = context;
            _asyncLogger = new AsyncLogger(_logger, _context);
            _configuration = configuration;
        }

        #region 
        [Route("find")]
        [HttpPost]
        public IActionResult ListItems_New([FromBody] QueryRequestBody query)
        {
            BaseModels<TradePromotionActivityReportModel> model = new BaseModels<TradePromotionActivityReportModel>();
            string _keywordSearch = "";
            bool _orderBy_ASC = true;
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }

                Func<TradePromotionActivityReportModel, object> _orderByExpression = x => x.PlanName;
                Dictionary<string, Func<TradePromotionActivityReportModel, object>> _sortableFields = new Dictionary<string, Func<TradePromotionActivityReportModel, object>>
                    {
                        { "PlanName", x => x.PlanName },
                        { "StartDate", x => x.StartDateDisplay },
                        { "ImplementationCost", x => x.ImplementationCost },
                        { "FundingSupport", x => x.FundingSupport },
                        { "Scale", x => x.Scale },
                        { "NumParticipating", x => x.NumParticipating },
                    };

                if (query.Sort != null
                    && !string.IsNullOrEmpty(query.Sort.ColumnName)
                    && _sortableFields.ContainsKey(query.Sort.ColumnName))
                {
                    _orderBy_ASC = ("desc".Equals(query.Sort.Direction.ToLower()) ? false : true);
                    _orderByExpression = _sortableFields[query.Sort.ColumnName];
                }

                IQueryable<TradePromotionActivityReportModel> _data = (from t in _repo._context.TradePromotionActivityReports
                                                                       where !t.IsDel
                                                                       select new TradePromotionActivityReportModel
                                                                       {
                                                                           TradePromotionActivityReportId = t.TradePromotionActivityReportId,
                                                                           ScaleId = t.ScaleId,
                                                                           PlanName = t.PlanName,
                                                                           StartDateDisplay = "Ngày " + t.StartDate.ToString("dd'/'MM'/'yyyy") + ", tại " + t.Address.Trim(),
                                                                           ImplementationCost = t.ImplementationCost,
                                                                           FundingSupport = t.FundingSupport,
                                                                           Scale = t.Scale,
                                                                           NumParticipating = t.NumParticipating,
                                                                           StartDate = t.StartDate,
                                                                           EndDate = t.EndDate,
                                                                       }).ToList().AsQueryable();

                if (query.SearchValue != null && query.SearchValue != "")
                {
                    _keywordSearch = query.SearchValue.Trim().ToLower();
                    _data = _data.Where(x => x.PlanName.ToLower().Contains(_keywordSearch)
                    || x.StartDateDisplay.ToLower().Contains(_keywordSearch)
                    || x.ImplementationCost.ToString().Contains(_keywordSearch)
                    || x.FundingSupport.ToString().Contains(_keywordSearch)
                    || x.Scale.ToLower().Contains(_keywordSearch)
                    || x.NumParticipating.ToString().Contains(_keywordSearch)
                    );
                }

                int _countRows = _data.Count();
                if (_countRows == 0)
                {
                    return NotFound("Không có dữ liệu");
                }

                if (query.Filter != null && query.Filter.ContainsKey("ScaleId") && !string.IsNullOrEmpty(query.Filter["ScaleId"]))
                {
                    _data = _data.Where(x =>
                                x.ScaleId.ToString().Equals(query.Filter["ScaleId"]));
                }

                if (query.Filter != null && query.Filter.ContainsKey("MinTime") && !string.IsNullOrEmpty(query.Filter["MinTime"]))
                {
                    _data = _data.Where(x =>
                                x.StartDate >=
                                DateTime.ParseExact(query.Filter["MinTime"], "dd/MM/yyyy", null).Date);
                }

                if (query.Filter != null && query.Filter.ContainsKey("MaxTime")
                    && !string.IsNullOrEmpty(query.Filter["MaxTime"]))
                {
                    _data = _data.Where(x =>
                               x.StartDate <=
                                DateTime.ParseExact(query.Filter["MaxTime"], "dd/MM/yyyy", null).Date);
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
        #endregion

        [HttpPost()]
        public async Task<IActionResult> Create([FromForm] TradePromotionActivityReportModel data)
        {
            BaseModels<TradePromotionActivityReportModel> model = new BaseModels<TradePromotionActivityReportModel>();
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }

                SystemLog datalog = new SystemLog();

                var Files = Request.Form.Files;
                var LstFile = new List<TradePromotionActivityReportAttachFileModel>();
                foreach (var f in Files)
                {
                    if (f.Length > 0)
                    {
                        using (var ms = new MemoryStream())
                        {
                            f.CopyTo(ms);
                            upLoadFileModel up = new upLoadFileModel()
                            {
                                bs = ms.ToArray(),
                                FileName = f.FileName.Replace(" ", ""),
                                Linkfile = "TradePromotionActivityReport"
                            };
                            var result = Ulities.UploadFile(up, _configuration);

                            TradePromotionActivityReportAttachFileModel FileSave = new TradePromotionActivityReportAttachFileModel();
                            FileSave.LinkFile = result.link;
                            LstFile.Add(FileSave);
                        }
                    }
                }
                data.Files = LstFile;

                data = new Ulities().TrimModel(data);

                data.StartDate = DateTime.ParseExact(data.StartDateDisplay, "dd'/'MM'/'yyyy", null);
                if (!string.IsNullOrEmpty(data.EndDateDisplay))
                {
                    data.EndDate = DateTime.ParseExact(data.EndDateDisplay, "dd'/'MM'/'yyyy", null);
                }
                else
                {
                    data.EndDate = null;
                }
                
                if (string.IsNullOrEmpty(data.ParticipatingBusinessesJson))
                {
                    data.ParticipatingBusinesses = new List<TradePromotionActivityReportBusinessModel>();
                }
                else
                {
                    data.ParticipatingBusinesses = JsonConvert.DeserializeObject<List<TradePromotionActivityReportBusinessModel>>(data.ParticipatingBusinessesJson);
                }
                data.CreateUserId = loginData.Userid;

                await _repo.Insert(data);
                datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.CREATE, NameCategory_Const.TRADE_PROMOTION_ACTIVITY_REPORT, Action_Status.SUCCESS);
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
        public async Task<IActionResult> Update([FromForm] TradePromotionActivityReportModel data)
        {
            BaseModels<TradePromotionActivityReportModel> model = new BaseModels<TradePromotionActivityReportModel>();
            try
            {

                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }

                SystemLog datalog = new SystemLog();

                var CheckData = _context.TradePromotionActivityReports
                    .Where(x => x.TradePromotionActivityReportId == data.TradePromotionActivityReportId && !x.IsDel);
                if (!CheckData.Any())
                {
                    model.status = 0;
                    model.error = new ErrorModel()
                    {
                        Code = ErrCode_Const.PROPERTY_IS_NULL_OR_EMPTY
                    };
                    datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.UPDATE, NameCategory_Const.TRADE_PROMOTION_ACTIVITY_REPORT, Action_Status.FAIL);
                    _asyncLogger.LogInfo(JsonConvert.SerializeObject(datalog));
                    return BadRequest(model);
                }
                else
                {
                    var Files = Request.Form.Files;
                    var LstFile = new List<TradePromotionActivityReportAttachFileModel>();
                    foreach (var f in Files)
                    {
                        if (f.Length > 0)
                        {
                            using (var ms = new MemoryStream())
                            {
                                f.CopyTo(ms);
                                upLoadFileModel up = new upLoadFileModel()
                                {
                                    bs = ms.ToArray(),
                                    FileName = f.FileName.Replace(" ", ""),
                                    Linkfile = "TradePromotionActivityReport"
                                };
                                var result = Ulities.UploadFile(up, _configuration);

                                TradePromotionActivityReportAttachFileModel FileSave = new TradePromotionActivityReportAttachFileModel();
                                FileSave.LinkFile = result.link;
                                LstFile.Add(FileSave);
                            }
                        }
                    }
                    data.Files = LstFile;

                    data = new Ulities().TrimModel(data);

                    data.StartDate = DateTime.ParseExact(data.StartDateDisplay, "dd'/'MM'/'yyyy", null);
                    if (!string.IsNullOrEmpty(data.EndDateDisplay))
                    {
                        data.EndDate = DateTime.ParseExact(data.EndDateDisplay, "dd'/'MM'/'yyyy", null);
                    }
                    else
                    {
                        data.EndDate = null;
                    }

                    if (string.IsNullOrEmpty(data.ParticipatingBusinessesJson))
                    {
                        data.ParticipatingBusinesses = new List<TradePromotionActivityReportBusinessModel>();
                    }
                    else
                    {
                        data.ParticipatingBusinesses = JsonConvert.DeserializeObject<List<TradePromotionActivityReportBusinessModel>>(data.ParticipatingBusinessesJson);
                    }

                    data.UpdateTime = DateTime.Now;
                    data.UpdateUserId = loginData.Userid;

                    await _repo.Update(data, _configuration);

                    datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.UPDATE, NameCategory_Const.TRADE_PROMOTION_ACTIVITY_REPORT, Action_Status.SUCCESS);
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
        public IActionResult GetItemById(Guid id)
        {
            BaseModels<TradePromotionActivityReportModel> model = new BaseModels<TradePromotionActivityReportModel>();
            try
            {
                var info = _repo.FindById(id, _configuration);
                if (info == null)
                    return NotFound(ErrMsg_Const.GetMsg(ErrCode_Const.CANNOT_FIND_DATA_BY_QUERY));

                model.status = 1;
                model.data = info;
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
                await _repo.Delete(id, _configuration);

                datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.DELETE, NameCategory_Const.TRADE_PROMOTION_ACTIVITY_REPORT, Action_Status.SUCCESS);
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
            var allSheet = new List<int> { 3, 2, 1 };
            var data = _repo.FindData(query);

            string fromDate = "";
            if (query.Filter != null && query.Filter.ContainsKey("MinTime") && !string.IsNullOrEmpty(query.Filter["MinTime"]))
            {
                fromDate = query.Filter["MinTime"];
            }

            string toDate = "";
            if (query.Filter != null && query.Filter.ContainsKey("MaxTime") && !string.IsNullOrEmpty(query.Filter["MaxTime"]))
            {
                toDate = query.Filter["MaxTime"];
            }

            if (query.Filter != null && query.Filter.ContainsKey("ScaleId") && !string.IsNullOrEmpty(query.Filter["ScaleId"]))
            {
                allSheet = allSheet.Where(x => x != int.Parse(query.Filter["ScaleId"]) + 1).ToList();
            }

            if (!data.Any() || data.Count == 0)
            {
                return BadRequest();
            }

            try
            {
                using (var workbook = new XLWorkbook(@"Upload/Templates/BaoCaoHoatDongXucTienThuongMai.xlsx"))
                {
                    var groups = data.GroupBy(x => x.ScaleId);
                    foreach (var group in groups)
                    {
                        IXLWorksheet worksheet = workbook.Worksheets.Worksheet(group.Key + 1);

                        if (!string.IsNullOrEmpty(fromDate))
                        {
                            if (!string.IsNullOrEmpty(toDate))
                            {
                                worksheet.Cell(2, 1).Value = $"Từ ngày: {fromDate}, Đến ngày: {toDate}.";
                                worksheet.Cell(2, 1).Style.Font.SetItalic(true);
                            }
                            else
                            {
                                worksheet.Cell(2, 1).Value = $"Từ ngày: {fromDate}.";
                                worksheet.Cell(2, 1).Style.Font.SetItalic(true);
                            }
                        }
                        else if (!string.IsNullOrEmpty(toDate))
                        {
                            worksheet.Cell(2, 1).Value = $"Đến ngày: {toDate}.";
                            worksheet.Cell(2, 1).Style.Font.SetItalic(true);
                        }

                        int index = 5;
                        int row = 1;
                        foreach (var item in group)
                        {
                            if (row < group.Count())
                            {
                                worksheet.Row(index).CopyTo(worksheet.Row(index + 1));
                            }

                            worksheet.Cell(index, 1).Value = row;
                            worksheet.Cell(index, 2).Value = item.PlanName;
                            worksheet.Cell(index, 3).Value = item.StartDateDisplay;
                            if (group.Key == 0)
                            {
                                worksheet.Cell(index, 4).Value = item.ImplementationCost;
                                worksheet.Cell(index, 5).Value = item.FundingSupport;
                                worksheet.Cell(index, 6).Value = item.Scale;
                                worksheet.Cell(index, 7).Value = item.NumParticipating;
                                worksheet.Cell(index, 8).Value = item.Participating;
                            }
                            else
                            {
                                worksheet.Cell(index, 4).Value = item.PlanToJoin == false ? "x" : "";
                                worksheet.Cell(index, 5).Value = item.PlanToJoin == true ? "x" : "";
                                worksheet.Cell(index, 6).Value = item.ImplementationCost;
                                worksheet.Cell(index, 7).Value = item.FundingSupport;
                                worksheet.Cell(index, 8).Value = item.Scale;
                                worksheet.Cell(index, 9).Value = item.NumParticipating;
                                worksheet.Cell(index, 10).Value = item.Participating;
                            }

                            index++;
                            row++;
                        }
                    }
                    if (allSheet.Count() < 3)
                    {
                        foreach (var i in allSheet)
                        {
                            workbook.Worksheets.Worksheet(i).Delete();
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
