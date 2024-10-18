
using API_SoCongThuong.Classes;
using API_SoCongThuong.Logger;
using API_SoCongThuong.Models;
using API_SoCongThuong.Reponsitories.AdminFormalitiesRepository;
using API_SoCongThuong.Reponsitories.CategoryRepository;
using ClosedXML.Excel;
using EF_Core.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace API_SoCongThuong.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminFormalitiesController : ControllerBase
    {
        private AdminFormalitiesRepo _repoAdminFormalities;
        private CategoryRepo _repoCategory;
        private IConfiguration _configuration;
        private readonly ILogger<AsyncLogger> _logger;
        public SoHoa_SoCongThuongContext _context;
        private AsyncLogger _asyncLogger;

        public AdminFormalitiesController(SoHoa_SoCongThuongContext context, IConfiguration configuration, ILogger<AsyncLogger> logger)
        {
            _repoAdminFormalities = new AdminFormalitiesRepo(context);
            _repoCategory = new CategoryRepo(context);

            _logger = logger;
            _context = context;
            _asyncLogger = new AsyncLogger(_logger, _context);
            _configuration = configuration;
        }

        [Route("loadfields")]
        [HttpGet]
        public IActionResult LoadFields()
        {
            BaseModels<FieldView> model = new BaseModels<FieldView>();

            try
            {
                //Lấy Token, lấy model
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }

                //Query lấy data
                IQueryable<FieldView> _data = _repoCategory._context.Categories.Where(x => x.CategoryTypeCode == "ADMINISTRATIVE_PROCEDURE_FIELD").Select(x => new FieldView
                {
                    FieldId = x.CategoryId,
                    FieldName = x.CategoryName,
                    Priority = x.Piority,
                });

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

        [Route("find")]
        [HttpPost]
        public IActionResult ListItems_New([FromBody] QueryRequestBody query)//query truyền lên
        {

            BaseModels<AdminFormalitiesViewModel> model = new BaseModels<AdminFormalitiesViewModel>();
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

                Func<AdminFormalitiesViewModel, object> _orderByExpression = x => x.AdminFormalitiesCode; //Khởi tạo mặc định sắp xếp dữ liệu
                Dictionary<string, Func<AdminFormalitiesViewModel, object>> _sortableFields = new Dictionary<string, Func<AdminFormalitiesViewModel, object>>   //Khởi tạo các trường để sắp xếp
                    {
                        { "AdminFormalitiesCode", x => x.AdminFormalitiesCode },
                        { "AdminFormalitiesName", x => x.AdminFormalitiesName },
                        { "FieldName" , x => x.FieldName },
                        { "DVCLevel" , x => x.DVCLevel }
                    };
                if (query.Sort != null
                    && !string.IsNullOrEmpty(query.Sort.ColumnName)
                    && _sortableFields.ContainsKey(query.Sort.ColumnName))
                {
                    _orderBy_ASC = ("desc".Equals(query.Sort.Direction.ToLower()) ? false : true);    //Sắp xếp asc hoặc desc
                    _orderByExpression = _sortableFields[query.Sort.ColumnName]; //Trường cần sắp xếp
                }

                var _data = from A in _repoAdminFormalities._context.AdministrativeFormalities
                            where !A.IsDel
                            join C in _repoCategory._context.Categories.Select(x => new { x.CategoryId, x.CategoryName })
                                on A.FieldId equals C.CategoryId
                            select (new AdminFormalitiesViewModel
                            {
                                AdminFormalitiesId = A.AdminFormalitiesId,
                                AdminFormalitiesCode = A.AdminFormalitiesCode,
                                AdminFormalitiesName = A.AdminFormalitiesName,
                                FieldId = A.FieldId,
                                FieldName = C.CategoryName,
                                DVCLevel = A.Dvclevel,
                                DocUrl = A.DocUrl,
                                IsDel = A.IsDel
                            });

                if (query.SearchValue != null && query.SearchValue != "") //Kiểm tra điều kiện tìm kiếm
                {
                    _keywordSearch = query.SearchValue.Trim().ToLower();
                    _data = _data.Where(x =>
                       x.AdminFormalitiesName.ToLower().Contains(_keywordSearch)
                       || x.AdminFormalitiesCode.ToLower().Contains(_keywordSearch)
                       || x.FieldName.ToLower().Contains(_keywordSearch)
                       || x.DVCLevel.ToString().Contains(_keywordSearch)
                   );  //Lấy table đã select tìm kiếm theo keyword
                }

                if (query.Filter != null && query.Filter.ContainsKey("FieldId") && !string.IsNullOrEmpty(query.Filter["FieldId"]))
                {
                    _data = _data.Where(x => x.FieldId.ToString().Contains(string.Join("", query.Filter["FieldId"])));
                }

                if (query.Filter != null && query.Filter.ContainsKey("DVCLevel") && !string.IsNullOrEmpty(query.Filter["DVCLevel"]))
                {
                    _data = _data.Where(x => x.DVCLevel.ToString().Contains(string.Join("", query.Filter["DVCLevel"])));
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
            BaseModels<AdministrativeFormality> model = new BaseModels<AdministrativeFormality>();
            try
            {
                var result = _repoAdminFormalities.FindById(id);
                if (result == null)
                    return NotFound(ErrMsg_Const.GetMsg(ErrCode_Const.CANNOT_FIND_DATA_BY_QUERY));

                model.status = 1;
                model.items = result.ToList();
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
        public async Task<IActionResult> Update(AdministrativeFormality data)
        {
            BaseModels<AdministrativeFormality> model = new BaseModels<AdministrativeFormality>();
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                SystemLog datalog = new SystemLog();
                AdministrativeFormality? SaveData = _repoAdminFormalities._context.AdministrativeFormalities.Where(x => x.AdminFormalitiesId == data.AdminFormalitiesId).FirstOrDefault();
                if (SaveData != null)
                {
                    var adminFormality = _repoAdminFormalities.findByAdminFormalityCode(data.AdminFormalitiesCode, Guid.Parse(data.AdminFormalitiesId.ToString()));

                    if (adminFormality)
                    {
                        model.status = 0;
                        model.error = new ErrorModel()
                        {
                            Code = ErrCode_Const.EXCEPTION_API,
                            Msg = "Mã thủ tục hành chính đã tồn tại"
                        };
                        datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.UPDATE, NameCategory_Const.ADMIN_FOMALITIES, Action_Status.FAIL);
                        _asyncLogger.LogInfo(JsonConvert.SerializeObject(datalog));
                        return Ok(model);
                    }

                    var util = new Ulities();
                    data = util.TrimModel(data);

                    SaveData.AdminFormalitiesId = Guid.Parse(data.AdminFormalitiesId.ToString());
                    SaveData.AdminFormalitiesCode = data.AdminFormalitiesCode;
                    SaveData.AdminFormalitiesName = data.AdminFormalitiesName;
                    SaveData.FieldId = data.FieldId;
                    SaveData.Dvclevel = data.Dvclevel;
                    SaveData.DocUrl = data.DocUrl;
                    SaveData.UpdateUserId = loginData.Userid;
                    SaveData.UpdateTime = DateTime.Now;

                    await _repoAdminFormalities.Update(SaveData);
                    datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.UPDATE, NameCategory_Const.ADMIN_FOMALITIES, Action_Status.SUCCESS);
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
        public async Task<IActionResult> create(AdministrativeFormality data)
        {
            BaseModels<AdministrativeFormality> model = new BaseModels<AdministrativeFormality>();
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                SystemLog datalog = new SystemLog();
                var adminFormality = _repoAdminFormalities.findByAdminFormalityCode(data.AdminFormalitiesCode, null);

                if (adminFormality)
                {
                    model.status = 0;
                    model.error = new ErrorModel()
                    {
                        Code = ErrCode_Const.EXCEPTION_API,
                        Msg = "Mã thủ tục hành chính đã tồn tại"
                    };
                    datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.CREATE, NameCategory_Const.ADMIN_FOMALITIES, Action_Status.FAIL);
                    _asyncLogger.LogInfo(JsonConvert.SerializeObject(datalog));
                    return BadRequest(model);
                }

                var util = new Ulities();
                data = util.TrimModel(data);

                AdministrativeFormality SaveData = new AdministrativeFormality();
                SaveData.AdminFormalitiesCode = data.AdminFormalitiesCode;
                SaveData.AdminFormalitiesName = data.AdminFormalitiesName;
                SaveData.FieldId = data.FieldId;
                SaveData.Dvclevel = data.Dvclevel;
                SaveData.DocUrl = data.DocUrl;
                SaveData.CreateUserId = loginData.Userid;
                SaveData.CreateTime = DateTime.Now;

                await _repoAdminFormalities.Insert(SaveData);
                datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.CREATE, NameCategory_Const.ADMIN_FOMALITIES, Action_Status.SUCCESS);
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

        [HttpPut("deleteAdminFormality/{id}")]
        public async Task<IActionResult> deleteAdminFormality(Guid id)
        {
            BaseModels<AdministrativeFormality> model = new BaseModels<AdministrativeFormality>();
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                await _repoAdminFormalities.DeleteAdminFormalities(id);
                SystemLog datalog = new SystemLog();
                datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.DELETE, NameCategory_Const.ADMIN_FOMALITIES, Action_Status.SUCCESS);
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

        [Route("HtmlToPDF")]
        [HttpGet()]
        public async Task<IActionResult> HtmlToPdf()
        {
            BaseModels<AdministrativeFormality> model = new BaseModels<AdministrativeFormality>();
            try
            {
                await _repoAdminFormalities.ConvertHtmlToPdf();

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

        //[Route("ImportData")]
        //[HttpPost()]
        //public async Task<IActionResult> ImportData([FromForm] IFormFile File)
        //{
        //    BaseModels<AdministrativeFormality> model = new BaseModels<AdministrativeFormality>();
        //    try
        //    {
        //        await _repoAdminFormalities.ImportData(File);

        //        model.status = 1;
        //        return Ok(model);
        //    }
        //    catch (Exception ex)
        //    {

        //        model.status = 0;
        //        model.error = new ErrorModel()
        //        {
        //            Code = ErrCode_Const.EXCEPTION_API,
        //            Msg = ex.Message
        //        };
        //        return BadRequest(model);
        //    }
        //}

        [HttpPost("Export")]
        public IActionResult Export([FromBody] QueryRequestBody query)
        {
            var data = from A in _repoAdminFormalities._context.AdministrativeFormalities
                       where !A.IsDel
                       join C in _repoCategory._context.Categories.Select(x => new { x.CategoryId, x.CategoryName })
                           on A.FieldId equals C.CategoryId
                       select (new AdminFormalitiesViewModel
                       {
                           AdminFormalitiesId = A.AdminFormalitiesId,
                           AdminFormalitiesCode = A.AdminFormalitiesCode,
                           AdminFormalitiesName = A.AdminFormalitiesName,
                           FieldId = A.FieldId,
                           FieldName = C.CategoryName,
                           DVCLevel = A.Dvclevel,
                           DocUrl = A.DocUrl,
                           IsDel = A.IsDel
                       });

            string FilterTitle = "";
            if (query.Filter != null && query.Filter.ContainsKey("FieldId") && !string.IsNullOrEmpty(query.Filter["FieldId"]))
            {
                data = data.Where(x => x.FieldId.ToString().Contains(string.Join("", query.Filter["FieldId"])));
                FilterTitle += " Lĩnh vực " + data.Select(x => x.FieldName).FirstOrDefault();
            }

            if (query.Filter != null && query.Filter.ContainsKey("DVCLevel") && !string.IsNullOrEmpty(query.Filter["DVCLevel"]))
            {
                data = data.Where(x => x.DVCLevel.ToString().Contains(string.Join("", query.Filter["DVCLevel"])));
                FilterTitle += data.Select(x => x.DVCLevel).FirstOrDefault() == 1 ? " Toàn trình" : " Còn lại";
            }

            if (!data.Any())
            {
                return BadRequest();
            }

            try
            {
                using (var workbook = new XLWorkbook(@"Upload/Templates/ThuTucHanhChinh.xlsx"))
                {
                    IXLWorksheet worksheet = workbook.Worksheets.Worksheet(1);
                    worksheet.Cell(2, 1).Value = "THỦ TỤC HÀNH CHÍNH" + FilterTitle.ToUpper();
                    int index = 5;
                    int row = 1;
                    foreach (var item in data)
                    {
                        var addrow = worksheet.Row(index - 1);
                        addrow.InsertRowsBelow(1);
                        worksheet.Cell(index, 1).Value = row;
                        worksheet.Cell(index, 2).Value = item.FieldName;
                        worksheet.Cell(index, 3).Value = item.AdminFormalitiesCode;
                        worksheet.Cell(index, 4).Value = item.AdminFormalitiesName;
                        worksheet.Cell(index, 5).Value = item.DVCLevel;
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
