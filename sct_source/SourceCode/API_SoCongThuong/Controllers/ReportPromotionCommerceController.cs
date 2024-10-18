using DpsLibs.Web;
using EF_Core.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.Design;
using API_SoCongThuong.Classes;
using API_SoCongThuong.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using API_SoCongThuong.Reponsitories;
using Microsoft.EntityFrameworkCore.Internal;
using System.Net;
using ClosedXML.Excel;
using Newtonsoft.Json;
using System.Data;
using API_SoCongThuong.Logger;

namespace API_SoCongThuong.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportPromotionCommerceController : ControllerBase
    {
        private ReportPromotionCommerceRepo _repo;
        private IConfiguration _configuration;
        private readonly ILogger<AsyncLogger> _logger;
        public SoHoa_SoCongThuongContext _context;
        private AsyncLogger _asyncLogger;

        public ReportPromotionCommerceController(SoHoa_SoCongThuongContext context, IConfiguration configuration, ILogger<AsyncLogger> logger)
        {
            _repo = new ReportPromotionCommerceRepo(context);

            _logger = logger;
            _context = context;
            _asyncLogger = new AsyncLogger(_logger, _context);
            _configuration = configuration;

        }
        // Lấy danh sách 
        #region 
        [Route("find")]
        [HttpPost]
        public IActionResult ListItems_New([FromBody] QueryRequestBody query)//query truyền lên
        {

            BaseModels<ReportPromotionCommerceModel> model = new BaseModels<ReportPromotionCommerceModel>();
            string _keywordSearch = ""; //Keyword tìm kiếm
            bool _orderBy_ASC = false;  //Khởi tạo sắp xếp dữ liệu acs hoặc desc khi tìm kiếm
            try
            {
                ////Lấy Token, lấy model
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }

                Func<ReportPromotionCommerceModel, object> _orderByExpression = x => x.ReportPromotionCommerceId; //Khởi tạo mặc định sắp xếp dữ liệu
                Dictionary<string, Func<ReportPromotionCommerceModel, object>> _sortableFields = new Dictionary<string, Func<ReportPromotionCommerceModel, object>>   //Khởi tạo các trường để sắp xếp
                    {
                        { "ProjectName", x => x.ProjectName },
                        { "Host", x => x.Host },
                        { "StartTimeDisplay", x => x.StartTime },
                        { "Quantity", x => x.Location },
                        { "CreateName", x => x.CreateUserId },
                        { "CreateTimeDisplay", x => x.CreateTime },
                    };
                if (query.Sort != null
                    && !string.IsNullOrEmpty(query.Sort.ColumnName)
                    && _sortableFields.ContainsKey(query.Sort.ColumnName))
                {
                    _orderBy_ASC = ("desc".Equals(query.Sort.Direction.ToLower()) ? false : true);    //Sắp xếp asc hoặc desc
                    _orderByExpression = _sortableFields[query.Sort.ColumnName]; //Trường cần sắp xếp
                }

                IQueryable<ReportPromotionCommerceModel> _data = _repo._context.ReportPromotionCommerces.Where(x => !x.IsDel).Select(d=>
                    new ReportPromotionCommerceModel
                     {
                         ReportPromotionCommerceId = d.ReportPromotionCommerceId,
                         ProjectName=d.ProjectName,
                         Location=d.Location,
                         //StartTimeDisplay=string.Format("{0:dd/MM/yyyy}", d.StartTime),
                         StartTimeDisplay= d.StartTime.ToString("dd/MM/yyyy") ,
                         Host=d.Host,
                         StartTime=d.StartTime
                    });

                if (query.SearchValue != null && query.SearchValue != "") //Kiểm tra điều kiện tìm kiếm
                {
                    _keywordSearch = query.SearchValue.Trim().ToLower();
                    _data = _data.Where(x => x.ProjectName.ToLower().Contains(_keywordSearch)
                    || x.Location.Contains(_keywordSearch)
                    //|| x.StartTime.ToString().Contains(_keywordSearch)
                    //|| x.StartTimeDisplay.Contains(_keywordSearch)
                    || x.Host.Contains(_keywordSearch)
                    );  //Lấy table đã select tìm kiếm theo keyword
                }
                // model.items = _data.ToList();

                int _countRows = _data.Count(); //Đếm số dòng của table đã select được
                if (_countRows == 0)    //nếu table = 0 thì trả về không có dữ liệu
                {
                    return NotFound("Không có dữ liệu");
                }
                if (query.Filter != null && query.Filter.ContainsKey("MinTime")
              && !string.IsNullOrEmpty(query.Filter["MinTime"]))
                {
                    _data = _data.Where(x =>
                                (x.StartTime) >=
                                DateTime.ParseExact(query.Filter["MinTime"], "dd/MM/yyyy", null));
                }

                if (query.Filter != null && query.Filter.ContainsKey("MaxTime")
                    && !string.IsNullOrEmpty(query.Filter["MaxTime"]))
                {
                    _data = _data.Where(x =>
                               x.StartTime <=
                                DateTime.ParseExact(query.Filter["MaxTime"], "dd/MM/yyyy", null));
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
                ////Đoạn này lấy total đã tối ưu việc call DB nhiều lần
                //var listId = model.items.Select(x => x.CateCriteriaId).ToList();
                //var listTotal = _repo._context.CateCriteria.Where(x => listId.Contains(x.CateCriteriaId)).Select(x =>
                // new CateCriteriaModel
                // {
                //     CateCriteriaId = x.CateCriteriaId
                // }).ToList();
                //for (int i = 0; i < model.items.Count(); i++)
                //{
                //    int tt = listTotal.Where(x => x.CateCriterionId == model.items[i].CateCriterionId).Select(x => x.TotalStore).FirstOrDefault(0);
                //    model.items[i].TotalStore = tt;
                //}
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
        #endregion

        [HttpPost()]
        public async Task<IActionResult> create(ReportPromotionCommerceModel data)
        {
            BaseModels<ReportPromotionCommerceModel> model = new BaseModels<ReportPromotionCommerceModel>();
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
                if (string.IsNullOrEmpty(data.Host))
                {
                    model.status = 0;
                    model.error = new ErrorModel()
                    {
                        Code = ErrCode_Const.PROPERTY_IS_NULL_OR_EMPTY
                    };
                    datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.CREATE, NameCategory_Const.REPORT_PROMOTION_COMMERCE, Action_Status.FAIL);
                    _asyncLogger.LogInfo(JsonConvert.SerializeObject(datalog));
                    return BadRequest(model);
                }
                if (string.IsNullOrEmpty(data.ProjectName))
                {
                    model.status = 0;
                    model.error = new ErrorModel()
                    {
                        Code = ErrCode_Const.PROPERTY_IS_NULL_OR_EMPTY
                    };
                    datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.CREATE, NameCategory_Const.REPORT_PROMOTION_COMMERCE, Action_Status.FAIL);
                    _asyncLogger.LogInfo(JsonConvert.SerializeObject(datalog));
                    return BadRequest(model);
                }

                data.CreateTime = DateTime.Now;
                data.CreateUserId = loginData.Userid;
                await _repo.Insert(data);
                datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.CREATE, NameCategory_Const.REPORT_PROMOTION_COMMERCE, Action_Status.SUCCESS);
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
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(ReportPromotionCommerceModel data)
        {
            BaseModels<ReportPromotionCommerceModel> model = new BaseModels<ReportPromotionCommerceModel>();
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
                var CheckData = _repo.FindById(data.ReportPromotionCommerceId);
                if (CheckData == null)
                {
                    //chỗ này không tồn tại id
                    model.status = 0;
                    model.error = new ErrorModel()
                    {
                        Code = ErrCode_Const.PROPERTY_IS_NULL_OR_EMPTY
                    };
                    datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.UPDATE, NameCategory_Const.REPORT_PROMOTION_COMMERCE, Action_Status.FAIL);
                    _asyncLogger.LogInfo(JsonConvert.SerializeObject(datalog));
                    return BadRequest(model);
                }
                else
                {
                    if (string.IsNullOrEmpty(data.Host))
                    {
                        model.status = 0;
                        model.error = new ErrorModel()
                        {
                            Code = ErrCode_Const.PROPERTY_IS_NULL_OR_EMPTY
                        };
                        datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.UPDATE, NameCategory_Const.REPORT_PROMOTION_COMMERCE, Action_Status.FAIL);
                        _asyncLogger.LogInfo(JsonConvert.SerializeObject(datalog));
                        return BadRequest(model);
                    }
                    if (string.IsNullOrEmpty(data.ProjectName))
                    {
                        model.status = 0;
                        model.error = new ErrorModel()
                        {
                            Code = ErrCode_Const.PROPERTY_IS_NULL_OR_EMPTY
                        };
                        datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.UPDATE, NameCategory_Const.REPORT_PROMOTION_COMMERCE, Action_Status.FAIL);
                        _asyncLogger.LogInfo(JsonConvert.SerializeObject(datalog));
                        return BadRequest(model);
                    }

                    data.UpdateTime = DateTime.Now;
                    data.UpdateUserId = loginData.Userid;
                    await _repo.Update(data);
                    datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.UPDATE, NameCategory_Const.REPORT_PROMOTION_COMMERCE, Action_Status.SUCCESS);
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

        [HttpGet("{id}")]
        public IActionResult getItemById(Guid id)
        {
            BaseModels<ReportPromotionCommerceModel> model = new BaseModels<ReportPromotionCommerceModel>();
            try
            {
                var info = _repo.FindById(id);
                //var storelist = _repo.FindStoreId(id).Select(x => x.CateCriterionId).ToList().ToString();
                if (info == null)
                    return NotFound(ErrMsg_Const.GetMsg(ErrCode_Const.CANNOT_FIND_DATA_BY_QUERY));

                //Set data cho base model
                model.status = 1;
                model.data = info;
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
        //[Route("deletes")]
        //[HttpPut()]
        //public async Task<IActionResult> deletes(List<Guid> IdRemoves)
        //{
        //    BaseModels<object> model = new BaseModels<object>();
        //    try
        //    {
        //        await _repo.Deletes(IdRemoves);
        //        model.status = 1;
        //        return Ok(model);
        //    }
        //    catch (Exception ex)
        //    {

        //        model.status = 0;
        //        model.error = new ErrorModel()
        //        {
        //            Code = ErrCode_Const.EXCEPTION_API,
        //            Msg = ex.Message
        //        };
        //        return BadRequest(model);
        //    }
        //}
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
                datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.DELETE, NameCategory_Const.REPORT_PROMOTION_COMMERCE, Action_Status.SUCCESS);
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

        private List<ReportPromotionCommerceModel> FindData([FromBody] QueryRequestBody query)
        {
            string _keywordSearch = ""; //Keyword tìm kiếm
            bool _orderBy_ASC = true;  //Khởi tạo sắp xếp dữ liệu acs hoặc desc khi tìm kiếm
            Func<ReportPromotionCommerceModel, object> _orderByExpression = x => x.ReportPromotionCommerceId; //Khởi tạo mặc định sắp xếp dữ liệu
            Dictionary<string, Func<ReportPromotionCommerceModel, object>> _sortableFields = new Dictionary<string, Func<ReportPromotionCommerceModel, object>>   //Khởi tạo các trường để sắp xếp
                    {
                        { "ProjectName", x => x.ProjectName },
                        { "Host", x => x.Host },
                        { "StartTimeDisplay", x => x.StartTime },
                        { "Quantity", x => x.Location },
                        { "CreateName", x => x.CreateUserId },
                        { "CreateTimeDisplay", x => x.CreateTime },
                    };
            if (query.Sort != null
                && !string.IsNullOrEmpty(query.Sort.ColumnName)
                && _sortableFields.ContainsKey(query.Sort.ColumnName))
            {
                _orderBy_ASC = ("desc".Equals(query.Sort.Direction.ToLower()) ? false : true);    //Sắp xếp asc hoặc desc
                _orderByExpression = _sortableFields[query.Sort.ColumnName]; //Trường cần sắp xếp
            }

            IQueryable<ReportPromotionCommerceModel> _data = _repo._context.ReportPromotionCommerces.Where(x => !x.IsDel).Select(d =>
                new ReportPromotionCommerceModel
                {
                    ReportPromotionCommerceId = d.ReportPromotionCommerceId,
                    ProjectName = d.ProjectName,
                    Location = d.Location,
                    //StartTimeDisplay=string.Format("{0:dd/MM/yyyy}", d.StartTime),
                    StartTimeDisplay = d.StartTime.ToString("dd/MM/yyyy"),
                    Host = d.Host,
                    StartTime = d.StartTime
                });

            if (query.SearchValue != null && query.SearchValue != "") //Kiểm tra điều kiện tìm kiếm
            {
                _keywordSearch = query.SearchValue.Trim().ToLower();
                _data = _data.Where(x => x.ProjectName.ToLower().Contains(_keywordSearch)
                || x.Location.Contains(_keywordSearch)
                //|| x.StartTime.ToString().Contains(_keywordSearch)
                //|| x.StartTimeDisplay.Contains(_keywordSearch)
                || x.Host.Contains(_keywordSearch)
                );  //Lấy table đã select tìm kiếm theo keyword
            }
            // model.items = _data.ToList();

            if (query.Filter != null && query.Filter.ContainsKey("MinTime")
          && !string.IsNullOrEmpty(query.Filter["MinTime"]))
            {
                _data = _data.Where(x =>
                            (x.StartTime) >=
                            DateTime.ParseExact(query.Filter["MinTime"], "dd/MM/yyyy", null));
            }

            if (query.Filter != null && query.Filter.ContainsKey("MaxTime")
                && !string.IsNullOrEmpty(query.Filter["MaxTime"]))
            {
                _data = _data.Where(x =>
                           x.StartTime <=
                            DateTime.ParseExact(query.Filter["MaxTime"], "dd/MM/yyyy", null));
            }

            return _data.ToList();
        }

        [HttpPost("Export")]
        public IActionResult Export([FromBody] QueryRequestBody query)
        {
            var data = FindData(query);

            if (!data.Any() || data.Count == 0)
            {
                return BadRequest();
            }

            try
            {
                using (var workbook = new XLWorkbook(@"Upload/Templates/QuanLyBaoCaoHoatDongXucTienThuongMai.xlsx"))
                {
                    IXLWorksheet worksheet = workbook.Worksheets.Worksheet(1);

                    int index = 5;
                    int row = 1;
                    foreach (var item in data)
                    {
                        var addrow = worksheet.Row(index - 1);
                        addrow.InsertRowsBelow(1);

                        worksheet.Cell(index, 1).Value = row;
                        worksheet.Cell(index, 2).Value = item.ProjectName;
                        worksheet.Cell(index, 3).Value = item.Host;
                        worksheet.Cell(index, 4).Value = item.StartTimeDisplay;
                        worksheet.Cell(index, 5).Value = item.Location;

                        index++;
                        row++;
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
