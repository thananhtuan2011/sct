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
using System.Linq;
using System.Reflection.Metadata;
using System.Security.Principal;
using static System.Net.Mime.MediaTypeNames;

namespace API_SoCongThuong.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TotalRetailSaleController : ControllerBase
    {
        private TotalRetailSaleRepo _repo;
        private IConfiguration _configuration;
        private readonly ILogger<AsyncLogger> _logger;
        public SoHoa_SoCongThuongContext _context;
        private AsyncLogger _asyncLogger;
        public TotalRetailSaleController(SoHoa_SoCongThuongContext context, IConfiguration configuration, ILogger<AsyncLogger> logger)
        {
            _repo = new TotalRetailSaleRepo(context);
            _logger = logger;
            _context = context;
            _asyncLogger = new AsyncLogger(_logger, _context);
            _configuration = configuration;
        }

        [Route("find")]
        [HttpPost]
        public IActionResult ListItems_New([FromBody] QueryRequestBody query)//query truyền lên
        {

            BaseModels<TotalRetailSaleModel> model = new BaseModels<TotalRetailSaleModel>();
            string _keywordSearch = ""; //Keyword tìm kiếm
            bool _orderBy_ASC = true;  //Khởi tạo sắp xếp dữ liệu acs hoặc desc khi tìm kiếm
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
               
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                Func<TotalRetailSaleModel, object> _orderByExpression = x => x.TargetName; //Khởi tạo mặc định sắp xếp dữ liệu
                Dictionary<string, Func<TotalRetailSaleModel, object>> _sortableFields = new Dictionary<string, Func<TotalRetailSaleModel, object>>   //Khởi tạo các trường để sắp xếp
                {
                    { "TargetName", x => x.TargetName },
                    { "YORImplement", x => x.YORImplement },
                    { "ReportIndex", x => x.ReportIndex },
                    { "YORAccumulation", x => x.YORAccumulation },
                    { "PYImplement", x => x.PYImplement },
                    { "PYAccumulation", x => x.PYAccumulation },
                    { "RatioImplementLastMonth", x => x.RatioImplementLastMonth },
                    { "RatioImplementSamePeriod", x => x.RatioImplementSamePeriod },
                    { "RatioAccumulation", x => x.RatioAccumulation }
                };
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

                if (month == 0)
                {
                    month = 12;
                    year--;
                }

                int preMonth = 0;
                int preYear = 0;
                if (month == 1)
                {
                    preMonth = 12;
                    preYear = year - 1;
                } else 
                {
                    preMonth = month - 1;
                    preYear = year;
                }
                List<string> targetName = new List<string>() {"", "Bán lẻ hàng hóa", "Lưu trú, ăn uống","Du lịch","Dịch vụ khác"};
                IQueryable<TotalRetailSale> _data = _repo._context.TotalRetailSales.Where(x => !x.IsDel).Select(x => new TotalRetailSale
                {
                    Target = x.Target,
                    Month = x.Month,
                    Year = x.Year,
                    TotalRetailSaleId = x.TotalRetailSaleId,
                    ReportIndex = x.ReportIndex
                }).ToList().AsQueryable();
                IQueryable< TotalRetailSaleModel> result = _data.Where(x => x.Month == month && x.Year == year).Select(x => new TotalRetailSaleModel
                {
                    TotalRetailSaleId = x.TotalRetailSaleId,
                    Target = x.Target,
                    TargetName = targetName[x.Target],
                    ReportIndex = x.ReportIndex,
                    Month = month < 10 ? $"{year}-0{month}" : $"{year}-{month}",
                    YORImplement = _data.Where(d => d.Month == preMonth && d.Year == preYear && d.Target == x.Target).Select(d => d.ReportIndex).FirstOrDefault(),
                    YORAccumulation = _data.Where(d => d.Month <= month && d.Year == year && d.Target == x.Target).Sum(d => d.ReportIndex),
                    PYImplement = _data.Where(d => d.Month == month && d.Year == (year - 1) && d.Target == x.Target ).Select(d => d.ReportIndex).FirstOrDefault(),
                    PYAccumulation = _data.Where(d => d.Month <= month && d.Year == (year-1) && d.Target == x.Target).Sum(d => d.ReportIndex),
                    RatioImplementLastMonth = _data.Where(d => d.Month == preMonth && d.Year == preYear && d.Target == x.Target).Select(d => d.ReportIndex).FirstOrDefault() == 0 ? 0 : Math.Round((x.ReportIndex / _data.Where(d => d.Month == preMonth && d.Year == preYear && d.Target == x.Target).Select(d => d.ReportIndex).FirstOrDefault() * 100 ), 3),
                    RatioImplementSamePeriod = _data.Where(d => d.Month == month && d.Year == (year - 1) && d.Target == x.Target).Select(d => d.ReportIndex).FirstOrDefault() == 0 ? 0 : Math.Round((x.ReportIndex / _data.Where(d => d.Month == month && d.Year == year - 1 && d.Target == x.Target).Select(d => d.ReportIndex).FirstOrDefault() * 100 ), 3),
                    RatioAccumulation = _data.Where(d => d.Month <= month && d.Year == (year - 1) && d.Target == x.Target).Sum(d => d.ReportIndex) == 0 ? 0 : Math.Round((_data.Where(d => d.Month <= month && d.Year == year && d.Target == x.Target).Sum(d => d.ReportIndex) / _data.Where(d => d.Month <= month && d.Year == year - 1 && d.Target == x.Target).Sum(d => d.ReportIndex) * 100 ), 3),
                }).ToList().AsQueryable() ;

                if (query.SearchValue != null && query.SearchValue != "") //Kiểm tra điều kiện tìm kiếm
                {
                    _keywordSearch = query.SearchValue.Trim().ToLower();
                    result = result.Where(x =>
                       x.TargetName.ToLower().Contains(_keywordSearch)
                   );  //Lấy table đã select tìm kiếm theo keyword
                }

                if (query.Sort != null
                  && !string.IsNullOrEmpty(query.Sort.ColumnName)
                  && _sortableFields.ContainsKey(query.Sort.ColumnName))
                {
                    _orderBy_ASC = ("desc".Equals(query.Sort.Direction.ToLower()) ? false : true);    //Sắp xếp asc hoặc desc
                    _orderByExpression = _sortableFields[query.Sort.ColumnName]; //Trường cần sắp xếp
                }


                //_dataTotal.RatioImplementLastMonth = _dataTotal.YORImplement == 0 ? 0 : Math.Round((decimal)((_dataTotal.ReportIndex / _dataTotal.YORImplement) * 100)),2);


                int _countRows = result.Count(); //Đếm số dòng của table đã select được
                if (_countRows == 0)    //nếu table = 0 thì trả về không có dữ liệu
                {
                    return NotFound("Không có dữ liệu");
                }
                if (query.Panigator.More)    //query more = true
                {
                    model.status = 1;
                    model.items = result.ToList();
                    model.total = _countRows;
                    return Ok(model);
                }
                TotalRetailSaleModel _dataTotal = new TotalRetailSaleModel();
                foreach (TotalRetailSaleModel item in result)
                {
                    _dataTotal.Target = 0;
                    _dataTotal.TargetName = "Tổng mức bán lẻ hàng hóa và doanh thu dịch vụ";
                    _dataTotal.ReportIndex += item.ReportIndex;
                    _dataTotal.YORImplement += item.YORImplement;
                    _dataTotal.YORAccumulation += item.YORAccumulation;
                    _dataTotal.PYImplement += item.PYImplement;
                    _dataTotal.PYAccumulation += item.PYAccumulation;
                }
                if (_dataTotal.YORImplement != null && _dataTotal.YORImplement != 0)
                {
                    _dataTotal.RatioImplementLastMonth = Math.Round((decimal)((_dataTotal.ReportIndex / _dataTotal.YORImplement) * 100), 2);
                }
                else
                {
                    _dataTotal.RatioImplementLastMonth = 0;
                }

                if (_dataTotal.PYImplement != null && _dataTotal.PYImplement != 0)
                {
                    _dataTotal.RatioImplementSamePeriod = Math.Round((decimal)((_dataTotal.ReportIndex / _dataTotal.PYImplement) * 100), 2);
                }
                else
                {
                    _dataTotal.RatioImplementSamePeriod = 0;
                }

                if (_dataTotal.PYAccumulation != null && _dataTotal.PYAccumulation != 0)
                {
                    _dataTotal.RatioAccumulation = Math.Round((decimal)((_dataTotal.YORAccumulation / _dataTotal.PYAccumulation) * 100), 2);
                }
                else
                {
                    _dataTotal.RatioAccumulation = 0;
                }

             //   model.items = result.ToList();
                if (_orderBy_ASC) //Sắp xếp dữ liệu theo acs
                {
                    model.items = result
                        .OrderBy(_orderByExpression)
                        .Skip((query.Panigator.PageIndex - 1) * query.Panigator.PageSize)
                        .Take(query.Panigator.PageSize)
                        .ToList();
                }
                else //Sắp xếp dữ liệu theo desc
                {
                    model.items = result
                        .OrderByDescending(_orderByExpression)
                        .Skip((query.Panigator.PageIndex - 1) * query.Panigator.PageSize)
                        .Take(query.Panigator.PageSize)
                        .ToList();
                }
                model.items.Insert(0,_dataTotal);
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
            BaseModels<TotalRetailSaleModel> model = new BaseModels<TotalRetailSaleModel>();
            try
            {
                var result = _repo.FindById(id);
                if (result == null)
                {
                    model.status = 0;
                    model.error = new ErrorModel()
                    {
                        Code = ErrCode_Const.CANNOT_FIND_DATA_BY_QUERY,
                        Msg = "Không có dữ liệu",
                    };
                    return NotFound(model);
                }
                else
                {
                    model.status = 1;
                    model.items = result.ToList();
                    return Ok(model);
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
        public async Task<IActionResult> Update(TotalRetailSaleModel data)
        {
            BaseModels<TotalRetailSaleModel> model = new BaseModels<TotalRetailSaleModel>();
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                SystemLog datalog = new SystemLog();
                int month = Int32.Parse(data.Month.Split("-")[1]);
                int year = Int32.Parse(data.Month.Split("-")[0]);
                int count = _repo._context.TotalRetailSales.Where(x => !x.IsDel && x.TotalRetailSaleId != data.TotalRetailSaleId && x.Target == data.Target && x.Month == month && x.Year == year  ).Count();
                if (count > 0)
                {
                    model.status = 0;
                    model.error = new ErrorModel()
                    {
                        Code = ErrCode_Const.EXCEPTION_API,
                        Msg = "Tháng báo cáo đã tồn tại!"
                    };
                    datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.UPDATE, NameCategory_Const.TOTAL_RETAIL_SALE, Action_Status.FAIL);
                    _asyncLogger.LogInfo(JsonConvert.SerializeObject(datalog));
                    return Ok(model);
                }
                TotalRetailSale? SaveData = _repo._context.TotalRetailSales.Where(x => !x.IsDel && x.TotalRetailSaleId == data.TotalRetailSaleId).FirstOrDefault();
                if(SaveData != null)
                {
                    SaveData.Target = data.Target;
                    SaveData.Month = month;
                    SaveData.Year = year;
                    SaveData.ReportIndex = data.ReportIndex;
                    SaveData.UpdateTime = DateTime.Now;
                    SaveData.UpdateUserId = loginData.Userid;
                    await _repo.Update(SaveData);
                    datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.UPDATE, NameCategory_Const.TOTAL_RETAIL_SALE, Action_Status.SUCCESS);
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
        public async Task<IActionResult> create(TotalRetailSaleModel data)
        {
            BaseModels<TotalRetailSaleModel> model = new BaseModels<TotalRetailSaleModel>();
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                SystemLog datalog = new SystemLog();
                int month = Int32.Parse(data.Month.Split("-")[1]);
                int year = Int32.Parse(data.Month.Split("-")[0]);
                var _data = _repo._context.TotalRetailSales.Where(x => !x.IsDel && x.Target == data.Target && x.Month == month && x.Year == year).FirstOrDefault();
                if (_data != null)
                {
                    model.status = 0;
                    model.error = new ErrorModel()
                    {
                        Code = ErrCode_Const.EXCEPTION_API,
                        Msg = "Tháng báo cáo đã tồn tại!"
                    };
                    datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.CREATE, NameCategory_Const.TOTAL_RETAIL_SALE, Action_Status.FAIL);
                    _asyncLogger.LogInfo(JsonConvert.SerializeObject(datalog));
                    return Ok(model);
                }
                TotalRetailSale SaveData = new TotalRetailSale();
                SaveData.Target = data.Target;
                SaveData.Month = month;
                SaveData.Year = year;
                SaveData.ReportIndex = data.ReportIndex;
                SaveData.CreateTime = DateTime.Now;
                SaveData.CreateUserId = loginData.Userid;
                await _repo.Insert(SaveData);
                datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.CREATE, NameCategory_Const.TOTAL_RETAIL_SALE, Action_Status.SUCCESS);
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
            BaseModels<TotalRetailSaleModel> model = new BaseModels<TotalRetailSaleModel>();
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);

                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                SystemLog datalog = new SystemLog();
                TotalRetailSale item = new TotalRetailSale();
                item.IsDel = true;
                item.TotalRetailSaleId = id;
                await _repo.Delete(item);
                datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.DELETE, NameCategory_Const.TOTAL_RETAIL_SALE, Action_Status.SUCCESS);
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
        private List<TotalRetailSaleModel> FindData([FromBody] QueryRequestBody query)//query truyền lên
        {
            string _keywordSearch = ""; //Keyword tìm kiếm
            List<TotalRetailSaleModel> Result = new List<TotalRetailSaleModel>();
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

            if (month == 0)
            {
                month = 12;
                year--;
            }

            int preMonth = 0;
            int preYear = 0;
            if (month == 1)
            {
                preMonth = 12;
                preYear = year - 1;
            }
            else
            {
                preMonth = month - 1;
                preYear = year;
            }
            List<string> targetName = new List<string>() { "", "Bán lẻ hàng hóa", "Lưu trú, ăn uống", "Du lịch", "Dịch vụ khác" };
            IQueryable<TotalRetailSale> _data = _repo._context.TotalRetailSales.Where(x => !x.IsDel).Select(x => new TotalRetailSale
            {
                Target = x.Target,
                Month = x.Month,
                Year = x.Year,
                TotalRetailSaleId = x.TotalRetailSaleId,
                ReportIndex = x.ReportIndex
            }).ToList().AsQueryable();
            IQueryable<TotalRetailSaleModel> result = _data.Where(x => x.Month == month && x.Year == year).Select(x => new TotalRetailSaleModel
            {
                TotalRetailSaleId = x.TotalRetailSaleId,
                Target = x.Target,
                TargetName = targetName[x.Target],
                ReportIndex = x.ReportIndex,
                Month = month < 10 ? $"{year}-0{month}" : $"{year}-{month}",
                YORImplement = _data.Where(d => d.Month == preMonth && d.Year == preYear && d.Target == x.Target).Select(d => d.ReportIndex).FirstOrDefault(),
                YORAccumulation = _data.Where(d => d.Month <= month && d.Year == year && d.Target == x.Target).Sum(d => d.ReportIndex),
                PYImplement = _data.Where(d => d.Month == month && d.Year == (year - 1) && d.Target == x.Target).Select(d => d.ReportIndex).FirstOrDefault(),
                PYAccumulation = _data.Where(d => d.Month <= month && d.Year == (year - 1) && d.Target == x.Target).Sum(d => d.ReportIndex),
                RatioImplementLastMonth = _data.Where(d => d.Month == preMonth && d.Year == preYear && d.Target == x.Target).Select(d => d.ReportIndex).FirstOrDefault() == 0 ? 0 : Math.Round((x.ReportIndex / _data.Where(d => d.Month == preMonth && d.Year == preYear && d.Target == x.Target).Select(d => d.ReportIndex).FirstOrDefault() * 100), 3),
                RatioImplementSamePeriod = _data.Where(d => d.Month == month && d.Year == (year - 1) && d.Target == x.Target).Select(d => d.ReportIndex).FirstOrDefault() == 0 ? 0 : Math.Round((x.ReportIndex / _data.Where(d => d.Month == month && d.Year == year - 1 && d.Target == x.Target).Select(d => d.ReportIndex).FirstOrDefault() * 100), 3),
                RatioAccumulation = _data.Where(d => d.Month <= month && d.Year == (year - 1) && d.Target == x.Target).Sum(d => d.ReportIndex) == 0 ? 0 : Math.Round((_data.Where(d => d.Month <= month && d.Year == year && d.Target == x.Target).Sum(d => d.ReportIndex) / _data.Where(d => d.Month <= month && d.Year == year - 1 && d.Target == x.Target).Sum(d => d.ReportIndex) * 100), 3),
            }).ToList().AsQueryable();

            if (query.SearchValue != null && query.SearchValue != "") //Kiểm tra điều kiện tìm kiếm
            {
                _keywordSearch = query.SearchValue.Trim().ToLower();
                result = result.Where(x =>
                   x.TargetName.ToLower().Contains(_keywordSearch)
               );  //Lấy table đã select tìm kiếm theo keyword
            }


            //_dataTotal.RatioImplementLastMonth = _dataTotal.YORImplement == 0 ? 0 : Math.Round((decimal)((_dataTotal.ReportIndex / _dataTotal.YORImplement) * 100)),2);


            int _countRows = result.Count(); //Đếm số dòng của table đã select được
            if (_countRows == 0)
            {
                return Result;
            }
            TotalRetailSaleModel _dataTotal = new TotalRetailSaleModel();
            foreach (TotalRetailSaleModel item in result)
            {
                _dataTotal.Target = 0;
                _dataTotal.TargetName = "Tổng mức bán lẻ hàng hóa và doanh thu dịch vụ";
                _dataTotal.ReportIndex += item.ReportIndex;
                _dataTotal.YORImplement += item.YORImplement;
                _dataTotal.YORAccumulation += item.YORAccumulation;
                _dataTotal.PYImplement += item.PYImplement;
                _dataTotal.PYAccumulation += item.PYAccumulation;
            }
            if (_dataTotal.YORImplement != null && _dataTotal.YORImplement != 0)
            {
                _dataTotal.RatioImplementLastMonth = Math.Round((decimal)((_dataTotal.ReportIndex / _dataTotal.YORImplement) * 100), 2);
            }
            else
            {
                _dataTotal.RatioImplementLastMonth = 0;
            }

            if (_dataTotal.PYImplement != null && _dataTotal.PYImplement != 0)
            {
                _dataTotal.RatioImplementSamePeriod = Math.Round((decimal)((_dataTotal.ReportIndex / _dataTotal.PYImplement) * 100), 2);
            }
            else
            {
                _dataTotal.RatioImplementSamePeriod = 0;
            }

            if (_dataTotal.PYAccumulation != null && _dataTotal.PYAccumulation != 0)
            {
                _dataTotal.RatioAccumulation = Math.Round((decimal)((_dataTotal.YORAccumulation / _dataTotal.PYAccumulation) * 100), 2);
            }
            else
            {
                _dataTotal.RatioAccumulation = 0;
            }
            Result = result.ToList();
            Result.Insert(0,_dataTotal);
            return Result;
        }

        [HttpPost("Export")]
        public IActionResult Export([FromBody] QueryRequestBody query)
        {
            //Query data
            var data = FindData(query);
            // var _data = OkObjectResult(dat)
            if (data == null || data.Count == 0)
            {
                return BadRequest();
            }
            try
            {
                int month = 0;
                int year = 0;
                if (query.Filter != null && query.Filter.ContainsKey("Month") && !string.IsNullOrEmpty(query.Filter["Month"]))
                {
                    string time = query.Filter["Month"];
                    month = Int32.Parse(time.Split("-")[1]);
                    year = Int32.Parse(time.Split("-")[0]);
                }
                else
                {
                    month = DateTime.Now.Month - 1;
                 
                    year = DateTime.Now.Year;

                    if(month == 0)
                    {
                        month = 12;
                        year --;
                    }    
                    
                }

                using (var workbook = new XLWorkbook(@"Upload/Templates/TongMucBanLeHangHoaVaDoanhThuDichVu.xlsx"))
                {
                    IXLWorksheet worksheet = workbook.Worksheets.Worksheet(1);
                    int index = 12;
                    int row = 0;
                    worksheet.Cell(7, 1).Value = $"Tháng {month} năm {year}";
                    //Thêm dữ liệu vào file:
                    foreach (var item in data)
                    {
                        if (row == 0)
                        {
                            worksheet.Cell(index, 2).Value = item.TargetName;
                            worksheet.Cell(index, 3).Value = item.YORImplement == 0 ? "--" : item.YORImplement;
                            worksheet.Cell(index, 4).Value = item.ReportIndex == 0 ? "--" : item.ReportIndex;
                            worksheet.Cell(index, 5).Value = item.YORAccumulation == 0 ? "--" : item.YORAccumulation;
                            worksheet.Cell(index, 6).Value = item.PYImplement == 0 ? "--" : item.PYImplement;
                            worksheet.Cell(index, 7).Value = item.PYAccumulation == 0 ? "--" : item.PYAccumulation ;
                            worksheet.Cell(index, 8).Value = item.RatioImplementLastMonth == 0 ? "--" : item.RatioImplementLastMonth;
                            worksheet.Cell(index, 9).Value = item.RatioImplementSamePeriod == 0 ? "--" : item.RatioImplementSamePeriod;
                            worksheet.Cell(index, 10).Value = item.RatioAccumulation == 0 ? "--" : item.RatioAccumulation;
                            //worksheet.Cell(index, 5).Value = item.Address;
                            index++;
                            row++;
                        }
                        else
                        {
                            var addrow = worksheet.Row(index - 1);
                            addrow.InsertRowsBelow(1);
                            worksheet.Cell(index, 1).Value = row;
                            worksheet.Cell(index, 2).Value = item.TargetName;
                            worksheet.Cell(index, 3).Value = item.YORImplement == 0 ? "--" : item.YORImplement;
                            worksheet.Cell(index, 4).Value = item.ReportIndex == 0 ? "--" : item.ReportIndex;
                            worksheet.Cell(index, 5).Value = item.YORAccumulation == 0 ? "--" : item.YORAccumulation;
                            worksheet.Cell(index, 6).Value = item.PYImplement == 0 ? "--" : item.PYImplement;
                            worksheet.Cell(index, 7).Value = item.PYAccumulation == 0 ? "--" : item.PYAccumulation;
                            worksheet.Cell(index, 8).Value = item.RatioImplementLastMonth == 0 ? "--" : item.RatioImplementLastMonth;
                            worksheet.Cell(index, 9).Value = item.RatioImplementSamePeriod == 0 ? "--" : item.RatioImplementSamePeriod;
                            worksheet.Cell(index, 10).Value = item.RatioAccumulation == 0 ? "--" : item.RatioAccumulation;
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
    }
}
