
using API_SoCongThuong.Classes;
using API_SoCongThuong.Logger;
using API_SoCongThuong.Models;
using API_SoCongThuong.Reponsitories.ProposedPowerProjectRepository;
using ClosedXML.Excel;
using EF_Core.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.ComponentModel.Design;
using System.Data;
using System.Globalization;
using static System.Net.Mime.MediaTypeNames;

namespace API_SoCongThuong.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProposedPowerProjectController : ControllerBase
    {
        private ProposedPowerProjectRepo _repo;
        private IConfiguration _configuration;
        private readonly ILogger<AsyncLogger> _logger;
        public SoHoa_SoCongThuongContext _context;
        private AsyncLogger _asyncLogger;
        public ProposedPowerProjectController(SoHoa_SoCongThuongContext context, IConfiguration configuration, ILogger<AsyncLogger> logger)
        {
            _repo = new ProposedPowerProjectRepo(context);
            _logger = logger;
            _context = context;
            _asyncLogger = new AsyncLogger(_logger, _context);
            _configuration = configuration;
        }

        [Route("find")]
        [HttpPost]
        public IActionResult ListItems_New([FromBody] QueryRequestBody query)//query truyền lên
        {

            BaseModels<ProposedPowerProjectModel> model = new BaseModels<ProposedPowerProjectModel>();
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

                Func<ProposedPowerProjectModel, object> _orderByExpression = x => x.ProjectName; //Khởi tạo mặc định sắp xếp dữ liệu
                Dictionary<string, Func<ProposedPowerProjectModel, object>> _sortableFields = new Dictionary<string, Func<ProposedPowerProjectModel, object>>   //Khởi tạo các trường để sắp xếp
                    {
                        { "ProjectName", x => x.ProjectName },
                        { "InvestorName", x => x.InvestorName },
                        { "EnergyIndustryName", x => x.EnergyIndustryName },
                        { "StatusName", x => x.StatusName },
                        { "PolicyDecision", x => x.PolicyDecision },
                        { "Wattage", x => x.Wattage },
                        { "Address", x => x.Address },
                    };
                if (query.Sort != null && !string.IsNullOrEmpty(query.Sort.ColumnName) && _sortableFields.ContainsKey(query.Sort.ColumnName))
                {
                    _orderBy_ASC = ("desc".Equals(query.Sort.Direction.ToLower()) ? false : true);    //Sắp xếp asc hoặc desc
                    _orderByExpression = _sortableFields[query.Sort.ColumnName]; //Trường cần sắp xếp
                }

                //Cách 1 dùng entity
                IQueryable<ProposedPowerProjectModel> _data = (from app in _context.ProposedPowerProjects
                                                               where !app.IsDel
                                                               join ei in _context.TypeOfEnergies
                                                               on app.EnergyIndustryId equals ei.TypeOfEnergyId into JoinEi
                                                               from eni in JoinEi.DefaultIfEmpty()
                                                               join c in _context.Categories
                                                               on app.StatusId equals c.CategoryId into JoinCa
                                                               from ca in JoinCa.DefaultIfEmpty()
                                                               select new ProposedPowerProjectModel
                                                               {
                                                                   ProposedPowerProjectId = app.ProposedPowerProjectId,
                                                                   EnergyIndustryId = eni.TypeOfEnergyId,
                                                                   EnergyIndustryName = eni.TypeOfEnergyName,
                                                                   ProjectName = app.ProjectName,
                                                                   StatusId = app.StatusId,
                                                                   StatusName = ca.CategoryName,
                                                                   InvestorName = app.InvestorName,
                                                                   Address = app.Address,
                                                                   Wattage = app.Wattage,
                                                                   PolicyDecision = app.PolicyDecision,
                                                                   Note = app.Note,
                                                               }).ToList().AsQueryable();

                if (query.SearchValue != null && query.SearchValue != "")
                {
                    _keywordSearch = query.SearchValue.Trim().ToLower();
                    _data = _data.Where(x =>
                       x.ProjectName.ToLower().Contains(_keywordSearch)
                       || x.InvestorName.ToLower().Contains(_keywordSearch)
                       || x.EnergyIndustryName.ToLower().Contains(_keywordSearch)
                       || x.Address.ToLower().Contains(_keywordSearch)
                       || x.Wattage.ToString().Contains(_keywordSearch)
                       || x.PolicyDecision.ToLower().Contains(_keywordSearch)
                       || x.StatusName.ToLower().Contains(_keywordSearch)
                   );
                }

                if (query.Filter != null && query.Filter.ContainsKey("EnergyIndustryId") && !string.IsNullOrEmpty(query.Filter["EnergyIndustryId"]))
                {
                    _data = _data.Where(x => x.EnergyIndustryId.ToString().Equals(string.Join("", query.Filter["EnergyIndustryId"])));
                }

                if (query.Filter != null && query.Filter.ContainsKey("StatusId") && !string.IsNullOrEmpty(query.Filter["StatusId"]))
                {
                    _data = _data.Where(x => x.StatusId.ToString().Equals(string.Join("", query.Filter["StatusId"])));
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
        public IActionResult GetItemById(Guid id)
        {
            BaseModels<ProposedPowerProjectModel> model = new BaseModels<ProposedPowerProjectModel>();
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
        public async Task<IActionResult> Update(ProposedPowerProjectModel data)
        {
            BaseModels<ProposedPowerProject> model = new BaseModels<ProposedPowerProject>();
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
                ProposedPowerProject? SaveData = _repo._context.ProposedPowerProjects.Where(x => x.ProposedPowerProjectId == data.ProposedPowerProjectId && !x.IsDel).FirstOrDefault();
                if (SaveData != null)
                {
                    SaveData.ProposedPowerProjectId = data.ProposedPowerProjectId;
                    SaveData.EnergyIndustryId = data.EnergyIndustryId;
                    SaveData.ProjectName = data.ProjectName;
                    SaveData.StatusId = data.StatusId;
                    SaveData.InvestorName = data.InvestorName;
                    SaveData.PolicyDecision = data.PolicyDecision;
                    SaveData.Wattage = data.Wattage;
                    SaveData.Address = data.Address;
                    SaveData.Note = data.Note;
                    //SaveData.ProposedDate = DateTime.ParseExact(data.ProposedDate, "dd'/'MM'/'yyyy", CultureInfo.InvariantCulture);
                    SaveData.UpdateUserId = loginData.Userid;
                    SaveData.UpdateTime = DateTime.Now;

                    await _repo.Update(SaveData);
                    datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.UPDATE, NameCategory_Const.PROPOSED_POWER_PROJECT, Action_Status.SUCCESS);
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
                    datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.UPDATE, NameCategory_Const.PROPOSED_POWER_PROJECT, Action_Status.FAIL);
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
        public async Task<IActionResult> Create(ProposedPowerProjectModel data)
        {
            BaseModels<ProposedPowerProject> model = new BaseModels<ProposedPowerProject>();
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

                ProposedPowerProject SaveData = new ProposedPowerProject();
                SaveData.EnergyIndustryId = data.EnergyIndustryId;
                SaveData.ProjectName = data.ProjectName;
                SaveData.StatusId = data.StatusId;
                SaveData.InvestorName = data.InvestorName;
                SaveData.PolicyDecision = data.PolicyDecision;
                SaveData.Wattage = data.Wattage;
                SaveData.Address = data.Address;
                SaveData.Note = data.Note;
                //SaveData.ProposedDate = DateTime.ParseExact(data.ProposedDate, "dd'/'MM'/'yyyy", CultureInfo.InvariantCulture);
                SaveData.CreateUserId = loginData.Userid;
                SaveData.CreateTime = DateTime.Now;

                await _repo.Insert(SaveData);
                datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.CREATE, NameCategory_Const.PROPOSED_POWER_PROJECT, Action_Status.SUCCESS);
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
            BaseModels<ProposedPowerProject> model = new BaseModels<ProposedPowerProject>();
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                SystemLog datalog = new SystemLog();
                ProposedPowerProject DeleteData = new ProposedPowerProject();
                DeleteData.ProposedPowerProjectId = id;
                DeleteData.IsDel = true;
                await _repo.DeleteProposedPowerProject(DeleteData);
                datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.DELETE, NameCategory_Const.PROPOSED_POWER_PROJECT, Action_Status.SUCCESS);
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
            var data = _repo.FindData(query).GroupBy(x => x.StatusId).ToList();

            if (!data.Any())
            {
                return BadRequest();
            }

            try
            {
                using (var workbook = new XLWorkbook(@"Upload/Templates/Danhsachcacduannguondiendangdexuat.xlsx"))
                {
                    var last = data.Last();
                    int currentSheet = 0;
                    foreach (var group in data)
                    {
                        IXLWorksheet worksheet = workbook.Worksheets.Worksheet(currentSheet + 1);

                        string name = data[currentSheet].Select(x => x.StatusName).FirstOrDefault() ?? "";
                        if (!group.Equals(last))
                        {
                            worksheet.Name = name;
                            currentSheet++;
                            worksheet.CopyTo(workbook, data[currentSheet].Select(x => x.StatusName).FirstOrDefault());
                        }

                        string filter = "";
                        if (query.Filter != null && query.Filter.ContainsKey("EnergyIndustryId") && !string.IsNullOrEmpty(query.Filter["EnergyIndustryId"]))
                        {
                            filter += _repo._context.TypeOfEnergies
                                        .Where(x => x.TypeOfEnergyId.ToString().Equals(query.Filter["EnergyIndustryId"]))
                                        .Select(x => x.TypeOfEnergyName)
                                        .FirstOrDefault() + " ";
                        }

                        worksheet.Cell(1, 1).Value = ("Danh sách các dự án nguồn điện " + filter + name).ToUpper();

                        int index = 4;
                        int row = 1;


                        foreach (var item in group)
                            if (row == 1)
                            {
                                worksheet.Cell(index, 1).Value = row;
                                worksheet.Cell(index, 2).Value = item.InvestorName;
                                worksheet.Cell(index, 3).Value = item.EnergyIndustryName;
                                worksheet.Cell(index, 4).Value = item.ProjectName;
                                worksheet.Cell(index, 5).Value = item.Address;
                                worksheet.Cell(index, 6).Value = item.Wattage;
                                worksheet.Cell(index, 7).Value = item.PolicyDecision;
                                worksheet.Cell(index, 8).Value = item.StatusName;
                                index++;
                                row++;
                            }
                            else
                            {
                                var addrow = worksheet.Row(index - 1);
                                addrow.InsertRowsBelow(1);
                                worksheet.Cell(index, 1).Value = row;
                                worksheet.Cell(index, 2).Value = item.InvestorName;
                                worksheet.Cell(index, 3).Value = item.EnergyIndustryName;
                                worksheet.Cell(index, 4).Value = item.ProjectName;
                                worksheet.Cell(index, 5).Value = item.Address;
                                worksheet.Cell(index, 6).Value = item.Wattage;
                                worksheet.Cell(index, 7).Value = item.PolicyDecision;
                                worksheet.Cell(index, 8).Value = item.StatusName;
                                index++;
                                row++;
                            }

                        worksheet.Row(index).Delete();
                        worksheet.SetTabActive().SetTabSelected();
                        // worksheet.Name = "";
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
