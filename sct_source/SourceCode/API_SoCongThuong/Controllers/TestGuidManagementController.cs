using API_SoCongThuong.Classes;
using API_SoCongThuong.Models;
using API_SoCongThuong.Reponsitories.TestGuidManagementRepository;
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
using API_SoCongThuong.Logger;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace API_SoCongThuong.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestGuidManagementController : ControllerBase
    {
        private TestGuidManagementRepo _repoTestGuidManagement;
        private IConfiguration _config;

        private readonly ILogger<AsyncLogger> _logger;
        public SoHoa_SoCongThuongContext _context;
        private AsyncLogger _asyncLogger;

        public TestGuidManagementController(SoHoa_SoCongThuongContext context, IConfiguration configuration, ILogger<AsyncLogger> logger)
        {
            _repoTestGuidManagement = new TestGuidManagementRepo(context);
            _config = configuration;
            _logger = logger;
            _context = context;
            _asyncLogger = new AsyncLogger(_logger, _context);
        }

        [Route("find")]
        [HttpPost]
        public IActionResult ListItems_New([FromBody] QueryRequestBody query)//query truyền lên
        {

            BaseModels<TestGuidManagementModel> model = new BaseModels<TestGuidManagementModel>();
            string _keywordSearch = ""; //Keyword tìm kiếm
            bool _orderBy_ASC = true;  //Khởi tạo sắp xếp dữ liệu acs hoặc desc khi tìm kiếm
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }

                Func<TestGuidManagementModel, object> _orderByExpression = x => x.InspectionAgency; //Khởi tạo mặc định sắp xếp dữ liệu
                Dictionary<string, Func<TestGuidManagementModel, object>> _sortableFields = new Dictionary<string, Func<TestGuidManagementModel, object>>   //Khởi tạo các trường để sắp xếp
                {
                    { "InspectionAgency", x => x.InspectionAgency },
                };
                if (query.Sort != null
                    && !string.IsNullOrEmpty(query.Sort.ColumnName)
                    && _sortableFields.ContainsKey(query.Sort.ColumnName))
                {
                    _orderBy_ASC = ("desc".Equals(query.Sort.Direction.ToLower()) ? false : true);    //Sắp xếp asc hoặc desc
                    _orderByExpression = _sortableFields[query.Sort.ColumnName]; //Trường cần sắp xếp
                }
                //Cách 1 dùng entity
                IQueryable<TestGuidManagementModel> _data = _repoTestGuidManagement._context.TestGuidManagements.Select(x => new TestGuidManagementModel
                {
                    TestGuidManagementId = x.TestGuidManagementId,
                    InspectionAgency = x.InspectionAgency ?? "",
                    Time = x.Time,
                    CoordinationAgency = x.CoordinationAgency,
                    Result = x.Result,
                    IsDel = x.IsDel,
                }); ;
                _data = _data.Where(x => !x.IsDel);
                if (query.SearchValue != null && query.SearchValue != "") //Kiểm tra điều kiện tìm kiếm
                {
                    _keywordSearch = query.SearchValue.Trim().ToLower();
                    _data = _data.Where(x =>
                       x.InspectionAgency.ToLower().Contains(_keywordSearch)
                       || x.CoordinationAgency.ToLower().Contains(_keywordSearch)
                       || x.Result.ToLower().Contains(_keywordSearch)
                   );
                }

                if (query.Filter != null && query.Filter.ContainsKey("MinDate") && !string.IsNullOrEmpty(query.Filter["MinDate"]))
                {
                    _data = _data.Where(x => x.Time >= DateTime.ParseExact(query.Filter["MinDate"], "dd/MM/yyyy", null));
                }

                if (query.Filter != null && query.Filter.ContainsKey("MaxDate") && !string.IsNullOrEmpty(query.Filter["MaxDate"]))
                {
                    _data = _data.Where(x => x.Time <= DateTime.ParseExact(query.Filter["MaxDate"], "dd/MM/yyyy", null));
                }

                int _countRows = _data.Count(); //Đếm số dòng của table đã select được
                if (_countRows == 0)    //nếu table = 0 thì trả về không có dữ liệu
                {
                    return NotFound("Không có dữ liệu");
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

                if (query.Panigator.More)    //query more = true
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
            BaseModels<TestGuidManagementModel> model = new BaseModels<TestGuidManagementModel>();
            try
            {
                var result = _repoTestGuidManagement.FindById(id, _config);
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
        public async Task<IActionResult> Update([FromForm] TestGuidManagementModel data)
        {
            BaseModels<TestGuidManagementModel> model = new BaseModels<TestGuidManagementModel>();
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                var CheckData = _repoTestGuidManagement.FindById(data.TestGuidManagementId, _config);
                SystemLog datalog = new SystemLog();
                if (CheckData == null)
                {
                    //chỗ này không tồn tại id
                    model.status = 0;
                    model.error = new ErrorModel()
                    {
                        Code = ErrCode_Const.PROPERTY_IS_NULL_OR_EMPTY
                    };
                    datalog = Ulities.WriteLog(_config, HttpContext, loginData.Username, ActionType_Const.UPDATE, NameCategory_Const.TEST_GUID_MANAGEMENT, Action_Status.FAIL);
                    _asyncLogger.LogInfo(JsonConvert.SerializeObject(datalog));
                    return BadRequest(model);
                }
                else
                {
                    #region gắn hàm upload file
                    var Files = Request.Form.Files;
                    var LstFile = new List<TestGuidManagementAttachFileModel>();
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
                                    Linkfile = "TestGuidManagement"
                                };
                                var result = Ulities.UploadFile(up, _config);

                                TestGuidManagementAttachFileModel fileSave = new TestGuidManagementAttachFileModel();
                                fileSave.LinkFile = result.link;
                                LstFile.Add(fileSave);
                            }
                        }
                    }
                    data.Details = LstFile;
                    #endregion
                    data.Time = Ulities.ConvertTimeZone(HttpContext.Request.Headers, JsonConvert.DeserializeObject<DateTime>(data.TimeGet));
                    data.UpdateTime = DateTime.Now;
                    data.UpdateUserId = loginData.Userid;
                    await _repoTestGuidManagement.Update(data, _config);
                    datalog = Ulities.WriteLog(_config, HttpContext, loginData.Username, ActionType_Const.UPDATE, NameCategory_Const.TEST_GUID_MANAGEMENT, Action_Status.SUCCESS);
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
        public async Task<IActionResult> create([FromForm] TestGuidManagementModel data)
        {
            BaseModels<TestGuidManagement> model = new BaseModels<TestGuidManagement>();
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }

                SystemLog datalog = new SystemLog();
                #region gắn hàm upload file
                var Files = Request.Form.Files;
                var LstFile = new List<TestGuidManagementAttachFileModel>();
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
                                Linkfile = "TestGuidManagement"
                            };
                            var result = Ulities.UploadFile(up, _config);

                            TestGuidManagementAttachFileModel fileSave = new TestGuidManagementAttachFileModel();
                            fileSave.LinkFile = result.link;
                            LstFile.Add(fileSave);
                        }
                    }
                }
                data.Details = LstFile;
                #endregion
                data.Time = Ulities.ConvertTimeZone(HttpContext.Request.Headers, JsonConvert.DeserializeObject<DateTime>(data.TimeGet));
                data.CreateTime = DateTime.Now;
                data.CreateUserId = loginData.Userid;

                await _repoTestGuidManagement.Insert(data);
                datalog = Ulities.WriteLog(_config, HttpContext, loginData.Username, ActionType_Const.CREATE, NameCategory_Const.TEST_GUID_MANAGEMENT, Action_Status.SUCCESS);
                _asyncLogger.LogInfo(JsonConvert.SerializeObject(datalog));
                //await _repoTestGuidManagement.Insert(SaveData);
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

        [HttpPut("deleteTestGuidManagement/{id}")]
        public async Task<IActionResult> deleteTestGuidManagement(Guid id)
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
                await _repoTestGuidManagement.Delete(id, _config);
                datalog = Ulities.WriteLog(_config, HttpContext, loginData.Username, ActionType_Const.DELETE, NameCategory_Const.TEST_GUID_MANAGEMENT, Action_Status.SUCCESS);
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

        private List<TestGuidManagementModel> FindData([FromBody] QueryRequestBody query)//query truyền lên
        {
            string _keywordSearch = ""; //Keyword tìm kiếm
            bool _orderBy_ASC = true;  //Khởi tạo sắp xếp dữ liệu acs hoặc desc khi tìm kiếm

            Func<TestGuidManagementModel, object> _orderByExpression = x => x.InspectionAgency; //Khởi tạo mặc định sắp xếp dữ liệu
            Dictionary<string, Func<TestGuidManagementModel, object>> _sortableFields = new Dictionary<string, Func<TestGuidManagementModel, object>>   //Khởi tạo các trường để sắp xếp
            {
                { "InspectionAgency", x => x.InspectionAgency },
            };
            if (query.Sort != null
                && !string.IsNullOrEmpty(query.Sort.ColumnName)
                && _sortableFields.ContainsKey(query.Sort.ColumnName))
            {
                _orderBy_ASC = ("desc".Equals(query.Sort.Direction.ToLower()) ? false : true);    //Sắp xếp asc hoặc desc
                _orderByExpression = _sortableFields[query.Sort.ColumnName]; //Trường cần sắp xếp
            }
            //Cách 1 dùng entity
            IQueryable<TestGuidManagementModel> _data = _repoTestGuidManagement._context.TestGuidManagements.Select(x => new TestGuidManagementModel
            {
                TestGuidManagementId = x.TestGuidManagementId,
                InspectionAgency = x.InspectionAgency ?? "",
                Time = x.Time,
                CoordinationAgency = x.CoordinationAgency,
                Result = x.Result,
                IsDel = x.IsDel,
                Details = _repoTestGuidManagement._context.TestGuidManagementAttachFiles.Where(d => d.TestGuidManagementId == x.TestGuidManagementId).Select(model => new TestGuidManagementAttachFileModel
                {
                    TestGuidManagementAttachFileId = model.TestGuidManagementAttachFileId,
                    LinkFile = string.IsNullOrEmpty(model.LinkFile) ? "" : _config.GetValue<string>("MinioConfig:Protocol") + _config.GetValue<string>("MinioConfig:MinioServer") + model.LinkFile,
                }).ToList()
            });
            _data = _data.Where(x => !x.IsDel);
            if (query.SearchValue != null && query.SearchValue != "") //Kiểm tra điều kiện tìm kiếm
            {
                _keywordSearch = query.SearchValue.Trim().ToLower();
                _data = _data.Where(x =>
                    /* x.TestGuidManagementId.ToString().ToLower().Contains(_keywordSearch)
                    || */
                    x.InspectionAgency.ToLower().Contains(_keywordSearch) ||
                    x.CoordinationAgency.ToLower().Contains(_keywordSearch) ||
                    x.Result.ToLower().Contains(_keywordSearch)
                );  //Lấy table đã select tìm kiếm theo keyword
            }

            if (query.Filter != null && query.Filter.ContainsKey("MinDate") && !string.IsNullOrEmpty(query.Filter["MinDate"]))
            {
                _data = _data.Where(x => x.Time >= DateTime.ParseExact(query.Filter["MinDate"], "dd/MM/yyyy", null));
            }

            if (query.Filter != null && query.Filter.ContainsKey("MaxDate") && !string.IsNullOrEmpty(query.Filter["MaxDate"]))
            {
                _data = _data.Where(x => x.Time <= DateTime.ParseExact(query.Filter["MaxDate"], "dd/MM/yyyy", null));
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

            try
            {
                using (var workbook = new XLWorkbook(@"Upload/Templates/ATKTMT_KiemTraCongTacBVMT.xlsx"))
                {
                    IXLWorksheet worksheet = workbook.Worksheets.Worksheet(1);

                    int index = 3;
                    int row = 1;
                    foreach (var item in data)
                    {
                        var addrow = worksheet.Row(index - 1);
                        addrow.InsertRowsBelow(1);
                        worksheet.Cell(index, 1).Value = row;
                        worksheet.Cell(index, 2).Value = item.InspectionAgency;
                        worksheet.Cell(index, 3).Value = item.Time.ToString("dd-MM-yyyy HH:mm");
                        worksheet.Cell(index, 4).Value = item.CoordinationAgency;
                        worksheet.Cell(index, 5).Value = item.Result;
                        if (item.Details.Count > 0)
                        {
                            var str = "";
                            foreach (var it in item.Details)
                            {
                                str += it.LinkFile;
                                str += "\r\n";
                            }
                            worksheet.Cell(index, 6).Value = str;
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
