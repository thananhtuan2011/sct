using API_SoCongThuong.Classes;
using API_SoCongThuong.Logger;
using API_SoCongThuong.Models;
using API_SoCongThuong.Reponsitories;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Office2019.Drawing.Diagram11;
using EF_Core.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Data;

namespace API_SoCongThuong.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecordsManagerController : ControllerBase
    {
        private RecordsManagerRepo _repo;

        private IConfiguration _configuration;
        private readonly ILogger<AsyncLogger> _logger;
        public SoHoa_SoCongThuongContext _context;
        private AsyncLogger _asyncLogger;


        public RecordsManagerController(SoHoa_SoCongThuongContext context, IConfiguration configuration, ILogger<AsyncLogger> logger)
        {
            _repo = new RecordsManagerRepo(context);

            _logger = logger;
            _context = context;
            _asyncLogger = new AsyncLogger(_logger, _context);
            _configuration = configuration;
        }

        [Route("find")]
        [HttpPost]
        public IActionResult ListItems_New([FromBody] QueryRequestBody query)//query truyền lên
        {

            BaseModels<RecordsManagerModel> model = new BaseModels<RecordsManagerModel>();
            string _keywordSearch = ""; //Keyword tìm kiếm
            bool _orderBy_ASC = true;  //Khởi tạo sắp xếp dữ liệu acs hoặc desc khi tìm kiếm
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                Func<RecordsManagerModel, object> _orderByExpression = x => x.CodeFile; //Khởi tạo mặc định sắp xếp dữ liệu
                Dictionary<string, Func<RecordsManagerModel, object>> _sortableFields = new Dictionary<string, Func<RecordsManagerModel, object>>
                {
                    { "CodeFile", x => x.CodeFile },
                    { "RecordsGroup" , x => x.RecordsFinancePlan },
                    { "Title", x => x.Title },
                    { "StorageTime" , x => x.StorageTime },
                    { "ReceptionTime" , x => x.ReceptionTime}
                };//Khởi tạo các trường để sắp xếp
                IQueryable<RecordsManagerModel> _data = _repo._context.RecordsManagers.Where(x => !x.IsDel).Select(x => new RecordsManagerModel
                {
                    RecordsManagerId = x.RecordsManagerId,
                    RecordsFinancePlanId = x.RecordsFinancePlanId,
                    CodeFile = x.CodeFile,
                    Title = x.Title,
                    ReceptionTime = x.ReceptionTime,
                    StorageTime = x.StorageTime,
                    Creator = x.Creator,
                    Note = x.Note,
                    RecordsFinancePlan = _repo._context.RecordsFinancePlans.Where(d =>!d.IsDel && d.RecordsFinancePlanId == x.RecordsFinancePlanId).Select(d => d.Name).FirstOrDefault()
                });

                if (query.Sort != null
                   && !string.IsNullOrEmpty(query.Sort.ColumnName)
                   && _sortableFields.ContainsKey(query.Sort.ColumnName))
                {
                    _orderBy_ASC = ("desc".Equals(query.Sort.Direction.ToLower()) ? false : true);    //Sắp xếp asc hoặc desc
                    _orderByExpression = _sortableFields[query.Sort.ColumnName]; //Trường cần sắp xếp
                }
                if (query.SearchValue != null && query.SearchValue != "") //Kiểm tra điều kiện tìm kiếm
                {
                    _keywordSearch = query.SearchValue.Trim().ToLower();
                    _data = _data.Where(x =>
                       /* x.DistrictId.ToString().ToLower().Contains(_keywordSearch)
                        || */
                       x.CodeFile.ToLower().Contains(_keywordSearch) || x.RecordsFinancePlan.ToLower().Contains(_keywordSearch) || x.Title.ToLower().Contains(_keywordSearch)
                   );  //Lấy table đã select tìm kiếm theo keyword
                }
                if (query.Filter != null && query.Filter.ContainsKey("RecordsFinancePlan") && !string.IsNullOrEmpty(query.Filter["RecordsFinancePlan"]))
                {
                    _data = _data.Where(x => x.RecordsFinancePlanId == Guid.Parse(query.Filter["RecordsFinancePlan"]));
                }
                //if (query.Filter != null && query.Filter.ContainsKey("Year") && !string.IsNullOrEmpty(query.Filter["Year"]))
                //{
                //    _data = _data.Where(x => x.ReceptionTime.Year == Int32.Parse((query.Filter["Year"])));
                //}
                if (query.Filter != null && query.Filter.ContainsKey("MinDate")
                   && !string.IsNullOrEmpty(query.Filter["MinDate"]))
                {
                    _data = _data.Where(x =>
                                (x.ReceptionTime) >=
                                DateTime.ParseExact(query.Filter["MinDate"], "dd/MM/yyyy", null));
                }

                if (query.Filter != null && query.Filter.ContainsKey("MaxDate")
                    && !string.IsNullOrEmpty(query.Filter["MaxDate"]))
                {
                    _data = _data.Where(x =>
                               x.ReceptionTime <=
                                DateTime.ParseExact(query.Filter["MaxDate"], "dd/MM/yyyy", null));
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

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(RecordsManagerModel data)
        {
            BaseModels<RecordsManagerModel> model = new BaseModels<RecordsManagerModel>();
            try
            {
              //  var ipAddress = Ulities.GetIPAddress(HttpContext);
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                //int count = _repo._context.RecordsManagers.Where(x => !x.IsDel && x.RecordsManagerId != data.RecordsManagerId && x.Code == data.Code).Count();
                //if (count > 0)
                //{
                //    model.status = 0;
                //    model.error = new ErrorModel()
                //    {
                //        Code = ErrCode_Const.EXCEPTION_API,
                //        Msg = "Mã nhóm hồ sơ đã tồn tại!"
                //    };
                //    return Ok(model);
                //}
                SystemLog datalog = new SystemLog();
                RecordsManager? _data = _repo._context.RecordsManagers.Where(x => !x.IsDel && x.RecordsManagerId == data.RecordsManagerId).FirstOrDefault();
                if (_data != null)
                {
                    _data.RecordsManagerId = data.RecordsManagerId;
                    _data.RecordsFinancePlanId = data.RecordsFinancePlanId;
                    _data.CodeFile = data.CodeFile; ;
                    _data.Title = data.Title; ;
                    _data.ReceptionTime = data.ReceptionTime;
                    _data.StorageTime = data.StorageTime; ;
                    _data.Creator = data.Creator; ;
                    _data.Note = data.Note;
                    _data.UpdateTime = DateTime.Now;
                    _data.UpdateUserId = loginData.Userid;
                    await _repo.Update(_data);
                    datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.UPDATE, NameCategory_Const.RECORDS_MANAGER, Action_Status.SUCCESS);
                    _asyncLogger.LogInfo(JsonConvert.SerializeObject(datalog));
                }

                model.status = 1;
        //        SystemLog datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username , ActionType_Const.UPDATE, "Update KHTC", "Thành công");
        //        _asyncLogger.LogInfo(JsonConvert.SerializeObject(datalog));
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
        public async Task<IActionResult> create(RecordsManagerModel data)
        {
            BaseModels<RecordsManagerModel> model = new BaseModels<RecordsManagerModel>();
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                //int count = _repo._context.RecordsManager.Where(x => !x.IsDel && x.Code == data.Code).Count();
                //if (count > 0)
                //{
                //    model.status = 0;
                //    model.error = new ErrorModel()
                //    {
                //        Code = ErrCode_Const.EXCEPTION_API,
                //        Msg = "Mã nhóm hồ sơ đã tồn tại!"
                //    };
                //    return Ok(model);
                //}
                SystemLog datalog = new SystemLog();
                RecordsManager SaveData = new RecordsManager();
                SaveData.RecordsFinancePlanId = data.RecordsFinancePlanId;
                SaveData.CodeFile = data.CodeFile;;
                SaveData.Title = data.Title;;
                SaveData.ReceptionTime = data.ReceptionTime;
                SaveData.StorageTime = data.StorageTime;;
                SaveData.Creator = data.Creator;;
                SaveData.Note = data.Note;
                SaveData.CreateTime = DateTime.Now;
                SaveData.CreateUserId = loginData.Userid;
                await _repo.Insert(SaveData);
                datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.CREATE, NameCategory_Const.RECORDS_MANAGER, Action_Status.SUCCESS);
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
            BaseModels<RecordsManagerModel> model = new BaseModels<RecordsManagerModel>();
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                SystemLog datalog = new SystemLog();
                RecordsManager item = new RecordsManager();
                item.RecordsManagerId = id;
                item.IsDel = true;
                await _repo.Delete(item);
                datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.DELETE, NameCategory_Const.RECORDS_MANAGER, Action_Status.SUCCESS);
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

        private List<RecordsManagerModel> FindData([FromBody] QueryRequestBody query)//query truyền lên
        {
            bool _orderBy_ASC = true;  //Khởi tạo sắp xếp dữ liệu acs hoặc desc khi tìm kiếm
            string _keywordSearch = "";
            Func<RecordsManagerModel, object> _orderByExpression = x => x.CodeFile; //Khởi tạo mặc định sắp xếp dữ liệu
            Dictionary<string, Func<RecordsManagerModel, object>> _sortableFields = new Dictionary<string, Func<RecordsManagerModel, object>>
            {
                { "CodeFile", x => x.CodeFile },
                { "RecordsGroup" , x => x.RecordsFinancePlan },
                { "Title", x => x.Title },
                { "StorageTime" , x => x.StorageTime },
                { "ReceptionTime" , x => x.ReceptionTime}
            };//Khởi tạo các trường để sắp xếp
            IQueryable<RecordsManagerModel> _data = _repo._context.RecordsManagers.Where(x => !x.IsDel).Select(x => new RecordsManagerModel
            {
                RecordsManagerId = x.RecordsManagerId,
                RecordsFinancePlanId = x.RecordsFinancePlanId,
                CodeFile = x.CodeFile,
                Title = x.Title,
                ReceptionTime = x.ReceptionTime,
                StorageTime = x.StorageTime,
                Creator = x.Creator,
                Note = x.Note,
                RecordsFinancePlan = _repo._context.RecordsFinancePlans.Where(d => !d.IsDel && d.RecordsFinancePlanId == x.RecordsFinancePlanId).Select(d => d.Name).FirstOrDefault()
            });

            if (query.Sort != null
                && !string.IsNullOrEmpty(query.Sort.ColumnName)
                && _sortableFields.ContainsKey(query.Sort.ColumnName))
            {
                _orderBy_ASC = ("desc".Equals(query.Sort.Direction.ToLower()) ? false : true);    //Sắp xếp asc hoặc desc
                _orderByExpression = _sortableFields[query.Sort.ColumnName]; //Trường cần sắp xếp
            }
            if (query.SearchValue != null && query.SearchValue != "") //Kiểm tra điều kiện tìm kiếm
            {
                _keywordSearch = query.SearchValue.Trim().ToLower();
                _data = _data.Where(x =>
                   /* x.DistrictId.ToString().ToLower().Contains(_keywordSearch)
                    || */
                   x.CodeFile.ToLower().Contains(_keywordSearch) || x.RecordsFinancePlan.ToLower().Contains(_keywordSearch) || x.Title.ToLower().Contains(_keywordSearch)
               );  //Lấy table đã select tìm kiếm theo keyword
            }
            if (query.Filter != null && query.Filter.ContainsKey("RecordsFinancePlan") && !string.IsNullOrEmpty(query.Filter["RecordsFinancePlan"]))
            {
                _data = _data.Where(x => x.RecordsFinancePlanId == Guid.Parse(query.Filter["RecordsFinancePlan"]));
            }
            if (query.Filter != null && query.Filter.ContainsKey("MinDate")
            && !string.IsNullOrEmpty(query.Filter["MinDate"]))
            {
                _data = _data.Where(x =>
                            (x.ReceptionTime) >=
                            DateTime.ParseExact(query.Filter["MinDate"], "dd/MM/yyyy", null));
            }

            if (query.Filter != null && query.Filter.ContainsKey("MaxDate")
                && !string.IsNullOrEmpty(query.Filter["MaxDate"]))
            {
                _data = _data.Where(x =>
                           x.ReceptionTime <=
                            DateTime.ParseExact(query.Filter["MaxDate"], "dd/MM/yyyy", null));
            }

            return _data.ToList();
        }

        [HttpPost("Export")]
        public IActionResult Export([FromBody] QueryRequestBody query)
        {
            var data = FindData(query);

            if (!data.Any())
            {
                return BadRequest();
            }

            int currentYear = DateTime.Now.Year;

            if (query.Filter != null && query.Filter.ContainsKey("Year") && !string.IsNullOrEmpty(query.Filter["Year"]))
            {
                currentYear = int.Parse(query.Filter["Year"]);
            }
            string minDate = "";
            string maxDate = "";
            if (query.Filter != null && query.Filter.ContainsKey("MinDate")
                && !string.IsNullOrEmpty(query.Filter["MinDate"]))
            {
                minDate = query.Filter["MinDate"];
            }

            if (query.Filter != null && query.Filter.ContainsKey("MaxDate")
                && !string.IsNullOrEmpty(query.Filter["MaxDate"]))
            {
                maxDate = query.Filter["MaxDate"];
            }
            else
            {
                maxDate = DateTime.Now.ToString("dd/MM/yyyy");
            }

            try
            {
                var result = data.GroupBy(x => x.RecordsFinancePlanId).ToList();
                using (var workbook = new XLWorkbook(@"Upload/Templates/QuanLyHoSoLuuTruPhongKHTC.xlsx"))
                {
                    IXLWorksheet worksheet = workbook.Worksheets.Worksheet(1);
                    worksheet.Cell(1, 1).Value = $"DANH MỤC HỒ SƠ LƯU CỦA PHÒNG KẾ HOẠCH TÀI CHÍNH TỔNG HỢP NĂM TỪ {minDate} ĐẾN {maxDate}";

                    int index = 4;
                    int row = 1;
                    int indexTitle = 1;
                    foreach (var item in result)
                    {
                        int i = 0;
                        foreach (var it in item)
                        {
                            if(i == 0)
                            {
                                var _addrow = worksheet.Row(index - 1);
                                _addrow.InsertRowsBelow(1);
                                worksheet.Cell(index, 2).Value = $"{indexTitle}. {it.RecordsFinancePlan}";
                                indexTitle++;
                                i++;
                                index++;
                                row++;
                            }
                            var addrow = worksheet.Row(index - 1);
                            addrow.InsertRowsBelow(1);
                            it.Creator = it.Creator.Replace(",", "\r\n");
                            worksheet.Cell(index, 1).Value = it.CodeFile;
                            worksheet.Cell(index, 2).Value = it.Title;
                            worksheet.Cell(index, 3).Value = it.StorageTime;
                            worksheet.Cell(index, 4).Value = it.Creator ;
                            worksheet.Cell(index, 5).Value = it.Note;
                            index++;
                            row ++;
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
