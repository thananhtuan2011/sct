
using API_SoCongThuong.Classes;
using API_SoCongThuong.Logger;
using API_SoCongThuong.Models;
using API_SoCongThuong.Reponsitories.RooftopSolarProjectManagementRepository;
using ClosedXML.Excel;
using EF_Core.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.ComponentModel.Design;
using System.Data;
using System.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace API_SoCongThuong.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RooftopSolarProjectManagementController : ControllerBase
    {
        private RooftopSolarProjectManagementRepo _repo;
        private IConfiguration _configuration;
        private readonly ILogger<AsyncLogger> _logger;
        public SoHoa_SoCongThuongContext _context;
        private AsyncLogger _asyncLogger;
        public RooftopSolarProjectManagementController(SoHoa_SoCongThuongContext context, IConfiguration configuration, ILogger<AsyncLogger> logger)
        {
            _repo = new RooftopSolarProjectManagementRepo(context);
            _logger = logger;
            _context = context;
            _asyncLogger = new AsyncLogger(_logger, _context);
            _configuration = configuration;
        }

        [Route("find")]
        [HttpPost]
        public IActionResult ListItems_New([FromBody] QueryRequestBody query)//query truyền lên
        {

            BaseModels<RooftopSolarProjectManagementModel> model = new BaseModels<RooftopSolarProjectManagementModel>();
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

                Func<RooftopSolarProjectManagementModel, object> _orderByExpression = x => x.ProjectName; //Khởi tạo mặc định sắp xếp dữ liệu
                Dictionary<string, Func<RooftopSolarProjectManagementModel, object>> _sortableFields = new Dictionary<string, Func<RooftopSolarProjectManagementModel, object>>   //Khởi tạo các trường để sắp xếp
                    {
                        { "ProjectName", x => x.ProjectName },
                        { "InvestorName", x => x.InvestorName },
                        { "Address", x => x.Address },
                        { "Area", x => x.Area },
                        { "SurveyPolicy", x => x.SurveyPolicy },
                        { "Wattage", x => x.Wattage },
                        { "Progress", x => x.Progress },
                        {"OperationDay", x => x.OperationDay },
                        {"DistrictName", x => x.DistrictName }
                    };
                if (query.Sort != null
                    && !string.IsNullOrEmpty(query.Sort.ColumnName)
                    && _sortableFields.ContainsKey(query.Sort.ColumnName))
                {
                    _orderBy_ASC = ("desc".Equals(query.Sort.Direction.ToLower()) ? false : true);    //Sắp xếp asc hoặc desc
                    _orderByExpression = _sortableFields[query.Sort.ColumnName]; //Trường cần sắp xếp
                }

                //Cách 1 dùng entity
                IQueryable<RooftopSolarProjectManagementModel> _data = (from r in _repo._context.RooftopSolarProjectManagements
                                                                        join d in _repo._context.Districts on r.District equals d.DistrictId
                                                                        select new RooftopSolarProjectManagementModel
                                                                        {
                                                                            RooftopSolarProjectManagementId = r.RooftopSolarProjectManagementId,
                                                                            ProjectName = r.ProjectName,
                                                                            InvestorName = r.InvestorName,
                                                                            Address = r.Address,
                                                                            Area = r.Area,
                                                                            SurveyPolicy = r.SurveyPolicy,
                                                                            Wattage = r.Wattage,
                                                                            Progress = r.Progress,
                                                                            IsDel = r.IsDel,
                                                                            OperationDay = r.OperationDay,
                                                                            District = r.District,
                                                                            DistrictName = d.DistrictName,
                                                                        }).ToList().AsQueryable();
                _data = _data.Where(x => !x.IsDel);


                if (query.SearchValue != null && query.SearchValue != "") //Kiểm tra điều kiện tìm kiếm
                {
                    _keywordSearch = query.SearchValue.Trim().ToLower();
                    _data = _data.Where(x =>
                       /* x.DistrictId.ToString().ToLower().Contains(_keywordSearch)
                        || */
                       x.ProjectName.ToLower().Contains(_keywordSearch)
                       || x.InvestorName.ToLower().Contains(_keywordSearch)
                       || x.Address.ToLower().Contains(_keywordSearch)
                       || x.SurveyPolicy.ToLower().Contains(_keywordSearch)
                       //|| x.Area.ToString().Contains(_keywordSearch)
                       || x.Wattage.ToString().Contains(_keywordSearch)
                       || x.Progress.ToLower().Contains(_keywordSearch)
                   );  //Lấy table đã select tìm kiếm theo keyword
                }
                if (query.Filter != null && query.Filter.ContainsKey("District") && !string.IsNullOrEmpty(query.Filter["District"]))
                {
                    _data = _data.Where(x => x.District == Guid.Parse(query.Filter["District"]));
                }

                if (query.Filter != null && query.Filter.ContainsKey("Wattage") && !string.IsNullOrEmpty(query.Filter["Wattage"]))
                {
                    string wattage = query.Filter["Wattage"];
                    switch (wattage)
                    {
                        case "INSTALL_CAPACITY_01" :
                            _data = _data.Where(x => (1000 * x.Wattage) < 10);
                            break;
                        case "INSTALL_CAPACITY_02":
                            _data = _data.Where(x => (1000 * x.Wattage) >= 10 && (1000 * x.Wattage) < 100);
                            break;
                        case "INSTALL_CAPACITY_03":
                            _data = _data.Where(x => (1000 * x.Wattage) >= 100 && (1000 * x.Wattage) < 1000);
                            break;
                    };
                    
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
            BaseModels<RooftopSolarProjectManagement> model = new BaseModels<RooftopSolarProjectManagement>();
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
        public async Task<IActionResult> Update(RooftopSolarProjectManagement data)
        {
            BaseModels<RooftopSolarProjectManagement> model = new BaseModels<RooftopSolarProjectManagement>();
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                SystemLog datalog = new SystemLog();
                RooftopSolarProjectManagement? SaveData = _repo._context.RooftopSolarProjectManagements.Where(x => x.RooftopSolarProjectManagementId == data.RooftopSolarProjectManagementId && !x.IsDel).FirstOrDefault();
                if (SaveData != null)
                {
                    SaveData.RooftopSolarProjectManagementId = Guid.Parse(data.RooftopSolarProjectManagementId.ToString());
                    SaveData.ProjectName = data.ProjectName;
                    SaveData.InvestorName = data.InvestorName;
                    SaveData.Address = data.Address;
                    SaveData.Area = data.Area;
                    SaveData.SurveyPolicy = data.SurveyPolicy;
                    SaveData.Wattage = data.Wattage;
                    SaveData.Progress = data.Progress;
                    SaveData.District = data.District;
                    SaveData.OperationDay = data.OperationDay;

                    SaveData.UpdateUserId = loginData.Userid;
                    SaveData.UpdateTime = DateTime.Now;

                    await _repo.Update(SaveData);
                    datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.UPDATE, NameCategory_Const.ROOF_TOP_SOLAR_PROJECT_MANAGEMENT, Action_Status.SUCCESS);
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
                    datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.UPDATE, NameCategory_Const.ROOF_TOP_SOLAR_PROJECT_MANAGEMENT, Action_Status.FAIL);
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
        public async Task<IActionResult> create(RooftopSolarProjectManagement data)
        {
            BaseModels<RooftopSolarProjectManagement> model = new BaseModels<RooftopSolarProjectManagement>();
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                SystemLog datalog = new SystemLog();
                RooftopSolarProjectManagement SaveData = new RooftopSolarProjectManagement();
                SaveData.ProjectName = data.ProjectName;
                SaveData.InvestorName = data.InvestorName;
                SaveData.Address = data.Address;
                SaveData.Area = data.Area;
                SaveData.SurveyPolicy = data.SurveyPolicy;
                SaveData.Wattage = data.Wattage;
                SaveData.Progress = data.Progress;
                SaveData.District = data.District;
                SaveData.OperationDay = data.OperationDay;

                SaveData.CreateUserId = loginData.Userid;
                SaveData.CreateTime = DateTime.Now;

                await _repo.Insert(SaveData);
                datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.CREATE, NameCategory_Const.ROOF_TOP_SOLAR_PROJECT_MANAGEMENT, Action_Status.SUCCESS);
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
            BaseModels<RooftopSolarProjectManagement> model = new BaseModels<RooftopSolarProjectManagement>();
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                SystemLog datalog = new SystemLog();
                RooftopSolarProjectManagement DeleteData = new RooftopSolarProjectManagement();
                DeleteData.RooftopSolarProjectManagementId = id;
                DeleteData.IsDel = true;
                await _repo.DeleteRooftopSolarProjectManagement(DeleteData);
                datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.DELETE, NameCategory_Const.ROOF_TOP_SOLAR_PROJECT_MANAGEMENT, Action_Status.SUCCESS);
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

            try
            {
                using (var workbook = new XLWorkbook(@"Upload/Templates/Danhsachduandienmattroiapmai.xlsx"))
                {
                    IXLWorksheet worksheet = workbook.Worksheets.Worksheet(1);

                    int index = 4;
                    int row = 1;

                    string filter = "";
                    if (query.Filter != null && query.Filter.ContainsKey("District") && !string.IsNullOrEmpty(query.Filter["District"]))
                    {
                        filter += " " + _repo._context.Districts
                                    .Where(x => x.DistrictId.ToString().Equals(query.Filter["District"]))
                                    .Select(x => x.DistrictName)
                                    .FirstOrDefault();
                    }

                    if (query.Filter != null && query.Filter.ContainsKey("Wattage") && !string.IsNullOrEmpty(query.Filter["Wattage"]))
                    {
                        string wattage = query.Filter["Wattage"];
                        switch (wattage)
                        {
                            case "INSTALL_CAPACITY_01":
                                filter += " Dưới 10 kW";
                                break;
                            case "INSTALL_CAPACITY_02":
                                filter += " Từ 10 kW đến dưới 100 kW";
                                break;
                            case "INSTALL_CAPACITY_03":
                                filter += " Từ 100 kW đến dưới 1 MW";
                                break;
                        };
                    }

                    worksheet.Cell(1, 1).Value = ("Danh sách các dự án điện mặt trời áp mái" + filter).ToUpper();

                    foreach (var item in data)
                    {
                        if (row == 1)
                        {
                            worksheet.Cell(index, 1).Value = row;
                            worksheet.Cell(index, 2).Value = item.ProjectName;
                            worksheet.Cell(index, 3).Value = item.DistrictName;
                            worksheet.Cell(index, 4).Value = item.InvestorName;
                            worksheet.Cell(index, 5).Value = item.Address;
                            worksheet.Cell(index, 6).Value = item.Area;
                            worksheet.Cell(index, 7).Value = item.Wattage;
                            worksheet.Cell(index, 8).Value = item.SurveyPolicy;
                            worksheet.Cell(index, 9).Value = item.Progress;
                            worksheet.Cell(index, 10).Value = item.OperationDay != null ? item.OperationDay?.ToString("dd/MM/yyyy") : "";

                            index++;
                            row++;
                        }
                        else
                        {
                            var addrow = worksheet.Row(index - 1);
                            addrow.InsertRowsBelow(1);
                            worksheet.Cell(index, 1).Value = row;
                            worksheet.Cell(index, 2).Value = item.ProjectName;
                            worksheet.Cell(index, 3).Value = item.DistrictName;
                            worksheet.Cell(index, 4).Value = item.InvestorName;
                            worksheet.Cell(index, 5).Value = item.Address;
                            worksheet.Cell(index, 6).Value = item.Area;
                            worksheet.Cell(index, 7).Value = item.Wattage;
                            worksheet.Cell(index, 8).Value = item.SurveyPolicy;
                            worksheet.Cell(index, 9).Value = item.Progress;
                            worksheet.Cell(index, 10).Value = item.OperationDay != null ? item.OperationDay?.ToString("dd/MM/yyyy") : "";
                            index++;
                            row++;
                        }
                    }

                    var delrow = worksheet.Row(index);
                    delrow.Delete();

                    //worksheet.Row(index).Delete();

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
