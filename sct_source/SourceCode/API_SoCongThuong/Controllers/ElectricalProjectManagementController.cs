
using API_SoCongThuong.Classes;
using API_SoCongThuong.Models;
using API_SoCongThuong.Reponsitories.CountryRepository;
using EF_Core.Models;
using Microsoft.AspNetCore.Mvc;
using ClosedXML.Excel;
using RestSharp.Extensions;
using API_SoCongThuong.Logger;
using Newtonsoft.Json;
using System.Data;

namespace API_SoCongThuong.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ElectricalProjectManagementController : ControllerBase
    {
        private ElectricalProjectManagementRepo _repo;
        private IConfiguration _configuration;
        private readonly ILogger<AsyncLogger> _logger;
        public SoHoa_SoCongThuongContext _context;
        private AsyncLogger _asyncLogger;
        public ElectricalProjectManagementController(SoHoa_SoCongThuongContext context, IConfiguration configuration, ILogger<AsyncLogger> logger)
        {
            _repo = new ElectricalProjectManagementRepo(context);
            _logger = logger;
            _context = context;
            _asyncLogger = new AsyncLogger(_logger, _context);
            _configuration = configuration;
        }

        [Route("find")]
        [HttpPost]
        public IActionResult Find([FromBody] QueryRequestBody query)
        {
            BaseModels<ElectricalProjectManagementModel> model = new BaseModels<ElectricalProjectManagementModel>();
            string _keywordSearch = "";
            bool _orderBy_ASC = true;
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }

                Func<ElectricalProjectManagementModel, object> _orderByExpression = x => x.BuildingCode;
                Dictionary<string, Func<ElectricalProjectManagementModel, object>> _sortableFields = new Dictionary<string, Func<ElectricalProjectManagementModel, object>>
                    {
                        { "BuildingCode", x => x.BuildingCode },
                        { "BuildingName", x => x.BuildingName },
                        { "Note", x => x.Note },
                        { "Status", x => x.Status },
                        { "Address", x => x.Address },
                        { "Wattage", x => x.Wattage },
                        { "Length", x => x.Length },
                        { "WireType", x => x.WireType },
                        { "VoltageLevelName", x => x.VoltageLevelName },
                    };

                if (query.Sort != null && !string.IsNullOrEmpty(query.Sort.ColumnName) && _sortableFields.ContainsKey(query.Sort.ColumnName))
                {
                    _orderBy_ASC = ("desc".Equals(query.Sort.Direction.ToLower()) ? false : true);
                    _orderByExpression = _sortableFields[query.Sort.ColumnName];
                }

                IQueryable<ElectricalProjectManagementModel> _data = (from l in _repo._context.ElectricalProjectManagements
                                                                      where !l.IsDel
                                                                      join d in _repo._context.Categories on l.VoltageLevel equals d.CategoryId
                                                                      select new ElectricalProjectManagementModel
                                                                      {
                                                                          ElectricalProjectManagementId = l.ElectricalProjectManagementId,
                                                                          BuildingCode = l.BuildingCode,
                                                                          BuildingName = l.BuildingName,
                                                                          Represent = l.Represent,
                                                                          Status = l.Status,
                                                                          Address = l.Address,
                                                                          District = l.District,
                                                                          Note = l.Note,
                                                                          VoltageLevel = l.VoltageLevel,
                                                                          VoltageLevelName = d.CategoryName,
                                                                          TypeOfConstruction = l.TypeOfConstruction,
                                                                          Wattage = l.Wattage,
                                                                          Length = l.Length,
                                                                          WireType = l.WireType,
                                                                      }).ToList().AsQueryable();

                if (query.SearchValue != null && query.SearchValue != "")
                {
                    _keywordSearch = query.SearchValue.Trim().ToLower();
                    _data = _data.Where(x =>
                       x.BuildingCode.ToLower().Contains(_keywordSearch)
                       || x.BuildingName.ToLower().Contains(_keywordSearch)
                       //|| x.Represent.ToLower().Contains(_keywordSearch)
                       || x.Address.ToLower().Contains(_keywordSearch)
                       || x.Wattage.ToLower().Contains(_keywordSearch)
                       || x.Length.ToLower().Contains(_keywordSearch)
                       || x.WireType.ToLower().Contains(_keywordSearch)
                       || x.VoltageLevelName.ToLower().Contains(_keywordSearch)
                       || x.Note.ToLower().Contains(_keywordSearch)
                   );
                }

                //if (query.Filter != null && query.Filter.ContainsKey("District") && !string.IsNullOrEmpty(query.Filter["District"]))
                //{
                //    _data = _data.Where(x => x.District.ToString() == query.Filter["District"]);
                //}

                //if (query.Filter != null && query.Filter.ContainsKey("Status") && !string.IsNullOrEmpty(query.Filter["Status"]))
                //{
                //    _data = _data.Where(x => x.Status.ToString() == query.Filter["Status"]);
                //}

                if (query.Filter != null && query.Filter.ContainsKey("VoltageLevel") && !string.IsNullOrEmpty(query.Filter["VoltageLevel"]))
                {
                    _data = _data.Where(x => x.VoltageLevel.ToString() == query.Filter["VoltageLevel"]);
                }

                if (query.Filter != null && query.Filter.ContainsKey("TypeOfConstruction") && !string.IsNullOrEmpty(query.Filter["TypeOfConstruction"]))
                {
                    _data = _data.Where(x => x.TypeOfConstruction.ToString() == query.Filter["TypeOfConstruction"]);
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
        public async Task<IActionResult> Create(ElectricalProjectManagementModel data)
        {
            BaseModels<ElectricalProjectManagement> model = new BaseModels<ElectricalProjectManagement>();
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                SystemLog datalog = new SystemLog();
                ElectricalProjectManagement SaveData = new ElectricalProjectManagement();
                SaveData.BuildingCode = data.BuildingCode;
                SaveData.BuildingName = data.BuildingName;
                SaveData.District = data.District;
                SaveData.Address = data.Address;
                SaveData.Represent = data.Represent;
                SaveData.Status = data.Status;
                SaveData.Note = data.Note;

                SaveData.CreateUserId = loginData.Userid;
                SaveData.CreateTime = DateTime.Now;
                SaveData.VoltageLevel = data.VoltageLevel;
                SaveData.TypeOfConstruction = data.TypeOfConstruction;
                SaveData.Wattage = data.Wattage;
                SaveData.Length = data.Length;
                SaveData.WireType = data.WireType;

                await _repo.Insert(SaveData);
                datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.CREATE, NameCategory_Const.ELECTRICAL_PROJECT_MANAGEMENT, Action_Status.SUCCESS);
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
        public IActionResult GetItemById(Guid id)
        {
            BaseModels<ElectricalProjectManagementModel> model = new BaseModels<ElectricalProjectManagementModel>();
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
        public async Task<IActionResult> Update(ElectricalProjectManagementModel data)
        {
            BaseModels<ElectricalProjectManagementModel> model = new BaseModels<ElectricalProjectManagementModel>();
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                SystemLog datalog = new SystemLog();
                ElectricalProjectManagement? SaveData = _repo._context.ElectricalProjectManagements.Where(x => x.ElectricalProjectManagementId == data.ElectricalProjectManagementId && !x.IsDel).FirstOrDefault();
                if (SaveData != null)
                {
                    SaveData.BuildingCode = data.BuildingCode;
                    SaveData.BuildingName = data.BuildingName;
                    SaveData.District = data.District;
                    SaveData.Address = data.Address;
                    SaveData.Represent = data.Represent;
                    SaveData.Status = data.Status;
                    SaveData.Note = data.Note;
                    SaveData.VoltageLevel = data.VoltageLevel;
                    SaveData.TypeOfConstruction = data.TypeOfConstruction;
                    SaveData.UpdateUserId = loginData.Userid;
                    SaveData.UpdateTime = DateTime.Now;

                    SaveData.Wattage = data.Wattage;
                    SaveData.Length = data.Length;
                    SaveData.WireType = data.WireType;

                    await _repo.Update(SaveData);
                    datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.UPDATE, NameCategory_Const.ELECTRICAL_PROJECT_MANAGEMENT, Action_Status.SUCCESS);
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
                    datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.UPDATE, NameCategory_Const.ELECTRICAL_PROJECT_MANAGEMENT, Action_Status.FAIL);
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
            BaseModels<ElectricalProjectManagement> model = new BaseModels<ElectricalProjectManagement>();
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                SystemLog datalog = new SystemLog();
                ElectricalProjectManagement? DeleteData = _repo._context.ElectricalProjectManagements.Where(x => !x.IsDel && x.ElectricalProjectManagementId == id).FirstOrDefault();

                if (DeleteData != null)
                {
                    DeleteData.IsDel = true;

                    await _repo.Delete(DeleteData);
                    datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.DELETE, NameCategory_Const.ELECTRICAL_PROJECT_MANAGEMENT, Action_Status.SUCCESS);
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
                    datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.DELETE, NameCategory_Const.ELECTRICAL_PROJECT_MANAGEMENT, Action_Status.FAIL);
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
                IQueryable<ElectricalProjectManagementModel> data = (from l in _repo._context.ElectricalProjectManagements
                                                                      where !l.IsDel
                                                                     join d in _repo._context.Categories on l.VoltageLevel equals d.CategoryId
                                                                     select new ElectricalProjectManagementModel
                                                                      {
                                                                          ElectricalProjectManagementId = l.ElectricalProjectManagementId,
                                                                          BuildingCode = l.BuildingCode,
                                                                          BuildingName = l.BuildingName,
                                                                          Represent = l.Represent,
                                                                          Status = l.Status,
                                                                          Address = l.Address,
                                                                          District = l.District,
                                                                          Note = l.Note,
                                                                          VoltageLevelName = d.CategoryName,
                                                                          TypeOfConstruction = l.TypeOfConstruction,
                                                                          Wattage = l.Wattage,
                                                                          Length = l.Length,
                                                                          WireType = l.WireType,
                                                                      }).ToList().AsQueryable();

                string Title = "QUẢN LÝ CÔNG TRÌNH CAO ÁP TRÊN ĐỊA BÀN ";
                string Status = "";
                string District = "TỈNH";

                string _keywordSearch = "";
                if (query.SearchValue != null && query.SearchValue != "")
                {
                    _keywordSearch = query.SearchValue.Trim().ToLower();
                    data = data.Where(x =>
                       x.BuildingCode.ToLower().Contains(_keywordSearch)
                       || x.BuildingName.ToLower().Contains(_keywordSearch)
                       //|| x.Represent.ToLower().Contains(_keywordSearch)
                       || x.Address.ToLower().Contains(_keywordSearch)
                       || x.Wattage.ToLower().Contains(_keywordSearch)
                       || x.Length.ToLower().Contains(_keywordSearch)
                       || x.WireType.ToLower().Contains(_keywordSearch)
                       || x.VoltageLevelName.ToLower().Contains(_keywordSearch)
                       || x.Note.ToLower().Contains(_keywordSearch)
                   );
                }

                if (query.Filter != null && query.Filter.ContainsKey("District") && !string.IsNullOrEmpty(query.Filter["District"]))
                {
                    data = data.Where(x => x.District.ToString() == query.Filter["District"]);
                    string DistrictName = _repo._context.Districts.Where(x => !x.IsDel && x.DistrictId.ToString() == query.Filter["District"]).FirstOrDefault().DistrictName ?? "";
                    if (DistrictName.HasValue())
                    {
                        District = "HUYỆN " + DistrictName.ToUpper();
                    }
                }

                if (query.Filter != null && query.Filter.ContainsKey("Status") && !string.IsNullOrEmpty(query.Filter["Status"]))
                {
                    data = data.Where(x => x.Status.ToString() == query.Filter["Status"]);
                    Status = query.Filter["Status"] == "1" ? "HOẠT ĐỘNG" : query.Filter["Status"] == "2" ? "TẠM NGỪNG" : "DỪNG HOẠT ĐỘNG";
                }

                if (!data.Any())
                {
                    return BadRequest();
                }

                using (var workbook = new XLWorkbook(@"Upload/Templates/Quanlycongtrinhdien.xlsx"))
                {
                    IXLWorksheet worksheet = workbook.Worksheets.Worksheet(1);
                    worksheet.Cell(1, 1).Value = (Title + Status + District).ToUpper();
                    int index = 5;

                    foreach (var item in data)
                    {
                        worksheet.Cell(index, 1).Value = index - 4;
                        worksheet.Cell(index, 2).Value = item.BuildingCode;
                        worksheet.Cell(index, 3).Value = item.BuildingName;
                        worksheet.Cell(index, 4).Value = item.Wattage;
                        worksheet.Cell(index, 5).Value = item.Length;
                        worksheet.Cell(index, 6).Value = item.WireType;
                        worksheet.Cell(index, 7).Value = item.VoltageLevelName;

                        worksheet.Cell(index, 8).Value = item.Represent;
                        worksheet.Cell(index, 9).Value = item.Status == 1 ? "Đang vận hành" : item.Status == 2 ? "Chưa vận hành" : "Tạm ngưng vận hành";
                        worksheet.Cell(index, 10).Value = item.Address;
                        worksheet.Cell(index, 11).Value = item.Note;
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
