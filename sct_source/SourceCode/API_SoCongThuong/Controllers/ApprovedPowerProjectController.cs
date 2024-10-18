using API_SoCongThuong.Classes;
using API_SoCongThuong.Logger;
using API_SoCongThuong.Models;
using API_SoCongThuong.Reponsitories.ApprovedPowerProjectRepository;
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
    public class ApprovedPowerProjectController : ControllerBase
    {
        private ApprovedPowerProjectRepo _repo;
        private IConfiguration _configuration;
        private readonly ILogger<AsyncLogger> _logger;
        public SoHoa_SoCongThuongContext _context;
        private AsyncLogger _asyncLogger;
        public ApprovedPowerProjectController(SoHoa_SoCongThuongContext context, IConfiguration configuration, ILogger<AsyncLogger> logger)
        {
            _repo = new ApprovedPowerProjectRepo(context);
            _logger = logger;
            _context = context;
            _asyncLogger = new AsyncLogger(_logger, _context);
            _configuration = configuration;
        }

        [Route("find")]
        [HttpPost]
        public IActionResult ListItems_New([FromBody] QueryRequestBody query)//query truyền lên
        {

            BaseModels<ApprovedPowerProjectModel> model = new BaseModels<ApprovedPowerProjectModel>();
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

                Func<ApprovedPowerProjectModel, object> _orderByExpression = x => x.ProjectName; //Khởi tạo mặc định sắp xếp dữ liệu
                Dictionary<string, Func<ApprovedPowerProjectModel, object>> _sortableFields = new Dictionary<string, Func<ApprovedPowerProjectModel, object>>   //Khởi tạo các trường để sắp xếp
                    {
                        { "ProjectName", x => x.ProjectName },
                        { "InvestorName", x => x.InvestorName },
                        { "Address", x => x.Address },
                        { "EnergyIndustryName", x => x.EnergyIndustryName! },
                        { "Wattage", x => x.Wattage },
                        { "Turbines", x => x.Turbines },
                        { "Substation", x => x.Substation },
                        { "PowerOutput", x => x.PowerOutput! },
                        { "StatusName", x => x.StatusName! },
                    };
                if (query.Sort != null
                    && !string.IsNullOrEmpty(query.Sort.ColumnName)
                    && _sortableFields.ContainsKey(query.Sort.ColumnName))
                {
                    _orderBy_ASC = ("desc".Equals(query.Sort.Direction.ToLower()) ? false : true);    //Sắp xếp asc hoặc desc
                    _orderByExpression = _sortableFields[query.Sort.ColumnName]; //Trường cần sắp xếp
                }

                //Cách 1 dùng entity
                IQueryable<ApprovedPowerProjectModel> _data = (from app in _repo._context.ApprovedPowerProjects
                                                               where !app.IsDel
                                                               join c in _repo._context.Categories
                                                               on app.Status equals c.CategoryId
                                                               join ei in _repo._context.TypeOfEnergies
                                                               on app.EnergyIndustryId equals ei.TypeOfEnergyId into JoinEi
                                                               from eni in JoinEi.DefaultIfEmpty()
                                                               select new ApprovedPowerProjectModel
                                                               {
                                                                   ApprovedPowerProjectId = app.ApprovedPowerProjectId,
                                                                   EnergyIndustryId = app.EnergyIndustryId,
                                                                   EnergyIndustryName = eni.TypeOfEnergyName,
                                                                   ProjectName = app.ProjectName,
                                                                   InvestorName = app.InvestorName,
                                                                   Address = app.Address ?? "",
                                                                   PolicyDecision = app.PolicyDecision,
                                                                   Wattage = app.Wattage,
                                                                   Turbines = app.Turbines,
                                                                   Substation = app.Substation,
                                                                   PowerOutput = app.PowerOutput ?? 0,
                                                                   Area = app.Area,
                                                                   Year = app.Year,
                                                                   Status = app.Status,
                                                                   StatusName = c.CategoryName,
                                                                   Note = app.Note,
                                                                   IsDel = app.IsDel
                                                               }).ToList().AsQueryable();

                if (query.SearchValue != null && query.SearchValue != "")
                {
                    _keywordSearch = query.SearchValue.Trim().ToLower();
                    _data = _data.Where(x =>
                       x.ProjectName.ToLower().Contains(_keywordSearch)
                       || x.InvestorName.ToLower().Contains(_keywordSearch)
                       || x.Address.ToLower().Contains(_keywordSearch)
                       || x.Wattage.ToString().Contains(_keywordSearch)
                       || x.Turbines.ToString().Contains(_keywordSearch)
                       || x.Substation.ToString().Contains(_keywordSearch)
                       || x.PowerOutput.ToString().Contains(_keywordSearch)
                   );
                }

                if (query.Filter != null && query.Filter.ContainsKey("EnergyIndustryId") && !string.IsNullOrEmpty(query.Filter["EnergyIndustryId"]))
                {
                    _data = _data.Where(x => x.EnergyIndustryId.ToString().Equals(string.Join("", query.Filter["EnergyIndustryId"])));
                }

                if (query.Filter != null && query.Filter.ContainsKey("StatusId") && !string.IsNullOrEmpty(query.Filter["StatusId"]))
                {
                    _data = _data.Where(x => x.Status.ToString().Equals(string.Join("", query.Filter["StatusId"])));
                }

                if (query.Filter != null && query.Filter.ContainsKey("Year") && !string.IsNullOrEmpty(query.Filter["Year"]))
                {
                    _data = _data.Where(x => x.Year.ToString().Equals(string.Join("", query.Filter["Year"])));
                }

                if (query.Filter != null && query.Filter.ContainsKey("PowerOutput") && !string.IsNullOrEmpty(query.Filter["PowerOutput"]))
                {
                    int PowerOutput;
                    if (int.TryParse(query.Filter["PowerOutput"], out PowerOutput)) {
                        _data = _data.Where(x => x.PowerOutput == PowerOutput);
                    }
                }

                int _countRows = _data.Count();
                if (_countRows == 0)
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
        public IActionResult GetItemById(Guid id)
        {
            BaseModels<ApprovedPowerProject> model = new BaseModels<ApprovedPowerProject>();
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
        public async Task<IActionResult> Update(ApprovedPowerProject data)
        {
            BaseModels<ApprovedPowerProject> model = new BaseModels<ApprovedPowerProject>();
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
                ApprovedPowerProject? SaveData = _repo._context.ApprovedPowerProjects.Where(x => x.ApprovedPowerProjectId == data.ApprovedPowerProjectId && !x.IsDel).FirstOrDefault();
                if (SaveData != null)
                {
                    SaveData.ApprovedPowerProjectId = data.ApprovedPowerProjectId;
                    SaveData.EnergyIndustryId = data.EnergyIndustryId;
                    SaveData.ProjectName = data.ProjectName;
                    SaveData.InvestorName = data.InvestorName;
                    SaveData.DistrictId = data.DistrictId;
                    SaveData.Address = data.Address;
                    SaveData.PolicyDecision = data.PolicyDecision;
                    SaveData.Wattage = data.Wattage;
                    SaveData.Turbines = data.Turbines;
                    SaveData.Substation = data.Substation;
                    SaveData.PowerOutput = data.PowerOutput ?? 0;
                    SaveData.Area = data.Area;
                    SaveData.Year = data.Year;
                    SaveData.Status = data.Status;
                    SaveData.Note = data.Note;

                    SaveData.UpdateUserId = loginData.Userid;
                    SaveData.UpdateTime = DateTime.Now;

                    await _repo.Update(SaveData);
                    datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.UPDATE, NameCategory_Const.APPROVED_POWER_PROJECT, Action_Status.SUCCESS);
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
                    datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.UPDATE, NameCategory_Const.APPROVED_POWER_PROJECT, Action_Status.FAIL);
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
        public async Task<IActionResult> Create(ApprovedPowerProject data)
        {
            BaseModels<ApprovedPowerProject> model = new BaseModels<ApprovedPowerProject>();
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

                ApprovedPowerProject SaveData = new ApprovedPowerProject();
                SaveData.EnergyIndustryId = data.EnergyIndustryId;
                SaveData.ProjectName = data.ProjectName;
                SaveData.InvestorName = data.InvestorName;
                SaveData.DistrictId = data.DistrictId;
                SaveData.Address = data.Address;
                SaveData.PolicyDecision = data.PolicyDecision;
                SaveData.Wattage = data.Wattage;
                SaveData.Turbines = data.Turbines;
                SaveData.Substation = data.Substation;
                SaveData.PowerOutput = data.PowerOutput ?? 0;
                SaveData.Area = data.Area;
                SaveData.Year = data.Year;
                SaveData.Status = data.Status;
                SaveData.Note = data.Note;
                SaveData.CreateUserId = loginData.Userid;
                SaveData.CreateTime = DateTime.Now;

                await _repo.Insert(SaveData);
                datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.CREATE, NameCategory_Const.APPROVED_POWER_PROJECT, Action_Status.SUCCESS);
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
        public async Task<IActionResult> Delete(Guid id)
        {
            BaseModels<ApprovedPowerProject> model = new BaseModels<ApprovedPowerProject>();
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                SystemLog datalog = new SystemLog();
                ApprovedPowerProject DeleteData = new ApprovedPowerProject();
                DeleteData.ApprovedPowerProjectId = id;
                DeleteData.IsDel = true;
                await _repo.DeleteApprovedPowerProject(DeleteData);
                datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.DELETE, NameCategory_Const.APPROVED_POWER_PROJECT, Action_Status.SUCCESS);
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

            string FilterTitle = "";
            if (query.Filter != null && query.Filter.ContainsKey("EnergyIndustryId") && !string.IsNullOrEmpty(query.Filter["EnergyIndustryId"]))
            {
                string name = data.Where(x => x.EnergyIndustryId.ToString() == query.Filter["EnergyIndustryId"]).Select(x => x.EnergyIndustryName).FirstOrDefault() ?? "";
                FilterTitle += " LĨNH VỰC " + name.ToUpper();
            }

            if (query.Filter != null && query.Filter.ContainsKey("StatusId") && !string.IsNullOrEmpty(query.Filter["StatusId"]))
            {
                string name = data.Where(x => x.Status.ToString() == query.Filter["StatusId"]).Select(x => x.StatusName).FirstOrDefault() ?? "";
                FilterTitle += " " + name.ToUpper();
            }

            if (query.Filter != null && query.Filter.ContainsKey("Year") && !string.IsNullOrEmpty(query.Filter["Year"]))
            {
                FilterTitle += " NĂM " + query.Filter["Year"];
            }

            try
            {
                using (var workbook = new XLWorkbook(@"Upload/Templates/Danhsachduandienduocpheduyet.xlsx"))
                {
                    IXLWorksheet worksheet = workbook.Worksheets.Worksheet(1);

                    int index = 5;
                    int row = 1;

                    worksheet.Cell(1, 1).Value = "DANH MỤC CÁC DỰ ÁN ĐƯỢC PHÊ DUYỆT" + FilterTitle;

                    foreach (var item in data)
                    {
                        if (row == 1)
                        {
                            worksheet.Cell(index, 1).Value = row;
                            worksheet.Cell(index, 2).Value = item.ProjectName;
                            worksheet.Cell(index, 3).Value = item.InvestorName;
                            worksheet.Cell(index, 4).Value = item.Address;
                            worksheet.Cell(index, 5).Value = item.EnergyIndustryName;
                            worksheet.Cell(index, 6).Value = item.Wattage;
                            worksheet.Cell(index, 7).Value = item.Turbines;
                            worksheet.Cell(index, 8).Value = item.Substation;
                            worksheet.Cell(index, 8).Value = item.StatusName;
                            worksheet.Cell(index, 10).Value = item.PowerOutput != null ? item.PowerOutput : "-";
                            worksheet.Cell(index, 11).Value = item.Year;
                            index++;
                            row++;
                        }
                        else
                        {
                            var addrow = worksheet.Row(index - 1);
                            addrow.InsertRowsBelow(1);
                            worksheet.Cell(index, 1).Value = row;
                            worksheet.Cell(index, 2).Value = item.ProjectName;
                            worksheet.Cell(index, 3).Value = item.InvestorName;
                            worksheet.Cell(index, 4).Value = item.Address;
                            worksheet.Cell(index, 5).Value = item.EnergyIndustryName;
                            worksheet.Cell(index, 6).Value = item.Wattage;
                            worksheet.Cell(index, 7).Value = item.Turbines;
                            worksheet.Cell(index, 8).Value = item.Substation;
                            worksheet.Cell(index, 8).Value = item.StatusName;
                            worksheet.Cell(index, 10).Value = item.PowerOutput != null ? item.PowerOutput : "-";
                            worksheet.Cell(index, 11).Value = item.Year;
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
