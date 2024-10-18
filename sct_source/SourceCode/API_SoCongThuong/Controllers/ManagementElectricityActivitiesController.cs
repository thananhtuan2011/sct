
using API_SoCongThuong.Classes;
using API_SoCongThuong.Logger;
using API_SoCongThuong.Models;
using API_SoCongThuong.Reponsitories.ManagementElectricityActivitiesRepository;
using ClosedXML.Excel;
using EF_Core.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Data;

namespace API_SoCongThuong.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ManagementElectricityActivitiesController : ControllerBase
    {
        private ManagementElectricityActivitiesRepo _repo;
        private IConfiguration _configuration;
        private readonly ILogger<AsyncLogger> _logger;
        public SoHoa_SoCongThuongContext _context;
        private AsyncLogger _asyncLogger;

        public ManagementElectricityActivitiesController(SoHoa_SoCongThuongContext context, IConfiguration configuration, ILogger<AsyncLogger> logger)
        {
            _repo = new ManagementElectricityActivitiesRepo(context);
            _logger = logger;
            _context = context;
            _asyncLogger = new AsyncLogger(_logger, _context);
            _configuration = configuration;
        }

        [Route("find")]
        [HttpPost]
        public IActionResult Find([FromBody] QueryRequestBody query)
        {
            BaseModels<ManagementElectricityActivitiesModel> model = new BaseModels<ManagementElectricityActivitiesModel>();
            string _keywordSearch = "";
            bool _orderBy_ASC = false;
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }

                Func<ManagementElectricityActivitiesModel, object> _orderByExpression = x => x.ProjectName;
                Dictionary<string, Func<ManagementElectricityActivitiesModel, object>> _sortableFields = new Dictionary<string, Func<ManagementElectricityActivitiesModel, object>>
                    {
                        { "ProjectName", x => x.ProjectName },
                        { "MaxWattage", x => x.MaxWattage },
                        { "Type", x => x.Type },
                        { "DistrictName", x => x.DistrictName },
                    };
                if (query.Sort != null && !string.IsNullOrEmpty(query.Sort.ColumnName) && _sortableFields.ContainsKey(query.Sort.ColumnName))
                {
                    _orderBy_ASC = ("desc".Equals(query.Sort.Direction.ToLower()) ? false : true);
                    _orderByExpression = _sortableFields[query.Sort.ColumnName];
                }

                IQueryable<ManagementElectricityActivitiesModel> _data = (from m in _repo._context.ManagementElectricityActivities
                                                                          where !m.IsDel
                                                                          join d in _repo._context.Districts
                                                                          on m.DistrictId equals d.DistrictId
                                                                          select new ManagementElectricityActivitiesModel
                                                                          {
                                                                              ManagementElectricityActivitiesId = m.ManagementElectricityActivitiesId,
                                                                              ProjectName = m.ProjectName,
                                                                              Type = m.Type,
                                                                              TypeName = m.Type == 1 ? "Trong khu công nghiệp" : m.Type == 2 ? "Kết hợp trại nông nghiệp" : "Khác",
                                                                              MaxWattage = m.MaxWattage,
                                                                              DistrictId = m.DistrictId,
                                                                              DistrictName = d.DistrictName,
                                                                          }).ToList().AsQueryable();

                if (query.SearchValue != null && query.SearchValue != "")
                {
                    _keywordSearch = query.SearchValue.Trim().ToLower();
                    _data = _data.Where(x => x.ProjectName.ToLower().Contains(_keywordSearch)
                            || x.TypeName.ToLower().Contains(_keywordSearch)
                            || x.MaxWattage.ToString().Contains(_keywordSearch)
                            || x.DistrictName.ToLower().Contains(_keywordSearch));
                }

                if (query.Filter != null && query.Filter.ContainsKey("Type") && !string.IsNullOrEmpty(query.Filter["Type"]))
                {
                    _data = _data.Where(x => x.Type.ToString() == query.Filter["Type"]);
                }

                if (query.Filter != null && query.Filter.ContainsKey("District") && !string.IsNullOrEmpty(query.Filter["District"]))
                {
                    _data = _data.Where(x => x.DistrictId.ToString() == query.Filter["District"]);
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
        public async Task<IActionResult> Create(ManagementElectricityActivitiesModel data)
        {
            BaseModels<ManagementElectricityActivity> model = new BaseModels<ManagementElectricityActivity>();
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                SystemLog datalog = new SystemLog();
                ManagementElectricityActivity SaveData = new ManagementElectricityActivity();
                SaveData.ProjectName = data.ProjectName;
                SaveData.Wattage = data.Wattage;
                SaveData.MaxWattage = data.MaxWattage;
                SaveData.Type = data.Type;
                SaveData.DistrictId = data.DistrictId;
                SaveData.DateOfAcceptance = data.DateOfAcceptance;
                SaveData.ConnectorAgreement = data.ConnectorAgreement;
                SaveData.PowerPurchaseAgreement = data.PowerPurchaseAgreement;
                SaveData.AnotherContent = data.AnotherContent;

                SaveData.CreateUserId = loginData.Userid;
                SaveData.CreateTime = DateTime.Now;

                await _repo.Insert(SaveData);
                datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.CREATE, NameCategory_Const.MANAGEMENT_ELECTRICITY_ACTIVITIES, Action_Status.SUCCESS);
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
            BaseModels<ManagementElectricityActivity> model = new BaseModels<ManagementElectricityActivity>();
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
        public async Task<IActionResult> Update(ManagementElectricityActivity data)
        {
            BaseModels<ManagementElectricityActivity> model = new BaseModels<ManagementElectricityActivity>();
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                SystemLog datalog = new SystemLog();
                ManagementElectricityActivity? SaveData = _repo._context.ManagementElectricityActivities.Where(x => x.ManagementElectricityActivitiesId == data.ManagementElectricityActivitiesId && !x.IsDel).FirstOrDefault();
                if (SaveData != null)
                {
                    SaveData.ProjectName = data.ProjectName;
                    SaveData.Wattage = data.Wattage;
                    SaveData.MaxWattage = data.MaxWattage;
                    SaveData.Type = data.Type;
                    SaveData.DistrictId = data.DistrictId;
                    SaveData.DateOfAcceptance = data.DateOfAcceptance;
                    SaveData.ConnectorAgreement = data.ConnectorAgreement;
                    SaveData.PowerPurchaseAgreement = data.PowerPurchaseAgreement;
                    SaveData.AnotherContent = data.AnotherContent;

                    SaveData.UpdateUserId = loginData.Userid;
                    SaveData.UpdateTime = DateTime.Now;

                    await _repo.Update(SaveData);
                    datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.UPDATE, NameCategory_Const.MANAGEMENT_ELECTRICITY_ACTIVITIES, Action_Status.SUCCESS);
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
                    datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.UPDATE, NameCategory_Const.MANAGEMENT_ELECTRICITY_ACTIVITIES, Action_Status.FAIL);
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
        public async Task<IActionResult> Delete(Guid id)
        {
            BaseModels<ManagementElectricityActivity> model = new BaseModels<ManagementElectricityActivity>();
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                SystemLog datalog = new SystemLog();
                ManagementElectricityActivity DeleteData = _repo._context.ManagementElectricityActivities.Where(x => x.ManagementElectricityActivitiesId == id && !x.IsDel).FirstOrDefault();
                if (DeleteData != null)
                {
                    DeleteData.IsDel = true;
                    await _repo.Delete(DeleteData);
                    datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.DELETE, NameCategory_Const.MANAGEMENT_ELECTRICITY_ACTIVITIES, Action_Status.SUCCESS);
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
                    datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.DELETE, NameCategory_Const.MANAGEMENT_ELECTRICITY_ACTIVITIES, Action_Status.FAIL);
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
        public IActionResult Export([FromBody] QueryRequestBody query)
        {
            var data = _repo.FindData(query);

            if (!data.Any())
            {
                return BadRequest();
            }

            try
            {
                using (var workbook = new XLWorkbook(@"Upload/Templates/Thongtinquanlyhoatdongdienluc.xlsx"))
                {
                    IXLWorksheet worksheet = workbook.Worksheets.Worksheet(1);

                    int index = 7;
                    int row = 1;

                    foreach (var item in data)
                    {
                        if (row == 1)
                        {
                            worksheet.Cell(index, 1).Value = row;
                            worksheet.Cell(index, 2).Value = item.ProjectName;
                            worksheet.Cell(index, 3).Value = item.MaxWattage;
                            worksheet.Cell(index, 4).Value = item.Wattage;
                            worksheet.Cell(index, 5).Value = item.Type == 1 ? 1 : "";
                            worksheet.Cell(index, 6).Value = item.Type == 2 ? 1 : "";
                            worksheet.Cell(index, 7).Value = item.Type == 3 ? 1 : "";
                            worksheet.Cell(index, 8).Value = item.DateOfAcceptance.ToString("HH ' giờ ' mm ' phút, ' ' ngày ' dd'/'MM'/'yyyy");
                            worksheet.Cell(index, 9).Value = item.ConnectorAgreement;
                            worksheet.Cell(index, 10).Value = item.PowerPurchaseAgreement;
                            worksheet.Cell(index, 11).Value = item.AnotherContent;
                            index++;
                            row++;
                        }
                        else
                        {
                            var addrow = worksheet.Row(index - 1);
                            addrow.InsertRowsBelow(1);
                            worksheet.Cell(index, 1).Value = row;
                            worksheet.Cell(index, 2).Value = item.ProjectName;
                            worksheet.Cell(index, 3).Value = item.MaxWattage;
                            worksheet.Cell(index, 4).Value = item.Wattage;
                            worksheet.Cell(index, 5).Value = item.Type == 1 ? 1 : "";
                            worksheet.Cell(index, 6).Value = item.Type == 2 ? 1 : "";
                            worksheet.Cell(index, 7).Value = item.Type == 3 ? 1 : "";
                            worksheet.Cell(index, 8).Value = item.DateOfAcceptance.ToString("HH ' giờ ' mm ' phút, ' ' ngày ' dd'/'MM'/'yyyy");
                            worksheet.Cell(index, 9).Value = item.ConnectorAgreement;
                            worksheet.Cell(index, 10).Value = item.PowerPurchaseAgreement;
                            worksheet.Cell(index, 11).Value = item.AnotherContent;
                            index++;
                            row++;
                        }
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
