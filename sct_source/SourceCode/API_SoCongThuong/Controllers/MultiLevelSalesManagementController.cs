
using API_SoCongThuong.Classes;
using API_SoCongThuong.Logger;
using API_SoCongThuong.Models;
using API_SoCongThuong.Reponsitories;
using API_SoCongThuong.Reponsitories.MultiLevelSalesManagementRepository;
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
    public class MultiLevelSalesManagementController : ControllerBase
    {
        private MultiLevelSalesManagementRepo _repo;
        private IConfiguration _configuration;
        private readonly ILogger<AsyncLogger> _logger;
        public SoHoa_SoCongThuongContext _context;
        private AsyncLogger _asyncLogger;
        public MultiLevelSalesManagementController(SoHoa_SoCongThuongContext context, IConfiguration configuration, ILogger<AsyncLogger> logger)
        {
            _repo = new MultiLevelSalesManagementRepo(context);

            _logger = logger;
            _context = context;
            _asyncLogger = new AsyncLogger(_logger, _context);
            _configuration = configuration;
        }

        [Route("find")]
        [HttpPost]
        public IActionResult ListItems_New([FromBody] QueryRequestBody query)//query truyền lên
        {

            BaseModels<MultiLevelSalesManagementModel> model = new BaseModels<MultiLevelSalesManagementModel>();
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

                Func<MultiLevelSalesManagementModel, object> _orderByExpression = x => x.BusinessName; //Khởi tạo mặc định sắp xếp dữ liệu
                Dictionary<string, Func<MultiLevelSalesManagementModel, object>> _sortableFields = new Dictionary<string, Func<MultiLevelSalesManagementModel, object>>   //Khởi tạo các trường để sắp xếp
                    {
                        { "BusinessName", x => x.BusinessName },
                        { "YearReport", x => x.YearReport },
                        { "Turnover", x => x.Turnover },
                        { "Participants", x => x.Participants },
                        { "NewParticipants", x => x.NewParticipants },
                        { "Terminations", x => x.Terminations },
                        { "BasicTrainings", x => x.BasicTrainings },
                        { "Commission", x => x.Commission },
                        { "PromotionalValue", x => x.PromotionalValue },
                        { "TaxDeduction", x => x.TaxDeduction },
                        { "BuyBackGoods", x => x.BuyBackGoods },
                    };
                if (query.Sort != null
                    && !string.IsNullOrEmpty(query.Sort.ColumnName)
                    && _sortableFields.ContainsKey(query.Sort.ColumnName))
                {
                    _orderBy_ASC = ("desc".Equals(query.Sort.Direction.ToLower()) ? false : true);    //Sắp xếp asc hoặc desc
                    _orderByExpression = _sortableFields[query.Sort.ColumnName]; //Trường cần sắp xếp
                }

                IQueryable<MultiLevelSalesManagementModel> _data = from m in _repo._context.MultiLevelSalesManagements.Where(x => !x.IsDel)
                                                                   join bm in _repo._context.BusinessMultiLevels.Where  (x => !x.IsDel)
                                                                   on m.BusinessId equals bm.BusinessMultiLevelId into mbm
                                                                   from bm in mbm.DefaultIfEmpty()
                                                                   join b in _repo._context.Businesses.Where(x => !x.IsDel)
                                                                   on bm.BusinessId equals b.BusinessId into res
                                                                   from r in res.DefaultIfEmpty()
                                                                   select new MultiLevelSalesManagementModel
                                                                   {
                                                                       MultiLevelSalesManagementId = m.MultiLevelSalesManagementId,
                                                                       BusinessName = r.BusinessNameVi,
                                                                       StartDate = m.StartDate,
                                                                       YearReport = m.YearReport,
                                                                       Turnover = m.Turnover,
                                                                       Participants = m.Participants,
                                                                       NewParticipants = m.NewParticipants,
                                                                       Terminations = m.Terminations,
                                                                       BasicTrainings = m.BasicTrainings,
                                                                       Commission = m.Commission,
                                                                       PromotionalValue = m.PromotionalValue,
                                                                       TaxDeduction = m.TaxDeduction,
                                                                       BuyBackGoods = m.BuyBackGoods,
                                                                   };
                //_data = _data.Where(x => !x.IsDel);

                //Search
                if (query.SearchValue != null && query.SearchValue != "") //Kiểm tra điều kiện tìm kiếm
                {
                    _keywordSearch = query.SearchValue.Trim().ToLower();

                    _data = _data.Where(x =>
                        x.BusinessName.ToLower().Contains(_keywordSearch) ||
                        x.Turnover.ToString().Contains(_keywordSearch) ||
                        x.Participants.ToString().Contains(_keywordSearch) ||
                        x.NewParticipants.ToString().Contains(_keywordSearch) ||
                        x.Terminations.ToString().Contains(_keywordSearch) ||
                        x.BasicTrainings.ToString().Contains(_keywordSearch) ||
                        x.Commission.ToString().Contains(_keywordSearch) ||
                        x.PromotionalValue.ToString().Contains(_keywordSearch) ||
                        x.TaxDeduction.ToString().Contains(_keywordSearch) ||
                        x.BuyBackGoods.ToString().Contains(_keywordSearch)
                   );
                }

                if (query.Filter != null && query.Filter.ContainsKey("YearReport")
                    && !string.IsNullOrEmpty(query.Filter["YearReport"]))
                {
                    _data = _data.Where(x => x.YearReport.ToString() == query.Filter["YearReport"]);
                }

                if (query.Filter != null && query.Filter.ContainsKey("MinDate")
                    && !string.IsNullOrEmpty(query.Filter["MinDate"]))
                {
                    _data = _data.Where(x =>
                                (x.StartDate) >=
                                DateTime.ParseExact(query.Filter["MinDate"], "dd/MM/yyyy", null));
                }

                if (query.Filter != null && query.Filter.ContainsKey("MaxDate")
                    && !string.IsNullOrEmpty(query.Filter["MaxDate"]))
                {
                    _data = _data.Where(x =>
                               x.StartDate <=
                                DateTime.ParseExact(query.Filter["MaxDate"], "dd/MM/yyyy", null));
                }

                int _countRows = _data.Count(); //Đếm số dòng của table đã select được
                if (_countRows == 0)    //nếu table = 0 thì trả về không có dữ liệu
                {
                    //model.status = 0;
                    //model.error = new ErrorModel()
                    //{
                    //    Code = ErrCode_Const.EXCEPTION_API,
                    //    Msg = "Không có dữ liệu"
                    //};
                    //return BadRequest(model);
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
            BaseModels<MultiLevelSalesManagementModel> model = new BaseModels<MultiLevelSalesManagementModel>();
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
        public async Task<IActionResult> Update(MultiLevelSalesManagement data)
        {
            BaseModels<MultiLevelSalesManagement> model = new BaseModels<MultiLevelSalesManagement>();
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                SystemLog datalog = new SystemLog();
                var CheckData = _repo._context.MultiLevelSalesManagements.Where(x => x.MultiLevelSalesManagementId == data.MultiLevelSalesManagementId && !x.IsDel);
                if (CheckData.Any())
                {
                    var check = _repo._context.MultiLevelSalesManagements.Where(x => x.BusinessId == data.BusinessId && x.YearReport == data.YearReport && !x.IsDel && x.MultiLevelSalesManagementId != data.MultiLevelSalesManagementId);
                    if (check.Any())
                    {
                        model.status = 0;
                        model.error = new ErrorModel()
                        {
                            Code = ErrCode_Const.PROPERTY_IS_INVALID,
                            Msg = "Báo cáo năm " + data.YearReport.ToString() + " của công ty này đã tồn tại",
                        };
                        datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.UPDATE, NameCategory_Const.MULTI_LEVEL_SALE_MANAGEMENT, Action_Status.FAIL);
                        _asyncLogger.LogInfo(JsonConvert.SerializeObject(datalog));
                        return BadRequest(model);
                    }

                    data.UpdateUserId = loginData.Userid;
                    data.UpdateTime = DateTime.Now;

                    await _repo.Update(data);
                    datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.UPDATE, NameCategory_Const.MULTI_LEVEL_SALE_MANAGEMENT, Action_Status.SUCCESS);
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
        public async Task<IActionResult> create(MultiLevelSalesManagement data)
        {
            BaseModels<MultiLevelSalesManagement> model = new BaseModels<MultiLevelSalesManagement>();
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.USER_NOT_FOUND));
                }
                SystemLog datalog = new SystemLog();
                var check = _repo._context.MultiLevelSalesManagements.Where(x => x.BusinessId == data.BusinessId && x.YearReport == data.YearReport && !x.IsDel);
                if (check.Any())
                {
                    model.status = 0;
                    model.error = new ErrorModel()
                    {
                        Code = ErrCode_Const.PROPERTY_IS_INVALID,
                        Msg = "Báo cáo năm " + data.YearReport.ToString() + " của công ty này đã tồn tại",
                    };
                    datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.CREATE, NameCategory_Const.MULTI_LEVEL_SALE_MANAGEMENT, Action_Status.FAIL);
                    _asyncLogger.LogInfo(JsonConvert.SerializeObject(datalog));
                    return BadRequest(model);
                }

                data.CreateUserId = loginData.Userid;
                data.CreateTime = DateTime.Now;

                await _repo.Insert(data);
                datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.CREATE, NameCategory_Const.MULTI_LEVEL_SALE_MANAGEMENT, Action_Status.SUCCESS);
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
            BaseModels<object> model = new BaseModels<object>();
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                SystemLog datalog = new SystemLog();
                await _repo.Delete(id);
                datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.DELETE, NameCategory_Const.MULTI_LEVEL_SALE_MANAGEMENT, Action_Status.SUCCESS);
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

        private List<MultiLevelSalesManagementModel> FindData([FromBody] QueryRequestBody query)//query truyền lên
        {
            bool _orderBy_ASC = true;  //Khởi tạo sắp xếp dữ liệu acs hoặc desc khi tìm kiếm
            string _keywordSearch = "";
            Func<MultiLevelSalesManagementModel, object> _orderByExpression = x => x.BusinessName; //Khởi tạo mặc định sắp xếp dữ liệu
            Dictionary<string, Func<MultiLevelSalesManagementModel, object>> _sortableFields = new Dictionary<string, Func<MultiLevelSalesManagementModel, object>>   //Khởi tạo các trường để sắp xếp
                    {
                        { "BusinessName", x => x.BusinessName },
                        { "YearReport", x => x.YearReport },
                        { "Turnover", x => x.Turnover },
                        { "Participants", x => x.Participants },
                        { "NewParticipants", x => x.NewParticipants },
                        { "Terminations", x => x.Terminations },
                        { "BasicTrainings", x => x.BasicTrainings },
                        { "Commission", x => x.Commission },
                        { "PromotionalValue", x => x.PromotionalValue },
                        { "TaxDeduction", x => x.TaxDeduction },
                        { "BuyBackGoods", x => x.BuyBackGoods },
                    };
            if (query.Sort != null
                && !string.IsNullOrEmpty(query.Sort.ColumnName)
                && _sortableFields.ContainsKey(query.Sort.ColumnName))
            {
                _orderBy_ASC = ("desc".Equals(query.Sort.Direction.ToLower()) ? false : true);    //Sắp xếp asc hoặc desc
                _orderByExpression = _sortableFields[query.Sort.ColumnName]; //Trường cần sắp xếp
            }

            IQueryable<MultiLevelSalesManagementModel> _data = from m in _repo._context.MultiLevelSalesManagements.Where(x => !x.IsDel)
                                                               join bm in _repo._context.BusinessMultiLevels.Where(x => !x.IsDel)
                                                               on m.BusinessId equals bm.BusinessMultiLevelId into mbm
                                                               from bm in mbm.DefaultIfEmpty()
                                                               join b in _repo._context.Businesses.Where(x => !x.IsDel)
                                                               on bm.BusinessId equals b.BusinessId into res
                                                               from r in res.DefaultIfEmpty()
                                                               select new MultiLevelSalesManagementModel
                                                               {
                                                                   MultiLevelSalesManagementId = m.MultiLevelSalesManagementId,
                                                                   BusinessName = r.BusinessNameVi,
                                                                   StartDate = m.StartDate,
                                                                   YearReport = m.YearReport,
                                                                   Turnover = m.Turnover,
                                                                   Participants = m.Participants,
                                                                   NewParticipants = m.NewParticipants,
                                                                   Terminations = m.Terminations,
                                                                   BasicTrainings = m.BasicTrainings,
                                                                   Commission = m.Commission,
                                                                   PromotionalValue = m.PromotionalValue,
                                                                   TaxDeduction = m.TaxDeduction,
                                                                   BuyBackGoods = m.BuyBackGoods,
                                                               };

            if (query.SearchValue != null && query.SearchValue != "") //Kiểm tra điều kiện tìm kiếm
            {
                _keywordSearch = query.SearchValue.Trim().ToLower();
                _data = _data.Where(x =>
                    x.BusinessName.ToLower().Contains(_keywordSearch) ||
                    x.Turnover.ToString().Contains(_keywordSearch) ||
                    x.Participants.ToString().Contains(_keywordSearch) ||
                    x.NewParticipants.ToString().Contains(_keywordSearch) ||
                    x.Terminations.ToString().Contains(_keywordSearch) ||
                    x.BasicTrainings.ToString().Contains(_keywordSearch) ||
                    x.Commission.ToString().Contains(_keywordSearch) ||
                    x.PromotionalValue.ToString().Contains(_keywordSearch) ||
                    x.TaxDeduction.ToString().Contains(_keywordSearch) ||
                    x.BuyBackGoods.ToString().Contains(_keywordSearch)
               );
            }

            if (query.Filter != null && query.Filter.ContainsKey("YearReport")
                && !string.IsNullOrEmpty(query.Filter["YearReport"]))
            {
                _data = _data.Where(x => x.YearReport.ToString() == query.Filter["YearReport"]);
            }

            if (query.Filter != null && query.Filter.ContainsKey("MinDate")
                && !string.IsNullOrEmpty(query.Filter["MinDate"]))
            {
                _data = _data.Where(x =>
                            (x.StartDate) >=
                            DateTime.ParseExact(query.Filter["MinDate"], "dd/MM/yyyy", null));
            }

            if (query.Filter != null && query.Filter.ContainsKey("MaxDate")
                && !string.IsNullOrEmpty(query.Filter["MaxDate"]))
            {
                _data = _data.Where(x =>
                           x.StartDate <=
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

            try
            {
                using (var workbook = new XLWorkbook(@"Upload/Templates/Danhsachcosohoatdongbanhangdacap.xlsx"))
                {
                    IXLWorksheet worksheet = workbook.Worksheets.Worksheet(1);
                    int index = 5;
                    int row = 1;

                    //Thêm dữ liệu vào file:
                    foreach (var item in data)
                    {
                        if (row == 1)
                        {
                            worksheet.Cell(index, 1).Value = row;
                            worksheet.Cell(index, 2).Value = item.BusinessName;
                            worksheet.Cell(index, 3).Value = item.Turnover;
                            worksheet.Cell(index, 4).Value = item.Participants;
                            worksheet.Cell(index, 5).Value = item.NewParticipants;
                            worksheet.Cell(index, 6).Value = item.Terminations;
                            worksheet.Cell(index, 7).Value = item.BasicTrainings;
                            worksheet.Cell(index, 8).Value = item.Commission;
                            worksheet.Cell(index, 9).Value = item.PromotionalValue ?? 0;
                            worksheet.Cell(index, 10).Value = item.TaxDeduction ?? 0;
                            worksheet.Cell(index, 11).Value = item.BuyBackGoods ?? 0;
                            index++;
                            row++;
                        }
                        else
                        {
                            var addrow = worksheet.Row(index - 1);
                            addrow.InsertRowsBelow(1);
                            worksheet.Cell(index, 1).Value = row;
                            worksheet.Cell(index, 2).Value = item.BusinessName;
                            worksheet.Cell(index, 3).Value = item.Turnover;
                            worksheet.Cell(index, 4).Value = item.Participants;
                            worksheet.Cell(index, 5).Value = item.NewParticipants;
                            worksheet.Cell(index, 6).Value = item.Terminations;
                            worksheet.Cell(index, 7).Value = item.BasicTrainings;
                            worksheet.Cell(index, 8).Value = item.Commission;
                            worksheet.Cell(index, 9).Value = item.PromotionalValue ?? 0;
                            worksheet.Cell(index, 10).Value = item.TaxDeduction ?? 0;
                            worksheet.Cell(index, 11).Value = item.BuyBackGoods ?? 0;
                            index++;
                            row++;
                        }
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

        [HttpGet("getBusinessMultiLevel")]
        public IActionResult getBusinessMultiLevel()
        {
            BaseModels<BusinessMultiLevelModel> model = new BaseModels<BusinessMultiLevelModel>();
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                var result = from m in _repo._context.BusinessMultiLevels.Where(x => !x.IsDel)
                             join b in _repo._context.Businesses on m.BusinessId equals b.BusinessId
                             join cate in _repo._context.Categories on m.Status equals cate.CategoryId
                             join d in _repo._context.Districts on  b.DistrictId equals d.DistrictId
                             select new BusinessMultiLevelModel()
                             {
                                 BusinessMultiLevelId = m.BusinessMultiLevelId,
                                 BusinessName = b.BusinessNameVi,
                                 BusinessId = m.BusinessId,
                                 BusinessCode = b.BusinessCode,
                                 NumCert = m.NumCert,
                                 CertDate = m.CertDate,
                                 CertExp = m.CertExp,
                                 Goods = m.Goods,
                                 Address = m.Address,
                                 StartDate = m.StartDate,
                                 Status = m.Status,
                                 DistrictId = m.DistrictId,
                                 Contact = m.Contact,
                                 AddressContact = m.AddressContact,
                                 PhoneNumber = m.PhoneNumber,
                                 StatusName = cate.CategoryName,
                                 DistrictName = d.DistrictName,
                                 Note = m.Note,
                                 LocalConfirm = m.LocalConfirm
                             };
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
    }

}
