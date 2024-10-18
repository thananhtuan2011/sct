using API_SoCongThuong.Classes;
using API_SoCongThuong.Logger;
using API_SoCongThuong.Models;
using API_SoCongThuong.Reponsitories;
using ClosedXML.Excel;
using EF_Core.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Data;
using System.Reflection.Metadata;
using System.Security.Principal;
using static System.Net.Mime.MediaTypeNames;


namespace API_SoCongThuong.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportIndexIndustryController : ControllerBase
    {
        private ReportIndexIndustryRepo _repo;

        private IConfiguration _configuration;
        private readonly ILogger<AsyncLogger> _logger;
        public SoHoa_SoCongThuongContext _context;
        private AsyncLogger _asyncLogger;

        public ReportIndexIndustryController(SoHoa_SoCongThuongContext context, IConfiguration configuration, ILogger<AsyncLogger> logger)
        {
            _repo = new ReportIndexIndustryRepo(context);

            _logger = logger;
            _context = context;
            _asyncLogger = new AsyncLogger(_logger, _context);
            _configuration = configuration;
        }

        [Route("find")]
        [HttpPost]
        public IActionResult ListItems_New([FromBody] QueryRequestBody query)//query truyền lên
        {

            BaseModels<ReportIndexIndustryModel> model = new BaseModels<ReportIndexIndustryModel>();
            string _keywordSearch = ""; //Keyword tìm kiếm
            bool _orderBy_ASC = true;
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                Func<ReportIndexIndustryModel, object> _orderByExpression = x => x.Target; //Khởi tạo mặc định sắp xếp dữ liệu
                Dictionary<string, Func<ReportIndexIndustryModel, object>> _sortableFields = new Dictionary<string, Func<ReportIndexIndustryModel, object>>   //Khởi tạo các trường để sắp xếp
                    {
                        { "Target", x => x.Target},
                        { "TargetName", x => x.TargetName},
                        { "ComparedPreviousMonth", x => x.ComparedPreviousMonth },
                        { "SamePeriod", x => x.SamePeriod },
                        { "Accumulation", x => x.Accumulation },

                     };
                if (query.Sort != null
                   && !string.IsNullOrEmpty(query.Sort.ColumnName)
                   && _sortableFields.ContainsKey(query.Sort.ColumnName))
                {
                    _orderBy_ASC = ("desc".Equals(query.Sort.Direction.ToLower()) ? false : true);    //Sắp xếp asc hoặc desc
                    _orderByExpression = _sortableFields[query.Sort.ColumnName]; //Trường cần sắp xếp
                }
                int month = 0;
                int year = 0;
                if (!query.Filter.Any())
                {
                    month = DateTime.Now.Month - 1;
                    year = DateTime.Now.Year;
                }
                else if (query.Filter != null && query.Filter.ContainsKey("Month") && !string.IsNullOrEmpty(query.Filter["Month"]))
                {
                    string time = query.Filter["Month"];
                    month = Int32.Parse(time.Split("-")[1]);
                    year = Int32.Parse(time.Split("-")[0]);
                }

                var result = new List<ReportIndexIndustryModel>();
                List<int> checkPoint = new List<int>();

                IQueryable<DataTarget> samePeriod = _repo._context.ReportIndexIndustries.Where(x => !x.IsDel && x.Month == month && x.Year == (year - 1)).Select(x => new DataTarget { Target = x.Target, ReportIndex = x.ReportIndex });
                IQueryable<DataTarget> comparedPreviousMonth;
                if (month != 1)
                {
                    comparedPreviousMonth = _repo._context.ReportIndexIndustries.Where(x => !x.IsDel && x.Month == (month - 1) && x.Year == year).Select(x => new DataTarget { Target = x.Target, ReportIndex = x.ReportIndex });
                }
                else
                {
                    comparedPreviousMonth = _repo._context.ReportIndexIndustries.Where(x => !x.IsDel && x.Month == 12 && x.Year == (year - 1)).Select(x => new DataTarget { Target = x.Target, ReportIndex = x.ReportIndex });
                }

                List<string> dataTarget = new List<string>() { "Toàn ngành công nghiệp", "Khai khoáng", "Công nghiệp chế biến, chế tạo", "Sản xuất và phân phối điện, khí đốt, nước nóng, hơi nước và điều hòa không khí", "Cung cấp nước, hoạt động quản lý và xử lý rác thải, nước thải" };


                IQueryable<ReportIndexIndustryModel> _data = _repo._context.ReportIndexIndustries.Where(x => !x.IsDel && x.Month <= month && x.Year == year).Select(x => new ReportIndexIndustryModel
                {
                    ReportIndexIndustryId = x.ReportIndexIndustryId,
                    Target = x.Target,
                    TargetName = dataTarget[x.Target],
                    Month = month < 10 ? $"{year}-0{month}" : $"{year}-{month}",
                    ReportIndex = x.ReportIndex,
                    DataTarget = _repo._context.ReportIndexIndustries.Where(d => d.Month <= month && d.Year == year && d.Target == x.Target).Select(d => new ValueDataTarget
                    {
                        Month = d.Month,
                        ReportIndex = d.ReportIndex,
                        ReportIndexIndustryId = d.ReportIndexIndustryId
                    }).OrderBy(d => d.Month).ToList(),
                    ComparedPreviousMonth = comparedPreviousMonth.Where(c => c.Target == x.Target).Select(c => c.ReportIndex).FirstOrDefault() > 0 ? Math.Round((_repo._context.ReportIndexIndustries.Where(r => !r.IsDel && r.Target == x.Target && r.Month == month && r.Year == year).Select(r => r.ReportIndex).FirstOrDefault() / comparedPreviousMonth.Where(c => c.Target == x.Target).Select(c => c.ReportIndex).FirstOrDefault()) * 100, 2) : 0,
                    SamePeriod = samePeriod.Where(c => c.Target == x.Target).Select(c => c.ReportIndex).FirstOrDefault() > 0 ? Math.Round((_repo._context.ReportIndexIndustries.Where(r => !r.IsDel && r.Target == x.Target && r.Month == month && r.Year == year).Select(r => r.ReportIndex).FirstOrDefault() / samePeriod.Where(c => c.Target == x.Target).Select(c => c.ReportIndex).FirstOrDefault()) * 100, 2) : 0,
                    Accumulation = _repo._context.ReportIndexIndustries.Where(c => !c.IsDel && c.Month <= month && c.Year == (year - 1) && c.Target == x.Target).Sum(c => c.ReportIndex) > 0 ? Math.Round((_repo._context.ReportIndexIndustries.Where(r => !r.IsDel && r.Target == x.Target && r.Month <= month && r.Year == year).Sum(r => r.ReportIndex) / _repo._context.ReportIndexIndustries.Where(c => !c.IsDel && c.Month <= month && c.Year == (year - 1) && c.Target == x.Target).Sum(c => c.ReportIndex)) * 100, 2) : 0,
                }).ToList().AsQueryable();

                if (query.SearchValue != null && query.SearchValue != "")
                {
                    _keywordSearch = query.SearchValue.Trim().ToLower();
                    _data = _data.Where(x =>
                       x.TargetName.ToLower().Contains(_keywordSearch)
                   );
                }

                if (_orderBy_ASC) //Sắp xếp dữ liệu theo acs
                {
                    _data = _data
                        .OrderBy(_orderByExpression)
                        .ToList().AsQueryable();
                }
                else //Sắp xếp dữ liệu theo desc
                {
                    _data = _data
                        .OrderByDescending(_orderByExpression)
                        .ToList().AsQueryable();
                }

                foreach (var item in _data)
                {
                    if (checkPoint.Count == 5) break;
                    if (!checkPoint.Contains(item.Target))
                    {
                        List<ValueDataTarget> customDataTarget = new List<ValueDataTarget>();
                        if (item.DataTarget != null)
                        {
                            for (int i = 1; i <= month; i++)
                            {
                                ValueDataTarget valueDataTarget = new ValueDataTarget();
                                bool checkAdd = false;
                                foreach (var it in item.DataTarget)
                                {
                                    if (i == it.Month)
                                    {
                                        valueDataTarget.Month = it.Month;
                                        valueDataTarget.ReportIndex = it.ReportIndex;
                                        valueDataTarget.ReportIndexIndustryId = it.ReportIndexIndustryId;
                                        customDataTarget.Add(valueDataTarget);
                                        checkAdd = true;
                                        break;
                                    }
                                }
                                if (!checkAdd)
                                {
                                    valueDataTarget.Month = i;
                                    valueDataTarget.ReportIndex = 0;
                                    valueDataTarget.ReportIndexIndustryId = Guid.Empty;
                                    customDataTarget.Add(valueDataTarget);
                                }
                            }
                        }
                        item.DataTarget = customDataTarget;
                        int count = customDataTarget.Count;
                        if (count > 0)
                        {
                            item.ReportIndex = customDataTarget[count - 1].ReportIndex;
                            item.ReportIndexIndustryId = customDataTarget[count - 1].ReportIndexIndustryId;
                        }
                        checkPoint.Add(item.Target);
                        result.Add(item);
                    }
                }

                //              _data = _data.DistinctBy(x => x.Target).AsQueryable();


                int _countRows = result.Count(); //Đếm số dòng của table đã select được
             //   result = (List<ReportIndexIndustryModel>)result.AsQueryable();
                if (_countRows == 0)    //nếu table = 0 thì trả về không có dữ liệu
                {
                    return NotFound("Không có dữ liệu");
                }
                for(int i = 0; i<result.Count; i++)
                {
                    if (result[i].Target == 0)
                    {
                        if (i == 0)
                            break;
                        ReportIndexIndustryModel tmp = new ReportIndexIndustryModel();
                        tmp = result[0];
                        result[0] = result[i];
                        result[i] = tmp;
                        break;
                    }
                }
                if (query.Panigator.More)    //query more = true
                {
                    model.status = 1;
                    model.items = result.ToList();
                    model.total = _countRows;
                    return Ok(model);
                }
                model.items = result.ToList();
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
            BaseModels<ReportIndexIndustryModel> model = new BaseModels<ReportIndexIndustryModel>();
            try
            {
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


        [HttpPut("{id}")]
        public async Task<IActionResult> Update(ReportIndexIndustryModel data)
        {
            BaseModels<ReportIndexIndustryModel> model = new BaseModels<ReportIndexIndustryModel>();
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                int Month = Int32.Parse(data.Month.Split("-")[1]);
                int Year = Int32.Parse(data.Month.Split("-")[0]);
                int count = _repo._context.ReportIndexIndustries.Where(x => !x.IsDel && x.ReportIndexIndustryId != data.ReportIndexIndustryId && x.Month == Month && x.Year == Year && x.Target == data.Target).Count();
                if (count > 0)
                {
                    model.status = 0;
                    model.error = new ErrorModel()
                    {
                        Code = ErrCode_Const.EXCEPTION_API,
                        Msg = "Chỉ tiêu báo cáo đã tồn tại!"
                    };
                    return Ok(model);
                }
                SystemLog datalog = new SystemLog();
                ReportIndexIndustry? _data = _repo._context.ReportIndexIndustries.Where(x => !x.IsDel && x.ReportIndexIndustryId == data.ReportIndexIndustryId).FirstOrDefault();
                if (_data != null)
                {
                    _data.Month = Month;
                    _data.Year = Year;
                    _data.ReportIndex = data.ReportIndex;
                    _data.UpdateTime = DateTime.Now;
                    _data.UpdateUserId = loginData.Userid;
                    _data.Target = data.Target;
                    await _repo.Update(_data);
                    datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.UPDATE, NameCategory_Const.REPORT_INDEX_INDUSTRY, Action_Status.SUCCESS);
                    _asyncLogger.LogInfo(JsonConvert.SerializeObject(datalog));
                }
                else
                {
                    ReportIndexIndustry SaveData = new ReportIndexIndustry();
                    SaveData.Month = Month;
                    SaveData.Year = Year;
                    SaveData.Target = data.Target;
                    SaveData.ReportIndex = data.ReportIndex;
                    SaveData.CreateTime = DateTime.Now;
                    SaveData.CreateUserId = loginData.Userid;
                    await _repo.Insert(SaveData);
                    datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.UPDATE, NameCategory_Const.REPORT_INDEX_INDUSTRY, Action_Status.SUCCESS);
                    _asyncLogger.LogInfo(JsonConvert.SerializeObject(datalog));
                }

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
        [HttpPost()]
        public async Task<IActionResult> create(ReportIndexIndustryModel data)
        {
            BaseModels<ReportIndexIndustryModel> model = new BaseModels<ReportIndexIndustryModel>();
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                SystemLog datalog = new SystemLog();
                int Month = Int32.Parse(data.Month.Split("-")[1]);
                int Year = Int32.Parse(data.Month.Split("-")[0]);
                int count = _repo._context.ReportIndexIndustries.Where(x => !x.IsDel && x.Target == data.Target && x.Month == Month && x.Year == Year).Count();
                if (count > 0)
                {
                    model.status = 0;
                    model.error = new ErrorModel()
                    {
                        Code = ErrCode_Const.EXCEPTION_API,
                        Msg = "Chỉ tiêu báo cáo đã tồn tại!"
                    };
                    datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.CREATE, NameCategory_Const.REPORT_INDEX_INDUSTRY, Action_Status.FAIL);
                    _asyncLogger.LogInfo(JsonConvert.SerializeObject(datalog));
                    return Ok(model);
                }
                ReportIndexIndustry SaveData = new ReportIndexIndustry();
                SaveData.Month = Month;
                SaveData.Year = Year;
                SaveData.Target = data.Target;
                SaveData.ReportIndex = data.ReportIndex;
                SaveData.CreateTime = DateTime.Now;
                SaveData.CreateUserId = loginData.Userid;
                await _repo.Insert(SaveData);
                datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.CREATE, NameCategory_Const.REPORT_INDEX_INDUSTRY, Action_Status.SUCCESS);
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
            BaseModels<ReportIndexIndustryModel> model = new BaseModels<ReportIndexIndustryModel>();
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                SystemLog datalog = new SystemLog();
                ReportIndexIndustry item = new ReportIndexIndustry();
                item.ReportIndexIndustryId = id;
                item.IsDel = true;
                await _repo.Delete(item);
                datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.DELETE, NameCategory_Const.REPORT_INDEX_INDUSTRY, Action_Status.SUCCESS);
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

        [HttpPost("Export")]
        public IActionResult Export([FromBody] QueryRequestBody query)
        {
            //Query data
            var data = FindData(query);
            // var _data = OkObjectResult(dat)
            if (data == null)
            {
                return BadRequest();
            }
            try
            {
                int month = data[0].MonthX;
                int year = data[0].YearX;

                using (var workbook = new XLWorkbook(@"Upload/Templates/QuanLyBaoCaoChiSoSanXuatCongNghiepThang.xlsx"))
                {
                    IXLWorksheet worksheet = workbook.Worksheets.Worksheet(1);
                    int index = 12;
                    int row = 1;
                    int preMonth = month - 1 == 0 ? 12 : month - 1;
                    string text = preMonth == 12 ? "trước" : "báo cáo";
                    worksheet.Cell(9, 3).Value = $"Các tháng năm {year} so với tháng bình quân năm gốc 2015";
                    worksheet.Cell(7, 1).Value = $"Tháng {month} năm {year}";
                    worksheet.Cell(9, 4).Value = $"Tháng {month} so với tháng {preMonth} của năm {text}";
                    worksheet.Cell(9, 5).Value = $"Tháng {month} so với tháng cùng kỳ năm trước";
                    worksheet.Cell(9, 6).Value = $"Lũy kế đến tháng {month} so với lũy kế cùng kỳ năm trước";

                    if (month > 1)
                    {
                        worksheet.Column(3).InsertColumnsAfter(month - 1);
                    }
                    worksheet.Range(9, 3, 9, 3 + month - 1).Merge();
                    for (int i = 1; i <= month; i++)
                    {
                        worksheet.Cell(10, 2 + i).Value = $"Tháng {i}";
                        worksheet.Cell(11, 2 + i).Value = i;
                    }

                    worksheet.Cell(11, 3 + month).Value = month + 1;
                    worksheet.Cell(11, 4 + month).Value = month + 2;
                    worksheet.Cell(11, 5 + month).Value = month + 3;

                    //Thêm dữ liệu vào file:
                    List<string> dataTarget = new List<string>() { "Toàn ngành công nghiệp", "Khai khoáng", "Công nghiệp chế biến, chế tạo", "Sản xuất và phân phối điện, khí đốt, nước nóng, hơi nước và điều hòa không khí", "Cung cấp nước, hoạt động quản lý và xử lý rác thải, nước thải" };
                    foreach (var item in data)
                    {
                        if (row == 1)
                        {
                            worksheet.Cell(index, 1).Value = row;
                            worksheet.Cell(index, 2).Value = dataTarget[item.Target];
                            if (item.DataTarget != null)
                            {
                                for (int i = 1; i <= month; i++)
                                {
                                    worksheet.Cell(index, 2 + i).Value = item.DataTarget[i - 1].ReportIndex == 0 ? "--" : item.DataTarget[i - 1].ReportIndex;
                                }
                            }
                            worksheet.Cell(index, 3 + month).Value = item.ComparedPreviousMonth == 0 ? "--" : item.ComparedPreviousMonth;
                            worksheet.Cell(index, 4 + month).Value = item.SamePeriod == 0 ? "--" : item.SamePeriod;
                            worksheet.Cell(index, 5 + month).Value = item.Accumulation == 0 ? "--" : item.Accumulation;

                            //worksheet.Cell(index, 5).Value = item.Address;
                            index++;
                            row++;
                        }
                        else
                        {
                            var addrow = worksheet.Row(index - 1);
                            addrow.InsertRowsBelow(1);
                            worksheet.Cell(index, 1).Value = row;
                            worksheet.Cell(index, 2).Value = dataTarget[item.Target];
                            if (item.DataTarget != null)
                            {
                                for (int i = 1; i <= month; i++)
                                {
                                    worksheet.Cell(index, 2 + i).Value = item.DataTarget[i - 1].ReportIndex == 0 ? "--" : item.DataTarget[i - 1].ReportIndex;
                                }
                            }
                            worksheet.Cell(index, 3 + month).Value = item.ComparedPreviousMonth == 0 ? "--" : item.ComparedPreviousMonth;
                            worksheet.Cell(index, 4 + month).Value = item.SamePeriod == 0 ? "--" : item.SamePeriod;
                            worksheet.Cell(index, 5 + month).Value = item.Accumulation == 0 ? "--" : item.Accumulation;

                            //worksheet.Cell(index, 5).Value = item.Address;
                            index++;
                            row++;
                        }
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

        private List<ReportIndexIndustryModel> FindData([FromBody] QueryRequestBody query)//query truyền lên
        {

            int month = 0;
            int year = 0;
            if (!query.Filter.Any())
            {
                month = DateTime.Now.Month - 1;
                year = DateTime.Now.Year;
            }
            else if (query.Filter != null && query.Filter.ContainsKey("Month") && !string.IsNullOrEmpty(query.Filter["Month"]))
            {
                string time = query.Filter["Month"];
                month = Int32.Parse(time.Split("-")[1]);
                year = Int32.Parse(time.Split("-")[0]);
            }

            List<ReportIndexIndustryModel> result = new List<ReportIndexIndustryModel>();
            List<int> checkPoint = new List<int>();

            IQueryable<DataTarget> samePeriod = _repo._context.ReportIndexIndustries.Where(x => !x.IsDel && x.Month == month && x.Year == (year - 1)).Select(x => new DataTarget { Target = x.Target, ReportIndex = x.ReportIndex });
            IQueryable<DataTarget> comparedPreviousMonth;
            if (month != 1)
            {
                comparedPreviousMonth = _repo._context.ReportIndexIndustries.Where(x => !x.IsDel && x.Month == (month - 1) && x.Year == year).Select(x => new DataTarget { Target = x.Target, ReportIndex = x.ReportIndex });
            }
            else
            {
                comparedPreviousMonth = _repo._context.ReportIndexIndustries.Where(x => !x.IsDel && x.Month == 12 && x.Year == (year - 1)).Select(x => new DataTarget { Target = x.Target, ReportIndex = x.ReportIndex });
            }

            var _data = _repo._context.ReportIndexIndustries.Where(x => !x.IsDel && x.Month <= month && x.Year == year).Select(x => new ReportIndexIndustryModel
            {
                ReportIndexIndustryId = x.ReportIndexIndustryId,
                Target = x.Target,
                Month = x.Month < 10 ? $"{x.Year}-0{x.Month}" : $"{x.Year}-{x.Month}",
                ReportIndex = x.ReportIndex,
                MonthX = month,
                YearX = year,
                DataTarget = _repo._context.ReportIndexIndustries.Where(d => d.Month <= month && d.Year == year && d.Target == x.Target).Select(d => new ValueDataTarget
                {
                    Month = d.Month,
                    ReportIndex = d.ReportIndex
                }).OrderBy(d => d.Month).ToList(),
                ComparedPreviousMonth = comparedPreviousMonth.Where(c => c.Target == x.Target).Select(c => c.ReportIndex).FirstOrDefault() > 0 ? Math.Round((_repo._context.ReportIndexIndustries.Where(r => !r.IsDel && r.Target == x.Target && r.Month == month && r.Year == year).Select(r => r.ReportIndex).FirstOrDefault() / comparedPreviousMonth.Where(c => c.Target == x.Target).Select(c => c.ReportIndex).FirstOrDefault()) * 100, 2) : 0,
                SamePeriod = samePeriod.Where(c => c.Target == x.Target).Select(c => c.ReportIndex).FirstOrDefault() > 0 ? Math.Round((_repo._context.ReportIndexIndustries.Where(r => !r.IsDel && r.Target == x.Target && r.Month == month && r.Year == year).Select(r => r.ReportIndex).FirstOrDefault() / samePeriod.Where(c => c.Target == x.Target).Select(c => c.ReportIndex).FirstOrDefault()) * 100, 2) : 0,
                Accumulation = _repo._context.ReportIndexIndustries.Where(c => !c.IsDel && c.Month <= month && c.Year == (year - 1) && c.Target == x.Target).Sum(c => c.ReportIndex) > 0 ? Math.Round((_repo._context.ReportIndexIndustries.Where(r => !r.IsDel && r.Target == x.Target && r.Month <= month && r.Year == year).Sum(r => r.ReportIndex) / _repo._context.ReportIndexIndustries.Where(c => !c.IsDel && c.Month <= month && c.Year == (year - 1) && c.Target == x.Target).Sum(c => c.ReportIndex)) * 100, 2) : 0,
            }).OrderBy(x => x.Target);

            foreach (var item in _data)
            {
                if (checkPoint.Count == 5) break;
                if (!checkPoint.Contains(item.Target))
                {
                    List<ValueDataTarget> customDataTarget = new List<ValueDataTarget>();
                    if (item.DataTarget != null)
                    {
                        for (int i = 1; i <= month; i++)
                        {
                            ValueDataTarget valueDataTarget = new ValueDataTarget();
                            bool checkAdd = false;
                            foreach (var it in item.DataTarget)
                            {
                                if (i == it.Month)
                                {
                                    valueDataTarget.Month = it.Month;
                                    valueDataTarget.ReportIndex = it.ReportIndex;
                                    customDataTarget.Add(valueDataTarget);
                                    checkAdd = true;
                                    break;
                                }
                            }
                            if (!checkAdd)
                            {
                                valueDataTarget.Month = i;
                                valueDataTarget.ReportIndex = 0;
                                customDataTarget.Add(valueDataTarget);
                            }
                        }
                    }
                    item.DataTarget = customDataTarget;
                    checkPoint.Add(item.Target);
                    result.Add(item);
                }
            }

            return result.ToList();
        }

    }
}

