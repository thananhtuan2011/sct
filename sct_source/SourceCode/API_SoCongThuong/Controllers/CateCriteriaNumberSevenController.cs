using DpsLibs.Web;
using EF_Core.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.Design;
using API_SoCongThuong.Classes;
using API_SoCongThuong.Models;
using API_SoCongThuong.Reponsitories.BusinessIndustryRepository;
using API_SoCongThuong.Reponsitories.BusinessRepository;
using API_SoCongThuong.Reponsitories.IndustryRepository;
using API_SoCongThuong.Reponsitories.TypeOfBusinessRepository;
using API_SoCongThuong.Reponsitories.TypeOfProfessionRepository;
using API_SoCongThuong.Reponsitories.CategoryRepository;
using API_SoCongThuong.Reponsitories.DistrictRepository;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using API_SoCongThuong.Reponsitories;
using Microsoft.EntityFrameworkCore.Internal;
using ClosedXML.Excel;
using System.Net;
using System.Net.Http.Headers;
using System.Collections;
using API_SoCongThuong.Logger;
using Newtonsoft.Json;
using System.Data;

namespace API_SoCongThuong.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CateCriteriaNumberSevenController : ControllerBase
    {
        private CateCriteriaNumberSevenRepo _repo;
        private CateRetailRepo _repoCateRetail;
        //private BusinessRepo _repoBusi;

        private IConfiguration _configuration;
        private readonly ILogger<AsyncLogger> _logger;
        public SoHoa_SoCongThuongContext _context;
        private AsyncLogger _asyncLogger;

        public CateCriteriaNumberSevenController(SoHoa_SoCongThuongContext context, IConfiguration configuration, ILogger<AsyncLogger> logger)
        {
            _repo = new CateCriteriaNumberSevenRepo(context);
            _repoCateRetail = new CateRetailRepo(context);
            //_repoTypeOfBusiness = new TypeOfBusinessRepo(context);

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

            BaseModels<CateCriteriaNumberSevenModel> model = new BaseModels<CateCriteriaNumberSevenModel>();
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

                Func<CateCriteriaNumberSevenModel, object> _orderByExpression = x => x.CateCriteriaNumberSevenCode; //Khởi tạo mặc định sắp xếp dữ liệu
                Dictionary<string, Func<CateCriteriaNumberSevenModel, object>> _sortableFields = new Dictionary<string, Func<CateCriteriaNumberSevenModel, object>>   //Khởi tạo các trường để sắp xếp
                    {
                    { "cateCriteriaNumberSevenCode", x => x.CateCriteriaNumberSevenCode },
                    { "CheckName", x => x.CheckUserId },
                    { "ConfirmName", x => x.ConfirmUserId },
                    { "ConfirmTimeDisplay", x => x.ConfirmTime },
                    { "CreateTimeDisplay", x => x.CreateTime },
                    { "IsAction", x => x.IsAction },
                    };
                if (query.Sort != null
                    && !string.IsNullOrEmpty(query.Sort.ColumnName)
                    && _sortableFields.ContainsKey(query.Sort.ColumnName))
                {
                    _orderBy_ASC = ("desc".Equals(query.Sort.Direction.ToLower()) ? false : true);    //Sắp xếp asc hoặc desc
                    _orderByExpression = _sortableFields[query.Sort.ColumnName]; //Trường cần sắp xếp
                }


                //var lstData = _repoBusi._context.Businesses
                //                   .Where(c => !c.IsDel)
                //                   .ToList();

                IQueryable<CateCriteriaNumberSevenModel> _data = _repo._context.CateCriteriaNumberSevens.Where(x => !x.IsDel).GroupJoin(
                    _repo._context.Users,
                    cc => cc.CreateUserId,
                    u => u.UserId,
                    (cc, u) => new//lấy người tạo
                    { cc, u })
                    .SelectMany(rs => rs.u.DefaultIfEmpty(), (info, use) => new { info, use }).GroupJoin(
                    _repo._context.Users,
                        query1 => query1.info.cc.ConfirmUserId,
                        u2 => u2.UserId,
                        (query1, u2) => new { query1, u2 }//lấy người duyệt
                    ).SelectMany(rs => rs.u2.DefaultIfEmpty(), (info1, use1) => new { info1, use1 }).GroupJoin(
                    _repo._context.Users,
                        query2 => query2.info1.query1.info.cc.CheckUserId,
                        u3 => u3.UserId,//lấy người kiểm tra
                        (query2, u3) => new { query2, u3 }).SelectMany(rs => rs.u3.DefaultIfEmpty(), (info2, use2) => new CateCriteriaNumberSevenModel//lấy người kiểm tra
                        {
                            CateCriteriaNumberSevenId = info2.query2.info1.query1.info.cc.CateCriteriaNumberSevenId,
                            CateCriteriaNumberSevenCode = info2.query2.info1.query1.info.cc.CateCriteriaNumberSevenCode,
                            CreateName = info2.query2.info1.query1.use.FullName,
                            CreateTime = info2.query2.info1.query1.info.cc.CreateTime,
                            CreateTimeDisplay = string.Format("{0:dd/MM/yyyy}", info2.query2.info1.query1.info.cc.CreateTime),
                            ConfirmTimeDisplay = string.Format("{0:dd/MM/yyyy}", info2.query2.info1.query1.info.cc.ConfirmTime),
                            ConfirmName = info2.query2.use1.FullName,
                            CheckName = use2.FullName,
                            CreateUserId = info2.query2.info1.query1.info.cc.CreateUserId,
                        }
                    ).ToList().AsQueryable();
                //_data = _data.Where(x => !x.IsDel);

                if (query.SearchValue != null && query.SearchValue != "") //Kiểm tra điều kiện tìm kiếm
                {
                    _keywordSearch = query.SearchValue.Trim().ToLower();
                    _data = _data.Where(x => x.CateCriteriaNumberSevenCode.ToLower().Contains(_keywordSearch)
                    || x.CreateName.ToLower().Contains(_keywordSearch)
                    //|| x.CreateTimeDisplay.ToLower().Contains(_keywordSearch)
                    //|| x.ConfirmTimeDisplay.ToString().Contains(_keywordSearch)
                    || x.ConfirmName.ToLower().Contains(_keywordSearch)
                    || x.CheckName.ToString().ToLower().Contains(_keywordSearch)
                    );  //Lấy table đã select tìm kiếm theo keyword
                }
                if (query.Filter != null && query.Filter.ContainsKey("InputDataPersonId"))
                {
                    var inputDataPersonIdFilter = query.Filter["InputDataPersonId"];
                    _data = _data.Where(x => Guid.Parse(inputDataPersonIdFilter) == x.CreateUserId);
                }

                if (query.Filter != null && query.Filter.ContainsKey("MinTime")
                  && !string.IsNullOrEmpty(query.Filter["MinTime"]))
                {
                    _data = _data.Where(x =>
                                (x.CreateTime) >=
                                DateTime.ParseExact(query.Filter["MinTime"], "dd/MM/yyyy", null));
                }

                if (query.Filter != null && query.Filter.ContainsKey("MaxTime")
                    && !string.IsNullOrEmpty(query.Filter["MaxTime"]))
                {
                    _data = _data.Where(x =>
                               x.CreateTime <=
                                DateTime.ParseExact(query.Filter["MaxTime"], "dd/MM/yyyy", null));
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

        [HttpPost()]
        public async Task<IActionResult> create(CateCriteriaNumberSevenModel data)
        {
            BaseModels<CateCriteriaNumberSevenModel> model = new BaseModels<CateCriteriaNumberSevenModel>();
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                SystemLog datalog = new SystemLog();
                if (string.IsNullOrEmpty(data.CateCriteriaNumberSevenCode))
                {
                    model.status = 0;
                    model.error = new ErrorModel()
                    {
                        Code = ErrCode_Const.PROPERTY_IS_NULL_OR_EMPTY
                    };
                    datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.CREATE, NameCategory_Const.NUMBER_SEVEN, Action_Status.FAIL);
                    _asyncLogger.LogInfo(JsonConvert.SerializeObject(datalog));
                    return BadRequest(model);
                }
                //if (data.ConfirmTime == DateTime.MinValue)
                //{
                //    model.status = 0;
                //    model.error = new ErrorModel()
                //    {
                //        Code = ErrCode_Const.PROPERTY_IS_NULL_OR_EMPTY
                //    };
                //    return BadRequest(model);
                //}
                await _repo.Insert(data);
                datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.CREATE, NameCategory_Const.NUMBER_SEVEN, Action_Status.SUCCESS);
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
        public async Task<IActionResult> Update(CateCriteriaNumberSevenModel data)
        {
            BaseModels<CateCriteriaNumberSevenModel> model = new BaseModels<CateCriteriaNumberSevenModel>();
            try
            {

                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                SystemLog datalog = new SystemLog();
                var CheckData = _repo.FindById(data.CateCriteriaNumberSevenId);
                if (CheckData == null)
                {
                    //chỗ này không tồn tại id
                    model.status = 0;
                    model.error = new ErrorModel()
                    {
                        Code = ErrCode_Const.PROPERTY_IS_NULL_OR_EMPTY
                    };
                    datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.UPDATE, NameCategory_Const.NUMBER_SEVEN, Action_Status.FAIL);
                    _asyncLogger.LogInfo(JsonConvert.SerializeObject(datalog));
                    return BadRequest(model);
                }
                else
                {
                    if (string.IsNullOrEmpty(data.CateCriteriaNumberSevenCode))
                    {
                        model.status = 0;
                        model.error = new ErrorModel()
                        {
                            Code = ErrCode_Const.PROPERTY_IS_NULL_OR_EMPTY
                        };
                        datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.UPDATE, NameCategory_Const.NUMBER_SEVEN, Action_Status.FAIL);
                        _asyncLogger.LogInfo(JsonConvert.SerializeObject(datalog));
                        return BadRequest(model);
                    }
                    //if (data.ConfirmTime == DateTime.MinValue)
                    //{
                    //    model.status = 0;
                    //    model.error = new ErrorModel()
                    //    {
                    //        Code = ErrCode_Const.PROPERTY_IS_NULL_OR_EMPTY
                    //    };
                    //    datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.UPDATE, NameCategory_Const.NUMBER_SEVEN, Action_Status.FAIL);
                    //    _asyncLogger.LogInfo(JsonConvert.SerializeObject(datalog));
                    //    return BadRequest(model);
                    //}

                    data.UpdateTime = DateTime.Now;
                    data.UpdateUserId = loginData.Userid;
                    await _repo.Update(data);
                    datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.UPDATE, NameCategory_Const.NUMBER_SEVEN, Action_Status.SUCCESS);
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
            BaseModels<CateCriteriaNumberSevenModel> model = new BaseModels<CateCriteriaNumberSevenModel>();
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
                datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.DELETE, NameCategory_Const.NUMBER_SEVEN, Action_Status.SUCCESS);
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
        // Load data User
        #region Danh sách  User
        [Route("loaduser")]
        [HttpGet]
        public IActionResult LoadUser()
        {
            BaseModels<User> model = new BaseModels<User>();
            try
            {
                //Lấy Token, lấy model
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }

                //Query lấy data
                IQueryable<User> _data = _repoCateRetail.FindUser();

                //Trả data về model
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

        // Load data district
        #region Danh sách district
        [Route("loaddistrict")]
        [HttpGet]
        public IActionResult LoadDistrict()
        {
            BaseModels<District> model = new BaseModels<District>();
            try
            {
                //Lấy Token, lấy model
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }

                //Query lấy data
                IQueryable<District> _data = _repo._context.Districts.Where(x => !x.IsDel).Select(x => new District
                {
                    DistrictId = x.DistrictId,
                    DistrictName = x.DistrictName,
                    CommuneNumber = _repo._context.Communes.Where(c => c.DistrictId == x.DistrictId && !c.IsDel).Count()
                });

                //Trả data về model
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

        [HttpGet("export")]
        public IActionResult ExportFile()
        {
            var timecheck = new DateTime(DateTime.Now.Year - 1, 6, 1);
            var listId = _repo._context.CateCriteriaNumberSevens.Where(x => x.ConfirmTime >= timecheck && !x.IsDel).Select(x => x.CateCriteriaNumberSevenId).ToList();

            if (listId == null)
            {
                return BadRequest();
            }

            try
            {
                using (var workbook = new XLWorkbook(@"Upload/Templates/Tieuchiso7.xlsx"))
                {
                    var details = _repo._context.CateCriteriaNumberSevenDetails.Where(x => listId.Contains(x.CateCriteriaNumberSevenId)).Join(
                                  _repo._context.Districts, cc => cc.DistrictId, dd => dd.DistrictId,
                                    (cc, dd) => new
                                    {
                                        DistrictId = cc.DistrictId,
                                        CateCriteriaNumberSevenDetailId = cc.CateCriteriaNumberSevenDetailId,
                                        DistrictName = dd.DistrictName,
                                        NumberOfQualifyingWard = cc.NumberOfQualifyingWard,
                                        NumberOfWard = cc.NumberOfWard,
                                        NumberOfWardWithMarket = cc.NumberOfWardWithMarket,
                                        NumberOfWardCommercialInfrastructure = cc.NumberOfWardCommercialInfrastructure,
                                        NumberOfWardCommercialInfrastructureEstimate = cc.NumberOfWardCommercialInfrastructureEstimate,
                                        NumberOfWardCommercialInfrastructurePlan = cc.NumberOfWardCommercialInfrastructurePlan,
                                        NumberOfWardNewCountryside = cc.NumberOfWardNewCountryside,
                                        NumberOfWardNewCountrysideEstimate = cc.NumberOfWardNewCountrysideEstimate,
                                        NumberOfWardNewCountrysidePlan = cc.NumberOfWardNewCountrysidePlan,
                                    });
                    var data = details.GroupBy(x => x.DistrictId)
                                      .Select(r => new
                                      {
                                          DistrictId = r.Key,
                                          DistrictName = r.First().DistrictName,
                                          NumberOfWard = r.Max(x => x.NumberOfWard),
                                          NumberOfQualifyingWard = r.Max(x => x.NumberOfQualifyingWard),
                                          NumberOfWardWithMarket = r.Max(x => x.NumberOfWardWithMarket),
                                          NumberOfWardCommercialInfrastructure = r.Max(x => x.NumberOfWardCommercialInfrastructure),
                                          NumberOfWardNewCountryside = r.Max(x => x.NumberOfWardNewCountryside),
                                          NumberOfWardCommercialInfrastructurePlan = r.Max(x => x.NumberOfWardCommercialInfrastructurePlan),
                                          NumberOfWardNewCountrysidePlan = r.Max(x => x.NumberOfWardNewCountrysidePlan),
                                          NumberOfWardCommercialInfrastructureEstimate = r.Max(x => x.NumberOfWardCommercialInfrastructureEstimate),
                                          NumberOfWardNewCountrysideEstimate = r.Max(x => x.NumberOfWardNewCountrysideEstimate)
                                      }).ToList();

                    IXLWorksheet worksheet = workbook.Worksheets.Worksheet(1);
                    int index = 14;
                    int row = 1;
                    int rowend = data.Count();

                    //Các ô thời gian trong file:
                    var currentTime = DateTime.Now;
                    worksheet.Cell(7, 1).Value = "6 tháng " + currentTime.Year.ToString();
                    //worksheet.Cell(17, 9).Value

                    //Thêm dữ liệu vào file:
                    foreach (var item in data)
                    {
                        if (row == 1)
                        {
                            worksheet.Cell(index, 1).Value = row;
                            worksheet.Cell(index, 2).Value = item.DistrictName;
                            worksheet.Cell(index, 3).Value = item.NumberOfWard;
                            worksheet.Cell(index, 4).Value = item.NumberOfQualifyingWard;
                            worksheet.Cell(index, 5).Value = item.NumberOfWardWithMarket;
                            worksheet.Cell(index, 6).Value = item.NumberOfWardCommercialInfrastructure;
                            worksheet.Cell(index, 7).Value = item.NumberOfWardNewCountryside;
                            worksheet.Cell(index, 8).Value = item.NumberOfWardCommercialInfrastructurePlan;
                            worksheet.Cell(index, 9).Value = item.NumberOfWardNewCountrysidePlan;
                            worksheet.Cell(index, 10).Value = item.NumberOfWardCommercialInfrastructureEstimate;
                            worksheet.Cell(index, 11).Value = item.NumberOfWardNewCountrysideEstimate;
                            index++;
                            row++;
                        }
                        else if (row < rowend)
                        {
                            var addrow = worksheet.Row(index - 1);
                            addrow.InsertRowsBelow(1);
                            worksheet.Cell(index, 1).Value = row;
                            worksheet.Cell(index, 2).Value = item.DistrictName;
                            worksheet.Cell(index, 3).Value = item.NumberOfWard;
                            worksheet.Cell(index, 4).Value = item.NumberOfQualifyingWard;
                            worksheet.Cell(index, 5).Value = item.NumberOfWardWithMarket;
                            worksheet.Cell(index, 6).Value = item.NumberOfWardCommercialInfrastructure;
                            worksheet.Cell(index, 7).Value = item.NumberOfWardNewCountryside;
                            worksheet.Cell(index, 8).Value = item.NumberOfWardCommercialInfrastructurePlan;
                            worksheet.Cell(index, 9).Value = item.NumberOfWardNewCountrysidePlan;
                            worksheet.Cell(index, 10).Value = item.NumberOfWardCommercialInfrastructureEstimate;
                            worksheet.Cell(index, 11).Value = item.NumberOfWardNewCountrysideEstimate;
                            index++;
                            row++;
                        }
                        else
                        {
                            worksheet.Cell(index, 1).Value = row;
                            worksheet.Cell(index, 2).Value = item.DistrictName;
                            worksheet.Cell(index, 3).Value = item.NumberOfWard;
                            worksheet.Cell(index, 4).Value = item.NumberOfQualifyingWard;
                            worksheet.Cell(index, 5).Value = item.NumberOfWardWithMarket;
                            worksheet.Cell(index, 6).Value = item.NumberOfWardCommercialInfrastructure;
                            worksheet.Cell(index, 7).Value = item.NumberOfWardNewCountryside;
                            worksheet.Cell(index, 8).Value = item.NumberOfWardCommercialInfrastructurePlan;
                            worksheet.Cell(index, 9).Value = item.NumberOfWardNewCountrysidePlan;
                            worksheet.Cell(index, 10).Value = item.NumberOfWardCommercialInfrastructureEstimate;
                            worksheet.Cell(index, 11).Value = item.NumberOfWardNewCountrysideEstimate;
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
    }
}
