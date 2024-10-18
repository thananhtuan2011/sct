
using API_SoCongThuong.Classes;
using API_SoCongThuong.Logger;
using API_SoCongThuong.Models;
using API_SoCongThuong.Reponsitories.ElectricityInspectorCardRepository;
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
    public class ElectricityInspectorCardController : ControllerBase
    {
        private ElectricityInspectorCardRepo _repo;

        private IConfiguration _configuration;
        private readonly ILogger<AsyncLogger> _logger;
        public SoHoa_SoCongThuongContext _context;
        private AsyncLogger _asyncLogger;

        public ElectricityInspectorCardController(SoHoa_SoCongThuongContext context, IConfiguration configuration, ILogger<AsyncLogger> logger)
        {
            _repo = new ElectricityInspectorCardRepo(context);
            _logger = logger;
            _context = context;
            _asyncLogger = new AsyncLogger(_logger, _context);
            _configuration = configuration;
        }

        [Route("find")]
        [HttpPost]
        public IActionResult ListItems_New([FromBody] QueryRequestBody query)//query truyền lên
        {

            BaseModels<ElectricityInspectorCardModel> model = new BaseModels<ElectricityInspectorCardModel>();
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

                Func<ElectricityInspectorCardModel, object> _orderByExpression = x => x.InspectorName;
                Dictionary<string, Func<ElectricityInspectorCardModel, object>> _sortableFields = new Dictionary<string, Func<ElectricityInspectorCardModel, object>>
                    {
                        { "InspectorName", x => x.InspectorName },
                        { "Birthday", x => x.BirthdayDate },
                        { "LicenseDate", x => x.LicenseDateDate },
                        { "Degree", x => x.Degree },
                        { "Unit", x => x.Unit },
                        { "Seniority", x => x.Seniority },
                        { "CardColor", x => x.CardColor },
                    };

                if (query.Sort != null
                    && !string.IsNullOrEmpty(query.Sort.ColumnName)
                    && _sortableFields.ContainsKey(query.Sort.ColumnName))
                {
                    _orderBy_ASC = ("desc".Equals(query.Sort.Direction.ToLower()) ? false : true);
                    _orderByExpression = _sortableFields[query.Sort.ColumnName];
                }

                IQueryable<ElectricityInspectorCardModel> _data = (from e in _repo._context.ElectricityInspectorCards
                                                                   where !e.IsDel
                                                                   join c in _repo._context.Categories.Where(x => !x.IsDel && x.CategoryTypeCode == "CARD_COLOR")
                                                                   on e.CardColor equals c.CategoryId
                                                                   select new ElectricityInspectorCardModel
                                                                   {
                                                                       ElectricityInspectorCardId = e.ElectricityInspectorCardId,
                                                                       InspectorName = e.InspectorName,
                                                                       Birthday = e.Birthday.ToString("dd'/'MM'/'yyyy"),
                                                                       BirthdayDate = e.Birthday,
                                                                       LicenseDate = e.LicenseDate.ToString("dd'/'MM'/'yyyy"),
                                                                       LicenseDateDate = e.LicenseDate,
                                                                       Degree = e.Degree,
                                                                       Unit = e.Unit,
                                                                       Seniority = e.Seniority,
                                                                       CardColor = e.CardColor,
                                                                       CardColorName = c.CategoryName,
                                                                   }).ToList().AsQueryable();

                if (query.SearchValue != null && query.SearchValue != "")
                {
                    _keywordSearch = query.SearchValue.Trim().ToLower();
                    _data = _data.Where(x =>
                       x.InspectorName.ToLower().Contains(_keywordSearch)
                       || x.Degree.ToLower().Contains(_keywordSearch)
                       || x.Unit.ToLower().Contains(_keywordSearch)
                       || x.Seniority.ToLower().Contains(_keywordSearch)
                       || x.CardColorName.ToLower().Contains(_keywordSearch)
                   );
                }

                if (query.Filter != null && query.Filter.ContainsKey("MinDate") && !string.IsNullOrEmpty(query.Filter["MinDate"]))
                {
                    _data = _data.Where(x => DateTime.ParseExact(x.LicenseDate, "dd/MM/yyyy", null) >= DateTime.ParseExact(query.Filter["MinDate"], "dd/MM/yyyy", null));
                }

                if (query.Filter != null && query.Filter.ContainsKey("MaxDate") && !string.IsNullOrEmpty(query.Filter["MaxDate"]))
                {
                    _data = _data.Where(x => DateTime.ParseExact(x.LicenseDate, "dd/MM/yyyy", null) <= DateTime.ParseExact(query.Filter["MaxDate"], "dd/MM/yyyy", null));
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
            BaseModels<ElectricityInspectorCardModel> model = new BaseModels<ElectricityInspectorCardModel>();
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
        public async Task<IActionResult> Update(ElectricityInspectorCardModel data)
        {
            BaseModels<ElectricityInspectorCard> model = new BaseModels<ElectricityInspectorCard>();
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }

                var util = new Ulities();
                data = util.TrimModel(data);
                SystemLog datalog = new SystemLog();
                ElectricityInspectorCard? SaveData = _repo._context.ElectricityInspectorCards.Where(x => x.ElectricityInspectorCardId == data.ElectricityInspectorCardId && !x.IsDel).FirstOrDefault();
                if (SaveData != null)
                {
                    SaveData.ElectricityInspectorCardId = Guid.Parse(data.ElectricityInspectorCardId.ToString());
                    SaveData.InspectorName = data.InspectorName;
                    SaveData.Birthday = DateTime.ParseExact(data.Birthday, "dd/MM/yyyy", null);
                    SaveData.LicenseDate = DateTime.ParseExact(data.LicenseDate, "dd/MM/yyyy", null);
                    SaveData.Degree = data.Degree;
                    SaveData.Unit = data.Unit;
                    SaveData.Seniority = data.Seniority;
                    SaveData.CardColor = data.CardColor;
                    SaveData.UpdateUserId = loginData.Userid;
                    SaveData.UpdateTime = DateTime.Now;

                    await _repo.Update(SaveData);
                    datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.UPDATE, NameCategory_Const.ELECTRICITY_INSPECTOR_CARD, Action_Status.SUCCESS);
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
                    datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.UPDATE, NameCategory_Const.ELECTRICITY_INSPECTOR_CARD, Action_Status.FAIL);
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
        public async Task<IActionResult> Create(ElectricityInspectorCardModel data)
        {
            BaseModels<ElectricityInspectorCard> model = new BaseModels<ElectricityInspectorCard>();
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                SystemLog datalog = new SystemLog();
                var util = new Ulities();
                data = util.TrimModel(data);

                ElectricityInspectorCard SaveData = new ElectricityInspectorCard();
                SaveData.InspectorName = data.InspectorName;
                SaveData.Birthday = DateTime.ParseExact(data.Birthday, "dd/MM/yyyy", null);
                SaveData.LicenseDate = DateTime.ParseExact(data.LicenseDate, "dd/MM/yyyy", null);
                SaveData.Degree = data.Degree;
                SaveData.Unit = data.Unit;
                SaveData.Seniority = data.Seniority;
                SaveData.CardColor = data.CardColor;
                SaveData.CreateUserId = loginData.Userid;
                SaveData.CreateTime = DateTime.Now;

                await _repo.Insert(SaveData);
                datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.CREATE, NameCategory_Const.ELECTRICITY_INSPECTOR_CARD, Action_Status.SUCCESS);
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
            BaseModels<ElectricityInspectorCard> model = new BaseModels<ElectricityInspectorCard>();
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                SystemLog datalog = new SystemLog();
                ElectricityInspectorCard DeleteData = new ElectricityInspectorCard();
                DeleteData.ElectricityInspectorCardId = id;
                DeleteData.IsDel = true;
                await _repo.DeleteElectricityInspectorCard(DeleteData);
                datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.DELETE, NameCategory_Const.ELECTRICITY_INSPECTOR_CARD, Action_Status.SUCCESS);
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

        [HttpPost("ExportExcel")]
        public IActionResult Export([FromBody] QueryRequestBody query)
        {
            var data = _repo.FindData(query);

            if (!data.Any())
            {
                return BadRequest();
            }

            string Title = "DANH SÁCH CẤP THẺ KIỂM TRA VIÊN ĐIỆN LỰC ";
            string FromDate = "";
            string ToDate = "ĐẾN HIỆN TẠI.";

            if (query.Filter != null && query.Filter.ContainsKey("MinDate") && !string.IsNullOrEmpty(query.Filter["MinDate"]))
            {
                FromDate = "TỪ NGÀY " + query.Filter["MinDate"] + " ";
            }

            if (query.Filter != null && query.Filter.ContainsKey("MaxDate") && !string.IsNullOrEmpty(query.Filter["MaxDate"]))
            {
                ToDate = "ĐẾN NGÀY " + query.Filter["MaxDate"] + ".";
            }

            try
            {
                using (var workbook = new XLWorkbook(@"Upload/Templates/Danhsachcapthekiemtraviendienluc.xlsx"))
                {
                    IXLWorksheet worksheet = workbook.Worksheets.Worksheet(1);
                    worksheet.Cell(2, 1).Value = Title + FromDate + ToDate;

                    int index = 6;
                    int row = 1;

                    foreach (var item in data)
                    {
                        if (row == 1)
                        {
                            worksheet.Cell(index, 1).Value = row;
                            worksheet.Cell(index, 2).Value = item.InspectorName;
                            worksheet.Cell(index, 3).Value = item.Birthday;
                            worksheet.Cell(index, 4).Value = item.Degree;
                            worksheet.Cell(index, 5).Value = item.Unit;
                            worksheet.Cell(index, 6).Value = item.Seniority;
                            worksheet.Cell(index, 7).Value = item.CardColorName == "Hồng" ? "x" : "";
                            worksheet.Cell(index, 8).Value = item.CardColorName == "Cam" ? "x" : "";
                            worksheet.Cell(index, 9).Value = item.CardColorName == "Vàng" ? "x" : "";
                            index++;
                            row++;
                        }
                        else
                        {
                            var addrow = worksheet.Row(index - 1);
                            addrow.InsertRowsBelow(1);
                            worksheet.Cell(index, 1).Value = row;
                            worksheet.Cell(index, 2).Value = item.InspectorName;
                            worksheet.Cell(index, 3).Value = item.Birthday;
                            worksheet.Cell(index, 4).Value = item.Degree;
                            worksheet.Cell(index, 5).Value = item.Unit;
                            worksheet.Cell(index, 6).Value = item.Seniority;
                            worksheet.Cell(index, 7).Value = item.CardColorName == "Hồng" ? "x" : "";
                            worksheet.Cell(index, 8).Value = item.CardColorName == "Cam" ? "x" : "";
                            worksheet.Cell(index, 9).Value = item.CardColorName == "Vàng" ? "x" : "";
                            index++;
                            row++;
                        }
                    }

                    var delrow = worksheet.Row(index);
                    delrow.Delete();

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
