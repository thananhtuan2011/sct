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
using ClosedXML.Excel;
using API_SoCongThuong.Logger;
using Newtonsoft.Json;
using System.Data;

namespace API_SoCongThuong.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CateManageAncolLocalBussinesController : ControllerBase
    {
        private CateManageAncolLocalBussinesRepo _repo;
        private IConfiguration _configuration;
        private readonly ILogger<AsyncLogger> _logger;
        public SoHoa_SoCongThuongContext _context;
        private AsyncLogger _asyncLogger;

        public CateManageAncolLocalBussinesController(SoHoa_SoCongThuongContext context, IConfiguration configuration, ILogger<AsyncLogger> logger)
        {
            _repo = new CateManageAncolLocalBussinesRepo(context);
            _logger = logger;
            _context = context;
            _asyncLogger = new AsyncLogger(_logger, _context);
            _configuration = configuration;
        }

        [Route("find")]
        [HttpPost]
        public IActionResult ListItems_New([FromBody] QueryRequestBody query)
        {
            BaseModels<CateManageAncolLocalBussinesModel> model = new BaseModels<CateManageAncolLocalBussinesModel>();
            string _keywordSearch = "";
            bool _orderBy_ASC = false;
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }

                Func<CateManageAncolLocalBussinesModel, object> _orderByExpression = x => x.CateManageAncolLocalBussinessId;
                Dictionary<string, Func<CateManageAncolLocalBussinesModel, object>> _sortableFields = new Dictionary<string, Func<CateManageAncolLocalBussinesModel, object>>
                    {
                        { "BusinessName", x => x.BusinessName },
                        { "BusinessCode", x => x.BusinessCode },
                        { "RepresentName", x => x.RepresentName },
                        { "Investment", x => x.Investment },
                        { "TypeOfProfessionName", x => x.TypeOfProfessionId },
                        { "DateChangeDisplay", x => x.DateChange },
                        { "DateReleaseDisplay", x => x.DateRelease },
                        { "NumberOfWorker", x => x.NumberOfWorker },
                        { "CreateName", x => x.CreateUserId },
                        { "CreateTimeDisplay", x => x.CreateTime },
                        { "IsActive", x => x.IsActive },
                    };

                if (query.Sort != null && !string.IsNullOrEmpty(query.Sort.ColumnName) && _sortableFields.ContainsKey(query.Sort.ColumnName))
                {
                    _orderBy_ASC = ("desc".Equals(query.Sort.Direction.ToLower()) ? false : true);
                    _orderByExpression = _sortableFields[query.Sort.ColumnName];
                }

                IQueryable<CateManageAncolLocalBussinesModel> _data = (_repo._context.CateManageAncolLocalBussines.Where(x => !x.IsDel).GroupJoin(
                    _repo._context.Users, cc => cc.CreateUserId, u => u.UserId,
                     (cc, u) => new { cc, u }).SelectMany(rs => rs.u.DefaultIfEmpty(), (info, use) => new { info, use }).GroupJoin(
                    _repo._context.TypeOfProfessions, query => query.info.cc.TypeOfProfessionId, pro => pro.TypeOfProfessionId,
                    (query, pro) => new { query, pro }).SelectMany(rs => rs.pro.DefaultIfEmpty(), (info1, profess) => new { info1, profess }).GroupJoin(
                    _repo._context.Businesses, query1 => query1.info1.query.info.cc.BusinessId, bus => bus.BusinessId,
                    (query1, bus) => new { query1, bus }).SelectMany(rs => rs.bus.DefaultIfEmpty(), (info2, busi) => new CateManageAncolLocalBussinesModel
                    {
                        CateManageAncolLocalBussinessId = info2.query1.info1.query.info.cc.CateManageAncolLocalBussinessId,
                        NumberOfWorker = info2.query1.info1.query.info.cc.NumberOfWorker,
                        TypeOfProfessionName = info2.query1.profess.TypeOfProfessionName,
                        Investment = info2.query1.info1.query.info.cc.Investment,
                        DateChangeDisplay =string.Format("{0:dd/MM/yyyy}", info2.query1.info1.query.info.cc.DateChange),
                        DateReleaseDisplay = string.Format("{0:dd/MM/yyyy}", info2.query1.info1.query.info.cc.DateRelease),
                        DateRelease = info2.query1.info1.query.info.cc.DateRelease,
                        BusinessName = busi.BusinessNameVi,
                        BusinessCode = busi.BusinessCode,
                        RepresentName = busi.NguoiDaiDien,
                        IsActive = info2.query1.info1.query.info.cc.IsActive,
                        ActionName = (bool)info2.query1.info1.query.info.cc.IsActive ? "Đang hoạt động" : "Tạm ngưng",
                        CreateName = info2.query1.info1.query.use.FullName,
                        CreateTimeDisplay = info2.query1.info1.query.info.cc.CreateTime.ToString("dd/MM/yyyy hh:mm")
                    })).ToList().AsQueryable();

                if (query.SearchValue != null && query.SearchValue != "")
                {
                    _keywordSearch = query.SearchValue.Trim().ToLower();
                    _data = _data.Where(x => 
                    x.BusinessName.ToLower().Contains(_keywordSearch)
                    || x.BusinessCode.ToLower().Contains(_keywordSearch)
                    || x.RepresentName.ToLower().Contains(_keywordSearch)
                    || x.Investment.ToString().Contains(_keywordSearch)
                    || x.TypeOfProfessionName.ToLower().Contains(_keywordSearch)
                    || x.NumberOfWorker.ToString().Contains(_keywordSearch)
                    || x.ActionName.ToLower().Contains(_keywordSearch)
                    );
                }

                if (query.Filter != null && query.Filter.ContainsKey("YearReport"))
                {
                    _data = _data.Where(x => x.DateRelease.Year.ToString().Equals(string.Join("", query.Filter["YearReport"])));
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
        public async Task<IActionResult> Create(CateManageAncolLocalBussinesModel data)
        {
            BaseModels<CateManageAncolLocalBussinesModel> model = new BaseModels<CateManageAncolLocalBussinesModel>();
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
                if (string.IsNullOrEmpty(data.BusinessId.ToString()))
                {
                    model.status = 0;
                    model.error = new ErrorModel()
                    {
                        Code = ErrCode_Const.PROPERTY_IS_NULL_OR_EMPTY
                    };
                    datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.CREATE, NameCategory_Const.CATE_MANAGE_ANCOL_LOCAL_BUSINESS, Action_Status.FAIL);
                    _asyncLogger.LogInfo(JsonConvert.SerializeObject(datalog));
                    return BadRequest(model);
                }
                if (data.DateRelease==DateTime.MinValue)
                {
                    model.status = 0;
                    model.error = new ErrorModel()
                    {
                        Code = ErrCode_Const.PROPERTY_IS_NULL_OR_EMPTY
                    };
                    datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.CREATE, NameCategory_Const.CATE_MANAGE_ANCOL_LOCAL_BUSINESS, Action_Status.FAIL);
                    _asyncLogger.LogInfo(JsonConvert.SerializeObject(datalog));
                    return BadRequest(model);
                }

                data.CreateTime = DateTime.Now;
                data.CreateUserId = loginData.Userid;
                await _repo.Insert(data);
                datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.CREATE, NameCategory_Const.CATE_MANAGE_ANCOL_LOCAL_BUSINESS, Action_Status.SUCCESS);
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
        public async Task<IActionResult> Update(CateManageAncolLocalBussinesModel data)
        {
            BaseModels<CateManageAncolLocalBussinesModel> model = new BaseModels<CateManageAncolLocalBussinesModel>();
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
                var CheckData = _repo.FindById(data.CateManageAncolLocalBussinessId);
                if (CheckData == null)
                {
                    model.status = 0;
                    model.error = new ErrorModel()
                    {
                        Code = ErrCode_Const.PROPERTY_IS_NULL_OR_EMPTY
                    };
                    datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.UPDATE, NameCategory_Const.CATE_MANAGE_ANCOL_LOCAL_BUSINESS, Action_Status.FAIL);
                    _asyncLogger.LogInfo(JsonConvert.SerializeObject(datalog));
                    return BadRequest(model);
                }
                else
                {
                    if (string.IsNullOrEmpty(data.BusinessId.ToString()))
                    {
                        model.status = 0;
                        model.error = new ErrorModel()
                        {
                            Code = ErrCode_Const.PROPERTY_IS_NULL_OR_EMPTY
                        };
                        datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.UPDATE, NameCategory_Const.CATE_MANAGE_ANCOL_LOCAL_BUSINESS, Action_Status.FAIL);
                        _asyncLogger.LogInfo(JsonConvert.SerializeObject(datalog));
                        return BadRequest(model);
                    }
                    if (data.DateRelease == DateTime.MinValue)
                    {
                        model.status = 0;
                        model.error = new ErrorModel()
                        {
                            Code = ErrCode_Const.PROPERTY_IS_NULL_OR_EMPTY
                        };
                        datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.UPDATE, NameCategory_Const.CATE_MANAGE_ANCOL_LOCAL_BUSINESS, Action_Status.FAIL);
                        _asyncLogger.LogInfo(JsonConvert.SerializeObject(datalog));
                        return BadRequest(model);
                    }

                    data.UpdateTime = DateTime.Now;
                    data.UpdateUserId = loginData.Userid;
                    datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.UPDATE, NameCategory_Const.CATE_MANAGE_ANCOL_LOCAL_BUSINESS, Action_Status.SUCCESS);
                    _asyncLogger.LogInfo(JsonConvert.SerializeObject(datalog));
                    await _repo.Update(data);
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
            BaseModels<CateManageAncolLocalBussinesModel> model = new BaseModels<CateManageAncolLocalBussinesModel>();
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

        [Route("deletes")]
        [HttpPut()]
        public async Task<IActionResult> deletes(List<Guid> IdRemoves)
        {
            BaseModels<object> model = new BaseModels<object>();
            try
            {
                await _repo.Deletes(IdRemoves);
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
                datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.DELETE, NameCategory_Const.CATE_MANAGE_ANCOL_LOCAL_BUSINESS, Action_Status.SUCCESS);
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

        [HttpGet("GetListBusiness")]
        public IActionResult GetListBusiness()
        {
            BaseModels<List<Business>> model = new BaseModels<List<Business>>();
            try
            {
                var data=_repo.GetListBusiness();
                model.data = data;
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

        [HttpGet("GetListTypeOfProfession")]
        public IActionResult GetListTypeOfProfession()
        {
            BaseModels<List<TypeOfProfession>> model = new BaseModels<List<TypeOfProfession>>();
            try
            {
                var data = _repo.GetListTypeOfProfession();
                model.data = data;
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
            var data = _repo.FindData(query).GroupBy(x => x.DateRelease.Month).OrderBy(x => x.Key).ToList();

            if (!data.Any())
            {
                return BadRequest();
            }

            int currentYear = DateTime.Now.Year;

            if (query.Filter != null && query.Filter.ContainsKey("YearReport") && !string.IsNullOrEmpty(query.Filter["YearReport"]))
            {
                currentYear = int.Parse(query.Filter["YearReport"]);
            };

            try
            {
                using (var workbook = new XLWorkbook(@"Upload/Templates/Danhsachdoanhnghiepsanxuatcongnghiep.xlsx"))
                {
                    IXLWorksheet worksheet = workbook.Worksheets.Worksheet(1);
                    worksheet.Cell(1, 1).Value = $"DANH SÁCH DOANH NGHIỆP SẢN XUẤT CÔNG NGHIỆP TỈNH BẾN TRE NĂM {currentYear}";

                    int index = 3;

                    foreach (var group in data)
                    {
                        int row = 1;
                        worksheet.Row(index).InsertRowsBelow(1);
                        worksheet.Cell(index, 1).Value = $"Tháng {group.Key}";
                        worksheet.Cell(index, 1).Style.Font.SetBold(true);
                        worksheet.Range(index, 1, index, 25).Merge();
                        index++;

                        foreach (var item in group)
                        {
                            worksheet.Cell(index, 1).Value = row;
                            worksheet.Cell(index, 2).Value = item.BusinessCode.ToUpper();
                            worksheet.Cell(index, 3).Value = item.BusinessName.ToUpper();
                            worksheet.Cell(index, 4).Value = item.Address;
                            worksheet.Cell(index, 5).Value = item.District;
                            worksheet.Cell(index, 6).Value = item.Commune;
                            worksheet.Cell(index, 7).Value = item.Investment.ToString();
                            worksheet.Cell(index, 8).Value = item.ActionName;
                            worksheet.Cell(index, 9).Value = item.PhoneNumber;
                            worksheet.Cell(index, 10).Value = item.Email;
                            worksheet.Cell(index, 11).Value = item.RepresentName;
                            worksheet.Cell(index, 12).Value = item.RepresentBirthDay;
                            worksheet.Cell(index, 13).Value = item.CitizenId;
                            worksheet.Cell(index, 14).Value = item.CitizenIdDate;
                            worksheet.Cell(index, 15).Value = item.IssuerCitizenId;
                            worksheet.Cell(index, 16).Value = item.Owner;
                            worksheet.Cell(index, 17).Value = item.TypeOfProfessionName;
                            worksheet.Cell(index, 18).Value = item.LstProfession;
                            worksheet.Cell(index, 19).Value = item.DateReleaseDisplay;
                            worksheet.Cell(index, 20).Value = item.DateChangeDisplay;
                            worksheet.Cell(index, 21).Value = item.TypeOfBusinessName;
                            worksheet.Cell(index, 22).Value = item.NumberOfWorker;
                            worksheet.Cell(index, 23).Value = item.LstCapitalContributing;
                            worksheet.Cell(index, 24).Value = item.LstShareholder;
                            worksheet.Cell(index, 25).Value = "TN";
                            worksheet.Row(index).InsertRowsBelow(1);
                            index++;
                            row++;
                        }
                    }

                    worksheet.Row(index).Delete();

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
