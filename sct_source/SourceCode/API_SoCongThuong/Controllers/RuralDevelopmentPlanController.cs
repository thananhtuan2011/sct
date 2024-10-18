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
using Microsoft.AspNetCore.Mvc.Filters;
using static API_SoCongThuong.Classes.Ulities;
using Newtonsoft.Json;
using System.Collections.Generic;
using ClosedXML.Excel;
using API_SoCongThuong.Logger;
using System.Data;

namespace API_SoCongThuong.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RuralDevelopmentPlanController : ControllerBase
    {
        private RuralDevelopmentPlanRepo _repo;
        private IConfiguration _configuration;
        private readonly ILogger<AsyncLogger> _logger;
        public SoHoa_SoCongThuongContext _context;
        private AsyncLogger _asyncLogger;
        public RuralDevelopmentPlanController(SoHoa_SoCongThuongContext context, IConfiguration configuration, ILogger<AsyncLogger> logger)
        {
            _repo = new RuralDevelopmentPlanRepo(context);

            _logger = logger;
            _context = context;
            _asyncLogger = new AsyncLogger(_logger, _context);
            _configuration = configuration;
        }

        [Route("find")]
        [HttpPost]
        public IActionResult ListItems_New([FromBody] QueryRequestBody query)//query truyền lên
        {

            BaseModels<RuralDevelopmentPlanModel> model = new BaseModels<RuralDevelopmentPlanModel>();
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

                Func<RuralDevelopmentPlanModel, object> _orderByExpression = x => x.RuralDevelopmentPlanId; //Khởi tạo mặc định sắp xếp dữ liệu
                Dictionary<string, Func<RuralDevelopmentPlanModel, object>> _sortableFields = new Dictionary<string, Func<RuralDevelopmentPlanModel, object>>   //Khởi tạo các trường để sắp xếp
                {
                    { "SuperMarketShoppingMallName", x => x.SuperMarketShoppingMallName },
                    { "Address", x => x.Address },
                    { "TotalInvestment", x => x.TotalInvestment },
                    { "Budget", x => x.Budget },
                    { "OutOfBudget", x => x.OutOfBudget },
                    { "Type", x => x.Type },
                };
                if (query.Sort != null
                    && !string.IsNullOrEmpty(query.Sort.ColumnName)
                    && _sortableFields.ContainsKey(query.Sort.ColumnName))
                {
                    _orderBy_ASC = ("desc".Equals(query.Sort.Direction.ToLower()) ? false : true);    //Sắp xếp asc hoặc desc
                    _orderByExpression = _sortableFields[query.Sort.ColumnName]; //Trường cần sắp xếp
                }

                IQueryable<RuralDevelopmentPlanModel> _data = _repo._context.RuralDevelopmentPlans.Where(x => !x.IsDel).Select(x => new RuralDevelopmentPlanModel
                {
                    RuralDevelopmentPlanId = x.RuralDevelopmentPlanId,
                    SuperMarketShoppingMallName = x.SuperMarketShoppingMallName,
                    Address = x.Address,
                    TotalInvestment = x.TotalInvestment,
                    Budget = x.Budget,
                    OutOfBudget = x.OutOfBudget,
                    Type = x.Type,
                    StageId = x.StageId,
                    Stages = _repo._context.RuralDevelopmentPlanStages.Where(s => s.RuralDevelopmentPlanId == x.RuralDevelopmentPlanId).Select(_ => new PlanStageModel
                    {
                        RuralDevelopmentPlanId = _.RuralDevelopmentPlanId,
                        PlanStageId = _.PlanStageId,
                        Budget = _.Budget,
                        StageId = _.StageId,
                        Year = _.Year,
                    }).ToList()
                }).ToList().AsQueryable();

                if (query.SearchValue != null && query.SearchValue != "") //Kiểm tra điều kiện tìm kiếm
                {
                    _keywordSearch = query.SearchValue.Trim().ToLower();
                    _data = _data.Where(x => x.Address.ToLower().Contains(_keywordSearch) || x.SuperMarketShoppingMallName.ToLower().Contains(_keywordSearch));
                }

                if (query.Filter != null && query.Filter.ContainsKey("Stage")
                                         && !string.IsNullOrEmpty(query.Filter["Stage"]))
                {
                    _data = _data.Where(x => x.StageId.ToString() == query.Filter["Stage"]);
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

        [HttpPost()]
        public async Task<IActionResult> Create(RuralDevelopmentPlanModel data)
        {
            BaseModels<RuralDevelopmentPlanModel> model = new BaseModels<RuralDevelopmentPlanModel>();
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                SystemLog datalog = new SystemLog();
                data.CreateTime = DateTime.Now;
                data.CreateUserId = loginData.Userid;

                await _repo.Insert(data);
                datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.CREATE, NameCategory_Const.RURAL_DEVELOPMENT_PLAN, Action_Status.SUCCESS);
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
        public async Task<IActionResult> Update(RuralDevelopmentPlanModel data)
        {
            BaseModels<RuralDevelopmentPlanModel> model = new BaseModels<RuralDevelopmentPlanModel>();
            try
            {

                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                SystemLog datalog = new SystemLog();
                var CheckData = _repo.FindById(data.RuralDevelopmentPlanId);
                if (CheckData == null)
                {
                    //chỗ này không tồn tại id
                    model.status = 0;
                    model.error = new ErrorModel()
                    {
                        Code = ErrCode_Const.PROPERTY_IS_NULL_OR_EMPTY
                    };
                    datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.UPDATE, NameCategory_Const.RURAL_DEVELOPMENT_PLAN, Action_Status.FAIL);
                    _asyncLogger.LogInfo(JsonConvert.SerializeObject(datalog));
                    return BadRequest(model);
                }
                else
                {
                    data.UpdateTime = DateTime.Now;
                    data.UpdateUserId = loginData.Userid;

                    await _repo.Update(data);
                    datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.UPDATE, NameCategory_Const.RURAL_DEVELOPMENT_PLAN, Action_Status.SUCCESS);
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
            BaseModels<RuralDevelopmentPlanModel> model = new BaseModels<RuralDevelopmentPlanModel>();
            try
            {
                var info = _repo.FindById(id);

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
                datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.DELETE, NameCategory_Const.RURAL_DEVELOPMENT_PLAN, Action_Status.SUCCESS);
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

        [Route("loadStage")]
        [HttpGet]
        public IActionResult LoadStage()
        {
            BaseModels<Stage> model = new BaseModels<Stage>();

            try
            {
                //Lấy Token, lấy model
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }

                IQueryable<Stage> _data = _repo._context.Stages.Where(x => !x.IsDel).OrderBy(x => x.StartYear).ThenBy(x => x.EndYear);

                model.status = 1;
                model.items = _data.ToList();
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
            var data = _repo.FindData(query);

            if (!data.Any())
            {
                return BadRequest();
            }

            var range = data[0].Stages.Count() - 1;
            var yearRange = data[0].Stages.Select(x => x.Year).ToList().OrderBy(x => x).ToList();

            try
            {
                using (var workbook = new XLWorkbook(@"Upload/Templates/Kehoachphattriennongthon.xlsx"))
                {
                    IXLWorksheet worksheet = workbook.Worksheets.Worksheet(1);
                    int startYear = yearRange.First();
                    int endYear = yearRange.Last();
                    worksheet.Cell(2, 1).Value = $"Giai đoạn {startYear} - {endYear}";
                    int index = 5;
                    int row = 1;

                    worksheet.Column(4).InsertColumnsAfter(range);
                    worksheet.Range(3, 4, 3, 4 + range).Merge();

                    for (int i = 4; i < 5 + range; i++)
                    {
                        worksheet.Cell(4, i).Value = yearRange[i - 4];
                    }

                    foreach (var item in data)
                    {
                        if (row == 1)
                        {
                            worksheet.Cell(index, 1).Value = row;
                            worksheet.Cell(index, 2).Value = item.SuperMarketShoppingMallName.ToUpper().Trim();
                            worksheet.Cell(index, 3).Value = item.Address.Trim();
                            for (int indexColumn = 4; indexColumn < 5 + range; indexColumn++)
                            {
                                worksheet.Cell(index, indexColumn).Value = item.Stages.OrderBy(x => x.Year).ToList()[indexColumn - 4].Budget.HasValue ? item.Stages.OrderBy(x => x.Year).ToList()[0].Budget.Value / 1_000_000 : 0;
                            }
                            worksheet.Cell(index, 5 + range).Value = item.TotalInvestment / 1_000_000;
                            worksheet.Cell(index, 6 + range).Value = item.Budget / 1_000_000;
                            worksheet.Cell(index, 7 + range).Value = item.OutOfBudget / 1_000_000;
                            worksheet.Cell(index, 8 + range).Value = item.Type == 1 ? "x" : "";
                            worksheet.Cell(index, 9 + range).Value = item.Type == 0 ? "x" : "";
                            index++;
                            row++;
                        }
                        else
                        {
                            var addrow = worksheet.Row(index - 1);
                            addrow.InsertRowsBelow(1);
                            worksheet.Cell(index, 1).Value = row;
                            worksheet.Cell(index, 2).Value = item.SuperMarketShoppingMallName.ToUpper().Trim();
                            worksheet.Cell(index, 3).Value = item.Address.Trim();
                            for (int indexColumn = 4; indexColumn < 5 + range; indexColumn++)
                            {
                                worksheet.Cell(index, indexColumn).Value = item.Stages.OrderBy(x => x.Year).ToList()[indexColumn - 4].Budget.HasValue ? item.Stages.OrderBy(x => x.Year).ToList()[0].Budget.Value / 1_000_000 : 0;
                            }
                            worksheet.Cell(index, 5 + range).Value = item.TotalInvestment / 1_000_000;
                            worksheet.Cell(index, 6 + range).Value = item.Budget / 1_000_000;
                            worksheet.Cell(index, 7 + range).Value = item.OutOfBudget / 1_000_000;
                            worksheet.Cell(index, 8 + range).Value = item.Type == 1 ? "x" : "";
                            worksheet.Cell(index, 9 + range).Value = item.Type == 0 ? "x" : "";
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
