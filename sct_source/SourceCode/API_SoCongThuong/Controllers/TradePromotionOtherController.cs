using DpsLibs.Web;
using EF_Core.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.Design;
using API_SoCongThuong.Classes;
using API_SoCongThuong.Models;
using API_SoCongThuong.Reponsitories.BusinessIndustryRepository;
using API_SoCongThuong.Reponsitories.BusinessRepository;
using API_SoCongThuong.Reponsitories.IndustryRepository;
using API_SoCongThuong.Reponsitories.TypeOfBusinessRepository;
using API_SoCongThuong.Reponsitories.TypeOfProfessionRepository;
using API_SoCongThuong.Reponsitories.CategoryRepository;
using API_SoCongThuong.Reponsitories.DistrictRepository;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using API_SoCongThuong.Reponsitories;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.AspNetCore.Mvc.Filters;
using static API_SoCongThuong.Classes.Ulities;
using ClosedXML.Excel;
using System.Globalization;
using API_SoCongThuong.Logger;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Data;

namespace API_SoCongThuong.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TradePromotionOtherController : ControllerBase
    {
        private TradePromotionOtherRepo _repo;
        private IConfiguration _config;
        //private BusinessRepo _repoBusi;

        private readonly ILogger<AsyncLogger> _logger;
        public SoHoa_SoCongThuongContext _context;
        private AsyncLogger _asyncLogger;

        public TradePromotionOtherController(SoHoa_SoCongThuongContext context, IConfiguration configuration, ILogger<AsyncLogger> logger)
        {
            _repo = new TradePromotionOtherRepo(context);
            _config = configuration;

            _logger = logger;
            _context = context;
            _asyncLogger = new AsyncLogger(_logger, _context);

        }

        [Route("find")]
        [HttpPost]
        public IActionResult ListItems_New([FromBody] QueryRequestBody query)
        {

            BaseModels<TradePromotionOtherModel> model = new BaseModels<TradePromotionOtherModel>();
            string _keywordSearch = "";
            bool _orderBy_ASC = true;
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }

                Func<TradePromotionOtherModel, object> _orderByExpression = x => x.Content;
                Dictionary<string, Func<TradePromotionOtherModel, object>> _sortableFields = new Dictionary<string, Func<TradePromotionOtherModel, object>>
                    {
                        { "Content", x => x.Content },
                        { "StartDate", x => x.StartDate },
                        { "Address", x => x.Address },
                        { "ImplementationCost", x => x.ImplementationCost },
                    };

                if (query.Sort != null
                    && !string.IsNullOrEmpty(query.Sort.ColumnName)
                    && _sortableFields.ContainsKey(query.Sort.ColumnName))
                {
                    _orderBy_ASC = ("desc".Equals(query.Sort.Direction.ToLower()) ? false : true);
                    _orderByExpression = _sortableFields[query.Sort.ColumnName];
                }

                IQueryable<TradePromotionOtherModel> _data = _repo._context.TradePromotionOthers.Where(x => !x.IsDel)
                    .Select(info => new TradePromotionOtherModel
                    {
                        TradePromotionOtherId = info.TradePromotionOtherId,
                        TypeOfActivity = info.TypeOfActivity,
                        Content = info.Content,
                        Address = info.Address,
                        StartDate = info.StartDate,
                        ImplementationCost = info.ImplementationCost,
                        StartDateDisplay = string.Format("{0:dd/MM/yyyy}", info.StartDate)
                    }).ToList().AsQueryable();

                if (query.SearchValue != null && query.SearchValue != "")
                {
                    _keywordSearch = query.SearchValue.Trim().ToLower();
                    _data = _data.Where(
                        x => x.Address.ToLower().Contains(_keywordSearch)
                        || x.Content.ToLower().Contains(_keywordSearch)
                        || x.ImplementationCost.ToString().Contains(_keywordSearch)
                    );
                }

                if (query.Filter != null && query.Filter.ContainsKey("Type") && !string.IsNullOrEmpty(query.Filter["Type"]))
                {
                    _data = _data.Where(x => x.TypeOfActivity.ToString() == query.Filter["Type"]);
                }

                if (query.Filter != null && query.Filter.ContainsKey("MinTime") && !string.IsNullOrEmpty(query.Filter["MinTime"]))
                {
                    _data = _data.Where(x =>
                                x.StartDate >=
                                DateTime.ParseExact(query.Filter["MinTime"], "dd/MM/yyyy", null));
                }

                if (query.Filter != null && query.Filter.ContainsKey("MaxTime") && !string.IsNullOrEmpty(query.Filter["MaxTime"]))
                {
                    _data = _data.Where(x =>
                               x.StartDate <=
                                DateTime.ParseExact(query.Filter["MaxTime"], "dd/MM/yyyy", null));
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
        public async Task<IActionResult> create([FromForm] TradePromotionOtherModel data)
        {
            BaseModels<TradePromotionOtherModel> model = new BaseModels<TradePromotionOtherModel>();
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                SystemLog datalog = new SystemLog();


                var Files = Request.Form.Files;
                var LstFile = new List<TradePromotionOtherDetailModel>();
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
                                Linkfile = "TradePromotionOtherDetail"
                            };
                            var result = Ulities.UploadFile(up, _config);

                            TradePromotionOtherDetailModel fileSave = new TradePromotionOtherDetailModel();
                            fileSave.LinkFile = result.link;
                            LstFile.Add(fileSave);
                        }
                    }
                }

                data.Details = LstFile;

                data.StartDate = DateTime.ParseExact(data.StartDateDisplay!, "dd/MM/yyyy", null);
                if (!string.IsNullOrEmpty(data.EndDateDisplay))
                {
                    data.EndDate = DateTime.ParseExact(data.EndDateDisplay, "dd/MM/yyyy", null);
                }
                data.CreateTime = DateTime.Now;
                data.CreateUserId = loginData.Userid;

                await _repo.Insert(data);
                datalog = Ulities.WriteLog(_config, HttpContext, loginData.Username, ActionType_Const.CREATE, NameCategory_Const.TRADE_PROMOTION_OTHER, Action_Status.SUCCESS);
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
        public async Task<IActionResult> Update([FromForm] TradePromotionOtherModel data)
        {
            BaseModels<TradePromotionOtherModel> model = new BaseModels<TradePromotionOtherModel>();
            try
            {

                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                SystemLog datalog = new SystemLog();

                var CheckData = _repo._context.TradePromotionOthers.Where(x => x.TradePromotionOtherId == data.TradePromotionOtherId);
                if (!CheckData.Any())
                {
                    model.status = 0;
                    model.error = new ErrorModel()
                    {
                        Code = ErrCode_Const.PROPERTY_IS_NULL_OR_EMPTY
                    };
                    datalog = Ulities.WriteLog(_config, HttpContext, loginData.Username, ActionType_Const.UPDATE, NameCategory_Const.TRADE_PROMOTION_OTHER, Action_Status.FAIL);
                    _asyncLogger.LogInfo(JsonConvert.SerializeObject(datalog));
                    return BadRequest(model);
                }
                else
                {
                    var Files = Request.Form.Files;
                    var LstFile = new List<TradePromotionOtherDetailModel>();
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
                                    Linkfile = "TradePromotionOtherDetail"
                                };
                                var result = Ulities.UploadFile(up, _config);

                                TradePromotionOtherDetailModel fileSave = new TradePromotionOtherDetailModel();
                                fileSave.LinkFile = result.link;
                                LstFile.Add(fileSave);
                            }
                        }
                    }
                    data.Details = LstFile;

                    data.StartDate = DateTime.ParseExact(data.StartDateDisplay!, "dd/MM/yyyy", null);
                    if (!string.IsNullOrEmpty(data.EndDateDisplay))
                    {
                        data.EndDate = DateTime.ParseExact(data.EndDateDisplay, "dd/MM/yyyy", null);
                    }

                    data.UpdateTime = DateTime.Now;
                    data.UpdateUserId = loginData.Userid;

                    await _repo.Update(data, _config);
                    datalog = Ulities.WriteLog(_config, HttpContext, loginData.Username, ActionType_Const.UPDATE, NameCategory_Const.TRADE_PROMOTION_OTHER, Action_Status.SUCCESS);
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
            BaseModels<TradePromotionOtherModel> model = new BaseModels<TradePromotionOtherModel>();
            try
            {
                var info = _repo.FindById(id, _config);
                if (info == null)
                {
                    return NotFound(ErrMsg_Const.GetMsg(ErrCode_Const.CANNOT_FIND_DATA_BY_QUERY));
                }
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
                await _repo.Delete(id, _config);

                datalog = Ulities.WriteLog(_config, HttpContext, loginData.Username, ActionType_Const.DELETE, NameCategory_Const.TRADE_PROMOTION_OTHER, Action_Status.SUCCESS);
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
            var allSheet = new List<int> { 4, 3, 2, 1 };
            var data = _repo.FindData(query);
            string DateStart = "";
            string DateEnd = "";

            if (!data.Any() || data.Count == 0)
            {
                return BadRequest();
            }

            if (query.Filter != null && query.Filter.ContainsKey("MinTime") && !string.IsNullOrEmpty(query.Filter["MinTime"]))
            {
                DateStart = query.Filter["MinTime"];
            }

            if (query.Filter != null && query.Filter.ContainsKey("MaxTime") && !string.IsNullOrEmpty(query.Filter["MaxTime"]))
            {
                DateEnd = query.Filter["MaxTime"];
            }

            if (query.Filter != null && query.Filter.ContainsKey("Type") && !string.IsNullOrEmpty(query.Filter["Type"]))
            {
                allSheet = allSheet.Where(x => x != int.Parse(query.Filter["Type"]) + 1).ToList();
            }

            try
            {
                using (var workbook = new XLWorkbook(@"Upload/Templates/HoatDongXucTienThuongMaiKhac.xlsx"))
                {
                    var groups = data.GroupBy(x => x.TypeOfActivity);
                    foreach (var group in groups)
                    {
                        IXLWorksheet worksheet = workbook.Worksheets.Worksheet(group.Key + 1);
                        int index = 4;
                        int row = 1;

                        if (!string.IsNullOrEmpty(DateStart))
                        {
                            if (!string.IsNullOrEmpty(DateEnd))
                            {
                                worksheet.Cell(2, 1).Value = $"Từ ngày {DateStart} Đến ngày {DateEnd}";
                            }
                            else
                            {
                                worksheet.Cell(2, 1).Value = $"Từ ngày {DateStart}";
                            }
                            worksheet.Cell(2, 1).Style.Font.SetItalic(true);
                        }

                        foreach (var item in group)
                        {
                            if (row < group.Count())
                            {
                                worksheet.Row(row).CopyTo(worksheet.Row(row + 1));
                            }

                            worksheet.Cell(index, 1).Value = row;
                            worksheet.Cell(index, 2).Value = item.Content;

                            if (group.Key == 0)
                            {
                                string time = string.IsNullOrEmpty(item.Time) ? "" : item.Time.Trim() + ", ";
                                if (!string.IsNullOrEmpty(item.EndDateDisplay))
                                {
                                    worksheet.Cell(index, 3).Value = $"{time}{item.StartDateDisplay} - {item.EndDateDisplay}, tại {item.Address}";
                                }
                                else
                                {
                                    worksheet.Cell(index, 3).Value = $"{time}{item.StartDateDisplay}, tại {item.Address}";
                                }
                                worksheet.Cell(index, 4).Value = item.ImplementationCost;
                                worksheet.Cell(index, 5).Value = item.Participating;
                                worksheet.Cell(index, 6).Value = item.Coordination;
                                worksheet.Cell(index, 7).Value = item.Result;
                            }

                            if (group.Key == 1)
                            {
                                string time = string.IsNullOrEmpty(item.Time) ? "" : item.Time.Trim() + ", ";
                                if (string.IsNullOrEmpty(DateEnd))
                                {
                                    worksheet.Cell(index, 3).Value = $"{time}{item.StartDateDisplay} - {item.EndDateDisplay}, tại {item.Address}";
                                }
                                else
                                {
                                    worksheet.Cell(index, 3).Value = $"{time}{item.StartDateDisplay}, tại {item.Address}";
                                }
                                worksheet.Cell(index, 4).Value = item.ImplementationCost;
                                worksheet.Cell(index, 5).Value = item.Participating;
                            }

                            if (group.Key == 2)
                            {
                                worksheet.Cell(index, 3).Value = item.Time;
                                worksheet.Cell(index, 4).Value = item.ImplementationCost;
                                worksheet.Cell(index, 5).Value = item.Participating;
                                worksheet.Cell(index, 6).Value = item.Note;
                            }

                            if (group.Key == 3)
                            {
                                string time = string.IsNullOrEmpty(item.Time) ? "" : item.Time.Trim() + ", ";
                                if (string.IsNullOrEmpty(DateEnd))
                                {
                                    worksheet.Cell(index, 3).Value = $"{time}{item.StartDateDisplay} - {item.EndDateDisplay}, tại {item.Address}";
                                }
                                else
                                {
                                    worksheet.Cell(index, 3).Value = $"{time}{item.StartDateDisplay}, tại {item.Address}";
                                }
                                worksheet.Cell(index, 4).Value = item.ImplementationCost;
                                worksheet.Cell(index, 5).Value = item.Note;
                            }

                            index++;
                            row++;
                        }
                    }

                    if (allSheet.Count() < 4)
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

