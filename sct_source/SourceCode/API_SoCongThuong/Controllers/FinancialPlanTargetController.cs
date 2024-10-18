using API_SoCongThuong.Classes;
using API_SoCongThuong.Models;
using API_SoCongThuong.Reponsitories.FinancialPlanTargetRepository;
using API_SoCongThuong.Reponsitories;
using EF_Core.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.Design;
using static System.Net.Mime.MediaTypeNames;
using ClosedXML.Excel;
using System.Net;
using API_SoCongThuong.Logger;
using Newtonsoft.Json;

namespace API_SoCongThuong.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FinancialPlanTargetController : ControllerBase
    {
        private FinancialPlanTargetRepo _repo;
        private IConfiguration _configuration;
        private readonly ILogger<AsyncLogger> _logger;
        public SoHoa_SoCongThuongContext _context;
        private AsyncLogger _asyncLogger;
        public FinancialPlanTargetController(SoHoa_SoCongThuongContext context, IConfiguration configuration, ILogger<AsyncLogger> logger)
        {
            _repo = new FinancialPlanTargetRepo(context);

            _logger = logger;
            _context = context;
            _asyncLogger = new AsyncLogger(_logger, _context);
            _configuration = configuration;
        }

        [Route("find")]
        [HttpPost]
        public IActionResult Find([FromBody] QueryRequestBody query)
        {
            BaseModels<FinancialPlanTargetModel> model = new BaseModels<FinancialPlanTargetModel>();
            string _keywordSearch = "";
            bool _orderBy_ASC = true;
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }

                Func<FinancialPlanTargetModel, object> _orderByExpression = x => x.Name;

                Dictionary<string, Func<FinancialPlanTargetModel, object>> _sortableFields = new Dictionary<string, Func<FinancialPlanTargetModel, object>>
                    {
                        { "Name", x => x.Name },
                        { "Year", x => x.Year },
                        { "Unit", x => x.Unit},
                        { "CompareLastMonth", x => x.CompareLastMonth},
                        { "ComparedSameMonth", x => x.ComparedSameMonth},
                        { "AccumulatedComparedYearPlan", x => x.AccumulatedComparedYearPlan},
                        { "AccumulatedComparedPeriod", x => x.AccumulatedComparedPeriod},
                        {"Plan", x=> x.Plan },
                    {"ValueSameMonthLastYear", x => x.ValueSameMonthLastYear },
                    {"ValueLastMonth", x => x.ValueLastMonth },
                    {"EstimatedMonth", x => x.EstimatedMonth },
                    {"CumulativeToMonth", x => x.CumulativeToMonth },
                    {"CumulativeToMonthLastYear", x => x.CumulativeToMonthLastYear },

                    };

                if (query.Sort != null && !string.IsNullOrEmpty(query.Sort.ColumnName) && _sortableFields.ContainsKey(query.Sort.ColumnName))
                {
                    _orderBy_ASC = ("desc".Equals(query.Sort.Direction.ToLower()) ? false : true);
                    _orderByExpression = _sortableFields[query.Sort.ColumnName];
                }

                var MaxByMonthYear = _repo._context.FinancialPlanTargets.Where(x => !x.IsDel).OrderByDescending(x => x.Year).ThenByDescending(x => x.Month).FirstOrDefault();
                int MaxMonth;
                int MaxYear;
                if (MaxByMonthYear != null)
                {
                    MaxMonth = MaxByMonthYear.Month;
                    MaxYear = MaxByMonthYear.Year;
                }
                else
                {
                    return NotFound("Không có dữ liệu");
                }

                if (query.Filter != null && query.Filter.ContainsKey("Date") && !string.IsNullOrEmpty(query.Filter["Date"]))
                {
                    var date = query.Filter["Date"].Split("-");
                    MaxMonth = int.Parse(date[1]);
                    MaxYear = int.Parse(date[0]);
                }

                int LastMonth = MaxMonth - 1 == 0 ? 12 : MaxMonth - 1;
                int LastYear = MaxMonth - 1 == 0 ? MaxYear - 1 : MaxYear;

                var currentData = _repo._context.FinancialPlanTargets.Where(f => !f.IsDel && f.Month == MaxMonth && f.Year == MaxYear).ToList();
                var lastMonthData = _repo._context.FinancialPlanTargets.Where(f => !f.IsDel && f.Month == LastMonth && f.Year == LastYear).ToList();
                var lastYearData = _repo._context.FinancialPlanTargets.Where(f => !f.IsDel && f.Month == MaxMonth && f.Year == MaxYear - 1).ToList();
                var _data = (from cr in currentData
                             join lm in lastMonthData on cr.Name equals lm.Name into JoinLm
                             from lmd in JoinLm.DefaultIfEmpty()
                             join ly in lastYearData on cr.Name equals ly.Name into JoinMy
                             from lyd in JoinMy.DefaultIfEmpty()
                             select new FinancialPlanTargetModel
                             {
                                 FinancialPlanTargetsId = cr.FinancialPlanTargetsId,
                                 Type = cr.Type,
                                 Name = cr.Name,
                                 Year = cr.Year,
                                 Unit = cr.Unit,
                                 Plan = cr.Planning,
                                 Date = MaxYear.ToString() + "-" + MaxMonth.ToString("D2"),
                                 ValueSameMonthLastYear = lyd?.EstimatedMonth ?? 0,
                                 ValueLastMonth = lmd?.EstimatedMonth ?? 0,
                                 EstimatedMonth = cr.EstimatedMonth,
                                 CumulativeToMonth = cr.CumulativeToMonth,
                                 CumulativeToMonthLastYear = lyd?.CumulativeToMonth ?? 0,
                                 CompareLastMonth = lmd?.EstimatedMonth != null && lmd.EstimatedMonth != 0 ? Math.Round(cr.EstimatedMonth / lmd.EstimatedMonth * 100, 2) : 0,
                                 ComparedSameMonth = lyd?.EstimatedMonth != null && lyd.EstimatedMonth != 0 ? Math.Round(cr.EstimatedMonth / lyd.EstimatedMonth * 100, 2) : 0,
                                 AccumulatedComparedYearPlan = cr.Planning != 0 ? Math.Round(cr.EstimatedMonth / cr.Planning * 100, 2) : 0,
                                 AccumulatedComparedPeriod = lyd?.CumulativeToMonth != null && lyd.CumulativeToMonth != 0 ? Math.Round(cr.CumulativeToMonth / lyd.CumulativeToMonth * 100, 2) : 0
                             }).ToList().AsQueryable();

                //Search Data
                if (query.SearchValue != null && query.SearchValue != "")
                {
                    _keywordSearch = query.SearchValue.Trim().ToLower();
                    _data = _data.Where(x =>
                        x.Name.ToLower().Contains(_keywordSearch)
                        || x.Year.ToString().Contains(_keywordSearch)
                        || x.Unit.ToLower().Contains(_keywordSearch)
                        || x.CompareLastMonth.ToString().Contains(_keywordSearch)
                        || x.ComparedSameMonth.ToString().Contains(_keywordSearch)
                        || x.AccumulatedComparedYearPlan.ToString().Contains(_keywordSearch)
                        || x.AccumulatedComparedPeriod.ToString().Contains(_keywordSearch)
                    );
                }

                //Filter Data
                if (query.Filter != null && query.Filter.ContainsKey("Target") && !string.IsNullOrEmpty(query.Filter["Target"]))
                {
                    var Target = string.Join("", query.Filter["Target"]);
                    if (int.Parse(Target) == 4)
                    {
                        _data = _data.Where(x => x.Type > 7 && x.Type < 11);
                    }
                    else if (int.Parse(Target) == 3)
                    {
                        _data = _data.Where(x => x.Type > 2 && x.Type < 8);
                    }
                    else if (int.Parse(Target) == 2)
                    {
                        _data = _data.Where(x => x.Type == 2);
                    }
                    else if (int.Parse(Target) == 1)
                    {
                        _data = _data.Where(x => x.Type == 1);
                    }
                }

                if (query.Filter != null && query.Filter.ContainsKey("MinDate") && !string.IsNullOrEmpty(query.Filter["MinDate"]))
                {
                    _data = _data.Where(x => x.CreateTime >= DateTime.ParseExact(query.Filter["MinDate"], "dd/MM/yyyy", null));
                }

                if (query.Filter != null && query.Filter.ContainsKey("MaxDate") && !string.IsNullOrEmpty(query.Filter["MaxDate"]))
                {
                    _data = _data.Where(x => x.CreateTime <= DateTime.ParseExact(query.Filter["MaxDate"], "dd/MM/yyyy", null));
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
        public async Task<IActionResult> Create(FinancialPlanTargetModel data)
        {
            BaseModels<FinancialPlanTarget> model = new BaseModels<FinancialPlanTarget>();
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                SystemLog datalog = new SystemLog();
                var Check = _repo._context.FinancialPlanTargets.Where(x => x.Name == data.Name && x.Year == data.Year).Any();
                if (Check)
                {
                    model.status = 0;
                    model.error = new ErrorModel()
                    {
                        Code = ErrCode_Const.EXCEPTION_API,
                        Msg = "Báo cáo của " + data.Name + " vào năm " + data.Year + " đã tồn tại"
                    };
                    datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.CREATE, NameCategory_Const.FINANCIAL_PLAN_TARGET, Action_Status.FAIL);
                    _asyncLogger.LogInfo(JsonConvert.SerializeObject(datalog));
                    return BadRequest(model);
                }

                var util = new Ulities();
                data = util.TrimModel(data);

                FinancialPlanTarget SaveData = new FinancialPlanTarget();
                SaveData.Type = data.Type;
                SaveData.Name = data.Name;
                SaveData.Unit = data.Unit;
                SaveData.Year = int.Parse(data.Date.Split("-")[0]);
                SaveData.Month = int.Parse(data.Date.Split("-")[1]);
                SaveData.Planning = data.Plan;
                SaveData.EstimatedMonth = data.EstimatedMonth;
                SaveData.CumulativeToMonth = data.CumulativeToMonth;

                SaveData.CreateUserId = loginData.Userid;
                SaveData.CreateTime = DateTime.Now;

                await _repo.Insert(SaveData);
                datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.CREATE, NameCategory_Const.FINANCIAL_PLAN_TARGET, Action_Status.SUCCESS);
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
        public IActionResult getItemById(Guid id)
        {
            BaseModels<FinancialPlanTargetModel> model = new BaseModels<FinancialPlanTargetModel>();
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
        public async Task<IActionResult> Update(FinancialPlanTargetModel data)
        {
            BaseModels<FinancialPlanTarget> model = new BaseModels<FinancialPlanTarget>();
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                SystemLog datalog = new SystemLog();
                var Check = _repo._context.FinancialPlanTargets.Where(x => x.Name == data.Name && x.Year == data.Year && x.FinancialPlanTargetsId != data.FinancialPlanTargetsId).Any();
                if (Check)
                {
                    model.status = 0;
                    model.error = new ErrorModel()
                    {
                        Code = ErrCode_Const.EXCEPTION_API,
                        Msg = "Báo cáo của " + data.Name + " vào năm " + data.Year + " đã tồn tại"
                    };
                    datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.UPDATE, NameCategory_Const.FINANCIAL_PLAN_TARGET, Action_Status.FAIL);
                    _asyncLogger.LogInfo(JsonConvert.SerializeObject(datalog));
                    return Ok(model);
                }

                var util = new Ulities();
                data = util.TrimModel(data);

                var SaveData = _repo._context.FinancialPlanTargets.Where(x => x.FinancialPlanTargetsId == data.FinancialPlanTargetsId).FirstOrDefault();
                if (SaveData != null)
                {
                    SaveData.Type = data.Type;
                    SaveData.Name = data.Name;
                    SaveData.Unit = data.Unit;
                    SaveData.Year = int.Parse(data.Date.Split("-")[0]);
                    SaveData.Month = int.Parse(data.Date.Split("-")[1]);
                    SaveData.Planning = data.Plan;
                    SaveData.EstimatedMonth = data.EstimatedMonth;
                    SaveData.CumulativeToMonth = data.CumulativeToMonth;

                    SaveData.UpdateUserId = loginData.Userid;
                    SaveData.UpdateTime = DateTime.Now;

                    await _repo.Update(SaveData);
                    datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.UPDATE, NameCategory_Const.FINANCIAL_PLAN_TARGET, Action_Status.SUCCESS);
                    _asyncLogger.LogInfo(JsonConvert.SerializeObject(datalog));
                    model.status = 1;
                    return Ok(model);
                }
                else
                {
                    model.status = 0;
                    datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.UPDATE, NameCategory_Const.FINANCIAL_PLAN_TARGET, Action_Status.FAIL);
                    _asyncLogger.LogInfo(JsonConvert.SerializeObject(datalog));
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

        [HttpPut("delete/{id}")]
        public async Task<IActionResult> delete(Guid id)
        {
            BaseModels<FinancialPlanTarget> model = new BaseModels<FinancialPlanTarget>();
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                SystemLog datalog = new SystemLog();
                var SaveData = _repo._context.FinancialPlanTargets.Where(x => x.FinancialPlanTargetsId == id).FirstOrDefault();
                if (SaveData != null)
                {
                    SaveData.IsDel = true;
                    await _repo.Delete(SaveData);
                    datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.DELETE, NameCategory_Const.FINANCIAL_PLAN_TARGET, Action_Status.SUCCESS);
                    _asyncLogger.LogInfo(JsonConvert.SerializeObject(datalog));
                    model.status = 1;
                    return Ok(model);
                }
                else
                {
                    model.status = 0;
                    datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.DELETE, NameCategory_Const.FINANCIAL_PLAN_TARGET, Action_Status.FAIL);
                    _asyncLogger.LogInfo(JsonConvert.SerializeObject(datalog));
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

        [HttpPost("ExportExcel")]
        public IActionResult ExportExcel(QueryRequestBody query)
        {
            //Query data
            var (data, month, year) = _repo.FindData(query);

            //Check null
            if (data == null)
            {
                return BadRequest();
            }

            var ListGroup = data.GroupBy(x => x.Type);

            try
            {
                using (var workbook = new XLWorkbook(@"Upload/Templates/Chitieusanxuatkinhdoanhxuatkhauchuyeu.xlsx"))
                {
                    IXLWorksheet worksheet = workbook.Worksheets.Worksheet(1);

                    DateTime now = DateTime.Now;
                    worksheet.Cell(5, 3).Value = "K.hoạch\r\n" + year.ToString();
                    worksheet.Cell(5, 4).Value = "Thực hiện tháng " + month + "/" + (year - 1).ToString();
                    worksheet.Cell(5, 5).Value = "Thực hiện tháng " + ((month - 1) == 0 ? "12" : (month - 1).ToString()) + ((month - 1) == 0 ? (year - 1).ToString() : year.ToString());
                    worksheet.Cell(5, 6).Value = "Ước tính TH tháng " + month + "/" + year.ToString();
                    worksheet.Cell(5, 7).Value = month + "/" + year.ToString();
                    worksheet.Cell(5, 8).Value = month + "/" + (year - 1).ToString();

                    string searchKey = "";
                    Dictionary<int, string> ListSearch = new Dictionary<int, string>
                    {
                        { 1, "Giá trị sản xuất" },
                        { 2, "Sản phẩm chủ yếu" },
                        { 3, "Tổng kim ngạch xuất khẩu" },
                        { 4, "Phân theo khối doanh nghiệp" },
                        { 5, "Phân theo nhóm hàng" },
                        { 6, "Mặt hàng xuất khẩu chủ yếu" },
                        { 7, "Thị trường xuất khẩu" },
                        { 8, "Tổng kim ngạch nhập khẩu" },
                        { 9, "Mặt hàng nhập khẩu chủ yếu" },
                        { 10, "Tổng MBLHH-DVXH" }
                    };

                    int index;
                    foreach (var Group in ListGroup.OrderBy(x => x.Key))
                    {
                        searchKey = ListSearch[Group.Key];
                        if (!string.IsNullOrEmpty(searchKey))
                        {
                            var CellGroup = worksheet.Cells("A7:A5000").FirstOrDefault(x => x.Value.ToString().Contains(searchKey));
                            if (CellGroup != null)
                            {
                                if (Group.Key == 3 || Group.Key == 8 || Group.Key == 10)
                                {
                                    index = CellGroup.Address.RowNumber;
                                    foreach (var item in Group)
                                    {
                                        worksheet.Cell(index, 2).Value = item.Unit;
                                        worksheet.Cell(index, 3).Value = item.Plan;
                                        worksheet.Cell(index, 4).Value = item.ValueSameMonthLastYear;
                                        worksheet.Cell(index, 5).Value = item.ValueLastMonth;
                                        worksheet.Cell(index, 6).Value = item.EstimatedMonth;
                                        worksheet.Cell(index, 7).Value = item.CumulativeToMonth;
                                        worksheet.Cell(index, 8).Value = item.CumulativeToMonthLastYear;
                                    }
                                }
                                else if (Group.Key == 1 || Group.Key == 4 || Group.Key == 5)
                                {
                                    IXLCells Range = worksheet.Cells("A8:A9");
                                    if (Group.Key == 4)
                                    {
                                        Range = worksheet.Cells($"A{CellGroup.Address.RowNumber + 1}:A{CellGroup.Address.RowNumber + 2}");
                                    }
                                    else if (Group.Key == 5)
                                    {
                                        Range = worksheet.Cells($"A{CellGroup.Address.RowNumber + 1}:A{CellGroup.Address.RowNumber + 4}");
                                    }

                                    foreach (var item in Group)
                                    {
                                        IXLCell? ItemCell = Range.FirstOrDefault(x => x.Value.ToString().Contains(item.Name));
                                        if (ItemCell != null)
                                        {
                                            index = ItemCell.Address.RowNumber;
                                            worksheet.Cell(index, 2).Value = item.Unit;
                                            worksheet.Cell(index, 3).Value = item.Plan;
                                            worksheet.Cell(index, 4).Value = item.ValueSameMonthLastYear;
                                            worksheet.Cell(index, 5).Value = item.ValueLastMonth;
                                            worksheet.Cell(index, 6).Value = item.EstimatedMonth;
                                            worksheet.Cell(index, 7).Value = item.CumulativeToMonth;
                                            worksheet.Cell(index, 8).Value = item.CumulativeToMonthLastYear;
                                        }
                                    }
                                }
                                else
                                {
                                    index = CellGroup.Address.RowNumber + 1;
                                    foreach (var item in Group)
                                    {
                                        worksheet.Cell(index, 1).Value = "+ " + item.Name;
                                        worksheet.Cell(index, 2).Value = item.Unit;
                                        worksheet.Cell(index, 3).Value = item.Plan;
                                        worksheet.Cell(index, 4).Value = item.ValueSameMonthLastYear;
                                        worksheet.Cell(index, 5).Value = item.ValueLastMonth;
                                        worksheet.Cell(index, 6).Value = item.EstimatedMonth;
                                        worksheet.Cell(index, 7).Value = item.CumulativeToMonth;
                                        worksheet.Cell(index, 8).Value = item.CumulativeToMonthLastYear;
                                        worksheet.Row(index).InsertRowsBelow(1);
                                        index++;
                                    }
                                    worksheet.Row(index).Delete();
                                }
                            }
                        }
                    }

                    //Lưu file:
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
