using API_SoCongThuong.Classes;
using API_SoCongThuong.Models;
using API_SoCongThuong.Reponsitories.TrainingClassManagementRepository;
using EF_Core.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.Design;
using System.Text;
using Newtonsoft.Json;
using StackExchange.Redis;


using static API_SoCongThuong.Classes.Ulities;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Office2010.Excel;
using API_SoCongThuong.Logger;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace API_SoCongThuong.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TrainingClassManagementController : ControllerBase
    {
        private TrainingClassManagementRepo _repoTrainingClassManagement;
        private IConfiguration _config;

        private readonly ILogger<AsyncLogger> _logger;
        public SoHoa_SoCongThuongContext _context;
        private AsyncLogger _asyncLogger;

        public TrainingClassManagementController(SoHoa_SoCongThuongContext context, IConfiguration configuration, ILogger<AsyncLogger> logger)
        {
            _repoTrainingClassManagement = new TrainingClassManagementRepo(context);
            _config = configuration;

            _logger = logger;
            _context = context;
            _asyncLogger = new AsyncLogger(_logger, _context);

        }

        [Route("find")]
        [HttpPost]
        public IActionResult ListItems_New([FromBody] QueryRequestBody query)
        {

            BaseModels<TrainingClassManagementModel> model = new BaseModels<TrainingClassManagementModel>();
            string _keywordSearch = "";
            bool _orderBy_ASC = true;
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }

                Func<TrainingClassManagementModel, object> _orderByExpression = x => x.Topic;
                Dictionary<string, Func<TrainingClassManagementModel, object>> _sortableFields = new Dictionary<string, Func<TrainingClassManagementModel, object>> 
                {
                    { "Topic", x => x.Topic },
                };
                if (query.Sort != null
                    && !string.IsNullOrEmpty(query.Sort.ColumnName)
                    && _sortableFields.ContainsKey(query.Sort.ColumnName))
                {
                    _orderBy_ASC = ("desc".Equals(query.Sort.Direction.ToLower()) ? false : true);
                    _orderByExpression = _sortableFields[query.Sort.ColumnName];
                }

                IQueryable<TrainingClassManagementModel> _data = _repoTrainingClassManagement._context.TrainingClassManagements.Select(x => new TrainingClassManagementModel
                {
                    TrainingClassManagementId = x.TrainingClassManagementId,
                    Topic = x.Topic ?? "",
                    Location = x.Location,
                    Participant = x.Participant,
                    TimeStart = x.TimeStart,
                    NumberOfAttendees = x.NumberOfAttendees,
                    IsDel = x.IsDel,
                }).ToList().AsQueryable();

                _data = _data.Where(x => !x.IsDel);

                if (query.SearchValue != null && query.SearchValue != "")
                {
                    _keywordSearch = query.SearchValue.Trim().ToLower();
                    _data = _data.Where(x =>
                       x.Topic.ToLower().Contains(_keywordSearch)
                       || x.Location.ToLower().Contains(_keywordSearch)
                       || x.Participant.ToLower().Contains(_keywordSearch)
                       || x.NumberOfAttendees.ToString().Contains(_keywordSearch)
                   ); 
                }

                if (query.Filter != null && query.Filter.ContainsKey("MinDate") && !string.IsNullOrEmpty(query.Filter["MinDate"]))
                {
                    _data = _data.Where(x =>
                                x.TimeStart.Date >= DateTime.ParseExact(query.Filter["MinDate"], "dd/MM/yyyy", null).Date);
                }

                if (query.Filter != null && query.Filter.ContainsKey("MaxDate") && !string.IsNullOrEmpty(query.Filter["MaxDate"]))
                {
                    _data = _data.Where(x =>
                               x.TimeStart.Date <= DateTime.ParseExact(query.Filter["MaxDate"], "dd/MM/yyyy", null).Date);
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

        [HttpGet("{id}")]
        public IActionResult getItemById(Guid id)
        {
            BaseModels<TrainingClassManagementModel> model = new BaseModels<TrainingClassManagementModel>();
            try
            {
                var result = _repoTrainingClassManagement.FindById(id, _config);
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
        public async Task<IActionResult> Update([FromForm] TrainingClassManagementModel data)
        {
            BaseModels<TrainingClassManagementModel> model = new BaseModels<TrainingClassManagementModel>();
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                SystemLog datalog = new SystemLog();
                var CheckData = _repoTrainingClassManagement.FindById(data.TrainingClassManagementId, _config);
                if (CheckData == null)
                {
                    //chỗ này không tồn tại id
                    model.status = 0;
                    model.error = new ErrorModel()
                    {
                        Code = ErrCode_Const.PROPERTY_IS_NULL_OR_EMPTY
                    };
                    datalog = Ulities.WriteLog(_config, HttpContext, loginData.Username, ActionType_Const.UPDATE, NameCategory_Const.TRAINING_CLASS_MANAGEMENT, Action_Status.FAIL);
                    _asyncLogger.LogInfo(JsonConvert.SerializeObject(datalog));
                    return BadRequest(model);
                }
                else
                {
                    #region gắn hàm upload file
                    var Files = Request.Form.Files;
                    var LstFile = new List<TrainingClassManagementAttachFileModel>();
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
                                    Linkfile = "TrainingClassManagement"
                                };
                                var result = Ulities.UploadFile(up, _config);

                                TrainingClassManagementAttachFileModel fileSave = new TrainingClassManagementAttachFileModel();
                                fileSave.LinkFile = result.link;
                                LstFile.Add(fileSave);
                            }
                        }
                    }
                    data.Details = LstFile;
                    #endregion
                    data.TimeStart = Ulities.ConvertTimeZone(HttpContext.Request.Headers, JsonConvert.DeserializeObject<DateTime>(data.TimeStartGet));
                    data.UpdateTime = DateTime.Now;
                    data.UpdateUserId = loginData.Userid;
                    await _repoTrainingClassManagement.Update(data, _config);
                    datalog = Ulities.WriteLog(_config, HttpContext, loginData.Username, ActionType_Const.UPDATE, NameCategory_Const.TRAINING_CLASS_MANAGEMENT, Action_Status.SUCCESS);
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
        public async Task<IActionResult> create([FromForm] TrainingClassManagementModel data)
        {
            BaseModels<TrainingClassManagement> model = new BaseModels<TrainingClassManagement>();
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }


                #region gắn hàm upload file
                var Files = Request.Form.Files;
                var LstFile = new List<TrainingClassManagementAttachFileModel>();
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
                                Linkfile = "TrainingClassManagement"
                            };
                            var result = Ulities.UploadFile(up, _config);

                            TrainingClassManagementAttachFileModel fileSave = new TrainingClassManagementAttachFileModel();
                            fileSave.LinkFile = result.link;
                            LstFile.Add(fileSave);
                        }
                    }
                }
                data.Details = LstFile;
                #endregion
                data.TimeStart = Ulities.ConvertTimeZone(HttpContext.Request.Headers, JsonConvert.DeserializeObject<DateTime>(data.TimeStartGet));
                data.CreateTime = DateTime.Now;
                data.CreateUserId = loginData.Userid;
                SystemLog datalog = new SystemLog();
                await _repoTrainingClassManagement.Insert(data);
                datalog = Ulities.WriteLog(_config, HttpContext, loginData.Username, ActionType_Const.CREATE, NameCategory_Const.TRAINING_CLASS_MANAGEMENT, Action_Status.SUCCESS);
                _asyncLogger.LogInfo(JsonConvert.SerializeObject(datalog));
                //await _repoTrainingClassManagement.Insert(SaveData);
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

        [HttpPut("deleteTrainingClassManagement/{id}")]
        public async Task<IActionResult> deleteTrainingClassManagement(Guid id)
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
                await _repoTrainingClassManagement.Delete(id, _config);
                datalog = Ulities.WriteLog(_config, HttpContext, loginData.Username, ActionType_Const.DELETE, NameCategory_Const.TRAINING_CLASS_MANAGEMENT, Action_Status.SUCCESS);
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

        private List<TrainingClassManagementModel> FindData([FromBody] QueryRequestBody query)
        {
            string _keywordSearch = ""; //Keyword tìm kiếm
            bool _orderBy_ASC = true;  //Khởi tạo sắp xếp dữ liệu acs hoặc desc khi tìm kiếm
            Func<TrainingClassManagementModel, object> _orderByExpression = x => x.Topic; //Khởi tạo mặc định sắp xếp dữ liệu
            Dictionary<string, Func<TrainingClassManagementModel, object>> _sortableFields = new Dictionary<string, Func<TrainingClassManagementModel, object>>   //Khởi tạo các trường để sắp xếp
            {
                { "Topic", x => x.Topic },
            };
            if (query.Sort != null
                && !string.IsNullOrEmpty(query.Sort.ColumnName)
                && _sortableFields.ContainsKey(query.Sort.ColumnName))
            {
                _orderBy_ASC = ("desc".Equals(query.Sort.Direction.ToLower()) ? false : true);    //Sắp xếp asc hoặc desc
                _orderByExpression = _sortableFields[query.Sort.ColumnName]; //Trường cần sắp xếp
            }
            //Cách 1 dùng entity
            IQueryable<TrainingClassManagementModel> _data = _repoTrainingClassManagement._context.TrainingClassManagements.Select(x => new TrainingClassManagementModel
            {
                TrainingClassManagementId = x.TrainingClassManagementId,
                Topic = x.Topic ?? "",
                Location = x.Location,
                Participant = x.Participant,
                TimeStart = x.TimeStart,
                NumberOfAttendees = x.NumberOfAttendees,
                Details = _repoTrainingClassManagement._context.TrainingClassManagementAttachFiles.Where(d => d.TrainingClassManagementId == x.TrainingClassManagementId).Select(model => new TrainingClassManagementAttachFileModel
                {
                    TrainingClassManagementAttachFileId = model.TrainingClassManagementAttachFileId,
                    LinkFile = string.IsNullOrEmpty(model.LinkFile) ? "" : _config.GetValue<string>("MinioConfig:Protocol") + _config.GetValue<string>("MinioConfig:MinioServer") + model.LinkFile,
                }).ToList(),
                IsDel = x.IsDel,
            });

            _data = _data.Where(x => !x.IsDel);

            if (query.SearchValue != null && query.SearchValue != "") //Kiểm tra điều kiện tìm kiếm
            {
                _keywordSearch = query.SearchValue.Trim().ToLower();
                _data = _data.Where(x =>
                    /* x.TrainingClassManagementId.ToString().ToLower().Contains(_keywordSearch)
                    || */
                    x.Topic.ToLower().Contains(_keywordSearch)
                );  //Lấy table đã select tìm kiếm theo keyword
            }

            if (query.Filter != null && query.Filter.ContainsKey("MinDate") && !string.IsNullOrEmpty(query.Filter["MinDate"]))
            {
                _data = _data.Where(x =>
                            (x.TimeStart) >=
                            DateTime.ParseExact(query.Filter["MinDate"], "dd/MM/yyyy", null));
            }

            if (query.Filter != null && query.Filter.ContainsKey("MaxDate") && !string.IsNullOrEmpty(query.Filter["MaxDate"]))
            {
                _data = _data.Where(x =>
                            x.TimeStart <=
                            DateTime.ParseExact(query.Filter["MaxDate"], "dd/MM/yyyy", null));
            }

            int _countRows = _data.Count(); //Đếm số dòng của table đã select được

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

            try
            {
                using (var workbook = new XLWorkbook(@"Upload/Templates/ATKTMT_LopTapHuan.xlsx"))
                {
                    IXLWorksheet worksheet = workbook.Worksheets.Worksheet(1);

                    int index = 3;
                    int row = 1;
                    foreach (var item in data)
                    {
                        var addrow = worksheet.Row(index - 1);
                        addrow.InsertRowsBelow(1);
                        worksheet.Cell(index, 1).Value = row;
                        worksheet.Cell(index, 2).Value = item.Topic;
                        worksheet.Cell(index, 3).Value = item.Location;
                        worksheet.Cell(index, 4).Value = item.Participant;
                        worksheet.Cell(index, 5).Value = item.NumberOfAttendees;
                        worksheet.Cell(index, 6).Value = item.TimeStart.ToString("dd-MM-yyyy");
                        if (item.Details.Count > 0)
                        {
                            var str = "";
                            foreach(var it in item.Details)
                            {
                                str += it.LinkFile;
                                str += "\r\n";
                            }
                            worksheet.Cell(index, 7).Value = str;
                        }
                        index++;
                        row++;
                    }
                
                    var delrow = worksheet.Row(index);
                    delrow.Delete();
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
