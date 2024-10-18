
using API_SoCongThuong.Classes;
using API_SoCongThuong.Logger;
using API_SoCongThuong.Models;
using API_SoCongThuong.Reponsitories;
using API_SoCongThuong.Reponsitories.ProcessAdministrativeProceduresRepository;
using ClosedXML.Excel;
using EF_Core.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.ComponentModel.Design;
using System.Data;
using static System.Net.Mime.MediaTypeNames;

namespace API_SoCongThuong.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProcessAdministrativeProceduresController : ControllerBase
    {
        private ProcessAdministrativeProceduresRepo _repo;

        private IConfiguration _configuration;
        private readonly ILogger<AsyncLogger> _logger;
        public SoHoa_SoCongThuongContext _context;
        private AsyncLogger _asyncLogger;
        public ProcessAdministrativeProceduresController(SoHoa_SoCongThuongContext context, IConfiguration configuration, ILogger<AsyncLogger> logger)
        {
            _repo = new ProcessAdministrativeProceduresRepo(context);
            _logger = logger;
            _context = context;
            _asyncLogger = new AsyncLogger(_logger, _context);
            _configuration = configuration;
        }

        [Route("find")]
        [HttpPost]
        public IActionResult ListItems_New([FromBody] QueryRequestBody query)//query truyền lên
        {

            BaseModels<ProcessAdministrativeProceduresModel> model = new BaseModels<ProcessAdministrativeProceduresModel>();
            string _keywordSearch = ""; //Keyword tìm kiếm
            bool _orderBy_ASC = true;  //Khởi tạo sắp xếp dữ liệu acs hoặc desc khi tìm kiếm
            try
            {
                //Lấy Token, lấy model
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }

                Func<ProcessAdministrativeProceduresModel, object> _orderByExpression = x => x.ProcessAdministrativeProceduresCode; //Khởi tạo mặc định sắp xếp dữ liệu
                Dictionary<string, Func<ProcessAdministrativeProceduresModel, object>> _sortableFields = new Dictionary<string, Func<ProcessAdministrativeProceduresModel, object>>   //Khởi tạo các trường để sắp xếp
                    {
                        { "ProcessAdministrativeProceduresFieldName", x => x.ProcessAdministrativeProceduresFieldName },
                        { "ProcessAdministrativeProceduresCode", x => x.ProcessAdministrativeProceduresCode },
                        { "ProcessAdministrativeProceduresName", x => x.ProcessAdministrativeProceduresName },
                        { "TotalTimeProcess", x => x.TotalTimeProcess},
                    };
                if (query.Sort != null
                    && !string.IsNullOrEmpty(query.Sort.ColumnName)
                    && _sortableFields.ContainsKey(query.Sort.ColumnName))
                {
                    _orderBy_ASC = ("desc".Equals(query.Sort.Direction.ToLower()) ? false : true);    //Sắp xếp asc hoặc desc
                    _orderByExpression = _sortableFields[query.Sort.ColumnName]; //Trường cần sắp xếp
                }

                IQueryable<ProcessAdministrativeProceduresModel> _data = from p in _repo._context.ProcessAdministrativeProcedures.Where(x => !x.IsDel)
                                                                         join c in _repo._context.Categories
                                                                         on p.ProcessAdministrativeProceduresField equals c.CategoryId
                                                                         //into res
                                                                         //from r in res.DefaultIfEmpty()
                                                                         select new ProcessAdministrativeProceduresModel
                                                                         {
                                                                             ProcessAdministrativeProceduresId = p.ProcessAdministrativeProceduresId,
                                                                             ProcessAdministrativeProceduresField = p.ProcessAdministrativeProceduresField,
                                                                             ProcessAdministrativeProceduresFieldName = c.CategoryName,
                                                                             ProcessAdministrativeProceduresCode = p.ProcessAdministrativeProceduresCode,
                                                                             ProcessAdministrativeProceduresName = p.ProcessAdministrativeProceduresName,
                                                                             TotalTimeProcess = _repo._context.ProcessAdministrativeProceduresSteps.Where(res => res.ProcessAdministrativeProceduresId == p.ProcessAdministrativeProceduresId).Sum(x => x.ProcessingTime),
                                                                             IsDel = p.IsDel
                                                                         };
                //_data = _data

                //Search
                if (query.SearchValue != null && query.SearchValue != "") //Kiểm tra điều kiện tìm kiếm
                {
                    _keywordSearch = query.SearchValue.Trim().ToLower();
                    _data = _data.Where(x =>
                        x.ProcessAdministrativeProceduresFieldName.ToLower().Contains(_keywordSearch) ||
                        x.ProcessAdministrativeProceduresCode.ToLower().Contains(_keywordSearch) ||
                        x.ProcessAdministrativeProceduresName.ToLower().Contains(_keywordSearch) ||
                        x.TotalTimeProcess.ToString().Contains(_keywordSearch)
                   );
                }

                //Filter
                if (query.Filter != null && query.Filter.ContainsKey("idGroupParent") && !string.IsNullOrEmpty(query.Filter["idGroupParent"]))
                {
                    _data = _data.Where(x => x.ProcessAdministrativeProceduresId.ToString().Contains(string.Join("", query.Filter["idGroupParent"])));
                }

                int _countRows = _data.Count(); //Đếm số dòng của table đã select được
                if (_countRows == 0)    //nếu table = 0 thì trả về không có dữ liệu
                {
                    return NotFound("Không có dữ liệu");
                }
                if (query.Panigator.More)    //query more = true
                {
                    model.status = 1;
                    model.items = _data.ToList();
                    model.total = _countRows;
                    return Ok(model);
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
            BaseModels<ProcessAdministrativeProceduresModel> model = new BaseModels<ProcessAdministrativeProceduresModel>();
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
        public async Task<IActionResult> Update(ProcessAdministrativeProceduresModel data)
        {
            BaseModels<ProcessAdministrativeProcedure> model = new BaseModels<ProcessAdministrativeProcedure>();
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                SystemLog datalog = new SystemLog();
                ProcessAdministrativeProcedure? CheckData = _repo._context.ProcessAdministrativeProcedures.Where(x => x.ProcessAdministrativeProceduresId == data.ProcessAdministrativeProceduresId && !x.IsDel).FirstOrDefault();
                if (CheckData != null)
                {
                    var ProcessAdministrativeProceduresCode = _repo.findByProcessAdministrativeProceduresCode(data.ProcessAdministrativeProceduresCode, Guid.Parse(data.ProcessAdministrativeProceduresId.ToString()));

                    if (ProcessAdministrativeProceduresCode)
                    {
                        model.status = 0;
                        model.error = new ErrorModel()
                        {
                            Code = ErrCode_Const.EXCEPTION_API,
                            Msg = "Mã quy trình này đã tồn tại"
                        };
                        datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.UPDATE, NameCategory_Const.PROCESS_ADMINISTRATIVE_PROCEDURE, Action_Status.FAIL);
                        _asyncLogger.LogInfo(JsonConvert.SerializeObject(datalog));
                        return Ok(model);
                    }

                    data.UpdateUserId = loginData.Userid;
                    data.UpdateTime = DateTime.Now;

                    await _repo.Update(data);
                    datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.UPDATE, NameCategory_Const.PROCESS_ADMINISTRATIVE_PROCEDURE, Action_Status.SUCCESS);
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
                    datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.UPDATE, NameCategory_Const.PROCESS_ADMINISTRATIVE_PROCEDURE, Action_Status.FAIL);
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

        [HttpPost()]
        public async Task<IActionResult> create(ProcessAdministrativeProceduresModel data)
        {
            BaseModels<ProcessAdministrativeProcedure> model = new BaseModels<ProcessAdministrativeProcedure>();
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                var ProcessAdministrativeProceduresCode = _repo.findByProcessAdministrativeProceduresCode(data.ProcessAdministrativeProceduresCode, null);
                SystemLog datalog = new SystemLog();
                if (ProcessAdministrativeProceduresCode)
                {
                    model.status = 0;
                    model.error = new ErrorModel()
                    {
                        Code = ErrCode_Const.EXCEPTION_API,
                        Msg = "Mã thủ tục hành chính đã tồn tại"
                    };
                    datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.CREATE, NameCategory_Const.PROCESS_ADMINISTRATIVE_PROCEDURE, Action_Status.FAIL);
                    _asyncLogger.LogInfo(JsonConvert.SerializeObject(datalog));
                    return BadRequest(model);
                }

                data.CreateUserId = loginData.Userid;
                data.CreateTime = DateTime.Now;

                await _repo.Insert(data);
                datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.CREATE, NameCategory_Const.PROCESS_ADMINISTRATIVE_PROCEDURE, Action_Status.SUCCESS);
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
                datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.DELETE, NameCategory_Const.PROCESS_ADMINISTRATIVE_PROCEDURE, Action_Status.SUCCESS);
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

        [Route("loadfield")]
        [HttpGet]
        public IActionResult LoadProcessAdministrativeProceduresField()
        {
            BaseModels<Category> model = new BaseModels<Category>();

            try
            {
                //Lấy Token, lấy model
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }

                IQueryable<Category> _data = _repo._context.Categories.Where(x => x.CategoryTypeCode == "ADMINISTRATIVE_PROCEDURE_FIELD" && x.IsAction == true);

                model.status = 1;
                model.items = _data.ToList();
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

        private List<ProcessAdministrativeProceduresModel> FindData([FromBody] QueryRequestBody query)
        {
            string _keywordSearch = ""; //Keyword tìm kiếm
            bool _orderBy_ASC = true;  //Khởi tạo sắp xếp dữ liệu acs hoặc desc khi tìm kiếm
            Func<ProcessAdministrativeProceduresModel, object> _orderByExpression = x => x.ProcessAdministrativeProceduresCode; //Khởi tạo mặc định sắp xếp dữ liệu
            Dictionary<string, Func<ProcessAdministrativeProceduresModel, object>> _sortableFields = new Dictionary<string, Func<ProcessAdministrativeProceduresModel, object>>   //Khởi tạo các trường để sắp xếp
                    {
                        { "ProcessAdministrativeProceduresFieldName", x => x.ProcessAdministrativeProceduresFieldName },
                        { "ProcessAdministrativeProceduresCode", x => x.ProcessAdministrativeProceduresCode },
                        { "ProcessAdministrativeProceduresName", x => x.ProcessAdministrativeProceduresName },
                        { "TotalTimeProcess", x => x.TotalTimeProcess},
                    };
            if (query.Sort != null
                && !string.IsNullOrEmpty(query.Sort.ColumnName)
                && _sortableFields.ContainsKey(query.Sort.ColumnName))
            {
                _orderBy_ASC = ("desc".Equals(query.Sort.Direction.ToLower()) ? false : true);    //Sắp xếp asc hoặc desc
                _orderByExpression = _sortableFields[query.Sort.ColumnName]; //Trường cần sắp xếp
            }

            IQueryable<ProcessAdministrativeProceduresModel> _data = from p in _repo._context.ProcessAdministrativeProcedures.Where(x => !x.IsDel)
                                                                     join c in _repo._context.Categories
                                                                     on p.ProcessAdministrativeProceduresField equals c.CategoryId
                                                                     //into res
                                                                     //from r in res.DefaultIfEmpty()
                                                                     select new ProcessAdministrativeProceduresModel
                                                                     {
                                                                         ProcessAdministrativeProceduresId = p.ProcessAdministrativeProceduresId,
                                                                         ProcessAdministrativeProceduresField = p.ProcessAdministrativeProceduresField,
                                                                         ProcessAdministrativeProceduresFieldName = c.CategoryName,
                                                                         ProcessAdministrativeProceduresCode = p.ProcessAdministrativeProceduresCode,
                                                                         ProcessAdministrativeProceduresName = p.ProcessAdministrativeProceduresName,
                                                                         TotalTimeProcess = _repo._context.ProcessAdministrativeProceduresSteps.Where(res => res.ProcessAdministrativeProceduresId == p.ProcessAdministrativeProceduresId).Sum(x => x.ProcessingTime),
                                                                         IsDel = p.IsDel
                                                                     };
            //_data = _data

            //Search
            if (query.SearchValue != null && query.SearchValue != "") //Kiểm tra điều kiện tìm kiếm
            {
                _keywordSearch = query.SearchValue.Trim().ToLower();
                _data = _data.Where(x =>
                    x.ProcessAdministrativeProceduresFieldName.ToLower().Contains(_keywordSearch) ||
                    x.ProcessAdministrativeProceduresCode.ToLower().Contains(_keywordSearch) ||
                    x.ProcessAdministrativeProceduresName.ToLower().Contains(_keywordSearch) ||
                    x.TotalTimeProcess.ToString().Contains(_keywordSearch)
               );
            }

            //Filter
            if (query.Filter != null && query.Filter.ContainsKey("idGroupParent") && !string.IsNullOrEmpty(query.Filter["idGroupParent"]))
            {
                _data = _data.Where(x => x.ProcessAdministrativeProceduresId.ToString().Contains(string.Join("", query.Filter["idGroupParent"])));
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
                using (var workbook = new XLWorkbook(@"Upload/Templates/QuyTrinhNoiBoGiaiQuyetThuTucHanhChinh.xlsx"))
                {
                    IXLWorksheet worksheet = workbook.Worksheets.Worksheet(1);

                    int index = 5;
                    int row = 1;
                    foreach (var item in data)
                    {
                        var addrow = worksheet.Row(index - 1);
                        addrow.InsertRowsBelow(1);

                        worksheet.Cell(index, 1).Value = row;
                        worksheet.Cell(index, 2).Value = item.ProcessAdministrativeProceduresFieldName;
                        worksheet.Cell(index, 3).Value = item.ProcessAdministrativeProceduresCode;
                        worksheet.Cell(index, 4).Value = item.ProcessAdministrativeProceduresName;
                        worksheet.Cell(index, 5).Value = $"{item.TotalTimeProcess} ngày ";

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

        [HttpGet("ExportById/{id}")]
        public IActionResult Export(Guid id)
        {
            var data = _context.ProcessAdministrativeProcedures.Where(x => x.ProcessAdministrativeProceduresId == id && !x.IsDel).FirstOrDefault();
            var steps = _context.ProcessAdministrativeProceduresSteps.Where(x => x.ProcessAdministrativeProceduresId == id).ToList();

            if (data == null)
            {
                return BadRequest();
            }

            try
            {
                using (var workbook = new XLWorkbook(@"Upload/Templates/QuyTrinhNoiBoGiaiQuyetTTHC.xlsx"))
                {
                    IXLWorksheet worksheet = workbook.Worksheets.Worksheet(1);

                    worksheet.Cell(1, 2).Value = data.ProcessAdministrativeProceduresName;
                    worksheet.Cell(2, 2).Value = data.ProcessAdministrativeProceduresCode;
                    worksheet.Cell(3, 2).Value = _repo._context.Categories
                                                    .Where(x => x.CategoryId == data.ProcessAdministrativeProceduresField)
                                                    .Select(x => x.CategoryName)
                                                    .FirstOrDefault();

                    int index = 6;
                    int row = 1;
                    foreach (var item in steps)
                    {
                        var addrow = worksheet.Row(index);
                        addrow.InsertRowsBelow(1);
                        worksheet.Cell(index, 1).Value = "Bước " + row.ToString();
                        worksheet.Cell(index, 2).Value = item.ImplementingAgencies;
                        worksheet.Cell(index, 3).Value = item.ContentImplementation;
                        worksheet.Cell(index, 4).Value = item.ProcessingTime;

                        worksheet.Row(index).Style.Font.SetBold(false);
                        worksheet.Row(index).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        worksheet.Cell(index, 4).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                        index++;
                        row++;
                    }
                    var delrow = worksheet.Row(index);
                    delrow.Delete();
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
