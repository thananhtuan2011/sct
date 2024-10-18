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
using DocumentFormat.OpenXml.EMMA;
using System;
using ClosedXML.Excel;
using API_SoCongThuong.Logger;
using Newtonsoft.Json;
using System.Data;

namespace API_SoCongThuong.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RPOSOfInvestmentProjectsController : ControllerBase
    {
        private RPOSOfInvestmentProjectsRepo _repo;
        private IConfiguration _configuration;
        private readonly ILogger<AsyncLogger> _logger;
        public SoHoa_SoCongThuongContext _context;
        private AsyncLogger _asyncLogger;

        public RPOSOfInvestmentProjectsController(SoHoa_SoCongThuongContext context, IConfiguration configuration, ILogger<AsyncLogger> logger)
        {
            _repo = new RPOSOfInvestmentProjectsRepo(context);

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
            BaseModels<RPOSOfInvestmentProjectModel> model = new BaseModels<RPOSOfInvestmentProjectModel>();

            //Keyword tìm kiếm
            string _keywordSearch = "";

            //Khởi tạo sắp xếp dữ liệu acs hoặc desc khi tìm kiếm
            bool _orderBy_ASC = false;
            try
            {
                ////Lấy Token, lấy model
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }

                //Khởi tạo mặc định sắp xếp dữ liệu
                Func<RPOSOfInvestmentProjectModel, object> _orderByExpression = x => x.ReportOperationalStatusOfInvestmentProjectsId;

                //Khởi tạo các trường để sắp xếp
                Dictionary<string, Func<RPOSOfInvestmentProjectModel, object>> _sortableFields = new Dictionary<string, Func<RPOSOfInvestmentProjectModel, object>>
                    {
                        { "CriteriaName", x => x.CriteriaName },
                        { "OriginalArea", x => x.Units },
                        { "Quantity", x => x.Quantity },
                        { "ExpandedArea", x => x.Note },
                        { "CreateName", x => x.CreateUserId },
                        { "CreateTimeDisplay", x => x.CreateTime },
                        { "IsAction", x => x.IsAction },
                        {"Year", x => x.Year },
                        {"ReportingPeriod", x => x.ReportingPeriod }
                    };

                //Kiểm tra điều kiện sắp xếp
                if (query.Sort != null && !string.IsNullOrEmpty(query.Sort.ColumnName) && _sortableFields.ContainsKey(query.Sort.ColumnName))
                {
                    //Sắp xếp asc hoặc desc
                    _orderBy_ASC = ("desc".Equals(query.Sort.Direction.ToLower()) ? false : true);

                    //Trường cần sắp xếp
                    _orderByExpression = _sortableFields[query.Sort.ColumnName];
                }

                IQueryable<RPOSOfInvestmentProjectModel> _data = _repo._context.ReportOperationalStatusOfInvestmentProjects.Where(x => !x.IsDel).GroupJoin(
                    _repo._context.Users,
                    cc => cc.CreateUserId,
                    u => u.UserId,
                     (cc, u) => new { cc, u }).SelectMany(result => result.u.DefaultIfEmpty(), (info, us) => new { info, us }).GroupJoin(
                    _repo._context.IndustrialManagementTargets, query => query.info.cc.Criteria, cri => cri.IndustrialManagementTargetId,
                    (query, cri) => new { query, cri }).SelectMany(rs => rs.cri.DefaultIfEmpty(), (info1, cri) => new RPOSOfInvestmentProjectModel
                    {
                        ReportOperationalStatusOfInvestmentProjectsId = info1.query.info.cc.ReportOperationalStatusOfInvestmentProjectsId,
                        CriteriaName = cri.Name,
                        Units = cri.Unit,
                        Quantity = info1.query.info.cc.Quantity,
                        Note = info1.query.info.cc.Note,
                        Year = info1.query.info.cc.Year,
                        ReportingPeriod = info1.query.info.cc.ReportingPeriod,
                        CreateName = info1.query.us.FullName,
                        CreateTime = info1.query.info.cc.CreateTime,
                        CreateTimeDisplay = info1.query.info.cc.CreateTime.ToString("dd/MM/yyyy hh:mm")
                    }).ToList().AsQueryable();

                //Kiểm tra điều kiện tìm kiếm
                if (query.SearchValue != null && query.SearchValue != "")
                {
                    _keywordSearch = query.SearchValue.Trim().ToLower();

                    //Lấy table đã select tìm kiếm theo keyword
                    _data = _data.Where(x => x.CriteriaName.ToLower().Contains(_keywordSearch)
                    || x.Units.ToString().Contains(_keywordSearch)
                    || x.Quantity.ToString().Contains(_keywordSearch)
                    || x.Note.ToString().Contains(_keywordSearch)
                    );
                }

                // model.items = _data.ToList();
                if (query.Filter != null && query.Filter.ContainsKey("ReportingPeriod") && !string.IsNullOrEmpty(query.Filter["ReportingPeriod"]))
                {
                    _data = _data.Where(x => x.ReportingPeriod.ToString() == query.Filter["ReportingPeriod"]);
                }

                if (query.Filter != null && query.Filter.ContainsKey("Year")
                    && !string.IsNullOrEmpty(query.Filter["Year"]))
                {
                    _data = _data.Where(x => x.Year.ToString() == query.Filter["Year"]);
                }
                else
                {
                    _data = _data.Where(x => x.Year == DateTime.Now.Year);
                }

                //Đếm số dòng của table đã select được
                int _countRows = _data.Count();

                //Nếu table = 0 thì trả về không có dữ liệu
                if (_countRows == 0)
                {
                    return NotFound("Không có dữ liệu");
                }
                //query more = true
                if (query.Panigator.More)
                {
                    model.status = 1;
                    model.items = _data.ToList();
                    model.total = _countRows;
                    return Ok(model);
                }
                //Sắp xếp dữ liệu theo acs
                if (_orderBy_ASC)
                {
                    model.items = _data
                        .OrderBy(_orderByExpression)
                        .Skip((query.Panigator.PageIndex - 1) * query.Panigator.PageSize)
                        .Take(query.Panigator.PageSize)
                        .ToList();
                }
                //Sắp xếp dữ liệu theo desc
                else
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
        #endregion

        [HttpPost()]
        public async Task<IActionResult> create(RPOSOfInvestmentProjectModel data)
        {
            BaseModels<RPOSOfInvestmentProjectModel> model = new BaseModels<RPOSOfInvestmentProjectModel>();
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                SystemLog datalog = new SystemLog();
                if (string.IsNullOrEmpty(data.Units))
                {
                    model.status = 0;
                    model.error = new ErrorModel()
                    {
                        Code = ErrCode_Const.PROPERTY_IS_NULL_OR_EMPTY
                    };
                    datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.CREATE, NameCategory_Const.REPORT_INVESTMENT, Action_Status.FAIL);
                    _asyncLogger.LogInfo(JsonConvert.SerializeObject(datalog));
                    return BadRequest(model);
                }
                int count = _repo._context.ReportOperationalStatusOfInvestmentProjects.Where(x => !x.IsDel && x.Criteria == data.Criteria && x.Year == data.Year && x.ReportingPeriod == data.ReportingPeriod).Count();
                if(count > 0)
                {
                    model.status = 0;
                    model.error = new ErrorModel()
                    {
                        Code = ErrCode_Const.EXCEPTION_API,
                        Msg = "Kỳ báo cáo đã tồn tại"
                    };
                    datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.CREATE, NameCategory_Const.REPORT_INVESTMENT, Action_Status.FAIL);
                    _asyncLogger.LogInfo(JsonConvert.SerializeObject(datalog));
                    return Ok(model);
                }
                data.CreateTime = DateTime.Now;
                data.CreateUserId = loginData.Userid;
                await _repo.Insert(data);
                datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.CREATE, NameCategory_Const.REPORT_INVESTMENT, Action_Status.SUCCESS);
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
        public async Task<IActionResult> Update(RPOSOfInvestmentProjectModel data)
        {
            BaseModels<RPOSOfInvestmentProjectModel> model = new BaseModels<RPOSOfInvestmentProjectModel>();
            try
            {

                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }

                var CheckData = _repo.FindById(data.ReportOperationalStatusOfInvestmentProjectsId);
                SystemLog datalog = new SystemLog();
                if (CheckData == null)
                {
                    //chỗ này không tồn tại id
                    model.status = 0;
                    model.error = new ErrorModel()
                    {
                        Code = ErrCode_Const.PROPERTY_IS_NULL_OR_EMPTY
                    };
                    datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.UPDATE, NameCategory_Const.REPORT_INVESTMENT, Action_Status.FAIL);
                    _asyncLogger.LogInfo(JsonConvert.SerializeObject(datalog));
                    return BadRequest(model);
                }
                else
                {
                    if (string.IsNullOrEmpty(data.Units))
                    {
                        model.status = 0;
                        model.error = new ErrorModel()
                        {
                            Code = ErrCode_Const.PROPERTY_IS_NULL_OR_EMPTY
                        };
                        datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.UPDATE, NameCategory_Const.REPORT_INVESTMENT, Action_Status.FAIL);
                        _asyncLogger.LogInfo(JsonConvert.SerializeObject(datalog));
                        return BadRequest(model);
                    }

                    int count = _repo._context.ReportOperationalStatusOfInvestmentProjects.Where(x => !x.IsDel && x.Criteria == data.Criteria && x.ReportOperationalStatusOfInvestmentProjectsId != data.ReportOperationalStatusOfInvestmentProjectsId && x.Year == data.Year && x.ReportingPeriod == data.ReportingPeriod).Count();
                    if (count > 0)
                    {
                        model.status = 0;
                        model.error = new ErrorModel()
                        {
                            Code = ErrCode_Const.EXCEPTION_API,
                            Msg = "Kỳ báo cáo đã tồn tại"
                        };
                        datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.UPDATE, NameCategory_Const.REPORT_INVESTMENT, Action_Status.FAIL);
                        _asyncLogger.LogInfo(JsonConvert.SerializeObject(datalog));
                        return Ok(model);
                    }

                    data.UpdateTime = DateTime.Now;
                    data.UpdateUserId = loginData.Userid;
                    await _repo.Update(data);
                    datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.UPDATE, NameCategory_Const.REPORT_INVESTMENT, Action_Status.SUCCESS);
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
            BaseModels<RPOSOfInvestmentProjectModel> model = new BaseModels<RPOSOfInvestmentProjectModel>();
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
                datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.DELETE, NameCategory_Const.REPORT_INVESTMENT, Action_Status.SUCCESS);
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

        private List<RPOSOfInvestmentProjectModel> FindData([FromBody] QueryRequestBody query)//query truyền lên
        {
            bool _orderBy_ASC = true;  //Khởi tạo sắp xếp dữ liệu acs hoặc desc khi tìm kiếm
            string _keywordSearch = "";
            Func<RPOSOfInvestmentProjectModel, object> _orderByExpression = x => x.ReportOperationalStatusOfInvestmentProjectsId;

            //Khởi tạo các trường để sắp xếp
            Dictionary<string, Func<RPOSOfInvestmentProjectModel, object>> _sortableFields = new Dictionary<string, Func<RPOSOfInvestmentProjectModel, object>>
                    {
                        { "CriteriaName", x => x.CriteriaName },
                        { "OriginalArea", x => x.Units },
                        { "Quantity", x => x.Quantity },
                        { "ExpandedArea", x => x.Note },
                        { "CreateName", x => x.CreateUserId },
                        { "CreateTimeDisplay", x => x.CreateTime },
                        { "IsAction", x => x.IsAction },
                        {"Year", x => x.Year },
                        {"ReportingPeriod", x => x.ReportingPeriod }
                    };

            //Kiểm tra điều kiện sắp xếp
            if (query.Sort != null && !string.IsNullOrEmpty(query.Sort.ColumnName) && _sortableFields.ContainsKey(query.Sort.ColumnName))
            {
                //Sắp xếp asc hoặc desc
                _orderBy_ASC = ("desc".Equals(query.Sort.Direction.ToLower()) ? false : true);

                //Trường cần sắp xếp
                _orderByExpression = _sortableFields[query.Sort.ColumnName];
            }

            IQueryable<RPOSOfInvestmentProjectModel> _data = _repo._context.ReportOperationalStatusOfInvestmentProjects.Where(x => !x.IsDel).GroupJoin(
                _repo._context.Users,
                cc => cc.CreateUserId,
                u => u.UserId,
                 (cc, u) => new { cc, u }).SelectMany(result => result.u.DefaultIfEmpty(), (info, us) => new { info, us }).GroupJoin(
                _repo._context.IndustrialManagementTargets, query => query.info.cc.Criteria, cri => cri.IndustrialManagementTargetId,
                (query, cri) => new { query, cri }).SelectMany(rs => rs.cri.DefaultIfEmpty(), (info1, cri) => new RPOSOfInvestmentProjectModel
                {
                    ReportOperationalStatusOfInvestmentProjectsId = info1.query.info.cc.ReportOperationalStatusOfInvestmentProjectsId,
                    CriteriaName = cri.Name,
                    Units = cri.Unit,
                    Quantity = info1.query.info.cc.Quantity,
                    Note = info1.query.info.cc.Note,
                    Year = info1.query.info.cc.Year,
                    ReportingPeriod = info1.query.info.cc.ReportingPeriod,
                }).ToList().AsQueryable();

            //Kiểm tra điều kiện tìm kiếm
            if (query.SearchValue != null && query.SearchValue != "")
            {
                _keywordSearch = query.SearchValue.Trim().ToLower();

                //Lấy table đã select tìm kiếm theo keyword
                _data = _data.Where(x => x.CriteriaName.ToLower().Contains(_keywordSearch)
                || x.Units.ToString().Contains(_keywordSearch)
                || x.Quantity.ToString().Contains(_keywordSearch)
                || x.Note.ToString().Contains(_keywordSearch)
                );
            }

            // model.items = _data.ToList();
            if (query.Filter != null && query.Filter.ContainsKey("ReportingPeriod") && !string.IsNullOrEmpty(query.Filter["ReportingPeriod"]))
            {
                _data = _data.Where(x => x.ReportingPeriod.ToString() == query.Filter["ReportingPeriod"]);
            }

            if (query.Filter != null && query.Filter.ContainsKey("Year")
                && !string.IsNullOrEmpty(query.Filter["Year"]))
            {
                _data = _data.Where(x => x.Year.ToString() == query.Filter["Year"]);
            }
            else
            {
                _data = _data.Where(x => x.Year == DateTime.Now.Year);
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
                string currentYear = DateTime.Now.ToString("yyyy");
                string term = "";
                if (query.Filter != null && query.Filter.ContainsKey("ReportingPeriod") && !string.IsNullOrEmpty(query.Filter["ReportingPeriod"]))
                {
                    term = query.Filter["ReportingPeriod"];
                }

                if (query.Filter != null && query.Filter.ContainsKey("Year")
                    && !string.IsNullOrEmpty(query.Filter["Year"]))
                {
                    currentYear = query.Filter["Year"];
                }
                if(term == "1")
                {
                    term = $"6 tháng đầu năm {currentYear}";
                }else if(term == "2")
                {
                    term = $"cả năm {currentYear}";
                }
                string title = $"Báo cáo tình hình hoạt động của dự án đầu tư trong cụm công nghiệp {term}";
                using (var workbook = new XLWorkbook(@"Upload/Templates/BaoCaoHoatDongDauTuDuAnCumCongNghiep.xlsx"))
                {
                    IXLWorksheet worksheet = workbook.Worksheets.Worksheet(1);
                    worksheet.Cell(2, 1).Value = title;
                    worksheet.Cell(6, 1).Value = $"Kỳ báo cáo: {term}";

                    int index = 11;
                    int row = 1;
                    foreach (var item in data)
                    {
                        var addrow = worksheet.Row(index);
                        addrow.InsertRowsBelow(1);

                        worksheet.Cell(index, 1).Value = row;
                        worksheet.Cell(index, 2).Value = item.CriteriaName;
                        worksheet.Cell(index, 3).Value = item.Units;
                        worksheet.Cell(index, 4).Value = item.Quantity;
                        worksheet.Cell(index, 5).Value = item.Note;

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
