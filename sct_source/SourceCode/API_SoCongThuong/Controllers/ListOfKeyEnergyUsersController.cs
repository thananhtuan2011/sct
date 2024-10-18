
using API_SoCongThuong.Classes;
using API_SoCongThuong.Models;
using API_SoCongThuong.Reponsitories.CountryRepository;
using EF_Core.Models;
using Microsoft.AspNetCore.Mvc;
using ClosedXML.Excel;
using API_SoCongThuong.Logger;
using Newtonsoft.Json;
using System.Data;

namespace API_SoCongThuong.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ListOfKeyEnergyUsersController : ControllerBase
    {
        private ListOfKeyEnergyUsersRepo _repo;
        private IConfiguration _configuration;
        private readonly ILogger<AsyncLogger> _logger;
        public SoHoa_SoCongThuongContext _context;
        private AsyncLogger _asyncLogger;
        public ListOfKeyEnergyUsersController(SoHoa_SoCongThuongContext context, IConfiguration configuration, ILogger<AsyncLogger> logger)
        {
            _repo = new ListOfKeyEnergyUsersRepo(context);
            _logger = logger;
            _context = context;
            _asyncLogger = new AsyncLogger(_logger, _context);
            _configuration = configuration;
        }

        [Route("find")]
        [HttpPost]
        public IActionResult Find([FromBody] QueryRequestBody query)
        {
            BaseModels<ListOfKeyEnergyUsersModel> model = new BaseModels<ListOfKeyEnergyUsersModel>();
            string _keywordSearch = "";
            bool _orderBy_ASC = true;
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }

                Func<ListOfKeyEnergyUsersModel, object> _orderByExpression = x => x.BusinessName;
                Dictionary<string, Func<ListOfKeyEnergyUsersModel, object>> _sortableFields = new Dictionary<string, Func<ListOfKeyEnergyUsersModel, object>>
                    {
                        { "BusinessName", x => x.BusinessName },
                        { "Address", x => x.Address },
                        { "Date", x => x.Date },
                        { "ProfessionName", x => x.ProfessionName },
                        { "EnergyConsumption", x => x.EnergyConsumption },
                        { "Decision", x => x.Decision },
                    };

                if (query.Sort != null && !string.IsNullOrEmpty(query.Sort.ColumnName) && _sortableFields.ContainsKey(query.Sort.ColumnName))
                {
                    _orderBy_ASC = ("desc".Equals(query.Sort.Direction.ToLower()) ? false : true);
                    _orderByExpression = _sortableFields[query.Sort.ColumnName];
                }

                IQueryable<ListOfKeyEnergyUsersModel> _data = (from l in _repo._context.ListOfKeyEnergyUsers
                                                               where !l.IsDel
                                                               join b in _repo._context.Businesses
                                                               on l.BusinessId equals b.BusinessId
                                                               join d in _repo._context.Districts
                                                               on b.DistrictId equals d.DistrictId
                                                               join t in _repo._context.TypeOfProfessions
                                                               on l.Profession equals t.TypeOfProfessionId
                                                               where !b.IsDel
                                                               select new ListOfKeyEnergyUsersModel
                                                               {
                                                                   ListOfKeyEnergyUsersId = l.ListOfKeyEnergyUsersId,
                                                                   BusinessName = b.BusinessNameVi.ToUpper(),
                                                                   Address = l.Address,
                                                                   Date = l.Date,
                                                                   ManufactProfession = l.ManufactProfession,
                                                                   EnergyConsumption = l.EnergyConsumption,
                                                                   DistrictName = d.DistrictName,
                                                                   ProfessionName = t.TypeOfProfessionName,
                                                                   Profession = l.Profession,
                                                                   District = d.DistrictId,
                                                                   // Date = l.Date.ToString("dd'/'MM'/'yyyy"),
                                                                   Decision = l.Decision ?? "",
                                                                   Note = l.Note ?? "",
                                                                   Link = l.Link ?? "",
                                                               }).ToList().AsQueryable();

                if (query.SearchValue != null && query.SearchValue != "")
                {
                    _keywordSearch = query.SearchValue.Trim().ToLower();
                    _data = _data.Where(x =>
                       x.BusinessName.ToLower().Contains(_keywordSearch)
                       || x.Address.ToLower().Contains(_keywordSearch)
                       || x.Date.ToString().Contains(_keywordSearch)
                       || x.ProfessionName.ToLower().Contains(_keywordSearch)
                       || x.EnergyConsumption.ToString().Contains(_keywordSearch)
                       || x.Decision.ToLower().Contains(_keywordSearch)
                   );
                }

                //if (query.Filter != null && query.Filter.ContainsKey("District") && !string.IsNullOrEmpty(query.Filter["District"]))
                //{
                //    _data = _data.Where(x => x.District == Guid.Parse(query.Filter["District"]));
                //}

                //if (query.Filter != null && query.Filter.ContainsKey("Profession") && !string.IsNullOrEmpty(query.Filter["Profession"]))
                //{
                //    _data = _data.Where(x => x.Profession == Guid.Parse(query.Filter["Profession"]));
                //}

                if (query.Filter != null && query.Filter.ContainsKey("Date") && !string.IsNullOrEmpty(query.Filter["Date"]))
                {
                    _data = _data.Where(x => x.Date.ToString() == query.Filter["Date"]);
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
        public async Task<IActionResult> Create(ListOfKeyEnergyUsersModel data)
        {
            BaseModels<ListOfKeyEnergyUser> model = new BaseModels<ListOfKeyEnergyUser>();
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                SystemLog datalog = new SystemLog();
                ListOfKeyEnergyUser SaveData = new ListOfKeyEnergyUser();
                SaveData.BusinessId = data.BusinessId;
                SaveData.Address = data.Address;
                SaveData.Date = data.Date;
                SaveData.Link = data.Link;
                SaveData.Profession = data.Profession;
                SaveData.ManufactProfession = data.ManufactProfession;
                SaveData.Note = data.Note;
                SaveData.EnergyConsumption = data.EnergyConsumption;
                SaveData.Decision = data.Decision;

                SaveData.CreateUserId = loginData.Userid;
                SaveData.CreateTime = DateTime.Now;

                await _repo.Insert(SaveData);
                datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.CREATE, NameCategory_Const.LIST_OF_KEY_ENERGY_USERS, Action_Status.SUCCESS);
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
            BaseModels<ListOfKeyEnergyUsersModel> model = new BaseModels<ListOfKeyEnergyUsersModel>();
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
        public async Task<IActionResult> Update(ListOfKeyEnergyUsersModel data)
        {
            BaseModels<ListOfKeyEnergyUsersModel> model = new BaseModels<ListOfKeyEnergyUsersModel>();
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                SystemLog datalog = new SystemLog();
                ListOfKeyEnergyUser? SaveData = _repo._context.ListOfKeyEnergyUsers.Where(x => x.ListOfKeyEnergyUsersId == data.ListOfKeyEnergyUsersId && !x.IsDel).FirstOrDefault();
                if (SaveData != null)
                {
                    SaveData.BusinessId = data.BusinessId;
                    SaveData.Address = data.Address;
                    SaveData.Date = data.Date;
                    SaveData.Link = data.Link;
                    SaveData.EnergyConsumption = data.EnergyConsumption;
                    SaveData.Profession = data.Profession;
                    SaveData.ManufactProfession = data.ManufactProfession;
                    SaveData.Note = data.Note;
                    SaveData.Decision = data.Decision;

                    SaveData.UpdateUserId = loginData.Userid;
                    SaveData.UpdateTime = DateTime.Now;

                    await _repo.Update(SaveData);
                    datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.UPDATE, NameCategory_Const.LIST_OF_KEY_ENERGY_USERS, Action_Status.SUCCESS);
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
                    datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.UPDATE, NameCategory_Const.LIST_OF_KEY_ENERGY_USERS, Action_Status.FAIL);
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
            BaseModels<ListOfKeyEnergyUser> model = new BaseModels<ListOfKeyEnergyUser>();
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                SystemLog datalog = new SystemLog();
                ListOfKeyEnergyUser? DeleteData = _repo._context.ListOfKeyEnergyUsers.Where(x => !x.IsDel && x.ListOfKeyEnergyUsersId == id).FirstOrDefault();

                if (DeleteData != null)
                {
                    DeleteData.IsDel = true;

                    await _repo.Delete(DeleteData);
                    datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.DELETE, NameCategory_Const.LIST_OF_KEY_ENERGY_USERS, Action_Status.SUCCESS);
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
                    datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.DELETE, NameCategory_Const.LIST_OF_KEY_ENERGY_USERS, Action_Status.FAIL);
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
                IQueryable<ListOfKeyEnergyUsersModel> _data = (from l in _repo._context.ListOfKeyEnergyUsers
                                                               where !l.IsDel
                                                               join b in _repo._context.Businesses
                                                               on l.BusinessId equals b.BusinessId
                                                               join d in _repo._context.Districts
                                                               on b.DistrictId equals d.DistrictId
                                                               join t in _repo._context.TypeOfProfessions
                                                               on l.Profession equals t.TypeOfProfessionId
                                                               where !b.IsDel
                                                               select new ListOfKeyEnergyUsersModel
                                                               {
                                                                   ListOfKeyEnergyUsersId = l.ListOfKeyEnergyUsersId,
                                                                   BusinessName = b.BusinessNameVi.ToUpper(),
                                                                   Address = l.Address,
                                                                   Date = l.Date,
                                                                   ManufactProfession = l.ManufactProfession,
                                                                   EnergyConsumption = l.EnergyConsumption,
                                                                   DistrictName = d.DistrictName,
                                                                   ProfessionName = t.TypeOfProfessionName,
                                                                   Profession = l.Profession,
                                                                   District = d.DistrictId,
                                                                   Decision = l.Decision ?? "",
                                                                   Note = l.Note ?? "",
                                                                   Link = l.Link ?? "",
                                                               }).ToList().AsQueryable();

                string Title = "DANH SÁCH CƠ SỞ SỬ DỤNG NĂNG LƯỢNG TRỌNG ĐIỂM ";
                string date = "";
                string _keywordSearch = "";

                if (query.SearchValue != null && query.SearchValue != "")
                {
                    _keywordSearch = query.SearchValue.Trim().ToLower();
                    _data = _data.Where(x =>
                       x.BusinessName.ToLower().Contains(_keywordSearch)
                       || x.Address.ToLower().Contains(_keywordSearch)
                       || x.Date.ToString().Contains(_keywordSearch)
                       || x.ProfessionName.ToLower().Contains(_keywordSearch)
                       || x.EnergyConsumption.ToString().Contains(_keywordSearch)
                       || x.Decision.ToLower().Contains(_keywordSearch)
                   );
                }

                //if (query.Filter != null && query.Filter.ContainsKey("District") && !string.IsNullOrEmpty(query.Filter["District"]))
                //{
                //    _data = _data.Where(x => x.District == Guid.Parse(query.Filter["District"]));
                //}

                //if (query.Filter != null && query.Filter.ContainsKey("Profession") && !string.IsNullOrEmpty(query.Filter["Profession"]))
                //{
                //    _data = _data.Where(x => x.Profession == Guid.Parse(query.Filter["Profession"]));
                //}

                if (query.Filter != null && query.Filter.ContainsKey("Date") && !string.IsNullOrEmpty(query.Filter["Date"]))
                {
                    date = $"Năm {query.Filter["Date"]}";
                    _data = _data.Where(x => x.Date.ToString() == query.Filter["Date"]);
                }

                if (!_data.Any())
                {
                    return BadRequest();
                }

                using (var workbook = new XLWorkbook(@"Upload/Templates/Danhsachcososudungnangluongtrongdiem.xlsx"))
                {
                    IXLWorksheet worksheet = workbook.Worksheets.Worksheet(1);
                    worksheet.Cell(1, 1).Value = (Title + date ).ToUpper();
                    int index = 4  ;
                    foreach (var item in _data)
                    {
                        worksheet.Cell(index, 1).Value = index - 3;
                        worksheet.Cell(index, 2).Value = item.BusinessName.ToUpper();
                        worksheet.Cell(index, 3).Value = item.Address;
                        worksheet.Cell(index, 4).Value = item.ProfessionName;
                        worksheet.Cell(index, 5).Value = item.ManufactProfession;
                        worksheet.Cell(index, 6).Value = item.EnergyConsumption;
                        worksheet.Cell(index, 7).Value = item.Note;


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
