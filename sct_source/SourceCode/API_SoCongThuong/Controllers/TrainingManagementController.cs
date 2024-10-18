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
using DocumentFormat.OpenXml.EMMA;
using System.Globalization;
using API_SoCongThuong.Logger;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Data;

namespace API_SoCongThuong.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TrainingManagementController : ControllerBase
    {
        private TrainingManagementRepo _repo;
        private IConfiguration _config;
        //private BusinessRepo _repoBusi;

        private readonly ILogger<AsyncLogger> _logger;
        public SoHoa_SoCongThuongContext _context;
        private AsyncLogger _asyncLogger;

        public TrainingManagementController(SoHoa_SoCongThuongContext context, IConfiguration configuration, ILogger<AsyncLogger> logger)
        {
            _repo = new TrainingManagementRepo(context);
            _logger = logger;
            _context = context;
            _asyncLogger = new AsyncLogger(_logger, _context);
            _config = configuration;
        }

        [Route("find")]
        [HttpPost]
        public IActionResult ListItems_New([FromBody] QueryRequestBody query)
        {
            BaseModels<TrainingManagementModel> model = new BaseModels<TrainingManagementModel>();
            string _keywordSearch = "";
            bool _orderBy_ASC = true;
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }

                Func<TrainingManagementModel, object> _orderByExpression = x => x.Content;
                Dictionary<string, Func<TrainingManagementModel, object>> _sortableFields = new Dictionary<string, Func<TrainingManagementModel, object>>
                    {
                        { "Content", x => x.Content },
                        { "StartDate", x => x.StartDate },
                        { "Address", x => x.Address },
                        { "Participating", x => x.Participating },
                        { "NumParticipating", x => x.NumParticipating },
                        { "ImplementationCost", x => x.ImplementationCost },
                    };

                if (query.Sort != null && !string.IsNullOrEmpty(query.Sort.ColumnName) && _sortableFields.ContainsKey(query.Sort.ColumnName))
                {
                    _orderBy_ASC = ("desc".Equals(query.Sort.Direction.ToLower()) ? false : true);
                    _orderByExpression = _sortableFields[query.Sort.ColumnName];
                }

                IQueryable<TrainingManagementModel> _data = _repo._context.TrainingManagements.Where(x => !x.IsDel)
                    .Select(info => new TrainingManagementModel
                    {
                        TrainingManagementId = info.TrainingManagementId,
                        Content = info.Content,
                        StartDate = info.StartDate,
                        StartDateDisplay = info.StartDate.ToString("dd'/'MM'/'yyyy"),
                        DistrictId = info.DistrictId,
                        Address = info.Address,
                        Participating = info.Participating,
                        NumParticipating = info.NumParticipating,
                        ImplementationCost = info.ImplementationCost,
                    }).ToList().AsQueryable();

                if (query.SearchValue != null && query.SearchValue != "")
                {
                    _keywordSearch = query.SearchValue.Trim().ToLower();
                    _data = _data.Where(x =>
                        x.Content.ToLower().Contains(_keywordSearch)
                        || x.Address.ToLower().Contains(_keywordSearch)
                        || x.Participating.ToLower().Contains(_keywordSearch)
                        || x.NumParticipating.ToString().Contains(_keywordSearch)
                        || x.ImplementationCost.ToString().Contains(_keywordSearch)
                    );
                }

                if (query.Filter != null && query.Filter.ContainsKey("DistrictId") && !string.IsNullOrEmpty(query.Filter["DistrictId"]))
                {
                    _data = _data.Where(x => x.DistrictId.ToString() == query.Filter["DistrictId"]);
                }

                if (query.Filter != null && query.Filter.ContainsKey("MinTime") && !string.IsNullOrEmpty(query.Filter["MinTime"]))
                {
                    _data = _data.Where(x =>
                        x.StartDate >= DateTime.ParseExact(query.Filter["MinTime"], "dd/MM/yyyy", null));
                }

                if (query.Filter != null && query.Filter.ContainsKey("MaxTime") && !string.IsNullOrEmpty(query.Filter["MaxTime"]))
                {
                    _data = _data.Where(x =>
                        x.StartDate <= DateTime.ParseExact(query.Filter["MaxTime"], "dd/MM/yyyy", null));
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
        public async Task<IActionResult> Create([FromForm] TrainingManagementModel data)
        {
            BaseModels<TrainingManagementModel> model = new BaseModels<TrainingManagementModel>();
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                SystemLog datalog = new SystemLog();

                var Files = Request.Form.Files;
                var LstFile = new List<TrainingManagementAttachFileModel>();
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
                                Linkfile = "TrainingManagement"
                            };
                            var result = Ulities.UploadFile(up, _config);

                            TrainingManagementAttachFileModel fileSave = new TrainingManagementAttachFileModel();
                            fileSave.LinkFile = result.link;
                            LstFile.Add(fileSave);
                        }
                    }
                }
                data.Details = LstFile;

                data = new Ulities().TrimModel(data);
                data.StartDate = DateTime.ParseExact(data.StartDateDisplay!, "dd/MM/yyyy", null);
                if (!string.IsNullOrEmpty(data.EndDateDisplay))
                {
                    data.EndDate = DateTime.ParseExact(data.EndDateDisplay, "dd/MM/yyyy", null);
                }
                data.CreateTime = DateTime.Now;
                data.CreateUserId = loginData.Userid;

                await _repo.Insert(data);

                datalog = Ulities.WriteLog(_config, HttpContext, loginData.Username, ActionType_Const.CREATE, NameCategory_Const.TRAINING_MANAGEMENT, Action_Status.SUCCESS);
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
        public async Task<IActionResult> Update([FromForm] TrainingManagementModel data)
        {
            BaseModels<TrainingManagementModel> model = new BaseModels<TrainingManagementModel>();
            try
            {

                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }

                SystemLog datalog = new SystemLog();
                var CheckData = _repo._context.TrainingManagements.Where(x => !x.IsDel && x.TrainingManagementId == data.TrainingManagementId);
                if (!CheckData.Any())
                {
                    model.status = 0;
                    model.error = new ErrorModel()
                    {
                        Code = ErrCode_Const.PROPERTY_IS_NULL_OR_EMPTY
                    };
                    datalog = Ulities.WriteLog(_config, HttpContext, loginData.Username, ActionType_Const.UPDATE, NameCategory_Const.TRAINING_MANAGEMENT, Action_Status.FAIL);
                    _asyncLogger.LogInfo(JsonConvert.SerializeObject(datalog));
                    return BadRequest(model);
                }
                else
                {
                    var Files = Request.Form.Files;
                    if (Files.Any())
                    {
                        var LstFile = new List<TrainingManagementAttachFileModel>();
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
                                        Linkfile = "TrainingManagement"
                                    };
                                    var result = Ulities.UploadFile(up, _config);

                                    TrainingManagementAttachFileModel fileSave = new TrainingManagementAttachFileModel();
                                    fileSave.LinkFile = result.link;
                                    LstFile.Add(fileSave);
                                }
                            }
                        }
                        data.Details = LstFile;
                    }

                    data = new Ulities().TrimModel(data);
                    data.StartDate = DateTime.ParseExact(data.StartDateDisplay!, "dd/MM/yyyy", null);
                    if (!string.IsNullOrEmpty(data.EndDateDisplay))
                    {
                        data.EndDate = DateTime.ParseExact(data.EndDateDisplay, "dd/MM/yyyy", null);
                    }
                    data.UpdateTime = DateTime.Now;
                    data.UpdateUserId = loginData.Userid;

                    await _repo.Update(data, _config);
                    datalog = Ulities.WriteLog(_config, HttpContext, loginData.Username, ActionType_Const.UPDATE, NameCategory_Const.TRAINING_MANAGEMENT, Action_Status.SUCCESS);
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
            BaseModels<TrainingManagementModel> model = new BaseModels<TrainingManagementModel>();
            try
            {
                var info = _repo.FindById(id, _config);
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
                await _repo.Delete(id, _config);

                datalog = Ulities.WriteLog(_config, HttpContext, loginData.Username, ActionType_Const.DELETE, NameCategory_Const.TRAINING_MANAGEMENT, Action_Status.SUCCESS);
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

            if (!data.Any() || data.Count == 0)
            {
                return BadRequest();
            }

            string districtName = "";
            if (query.Filter != null && query.Filter.ContainsKey("DistrictId") && !string.IsNullOrEmpty(query.Filter["DistrictId"]))
            {
                districtName = _context.Districts.Where(x => x.DistrictId.ToString() == query.Filter["DistrictId"]).FirstOrDefault()!.DistrictName;
                if (!string.IsNullOrEmpty(districtName))
                {
                    districtName = districtName + " ";
                }
            }

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

            try
            {
                using (var workbook = new XLWorkbook(@"Upload/Templates/QuanLyDaoTaoTapHuan.xlsx"))
                {
                    IXLWorksheet worksheet = workbook.Worksheets.Worksheet(1);

                    string filter = "";
                    if (!string.IsNullOrEmpty(fromDate))
                    {
                        if (!string.IsNullOrEmpty(toDate))
                        {
                            filter = $"Từ ngày: {fromDate}, Đến ngày: {toDate}.";
                        }
                        else
                        {
                            filter = $"Từ ngày: {fromDate}.";
                        }
                    }
                    else if (!string.IsNullOrEmpty(toDate))
                    {
                        filter = $"Đến ngày: {toDate}.";
                    }
                    
                    if (!string.IsNullOrEmpty(districtName))
                    {
                        filter = districtName + filter;
                    }
                    worksheet.Cell(2, 1).Value = filter;
                    worksheet.Cell(2, 1).Style.Font.SetItalic(true);

                    int index = 4;
                    int row = 1;

                    foreach (var item in data)
                    {
                        if (row < data.Count())
                        {
                            worksheet.Row(index).CopyTo(worksheet.Row(index + 1));
                        }

                        worksheet.Cell(index, 1).Value = row;
                        worksheet.Cell(index, 2).Value = item.Content;
                        worksheet.Cell(index, 3).Value = "Ngày " + item.StartDate.ToString("dd'/'MM'/'yyyy") + ", tại " + item.Address.Trim();
                        worksheet.Cell(index, 4).Value = item.ImplementationCost;
                        worksheet.Cell(index, 5).Value = item.NumParticipating;
                        worksheet.Cell(index, 6).Value = item.Participating;
                        worksheet.Cell(index, 7).Value = item.Annunciator ?? "";

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
