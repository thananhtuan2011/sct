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
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using System.Drawing.Drawing2D;
using System;
using ClosedXML.Excel;
using API_SoCongThuong.Logger;
using Newtonsoft.Json;
using System.Data;

namespace API_SoCongThuong.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ResultsIndustrialPromotionVoting : ControllerBase
    {
        private ResultsIndustrialPromotionVotingRepo _repo;
        private IConfiguration _configuration;
        private readonly ILogger<AsyncLogger> _logger;
        public SoHoa_SoCongThuongContext _context;
        private AsyncLogger _asyncLogger;

        public ResultsIndustrialPromotionVoting(SoHoa_SoCongThuongContext context, IConfiguration configuration, ILogger<AsyncLogger> logger)
        {
            _repo = new ResultsIndustrialPromotionVotingRepo(context);
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

            BaseModels<ResultsIndustrialPromotionVotingModel> model = new BaseModels<ResultsIndustrialPromotionVotingModel>();
            string _keywordSearch = ""; //Keyword tìm kiếm
            bool _orderBy_ASC = false;  //Khởi tạo sắp xếp dữ liệu acs hoặc desc khi tìm kiếm
            try
            {
                //Lấy Token, lấy model
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }

                Func<ResultsIndustrialPromotionVotingModel, object> _orderByExpression = x => x.ResultsIndustrialPromotionVotingId; //Khởi tạo mặc định sắp xếp dữ liệu
                Dictionary<string, Func<ResultsIndustrialPromotionVotingModel, object>> _sortableFields = new Dictionary<string, Func<ResultsIndustrialPromotionVotingModel, object>>   //Khởi tạo các trường để sắp xếp
                    {
                        { "TargetsName", x => x.TargetsName },
                        { "NumbersRegister", x => x.NumbersRegister },
                        { "NumberCertified", x => x.NumberCertified },
                        { "Locallity", x => x.Locallity },
                        { "Unit", x => x.Unit },
                    };
                if (query.Sort != null
                    && !string.IsNullOrEmpty(query.Sort.ColumnName)
                    && _sortableFields.ContainsKey(query.Sort.ColumnName))
                {
                    _orderBy_ASC = ("desc".Equals(query.Sort.Direction.ToLower()) ? false : true);    //Sắp xếp asc hoặc desc
                    _orderByExpression = _sortableFields[query.Sort.ColumnName]; //Trường cần sắp xếp
                }


                //var lstData = _repoBusi._context.Businesses
                //                   .Where(c => !c.IsDel )
                //                   .ToList();

                var _data = from A in _repo._context.ResultsIndustrialPromotionVotingRps.Where(x => !x.IsDel)
                            join C in _repo._context.Categories.Select(x => new { x.CategoryId, x.CategoryName })
                                on A.Targets equals C.CategoryId
                            select (new ResultsIndustrialPromotionVotingModel
                            {
                                ResultsIndustrialPromotionVotingId = A.ResultsIndustrialPromotionVotingId,
                                NumbersRegister = A.NumbersRegister,
                                NumberCertified = A.NumberCertified,
                                Targets = A.Targets,
                                TargetsName = C.CategoryName,
                                Locallity = A.Locallity,
                                ActionName = (bool)A.Locallity ? "Cấp Huyện" : "Cấp Tỉnh",
                                Unit = A.Unit,
                                IsDel = A.IsDel
                            }
                            );

                //_data = _data.Where(x => !x.IsDel);
                if (query.SearchValue != null && query.SearchValue != "") //Kiểm tra điều kiện tìm kiếm
                {
                    _keywordSearch = query.SearchValue.Trim().ToLower();
                    _data = _data.Where(x =>
                       x.NumberCertified.ToString().Contains(_keywordSearch)
                       || x.NumbersRegister.ToString().Contains(_keywordSearch)
                       || x.Locallity.ToString().Contains(_keywordSearch)
                       || x.TargetsName.Contains(_keywordSearch)
                       || x.Unit.Contains(_keywordSearch)
                   );  //Lấy table đã select tìm kiếm theo keyword
                }

                if (query.Filter != null && query.Filter.ContainsKey("AreaForm") && !string.IsNullOrEmpty(query.Filter["AreaForm"]))
                {
                    _data = _data.Where(x =>
                                x.Locallity == (query.Filter["AreaForm"] == "1" ? true : false));
                }
                // model.items = _data.ToList();

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
        // Load data cho dropdown
        #region
        [Route("loadTargets")]
        [HttpGet]
        public IActionResult loadTargets()
        {
            BaseModels<Category> model = new BaseModels<Category>();

            try
            {
                //Lấy Token, lấy model
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                IQueryable<Category> _data = _repo._context.Categories.Where(x=>x.CategoryTypeCode== "PRODUCTS_VOTING");

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
        #endregion

        [HttpPost()]
        public async Task<IActionResult> create(ResultsIndustrialPromotionVotingModel data)
        {
            BaseModels<ResultsIndustrialPromotionVotingModel> model = new BaseModels<ResultsIndustrialPromotionVotingModel>();
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }


                SystemLog datalog = new SystemLog();
                data.CreateUserId = loginData.Userid;
                data.CreateTime = DateTime.Now;

                await _repo.Insert(data);
                datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.CREATE, NameCategory_Const.RESULT_INDUSTRIAL_PROMOTION_VOTING, Action_Status.SUCCESS);
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
        public async Task<IActionResult> Update(ResultsIndustrialPromotionVotingModel data)
        {
            BaseModels<ResultsIndustrialPromotionVotingModel> model = new BaseModels<ResultsIndustrialPromotionVotingModel>();
            try
            {

                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }

                var CheckData = _repo.FindById(data.ResultsIndustrialPromotionVotingId);
                SystemLog datalog = new SystemLog();
                if (CheckData == null)
                {
                    //chỗ này không tồn tại id
                    model.status = 0;
                    model.error = new ErrorModel()
                    {
                        Code = ErrCode_Const.PROPERTY_IS_NULL_OR_EMPTY
                    };
                    datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.UPDATE, NameCategory_Const.RESULT_INDUSTRIAL_PROMOTION_VOTING, Action_Status.FAIL);
                    _asyncLogger.LogInfo(JsonConvert.SerializeObject(datalog));
                    return BadRequest(model);
                }
                else
                {
                    if (string.IsNullOrEmpty(data.ResultsIndustrialPromotionVotingId.ToString()))
                    {
                        model.status = 0;
                        model.error = new ErrorModel()
                        {
                            Code = ErrCode_Const.PROPERTY_IS_NULL_OR_EMPTY
                        };
                        datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.UPDATE, NameCategory_Const.RESULT_INDUSTRIAL_PROMOTION_VOTING, Action_Status.FAIL);
                        _asyncLogger.LogInfo(JsonConvert.SerializeObject(datalog));
                        return BadRequest(model);
                    }
                    data.UpdateTime = DateTime.Now;
                    data.UpdateUserId = loginData.Userid;
                    await _repo.Update(data);
                    datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.UPDATE, NameCategory_Const.RESULT_INDUSTRIAL_PROMOTION_VOTING, Action_Status.SUCCESS);
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
                datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.DELETE, NameCategory_Const.RESULT_INDUSTRIAL_PROMOTION_VOTING, Action_Status.SUCCESS);
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
            BaseModels<ResultsIndustrialPromotionVotingModel> model = new BaseModels<ResultsIndustrialPromotionVotingModel>();
            try
            {
                var result = _repo.FindById(id);
                if (result == null)
                    return NotFound(ErrMsg_Const.GetMsg(ErrCode_Const.CANNOT_FIND_DATA_BY_QUERY));

                model.status = 1;
                model.data = result;
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


        [HttpGet("export")]
        public IActionResult ExportFile()
        {
            //Query data
            var data = from A in _repo._context.ResultsIndustrialPromotionVotingRps.Where(x => !x.IsDel)
                       join C in _repo._context.Categories.Select(x => new { x.CategoryId, x.CategoryName })
                           on A.Targets equals C.CategoryId
                       select (new ResultsIndustrialPromotionVotingModel
                       {
                           ResultsIndustrialPromotionVotingId = A.ResultsIndustrialPromotionVotingId,
                           NumbersRegister = A.NumbersRegister,
                           NumberCertified = A.NumberCertified,
                           Targets = A.Targets,
                           TargetsName = C.CategoryName,
                           Locallity = A.Locallity,
                           ActionName = (bool)A.Locallity ? "Cấp Huyện" : "Cấp Tỉnh",
                           Unit = A.Unit,
                           IsDel = A.IsDel
                       }
                       );
            var _data = (from A in _repo._context.ResultsIndustrialPromotionVotingRps.Where(x => !x.IsDel)
                         join C in _repo._context.Categories.Select(x => new { x.CategoryId, x.CategoryName })
                             on A.Targets equals C.CategoryId
                         select new ResultsIndustrialPromotionVotingModel
                         {
                             ResultsIndustrialPromotionVotingId = A.ResultsIndustrialPromotionVotingId,
                             NumbersRegister = A.NumbersRegister,
                             NumberCertified = A.NumberCertified,
                             Targets = A.Targets,
                             TargetsName = C.CategoryName,
                             Locallity = A.Locallity,
                             ActionName = (bool)A.Locallity ? "Cấp Huyện" : "Cấp Tỉnh",
                             Unit = A.Unit,
                             IsDel = A.IsDel
                         }).GroupBy( d => d.Targets).Select(d => new
                         {
                             name = d.ToList().Select( x => x.TargetsName).FirstOrDefault(),
                             target = d.Key,
                             countNumbersRegister_Provice = data.Where(item => item.Targets == d.Key && !item.Locallity).Select( item => item.NumbersRegister).Sum(),
                             countNumbersRegister_District = data.Where(item => item.Targets == d.Key && item.Locallity).Select(item => item.NumbersRegister).Sum(),

                             countNumberCertified_Provice = data.Where(item => item.Targets == d.Key && !item.Locallity).Select(item => item.NumberCertified).Sum(),
                             countNumberCertified_District = data.Where(item => item.Targets == d.Key && item.Locallity).Select(item => item.NumberCertified).Sum(),

                         });
                        
            if (data == null)
            {
                return BadRequest();
            }

            try
            {
                using (var workbook = new XLWorkbook(@"Upload/Templates/Baocaoketquabinhchonsanphamnongthontieubieu.xlsx"))
                {
                    IXLWorksheet worksheet = workbook.Worksheets.Worksheet(1);
                    int index = 9;

                    //Thêm dữ liệu vào file:
                    foreach (var item in _data)
                    {
                        worksheet.Cell(index, 2).Value = item.name;
                        worksheet.Cell(index, 4).Value = item.countNumbersRegister_District;
                        worksheet.Cell(index, 5).Value = item.countNumbersRegister_Provice;
                        worksheet.Cell(index + 5, 2).Value = item.name;
                        worksheet.Cell(index + 5, 4).Value = item.countNumbersRegister_District;
                        worksheet.Cell(index + 5, 5).Value = item.countNumbersRegister_Provice;
                        index++;
                        
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
    }
}
